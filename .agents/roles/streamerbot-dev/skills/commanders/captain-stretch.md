# Commander — Captain Stretch

## Character

Executive function, authority, shrimp denial. The commander trying to maintain order while things slip.

## Scripts

| Script | Purpose |
|---|---|
| `captain-stretch-redeem.cs` | Channel point redeem — assigns Captain Stretch role |
| `captain-stretch-stretch.cs` | Captain Stretch's primary command |
| `captain-stretch-shrimp.cs` | Shrimp-related interaction (character's defining denial mechanic) |
| `captain-stretch-thank.cs` | Handles `!thank` support command |

## State Variables

- `current_captain_stretch` — userId of active Captain Stretch
- `captain_stretch_thank_count` — support count for current tenure
- `captain_stretch_high_score` — all-time high support count (persisted)
- `captain_stretch_high_score_user` — all-time high scorer (persisted)

## Detailed Docs

`Actions/Commanders/Captain Stretch/README.md`
