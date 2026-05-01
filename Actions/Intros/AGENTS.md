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

This folder owns the Streamer.bot actions for custom viewer intros. It captures custom-intro redemptions into the local info-service and plays approved intro audio when Streamer.bot's `Twitch -> Chat -> First Words` trigger fires for a viewer; with First Words reset per stream, this is the viewer's first chat of the stream.

The route crosses three systems:

1. Streamer.bot C# actions in [Actions/Intros/](./).
2. The local info-service collections served from [Apps/info-service/](../../Apps/info-service/).
3. Mix It Up audio playback through the operator-created `Intros - Play Custom Intro` Streamer.bot sub-action chain.

## When to Activate

Use this guide when editing or reviewing files under [Actions/Intros/](./):

- [first-chat-intro.cs](first-chat-intro.cs) — reads the `user-intros` collection and dispatches approved intro audio from the `Twitch -> Chat -> First Words` trigger.
- [redeem-capture.cs](redeem-capture.cs) — writes Custom Intro channel-point redemptions into the `pending-intros` collection.

Activate `app-dev` when collection schemas, info-service routes, production-manager fulfillment, or app runtime behavior changes.

Activate `brand-steward` before changing public-facing intro copy, reward wording, onboarding text, or moderation guidance for viewer intro submissions.

Activate `ops` when changing Mix It Up command setup, local API tooling, validation reports, sync handoffs, or operator checklists.

## Primary Owner

`streamerbot-dev` owns the C# runtime actions, Streamer.bot trigger wiring expectations, defensive argument handling, paste targets, and Streamer.bot-to-info-service call behavior.

## Secondary Owners / Chain To

- `app-dev` — chain for [Apps/info-service/](../../Apps/info-service/) schemas, routes, data persistence, and future production-manager fulfillment workflows.
- `brand-steward` — chain for public reward copy, viewer instructions, content-safety wording, or intro submission messaging.
- `ops` — chain for Mix It Up command/API conventions, validation, sync, and final handoff.

Use the app-local and tool-local guides as the preferred setup and protocol references for info-service and Mix It Up work.

## Required Reading

Before changing scripts, read:

- [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) for Info Service / Assets constants.
- [Actions/Helpers/AGENTS.md](../Helpers/AGENTS.md) for C# helper patterns.
- [Apps/info-service/AGENTS.md](../../Apps/info-service/AGENTS.md) and [Apps/info-service/INFO-SERVICE-PLAN.md](../../Apps/info-service/INFO-SERVICE-PLAN.md) for current collection schemas and REST routes.
- [Tools/MixItUp/AGENTS.md](../../Tools/MixItUp/AGENTS.md) for the current Custom Intro Mix It Up command convention and API payload shape.
- [Apps/info-service/README.md](../../Apps/info-service/README.md) for app-side startup and data ownership.
- [Tools/MixItUp/README.md](../../Tools/MixItUp/README.md) for local Mix It Up API tooling when command discovery is needed.
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) if public-facing wording changes.

## Local Workflow

1. Identify the side of the flow being changed: redemption capture, First Words playback, info-service data contract, or Mix It Up playback setup.
2. Preserve the info-service base URL and collection names unless a coordinated app-dev migration updates [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) and app docs.
3. Keep Streamer.bot trigger arguments defensive. Missing `userId` or `redeemId` must log and no-op safely.
4. Preserve idempotency in [redeem-capture.cs](redeem-capture.cs). Duplicate `redeemId` records should not create duplicate pending entries.
5. Preserve the First Words playback no-op behavior for missing records, disabled intros, missing sound files, HTTP errors, or malformed JSON.
6. Keep audio playback delegated to the `Intros - Play Custom Intro` Streamer.bot action unless the operator explicitly requests direct Mix It Up API integration.
7. If changing asset subpaths or collection names, update [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) and app-side protocol docs together when in scope.
8. For public instructions or reward descriptions, chain to `brand-steward` before finalizing text.

## Validation

For documentation-only changes, run:

```bash
python3 Tools/AgentTree/validate.py
```

For script changes:

- Manually review C# for Streamer.bot inline compatibility.
- Confirm info-service is running at http://127.0.0.1:8766.
- Smoke test GET /health for the info-service.
- For redemption capture, trigger a test Custom Intro redemption and confirm a `pending-intros` record appears.
- For First Words playback, create or use a `user-intros` record with `enabled` set true and a valid sound file, then trigger Streamer.bot `Twitch -> Chat -> First Words` behavior.
- Confirm `intro_sound_file_path` is set before the `Intros - Play Custom Intro` action runs.
- Confirm Mix It Up receives the path and plays the expected local audio file.

