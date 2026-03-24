# Content Pipeline Phase Map

Current-state reference for agents working on `Tools/ContentPipeline/`.

## Phase Contracts

### Phase 1 — `transcribe.py`

- **Consumes:** recording `.mp4`
- **Produces:** `data/transcripts/<recording>.srt` and `.json`
- **Key code contracts:**
  - batch discovery skips `-vertical.mp4` files
  - transcript JSON stores `source.file_name`, `source.path`, transcription metadata, and segments
  - `faster-whisper` is imported lazily at runtime
  - VAD and word timestamps default on through `config.py`

### Phase 2 — `detect_highlights.py`

- **Consumes:** transcript JSON
- **Produces:** `data/highlights/<recording>.json`
- **Key code contracts:**
  - transcript segments are grouped into overlapping sliding windows
  - Ollama is called over raw HTTP via `urllib.request`
  - valid categories are `technical`, `entertainment`, and `hybrid`
  - overlapping suggestions are deduplicated and ranked
  - optional prompt feedback is loaded from `data/feedback/prompt_context.txt`

### Phase 3 — `extract_clips.py`

- **Consumes:** highlight manifest JSON plus source recording media
- **Produces:** extracted clip `.mp4` files and one clip manifest JSON
- **Key code contracts:**
  - extraction uses full re-encode, not stream copy
  - default padding is 2 seconds before and 1 second after each highlight
  - horizontal video is scaled and center-cropped to 9:16
  - portrait video is scaled and padded to the configured output frame
  - source recording resolution comes from stored source metadata first, then same-stem candidates in the recordings directory

### Phase 4 — `format_instagram.py`

- **Consumes:** clip manifest JSON, extracted clip video, transcript JSON
- **Produces:** `data/review_queue/<clip>.mp4` and `<clip>_meta.json`
- **Key code contracts:**
  - transcript lookup strips a trailing `-vertical` stem when needed
  - subtitles are generated as ASS and burned in with FFmpeg
  - output validation enforces configured duration, size, codec, and resolution limits
  - metadata stores caption, hashtags, subtitle settings, compliance targets, and source references

### Phase 5 — `review_server.py`

- **Consumes:** review queue metadata and review videos
- **Produces:** updated queue metadata; approved items moved into `data/published/`
- **Key code contracts:**
  - active queue includes `pending` and `skipped` items only
  - `rejected` items remain on disk but leave the active queue
  - approval moves both video and metadata into `data/published/`
  - final caption and hashtags are written into the `published` block
  - UI is a single-file FastAPI app with inline HTML/CSS/JS

### Phase 6 — `feedback.py`

- **Consumes:** published metadata and optional platform metrics CSVs
- **Produces:** `feedback.db`, `summary.json`, `prompt_context.txt`, and optional template CSV output
- **Key code contracts:**
  - SQLite schema lives in `lib/feedback_store.py`
  - `sync` re-indexes published metadata before rebuilding summary files
  - CSV import matches by clip id, file name, then exact caption text
  - summary output is the only feedback automatically consumed by Phase 2

## Operational Assumptions That Still Matter

- `config.py` is the source of truth for defaults and path resolution.
- Scripts should work whether run from Windows or WSL, including Windows-style recording paths.
- `Tools/ContentPipeline/data/` is disposable local state, not committed project data.
- Pipeline phases are useful independently; do not hard-couple them into one required orchestrator.
- Review metadata is part of the workflow contract. Changes there affect review, publishing, and analytics.

## Maintenance Checklist for Future Authors

When changing the pipeline:

1. Verify README claims against actual code, especially defaults and fallback behavior.
2. Preserve CLI usability for direct single-phase runs.
3. Update manifest-building helpers in `lib/` before documenting new fields.
4. Keep review workflow behavior explicit when changing statuses, move rules, or metadata fields.
5. Keep Phase 2 prompt inputs aligned with content-repurposer strategy docs, but describe only current behavior in pipeline docs.
6. If you change output directories, config names, or file naming, update both operator docs and this phase map.
7. Do not add build-history notes here; keep this file limited to current operating contracts.
