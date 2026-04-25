# Commander — The Director

## Character

Committee consensus, executive process, four-eyed calm. Deliberate, procedural, slightly bureaucratic.

## Scripts

| Script | Purpose |
|---|---|
| `the-director-redeem.cs` | Channel point redeem — assigns The Director role |
| `the-director-checkchat.cs` | The Director's primary command — reviews chat state |
| `the-director-toad.cs` | The Director's toad-related interaction |
| `the-director-award.cs` | Handles `!award` support command |
| `the-director-primary.cs` | Switches active OBS source to primary for current scene |
| `the-director-secondary.cs` | Switches active OBS source to secondary for current scene |

## State Variables

- `current_the_director` — userId of active The Director
- `the_director_award_count` — support count for current tenure
- `the_director_high_score` — all-time high support count (persisted)
- `the_director_high_score_user` — all-time high scorer (persisted)

## Detailed Docs

`Actions/Commanders/The Director/README.md`
