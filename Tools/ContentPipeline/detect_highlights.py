#!/usr/bin/env python3
"""Detect clip-worthy highlights from transcript JSON files using Ollama."""

from __future__ import annotations

import argparse
from dataclasses import replace
import json
from pathlib import Path
import sys
from typing import Any, Sequence
import urllib.error
import urllib.request

from config import Settings, load_settings
from lib.highlight_io import build_highlight_manifest, format_seconds, load_transcript_json, normalize_text, write_highlight_manifest

VALID_CATEGORIES = {"technical", "entertainment", "hybrid"}


def build_parser() -> argparse.ArgumentParser:
    """Build the CLI parser."""
    parser = argparse.ArgumentParser(description="Detect short-form highlights from transcript JSON via Ollama")
    parser.add_argument(
        "inputs",
        nargs="*",
        help="Transcript JSON path(s), file name(s), or stems. If omitted, batch mode scans the transcripts directory.",
    )
    parser.add_argument("--transcripts-dir", type=Path, help="Override transcript input directory.")
    parser.add_argument("--highlights-dir", type=Path, help="Override highlight output directory.")
    parser.add_argument("--ollama-url", help="Override Ollama generate endpoint.")
    parser.add_argument("--model", help="Override Ollama model name.")
    parser.add_argument("--window-minutes", type=int, help="Sliding window size in minutes.")
    parser.add_argument("--overlap-minutes", type=int, help="Window overlap size in minutes.")
    parser.add_argument("--max-highlights-per-window", type=int, help="Maximum suggestions requested per window.")
    parser.add_argument("--overwrite", action="store_true", help="Rebuild outputs even if they already exist.")
    parser.add_argument("--dry-run", action="store_true", help="Print planned work without calling Ollama.")
    parser.add_argument("--limit", type=int, help="Only process the first N resolved transcript files.")
    return parser


def parse_args() -> argparse.Namespace:
    """Parse command-line arguments."""
    return build_parser().parse_args()


def parse_args_from(argv: Sequence[str]) -> argparse.Namespace:
    """Parse arguments from an explicit argv list for tests."""
    return build_parser().parse_args(list(argv))


def resolve_inputs(args: argparse.Namespace) -> tuple[Settings, list[Path]]:
    """Resolve input transcript files using CLI args and config."""
    settings = load_settings()
    transcripts_dir = (args.transcripts_dir or settings.transcripts_dir).resolve()
    highlights_dir = (args.highlights_dir or settings.highlights_dir).resolve()

    if not transcripts_dir.exists() and not args.inputs:
        raise FileNotFoundError(f"Transcripts directory does not exist: {transcripts_dir}")

    settings = replace(settings, transcripts_dir=transcripts_dir, highlights_dir=highlights_dir)

    if args.inputs:
        transcripts = [resolve_transcript_path(value, transcripts_dir) for value in args.inputs]
    else:
        transcripts = find_transcripts(transcripts_dir)

    if args.limit is not None:
        if args.limit <= 0:
            raise ValueError("--limit must be greater than 0")
        transcripts = transcripts[: args.limit]

    if not transcripts:
        raise FileNotFoundError("No transcript JSON files matched the requested inputs.")

    return settings, transcripts


def resolve_transcript_path(value: str, transcripts_dir: Path) -> Path:
    """Resolve a user-supplied transcript reference into a real JSON file path."""
    candidate = Path(value).expanduser()

    possible_paths: list[Path] = []
    if candidate.is_absolute():
        possible_paths.append(candidate)
    else:
        possible_paths.append((Path.cwd() / candidate).resolve())
        possible_paths.append((transcripts_dir / candidate).resolve())
        if candidate.suffix == "":
            possible_paths.append((transcripts_dir / f"{candidate.name}.json").resolve())

    for path in possible_paths:
        if path.exists() and path.is_file() and path.suffix.lower() == ".json":
            return path

    raise FileNotFoundError(
        f"Could not find transcript JSON: {value}. Expected an existing .json file path, a file in {transcripts_dir}, or a transcript stem."
    )


def find_transcripts(directory: Path) -> list[Path]:
    """Return sorted transcript JSON files."""
    return [path for path in sorted(directory.iterdir()) if path.is_file() and path.suffix.lower() == ".json"]


