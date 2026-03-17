# Bits Script Reference

Shared constants reference: `Actions/SHARED-CONSTANTS.md`

## Script: `bits-tier-1.cs`

### Purpose
Forwards Tier 1 cheer text to Mix It Up with sanitization and TTS pacing wait.

### Expected Trigger / Input
- Tier 1 cheer trigger with message text (`messageStripped` fallback `message` then `rawInput`).

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Forwards cheer text to Mix It Up.
- Prefers Streamer.bot's `messageStripped` value so cheer tokens are already removed.
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_TIER_1_COMMAND_ID` *(placeholder in script; not configured yet)*
- Payload `Arguments`: full cheer text from `messageStripped` when available (no word cap)

### OBS Interactions
- None.

### Wait Behavior
- Wait after successful Mix It Up call: `3000ms + 400ms per word + 500ms buffer`.

### Chat / Log Output
- Reads cheer text from trigger args.
- Sends no chat output.

### Operator Notes
- Replace placeholder command ID before production use.

---

## Script: `bits-tier-2.cs`

### Purpose
Forwards Tier 2 cheer text to Mix It Up with a 250-word cap.

### Expected Trigger / Input
- Tier 2 cheer trigger with message text (`messageStripped` fallback `message` then `rawInput`).

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Forwards cheer text, caps to 250 words, and sends it to Mix It Up.
- Prefers Streamer.bot's `messageStripped` value so cheer tokens are already removed.
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_TIER_2_COMMAND_ID` *(placeholder in script; not configured yet)*
- Payload `Arguments`: cheer text from `messageStripped` when available, capped to first 250 words

### OBS Interactions
- None.

### Wait Behavior
- Wait after successful Mix It Up call: `3000ms + 400ms per word + 500ms buffer`.

### Chat / Log Output
- Reads cheer text from trigger args.
- Sends no chat output.

### Operator Notes
- Replace placeholder command ID before production use.

---

## Script: `bits-tier-3.cs`

### Purpose
Forwards Tier 3 cheer text to Mix It Up with a 100-word cap.

### Expected Trigger / Input
- Tier 3 cheer trigger with message text (`messageStripped` fallback `message` then `rawInput`).

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Forwards cheer text, caps to 100 words, and sends it to Mix It Up.
- Prefers Streamer.bot's `messageStripped` value so cheer tokens are already removed.
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `23f1afd1-7375-475d-afee-058ef4f7f68d`
- Payload `Arguments`: cheer text from `messageStripped` when available, capped to first 100 words

### OBS Interactions
- None.

### Wait Behavior
- Wait after successful Mix It Up call: `3000ms + 400ms per word + 500ms buffer`.

### Chat / Log Output
- Reads cheer text from trigger args.
- Sends no chat output.

### Operator Notes
- Current command ID is configured.

---

## Script: `bits-tier-4.cs`

### Purpose
Forwards Tier 4 cheer text to Mix It Up with a 10-word cap.

### Expected Trigger / Input
- Tier 4 cheer trigger with message text (`messageStripped` fallback `message` then `rawInput`).

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Forwards cheer text, caps to 10 words, and sends it to Mix It Up.
- Prefers Streamer.bot's `messageStripped` value so cheer tokens are already removed.
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `3a83b335-ae69-425c-b4d2-a52d1734a9f7`
- Payload `Arguments`: cheer text from `messageStripped` when available, capped to first 10 words

### OBS Interactions
- None.

### Wait Behavior
- Wait after successful Mix It Up call: `3000ms + 400ms per word + 500ms buffer`.

### Chat / Log Output
- Reads cheer text from trigger args.
- Sends no chat output.

### Operator Notes
- Current command ID is configured from the saved Mix It Up command export.

---

## Script: `gigantify-emote.cs`

### Purpose
Handles the `gigantify emote` automatic reward redemption by calling a Mix It Up command with a fixed message, fixed type, and the gigantified emote details when available.

### Expected Trigger / Input
- Streamer.bot trigger: `Twitch -> Channel Reward -> Automatic Reward Redemption`.
- For gigantify-style rewards, Streamer.bot may provide:
  - `gigantifiedEmoteId`
  - `gigantifiedEmoteName`
  - `gigantifiedEmoteUrl`

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Calls Mix It Up Run Command API for the gigantify emote flow.
- Sends a fixed message payload of `whos that emote?`.
- Sends `SpecialIdentifiers.type = normal`.
- Forwards `emoteId`, `emoteName`, and `emoteUrl` into `SpecialIdentifiers` when available from the trigger.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `29d47997-6075-412d-88a0-43619b59bcfd`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = "whos that emote?"`
  - `SpecialIdentifiers = { type = "normal", emoteId = "...", emoteName = "...", emoteUrl = "..." }`
  - `IgnoreRequirements = false`

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs warning/error messages when the Mix It Up call fails.

### Operator Notes
- Current command ID is configured from the saved Mix It Up command export.
- Wire this script to `Twitch -> Channel Reward -> Automatic Reward Redemption`.
- Filter the action so it only runs for the `gigantify emote` automatic reward.

---

## Script: `message-effects.cs`

### Purpose
Handles the `message effects` automatic reward redemption by forwarding the redeeming user's entered text to a Mix It Up command.

### Expected Trigger / Input
- Streamer.bot trigger: `Twitch -> Channel Reward -> Automatic Reward Redemption`.
- Streamer.bot may provide the entered text under one of these args, depending on trigger wiring/version:
  - `userInput`
  - `input0`
  - `message`
  - `rawInput`

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Calls Mix It Up Run Command API for the message effects flow.
- Sends the first non-empty value found in `userInput`, `input0`, `message`, or `rawInput` in `Arguments`.
- Sends an empty `SpecialIdentifiers` object for now.
- Logs a warning if none of those args are populated.
- Logs which fallback arg was used when the text does not come from `userInput`.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `28397ebb-7a68-4a52-b448-3044a811c008`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = "<userInput text>"`
  - `SpecialIdentifiers = { }`
  - `IgnoreRequirements = false`

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs warning/error messages when the Mix It Up call fails or when `userInput` is blank.

### Operator Notes
- Current command ID is configured from the saved Mix It Up command export.
- Wire this script to `Twitch -> Channel Reward -> Automatic Reward Redemption`.
- Filter the action so it only runs for the `message effects` automatic reward.

---

## Script: `on-screen-celebration.cs`

### Purpose
Handles the `on screen celebration` automatic reward redemption by calling a Mix It Up command with no message text and no special identifiers.

### Expected Trigger / Input
- Streamer.bot trigger: `Twitch -> Channel Reward -> Automatic Reward Redemption`.

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Calls Mix It Up Run Command API for the on-screen celebration flow.
- Sends an empty `Arguments` value.
- Sends an empty `SpecialIdentifiers` object.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `3b9123d2-8d22-40f0-ad9b-baf6530388ee`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers = { }`
  - `IgnoreRequirements = false`

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs warning/error messages when the Mix It Up call fails.

### Operator Notes
- Current command ID is configured from the saved Mix It Up command export.
- Wire this script to `Twitch -> Channel Reward -> Automatic Reward Redemption`.
- Filter the action so it only runs for the `on screen celebration` automatic reward.
