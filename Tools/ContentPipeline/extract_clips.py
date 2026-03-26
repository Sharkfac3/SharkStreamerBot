#!/usr/bin/env python3
"""Extract vertical-ready clips from highlight manifests using FFmpeg."""

from __future__ import annotations

import argparse
from dataclasses import replace
from pathlib import Path
import sys
from typing import Any, Sequence

from config import Settings, ensure_recordings_dir_exists, load_settings
from lib.clip_manifest import (
    build_clip_manifest,
    clip_manifest_name,
    clip_output_name,
    derive_clip_prefix,
    load_highlight_manifest,
    write_clip_manifest,
)
from lib.ffmpeg_utils import (
    build_video_filter,
    extract_duration_seconds,
    extract_video_stream_info,
    is_vertical_video,
    probe_media,
    run_ffmpeg,
    tool_path_arg,
)


def build_parser() -> argparse.ArgumentParser:
    """Build the CLI parser."""
    parser = argparse.ArgumentParser(description="Extract short-form clips from highlight manifests with FFmpeg")
    parser.add_argument(
        "inputs",
        nargs="*",
        help="Highlight manifest path(s), file name(s), or stems. If omitted, batch mode scans the highlights directory.",
    )
    parser.add_argument("--recordings-dir", type=Path, help="Override recordings directory.")
    parser.add_argument("--highlights-dir", type=Path, help="Override highlight manifest directory.")
    parser.add_argument("--clips-dir", type=Path, help="Override clip output directory.")
    parser.add_argument("--ffmpeg-path", type=Path, help="Override ffmpeg executable path.")
    parser.add_argument("--ffprobe-path", type=Path, help="Override ffprobe executable path.")
    parser.add_argument("--pre-pad", type=float, help="Seconds to include before each highlight.")
    parser.add_argument("--post-pad", type=float, help="Seconds to include after each highlight.")
    parser.add_argument("--overwrite", action="store_true", help="Rebuild outputs even if they already exist.")
    parser.add_argument("--dry-run", action="store_true", help="Print planned work without running ffmpeg.")
    parser.add_argument("--limit", type=int, help="Only process the first N resolved highlight manifests.")
    return parser


def parse_args() -> argparse.Namespace:
    """Parse command-line arguments."""
    return build_parser().parse_args()


def parse_args_from(argv: Sequence[str]) -> argparse.Namespace:
    """Parse arguments from an explicit argv list for tests."""
    return build_parser().parse_args(list(argv))


def resolve_inputs(args: argparse.Namespace) -> tuple[Settings, list[Path]]:
    """Resolve input highlight manifests using CLI args and config."""
    settings = load_settings()
    recordings_dir = ensure_recordings_dir_exists((args.recordings_dir or settings.recordings_dir).resolve())
    highlights_dir = (args.highlights_dir or settings.highlights_dir).resolve()
    clips_dir = (args.clips_dir or settings.clips_dir).resolve()

    if not highlights_dir.exists() and not args.inputs:
        raise FileNotFoundError(f"Highlights directory does not exist: {highlights_dir}")

    settings = replace(
        settings,
        recordings_dir=recordings_dir,
        highlights_dir=highlights_dir,
        clips_dir=clips_dir,
    )

    if args.inputs:
        manifests = [resolve_highlight_manifest_path(value, highlights_dir) for value in args.inputs]
    else:
        manifests = find_highlight_manifests(highlights_dir)

    if args.limit is not None:
        if args.limit <= 0:
            raise ValueError("--limit must be greater than 0")
        manifests = manifests[: args.limit]

    if not manifests:
        raise FileNotFoundError("No highlight manifests matched the requested inputs.")

    return settings, manifests


def resolve_highlight_manifest_path(value: str, highlights_dir: Path) -> Path:
    """Resolve a user-supplied highlight manifest reference into a JSON file path."""
    candidate = Path(value).expanduser()

    possible_paths: list[Path] = []
    if candidate.is_absolute():
        possible_paths.append(candidate)
    else:
        possible_paths.append((Path.cwd() / candidate).resolve())
        possible_paths.append((highlights_dir / candidate).resolve())
        if candidate.suffix == "":
            possible_paths.append((highlights_dir / f"{candidate.name}.json").resolve())

    for path in possible_paths:
        if path.exists() and path.is_file() and path.suffix.lower() == ".json":
            return path

    raise FileNotFoundError(
        f"Could not find highlight manifest: {value}. Expected an existing .json file path, a file in {highlights_dir}, or a manifest stem."
    )


