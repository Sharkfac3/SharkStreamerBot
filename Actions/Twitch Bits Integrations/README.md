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
- Uses the standardized bits readout helper/payload shape (`Platform`, `Arguments`, populated `SpecialIdentifiers`, `IgnoreRequirements`).
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `cdca2818-49c3-43e0-ab2f-cfdce55ac16c`
- Payload `Arguments`: full cheer text from `messageStripped` when available (no word cap)
- Payload `SpecialIdentifiers`:
  - `bitsuser` — `user` arg, or empty string when missing
  - `bitsuserid` — `userId` arg, or empty string when missing
  - `bitsamount` — `bits` arg as string, or `0` when missing/non-numeric
  - `bitstier` — `1`
  - `bitstype` — `tier1`
  - `bitsmessage` — final sanitized/capped message sent in `Arguments`
  - `bitsmessagetype` — `message` when the final message is non-empty, otherwise `none`
  - `bitswordcount` — final message word count as string
  - `bitscap` — `none`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = "<final cheer text>"`
  - `SpecialIdentifiers = { bitsuser = "...", bitsuserid = "...", bitsamount = "...", bitstier = "1", bitstype = "tier1", bitsmessage = "...", bitsmessagetype = "message|none", bitswordcount = "...", bitscap = "none" }`
  - `IgnoreRequirements = false`

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
- Uses the standardized bits readout helper/payload shape (`Platform`, `Arguments`, populated `SpecialIdentifiers`, `IgnoreRequirements`).
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `aa960b7a-7e9e-42e1-be65-fd4f4b3ca6b0`
- Payload `Arguments`: cheer text from `messageStripped` when available, capped to first 250 words
- Payload `SpecialIdentifiers`:
  - `bitsuser` — `user` arg, or empty string when missing
  - `bitsuserid` — `userId` arg, or empty string when missing
  - `bitsamount` — `bits` arg as string, or `0` when missing/non-numeric
  - `bitstier` — `2`
  - `bitstype` — `tier2`
  - `bitsmessage` — final sanitized/capped message sent in `Arguments`
  - `bitsmessagetype` — `message` when the final message is non-empty, otherwise `none`
  - `bitswordcount` — final message word count as string
  - `bitscap` — `250 words`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = "<final cheer text capped to 250 words>"`
  - `SpecialIdentifiers = { bitsuser = "...", bitsuserid = "...", bitsamount = "...", bitstier = "2", bitstype = "tier2", bitsmessage = "...", bitsmessagetype = "message|none", bitswordcount = "...", bitscap = "250 words" }`
  - `IgnoreRequirements = false`

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
- Uses the standardized bits readout helper/payload shape (`Platform`, `Arguments`, populated `SpecialIdentifiers`, `IgnoreRequirements`).
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `6d05a5c6-6cd9-471b-9358-18dde2df4318`
- Payload `Arguments`: cheer text from `messageStripped` when available, capped to first 100 words
- Payload `SpecialIdentifiers`:
  - `bitsuser` — `user` arg, or empty string when missing
  - `bitsuserid` — `userId` arg, or empty string when missing
  - `bitsamount` — `bits` arg as string, or `0` when missing/non-numeric
  - `bitstier` — `3`
  - `bitstype` — `tier3`
  - `bitsmessage` — final sanitized/capped message sent in `Arguments`
  - `bitsmessagetype` — `message` when the final message is non-empty, otherwise `none`
  - `bitswordcount` — final message word count as string
  - `bitscap` — `100 words`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = "<final cheer text capped to 100 words>"`
  - `SpecialIdentifiers = { bitsuser = "...", bitsuserid = "...", bitsamount = "...", bitstier = "3", bitstype = "tier3", bitsmessage = "...", bitsmessagetype = "message|none", bitswordcount = "...", bitscap = "100 words" }`
  - `IgnoreRequirements = false`

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
- Uses the standardized bits readout helper/payload shape (`Platform`, `Arguments`, populated `SpecialIdentifiers`, `IgnoreRequirements`).
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `35405bfe-660f-46f2-bec6-8a1da9ec1af2`
- Payload `Arguments`: cheer text from `messageStripped` when available, capped to first 10 words
- Payload `SpecialIdentifiers`:
  - `bitsuser` — `user` arg, or empty string when missing
  - `bitsuserid` — `userId` arg, or empty string when missing
  - `bitsamount` — `bits` arg as string, or `0` when missing/non-numeric
  - `bitstier` — `4`
  - `bitstype` — `tier4`
  - `bitsmessage` — final sanitized/capped message sent in `Arguments`
  - `bitsmessagetype` — `message` when the final message is non-empty, otherwise `none`
  - `bitswordcount` — final message word count as string
  - `bitscap` — `10 words`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = "<final cheer text capped to 10 words>"`
  - `SpecialIdentifiers = { bitsuser = "...", bitsuserid = "...", bitsamount = "...", bitstier = "4", bitstype = "tier4", bitsmessage = "...", bitsmessagetype = "message|none", bitswordcount = "...", bitscap = "10 words" }`
  - `IgnoreRequirements = false`

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
- Sends populated `SpecialIdentifiers` for Mix It Up branching/text fields.
- Uses the same standardized bits readout helper/payload shape as the cheer-tier scripts.
- Waits after a successful Mix It Up call so TTS/message-effect readouts are less likely to overlap.
- Logs a warning if none of those args are populated.
- Logs which fallback arg was used when the text does not come from `userInput`.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_MESSAGE_EFFECTS_COMMAND_ID` *(placeholder; must be replaced)*
- Payload `Arguments`: first non-empty trimmed text from `userInput`, `input0`, `message`, or `rawInput`
- Payload `SpecialIdentifiers`:
  - `messageeffectsuser` — `user` arg, or empty string when missing
  - `messageeffectsuserid` — `userId` arg, or empty string when missing
  - `messageeffectsmessage` — final message text sent in `Arguments`
  - `messageeffectstype` — `message` when final text is non-empty, otherwise `none`
  - `messageeffectssourcearg` — arg name used for the final text (`userInput`, `input0`, `message`, `rawInput`, or empty string)
  - `messageeffectswordcount` — final message word count as string
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = "<final user text>"`
  - `SpecialIdentifiers = { messageeffectsuser = "...", messageeffectsuserid = "...", messageeffectsmessage = "...", messageeffectstype = "message|none", messageeffectssourcearg = "userInput|input0|message|rawInput|", messageeffectswordcount = "..." }`
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
Handles the `on screen celebration` automatic reward redemption by calling a Mix It Up command with no `Arguments` text and populated celebration metadata identifiers.

