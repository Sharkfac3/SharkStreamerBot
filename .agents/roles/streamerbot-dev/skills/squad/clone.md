# Squad — Clone

## Scripts

| Script | Path | Purpose |
|---|---|---|
| `clone-empire-main.cs` | `Actions/Squad/Clone/` | Entry point — handles `!clone` trigger, lock acquisition, join-phase setup |
| `clone-empire-join.cs` | `Actions/Squad/Clone/` | Registers a player during the 60-second join window |
| `clone-empire-start.cs` | `Actions/Squad/Clone/` | Fired by join-window timer; spawns empire cells and opens movement phase |
| `clone-empire-move.cs` | `Actions/Squad/Clone/` | Processes player movement commands (`!up`/`!down`/`!left`/`!right`) |
| `clone-empire-tick.cs` | `Actions/Squad/Clone/` | 5-second repeating tick — grows empire, kills inactive players, checks win/loss |

## Key State Variables

From `Actions/SHARED-CONSTANTS.md`:
- `clone_unlocked` — whether Clone is unlocked (persisted; preserved from old game)
- `empire_game_active` — true while the Empire Grid game is running
- `empire_join_active` — true only during the 60-second join window
- `empire_game_start_utc` — Unix ms when movement phase opened (0 = not started)
- `empire_players_json` — JSON array of living players (userId, userName, col, row, lastMoveUtc)
- `empire_cells_json` — JSON array of empire cell positions (col, row)

## Detailed Docs

`Actions/Squad/Clone/README.md` — full trigger variable reference, behavioral spec.

## Notes

- Clone is the "wildcard" character — random intrusive thoughts, unexplained presence
- Lock pattern: acquire in `clone-empire-main.cs`, release in `clone-empire-tick.cs` on all terminal paths (win/loss/fault)
- Timers: `Clone - Join Window` (one-shot 60s → fires `clone-empire-start.cs`), `Clone - Game Tick` (5s repeating → fires `clone-empire-tick.cs`)
