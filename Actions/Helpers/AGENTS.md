---
id: actions-helpers
type: domain-route
description: Routing guide for reusable Streamer.bot C# helper snippet references.
owner: streamerbot-dev
secondaryOwners:
  - ops
workflows:
  - change-summary
  - sync
  - validation
status: active
path: Actions/Helpers/
---

# Actions/Helpers — Agent Guide

This folder contains reusable Streamer.bot inline C# helper snippets split by concept.

- Keep helper files documentation-only and copy/paste-ready.
- Do not assume Streamer.bot actions can import files from this folder at runtime.

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
| [overlay-broker.md](overlay-broker.md) | Stream-overlay broker WebSocket connect/register helper and ClientHello pattern. |
| [timers.md](timers.md) | Timer enable/disable/reset/interval-update patterns. |
| [triggers/](triggers/README.md) | Canonical Streamer.bot trigger catalog — args, version, caveats — mirrored from upstream nav 1:1. |

## Note

Per-feature `Trigger Variables` blocks are being migrated to per-script `Args Consumed` tables; canonical args live in `triggers/`.
