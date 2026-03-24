#!/usr/bin/env python3
"""Format extracted clips for Instagram review with burned-in subtitles."""

from __future__ import annotations

import argparse
from dataclasses import replace
from datetime import UTC, datetime
import json
from pathlib import Path
import sys
import tempfile
import textwrap
from typing import Any, Sequence

from config import Settings, load_settings
from lib.clip_manifest import load_clip_manifest, review_metadata_name, review_output_name
from lib.ffmpeg_utils import (
    build_ass_subtitle_filter,
    extract_audio_stream_info,
    extract_duration_seconds,
    extract_video_stream_info,
    probe_media,
    run_ffmpeg,
    tool_path_arg,
)
from lib.transcript_io import collect_segments_in_range, find_transcript_path, load_transcript_json


INSTAGRAM_ASPECT_RATIO = "9:16"


def build_parser() -> argparse.ArgumentParser:
    """Build the CLI parser."""
    parser = argparse.ArgumentParser(description="Burn subtitles and prepare Instagram review clips")
    parser.add_argument(
        "inputs",
        nargs="*",
        help="Clip manifest path(s), file name(s), or stems. If omitted, batch mode scans the clips directory for manifest JSON files.",
    )
    parser.add_argument("--clips-dir", type=Path, help="Override clip manifest/input directory.")
    parser.add_argument("--transcripts-dir", type=Path, help="Override transcript directory.")
    parser.add_argument("--review-queue-dir", type=Path, help="Override review queue output directory.")
    parser.add_argument("--ffmpeg-path", type=Path, help="Override ffmpeg executable path.")
    parser.add_argument("--ffprobe-path", type=Path, help="Override ffprobe executable path.")
    parser.add_argument("--overwrite", action="store_true", help="Rebuild outputs even if they already exist.")
    parser.add_argument("--dry-run", action="store_true", help="Print planned work without running ffmpeg.")
    parser.add_argument("--limit", type=int, help="Only process the first N resolved clip manifests.")
    return parser


def parse_args() -> argparse.Namespace:
    """Parse command-line arguments."""
    return build_parser().parse_args()


def parse_args_from(argv: Sequence[str]) -> argparse.Namespace:
    """Parse arguments from an explicit argv list for tests."""
    return build_parser().parse_args(list(argv))


def resolve_inputs(args: argparse.Namespace) -> tuple[Settings, list[Path]]:
    """Resolve clip manifests using CLI args and config."""
    settings = load_settings()
    clips_dir = (args.clips_dir or settings.clips_dir).resolve()
    transcripts_dir = (args.transcripts_dir or settings.transcripts_dir).resolve()
    review_queue_dir = (args.review_queue_dir or settings.review_queue_dir).resolve()

    if not clips_dir.exists() and not args.inputs:
        raise FileNotFoundError(f"Clips directory does not exist: {clips_dir}")

    settings = replace(
        settings,
        clips_dir=clips_dir,
        transcripts_dir=transcripts_dir,
        review_queue_dir=review_queue_dir,
    )

    if args.inputs:
        manifests = [resolve_clip_manifest_path(value, clips_dir) for value in args.inputs]
    else:
        manifests = find_clip_manifests(clips_dir)

    if args.limit is not None:
        if args.limit <= 0:
            raise ValueError("--limit must be greater than 0")
        manifests = manifests[: args.limit]

    if not manifests:
        raise FileNotFoundError("No clip manifests matched the requested inputs.")

    return settings, manifests


def resolve_clip_manifest_path(value: str, clips_dir: Path) -> Path:
    """Resolve a user-supplied clip manifest reference into a JSON file path."""
    candidate = Path(value).expanduser()

    possible_paths: list[Path] = []
    if candidate.is_absolute():
        possible_paths.append(candidate)
    else:
        possible_paths.append((Path.cwd() / candidate).resolve())
        possible_paths.append((clips_dir / candidate).resolve())
        if candidate.suffix == "":
            possible_paths.append((clips_dir / f"{candidate.name}.json").resolve())

    for path in possible_paths:
        if path.exists() and path.is_file() and path.suffix.lower() == ".json":
            return path

    raise FileNotFoundError(
        f"Could not find clip manifest: {value}. Expected an existing .json file path, a file in {clips_dir}, or a manifest stem."
    )


