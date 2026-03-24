#!/usr/bin/env python3
"""Transcript serialization helpers."""

from __future__ import annotations

from datetime import UTC, datetime
from pathlib import Path
from typing import Any
import json
import math
import re


VERTICAL_SUFFIX = re.compile(r"-vertical$", re.IGNORECASE)


def write_srt(segments: list[dict[str, Any]], output_path: Path) -> None:
    """Write transcript segments to SRT format."""
    lines: list[str] = []

    for index, segment in enumerate(segments, start=1):
        start = float(segment.get("start", 0.0))
        end = float(segment.get("end", 0.0))
        text = str(segment.get("text", "")).strip()

        lines.append(str(index))
        lines.append(f"{format_srt_timestamp(start)} --> {format_srt_timestamp(end)}")
        lines.append(text)
        lines.append("")

    output_path.parent.mkdir(parents=True, exist_ok=True)
    output_path.write_text("\n".join(lines).rstrip() + "\n", encoding="utf-8")


def write_transcript_json(payload: dict[str, Any], output_path: Path) -> None:
    """Write transcript metadata to JSON with stable formatting."""
    output_path.parent.mkdir(parents=True, exist_ok=True)
    output_path.write_text(json.dumps(payload, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")


def load_transcript_json(path: Path) -> dict[str, Any]:
    """Load and validate a transcript JSON payload."""
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


def build_transcript_payload(
    *,
    source_path: Path,
    info: Any,
    segments: list[dict[str, Any]],
    model_name: str,
    language: str | None,
    word_timestamps: bool,
    vad_enabled: bool,
) -> dict[str, Any]:
    """Build the persisted JSON payload for a transcript."""
    return {
        "schema_version": 1,
        "generated_at_utc": datetime.now(UTC).isoformat(),
        "source": {
            "file_name": source_path.name,
            "path": str(source_path),
        },
        "transcription": {
            "model": model_name,
            "language": language or getattr(info, "language", None),
            "language_probability": getattr(info, "language_probability", None),
            "duration_seconds": getattr(info, "duration", None),
            "duration_after_vad_seconds": getattr(info, "duration_after_vad", None),
            "word_timestamps": word_timestamps,
            "vad_filter": vad_enabled,
        },
        "segments": segments,
    }


def segment_to_dict(segment: Any) -> dict[str, Any]:
    """Convert a faster-whisper segment object to a JSON-safe dict."""
    return {
        "id": getattr(segment, "id", None),
        "seek": getattr(segment, "seek", None),
        "start": _clean_number(getattr(segment, "start", None)),
        "end": _clean_number(getattr(segment, "end", None)),
        "text": str(getattr(segment, "text", "")).strip(),
        "avg_logprob": _clean_number(getattr(segment, "avg_logprob", None)),
        "compression_ratio": _clean_number(getattr(segment, "compression_ratio", None)),
        "no_speech_prob": _clean_number(getattr(segment, "no_speech_prob", None)),
        "temperature": _clean_number(getattr(segment, "temperature", None)),
        "words": [word_to_dict(word) for word in getattr(segment, "words", []) or []],
    }


def word_to_dict(word: Any) -> dict[str, Any]:
    """Convert a faster-whisper word object to a JSON-safe dict."""
    return {
        "start": _clean_number(getattr(word, "start", None)),
        "end": _clean_number(getattr(word, "end", None)),
        "word": str(getattr(word, "word", "")),
        "probability": _clean_number(getattr(word, "probability", None)),
    }


def transcript_stem_from_recording_name(file_name: str | None, fallback_stem: str | None = None) -> str:
    """Return the transcript stem that corresponds to a recording file name."""
    if file_name:
        stem = Path(file_name).stem
        stem = VERTICAL_SUFFIX.sub("", stem)
        if stem:
            return stem

    if fallback_stem:
        return fallback_stem

    raise ValueError("Could not determine transcript stem from recording name")


def find_transcript_path(transcripts_dir: Path, recording_file_name: str | None, fallback_stem: str | None = None) -> Path:
    """Resolve the transcript JSON path for a recording."""
    stem = transcript_stem_from_recording_name(recording_file_name, fallback_stem=fallback_stem)
    path = (transcripts_dir / f"{stem}.json").resolve()
    if not path.exists() or not path.is_file():
        raise FileNotFoundError(
            f"Could not find transcript JSON for recording {recording_file_name!r}. Expected: {path}"
        )
    return path


def collect_segments_in_range(transcript_payload: dict[str, Any], start_time: float, end_time: float) -> list[dict[str, Any]]:
    """Return normalized transcript segments overlapping the requested time range."""
    raw_segments = transcript_payload.get("segments")
    if not isinstance(raw_segments, list):
        raise ValueError("Transcript payload is missing a 'segments' list")

    segments: list[dict[str, Any]] = []
    for item in raw_segments:
        if not isinstance(item, dict):
            continue

        start = _as_float(item.get("start"))
        end = _as_float(item.get("end"))
        text = " ".join(str(item.get("text") or "").split())
        if start is None or end is None or end <= start or not text:
            continue
        if end <= start_time or start >= end_time:
            continue

        segments.append(
            {
                "start": start,
                "end": end,
                "text": text,
                "words": item.get("words") if isinstance(item.get("words"), list) else [],
            }
        )

    segments.sort(key=lambda segment: (segment["start"], segment["end"]))
    return segments


def format_srt_timestamp(value: float) -> str:
    """Return an SRT timestamp from seconds."""
    total_milliseconds = max(0, int(round(value * 1000)))
    hours, remainder = divmod(total_milliseconds, 3_600_000)
    minutes, remainder = divmod(remainder, 60_000)
    seconds, milliseconds = divmod(remainder, 1_000)
    return f"{hours:02}:{minutes:02}:{seconds:02},{milliseconds:03}"


def _clean_number(value: Any) -> float | int | None:
    if value is None:
        return None
    if isinstance(value, bool):
        return int(value)
    if isinstance(value, int):
        return value
    if isinstance(value, float):
        if math.isnan(value) or math.isinf(value):
            return None
        return value
    return value


def _as_float(value: Any) -> float | None:
    if value is None:
        return None
    if isinstance(value, (int, float)):
        return float(value)
    try:
        return float(str(value).strip())
    except ValueError:
        return None
