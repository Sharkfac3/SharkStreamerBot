# Operator Prompt: Build Content Pipeline

> **What this is:** A prompt you paste into a new Claude Code session to build the content pipeline one phase at a time. Each run builds one phase. Run it repeatedly, advancing through phases as each one is completed and verified.

---

## Prompt

```
You are the content-repurposer agent for this project.

Before starting, read these files in order:
1. `.agents/ENTRY.md`
2. `.agents/roles/content-repurposer/role.md`
3. `.agents/roles/content-repurposer/skills/core.md`
4. `.agents/roles/content-repurposer/context/pipeline-implementation.md`
5. `.agents/roles/content-repurposer/skills/pipeline/_index.md`
6. `Tools/MixItUp/Api/get_commands.py` (for Python tool conventions)
7. `WORKING.md` (check for conflicts)

Your job: Build the next unbuilt phase of the content pipeline described in
`context/pipeline-implementation.md`. The phases are:

  Phase 1: transcribe.py (faster-whisper transcription)
  Phase 2: detect_highlights.py (Ollama LLM highlight detection)
  Phase 3: extract_clips.py (FFmpeg clip extraction)
  Phase 4: format_instagram.py (subtitle burning + Instagram formatting)
  Phase 5: review_server.py (FastAPI review queue web UI)
  Phase 6: feedback.py (SQLite analytics feedback loop)

To determine which phase to build next:
- Check what already exists in `Tools/ContentPipeline/`
- The next phase is the first one whose main script does not yet exist
- If no scripts exist yet, start with Phase 1 and create the project structure
  (config.py, requirements.txt, lib/ folder, .gitignore entries, README.md)

For the phase you are building:
1. Read the implementation spec in `context/pipeline-implementation.md` for that phase
2. Read the brand docs referenced in the spec (BRAND-VOICE.md, clip-strategy, etc.)
   if the phase needs them (Phase 2 especially)
3. Follow the Python conventions from `get_commands.py`: argparse CLI, pathlib,
   docstrings, clear error messages
4. Write the code
5. Test it against a real recording if possible
6. Register your work in WORKING.md before starting and clear it when done
7. Run the ops change-summary skill after completing code changes

IMPORTANT constraints:
- Recordings are at: C:\Users\sharkfac3\Workspace\streamStuff\recordings
- Working data goes in: Tools/ContentPipeline/data/ (this is .gitignored)
- Config reads from: Tools/ContentPipeline/.env (also .gitignored)
- Vertical recordings end in -vertical.mp4 — prefer these for clip extraction
- Earlier recordings (before Feb 2026) may not have vertical versions
- The operator has ADHD — minimize friction, maximize clarity
- Commit directly to main for single-phase work (per project conventions)
```

---

## Usage

1. Open a new Claude Code session in the SharkStreamerBot repo
2. Paste the prompt above
3. Let the agent build one phase
4. Verify the phase works (see verification steps in `context/pipeline-implementation.md`)
5. Repeat for the next phase in a new session

## Prerequisites

Before running Phase 1 for the first time, ensure these are installed:

```bash
# FFmpeg (needed from Phase 3 onward, but install now)
winget install ffmpeg

# Ollama (needed from Phase 2 onward)
# Download from https://ollama.com/download
# Then pull the model:
ollama pull llama3.1:70b-instruct-q4_K_M
# Or for faster iteration during development:
ollama pull llama3.1:8b

# Python dependencies will be installed by the agent via requirements.txt
```

## Phase Verification Checklist

After each phase, manually verify:

- [ ] **Phase 1:** `python Tools/ContentPipeline/transcribe.py "path/to/recording.mp4"` produces `.srt` + `.json` in `data/transcripts/`; open the SRT in a text editor and spot-check timestamps against the video
- [ ] **Phase 2:** `python Tools/ContentPipeline/detect_highlights.py "data/transcripts/recording.json"` produces highlights JSON; open the recording in VLC and jump to 3-5 suggested timestamps to confirm they are interesting
- [ ] **Phase 3:** `python Tools/ContentPipeline/extract_clips.py "data/highlights/recording.json"` produces playable MP4 clips at 9:16; play them and confirm they start/end at reasonable points
- [ ] **Phase 4:** `python Tools/ContentPipeline/format_instagram.py "data/clips/clip.mp4"` produces a clip with readable burned-in captions; confirm the file plays in Instagram's upload preview
- [ ] **Phase 5:** `python Tools/ContentPipeline/review_server.py` starts on localhost:8080; open in browser, confirm clips play, keyboard shortcuts work, and approve/reject persists
- [ ] **Phase 6:** Import test metrics and confirm they appear in the feedback database; run highlight detection with feedback context and confirm the prompt includes performance data
