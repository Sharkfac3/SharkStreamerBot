---
id: actions-xj-drivethrough
type: domain-route
description: XJ Drivethrough Streamer.bot overlay action, asset requirements, product-facing handoffs, and validation notes.
owner: streamerbot-dev
secondaryOwners:
  - product-dev
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# XJ Drivethrough — Agent Guide

## Purpose

This folder owns the XJ Drivethrough Streamer.bot action. The action rolls a 1-100 chance gate and, on rolls above 85, drives a Jeep Cherokee XJ image across the overlay while playing a rev-limiter audio clip through the overlay audio system.

The feature is a Streamer.bot runtime bridge into the stream overlay. Successful chance rolls publish generic overlay spawn, move, audio-play, audio-stop, and remove messages through the broker. Failed chance rolls log and exit without touching the re-entry guard or broker.

## When to Activate

Use this guide when editing or reviewing files under [Actions/XJ Drivethrough/](./):

- [xj-drivethrough-main.cs](xj-drivethrough-main.cs) — complete drivethrough sequence and broker publish helper.

Activate `product-dev` if the XJ visual becomes product-facing, is tied to real off-road product messaging, or needs technical/customer-facing documentation.

Activate `brand-steward` before changing public command wording, stream-facing names, humor tone, vehicle framing, alert copy, or any messaging viewers will see or hear.

## Primary Owner

`streamerbot-dev` owns the C# runtime behavior, Streamer.bot trigger assumptions, global re-entry guard, broker-publish helper, paste readiness, and manual operator setup notes.

## Secondary Owners / Chain To

- `product-dev` — chain if the XJ effect becomes tied to product documentation, technical explanation, sponsorship/product positioning, or customer-facing content.
- `brand-steward` — chain for public copy, feature naming, voice/tone, and stream-visible messaging.
- `app-dev` — chain if overlay broker topics, rendering, audio handling, or public asset paths need app-side changes.
- `ops` — chain for validation, sync, and final handoff workflow.

## Required Reading

Before changing scripts, read:

- [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) for XJ Drivethrough and Overlay / Broker constants.
- [Actions/HELPER-SNIPPETS.md](../HELPER-SNIPPETS.md) for reusable C# patterns.
- [Actions/Overlay/AGENTS.md](../Overlay/AGENTS.md) for broker publisher rules and overlay handoffs.
- [Apps/stream-overlay/AGENTS.md](../../Apps/stream-overlay/AGENTS.md) for route/ownership, [protocol](../../Apps/stream-overlay/docs/protocol.md) for broker changes, and [asset system](../../Apps/stream-overlay/docs/asset-system.md) for asset or audio behavior changes.
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) if public copy changes.

## Local Workflow

1. Preserve the single-action sequence unless the operator explicitly asks for a multi-action workflow.
2. Preserve the chance gate unless the operator explicitly asks to change trigger frequency. Current behavior rolls 1-100 inclusive and only runs on rolls greater than 85 (86-100).
3. Keep the re-entry guard reliable. `xj_drivethrough_active` must be cleared on every terminal path after the active slot is claimed; failed chance rolls should exit before claiming it.
4. Keep broker constants and topic strings aligned with [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) and the overlay app contract.
5. Maintain cleanup behavior: audio stop and overlay remove should run after the drive finishes.
6. If changing asset ID, source path, dimensions, duration, audio ID, or chance constants, update [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) when in scope.
7. Keep scripts self-contained and paste-ready for Streamer.bot inline C#.
8. If the effect becomes product/content messaging instead of a pure gag/overlay effect, chain to `product-dev` and `brand-steward` before finalizing wording or documentation.

## Validation

For documentation-only changes, run:

```bash
python3 Tools/AgentTree/validate.py
```

For script changes:

- Manually review C# for Streamer.bot inline compatibility.
- Confirm the overlay broker is running and connected before triggering the action.
- Confirm the XJ image asset exists in the overlay public images folder.
- Confirm the rev-limiter audio asset exists in the overlay public audio folder.
- Confirm the OBS browser source has audio enabled.
- Trigger [xj-drivethrough-main.cs](xj-drivethrough-main.cs) and verify failed rolls log a no-op without sending broker messages.
- Trigger until the chance gate passes, then verify the image drives fully across, audio starts, audio stops, and the asset is removed.
- Trigger the action twice quickly after a passed roll and verify the re-entry guard drops overlapping requests safely.

## Boundaries / Out of Scope

- Do not implement app-side overlay rendering or audio changes in this folder; use [Apps/stream-overlay/AGENTS.md](../../Apps/stream-overlay/AGENTS.md), [rendering notes](../../Apps/stream-overlay/docs/rendering-notes.md), and [asset system](../../Apps/stream-overlay/docs/asset-system.md).
- Do not rename asset IDs, paths, or global variables without explicit operator approval.
- Do not introduce product-facing claims, sponsorship framing, or off-road product documentation without `product-dev` and `brand-steward` review.
- Do not move this route into Twitch, Squad, Overlay, or Product docs during this coverage-fill prompt.

