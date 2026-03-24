# Pipeline Tooling — content-repurposer

## Purpose

Covers the local Python tooling in `Tools/ContentPipeline/` that moves stream recordings through transcription, highlight detection, clip extraction, review formatting, operator review, and feedback capture.

Load this skill area when the task is about how the pipeline works, how to operate it, or how to maintain its code and docs.

## When to Load

- Building, modifying, or debugging any `Tools/ContentPipeline/` script
- Changing manifest schemas or phase-to-phase contracts
- Troubleshooting transcription, highlight detection, clip extraction, formatting, review flow, or feedback import
- Updating operator docs for the content pipeline
- Reviewing whether pipeline documentation still matches the current codebase

## Files in This Skill Area

| File | Purpose |
|---|---|
| `_index.md` | Routing, scope, and maintenance expectations |
| `phase-map.md` | Current phase contracts, inputs/outputs, and author checklist |

## Current Pipeline Shape

```text
transcribe.py
  -> data/transcripts/

detect_highlights.py
  -> data/highlights/

extract_clips.py
  -> data/clips/

format_instagram.py
  -> data/review_queue/

review_server.py
  -> data/published/

feedback.py
  -> data/feedback/
```

Every phase is independently runnable. Treat the JSON written by each phase as the contract for the next phase.

## Source of Truth

Use these in order when validating claims:

1. `Tools/ContentPipeline/config.py` — defaults, paths, limits, and dependency expectations
2. the phase script itself — CLI behavior and runtime flow
3. `Tools/ContentPipeline/lib/` helpers — manifest shape and fallback behavior
4. `Tools/ContentPipeline/README.md` — operator-facing workflow documentation
5. `phase-map.md` — agent-facing summary of current contracts

If docs disagree with code, fix the docs to match the code unless the task explicitly includes changing behavior.

## Maintenance Expectations

- Keep docs focused on current operation and upkeep, not build history.
- Keep each phase runnable on its own from the CLI.
- Keep `README.md` operator-focused: setup, execution, outputs, troubleshooting.
- Keep `phase-map.md` agent-focused: contracts, assumptions, maintenance checklist.
- Keep `data/` treated as local working state.
- Keep the review UI dependency-light: FastAPI plus inline HTML/CSS/JS, no frontend build step.

## Related Project Files

| Path | Why it matters |
|---|---|
| `Tools/ContentPipeline/README.md` | Operator workflow and troubleshooting |
| `Tools/ContentPipeline/config.py` | Central config and path resolution |
| `Tools/ContentPipeline/lib/` | Manifest schema and ffmpeg/analytics helpers |
| `../core.md` | Clip-worthiness and caption constraints that inform Phase 2 prompt behavior |
| `../clip-strategy/_index.md` | Selection criteria reflected in highlight detection prompts |
| `../platforms/_index.md` | Platform requirements that Phase 4 output is expected to satisfy |

## After Code Changes

Chain to ops and load `ops-change-summary` before handing work back to the operator.
