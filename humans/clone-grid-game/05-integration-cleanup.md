# Prompt 05 — Clone Grid Game: Integration, Cleanup & Self-Destruct

## Context

You are working on SharkStreamerBot. Read `.agents/ENTRY.md` before starting. Activate the **streamerbot-dev** role for script edits and **ops** role for change summary output.

Check `WORKING.md` before touching any files. Add your row to Active Work.

**Prerequisites:** Prompts 01–04 must be complete. All new scripts and the overlay renderer exist. This prompt finalizes integration, removes old files, and self-destructs the prompt series.

---

## What You Are Doing

1. Update `stream-start.cs` — reset new empire vars, remove old clone vars
2. Update `SHARED-CONSTANTS.md` — update the "Used in" file lists to include new scripts
3. Rewrite `Actions/Squad/Clone/README.md`
4. Delete old Clone musical-chairs scripts that are now replaced
5. Delete the `humans/clone-grid-game/` prompt folder (self-destruct)
6. Output a full operator wiring checklist

---

## Step 1 — Update `stream-start.cs`

File: `Actions/Twitch Core Integrations/stream-start.cs`

Read the file first. Find the section that resets Clone-related variables. It will contain resets for:
- `clone_game_active`
- `clone_round`
- `clone_positions_count`
- `clone_positions_open`
- `clone_position_removed_last`
- `clone_winners`
- `clone_round1_pool`
- `clone_pos_1` through `clone_pos_5`
- Timer disable: `Clone - Volley Timer`

**Remove** all of those resets.

**Add** these resets in their place (keep `clone_unlocked` reset logic intact — it is persisted and should NOT be reset at stream start):

```csharp
// Clone Grid Game — reset all non-persisted state at stream start
CPH.SetGlobalVar("empire_game_active",    false, false);
CPH.SetGlobalVar("empire_join_active",    false, false);
CPH.SetGlobalVar("empire_game_start_utc", 0L,    false);
CPH.SetGlobalVar("empire_players_json",   "[]",  false);
CPH.SetGlobalVar("empire_cells_json",     "[]",  false);

// Disable Clone timers in case stream crashed mid-game
CPH.DisableTimer("Clone - Join Window");
CPH.DisableTimer("Clone - Game Tick");
```

> `clone_unlocked` is persisted (`true` when Clone is unlocked). Do NOT reset it. It must survive stream restarts so Disco Party can include Clone in the dance.

If `TIMER_CLONE_VOLLEY` (`"Clone - Volley Timer"`) disable was in stream-start.cs, remove it — that timer no longer exists.

---

## Step 2 — Update `SHARED-CONSTANTS.md`

File: `Actions/SHARED-CONSTANTS.md`

Read the file. The Clone Grid Game section was written in Prompt 01. Update the **"Used in"** file list at the bottom of that section:

```markdown
### Used in
- `Actions/Squad/Clone/clone-empire-main.cs`
- `Actions/Squad/Clone/clone-empire-join.cs`
- `Actions/Squad/Clone/clone-empire-start.cs`
- `Actions/Squad/Clone/clone-empire-move.cs`
- `Actions/Squad/Clone/clone-empire-tick.cs`
- `Actions/Twitch Core Integrations/stream-start.cs`
```

Also update the **Mini-game Lock** section — add the new scripts to its "Used in" list and remove the old clone scripts:

Remove from "Used in":
- `Actions/Squad/Clone/clone-main.cs`
- `Actions/Squad/Clone/clone-volley.cs`

Add to "Used in":
- `Actions/Squad/Clone/clone-empire-main.cs`
- `Actions/Squad/Clone/clone-empire-move.cs`
- `Actions/Squad/Clone/clone-empire-tick.cs`

---

## Step 3 — Rewrite `Actions/Squad/Clone/README.md`

Read the existing README to see what sections exist. Replace the entire file with the following content:

