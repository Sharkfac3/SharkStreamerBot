# Content Pipeline

Local Python tooling for turning SharkStreamerBot stream recordings into review-ready short-form clips.

## What the Pipeline Does

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

Each phase can be run on its own. The normal operator flow is:

1. transcribe recordings
2. detect highlights from transcript JSON
3. extract clips from highlight manifests
4. format clips for Instagram review
5. review and approve clips in the web UI
6. sync published metadata and import platform metrics

## Repository Layout

- `config.py` — shared settings loader with `.env` support
- `transcribe.py` — recording → transcript
- `detect_highlights.py` — transcript → highlight manifest
- `extract_clips.py` — highlight manifest → clip files + clip manifest
- `format_instagram.py` — clip manifest → review queue video + metadata
- `review_server.py` — review queue web app
- `feedback.py` — published clip analytics and prompt feedback
- `lib/` — shared manifest, transcript, ffmpeg, and analytics helpers
- `data/` — local working outputs; gitignored

## Setup

### 1. Create a virtual environment

```bash
python3 -m venv .venv
source .venv/bin/activate
pip install -r Tools/ContentPipeline/requirements.txt
```

### 2. Install non-pip dependencies

Required outside Python:

- **FFmpeg** and **ffprobe** on PATH for clip extraction and formatting
- **Ollama** for highlight detection
- an Ollama model, default: `llama3.1:8b`

Example:

```bash
ollama pull llama3.1:8b
```

### 3. Optional config overrides

Copy `Tools/ContentPipeline/.env.example` to `Tools/ContentPipeline/.env` if you need to override defaults.

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
- highlight windows: 5 minutes with 1 minute overlap
- max highlights requested per window: 3
- clip padding: 2 seconds before, 1 second after
- clip output: 1080x1920, 30 fps, `h264_nvenc`, AAC, 8 Mbps
- review limits: 90 seconds max, 250 MB max
- subtitle defaults: Arial, white text, black outline, lower-third placement

`config.py` also converts Windows recording paths correctly when the scripts run from WSL.

## Running the Pipeline

### Phase 1 — Transcription

Dry run:

```bash
python3 Tools/ContentPipeline/transcribe.py --dry-run --limit 3
```

Single recording:

```bash
python3 Tools/ContentPipeline/transcribe.py "2026-02-23 21-11-57.mp4"
```

Batch mode:

```bash
python3 Tools/ContentPipeline/transcribe.py --limit 5
```

Input resolution rules:
- full file path works
- a filename relative to the recordings directory works
- a stem without `.mp4` works

Batch mode scans the recordings directory for `.mp4` files and skips `-vertical.mp4` files to avoid transcribing duplicate audio.

Outputs per recording:
- `data/transcripts/<recording>.srt`
- `data/transcripts/<recording>.json`

The JSON transcript stores source recording info, transcription metadata, segment timestamps, and word timestamps when available.

### Phase 2 — Highlight Detection

Dry run:

```bash
python3 Tools/ContentPipeline/detect_highlights.py --dry-run --limit 3
```

Single transcript:

```bash
python3 Tools/ContentPipeline/detect_highlights.py "2026-02-23 21-11-57"
```

Batch mode:

```bash
python3 Tools/ContentPipeline/detect_highlights.py --limit 5
```

Input resolution rules:
- full transcript JSON path works
- a filename relative to `data/transcripts/` works
- a stem without `.json` works

The detector:
- reads transcript JSON
- splits it into 5-minute windows with 1-minute overlap by default
- calls Ollama over HTTP using the Python standard library
- asks for up to 3 highlights per window by default
- deduplicates overlapping suggestions across windows
- writes one highlight manifest per transcript

If `data/feedback/prompt_context.txt` exists, Phase 2 appends it to the prompt as performance feedback.

Outputs per transcript:
- `data/highlights/<recording>.json`

Each manifest includes source transcript/recording references, detection metadata, and deduplicated highlights with timestamps, category, caption suggestion, confidence, and rank.

### Phase 3 — Clip Extraction

Dry run:

```bash
python3 Tools/ContentPipeline/extract_clips.py --dry-run --limit 3
```

Single manifest:

```bash
python3 Tools/ContentPipeline/extract_clips.py "2026-02-23 21-11-57"
```

Batch mode:

```bash
python3 Tools/ContentPipeline/extract_clips.py --limit 5
```

Input resolution rules:
- full highlight manifest path works
- a filename relative to `data/highlights/` works
- a stem without `.json` works

Extraction behavior:
- uses `ffprobe` to inspect the chosen source recording
- uses `ffmpeg` to re-encode each clip for precise cuts
- adds 2 seconds of pre-pad and 1 second of post-pad by default
- outputs 1080x1920, 30 fps, H.264 + AAC clips
- center-crops horizontal sources to 9:16
- pads portrait sources to the configured 1080x1920 frame

Source recording selection is driven by the highlight manifest's stored source recording reference. If that exact source file is not available, extraction also checks same-stem alternatives in the recordings directory, including a `-vertical.mp4` variant.

