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
- README or operator documentation in this folder

Activate the `brand-steward` role before changing public commander copy, character voice, command names presented to chat, or lore/character positioning.

## Primary Owner

`streamerbot-dev` owns the C# runtime behavior, Streamer.bot paste readiness, global variable use, command wiring expectations, and compatibility with existing channel-point and chat-command triggers.

## Secondary Owners / Chain To

- `brand-steward` — chain for public chat text, commander character voice, reward descriptions, naming, or any change to Captain Stretch, The Director, or Water Wizard identity.
- `ops` — chain only for validation, paste/sync workflow, or agent-tree maintenance beyond this local guide.

## Required Reading

Read the local README first, then the relevant commander README before editing scripts:

- [Actions/Commanders/README.md](README.md)
- [Actions/Commanders/Captain Stretch/README.md](Captain%20Stretch/README.md)
- [Actions/Commanders/The Director/README.md](The%20Director/README.md)
- [Actions/Commanders/Water Wizard/README.md](Water%20Wizard/README.md)
- [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md)
- [Actions/HELPER-SNIPPETS.md](../HELPER-SNIPPETS.md)
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) when public copy changes
- [Creative/Brand/CHARACTER-CODEX.md](../../Creative/Brand/CHARACTER-CODEX.md) when commander characterization changes

## Local Workflow

1. Identify the commander and trigger type: channel-point redeem, commander-only command, support command, or shared help.
2. Preserve the commander slot model: Captain Stretch, The Director, and Water Wizard are independent slots and may all be active simultaneously.
3. Preserve chat-facing command names unless the operator explicitly requests a rename.
4. Read current state through `CPH.TryGetArg` or Streamer.bot globals defensively; protect against missing or malformed inputs.
5. Keep scripts self-contained and paste-ready. Do not assume shared runtime files can be imported by Streamer.bot actions.
6. Update local README documentation when trigger variables, command behavior, state variables, or operator wiring changes.
7. If a new global variable, OBS source, timer, or command contract is introduced, update [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) when in scope or flag it in the handoff when out of scope.

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
      "triggers": ["Twitch -> Chat Command -> !commanders"],
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
        "Read the three non-persisted commander slot globals defensively.",
        "Send one chat message listing only slots that currently have non-blank commander names.",
        "If all three slots are blank, send a short fallback message that the commander deck is open.",
        "Do not create, mutate, or persist any globals."
      ],
      "failureBehavior": [
        "Treat missing or blank commander globals as open slots.",
        "Return true after sending the roster or open-deck fallback message."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !commanders"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->

## Validation

For script changes, perform the narrowest safe validation available:

- Review the edited C# for Streamer.bot paste readiness: one `Execute()` entry point, no external imports beyond what Streamer.bot supports, and no dependencies on repo-only helper files.
- Verify all global names, OBS names, timers, and Mix It Up command names against [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md).
- Run shared-constants validation when constants or documented references change:

```bash
python3 Tools/StreamerBot/validate-shared-constants.py
```

For agent-doc changes, follow [validation](../../.agents/workflows/validation.md) and run the agent-tree validator with the task-requested report path. Record command output in the handoff or final change summary.

## Boundaries / Out of Scope

- Do not rename chat commands, Twitch rewards, global variables, or high-score keys unless explicitly requested.
- Do not collapse the three commander slots into a single active commander model.
- Do not change brand voice or character identity without `brand-steward` review.
- Do not migrate Twitch, Squad, Voice Commands, LotAT, overlay app, or creative-domain behavior from this guide.

## Handoff Notes

After changes, follow these workflows:

- [change-summary](../../.agents/workflows/change-summary.md) — terminal summary with changed files, paste targets, setup steps, and validation output.
- [sync](../../.agents/workflows/sync.md) — repo-to-Streamer.bot manual paste expectations.
- [validation](../../.agents/workflows/validation.md) — validation command selection and failure reporting.

Paste targets are the edited `.cs` files under [Actions/Commanders/](./). Operator must manually paste changed script contents into the matching Streamer.bot actions and verify trigger wiring for channel-point redeems or chat commands.

Brand handoff triggers: public chat copy, reward wording, command help text, commander identity, or character/lore changes. Include exactly which strings changed and whether `brand-steward` reviewed them.
