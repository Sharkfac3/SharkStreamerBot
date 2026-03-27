#!/usr/bin/env python3
"""Review and publish generated art candidates."""

from __future__ import annotations

import argparse
from datetime import UTC, datetime
import html
import json
from pathlib import Path
import shutil
import sys
from typing import Any, Sequence

from config import load_settings
from lib.manifest_io import read_manifest, timestamp_utc, write_manifest
from lib.naming import asset_filename


APP_TITLE = "Art Pipeline Review"
DEFAULT_HOST = "127.0.0.1"
IMAGE_STATUS_PENDING = "pending_review"
IMAGE_STATUS_APPROVED = "approved"
IMAGE_STATUS_REJECTED = "rejected"
SPEC_STATUS_PENDING = "pending_review"
SPEC_STATUS_APPROVED = "approved"
SPEC_STATUS_PUBLISHED = "published"
SPEC_STATUS_REGEN = "needs_regeneration"
SPEC_STATUS_FAILED = "generation_failed"
FINAL_SPEC_STATUSES = {SPEC_STATUS_APPROVED, SPEC_STATUS_PUBLISHED, SPEC_STATUS_REGEN, SPEC_STATUS_FAILED}
ALLOWED_EXTENSIONS = {".png", ".jpg", ".jpeg", ".webp", ".gif"}


HTML_SHELL = """<!doctype html>
<html lang=\"en\">
<head>
  <meta charset=\"utf-8\" />
  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />
  <title>{title}</title>
  <style>
    :root {{
      color-scheme: dark;
      --bg: #09101b;
      --bg-2: #0e1728;
      --panel: rgba(18, 27, 46, 0.94);
      --panel-2: #16233c;
      --panel-3: #1b2c4a;
      --text: #eef4ff;
      --muted: #9cb0d3;
      --border: #2b3b5f;
      --accent: #7dd3fc;
      --accent-2: #c084fc;
      --good: #4ade80;
      --warn: #facc15;
      --bad: #fb7185;
      --shadow: rgba(0, 0, 0, 0.35);
    }}
    * {{ box-sizing: border-box; }}
    body {{
      margin: 0;
      font-family: Arial, Helvetica, sans-serif;
      background: linear-gradient(180deg, #08101b, #0d1526 35%, #09101b);
      color: var(--text);
      min-height: 100vh;
    }}
    a {{ color: var(--accent); text-decoration: none; }}
    a:hover {{ text-decoration: underline; }}
    code {{ font-family: Consolas, monospace; background: #0b1430; padding: 2px 6px; border-radius: 6px; }}
    .wrap {{ max-width: 1500px; margin: 0 auto; padding: 22px 16px 40px; }}
    .topbar {{ display: flex; justify-content: space-between; gap: 16px; flex-wrap: wrap; align-items: flex-start; margin-bottom: 18px; }}
    h1, h2, h3, p {{ margin-top: 0; }}
    .title p {{ color: var(--muted); margin-bottom: 0; }}
    .panel {{
      background: var(--panel);
      border: 1px solid var(--border);
      border-radius: 18px;
      padding: 16px;
      box-shadow: 0 18px 60px var(--shadow);
      backdrop-filter: blur(10px);
    }}
    .meta-grid, .stats-grid, .spec-grid, .candidate-grid {{ display: grid; gap: 14px; }}
    .meta-grid {{ grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); margin-top: 14px; }}
    .stats-grid {{ grid-template-columns: repeat(auto-fit, minmax(120px, 1fr)); margin-bottom: 18px; }}
    .spec-grid {{ grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); }}
    .candidate-grid {{ grid-template-columns: repeat(auto-fit, minmax(420px, 1fr)); margin-top: 18px; }}
    .card {{ background: var(--panel-2); border: 1px solid var(--border); border-radius: 16px; overflow: hidden; }}
    .card-body {{ padding: 14px; }}
    .meta-card, .stat-card {{ background: var(--panel-2); border: 1px solid var(--border); border-radius: 14px; padding: 12px; }}
    .label {{ font-size: 0.78rem; text-transform: uppercase; letter-spacing: 0.04em; color: var(--muted); margin-bottom: 6px; }}
    .value {{ font-size: 1rem; line-height: 1.4; word-break: break-word; }}
    .badge {{ display: inline-flex; align-items: center; gap: 8px; border-radius: 999px; padding: 7px 12px; font-size: 0.9rem; font-weight: 700; border: 1px solid transparent; }}
    .badge.pending_review {{ color: var(--accent); background: rgba(125, 211, 252, 0.13); border-color: rgba(125, 211, 252, 0.32); }}
    .badge.approved {{ color: var(--good); background: rgba(74, 222, 128, 0.14); border-color: rgba(74, 222, 128, 0.34); }}
    .badge.published {{ color: #86efac; background: rgba(34, 197, 94, 0.18); border-color: rgba(34, 197, 94, 0.36); }}
    .badge.needs_regeneration {{ color: var(--warn); background: rgba(250, 204, 21, 0.14); border-color: rgba(250, 204, 21, 0.34); }}
    .badge.generation_failed, .badge.rejected {{ color: var(--bad); background: rgba(251, 113, 133, 0.14); border-color: rgba(251, 113, 133, 0.34); }}
    .thumb {{ width: 100%; aspect-ratio: 4 / 5; background: #060a14; display: block; object-fit: contain; }}
    .candidate-image {{ width: 100%; min-height: 420px; max-height: 75vh; object-fit: contain; background: #04070e; display: block; }}
    .candidate-card {{ position: relative; transition: transform 0.12s ease, border-color 0.12s ease, box-shadow 0.12s ease; cursor: pointer; }}
    .candidate-card:hover {{ transform: translateY(-2px); }}
    .candidate-card.selected {{ border-color: var(--accent-2); box-shadow: 0 0 0 2px rgba(192, 132, 252, 0.45); }}
    .candidate-card.approved {{ border-color: rgba(74, 222, 128, 0.5); }}
    .candidate-card.rejected {{ opacity: 0.82; }}
    .candidate-card .radio {{ position: absolute; top: 12px; right: 12px; width: 18px; height: 18px; border-radius: 999px; border: 2px solid white; background: rgba(8, 16, 27, 0.85); }}
    .candidate-card.selected .radio {{ background: var(--accent-2); border-color: var(--accent-2); box-shadow: 0 0 0 4px rgba(192, 132, 252, 0.18); }}
    .button-row {{ display: flex; flex-wrap: wrap; gap: 10px; margin-top: 18px; }}
    button {{
      border: 1px solid var(--border);
      border-radius: 12px;
      padding: 12px 16px;
      font: inherit;
      font-weight: 700;
      cursor: pointer;
      color: var(--text);
      background: var(--panel-3);
    }}
    button:hover {{ filter: brightness(1.07); }}
    button:disabled {{ opacity: 0.45; cursor: not-allowed; }}
    button.approve {{ background: rgba(74, 222, 128, 0.18); border-color: rgba(74, 222, 128, 0.4); }}
    button.reject {{ background: rgba(251, 113, 133, 0.18); border-color: rgba(251, 113, 133, 0.4); }}
    button.regen {{ background: rgba(250, 204, 21, 0.18); border-color: rgba(250, 204, 21, 0.4); }}
    button.publish {{ background: rgba(125, 211, 252, 0.18); border-color: rgba(125, 211, 252, 0.4); }}
    .hint {{ color: var(--muted); line-height: 1.5; }}
    .status {{ min-height: 24px; margin-top: 12px; color: var(--accent); font-weight: 700; }}
    .status.error {{ color: var(--bad); }}
    .inline-list {{ margin: 0; padding-left: 18px; color: var(--muted); }}
    .header-actions {{ display: flex; gap: 10px; flex-wrap: wrap; }}
    .approved-panel {{ margin-top: 18px; display: none; }}
    .approved-panel.visible {{ display: block; }}
    .approved-preview {{ width: min(540px, 100%); border-radius: 14px; border: 1px solid var(--border); background: #04070e; display: block; margin-bottom: 14px; }}
    .empty {{ text-align: center; color: var(--muted); padding: 38px 20px; }}
    @media (max-width: 860px) {{
      .candidate-grid {{ grid-template-columns: 1fr; }}
      .candidate-image {{ min-height: 320px; }}
    }}
  </style>
</head>
<body>
  <div class=\"wrap\">{body}</div>
</body>
</html>
"""


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description="Review and publish art pipeline candidates")
    parser.add_argument("batch_id", help="Batch id of the candidate manifest")
    parser.add_argument("--host", default=DEFAULT_HOST, help=f"Bind host (default: {DEFAULT_HOST})")
    parser.add_argument("--port", type=int, help="Bind port (default: config review_port)")
    parser.add_argument(
        "--approve",
        help="Approve and publish the specified candidate filename immediately, then exit",
    )
    parser.add_argument("--reload", action="store_true", help="Enable uvicorn auto-reload for local development")
    return parser


