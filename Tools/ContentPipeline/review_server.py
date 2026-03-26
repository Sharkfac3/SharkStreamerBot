#!/usr/bin/env python3
"""Serve an ADHD-friendly one-clip-at-a-time review queue for formatted clips."""

from __future__ import annotations

import argparse
from datetime import UTC, datetime
import json
from pathlib import Path
import shutil
import sys
from typing import Any, Sequence

from config import load_settings


APP_TITLE = "Content Pipeline Review Queue"
DEFAULT_HOST = "127.0.0.1"
DEFAULT_PORT = 8765
QUEUE_STATUS_PENDING = "pending"
QUEUE_STATUS_SKIPPED = "skipped"
QUEUE_STATUS_REJECTED = "rejected"
QUEUE_STATUS_APPROVED = "approved"
ALLOWED_ACTIONS = {QUEUE_STATUS_SKIPPED, QUEUE_STATUS_REJECTED}


HTML_PAGE = """<!doctype html>
<html lang=\"en\">
<head>
  <meta charset=\"utf-8\" />
  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />
  <title>Content Review Queue</title>
  <style>
    :root {
      color-scheme: dark;
      --bg: #0b1020;
      --panel: #121a30;
      --panel-2: #182341;
      --text: #ecf2ff;
      --muted: #94a3c7;
      --accent: #7dd3fc;
      --good: #4ade80;
      --warn: #facc15;
      --bad: #fb7185;
      --border: #293557;
      --focus: #c084fc;
    }

    * { box-sizing: border-box; }
    body {
      margin: 0;
      font-family: Arial, Helvetica, sans-serif;
      background: linear-gradient(180deg, #09101f, #11182e 35%, #0b1020);
      color: var(--text);
      min-height: 100vh;
    }

    .wrap {
      max-width: 920px;
      margin: 0 auto;
      padding: 20px 16px 48px;
    }

    .topbar {
      display: flex;
      gap: 12px;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 16px;
      flex-wrap: wrap;
    }

    .title h1 {
      margin: 0;
      font-size: 1.5rem;
    }

    .title p {
      margin: 4px 0 0;
      color: var(--muted);
      font-size: 0.95rem;
    }

    .pill {
      background: rgba(125, 211, 252, 0.14);
      border: 1px solid rgba(125, 211, 252, 0.35);
      color: var(--accent);
      border-radius: 999px;
      padding: 8px 12px;
      font-size: 0.9rem;
      white-space: nowrap;
    }

    .panel {
      background: rgba(18, 26, 48, 0.88);
      border: 1px solid var(--border);
      border-radius: 16px;
      padding: 16px;
      box-shadow: 0 18px 60px rgba(0, 0, 0, 0.28);
      backdrop-filter: blur(10px);
    }

    .progress {
      display: flex;
      justify-content: space-between;
      align-items: center;
      gap: 12px;
      margin-bottom: 12px;
      flex-wrap: wrap;
    }

    .progress strong { font-size: 1rem; }

    .meta-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
      gap: 10px;
      margin: 12px 0 18px;
    }

    .meta-card {
      background: var(--panel-2);
      border: 1px solid var(--border);
      border-radius: 12px;
      padding: 10px 12px;
    }

    .meta-card .label {
      font-size: 0.8rem;
      color: var(--muted);
      text-transform: uppercase;
      letter-spacing: 0.04em;
    }

    .meta-card .value {
      margin-top: 4px;
      font-size: 1rem;
      line-height: 1.35;
      word-break: break-word;
    }

    video {
      width: 100%;
      max-height: 72vh;
      border-radius: 14px;
      background: #000;
      margin-bottom: 14px;
    }

    label {
      display: block;
      margin-bottom: 8px;
      font-weight: 700;
    }

    textarea, input[type=\"text\"] {
      width: 100%;
      border-radius: 12px;
      border: 1px solid var(--border);
      background: #0c1430;
      color: var(--text);
      padding: 12px 14px;
      font: inherit;
      margin-bottom: 14px;
    }

    textarea:focus, input[type=\"text\"]:focus, button:focus {
      outline: 2px solid var(--focus);
      outline-offset: 2px;
    }

    textarea { min-height: 96px; resize: vertical; }

    .button-row {
      display: grid;
      grid-template-columns: repeat(4, minmax(0, 1fr));
      gap: 10px;
      margin-bottom: 12px;
    }

    button {
      border: 1px solid var(--border);
      border-radius: 12px;
      padding: 14px 12px;
      font: inherit;
      font-weight: 700;
      cursor: pointer;
      color: var(--text);
      background: var(--panel-2);
    }

    button.approve { background: rgba(74, 222, 128, 0.18); border-color: rgba(74, 222, 128, 0.45); }
    button.skip { background: rgba(250, 204, 21, 0.18); border-color: rgba(250, 204, 21, 0.42); }
    button.reject { background: rgba(251, 113, 133, 0.18); border-color: rgba(251, 113, 133, 0.42); }
    button.edit { background: rgba(125, 211, 252, 0.18); border-color: rgba(125, 211, 252, 0.42); }

    .hint {
      color: var(--muted);
      font-size: 0.92rem;
      line-height: 1.45;
    }

    .status {
      min-height: 24px;
      margin-top: 6px;
      font-size: 0.95rem;
      color: var(--accent);
    }

    .empty {
      text-align: center;
      padding: 48px 20px;
      color: var(--muted);
    }

    .hotkeys {
      margin-top: 12px;
      display: flex;
      gap: 8px;
      flex-wrap: wrap;
    }

    .hotkeys span {
      background: #0c1430;
      border: 1px solid var(--border);
      border-radius: 999px;
      padding: 6px 10px;
      font-size: 0.85rem;
      color: var(--muted);
    }

    @media (max-width: 720px) {
      .button-row { grid-template-columns: 1fr 1fr; }
      .wrap { padding-left: 10px; padding-right: 10px; }
    }
  </style>
</head>
<body>
  <div class=\"wrap\">
    <div class=\"topbar\">
      <div class=\"title\">
        <h1>Review queue</h1>
        <p>One clip at a time. Keep the friction low.</p>
      </div>
      <div class=\"pill\" id=\"queueSummary\">Loading queue…</div>
    </div>

    <div class=\"panel\" id=\"panel\">
      <div class=\"empty\">Loading…</div>
    </div>
  </div>

  <script>
    const state = {
      queue: [],
      currentId: localStorage.getItem("contentPipeline.currentId") || null,
      loading: false,
    };

    async function fetchJson(url, options = {}) {
      const response = await fetch(url, {
        headers: { "Content-Type": "application/json" },
        ...options,
      });

      let payload = null;
      try {
        payload = await response.json();
      } catch (error) {
        payload = null;
      }

      if (!response.ok) {
        const message = payload && payload.detail ? payload.detail : `Request failed (${response.status})`;
        throw new Error(message);
      }

      return payload;
    }

    function escapeHtml(value) {
      return String(value ?? "")
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/\"/g, "&quot;")
        .replace(/'/g, "&#39;");
    }

    function currentIndex() {
      return state.queue.findIndex((item) => item.id === state.currentId);
    }

    function currentItem() {
      const index = currentIndex();
      return index >= 0 ? state.queue[index] : null;
    }

    function chooseCurrentItem() {
      if (state.queue.length === 0) {
        state.currentId = null;
        localStorage.removeItem("contentPipeline.currentId");
        return null;
      }

      if (!state.currentId || !state.queue.some((item) => item.id === state.currentId)) {
        state.currentId = state.queue[0].id;
      }

      localStorage.setItem("contentPipeline.currentId", state.currentId);
      return currentItem();
    }

    function summaryText() {
      if (state.queue.length === 0) {
        return "Queue clear";
      }

      const index = Math.max(0, currentIndex());
      return `${index + 1} of ${state.queue.length} pending`;
    }

    function setStatus(message, isError = false) {
      const node = document.getElementById("statusMessage");
      if (!node) {
        return;
      }
      node.textContent = message || "";
      node.style.color = isError ? "#fb7185" : "#7dd3fc";
    }

    function renderEmpty() {
      document.getElementById("queueSummary").textContent = "Queue clear";
      document.getElementById("panel").innerHTML = `
        <div class=\"empty\">
          <h2 style=\"margin-top: 0; color: #ecf2ff;\">Nothing waiting for review</h2>
          <p>The review queue is empty. Run <code>format_instagram.py</code> to create new review-ready clips.</p>
        </div>
      `;
    }

    function renderItem(item) {
      document.getElementById("queueSummary").textContent = summaryText();

      const progressLabel = `${Math.max(1, currentIndex() + 1)} of ${state.queue.length}`;
      const hashtagsValue = (item.hashtags || []).join(" ");
      const description = item.description || "—";
      const suggested = item.suggested_caption || "—";

      document.getElementById("panel").innerHTML = `
        <div class=\"progress\">
          <strong>${escapeHtml(progressLabel)}</strong>
          <span class=\"hint\">Auto-play is on. Approve moves it to published.</span>
        </div>

        <video id=\"reviewVideo\" controls autoplay playsinline>
          <source src=\"${escapeHtml(item.video_url)}\" type=\"video/mp4\" />
        </video>

        <div class=\"meta-grid\">
          <div class=\"meta-card\"><div class=\"label\">Clip</div><div class=\"value\">${escapeHtml(item.file_name)}</div></div>
          <div class=\"meta-card\"><div class=\"label\">Category</div><div class=\"value\">${escapeHtml(item.category)}</div></div>
          <div class=\"meta-card\"><div class=\"label\">Confidence</div><div class=\"value\">${escapeHtml(item.confidence_score)}</div></div>
          <div class=\"meta-card\"><div class=\"label\">Duration</div><div class=\"value\">${escapeHtml(item.duration_label)}</div></div>
        </div>

        <div class=\"hint\" style=\"margin-bottom: 12px;\"><strong>Description:</strong> ${escapeHtml(description)}</div>
        <div class=\"hint\" style=\"margin-bottom: 16px;\"><strong>Suggested:</strong> ${escapeHtml(suggested)}</div>

        <label for=\"captionInput\">Caption</label>
        <textarea id=\"captionInput\" spellcheck=\"true\">${escapeHtml(item.caption)}</textarea>

        <label for=\"hashtagsInput\">Hashtags</label>
        <input id=\"hashtagsInput\" type=\"text\" value=\"${escapeHtml(hashtagsValue)}\" spellcheck=\"false\" />

        <div class=\"button-row\">
          <button class=\"approve\" id=\"approveButton\">Approve (A)</button>
          <button class=\"skip\" id=\"skipButton\">Skip (S)</button>
          <button class=\"reject\" id=\"rejectButton\">Trash (T)</button>
          <button class=\"edit\" id=\"saveButton\">Save edits (Ctrl+Enter)</button>
        </div>

        <div class=\"status\" id=\"statusMessage\"></div>

        <div class=\"hotkeys\">
          <span>A = approve</span>
          <span>S = skip</span>
          <span>T = trash</span>
          <span>E = focus caption</span>
          <span>Ctrl+Enter = save edits</span>
        </div>
      `;

      document.getElementById("approveButton").addEventListener("click", () => runAction("approve"));
      document.getElementById("skipButton").addEventListener("click", () => runAction("skip"));
      document.getElementById("rejectButton").addEventListener("click", () => runAction("reject"));
      document.getElementById("saveButton").addEventListener("click", () => saveEdits(false));

      const video = document.getElementById("reviewVideo");
      video.play().catch(() => {});
    }

    async function loadQueue(preferredId = null) {
      const payload = await fetchJson("/api/queue");
      state.queue = payload.items || [];
      if (preferredId) {
        state.currentId = preferredId;
      }
      const item = chooseCurrentItem();
      if (!item) {
        renderEmpty();
        return;
      }
      renderItem(item);
    }

    function buildEditPayload() {
      const item = currentItem();
      if (!item) {
        throw new Error("No current item loaded.");
      }

      const caption = document.getElementById("captionInput").value.trim();
      const hashtagsRaw = document.getElementById("hashtagsInput").value.trim();
      const hashtags = hashtagsRaw ? hashtagsRaw.split(/\\s+/).filter(Boolean) : [];
      return { caption, hashtags };
    }

    async function saveEdits(showStatus = true) {
      const item = currentItem();
      if (!item) {
        return;
      }

      const payload = buildEditPayload();
      const updated = await fetchJson(`/api/items/${encodeURIComponent(item.id)}/save`, {
        method: "POST",
        body: JSON.stringify(payload),
      });

      const queueItem = state.queue.find((entry) => entry.id === item.id);
      if (queueItem) {
        queueItem.caption = updated.item.caption;
        queueItem.hashtags = updated.item.hashtags;
      }

      if (showStatus) {
        setStatus("Saved edits.");
      }
    }

    async function runAction(action) {
      if (state.loading) {
        return;
      }

      const item = currentItem();
      if (!item) {
        return;
      }

      state.loading = true;
      setStatus(action === "approve" ? "Approving…" : action === "skip" ? "Skipping…" : "Sending to trash…");

      try {
        const edits = buildEditPayload();
        const endpoint = action === "approve"
          ? `/api/items/${encodeURIComponent(item.id)}/approve`
          : `/api/items/${encodeURIComponent(item.id)}/action`;
        const payload = action === "approve"
          ? edits
          : { ...edits, action: action === "skip" ? "skipped" : "rejected" };

        await fetchJson(endpoint, {
          method: "POST",
          body: JSON.stringify(payload),
        });

        const nextIndex = currentIndex();
        state.queue = state.queue.filter((entry) => entry.id !== item.id);
        if (state.queue.length === 0) {
          state.currentId = null;
          renderEmpty();
        } else {
          const nextItem = state.queue[Math.min(nextIndex, state.queue.length - 1)];
          state.currentId = nextItem.id;
          localStorage.setItem("contentPipeline.currentId", state.currentId);
          renderItem(nextItem);
          setStatus(action === "approve" ? "Approved and moved to published." : action === "skip" ? "Skipped for now." : "Marked as trash.");
        }
      } catch (error) {
        setStatus(error.message || String(error), true);
      } finally {
        state.loading = false;
      }
    }

    document.addEventListener("keydown", async (event) => {
      const active = document.activeElement;
      const isTyping = active && (active.tagName === "TEXTAREA" || active.tagName === "INPUT");

      if (event.key === "e" || event.key === "E") {
        event.preventDefault();
        const caption = document.getElementById("captionInput");
        if (caption) {
          caption.focus();
          caption.select();
        }
        return;
      }

      if (event.key === "Enter" && event.ctrlKey) {
        event.preventDefault();
        try {
          await saveEdits(true);
        } catch (error) {
          setStatus(error.message || String(error), true);
        }
        return;
      }

      if (isTyping) {
        return;
      }

      if (event.key === "a" || event.key === "A") {
        event.preventDefault();
        runAction("approve");
      } else if (event.key === "s" || event.key === "S") {
        event.preventDefault();
        runAction("skip");
      } else if (event.key === "t" || event.key === "T") {
        event.preventDefault();
        runAction("reject");
      }
    });

    loadQueue().catch((error) => {
      document.getElementById("queueSummary").textContent = "Queue error";
      document.getElementById("panel").innerHTML = `<div class=\"empty\"><strong>Could not load queue.</strong><p>${escapeHtml(error.message || String(error))}</p></div>`;
    });
  </script>
</body>
</html>
"""


