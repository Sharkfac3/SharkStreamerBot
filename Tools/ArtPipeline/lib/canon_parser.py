#!/usr/bin/env python3
"""Parsers for art agent markdown canon files."""

from __future__ import annotations

from dataclasses import dataclass
from pathlib import Path
import re


HEADING_PATTERN = re.compile(r"^(#{1,6})\s+(.*\S)\s*$", re.MULTILINE)
BULLET_PATTERN = re.compile(r"^\s*[-*]\s+(.*\S)\s*$")
BACKGROUND_ROW_PATTERN = re.compile(r"^\|\s*(.*?)\s*\|\s*(.*?)\s*\|\s*$")
SENTENCE_SPLIT_PATTERN = re.compile(r"(?<=[.!?])\s+")
NAME_PATTERN = re.compile(r"\*\*(.*?)\*\*")
SPECIES_PATTERNS = [
    re.compile(r"\b(?:is|appears as|looks like)\s+(?:an?|the)\s+([^.,\n]+)", re.IGNORECASE),
    re.compile(r"\b(?:always|typically)\s+(?:a|an)\s+([^.,\n]+)", re.IGNORECASE),
]
NEGATIVE_PREFIXES = ("never ", "avoid ", "no ", "without ")
STYLE_NEGATIVE_TEXT = {
    "photorealism or 3d rendering style": ["photorealistic", "3d render"],
    "western cartoon or comic book style (flat shading, thick uniform lines without taper)": [
        "western cartoon",
        "comic book style",
        "flat shading",
        "thick uniform lines",
    ],
    "gritty, painted, or textured realism": ["gritty", "painted realism", "textured realism"],
    "horror or unsettling styling": ["horror", "unsettling styling"],
    "hyper-detailed skin or material texture": ["hyper-detailed skin", "material texture"],
    "excessive visual noise (too many elements competing for attention)": ["excessive visual noise"],
    "desaturated, washed-out, or muddy palettes": ["desaturated", "washed-out", "muddy palettes"],
    "missing eye highlights — eyes without a specular dot look lifeless": ["missing eye highlights"],
    "soft airbrush shading, painted textures, complex gradient maps": [
        "soft airbrush shading",
        "painted textures",
        "complex gradient maps",
    ],
}
MANDATORY_NEGATIVE_BASE = [
    "photorealistic",
    "3d render",
    "western cartoon",
    "gritty",
    "painterly",
    "blurry",
    "low contrast",
    "missing eye highlights",
]
BACKGROUND_KEY_MAP = {
    "emotes": ["emote"],
    "overlays / panels": ["overlay", "panel"],
    "thumbnails / banners": ["thumbnail", "banner"],
    "character sheets": ["character-sheet"],
}


@dataclass(frozen=True)
class CharacterCanon:
    """Structured character appearance data parsed from an art agent markdown file."""

    name: str
    species: str
    body: list[str]
    clothing: list[str]
    colors: list[str]
    eyes: list[str]
    expression_default: str
    accessories: list[str]
    avoid: list[str]
    raw_sections: dict[str, str]


@dataclass(frozen=True)
class StyleCanon:
    """Structured style data parsed from the shared stream style agent markdown file."""

    base_style_tags: list[str]
    negative_base: list[str]
    background_rules: dict[str, str]
    raw_sections: dict[str, str]


def parse_character_agent(path: Path) -> CharacterCanon:
    """Parse a character art agent markdown file into structured canon fields."""
    text = path.read_text(encoding="utf-8")
    raw_sections = _parse_markdown_sections(text)

    name = _extract_character_name(text, path)
    species = _extract_species(raw_sections)
    body = _extract_body(raw_sections)
    clothing = _extract_clothing(raw_sections)
    colors = _collect_list(raw_sections, ["skin", "color_palette"], include_prose=False)
    eyes = _extract_eyes(raw_sections)
    expression_default = _extract_expression_default(raw_sections)
    accessories = _collect_list(raw_sections, ["signature_item", "staff"], split_sentences=True)
    avoid = _extract_avoid(raw_sections)

    return CharacterCanon(
        name=name,
        species=species,
        body=body,
        clothing=clothing,
        colors=colors,
        eyes=eyes,
        expression_default=expression_default,
        accessories=accessories,
        avoid=avoid,
        raw_sections=raw_sections,
    )


