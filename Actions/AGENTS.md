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

1. Check `WORKING.md` for active edits and file conflicts.
2. Read this file.
3. Read `Actions/SHARED-CONSTANTS.md` for canonical global var, timer, OBS source, and broker topic names.
4. Read `Actions/HELPER-SNIPPETS.md` and/or `Actions/Helpers/README.md` for reusable Streamer.bot C# patterns.
5. Read the local folder `AGENTS.md` for the action group you will edit.
6. After changing C# scripts, include Streamer.bot paste targets and validation/smoke-test notes in your handoff.

## Domain Rules

- Keep `Actions/` focused on Streamer.bot runtime scripts and action-group docs.
- Scripts should remain pasteable into Streamer.bot `Execute C# Code` actions.
- Use Streamer.bot's `CPHInline` style unless a local guide explicitly says otherwise.
- Do not add external NuGet/package dependencies to runtime scripts.
- Preserve existing chat command names, global keys, timer names, OBS source names, and Mix It Up command IDs unless the operator explicitly asks for a migration.
- Check `Actions/SHARED-CONSTANTS.md` before adding or renaming shared values.
- Be explicit about persisted vs. non-persisted globals when using `CPH.SetGlobalVar`.
- Prefer small, local changes over broad refactors; live stream reliability comes first.
- Avoid duplicate helper implementations when a pattern already exists in `Actions/Helpers/`.

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
| `Actions/HELPER-SNIPPETS.md` | Compatibility index for reusable Streamer.bot implementation snippets. |
| `Actions/Helpers/README.md` | Topic-based helper index for chat input, JSON, timers, OBS, Mix It Up, and mini-game contracts. |
| `Actions/Helpers/cph-api-signatures.md` | Streamer.bot `CPH` method signatures and usage notes. |
| `Actions/Helpers/mini-game-contract.md` | Required contract for Squad-style mini-games and shared lock behavior. |
| `Actions/Helpers/mixitup-command-api.md` | Mix It Up command API payload and call patterns. |
| `Actions/Helpers/json-no-external-libraries.md` | JSON handling patterns that avoid external dependencies. |

## Sync and Handoff Expectations

For changed C# files, include in your final summary:

- Streamer.bot action name or likely paste target.
- Trigger expectations, if changed or newly added.
- Globals, timers, OBS sources, broker topics, or Mix It Up command IDs touched.
- Validation performed, such as script review, local grep checks, or smoke-test recommendations.
- Any required operator setup in Streamer.bot, OBS, Mix It Up, or local apps.

If a change affects shared names, update `Actions/SHARED-CONSTANTS.md` and all listed consumers before handoff.

## Boundaries

- App implementation belongs in `Apps/`, not `Actions/`.
- Mix It Up export/import tooling belongs in `Tools/MixItUp/`, not `Actions/`.
- Streamer.bot support tooling belongs in `Tools/StreamerBot/`, not `Actions/`.
- Brand/canon/story/art content belongs in `Creative/` unless it is embedded runtime copy for an action.
- Repo-wide workflow and architecture docs belong in `.agents/`.
