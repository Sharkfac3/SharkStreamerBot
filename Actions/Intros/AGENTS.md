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

This folder owns Streamer.bot actions for custom viewer intros. `redeem-capture.cs` captures Custom Intro channel-point redemptions into info-service; `first-chat-intro.cs` resolves approved intro assets and dispatches the Mix It Up intro chain when Streamer.bot's `Twitch -> Chat -> First Words` trigger fires for that viewer.

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
- Preserve no-op behavior for missing args, missing/disabled records, missing all usable assets, HTTP errors, malformed JSON, or failed posts.
- Treat `first-chat-intro.cs` as the authoritative gatekeeper for whether a first-chat intro runs at all; Mix It Up should stay playback-focused.
- Keep filename resolution local to Streamer.bot: normalize stored asset values to filename-only names, resolve full paths under the Assets intro sound/gif subfolders, and pass resolved paths downstream.
- Use the helper pattern from `Actions/Helpers/mixitup-command-api.md` for first-chat playback handoff.
- Send resolved intro asset paths to Mix It Up in `SpecialIdentifiers.intro_sound_file_path` and `SpecialIdentifiers.intro_gif_file_path`.
- Keep the retired `Intros - Play Custom Intro` wrapper out of the runtime path.
- Coordinate any collection name, asset subpath, or base URL migration with app docs and shared constants.

## Script Summary

| Script | Summary |
|---|---|
| `first-chat-intro.cs` | Contracted First Words handler that loads approved `user-intros`, normalizes optional sound/gif filenames, resolves local asset paths, and calls Mix It Up `Custom Intro` directly via API only when at least one asset resolves. |
| `redeem-capture.cs` | Contracted channel reward handler that deduplicates by redemption ID and writes pending records to `pending-intros`. |

## Action Contracts

Contracts for all Intros scripts live in [contracts.md](contracts.md). Load it when validating or updating a script contract.

## Runtime Notes

Current flow:

| Runtime event | Repo file | Result |
|---|---|---|
| Custom Intro channel-point redemption | `redeem-capture.cs` | Creates a `pending-intros` review record keyed by redemption ID. |
| `Twitch -> Chat -> First Words` | `first-chat-intro.cs` | Resolves enabled custom intro sound/gif assets for the viewer and dispatches Mix It Up `Custom Intro` directly when at least one asset is usable. |

Info-service collections: `pending-intros` for submitted requests awaiting operator review; `user-intros` for approved per-user intro records. The C# script resolves filename-only intro assets under [Assets/](../../Assets/) and calls Mix It Up `Custom Intro` directly only when at least one local asset resolved, sending resolved paths in `SpecialIdentifiers.intro_sound_file_path` and `SpecialIdentifiers.intro_gif_file_path`. The old `Intros - Play Custom Intro` Streamer.bot wrapper is retired and should stay out of the active runtime path; Mix It Up playback conventions live in [Tools/MixItUp/AGENTS.md](../../Tools/MixItUp/AGENTS.md).

## Validation / Boundaries / Handoff

Follow [Actions/AGENTS.md](../AGENTS.md) for shared boundaries, contract validation, sync, paste targets, and terminal handoff. Do not implement app-side info-service changes here, duplicate Mix It Up tooling docs, change public reward/policy copy without `brand-steward`, or expose private info-service notes in chat/overlay output.
