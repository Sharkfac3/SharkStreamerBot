#!/usr/bin/env python3
"""Configuration helpers for the content pipeline."""

from __future__ import annotations

from dataclasses import dataclass
from pathlib import Path
import os
import shutil
import sys
from typing import Iterable


TOOL_ROOT = Path(__file__).resolve().parent
REPO_ROOT = TOOL_ROOT.parent.parent
ENV_PATH = TOOL_ROOT / ".env"
DEFAULT_RECORDINGS_DIR = r"C:\Users\sharkfac3\Workspace\streamStuff\recordings"


@dataclass(frozen=True)
class Settings:
    """Resolved configuration for pipeline scripts."""

    repo_root: Path
    tool_root: Path
    data_dir: Path
    transcripts_dir: Path
    highlights_dir: Path
    clips_dir: Path
    review_queue_dir: Path
    published_dir: Path
    feedback_dir: Path
    feedback_db_path: Path
    feedback_prompt_path: Path
    feedback_summary_path: Path
    recordings_dir: Path
    whisper_model: str
    whisper_device: str
    whisper_compute_type: str
    whisper_language: str | None
    whisper_beam_size: int
    whisper_vad: bool
    word_timestamps: bool
    ollama_url: str
    ollama_model: str
    ollama_keep_alive: str
    ollama_unload_after_run: bool
    highlight_window_minutes: int
    highlight_window_overlap_minutes: int
    highlight_max_per_window: int
    ffmpeg_path: Path | None
    ffprobe_path: Path | None
    clip_pre_pad_seconds: float
    clip_post_pad_seconds: float
    clip_width: int
    clip_height: int
    clip_fps: int
    clip_video_bitrate: str
    clip_video_codec: str
    clip_audio_codec: str
    clip_audio_bitrate: str
    clip_audio_sample_rate: int
    review_max_duration_seconds: int
    review_max_file_size_mb: int
    review_subtitle_font_name: str
    review_subtitle_font_size: int
    review_subtitle_outline: int
    review_subtitle_margin_v: int
    review_subtitle_line_width: int
    review_subtitle_max_lines: int

    def ensure_data_dirs(self) -> None:
        for path in (
            self.data_dir,
            self.transcripts_dir,
            self.highlights_dir,
            self.clips_dir,
            self.review_queue_dir,
            self.published_dir,
            self.feedback_dir,
        ):
            path.mkdir(parents=True, exist_ok=True)


