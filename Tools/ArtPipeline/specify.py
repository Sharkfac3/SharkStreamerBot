#!/usr/bin/env python3
"""Create spec manifests for downstream art pipeline phases."""

from __future__ import annotations

import argparse
import json
from pathlib import Path
import re
import sys
from typing import Any, Sequence

from config import load_settings
from lib.canon_parser import parse_character_agent
from lib.manifest_io import SCHEMA_VERSION, generate_batch_id, write_manifest
from lib.naming import spec_id


STYLE_AGENT_PATH = Path("Creative/Art/Agents/stream-style-art-agent.md")
CHARACTER_AGENT_DIR = Path("Creative/Art/Agents")
TIMESTAMP_FORMAT = "%Y-%m-%dT%H:%M:%SZ"
DEFAULT_EXPRESSION = "neutral"
DEFAULT_POSE = "standing"
AUTO_BATCH_SUFFIX = "v1"


def build_parser() -> argparse.ArgumentParser:
    """Build the CLI parser."""
    parser = argparse.ArgumentParser(description="Build art spec manifests from character agents and asset types")
    parser.add_argument("--asset-type", required=True, help="Asset type key from Tools/ArtPipeline/asset_types.json")
    parser.add_argument("--characters", nargs="+", required=True, help="One or more character slugs")
    parser.add_argument("--batch-id", help="Optional batch id. Auto-generated if omitted.")
    parser.add_argument("--expression", help="Override expression for all generated specs")
    parser.add_argument("--pose", help="Override pose for all generated specs")
    parser.add_argument("--notes", default="", help="Optional note stored on each generated spec")
    parser.add_argument("--dry-run", action="store_true", help="Print manifest JSON without writing a file")
    return parser


def parse_args() -> argparse.Namespace:
    """Parse command-line arguments."""
    return build_parser().parse_args()


def parse_args_from(argv: Sequence[str]) -> argparse.Namespace:
    """Parse arguments from an explicit argv list for tests."""
    return build_parser().parse_args(list(argv))


def load_asset_types(path: Path) -> dict[str, Any]:
    """Load the asset type registry."""
    try:
        payload = json.loads(path.read_text(encoding="utf-8"))
    except FileNotFoundError as error:
        raise FileNotFoundError(f"Asset type registry not found: {path}") from error
    except json.JSONDecodeError as error:
        raise ValueError(f"Invalid JSON in asset type registry {path}: {error}") from error

    if not isinstance(payload, dict):
        raise ValueError(f"Asset type registry must be a JSON object: {path}")

    return payload


def normalize_slug(value: str) -> str:
    """Normalize user input to a slug."""
    text = str(value or "").strip().lower()
    text = re.sub(r"[^a-z0-9]+", "-", text)
    return text.strip("-")


def dedupe_characters(characters: Sequence[str]) -> list[str]:
    """Return normalized unique character slugs preserving order."""
    seen: set[str] = set()
    result: list[str] = []

    for value in characters:
        slug = normalize_slug(value)
        if not slug or slug in seen:
            continue
        seen.add(slug)
        result.append(slug)

    if not result:
        raise ValueError("At least one valid character slug is required.")

    return result


def character_agent_path(character_slug: str) -> Path:
    """Resolve the markdown agent file for a character slug."""
    return CHARACTER_AGENT_DIR / f"{character_slug}-art-agent.md"


def require_file(path: Path, label: str) -> None:
    """Require that a file exists."""
    if not path.exists() or not path.is_file():
        raise FileNotFoundError(f"Missing {label}: {path}")


def normalize_expression(value: str) -> str:
    """Reduce parsed expression text to one simple phrase."""
    text = str(value or "").strip()
    if not text:
        return ""

    text = text.replace("\r", " ").replace("\n", " ")
    text = re.sub(r"\s+", " ", text).strip(" ,.;:-")
    if not text:
        return ""

    parts = re.split(r"[,;]|\.\s+|\s+or\s+|\s+/\s+", text, maxsplit=1, flags=re.IGNORECASE)
    candidate = parts[0].strip(" ,.;:-").lower()
    return re.sub(r"\s+", " ", candidate)


def choose_expression(character_slug: str, override: str | None, agent_path: Path) -> str:
    """Pick the final expression for one character spec."""
    if override and override.strip():
        normalized_override = normalize_expression(override)
        if normalized_override:
            return normalized_override
        print(
            f"[specify] Warning: expression override normalized empty for {character_slug}; using {DEFAULT_EXPRESSION!r}",
            file=sys.stderr,
        )
        return DEFAULT_EXPRESSION

    try:
        canon = parse_character_agent(agent_path)
    except Exception as error:
        print(
            f"[specify] Warning: failed to parse expression from {agent_path}: {error}; using {DEFAULT_EXPRESSION!r}",
            file=sys.stderr,
        )
        return DEFAULT_EXPRESSION

    normalized_expression = normalize_expression(canon.expression_default)
    if normalized_expression:
        return normalized_expression

    print(
        f"[specify] Warning: no parseable expression in {agent_path}; using {DEFAULT_EXPRESSION!r}",
        file=sys.stderr,
    )
    return DEFAULT_EXPRESSION