def parse_style_agent(path: Path) -> StyleCanon:
    """Parse the shared stream style markdown file into structured canon fields."""
    text = path.read_text(encoding="utf-8")
    raw_sections = _parse_markdown_sections(text)

    base_style_tags = _extract_style_tags(raw_sections)
    negative_base = _extract_style_negatives(raw_sections)
    background_rules = _extract_background_rules(raw_sections)

    return StyleCanon(
        base_style_tags=base_style_tags,
        negative_base=negative_base,
        background_rules=background_rules,
        raw_sections=raw_sections,
    )


def _parse_markdown_sections(text: str) -> dict[str, str]:
    matches = list(HEADING_PATTERN.finditer(text))
    sections: dict[str, str] = {}

    for index, match in enumerate(matches):
        heading = match.group(2).strip()
        start = match.end()
        end = matches[index + 1].start() if index + 1 < len(matches) else len(text)
        body = text[start:end].strip()
        sections[_normalize_heading(heading)] = body

    return sections


def _normalize_heading(heading: str) -> str:
    text = heading.strip().lower()
    text = re.sub(r"[^a-z0-9]+", "_", text)
    return text.strip("_")


def _extract_character_name(text: str, path: Path) -> str:
    strong_names = [name.strip() for name in NAME_PATTERN.findall(text) if name.strip()]
    if strong_names:
        for candidate in strong_names:
            if candidate.lower().startswith("the water wizard"):
                return "The Water Wizard"
            if candidate.lower().startswith("captain stretch"):
                return "Captain Stretch"
            if candidate.lower().startswith("the director"):
                return "The Director"
        return strong_names[0]

    stem = path.stem.replace("-art-agent", "").replace("-", " ").strip()
    return stem.title()


def _extract_species(raw_sections: dict[str, str]) -> str:
    for key in ("core_rules", "character_canon", "core_character_concept", "body_structure", "age"):
        section = raw_sections.get(key, "")
        for line in _iter_lines(section):
            for pattern in SPECIES_PATTERNS:
                match = pattern.search(line)
                if match:
                    species = _clean_phrase(match.group(1))
                    species = re.sub(r"\bwho\b.*$", "", species, flags=re.IGNORECASE).strip(" ,.;:-")
                    if species:
                        return species
    return ""


def _extract_body(raw_sections: dict[str, str]) -> list[str]:
    body = _collect_list(raw_sections, ["body_structure"], split_sentences=True, include_prose=False)

    if not body:
        canon_items = _extract_positive_items(raw_sections.get("character_canon", ""), split_sentences=True)
        body.extend(item for item in canon_items if _looks_like_body_trait(item))
        core_rule_items = _extract_positive_items(raw_sections.get("core_rules", ""), split_sentences=True)
        body.extend(item for item in core_rule_items if not _looks_like_clothing_or_accessory(item))

    extra_sections = [raw_sections.get("age", ""), raw_sections.get("hair", ""), raw_sections.get("facial_hair", "")]
    for section in extra_sections:
        body.extend(_extract_positive_items(section, split_sentences=True))

    return _dedupe(body)


def _extract_clothing(raw_sections: dict[str, str]) -> list[str]:
    section = raw_sections.get("clothing", "")
    if not section:
        return []

    section = re.split(r"\n\s*The style should communicate\s*:\s*", section, maxsplit=1, flags=re.IGNORECASE)[0]
    clothing = _extract_items(section, include_prose=False)
    return _dedupe(item for item in clothing if not _looks_like_expression_trait(item))