def load_settings() -> Settings:
    """Load settings from .env and environment variables."""
    file_values = _load_env_file(ENV_PATH)

    data_dir = _resolve_path(_get_value("CONTENT_PIPELINE_DATA_DIR", file_values), TOOL_ROOT / "data")
    transcripts_dir = _resolve_path(_get_value("CONTENT_PIPELINE_TRANSCRIPTS_DIR", file_values), data_dir / "transcripts")
    highlights_dir = _resolve_path(_get_value("CONTENT_PIPELINE_HIGHLIGHTS_DIR", file_values), data_dir / "highlights")
    feedback_dir = _resolve_path(_get_value("CONTENT_PIPELINE_FEEDBACK_DIR", file_values), data_dir / "feedback")
    recordings_dir = _resolve_path(
        _get_value("CONTENT_PIPELINE_RECORDINGS_DIR", file_values),
        _path_from_string(DEFAULT_RECORDINGS_DIR),
    )

    return Settings(
        repo_root=REPO_ROOT,
        tool_root=TOOL_ROOT,
        data_dir=data_dir,
        transcripts_dir=transcripts_dir,
        highlights_dir=highlights_dir,
        clips_dir=_resolve_path(_get_value("CONTENT_PIPELINE_CLIPS_DIR", file_values), data_dir / "clips"),
        review_queue_dir=_resolve_path(_get_value("CONTENT_PIPELINE_REVIEW_QUEUE_DIR", file_values), data_dir / "review_queue"),
        published_dir=_resolve_path(_get_value("CONTENT_PIPELINE_PUBLISHED_DIR", file_values), data_dir / "published"),
        feedback_dir=feedback_dir,
        feedback_db_path=_resolve_path(_get_value("CONTENT_PIPELINE_FEEDBACK_DB_PATH", file_values), feedback_dir / "feedback.db"),
        feedback_prompt_path=_resolve_path(_get_value("CONTENT_PIPELINE_FEEDBACK_PROMPT_PATH", file_values), feedback_dir / "prompt_context.txt"),
        feedback_summary_path=_resolve_path(_get_value("CONTENT_PIPELINE_FEEDBACK_SUMMARY_PATH", file_values), feedback_dir / "summary.json"),
        recordings_dir=recordings_dir,
        whisper_model=_get_value("CONTENT_PIPELINE_WHISPER_MODEL", file_values) or "large-v3",
        whisper_device=(_get_value("CONTENT_PIPELINE_WHISPER_DEVICE", file_values) or "auto").strip(),
        whisper_compute_type=(_get_value("CONTENT_PIPELINE_WHISPER_COMPUTE_TYPE", file_values) or "auto").strip(),
        whisper_language=_normalize_optional_text(_get_value("CONTENT_PIPELINE_WHISPER_LANGUAGE", file_values)),
        whisper_beam_size=_parse_int(_get_value("CONTENT_PIPELINE_WHISPER_BEAM_SIZE", file_values), 5),
        whisper_vad=_parse_bool(_get_value("CONTENT_PIPELINE_WHISPER_VAD", file_values), True),
        word_timestamps=_parse_bool(_get_value("CONTENT_PIPELINE_WORD_TIMESTAMPS", file_values), True),
        ollama_url=(_get_value("CONTENT_PIPELINE_OLLAMA_URL", file_values) or "http://localhost:11434/api/generate").strip(),
        ollama_model=(_get_value("CONTENT_PIPELINE_OLLAMA_MODEL", file_values) or "llama3.1:8b").strip(),
        ollama_keep_alive=(_get_value("CONTENT_PIPELINE_OLLAMA_KEEP_ALIVE", file_values) or "10m").strip(),
        ollama_unload_after_run=_parse_bool(_get_value("CONTENT_PIPELINE_OLLAMA_UNLOAD_AFTER_RUN", file_values), True),
        highlight_window_minutes=_parse_int(_get_value("CONTENT_PIPELINE_HIGHLIGHT_WINDOW_MINUTES", file_values), 5),
        highlight_window_overlap_minutes=_parse_int(
            _get_value("CONTENT_PIPELINE_HIGHLIGHT_WINDOW_OVERLAP_MINUTES", file_values),
            1,
        ),
        highlight_max_per_window=_parse_int(_get_value("CONTENT_PIPELINE_HIGHLIGHT_MAX_PER_WINDOW", file_values), 3),
        ffmpeg_path=_resolve_optional_binary_path(_get_value("CONTENT_PIPELINE_FFMPEG_PATH", file_values), "ffmpeg"),
        ffprobe_path=_resolve_optional_binary_path(_get_value("CONTENT_PIPELINE_FFPROBE_PATH", file_values), "ffprobe"),
        clip_pre_pad_seconds=_parse_float(_get_value("CONTENT_PIPELINE_CLIP_PRE_PAD_SECONDS", file_values), 2.0),
        clip_post_pad_seconds=_parse_float(_get_value("CONTENT_PIPELINE_CLIP_POST_PAD_SECONDS", file_values), 1.0),
        clip_width=_parse_int(_get_value("CONTENT_PIPELINE_CLIP_WIDTH", file_values), 1080),
        clip_height=_parse_int(_get_value("CONTENT_PIPELINE_CLIP_HEIGHT", file_values), 1920),
        clip_fps=_parse_int(_get_value("CONTENT_PIPELINE_CLIP_FPS", file_values), 30),
        clip_video_bitrate=(_get_value("CONTENT_PIPELINE_CLIP_VIDEO_BITRATE", file_values) or "8M").strip(),
        clip_video_codec=(_get_value("CONTENT_PIPELINE_CLIP_VIDEO_CODEC", file_values) or "h264_nvenc").strip(),
        clip_audio_codec=(_get_value("CONTENT_PIPELINE_CLIP_AUDIO_CODEC", file_values) or "aac").strip(),
        clip_audio_bitrate=(_get_value("CONTENT_PIPELINE_CLIP_AUDIO_BITRATE", file_values) or "192k").strip(),
        clip_audio_sample_rate=_parse_int(_get_value("CONTENT_PIPELINE_CLIP_AUDIO_SAMPLE_RATE", file_values), 48000),
        review_max_duration_seconds=_parse_int(
            _get_value("CONTENT_PIPELINE_REVIEW_MAX_DURATION_SECONDS", file_values),
            90,
        ),
        review_max_file_size_mb=_parse_int(_get_value("CONTENT_PIPELINE_REVIEW_MAX_FILE_SIZE_MB", file_values), 250),
        review_subtitle_font_name=(_get_value("CONTENT_PIPELINE_REVIEW_SUBTITLE_FONT_NAME", file_values) or "Arial").strip(),
        review_subtitle_font_size=_parse_int(_get_value("CONTENT_PIPELINE_REVIEW_SUBTITLE_FONT_SIZE", file_values), 60),
        review_subtitle_outline=_parse_int(_get_value("CONTENT_PIPELINE_REVIEW_SUBTITLE_OUTLINE", file_values), 3),
        review_subtitle_margin_v=_parse_int(_get_value("CONTENT_PIPELINE_REVIEW_SUBTITLE_MARGIN_V", file_values), 340),
        review_subtitle_line_width=_parse_int(_get_value("CONTENT_PIPELINE_REVIEW_SUBTITLE_LINE_WIDTH", file_values), 28),
        review_subtitle_max_lines=_parse_int(_get_value("CONTENT_PIPELINE_REVIEW_SUBTITLE_MAX_LINES", file_values), 2),
    )


