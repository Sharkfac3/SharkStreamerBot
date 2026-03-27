#!/usr/bin/env python3
"""Deterministic prompt assembly for art pipeline specs."""

from __future__ import annotations

from dataclasses import dataclass
from typing import Any
import re

from .canon_parser import CharacterCanon, StyleCanon


MANDATORY_STYLE_PREFIX = ["anime style", "cel shading", "clean linework"]
MANDATORY_NEGATIVE_PREFIX = [
    "photorealistic",
    "3d render",
    "western cartoon",
    "gritty",
    "painterly",
    "blurry",
    "low contrast",
    "missing eye highlights",
]
QUALITY_TAGS = ["high contrast", "bold silhouette", "detailed"]
ASSET_TYPE_NEGATIVES = {
    "overlay": ["background elements", "text", "watermark", "border"],
    "panel": ["background elements", "text", "watermark", "border"],
    "redemption-overlay": ["background elements", "text", "watermark", "border"],
}
STYLE_BACKGROUND_NORMALIZATION = {
    "transparent (png)": "transparent background",
    "scene or stylized anime gradient background": "scene background",
    "clean white or soft neutral — no distracting background": "clean white background",
}


@dataclass(frozen=True)
class PromptOutput:
    """Structured prompt output for one spec."""

    positive_prompt: str
    negative_prompt: str
    generation_params: dict[str, Any]
    canon_sources: list[str]


def build_prompt(
    character_canon: CharacterCanon,
    style_canon: StyleCanon,
    spec: dict[str, Any],
    asset_type_config: dict[str, Any],
) -> PromptOutput:
    """Build deterministic positive/negative prompts and generation params for one spec."""
    requirements = dict(spec.get("requirements") or {})
    asset_type = str(spec.get("asset_type") or "").strip()

    positive_parts = _join_prompt_parts(
        MANDATORY_STYLE_PREFIX,
        _normalize_body(character_canon),
        _normalize_clothing(character_canon.clothing),
        _normalize_colors(character_canon.colors),
        _normalize_eyes(character_canon),
        _normalize_accessories(character_canon.accessories),
        [_normalize_expression(requirements.get("expression") or character_canon.expression_default)],
        [_normalize_framing(requirements.get("framing"))],
        [_resolve_background_tag(style_canon, asset_type, requirements)],
        QUALITY_TAGS,
    )

    negative_parts = _join_prompt_parts(
        _normalize_negative_base(style_canon.negative_base),
        _normalize_avoids(character_canon.avoid),
        _asset_type_exclusions(asset_type),
    )

    generation_params = _build_generation_params(requirements, asset_type_config)
    canon_sources = _canon_sources(spec)

    return PromptOutput(
        positive_prompt=", ".join(positive_parts),
        negative_prompt=", ".join(negative_parts),
        generation_params=generation_params,
        canon_sources=canon_sources,
    )


def _build_generation_params(requirements: dict[str, Any], asset_type_config: dict[str, Any]) -> dict[str, Any]:
    params = dict(asset_type_config.get("sd_defaults") or {})

    width = _int_or_none(requirements.get("width"))
    height = _int_or_none(requirements.get("height"))
    if width is not None:
        params["width"] = width
    if height is not None:
        params["height"] = height

    params.setdefault("seed", -1)
    return params


def _canon_sources(spec: dict[str, Any]) -> list[str]:
    values = [spec.get("style_agent"), spec.get("character_agent")]
    result: list[str] = []
    seen: set[str] = set()

    for value in values:
        text = str(value or "").strip()
        if not text or text in seen:
            continue
        seen.add(text)
        result.append(text)

    return result


def _normalize_body(character_canon: CharacterCanon) -> list[str]:
    parts: list[str] = []
    if character_canon.species:
        parts.append(_clean_tag(character_canon.species))

    for item in character_canon.body:
        normalized = _normalize_body_item(item, character_canon.species)
        if normalized:
            parts.append(normalized)

    return _dedupe(parts)


