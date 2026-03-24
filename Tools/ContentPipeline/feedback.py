#!/usr/bin/env python3
"""Track published clip analytics in SQLite and generate prompt-ready feedback insights."""

from __future__ import annotations

import argparse
import json
from pathlib import Path
import sys
from typing import Sequence

from config import load_settings
from lib.feedback_store import (
    build_feedback_summary,
    build_template_csv,
    connect_database,
    import_metrics_csv,
    initialize_database,
    sync_published_metadata,
    write_summary_files,
)

DEFAULT_PLATFORM = "instagram"


def build_parser() -> argparse.ArgumentParser:
    """Build the CLI parser."""
    parser = argparse.ArgumentParser(description="ContentPipeline analytics feedback loop")
    subparsers = parser.add_subparsers(dest="command", required=True)

    sync_parser = subparsers.add_parser("sync", help="Index published clip metadata into SQLite and rebuild summary files.")
    add_shared_arguments(sync_parser)

    import_parser = subparsers.add_parser(
        "import-csv",
        help="Import an analytics CSV and rebuild summary files. Rows should include clip_id/video file name or exact caption.",
    )
    add_shared_arguments(import_parser)
    import_parser.add_argument("csv_paths", nargs="+", type=Path, help="Analytics CSV file(s) to import.")
    import_parser.add_argument("--platform", default=DEFAULT_PLATFORM, help=f"Platform label (default: {DEFAULT_PLATFORM})")

    report_parser = subparsers.add_parser("report", help="Print the current feedback summary.")
    add_shared_arguments(report_parser)
    report_parser.add_argument("--json", action="store_true", help="Print raw JSON instead of a human summary.")

    template_parser = subparsers.add_parser(
        "template",
        help="Export a merge-friendly CSV template listing published clips and expected metrics columns.",
    )
    add_shared_arguments(template_parser)
    template_parser.add_argument("--out", type=Path, help="Override output CSV path.")

    return parser


def add_shared_arguments(parser: argparse.ArgumentParser) -> None:
    """Add shared path overrides to a sub-command parser."""
    parser.add_argument("--published-dir", type=Path, help="Override published metadata directory.")
    parser.add_argument("--db", type=Path, help="Override SQLite database path.")
    parser.add_argument("--summary-json", type=Path, help="Override machine-readable summary output path.")
    parser.add_argument("--prompt-context", type=Path, help="Override prompt-context text output path.")


def parse_args() -> argparse.Namespace:
    """Parse CLI arguments."""
    return build_parser().parse_args()


def parse_args_from(argv: Sequence[str]) -> argparse.Namespace:
    """Parse arguments from an explicit argv list for tests."""
    return build_parser().parse_args(list(argv))


def resolve_paths(args: argparse.Namespace) -> dict[str, Path]:
    """Resolve settings-backed paths with CLI overrides."""
    settings = load_settings()
    settings.ensure_data_dirs()
    return {
        "published_dir": (args.published_dir or settings.published_dir).resolve(),
        "db_path": (args.db or settings.feedback_db_path).resolve(),
        "summary_json": (args.summary_json or settings.feedback_summary_path).resolve(),
        "prompt_context": (args.prompt_context or settings.feedback_prompt_path).resolve(),
        "template_csv": (settings.feedback_dir / "metrics_template.csv").resolve(),
    }


def ensure_database(db_path: Path):
    """Open and initialize the SQLite database."""
    connection = connect_database(db_path)
    initialize_database(connection)
    return connection


def refresh_summary(connection, *, summary_json: Path, prompt_context: Path) -> dict[str, object]:
    """Rebuild both persisted summary files and return the summary payload."""
    summary = build_feedback_summary(connection)
    write_summary_files(summary, json_path=summary_json, prompt_path=prompt_context)
    return summary


def handle_sync(args: argparse.Namespace) -> int:
    paths = resolve_paths(args)
    published_dir = paths["published_dir"]
    if not published_dir.exists():
        print(f"[feedback] Published directory does not exist: {published_dir}", file=sys.stderr)
        return 1

    with ensure_database(paths["db_path"]) as connection:
        indexed = sync_published_metadata(connection, published_dir)
        summary = refresh_summary(
            connection,
            summary_json=paths["summary_json"],
            prompt_context=paths["prompt_context"],
        )

    print(f"[feedback] Indexed {indexed} published clip metadata file(s) from {published_dir}")
    print(f"[feedback] Database:       {paths['db_path']}")
    print(f"[feedback] Summary JSON:   {paths['summary_json']}")
    print(f"[feedback] Prompt context: {paths['prompt_context']}")
    print(f"[feedback] Clips tracked:  {summary.get('clip_count', 0)}")
    print(f"[feedback] With metrics:   {summary.get('metric_clip_count', 0)}")
    return 0