def find_clip_manifests(directory: Path) -> list[Path]:
    """Return sorted clip manifest JSON files from the clips directory."""
    manifests: list[Path] = []
    for path in sorted(directory.iterdir()):
        if not path.is_file() or path.suffix.lower() != ".json":
            continue
        if path.name.lower().endswith("_meta.json"):
            continue
        manifests.append(path)
    return manifests


def build_runtime_options(args: argparse.Namespace, settings: Settings) -> dict[str, Any]:
    """Merge CLI overrides with config defaults."""
    ffmpeg_path = args.ffmpeg_path or settings.ffmpeg_path
    ffprobe_path = args.ffprobe_path or settings.ffprobe_path

    if ffmpeg_path is None:
        raise FileNotFoundError(
            "Could not find ffmpeg. Install it or set CONTENT_PIPELINE_FFMPEG_PATH in Tools/ContentPipeline/.env."
        )
    if ffprobe_path is None:
        raise FileNotFoundError(
            "Could not find ffprobe. Install it or set CONTENT_PIPELINE_FFPROBE_PATH in Tools/ContentPipeline/.env."
        )
    if settings.review_max_duration_seconds <= 0:
        raise ValueError("CONTENT_PIPELINE_REVIEW_MAX_DURATION_SECONDS must be greater than 0")
    if settings.review_max_file_size_mb <= 0:
        raise ValueError("CONTENT_PIPELINE_REVIEW_MAX_FILE_SIZE_MB must be greater than 0")
    if settings.review_subtitle_font_size <= 0:
        raise ValueError("CONTENT_PIPELINE_REVIEW_SUBTITLE_FONT_SIZE must be greater than 0")
    if settings.review_subtitle_margin_v < 0:
        raise ValueError("CONTENT_PIPELINE_REVIEW_SUBTITLE_MARGIN_V must be 0 or greater")
    if settings.review_subtitle_line_width <= 0:
        raise ValueError("CONTENT_PIPELINE_REVIEW_SUBTITLE_LINE_WIDTH must be greater than 0")
    if settings.review_subtitle_max_lines <= 0:
        raise ValueError("CONTENT_PIPELINE_REVIEW_SUBTITLE_MAX_LINES must be greater than 0")

    return {
        "ffmpeg_path": ffmpeg_path.resolve(),
        "ffprobe_path": ffprobe_path.resolve(),
        "width": settings.clip_width,
        "height": settings.clip_height,
        "fps": settings.clip_fps,
        "video_codec": settings.clip_video_codec,
        "video_bitrate": settings.clip_video_bitrate,
        "audio_codec": settings.clip_audio_codec,
        "audio_bitrate": settings.clip_audio_bitrate,
        "audio_sample_rate": settings.clip_audio_sample_rate,
        "max_duration_seconds": settings.review_max_duration_seconds,
        "max_file_size_bytes": settings.review_max_file_size_mb * 1024 * 1024,
        "font_name": settings.review_subtitle_font_name,
        "font_size": settings.review_subtitle_font_size,
        "outline": settings.review_subtitle_outline,
        "margin_v": settings.review_subtitle_margin_v,
        "line_width": settings.review_subtitle_line_width,
        "max_lines": settings.review_subtitle_max_lines,
    }


