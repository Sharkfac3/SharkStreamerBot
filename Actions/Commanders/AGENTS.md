---
id: actions-commanders
type: domain-route
description: Streamer.bot commander role actions, support commands, state variables, paste targets, and brand handoffs.
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Commanders — Agent Guide

## Purpose

This folder owns the Streamer.bot C# actions for the commander role system: Captain Stretch, The Director, Water Wizard, their channel-point redeems, their commander-only commands, their support commands, and shared commander help behavior.

Commander work prioritizes live-stream reliability, stable chat commands, and backward-compatible role assignment. The three commander slots can all be active at the same time.

## When to Activate

Use this guide when editing or reviewing files under [Actions/Commanders/](./), including:

- [Actions/Commanders/commander-help.cs](commander-help.cs)
- [Actions/Commanders/commanders.cs](commanders.cs)
- [Actions/Commanders/Captain Stretch/](Captain%20Stretch/) scripts
- [Actions/Commanders/The Director/](The%20Director/) scripts
- [Actions/Commanders/Water Wizard/](Water%20Wizard/) scripts
- Script Reference or operator documentation in this folder

## Primary Owner

`streamerbot-dev` owns the C# runtime behavior, Streamer.bot paste readiness, global variable use, command wiring expectations, and compatibility with existing channel-point and chat-command triggers.

## Secondary Owners / Chain To

Chain to `brand-steward` for any change to commander character name, role description, or reward copy.

## Required Reading

Read [Actions/AGENTS.md](../AGENTS.md) plus the relevant commander README before editing scripts:

- [Actions/Commanders/Captain Stretch/AGENTS.md](Captain%20Stretch/AGENTS.md)
- [Actions/Commanders/The Director/AGENTS.md](The%20Director/AGENTS.md)
- [Actions/Commanders/Water Wizard/AGENTS.md](Water%20Wizard/AGENTS.md)
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) when public copy changes
- [Creative/Brand/CHARACTER-CODEX.md](../../Creative/Brand/CHARACTER-CODEX.md) when commander characterization changes

## Local Workflow

1. Identify the commander and trigger type: channel-point redeem, commander-only command, support command, or shared help.
2. Preserve the commander slot model: Captain Stretch, The Director, and Water Wizard are independent slots and may all be active simultaneously.
3. Preserve chat-facing command names unless the operator explicitly requests a rename.
4. Update the Script Reference section in this file when trigger variables, command behavior, state variables, or operator wiring changes.

Commander script map:

| Commander | Scripts |
|---|---|
| Captain Stretch | [captain-stretch-redeem.cs](Captain%20Stretch/captain-stretch-redeem.cs), [captain-stretch-stretch.cs](Captain%20Stretch/captain-stretch-stretch.cs), [captain-stretch-shrimp.cs](Captain%20Stretch/captain-stretch-shrimp.cs), [captain-stretch-thank.cs](Captain%20Stretch/captain-stretch-thank.cs), [captain-stretch-generalfocus.cs](Captain%20Stretch/captain-stretch-generalfocus.cs) |
| The Director | [the-director-redeem.cs](The%20Director/the-director-redeem.cs), [the-director-checkchat.cs](The%20Director/the-director-checkchat.cs), [the-director-toad.cs](The%20Director/the-director-toad.cs), [the-director-award.cs](The%20Director/the-director-award.cs), [the-director-primary.cs](The%20Director/the-director-primary.cs), [the-director-secondary.cs](The%20Director/the-director-secondary.cs) |
| Water Wizard | [water-wizard-redeem.cs](Water%20Wizard/water-wizard-redeem.cs), [water-wizard-hail.cs](Water%20Wizard/water-wizard-hail.cs), [wizard-hydrate.cs](Water%20Wizard/wizard-hydrate.cs), [water-wizard-orb.cs](Water%20Wizard/water-wizard-orb.cs), [water-wizard-castrest.cs](Water%20Wizard/water-wizard-castrest.cs) |
| Shared helpers | [commander-help.cs](commander-help.cs), [commanders.cs](commanders.cs) |

Support command rules:

| Command | Commander | Counter variable |
|---|---|---|
| `!thank` | Captain Stretch | `captain_stretch_thank_count` |
| `!award` | The Director | `the_director_award_count` |
| `!hail` | Water Wizard | `water_wizard_hail_count` |

Active commanders cannot support themselves with their own support command. On commander redeem, compare the outgoing tenure counter to the persistent high score for that role.

## SHARED-STATE

Current commander slot globals:

| Role | Active slot global |
|---|---|
| Captain Stretch | `current_captain_stretch` |
| The Director | `current_the_director` |
| Water Wizard | `current_water_wizard` |

Support tenure counters and persistent high-score keys:

