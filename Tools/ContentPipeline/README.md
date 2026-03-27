# Content Pipeline

Local Python tooling for turning SharkStreamerBot stream recordings into review-ready short-form clips.

This README is the agent/developer reference for the tooling in `Tools/ContentPipeline/`.

## Pipeline Overview

The pipeline is split into six independently runnable phases:

```text
recording.mp4
  ↓
transcribe.py
  ↓  data/transcripts/<recording>.srt + .json

detect_highlights.py
  ↓  data/highlights/<recording>.json

extract_clips.py
  ↓  data/clips/<recording>_<index>_<category>.mp4 + data/clips/<recording>.json

format_instagram.py
  ↓  data/review_queue/<clip>.mp4 + <clip>_meta.json

review_server.py
  ↓  approved items moved to data/published/

feedback.py
  ↓  data/feedback/feedback.db + summary.json + prompt_context.txt
```

Each phase remains runnable on its own. `run_pipeline.py` is the orchestrator wrapper that executes Phases 1–4 in sequence, then launches the review UI unless `--skip-review` is set.

## Repository Layout

- `config.py` — shared settings loader with `.env` support
- `check_setup.py` — preflight environment validation for operator machines
- `run_pipeline.py` — one-recording orchestrator for Phases 1–4 plus review UI launch
- `transcribe.py` — recording → transcript
- `detect_highlights.py` — transcript → highlight manifest
- `extract_clips.py` — highlight manifest → clip files + clip manifest
- `format_instagram.py` — clip manifest → review queue video + metadata
- `review_server.py` — review queue web app
- `feedback.py` — published clip analytics and prompt feedback
- `lib/` — shared manifest, transcript, ffmpeg, and analytics helpers
- `data/` — local working outputs; gitignored

## Settings and Defaults

`config.py` is the source of truth for settings. `Tools/ContentPipeline/.env.example` documents the supported overrides.

Ollama integration note:
- the pipeline talks to Ollama over HTTP
- the default URL is `http://localhost:11434/api/generate`
- in the preferred operator setup for this project, Ollama runs natively on Windows 11 while the repo tools run from WSL
- as long as WSL can reach `http://localhost:11434`, no pipeline code changes are required

Important defaults from `config.py`:

- recordings dir: `C:\Users\sharkfac3\Workspace\streamStuff\recordings`
- data dir: `Tools/ContentPipeline/data/`
- transcripts dir: `data/transcripts/`
- highlights dir: `data/highlights/`
- clips dir: `data/clips/`
- review queue dir: `data/review_queue/`
- published dir: `data/published/`
- feedback dir: `data/feedback/`
- Whisper model: `large-v3`
- Whisper device: `auto`
- Whisper compute type: `auto`
- Whisper VAD: enabled
- word timestamps: enabled
- Ollama URL: `http://localhost:11434/api/generate`
- Ollama model: `llama3.1:8b`
- Ollama keep-alive during Phase 2 requests: `10m`
- Ollama unload after Phase 2 run: enabled
- highlight windows: 5 minutes with 1 minute overlap
- max highlights requested per window: 3
- clip padding: 2 seconds before, 1 second after
- clip output: 1080x1920, 30 fps, `h264_nvenc`, AAC, 8 Mbps
- review limits: 90 seconds max, 250 MB max
- subtitle defaults: Arial, white text, black outline, lower-third placement

`config.py` also converts Windows recording paths correctly when scripts run from WSL.

## Phase Contracts

### Phase 1 — `transcribe.py`

Responsibilities:
- resolve recordings by absolute path, file name, or stem
- scan the recordings directory in batch mode
- skip `-vertical.mp4` recordings in batch mode to avoid duplicate audio work
- write both subtitle and JSON transcript outputs

Outputs per recording:
- `data/transcripts/<recording>.srt`
- `data/transcripts/<recording>.json`

Transcript JSON stores source recording info, transcription metadata, segment timestamps, and word timestamps when available.

### Phase 2 — `detect_highlights.py`