def process_manifest(
    manifest_path: Path,
    *,
    settings: Settings,
    options: dict[str, Any],
    overwrite: bool,
    dry_run: bool,
) -> tuple[int, int]:
    """Format every clip in a manifest for Instagram review."""
    clip_manifest = load_clip_manifest(manifest_path)
    source_recording = clip_manifest.get("source_recording") if isinstance(clip_manifest.get("source_recording"), dict) else {}
    transcript_path = find_transcript_path(
        settings.transcripts_dir,
        source_recording.get("file_name") if isinstance(source_recording, dict) else None,
        fallback_stem=manifest_path.stem,
    )
    transcript_payload = load_transcript_json(transcript_path)

    produced = 0
    skipped = 0
    clips = collect_clips(clip_manifest, manifest_path, settings.clips_dir)

    with tempfile.TemporaryDirectory(prefix="content-pipeline-", dir=settings.data_dir) as temp_dir_name:
        temp_dir = Path(temp_dir_name)

        for clip in clips:
            output_video = settings.review_queue_dir / review_output_name(clip["clip_path"].stem)
            output_meta = settings.review_queue_dir / review_metadata_name(clip["clip_path"].stem)
            exists_already = output_video.exists() and output_meta.exists()

            if exists_already and not overwrite and not dry_run:
                skipped += 1
                continue

            clip_probe = probe_media(Path(options["ffprobe_path"]), clip["clip_path"])
            input_video_info = extract_video_stream_info(clip_probe)
            input_audio_info = extract_audio_stream_info(clip_probe)
            clip_duration = extract_duration_seconds(clip_probe) or clip["clip_duration_seconds"]
            if clip_duration is None:
                raise RuntimeError(f"Could not determine clip duration for {clip['clip_path'].name}")
            if clip_duration > float(options["max_duration_seconds"]) + 0.05:
                raise ValueError(
                    f"Clip {clip['clip_path'].name} is {clip_duration:.2f}s, which exceeds the Instagram limit of {options['max_duration_seconds']}s"
                )

            transcript_segments = collect_segments_in_range(
                transcript_payload,
                clip["clip_start_time"],
                clip["clip_end_time"],
            )
            subtitle_text = build_ass_document(transcript_segments, clip, options)
            subtitle_file = temp_dir / f"{clip['clip_path'].stem}.ass"
            subtitle_file.write_text(subtitle_text, encoding="utf-8")

            caption = build_caption(clip)
            hashtags = build_hashtags(str(clip["category"]))
            metadata = build_review_metadata(
                manifest_path=manifest_path,
                transcript_path=transcript_path,
                clip=clip,
                output_video=output_video,
                output_meta=output_meta,
                input_video_info=input_video_info,
                input_audio_info=input_audio_info,
                clip_duration=clip_duration,
                transcript_segments=transcript_segments,
                caption=caption,
                hashtags=hashtags,
                options=options,
            )

            if dry_run:
                continue

            command = build_ffmpeg_command(
                ffmpeg_path=Path(options["ffmpeg_path"]),
                input_path=clip["clip_path"],
                output_path=output_video,
                subtitle_path=subtitle_file,
                options=options,
                overwrite=overwrite,
            )

            try:
                run_ffmpeg(Path(options["ffmpeg_path"]), command)
            except Exception as error:
                raise RuntimeError(f"FFmpeg failed for {output_video.name}: {error}") from error

            output_probe = probe_media(Path(options["ffprobe_path"]), output_video)
            output_video_info = extract_video_stream_info(output_probe)
            output_audio_info = extract_audio_stream_info(output_probe)
            output_duration = extract_duration_seconds(output_probe) or clip_duration
            validate_instagram_output(
                output_video,
                output_video_info=output_video_info,
                output_audio_info=output_audio_info,
                duration_seconds=output_duration,
                options=options,
            )

            metadata["review_output"]["video_info"] = output_video_info
            metadata["review_output"]["audio_info"] = output_audio_info
            metadata["review_output"]["duration_seconds"] = round(output_duration, 3)
            metadata["review_output"]["file_size_bytes"] = output_video.stat().st_size
            metadata["review_output"]["status"] = "ready_for_review"

            output_meta.parent.mkdir(parents=True, exist_ok=True)
            output_meta.write_text(json.dumps(metadata, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
            produced += 1

    return produced, skipped


def collect_clips(clip_manifest: dict[str, Any], manifest_path: Path, clips_dir: Path) -> list[dict[str, Any]]:
    """Normalize clip entries from a clip manifest."""
    raw_clips = clip_manifest.get("clips")
    if not isinstance(raw_clips, list):
        raise ValueError(f"Clip manifest is missing a 'clips' list: {manifest_path}")

    clips: list[dict[str, Any]] = []
    for item in raw_clips:
        if not isinstance(item, dict):
            continue

        clip_path = resolve_clip_path(item, clips_dir)
        clip_start_time = _as_float(item.get("clip_start_time"))
        clip_end_time = _as_float(item.get("clip_end_time"))
        clip_duration_seconds = _as_float(item.get("clip_duration_seconds"))
        if clip_start_time is None or clip_end_time is None or clip_end_time <= clip_start_time:
            raise ValueError(f"Clip entry is missing valid clip_start_time/clip_end_time in {manifest_path}")

        clips.append(
            {
                "index": _as_int(item.get("index")) or len(clips) + 1,
                "clip_path": clip_path,
                "category": str(item.get("category") or "clip").strip() or "clip",
                "description": str(item.get("description") or "").strip(),
                "suggested_caption": str(item.get("suggested_caption") or "").strip(),
                "confidence_score": round(_as_float(item.get("confidence_score")) or 0.0, 3),
                "highlight_start_time": _as_float(item.get("highlight_start_time")),
                "highlight_end_time": _as_float(item.get("highlight_end_time")),
                "clip_start_time": float(clip_start_time),
                "clip_end_time": float(clip_end_time),
                "clip_duration_seconds": clip_duration_seconds,
                "source_variant": str(item.get("source_variant") or "").strip() or None,
                "status": str(item.get("status") or "").strip() or None,
            }
        )

    clips.sort(key=lambda clip: (clip["index"], clip["clip_start_time"]))
    return clips


def resolve_clip_path(clip_entry: dict[str, Any], clips_dir: Path) -> Path:
    """Resolve the clip video path referenced by a clip manifest entry."""
    candidates: list[Path] = []
    path_value = clip_entry.get("output_path")
    file_name = clip_entry.get("output_file_name")

    if isinstance(path_value, str) and path_value.strip():
        candidates.append(Path(path_value).expanduser())
    if isinstance(file_name, str) and file_name.strip():
        candidates.append((clips_dir / file_name).resolve())

    for path in candidates:
        resolved = path.resolve() if path.is_absolute() else (Path.cwd() / path).resolve()
        if resolved.exists() and resolved.is_file() and resolved.suffix.lower() == ".mp4":
            return resolved

    raise FileNotFoundError(
        f"Could not resolve clip video from entry with output_path={path_value!r} output_file_name={file_name!r}."
    )


def build_ass_document(transcript_segments: list[dict[str, Any]], clip: dict[str, Any], options: dict[str, Any]) -> str:
    """Return ASS subtitle text for one clip."""
    lines = [
        "[Script Info]",
        "ScriptType: v4.00+",
        "WrapStyle: 2",
        "ScaledBorderAndShadow: yes",
        f"PlayResX: {options['width']}",
        f"PlayResY: {options['height']}",
        "",
        "[V4+ Styles]",
        (
            "Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, OutlineColour, BackColour, "
            "Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, "
            "Shadow, Alignment, MarginL, MarginR, MarginV, Encoding"
        ),
        (
            f"Style: Default,{options['font_name']},{options['font_size']},&H00FFFFFF,&H0000FFFF,&H00000000,"
            f"&H64000000,1,0,0,0,100,100,0,0,1,{options['outline']},0,2,80,80,{options['margin_v']},1"
        ),
        "",
        "[Events]",
        "Format: Layer, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text",
    ]

    for segment in transcript_segments:
        start_time = max(float(segment["start"]), float(clip["clip_start_time"])) - float(clip["clip_start_time"])
        end_time = min(float(segment["end"]), float(clip["clip_end_time"])) - float(clip["clip_start_time"])
        if end_time - start_time <= 0.05:
            continue

        text = wrap_subtitle_text(
            str(segment["text"]),
            line_width=int(options["line_width"]),
            max_lines=int(options["max_lines"]),
        )
        if not text:
            continue

        lines.append(
            f"Dialogue: 0,{format_ass_timestamp(start_time)},{format_ass_timestamp(end_time)},Default,,0,0,0,,{escape_ass_text(text)}"
        )

    return "\n".join(lines) + "\n"


def wrap_subtitle_text(text: str, *, line_width: int, max_lines: int) -> str:
    """Wrap subtitle text to a small number of readable mobile lines."""
    cleaned = " ".join(text.split())
    if not cleaned:
        return ""

    wrapped = textwrap.wrap(cleaned, width=max(8, line_width), break_long_words=False, break_on_hyphens=False)
    if not wrapped:
        return ""

    if len(wrapped) > max_lines:
        kept = wrapped[: max_lines]
        kept[-1] = textwrap.shorten(" ".join(wrapped[max_lines - 1 :]), width=max(8, line_width), placeholder="…")
        wrapped = kept

    return "\\N".join(wrapped)


def escape_ass_text(text: str) -> str:
    """Escape text for ASS dialogue lines."""
    return text.replace("\\", r"\\").replace("{", r"\{").replace("}", r"\}")


def format_ass_timestamp(value: float) -> str:
    """Format seconds for ASS timestamps (H:MM:SS.cc)."""
    total_centiseconds = max(0, int(round(value * 100)))
    hours, remainder = divmod(total_centiseconds, 360000)
    minutes, remainder = divmod(remainder, 6000)
    seconds, centiseconds = divmod(remainder, 100)
    return f"{hours}:{minutes:02}:{seconds:02}.{centiseconds:02}"


def build_ffmpeg_command(
    *,
    ffmpeg_path: Path,
    input_path: Path,
    output_path: Path,
    subtitle_path: Path,
    options: dict[str, Any],
    overwrite: bool,
) -> list[str]:
    """Build the ffmpeg argument list for one formatted review clip."""
    output_path.parent.mkdir(parents=True, exist_ok=True)
    subtitle_filter = build_ass_subtitle_filter(ffmpeg_path, subtitle_path)
    compliance_filter = (
        f"scale={options['width']}:{options['height']}:force_original_aspect_ratio=decrease,"
        f"pad={options['width']}:{options['height']}:(ow-iw)/2:(oh-ih)/2,"
        f"fps={options['fps']},format=yuv420p"
    )

    return [
        "-y" if overwrite else "-n",
        "-hide_banner",
        "-loglevel",
        "error",
        "-i",
        tool_path_arg(ffmpeg_path, input_path),
        "-vf",
        f"{subtitle_filter},{compliance_filter}",
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


def validate_instagram_output(
    output_path: Path,
    *,
    output_video_info: dict[str, Any],
    output_audio_info: dict[str, Any] | None,
    duration_seconds: float,
    options: dict[str, Any],
) -> None:
    """Validate the formatted review clip against Instagram-ready constraints."""
    width = _as_int(output_video_info.get("width")) or 0
    height = _as_int(output_video_info.get("height")) or 0
    codec_name = str(output_video_info.get("codec_name") or "").lower()
    audio_codec_name = str((output_audio_info or {}).get("codec_name") or "").lower()
    file_size = output_path.stat().st_size

    if width != int(options["width"]) or height != int(options["height"]):
        raise RuntimeError(
            f"Formatted clip {output_path.name} has resolution {width}x{height}; expected {options['width']}x{options['height']}"
        )
    if duration_seconds > float(options["max_duration_seconds"]) + 0.05:
        raise RuntimeError(
            f"Formatted clip {output_path.name} is {duration_seconds:.2f}s; expected <= {options['max_duration_seconds']}s"
        )
    if file_size > int(options["max_file_size_bytes"]):
        raise RuntimeError(
            f"Formatted clip {output_path.name} is {file_size} bytes; expected <= {options['max_file_size_bytes']} bytes"
        )
    if codec_name != "h264":
        raise RuntimeError(f"Formatted clip {output_path.name} video codec is {codec_name!r}; expected 'h264'")
    if audio_codec_name and audio_codec_name != "aac":
        raise RuntimeError(f"Formatted clip {output_path.name} audio codec is {audio_codec_name!r}; expected 'aac'")


def build_caption(clip: dict[str, Any]) -> str:
    """Choose the operator-facing caption text for the review queue metadata."""
    for candidate in (clip.get("suggested_caption"), clip.get("description")):
        text = " ".join(str(candidate or "").split())
        if text:
            return text

    category = str(clip.get("category") or "clip").strip().lower()
    if category == "technical":
        return "Built a fix. Learned something. Keeping the useful part."
    if category == "entertainment":
        return "The stream got weird in exactly the way it usually does."
    if category == "hybrid":
        return "Something useful happened. It was also chaos."
    return "Pulled a clip from stream review."


def build_hashtags(category: str) -> list[str]:
    """Return a small, non-spammy hashtag set for Instagram metadata."""
    normalized = str(category or "clip").strip().lower()
    base = ["#sharkfac3", "#starshipshamples", "#buildinpublic"]

    if normalized == "technical":
        extra = ["#offroad", "#fabrication", "#engineering"]
    elif normalized == "entertainment":
        extra = ["#streamclips", "#chaos", "#livestream"]
    else:
        extra = ["#offroad", "#streamclips", "#chaos"]

    return base + extra


def build_review_metadata(
    *,
    manifest_path: Path,
    transcript_path: Path,
    clip: dict[str, Any],
    output_video: Path,
    output_meta: Path,
    input_video_info: dict[str, Any],
    input_audio_info: dict[str, Any] | None,
    clip_duration: float,
    transcript_segments: list[dict[str, Any]],
    caption: str,
    hashtags: list[str],
    options: dict[str, Any],
) -> dict[str, Any]:
    """Build persisted metadata for a review queue item."""
    return {
        "schema_version": 1,
        "generated_at_utc": datetime.now(UTC).isoformat(),
        "source_clip_manifest": {
            "file_name": manifest_path.name,
            "path": str(manifest_path),
        },
        "source_transcript": {
            "file_name": transcript_path.name,
            "path": str(transcript_path),
        },
        "source_clip": {
            "file_name": clip["clip_path"].name,
            "path": str(clip["clip_path"]),
            "index": clip["index"],
            "category": clip["category"],
            "description": clip["description"],
            "suggested_caption": clip["suggested_caption"],
            "confidence_score": clip["confidence_score"],
            "clip_start_time": round(float(clip["clip_start_time"]), 3),
            "clip_end_time": round(float(clip["clip_end_time"]), 3),
            "clip_duration_seconds": round(float(clip_duration), 3),
            "source_variant": clip.get("source_variant"),
        },
        "instagram": {
            "caption": caption,
            "hashtags": hashtags,
            "category": clip["category"],
            "caption_with_hashtags": "\n\n".join([caption, " ".join(hashtags)]).strip(),
            "compliance": {
                "max_duration_seconds": options["max_duration_seconds"],
                "max_file_size_bytes": options["max_file_size_bytes"],
                "aspect_ratio": INSTAGRAM_ASPECT_RATIO,
                "width": options["width"],
                "height": options["height"],
                "video_codec": "h264",
                "audio_codec": "aac",
            },
        },
        "subtitles": {
            "segment_count": len(transcript_segments),
            "segments_found": bool(transcript_segments),
            "font_name": options["font_name"],
            "font_size": options["font_size"],
            "outline": options["outline"],
            "margin_v": options["margin_v"],
            "line_width": options["line_width"],
            "max_lines": options["max_lines"],
        },
        "review_output": {
            "video_file_name": output_video.name,
            "video_path": str(output_video),
            "metadata_file_name": output_meta.name,
            "metadata_path": str(output_meta),
            "input_video_info": input_video_info,
            "input_audio_info": input_audio_info,
            "status": "planned",
        },
    }


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
        print(f"[instagram] Configuration error: {error}", file=sys.stderr)
        return 1

    print(f"[instagram] Clip manifests: {settings.clips_dir}")
    print(f"[instagram] Transcripts:    {settings.transcripts_dir}")
    print(f"[instagram] Review queue:   {settings.review_queue_dir}")
    print(
        f"[instagram] FFmpeg: ffmpeg={options['ffmpeg_path']} ffprobe={options['ffprobe_path']} "
        f"size={options['width']}x{options['height']} fps={options['fps']} max_duration={options['max_duration_seconds']}s"
    )

    produced = 0
    skipped = 0

    for manifest_path in manifests:
        if args.dry_run:
            print(f"[instagram] plan: {manifest_path.name}")
        else:
            print(f"[instagram] Formatting {manifest_path.name}...")

        try:
            manifest_produced, manifest_skipped = process_manifest(
                manifest_path,
                settings=settings,
                options=options,
                overwrite=args.overwrite,
                dry_run=args.dry_run,
            )
        except Exception as error:
            print(f"[instagram] Failed for {manifest_path.name}: {error}", file=sys.stderr)
            return 1

        produced += manifest_produced
        skipped += manifest_skipped
        print(f"[instagram] Finished {manifest_path.name}: produced={manifest_produced} skipped={manifest_skipped}")

    print(f"[instagram] Done. produced={produced} skipped={skipped} manifests={len(manifests)}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
