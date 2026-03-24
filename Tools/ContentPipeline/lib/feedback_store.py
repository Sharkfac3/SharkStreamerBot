#!/usr/bin/env python3
"""SQLite helpers for ContentPipeline feedback analytics."""

from __future__ import annotations

from datetime import UTC, datetime
import csv
import json
from pathlib import Path
import sqlite3
from typing import Any, Iterable


CLIP_ID_ALIASES = [
    "clip_id",
    "clip",
    "id",
    "asset_id",
    "video_file_name",
    "filename",
    "file_name",
    "video",
    "reel",
    "slug",
]
CAPTION_ALIASES = ["final_caption", "caption", "title", "description", "post_caption"]
POSTED_AT_ALIASES = ["posted_at", "publish_date", "published_at", "date", "created_at"]

METRIC_ALIASES: dict[str, list[str]] = {
    "views": ["views", "plays", "video_views", "reel_plays", "impressions"],
    "reach": ["reach", "accounts_reached"],
    "likes": ["likes", "likes_count"],
    "comments": ["comments", "comments_count"],
    "shares": ["shares", "shares_count"],
    "saves": ["saves", "saves_count"],
    "watch_time_seconds": ["watch_time_seconds", "watch_time", "watch_time_sec", "total_watch_time_seconds"],
    "avg_watch_time_seconds": [
        "avg_watch_time_seconds",
        "average_watch_time_seconds",
        "avg_watch_time",
        "average_watch_time",
    ],
    "completion_rate": ["completion_rate", "watch_through_rate", "completion", "retention"],
    "profile_visits": ["profile_visits", "visits"],
    "follows": ["follows", "follows_gained"],
    "link_clicks": ["link_clicks", "website_taps", "website_clicks"],
}


SCHEMA_SQL = """
CREATE TABLE IF NOT EXISTS clips (
    clip_id TEXT PRIMARY KEY,
    metadata_path TEXT NOT NULL UNIQUE,
    video_file_name TEXT NOT NULL,
    category TEXT,
    approved_at_utc TEXT,
    duration_seconds REAL,
    confidence_score REAL,
    source_variant TEXT,
    source_recording_file_name TEXT,
    final_caption TEXT,
    final_hashtags_json TEXT NOT NULL,
    created_at_utc TEXT NOT NULL,
    updated_at_utc TEXT NOT NULL,
    raw_metadata_json TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS metric_imports (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    imported_at_utc TEXT NOT NULL,
    source_path TEXT NOT NULL,
    row_count INTEGER NOT NULL,
    notes TEXT
);

CREATE TABLE IF NOT EXISTS clip_metrics (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    clip_id TEXT NOT NULL,
    import_id INTEGER NOT NULL,
    source_path TEXT NOT NULL,
    platform TEXT NOT NULL,
    posted_at TEXT,
    views REAL,
    reach REAL,
    likes REAL,
    comments REAL,
    shares REAL,
    saves REAL,
    watch_time_seconds REAL,
    avg_watch_time_seconds REAL,
    completion_rate REAL,
    profile_visits REAL,
    follows REAL,
    link_clicks REAL,
    raw_row_json TEXT NOT NULL,
    created_at_utc TEXT NOT NULL,
    FOREIGN KEY (clip_id) REFERENCES clips (clip_id),
    FOREIGN KEY (import_id) REFERENCES metric_imports (id)
);

CREATE INDEX IF NOT EXISTS idx_clip_metrics_clip_id ON clip_metrics (clip_id);
CREATE INDEX IF NOT EXISTS idx_clips_category ON clips (category);
"""


def connect_database(path: Path) -> sqlite3.Connection:
    """Open the SQLite database with row access enabled."""
    path.parent.mkdir(parents=True, exist_ok=True)
    connection = sqlite3.connect(path)
    connection.row_factory = sqlite3.Row
    return connection


def initialize_database(connection: sqlite3.Connection) -> None:
    """Create tables when they do not exist yet."""
    connection.executescript(SCHEMA_SQL)
    connection.commit()


def now_utc_iso() -> str:
    return datetime.now(UTC).isoformat()