Outputs per manifest:
- `data/clips/<recording>_<index>_<category>.mp4`
- `data/clips/<recording>.json`

The clip manifest records clip ranges, output files, source recording metadata, source variant, and ffmpeg/ffprobe context.

### Phase 4 — Instagram Formatting

Dry run:

```bash
python3 Tools/ContentPipeline/format_instagram.py --dry-run --limit 3
```

Single manifest:

```bash
python3 Tools/ContentPipeline/format_instagram.py "2026_03_04_21_01_35"
```

Batch mode:

```bash
python3 Tools/ContentPipeline/format_instagram.py --limit 5
```

Formatting behavior:
- resolves the matching transcript from `data/transcripts/`
- collects transcript segments that overlap each extracted clip range
- builds ASS subtitles with lower-third placement and wrapped mobile-readable lines
- burns subtitles into the clip with FFmpeg
- validates the result against the configured Instagram-ready constraints

Outputs per clip:
- `data/review_queue/<clip>.mp4`
- `data/review_queue/<clip>_meta.json`

Review metadata stores caption text, hashtags, subtitle settings, transcript references, compliance targets, and review workflow state.

### Phase 5 — Review Queue

Start the server:

```bash
python3 Tools/ContentPipeline/review_server.py
```

Optional flags:

```bash
python3 Tools/ContentPipeline/review_server.py --host 0.0.0.0 --port 8765
python3 Tools/ContentPipeline/review_server.py --reload
```

Current review workflow:
- the UI shows one pending item at a time
- clips auto-play in the browser
- caption and hashtags are editable inline
- `A` approves the item
- `S` marks it skipped
- `T` marks it rejected/trash
- `E` focuses the caption field
- `Ctrl+Enter` saves edits without changing queue state

Queue behavior:
- pending and skipped items appear in the active queue
- rejected items stay on disk with updated metadata but no longer appear in the queue
- approved items move from `data/review_queue/` to `data/published/`
- approval writes final caption/hashtag values into published metadata

### Phase 6 — Feedback Loop

Index published metadata:

```bash
python3 Tools/ContentPipeline/feedback.py sync
```

Export a metrics template CSV:

```bash
python3 Tools/ContentPipeline/feedback.py template
```

Import platform metrics:

```bash
python3 Tools/ContentPipeline/feedback.py import-csv Tools/ContentPipeline/data/feedback/metrics_template.csv
```

Print the current summary:

```bash
python3 Tools/ContentPipeline/feedback.py report
python3 Tools/ContentPipeline/feedback.py report --json
```

Matching order during CSV import:
1. `clip_id`
2. `video_file_name` / `file_name`
3. exact caption text

Feedback outputs:
- `data/feedback/feedback.db` — SQLite database
- `data/feedback/summary.json` — aggregate summary
- `data/feedback/prompt_context.txt` — short text block for Phase 2 prompt context

## Working Data and Outputs

`Tools/ContentPipeline/data/` is local working state and should not be committed.

Current output folders:

- `data/transcripts/`
- `data/highlights/`
- `data/clips/`
- `data/review_queue/`
- `data/published/`
- `data/feedback/`

## Troubleshooting

### Missing Python dependencies

Common runtime messages:
- `faster-whisper is not installed` → run `pip install -r Tools/ContentPipeline/requirements.txt`
- `FastAPI is not installed` or `uvicorn is not installed` → run `pip install -r Tools/ContentPipeline/requirements.txt`

`transcribe.py` delays the `faster-whisper` import until actual transcription work starts, so `--help` and `--dry-run` still work before that package is installed.

### FFmpeg or ffprobe not found

If extraction or formatting reports missing FFmpeg tools:
- install FFmpeg so `ffmpeg` and `ffprobe` are on PATH, or
- set `CONTENT_PIPELINE_FFMPEG_PATH` and `CONTENT_PIPELINE_FFPROBE_PATH` in `Tools/ContentPipeline/.env`

### Ollama not reachable or model errors

If highlight detection fails:
- confirm Ollama is running
- confirm the configured model is pulled locally
- confirm `CONTENT_PIPELINE_OLLAMA_URL` matches the running endpoint

### Missing source files

The scripts fail early when required source directories or files are missing. Typical causes:
- recordings directory does not exist
- transcript JSON has not been generated yet
- highlight manifest has not been generated yet
- transcript file expected by `format_instagram.py` is missing

Run the phases in order unless you are intentionally targeting a specific existing intermediate file.

### Clip formatting validation failures

`format_instagram.py` rejects outputs that exceed configured limits, including:
- duration over 90 seconds
- output file larger than 250 MB
- output video codec not H.264
- output audio codec not AAC
- output resolution not 1080x1920

If this happens, inspect the source clip duration and your config overrides.

## Maintenance Expectations

- Keep each phase independently runnable from the CLI.
- Keep manifest schemas stable or document changes in the agent-facing pipeline skill docs.
- Keep operator docs in this README aligned with `config.py` defaults and actual script behavior.
- Keep `data/` local-only.
- The review UI is plain FastAPI + inline HTML/CSS/JS; there is no frontend build step.
