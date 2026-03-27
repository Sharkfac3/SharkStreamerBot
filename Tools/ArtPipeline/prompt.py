#!/usr/bin/env python3
"""Build prompt manifests from art spec manifests."""

from __future__ import annotations

import argparse
import json
from pathlib import Path
import sys
from typing import Any, Sequence

from config import load_settings
from lib.canon_parser import parse_character_agent, parse_style_agent
from lib.manifest_io import SCHEMA_VERSION, read_manifest, timestamp_utc, write_manifest
from lib.prompt_builder import build_prompt


DEFAULT_STATUS = "ready"


def build_parser() -> argparse.ArgumentParser:
    """Build the CLI parser."""
    parser = argparse.ArgumentParser(description="Build art prompt manifests from spec manifests")
    parser.add_argument("batch_id", help="Batch id of the source spec manifest")
    parser.add_argument("--batch-count", type=int, help="Override generation batch count for all prompts")
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


def build_manifest(args: argparse.Namespace) -> tuple[Path | None, dict[str, Any]]:
    """Build the prompt manifest and destination path."""
    settings = load_settings()
    asset_types = load_asset_types(settings.asset_types_path)

    source_spec = settings.specs_dir / f"{args.batch_id}.json"
    spec_manifest = read_manifest(source_spec)

    batch_id = str(spec_manifest.get("batch_id") or "").strip()
    if not batch_id:
        raise ValueError(f"Spec manifest is missing batch_id: {source_spec}")

    specs = spec_manifest.get("specs")
    if not isinstance(specs, list) or not specs:
        raise ValueError(f"Spec manifest must contain a non-empty specs array: {source_spec}")

    style_agent_rel = specs[0].get("style_agent")
    style_agent_path = settings.repo_root / str(style_agent_rel or "")
    if not style_agent_rel or not style_agent_path.is_file():
        raise FileNotFoundError(f"Missing style agent: {style_agent_path}")

    style_canon = parse_style_agent(style_agent_path)
    prompt_entries = [
        build_prompt_entry(spec, style_canon=style_canon, asset_types=asset_types, settings=settings, args=args)
        for spec in specs
    ]

    manifest = {
        "schema_version": SCHEMA_VERSION,
        "batch_id": batch_id,
        "created_at_utc": timestamp_utc(),
        "source_spec": source_spec.relative_to(settings.tool_root).as_posix(),
        "prompts": prompt_entries,
    }

    return settings.prompts_dir / f"{batch_id}.json", manifest


def build_prompt_entry(
    spec: dict[str, Any],
    *,
    style_canon,
    asset_types: dict[str, Any],
    settings,
    args: argparse.Namespace,
) -> dict[str, Any]:
    """Build one prompt manifest entry from one spec."""
    if not isinstance(spec, dict):
        raise ValueError("Each spec entry must be a JSON object")

    asset_type = str(spec.get("asset_type") or "").strip()
    if asset_type not in asset_types:
        available = ", ".join(sorted(asset_types)) or "(none)"
        raise ValueError(f"Unknown asset type in spec {spec.get('spec_id')!r}: {asset_type!r}. Available: {available}")

    character_agent_rel = spec.get("character_agent")
    character_agent_path = settings.repo_root / str(character_agent_rel or "")
    if not character_agent_rel or not character_agent_path.is_file():
        raise FileNotFoundError(f"Missing character agent: {character_agent_path}")

    character_canon = parse_character_agent(character_agent_path)
    prompt_output = build_prompt(character_canon, style_canon, spec, asset_types[asset_type])

    generation_params = dict(prompt_output.generation_params)
    generation_params["batch_count"] = _resolve_batch_count(args, settings)
    generation_params.setdefault("seed", -1)

    return {
        "spec_id": spec.get("spec_id"),
        "character": spec.get("character"),
        "positive_prompt": prompt_output.positive_prompt,
        "negative_prompt": prompt_output.negative_prompt,
        "generation_params": generation_params,
        "canon_sources": prompt_output.canon_sources,
        "status": DEFAULT_STATUS,
    }


def _resolve_batch_count(args: argparse.Namespace, settings) -> int:
    if args.batch_count is not None:
        if args.batch_count <= 0:
            raise ValueError("--batch-count must be greater than 0")
        return args.batch_count
    return settings.default_batch_count


def main(argv: Sequence[str] | None = None) -> int:
    args = parse_args() if argv is None else parse_args_from(argv)

    try:
        output_path, manifest = build_manifest(args)
    except Exception as error:
        print(f"[prompt] Configuration error: {error}", file=sys.stderr)
        return 1

    if args.dry_run:
        print(json.dumps(manifest, indent=2, ensure_ascii=False))
        print(f"[prompt] Dry run complete. prompts={len(manifest['prompts'])}", file=sys.stderr)
        return 0

    try:
        settings = load_settings()
        settings.ensure_data_dirs()
        assert output_path is not None
        if output_path.exists():
            raise FileExistsError(
                f"Prompt manifest already exists: {output_path}. Re-run with a different batch id. Future --force support can overwrite."
            )
        write_manifest(output_path, manifest)
    except Exception as error:
        print(f"[prompt] Write error: {error}", file=sys.stderr)
        return 1

    print(f"[prompt] Wrote {output_path}")
    print(f"[prompt] Done. prompts={len(manifest['prompts'])}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
