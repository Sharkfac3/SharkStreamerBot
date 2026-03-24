# Pipeline Tooling — content-repurposer

## Purpose

Covers the local Python tooling that automates the content pipeline from stream recordings to Instagram-ready clips. This skill area bridges the strategic skills (clip-strategy, platforms) with actual executable tools.

## When to Load

- Building, modifying, or debugging any `Tools/ContentPipeline/` script
- Adding a new pipeline phase or enhancing an existing one
- Troubleshooting transcription, highlight detection, clip extraction, or formatting
- Working on the review queue web UI
- Integrating feedback/analytics into highlight detection

## Files in This Skill Area

| File | Purpose |
|---|---|
| `_index.md` | This overview — pipeline architecture and phase relationships |
| `../../../context/pipeline-implementation.md` | Full implementation spec with hardware details, architecture, and per-phase requirements |

## Pipeline Phases

```
Phase 1          Phase 2              Phase 3           Phase 4             Phase 5          Phase 6
transcribe.py → detect_highlights.py → extract_clips.py → format_instagram.py → review_server.py → feedback.py
   ↓                ↓                     ↓                  ↓                    ↓               ↓
 .srt/.json      highlights.json      raw clips.mp4     captioned clips     approved clips    insights
(transcripts/)  (highlights/)        (clips/)          (review_queue/)     (published/)      (feedback.db)
```

Each phase reads the previous phase's output. Each phase is independently runnable and valuable.

## Related Project Files

| Path | Why it matters |
|---|---|
| `Tools/ContentPipeline/` | All pipeline code lives here |
| `Tools/ContentPipeline/data/` | Working directory for pipeline outputs (.gitignored) |
| `Tools/ContentPipeline/config.py` | Paths, model names, thresholds — the central config |
| `Tools/MixItUp/Api/get_commands.py` | Python tool convention reference (argparse, pathlib, docstrings) |
| `Creative/Brand/BRAND-VOICE.md` | Voice rules encoded into Phase 2 LLM prompt |
| `skills/clip-strategy/_index.md` | Clip criteria encoded into Phase 2 LLM prompt |
| `skills/core.md` | Caption rules encoded into Phase 2 LLM prompt |

## Cross-References

- `../core.md` — always load before this skill; defines what makes a good clip and caption rules
- `../clip-strategy/_index.md` — clip selection criteria that Phase 2 encodes into its LLM prompt
- `../platforms/_index.md` — platform specs that Phase 4 must comply with
- `../../ops/role.md` — chain to ops after any code change for change-summary

## Notes for Future Authors

- Each phase should remain independently runnable via CLI (`python <script>.py <args>`)
- Config reads from `Tools/ContentPipeline/.env` with fallback defaults in `config.py`
- The `data/` directory is `.gitignored` — never commit working data
- When adding a new phase, update this file's pipeline diagram and the context spec
- The review queue is vanilla HTML/JS — no build tools, no npm, no framework