## Boundaries / Out of Scope

- Do not implement or refactor app-side info-service behavior in this folder.
- Do not document full app-side info-service internals here; defer to [Apps/info-service/AGENTS.md](../../Apps/info-service/AGENTS.md) and [Apps/info-service/INFO-SERVICE-PLAN.md](../../Apps/info-service/INFO-SERVICE-PLAN.md).
- Do not document full Mix It Up tooling here; defer to [Tools/MixItUp/AGENTS.md](../../Tools/MixItUp/AGENTS.md).
- Do not change public reward copy or intro policy without `brand-steward` review.
- Do not store or expose private notes from info-service records in chat or overlay output.

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
      "triggers": ["Twitch -> Chat -> First Words"],
      "globals": ["intro_sound_file_path"],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": ["http://127.0.0.1:8766"],
      "requiredLiterals": [
        "user-intros",
        "enabled",
        "soundFile",
        "C:\\Users\\sharkfac3\\Workspace\\coding\\SharkStreamerBot\\Assets",
        "user-intros\\sound\\",
        "Intros - Play Custom Intro"
      ],
      "runtimeBehavior": [
        "Read userId from the First Words trigger and no-op when it is missing.",
        "GET the user-intros record for userId from the info-service.",
        "No-op unless the record exists, enabled is true, soundFile is non-empty, and the resolved sound file exists.",
        "Set non-persisted intro_sound_file_path to the resolved asset path, then run Intros - Play Custom Intro synchronously."
      ],
      "failureBehavior": [
        "Log and return true for missing userId, 404 records, non-200 responses, HTTP exceptions, malformed JSON, disabled intros, empty soundFile, or missing local files."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action: Intros - First Chat Intro"
    },
    {
      "script": "redeem-capture.cs",
      "action": "Intros - Redeem Capture",
      "purpose": "Capture Custom Intro channel-point redemptions into the pending-intros info-service collection.",
      "triggers": ["Twitch -> Channel Reward -> Reward Redemption"],
      "globals": [],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": ["http://127.0.0.1:8766"],
      "requiredLiterals": [
        "pending-intros",
        "redemptionId",
        "rewardName",
        "rawInput",
        "status",
        "pending"
      ],
      "runtimeBehavior": [
        "Read userId, userLogin, redemptionId, rewardName, and rawInput from the Reward Redemption trigger.",
        "No-op when redemptionId is missing.",
        "GET the pending-intros record by redemptionId and no-op when it already exists.",
        "POST a pending record keyed by redemptionId with user, reward, input, redeemUtc, and status fields."
      ],
      "failureBehavior": [
        "Log and return true for missing redemptionId, duplicate records, duplicate-check HTTP exceptions, POST non-success responses, or POST exceptions."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action: Intros - Redeem Capture"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->

## Handoff Notes

Use the terminal workflows after changes:

- [change-summary](../../.agents/workflows/change-summary.md)
- [sync](../../.agents/workflows/sync.md)
- [validation](../../.agents/workflows/validation.md)

For code changes, list Streamer.bot paste targets for each edited script and include any operator setup required for:

- `Twitch -> Chat -> First Words` trigger wiring, including First Words reset timing if per-stream first-chat behavior is desired.
- Custom Intro channel-point redemption trigger wiring.
- The `Intros - Play Custom Intro` action chain.
- Mix It Up `Custom Intro` command setup.
- info-service startup and data records.

## Runtime Notes

Current flow:

| Runtime event | Repo file | External dependency | Result |
|---|---|---|---|
| Custom Intro channel-point redemption | [redeem-capture.cs](redeem-capture.cs) | [Apps/info-service/](../../Apps/info-service/) | Creates a `pending-intros` record keyed by redemption ID. |
| `Twitch -> Chat -> First Words` | [first-chat-intro.cs](first-chat-intro.cs) | [Apps/info-service/](../../Apps/info-service/) and Mix It Up | Plays enabled custom intro audio for the viewer; per-stream first-chat behavior depends on resetting First Words for the stream. |

Info-service collections used by this route:

| Collection | Used by | Purpose |
|---|---|---|
| `pending-intros` | [redeem-capture.cs](redeem-capture.cs) | Operator review queue for submitted intro requests. |
| `user-intros` | [first-chat-intro.cs](first-chat-intro.cs) | Approved per-user intro records. |

The current Mix It Up playback convention is documented in [Tools/MixItUp/AGENTS.md](../../Tools/MixItUp/AGENTS.md). The C# script sets `intro_sound_file_path`, then runs `Intros - Play Custom Intro`; the sub-action chain passes the path into the Mix It Up `Custom Intro` command.
