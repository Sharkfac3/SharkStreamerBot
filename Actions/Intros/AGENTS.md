---
id: actions-intros
type: domain-route
description: Custom intro Streamer.bot actions spanning info-service data, Mix It Up audio, and public-copy handoffs.
owner: streamerbot-dev
secondaryOwners:
  - app-dev
  - brand-steward
  - ops
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Intros — Agent Guide

## Purpose

This folder owns Streamer.bot actions for custom viewer intros. `redeem-capture.cs` captures Custom Intro channel-point redemptions into info-service; `first-chat-intro.cs` plays approved intro audio when Streamer.bot's `Twitch -> Chat -> First Words` trigger fires for that viewer.

Important routing distinction: redemption capture is the submission/review intake path, while First Words playback is the per-stream first-chat intro path. Per-stream behavior depends on the operator resetting First Words for the stream.

## Ownership

`streamerbot-dev` owns runtime behavior here. Shared ownership, validation, paste/sync, and brand-review rules inherit from [Actions/AGENTS.md](../AGENTS.md).

Chain to `app-dev` for info-service schema/routes/data behavior, `brand-steward` for public reward or intro-policy wording, and `ops` for Mix It Up setup/tooling or validation workflow changes.

## Required Reading

- [Actions/AGENTS.md](../AGENTS.md) for shared constants, helper-pattern routing, contracts, validation, and handoff rules.
- [Apps/info-service/AGENTS.md](../../Apps/info-service/AGENTS.md), [INFO-SERVICE-PLAN.md](../../Apps/info-service/INFO-SERVICE-PLAN.md), and [README.md](../../Apps/info-service/README.md) for collection schemas, REST routes, startup, and data ownership.
- [Tools/MixItUp/AGENTS.md](../../Tools/MixItUp/AGENTS.md) and [README.md](../../Tools/MixItUp/README.md) for Custom Intro command conventions and API discovery.
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) if public-facing wording changes.

## Local Workflow

- Keep the two paths separate: channel reward redemptions write `pending-intros`; First Words reads approved `user-intros`.
- Preserve idempotency in `redeem-capture.cs`: duplicate `redeemId` records must not create duplicate pending entries.
- Preserve no-op behavior for missing args, missing/disabled records, missing sound files, HTTP errors, malformed JSON, or failed posts.
- Keep audio playback delegated to the `Intros - Play Custom Intro` Streamer.bot action unless the operator explicitly requests direct Mix It Up integration.
- Coordinate any collection name, asset subpath, or base URL migration with app docs and shared constants.

## Script Summary

| Script | Summary |
|---|---|
| `first-chat-intro.cs` | Contracted First Words handler that loads approved `user-intros`, sets `intro_sound_file_path`, and runs `Intros - Play Custom Intro`. |
| `redeem-capture.cs` | Contracted channel reward handler that deduplicates by redemption ID and writes pending records to `pending-intros`. |

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "first-chat-intro.cs",
      "action": "Intros - First Chat Intro",
      "purpose": "Play an approved custom intro sound for a viewer when their First Words trigger fires.",
      "triggers": [
        "Twitch -> Chat -> First Words"
      ],
      "globals": [
        "intro_sound_file_path"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [
        "http://127.0.0.1:8766"
      ],
      "requiredLiterals": [
        "user-intros",
        "enabled",
        "soundFile",
        "C:\\Users\\sharkfac3\\Workspace\\coding\\SharkStreamerBot\\Assets",
        "user-intros\\sound\\",
        "Intros - Play Custom Intro"
      ],
      "runtimeBehavior": [
        "Reads userId from First Words and no-ops when missing.",
        "GETs the user-intros record from info-service.",
        "Requires enabled record, soundFile, and existing local asset.",
        "Sets intro_sound_file_path, then runs Intros - Play Custom Intro synchronously."
      ],
      "failureBehavior": [
        "No-ops for missing userId, missing records, disabled intros, bad HTTP, malformed JSON, or missing files."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action: Intros - First Chat Intro"
    },
    {
      "script": "redeem-capture.cs",
      "action": "Intros - Redeem Capture",
      "purpose": "Capture Custom Intro channel-point redemptions into the pending-intros info-service collection.",
      "triggers": [
        "Twitch -> Channel Reward -> Reward Redemption"
      ],
      "globals": [],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [
        "http://127.0.0.1:8766"
      ],
      "requiredLiterals": [
        "pending-intros",
        "redemptionId",
        "rewardName",
        "rawInput",
        "status",
        "pending"
      ],
      "runtimeBehavior": [
        "Reads redemption/user/reward/input fields from Reward Redemption.",
        "No-ops when redemptionId is missing.",
        "GETs pending-intros by redemptionId and skips duplicates.",
        "POSTs a pending record keyed by redemptionId."
      ],
      "failureBehavior": [
        "No-ops for missing redemptionId, duplicates, HTTP exceptions, or failed POST responses."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action: Intros - Redeem Capture"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->

## Runtime Notes

Current flow:

| Runtime event | Repo file | Result |
|---|---|---|
| Custom Intro channel-point redemption | `redeem-capture.cs` | Creates a `pending-intros` review record keyed by redemption ID. |
| `Twitch -> Chat -> First Words` | `first-chat-intro.cs` | Plays enabled custom intro audio for the viewer through the `Intros - Play Custom Intro` chain. |

Info-service collections: `pending-intros` for submitted requests awaiting operator review; `user-intros` for approved per-user intro records. The C# script sets `intro_sound_file_path`, then runs `Intros - Play Custom Intro`; Mix It Up playback conventions live in [Tools/MixItUp/AGENTS.md](../../Tools/MixItUp/AGENTS.md).

## Validation / Boundaries / Handoff

Follow [Actions/AGENTS.md](../AGENTS.md) for shared boundaries, contract validation, sync, paste targets, and terminal handoff. Do not implement app-side info-service changes here, duplicate Mix It Up tooling docs, change public reward/policy copy without `brand-steward`, or expose private info-service notes in chat/overlay output.
