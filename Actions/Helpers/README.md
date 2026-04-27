---
id: actions-helpers
type: domain-route
description: Index for reusable Streamer.bot C# helper snippets under Actions/Helpers.
owner: streamerbot-dev
secondaryOwners:
  - ops
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Actions/Helpers

## Purpose

Folder index for reusable Streamer.bot C# helper snippets. These files contain copy/paste patterns for Streamer.bot inline C# actions and related runtime documentation.

## Helper files

| Helper | Description |
|---|---|
| [chat-input.md](chat-input.md) | Defensive chat text, duplicate message, and sender argument helpers. |
| [cph-api-signatures.md](cph-api-signatures.md) | Verified CPH method signatures and policy for unverified calls. |
| [json-no-external-libraries.md](json-no-external-libraries.md) | No-external-library JSON parse/serialize helper for Streamer.bot inline C#. |
| [mini-game-contract.md](mini-game-contract.md) | Checklist for mini-game lock usage, terminal paths, and docs updates. |
| [mini-game-lock.md](mini-game-lock.md) | Global mini-game lock constants and acquire/release snippets. |
| [mixitup-command-api.md](mixitup-command-api.md) | Streamer.bot C# helper for Mix It Up command API calls and unlock pacing. |
| [obs-scenes.md](obs-scenes.md) | OBS scene switching helper and direct-call guidance. |
| [timers.md](timers.md) | Timer enable/disable/reset/interval-update patterns. |

## Compatibility note

[Actions/HELPER-SNIPPETS.md](../HELPER-SNIPPETS.md) is retained as a thin compatibility index for legacy references. New work should use the concept-specific files above.
