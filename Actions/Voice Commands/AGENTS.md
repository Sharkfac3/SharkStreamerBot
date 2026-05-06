---
id: actions-voice-commands
type: domain-route
description: Streamer.bot voice-command mode actions, OBS scene switching, paste targets, and validation guidance.
owner: streamerbot-dev
secondaryOwners: []
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Voice Commands — Agent Guide

## Purpose

This folder owns Streamer.bot C# actions that set the current stream mode and switch OBS scenes for that mode.

Voice-command work prioritizes exact `stream_mode` values, exact OBS scene names, safe fallback to workspace, and low operator friction during live production.

## When to Activate

Use this guide when editing or reviewing:

- [mode-garage.cs](mode-garage.cs)
- [mode-workspace.cs](mode-workspace.cs)
- [mode-gamer.cs](mode-gamer.cs)
- [scene-chat.cs](scene-chat.cs)
- [scene-main.cs](scene-main.cs)
- [scene-housekeeping.cs](scene-housekeeping.cs)
- [scene-dance.cs](scene-dance.cs)

## Ownership

`streamerbot-dev` owns these C# runtime actions. For shared ownership, validation, paste/sync, boundaries, and handoff rules, start with [Actions/AGENTS.md](../AGENTS.md).

## Required Reading

- [Actions/AGENTS.md](../AGENTS.md)
- [Actions/Helpers/obs-scenes.md](../Helpers/obs-scenes.md)
- [Actions/Twitch Core Integrations/stream-start.cs](../Twitch%20Core%20Integrations/stream-start.cs) when changing the default stream mode
- [Actions/Twitch Channel Points/disco-party.cs](../Twitch%20Channel%20Points/disco-party.cs) when changing dance/disco scene behavior

## Folder-Specific Rules

- Mode-prefixed scripts change stream mode state by writing canonical lowercase `stream_mode` values: `garage`, `workspace`, or `gamer`.
- Scene-prefixed scripts read `stream_mode` and change OBS scenes; missing, empty, or unknown mode must fall back to `workspace`.
- Scene scripts use direct Streamer.bot OBS scene switching (`CPH.ObsSetScene(targetScene)`); see [Actions/Helpers/obs-scenes.md](../Helpers/obs-scenes.md).
- Normal scenes follow `<Mode>: <Section>` naming, e.g. `Garage: Chat`; `scene-dance.cs` uses `Disco Party: <Mode>`.
- Do not rename modes, globals, OBS scenes, or triggers unless explicitly requested.

## Script Summary

| Script | Category | What it changes |
|---|---|---|
| [mode-garage.cs](mode-garage.cs) | mode | Sets `stream_mode` to `garage`. |
| [mode-workspace.cs](mode-workspace.cs) | mode | Sets `stream_mode` to `workspace`. |
| [mode-gamer.cs](mode-gamer.cs) | mode | Sets `stream_mode` to `gamer`. |
| [scene-chat.cs](scene-chat.cs) | scene | Switches OBS to `Garage: Chat`, `Workspace: Chat`, or `Gamer: Chat`. |
| [scene-main.cs](scene-main.cs) | scene | Switches OBS to `Garage: Main`, `Workspace: Main`, or `Gamer: Main`. |
| [scene-housekeeping.cs](scene-housekeeping.cs) | scene | Switches OBS to `Garage: Housekeeping`, `Workspace: Housekeeping`, or `Gamer: Housekeeping`. |
| [scene-dance.cs](scene-dance.cs) | scene | Switches OBS to `Disco Party: Garage`, `Disco Party: Workspace`, or `Disco Party: Gamer`. |

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "mode-gamer.cs",
      "action": "Mode Gamer",
      "purpose": "Contracts expected runtime behavior for mode-gamer.cs.",
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
        "Runs the documented mode-gamer.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for mode-gamer.cs"
    },
    {
      "script": "mode-garage.cs",
      "action": "Mode Garage",
      "purpose": "Contracts expected runtime behavior for mode-garage.cs.",
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
        "Runs the documented mode-garage.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for mode-garage.cs"
    },
    {
      "script": "mode-workspace.cs",
      "action": "Mode Workspace",
      "purpose": "Contracts expected runtime behavior for mode-workspace.cs.",
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
        "Runs the documented mode-workspace.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for mode-workspace.cs"
    },
    {
      "script": "scene-chat.cs",
      "action": "Scene Chat",
      "purpose": "Contracts expected runtime behavior for scene-chat.cs.",
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
        "Runs the documented scene-chat.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for scene-chat.cs"
    },
    {
      "script": "scene-dance.cs",
      "action": "Scene Dance",
      "purpose": "Contracts expected runtime behavior for scene-dance.cs.",
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
        "Runs the documented scene-dance.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for scene-dance.cs"
    },
    {
      "script": "scene-housekeeping.cs",
      "action": "Scene Housekeeping",
      "purpose": "Contracts expected runtime behavior for scene-housekeeping.cs.",
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
        "Runs the documented scene-housekeeping.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for scene-housekeeping.cs"
    },
    {
      "script": "scene-main.cs",
      "action": "Scene Main",
      "purpose": "Contracts expected runtime behavior for scene-main.cs.",
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
        "Runs the documented scene-main.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for scene-main.cs"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->
