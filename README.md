# SharkStreamerBot

This repo stores:
- **Streamer.bot C# action scripts** in `Actions/`
- **standalone TypeScript apps** in `Apps/` (stream overlay, info service, production manager)
- **local/external tooling** in `Tools/`
- **creative scaffolding** in `Creative/`
- **agent knowledge tree** in `.agents/` (roles, workflows, shared context, manifest routing)

> Important: scripts are not auto-deployed. After edits, copy/paste each changed script into its matching Streamer.bot action.

## Agent Entry Point

**All agents start here:** `.agents/ENTRY.md`

It identifies the available roles, how to navigate context on demand, and where shared project knowledge lives. Read it before any task.

## Top-level routing

- `Actions/` — Streamer.bot action source. This remains the runtime source area.
- `Apps/` — standalone TypeScript apps: `stream-overlay/` (Phaser OBS browser source + WebSocket broker), `info-service/` (file-backed REST API for per-viewer data, port 8766), `production-manager/` (React admin UI, port 5174 dev / 4174 preview).
- `Tools/` — Mix It Up utilities, Streamer.bot support tooling, overlays, validators, and related local scripts.
- `Creative/` — art-generation, world-building, marketing, and other reusable creative scaffolding.
- `.agents/` — shared agent knowledge tree (roles, workflows, shared context, manifest routing). Agent-agnostic — used by all agents.

## Documentation Standard

All script docs use the same template sections:
- Purpose
- Expected Trigger / Input
- Required Runtime Variables
- Key Outputs / Side Effects
- Mix It Up Actions
- OBS Interactions
- Wait Behavior
- Chat / Log Output
- Operator Notes

## Docs Index

### Apps
- `Apps/stream-overlay/README.md`

### Squad
- `Actions/Squad/AGENTS.md`
- `Actions/Squad/Clone/README.md`
- `Actions/Squad/Duck/README.md`
- `Actions/Squad/Pedro/README.md`
- `Actions/Squad/Toothless/README.md`

### Architecture / Routing
- `AGENTS.md` — root agent entry and quick role routing
- `.agents/ENTRY.md` — manifest-backed agent entrypoint
- `.agents/_shared/project.md` — repo-wide context, domains, priorities
- `.agents/_shared/conventions.md` — file routing and naming conventions
- `Tools/README.md`
- `Tools/MixItUp/README.md`
- `Tools/MixItUp/Api/README.md`
- `Tools/StreamerBot/README.md`
- `Creative/README.md`
- `Creative/Art/README.md`
- `Creative/WorldBuilding/README.md`
- `Creative/Marketing/README.md`

### Reusable Patterns
- `Actions/HELPER-SNIPPETS.md`

### Commanders
- `Actions/Commanders/AGENTS.md`
- `Actions/Commanders/Captain Stretch/README.md`
- `Actions/Commanders/The Director/README.md`
- `Actions/Commanders/Water Wizard/README.md`

### Twitch Core Integrations
- `Actions/Twitch Core Integrations/AGENTS.md`

### Voice Commands
- `Actions/Voice Commands/AGENTS.md`

### Twitch Channel Points
- `Actions/Twitch Channel Points/AGENTS.md`

### Twitch Hype Train
- `Actions/Twitch Hype Train/AGENTS.md`

### Twitch Bits Integrations
- `Actions/Twitch Bits Integrations/AGENTS.md`

## Mini-game Contribution Contract (All Features)
Any new mini-game (inside or outside `Actions/Squad/`) must follow this contract:
1. Acquire global lock at mini-game start:
   - `minigame_active = true`
   - `minigame_name = <game_name>`
2. If lock is owned by another game, do not start and send a clear blocked message.
3. Release lock on **every** terminal path:
   - win, loss, timeout, cancel, manual stop, resolve guard path.
4. For single-action mini-games, release lock in a `finally` block.
5. Keep lock vars/reset behavior synchronized with:
   - `Actions/Twitch Core Integrations/stream-start.cs`
   - `Actions/HELPER-SNIPPETS.md`

## Commander Model Reminder
- Three commander slots exist (Captain Stretch, The Director, Water Wizard).
- All three can be active at the same time.
- Each commander can have separate command scripts in its folder.

## Operator Sync Reminder
1. Edit scripts in this repo.
2. Copy/paste each changed script into Streamer.bot.
3. Run one happy-path and one edge-case test.
4. Confirm chat/log/OBS behavior is correct.
