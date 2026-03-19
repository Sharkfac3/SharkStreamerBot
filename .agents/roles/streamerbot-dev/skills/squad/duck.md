# Squad — Duck

## Scripts

| Script | Path | Purpose |
|---|---|---|
| `duck-main.cs` | `Actions/Squad/Duck/` | Entry point — handles `!duck` trigger, lock, event setup |
| `duck-call.cs` | `Actions/Squad/Duck/` | Handles `!quack` calls during active duck event |
| `duck-resolve.cs` | `Actions/Squad/Duck/` | Resolves the duck event outcome |

## Key State Variables

From `Actions/SHARED-CONSTANTS.md`:
- `duck_event_active` — active event flag
- `duck_quack_count` — number of quacks received
- `duck_caller` — userId of who called the duck
- `duck_unlocked` — whether Duck is available this stream

## Detailed Docs

`Actions/Squad/Duck/README.md` — full trigger variable reference, behavioral spec.

## Notes

- Duck represents hyperfocus rabbit holes and bad ideas that seem fine
- The Duck Call Window timer gates the quack-collection window — see SHARED-CONSTANTS for timer name
- Lock: acquire in `duck-main.cs`, release in `duck-resolve.cs` on all terminal paths