def parse_args() -> argparse.Namespace:
    return build_parser().parse_args()


def parse_args_from(argv: Sequence[str]) -> argparse.Namespace:
    return build_parser().parse_args(list(argv))


def create_app(batch_id: str) -> Any:
    try:
        from fastapi import FastAPI, HTTPException
        from fastapi.responses import FileResponse, HTMLResponse
    except ImportError as error:
        raise RuntimeError(
            "FastAPI is not installed. Run 'pip install -r Tools/ArtPipeline/requirements.txt'."
        ) from error

    settings = load_settings()
    app = FastAPI(title=APP_TITLE)

    @app.get("/", response_class=HTMLResponse)
    def dashboard() -> str:
        state = load_batch_state(settings, batch_id)
        return render_dashboard_page(state)

    @app.get("/spec/{spec_id}", response_class=HTMLResponse)
    def spec_page(spec_id: str) -> str:
        state = load_batch_state(settings, batch_id)
        spec = get_spec_or_raise(state, spec_id)
        return render_spec_page(state, spec)

    @app.get("/images/{image_path:path}")
    def image_file(image_path: str) -> Any:
        path = resolve_tool_relative_file(settings, image_path)
        if not path.exists() or not path.is_file():
            raise HTTPException(status_code=404, detail=f"Image not found: {image_path}")
        if path.suffix.lower() not in ALLOWED_EXTENSIONS:
            raise HTTPException(status_code=400, detail="Unsupported image type.")
        return FileResponse(path, filename=path.name)

    @app.post("/spec/{spec_id}/approve")
    def approve_spec(spec_id: str, payload: dict[str, Any]) -> dict[str, Any]:
        filename = str(payload.get("filename") or "").strip()
        if not filename:
            raise HTTPException(status_code=400, detail="filename is required.")
        try:
            result = approve_candidate(settings, batch_id, spec_id, filename)
        except FileNotFoundError as error:
            raise HTTPException(status_code=404, detail=str(error)) from error
        except ValueError as error:
            raise HTTPException(status_code=400, detail=str(error)) from error
        return result

    @app.post("/spec/{spec_id}/reject")
    def reject_spec(spec_id: str, payload: dict[str, Any]) -> dict[str, Any]:
        filename = str(payload.get("filename") or "").strip()
        if not filename:
            raise HTTPException(status_code=400, detail="filename is required.")
        try:
            result = reject_candidate(settings, batch_id, spec_id, filename)
        except FileNotFoundError as error:
            raise HTTPException(status_code=404, detail=str(error)) from error
        except ValueError as error:
            raise HTTPException(status_code=400, detail=str(error)) from error
        return result

    @app.post("/spec/{spec_id}/regenerate")
    def regenerate_spec(spec_id: str) -> dict[str, Any]:
        try:
            result = mark_for_regeneration(settings, batch_id, spec_id)
        except FileNotFoundError as error:
            raise HTTPException(status_code=404, detail=str(error)) from error
        except ValueError as error:
            raise HTTPException(status_code=400, detail=str(error)) from error
        return result

    @app.post("/spec/{spec_id}/publish")
    def publish_spec(spec_id: str) -> dict[str, Any]:
        try:
            result = publish_spec_asset(settings, batch_id, spec_id)
        except FileNotFoundError as error:
            raise HTTPException(status_code=404, detail=str(error)) from error
        except ValueError as error:
            raise HTTPException(status_code=400, detail=str(error)) from error
        return result

    @app.get("/status")
    def status() -> dict[str, Any]:
        return build_status_payload(load_batch_state(settings, batch_id))

    return app