def _extract_eyes(raw_sections: dict[str, str]) -> list[str]:
    eyes = _extract_items(raw_sections.get("eyes", ""), split_sentences=True)
    if eyes:
        return _dedupe(item for item in eyes if item.lower() != "examples:")

    face_items = _extract_items(raw_sections.get("face", ""), split_sentences=True, include_prose=False)
    eye_related = [item for item in face_items if "eye" in item.lower() or "pupil" in item.lower()]
    return _dedupe(eye_related)


def _extract_expression_default(raw_sections: dict[str, str]) -> str:
    section = raw_sections.get("expression", "")
    if not section:
        return ""

    preferred_section = re.split(r"\n\s*Avoid\s*:\s*", section, maxsplit=1, flags=re.IGNORECASE)[0]
    items = _extract_positive_items(preferred_section, split_sentences=True, include_prose=False)
    if not items:
        items = _extract_positive_items(preferred_section, split_sentences=True)
    items = [item for item in items if not item.endswith(":")]
    if not items:
        return ""
    return ", ".join(items)


def _extract_avoid(raw_sections: dict[str, str]) -> list[str]:
    avoid: list[str] = []

    for key in ("core_rules", "expression", "visual_style", "what_to_avoid"):
        section = raw_sections.get(key, "")
        for item in _extract_items(section, split_sentences=True):
            normalized = item.lower().strip()
            if normalized.startswith("never "):
                avoid.append(item[6:].strip())
            elif normalized.startswith("avoid "):
                avoid.append(item[6:].strip())
            elif key == "what_to_avoid":
                mapped = STYLE_NEGATIVE_TEXT.get(normalized)
                if mapped:
                    avoid.extend(mapped)
                else:
                    avoid.append(item)
            elif any(normalized.startswith(prefix) for prefix in NEGATIVE_PREFIXES):
                avoid.append(item)

    return _dedupe(_clean_phrase(item) for item in avoid if item)


def _extract_style_tags(raw_sections: dict[str, str]) -> list[str]:
    tags: list[str] = []
    for key in ("core_style", "line_art", "shading", "eyes", "color_philosophy", "emote_and_phone_readability"):
        tags.extend(_extract_items(raw_sections.get(key, ""), split_sentences=True))
    return _dedupe(tags)


def _extract_style_negatives(raw_sections: dict[str, str]) -> list[str]:
    negatives = list(MANDATORY_NEGATIVE_BASE)
    for item in _extract_items(raw_sections.get("what_to_avoid", ""), split_sentences=True):
        normalized = item.lower().strip()
        mapped = STYLE_NEGATIVE_TEXT.get(normalized)
        if mapped:
            negatives.extend(mapped)
        else:
            negatives.append(_clean_phrase(item))
    return _dedupe(negative for negative in negatives if negative)


def _extract_background_rules(raw_sections: dict[str, str]) -> dict[str, str]:
    section = raw_sections.get("backgrounds", "")
    rows = _extract_table_rows(section)
    rules: dict[str, str] = {}

    for raw_key, value in rows:
        normalized_key = raw_key.strip().lower()
        slugs = BACKGROUND_KEY_MAP.get(normalized_key)
        if not slugs:
            slugs = [_slugify(normalized_key)]
        for slug in slugs:
            rules[slug] = value.strip()

    return rules


def _extract_table_rows(section: str) -> list[tuple[str, str]]:
    rows: list[tuple[str, str]] = []
    for line in section.splitlines():
        match = BACKGROUND_ROW_PATTERN.match(line.strip())
        if not match:
            continue
        left = match.group(1).strip()
        right = match.group(2).strip()
        if not left or not right:
            continue
        if set(left) == {"-"}:
            continue
        if left.lower() == "asset type":
            continue
        rows.append((left, right))
    return rows


