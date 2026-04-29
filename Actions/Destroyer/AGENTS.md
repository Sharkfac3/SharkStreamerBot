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

This folder owns the Destroyer Streamer.bot actions. Viewers can spawn a destroyer image on the overlay and move it with directional chat commands while the overlay broker handles rendering.

The feature is a Streamer.bot runtime bridge into the stream overlay: C# actions publish generic overlay messages through the broker and store current position/lifetime state in non-persisted globals.

## When to Activate

Use this guide when editing or reviewing files under [Actions/Destroyer/](./):

- [destroyer-spawn.cs](destroyer-spawn.cs) — spawns the destroyer image at center screen.
- [destroyer-move.cs](destroyer-move.cs) — handles the shared move action for the directional commands.

Activate `brand-steward` before changing public command wording, chat-facing feedback, stream-visible theme/identity, or any lore/character framing for the destroyer.

## Primary Owner

`streamerbot-dev` owns this folder: Streamer.bot C# behavior, paste readiness, global variables, chat-command trigger assumptions, broker-publish helper usage, and runtime safety.

## Secondary Owners / Chain To

- `brand-steward` — chain for public text, feature naming, theme/lore implications, or stream-visible messaging.
- `app-dev` — chain if the overlay broker protocol, topic names, asset rendering behavior, or app-side asset locations need to change.
- `ops` — chain for validation, sync, and final handoff workflow.

## Required Reading

Before changing scripts, read:

- [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) for the Destroyer and Overlay / Broker constants.
- [Actions/HELPER-SNIPPETS.md](../HELPER-SNIPPETS.md) for reusable C# patterns.
- [Actions/Overlay/AGENTS.md](../Overlay/AGENTS.md) for broker publisher rules and overlay handoffs.
- [Apps/stream-overlay/AGENTS.md](../../Apps/stream-overlay/AGENTS.md) for route/ownership, [protocol](../../Apps/stream-overlay/docs/protocol.md) for broker changes, and [asset system](../../Apps/stream-overlay/docs/asset-system.md) for asset behavior changes.
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) if public copy changes.

## Local Workflow

1. Preserve the two-action model: spawn is separate from movement; all directional commands should route to [destroyer-move.cs](destroyer-move.cs).
2. Keep action code self-contained and paste-ready for Streamer.bot inline C#.
3. Keep overlay topic strings and broker constants aligned with [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) and the overlay app contract.
4. Keep the active-state guard reliable. The spawn action owns `destroyer_active`, `destroyer_x`, `destroyer_y`, and `destroyer_expire_utc`; move reads and updates those values.
5. Preserve boundary clamping so the image cannot move off canvas.
6. If changing asset size or starting position, update both scripts and [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) together when in scope.
7. Update this guide or a future local README if trigger wiring, asset paths, global variables, or operator setup changes.

## Validation

For documentation-only changes, run the agent-tree validator:

```bash
python3 Tools/AgentTree/validate.py
```

For script changes:

- Manually review C# for Streamer.bot inline compatibility.
- Verify global variable names and asset constants against [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md).
- Confirm the overlay broker is connected before triggering spawn or move actions.
- Smoke test `!destroyer`, `!up`, `!down`, `!left`, and `!right` in Streamer.bot.

## Boundaries / Out of Scope

- Do not change app-side overlay rendering code from this folder; use [Apps/stream-overlay/AGENTS.md](../../Apps/stream-overlay/AGENTS.md) and [rendering notes](../../Apps/stream-overlay/docs/rendering-notes.md).
- Do not rename chat commands, asset IDs, or global variables without explicit operator approval.
- Do not merge this route into Squad, Twitch, Voice Commands, LotAT, or Overlay docs.
- Do not add public lore/brand copy without `brand-steward` review.

## Handoff Notes

Use the terminal workflows after changes:

- [change-summary](../../.agents/workflows/change-summary.md)
- [sync](../../.agents/workflows/sync.md)
- [validation](../../.agents/workflows/validation.md)

Paste targets are the edited C# files in this folder. Note any Streamer.bot trigger wiring changes, overlay asset requirements, and whether [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) was updated or needs follow-up.

## Runtime Notes

Expected Streamer.bot commands:

| Command | Repo file | Runtime role |
|---|---|---|
| `!destroyer` | [destroyer-spawn.cs](destroyer-spawn.cs) | Spawn the destroyer image. |
| `!up` | [destroyer-move.cs](destroyer-move.cs) | Move up one step. |
| `!down` | [destroyer-move.cs](destroyer-move.cs) | Move down one step. |
| `!left` | [destroyer-move.cs](destroyer-move.cs) | Move left one step. |
| `!right` | [destroyer-move.cs](destroyer-move.cs) | Move right one step. |

Required overlay asset:

- [Apps/stream-overlay/packages/overlay/public/images/destroyer.jpg](../../Apps/stream-overlay/packages/overlay/public/images/destroyer.jpg)

If the asset is not present, the C# action may publish successfully while the overlay cannot render the image.
