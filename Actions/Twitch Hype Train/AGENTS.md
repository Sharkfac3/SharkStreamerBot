---
id: actions-twitch-hype-train
type: domain-route
description: Twitch hype train Streamer.bot event bridges, Mix It Up metadata payloads, paste targets, and brand handoffs.
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Twitch Hype Train — Agent Guide

## Purpose

This folder owns Streamer.bot C# scripts for Twitch hype train events. The scripts are lightweight bridges that forward hype train metadata to Mix It Up through populated `SpecialIdentifiers` while keeping `Arguments` empty for current command compatibility.

## When to Activate

Use this guide when editing or reviewing:

- [hype-train-start.cs](hype-train-start.cs)
- [hype-train-level-up.cs](hype-train-level-up.cs)
- [hype-train-end.cs](hype-train-end.cs)
- Script/operator documentation in this folder

## Ownership

`streamerbot-dev` owns runtime behavior; chain to `brand-steward` for public hype-train alert copy, overlay text, TTS/spoken text, reward/celebration wording, or Mix It Up message branches.

Start with [Actions/AGENTS.md](../AGENTS.md) for shared Actions rules, validation, sync, and handoff expectations. Also read [Twitch Core Integrations/AGENTS.md](../Twitch%20Core%20Integrations/AGENTS.md) when changes affect shared Twitch event conventions.

## Folder-Specific Rules

- Hype train phase sequence is `start -> level-up -> end`; `level-up`/progress can fire multiple times during one train.
- Preserve lightweight bridge behavior: forward event metadata to Mix It Up, not a stateful mini-game or OBS controller.
- Keep `Arguments = ""` and put metadata in lowercase/no-space `hypetrain*` `SpecialIdentifiers`.
- Placeholder command IDs must log a warning and exit gracefully.
- Missing trigger args should resolve to empty strings, `0`, or `false` strings instead of throwing.
- Do not add OBS interactions unless explicitly requested.

## Script Summary

| Script | Purpose |
|---|---|
| [hype-train-start.cs](hype-train-start.cs) | Handles train start, forwards level/progress/type/timing/top-contributor metadata with `hypetrainevent = start`. |
| [hype-train-level-up.cs](hype-train-level-up.cs) | Handles repeatable train progress/level-up events, including `prevLevel`, with `hypetrainevent = levelup`. |
| [hype-train-end.cs](hype-train-end.cs) | Handles train end, forwards final level/progress/type/top-contributor metadata with `hypetrainevent = end`. |

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "hype-train-end.cs",
      "action": "Hype Train End",
      "purpose": "Contracts expected runtime behavior for hype-train-end.cs.",
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
        "Runs the documented hype-train-end.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for hype-train-end.cs"
    },
    {
      "script": "hype-train-level-up.cs",
      "action": "Hype Train Level Up",
      "purpose": "Contracts expected runtime behavior for hype-train-level-up.cs.",
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
        "Runs the documented hype-train-level-up.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for hype-train-level-up.cs"
    },
    {
      "script": "hype-train-start.cs",
      "action": "Hype Train Start",
      "purpose": "Contracts expected runtime behavior for hype-train-start.cs.",
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
        "Runs the documented hype-train-start.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for hype-train-start.cs"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->
