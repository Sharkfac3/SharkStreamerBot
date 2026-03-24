#!/usr/bin/env python3
"""Helpers for highlight detection manifests."""

from __future__ import annotations

from datetime import UTC, datetime
from pathlib import Path
from typing import Any
import json


def load_transcript_json(path: Path) -> dict[str, Any]:
    """Load a transcript JSON file and validate its basic shape."""
    try:
        payload = json.loads(path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as error:
        raise ValueError(f"Invalid JSON in transcript file {path}: {error}") from error

    if not isinstance(payload, dict):
        raise ValueError(f"Transcript payload must be a JSON object: {path}")

    segments = payload.get("segments")
    if not isinstance(segments, list):
        raise ValueError(f"Transcript payload is missing a 'segments' list: {path}")

    return payload


def write_highlight_manifest(payload: dict[str, Any], output_path: Path) -> None:
    """Write a highlight manifest JSON file with stable formatting."""
    output_path.parent.mkdir(parents=True, exist_ok=True)
    output_path.write_text(json.dumps(payload, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")


def build_highlight_manifest(
    *,
    transcript_path: Path,
    transcript_payload: dict[str, Any],
    model_name: str,
    ollama_url: str,
    window_count: int,
    highlights: list[dict[str, Any]],
) -> dict[str, Any]:
    """Build the persisted JSON payload for detected highlights."""
    source = transcript_payload.get("source") if isinstance(transcript_payload.get("source"), dict) else {}
    transcription = (
        transcript_payload.get("transcription") if isinstance(transcript_payload.get("transcription"), dict) else {}
    )

    return {
        "schema_version": 1,
        "generated_at_utc": datetime.now(UTC).isoformat(),
        "source_transcript": {
            "file_name": transcript_path.name,
            "path": str(transcript_path),
        },
        "source_recording": {
            "file_name": source.get("file_name"),
            "path": source.get("path"),
        },
        "detection": {
            "model": model_name,
            "ollama_url": ollama_url,
            "window_count": window_count,
            "transcript_language": transcription.get("language"),
            "transcript_duration_seconds": transcription.get("duration_seconds"),
        },
        "highlights": highlights,
    }


def format_seconds(value: float) -> str:
    """Format seconds as H:MM:SS for human-readable logs and prompts."""
    total_seconds = max(0, int(round(value)))
    hours, remainder = divmod(total_seconds, 3600)
    minutes, seconds = divmod(remainder, 60)
    return f"{hours}:{minutes:02}:{seconds:02}"


def normalize_text(value: Any) -> str:
    """Return a compact one-line string for prompt/output use."""
    return " ".join(str(value or "").split())