def derive_clip_id_from_metadata(meta_path: Path, payload: dict[str, Any]) -> str:
    """Return the canonical clip id for a published metadata payload."""
    published = payload.get("published") if isinstance(payload.get("published"), dict) else {}
    video_file_name = str(published.get("video_file_name") or meta_path.stem.replace("_meta", "") + ".mp4").strip()
    return Path(video_file_name).stem


def upsert_clip_metadata(connection: sqlite3.Connection, meta_path: Path, payload: dict[str, Any]) -> str:
    """Insert or update one published clip record from metadata."""
    clip_id = derive_clip_id_from_metadata(meta_path, payload)
    published = payload.get("published") if isinstance(payload.get("published"), dict) else {}
    source_clip = payload.get("source_clip") if isinstance(payload.get("source_clip"), dict) else {}
    timestamp = now_utc_iso()

    connection.execute(
        """
        INSERT INTO clips (
            clip_id, metadata_path, video_file_name, category, approved_at_utc,
            duration_seconds, confidence_score, source_variant, source_recording_file_name,
            final_caption, final_hashtags_json, created_at_utc, updated_at_utc, raw_metadata_json
        )
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        ON CONFLICT(clip_id) DO UPDATE SET
            metadata_path=excluded.metadata_path,
            video_file_name=excluded.video_file_name,
            category=excluded.category,
            approved_at_utc=excluded.approved_at_utc,
            duration_seconds=excluded.duration_seconds,
            confidence_score=excluded.confidence_score,
            source_variant=excluded.source_variant,
            source_recording_file_name=excluded.source_recording_file_name,
            final_caption=excluded.final_caption,
            final_hashtags_json=excluded.final_hashtags_json,
            updated_at_utc=excluded.updated_at_utc,
            raw_metadata_json=excluded.raw_metadata_json
        """,
        (
            clip_id,
            str(meta_path),
            str(published.get("video_file_name") or f"{clip_id}.mp4").strip(),
            str(published.get("category") or source_clip.get("category") or "clip").strip(),
            _normalize_text(published.get("approved_at_utc")),
            _as_float(source_clip.get("clip_duration_seconds")),
            _as_float(source_clip.get("confidence_score")),
            _normalize_text(source_clip.get("source_variant")),
            _normalize_text(payload.get("source_transcript", {}).get("file_name") if isinstance(payload.get("source_transcript"), dict) else None),
            _normalize_text(published.get("final_caption")),
            json.dumps(_normalize_hashtags(published.get("final_hashtags")), ensure_ascii=False),
            timestamp,
            timestamp,
            json.dumps(payload, ensure_ascii=False),
        ),
    )
    connection.commit()
    return clip_id


def sync_published_metadata(connection: sqlite3.Connection, published_dir: Path) -> int:
    """Scan published metadata files and upsert them into SQLite."""
    count = 0
    for meta_path in sorted(published_dir.glob("*_meta.json")):
        payload = _load_json(meta_path)
        upsert_clip_metadata(connection, meta_path, payload)
        count += 1
    return count


def import_metrics_csv(connection: sqlite3.Connection, csv_path: Path, *, platform: str = "instagram") -> dict[str, Any]:
    """Import one metrics CSV using clip-id or caption matching heuristics."""
    rows = list(csv.DictReader(csv_path.open("r", encoding="utf-8-sig", newline="")))
    if not rows:
        raise ValueError(f"CSV contains no data rows: {csv_path}")

    import_id = _insert_import_record(connection, csv_path, len(rows))
    imported = 0
    skipped = 0
    failures: list[str] = []

    for index, row in enumerate(rows, start=2):
        try:
            clip_id = match_clip_id(connection, row)
            if clip_id is None:
                skipped += 1
                failures.append(f"line {index}: could not match row to a published clip")
                continue

            connection.execute(
                """
                INSERT INTO clip_metrics (
                    clip_id, import_id, source_path, platform, posted_at,
                    views, reach, likes, comments, shares, saves,
                    watch_time_seconds, avg_watch_time_seconds, completion_rate,
                    profile_visits, follows, link_clicks, raw_row_json, created_at_utc
                )
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                """,
                (
                    clip_id,
                    import_id,
                    str(csv_path),
                    platform,
                    _normalize_text(_lookup_value(row, POSTED_AT_ALIASES)),
                    _extract_metric(row, "views"),
                    _extract_metric(row, "reach"),
                    _extract_metric(row, "likes"),
                    _extract_metric(row, "comments"),
                    _extract_metric(row, "shares"),
                    _extract_metric(row, "saves"),
                    _extract_metric(row, "watch_time_seconds"),
                    _extract_metric(row, "avg_watch_time_seconds"),
                    _extract_metric(row, "completion_rate"),
                    _extract_metric(row, "profile_visits"),
                    _extract_metric(row, "follows"),
                    _extract_metric(row, "link_clicks"),
                    json.dumps(row, ensure_ascii=False),
                    now_utc_iso(),
                ),
            )
            imported += 1
        except Exception as error:
            skipped += 1
            failures.append(f"line {index}: {error}")

    connection.commit()
    return {
        "source_path": str(csv_path),
        "import_id": import_id,
        "row_count": len(rows),
        "imported": imported,
        "skipped": skipped,
        "failures": failures,
    }