def choose_pose(override: str | None) -> str:
    """Pick the final pose value."""
    if override and override.strip():
        return re.sub(r"\s+", " ", override.strip().lower())
    return DEFAULT_POSE


def build_created_at_utc() -> str:
    """Return a UTC timestamp in strict Zulu format."""
    from datetime import UTC, datetime

    return datetime.now(UTC).strftime(TIMESTAMP_FORMAT)


def build_batch_id(asset_type: str, characters: list[str], explicit_batch_id: str | None) -> str:
    """Return the final batch id."""
    if explicit_batch_id and explicit_batch_id.strip():
        normalized = normalize_slug(explicit_batch_id)
        if normalized:
            return normalized
        raise ValueError("--batch-id normalized to an empty value")

    return generate_batch_id(asset_type, characters, suffix=AUTO_BATCH_SUFFIX)


def build_spec(
    character_slug: str,
    *,
    asset_type: str,
    asset_type_entry: dict[str, Any],
    expression_override: str | None,
    pose_override: str | None,
    notes: str,
) -> dict[str, Any]:
    """Build one spec entry for a character."""
    agent_path = character_agent_path(character_slug)
    require_file(agent_path, f"character agent for {character_slug}")

    requirements = dict(asset_type_entry.get("default_requirements") or {})
    requirements["expression"] = choose_expression(character_slug, expression_override, agent_path)
    requirements["pose"] = choose_pose(pose_override)

    return {
        "spec_id": spec_id(character_slug, asset_type),
        "character": character_slug,
        "character_agent": agent_path.as_posix(),
        "style_agent": STYLE_AGENT_PATH.as_posix(),
        "asset_type": asset_type,
        "requirements": requirements,
        "status": "pending",
        "notes": notes,
    }


def build_manifest(args: argparse.Namespace) -> tuple[Path | None, dict[str, Any]]:
    """Build the output manifest and destination path."""
    settings = load_settings()
    asset_types = load_asset_types(settings.asset_types_path)

    asset_type = normalize_slug(args.asset_type)
    if asset_type not in asset_types:
        available = ", ".join(sorted(asset_types)) or "(none)"
        raise ValueError(f"Unknown asset type {args.asset_type!r}. Available: {available}")

    require_file(settings.repo_root / STYLE_AGENT_PATH, "style agent")

    characters = dedupe_characters(args.characters)
    batch_id = build_batch_id(asset_type, characters, args.batch_id)
    output_path = settings.specs_dir / f"{batch_id}.json"

    if not args.dry_run and output_path.exists():
        raise FileExistsError(
            f"Spec manifest already exists: {output_path}. Re-run with a different --batch-id. Future --force support can overwrite."
        )

    asset_type_entry = asset_types[asset_type]
    if not isinstance(asset_type_entry, dict):
        raise ValueError(f"Asset type entry must be an object: {asset_type}")

    specs = [
        build_spec(
            character_slug,
            asset_type=asset_type,
            asset_type_entry=asset_type_entry,
            expression_override=args.expression,
            pose_override=args.pose,
            notes=args.notes or "",
        )
        for character_slug in characters
    ]

    manifest = {
        "schema_version": SCHEMA_VERSION,
        "batch_id": batch_id,
        "created_at_utc": build_created_at_utc(),
        "asset_type": asset_type,
        "specs": specs,
    }

    return output_path, manifest


def main(argv: Sequence[str] | None = None) -> int:
    args = parse_args() if argv is None else parse_args_from(argv)

    try:
        output_path, manifest = build_manifest(args)
    except Exception as error:
        print(f"[specify] Configuration error: {error}", file=sys.stderr)
        return 1

    if args.dry_run:
        print(json.dumps(manifest, indent=2, ensure_ascii=False))
        print(f"[specify] Dry run complete. specs={len(manifest['specs'])}", file=sys.stderr)
        return 0

    try:
        settings = load_settings()
        settings.ensure_data_dirs()
        assert output_path is not None
        write_manifest(output_path, manifest)
    except Exception as error:
        print(f"[specify] Write error: {error}", file=sys.stderr)
        return 1

    print(f"[specify] Wrote {output_path}")
    print(f"[specify] Done. specs={len(manifest['specs'])} asset_type={manifest['asset_type']}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
