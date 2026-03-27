#!/usr/bin/env python3
"""Generate candidate images from art prompt manifests."""

from __future__ import annotations

import argparse
from pathlib import Path
import sys
import time
from typing import Any, Sequence

from config import load_settings
from lib.backends import GenerationError, GenerationRequest, create_backend
from lib.manifest_io import SCHEMA_VERSION, read_manifest, timestamp_utc, write_manifest


def build_parser() -> argparse.ArgumentParser:
    """Build the CLI parser."""
    parser = argparse.ArgumentParser(description="Generate candidate images from prompt manifests")
    parser.add_argument("batch_id", help="Batch id of the source prompt manifest")
    parser.add_argument("--spec", help="Optional spec_id filter")
    parser.add_argument("--dry-run", action="store_true", help="Show planned requests without generating images")
    parser.add_argument("--backend", help="Override SD backend type from config")
    parser.add_argument("--url", help="Override SD backend URL from config")
    return parser


def parse_args() -> argparse.Namespace:
    """Parse command-line arguments."""
    return build_parser().parse_args()


def parse_args_from(argv: Sequence[str]) -> argparse.Namespace:
    """Parse arguments from an explicit argv list for tests."""
    return build_parser().parse_args(list(argv))


def load_prompt_manifest(batch_id: str) -> tuple[Any, dict[str, Any]]:
    """Load the prompt manifest for one batch."""
    settings = load_settings()
    manifest_path = settings.prompts_dir / f"{batch_id}.json"
    manifest = read_manifest(manifest_path)
    return settings, manifest


def select_prompts(manifest: dict[str, Any], spec_id: str | None) -> list[dict[str, Any]]:
    """Return the prompt entries to generate."""
    prompts = manifest.get("prompts")
    if not isinstance(prompts, list) or not prompts:
        raise ValueError("Prompt manifest must contain a non-empty prompts array")

    filtered: list[dict[str, Any]] = []
    for prompt in prompts:
        if not isinstance(prompt, dict):
            raise ValueError("Each prompt entry must be a JSON object")
        if spec_id and str(prompt.get("spec_id")) != spec_id:
            continue
        filtered.append(prompt)

    if spec_id and not filtered:
        raise ValueError(f"Spec {spec_id!r} was not found in the prompt manifest")

    return filtered


def build_request(prompt: dict[str, Any]) -> GenerationRequest:
    """Convert one prompt manifest entry into a backend request."""
    params = prompt.get("generation_params")
    if not isinstance(params, dict):
        raise ValueError(f"Prompt {prompt.get('spec_id')!r} is missing generation_params")

    return GenerationRequest(
        positive_prompt=str(prompt.get("positive_prompt") or "").strip(),
        negative_prompt=str(prompt.get("negative_prompt") or "").strip(),
        width=int(params.get("width", 0)),
        height=int(params.get("height", 0)),
        steps=int(params.get("steps", 0)),
        cfg_scale=float(params.get("cfg_scale", 0.0)),
        sampler=str(params.get("sampler") or "").strip(),
        seed=int(params.get("seed", -1)),
        batch_count=int(params.get("batch_count", 1)),
    )


def ensure_output_paths(settings: Any, batch_id: str, prompts: list[dict[str, Any]]) -> Path:
    """Validate output paths before generation starts."""
    settings.ensure_data_dirs()

    manifest_path = settings.candidates_dir / f"{batch_id}.json"
    if manifest_path.exists():
        raise FileExistsError(
            f"Candidate manifest already exists: {manifest_path}. Future optimization: add --force or resume support."
        )

    for prompt in prompts:
        spec_output_dir = settings.candidates_dir / batch_id / str(prompt.get("spec_id"))
        if spec_output_dir.exists():
            raise FileExistsError(
                f"Candidate output directory already exists: {spec_output_dir}. "
                "Future optimization: add --force or resume support."
            )

    return manifest_path


def build_candidate_manifest(
    *,
    batch_id: str,
    source_prompts_path: Path,
    backend_name: str,
    backend_url: str,
    candidates: list[dict[str, Any]],
    errors: list[dict[str, Any]],
    settings: Any,
) -> dict[str, Any]:
    """Build the output candidate manifest payload."""
    manifest: dict[str, Any] = {
        "schema_version": SCHEMA_VERSION,
        "batch_id": batch_id,
        "created_at_utc": timestamp_utc(),
        "source_prompts": source_prompts_path.relative_to(settings.tool_root).as_posix(),
        "backend": backend_name,
        "backend_url": backend_url,
        "candidates": candidates,
    }
    if errors:
        manifest["errors"] = errors
    return manifest


