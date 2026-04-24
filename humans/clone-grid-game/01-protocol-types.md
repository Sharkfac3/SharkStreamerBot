# Prompt 01 — Clone Grid Game: Protocol Types + Shared Constants

## Context

You are working on SharkStreamerBot — a Twitch stream automation project. Read `.agents/ENTRY.md` before starting. Activate the **app-dev** role (`roles/app-dev/`). Read `roles/app-dev/skills/core.md`.

Check `WORKING.md` before touching any files. Add your row to Active Work.

---

## What You Are Building

The Clone squad mini-game is being fully rebuilt as a **Star Wars-themed grid survival game**. This is Prompt 01 of 5 in a series. Your job is to:

1. Add new TypeScript protocol types for the new game to `@stream-overlay/shared`
2. Document new SHARED-CONSTANTS entries in `Actions/SHARED-CONSTANTS.md`

Later prompts will build Streamer.bot scripts (02, 03), the overlay renderer (04), and do final integration + cleanup (05). Do not touch Streamer.bot scripts or the overlay renderer in this prompt.

---

## Game Summary (read carefully — everything downstream depends on this)

**Trigger:** `!clone` starts the game. Other users type `!join` within 60 seconds. After the join window closes, 5 empire ships spawn randomly on a 32×18 grid. All players spawn at the center cell (col 16, row 9). Players use `!up` `!down` `!left` `!right` to move. If they survive 5 minutes, all surviving players win and Clone is unlocked. If all players die, the game ends in loss.

**Death rules:**
- Move into an empire cell → instant death
- No movement for 30 seconds → auto-die; empire spawns on that cell

**Empire growth rule:** When a player leaves a cell, if that cell is orthogonally adjacent to an empire cell E1, AND E1 is orthogonally adjacent to another empire cell E2 (not the cell just vacated), then the vacated cell becomes empire.

**Empire death rule:** If an empire cell has all 4 orthogonal neighbors occupied (by other empire cells, player cells, or grid edges), that empire cell is destroyed.

**Win:** Any player still alive after 5 minutes → all survivors win together. Clone unlock triggered.

**Grid:**
- 32 columns × 18 rows
- Cell size: ~52px (exact rendering handled by overlay)
- Center spawn: col 16, row 9 (1-indexed)
- Players may share cells; no collision between players

---

## Step 1 — Update `@stream-overlay/shared` Protocol Types

File: `Apps/stream-overlay/packages/shared/src/protocol.ts`

Read the file first. Find the existing Clone-related types near the bottom (`SquadCloneStartPayload`, `SquadCloneUpdateState`, `SquadCloneEndResult`). **Replace them entirely** with the types below.

Also update `TopicPayloadMap` — the clone entries currently map to `SquadGameUpdatePayload`; update them to use the new typed interfaces.

### Types to add/replace

```typescript
// ── Clone Grid Game (replacement for old Clone musical-chairs types) ──────────

/** Player entry in the grid game */
export interface CloneGridPlayer {
  userId: string;
  userName: string;
  /** Grid column, 1-indexed, range 1–32 */
  col: number;
  /** Grid row, 1-indexed, range 1–18 */
  row: number;
}

/** Empire cell position */
export interface CloneGridCell {
  col: number;
  row: number;
}

/**
 * squad.clone.start
 * Sent when the join window opens (immediately after !clone).
 * Empire array is empty at this point — empire spawns after join window closes.
 */
export interface SquadCloneGridStartPayload extends SquadGameStartPayload {
  game: "clone";
  /** Always "join" on the start event — join window is opening */
  phase: "join";
  /** Seconds the join window stays open (60) */
  joinWindowSeconds: number;
  /** Players already in the game (just the initiator at start) */
  players: CloneGridPlayer[];
  /** Grid dimensions */
  gridCols: number;
  gridRows: number;
}

/**
 * squad.clone.update — state field shape
 * Sent after every meaningful state change during the game.
 */
export interface SquadCloneGridUpdateState {
  /**
   * What triggered this update:
   * - "game_start"           join window closed, empire spawned, movement open
   * - "player_joined"        a new player joined during join phase
   * - "player_moved"         a player moved to a new cell
   * - "player_died"          a player moved into empire
   * - "player_inactivity"    a player was killed for not moving (30s)
   * - "empire_spawned"       empire growth rule created a new empire cell
   * - "empire_killed"        empire surrounded on all sides, removed
   */
  event:
    | "game_start"
    | "player_joined"
    | "player_moved"
    | "player_died"
    | "player_inactivity"
    | "empire_spawned"
    | "empire_killed";
  /** Full current player list (only living players) */
  players: CloneGridPlayer[];
  /** Full current empire cell list */
  empire: CloneGridCell[];
  gridCols: number;
  gridRows: number;
  /** Seconds elapsed since movement phase opened */
  elapsedSeconds: number;
  survivorCount: number;
  empireCount: number;
  /**
   * Human-readable detail about the event — e.g. username for player events,
   * "col,row" for empire events. Optional.
   */
  eventDetail?: string;
}

/**
 * squad.clone.end — result field shape
 */
export interface SquadCloneGridEndResult {
  outcome: "win" | "loss";
  /** Survivors still alive when the game ended (empty on loss) */
  survivors: CloneGridPlayer[];
}

// Keep old names as type aliases so existing squad-renderer import lines don't break.
// The overlay renderer (clone-renderer.ts) will be fully rewritten in Prompt 04
// and will switch to the Grid types directly.
/** @deprecated Use SquadCloneGridStartPayload */
export type SquadCloneStartPayload = SquadCloneGridStartPayload;
/** @deprecated Use SquadCloneGridUpdateState */
export type SquadCloneUpdateState = SquadCloneGridUpdateState;
/** @deprecated Use SquadCloneGridEndResult */
export type SquadCloneEndResult = SquadCloneGridEndResult;
```

### TopicPayloadMap updates

In `TopicPayloadMap`, update the clone entries:

```typescript
// Squad — Clone (grid game)
"squad.clone.start":  SquadCloneGridStartPayload;
"squad.clone.update": SquadGameUpdatePayload;   // generic wrapper; state cast to SquadCloneGridUpdateState in renderer
"squad.clone.end":    SquadGameEndPayload;       // generic wrapper; result cast to SquadCloneGridEndResult in renderer
```

After editing, run `pnpm typecheck` from `Apps/stream-overlay/` to confirm no type errors.

---

## Step 2 — Document New SHARED-CONSTANTS Entries

File: `Actions/SHARED-CONSTANTS.md`

Read the file first. Find the existing `## Clone (shared)` section. **Replace it entirely** with the block below.

```markdown
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
```

---

## Step 3 — Verify topics.ts

File: `Apps/stream-overlay/packages/shared/src/topics.ts`

Read it. Confirm `squad.clone.*` topic strings exist. No changes needed unless they are missing — in that case add them following the existing pattern.

---

## Wiring Required After This Prompt

None. This prompt is types + docs only. No Streamer.bot actions or overlay code changes yet.

## After Completing

1. Run `pnpm typecheck` from `Apps/stream-overlay/` — must pass clean.
2. Remove your row from WORKING.md Active Work. Add to Recently Completed.
3. Load `ops/skills/change-summary/_index.md` and produce a paste-target summary for the operator.