### Expected Trigger / Input
- Streamer.bot trigger: `Twitch -> Channel Reward -> Automatic Reward Redemption`.
- Streamer.bot may provide optional message text under `userInput`, `input0`, `message`, or `rawInput` depending on trigger wiring/version.

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Calls Mix It Up Run Command API for the on-screen celebration flow.
- Sends an empty `Arguments` value for compatibility with the current Mix It Up command.
- Sends populated `SpecialIdentifiers` for Mix It Up branching/text fields.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_ON_SCREEN_CELEBRATION_COMMAND_ID` *(placeholder; must be replaced)*
- Payload `Arguments`: empty string (`""`)
- Payload `SpecialIdentifiers`:
  - `celebrationuser` — `user` arg, or empty string when missing
  - `celebrationuserid` — `userId` arg, or empty string when missing
  - `celebrationtype` — `onscreencelebration`
  - `celebrationrewardid` — first non-empty value from `reward` or `rewardId`, or empty string when missing
  - `celebrationrewardname` — first non-empty value from `rewardName` or `rewardTitle`, or empty string when missing
  - `celebrationmessage` — first non-empty value from `userInput`, `input0`, `message`, or `rawInput`, or empty string when missing
  - `celebrationmessagetype` — `message` when `celebrationmessage` is non-empty, otherwise `none`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers = { celebrationuser = "...", celebrationuserid = "...", celebrationtype = "onscreencelebration", celebrationrewardid = "...", celebrationrewardname = "...", celebrationmessage = "...", celebrationmessagetype = "message|none" }`
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