def run_generation(args: argparse.Namespace) -> int:
    """Execute the generation flow."""
    try:
        settings, prompt_manifest = load_prompt_manifest(args.batch_id)
        selected_prompts = select_prompts(prompt_manifest, args.spec)
    except Exception as error:
        print(f"[generate] Configuration error: {error}", file=sys.stderr)
        return 1

    batch_id = str(prompt_manifest.get("batch_id") or args.batch_id)
    prompt_manifest_path = settings.prompts_dir / f"{batch_id}.json"
    backend_name = (args.backend or settings.sd_backend).strip()
    backend_url = (args.url or settings.sd_url).strip()

    try:
        backend = create_backend(backend_name, backend_url)
    except Exception as error:
        print(f"[generate] Backend error: {error}", file=sys.stderr)
        return 1

    health_ok = backend.health_check()
    print(f"[generate] Health check: {'ok' if health_ok else 'unavailable'} ({backend_name} @ {backend_url})")

    if args.dry_run:
        print(f"[generate] Dry run only. planned_specs={len(selected_prompts)}")
        for index, prompt in enumerate(selected_prompts, start=1):
            request = build_request(prompt)
            print(
                f"[{index}/{len(selected_prompts)}] {prompt.get('spec_id')} -> "
                f"{request.batch_count} candidate(s), {request.width}x{request.height}, "
                f"steps={request.steps}, cfg={request.cfg_scale}, sampler={request.sampler}, seed={request.seed}"
            )
        return 0

    if not health_ok:
        print(
            f"[generate] Backend unavailable. Start {backend_name} at {backend_url} and retry.",
            file=sys.stderr,
        )
        return 1

    try:
        manifest_output_path = ensure_output_paths(settings, batch_id, selected_prompts)
    except Exception as error:
        print(f"[generate] Output error: {error}", file=sys.stderr)
        return 1

    started = time.monotonic()
    total_generated = 0
    candidate_entries: list[dict[str, Any]] = []
    error_entries: list[dict[str, Any]] = []

    for index, prompt in enumerate(selected_prompts, start=1):
        spec_id = str(prompt.get("spec_id") or "").strip()
        character = str(prompt.get("character") or "").strip()
        request = build_request(prompt)
        spec_output_dir = settings.candidates_dir / batch_id / spec_id

        print(f"[{index}/{len(selected_prompts)}] Generating {spec_id} ({request.batch_count} candidates)...")

        try:
            generated_images = backend.generate(request)
        except GenerationError as error:
            print(f"  - failed: {error}", file=sys.stderr)
            candidate_entries.append(
                {
                    "spec_id": spec_id,
                    "character": character,
                    "images": [],
                    "status": "generation_failed",
                    "error": str(error),
                }
            )
            error_entries.append({"spec_id": spec_id, "error": str(error)})
            continue

        spec_output_dir.mkdir(parents=True, exist_ok=False)
        image_entries: list[dict[str, Any]] = []

        for image_index, generated in enumerate(generated_images, start=1):
            filename = f"{spec_id}_{image_index:03d}.png"
            image_path = spec_output_dir / filename
            image_path.write_bytes(generated.image_data)
            image_entries.append(
                {
                    "filename": filename,
                    "path": image_path.relative_to(settings.tool_root).as_posix(),
                    "seed": generated.seed,
                    "generation_time_seconds": generated.generation_time_seconds,
                    "status": "pending_review",
                }
            )
            total_generated += 1
            print(f"  - candidate {image_index:03d}: seed={generated.seed}, {generated.generation_time_seconds:.1f}s")

        candidate_entries.append(
            {
                "spec_id": spec_id,
                "character": character,
                "images": image_entries,
            }
        )

    manifest = build_candidate_manifest(
        batch_id=batch_id,
        source_prompts_path=prompt_manifest_path,
        backend_name=backend_name,
        backend_url=backend_url,
        candidates=candidate_entries,
        errors=error_entries,
        settings=settings,
    )
    write_manifest(manifest_output_path, manifest)

    elapsed = time.monotonic() - started
    print(f"Done. {total_generated} candidates generated in {elapsed:.1f}s")
    if error_entries:
        print(f"[generate] Completed with {len(error_entries)} failed spec(s). See {manifest_output_path} for details.")
    else:
        print(f"[generate] Wrote {manifest_output_path}")
    return 0


def main(argv: Sequence[str] | None = None) -> int:
    args = parse_args() if argv is None else parse_args_from(argv)
    return run_generation(args)


if __name__ == "__main__":
    raise SystemExit(main())