def _normalize_body_item(value: str, species: str) -> str:
    text = _clean_tag(value)
    if not text:
        return ""

    lower = text.lower().rstrip(".")
    species_lower = _clean_tag(species).lower()

    if lower == species_lower:
        return ""
    if lower.startswith("the director is a friendly toad"):
        return ""
    if lower.startswith("the water wizard is always a humanoid wizard"):
        return ""
    if lower.startswith("the water wizard appears middle-aged to elderly"):
        return "middle-aged to elderly"
    if lower.startswith("always appears middle-aged to elderly"):
        return "middle-aged to elderly"
    if lower.startswith("always has silver or white hair and a short beard"):
        return "silver or white hair"
    if lower.startswith("he should look experienced and wise"):
        return "experienced and wise"
    if lower.startswith("long silver or white hair"):
        return "long silver or white hair"
    if lower.startswith("hair may be slightly messy or wind-worn"):
        return "slightly messy or wind-worn hair"
    if lower.startswith("often extends to the shoulders or slightly past"):
        return "shoulder-length hair"
    if lower.startswith("short silver beard"):
        return "short silver beard"
    if lower.startswith("well kept but natural"):
        return "well-kept natural beard"
    if lower.startswith("the director is always a toad"):
        return ""
    if lower.startswith("the director is always cartoon-like"):
        return "cartoon-like"
    if lower.startswith("keep detail level simple and readable"):
        return "simple readable design"
    if lower.startswith("use minimal texture"):
        return "minimal texture"
    if lower.startswith("use few or no warts"):
        return "few or no warts"
    if lower.startswith("the director should feel relaxed"):
        return "quietly in control"

    return text.rstrip(".")


def _normalize_clothing(values: list[str]) -> list[str]:
    return _dedupe(_clean_tag(value) for value in values if _clean_tag(value))


def _normalize_colors(values: list[str]) -> list[str]:
    return _dedupe(_clean_tag(value) for value in values if _clean_tag(value))


def _normalize_eyes(character_canon: CharacterCanon) -> list[str]:
    normalized_name = _slugify(character_canon.name)
    parts: list[str] = []

    if normalized_name == "the-director":
        parts.append("four eyes")

    for item in character_canon.eyes:
        normalized = _normalize_eye_item(item, normalized_name)
        if normalized:
            parts.append(normalized)

    return _dedupe(parts)


def _normalize_eye_item(value: str, character_slug: str) -> str:
    text = _clean_tag(value)
    if not text:
        return ""

    lower = text.lower().rstrip(".")

    if character_slug == "the-director":
        if lower in {"star", "spiral", "heart", "diamond", "x", "circle"}:
            return f"{lower} pupil"
        if lower.startswith("the director has four eyes"):
            return ""
        if lower.startswith("each eye must have a different pupil shape"):
            return "different pupil shapes"
        if lower.startswith("the eyes should feel surreal"):
            return "surreal readable eyes"

    if lower.startswith("bright blue eyes"):
        return "bright blue eyes"
    if lower.startswith("eyes may have a subtle magical glow"):
        return "subtle magical glow"

    return text.rstrip(".")


def _normalize_accessories(values: list[str]) -> list[str]:
    parts: list[str] = []
    for item in values:
        normalized = _normalize_accessory_item(item)
        if normalized:
            parts.append(normalized)
    return _dedupe(parts)


def _normalize_accessory_item(value: str) -> str:
    text = _clean_tag(value)
    if not text:
        return ""

    lower = text.lower().rstrip(".")

    if lower.startswith("the water wizard wears a large glowing blue gemstone pendant"):
        return "large glowing blue gemstone pendant"
    if lower.startswith("the gemstone should appear bright and slightly luminous"):
        return "slightly luminous gemstone"
    if lower.startswith("the water wizard frequently carries a wooden staff"):
        return "wooden staff"
    if lower.startswith("the staff contains a crystal orb at the top"):
        return "crystal orb staff"
    if lower.startswith("inside the orb is swirling magical water energy"):
        return "swirling water magic"
    if lower.startswith("the gem symbolizes"):
        return ""

    return text.rstrip(".")


