# Commander — Water Wizard

## Character

Calm wisdom, hydration solutions, festival brain. The grounding presence — always has a water-based answer.

## Scripts

| Script | Purpose |
|---|---|
| `water-wizard-redeem.cs` | Channel point redeem — assigns Water Wizard role |
| `water-wizard-hail.cs` | Handles `!hail` support command |
| `wizard-hydrate.cs` | Water Wizard's primary hydration command |
| `water-wizard-orb.cs` | The orb interaction |
| `water-wizard-castrest.cs` | Cast rest — initiates a rest/focus loop phase |

## State Variables

- `current_water_wizard` — userId of active Water Wizard
- `water_wizard_hail_count` — support count for current tenure
- `water_wizard_high_score` — all-time high support count (persisted)
- `water_wizard_high_score_user` — all-time high scorer (persisted)

## Detailed Docs

`Actions/Commanders/Water Wizard/README.md`
