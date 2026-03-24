# Content Pipeline

Local Python tooling for turning stream recordings into short-form content.

## Phase Status

- [x] Phase 1 — `transcribe.py`
- [x] Phase 2 — `detect_highlights.py`
- [x] Phase 3 — `extract_clips.py`
- [x] Phase 4 — `format_instagram.py`
- [x] Phase 5 — `review_server.py`
- [ ] Phase 6 — `feedback.py`

## What Exists Right Now

This pass ships **Phase 1 transcription**, **Phase 2 highlight detection**, **Phase 3 clip extraction**, **Phase 4 Instagram formatting**, and **Phase 5 review queue tooling**:

- `config.py` — central config loader with `.env` support
- `transcribe.py` — faster-whisper transcription CLI
- `detect_highlights.py` — Ollama-powered highlight detection CLI
- `extract_clips.py` — FFmpeg-powered clip extraction CLI
- `format_instagram.py` — subtitle burning + Instagram review formatting CLI
- `review_server.py` — FastAPI review queue with keyboard-first one-clip workflow
- `requirements.txt` — Python dependency list
- `lib/transcript_io.py` — transcript readers/writers and time-range helpers
- `lib/highlight_io.py` — highlight manifest helpers
- `lib/clip_manifest.py` — clip/review manifest readers/writers
- `lib/ffmpeg_utils.py` — ffmpeg/ffprobe helpers
- `data/` — working output folders (gitignored)

## Install

```bash
python3 -m venv .venv
source .venv/bin/activate
pip install -r Tools/ContentPipeline/requirements.txt
```

Install local non-pip dependencies separately:

- FFmpeg on PATH for clip extraction and formatting
- Ollama: https://ollama.com/download
- Pull a model: `ollama pull llama3.1:8b`

## Configure

Create `Tools/ContentPipeline/.env` from `.env.example` if you want to override defaults.

Important defaults:

- recordings directory: `C:\Users\sharkfac3\Workspace\streamStuff\recordings`
- transcript output: `Tools/ContentPipeline/data/transcripts/`
- highlight output: `Tools/ContentPipeline/data/highlights/`
- clip output: `Tools/ContentPipeline/data/clips/`
- review queue output: `Tools/ContentPipeline/data/review_queue/`
- Whisper model: `large-v3`
- Whisper VAD filter: enabled
- Ollama URL: `http://localhost:11434/api/generate`
- Ollama model: `llama3.1:8b`
- highlight windows: 5 minutes with 1 minute overlap
- ffmpeg/ffprobe: auto-detected from PATH when possible
- clip padding: 2 seconds before, 1 second after
- clip output: 1080x1920, 30fps, H.264 NVENC, AAC, 8 Mbps
- review output: 1080x1920, 30fps, H.264/AAC, ≤90s, ≤250 MB
- subtitle defaults: Arial, white text, black outline, lower-third placement

The config loader also understands Windows-style paths when the tool is run from WSL.

## Usage

### Phase 1 — dry run transcription

```bash
python3 Tools/ContentPipeline/transcribe.py --dry-run --limit 3
```

### Phase 1 — transcribe one recording

```bash
python3 Tools/ContentPipeline/transcribe.py "2026-02-23 21-11-57.mp4"
```

You can pass either:
- a full path
- a filename relative to the recordings folder
- a filename stem without `.mp4`

### Phase 1 — batch transcribe recordings

```bash
python3 Tools/ContentPipeline/transcribe.py --limit 5
```

By default, batch mode scans the recordings directory and processes **non-vertical** `.mp4` files. This avoids duplicating work on `-vertical.mp4` recordings that contain the same audio.

### Phase 2 — dry run highlight detection

```bash
python3 Tools/ContentPipeline/detect_highlights.py --dry-run --limit 3
```

### Phase 2 — detect highlights for one transcript

```bash
python3 Tools/ContentPipeline/detect_highlights.py "2026-02-23 21-11-57"
```

You can pass either:
- a full transcript JSON path
- a filename relative to the transcripts folder
- a transcript stem without `.json`

### Phase 2 — batch detect highlights

```bash
python3 Tools/ContentPipeline/detect_highlights.py --limit 5
```

### Phase 3 — dry run clip extraction

```bash
python3 Tools/ContentPipeline/extract_clips.py --dry-run --limit 3
```

