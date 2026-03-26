#!/usr/bin/env python3
"""Preflight environment checks for the content pipeline."""

from __future__ import annotations

from pathlib import Path
import importlib
import json
import os
import re
import subprocess
import sys
from typing import Any
from urllib import error, parse, request

from config import Settings, load_settings


REQUIREMENTS_PATH = Path(__file__).resolve().parent / "requirements.txt"
GREEN = "\033[32m"
RED = "\033[31m"
RESET = "\033[0m"
TOTAL_CHECKS = 9


def main() -> int:
    use_color = sys.stdout.isatty()
    passed = 0
    settings: Settings | None = None
    settings_error: str | None = None
    ollama_tags_payload: dict[str, Any] | None = None

    try:
        settings = load_settings()
    except Exception as exc:  # pragma: no cover - defensive path for local setup issues
        settings_error = f"Failed to load config: {exc}"

    ok, message = check_python_version()
    passed += report(ok, message, use_color)

    ok, message = check_python_packages(REQUIREMENTS_PATH)
    passed += report(ok, message, use_color)

    ok, message = check_ffmpeg(settings, settings_error)
    passed += report(ok, message, use_color)

    ok, message = check_ffprobe(settings, settings_error)
    passed += report(ok, message, use_color)

    ok, message = check_nvidia_gpu()
    passed += report(ok, message, use_color)

    ok, message, ollama_tags_payload = check_ollama_reachable(settings, settings_error)
    passed += report(ok, message, use_color)

    ok, message = check_ollama_model(settings, settings_error, ollama_tags_payload)
    passed += report(ok, message, use_color)

    ok, message = check_recordings_dir(settings, settings_error)
    passed += report(ok, message, use_color)

    ok, message = check_data_dirs(settings, settings_error)
    passed += report(ok, message, use_color)

    if passed == TOTAL_CHECKS:
        print(f"{passed}/{TOTAL_CHECKS} checks passed — ready to run pipeline")
        return 0

    print(f"{passed}/{TOTAL_CHECKS} checks passed — fix failures above before running")
    return 1


def report(ok: bool, message: str, use_color: bool) -> int:
    label = "PASS" if ok else "FAIL"
    prefix = f"[{label}]"
    if use_color:
        color = GREEN if ok else RED
        prefix = f"{color}{prefix}{RESET}"
    print(f"{prefix} {message}")
    return 1 if ok else 0


def check_python_version() -> tuple[bool, str]:
    version = sys.version_info
    if version >= (3, 10):
        return True, f"Python {version.major}.{version.minor}.{version.micro} detected"
    return False, f"Python {version.major}.{version.minor}.{version.micro} detected; Python 3.10+ required"


def check_python_packages(requirements_path: Path) -> tuple[bool, str]:
    if not requirements_path.exists():
        return False, f"requirements.txt not found at {requirements_path}"

    missing: list[str] = []
    checked: list[str] = []

    for requirement in requirements_path.read_text(encoding="utf-8").splitlines():
        requirement = requirement.strip()
        if not requirement or requirement.startswith("#"):
            continue

        package_name = parse_requirement_name(requirement)
        import_name = package_name.replace("-", "_")
        checked.append(package_name)

        try:
            importlib.import_module(import_name)
        except Exception:
            missing.append(package_name)

    if missing:
        return False, f"Missing Python packages: {', '.join(missing)}"

    return True, f"Python packages importable: {', '.join(checked)}"


def parse_requirement_name(requirement: str) -> str:
    name = re.split(r"[<>=!~;\[]", requirement, maxsplit=1)[0].strip()
    return name


def check_ffmpeg(settings: Settings | None, settings_error: str | None) -> tuple[bool, str]:
    if settings_error:
        return False, settings_error
    if settings is None or settings.ffmpeg_path is None:
        return False, "FFmpeg not found on PATH or in CONTENT_PIPELINE_FFMPEG_PATH"

    ffmpeg_path = settings.ffmpeg_path.resolve()
    ok, detail = ffmpeg_supports_nvenc(ffmpeg_path)
    if not ok:
        return False, f"FFmpeg found at {ffmpeg_path}, but {detail}"

    return True, f"FFmpeg found at {ffmpeg_path} (h264_nvenc available)"


def ffmpeg_supports_nvenc(ffmpeg_path: Path) -> tuple[bool, str]:
    try:
        completed = subprocess.run(
            [str(ffmpeg_path), "-hide_banner", "-encoders"],
            capture_output=True,
            text=True,
            check=False,
        )
    except OSError as exc:
        return False, f"could not run ffmpeg: {exc}"

    if completed.returncode != 0:
        stderr = completed.stderr.strip() or completed.stdout.strip() or "unknown ffmpeg error"
        return False, f"ffmpeg -encoders failed: {stderr}"

    encoder_output = f"{completed.stdout}\n{completed.stderr}"
    if "h264_nvenc" not in encoder_output:
        return False, "h264_nvenc encoder is not available"

    return True, "h264_nvenc encoder is available"


def check_ffprobe(settings: Settings | None, settings_error: str | None) -> tuple[bool, str]:
    if settings_error:
        return False, settings_error
    if settings is None or settings.ffprobe_path is None:
        return False, "ffprobe not found on PATH or in CONTENT_PIPELINE_FFPROBE_PATH"

    return True, f"ffprobe found at {settings.ffprobe_path.resolve()}"


