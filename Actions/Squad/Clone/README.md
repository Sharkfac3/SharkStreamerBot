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