def build_template_csv(connection: sqlite3.Connection, output_path: Path) -> int:
    """Write a merge-friendly CSV template for manual insights enrichment."""
    rows = connection.execute(
        """
        SELECT clip_id, video_file_name, category, approved_at_utc, duration_seconds, final_caption, final_hashtags_json
        FROM clips
        ORDER BY approved_at_utc, clip_id
        """
    ).fetchall()

    output_path.parent.mkdir(parents=True, exist_ok=True)
    with output_path.open("w", encoding="utf-8", newline="") as handle:
        writer = csv.writer(handle)
        writer.writerow(
            [
                "clip_id",
                "video_file_name",
                "category",
                "approved_at_utc",
                "duration_seconds",
                "final_caption",
                "final_hashtags",
                "posted_at",
                "views",
                "reach",
                "likes",
                "comments",
                "shares",
                "saves",
                "watch_time_seconds",
                "avg_watch_time_seconds",
                "completion_rate",
                "profile_visits",
                "follows",
                "link_clicks",
            ]
        )
        for row in rows:
            writer.writerow(
                [
                    row["clip_id"],
                    row["video_file_name"],
                    row["category"],
                    row["approved_at_utc"],
                    row["duration_seconds"],
                    row["final_caption"],
                    " ".join(json.loads(row["final_hashtags_json"] or "[]")),
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                ]
            )
    return len(rows)


def build_feedback_summary(connection: sqlite3.Connection) -> dict[str, Any]:
    """Build aggregate insights from the latest imported metrics per clip."""
    clip_count = int(connection.execute("SELECT COUNT(*) FROM clips").fetchone()[0])
    metric_clip_count = int(connection.execute("SELECT COUNT(DISTINCT clip_id) FROM clip_metrics").fetchone()[0])
    import_count = int(connection.execute("SELECT COUNT(*) FROM metric_imports").fetchone()[0])

    latest_rows = connection.execute(
        """
        WITH ranked AS (
            SELECT
                clip_metrics.*, clips.category, clips.duration_seconds, clips.final_hashtags_json,
                ROW_NUMBER() OVER (PARTITION BY clip_metrics.clip_id ORDER BY clip_metrics.id DESC) AS row_num
            FROM clip_metrics
            JOIN clips ON clips.clip_id = clip_metrics.clip_id
        )
        SELECT * FROM ranked WHERE row_num = 1
        """
    ).fetchall()

    metrics = [dict(row) for row in latest_rows]
    summary = {
        "generated_at_utc": now_utc_iso(),
        "clip_count": clip_count,
        "metric_clip_count": metric_clip_count,
        "import_count": import_count,
        "top_categories": _top_categories(metrics),
        "top_duration_buckets": _top_duration_buckets(metrics),
        "top_hashtags": _top_hashtags(metrics),
        "top_clips": _top_clips(metrics),
        "lessons": _build_lessons(metrics),
    }
    return summary


