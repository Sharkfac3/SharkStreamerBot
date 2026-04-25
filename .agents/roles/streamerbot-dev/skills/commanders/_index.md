# Commanders — Feature Overview

## Model

- Three commander slots: **Captain Stretch**, **The Director**, **Water Wizard**
- All three can be active simultaneously
- A Twitch user redeems into a commander role via channel point reward
- Each commander has its own folder with redeem + command scripts

## Action Folders

| Commander | Path | Scripts |
|---|---|---|
| Captain Stretch | `Actions/Commanders/Captain Stretch/` | `captain-stretch-redeem.cs`, `captain-stretch-stretch.cs`, `captain-stretch-shrimp.cs`, `captain-stretch-thank.cs`, `captain-stretch-generalfocus.cs` |
| The Director | `Actions/Commanders/The Director/` | `the-director-redeem.cs`, `the-director-checkchat.cs`, `the-director-toad.cs`, `the-director-award.cs`, `the-director-primary.cs`, `the-director-secondary.cs` |
| Water Wizard | `Actions/Commanders/Water Wizard/` | `water-wizard-redeem.cs`, `water-wizard-hail.cs`, `wizard-hydrate.cs`, `water-wizard-orb.cs`, `water-wizard-castrest.cs` |

## Detailed Docs

Read the relevant README before making changes:
- `Actions/Commanders/Captain Stretch/README.md`
- `Actions/Commanders/The Director/README.md`
- `Actions/Commanders/Water Wizard/README.md`
- `Actions/Commanders/README.md` (model overview, trigger variables)

## Support Commands

Chat can support active commanders:

| Command | Commander | Counter Variable |
|---|---|---|
| `!thank` | Captain Stretch | `captain_stretch_thank_count` |
| `!award` | The Director | `the_director_award_count` |
| `!hail` | Water Wizard | `water_wizard_hail_count` |

Rules:
- Active commander **cannot** support themselves with their own support command
- Each support command increments a per-tenure counter
- On new commander redeem, outgoing tenure counter is compared to the persistent high score for that role

## Shared Constants (key variables)

From `Actions/SHARED-CONSTANTS.md`:
- `current_captain_stretch`, `current_the_director`, `current_water_wizard` — active user per slot
- `*_thank_count`, `*_award_count`, `*_hail_count` — per-tenure counters (non-persisted)
- `*_high_score`, `*_high_score_user` — all-time records (persisted)

## Behavioral Expectations

- Handle spam/rapid triggers gracefully
- Protect against invalid or missing inputs
- Preserve backward compatibility for existing chat commands
- Do not rename triggers/commands that chat depends on unless explicitly requested
- Existing redeem/assignment behavior must be preserved unless a change is explicitly requested

## Sub-Skills

Load the specific commander file when your task is scoped to that commander:
- `captain-stretch.md`
- `the-director.md`
- `water-wizard.md`
