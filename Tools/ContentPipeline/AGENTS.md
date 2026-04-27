---
id: tools-content-pipeline
type: domain-route
description: Short-form content tooling for transcription, highlight detection, clip extraction, review, publish, and feedback loops.
owner: content-repurposer
secondaryOwners:
  - ops
workflows:
  - change-summary
  - validation
status: active
---

# Tools/ContentPipeline — Agent Guide

## Purpose

[Tools/ContentPipeline/](./) contains local Python tooling that turns stream recordings into review-ready short-form clips. It owns the mechanics for transcription, highlight detection, clip extraction, vertical formatting, operator review, and feedback capture.

The content-repurposer role owns clip-worthiness, platform fit, captions, and feedback interpretation. This folder owns the current tooling contracts that make those decisions repeatable.

## When to Activate

Use this guide when working on:

- any script under [Tools/ContentPipeline/](./)
- pipeline manifest schemas or phase-to-phase contracts
- transcription, highlight detection, clip extraction, formatting, review, publishing, or feedback behavior
- environment setup, dependency checks, or operator troubleshooting for this pipeline
- docs that describe how the content pipeline currently behaves

Do not activate this guide for manual clip strategy, platform copywriting, or content calendars unless the work also changes local tooling.

## Primary Owner

Primary owner: `content-repurposer`.

`content-repurposer` owns short-form strategy, selection criteria, caption/platform requirements, review expectations, and how analytics feedback should influence future clips.

## Secondary Owners / Chain To

| Role | Chain when |
|---|---|
| `ops` | Running local tooling validation, dependency checks, repo handoff, or agent-tree validation. |
| `brand-steward` | Captions, public positioning, or platform copy affects brand voice or community messaging. |
| `product-dev` | A clip is intended to package technical product/R&D knowledge for customer-facing reuse. |
| `app-dev` | Pipeline output needs app/dashboard integration rather than local scripts. |

## Required Reading

Read these first for content-pipeline work:

1. [Tools/ContentPipeline/README.md](./README.md) — main developer/agent reference.
2. [Tools/ContentPipeline/config.py](./config.py) — defaults, paths, limits, and dependency expectations.
3. [Tools/ContentPipeline/check_setup.py](./check_setup.py) — operator preflight validation.
4. [Tools/ContentPipeline/run_pipeline.py](./run_pipeline.py) — orchestrator for Phases 1 through 4 plus review launch.
5. [Tools/ContentPipeline/lib/](./lib/) — shared manifest, transcript, ffmpeg, and analytics helpers.
6. [Tools/ContentPipeline/.env.example](./.env.example) — supported local overrides.

## Local Workflow

The pipeline has six independently runnable phases:

1. [transcribe.py](./transcribe.py) creates transcript outputs.
2. [detect_highlights.py](./detect_highlights.py) finds highlight candidates.
3. [extract_clips.py](./extract_clips.py) cuts and reframes clips.
4. [format_instagram.py](./format_instagram.py) burns subtitles and prepares review metadata.
5. [review_server.py](./review_server.py) serves the operator review UI and moves approved clips.
6. [feedback.py](./feedback.py) indexes published clips and platform metrics for future prompt context.

[run_pipeline.py](./run_pipeline.py) is the wrapper for one-recording runs through Phases 1 to 4, then review UI launch unless review is skipped.

```bash
python3 Tools/ContentPipeline/check_setup.py
python3 Tools/ContentPipeline/run_pipeline.py <recording-file-or-stem>
```

Keep each phase usable directly from the CLI. Treat JSON files written by each phase as the contract for the next phase.

## Validation

For dependency and environment preflight, run:

```bash
python3 Tools/ContentPipeline/check_setup.py
```

For syntax-level validation after code edits, run targeted Python compilation, for example:

```bash
python3 -m py_compile Tools/ContentPipeline/*.py Tools/ContentPipeline/lib/*.py
```

For agent-doc changes, follow [.agents/workflows/validation.md](../../.agents/workflows/validation.md) and run the agent-tree validator with the task-requested report path.

When a pipeline behavior changes, also run the smallest direct phase command that exercises the changed contract, and record the output in the handoff.

## Boundaries / Out of Scope

Do not use this folder to:

- commit local working media, transcripts, clips, review queues, published outputs, or feedback databases
- add build-history notes to operational docs
- hard-couple phases so direct single-phase runs stop working
- change brand voice or platform strategy without `brand-steward` or `content-repurposer` review
- move operator review into a complex frontend build unless explicitly requested
- silently change output naming, directories, or metadata fields without updating docs and downstream assumptions

## Handoff Notes

After changes, report:

- changed files in [Tools/ContentPipeline/](./)
- setup or dependency changes
- phase commands run and exact validation output
- changed manifest fields or status behavior
- output directory or naming changes
- review/publish behavior changes
- analytics or feedback prompt-context changes

## Runtime Notes

### Phase map

| Phase | Script | Key contract |
|---|---|---|
| 1 | [transcribe.py](./transcribe.py) | Produces transcript subtitle and JSON outputs; batch discovery skips vertical duplicates. |
| 2 | [detect_highlights.py](./detect_highlights.py) | Uses overlapping transcript windows, Ollama HTTP calls, category ranking, and optional feedback prompt context. |
| 3 | [extract_clips.py](./extract_clips.py) | Re-encodes clips with configured padding and vertical output framing. |
| 4 | [format_instagram.py](./format_instagram.py) | Burns ASS subtitles, validates platform constraints, and writes review metadata. |
| 5 | [review_server.py](./review_server.py) | Keeps pending/skipped items active, rejects without deleting, and moves approved items to published output. |
| 6 | [feedback.py](./feedback.py) | Syncs published metadata, imports metrics CSVs, and rebuilds summary and prompt context outputs. |

### Operational assumptions

- [config.py](./config.py) is the source of truth for defaults and path resolution.
- Scripts should work from Windows or WSL, including Windows-style recording paths.
- Ollama is reached over HTTP; the preferred operator setup is Windows-hosted Ollama reachable from WSL at localhost.
- The review UI is FastAPI plus inline HTML, CSS, and JavaScript.
- Phase 3 currently requires NVIDIA NVENC through FFmpeg; there is no CPU fallback path in the current tooling.
- Local working state belongs under the pipeline data folder and remains gitignored.

## Known Gotchas

- Changing review metadata affects review, publishing, analytics, and feedback import.
- Phase 2 prompt feedback is consumed from the generated prompt-context output only.
- CSV metric import matches by clip id first, then file name, then exact caption text.
- Transcript lookup strips a trailing vertical stem during formatting when needed.
- Source recording resolution comes from stored metadata first, then same-stem candidates in the recordings directory.
