#!/usr/bin/env python3
"""Naming helpers for art pipeline specs and generated assets."""

from __future__ import annotations

from typing import Any
import re


SLUG_PATTERN = re.compile(r"[^a-z0-9]+")
CHARACTER_FILENAME_MAP = {
    "captain-stretch": "captainstretch",
    "the-director": "thedirector",
    "water-wizard": "waterwizard",
}


def asset_filename(character: str, asset_type: str, version_note: str = "", extension: str = "png") -> str:
    """Return the generated asset filename for a character asset."""
    character_part = _character_filename_slug(character)
    asset_type_part = _slugify(asset_type)
    parts = [character_part, asset_type_part]

    if version_note.strip():
        parts.append(_slugify(version_note))

    normalized_extension = _normalize_extension(extension)
    return f"{'-'.join(part for part in parts if part)}.{normalized_extension}"


def spec_id(character: str, asset_type: str) -> str:
    """Return the manifest/spec identifier for one character asset type pairing."""
    return f"{_slugify(character)}-{_slugify(asset_type)}"


def _character_filename_slug(character: str) -> str:
    normalized = _slugify(character)
    if normalized in CHARACTER_FILENAME_MAP:
        return CHARACTER_FILENAME_MAP[normalized]
    return normalized.replace("-", "")


def _normalize_extension(extension: str) -> str:
    text = str(extension or "png").strip().lower().lstrip(".")
    return text or "png"


def _slugify(value: Any) -> str:
    text = str(value or "").strip().lower()
    text = SLUG_PATTERN.sub("-", text)
    return text.strip("-")