def build_runtime_options(args: argparse.Namespace, settings: Settings) -> dict[str, Any]:
    """Merge CLI overrides with config defaults."""
    window_minutes = args.window_minutes or settings.highlight_window_minutes
    overlap_minutes = args.overlap_minutes or settings.highlight_window_overlap_minutes
    max_per_window = args.max_highlights_per_window or settings.highlight_max_per_window

    if window_minutes <= 0:
        raise ValueError("--window-minutes must be greater than 0")
    if overlap_minutes < 0:
        raise ValueError("--overlap-minutes must be 0 or greater")
    if overlap_minutes >= window_minutes:
        raise ValueError("--overlap-minutes must be smaller than --window-minutes")
    if max_per_window <= 0:
        raise ValueError("--max-highlights-per-window must be greater than 0")

    return {
        "ollama_url": (args.ollama_url or settings.ollama_url).strip(),
        "model": (args.model or settings.ollama_model).strip(),
        "window_minutes": window_minutes,
        "overlap_minutes": overlap_minutes,
        "max_highlights_per_window": max_per_window,
    }


def output_path(transcript_path: Path, highlights_dir: Path) -> Path:
    """Return the output path for a transcript's highlight manifest."""
    return highlights_dir / f"{transcript_path.stem}.json"


def collect_segments(transcript_payload: dict[str, Any]) -> list[dict[str, Any]]:
    """Normalize transcript segments into a sorted list."""
    raw_segments = transcript_payload.get("segments")
    if not isinstance(raw_segments, list):
        raise ValueError("Transcript payload is missing a 'segments' list")

    segments: list[dict[str, Any]] = []
    for raw_segment in raw_segments:
        if not isinstance(raw_segment, dict):
            continue

        start = _as_float(raw_segment.get("start"))
        end = _as_float(raw_segment.get("end"))
        text = normalize_text(raw_segment.get("text"))
        if start is None or end is None or end <= start or not text:
            continue

        segments.append({
            "start": start,
            "end": end,
            "text": text,
        })

    segments.sort(key=lambda item: (item["start"], item["end"]))
    return segments


def build_windows(segments: list[dict[str, Any]], window_minutes: int, overlap_minutes: int) -> list[dict[str, Any]]:
    """Split transcript segments into overlapping time windows."""
    if not segments:
        return []

    window_seconds = float(window_minutes * 60)
    overlap_seconds = float(overlap_minutes * 60)
    step_seconds = window_seconds - overlap_seconds
    last_end = max(segment["end"] for segment in segments)

    windows: list[dict[str, Any]] = []
    start = 0.0
    index = 1

    while start < last_end + 0.001:
        end = min(last_end, start + window_seconds)
        window_segments = [
            segment for segment in segments if segment["end"] > start and segment["start"] < end
        ]
        if window_segments:
            windows.append({
                "index": index,
                "start": start,
                "end": end,
                "segments": window_segments,
            })
            index += 1

        if end >= last_end:
            break
        start += step_seconds

    return windows


def build_prompt(window: dict[str, Any], max_highlights: int) -> str:
    """Build the LLM prompt for a single transcript window."""
    lines: list[str] = []
    for segment in window["segments"]:
        lines.append(f"[{format_seconds(segment['start'])} -> {format_seconds(segment['end'])}] {segment['text']}")

    transcript_block = "\n".join(lines)
    return f"""
You are helping turn a live stream recording into short-form social clips for SharkFac3.

Brand and selection rules:
- The voice is authentic, warm, self-aware, and knowledgeable without hype.
- Never use clickbait or exaggerated language like INSANE, UNBELIEVABLE, YOU WON'T BELIEVE, MUST SEE.
- Good clips are either technical/informational, entertainment/chaos, or ideally both.
- Technical clips: breakthroughs, clever fixes, useful explanations, failures that teach something.
- Entertainment clips: genuine funny moments, chaos, unscripted reactions, memorable character or game moments.
- The best captions are honest, specific, and hook-first.
- Standalone value matters: a new viewer should understand why the moment is interesting.
- Prefer clips that could work at roughly 15 to 90 seconds.

Output rules:
- Return ONLY valid JSON.
- Return an object with a single key named "highlights".
- The value of "highlights" must be a JSON array.
- Suggest at most {max_highlights} highlights.
- Use absolute times in seconds from the full recording, not relative times.
- Categories must be one of: technical, entertainment, hybrid.
- confidence_score must be between 0 and 1.
- If nothing in this window is worth clipping, return {{"highlights": []}}.

Each highlight object must have:
- start_time
- end_time
- category
- description
- suggested_caption
- confidence_score

Transcript window metadata:
- window_index: {window['index']}
- window_start_seconds: {window['start']:.2f}
- window_end_seconds: {window['end']:.2f}

Transcript:
{transcript_block}
""".strip()


