#!/usr/bin/env python3
"""Configuration helpers for the art pipeline."""

from __future__ import annotations

from dataclasses import dataclass
from pathlib import Path
import os
import sys


TOOL_ROOT = Path(__file__).resolve().parent
REPO_ROOT = TOOL_ROOT.parent.parent
ENV_PATH = TOOL_ROOT / ".env"


@dataclass(frozen=True)
class Settings:
    """Resolved configuration for art pipeline scripts."""

    repo_root: Path
    tool_root: Path
    data_dir: Path
    specs_dir: Path
    prompts_dir: Path
    candidates_dir: Path
    assets_dir: Path
    projects_dir: Path
    asset_types_path: Path
    sd_backend: str
    sd_url: str
    default_batch_count: int
    review_port: int

    def ensure_data_dirs(self) -> None:
        for path in (self.data_dir, self.specs_dir, self.prompts_dir, self.candidates_dir):
            path.mkdir(parents=True, exist_ok=True)


def load_settings() -> Settings:
    """Load settings from .env and environment variables."""
    file_values = _load_env_file(ENV_PATH)

    data_dir = _resolve_path(_get_value("ART_PIPELINE_DATA_DIR", file_values), TOOL_ROOT / "data")

    return Settings(
        repo_root=REPO_ROOT,
        tool_root=TOOL_ROOT,
        data_dir=data_dir,
        specs_dir=_resolve_path(_get_value("ART_PIPELINE_SPECS_DIR", file_values), data_dir / "specs"),
        prompts_dir=_resolve_path(_get_value("ART_PIPELINE_PROMPTS_DIR", file_values), data_dir / "prompts"),
        candidates_dir=_resolve_path(_get_value("ART_PIPELINE_CANDIDATES_DIR", file_values), data_dir / "candidates"),
        assets_dir=_resolve_path(_get_value("ART_PIPELINE_ASSETS_DIR", file_values), REPO_ROOT / "Creative/Art/Assets"),
        projects_dir=_resolve_path(_get_value("ART_PIPELINE_PROJECTS_DIR", file_values), REPO_ROOT / "Creative/Art/Projects"),
        asset_types_path=_resolve_path(
            _get_value("ART_PIPELINE_ASSET_TYPES_PATH", file_values),
            REPO_ROOT / "Tools/ArtPipeline/asset_types.json",
        ),
        sd_backend=(_get_value("ART_PIPELINE_SD_BACKEND", file_values) or "comfyui").strip(),
        sd_url=(_get_value("ART_PIPELINE_SD_URL", file_values) or "http://127.0.0.1:8188").strip(),
        default_batch_count=_parse_int(_get_value("ART_PIPELINE_DEFAULT_BATCH_COUNT", file_values), 4),
        review_port=_parse_int(_get_value("ART_PIPELINE_REVIEW_PORT", file_values), 8766),
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


def _parse_int(value: str | None, default: int) -> int:
    if value is None or value == "":
        return default

    try:
        return int(value)
    except ValueError as error:
        raise ValueError(f"Invalid integer value: {value!r}") from error


def _resolve_path(value: str | None, default: Path) -> Path:
    if value is None or value.strip() == "":
        return default.resolve()

    path = _path_from_string(value)
    if not path.is_absolute():
        path = TOOL_ROOT / path
    return path.resolve()


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
