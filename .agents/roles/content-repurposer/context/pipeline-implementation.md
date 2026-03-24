# Content Pipeline — Implementation Spec

> **Source:** Planned 2026-03-24 based on operator hardware assessment and recording inventory analysis.

## Problem

The content-repurposer role has strategy docs (clip criteria, platform specs, brand voice rules) but zero tooling. Stream recordings accumulate in `C:\Users\sharkfac3\Workspace\streamStuff\recordings` with no automated path to Instagram-ready content.

## Operator Infrastructure

| Resource | Spec | Pipeline Use |
|---|---|---|
| Workstation CPU | AMD Threadripper 5965 | LLM inference (Ollama, 70B quantized, ~40GB RAM) |
| Workstation RAM | 128GB | Comfortably runs 70B model alongside other work |
| Workstation GPU | NVIDIA RTX 4080 (16GB VRAM) | Whisper transcription + NVENC video encoding |
| NAS | 8TB+ free | Archive processed recordings and published clips |
| Mac Minis | Not purchased yet | Not needed unless workstation becomes contended during streams |

## Recording Inventory

- **96 total recordings** in the recordings directory
- **45 have vertical counterparts** (`-vertical.mp4`, 9:16 aspect ratio) — OBS outputs these automatically starting Feb 18, 2026
- **51 earlier recordings** are horizontal-only — require center-crop fallback
- File naming: `YYYY-MM-DD HH-MM-SS.mp4` and `YYYY-MM-DD HH-MM-SS-vertical.mp4`
- Typical file size: 1.8–3 GB per recording

## Architecture

```
Source:                              Code (committed):
  streamStuff/recordings/              Tools/ContentPipeline/
    ├── recording.mp4                    ├── config.py
    └── recording-vertical.mp4          ├── transcribe.py        (Phase 1)
                                        ├── detect_highlights.py (Phase 2)
Working Data (.gitignored):             ├── extract_clips.py     (Phase 3)
  Tools/ContentPipeline/data/           ├── format_instagram.py  (Phase 4)
    ├── transcripts/                    ├── review_server.py     (Phase 5)
    ├── highlights/                     ├── feedback.py          (Phase 6)
    ├── clips/                          ├── requirements.txt
    ├── review_queue/                   └── lib/
    └── published/                          ├── ffmpeg_utils.py
                                            ├── transcript_io.py
                                            └── clip_manifest.py
```

## Phase Summary

### Phase 1: Transcription
- **Tool:** `faster-whisper` with `large-v3` model on RTX 4080 (CUDA)
- **Input:** MP4 recording
- **Output:** `data/transcripts/{name}.srt` + `{name}.json` (word-level timestamps, confidence scores)
- **Speed:** ~6-15x realtime (4-hour stream in ~15-40 min)
- **Key detail:** Enable VAD filter to skip silence and prevent Whisper hallucinations
- **Standalone value:** Searchable stream archive

### Phase 2: Highlight Detection
- **Tool:** Ollama (`llama3.1:70b-instruct-q4_K_M` or `llama3.1:8b` for iteration)
- **Input:** Transcript JSON
- **Output:** `data/highlights/{name}.json` — list of `{start_time, end_time, category, description, suggested_caption, confidence_score}`
- **Method:** 5-minute sliding windows with 1-min overlap, LLM system prompt encodes clip criteria from `skills/clip-strategy/_index.md` + caption rules from `skills/core.md` + voice from `Creative/Brand/BRAND-VOICE.md`
- **Key detail:** Transcript-only analysis initially; Phase 2b adds audio energy + scene detection
- **Standalone value:** Timestamped highlight list operator can jump to in VLC

### Phase 3: Clip Extraction
- **Tool:** FFmpeg with NVENC hardware encoding (`h264_nvenc`)
- **Input:** Highlight manifest + vertical MP4 (or horizontal with center-crop fallback)
- **Output:** `data/clips/{date}_{index}_{category}.mp4` at 1080x1920, 30fps, ~8Mbps, AAC
- **Key detail:** Full re-encode (not stream copy) for precise cuts; 2s pre-pad, 1s post-pad
- **Standalone value:** Playable clips ready for manual upload

### Phase 4: Instagram Formatting
- **Tool:** FFmpeg ASS subtitle filter + NVENC
- **Input:** Clip + corresponding transcript segments
- **Output:** `data/review_queue/{name}.mp4` + `{name}_meta.json` (caption, hashtags, category)
- **Key detail:** White text, black outline, lower third, mobile-readable size; ensures Reels compliance (H.264, AAC, ≤250MB, 9:16, 30fps, ≤90s)
- **Standalone value:** Upload-ready clips with burned-in captions

### Phase 5: Review Queue
- **Tool:** FastAPI + uvicorn, vanilla HTML/CSS/JS (no build step)
- **Input:** `data/review_queue/` contents
- **Output:** Approved clips moved to `data/published/` with final metadata
- **UX (ADHD-optimized):** One clip at a time, auto-play, keyboard shortcuts (A/S/T/E), inline caption editing, progress indicator ("3 of 12"), session persistence
- **Standalone value:** Low-friction approval workflow

### Phase 6: Feedback Loop (Future)
- **Tool:** SQLite + Instagram Insights CSV import (or Graph API later)
- **Input:** Published clip metadata + performance metrics
- **Output:** Insights fed back into Phase 2 LLM prompt as scoring context
- **Standalone value:** Knowledge base of what content works

## Dependencies to Install

```
pip install faster-whisper   # Whisper transcription (CTranslate2)
pip install fastapi uvicorn  # Review queue web server
# FFmpeg: winget install ffmpeg (or choco install ffmpeg)
# Ollama: https://ollama.com/download (Windows native)
# Then: ollama pull llama3.1:70b-instruct-q4_K_M
```

## Risks

| Risk | Mitigation |
|---|---|
| Whisper hallucinations on silence | VAD filter in faster-whisper |
| LLM over-suggests highlights | Intentional; tune threshold from review data over time |
| Early recordings lack vertical | Center-crop fallback for horizontal-only files |
| Large files on NAS = slow I/O | Process from local SSD, archive to NAS after |
| FFmpeg not on Windows PATH | Config allows explicit path; document in README |

## Python Conventions

Follow the pattern in `Tools/MixItUp/Api/get_commands.py`: argparse CLI, pathlib for paths, docstrings, stdlib-heavy, clear error messages. Read from `.env` with sensible defaults.
