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

The following contract is the source of truth for script behavior. Update it before runtime behavior changes, then refresh the script stamp as described in [Actions/AGENTS.md](../AGENTS.md).

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "xj-drivethrough-main.cs",
      "action": "XJ Drivethrough / Main",
      "purpose": "Handles !xj chance-gated drivethroughs plus commander XJ pieces and 10-second triforce combos.",
      "triggers": [
        "Twitch -> Chat Command -> !xj",
        "Optional operator-wired manual trigger for testing"
      ],
      "globals": [
        "xj_drivethrough_active",
        "broker_connected",
        "current_water_wizard",
        "current_captain_stretch",
        "current_the_director",
        "xj_commander_triforce_active",
        "xj_commander_triforce_seen_json",
        "xj_commander_triforce_deadline_utc",
        "xj_commander_triforce_count",
        "xj_commander_triforce_high_score"
      ],
      "timers": [
        "XJ - Commander Triforce Window"
      ],
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
        "images/xj-left.png",
        "images/xj-middle.png",
        "images/xj-right.png",
        "xj-left",
        "xj-center",
        "xj-right",
        "xj-rev-limiter",
        "audio/xj-rev-limiter.mp3",
        "XJ_CHANCE_MIN",
        "XJ_CHANCE_MAX_EXCLUSIVE",
        "XJ_TRIGGER_THRESHOLD",
        "WAIT_SPAWN_SETTLE_MS",
        "WAIT_POST_DRIVE_MS",
        "XJ_COMMANDER_DISPLAY_MS",
        "XJ_COMMANDER_TRIFORCE_WINDOW_MS"
      ],
      "runtimeBehavior": [
        "Matches caller against active commander slot globals case-insensitively.",
        "Commander callers bypass non-commander chance gate.",
        "Non-commanders roll 1-100 before claiming xj_drivethrough_active.",
        "Non-commander rolls 75 or lower send no broker messages.",
        "Passed non-commander rolls claim the re-entry guard.",
        "Duplicate non-commander requests drop while guard is active.",
        "Spawns non-commander XJ off-screen left without enter animation.",
        "Waits briefly before move so asset loading settles.",
        "Moves XJ across 1920x1080 over 10000ms.",
        "Starts rev-limiter audio immediately after move publish.",
        "Stops audio and removes XJ after drive plus buffer.",
        "Releases xj_drivethrough_active in finally after claiming guard.",
        "Water Wizard spawns left XJ piece for 10000ms.",
        "Captain Stretch spawns center XJ piece for 10000ms.",
        "The Director spawns right XJ piece for 10000ms.",
        "Multi-role users spawn every matching commander piece.",
        "Commander pieces use stable per-piece asset IDs.",
        "Commander cleanup relies on overlay.spawn lifetime.",
        "Only commander calls affect triforce state.",
        "Tracks current-stream count separately from persisted high score.",
        "First commander role opens the triforce timer window.",
        "Active triforce windows count each commander role once.",
        "Completing all three roles increments count, updates high score, chats, plays audio, and clears state.",
        "Expired triforce windows clear state without changing counts or playing audio."
      ],
      "failureBehavior": [
        "Non-commander broker publish failure logs and relies on finally guard release.",
        "Commander spawn failure logs while keeping triforce state consistent.",
        "Disconnected WebSocket attempts reconnect and clears broker_connected if reconnect fails."
      ],
      "pasteTarget": "Streamer.bot action that runs the XJ Drivethrough main Execute C# Code sub-action"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->

## Runtime Notes

Expected overlay assets are overlay-public paths: images/xj-drivethrough.png, images/xj-left.png, images/xj-middle.png, images/xj-right.png, and audio/xj-rev-limiter.mp3. Commander piece images are 640×640 px and align left/middle/right across a 1920 px canvas.

Core runtime values include `xj_drivethrough_active`, commander slot globals (`current_water_wizard`, `current_captain_stretch`, `current_the_director`), triforce globals (`xj_commander_triforce_*`), and broker topics listed in the action contract.

The repo may not contain all binary assets yet; missing files can still allow successful C# publishes while the overlay cannot display/play the intended media.

## Validation / Boundaries / Handoff

Follow [Actions/AGENTS.md](../AGENTS.md) for shared boundaries, contract validation, sync, paste targets, and terminal handoff. Do not implement app-side overlay/audio changes here, rename assets/globals without operator approval, or add product-facing claims without `product-dev` and `brand-steward` review.
