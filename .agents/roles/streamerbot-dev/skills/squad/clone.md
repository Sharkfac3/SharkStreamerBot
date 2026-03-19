# Squad — Clone

## Scripts

| Script | Path | Purpose |
|---|---|---|
| `clone-main.cs` | `Actions/Squad/Clone/` | Entry point — handles chat trigger, lock acquisition, round setup |
| `clone-position.cs` | `Actions/Squad/Clone/` | Processes position selections during active round |
| `clone-volley.cs` | `Actions/Squad/Clone/` | Resolves the volley outcome |

## Key State Variables

From `Actions/SHARED-CONSTANTS.md`:
- `clone_unlocked` — whether Clone mini-game is available this stream
- `clone_game_active` — active game flag (part of mini-game lock pattern)
- `clone_round` — current round number
- `clone_positions_*` — player position tracking
- `clone_winners` — winner tracking across rounds

## Detailed Docs

`Actions/Squad/Clone/README.md` — full trigger variable reference, behavioral spec.

## Notes

- Clone is the "wildcard" character — random intrusive thoughts, unexplained presence
- Lock pattern: acquire in `clone-main.cs`, release in `clone-volley.cs` on all terminal paths
