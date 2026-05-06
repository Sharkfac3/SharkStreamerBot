---
id: actions-commanders-the-director
type: domain-route
description: The Director commander slot scripts, redeem behavior, !award support command, and commander-only commands.
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# The Director — Agent Guide

## Purpose

This folder owns The Director commander slot scripts.
The slot is assigned via channel-point redeem, supported by public `!award`, and controlled by The Director-only commands: `!checkchat`, `!toad`, `!primary`, and `!secondary`.

## When to Activate

Use this guide when editing or reviewing any file under [Actions/Commanders/The Director/](./).

Activate the `brand-steward` role before changing public chat copy, command names presented to chat, or The Director character identity.

## Ownership

Owner: `streamerbot-dev`. Chain to `brand-steward` for The Director character voice, reward copy, or public command names. See `Actions/AGENTS.md` for full ownership rules.

## Required Reading

- [Actions/AGENTS.md](../../AGENTS.md) — shared Actions runtime, validation, ownership, and handoff rules.
- [Actions/Commanders/AGENTS.md](../AGENTS.md) — parent commander guide and shared commander slot model rules.
- [Creative/Brand/BRAND-VOICE.md](../../../Creative/Brand/BRAND-VOICE.md) when public copy changes.
- [Creative/Brand/CHARACTER-CODEX.md](../../../Creative/Brand/CHARACTER-CODEX.md) when The Director characterization changes.

## Local Workflow

