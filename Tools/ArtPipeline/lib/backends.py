#!/usr/bin/env python3
"""Stable Diffusion backend abstractions for the art pipeline."""

from __future__ import annotations

from dataclasses import dataclass
import json
import os
import random
import time
from typing import Any, Protocol
from urllib.error import HTTPError, URLError
from urllib.parse import urlencode
from urllib.request import Request, urlopen


HEALTH_TIMEOUT_SECONDS = 10.0
GENERATION_TIMEOUT_SECONDS = 120.0
TRANSIENT_RETRY_COUNT = 2
DEFAULT_COMFYUI_MODEL = "v1-5-pruned-emaonly.safetensors"
SAMPLER_NAME_MAP = {
    "euler": "euler",
    "euler_a": "euler_ancestral",
    "euler ancestral": "euler_ancestral",
    "dpmpp_2m": "dpmpp_2m",
    "dpm++ 2m": "dpmpp_2m",
    "heun": "heun",
}


class GenerationError(RuntimeError):
    """Raised when image generation fails."""


@dataclass
class GenerationRequest:
    positive_prompt: str
    negative_prompt: str
    width: int
    height: int
    steps: int
    cfg_scale: float
    sampler: str
    seed: int
    batch_count: int


@dataclass
class GeneratedImage:
    image_data: bytes
    seed: int
    generation_time_seconds: float


class GenerationBackend(Protocol):
    def health_check(self) -> bool: ...

    def generate(self, request: GenerationRequest) -> list[GeneratedImage]: ...


class ComfyUiBackend:
    """ComfyUI backend using prompt submission, history polling, and image fetches."""

    def __init__(self, base_url: str) -> None:
        self.base_url = base_url.rstrip("/")
        self._checkpoint_name = (
            os.environ.get("ART_PIPELINE_SD_MODEL")
            or os.environ.get("COMFYUI_CHECKPOINT")
            or DEFAULT_COMFYUI_MODEL
        ).strip()

    def health_check(self) -> bool:
        try:
            response = _http_get_json(f"{self.base_url}/system_stats", timeout=HEALTH_TIMEOUT_SECONDS)
            return isinstance(response, dict)
        except (GenerationError, HTTPError, URLError, TimeoutError, OSError):
            return False

    def generate(self, request: GenerationRequest) -> list[GeneratedImage]:
        if request.batch_count <= 0:
            raise GenerationError("batch_count must be greater than 0")

        deadline = time.monotonic() + GENERATION_TIMEOUT_SECONDS
        results: list[GeneratedImage] = []

        for _ in range(request.batch_count):
            actual_seed = request.seed if request.seed >= 0 else _random_seed()
            results.append(self._generate_one(request, actual_seed=actual_seed, deadline=deadline))

        return results

    def _generate_one(self, request: GenerationRequest, *, actual_seed: int, deadline: float) -> GeneratedImage:
        last_error: Exception | None = None

        for attempt in range(TRANSIENT_RETRY_COUNT + 1):
            try:
                started = time.monotonic()
                prompt_id = self._submit_prompt(request, actual_seed=actual_seed, deadline=deadline)
                history = self._wait_for_history(prompt_id, deadline=deadline)
                image_ref = _extract_first_image_ref(history)
                image_data = self._fetch_image(image_ref, deadline=deadline)
                return GeneratedImage(
                    image_data=image_data,
                    seed=actual_seed,
                    generation_time_seconds=round(time.monotonic() - started, 3),
                )
            except (URLError, TimeoutError, OSError) as error:
                last_error = error
                if attempt >= TRANSIENT_RETRY_COUNT:
                    break
            except HTTPError as error:
                raise GenerationError(f"ComfyUI returned HTTP {error.code}: {error.reason}") from error

        raise GenerationError(f"ComfyUI generation failed after retries: {last_error}") from last_error

    def _submit_prompt(self, request: GenerationRequest, *, actual_seed: int, deadline: float) -> str:
        workflow = _build_comfyui_workflow(
            checkpoint_name=self._checkpoint_name,
            positive_prompt=request.positive_prompt,
            negative_prompt=request.negative_prompt,
            width=request.width,
            height=request.height,
            steps=request.steps,
            cfg_scale=request.cfg_scale,
            sampler=request.sampler,
            seed=actual_seed,
        )

        payload = _http_post_json(
            f"{self.base_url}/prompt",
            body={"prompt": workflow, "client_id": "art-pipeline"},
            timeout=_remaining_timeout(deadline),
        )
        prompt_id = str(payload.get("prompt_id") or "").strip()
        if not prompt_id:
            raise GenerationError("ComfyUI did not return a prompt_id")
        return prompt_id

    def _wait_for_history(self, prompt_id: str, *, deadline: float) -> dict[str, Any]:
        while True:
            if time.monotonic() >= deadline:
                raise GenerationError(f"Timed out waiting for ComfyUI prompt {prompt_id}")

            payload = _http_get_json(
                f"{self.base_url}/history/{prompt_id}",
                timeout=min(5.0, _remaining_timeout(deadline)),
            )

            if isinstance(payload, dict):
                history = payload.get(prompt_id)
                if isinstance(history, dict) and _history_has_images(history):
                    return history

            time.sleep(1.0)

    def _fetch_image(self, image_ref: dict[str, str], *, deadline: float) -> bytes:
        query = urlencode(image_ref)
        image_data = _http_get_bytes(
            f"{self.base_url}/view?{query}",
            timeout=_remaining_timeout(deadline),
        )
        if not image_data:
            raise GenerationError("ComfyUI returned an empty image payload")
        return image_data


