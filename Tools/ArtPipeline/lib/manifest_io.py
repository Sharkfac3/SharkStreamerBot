#!/usr/bin/env python3
"""Helpers for art pipeline manifest I/O and batch identifiers."""

from __future__ import annotations

from datetime import UTC, datetime
from pathlib import Path
from typing import Any
import json
import re


SCHEMA_VERSION = 1
SLUG_PATTERN = re.compile(r"[^a-z0-9]+")


def read_manifest(path: Path) -> dict[str, Any]:
    """Load and validate an art pipeline manifest JSON file."""
    try:
        payload = json.loads(path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as error:
        raise ValueError(f"Invalid JSON in manifest {path}: {error}") from error

    if not isinstance(payload, dict):
        raise ValueError(f"Art pipeline manifest must be a JSON object: {path}")

    schema_version = payload.get("schema_version")
    if schema_version != SCHEMA_VERSION:
        raise ValueError(
            f"Unsupported manifest schema_version in {path}: expected {SCHEMA_VERSION}, got {schema_version!r}"
        )

    return payload


def write_manifest(path: Path, data: dict[str, Any]) -> Path:
    """Write an art pipeline manifest JSON file with stable formatting."""
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(data, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    return path


def generate_batch_id(asset_type: str, characters: list[str], suffix: str = "") -> str:
    """Return a stable batch identifier for one generation run."""
    normalized_characters = [_slugify(character) for character in characters if _slugify(character)]
    character_part = "-".join(normalized_characters) if normalized_characters else "shared"

    parts = [character_part, _slugify(asset_type)]
    if suffix.strip():
        parts.append(_slugify(suffix))

    return "-".join(part for part in parts if part)


def timestamp_utc() -> str:
    """Return the current UTC timestamp in ISO 8601 format."""
    return datetime.now(UTC).isoformat()


def _slugify(value: Any) -> str:
    text = str(value or "").strip().lower()
    text = SLUG_PATTERN.sub("-", text)
    return text.strip("-")
