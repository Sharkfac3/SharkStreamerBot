---
id: actions-twitch-channel-points
type: domain-route
description: Twitch channel point redemption Streamer.bot actions, OBS/Mix It Up behavior, paste targets, and brand handoffs.
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Twitch Channel Points — Agent Guide

## Purpose

This folder owns repo-maintained Streamer.bot C# scripts for Twitch channel point redemptions. Current actions cover Disco Party and Explain Current Task, bridging redemptions into OBS scene/recording behavior and Mix It Up commands.

## When to Activate

Use this guide when editing or reviewing:

- [disco-party.cs](disco-party.cs)
- [explain-current-task.cs](explain-current-task.cs)
- Script/operator documentation in this folder

## Ownership

`streamerbot-dev` owns runtime behavior; chain to `brand-steward` for public redemption copy, reward names/descriptions, chat messages, overlay text, TTS/spoken text, or Mix It Up viewer-facing wording.

Start with [Actions/AGENTS.md](../AGENTS.md) for shared Actions rules, validation, sync, and handoff expectations. Also read [Twitch Core Integrations/AGENTS.md](../Twitch%20Core%20Integrations/AGENTS.md) when changes add/reset shared session state.

## Folder-Specific Rules

- Preserve existing reward names, reward IDs, command IDs, OBS scene names, and trigger wiring unless explicitly asked to migrate them.
- Text-input rewards may expose viewer text through `userInput`, `input0`, `message`, or `rawInput`; read defensively.
- Disco Party routes by `stream_mode`; missing/unknown mode falls back to `workspace` behavior.
- Exact Disco Party OBS scenes are `Disco Party: Garage`, `Disco Party: Workspace`, and `Disco Party: Gamer`.
- `Explain: Ask Away` exists in Streamer.bot but has no matching repo script here yet; do not invent it unless requested.
- Custom Intro redemptions belong to [Actions/Intros/](../Intros/), not this folder.

## Script Summary

| Script | Purpose |
|---|---|
| [disco-party.cs](disco-party.cs) | Runs Disco Party start/end Mix It Up commands, switches to the mode-matched OBS scene, fires unlocked squad dance commands, then returns to the previous scene. |
| [explain-current-task.cs](explain-current-task.cs) | Ensures OBS recording is active, then triggers the Explain Current Task Mix It Up flow; command ID placeholder is `replace-with-actual-id-dyude-cmon`. |

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "disco-party.cs",
      "action": "Disco Party",
      "purpose": "Contracts expected runtime behavior for disco-party.cs.",
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
        "Runs the documented disco-party.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for disco-party.cs"
    },
    {
      "script": "explain-current-task.cs",
      "action": "Explain Current Task",
      "purpose": "Contracts expected runtime behavior for explain-current-task.cs.",
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
        "Runs the documented explain-current-task.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for explain-current-task.cs"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->
