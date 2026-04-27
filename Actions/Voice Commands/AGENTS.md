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

This folder owns Streamer.bot C# actions driven by voice commands, buttons, hotkeys, or chained actions that set the current stream mode and switch OBS scenes based on that mode.

Voice-command work prioritizes reliable scene switching, safe fallback behavior, exact OBS scene names, and low operator friction during live production.

## When to Activate

Use this guide when editing or reviewing files under [Actions/Voice Commands/](./), including:

- [Actions/Voice Commands/mode-garage.cs](mode-garage.cs)
- [Actions/Voice Commands/mode-workspace.cs](mode-workspace.cs)
- [Actions/Voice Commands/mode-gamer.cs](mode-gamer.cs)
- [Actions/Voice Commands/scene-chat.cs](scene-chat.cs)
- [Actions/Voice Commands/scene-main.cs](scene-main.cs)
- [Actions/Voice Commands/scene-housekeeping.cs](scene-housekeeping.cs)
- [Actions/Voice Commands/scene-dance.cs](scene-dance.cs)
- README or operator documentation in this folder

Chain to another role only when the requested change crosses this folder's runtime boundary. Examples: `brand-steward` for public copy added to a voice command, `app-dev` for future app-driven scene control, or `ops` for validation/paste workflow.

## Primary Owner

`streamerbot-dev` owns the C# runtime behavior, Streamer.bot paste readiness, global `stream_mode` contract, OBS scene-switch calls, and compatibility with existing voice-command action wiring.

## Secondary Owners / Chain To

No secondary owner is required for normal voice-command script work.

Conditional handoffs:

- `brand-steward` — chain if a voice command starts sending public chat output, stream titles, labels, or community-facing copy.
- `app-dev` — chain if scene mode becomes controlled by an external app, overlay broker, or dashboard.
- `ops` — chain only for validation, paste/sync workflow, or agent-tree maintenance beyond this local guide.

## Required Reading

Read the local README and shared constants before editing scripts:

- [Actions/Voice Commands/README.md](README.md)
- [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md)
- [Actions/Helpers/obs-scenes.md](../Helpers/obs-scenes.md)
- [Actions/Twitch Core Integrations/stream-start.cs](../Twitch%20Core%20Integrations/stream-start.cs) when changing the default stream mode
- [Actions/Twitch Channel Points/disco-party.cs](../Twitch%20Channel%20Points/disco-party.cs) when changing dance/disco scene behavior

## Local Workflow

1. Determine whether the change affects mode-setting scripts, scene-switching scripts, or both.
2. Preserve the canonical lowercase mode values: `garage`, `workspace`, and `gamer`.
3. Preserve fallback behavior: scripts that read `stream_mode` should use `workspace` when the value is missing, empty, or unrecognized.
4. Use Streamer.bot's OBS scene setter directly. The known-good call pattern is `CPH.ObsSetScene(targetScene)`; see [Actions/Helpers/obs-scenes.md](../Helpers/obs-scenes.md).
5. Keep scripts self-contained and paste-ready. Do not assume shared runtime files can be imported by Streamer.bot actions.
6. Update [Actions/Voice Commands/README.md](README.md) when supported modes, scene names, trigger expectations, or operator notes change.
7. Update [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) when global variables or OBS scene/source contracts change, if in scope; otherwise flag the follow-up in the handoff.

Script map:

| Script | Runtime behavior |
|---|---|
| [mode-garage.cs](mode-garage.cs) | Sets `stream_mode` to `garage`. |
| [mode-workspace.cs](mode-workspace.cs) | Sets `stream_mode` to `workspace`. |
| [mode-gamer.cs](mode-gamer.cs) | Sets `stream_mode` to `gamer`. |
| [scene-chat.cs](scene-chat.cs) | Switches to the mode-appropriate Chat scene. |
| [scene-main.cs](scene-main.cs) | Switches to the mode-appropriate Main scene. |
| [scene-housekeeping.cs](scene-housekeeping.cs) | Switches to the mode-appropriate Housekeeping scene. |
| [scene-dance.cs](scene-dance.cs) | Switches to the mode-appropriate Disco Party scene. |

Scene naming contract:

| Mode | Chat | Main | Housekeeping | Dance |
|---|---|---|---|---|
| `garage` | `Garage: Chat` | `Garage: Main` | `Garage: Housekeeping` | `Disco Party: Garage` |
| `workspace` | `Workspace: Chat` | `Workspace: Main` | `Workspace: Housekeeping` | `Disco Party: Workspace` |
| `gamer` | `Gamer: Chat` | `Gamer: Main` | `Gamer: Housekeeping` | `Disco Party: Gamer` |

## Validation

For script changes, perform the narrowest safe validation available:

- Review edited C# for Streamer.bot paste readiness: one `Execute()` entry point per action, no external runtime imports, and no dependency on repo-only helper files.
- Verify `stream_mode` reads/writes and OBS scene names against [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) and [Actions/Voice Commands/README.md](README.md).
- Confirm every scene script has a safe `workspace` fallback for missing or unknown mode values.
- Confirm OBS scene switching uses the direct Streamer.bot call pattern rather than reflection-based method discovery.
- Run shared-constants validation when constants or documented references change:

```bash
python3 Tools/StreamerBot/validate-shared-constants.py
```

For agent-doc changes, follow [validation](../../.agents/workflows/validation.md) and run the agent-tree validator with the task-requested report path. Record command output in the handoff or final change summary.

## Boundaries / Out of Scope

- Do not rename OBS scenes, modes, global variables, or action triggers unless explicitly requested.
- Do not add chat output to these scripts unless the operator explicitly wants public feedback.
- Do not hard-fail on unknown `stream_mode`; fall back to `workspace` and log warnings where appropriate.
- Do not replace the direct OBS scene-switching call with reflection-based method lookup.
- Do not migrate Twitch, Squad, Commanders, LotAT, overlay app, or creative-domain behavior from this guide.

## Handoff Notes

After changes, follow these workflows:

- [change-summary](../../.agents/workflows/change-summary.md) — terminal summary with changed files, paste targets, setup steps, and validation output.
- [sync](../../.agents/workflows/sync.md) — repo-to-Streamer.bot manual paste expectations.
- [validation](../../.agents/workflows/validation.md) — validation command selection and failure reporting.

Paste targets are the edited `.cs` files under [Actions/Voice Commands/](./). Operator must manually paste changed script contents into the matching Streamer.bot actions and verify voice command, button, hotkey, or chained-action wiring.

Operator should test at least one mode script and one scene script after paste, then verify the OBS target scene changes as expected for the current `stream_mode`.
