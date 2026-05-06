---
id: actions-temporary
type: domain-route
description: Temporary Streamer.bot timer bridge actions, ownership notes, paste targets, and Rest Focus relationship.
owner: streamerbot-dev
secondaryOwners: []
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Temporary — Agent Guide

## Purpose

Short-lived or experimental Streamer.bot C# scripts live here while they are being evaluated. Scripts in this folder may be removed or promoted to a permanent folder when the behavior stabilizes.

The current scripts are temporary focus-timer bridges being evaluated before any possible migration into [Actions/Rest Focus Loop/](../Rest%20Focus%20Loop/).

## When to Activate

Use this guide when editing or reviewing:

- [temp-focus-timer-start.cs](temp-focus-timer-start.cs)
- [temp-focus-timer-end.cs](temp-focus-timer-end.cs)

## Ownership

`streamerbot-dev` owns runtime behavior here; follow shared ownership, validation, sync, and handoff rules in [Actions/AGENTS.md](../AGENTS.md).

## Script Summary

| Script | Summary |
|---|---|
| [temp-focus-timer-start.cs](temp-focus-timer-start.cs) | Temporary voice-command/manual bridge that calls the “Temporary Lock In Timer” Mix It Up command and enables the `Temp Focus Timer` Streamer.bot timer. |
| [temp-focus-timer-end.cs](temp-focus-timer-end.cs) | Timer-end bridge for `Temp Focus Timer` that calls the “Commander - Captain Stretch - Lock In Timer - End” Mix It Up command. |

Keep `Temp Focus Timer` and placeholder Mix It Up command IDs unchanged unless the operator explicitly provides replacements. If these scripts graduate, flag the migration rather than silently treating this folder as part of Rest Focus Loop.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "temp-focus-timer-end.cs",
      "action": "Temp Focus Timer End",
      "purpose": "Contracts expected runtime behavior for temp-focus-timer-end.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented temp-focus-timer-end.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for temp-focus-timer-end.cs"
    },
    {
      "script": "temp-focus-timer-start.cs",
      "action": "Temp Focus Timer Start",
      "purpose": "Contracts expected runtime behavior for temp-focus-timer-start.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented temp-focus-timer-start.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for temp-focus-timer-start.cs"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->
