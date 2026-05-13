---
id: constants-stream-core
type: constants
description: OBS scene names, stream mode globals, and Disco Party state constants.
owner: streamerbot-dev
parent: ../SHARED-CONSTANTS.md
---

# Stream Core Constants

Canonical OBS scene names, stream mode globals, and Disco Party state for use across all Actions scripts. Names here are the source of truth — do not hardcode these strings in scripts.

## OBS
- `OBS_SCENE_DISCO_GARAGE` = `Disco Party: Garage`
- `OBS_SCENE_DISCO_WORKSPACE` = `Disco Party: Workspace`
- `OBS_SCENE_DISCO_GAMER` = `Disco Party: Gamer`

Used in:
- `Actions/Twitch Core Integrations/stream-start.cs`
- `Actions/Twitch Channel Points/disco-party.cs`

---

## Stream Mode (shared)
- `VAR_STREAM_MODE` = `stream_mode`
- `MODE_GARAGE` = `garage`
- `MODE_WORKSPACE` = `workspace`
- `MODE_GAMER` = `gamer`

Used in:
- `Actions/Twitch Core Integrations/stream-start.cs`
- `Actions/Voice Commands/mode-garage.cs`
- `Actions/Voice Commands/mode-workspace.cs`
- `Actions/Voice Commands/mode-gamer.cs`
- `Actions/Twitch Channel Points/disco-party.cs`

---

## Disco Party (shared)
- `VAR_DISCO_PARTY_ACTIVE` = `disco_party_active` — re-entry guard; true while the 60s dance sequence is running
- `VAR_DISCO_PARTY_PREV_SCENE` = `disco_party_prev_scene` — OBS scene name saved before switching to Disco; used to return afterward

Used in:
- `Actions/Twitch Channel Points/disco-party.cs`
- `Actions/Twitch Core Integrations/stream-start.cs`

Operator notes:
- Both vars are non-persisted. `stream-start.cs` resets them to `false` / `""` at stream start.
- Dance command IDs (one per squad member/Toothless rarity) live as constants inside `disco-party.cs`.
  Replace all `REPLACE_WITH_*` placeholders after creating "Squad - \<Member\> - Dance" commands in Mix It Up.
