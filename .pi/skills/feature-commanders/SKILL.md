---
name: feature-commanders
description: Commander role assignment and commander-specific chat commands. Covers Captain Stretch, The Director, and Water Wizard. Load when working on any script under Actions/Commanders/.
---

# Feature: Commanders

## Scope

Commander scripts handle role assignment via Twitch redeems and commander-specific chat commands.

## Model Rules

- Three commander slots exist: **Captain Stretch**, **The Director**, **Water Wizard**.
- All three can be active simultaneously.
- A Twitch user redeems into a commander role.
- Each commander has its own folder with redeem + command scripts.
- Existing redeem/assignment behavior must be preserved unless a change is explicitly requested.

## Action Folders

| Commander | Path | Scripts |
|---|---|---|
| Captain Stretch | `Actions/Commanders/Captain Stretch/` | `captain-stretch-redeem.cs`, `captain-stretch-stretch.cs`, `captain-stretch-shrimp.cs`, `captain-stretch-thank.cs` |
| The Director | `Actions/Commanders/The Director/` | `the-director-redeem.cs`, `the-director-checkchat.cs`, `the-director-toad.cs`, `the-director-award.cs` |
| Water Wizard | `Actions/Commanders/Water Wizard/` | `water-wizard-redeem.cs`, `water-wizard-hail.cs`, `wizard-hydrate.cs`, `water-wizard-orb.cs` |

## Detailed Docs

Each commander has its own README:

- `Actions/Commanders/Captain Stretch/README.md`
- `Actions/Commanders/The Director/README.md`
- `Actions/Commanders/Water Wizard/README.md`
- `Actions/Commanders/README.md` (model overview)

**Read the relevant commander README before making changes.**

## Support Commands

Chat can support active commanders:

| Command | Commander | Variable |
|---|---|---|
| `!thank` | Captain Stretch | `captain_stretch_thank_count` |
| `!award` | The Director | `the_director_award_count` |
| `!hail` | Water Wizard | `water_wizard_hail_count` |

Rules:
- Active commander **cannot** support themselves with their own support command.
- Each support command increments a per-tenure counter.
- On new commander redeem, outgoing tenure counter is compared to the persistent high score for that role.

## Shared Constants

Commander globals are documented in `Actions/SHARED-CONSTANTS.md`. Key variables:

- `current_captain_stretch`, `current_the_director`, `current_water_wizard` — active user per slot.
- `*_thank_count`, `*_award_count`, `*_hail_count` — per-tenure counters (non-persisted).
- `*_high_score`, `*_high_score_user` — all-time records (persisted).

## Behavioral Expectations

- Handle spam/rapid triggers gracefully.
- Protect against invalid or missing inputs.
- Preserve backward compatibility for existing chat commands.
- Do not rename triggers/commands that chat depends on unless explicitly requested.

## Trigger Variables

Access in C# via `CPH.TryGetArg("variableName", out T value)`.

### Channel Reward Redemption (commander role redeems)

Commander role assignment is triggered via Twitch → Channel Reward → Reward Redemption.

| Variable | Type | Notes |
|---|---|---|
| `user` | string | Display name of the redeeming user — becomes the new commander |
| `userId` | string | Twitch user ID |
| `rewardName` | string | Name of the channel point reward |
| `rewardId` | string | Unique reward identifier |
| `rawInput` | string | Optional user text input (if the reward prompts for it) |

### Chat Message (support commands: !thank, !award, !hail)

Commander support commands are triggered via Twitch → Chat → Message or a Command trigger.

| Variable | Type | Notes |
|---|---|---|
| `user` | string | Display name of the user running the command |
| `userId` | string | Twitch user ID |
| `message` | string | Full chat message |
| `rawInput` | string | Fallback if `message` is empty |
| `msgId` | string | Unique message ID — use for duplicate detection |