1. Do not rename `current_the_director`, `the_director_award_count`, `the_director_award_high_score`, or `the_director_award_high_score_user` without explicit operator request.
2. Update the Action Contracts and Script Summary sections in this file when The Director trigger variables, command behavior, state variables, or operator wiring changes.
3. Keep `SCENE_SOURCE_MAP` in `the-director-primary.cs` and `the-director-secondary.cs` in sync when scene/source entries change.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "the-director-redeem.cs",
      "action": "Commanders - The Director - Redeem",
      "purpose": "Assigns The Director, finalizes outgoing !award score, checks high score, and fires the Mix It Up redeem command.",
      "triggers": [
        "Twitch -> Channel Reward -> Reward Redemption (The Director reward)"
      ],
      "globals": [
        "current_the_director",
        "the_director_award_count",
        "the_director_checkchat_next_allowed_utc",
        "the_director_toad_next_allowed_utc",
        "the_director_award_high_score",
        "the_director_award_high_score_user"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [
        "51146998-c2bc-4f46-b6a8-13069565a562"
      ],
      "overlayTopics": [],
      "serviceUrls": [
        "http://localhost:8911/api/v2/commands/{commandId}"
      ],
      "requiredLiterals": [
        "current_the_director",
        "the_director_award_count"
      ],
      "runtimeBehavior": [
        "Reads current_the_director and finalizes outgoing !award high-score state.",
        "Writes the new director to current_the_director.",
        "Resets !award count and The Director cooldown globals.",
        "Fires Mix It Up redeem command with new director identifiers."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for The Director channel-point redeem"
    },
    {
      "script": "the-director-award.cs",
      "action": "Commanders - The Director - Award",
      "purpose": "Handles public !award support calls for the current The Director. Increments the per-tenure award count. Blocks self-support.",
      "triggers": [
        "Twitch -> Chat Command -> !award"
      ],
      "globals": [
        "current_the_director",
        "the_director_award_count"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [
        "current_the_director",
        "the_director_award_count"
      ],
      "runtimeBehavior": [
        "Reads current_the_director defensively.",
        "Guides chat when no Director is active.",
        "Blocks The Director from self-supporting.",
        "Increments the_director_award_count and sends success chat."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !award"
    },
    {
      "script": "the-director-checkchat.cs",
      "action": "Commanders - The Director - Check Chat",
      "purpose": "Handles The Director-only !checkchat with input limits, a 1-minute cooldown, and Mix It Up dispatch.",
      "triggers": [
        "Twitch -> Chat Command -> !checkchat"
      ],
      "globals": [
        "current_the_director",
        "the_director_checkchat_next_allowed_utc"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [
        "06e3851f-81a2-40cb-a911-33c5ec04a3f2"
      ],
      "overlayTopics": [],
      "serviceUrls": [
        "http://localhost:8911/api/v2/commands/{commandId}"
      ],
      "requiredLiterals": [
        "current_the_director",
        "the_director_checkchat_next_allowed_utc"
      ],
      "runtimeBehavior": [
        "Reads current_the_director defensively.",
        "Guides unauthorized callers toward !award or redeem.",
        "Reports remaining cooldown to The Director.",
        "Validates optional text: 0-20 words, max 40 characters.",
        "Fires Mix It Up with the validated checkchat text.",
        "Sets the_director_checkchat_next_allowed_utc to now plus 1 minute."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !checkchat"
    },
    {
      "script": "the-director-toad.cs",
      "action": "Commanders - The Director - Toad",
      "purpose": "Handles The Director-only !toad with optional text, cooldown, and 1-in-10 hypno variant.",
      "triggers": [
        "Twitch -> Chat Command -> !toad"
      ],
      "globals": [
        "current_the_director",
        "the_director_toad_next_allowed_utc"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [
        "5440fa4e-b84e-438a-a409-f398b637f3e7"
      ],
      "overlayTopics": [],
      "serviceUrls": [
        "http://localhost:8911/api/v2/commands/{commandId}"
      ],
      "requiredLiterals": [
        "current_the_director",
        "the_director_toad_next_allowed_utc",
        "normal",
        "hypno"
      ],
      "runtimeBehavior": [
        "Reads current_the_director defensively.",
        "Guides unauthorized callers toward !award or redeem.",
        "Reports remaining cooldown to The Director.",
        "Validates optional text: 0-30 words.",
        "Chooses normal type by default, hypno on a 1-in-10 roll.",
        "Fires Mix It Up with toad text and type identifier.",
        "Sets the_director_toad_next_allowed_utc to now plus 1 minute."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !toad"
    },
    {
      "script": "the-director-primary.cs",
      "action": "Commanders - The Director - Primary",
      "purpose": "Lets The Director switch the current OBS scene to its primary source layout.",
      "triggers": [
        "Twitch -> Chat Command -> !primary"
      ],
      "globals": [
        "current_the_director"
      ],
      "timers": [],
      "obsSources": [
        "Main Screen Capture",
        "Quest POV"
      ],
      "obsScenes": [
        "Workspace: Main"
      ],
      "mixItUpCommandIds": [
        "REPLACE_WITH_PRIMARY_COMMAND_ID"
      ],
      "overlayTopics": [],
      "serviceUrls": [
        "http://localhost:8911/api/v2/commands/{commandId}"
      ],
      "requiredLiterals": [
        "current_the_director",
        "primary",
        "Workspace: Main",
        "Main Screen Capture",
        "Quest POV"
      ],
      "runtimeBehavior": [
        "Reads current_the_director defensively.",
        "Requires the caller to be active The Director.",
        "Reads the active OBS scene.",
        "Looks up source pair in SCENE_SOURCE_MAP.",
        "Shows primary source and hides secondary source.",
        "Optionally fires configured Mix It Up primary command.",
        "Logs the completed source switch."
      ],
      "failureBehavior": [
        "Guard exits leave OBS unchanged.",
        "Mix It Up remains inactive until placeholder command ID is replaced."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !primary"
    },
    {
      "script": "the-director-secondary.cs",
      "action": "Commanders - The Director - Secondary",
      "purpose": "Lets The Director switch the current OBS scene to its secondary source layout.",
      "triggers": [
        "Twitch -> Chat Command -> !secondary"
      ],
      "globals": [
        "current_the_director"
      ],
      "timers": [],
      "obsSources": [
        "Main Screen Capture",
        "Quest POV"
      ],
      "obsScenes": [
        "Workspace: Main"
      ],
      "mixItUpCommandIds": [
        "REPLACE_WITH_SECONDARY_COMMAND_ID"
      ],
      "overlayTopics": [],
      "serviceUrls": [
        "http://localhost:8911/api/v2/commands/{commandId}"
      ],
      "requiredLiterals": [
        "current_the_director",
        "secondary",
        "Workspace: Main",
        "Main Screen Capture",
        "Quest POV"
      ],
      "runtimeBehavior": [
        "Reads current_the_director defensively.",
        "Requires the caller to be active The Director.",
        "Reads the active OBS scene.",
        "Looks up source pair in SCENE_SOURCE_MAP.",
        "Shows secondary source and hides primary source.",
        "Optionally fires configured Mix It Up secondary command.",
        "Logs the completed source switch."
      ],
      "failureBehavior": [
        "Guard exits leave OBS unchanged.",
        "Mix It Up remains inactive until placeholder command ID is replaced."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !secondary"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->

## Validation

See `Actions/Commanders/AGENTS.md` and `Actions/AGENTS.md` for shared rules.

## Boundaries / Out of Scope

See `Actions/Commanders/AGENTS.md` and `Actions/AGENTS.md` for shared rules.

## Handoff Notes

See `Actions/Commanders/AGENTS.md` and `Actions/AGENTS.md` for shared rules.

---

## Script Summary

| Script | Summary |
|---|---|
| `the-director-redeem.cs` | Assigns The Director, finalizes outgoing `!award` score, checks persistent high-score keys, resets The Director cooldowns, and fires Mix It Up redeem command. |
| `the-director-award.cs` | Handles public `!award`, increments `the_director_award_count`, blocks self-support, and guides chat when no Director is active. |
| `the-director-checkchat.cs` | Handles The Director-only `!checkchat` with optional text limits, 1-minute cooldown, and Mix It Up dispatch. |
| `the-director-toad.cs` | Handles The Director-only `!toad` with optional text, 1-minute cooldown, and normal/hypno Mix It Up variant. |
| `the-director-primary.cs` | Handles The Director-only `!primary`, showing the primary OBS source and hiding the secondary source for the active scene. |
| `the-director-secondary.cs` | Handles The Director-only `!secondary`, showing the secondary OBS source and hiding the primary source for the active scene. |

### Unique State Keys

- Active slot global: `current_the_director`
- Support counter: `the_director_award_count`
- Persistent high-score keys: `the_director_award_high_score`, `the_director_award_high_score_user`
- The Director-only cooldown keys: `the_director_checkchat_next_allowed_utc`, `the_director_toad_next_allowed_utc`

### Primary / Secondary Scene Map

Keep `SCENE_SOURCE_MAP` in `the-director-primary.cs` and `the-director-secondary.cs` in sync when scene/source entries change.
Currently mapped: `Workspace: Main` (primary: `Main Screen Capture`, secondary: `Quest POV`).
