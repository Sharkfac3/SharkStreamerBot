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
- Uses the standardized bits readout helper/payload shape (`Platform`, `Arguments`, empty `SpecialIdentifiers`, `IgnoreRequirements`).
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `cdca2818-49c3-43e0-ab2f-cfdce55ac16c`
- Payload `Arguments`: full cheer text from `messageStripped` when available (no word cap)

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
- Uses the standardized bits readout helper/payload shape (`Platform`, `Arguments`, empty `SpecialIdentifiers`, `IgnoreRequirements`).
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `aa960b7a-7e9e-42e1-be65-fd4f4b3ca6b0`
- Payload `Arguments`: cheer text from `messageStripped` when available, capped to first 250 words

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
- Uses the standardized bits readout helper/payload shape (`Platform`, `Arguments`, empty `SpecialIdentifiers`, `IgnoreRequirements`).
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `6d05a5c6-6cd9-471b-9358-18dde2df4318`
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
- Uses the standardized bits readout helper/payload shape (`Platform`, `Arguments`, empty `SpecialIdentifiers`, `IgnoreRequirements`).
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `35405bfe-660f-46f2-bec6-8a1da9ec1af2`
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
- Command ID: `REPLACE_WITH_GIGANTIFY_EMOTE_COMMAND_ID` *(placeholder; must be replaced)*
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
- Replace the placeholder command ID after the matching Mix It Up action group is created and appears in the saved Mix It Up command export.
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
- Uses the same standardized bits readout helper/payload shape as the cheer-tier scripts.
- Waits after a successful Mix It Up call so TTS/message-effect readouts are less likely to overlap.
- Logs a warning if none of those args are populated.
- Logs which fallback arg was used when the text does not come from `userInput`.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_MESSAGE_EFFECTS_COMMAND_ID` *(placeholder; must be replaced)*
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = "<userInput text>"`
  - `SpecialIdentifiers = { }`
  - `IgnoreRequirements = false`

### OBS Interactions
- None.

### Wait Behavior
- Wait after successful Mix It Up call: `3000ms + 400ms per word + 500ms buffer`.

### Chat / Log Output
- No chat output.
- Logs warning/error messages when the Mix It Up call fails or when `userInput` is blank.

### Operator Notes
- Replace the placeholder command ID after the matching Mix It Up action group is created and appears in the saved Mix It Up command export.
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
- Command ID: `REPLACE_WITH_ON_SCREEN_CELEBRATION_COMMAND_ID` *(placeholder; must be replaced)*
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
- Replace the placeholder command ID after the matching Mix It Up action group is created and appears in the saved Mix It Up command export.
- Wire this script to `Twitch -> Channel Reward -> Automatic Reward Redemption`.
- Filter the action so it only runs for the `on screen celebration` automatic reward.