def candidate_manifest_path(settings: Any, batch_id: str) -> Path:
    return settings.candidates_dir / f"{batch_id}.json"


def load_batch_state(settings: Any, batch_id: str) -> dict[str, Any]:
    manifest_path = candidate_manifest_path(settings, batch_id)
    if not manifest_path.exists():
        raise FileNotFoundError(
            f"Candidate manifest not found: {manifest_path}. Run generate.py first or create test fixtures."
        )

    candidate_manifest = read_manifest(manifest_path)
    prompt_manifest = load_prompt_manifest(settings, candidate_manifest)
    specs = build_specs(candidate_manifest, prompt_manifest)
    return {
        "settings": settings,
        "batch_id": batch_id,
        "manifest_path": manifest_path,
        "candidate_manifest": candidate_manifest,
        "prompt_manifest": prompt_manifest,
        "specs": specs,
        "counts": summarize_spec_counts(specs),
    }


def load_prompt_manifest(settings: Any, candidate_manifest: dict[str, Any]) -> dict[str, Any]:
    relative_path = str(candidate_manifest.get("source_prompts") or "").strip()
    if not relative_path:
        raise ValueError("Candidate manifest is missing source_prompts.")
    prompt_path = resolve_tool_relative_file(settings, relative_path)
    if not prompt_path.exists():
        raise FileNotFoundError(f"Prompt manifest not found: {prompt_path}")
    return read_manifest(prompt_path)


def resolve_tool_relative_file(settings: Any, relative_path: str) -> Path:
    candidate = (settings.tool_root / relative_path).resolve()
    tool_root = settings.tool_root.resolve()
    try:
        candidate.relative_to(tool_root)
    except ValueError as error:
        raise ValueError(f"Invalid path outside tool root: {relative_path}") from error
    return candidate


def build_specs(candidate_manifest: dict[str, Any], prompt_manifest: dict[str, Any]) -> list[dict[str, Any]]:
    prompt_lookup: dict[str, dict[str, Any]] = {}
    for prompt in prompt_manifest.get("prompts", []):
        if isinstance(prompt, dict) and prompt.get("spec_id"):
            prompt_lookup[str(prompt["spec_id"])] = prompt

    specs: list[dict[str, Any]] = []
    for entry in candidate_manifest.get("candidates", []):
        if not isinstance(entry, dict):
            continue
        spec_id = str(entry.get("spec_id") or "").strip()
        if not spec_id:
            continue
        prompt = prompt_lookup.get(spec_id, {})
        images = []
        for image in entry.get("images", []):
            if not isinstance(image, dict):
                continue
            normalized = dict(image)
            normalized["status"] = normalize_image_status(image.get("status"))
            images.append(normalized)

        status = normalize_spec_status(entry.get("status"))
        if status == SPEC_STATUS_PENDING:
            status = infer_spec_status(images, entry.get("error"))

        approved_image = next((image for image in images if image.get("status") == IMAGE_STATUS_APPROVED), None)
        thumbnail = approved_image or (images[0] if images else None)
        publish_record = entry.get("publish") if isinstance(entry.get("publish"), dict) else {}

        specs.append(
            {
                "spec_id": spec_id,
                "character": str(entry.get("character") or prompt.get("character") or spec_id).strip(),
                "status": status,
                "error": str(entry.get("error") or "").strip(),
                "images": images,
                "approved_image": approved_image,
                "thumbnail": thumbnail,
                "prompt": prompt,
                "publish": publish_record,
            }
        )

    specs.sort(key=lambda item: item["spec_id"])
    return specs


def summarize_spec_counts(specs: list[dict[str, Any]]) -> dict[str, int]:
    counts = {
        SPEC_STATUS_PENDING: 0,
        SPEC_STATUS_APPROVED: 0,
        SPEC_STATUS_PUBLISHED: 0,
        SPEC_STATUS_REGEN: 0,
        SPEC_STATUS_FAILED: 0,
    }
    for spec in specs:
        status = normalize_spec_status(spec.get("status"))
        counts[status] = counts.get(status, 0) + 1
    counts["total"] = len(specs)
    return counts


