---
id: constants-mini-games
type: constants
description: Mini-game lock state and per-game globals for Duck, Clone Empire, Pedro, and Toothless.
owner: streamerbot-dev
secondaryOwners: [app-dev]
parent: ../SHARED-CONSTANTS.md
---

# Mini-Game Constants

Canonical globals and state definitions for all Squad mini-games. The mini-game lock must be acquired before any game starts and released on all terminal paths.

## Mini-game Lock (shared)
- `VAR_MINIGAME_ACTIVE` = `minigame_active`
- `VAR_MINIGAME_NAME` = `minigame_name`

Used in:
- `Actions/Squad/Clone/clone-empire-main.cs`
- `Actions/Squad/Clone/clone-empire-move.cs`
- `Actions/Squad/Clone/clone-empire-tick.cs`
- `Actions/Squad/Duck/duck-main.cs`
- `Actions/Squad/Duck/duck-call.cs`
- `Actions/Squad/Duck/duck-resolve.cs`
- `Actions/Squad/Pedro/pedro-main.cs`
- `Actions/Squad/Toothless/toothless-main.cs`
- `Actions/Twitch Core Integrations/stream-start.cs`

---

## Duck (shared)
- `VAR_DUCK_EVENT_ACTIVE` = `duck_event_active`
- `VAR_DUCK_QUACK_COUNT` = `duck_quack_count`
- `VAR_DUCK_CALLER` = `duck_caller`
- `VAR_DUCK_UNLOCKED` = `duck_unlocked`
- `VAR_DUCK_TARGET_QUACKS` = `duck_target_quacks`
- `VAR_DUCK_UNIQUE_QUACKERS` = `duck_unique_quackers`
- `VAR_DUCK_UNIQUE_QUACKER_COUNT` = `duck_unique_quacker_count`
- `OBS_SOURCE_DUCK_DANCING` = `Duck - Dancing`
- `TIMER_DUCK_CALL_WINDOW` = `Duck - Call Window`

Used in:
- `Actions/Squad/Duck/duck-main.cs`
- `Actions/Squad/Duck/duck-call.cs`
- `Actions/Squad/Duck/duck-resolve.cs`
- `Actions/Twitch Core Integrations/stream-start.cs`

---

## Clone Grid Game (shared)

> The old Clone musical-chairs game has been replaced by the Empire Grid survival game.
> `clone_unlocked` and `OBS_SOURCE_CLONE_DANCING` are preserved for Disco Party integration.

### Preserved from old Clone
- `VAR_CLONE_UNLOCKED` = `clone_unlocked` *(persisted bool; true once Clone is unlocked)*
- `OBS_SOURCE_CLONE_DANCING` = `Clone - Dancing` *(OBS source shown during unlock celebration)*

### Grid game — global state vars (non-persisted)
- `VAR_EMPIRE_GAME_ACTIVE`    = `empire_game_active`    — true while game is running (join or movement phase)
- `VAR_EMPIRE_JOIN_ACTIVE`    = `empire_join_active`    — true only during 60-second join window
- `VAR_EMPIRE_GAME_START_UTC` = `empire_game_start_utc` — Unix ms when movement phase opened (0 = not started)
- `VAR_EMPIRE_PLAYERS_JSON`   = `empire_players_json`   — JSON array of living players (see structure below)
- `VAR_EMPIRE_CELLS_JSON`     = `empire_cells_json`     — JSON array of empire cells (see structure below)

### Grid game — timers
- `TIMER_CLONE_JOIN_WINDOW` = `Clone - Join Window`   — 60-second one-shot; fires clone-empire-start.cs
- `TIMER_CLONE_GAME_TICK`   = `Clone - Game Tick`     — 5-second repeating; fires clone-empire-tick.cs

### Grid game — constants (not stored in globals; defined as C# constants in scripts)
- `EMPIRE_GRID_COLS`          = `32`
- `EMPIRE_GRID_ROWS`          = `18`
- `EMPIRE_SPAWN_COL`          = `16`   (center column)
- `EMPIRE_SPAWN_ROW`          = `9`    (center row)
- `EMPIRE_JOIN_WINDOW_S`      = `60`   (seconds)
- `EMPIRE_WIN_DURATION_MS`    = `300000` (5 minutes in ms)
- `EMPIRE_INACTIVITY_KILL_MS` = `30000`  (30 seconds in ms)
- `EMPIRE_INITIAL_COUNT`      = `5`   (empire ships spawned at game start)
- `EMPIRE_MOVE_COOLDOWN_MS`   = `1000` (1 second per-player move cooldown)
- `MINIGAME_NAME_CLONE_EMPIRE`= `clone_empire`

### JSON structures stored in global vars

**empire_players_json** — array of living player objects:
```json
[
  { "userId": "12345", "userName": "alice", "col": 16, "row": 9, "lastMoveUtc": 1714000000000 }
]
```
`lastMoveUtc` is set to `empire_game_start_utc` when movement opens. Used for inactivity detection.

**empire_cells_json** — array of empire cell positions:
```json
[
  { "col": 5, "row": 3 },
  { "col": 12, "row": 7 }
]
```

### Used in
- `Actions/Squad/Clone/clone-empire-main.cs`
- `Actions/Squad/Clone/clone-empire-join.cs`
- `Actions/Squad/Clone/clone-empire-start.cs`
- `Actions/Squad/Clone/clone-empire-move.cs`
- `Actions/Squad/Clone/clone-empire-tick.cs`
- `Actions/Twitch Core Integrations/stream-start.cs`

---

## Pedro (shared)
- `VAR_PEDRO_GAME_ENABLED` = `pedro_game_enabled`
- `VAR_PEDRO_MENTION_COUNT` = `pedro_mention_count`
- `VAR_PEDRO_UNLOCKED` = `pedro_unlocked`
- `VAR_PEDRO_NEXT_ALLOWED_UTC` = `pedro_next_allowed_utc`
- `VAR_PEDRO_SECRET_UNLOCK_ACTIVE` = `pedro_secret_unlock_active`
- `VAR_PEDRO_LAST_MESSAGE_ID` = `pedro_last_message_id`
- `OBS_SOURCE_PEDRO_DANCING` = `Pedro - Dancing`
- `TIMER_PEDRO_CALL_WINDOW` = `Pedro - Call Window`

Used in:
- `Actions/Squad/Pedro/pedro-main.cs`
- `Actions/Squad/Pedro/pedro-call.cs`
- `Actions/Squad/Pedro/pedro-resolve.cs`
- `Actions/Twitch Core Integrations/stream-start.cs`

---

## Toothless (shared)
- `PREFIX_RARITY` = `rarity_`
- `PREFIX_BOOST` = `boost_`
- `MEMBER_TOOTHLESS` = `toothless`
- `VAR_LAST_ROLL` = `last_roll`
- `VAR_LAST_RARITY` = `last_rarity`
- `VAR_LAST_USER` = `last_user`
- `OBS_SCENE_DISCO_WORKSPACE` = `Disco Party: Workspace`

Used in:
- `Actions/Squad/Toothless/toothless-main.cs`
- `Actions/Squad/offering.cs`
- `Actions/Twitch Core Integrations/stream-start.cs`
