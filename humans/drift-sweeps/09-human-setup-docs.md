# Drift Sweep 09 — Human Setup & Pipeline Docs

You are doing a drift sweep for the SharkStreamerBot project. Your job is to read every file listed below, find anything stale or incorrect, and fix it in place. Report each change.

These docs are operator-facing setup guides. They describe how to configure Streamer.bot, Mix It Up, OBS, and other tools. Drift here usually means: file paths changed, port numbers changed, or a setup step references a feature that has been refactored.

Do NOT add features, reorganize, or improve style. Only fix factual drift.

---

## Before you start

1. Read `WORKING.md` at repo root. Check for active file conflicts before editing anything.
2. Edit files in place with minimal diffs — change only what is factually wrong.
3. Do NOT run `git add` or `git commit`. The human operator commits all changes manually.

---

## Ground truth sources (read these first)

- `Apps/stream-overlay/packages/shared/src/topics.ts` — broker port (8765)
- `Apps/info-service/src/index.ts` — info-service port (8766)
- `Apps/production-manager/vite.config.ts` — production-manager dev port (5174), preview (4174)
- `Actions/SHARED-CONSTANTS.md` — global variable names
- `.agents/_shared/mixitup-api.md` — Mix It Up API conventions
- `humans/mixitup-placeholder-checklist.md` — Mix It Up setup tasks (cross-check this against current state too)

---

## Files to sweep

### `humans/setup-info/overlay-setup.md`

Drift to look for:
- Broker port: should be 8765 — cross-check against `topics.ts`
- OBS browser source URL for dev mode: should be `http://localhost:5173`
- OBS local file path for production mode: should point to `Apps/stream-overlay/packages/overlay/dist/index.html`
- Streamer.bot connection steps: cross-check broker-connect script name against `Actions/Overlay/`
- Any setup steps referencing files that no longer exist or have been renamed

### `humans/setup-info/lotat-streamerbot-setup.md`

Drift to look for:
- Script names: cross-check against actual files in `Actions/LotAT/`
- Trigger names and event types: still accurate?
- Global variable names: cross-check against `SHARED-CONSTANTS.md`
- Overlay publish steps: should reference `Actions/LotAT/overlay-publish.cs` copy pattern

### `humans/setup-info/lotat-story-viewer-setup.md`

Drift to look for:
- File paths to story viewer tools: cross-check against `Tools/` directory
- Any port or URL references

### `humans/setup-info/content-pipeline-setup.md`

Drift to look for:
- Tool paths: cross-check against `Tools/ContentPipeline/` directory (if it exists)
- Any platform API steps that are outdated
- Software version requirements that may have changed

### `humans/setup-info/ollama-windows-11-setup.md`

Drift to look for:
- Model names or commands that have changed
- File paths for model storage
- Any integration steps with this project that reference files by path

### `humans/art-pipeline/README.md`

Drift to look for:
- Tool paths: cross-check against `Tools/` and `Creative/Art/` directories
- Any character names or asset paths that have changed
- Setup steps referencing models or tools that have been updated

### `humans/art-pipeline/SETUP.md`

Drift to look for:
- Same as README.md — tool paths, model names, directory structure

### `humans/art-pipeline/FULL-RUN.md`

Drift to look for:
- Any step-by-step instructions referencing files, folders, or commands that have changed
- Character or output path references

### `humans/mixitup-placeholder-checklist.md`

Drift to look for:
- Any Mix It Up commands listed as "create this" that now exist (based on `mixitup-api.md`)
- Payload shapes: cross-check against `mixitup-api.md`
- Custom Intro command: cross-check against the full spec in `mixitup-api.md § Custom Intro Command`
- Any completed tasks that should be marked done or removed from the checklist

### `humans/todo for shark.md`

Drift to look for:
- Any tasks that are clearly complete based on what exists in the repo (built features, committed files)
- Mark complete items clearly or note them — do NOT delete them, as the operator may want to keep the history

---

## Output format

For each file:
- File path
- What was stale
- What you changed it to

If a file has no drift, say so.