def normalize_image_status(value: Any) -> str:
    status = str(value or "").strip().lower()
    if status in {IMAGE_STATUS_PENDING, IMAGE_STATUS_APPROVED, IMAGE_STATUS_REJECTED}:
        return status
    return IMAGE_STATUS_PENDING


def normalize_spec_status(value: Any) -> str:
    status = str(value or "").strip().lower()
    if status in {SPEC_STATUS_PENDING, SPEC_STATUS_APPROVED, SPEC_STATUS_PUBLISHED, SPEC_STATUS_REGEN, SPEC_STATUS_FAILED}:
        return status
    return SPEC_STATUS_PENDING


def infer_spec_status(images: list[dict[str, Any]], error: Any) -> str:
    if error:
        return SPEC_STATUS_FAILED
    if any(image.get("status") == IMAGE_STATUS_APPROVED for image in images):
        return SPEC_STATUS_APPROVED
    if images and all(image.get("status") == IMAGE_STATUS_REJECTED for image in images):
        return SPEC_STATUS_REGEN
    return SPEC_STATUS_PENDING


def get_spec_or_raise(state: dict[str, Any], spec_id: str) -> dict[str, Any]:
    for spec in state["specs"]:
        if spec["spec_id"] == spec_id:
            return spec
    raise FileNotFoundError(f"Spec not found: {spec_id}")


def update_candidate_manifest(settings: Any, batch_id: str, updater: Any) -> dict[str, Any]:
    manifest_path = candidate_manifest_path(settings, batch_id)
    manifest = read_manifest(manifest_path)
    updater(manifest)
    manifest["updated_at_utc"] = timestamp_utc()
    write_manifest(manifest_path, manifest)
    return manifest


def find_manifest_spec(manifest: dict[str, Any], spec_id: str) -> dict[str, Any]:
    for entry in manifest.get("candidates", []):
        if isinstance(entry, dict) and str(entry.get("spec_id") or "").strip() == spec_id:
            entry.setdefault("images", [])
            return entry
    raise FileNotFoundError(f"Spec not found: {spec_id}")


def find_manifest_image(spec_entry: dict[str, Any], filename: str) -> dict[str, Any]:
    for image in spec_entry.get("images", []):
        if isinstance(image, dict) and str(image.get("filename") or "").strip() == filename:
            return image
    raise FileNotFoundError(f"Candidate not found: {filename}")


def approve_candidate(settings: Any, batch_id: str, spec_id: str, filename: str) -> dict[str, Any]:
    def updater(manifest: dict[str, Any]) -> None:
        spec_entry = find_manifest_spec(manifest, spec_id)
        images = spec_entry.get("images", [])
        if not images:
            raise ValueError(f"Spec {spec_id} has no candidate images.")

        selected = False
        for image in images:
            image_filename = str(image.get("filename") or "").strip()
            if image_filename == filename:
                image["status"] = IMAGE_STATUS_APPROVED
                image["approved_at_utc"] = timestamp_utc()
                selected = True
            else:
                image["status"] = IMAGE_STATUS_REJECTED
        if not selected:
            raise FileNotFoundError(f"Candidate not found: {filename}")

        spec_entry["status"] = SPEC_STATUS_APPROVED
        spec_entry["approved_filename"] = filename
        spec_entry["approved_at_utc"] = timestamp_utc()

    update_candidate_manifest(settings, batch_id, updater)
    return {
        "ok": True,
        "status": SPEC_STATUS_APPROVED,
        "approved_filename": filename,
        "message": f"Approved {filename}",
    }


def reject_candidate(settings: Any, batch_id: str, spec_id: str, filename: str) -> dict[str, Any]:
    def updater(manifest: dict[str, Any]) -> None:
        spec_entry = find_manifest_spec(manifest, spec_id)
        target = find_manifest_image(spec_entry, filename)
        target["status"] = IMAGE_STATUS_REJECTED
        target["rejected_at_utc"] = timestamp_utc()

        images = [image for image in spec_entry.get("images", []) if isinstance(image, dict)]
        approved = next((image for image in images if normalize_image_status(image.get("status")) == IMAGE_STATUS_APPROVED), None)
        if approved:
            spec_entry["status"] = SPEC_STATUS_APPROVED
            spec_entry["approved_filename"] = approved.get("filename")
        elif images and all(normalize_image_status(image.get("status")) == IMAGE_STATUS_REJECTED for image in images):
            spec_entry["status"] = SPEC_STATUS_REGEN
            spec_entry.pop("approved_filename", None)
        else:
            spec_entry["status"] = SPEC_STATUS_PENDING
            spec_entry.pop("approved_filename", None)

    update_candidate_manifest(settings, batch_id, updater)
    return {"ok": True, "message": f"Rejected {filename}"}