```markdown
# Clone Grid Game

## Purpose
Star Wars-themed grid survival mini-game. Viewers pilot Rebel ships across a 32×18 grid, avoiding Empire-controlled territory. Surviving 5 minutes unlocks the Clone squad member.

## Commands

| Command | Who | Phase | Effect |
|---|---|---|---|
| `!clone` | Any viewer | Idle | Starts the game, opens 60-second join window |
| `!join` | Any viewer | Join window | Joins the game as a Rebel pilot |
| `!up` | Joined players | Movement phase | Move one cell up |
| `!down` | Joined players | Movement phase | Move one cell down |
| `!left` | Joined players | Movement phase | Move one cell left |
| `!right` | Joined players | Movement phase | Move one cell right |

## Game Flow

1. **Join phase (60 seconds):** After `!clone`, viewers type `!join`. The initiator is automatically the first player. All players spawn at grid center (col 16, row 9).
2. **Movement phase opens:** 5 empire ships spawn randomly. Players may now move. Inactivity timer starts (30s per player).
3. **Survival:** Players navigate the grid avoiding empire cells.
4. **Win:** Any players alive after 5 minutes win together — Clone is unlocked.
5. **Loss:** All players die before 5 minutes — no unlock.

## Grid Rules

### Death
- Move into an empire cell → instant death.
- No movement for 30 seconds → auto-death; empire spawns on that cell.

### Empire Growth (Rule 2)
When a player leaves a cell: if that cell is orthogonally adjacent to empire E1, and E1 has another orthogonal empire neighbor E2 (not the vacated cell), then the vacated cell becomes empire.

### Empire Death (Rule 3)
If an empire cell's all 4 orthogonal neighbors are occupied (by empire, players, or grid edges), that empire cell is destroyed.

## Scripts

| File | Trigger | Purpose |
|---|---|---|
| `clone-empire-main.cs` | `!clone` | Start, join window, acquire lock |
| `clone-empire-join.cs` | `!join` | Add player during join window |
| `clone-empire-start.cs` | Timer: Clone - Join Window | Spawn empire, open movement |
| `clone-empire-move.cs` | `!up/!down/!left/!right` | Move player, apply rules |
| `clone-empire-tick.cs` | Timer: Clone - Game Tick | Inactivity + win check |

## Timers

| Timer Name | Interval | Repeat | Purpose |
|---|---|---|---|
| `Clone - Join Window` | 60s | No | Fires clone-empire-start.cs |
| `Clone - Game Tick` | 5s | Yes | Fires clone-empire-tick.cs |

## Global Variables (non-persisted)

| Variable | Type | Purpose |
|---|---|---|
| `empire_game_active` | bool | True while game is running |
| `empire_join_active` | bool | True only during join window |
| `empire_game_start_utc` | long | Unix ms when movement opened |
| `empire_players_json` | string | JSON array of living players |
| `empire_cells_json` | string | JSON array of empire cells |

## Persisted Variables

| Variable | Type | Purpose |
|---|---|---|
| `clone_unlocked` | bool | Set true on win; survives stream restart; used by Disco Party |

## Mix It Up Integration

On win, triggers the Clone unlock Mix It Up command (ID configured as constant `MIXITUP_CLONE_UNLOCK_COMMAND_ID` in `clone-empire-tick.cs`). Operator must replace `REPLACE_WITH_CLONE_UNLOCK_COMMAND_ID` with the actual Mix It Up command ID after creating the command.

## OBS Integration

`Clone - Dancing` OBS source shown during unlock celebration (triggered by Mix It Up, not directly by these scripts).

## Overlay Integration

Sends broker messages via WebSocket (index 0) to the stream-overlay broker:
- `squad.clone.start` — join phase opened
- `squad.clone.update` — any game state change
- `squad.clone.end` — game over (win or loss)

## Operator Notes

- `!join` is shared with LotAT — both scripts guard themselves internally; no conflict.
- `!up/!down/!left/!right` are shared with Destroyer — mini-game lock prevents conflicts.
- Replace `REPLACE_WITH_CLONE_UNLOCK_COMMAND_ID` in `clone-empire-tick.cs` before going live.
- After stream-start.cs update, run a manual stream-start test to confirm no errors.
```

---

## Step 4 — Delete Old Clone Scripts

These scripts implement the old musical-chairs game and are fully replaced. Delete them:

- `Actions/Squad/Clone/clone-main.cs`
- `Actions/Squad/Clone/clone-position.cs`
- `Actions/Squad/Clone/clone-volley.cs`
- `Actions/Squad/Clone/overlay-publish.cs` *(old overlay-publish; new scripts publish inline)*