def _normalize_expression(value: Any) -> str:
    text = _clean_tag(value)
    if not text:
        return ""
    return text.rstrip(".")


def _normalize_framing(value: Any) -> str:
    text = _clean_tag(value)
    if not text:
        return ""
    if text.endswith("framing"):
        return text
    return f"{text} framing"


def _resolve_background_tag(style_canon: StyleCanon, asset_type: str, requirements: dict[str, Any]) -> str:
    if _is_transparent_requirement(requirements):
        return "transparent background"

    rule = _background_rule_for_asset_type(style_canon, asset_type)
    normalized = STYLE_BACKGROUND_NORMALIZATION.get(rule.lower(), "") if rule else ""
    if normalized:
        return normalized

    if rule:
        cleaned = _clean_tag(rule).rstrip(".")
        return cleaned.lower() if cleaned else ""

    return ""


def _background_rule_for_asset_type(style_canon: StyleCanon, asset_type: str) -> str:
    normalized_asset_type = _slugify(asset_type)
    rules = style_canon.background_rules or {}

    if normalized_asset_type in rules:
        return rules[normalized_asset_type]

    parts = [part for part in normalized_asset_type.split("-") if part]
    for part in parts:
        if part in rules:
            return rules[part]

    for key, value in rules.items():
        if key in normalized_asset_type or normalized_asset_type in key:
            return value

    return ""


def _normalize_negative_base(values: list[str]) -> list[str]:
    normalized = _dedupe(_clean_negative(value) for value in values if _clean_negative(value))
    if not normalized:
        return list(MANDATORY_NEGATIVE_PREFIX)

    result: list[str] = []
    seen: set[str] = set()

    for required in MANDATORY_NEGATIVE_PREFIX:
        key = required.lower()
        if key not in seen:
            seen.add(key)
            result.append(required)

    for value in normalized:
        key = value.lower()
        if key not in seen and key in {item.lower() for item in MANDATORY_NEGATIVE_PREFIX}:
            seen.add(key)
            result.append(value)

    return result


def _normalize_avoids(values: list[str]) -> list[str]:
    return _dedupe(_clean_negative(value) for value in values if _clean_negative(value))


def _asset_type_exclusions(asset_type: str) -> list[str]:
    normalized = _slugify(asset_type)
    parts: list[str] = []

    if normalized in ASSET_TYPE_NEGATIVES:
        parts.extend(ASSET_TYPE_NEGATIVES[normalized])

    for token in normalized.split("-"):
        parts.extend(ASSET_TYPE_NEGATIVES.get(token, []))

    return _dedupe(parts)


def _join_prompt_parts(*groups: list[str]) -> list[str]:
    combined: list[str] = []
    for group in groups:
        combined.extend(group)
    return _dedupe(combined)


def _is_transparent_requirement(requirements: dict[str, Any]) -> bool:
    value = requirements.get("transparent_background")
    if isinstance(value, bool):
        return value
    if isinstance(value, str):
        return value.strip().lower() in {"1", "true", "yes", "on"}
    return False


def _int_or_none(value: Any) -> int | None:
    if value is None or value == "":
        return None
    try:
        return int(value)
    except (TypeError, ValueError):
        return None


def _clean_tag(value: Any) -> str:
    text = str(value or "").strip()
    text = text.replace("**", "")
    text = text.replace("__", "")
    text = text.replace("\r", " ").replace("\n", " ")
    text = text.replace("—", "-")
    text = re.sub(r"\s+", " ", text)
    text = text.strip(" ,;:-")
    return text


def _clean_negative(value: Any) -> str:
    text = _clean_tag(value)
    if not text:
        return ""
    return text.rstrip(".")


def _slugify(value: Any) -> str:
    text = str(value or "").strip().lower()
    text = re.sub(r"[^a-z0-9]+", "-", text)
    return text.strip("-")


def _dedupe(values) -> list[str]:
    seen: set[str] = set()
    result: list[str] = []
    for value in values:
        text = _clean_tag(value)
        if not text:
            continue
        key = text.lower()
        if key in seen:
            continue
        seen.add(key)
        result.append(text)
    return result