## Handoff Notes

Use the terminal workflows after changes:

- [change-summary](../../.agents/workflows/change-summary.md)
- [sync](../../.agents/workflows/sync.md)
- [validation](../../.agents/workflows/validation.md)

For code changes, list [xj-drivethrough-main.cs](xj-drivethrough-main.cs) as the Streamer.bot paste target and note any trigger wiring, asset, audio, OBS, or overlay-broker setup the operator must verify.

Flag ownership ambiguity if the feature is no longer just an overlay gag and should be product/content-owned.

## Action Contracts

The following contract is the source of truth for the script behavior. Update this block before changing runtime behavior, then refresh the script stamp with `python3 Tools/StreamerBot/Validation/action_contracts.py --script "Actions/XJ Drivethrough/xj-drivethrough-main.cs" --stamp`.

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "xj-drivethrough-main.cs",
      "action": "XJ Drivethrough / Main",
      "purpose": "Roll a 1-100 chance gate and, on rolls above 85, drive the Jeep Cherokee XJ image across the stream overlay while playing the rev-limiter audio clip.",
      "triggers": [
        "Any Streamer.bot trigger: chat command, channel point redemption, manual button, or operator-wired trigger"
      ],
      "globals": [
        "xj_drivethrough_active",
        "broker_connected"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [
        "overlay.spawn",
        "overlay.move",
        "overlay.audio.play",
        "overlay.audio.stop",
        "overlay.remove"
      ],
      "serviceUrls": [],
      "requiredLiterals": [
        "streamerbot",
        "xj-drivethrough",
        "images/xj-drivethrough.png",
        "xj-rev-limiter",
        "audio/xj-rev-limiter.mp3",
        "XJ_CHANCE_MIN",
        "XJ_CHANCE_MAX_EXCLUSIVE",
        "XJ_TRIGGER_THRESHOLD",
        "WAIT_SPAWN_SETTLE_MS",
        "WAIT_POST_DRIVE_MS"
      ],
      "runtimeBehavior": [
        "Roll 1-100 inclusive before claiming the re-entry guard.",
        "Exit with no broker messages when the roll is 85 or lower.",
        "Claim non-persisted xj_drivethrough_active only after the chance gate passes.",
        "Drop duplicate requests while xj_drivethrough_active is true.",
        "Spawn the XJ asset off-screen left with no enter animation.",
        "Wait briefly after spawn before publishing overlay.move so first-time asset loading can settle.",
        "Move the XJ asset fully across the 1920x1080 canvas over 10000ms.",
        "Start the rev-limiter audio immediately after the move command.",
        "After the drive duration plus buffer, stop the audio and remove the XJ asset with no exit animation.",
        "Always release xj_drivethrough_active in a finally block after the active slot is claimed."
      ],
      "failureBehavior": [
        "If broker publish fails during spawn, log the error, return true, and rely on the finally block to release the guard.",
        "If the WebSocket is disconnected, attempt one reconnect, resend client.hello, and mark broker_connected false if reconnect fails.",
        "Return true for handled no-op or failure paths so Streamer.bot does not treat expected live-stream conditions as script crashes."
      ],
      "pasteTarget": "Streamer.bot action that runs the XJ Drivethrough main Execute C# Code sub-action"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->

## Runtime Notes

Expected asset locations in the overlay app:

| Asset | Expected overlay public path |
|---|---|
| XJ image | images/xj-drivethrough.png |
| Rev-limiter audio | audio/xj-rev-limiter.mp3 |

The repo may not contain those binary assets yet. If missing, the C# action can publish successfully while the overlay cannot display or play the intended media.

Movement timing note: XJ is a spawn-then-immediate-move effect. Keep a publisher-side settle delay before `overlay.move` so first-time asset loading does not leave the Jeep off-screen. The overlay renderer has a pending-move safety net, but future moving-asset effects should still follow the same pattern documented in `Apps/stream-overlay/docs/asset-system.md`.

Core runtime values:

| Value | Meaning |
|---|---|
| `xj_drivethrough_active` | Non-persisted guard that prevents overlapping drivethrough sequences. |
| `XJ_CHANCE_MIN = 1` | Inclusive lower bound for chance rolls. |
| `XJ_CHANCE_MAX_EXCLUSIVE = 101` | Exclusive upper bound for chance rolls, producing 1-100. |
| `XJ_TRIGGER_THRESHOLD = 85` | Rolls must be greater than this value; 86-100 trigger the sequence. |
