# Bits Script Reference

Shared constants reference: `Actions/SHARED-CONSTANTS.md`

## Script: `bits-tier-1.cs`

### Purpose
Forwards Tier 1 cheer text to Mix It Up with sanitization and TTS pacing wait.

### Expected Trigger / Input
- Tier 1 cheer trigger with message text (`message` fallback `rawInput`).

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Sanitizes cheer text and forwards it to Mix It Up.
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_TIER_1_COMMAND_ID` *(placeholder in script; not configured yet)*
- Payload `Arguments`: sanitized full cheer text (Cheer tokens removed, no word cap)

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
- Tier 2 cheer trigger with message text (`message` fallback `rawInput`).

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Sanitizes cheer text, caps to 250 words, forwards to Mix It Up.
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_TIER_2_COMMAND_ID` *(placeholder in script; not configured yet)*
- Payload `Arguments`: sanitized cheer text capped to first 250 words

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
- Tier 3 cheer trigger with message text (`message` fallback `rawInput`).

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Sanitizes cheer text, caps to 100 words, forwards to Mix It Up.
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `23f1afd1-7375-475d-afee-058ef4f7f68d`
- Payload `Arguments`: sanitized cheer text capped to first 100 words

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
- Tier 4 cheer trigger with message text (`message` fallback `rawInput`).

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Sanitizes cheer text, caps to 10 words, forwards to Mix It Up.
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_TIER_4_COMMAND_ID` *(placeholder in script; not configured yet)*
- Payload `Arguments`: sanitized cheer text capped to first 10 words

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
- Command ID: `replace-this-with-bit-gigantify-emote` *(placeholder; replace with the real ID when ready)*
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
- Replace `replace-this-with-bit-gigantify-emote` with the actual Mix It Up command ID when available.
- Wire this script to `Twitch -> Channel Reward -> Automatic Reward Redemption`.
- Filter the action so it only runs for the `gigantify emote` automatic reward.

---

## Script: `message-effects.cs`

### Purpose
Handles the `message effects` automatic reward redemption by forwarding the redeeming user's `userInput` text to a Mix It Up command.

### Expected Trigger / Input
- Streamer.bot trigger: `Twitch -> Channel Reward -> Automatic Reward Redemption`.
- Streamer.bot should provide:
  - `userInput`

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Calls Mix It Up Run Command API for the message effects flow.
- Sends the trimmed `userInput` value in `Arguments`.
- Sends an empty `SpecialIdentifiers` object for now.
- Logs a warning if `userInput` is missing or blank.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `replace-this-id-with-bits-message-effects` *(placeholder; replace with the real ID when available)*
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
- Replace `replace-this-id-with-bits-message-effects` with the actual Mix It Up command ID when available.
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
- Command ID: `replace-this-with-bits-on-screen-celebration` *(placeholder; replace with the real ID when available)*
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
- Replace `replace-this-with-bits-on-screen-celebration` with the actual Mix It Up command ID when available.
- Wire this script to `Twitch -> Channel Reward -> Automatic Reward Redemption`.
- Filter the action so it only runs for the `on screen celebration` automatic reward.
