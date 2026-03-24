#!/usr/bin/env python3
"""FFmpeg and FFprobe helpers for the content pipeline."""

from __future__ import annotations

from pathlib import Path
import json
import subprocess
from typing import Any


def probe_media(ffprobe_path: Path, media_path: Path) -> dict[str, Any]:
    """Return selected media metadata from ffprobe JSON output."""
    command = [
        str(ffprobe_path),
        "-v",
        "error",
        "-print_format",
        "json",
        "-show_streams",
        "-show_format",
        tool_path_arg(ffprobe_path, media_path),
    ]

    completed = subprocess.run(command, capture_output=True, text=True, check=False)
    if completed.returncode != 0:
        stderr = completed.stderr.strip() or "unknown ffprobe error"
        raise RuntimeError(f"ffprobe failed for {media_path}: {stderr}")

    try:
        payload = json.loads(completed.stdout)
    except json.JSONDecodeError as error:
        raise RuntimeError(f"ffprobe returned invalid JSON for {media_path}: {error}") from error

    if not isinstance(payload, dict):
        raise RuntimeError(f"ffprobe output was not a JSON object for {media_path}")

    return payload


def extract_video_stream_info(probe_payload: dict[str, Any]) -> dict[str, Any]:
    """Return normalized video stream info from an ffprobe payload."""
    streams = probe_payload.get("streams")
    if not isinstance(streams, list):
        raise RuntimeError("ffprobe payload is missing a streams list")

    for stream in streams:
        if not isinstance(stream, dict):
            continue
        if str(stream.get("codec_type") or "").lower() != "video":
            continue

        width = _as_int(stream.get("width"))
        height = _as_int(stream.get("height"))
        if width is None or height is None or width <= 0 or height <= 0:
            raise RuntimeError("ffprobe video stream is missing valid width/height")

        return {
            "width": width,
            "height": height,
            "codec_name": stream.get("codec_name"),
            "pix_fmt": stream.get("pix_fmt"),
            "avg_frame_rate": stream.get("avg_frame_rate"),
            "display_aspect_ratio": stream.get("display_aspect_ratio"),
        }

    raise RuntimeError("ffprobe did not find a video stream")


def extract_audio_stream_info(probe_payload: dict[str, Any]) -> dict[str, Any] | None:
    """Return normalized audio stream info from an ffprobe payload when available."""
    streams = probe_payload.get("streams")
    if not isinstance(streams, list):
        raise RuntimeError("ffprobe payload is missing a streams list")

    for stream in streams:
        if not isinstance(stream, dict):
            continue
        if str(stream.get("codec_type") or "").lower() != "audio":
            continue

        return {
            "codec_name": stream.get("codec_name"),
            "sample_rate": _as_int(stream.get("sample_rate")),
            "channels": _as_int(stream.get("channels")),
            "channel_layout": stream.get("channel_layout"),
        }

    return None


def extract_duration_seconds(probe_payload: dict[str, Any]) -> float | None:
    """Return media duration in seconds when available."""
    format_payload = probe_payload.get("format")
    if isinstance(format_payload, dict):
        duration = _as_float(format_payload.get("duration"))
        if duration is not None and duration > 0:
            return duration

    streams = probe_payload.get("streams")
    if isinstance(streams, list):
        for stream in streams:
            if not isinstance(stream, dict):
                continue
            duration = _as_float(stream.get("duration"))
            if duration is not None and duration > 0:
                return duration

    return None


def is_vertical_video(video_info: dict[str, Any]) -> bool:
    """Return True when the source video is portrait-oriented."""
    width = _as_int(video_info.get("width")) or 0
    height = _as_int(video_info.get("height")) or 0
    return height > width


def build_video_filter(*, source_is_vertical: bool, width: int, height: int, fps: int) -> str:
    """Build the FFmpeg video filter for vertical clip output."""
    if source_is_vertical:
        base = f"scale={width}:{height}:force_original_aspect_ratio=decrease,pad={width}:{height}:(ow-iw)/2:(oh-ih)/2"
    else:
        base = f"scale={width}:{height}:force_original_aspect_ratio=increase,crop={width}:{height}"

    return f"{base},fps={fps},format=yuv420p"


def build_ass_subtitle_filter(ffmpeg_path: Path, subtitle_path: Path) -> str:
    """Return an FFmpeg ass filter expression for the given subtitle file."""
    escaped_path = filter_path_arg(ffmpeg_path, subtitle_path)
    return f"ass=filename='{escaped_path}'"


def run_ffmpeg(ffmpeg_path: Path, command: list[str]) -> None:
    """Run ffmpeg and raise a readable error on failure."""
    executable = str(ffmpeg_path)
    completed = subprocess.run([executable, *command], capture_output=True, text=True, check=False)
    if completed.returncode != 0:
        stderr = completed.stderr.strip() or "unknown ffmpeg error"
        raise RuntimeError(stderr)


def tool_path_arg(executable_path: Path, target_path: Path) -> str:
    """Return a media path string suitable for the given executable."""
    resolved = target_path.resolve()
    if _is_windows_executable(executable_path) and _is_wsl_mounted_path(resolved):
        return _wsl_to_windows_path(resolved)
    return str(resolved)


def filter_path_arg(executable_path: Path, target_path: Path) -> str:
    """Return a filter-safe path string for ffmpeg expressions like ass/subtitles."""
    raw = tool_path_arg(executable_path, target_path)
    normalized = raw.replace("\\", "/")
    return normalized.replace(":", r"\:").replace("'", r"\'")


def _is_windows_executable(path: Path) -> bool:
    return path.suffix.lower() == ".exe"


def _is_wsl_mounted_path(path: Path) -> bool:
    parts = path.parts
    return len(parts) >= 3 and parts[0] == "/" and parts[1] == "mnt" and len(parts[2]) == 1


def _wsl_to_windows_path(path: Path) -> str:
    parts = path.parts
    drive = parts[2].upper()
    remainder = "\\".join(parts[3:])
    if remainder:
        return f"{drive}:\\{remainder}"
    return f"{drive}:\\"


def _as_int(value: Any) -> int | None:
    if value is None:
        return None
    if isinstance(value, int):
        return value
    try:
        return int(str(value).strip())
    except ValueError:
        return None


def _as_float(value: Any) -> float | None:
    if value is None:
        return None
    if isinstance(value, (int, float)):
        return float(value)
    try:
        return float(str(value).strip())
    except ValueError:
        return None