def _collect_list(
    raw_sections: dict[str, str],
    keys: list[str],
    *,
    split_sentences: bool = False,
    include_prose: bool = True,
) -> list[str]:
    items: list[str] = []
    for key in keys:
        items.extend(_extract_items(raw_sections.get(key, ""), split_sentences=split_sentences, include_prose=include_prose))
    return _dedupe(items)


def _extract_positive_items(
    section: str,
    *,
    split_sentences: bool = False,
    include_prose: bool = True,
) -> list[str]:
    return [
        item
        for item in _extract_items(section, split_sentences=split_sentences, include_prose=include_prose)
        if not _is_negative_item(item)
    ]


def _extract_items(section: str, *, split_sentences: bool = False, include_prose: bool = True) -> list[str]:
    if not section.strip():
        return []

    items: list[str] = []
    bullet_items = _extract_bullet_items(section)
    if bullet_items:
        items.extend(bullet_items)

    if include_prose:
        prose_lines = [line for line in (_clean_phrase(line) for line in _iter_lines(_strip_bullets(section))) if line]
        if split_sentences:
            for line in prose_lines:
                items.extend(_split_sentences(line))
        else:
            items.extend(prose_lines)

    return _dedupe(items)


def _extract_bullet_items(section: str) -> list[str]:
    items: list[str] = []
    for line in section.splitlines():
        match = BULLET_PATTERN.match(line)
        if match:
            items.append(match.group(1).strip())
    return items


def _strip_bullets(section: str) -> str:
    lines: list[str] = []
    for line in section.splitlines():
        if BULLET_PATTERN.match(line):
            continue
        lines.append(line)
    return "\n".join(lines)


def _iter_lines(section: str) -> list[str]:
    return [line.strip() for line in section.splitlines() if line.strip() and not line.strip().startswith(">")]


def _split_sentences(value: str) -> list[str]:
    sentences: list[str] = []
    for sentence in SENTENCE_SPLIT_PATTERN.split(value):
        cleaned = _clean_phrase(sentence)
        if cleaned:
            sentences.append(cleaned)
    return sentences


def _is_negative_item(value: str) -> bool:
    normalized = _clean_phrase(value).lower()
    return normalized.startswith(NEGATIVE_PREFIXES)


def _looks_like_body_trait(value: str) -> bool:
    normalized = _clean_phrase(value).lower()
    keywords = (
        "toad",
        "shrimp",
        "wizard",
        "humanoid",
        "hair",
        "beard",
        "eye",
        "wart",
        "texture",
        "readable",
        "cartoon",
        "posture",
        "antennae",
        "exoskeleton",
        "head",
        "arms",
        "legs",
        "middle-aged",
        "elderly",
    )
    return any(keyword in normalized for keyword in keywords)


def _looks_like_clothing_or_accessory(value: str) -> bool:
    normalized = _clean_phrase(value).lower()
    keywords = (
        "wear",
        "wears",
        "clothing",
        "robe",
        "cloak",
        "uniform",
        "pendant",
        "staff",
        "hat",
        "jacket",
        "tie",
    )
    return any(keyword in normalized for keyword in keywords)


def _looks_like_expression_trait(value: str) -> bool:
    normalized = _clean_phrase(value).lower()
    return normalized in {"leadership", "calmness", "friendliness", "creative authority"}


def _clean_phrase(value: str) -> str:
    text = str(value or "").strip()
    text = text.replace("**", "")
    text = text.replace("__", "")
    text = re.sub(r"\s+", " ", text)
    text = text.strip("-*• ")
    return text.strip()


def _slugify(value: str) -> str:
    text = value.strip().lower()
    text = re.sub(r"[^a-z0-9]+", "-", text)
    return text.strip("-")


def _dedupe(values) -> list[str]:
    seen: set[str] = set()
    result: list[str] = []
    for value in values:
        text = _clean_phrase(value)
        if not text:
            continue
        key = text.lower()
        if key in seen:
            continue
        seen.add(key)
        result.append(text)
    return result
