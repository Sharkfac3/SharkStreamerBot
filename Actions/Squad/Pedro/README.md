# Pedro Script Reference

## Script: `pedro-main.cs`

### Purpose
Handles all Pedro chat logic in one action:
- Secret Mix It Up trigger phrase (no Pedro state changes).
- `!pedro` start command for mini-game.
- Mention counting until unlock.

### Expected Trigger / Input
- One Streamer.bot action with chat triggers for:
  - Message starts with `!pedro`, and/or
  - Message contains `pedro`.
- Reads `message` (fallback `rawInput`).
- Optionally reads `msgId` to prevent double-processing the same chat line.

### Required Runtime Variables
- Reads/writes `pedro_game_enabled` (bool).
- Reads/writes `pedro_mention_count` (int).
- Reads/writes `pedro_unlocked` (bool).
- Reads/writes `pedro_last_message_id` (string duplicate guard).

### Key Outputs / Side Effects
- `!pedro` enables Pedro mention mini-game.
- Counts occurrences of `pedro` while game is enabled.
- Unlocks Pedro at `100` mentions.
- Secret command `!pedro x500liVePedro` triggers only the Mix It Up command and does not unlock Pedro or enable the mini-game.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID constant in script:
  - `MIXITUP_PEDRO_UNLOCK_COMMAND_ID` (currently placeholder)
- Payload `Arguments`: empty string (`""`)
- Triggered on unlock and on secret command.

### OBS Interactions
- On unlock, shows source `Pedro - Dancing` on `Disco Party: Workspace`.

### Wait Behavior
- None.

### Chat / Log Output
- Announces mini-game start.
- Announces progress when `!pedro` is used again during active game.
- Announces unlock completion.
- Logs warning if Mix It Up command ID is not configured.

### Operator Notes
- Replace `MIXITUP_PEDRO_UNLOCK_COMMAND_ID` when the real ID is available.
- Keep OBS scene/source names synced exactly: `Disco Party: Workspace` + `Pedro - Dancing`.
- Suggested reset at stream start:
  - `pedro_game_enabled = false`
  - `pedro_mention_count = 0`
  - `pedro_unlocked = false`
  - `pedro_last_message_id = ""`