> Do NOT delete `README.md` — you just rewrote it.
> Do NOT delete the new `clone-empire-*.cs` files.

Verify before deleting that none of these files are referenced in WORKING.md as active.

---

## Step 5 — Self-Destruct

Delete the entire `humans/clone-grid-game/` folder and all files in it:
- `01-protocol-types.md`
- `02-scripts-game-start.md`
- `03-scripts-movement.md`
- `04-overlay-renderer.md`
- `05-integration-cleanup.md` *(this file)*

---

## Step 6 — Final Operator Wiring Checklist

Output this checklist to the user as your change summary:

---

### ✅ Clone Grid Game — Operator Wiring Checklist

#### Streamer.bot — New Actions to Create

| Action Name | Script to paste | Notes |
|---|---|---|
| `Clone - Empire - Start Game` | `clone-empire-main.cs` | |
| `Clone - Empire - Join` | `clone-empire-join.cs` | |
| `Clone - Empire - Open Movement` | `clone-empire-start.cs` | |
| `Clone - Empire - Move` | `clone-empire-move.cs` | |
| `Clone - Empire - Tick` | `clone-empire-tick.cs` | |

#### Streamer.bot — Command Triggers

| Chat Command | Add Action | Notes |
|---|---|---|
| `!clone` | `Clone - Empire - Start Game` | New command |
| `!join` | `Clone - Empire - Join` | **Add** alongside existing LotAT join action |
| `!up` | `Clone - Empire - Move` | **Add** alongside existing Destroyer move action |
| `!down` | `Clone - Empire - Move` | **Add** alongside existing Destroyer move action |
| `!left` | `Clone - Empire - Move` | **Add** alongside existing Destroyer move action |
| `!right` | `Clone - Empire - Move` | **Add** alongside existing Destroyer move action |

#### Streamer.bot — Timers to Create/Update

| Timer Name | Interval | Repeat | Fires Action |
|---|---|---|---|
| `Clone - Join Window` | 60 seconds | **No** (one-shot) | `Clone - Empire - Open Movement` |
| `Clone - Game Tick` | 5 seconds | **Yes** (repeating) | `Clone - Empire - Tick` |

Both timers should be **disabled** at creation — scripts enable them when needed.

#### Streamer.bot — Old Actions to Remove

| Old Action | Old Script | Reason |
|---|---|---|
| *(Clone start action)* | `clone-main.cs` | Replaced |
| *(Clone position action)* | `clone-position.cs` | Replaced |
| *(Clone volley action)* | `clone-volley.cs` | Replaced |
| *(Clone - Volley Timer fires action)* | — | Old timer gone |

#### Streamer.bot — Old Timer to Remove

| Timer Name | Reason |
|---|---|
| `Clone - Volley Timer` | Replaced by `Clone - Join Window` + `Clone - Game Tick` |

#### Mix It Up — One-Time Setup

1. Create (or verify) a "Clone Unlock" command in Mix It Up.
2. Copy its command ID.
3. In `clone-empire-tick.cs`, replace `REPLACE_WITH_CLONE_UNLOCK_COMMAND_ID` with the real ID.
4. Re-paste `clone-empire-tick.cs` into Streamer.bot.

#### Overlay — Build & Deploy

```bash
cd Apps/stream-overlay
pnpm typecheck   # must pass clean
pnpm build
```

Reload OBS browser source after build. Verify broker connection with test-overlay action.

#### Smoke Test Sequence

1. `!clone` in chat → join panel appears on overlay, chat confirms join window open
2. Second account types `!join` → player added, chat confirms
3. Wait 60 seconds → game starts, grid appears, empire ships visible, chat announces
4. Type `!up` / `!down` etc → player moves on grid
5. Stand still 30 seconds → auto-death fires, empire spawns
6. If solo: verify loss triggers correctly (all players dead)
7. Full 5-minute run (skip in testing: manually trigger win by setting `empire_game_start_utc` to a value 5+ minutes in the past, then wait for next tick)

---

## After Completing

1. Remove row from WORKING.md Active Work. Add to Recently Completed.
2. Produce change summary per `ops/skills/change-summary/_index.md`.
