---
id: actions-agent-guide
type: domain-guide
description: Streamer.bot Actions domain entrypoint and progressive discovery guide.
status: active
owner: streamerbot-dev
---

# Actions — Streamer.bot Runtime Guide

## Purpose

`Actions/` contains the source-controlled C# scripts and runtime notes for Streamer.bot actions. These scripts are edited in this repository, then manually pasted or synced into Streamer.bot by the operator.

Treat this folder as the live-stream runtime layer: Twitch events, chat commands, channel point rewards, OBS control, timers, Streamer.bot globals, Mix It Up bridges, and Streamer.bot-to-app integration points.

## Start Here

When working anywhere under `Actions/`:

1. Follow the repo coordination workflow for active edits and file conflicts.
2. Read this file.
3. Read [Actions/SHARED-CONSTANTS.md](SHARED-CONSTANTS.md) for canonical global var, timer, OBS source, and broker topic names.
4. Read [Actions/Helpers/AGENTS.md](Helpers/AGENTS.md) for reusable Streamer.bot C# patterns.
5. Read the local folder `AGENTS.md` for the action group you will edit.
6. After changing C# scripts, include Streamer.bot paste targets and validation/smoke-test notes in your handoff.

**Trigger arg lookup order** (when wiring or editing any script):

1. **Catalog** — `Actions/Helpers/triggers/<platform>/<subcategory>.md#<trigger>`. Canonical upstream args.
2. **Script docs** — local `AGENTS.md` and the feature README's `## Args Consumed` table for the `.cs` file.
3. **Upstream** — https://docs.streamer.bot/api/triggers (last resort; if the catalog is wrong, fix the catalog first).

## Shared Required Reading

Every session working under Actions/ must read:
- `Actions/SHARED-CONSTANTS.md` — canonical global var, timer, OBS source, and broker topic names
- `Actions/Helpers/AGENTS.md` — reusable Streamer.bot C# patterns

Local AGENTS.md files list additional required reading specific to that folder only.

## Rules

Domain rules and universal script rules for all work under `Actions/` live in [RULES.md](RULES.md). Read it before making any changes.

## Ownership

Domain-level ownership rules and role matrix live in [OWNERSHIP.md](OWNERSHIP.md). Per-folder ownership is in each folder's AGENTS.md.

## Contract Schema and Validation

Contract format specification, field definitions, and validation instructions live in [CONTRACT-SCHEMA.md](CONTRACT-SCHEMA.md). Load it when writing or validating a script contract.

## Folder Routing

| Path | Use for | Local guide |
|---|---|---|
| `Twitch Core Integrations/` | Stream start/reset, follows, subs, gift subs, watch streaks, core Twitch event bridges. | `Actions/Twitch Core Integrations/AGENTS.md` |
| `Twitch Channel Points/` | Channel point reward effects such as Disco Party and task explanation rewards. | `Actions/Twitch Channel Points/AGENTS.md` |
| `Twitch Bits Integrations/` | Cheer/bits tiers, on-screen celebration, emote/message effects. | `Actions/Twitch Bits Integrations/AGENTS.md` |
| `Twitch Hype Train/` | Hype train start, level-up, and end handlers. | `Actions/Twitch Hype Train/AGENTS.md` |
| `Voice Commands/` | Voice-triggered stream mode and OBS scene helpers. | `Actions/Voice Commands/AGENTS.md` |
| `Commanders/` | Commander role redeems, commander-only commands, and support commands. | `Actions/Commanders/AGENTS.md` |
| `Squad/` | Squad mini-games and interactions: Duck, Clone, Pedro, Toothless, offerings, game help. | `Actions/Squad/AGENTS.md` |
| `LotAT/` | Legend of the Ancients runtime, story-session flow, voting, dice, commander windows, overlay publishing. | `Actions/LotAT/AGENTS.md` |
| `Overlay/` | Streamer.bot WebSocket bridge to the custom stream overlay broker. | `Actions/Overlay/AGENTS.md` |
| `Intros/` | User intro capture and first-chat intro runtime integrations. | `Actions/Intros/AGENTS.md` |
| `Rest Focus Loop/` | Rest/focus phase timers and commander hooks into focus/rest behavior. | `Actions/Rest Focus Loop/AGENTS.md` |
| `Destroyer/` | Destroyer overlay spawn/move chat interaction. | `Actions/Destroyer/AGENTS.md` |
| `XJ Drivethrough/` | XJ drivethrough overlay/audio effect. | `Actions/XJ Drivethrough/AGENTS.md` |
| `Temporary/` | Short-lived or provisional Streamer.bot actions. | `Actions/Temporary/AGENTS.md` |
| `Helpers/` | Reusable implementation guidance for Streamer.bot C# patterns. | `Actions/Helpers/AGENTS.md` |

## Runtime Integration Map

Streamer.bot is used here as the live orchestration layer:

- **Twitch:** receives event triggers and chat/command input.
- **Streamer.bot globals:** stores session state, unlock state, mini-game locks, commander slots, and integration latches.
- **Streamer.bot timers:** drives join windows, decision windows, game ticks, rest/focus phases, and timeout handlers.
- **OBS:** switches scenes and toggles sources for stream modes, Disco Party, Squad reveals, and commander controls.
- **Mix It Up:** receives local HTTP command calls for alerts/effects that still live in Mix It Up.
- **Overlay broker:** receives WebSocket messages from Streamer.bot and forwards them to the TypeScript overlay app.
- **Info service:** receives local HTTP requests for file-backed assets and production-manager workflows.

## Shared References

| File | Purpose |
|---|---|
| `Actions/SHARED-CONSTANTS.md` | Canonical names for globals, timers, OBS scenes/sources, overlay topics, service URLs, and operator sync notes. |
| `Actions/Helpers/AGENTS.md` | Topic-based helper index for chat input, JSON, timers, OBS, Mix It Up, and mini-game contracts. |
| `Actions/Helpers/cph-api-signatures.md` | Streamer.bot `CPH` method signatures and usage notes. |
| `Actions/Helpers/mini-game-contract.md` | Required contract for Squad-style mini-games and shared lock behavior. |
| `Actions/Helpers/mixitup-command-api.md` | Mix It Up command API payload and call patterns. |
| `Actions/Helpers/json-no-external-libraries.md` | JSON handling patterns that avoid external dependencies. |


