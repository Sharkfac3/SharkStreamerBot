---
name: feature-voice-commands
description: Voice-command-driven mode and OBS scene switching actions under `Actions/Voice Commands/`. Load when working on voice command scripts or their operator docs.
---

# Voice Commands Feature Skill

## Feature Scope

This feature group is intentionally small and state-light:
- `mode-garage.cs`
- `mode-workspace.cs`
- `mode-gamer.cs`
- `scene-chat.cs`
- `scene-main.cs`
- `scene-housekeeping.cs`
- `README.md`

## Shared Contract

### Global variable
- `stream_mode`

### Canonical mode values
- `garage`
- `workspace`
- `gamer`

### Scene naming convention
All scene-switch actions assume OBS scenes follow:
- `<Mode Prefix>: <Section>`

Current expected mappings:
- `garage` -> `Garage: Chat` / `Garage: Main` / `Garage: Housekeeping`
- `workspace` -> `Workspace: Chat` / `Workspace: Main` / `Workspace: Housekeeping`
- `gamer` -> `Gamer: Chat` / `Gamer: Main` / `Gamer: Housekeeping`

## Behavioral Expectations

### Mode actions
Mode scripts should stay very small and predictable:
- Set `stream_mode` to one canonical lowercase value.
- Avoid chat output unless explicitly requested.
- Avoid unrelated side effects.

### Scene actions
Scene scripts should:
- Read `stream_mode` defensively.
- Normalize null/empty input safely.
- Fall back to `workspace` when the mode is unknown.
- Log warnings instead of failing hard.
- Preserve existing OBS scene naming unless the operator explicitly asks to rename scenes.

**OBS scene switching:** Use `CPH.ObsSetScene(targetScene)` — see `streamerbot-scripting` for the reflection gotcha.

## Editing Guidance

- Prefer targeted edits over refactors across all voice command scripts.
- Keep copy/paste readiness for each script.
- If you add a new mode or scene section, update:
  1. the relevant `.cs` actions,
  2. `Actions/Voice Commands/README.md`, and
  3. this skill if the new pattern becomes part of normal routing knowledge.
- Do not assume shared helper files can be referenced at runtime by Streamer.bot actions.