def find_highlight_manifests(directory: Path) -> list[Path]:
    """Return sorted highlight manifests."""
    return [path for path in sorted(directory.iterdir()) if path.is_file() and path.suffix.lower() == ".json"]


def build_runtime_options(args: argparse.Namespace, settings: Settings) -> dict[str, Any]:
    """Merge CLI overrides with config defaults."""
    ffmpeg_path = (args.ffmpeg_path or settings.ffmpeg_path)
    ffprobe_path = (args.ffprobe_path or settings.ffprobe_path)
    pre_pad = settings.clip_pre_pad_seconds if args.pre_pad is None else args.pre_pad
    post_pad = settings.clip_post_pad_seconds if args.post_pad is None else args.post_pad

    if ffmpeg_path is None:
        raise FileNotFoundError(
            "Could not find ffmpeg. Install it or set CONTENT_PIPELINE_FFMPEG_PATH in Tools/ContentPipeline/.env."
        )
    if ffprobe_path is None:
        raise FileNotFoundError(
            "Could not find ffprobe. Install it or set CONTENT_PIPELINE_FFPROBE_PATH in Tools/ContentPipeline/.env."
        )
    if pre_pad < 0:
        raise ValueError("--pre-pad must be 0 or greater")
    if post_pad < 0:
        raise ValueError("--post-pad must be 0 or greater")
    if settings.clip_width <= 0 or settings.clip_height <= 0:
        raise ValueError("Clip dimensions must be greater than 0")
    if settings.clip_fps <= 0:
        raise ValueError("Clip FPS must be greater than 0")

    return {
        "ffmpeg_path": ffmpeg_path.resolve(),
        "ffprobe_path": ffprobe_path.resolve(),
        "pre_pad": float(pre_pad),
        "post_pad": float(post_pad),
        "width": settings.clip_width,
        "height": settings.clip_height,
        "fps": settings.clip_fps,
        "video_bitrate": settings.clip_video_bitrate,
        "video_codec": settings.clip_video_codec,
        "audio_codec": settings.clip_audio_codec,
        "audio_bitrate": settings.clip_audio_bitrate,
        "audio_sample_rate": settings.clip_audio_sample_rate,
    }


