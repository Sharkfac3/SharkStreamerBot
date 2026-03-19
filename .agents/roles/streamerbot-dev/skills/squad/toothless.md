# Squad — Toothless

## Scripts

| Script | Path | Purpose |
|---|---|---|
| `toothless-main.cs` | `Actions/Squad/Toothless/` | Handles rarity roll, result output, state tracking |

## Key State Variables

From `Actions/SHARED-CONSTANTS.md`:
- `rarity_*` — rarity tier tracking variables
- `last_roll` — result of the most recent roll
- `last_rarity` — rarity tier of the most recent roll
- `last_user` — userId of the last roller

## Behavioral Notes

- Toothless represents threat response, security, and disproportionate reaction
- Rarity system determines outcome tier — consult SHARED-CONSTANTS for tier names and OBS source names
- OBS sources cycle on roll — hide/show based on rarity result
- Stream-start resets rarity state

## Detailed Docs

`Actions/Squad/Toothless/README.md` — full trigger variable reference, behavioral spec.