def check_nvidia_gpu() -> tuple[bool, str]:
    try:
        completed = subprocess.run(
            ["nvidia-smi"],
            capture_output=True,
            text=True,
            check=False,
        )
    except OSError as exc:
        return False, f"nvidia-smi not available: {exc}"

    if completed.returncode == 0:
        return True, "NVIDIA GPU detected via nvidia-smi"

    stderr = completed.stderr.strip() or completed.stdout.strip() or f"exit code {completed.returncode}"
    return False, f"nvidia-smi failed: {stderr}"


def check_ollama_reachable(
    settings: Settings | None,
    settings_error: str | None,
) -> tuple[bool, str, dict[str, Any] | None]:
    if settings_error:
        return False, settings_error, None
    if settings is None:
        return False, "Config not loaded", None

    tags_url = build_ollama_tags_url(settings.ollama_url)
    try:
        response, payload = get_json(tags_url)
    except RuntimeError as exc:
        return False, f"Ollama not reachable at {tags_url}: {exc}", None

    if response != 200:
        return False, f"Ollama returned HTTP {response} from {tags_url}", payload

    return True, f"Ollama reachable at {tags_url}", payload


def check_ollama_model(
    settings: Settings | None,
    settings_error: str | None,
    tags_payload: dict[str, Any] | None,
) -> tuple[bool, str]:
    if settings_error:
        return False, settings_error
    if settings is None:
        return False, "Config not loaded"
    if tags_payload is None:
        return False, f"Could not verify Ollama model {settings.ollama_model}; /api/tags did not return data"

    models = tags_payload.get("models")
    if not isinstance(models, list):
        return False, "Ollama /api/tags response is missing a models list"

    available_names: set[str] = set()
    for model in models:
        if not isinstance(model, dict):
            continue
        for key in ("name", "model"):
            value = model.get(key)
            if isinstance(value, str) and value.strip():
                available_names.add(value.strip())

    if settings.ollama_model in available_names:
        return True, f"Ollama model available: {settings.ollama_model}"

    if available_names:
        available = ", ".join(sorted(available_names))
        return False, f"Ollama model missing: {settings.ollama_model} (available: {available})"

    return False, f"Ollama model missing: {settings.ollama_model}"


def build_ollama_tags_url(ollama_url: str) -> str:
    parsed = parse.urlsplit(ollama_url.strip())
    if not parsed.scheme or not parsed.netloc:
        return "/api/tags"
    return parse.urlunsplit((parsed.scheme, parsed.netloc, "/api/tags", "", ""))


def get_json(url: str, timeout_seconds: float = 5.0) -> tuple[int, dict[str, Any]]:
    req = request.Request(url, method="GET")
    try:
        with request.urlopen(req, timeout=timeout_seconds) as response:
            status = getattr(response, "status", response.getcode())
            body = response.read().decode("utf-8")
    except error.HTTPError as exc:
        body = exc.read().decode("utf-8", errors="replace")
        payload = parse_json_object(body)
        return exc.code, payload
    except error.URLError as exc:
        raise RuntimeError(str(exc.reason)) from exc
    except OSError as exc:
        raise RuntimeError(str(exc)) from exc

    payload = parse_json_object(body)
    return status, payload


def parse_json_object(body: str) -> dict[str, Any]:
    try:
        payload = json.loads(body)
    except json.JSONDecodeError as exc:
        raise RuntimeError(f"invalid JSON response: {exc}") from exc

    if not isinstance(payload, dict):
        raise RuntimeError("JSON response was not an object")

    return payload


def check_recordings_dir(settings: Settings | None, settings_error: str | None) -> tuple[bool, str]:
    if settings_error:
        return False, settings_error
    if settings is None:
        return False, "Config not loaded"

    recordings_dir = settings.recordings_dir.resolve()
    if not recordings_dir.exists():
        return False, f"Recordings directory not found: {recordings_dir}"
    if not recordings_dir.is_dir():
        return False, f"Recordings path is not a directory: {recordings_dir}"
    if not os.access(recordings_dir, os.R_OK | os.X_OK):
        return False, f"Recordings directory is not readable: {recordings_dir}"

    return True, f"Recordings directory readable: {recordings_dir}"


def check_data_dirs(settings: Settings | None, settings_error: str | None) -> tuple[bool, str]:
    if settings_error:
        return False, settings_error
    if settings is None:
        return False, "Config not loaded"

    settings.ensure_data_dirs()
    expected = {
        "data": settings.data_dir,
        "transcripts": settings.transcripts_dir,
        "highlights": settings.highlights_dir,
        "clips": settings.clips_dir,
        "review_queue": settings.review_queue_dir,
        "published": settings.published_dir,
        "feedback": settings.feedback_dir,
    }

    missing = [name for name, path in expected.items() if not path.exists() or not path.is_dir()]
    if missing:
        return False, f"Missing data directories after ensure_data_dirs(): {', '.join(missing)}"

    not_writable = [name for name, path in expected.items() if not os.access(path, os.W_OK | os.X_OK)]
    if not_writable:
        return False, f"Data directories not writable: {', '.join(not_writable)}"

    return True, f"Data directories ready under {settings.data_dir.resolve()}"


if __name__ == "__main__":
    raise SystemExit(main())
