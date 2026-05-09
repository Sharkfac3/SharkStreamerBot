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

This folder owns `!xj`: a Streamer.bot-to-overlay gag where non-commanders roll a 1-100 chance gate and rolls above 75 drive an XJ image across the overlay with rev-limiter audio.

Active Water Wizard, Captain Stretch, and The Director callers bypass the chance gate. Their `!xj` shows role-specific XJ pieces for 10 seconds; if all three active commander roles appear inside the 10-second triforce window, the action increments the stream count, updates the persisted high score only on a new record, chats, and plays the rev-limiter audio. One user holding multiple commander roles counts for all matching roles.

## Ownership

`streamerbot-dev` owns runtime behavior here. Shared ownership, validation, paste/sync, and brand-review rules inherit from [Actions/AGENTS.md](../AGENTS.md).

Chain to `product-dev` if this stops being a pure overlay gag and becomes product/content messaging; chain to `app-dev` for overlay broker, renderer, audio, or public asset-path changes.

## Required Reading

- [Actions/AGENTS.md](../AGENTS.md) for shared constants, helper-pattern routing, contracts, validation, and handoff rules.
- [Actions/constants/effects.md](../constants/effects.md) for canonical XJ drivethrough asset IDs, timing, chance-gate values, and active-state globals.
- [Actions/constants/commanders.md](../constants/commanders.md) for shared commander slot globals used by commander routing.
- [Actions/Overlay/AGENTS.md](../Overlay/AGENTS.md) for broker publisher rules and overlay handoffs.
- [Apps/stream-overlay/AGENTS.md](../../Apps/stream-overlay/AGENTS.md), [protocol](../../Apps/stream-overlay/docs/protocol.md), and [asset system](../../Apps/stream-overlay/docs/asset-system.md) for app-side broker, asset, or audio changes.
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) if public copy changes.

## Local Workflow

- Preserve the non-commander chance gate: rolls are 1-100 inclusive, and only 76-100 trigger the drivethrough.
- Failed non-commander rolls must exit before claiming `xj_drivethrough_active`; passed rolls must release that guard on every terminal path.
- Commander routing must use shared commander slot globals, not hard-coded usernames.
- Keep cleanup behavior: audio stop and overlay remove run after the drive finishes.
- Keep the publisher-side settle delay before `overlay.move` so first-time asset loading does not leave the Jeep off-screen.

## Script Summary

| Script | Summary |
|---|---|
| `xj-drivethrough-main.cs` | Contracted script for `!xj` non-commander chance drivethrough, commander XJ pieces, and triforce tracking. |

## Action Contracts

Contracts for xj-drivethrough-main.cs live in [contracts.md](contracts.md).

## Runtime Notes

Canonical XJ constants and shared commander-slot names live in [Actions/constants/effects.md](../constants/effects.md) and [Actions/constants/commanders.md](../constants/commanders.md).

Expected overlay assets are overlay-public paths: images/xj-drivethrough.png, images/xj-left.png, images/xj-middle.png, images/xj-right.png, and audio/xj-rev-limiter.mp3. Commander piece images are 640×640 px and align left/middle/right across a 1920 px canvas.

Core runtime values include `xj_drivethrough_active`, commander slot globals (`current_water_wizard`, `current_captain_stretch`, `current_the_director`), triforce globals (`xj_commander_triforce_*`), and broker topics listed in the action contract.

The repo may not contain all binary assets yet; missing files can still allow successful C# publishes while the overlay cannot display/play the intended media.

## Validation / Boundaries / Handoff

Follow [Actions/AGENTS.md](../AGENTS.md) for shared boundaries, contract validation, sync, paste targets, and terminal handoff. Do not implement app-side overlay/audio changes here, rename assets/globals without operator approval, or add product-facing claims without `product-dev` and `brand-steward` review.
