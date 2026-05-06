---
id: actions-destroyer
type: domain-route
description: Destroyer Streamer.bot overlay-control actions, paste targets, asset requirements, and brand handoffs.
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Destroyer — Agent Guide

## Purpose

Streamer.bot actions for the Destroyer overlay chat interaction: viewers spawn a destroyer image, then move it with directional commands.

This is an overlay-broker feature, not an OBS source toggle or Squad mini-game. State is kept in non-persisted Streamer.bot globals and the overlay renders/removes the asset.

## When to Activate

Use this guide when editing or reviewing:

- [destroyer-spawn.cs](destroyer-spawn.cs)
- [destroyer-move.cs](destroyer-move.cs)

## Ownership

`streamerbot-dev` owns runtime behavior here; follow shared ownership, validation, sync, and handoff rules in [Actions/AGENTS.md](../AGENTS.md).

## Script Summary

| Script / command | Summary |
|---|---|
| [destroyer-spawn.cs](destroyer-spawn.cs) / `!destroyer` | Publishes `overlay.spawn` for the destroyer image asset, initializes `destroyer_active`, `destroyer_x`, `destroyer_y`, and `destroyer_expire_utc`, and ignores re-spawn while active. |
| [destroyer-move.cs](destroyer-move.cs) / `!up`, `!down`, `!left`, `!right` | Reads `%command%`, validates active/unexpired state, clamps movement to the 1920×1080 canvas, publishes `overlay.move`, and updates position globals. |

Required asset: [Apps/stream-overlay/packages/overlay/public/images/destroyer.jpg](../../Apps/stream-overlay/packages/overlay/public/images/destroyer.jpg). If it is missing, Streamer.bot may publish successfully while the overlay cannot render the destroyer.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "destroyer-move.cs",
      "action": "Destroyer Move",
      "purpose": "Contracts expected runtime behavior for destroyer-move.cs.",
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
        "Runs the documented destroyer-move.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for destroyer-move.cs"
    },
    {
      "script": "destroyer-spawn.cs",
      "action": "Destroyer Spawn",
      "purpose": "Contracts expected runtime behavior for destroyer-spawn.cs.",
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
        "Runs the documented destroyer-spawn.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for destroyer-spawn.cs"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->
