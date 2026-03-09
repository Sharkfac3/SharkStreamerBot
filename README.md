# SharkStreamerBot

This repo stores **Streamer.bot C# action scripts** and local **BuildTools** utilities.

> Important: scripts are not auto-deployed. After edits, copy/paste each changed script into its matching Streamer.bot action.

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

### Reusable Patterns
- `Actions/HELPER-SNIPPETS.md`

### Commanders
- `Actions/Commanders/README.md`
- `Actions/Commanders/Captain Stretch/README.md`
- `Actions/Commanders/The Director/README.md`
- `Actions/Commanders/Water Wizard/README.md`

### Twitch Integration
- `Actions/Twitch Integration/README.md`
- `Actions/Twitch Integration/Bits/README.md`

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
   - `Actions/Twitch Integration/stream-start.cs`
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
