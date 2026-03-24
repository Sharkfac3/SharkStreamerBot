#!/usr/bin/env python3
"""Transcribe stream recordings into SRT and JSON transcript files."""

from __future__ import annotations

import argparse
from dataclasses import replace
from pathlib import Path
import sys
from typing import Sequence

from config import Settings, find_recordings, load_settings
from lib.transcript_io import build_transcript_payload, segment_to_dict, write_srt, write_transcript_json


def build_parser() -> argparse.ArgumentParser:
    """Build the CLI parser."""
    parser = argparse.ArgumentParser(description="Transcribe stream recordings with faster-whisper")
    parser.add_argument(
        "inputs",
        nargs="*",
        help="Recording path(s), file name(s), or stems. If omitted, batch mode scans the recordings directory.",
    )
    parser.add_argument(
        "--recordings-dir",
        type=Path,
        help="Override recordings directory for batch mode or relative-name resolution.",
    )
    parser.add_argument(
        "--transcripts-dir",
        type=Path,
        help="Override transcript output directory.",
    )
    parser.add_argument("--model", help="Override Whisper model name.")
    parser.add_argument("--device", help="Override Whisper device (auto, cuda, cpu).")
    parser.add_argument("--compute-type", help="Override Whisper compute type (auto, float16, int8, etc.).")
    parser.add_argument("--language", help="Force a language code instead of auto-detect.")
    parser.add_argument("--beam-size", type=int, help="Override beam size.")
    parser.add_argument("--no-vad", action="store_true", help="Disable VAD filtering.")
    parser.add_argument("--no-word-timestamps", action="store_true", help="Disable word-level timestamps in JSON output.")
    parser.add_argument("--overwrite", action="store_true", help="Rebuild outputs even if they already exist.")
    parser.add_argument("--dry-run", action="store_true", help="Print planned work without running Whisper.")
    parser.add_argument("--limit", type=int, help="Only process the first N resolved recordings.")
    return parser


def parse_args() -> argparse.Namespace:
    """Parse command-line arguments."""
    return build_parser().parse_args()


def resolve_inputs(args: argparse.Namespace) -> tuple[Settings, list[Path]]:
    """Resolve input recordings using CLI args and config."""
    settings = load_settings()
    recordings_dir = (args.recordings_dir or settings.recordings_dir).resolve()

    if not recordings_dir.exists() and not args.inputs:
        raise FileNotFoundError(f"Recordings directory does not exist: {recordings_dir}")

    settings = replace(settings, recordings_dir=recordings_dir)

    if args.transcripts_dir:
        settings = replace(settings, transcripts_dir=args.transcripts_dir.resolve())

    if args.inputs:
        recordings = [resolve_recording_path(value, recordings_dir) for value in args.inputs]
    else:
        recordings = find_recordings(recordings_dir)

    if args.limit is not None:
        if args.limit <= 0:
            raise ValueError("--limit must be greater than 0")
        recordings = recordings[: args.limit]

    if not recordings:
        raise FileNotFoundError("No recordings matched the requested inputs.")

    return settings, recordings


def resolve_recording_path(value: str, recordings_dir: Path) -> Path:
    """Resolve a user-supplied recording reference into a real path."""
    candidate = Path(value).expanduser()

    possible_paths = []
    if candidate.is_absolute():
        possible_paths.append(candidate)
    else:
        possible_paths.append((Path.cwd() / candidate).resolve())
        possible_paths.append((recordings_dir / candidate).resolve())
        if candidate.suffix == "":
            possible_paths.append((recordings_dir / f"{candidate.name}.mp4").resolve())

    for path in possible_paths:
        if path.exists() and path.is_file():
            return path

    raise FileNotFoundError(
        f"Could not find recording: {value}. Expected an existing file path, a file in {recordings_dir}, or a .mp4 stem."
    )