def write_summary_files(summary: dict[str, Any], *, json_path: Path, prompt_path: Path) -> None:
    """Persist machine-readable and prompt-readable feedback summaries."""
    json_path.parent.mkdir(parents=True, exist_ok=True)
    prompt_path.parent.mkdir(parents=True, exist_ok=True)
    json_path.write_text(json.dumps(summary, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    prompt_path.write_text(build_prompt_context(summary), encoding="utf-8")


def build_prompt_context(summary: dict[str, Any]) -> str:
    """Render aggregate metrics into a short LLM-friendly text block."""
    lines = [
        "Feedback insights from published short-form clips:",
        f"- Published clips tracked: {summary.get('clip_count', 0)}",
        f"- Clips with imported metrics: {summary.get('metric_clip_count', 0)}",
    ]

    for entry in summary.get("top_categories", [])[:3]:
        lines.append(
            f"- Strong category: {entry['category']} (avg_score={entry['avg_score']:.2f}, clips={entry['clip_count']})"
        )

    for entry in summary.get("top_duration_buckets", [])[:3]:
        lines.append(
            f"- Strong duration bucket: {entry['bucket']}s (avg_score={entry['avg_score']:.2f}, clips={entry['clip_count']})"
        )

    hashtags = summary.get("top_hashtags", [])[:5]
    if hashtags:
        lines.append("- Hashtags that appear in stronger clips: " + ", ".join(item["hashtag"] for item in hashtags))

    for lesson in summary.get("lessons", [])[:5]:
        lines.append(f"- {lesson}")

    return "\n".join(lines).strip() + "\n"


def match_clip_id(connection: sqlite3.Connection, row: dict[str, Any]) -> str | None:
    """Match a CSV row to a clip by clip id, filename, or exact caption."""
    direct_value = _lookup_value(row, CLIP_ID_ALIASES)
    if direct_value:
        normalized = Path(str(direct_value).strip()).stem
        found = connection.execute(
            "SELECT clip_id FROM clips WHERE clip_id = ? OR video_file_name = ?",
            (normalized, str(direct_value).strip()),
        ).fetchone()
        if found:
            return str(found[0])

    caption = _normalize_text(_lookup_value(row, CAPTION_ALIASES))
    if caption:
        found = connection.execute("SELECT clip_id FROM clips WHERE final_caption = ?", (caption,)).fetchone()
        if found:
            return str(found[0])

    return None


def _insert_import_record(connection: sqlite3.Connection, csv_path: Path, row_count: int) -> int:
    cursor = connection.execute(
        "INSERT INTO metric_imports (imported_at_utc, source_path, row_count, notes) VALUES (?, ?, ?, ?)",
        (now_utc_iso(), str(csv_path), row_count, None),
    )
    connection.commit()
    return int(cursor.lastrowid)


def _lookup_value(row: dict[str, Any], aliases: Iterable[str]) -> Any:
    normalized_map = {str(key).strip().lower(): value for key, value in row.items()}
    for alias in aliases:
        if alias.lower() in normalized_map:
            return normalized_map[alias.lower()]
    return None


def _extract_metric(row: dict[str, Any], name: str) -> float | None:
    return _as_float(_lookup_value(row, METRIC_ALIASES[name]))


def _load_json(path: Path) -> dict[str, Any]:
    try:
        payload = json.loads(path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as error:
        raise ValueError(f"Invalid JSON in {path}: {error}") from error
    if not isinstance(payload, dict):
        raise ValueError(f"Expected JSON object in {path}")
    return payload


def _normalize_text(value: Any) -> str | None:
    text = " ".join(str(value or "").split()).strip()
    return text or None


def _normalize_hashtags(value: Any) -> list[str]:
    if isinstance(value, list):
        items = [str(item) for item in value]
    elif value is None:
        items = []
    else:
        items = str(value).split()

    normalized: list[str] = []
    seen: set[str] = set()
    for item in items:
        cleaned = str(item or "").strip()
        if not cleaned:
            continue
        if not cleaned.startswith("#"):
            cleaned = f"#{cleaned.lstrip('#')}"
        lowered = cleaned.lower()
        if lowered in seen:
            continue
        seen.add(lowered)
        normalized.append(cleaned)
    return normalized


def _as_float(value: Any) -> float | None:
    if value is None:
        return None
    text = str(value).strip()
    if not text:
        return None

    lowered = text.lower().replace(",", "")
    if lowered.endswith("%"):
        try:
            return float(lowered[:-1]) / 100.0
        except ValueError:
            return None

    try:
        return float(lowered)
    except ValueError:
        return None


def _clip_score(row: dict[str, Any]) -> float:
    views = float(row.get("views") or row.get("reach") or 0.0)
    if views <= 0:
        views = 1.0
    likes = float(row.get("likes") or 0.0)
    comments = float(row.get("comments") or 0.0)
    shares = float(row.get("shares") or 0.0)
    saves = float(row.get("saves") or 0.0)
    follows = float(row.get("follows") or 0.0)
    completion = float(row.get("completion_rate") or 0.0)
    return ((likes + comments * 2.0 + shares * 3.0 + saves * 3.0 + follows * 4.0) / views) + completion


def _top_categories(rows: list[dict[str, Any]]) -> list[dict[str, Any]]:
    grouped: dict[str, list[float]] = {}
    for row in rows:
        category = str(row.get("category") or "clip").strip() or "clip"
        grouped.setdefault(category, []).append(_clip_score(row))
    results = [
        {"category": category, "clip_count": len(scores), "avg_score": sum(scores) / len(scores)}
        for category, scores in grouped.items()
        if scores
    ]
    return sorted(results, key=lambda item: (-item["avg_score"], -item["clip_count"], item["category"]))


def _duration_bucket(value: Any) -> str:
    duration = _as_float(value)
    if duration is None:
        return "unknown"
    if duration < 20:
        return "0-19"
    if duration < 35:
        return "20-34"
    if duration < 50:
        return "35-49"
    if duration < 65:
        return "50-64"
    return "65+"


def _top_duration_buckets(rows: list[dict[str, Any]]) -> list[dict[str, Any]]:
    grouped: dict[str, list[float]] = {}
    for row in rows:
        bucket = _duration_bucket(row.get("duration_seconds"))
        grouped.setdefault(bucket, []).append(_clip_score(row))
    results = [
        {"bucket": bucket, "clip_count": len(scores), "avg_score": sum(scores) / len(scores)}
        for bucket, scores in grouped.items()
        if scores
    ]
    return sorted(results, key=lambda item: (-item["avg_score"], -item["clip_count"], item["bucket"]))


def _top_hashtags(rows: list[dict[str, Any]]) -> list[dict[str, Any]]:
    grouped: dict[str, list[float]] = {}
    for row in rows:
        for hashtag in json.loads(row.get("final_hashtags_json") or "[]"):
            grouped.setdefault(str(hashtag), []).append(_clip_score(row))
    results = [
        {"hashtag": hashtag, "clip_count": len(scores), "avg_score": sum(scores) / len(scores)}
        for hashtag, scores in grouped.items()
        if scores
    ]
    return sorted(results, key=lambda item: (-item["avg_score"], -item["clip_count"], item["hashtag"]))


def _top_clips(rows: list[dict[str, Any]]) -> list[dict[str, Any]]:
    ranked = sorted(
        rows,
        key=lambda row: (-_clip_score(row), str(row.get("clip_id") or "")),
    )
    result: list[dict[str, Any]] = []
    for row in ranked[:5]:
        result.append(
            {
                "clip_id": row.get("clip_id"),
                "category": row.get("category"),
                "score": round(_clip_score(row), 4),
                "views": row.get("views"),
                "likes": row.get("likes"),
                "comments": row.get("comments"),
                "shares": row.get("shares"),
                "saves": row.get("saves"),
            }
        )
    return result


def _build_lessons(rows: list[dict[str, Any]]) -> list[str]:
    lessons: list[str] = []
    categories = _top_categories(rows)
    durations = _top_duration_buckets(rows)
    hashtags = _top_hashtags(rows)

    if categories:
        top = categories[0]
        lessons.append(
            f"Favor {top['category']} clips when the transcript supports it; they are currently scoring best in imported metrics."
        )
    if durations:
        top = durations[0]
        lessons.append(
            f"The strongest duration bucket so far is {top['bucket']} seconds. Prefer that range when multiple cut lengths would work."
        )
    if hashtags:
        examples = ", ".join(entry["hashtag"] for entry in hashtags[:3])
        lessons.append(f"Reusable hashtag signal is emerging around {examples}. Keep them when they fit naturally.")
    if not lessons:
        lessons.append("No imported metrics yet. Import at least one CSV to generate clip performance guidance.")
    return lessons
