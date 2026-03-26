#!/usr/bin/env python3
"""Run the full content pipeline for a single recording."""

from __future__ import annotations

import argparse
from pathlib import Path
import shlex
import signal
import subprocess
import sys
from typing import Sequence

from config import _path_from_string, ensure_recordings_dir_exists, find_recordings, load_settings

REVIEW_HOST = "127.0.0.1"
REVIEW_PORT = 8000


def build_parser() -> argparse.ArgumentParser:
    """Build the CLI parser."""
    parser = argparse.ArgumentParser(description="Run all ContentPipeline phases for one recording")
    parser.add_argument(
        "recording",
        help="Recording name, stem, or absolute path. Non-absolute values resolve against the recordings directory.",
    )
    parser.add_argument("--overwrite", action="store_true", help="Rebuild outputs for phases that support overwrite.")
    parser.add_argument("--skip-review", action="store_true", help="Stop after Phase 4 and do not launch the review UI.")
    return parser


def parse_args() -> argparse.Namespace:
    """Parse CLI arguments."""
    return build_parser().parse_args()


def parse_args_from(argv: Sequence[str]) -> argparse.Namespace:
    """Parse arguments from an explicit argv list for tests."""
    return build_parser().parse_args(list(argv))


def resolve_recording(recording_value: str) -> Path:
    """Resolve one recording argument using ContentPipeline settings."""
    settings = load_settings()
    settings.ensure_data_dirs()

    candidate = _path_from_string(recording_value)
    if candidate.is_absolute():
        path = candidate.resolve()
        if path.exists() and path.is_file():
            return path
        raise FileNotFoundError(f"Recording not found: {recording_value}")

    recordings_dir = ensure_recordings_dir_exists(settings.recordings_dir)

    requested = Path(recording_value)
    if requested.suffix:
        direct_match = (recordings_dir / requested.name).resolve()
        if direct_match.exists() and direct_match.is_file():
            return direct_match

    requested_stem = requested.stem if requested.suffix else requested.name
    for path in find_recordings(recordings_dir):
        if path.stem == requested_stem:
            return path.resolve()

    raise FileNotFoundError(
        f"Could not resolve recording '{recording_value}' in {recordings_dir}. "
        "Use an absolute path or a recording stem/name from the recordings directory."
    )


def build_phase_command(script_name: str, *args: str | Path, overwrite: bool = False) -> list[str]:
    """Build a subprocess command for one pipeline phase."""
    command = [sys.executable, str(Path(__file__).resolve().parent / script_name)]
    command.extend(str(arg) for arg in args)
    if overwrite:
        command.append("--overwrite")
    return command


def run_phase(label: str, command: list[str]) -> None:
    """Run one phase and stop the pipeline on failure."""
    print(label)
    result = subprocess.run(command, capture_output=True, text=True)

    if result.stdout:
        print(result.stdout, end="" if result.stdout.endswith("\n") else "\n")

    if result.returncode == 0:
        return

    print(f"Command failed: {format_command(command)}", file=sys.stderr)
    if result.stderr:
        print(result.stderr, end="" if result.stderr.endswith("\n") else "\n", file=sys.stderr)
    else:
        print("<no stderr output>", file=sys.stderr)
    raise SystemExit(result.returncode)


def format_command(command: Sequence[str]) -> str:
    """Return a display-safe shell command."""
    return " ".join(shlex.quote(part) for part in command)


def count_review_items(review_queue_dir: Path) -> int:
    """Count queued review metadata files."""
    if not review_queue_dir.exists():
        return 0
    return sum(1 for path in review_queue_dir.glob("*_meta.json") if path.is_file())


def run_review_server() -> int:
    """Launch the review UI and wait until the operator stops it."""
    command = [
        sys.executable,
        str(Path(__file__).resolve().parent / "review_server.py"),
        "--host",
        REVIEW_HOST,
        "--port",
        str(REVIEW_PORT),
    ]

    process = subprocess.Popen(command)
    print(f"[Phase 5/5] Review UI ready → http://localhost:{REVIEW_PORT}")
    print("Press Ctrl+C to stop the review server.")

    try:
        return process.wait()
    except KeyboardInterrupt:
        try:
            process.send_signal(signal.SIGINT)
        except Exception:
            process.terminate()
        return process.wait()


def main(argv: Sequence[str] | None = None) -> int:
    args = parse_args() if argv is None else parse_args_from(argv)

    try:
        recording_path = resolve_recording(args.recording)
        settings = load_settings()
        settings.ensure_data_dirs()
    except Exception as error:
        print(f"[pipeline] Configuration error: {error}", file=sys.stderr)
        return 1

    recording_stem = recording_path.stem

    phase_commands = [
        ("[Phase 1/5] Transcribing...", build_phase_command("transcribe.py", recording_path, overwrite=args.overwrite)),
        (
            "[Phase 2/5] Detecting highlights...",
            build_phase_command("detect_highlights.py", recording_stem, overwrite=args.overwrite),
        ),
        (
            "[Phase 3/5] Extracting clips...",
            build_phase_command("extract_clips.py", recording_stem, overwrite=args.overwrite),
        ),
        (
            "[Phase 4/5] Formatting for review...",
            build_phase_command("format_instagram.py", recording_stem, overwrite=args.overwrite),
        ),
    ]

    for label, command in phase_commands:
        run_phase(label, command)
        if command and Path(command[1]).name == "detect_highlights.py":
            print("[pipeline] Phase 2 finished. Ollama model unload was requested to free VRAM.")

    clip_count = count_review_items(settings.review_queue_dir)
    print(f"Phase 4 complete — {clip_count} clips ready for review.")

    if args.skip_review:
        print("Review session ended. Run: python feedback.py sync")
        print("to feed results back into highlight detection.")
        return 0

    review_exit_code = run_review_server()
    print("Review session ended. Run: python feedback.py sync")
    print("to feed results back into highlight detection.")
    return review_exit_code


if __name__ == "__main__":
    raise SystemExit(main())
