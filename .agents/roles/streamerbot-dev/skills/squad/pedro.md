# Squad — Pedro

## Scripts

| Script | Path | Purpose |
|---|---|---|
| `pedro-main.cs` | `Actions/Squad/Pedro/` | Handles Pedro mention counting, secret unlock, cooldown/lock behavior |
| `pedro-call.cs` | `Actions/Squad/Pedro/` | Handles chat trigger during active Pedro event |
| `pedro-resolve.cs` | `Actions/Squad/Pedro/` | Resolves the Pedro event outcome |

## Key State Variables

From `Actions/SHARED-CONSTANTS.md`:
- `pedro_game_enabled` — whether Pedro interactions are active
- `pedro_mention_count` — running mention count toward unlock threshold
- `pedro_unlocked` — whether Pedro's secret has been triggered this stream

## Behavioral Notes

- Pedro is the "well-meaning escalation / engineering chaos" character
- Secret unlock: fires when chat mentions Pedro a threshold number of times
- Unlock can trigger Mix It Up multiple times per stream (by design — not a bug)
- Overlapping Pedro runs are prevented via a lock guard
- Invalid `!pedro` text triggers cryptic feedback (intentional)
- Unlock wait: **31 seconds** after triggering Mix It Up (shared unlock wait pattern)
- Pedro resolve wait applied after unlock sequence

## Detailed Docs

`Actions/Squad/Pedro/README.md` — full trigger variable reference, behavioral spec.