def detect_for_transcript(transcript_path: Path, output_file: Path, options: dict[str, Any]) -> tuple[int, int]:
    """Detect highlights for a transcript and persist the manifest."""
    transcript_payload = load_transcript_json(transcript_path)
    segments = collect_segments(transcript_payload)
    windows = build_windows(segments, int(options["window_minutes"]), int(options["overlap_minutes"]))

    highlights: list[dict[str, Any]] = []
    raw_suggestions = 0
    duration_seconds = infer_duration_seconds(transcript_payload, segments)

    for window in windows:
        prompt = build_prompt(window, int(options["max_highlights_per_window"]))
        payload = request_ollama(
            ollama_url=str(options["ollama_url"]),
            model=str(options["model"]),
            prompt=prompt,
        )
        suggestions = normalize_highlights(
            payload=payload,
            window=window,
            duration_seconds=duration_seconds,
        )
        raw_suggestions += len(suggestions)
        highlights.extend(suggestions)

    deduped = dedupe_highlights(highlights)
    manifest = build_highlight_manifest(
        transcript_path=transcript_path,
        transcript_payload=transcript_payload,
        model_name=str(options["model"]),
        ollama_url=str(options["ollama_url"]),
        window_count=len(windows),
        highlights=deduped,
    )
    write_highlight_manifest(manifest, output_file)
    return len(windows), raw_suggestions


def infer_duration_seconds(transcript_payload: dict[str, Any], segments: list[dict[str, Any]]) -> float:
    """Determine the full transcript duration for clamping highlight ranges."""
    transcription = transcript_payload.get("transcription")
    if isinstance(transcription, dict):
        duration = _as_float(transcription.get("duration_seconds"))
        if duration is not None and duration > 0:
            return duration

    if segments:
        return max(segment["end"] for segment in segments)
    return 0.0


def request_ollama(*, ollama_url: str, model: str, prompt: str) -> dict[str, Any]:
    """Send a single non-streaming prompt to Ollama and parse the response JSON."""
    request_body = json.dumps(
        {
            "model": model,
            "prompt": prompt,
            "stream": False,
            "options": {
                "temperature": 0.2,
            },
        }
    ).encode("utf-8")

    request = urllib.request.Request(
        ollama_url,
        data=request_body,
        headers={"Content-Type": "application/json", "Accept": "application/json"},
        method="POST",
    )

    try:
        with urllib.request.urlopen(request, timeout=300) as response:
            status = getattr(response, "status", 200)
            body_text = response.read().decode("utf-8")
    except urllib.error.URLError as error:
        raise RuntimeError(f"Ollama request failed: {error}") from error

    if status < 200 or status >= 300:
        raise RuntimeError(f"Ollama returned HTTP {status}")

    try:
        payload = json.loads(body_text)
    except json.JSONDecodeError as error:
        raise RuntimeError(f"Ollama returned invalid JSON: {error}") from error

    if not isinstance(payload, dict):
        raise RuntimeError("Ollama response must be a JSON object")

    response_text = payload.get("response")
    if not isinstance(response_text, str) or not response_text.strip():
        raise RuntimeError("Ollama response is missing a non-empty 'response' field")

    model_payload = parse_model_json(response_text)
    if not isinstance(model_payload, dict):
        raise RuntimeError("Ollama model output must decode to a JSON object")

    return model_payload


def parse_model_json(text: str) -> Any:
    """Extract the first JSON object from model text."""
    stripped = text.strip()
    try:
        return json.loads(stripped)
    except json.JSONDecodeError:
        pass

    start = stripped.find("{")
    end = stripped.rfind("}")
    if start == -1 or end == -1 or end <= start:
        raise RuntimeError("Model output did not contain a JSON object")

    snippet = stripped[start : end + 1]
    try:
        return json.loads(snippet)
    except json.JSONDecodeError as error:
        raise RuntimeError(f"Model output did not contain valid JSON: {error}") from error


def normalize_highlights(*, payload: dict[str, Any], window: dict[str, Any], duration_seconds: float) -> list[dict[str, Any]]:
    """Validate and normalize highlights returned by the LLM."""
    raw_highlights = payload.get("highlights")
    if raw_highlights is None:
        raise RuntimeError("Model output is missing the 'highlights' key")
    if not isinstance(raw_highlights, list):
        raise RuntimeError("Model output 'highlights' value must be a list")

    normalized: list[dict[str, Any]] = []
    for item in raw_highlights:
        if not isinstance(item, dict):
            continue

        start_time = _as_float(item.get("start_time"))
        end_time = _as_float(item.get("end_time"))
        description = normalize_text(item.get("description"))
        suggested_caption = normalize_text(item.get("suggested_caption"))
        category = normalize_category(item.get("category"))
        confidence_score = clamp(_as_float(item.get("confidence_score")) or 0.0, 0.0, 1.0)

        if start_time is None or end_time is None:
            continue

        if duration_seconds > 0:
            start_time = clamp(start_time, 0.0, duration_seconds)
            end_time = clamp(end_time, 0.0, duration_seconds)

        if end_time <= start_time:
            continue
        if not description or not suggested_caption:
            continue

        normalized.append(
            {
                "start_time": round(start_time, 3),
                "end_time": round(end_time, 3),
                "category": category,
                "description": description,
                "suggested_caption": suggested_caption,
                "confidence_score": round(confidence_score, 3),
                "source_window_index": window["index"],
                "source_window_start_time": round(float(window["start"]), 3),
                "source_window_end_time": round(float(window["end"]), 3),
            }
        )

    return normalized