| Role | Tenure counter | High score | High-score holder |
|---|---|---|---|
| Captain Stretch | `captain_stretch_thank_count` | `captain_stretch_thank_high_score` | `captain_stretch_thank_high_score_user` |
| The Director | `the_director_award_count` | `the_director_award_high_score` | `the_director_award_high_score_user` |
| Water Wizard | `water_wizard_hail_count` | `water_wizard_hail_high_score` | `water_wizard_hail_high_score_user` |

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "commanders.cs",
      "action": "Commanders - Active Commanders",
      "purpose": "Responds to the !commanders chat command with the currently active Captain Stretch, The Director, and Water Wizard slot holders.",
      "triggers": [
        "Twitch -> Chat Command -> !commanders"
      ],
      "globals": [
        "current_captain_stretch",
        "current_the_director",
        "current_water_wizard"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [
        "Captain Stretch",
        "The Director",
        "Water Wizard",
        "!commanderhelp"
      ],
      "runtimeBehavior": [
        "Reads all commander slot globals defensively.",
        "Sends active slot holders with Twitch @mentions.",
        "Sends open-deck fallback when all slots are blank.",
        "Does not create, mutate, or persist globals."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !commanders"
    },
    {
      "script": "commander-help.cs",
      "action": "Commanders - Help",
      "purpose": "Displays commander help/routing information for chat.",
      "triggers": [
        "Twitch -> Chat Command -> !commanders"
      ],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Sends compact commander help text to chat."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for commander help"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->

## Validation

See Actions/AGENTS.md for universal validation, boundary, and handoff rules.

## Boundaries / Out of Scope

See Actions/AGENTS.md for universal validation, boundary, and handoff rules.

## Handoff Notes

See Actions/AGENTS.md for universal validation, boundary, and handoff rules.

---

## Script Reference

### Model Rules
- Three commander slots exist:
  - Captain Stretch
  - The Director
  - Water Wizard
- All three commander slots can be active simultaneously.
- Redeem behavior should remain backward-compatible unless intentionally changed.

### Support Command Rules
- Chat can support active commanders with:
  - `!hail` (Water Wizard)
  - `!thank` (Captain Stretch)
  - `!award` (The Director)
- Active commander cannot support themselves with their own support command.
- Each support command increments a per-tenure counter.
- On commander redeem, outgoing tenure counter is compared to persistent high score for that role.

### Commander-Only Command Rules
- Water Wizard can run `!hydrate`, `!orb`, and `!castrest` when the relevant feature window is active.
- Captain Stretch can run `!stretch`, `!shrimp`, and `!generalfocus` when the relevant feature window is active.
- The Director can run `!checkchat`, `!toad`, `!primary`, and `!secondary`.
- Unauthorized callers should get short guidance that points them back to the active commander support command.
- New loop-control commands must preserve the existing commander assignment model.

### Commander Docs
- `Captain Stretch/AGENTS.md`
- `The Director/AGENTS.md`
- `Water Wizard/AGENTS.md`

---

### Script: `commander-help.cs`

#### Purpose
Gives the caller a short, commander-specific help message in chat.

#### Expected Trigger / Input
- Chat command or action trigger for a commander help command (operator chooses the exact command name, such as `!commanderhelp`).
- Reads `user`.

#### Required Runtime Variables
- Reads `current_captain_stretch`.
- Reads `current_the_director`.
- Reads `current_water_wizard`.

#### Key Outputs / Side Effects
- If caller is the active Captain Stretch, explains `!stretch` and `!shrimp`.
- If caller is the active The Director, explains `!checkchat`, `!toad`, `!primary`, and `!secondary`.
- If caller is the active Water Wizard, explains `!hydrate` and `!orb`.
- If caller holds multiple commander roles, sends one short help message for each matching role.
- If caller is not an active commander, sends a short guidance message telling them to redeem first.

#### Mix It Up Actions
- None.

#### OBS Interactions
- None directly.

#### Wait Behavior
- None.

#### Chat / Log Output
- Sends short role-specific command summaries in chat.
- Sends a short fallback guidance message for non-commanders.

#### Operator Notes
- Wire this script to the chat command name you want to use.
- This script is read-only: it does not create or change any global variables.

---

### Script: `commanders.cs`

#### Purpose
Tells chat who currently holds each commander slot.

#### Expected Trigger / Input
- Chat command trigger: `!commanders`.
- Does not require caller input.

#### Required Runtime Variables
- Reads `current_captain_stretch`.
- Reads `current_the_director`.
- Reads `current_water_wizard`.

#### Key Outputs / Side Effects
- If any commander slots are active, sends one chat message listing active slot holders with `@` before each username for Twitch mention notifications.
- If no commander slots are active, sends a short open-deck fallback message.
- Does not create or change any global variables.

#### Mix It Up Actions
- None.

#### OBS Interactions
- None directly.

#### Wait Behavior
- None.

#### Chat / Log Output
- Sends one short active-roster or fallback message in chat.

#### Operator Notes
- Wire this script to the `!commanders` chat command.
- This script is read-only: it does not create or change any global variables.

---

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

Support commands are triggered via Twitch → Chat → Message or a Command trigger.

| Variable | Type | Notes |
|---|---|---|
| `user` | string | Display name of the user running the command |
| `userId` | string | Twitch user ID |
| `message` | string | Full chat message |
| `rawInput` | string | Fallback if `message` is empty |
| `msgId` | string | Unique message ID — use for duplicate detection |