def build_parser() -> argparse.ArgumentParser:
    """Build the CLI parser."""
    parser = argparse.ArgumentParser(description="Serve the ContentPipeline review queue web UI")
    parser.add_argument("--host", default=DEFAULT_HOST, help=f"Bind host (default: {DEFAULT_HOST})")
    parser.add_argument("--port", type=int, default=DEFAULT_PORT, help=f"Bind port (default: {DEFAULT_PORT})")
    parser.add_argument("--review-queue-dir", type=Path, help="Override review queue directory.")
    parser.add_argument("--published-dir", type=Path, help="Override published output directory.")
    parser.add_argument("--reload", action="store_true", help="Enable uvicorn auto-reload for local development.")
    return parser


def parse_args() -> argparse.Namespace:
    """Parse command-line arguments."""
    return build_parser().parse_args()


def parse_args_from(argv: Sequence[str]) -> argparse.Namespace:
    """Parse arguments from an explicit argv list for tests."""
    return build_parser().parse_args(list(argv))


def create_app(*, review_queue_dir: Path | None = None, published_dir: Path | None = None) -> Any:
    """Create the FastAPI app lazily so --help works before dependencies are installed."""
    try:
        from fastapi import FastAPI, HTTPException
        from fastapi.responses import FileResponse, HTMLResponse
    except ImportError as error:
        raise RuntimeError(
            "FastAPI is not installed. Run 'pip install -r Tools/ContentPipeline/requirements.txt'."
        ) from error

    settings = load_settings()
    settings.ensure_data_dirs()
    queue_dir = (review_queue_dir or settings.review_queue_dir).resolve()
    published_output_dir = (published_dir or settings.published_dir).resolve()
    queue_dir.mkdir(parents=True, exist_ok=True)
    published_output_dir.mkdir(parents=True, exist_ok=True)

    app = FastAPI(title=APP_TITLE)

    @app.get("/", response_class=HTMLResponse)
    def index() -> str:
        return HTML_PAGE

    @app.get("/api/queue")
    def get_queue() -> dict[str, Any]:
        items = list_queue_items(queue_dir)
        return {
            "items": [build_queue_item_summary(item) for item in items],
            "pending_count": len(items),
            "review_queue_dir": str(queue_dir),
            "published_dir": str(published_output_dir),
        }

    @app.get("/api/items/{item_id}")
    def get_item(item_id: str) -> dict[str, Any]:
        try:
            item = load_queue_item(queue_dir, item_id)
        except FileNotFoundError as error:
            raise HTTPException(status_code=404, detail=str(error)) from error
        return {"item": build_queue_item_summary(item)}

    @app.post("/api/items/{item_id}/save")
    def save_item(item_id: str, payload: dict[str, Any]) -> dict[str, Any]:
        try:
            item = load_queue_item(queue_dir, item_id)
            update_instagram_metadata(item["payload"], payload)
            stamp_review_state(item["payload"], status=current_review_status(item["payload"]))
            persist_queue_item(item)
        except FileNotFoundError as error:
            raise HTTPException(status_code=404, detail=str(error)) from error
        except ValueError as error:
            raise HTTPException(status_code=400, detail=str(error)) from error

        item = load_queue_item(queue_dir, item_id)
        return {"item": build_queue_item_summary(item)}

    @app.post("/api/items/{item_id}/action")
    def action_item(item_id: str, payload: dict[str, Any]) -> dict[str, Any]:
        action = str(payload.get("action") or "").strip().lower()
        if action not in ALLOWED_ACTIONS:
            raise HTTPException(status_code=400, detail="Action must be 'skipped' or 'rejected'.")

        try:
            item = load_queue_item(queue_dir, item_id)
            update_instagram_metadata(item["payload"], payload)
            stamp_review_state(item["payload"], status=action)
            persist_queue_item(item)
        except FileNotFoundError as error:
            raise HTTPException(status_code=404, detail=str(error)) from error
        except ValueError as error:
            raise HTTPException(status_code=400, detail=str(error)) from error

        return {"ok": True, "action": action}

    @app.post("/api/items/{item_id}/approve")
    def approve_item(item_id: str, payload: dict[str, Any]) -> dict[str, Any]:
        try:
            item = load_queue_item(queue_dir, item_id)
            update_instagram_metadata(item["payload"], payload)
            stamp_review_state(item["payload"], status=QUEUE_STATUS_APPROVED)
            finalize_and_move_to_published(item, published_output_dir)
        except FileNotFoundError as error:
            raise HTTPException(status_code=404, detail=str(error)) from error
        except ValueError as error:
            raise HTTPException(status_code=400, detail=str(error)) from error

        return {"ok": True, "action": QUEUE_STATUS_APPROVED}

    @app.get("/media/review/{file_name}")
    def review_media(file_name: str) -> Any:
        path = safe_media_path(queue_dir, file_name)
        if not path.exists() or not path.is_file():
            raise HTTPException(status_code=404, detail=f"Review media not found: {file_name}")
        return FileResponse(path, media_type="video/mp4", filename=path.name)

    @app.get("/media/published/{file_name}")
    def published_media(file_name: str) -> Any:
        path = safe_media_path(published_output_dir, file_name)
        if not path.exists() or not path.is_file():
            raise HTTPException(status_code=404, detail=f"Published media not found: {file_name}")
        return FileResponse(path, media_type="video/mp4", filename=path.name)

    return app


