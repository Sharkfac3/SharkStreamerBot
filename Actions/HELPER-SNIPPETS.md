---
id: actions-helper-snippets
type: reference
description: Compatibility index for reusable Streamer.bot C# helper snippets now split under Actions/Helpers.
owner: streamerbot-dev
secondaryOwners:
  - ops
status: active
---

# Helper Snippets — Compatibility Index

Reusable Streamer.bot C# snippets have been reorganized by concept under [Actions/Helpers/](Helpers/). This file is retained as a thin compatibility index for legacy references, prompts, and bookmarks.

Use [Actions/Helpers/README.md](Helpers/README.md) as the authoritative helper index for new work.

## Section mapping

| Former section in this file | New helper file |
|---|---|
| `## 1) Mini-game Lock Helper (Global, cross-feature)` | [Actions/Helpers/mini-game-lock.md](Helpers/mini-game-lock.md) |
| `## 2) Mix It Up Command API Helper` | [Actions/Helpers/mixitup-command-api.md](Helpers/mixitup-command-api.md) |
| `## 3) Chat Message Input Helper (message/rawInput)` | [Actions/Helpers/chat-input.md](Helpers/chat-input.md) |
| `## 4) OBS Scene Switching` | [Actions/Helpers/obs-scenes.md](Helpers/obs-scenes.md) |
| `## 5) Verified CPH API Method Signatures` | [Actions/Helpers/cph-api-signatures.md](Helpers/cph-api-signatures.md) |
| `## 6) Timer Management` | [Actions/Helpers/timers.md](Helpers/timers.md) |
| `## 7) JSON Parse / Serialize Helper (No External Libraries)` | [Actions/Helpers/json-no-external-libraries.md](Helpers/json-no-external-libraries.md) |
| Overlay broker WebSocket connect/register helper | [Actions/Helpers/overlay-broker.md](Helpers/overlay-broker.md) |
| `## Required mini-game contract checklist` | [Actions/Helpers/mini-game-contract.md](Helpers/mini-game-contract.md) |

## Notes for agents

- Broad references may continue to point here when they mean “the helper snippet index.”
- Concept-specific guidance should link directly to the matching file under [Actions/Helpers/](Helpers/).
- Streamer.bot actions cannot import these files at runtime; copy the needed snippets into each inline C# action.