def process_manifest(
    manifest_path: Path,
    *,
    settings: Settings,
    options: dict[str, Any],
    overwrite: bool,
    dry_run: bool,
) -> tuple[int, int]:
    """Extract clips for a single highlight manifest and persist a clip manifest."""
    highlight_manifest = load_highlight_manifest(manifest_path)
    source_recording_path, source_variant = resolve_source_recording(highlight_manifest, manifest_path, settings.recordings_dir)

    probe_payload = probe_media(Path(options["ffprobe_path"]), source_recording_path)
    video_info = extract_video_stream_info(probe_payload)
    duration_seconds = extract_duration_seconds(probe_payload)
    source_is_vertical = is_vertical_video(video_info)
    filter_graph = build_video_filter(
        source_is_vertical=source_is_vertical,
        width=int(options["width"]),
        height=int(options["height"]),
        fps=int(options["fps"]),
    )

    highlights = collect_highlights(highlight_manifest)
    source_recording = highlight_manifest.get("source_recording") if isinstance(highlight_manifest.get("source_recording"), dict) else {}
    clip_prefix = derive_clip_prefix(source_recording.get("file_name"), manifest_path.stem)

    produced = 0
    skipped = 0
    clip_entries: list[dict[str, Any]] = []

    for index, highlight in enumerate(highlights, start=1):
        output_file = settings.clips_dir / clip_output_name(clip_prefix, index, str(highlight["category"]))
        clip_start = max(0.0, float(highlight["start_time"]) - float(options["pre_pad"]))
        clip_end = float(highlight["end_time"]) + float(options["post_pad"])
        if duration_seconds is not None:
            clip_end = min(clip_end, duration_seconds)

        clip_duration = max(0.0, clip_end - clip_start)
        if clip_duration <= 0.05:
            skipped += 1
            continue

        exists_already = output_file.exists()
        clip_entry = {
            "index": index,
            "output_file_name": output_file.name,
            "output_path": str(output_file),
            "category": highlight["category"],
            "description": highlight["description"],
            "suggested_caption": highlight["suggested_caption"],
            "confidence_score": highlight["confidence_score"],
            "highlight_start_time": round(float(highlight["start_time"]), 3),
            "highlight_end_time": round(float(highlight["end_time"]), 3),
            "clip_start_time": round(clip_start, 3),
            "clip_end_time": round(clip_end, 3),
            "clip_duration_seconds": round(clip_duration, 3),
            "source_variant": source_variant,
            "source_video_is_vertical": source_is_vertical,
            "status": "planned" if dry_run else ("skipped_existing" if exists_already and not overwrite else "created"),
        }
        clip_entries.append(clip_entry)

        if dry_run:
            continue

        if exists_already and not overwrite:
            skipped += 1
            continue

        command = build_ffmpeg_command(
            ffmpeg_path=Path(options["ffmpeg_path"]),
            input_path=source_recording_path,
            output_path=output_file,
            clip_start=clip_start,
            clip_end=clip_end,
            filter_graph=filter_graph,
            options=options,
            overwrite=overwrite,
        )

        try:
            run_ffmpeg(Path(options["ffmpeg_path"]), command)
        except Exception as error:
            raise RuntimeError(f"FFmpeg failed for {output_file.name}: {error}") from error

        produced += 1

    manifest_output_path = settings.clips_dir / clip_manifest_name(clip_prefix)
    manifest_payload = build_clip_manifest(
        highlight_manifest_path=manifest_path,
        highlight_manifest=highlight_manifest,
        source_recording_path=source_recording_path,
        source_variant=source_variant,
        source_duration_seconds=duration_seconds,
        source_video_info=video_info,
        ffmpeg_path=Path(options["ffmpeg_path"]),
        ffprobe_path=Path(options["ffprobe_path"]),
        clip_count=len(clip_entries),
        clips=clip_entries,
    )
    write_clip_manifest(manifest_payload, manifest_output_path)
    return produced, skipped


def resolve_source_recording(highlight_manifest: dict[str, Any], manifest_path: Path, recordings_dir: Path) -> tuple[Path, str]:
    """Resolve the preferred recording file for a highlight manifest."""
    source_recording = highlight_manifest.get("source_recording")
    candidate_paths: list[Path] = []

    if isinstance(source_recording, dict):
        path_value = source_recording.get("path")
        file_name = source_recording.get("file_name")

        if isinstance(path_value, str) and path_value.strip():
            candidate_paths.append(Path(path_value).expanduser())
        if isinstance(file_name, str) and file_name.strip():
            candidate_paths.append((recordings_dir / file_name).resolve())
            if file_name.lower().endswith(".mp4"):
                base = Path(file_name)
                if not base.stem.lower().endswith("-vertical"):
                    candidate_paths.append((recordings_dir / f"{base.stem}-vertical.mp4").resolve())

    candidate_paths.append((recordings_dir / f"{manifest_path.stem}.mp4").resolve())
    candidate_paths.append((recordings_dir / f"{manifest_path.stem}-vertical.mp4").resolve())

    for path in candidate_paths:
        if path.exists() and path.is_file():
            variant = "vertical" if path.stem.lower().endswith("-vertical") else "horizontal"
            return path.resolve(), variant

    source_name = None
    if isinstance(source_recording, dict):
        source_name = source_recording.get("file_name") or source_recording.get("path")

    raise FileNotFoundError(
        "Could not resolve a source recording for "
        f"{manifest_path.name}. Checked recordings dir {recordings_dir} using source reference {source_name!r}."
    )


