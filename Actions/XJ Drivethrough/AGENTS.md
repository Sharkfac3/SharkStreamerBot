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

This folder owns the XJ Drivethrough Streamer.bot action. The action drives a Jeep Cherokee XJ image across the overlay while playing a rev-limiter audio clip through the overlay audio system.

The feature is a Streamer.bot runtime bridge into the stream overlay. It publishes generic overlay spawn, move, audio-play, audio-stop, and remove messages through the broker.

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
2. Keep the re-entry guard reliable. `xj_drivethrough_active` must be cleared on every terminal path.
3. Keep broker constants and topic strings aligned with [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) and the overlay app contract.
4. Maintain cleanup behavior: audio stop and overlay remove should run after the drive finishes.
5. If changing asset ID, source path, dimensions, duration, or audio ID, update [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) when in scope.
6. Keep scripts self-contained and paste-ready for Streamer.bot inline C#.
7. If the effect becomes product/content messaging instead of a pure gag/overlay effect, chain to `product-dev` and `brand-steward` before finalizing wording or documentation.

## Validation

For documentation-only changes, run:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-06-validator.failures.txt
```

For script changes:

- Manually review C# for Streamer.bot inline compatibility.
- Confirm the overlay broker is running and connected before triggering the action.
- Confirm the XJ image asset exists in the overlay public images folder.
- Confirm the rev-limiter audio asset exists in the overlay public audio folder.
- Confirm the OBS browser source has audio enabled.
- Trigger [xj-drivethrough-main.cs](xj-drivethrough-main.cs) and verify the image drives fully across, audio starts, audio stops, and the asset is removed.
- Trigger the action twice quickly and verify the re-entry guard drops the second request safely.

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

## Runtime Notes

Expected asset locations in the overlay app:

| Asset | Expected overlay public path |
|---|---|
| XJ image | images/xj-drivethrough.png |
| Rev-limiter audio | audio/xj-rev-limiter.mp3 |

The repo may not contain those binary assets yet. If missing, the C# action can publish successfully while the overlay cannot display or play the intended media.

Core runtime variable:

| Variable | Meaning |
|---|---|
| `xj_drivethrough_active` | Non-persisted guard that prevents overlapping drivethrough sequences. |