def build_runtime_options(args: argparse.Namespace, settings: Settings) -> dict[str, object]:
    """Merge CLI overrides with config defaults."""
    whisper_device = args.device or settings.whisper_device
    whisper_compute_type = args.compute_type or settings.whisper_compute_type

    if whisper_device == "auto":
        whisper_device = "cuda"
    if whisper_compute_type == "auto":
        whisper_compute_type = "float16" if whisper_device == "cuda" else "int8"

    return {
        "model": args.model or settings.whisper_model,
        "device": whisper_device,
        "compute_type": whisper_compute_type,
        "language": args.language or settings.whisper_language,
        "beam_size": args.beam_size or settings.whisper_beam_size,
        "vad_filter": not args.no_vad if args.no_vad else settings.whisper_vad,
        "word_timestamps": False if args.no_word_timestamps else settings.word_timestamps,
    }


def output_paths(recording_path: Path, transcripts_dir: Path) -> tuple[Path, Path]:
    stem = recording_path.stem
    return transcripts_dir / f"{stem}.srt", transcripts_dir / f"{stem}.json"


def transcribe_file(recording_path: Path, srt_path: Path, json_path: Path, options: dict[str, object]) -> None:
    """Run faster-whisper for a single recording."""
    try:
        from faster_whisper import WhisperModel
    except ImportError as error:
        raise RuntimeError(
            "Missing dependency: faster-whisper is not installed. Run `pip install -r Tools/ContentPipeline/requirements.txt`."
        ) from error

    model = WhisperModel(
        str(options["model"]),
        device=str(options["device"]),
        compute_type=str(options["compute_type"]),
    )

    segments_iter, info = model.transcribe(
        str(recording_path),
        language=options["language"],
        beam_size=int(options["beam_size"]),
        vad_filter=bool(options["vad_filter"]),
        word_timestamps=bool(options["word_timestamps"]),
    )

    segments = [segment_to_dict(segment) for segment in segments_iter]
    payload = build_transcript_payload(
        source_path=recording_path,
        info=info,
        segments=segments,
        model_name=str(options["model"]),
        language=options["language"] if isinstance(options["language"], str) else None,
        word_timestamps=bool(options["word_timestamps"]),
        vad_enabled=bool(options["vad_filter"]),
    )

    write_srt(segments, srt_path)
    write_transcript_json(payload, json_path)


def main(argv: Sequence[str] | None = None) -> int:
    args = parse_args() if argv is None else parse_args_from(argv)

    try:
        settings, recordings = resolve_inputs(args)
        settings.ensure_data_dirs()
        options = build_runtime_options(args, settings)
    except Exception as error:
        print(f"[transcribe] Configuration error: {error}", file=sys.stderr)
        return 1

    print(f"[transcribe] Recordings directory: {settings.recordings_dir}")
    print(f"[transcribe] Transcript output:   {settings.transcripts_dir}")
    print(
        f"[transcribe] Whisper: model={options['model']} device={options['device']} "
        f"compute_type={options['compute_type']} vad={options['vad_filter']} "
        f"word_timestamps={options['word_timestamps']}"
    )

    processed = 0
    skipped = 0

    for recording_path in recordings:
        srt_path, json_path = output_paths(recording_path, settings.transcripts_dir)
        exists_already = srt_path.exists() and json_path.exists()

        if args.dry_run:
            action = "skip(existing)" if exists_already and not args.overwrite else "transcribe"
            print(f"[transcribe] {action}: {recording_path.name} -> {srt_path.name}, {json_path.name}")
            continue

        if exists_already and not args.overwrite:
            print(f"[transcribe] Skipping existing outputs for {recording_path.name}")
            skipped += 1
            continue

        print(f"[transcribe] Transcribing {recording_path.name}...")

        try:
            transcribe_file(recording_path, srt_path, json_path, options)
        except Exception as error:
            print(f"[transcribe] Failed for {recording_path.name}: {error}", file=sys.stderr)
            return 1

        processed += 1
        print(f"[transcribe] Wrote {srt_path.name} and {json_path.name}")

    print(f"[transcribe] Done. processed={processed} skipped={skipped} total={len(recordings)}")
    return 0


def parse_args_from(argv: Sequence[str]) -> argparse.Namespace:
    """Parse arguments from an explicit argv list for tests."""
    return build_parser().parse_args(list(argv))


if __name__ == "__main__":
    raise SystemExit(main())