def dedupe_highlights(highlights: list[dict[str, Any]]) -> list[dict[str, Any]]:
    """Collapse overlapping suggestions from adjacent windows."""
    deduped: list[dict[str, Any]] = []

    for candidate in sorted(highlights, key=lambda item: (item["start_time"], item["end_time"])):
        matched_index = -1
        for index, existing in enumerate(deduped):
            if highlights_overlap(candidate, existing):
                matched_index = index
                break

        if matched_index == -1:
            deduped.append(candidate)
            continue

        existing = deduped[matched_index]
        if float(candidate["confidence_score"]) > float(existing["confidence_score"]):
            deduped[matched_index] = candidate

    deduped.sort(key=lambda item: (-float(item["confidence_score"]), float(item["start_time"])))
    for index, item in enumerate(deduped, start=1):
        item["rank"] = index
    return deduped


def highlights_overlap(left: dict[str, Any], right: dict[str, Any]) -> bool:
    """Return True when two highlights likely describe the same moment."""
    left_start = float(left["start_time"])
    left_end = float(left["end_time"])
    right_start = float(right["start_time"])
    right_end = float(right["end_time"])

    intersection = max(0.0, min(left_end, right_end) - max(left_start, right_start))
    if intersection <= 0:
        return False

    left_length = max(0.001, left_end - left_start)
    right_length = max(0.001, right_end - right_start)
    overlap_ratio = intersection / min(left_length, right_length)
    start_delta = abs(left_start - right_start)
    return overlap_ratio >= 0.6 or start_delta <= 8.0


def normalize_category(value: Any) -> str:
    """Map model output categories onto the supported category set."""
    text = normalize_text(value).lower()
    if text in VALID_CATEGORIES:
        return text
    if text in {"both", "mixed", "combo", "technical+entertainment", "entertainment+technical"}:
        return "hybrid"
    if "tech" in text or "inform" in text or "educat" in text:
        return "technical"
    if "fun" in text or "chaos" in text or "comedy" in text or "entertain" in text:
        return "entertainment"
    return "hybrid"


def clamp(value: float, minimum: float, maximum: float) -> float:
    """Clamp a number to the inclusive range."""
    return max(minimum, min(maximum, value))


def _as_float(value: Any) -> float | None:
    if value is None:
        return None
    if isinstance(value, (int, float)):
        return float(value)
    try:
        return float(str(value).strip())
    except ValueError:
        return None


def main(argv: Sequence[str] | None = None) -> int:
    args = parse_args() if argv is None else parse_args_from(argv)

    try:
        settings, transcripts = resolve_inputs(args)
        settings.ensure_data_dirs()
        options = build_runtime_options(args, settings)
    except Exception as error:
        print(f"[highlights] Configuration error: {error}", file=sys.stderr)
        return 1

    print(f"[highlights] Transcript input:  {settings.transcripts_dir}")
    print(f"[highlights] Highlight output: {settings.highlights_dir}")
    print(
        f"[highlights] Ollama: model={options['model']} url={options['ollama_url']} "
        f"window={options['window_minutes']}m overlap={options['overlap_minutes']}m "
        f"max_per_window={options['max_highlights_per_window']}"
    )

    processed = 0
    skipped = 0

    for transcript_path in transcripts:
        result_path = output_path(transcript_path, settings.highlights_dir)
        exists_already = result_path.exists()

        if args.dry_run:
            action = "skip(existing)" if exists_already and not args.overwrite else "detect"
            print(f"[highlights] {action}: {transcript_path.name} -> {result_path.name}")
            continue

        if exists_already and not args.overwrite:
            print(f"[highlights] Skipping existing output for {transcript_path.name}")
            skipped += 1
            continue

        print(f"[highlights] Detecting highlights for {transcript_path.name}...")
        try:
            window_count, raw_suggestions = detect_for_transcript(transcript_path, result_path, options)
        except Exception as error:
            print(f"[highlights] Failed for {transcript_path.name}: {error}", file=sys.stderr)
            return 1

        processed += 1
        print(
            f"[highlights] Wrote {result_path.name} "
            f"(windows={window_count} raw_suggestions={raw_suggestions})"
        )

    print(f"[highlights] Done. processed={processed} skipped={skipped} total={len(transcripts)}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
