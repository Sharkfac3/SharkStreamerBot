---
id: constants-commanders
type: constants
description: Commander slot globals, tenure counters, and high-score tracking constants.
owner: streamerbot-dev
secondaryOwners: [brand-steward]
parent: ../SHARED-CONSTANTS.md
---

# Commander Constants

Canonical globals for the 3-slot commander system. These names are the source of truth for all commander scripts.

## Commanders (shared)
- `ARG_USER` = `user`
- `VAR_CURRENT_CAPTAIN_STRETCH` = `current_captain_stretch`
- `VAR_CURRENT_THE_DIRECTOR` = `current_the_director`
- `VAR_CURRENT_WATER_WIZARD` = `current_water_wizard`
- `VAR_CAPTAIN_STRETCH_THANK_COUNT` = `captain_stretch_thank_count`
- `VAR_CAPTAIN_STRETCH_STRETCH_NEXT_ALLOWED_UTC` = `captain_stretch_stretch_next_allowed_utc`
- `VAR_CAPTAIN_STRETCH_SHRIMP_NEXT_ALLOWED_UTC` = `captain_stretch_shrimp_next_allowed_utc`
- `VAR_CAPTAIN_STRETCH_THANK_HIGH_SCORE` = `captain_stretch_thank_high_score` *(persisted)*
- `VAR_CAPTAIN_STRETCH_THANK_HIGH_SCORE_USER` = `captain_stretch_thank_high_score_user` *(persisted)*
- `VAR_THE_DIRECTOR_AWARD_COUNT` = `the_director_award_count`
- `VAR_THE_DIRECTOR_CHECKCHAT_NEXT_ALLOWED_UTC` = `the_director_checkchat_next_allowed_utc`
- `VAR_THE_DIRECTOR_TOAD_NEXT_ALLOWED_UTC` = `the_director_toad_next_allowed_utc`
- `VAR_THE_DIRECTOR_AWARD_HIGH_SCORE` = `the_director_award_high_score` *(persisted)*
- `VAR_THE_DIRECTOR_AWARD_HIGH_SCORE_USER` = `the_director_award_high_score_user` *(persisted)*
- `VAR_WATER_WIZARD_HAIL_COUNT` = `water_wizard_hail_count`
- `VAR_WATER_WIZARD_HYDRATE_NEXT_ALLOWED_UTC` = `water_wizard_hydrate_next_allowed_utc`
- `VAR_WATER_WIZARD_ORB_NEXT_ALLOWED_UTC` = `water_wizard_orb_next_allowed_utc`
- `VAR_WATER_WIZARD_HAIL_HIGH_SCORE` = `water_wizard_hail_high_score` *(persisted)*
- `VAR_WATER_WIZARD_HAIL_HIGH_SCORE_USER` = `water_wizard_hail_high_score_user` *(persisted)*

Used in:
- `Actions/Commanders/commander-help.cs`
- `Actions/Commanders/commanders.cs`
- `Actions/Commanders/Captain Stretch/captain-stretch-redeem.cs`
- `Actions/Commanders/Captain Stretch/captain-stretch-thank.cs`
- `Actions/Commanders/Captain Stretch/captain-stretch-stretch.cs`
- `Actions/Commanders/Captain Stretch/captain-stretch-shrimp.cs`
- `Actions/Commanders/Captain Stretch/captain-stretch-generalfocus.cs`
- `Actions/Commanders/The Director/the-director-redeem.cs`
- `Actions/Commanders/The Director/the-director-award.cs`
- `Actions/Commanders/The Director/the-director-checkchat.cs`
- `Actions/Commanders/The Director/the-director-toad.cs`
- `Actions/Commanders/The Director/the-director-primary.cs`
- `Actions/Commanders/The Director/the-director-secondary.cs`

### The Director — Scene Source Map

`the-director-primary.cs` and `the-director-secondary.cs` each contain a `SCENE_SOURCE_MAP` dictionary
that maps OBS scene names to a `(primarySource, secondarySource)` pair.

When The Director uses `!primary` or `!secondary`, the script reads the current OBS scene,
looks it up in the map, then shows one source and hides the other.

**Before adding any new scene source switching logic**, check this map first.
**When adding a new scene entry**, update `SCENE_SOURCE_MAP` in **both** scripts and add the
source names to the table below.

| OBS Scene | Primary Source | Secondary Source |
|---|---|---|
| `Workspace: Main` | `Main Screen Capture` | `Quest POV` |
| *(others TBD — add rows here as sources are confirmed)* | | |
- `Actions/Commanders/Water Wizard/water-wizard-redeem.cs`
- `Actions/Commanders/Water Wizard/water-wizard-hail.cs`
- `Actions/Commanders/Water Wizard/wizard-hydrate.cs`
- `Actions/Commanders/Water Wizard/water-wizard-orb.cs`
- `Actions/Commanders/Water Wizard/water-wizard-castrest.cs`