def mark_for_regeneration(settings: Any, batch_id: str, spec_id: str) -> dict[str, Any]:
    command = f"python Tools/ArtPipeline/generate.py {batch_id} --spec {spec_id}"

    def updater(manifest: dict[str, Any]) -> None:
        spec_entry = find_manifest_spec(manifest, spec_id)
        for image in spec_entry.get("images", []):
            if isinstance(image, dict) and normalize_image_status(image.get("status")) != IMAGE_STATUS_REJECTED:
                image["status"] = IMAGE_STATUS_REJECTED
        spec_entry["status"] = SPEC_STATUS_REGEN
        spec_entry["regenerate_command"] = command
        spec_entry["regeneration_requested_at_utc"] = timestamp_utc()
        spec_entry.pop("approved_filename", None)
        spec_entry.pop("publish", None)

    update_candidate_manifest(settings, batch_id, updater)
    return {"ok": True, "status": SPEC_STATUS_REGEN, "command": command}


def publish_spec_asset(settings: Any, batch_id: str, spec_id: str) -> dict[str, Any]:
    state = load_batch_state(settings, batch_id)
    spec = get_spec_or_raise(state, spec_id)
    if not spec["approved_image"]:
        raise ValueError(f"Spec {spec_id} does not have an approved candidate.")
    if spec["status"] == SPEC_STATUS_PUBLISHED:
        existing = spec.get("publish", {})
        return {
            "ok": True,
            "status": SPEC_STATUS_PUBLISHED,
            "asset_path": str(existing.get("asset_path") or ""),
            "project_record_path": str(existing.get("project_record_path") or ""),
            "message": "Spec already published.",
        }

    approved_image = spec["approved_image"]
    source_image_path = resolve_tool_relative_file(settings, str(approved_image.get("path") or "").strip())
    if not source_image_path.exists():
        raise FileNotFoundError(f"Approved candidate image not found: {source_image_path}")

    prompt = spec["prompt"]
    asset_type = infer_asset_type(spec, prompt)
    extension = source_image_path.suffix.lower().lstrip(".") or "png"
    final_filename = asset_filename(spec["character"], asset_type, extension=extension)
    destination = settings.assets_dir / final_filename
    if destination.exists():
        raise ValueError(f"Asset already exists: {destination}")

    settings.assets_dir.mkdir(parents=True, exist_ok=True)
    settings.projects_dir.mkdir(parents=True, exist_ok=True)
    shutil.copy2(source_image_path, destination)

    record_path = settings.projects_dir / f"{batch_id}-{spec_id}.md"
    record_path.write_text(build_prompt_record_markdown(batch_id, spec, approved_image, destination), encoding="utf-8")

    published_at = timestamp_utc()

    def updater(manifest: dict[str, Any]) -> None:
        spec_entry = find_manifest_spec(manifest, spec_id)
        spec_entry["status"] = SPEC_STATUS_PUBLISHED
        spec_entry["publish"] = {
            "asset_filename": destination.name,
            "asset_path": destination.relative_to(settings.repo_root).as_posix(),
            "project_record_path": record_path.relative_to(settings.repo_root).as_posix(),
            "published_at_utc": published_at,
            "source_candidate": str(approved_image.get("filename") or "").strip(),
        }

    update_candidate_manifest(settings, batch_id, updater)
    return {
        "ok": True,
        "status": SPEC_STATUS_PUBLISHED,
        "asset_path": destination.relative_to(settings.repo_root).as_posix(),
        "project_record_path": record_path.relative_to(settings.repo_root).as_posix(),
        "message": f"Published {destination.name}",
    }


def infer_asset_type(spec: dict[str, Any], prompt: dict[str, Any]) -> str:
    raw = str(prompt.get("asset_type") or "").strip()
    if raw:
        return raw

    spec_id = spec["spec_id"]
    character = str(spec["character"] or "").strip()
    if spec_id.startswith(f"{character}-"):
        return spec_id[len(character) + 1 :]
    return spec_id


def build_prompt_record_markdown(batch_id: str, spec: dict[str, Any], approved_image: dict[str, Any], destination: Path) -> str:
    prompt = spec["prompt"]
    params = prompt.get("generation_params") if isinstance(prompt.get("generation_params"), dict) else {}
    canon_sources = prompt.get("canon_sources") if isinstance(prompt.get("canon_sources"), list) else []
    destination_text = destination.as_posix()
    lines = [
        f"# Art Prompt Record — {spec['spec_id']}",
        "",
        "## Summary",
        f"- Batch ID: `{batch_id}`",
        f"- Spec ID: `{spec['spec_id']}`",
        f"- Character: `{spec['character']}`",
        f"- Final asset: `{destination_text}`",
        f"- Approved candidate: `{approved_image.get('filename', '')}`",
        f"- Approved seed: `{approved_image.get('seed', '')}`",
        f"- Generation time: `{approved_image.get('generation_time_seconds', '')}` seconds",
        f"- Recorded at: `{datetime.now(UTC).isoformat()}`",
        "",
        "## Prompts",
        f"**Positive Prompt:** {prompt.get('positive_prompt', '')}",
        "",
        f"**Negative Prompt:** {prompt.get('negative_prompt', '')}",
        "",
        "## Generation Params",
    ]
    for key in sorted(params):
        lines.append(f"- {key}: `{params[key]}`")

    lines.extend(["", "## Agent Files"])
    if canon_sources:
        for item in canon_sources:
            lines.append(f"- `{item}`")
    else:
        lines.append("- None recorded")

    lines.extend(
        [
            "",
            "## Source Files",
            f"- Prompt manifest spec: `{spec['spec_id']}` in batch `{batch_id}`",
            f"- Candidate image: `{approved_image.get('path', '')}`",
            f"- Published asset: `{destination_text}`",
            "",
        ]
    )
    return "\n".join(lines)