def collect_highlights(highlight_manifest: dict[str, Any]) -> list[dict[str, Any]]:
    """Normalize and sort highlights from a manifest."""
    raw_highlights = highlight_manifest.get("highlights")
    if not isinstance(raw_highlights, list):
        raise ValueError("Highlight manifest is missing a 'highlights' list")

    highlights: list[dict[str, Any]] = []
    for item in raw_highlights:
        if not isinstance(item, dict):
            continue

        start_time = _as_float(item.get("start_time"))
        end_time = _as_float(item.get("end_time"))
        if start_time is None or end_time is None or end_time <= start_time:
            continue

        category = str(item.get("category") or "clip").strip() or "clip"
        description = str(item.get("description") or "").strip()
        suggested_caption = str(item.get("suggested_caption") or "").strip()
        confidence_score = _as_float(item.get("confidence_score")) or 0.0
        rank = _as_int(item.get("rank"))

        highlights.append(
            {
                "start_time": start_time,
                "end_time": end_time,
                "category": category,
                "description": description,
                "suggested_caption": suggested_caption,
                "confidence_score": round(confidence_score, 3),
                "rank": rank,
            }
        )

    highlights.sort(key=lambda item: (item["rank"] is None, item["rank"] or 0, item["start_time"]))
    return highlights


def build_ffmpeg_command(
    *,
    ffmpeg_path: Path,
    input_path: Path,
    output_path: Path,
    clip_start: float,
    clip_end: float,
    filter_graph: str,
    options: dict[str, Any],
    overwrite: bool,
) -> list[str]:
    """Build the ffmpeg argument list for one clip."""
    output_path.parent.mkdir(parents=True, exist_ok=True)
    duration = max(0.001, clip_end - clip_start)

    return [
        "-y" if overwrite else "-n",
        "-hide_banner",
        "-loglevel",
        "error",
        "-i",
        tool_path_arg(ffmpeg_path, input_path),
        "-ss",
        format_seconds(clip_start),
        "-t",
        format_seconds(duration),
        "-vf",
        filter_graph,
        "-c:v",
        str(options["video_codec"]),
        "-b:v",
        str(options["video_bitrate"]),
        "-c:a",
        str(options["audio_codec"]),
        "-b:a",
        str(options["audio_bitrate"]),
        "-ar",
        str(options["audio_sample_rate"]),
        "-ac",
        "2",
        "-movflags",
        "+faststart",
        tool_path_arg(ffmpeg_path, output_path),
    ]


def format_seconds(value: float) -> str:
    """Format seconds for ffmpeg arguments with millisecond precision."""
    return f"{max(0.0, value):.3f}"


def _as_float(value: Any) -> float | None:
    if value is None:
        return None
    if isinstance(value, (int, float)):
        return float(value)
    try:
        return float(str(value).strip())
    except ValueError:
        return None


def _as_int(value: Any) -> int | None:
    if value is None:
        return None
    if isinstance(value, int):
        return value
    try:
        return int(str(value).strip())
    except ValueError:
        return None


def main(argv: Sequence[str] | None = None) -> int:
    args = parse_args() if argv is None else parse_args_from(argv)

    try:
        settings, manifests = resolve_inputs(args)
        settings.ensure_data_dirs()
        options = build_runtime_options(args, settings)
    except Exception as error:
        print(f"[clips] Configuration error: {error}", file=sys.stderr)
        return 1

    print(f"[clips] Highlights input: {settings.highlights_dir}")
    print(f"[clips] Recordings dir:  {settings.recordings_dir}")
    print(f"[clips] Clip output:     {settings.clips_dir}")
    print(
        f"[clips] FFmpeg: ffmpeg={options['ffmpeg_path']} ffprobe={options['ffprobe_path']} "
        f"pre_pad={options['pre_pad']}s post_pad={options['post_pad']}s "
        f"size={options['width']}x{options['height']} fps={options['fps']} bitrate={options['video_bitrate']}"
    )

    produced = 0
    skipped = 0

    for manifest_path in manifests:
        if args.dry_run:
            print(f"[clips] plan: {manifest_path.name}")
        else:
            print(f"[clips] Extracting clips for {manifest_path.name}...")

        try:
            manifest_produced, manifest_skipped = process_manifest(
                manifest_path,
                settings=settings,
                options=options,
                overwrite=args.overwrite,
                dry_run=args.dry_run,
            )
        except Exception as error:
            print(f"[clips] Failed for {manifest_path.name}: {error}", file=sys.stderr)
            return 1

        produced += manifest_produced
        skipped += manifest_skipped
        print(
            f"[clips] Finished {manifest_path.name}: produced={manifest_produced} skipped={manifest_skipped}"
        )

    print(f"[clips] Done. produced={produced} skipped={skipped} manifests={len(manifests)}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
