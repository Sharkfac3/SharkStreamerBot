# SharkStreamerBot

This repo stores:
- **Streamer.bot C# action scripts** in `Actions/`
- **local/external tooling** in `Tools/`
- **creative scaffolding** in `Creative/`
- **architecture/workflow docs** in `Docs/`

> Important: scripts are not auto-deployed. After edits, copy/paste each changed script into its matching Streamer.bot action.

## Top-level routing

- `Actions/` — Streamer.bot action source. This remains the runtime source area.
- `Tools/` — Mix It Up utilities, Streamer.bot support tooling, overlays, validators, and related local scripts.
- `Creative/` — art-generation, world-building, marketing, and other reusable creative scaffolding.
- `Docs/` — architecture and workflow documentation.

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

### Squad
- `Actions/Squad/README.md`
- `Actions/Squad/Clone/README.md`
- `Actions/Squad/Duck/README.md`
- `Actions/Squad/Pedro/README.md`
- `Actions/Squad/Toothless/README.md`

### Architecture / Routing
- `Docs/Architecture/repo-structure.md`
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
- `Actions/Commanders/README.md`
- `Actions/Commanders/Captain Stretch/README.md`
- `Actions/Commanders/The Director/README.md`
- `Actions/Commanders/Water Wizard/README.md`

### Twitch Core Integrations
- `Actions/Twitch Core Integrations/README.md`

### Voice Commands
- `Actions/Voice Commands/README.md`

### Twitch Channel Points
- `Actions/Twitch Channel Points/README.md`

### Twitch Hype Train
- `Actions/Twitch Hype Train/README.md`

### Twitch Bits Integrations
- `Actions/Twitch Bits Integrations/README.md`

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