def build_status_payload(state: dict[str, Any]) -> dict[str, Any]:
    manifest = state["candidate_manifest"]
    return {
        "batch_id": state["batch_id"],
        "created_at_utc": manifest.get("created_at_utc"),
        "backend": manifest.get("backend"),
        "backend_url": manifest.get("backend_url"),
        "counts": state["counts"],
        "specs": [
            {
                "spec_id": spec["spec_id"],
                "character": spec["character"],
                "status": spec["status"],
                "approved_filename": spec["approved_image"].get("filename") if spec.get("approved_image") else None,
                "published_asset_path": spec.get("publish", {}).get("asset_path"),
                "project_record_path": spec.get("publish", {}).get("project_record_path"),
                "candidate_count": len(spec["images"]),
            }
            for spec in state["specs"]
        ],
    }


def render_dashboard_page(state: dict[str, Any]) -> str:
    manifest = state["candidate_manifest"]
    counts = state["counts"]
    spec_cards = "".join(render_spec_card(spec) for spec in state["specs"]) or '<div class="empty panel">No specs found.</div>'

    body = f"""
    <div class=\"topbar\">
      <div class=\"title\">
        <h1>{escape(APP_TITLE)}</h1>
        <p>Batch <code>{escape(state['batch_id'])}</code> · review multiple candidates down to one approved asset.</p>
      </div>
      <div class=\"panel\">
        <div class=\"label\">Created</div>
        <div class=\"value\">{escape(str(manifest.get('created_at_utc') or 'unknown'))}</div>
      </div>
    </div>

    <div class=\"panel\">
      <div class=\"meta-grid\">
        <div class=\"meta-card\"><div class=\"label\">Batch ID</div><div class=\"value\">{escape(state['batch_id'])}</div></div>
        <div class=\"meta-card\"><div class=\"label\">Backend</div><div class=\"value\">{escape(str(manifest.get('backend') or 'unknown'))}</div></div>
        <div class=\"meta-card\"><div class=\"label\">Backend URL</div><div class=\"value\">{escape(str(manifest.get('backend_url') or 'unknown'))}</div></div>
        <div class=\"meta-card\"><div class=\"label\">Manifest</div><div class=\"value\">{escape(state['manifest_path'].relative_to(state['settings'].repo_root).as_posix())}</div></div>
      </div>
    </div>

    <div class=\"stats-grid\" style=\"margin-top: 18px;\">
      {render_stat_card('Total specs', counts.get('total', 0))}
      {render_stat_card('Pending', counts.get('pending_review', 0))}
      {render_stat_card('Approved', counts.get('approved', 0))}
      {render_stat_card('Published', counts.get('published', 0))}
      {render_stat_card('Needs regen', counts.get('needs_regeneration', 0))}
    </div>

    <div class=\"spec-grid\">{spec_cards}</div>
    """
    return HTML_SHELL.format(title=APP_TITLE, body=body)


def render_stat_card(label: str, value: Any) -> str:
    return f'<div class="stat-card"><div class="label">{escape(label)}</div><div class="value">{escape(value)}</div></div>'


def render_spec_card(spec: dict[str, Any]) -> str:
    thumb_url = image_url(spec.get("thumbnail"))
    thumb_html = (
        f'<img class="thumb" src="{escape(thumb_url)}" alt="{escape(spec["spec_id"])} preview" />'
        if thumb_url
        else '<div class="thumb" style="display:flex;align-items:center;justify-content:center;color:#9cb0d3;">No image</div>'
    )
    publish = spec.get("publish", {})
    publish_line = (
        f'<div class="hint" style="margin-top:8px;">Published: <code>{escape(str(publish.get("asset_path") or ""))}</code></div>'
        if publish.get("asset_path")
        else ""
    )
    return f"""
    <div class=\"card\">
      {thumb_html}
      <div class=\"card-body\">
        <div style=\"display:flex;justify-content:space-between;gap:12px;align-items:flex-start;flex-wrap:wrap;\">
          <div>
            <h3 style=\"margin-bottom:6px;\">{escape(display_character_name(spec['character']))}</h3>
            <div class=\"hint\">{escape(spec['spec_id'])}</div>
          </div>
          {badge_html(spec['status'])}
        </div>
        <div class=\"hint\" style=\"margin-top:10px;\">{len(spec['images'])} candidate(s)</div>
        {publish_line}
        <div class=\"button-row\" style=\"margin-top:14px;\">
          <a href=\"/spec/{escape(spec['spec_id'])}\"><button type=\"button\">Review</button></a>
        </div>
      </div>
    </div>
    """


