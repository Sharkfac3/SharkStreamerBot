#!/usr/bin/env python3
"""Helpers for clip extraction manifests."""

from __future__ import annotations

from datetime import UTC, datetime
from pathlib import Path
from typing import Any
import json
import re


TIME_PATTERN = re.compile(r"[^0-9A-Za-z]+")


def load_highlight_manifest(path: Path) -> dict[str, Any]:
    """Load and validate a highlight manifest JSON file."""
    try:
        payload = json.loads(path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as error:
        raise ValueError(f"Invalid JSON in highlight manifest {path}: {error}") from error

    if not isinstance(payload, dict):
        raise ValueError(f"Highlight manifest must be a JSON object: {path}")

    highlights = payload.get("highlights")
    if not isinstance(highlights, list):
        raise ValueError(f"Highlight manifest is missing a 'highlights' list: {path}")

    return payload


def load_clip_manifest(path: Path) -> dict[str, Any]:
    """Load and validate a clip manifest JSON file."""
    try:
        payload = json.loads(path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as error:
        raise ValueError(f"Invalid JSON in clip manifest {path}: {error}") from error

    if not isinstance(payload, dict):
        raise ValueError(f"Clip manifest must be a JSON object: {path}")

    clips = payload.get("clips")
    if not isinstance(clips, list):
        raise ValueError(f"Clip manifest is missing a 'clips' list: {path}")

    return payload


def write_clip_manifest(payload: dict[str, Any], output_path: Path) -> None:
    """Write a clip extraction manifest with stable formatting."""
    output_path.parent.mkdir(parents=True, exist_ok=True)
    output_path.write_text(json.dumps(payload, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")


def build_clip_manifest(
    *,
    highlight_manifest_path: Path,
    highlight_manifest: dict[str, Any],
    source_recording_path: Path,
    source_variant: str,
    source_duration_seconds: float | None,
    source_video_info: dict[str, Any],
    ffmpeg_path: Path,
    ffprobe_path: Path,
    clip_count: int,
    clips: list[dict[str, Any]],
) -> dict[str, Any]:
    """Build the persisted JSON payload for extracted clips."""
    detection = highlight_manifest.get("detection") if isinstance(highlight_manifest.get("detection"), dict) else {}
    source_recording = (
        highlight_manifest.get("source_recording") if isinstance(highlight_manifest.get("source_recording"), dict) else {}
    )

    return {
        "schema_version": 1,
        "generated_at_utc": datetime.now(UTC).isoformat(),
        "source_highlights": {
            "file_name": highlight_manifest_path.name,
            "path": str(highlight_manifest_path),
        },
        "source_recording": {
            "file_name": source_recording.get("file_name") or source_recording_path.name,
            "path": str(source_recording_path),
            "variant": source_variant,
            "duration_seconds": source_duration_seconds,
            "video": source_video_info,
        },
        "upstream_detection": {
            "model": detection.get("model"),
            "ollama_url": detection.get("ollama_url"),
            "window_count": detection.get("window_count"),
        },
        "extraction": {
            "ffmpeg_path": str(ffmpeg_path),
            "ffprobe_path": str(ffprobe_path),
            "clip_count": clip_count,
        },
        "clips": clips,
    }


def derive_clip_prefix(source_file_name: str | None, fallback_stem: str) -> str:
    """Return a filesystem-safe prefix for extracted clip file names."""
    base_name = Path(source_file_name or fallback_stem).stem
    if base_name.lower().endswith("-vertical"):
        base_name = base_name[:-9]

    parts = [part for part in TIME_PATTERN.split(base_name) if part]
    if not parts:
        parts = [fallback_stem]

    return "_".join(parts)


def clip_output_name(prefix: str, index: int, category: str) -> str:
    """Return the clip output filename for one highlight."""
    normalized_category = slugify(category or "clip")
    return f"{prefix}_{index:03d}_{normalized_category}.mp4"


def clip_manifest_name(prefix: str) -> str:
    """Return the JSON manifest filename for one highlight manifest."""
    return f"{prefix}.json"


def review_output_name(clip_stem: str) -> str:
    """Return the review queue video filename for one formatted clip."""
    return f"{clip_stem}.mp4"


def review_metadata_name(clip_stem: str) -> str:
    """Return the review queue metadata filename for one formatted clip."""
    return f"{clip_stem}_meta.json"


def slugify(value: Any) -> str:
    """Return a simple lowercase slug for filenames."""
    text = str(value or "").strip().lower()
    text = TIME_PATTERN.sub("_", text)
    text = re.sub(r"_+", "_", text).strip("_")
    return text or "clip"