def handle_import_csv(args: argparse.Namespace) -> int:
    paths = resolve_paths(args)
    csv_paths = [path.resolve() for path in args.csv_paths]
    missing = [path for path in csv_paths if not path.exists()]
    if missing:
        print(f"[feedback] CSV file not found: {missing[0]}", file=sys.stderr)
        return 1

    with ensure_database(paths["db_path"]) as connection:
        sync_published_metadata(connection, paths["published_dir"])

        overall_imported = 0
        overall_skipped = 0
        for csv_path in csv_paths:
            try:
                result = import_metrics_csv(connection, csv_path, platform=args.platform)
            except Exception as error:
                print(f"[feedback] Failed to import {csv_path}: {error}", file=sys.stderr)
                return 1

            overall_imported += int(result["imported"])
            overall_skipped += int(result["skipped"])
            print(
                f"[feedback] Imported {result['imported']} row(s) from {csv_path} "
                f"(skipped={result['skipped']}, import_id={result['import_id']})"
            )
            for failure in result["failures"][:10]:
                print(f"[feedback]   note: {failure}")
            if len(result["failures"]) > 10:
                print(f"[feedback]   note: ... {len(result['failures']) - 10} more skipped row(s)")

        summary = refresh_summary(
            connection,
            summary_json=paths["summary_json"],
            prompt_context=paths["prompt_context"],
        )

    print(f"[feedback] Total imported rows: {overall_imported}")
    print(f"[feedback] Total skipped rows:  {overall_skipped}")
    print(f"[feedback] Clips with metrics:  {summary.get('metric_clip_count', 0)}")
    return 0


def handle_report(args: argparse.Namespace) -> int:
    paths = resolve_paths(args)
    with ensure_database(paths["db_path"]) as connection:
        sync_published_metadata(connection, paths["published_dir"])
        summary = refresh_summary(
            connection,
            summary_json=paths["summary_json"],
            prompt_context=paths["prompt_context"],
        )

    if args.json:
        print(json.dumps(summary, indent=2, ensure_ascii=False))
        return 0

    print("[feedback] Content pipeline performance summary")
    print(f"[feedback] Clips tracked:     {summary.get('clip_count', 0)}")
    print(f"[feedback] Clips w/ metrics:  {summary.get('metric_clip_count', 0)}")
    print(f"[feedback] CSV imports:       {summary.get('import_count', 0)}")

    top_categories = summary.get("top_categories") or []
    if top_categories:
        print("[feedback] Top categories:")
        for entry in top_categories[:5]:
            print(
                f"[feedback]   - {entry['category']}: clips={entry['clip_count']} avg_score={entry['avg_score']:.2f}"
            )

    top_durations = summary.get("top_duration_buckets") or []
    if top_durations:
        print("[feedback] Top duration buckets:")
        for entry in top_durations[:5]:
            print(
                f"[feedback]   - {entry['bucket']}s: clips={entry['clip_count']} avg_score={entry['avg_score']:.2f}"
            )

    lessons = summary.get("lessons") or []
    if lessons:
        print("[feedback] Lessons:")
        for lesson in lessons[:5]:
            print(f"[feedback]   - {lesson}")

    print(f"[feedback] Prompt context: {paths['prompt_context']}")
    return 0


def handle_template(args: argparse.Namespace) -> int:
    paths = resolve_paths(args)
    output_path = (args.out or paths["template_csv"]).resolve()

    with ensure_database(paths["db_path"]) as connection:
        sync_published_metadata(connection, paths["published_dir"])
        count = build_template_csv(connection, output_path)
        refresh_summary(
            connection,
            summary_json=paths["summary_json"],
            prompt_context=paths["prompt_context"],
        )

    print(f"[feedback] Wrote metrics template for {count} clip(s): {output_path}")
    return 0


def main(argv: Sequence[str] | None = None) -> int:
    args = parse_args() if argv is None else parse_args_from(argv)

    try:
        if args.command == "sync":
            return handle_sync(args)
        if args.command == "import-csv":
            return handle_import_csv(args)
        if args.command == "report":
            return handle_report(args)
        if args.command == "template":
            return handle_template(args)
    except KeyboardInterrupt:
        print("[feedback] Cancelled.", file=sys.stderr)
        return 130
    except Exception as error:
        print(f"[feedback] Unexpected failure: {error}", file=sys.stderr)
        return 1

    print(f"[feedback] Unsupported command: {args.command}", file=sys.stderr)
    return 1


if __name__ == "__main__":
    raise SystemExit(main())