def render_spec_page(state: dict[str, Any], spec: dict[str, Any]) -> str:
    prompt = spec["prompt"]
    params = prompt.get("generation_params") if isinstance(prompt.get("generation_params"), dict) else {}
    approved_image = spec.get("approved_image")
    publish = spec.get("publish", {})
    candidate_cards = "".join(render_candidate_card(image) for image in spec["images"]) or '<div class="empty">No candidate images found.</div>'
    approved_panel_class = "approved-panel visible" if approved_image else "approved-panel"
    approved_image_html = ""
    if approved_image:
        approved_image_html = (
            f'<img class="approved-preview" src="{escape(image_url(approved_image) or "")}" alt="Approved candidate" />'
            f'<div class="hint">Approved candidate: <code>{escape(str(approved_image.get("filename") or ""))}</code></div>'
        )
    publish_result_html = ""
    if publish.get("asset_path"):
        publish_result_html = f"""
        <div class=\"panel\" style=\"margin-top:18px;\">
          <h3>Published asset</h3>
          <div class=\"hint\">Final asset path: <code>{escape(str(publish.get('asset_path') or ''))}</code></div>
          <div class=\"hint\">Prompt record: <code>{escape(str(publish.get('project_record_path') or ''))}</code></div>
        </div>
        """

    regeneration_hint = ""
    if spec["status"] == SPEC_STATUS_REGEN:
        regeneration_hint = f"""
        <div class=\"panel\" style=\"margin-top:18px;\">
          <h3>Regeneration requested</h3>
          <p class=\"hint\">Run this manually, then refresh the page:</p>
          <p><code>python Tools/ArtPipeline/generate.py {escape(state['batch_id'])} --spec {escape(spec['spec_id'])}</code></p>
        </div>
        """

    body = f"""
    <div class=\"topbar\">
      <div class=\"title\">
        <div class=\"hint\"><a href=\"/\">← Back to dashboard</a></div>
        <h1 style=\"margin-bottom:8px;\">{escape(display_character_name(spec['character']))}</h1>
        <p>{escape(spec['spec_id'])}</p>
      </div>
      <div class=\"header-actions\">{badge_html(spec['status'])}</div>
    </div>

    <div class=\"panel\">
      <div class=\"meta-grid\">
        <div class=\"meta-card\"><div class=\"label\">Character</div><div class=\"value\">{escape(spec['character'])}</div></div>
        <div class=\"meta-card\"><div class=\"label\">Candidates</div><div class=\"value\">{len(spec['images'])}</div></div>
        <div class=\"meta-card\"><div class=\"label\">Sampler</div><div class=\"value\">{escape(str(params.get('sampler') or ''))}</div></div>
        <div class=\"meta-card\"><div class=\"label\">Size</div><div class=\"value\">{escape(str(params.get('width') or '?'))}×{escape(str(params.get('height') or '?'))}</div></div>
        <div class=\"meta-card\"><div class=\"label\">Steps</div><div class=\"value\">{escape(str(params.get('steps') or ''))}</div></div>
        <div class=\"meta-card\"><div class=\"label\">CFG</div><div class=\"value\">{escape(str(params.get('cfg_scale') or ''))}</div></div>
      </div>
      <p class=\"hint\" style=\"margin-top:14px;\">Click one candidate to select it. Then approve, reject, or mark the whole spec for regeneration.</p>
      <div class=\"button-row\">
        <button id=\"approveButton\" class=\"approve\" disabled>Approve Selected</button>
        <button id=\"rejectButton\" class=\"reject\" disabled>Reject Selected</button>
        <button id=\"regenButton\" class=\"regen\">Regenerate All</button>
        <button id=\"publishButton\" class=\"publish\" {'disabled' if not approved_image or publish.get('asset_path') else ''}>Publish</button>
      </div>
      <div id=\"statusMessage\" class=\"status\"></div>
    </div>

    <div id=\"candidateGrid\" class=\"candidate-grid\">{candidate_cards}</div>

    <div id=\"approvedPanel\" class=\"{approved_panel_class}\">
      <div class=\"panel\">
        <h3>Approved candidate</h3>
        {approved_image_html if approved_image_html else '<div class="hint">No candidate approved yet.</div>'}
      </div>
    </div>

    {publish_result_html}
    {regeneration_hint}

    <script>
      const state = {{
        specId: {json.dumps(spec['spec_id'])},
        selectedFilename: null,
        approvedFilename: {json.dumps(approved_image.get('filename') if approved_image else None)},
        publishedAssetPath: {json.dumps(publish.get('asset_path'))},
      }};

      function setStatus(message, isError = false) {{
        const node = document.getElementById('statusMessage');
        node.textContent = message || '';
        node.className = isError ? 'status error' : 'status';
      }}

      function updateButtons() {{
        document.getElementById('approveButton').disabled = !state.selectedFilename;
        document.getElementById('rejectButton').disabled = !state.selectedFilename;
        document.getElementById('publishButton').disabled = !state.approvedFilename || !!state.publishedAssetPath;
      }}

      function selectCard(filename) {{
        state.selectedFilename = filename;
        document.querySelectorAll('.candidate-card').forEach((card) => {{
          card.classList.toggle('selected', card.dataset.filename === filename);
        }});
        updateButtons();
      }}

      async function postJson(url, payload = null) {{
        const response = await fetch(url, {{
          method: 'POST',
          headers: {{ 'Content-Type': 'application/json' }},
          body: payload ? JSON.stringify(payload) : '{{}}',
        }});
        let data = null;
        try {{
          data = await response.json();
        }} catch (error) {{
          data = null;
        }}
        if (!response.ok) {{
          throw new Error(data && data.detail ? data.detail : `Request failed (${{response.status}})`);
        }}
        return data;
      }}

      document.querySelectorAll('.candidate-card').forEach((card) => {{
        card.addEventListener('click', () => selectCard(card.dataset.filename));
      }});

      document.getElementById('approveButton').addEventListener('click', async () => {{
        if (!state.selectedFilename) return;
        setStatus('Approving…');
        try {{
          const result = await postJson(`/spec/${{encodeURIComponent(state.specId)}}/approve`, {{ filename: state.selectedFilename }});
          state.approvedFilename = result.approved_filename || state.selectedFilename;
          state.selectedFilename = null;
          setStatus(result.message || 'Approved. Refreshing…');
          window.location.reload();
        }} catch (error) {{
          setStatus(error.message || String(error), true);
        }}
      }});

      document.getElementById('rejectButton').addEventListener('click', async () => {{
        if (!state.selectedFilename) return;
        setStatus('Rejecting…');
        try {{
          const result = await postJson(`/spec/${{encodeURIComponent(state.specId)}}/reject`, {{ filename: state.selectedFilename }});
          state.selectedFilename = null;
          setStatus(result.message || 'Rejected. Refreshing…');
          window.location.reload();
        }} catch (error) {{
          setStatus(error.message || String(error), true);
        }}
      }});

      document.getElementById('regenButton').addEventListener('click', async () => {{
        setStatus('Marking for regeneration…');
        try {{
          const result = await postJson(`/spec/${{encodeURIComponent(state.specId)}}/regenerate`);
          setStatus((result.message || 'Marked for regeneration.') + ' Refreshing…');
          window.location.reload();
        }} catch (error) {{
          setStatus(error.message || String(error), true);
        }}
      }});

      document.getElementById('publishButton').addEventListener('click', async () => {{
        if (!state.approvedFilename || state.publishedAssetPath) return;
        setStatus('Publishing…');
        try {{
          const result = await postJson(`/spec/${{encodeURIComponent(state.specId)}}/publish`);
          state.publishedAssetPath = result.asset_path || null;
          setStatus(result.message || 'Published. Refreshing…');
          window.location.reload();
        }} catch (error) {{
          setStatus(error.message || String(error), true);
        }}
      }});

      updateButtons();
    </script>
    """
    return HTML_SHELL.format(title=f"{APP_TITLE} — {spec['spec_id']}", body=body)