### Phase 3 — extract clips for one highlight manifest

```bash
python3 Tools/ContentPipeline/extract_clips.py "2026-02-23 21-11-57"
```

You can pass either:
- a full highlight manifest JSON path
- a filename relative to the highlights folder
- a highlight manifest stem without `.json`

### Phase 3 — batch extract clips

```bash
python3 Tools/ContentPipeline/extract_clips.py --limit 5
```

Clip extraction prefers `-vertical.mp4` recordings when available. If a vertical counterpart does not exist, it falls back to the horizontal recording and center-crops it to 9:16.

### Phase 4 — dry run Instagram formatting

```bash
python3 Tools/ContentPipeline/format_instagram.py --dry-run --limit 3
```

### Phase 4 — format one clip manifest for review

```bash
python3 Tools/ContentPipeline/format_instagram.py "2026_03_04_21_01_35"
```

You can pass either:
- a full clip manifest JSON path
- a filename relative to the clips folder
- a clip manifest stem without `.json`

### Phase 4 — batch format Instagram review clips

```bash
python3 Tools/ContentPipeline/format_instagram.py --limit 5
```

Formatting reads the matching transcript from `data/transcripts/`, burns ASS subtitles into each extracted clip, and writes review-ready outputs plus metadata into `data/review_queue/`.

### Phase 5 — launch the review queue web UI

```bash
python3 Tools/ContentPipeline/review_server.py
```

Optional flags:

```bash
python3 Tools/ContentPipeline/review_server.py --host 0.0.0.0 --port 8765
python3 Tools/ContentPipeline/review_server.py --reload
```

The review server:
- shows one pending clip at a time
- auto-plays the current clip
- supports inline caption + hashtag editing
- supports keyboard shortcuts: `A` approve, `S` skip, `T` trash, `E` focus caption, `Ctrl+Enter` save edits
- moves approved clips from `data/review_queue/` to `data/published/`

## Outputs

### Phase 1

For each recording, the tool writes:

- `data/transcripts/<recording-name>.srt`
- `data/transcripts/<recording-name>.json`

The JSON output includes:
- transcript metadata
- segment-level timestamps
- word-level timestamps when available
- confidence-related fields returned by faster-whisper

### Phase 2

For each transcript, the tool writes:

- `data/highlights/<recording-name>.json`

The highlight manifest includes:
- source transcript and recording metadata
- Ollama model + endpoint metadata
- deduplicated highlight suggestions across sliding windows
- per-highlight category, description, suggested caption, confidence, and absolute timestamps

### Phase 3

For each highlight manifest, the tool writes:

- `data/clips/<recording>_<index>_<category>.mp4`
- `data/clips/<recording>.json`

The clip manifest includes:
- selected source recording and whether vertical or horizontal fallback was used
- ffmpeg/ffprobe metadata for the source video
- padded extraction ranges per clip
- output clip paths and upstream highlight metadata

### Phase 4

For each clip entry in a clip manifest, the tool writes:

- `data/review_queue/<clip-name>.mp4`
- `data/review_queue/<clip-name>_meta.json`

The review metadata includes:
- source clip manifest and transcript references
- generated caption + hashtag bundle
- subtitle styling info and transcript segment counts
- final ffprobe validation details for the review-ready output

### Phase 5

When a clip is reviewed:

- skipped/rejected clips stay in `data/review_queue/` with updated `review_state`
- approved clips move to `data/published/`
- approved metadata stores the final caption/hashtags used at approval time

## Notes

- `faster-whisper` is imported only when transcription actually runs, so `--help` and `--dry-run` work even before dependencies are installed.
- Phase 2 uses the Python standard library to call Ollama over HTTP — no extra pip package required.
- Phase 3 uses `ffprobe` to inspect input media and `ffmpeg` for full re-encode extraction.
- Phase 4 uses ASS subtitles through FFmpeg and validates final H.264/AAC output against Instagram-oriented constraints.
- If CUDA is unavailable for transcription, set `CONTENT_PIPELINE_WHISPER_DEVICE=cpu` in `.env`.
- If FFmpeg is not detected automatically, set `CONTENT_PIPELINE_FFMPEG_PATH` and `CONTENT_PIPELINE_FFPROBE_PATH` in `.env`.
- `data/` and `.env` are local-only and should not be committed.