def list_queue_items(queue_dir: Path) -> list[dict[str, Any]]:
    """Load pending review items from the review queue directory."""
    items: list[dict[str, Any]] = []
    for meta_path in sorted(queue_dir.glob("*_meta.json")):
        try:
            item = load_queue_item(queue_dir, meta_path.name[:-10])
        except Exception:
            continue

        status = current_review_status(item["payload"])
        if status in {QUEUE_STATUS_PENDING, QUEUE_STATUS_SKIPPED}:
            items.append(item)

    items.sort(key=queue_sort_key)
    return items


def queue_sort_key(item: dict[str, Any]) -> tuple[int, str, str]:
    """Prioritize never-reviewed items before skipped ones, then stable by filename."""
    status = current_review_status(item["payload"])
    rank = 0 if status == QUEUE_STATUS_PENDING else 1
    generated = str(item["payload"].get("generated_at_utc") or "")
    return (rank, generated, item["id"])


def load_queue_item(queue_dir: Path, item_id: str) -> dict[str, Any]:
    """Load one queue item's metadata and associated video path."""
    safe_id = sanitize_item_id(item_id)
    meta_path = queue_dir / f"{safe_id}_meta.json"
    if not meta_path.exists():
        raise FileNotFoundError(f"Queue item not found: {item_id}")

    try:
        payload = json.loads(meta_path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as error:
        raise ValueError(f"Invalid JSON in queue metadata {meta_path}: {error}") from error

    if not isinstance(payload, dict):
        raise ValueError(f"Queue metadata must be a JSON object: {meta_path}")

    review_output = payload.get("review_output") if isinstance(payload.get("review_output"), dict) else {}
    video_file_name = str(review_output.get("video_file_name") or f"{safe_id}.mp4").strip()
    video_path = queue_dir / video_file_name

    return {
        "id": safe_id,
        "meta_path": meta_path,
        "video_path": video_path,
        "payload": payload,
    }


def build_queue_item_summary(item: dict[str, Any]) -> dict[str, Any]:
    """Build a compact payload for the browser UI."""
    payload = item["payload"]
    instagram = payload.get("instagram") if isinstance(payload.get("instagram"), dict) else {}
    source_clip = payload.get("source_clip") if isinstance(payload.get("source_clip"), dict) else {}
    review_output = payload.get("review_output") if isinstance(payload.get("review_output"), dict) else {}
    duration = source_clip.get("clip_duration_seconds") or review_output.get("duration_seconds")

    return {
        "id": item["id"],
        "file_name": item["video_path"].name,
        "video_url": f"/media/review/{item['video_path'].name}",
        "caption": str(instagram.get("caption") or "").strip(),
        "hashtags": normalize_hashtags(instagram.get("hashtags")),
        "category": str(source_clip.get("category") or instagram.get("category") or "clip").strip(),
        "description": str(source_clip.get("description") or "").strip(),
        "suggested_caption": str(source_clip.get("suggested_caption") or "").strip(),
        "confidence_score": format_confidence(source_clip.get("confidence_score")),
        "duration_label": format_duration_label(duration),
        "status": current_review_status(payload),
    }


def update_instagram_metadata(payload: dict[str, Any], edits: dict[str, Any]) -> None:
    """Apply operator caption/hashtag edits to a queue metadata payload."""
    if not isinstance(payload, dict):
        raise ValueError("Queue payload must be a JSON object.")

    instagram = payload.setdefault("instagram", {})
    if not isinstance(instagram, dict):
        raise ValueError("Queue payload instagram field must be an object.")

    if "caption" in edits:
        caption = " ".join(str(edits.get("caption") or "").split())
        instagram["caption"] = caption
    else:
        caption = " ".join(str(instagram.get("caption") or "").split())
        instagram["caption"] = caption

    if "hashtags" in edits:
        instagram["hashtags"] = normalize_hashtags(edits.get("hashtags"))
    else:
        instagram["hashtags"] = normalize_hashtags(instagram.get("hashtags"))

    instagram["caption_with_hashtags"] = build_caption_with_hashtags(instagram.get("caption"), instagram.get("hashtags"))


def stamp_review_state(payload: dict[str, Any], *, status: str) -> None:
    """Update in-place review workflow metadata."""
    if status not in {QUEUE_STATUS_PENDING, QUEUE_STATUS_SKIPPED, QUEUE_STATUS_REJECTED, QUEUE_STATUS_APPROVED}:
        raise ValueError(f"Unsupported review status: {status}")

    review_state = payload.setdefault("review_state", {})
    if not isinstance(review_state, dict):
        raise ValueError("review_state must be an object when present.")

    previous_status = str(review_state.get("status") or current_review_status(payload)).strip().lower()
    review_state["status"] = status
    review_state["updated_at_utc"] = datetime.now(UTC).isoformat()
    if not review_state.get("created_at_utc"):
        review_state["created_at_utc"] = review_state["updated_at_utc"]
    if status == QUEUE_STATUS_APPROVED:
        review_state["approved_at_utc"] = review_state["updated_at_utc"]
    if status == QUEUE_STATUS_REJECTED:
        review_state["rejected_at_utc"] = review_state["updated_at_utc"]
    if status == QUEUE_STATUS_SKIPPED:
        review_state["skipped_at_utc"] = review_state["updated_at_utc"]
    review_state["previous_status"] = previous_status

    review_output = payload.setdefault("review_output", {})
    if isinstance(review_output, dict):
        review_output["status"] = status


def current_review_status(payload: dict[str, Any]) -> str:
    """Return the queue item's current workflow status."""
    review_state = payload.get("review_state") if isinstance(payload.get("review_state"), dict) else {}
    status = str(review_state.get("status") or "").strip().lower()
    if status in {QUEUE_STATUS_PENDING, QUEUE_STATUS_SKIPPED, QUEUE_STATUS_REJECTED, QUEUE_STATUS_APPROVED}:
        return status

    review_output = payload.get("review_output") if isinstance(payload.get("review_output"), dict) else {}
    output_status = str(review_output.get("status") or "").strip().lower()
    if output_status == "ready_for_review":
        return QUEUE_STATUS_PENDING
    if output_status in {QUEUE_STATUS_PENDING, QUEUE_STATUS_SKIPPED, QUEUE_STATUS_REJECTED, QUEUE_STATUS_APPROVED}:
        return output_status
    return QUEUE_STATUS_PENDING


def persist_queue_item(item: dict[str, Any]) -> None:
    """Write queue metadata back to disk."""
    item["meta_path"].write_text(json.dumps(item["payload"], indent=2, ensure_ascii=False) + "\n", encoding="utf-8")


def finalize_and_move_to_published(item: dict[str, Any], published_dir: Path) -> None:
    """Move an approved item from review_queue/ into published/."""
    meta_path = item["meta_path"]
    video_path = item["video_path"]
    if not video_path.exists():
        raise FileNotFoundError(f"Review video not found: {video_path.name}")

    payload = item["payload"]
    source_clip = payload.get("source_clip") if isinstance(payload.get("source_clip"), dict) else {}
    instagram = payload.get("instagram") if isinstance(payload.get("instagram"), dict) else {}

    destination_video = published_dir / video_path.name
    destination_meta = published_dir / meta_path.name
    if destination_video.exists() or destination_meta.exists():
        raise ValueError(
            f"Published output already exists for {video_path.name}. Remove it first or rename the review item."
        )

    published_record = payload.setdefault("published", {})
    if not isinstance(published_record, dict):
        raise ValueError("published must be an object when present.")

    approved_at = datetime.now(UTC).isoformat()
    published_record.update(
        {
            "approved_at_utc": approved_at,
            "video_file_name": destination_video.name,
            "video_path": str(destination_video),
            "metadata_file_name": destination_meta.name,
            "metadata_path": str(destination_meta),
            "final_caption": str(instagram.get("caption") or "").strip(),
            "final_hashtags": normalize_hashtags(instagram.get("hashtags")),
            "category": str(source_clip.get("category") or instagram.get("category") or "clip").strip(),
        }
    )

    review_output = payload.setdefault("review_output", {})
    if isinstance(review_output, dict):
        review_output["status"] = QUEUE_STATUS_APPROVED
        review_output["video_path"] = str(destination_video)
        review_output["metadata_path"] = str(destination_meta)

    persist_queue_item(item)
    published_dir.mkdir(parents=True, exist_ok=True)
    shutil.move(str(video_path), str(destination_video))
    shutil.move(str(meta_path), str(destination_meta))


def sanitize_item_id(value: str) -> str:
    """Restrict item IDs to safe filename-ish values."""
    cleaned = "".join(ch for ch in str(value or "") if ch.isalnum() or ch in {"-", "_"})
    if not cleaned:
        raise ValueError("Invalid queue item id.")
    return cleaned


def safe_media_path(base_dir: Path, file_name: str) -> Path:
    """Resolve a media path without allowing directory traversal."""
    cleaned = Path(file_name).name
    if cleaned != file_name:
        raise ValueError("Invalid media file name.")
    return base_dir / cleaned


def normalize_hashtags(value: Any) -> list[str]:
    """Normalize hashtag input into a deduplicated list."""
    raw_items: list[str]
    if isinstance(value, list):
        raw_items = [str(item) for item in value]
    elif value is None:
        raw_items = []
    else:
        raw_items = str(value).split()

    result: list[str] = []
    seen: set[str] = set()
    for raw in raw_items:
        cleaned = str(raw or "").strip()
        if not cleaned:
            continue
        if not cleaned.startswith("#"):
            cleaned = f"#{cleaned.lstrip('#')}"
        if cleaned == "#":
            continue
        lowered = cleaned.lower()
        if lowered in seen:
            continue
        seen.add(lowered)
        result.append(cleaned)
    return result


def build_caption_with_hashtags(caption: Any, hashtags: Any) -> str:
    """Return a human-friendly caption bundle for copy/paste."""
    clean_caption = " ".join(str(caption or "").split())
    clean_hashtags = normalize_hashtags(hashtags)
    if clean_caption and clean_hashtags:
        return f"{clean_caption}\n\n{' '.join(clean_hashtags)}"
    if clean_hashtags:
        return " ".join(clean_hashtags)
    return clean_caption


def format_confidence(value: Any) -> str:
    """Format confidence scores for the UI."""
    try:
        return f"{float(value):.2f}"
    except (TypeError, ValueError):
        return "—"


def format_duration_label(value: Any) -> str:
    """Format clip duration as M:SS."""
    try:
        total_seconds = max(0, int(round(float(value))))
    except (TypeError, ValueError):
        return "—"
    minutes, seconds = divmod(total_seconds, 60)
    return f"{minutes}:{seconds:02d}"


def main(argv: Sequence[str] | None = None) -> int:
    args = parse_args() if argv is None else parse_args_from(argv)

    if args.port <= 0 or args.port > 65535:
        print("[review] Configuration error: --port must be between 1 and 65535", file=sys.stderr)
        return 1

    try:
        app = create_app(review_queue_dir=args.review_queue_dir, published_dir=args.published_dir)
    except Exception as error:
        print(f"[review] Configuration error: {error}", file=sys.stderr)
        return 1

    try:
        import uvicorn
    except ImportError:
        print(
            "[review] Missing dependency: uvicorn is not installed. Run 'pip install -r Tools/ContentPipeline/requirements.txt'.",
            file=sys.stderr,
        )
        return 1

    review_queue_dir = (args.review_queue_dir or load_settings().review_queue_dir).resolve()
    published_dir = (args.published_dir or load_settings().published_dir).resolve()
    print(f"[review] Review queue: {review_queue_dir}")
    print(f"[review] Published dir: {published_dir}")
    print(f"[review] Serving http://{args.host}:{args.port}")
    print("Review server started → http://localhost:8000")
    print("Press Ctrl+C to stop.")

    uvicorn.run(app, host=args.host, port=args.port, reload=args.reload)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