def render_candidate_card(image: dict[str, Any]) -> str:
    status = normalize_image_status(image.get("status"))
    return f"""
    <div class=\"card candidate-card {escape(status)}\" data-filename=\"{escape(str(image.get('filename') or ''))}\">
      <div class=\"radio\"></div>
      <img class=\"candidate-image\" src=\"{escape(image_url(image) or '')}\" alt=\"{escape(str(image.get('filename') or 'candidate image'))}\" />
      <div class=\"card-body\">
        <div style=\"display:flex;justify-content:space-between;gap:12px;align-items:flex-start;flex-wrap:wrap;\">
          <div>
            <div style=\"font-weight:700;\">{escape(str(image.get('filename') or ''))}</div>
            <div class=\"hint\">Seed: {escape(str(image.get('seed') or 'unknown'))}</div>
            <div class=\"hint\">Generation time: {escape(format_seconds(image.get('generation_time_seconds')))}</div>
          </div>
          {badge_html(status)}
        </div>
      </div>
    </div>
    """


def image_url(image: dict[str, Any] | None) -> str | None:
    if not image:
        return None
    path = str(image.get("path") or "").strip()
    if not path:
        return None
    return f"/images/{path}"


def badge_html(status: str) -> str:
    label = status.replace("_", " ")
    return f'<span class="badge {escape(status)}">{escape(label)}</span>'


def format_seconds(value: Any) -> str:
    try:
        return f"{float(value):.1f}s"
    except (TypeError, ValueError):
        return "unknown"


def display_character_name(value: str) -> str:
    return str(value or "").replace("-", " ").title()


def escape(value: Any) -> str:
    return html.escape(str(value))


def cli_approve(settings: Any, batch_id: str, filename: str) -> int:
    try:
        state = load_batch_state(settings, batch_id)
    except Exception as error:
        print(f"[review] Configuration error: {error}", file=sys.stderr)
        return 1

    target_spec = None
    for spec in state["specs"]:
        for image in spec["images"]:
            if str(image.get("filename") or "").strip() == filename:
                target_spec = spec
                break
        if target_spec:
            break

    if not target_spec:
        print(f"[review] Candidate not found in batch {batch_id}: {filename}", file=sys.stderr)
        return 1

    try:
        approve_candidate(settings, batch_id, target_spec["spec_id"], filename)
        result = publish_spec_asset(settings, batch_id, target_spec["spec_id"])
    except Exception as error:
        print(f"[review] Approval error: {error}", file=sys.stderr)
        return 1

    print(f"Approved and published: {filename}")
    print(f"Asset: {result.get('asset_path')}")
    print(f"Prompt record: {result.get('project_record_path')}")
    return 0


def main(argv: Sequence[str] | None = None) -> int:
    args = parse_args() if argv is None else parse_args_from(argv)
    settings = load_settings()
    port = args.port or settings.review_port

    if port <= 0 or port > 65535:
        print("[review] Configuration error: --port must be between 1 and 65535", file=sys.stderr)
        return 1

    if args.approve:
        return cli_approve(settings, args.batch_id, args.approve)

    try:
        app = create_app(args.batch_id)
    except Exception as error:
        print(f"[review] Configuration error: {error}", file=sys.stderr)
        return 1

    try:
        import uvicorn
    except ImportError:
        print(
            "[review] Missing dependency: uvicorn is not installed. Run 'pip install -r Tools/ArtPipeline/requirements.txt'.",
            file=sys.stderr,
        )
        return 1

    print(f"[review] Batch: {args.batch_id}")
    print(f"[review] Candidate manifest: {candidate_manifest_path(settings, args.batch_id)}")
    print(f"[review] Assets dir: {settings.assets_dir}")
    print(f"[review] Projects dir: {settings.projects_dir}")
    print(f"[review] Serving http://{args.host}:{port}")
    print("Press Ctrl+C to stop.")
    uvicorn.run(app, host=args.host, port=port, reload=args.reload)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
