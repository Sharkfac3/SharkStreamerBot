# Voice Commands — Feature Overview

## Scripts

| Script | Path | Purpose |
|---|---|---|
| `mode-garage.cs` | `Actions/Voice Commands/` | Sets `stream_mode = "garage"` |
| `mode-workspace.cs` | `Actions/Voice Commands/` | Sets `stream_mode = "workspace"` |
| `mode-gamer.cs` | `Actions/Voice Commands/` | Sets `stream_mode = "gamer"` |
| `scene-chat.cs` | `Actions/Voice Commands/` | Switches to mode-appropriate chat scene |
| `scene-main.cs` | `Actions/Voice Commands/` | Switches to mode-appropriate main scene |
| `scene-housekeeping.cs` | `Actions/Voice Commands/` | Switches to mode-appropriate housekeeping scene |
| `scene-dance.cs` | `Actions/Voice Commands/` | Switches to mode-appropriate disco/dance scene |

## Global Variable

`stream_mode` — canonical mode string, set by mode scripts, read by scene scripts and channel point scripts.

## Canonical Mode Values

- `garage`
- `workspace`
- `gamer`

## Scene Naming Convention

All scene-switch actions assume OBS scenes follow: `<Mode Prefix>: <Section>`

| Mode | Chat | Main | Housekeeping | Dance |
|---|---|---|---|---|
| `garage` | `Garage: Chat` | `Garage: Main` | `Garage: Housekeeping` | `Disco Party: Garage` |
| `workspace` | `Workspace: Chat` | `Workspace: Main` | `Workspace: Housekeeping` | `Disco Party: Workspace` |
| `gamer` | `Gamer: Chat` | `Gamer: Main` | `Gamer: Housekeeping` | `Disco Party: Gamer` |

## Behavioral Expectations

**Mode scripts:**
- Set `stream_mode` to one canonical lowercase value
- Avoid chat output unless explicitly requested
- Avoid unrelated side effects

**Scene scripts:**
- Read `stream_mode` defensively
- Normalize null/empty input safely
- Fall back to `workspace` when mode is unknown
- Log warnings instead of failing hard
- Preserve existing OBS scene naming unless operator explicitly requests rename

**OBS call:** `CPH.ObsSetScene(targetScene)` — see `streamerbot-dev/skills/core.md` for the reflection gotcha.

## Editing Guidance

- Prefer targeted edits over refactors across all voice command scripts
- Keep copy/paste readiness for each script
- If adding a new mode or scene section, update: the relevant `.cs` actions, `Actions/Voice Commands/README.md`, and the relevant skill file
- Do not assume shared helper files can be referenced at runtime by Streamer.bot actions

## Detailed Docs

`Actions/Voice Commands/README.md`