def create_backend(backend_type: str, base_url: str) -> GenerationBackend:
    normalized = str(backend_type or "").strip().lower()
    if normalized in {"comfyui", "comfy", "comfy-ui"}:
        return ComfyUiBackend(base_url)

    raise ValueError(
        f"Unsupported SD backend: {backend_type!r}. Supported backends: comfyui. "
        "Future optimization: add Automatic1111 or cloud backends behind the same protocol."
    )


def _build_comfyui_workflow(
    *,
    checkpoint_name: str,
    positive_prompt: str,
    negative_prompt: str,
    width: int,
    height: int,
    steps: int,
    cfg_scale: float,
    sampler: str,
    seed: int,
) -> dict[str, Any]:
    sampler_name = SAMPLER_NAME_MAP.get(sampler.strip().lower(), sampler.strip().lower() or "euler")

    return {
        "3": {
            "class_type": "KSampler",
            "inputs": {
                "seed": seed,
                "steps": steps,
                "cfg": cfg_scale,
                "sampler_name": sampler_name,
                "scheduler": "normal",
                "denoise": 1.0,
                "model": ["4", 0],
                "positive": ["6", 0],
                "negative": ["7", 0],
                "latent_image": ["5", 0],
            },
        },
        "4": {
            "class_type": "CheckpointLoaderSimple",
            "inputs": {
                "ckpt_name": checkpoint_name,
            },
        },
        "5": {
            "class_type": "EmptyLatentImage",
            "inputs": {
                "width": width,
                "height": height,
                "batch_size": 1,
            },
        },
        "6": {
            "class_type": "CLIPTextEncode",
            "inputs": {
                "text": positive_prompt,
                "clip": ["4", 1],
            },
        },
        "7": {
            "class_type": "CLIPTextEncode",
            "inputs": {
                "text": negative_prompt,
                "clip": ["4", 1],
            },
        },
        "8": {
            "class_type": "VAEDecode",
            "inputs": {
                "samples": ["3", 0],
                "vae": ["4", 2],
            },
        },
        "9": {
            "class_type": "SaveImage",
            "inputs": {
                "filename_prefix": "artpipeline",
                "images": ["8", 0],
            },
        },
    }


def _http_get_json(url: str, *, timeout: float) -> dict[str, Any]:
    payload = _http_get_bytes(url, timeout=timeout)
    try:
        parsed = json.loads(payload.decode("utf-8"))
    except json.JSONDecodeError as error:
        raise GenerationError(f"Invalid JSON response from {url}: {error}") from error
    if not isinstance(parsed, dict):
        raise GenerationError(f"Expected JSON object from {url}")
    return parsed


def _http_post_json(url: str, *, body: dict[str, Any], timeout: float) -> dict[str, Any]:
    request = Request(
        url,
        data=json.dumps(body).encode("utf-8"),
        headers={"Content-Type": "application/json"},
        method="POST",
    )
    payload = _http_read(request, timeout=timeout)
    try:
        parsed = json.loads(payload.decode("utf-8"))
    except json.JSONDecodeError as error:
        raise GenerationError(f"Invalid JSON response from {url}: {error}") from error
    if not isinstance(parsed, dict):
        raise GenerationError(f"Expected JSON object from {url}")
    return parsed


def _http_get_bytes(url: str, *, timeout: float) -> bytes:
    request = Request(url, method="GET")
    return _http_read(request, timeout=timeout)


def _http_read(request: Request, *, timeout: float) -> bytes:
    with urlopen(request, timeout=timeout) as response:
        payload = response.read()
    return payload


def _history_has_images(history: dict[str, Any]) -> bool:
    outputs = history.get("outputs")
    if not isinstance(outputs, dict):
        return False

    for node_output in outputs.values():
        if not isinstance(node_output, dict):
            continue
        images = node_output.get("images")
        if isinstance(images, list) and images:
            return True

    return False


def _extract_first_image_ref(history: dict[str, Any]) -> dict[str, str]:
    outputs = history.get("outputs")
    if not isinstance(outputs, dict):
        raise GenerationError("ComfyUI history did not include outputs")

    for node_output in outputs.values():
        if not isinstance(node_output, dict):
            continue
        images = node_output.get("images")
        if not isinstance(images, list):
            continue
        for image in images:
            if not isinstance(image, dict):
                continue
            filename = str(image.get("filename") or "").strip()
            if not filename:
                continue
            return {
                "filename": filename,
                "subfolder": str(image.get("subfolder") or ""),
                "type": str(image.get("type") or "output"),
            }

    raise GenerationError("ComfyUI history did not include generated image references")


def _remaining_timeout(deadline: float) -> float:
    remaining = deadline - time.monotonic()
    if remaining <= 0:
        raise GenerationError("Generation request exceeded the 120s timeout budget")
    return remaining


def _random_seed() -> int:
    return random.SystemRandom().randrange(0, 2**31 - 1)