def _get_value(name: str, file_values: dict[str, str]) -> str | None:
    if name in os.environ:
        return os.environ[name]
    return file_values.get(name)


def _load_env_file(path: Path) -> dict[str, str]:
    values: dict[str, str] = {}
    if not path.exists():
        return values

    for line_number, raw_line in enumerate(path.read_text(encoding="utf-8").splitlines(), start=1):
        line = raw_line.strip()
        if not line or line.startswith("#"):
            continue
        if "=" not in line:
            raise ValueError(f"Invalid .env entry at {path}:{line_number}: expected KEY=VALUE")

        key, value = line.split("=", 1)
        values[key.strip()] = value.strip().strip('"').strip("'")

    return values


def _parse_bool(value: str | None, default: bool) -> bool:
    if value is None or value == "":
        return default

    normalized = value.strip().lower()
    if normalized in {"1", "true", "yes", "on"}:
        return True
    if normalized in {"0", "false", "no", "off"}:
        return False

    raise ValueError(f"Invalid boolean value: {value!r}")


def _parse_int(value: str | None, default: int) -> int:
    if value is None or value == "":
        return default

    try:
        return int(value)
    except ValueError as error:
        raise ValueError(f"Invalid integer value: {value!r}") from error


def _parse_float(value: str | None, default: float) -> float:
    if value is None or value == "":
        return default

    try:
        return float(value)
    except ValueError as error:
        raise ValueError(f"Invalid float value: {value!r}") from error


def _normalize_optional_text(value: str | None) -> str | None:
    if value is None:
        return None
    cleaned = value.strip()
    return cleaned or None


def _resolve_path(value: str | None, default: Path) -> Path:
    if value is None or value.strip() == "":
        return default.resolve()

    path = _path_from_string(value)
    if not path.is_absolute():
        path = TOOL_ROOT / path
    return path.resolve()


def _resolve_optional_binary_path(value: str | None, executable_name: str) -> Path | None:
    normalized = (value or "").strip()
    if normalized:
        return _resolve_path(normalized, TOOL_ROOT / executable_name)

    candidates = [executable_name]
    if not executable_name.lower().endswith(".exe"):
        candidates.append(f"{executable_name}.exe")

    for candidate in candidates:
        found = shutil.which(candidate)
        if found:
            return Path(found).resolve()

    return None


def ensure_recordings_dir_exists(recordings_dir: Path) -> Path:
    """Exit with an actionable message if the resolved recordings directory is missing."""
    resolved_recordings_dir = recordings_dir.resolve()
    if resolved_recordings_dir.exists():
        return resolved_recordings_dir

    print(f"ERROR: Recordings directory not found: {resolved_recordings_dir}", file=sys.stderr)
    print(
        "Set CONTENT_PIPELINE_RECORDINGS_DIR in Tools/ContentPipeline/.env to your recordings path.",
        file=sys.stderr,
    )
    print("See Tools/ContentPipeline/.env.example for all options.", file=sys.stderr)
    sys.exit(1)


def _path_from_string(value: str) -> Path:
    text = value.strip()
    if _looks_like_windows_drive_path(text):
        converted = _windows_to_platform_path(text)
        if converted is not None:
            return converted
    return Path(text).expanduser()


def _looks_like_windows_drive_path(text: str) -> bool:
    return len(text) >= 3 and text[1] == ":" and text[2] in {"\\", "/"}


def _windows_to_platform_path(text: str) -> Path | None:
    drive = text[0].lower()
    remainder = text[2:].replace("\\", "/").lstrip("/")

    if os.name != "nt" and sys.platform.startswith("linux"):
        return Path("/mnt") / drive / remainder

    return Path(text)


def find_recordings(directory: Path, suffixes: Iterable[str] = (".mp4",)) -> list[Path]:
    """Return sorted recordings, excluding vertical duplicates by default."""
    wanted = {suffix.lower() for suffix in suffixes}
    files = [
        path
        for path in sorted(directory.iterdir())
        if path.is_file() and path.suffix.lower() in wanted and not path.name.lower().endswith("-vertical.mp4")
    ]
    return files