Responsibilities:
- read transcript JSON from `data/transcripts/`
- split transcript content into overlapping time windows
- call Ollama over HTTP using the Python standard library
- support either WSL-hosted or Windows-hosted Ollama, as long as the configured URL is reachable from the running environment
- keep the configured model warm while Phase 2 is actively processing windows
- explicitly unload the configured model when the run ends so it does not idle in VRAM
- request up to the configured number of highlights per window
- deduplicate overlapping suggestions across windows
- append `data/feedback/prompt_context.txt` to the prompt when present

Outputs per transcript:
- `data/highlights/<recording>.json`

Highlight manifests include source transcript/recording references, detection metadata, and deduplicated highlights with timestamps, category, caption suggestion, confidence, and rank.

### Phase 3 — `extract_clips.py`

Responsibilities:
- inspect source media with `ffprobe`
- re-encode clips with `ffmpeg` for precise cuts
- apply configured pre-pad and post-pad values
- output 1080x1920, 30 fps, H.264 + AAC clips
- center-crop horizontal sources to 9:16
- pad portrait sources to the configured target frame
- resolve missing exact source files through same-stem alternatives, including `-vertical.mp4`

Outputs per manifest:
- `data/clips/<recording>_<index>_<category>.mp4`
- `data/clips/<recording>.json`

The clip manifest records clip ranges, output files, source recording metadata, source variant, and ffmpeg/ffprobe context.

### Phase 4 — `format_instagram.py`

Responsibilities:
- resolve the matching transcript from `data/transcripts/`
- collect transcript segments overlapping each extracted clip range
- build ASS subtitles with lower-third placement and wrapped mobile-readable lines
- burn subtitles into the clip with FFmpeg
- validate the output against configured Instagram-ready constraints

Outputs per clip:
- `data/review_queue/<clip>.mp4`
- `data/review_queue/<clip>_meta.json`

Review metadata stores caption text, hashtags, subtitle settings, transcript references, compliance targets, and review workflow state.

### Phase 5 — `review_server.py`

Responsibilities:
- serve one pending clip at a time
- autoplay queue items in the browser
- allow inline editing of caption and hashtags
- keep pending and skipped items in the active queue
- keep rejected items on disk but remove them from the active queue
- move approved items from `data/review_queue/` to `data/published/`
- write final caption and hashtag values into published metadata

Current hotkeys:
- `A` approves the item
- `S` marks it skipped
- `T` marks it rejected/trash
- `E` focuses the caption field
- `Ctrl+Enter` saves edits without changing queue state

Default direct-script port remains `8765`. `run_pipeline.py` and the root review-server launcher bind the review UI to `http://localhost:8000` for operator consistency.

### Phase 6 — `feedback.py`

Responsibilities:
- index published metadata into SQLite
- export a metrics template CSV
- import platform metrics CSV data
- rebuild aggregate summaries and prompt feedback context for Phase 2

Feedback outputs:
- `data/feedback/feedback.db` — SQLite database
- `data/feedback/summary.json` — aggregate summary
- `data/feedback/prompt_context.txt` — short text block for Phase 2 prompt context

CSV import matching order:
1. `clip_id`
2. `video_file_name` / `file_name`
3. exact caption text

## Working Data and Outputs

`Tools/ContentPipeline/data/` is local working state and should not be committed.

Current output folders:

- `data/transcripts/`
- `data/highlights/`
- `data/clips/`
- `data/review_queue/`
- `data/published/`
- `data/feedback/`

## Agent Notes

- Keep each phase independently runnable from the CLI.
- Treat `config.py` as the canonical settings contract.
- Keep manifest schemas stable, or document changes in the agent-facing pipeline skill docs.
- Keep operator-facing instructions in the dedicated operator docs, not in this folder.
- Keep `data/` local-only.
- The review UI is plain FastAPI + inline HTML/CSS/JS; there is no frontend build step.
- Phase 3 currently requires NVIDIA NVENC via `h264_nvenc`; there is no CPU fallback path in the current tooling.
