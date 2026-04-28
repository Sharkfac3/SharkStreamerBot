---
id: trigger-catalog-phase-04-core-content
type: project-phase
description: Fill every Streamer.bot Core subcategory catalog file with args, version notes, caveats, and used-in-repo backlinks.
status: active
owner: streamerbot-dev
phase: 4
depends_on:
  - 02-wiring.md
---

# Phase 4 — Core Content Fill

## Goal

Apply the Phase 3 content-fill procedure to every Streamer.bot **Core** trigger.

## Procedure

Use the procedure from [03-twitch-content.md](03-twitch-content.md) verbatim. Differences:

- Inputs: `Actions/Helpers/triggers/core/*.md` (skeleton).
- Manifest: Core platform entries.
- Used-in-repo backfill scope: especially `Actions/Voice Commands/`, `Actions/Rest Focus Loop/`, `Actions/Temporary/`, `Actions/Helpers/timers.md` cross-refs, and any script whose Streamer.bot trigger is a Command, Timer, Hotkey, or System event.

## Core subcategories (from manifest)

Uncategorized, Commands, File Folder Watcher, File I/O, Global Variables, Groups, Inputs, MIDI, Processes, Quotes, System, Voice Control, WebSocket.

## Core-specific notes

- **Commands** — heavily used in this repo (every `! ` chat command wires here). Backfill thoroughly; many `.cs` scripts under `Actions/Commanders/`, `Actions/Squad/`, `Actions/Twitch Channel Points/` are command-triggered. Include the implicit shared args: `command`, `message`, `user`, `userId`, `rawInput`, `input0..N`.
- **Voice Control** — `Actions/Voice Commands/*.cs` consume these. Confirm the upstream arg names match what those scripts expect.
- **System** — actions like Streamer.bot start/stop, action queue, etc. Some `.cs` may listen.
- **WebSocket** — overlay broker scripts (`Actions/Overlay/broker-*.cs`) likely use WebSocket connection events. Backfill carefully.
- **Inputs / MIDI / File I/O / File Folder Watcher / Quotes / Processes** — likely `_Not yet wired._` for the repo's current scope. Document fully anyway; future agents may wire.

## Validation

Per Phase 3 validation checklist, scoped to `Actions/Helpers/triggers/core/`.

## Exit

Dirty tree. Change summary lists Core trigger count + which Core triggers had repo consumers.

## Next phase

Independent of other content phases — proceed to whichever platform prompt is next on your list, or run multiple in parallel sessions.
