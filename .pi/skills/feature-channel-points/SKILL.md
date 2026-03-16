---
name: feature-channel-points
description: Twitch Channel Point redeem scripts (Disco Party, Explain Current Task). Load when working on any script under Actions/Twitch Channel Points/.
---

# Feature: Channel Points

## Scope

| Script | Path | Trigger |
|---|---|---|
| Disco Party | `Actions/Twitch Channel Points/disco-party.cs` | Channel Reward → Reward Redemption |
| Explain Current Task | `Actions/Twitch Channel Points/explain-current-task.cs` | Channel Reward → Reward Redemption |

## Detailed Docs

- `Actions/Twitch Channel Points/README.md`

**Read the README before making changes.**

## Behavioral Expectations

- Scripts are triggered by specific named channel point redeems — do not change reward names or IDs without updating the Streamer.bot wiring.
- Disco Party switches OBS to a mode-specific scene using `CPH.ObsSetScene()` — see `streamerbot-scripting` for the OBS API notes.
- Explain Current Task ensures recording is active before triggering Mix It Up.
- Both scripts read `stream_mode` from global state — keep the mode fallback to `workspace`.
- No chat output from either script unless explicitly requested.

## OBS Dependency

Disco Party uses `stream_mode` to pick the target scene:
- `garage`    → `Disco Party: Garage`
- `workspace` → `Disco Party: Workspace`
- `gamer`     → `Disco Party: Gamer`

Scene names must match exactly what exists in OBS.

## Trigger Variables

Full trigger variable reference: `Actions/Twitch Channel Points/README.md`
