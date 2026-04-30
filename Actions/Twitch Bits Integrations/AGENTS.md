---
id: actions-twitch-bits-integrations
type: domain-route
description: Twitch bits and automatic reward Streamer.bot event bridges, Mix It Up payloads, pacing, and brand handoffs.
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Twitch Bits Integrations — Agent Guide

## Purpose

This folder owns Streamer.bot C# scripts for Twitch cheer/bits tiers and related automatic reward integrations that forward event data to Mix It Up. These scripts are bridge actions: they sanitize viewer-provided text, preserve the current Mix It Up payload contract, and keep audio/readout timing safe during live stream events.

This is part of the ratified Twitch target shape: one `streamerbot-dev` owner family with folder-local Twitch guides. Do not recreate flat Twitch wrapper skills; keep folder-specific runtime knowledge here.

## When to Activate

Use this guide when editing or reviewing files under [Actions/Twitch Bits Integrations/](./), including:

- [Actions/Twitch Bits Integrations/bits-tier-1.cs](bits-tier-1.cs)
- [Actions/Twitch Bits Integrations/bits-tier-2.cs](bits-tier-2.cs)
- [Actions/Twitch Bits Integrations/bits-tier-3.cs](bits-tier-3.cs)
- [Actions/Twitch Bits Integrations/bits-tier-4.cs](bits-tier-4.cs)
- [Actions/Twitch Bits Integrations/gigantify-emote.cs](gigantify-emote.cs)
- [Actions/Twitch Bits Integrations/message-effects.cs](message-effects.cs)
- [Actions/Twitch Bits Integrations/on-screen-celebration.cs](on-screen-celebration.cs)
- Script Reference or operator documentation in this folder

Activate `brand-steward` before changing public cheer/readout text, automatic reward wording, Mix It Up message copy, reward prompts, or any other viewer-facing event text.

## Primary Owner

`streamerbot-dev` owns the C# runtime behavior, Streamer.bot trigger compatibility, Mix It Up API payload shape, timing/wait behavior, and manual paste readiness for this folder.

## Secondary Owners / Chain To

- `brand-steward` — chain for public event/readout text, reward names/descriptions, overlay copy, or any text that viewers hear or see.
- `ops` — chain only for validation, paste/sync workflow, or agent-tree maintenance beyond this local guide.

## Required Reading

Read the following before editing scripts:

- [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md)
- [Actions/Helpers/AGENTS.md](../Helpers/AGENTS.md)
- [Actions/Twitch Core Integrations/AGENTS.md](../Twitch%20Core%20Integrations/AGENTS.md) when changes affect stream-start reset or shared Twitch event conventions
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) when public copy changes

## Local Workflow

1. Identify whether the change is a cheer tier script, an automatic reward script, or documentation-only.
2. Preserve the Streamer.bot trigger contract. Cheer-tier scripts are triggered by Twitch cheer events; automatic reward scripts are wired to `Twitch -> Channel Reward -> Automatic Reward Redemption` and filtered to the intended reward.
3. Keep Mix It Up payloads compatible:
   - `Platform = "Twitch"`
   - `Arguments` preserves the existing command behavior.
   - Structured metadata belongs in populated `SpecialIdentifiers` with lowercase, no-space keys.
   - `IgnoreRequirements = false` unless the operator explicitly requests otherwise.
4. Read Streamer.bot args defensively with safe fallbacks. Prefer `messageStripped` for cheer text, then `message`, then `rawInput`.
5. Preserve cheer token sanitization and word caps:
   - [bits-tier-1.cs](bits-tier-1.cs) — no word cap
   - [bits-tier-2.cs](bits-tier-2.cs) — 250-word cap
   - [bits-tier-3.cs](bits-tier-3.cs) — 100-word cap
   - [bits-tier-4.cs](bits-tier-4.cs) — 10-word cap
6. Preserve dynamic wait behavior for readout-style scripts: `3000ms + 400ms per word + 500ms buffer` where currently used.
7. Keep scripts lightweight and self-contained. Do not assume shared repo helper files can be imported by Streamer.bot.
8. Update the Script Reference section in this file if trigger variables, payload identifiers, command IDs, waits, or operator wiring changes.
9. If a new global variable, OBS source, timer, or shared command contract is introduced, update [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) when in scope or flag it in the handoff when out of scope.

Current script map:

| Script | Runtime purpose |
|---|---|
| [bits-tier-1.cs](bits-tier-1.cs) | Tier 1 cheer text to Mix It Up, no word cap, paced readout wait |
| [bits-tier-2.cs](bits-tier-2.cs) | Tier 2 cheer text to Mix It Up, 250-word cap, paced readout wait |
| [bits-tier-3.cs](bits-tier-3.cs) | Tier 3 cheer text to Mix It Up, 100-word cap, paced readout wait |
| [bits-tier-4.cs](bits-tier-4.cs) | Tier 4 cheer text to Mix It Up, 10-word cap, paced readout wait |
| [gigantify-emote.cs](gigantify-emote.cs) | Automatic reward bridge for gigantified emote metadata |
| [message-effects.cs](message-effects.cs) | Automatic reward bridge for viewer-entered message effects text |
| [on-screen-celebration.cs](on-screen-celebration.cs) | Automatic reward bridge for on-screen celebration metadata |

## Validation

For script changes, perform the narrowest safe validation available:

- Review edited C# for Streamer.bot paste readiness: one `Execute()` entry point, no unsupported imports, no repo-only runtime dependencies, and defensive arg handling.
- Verify global names, OBS names, timers, bit tier constants, and command contracts against [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md).
- For Mix It Up payload changes, confirm `Arguments` compatibility and populated `SpecialIdentifiers` in the Script Reference section in this file.
- Run shared-constants validation when constants or documented references change:

```bash
python3 Tools/StreamerBot/validate-shared-constants.py
```

For agent-doc changes, follow [validation](../../.agents/workflows/validation.md) and run the agent-tree validator with the task-requested report path. Record command output in the handoff or final change summary.

## Boundaries / Out of Scope

- Do not change cheer tier thresholds or reward wiring unless explicitly requested.
- Do not hardcode bit threshold values in scripts; use [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) as the canonical reference.
- Do not rename Mix It Up special identifier keys without updating the matching Mix It Up commands and the Script Reference section payload contract.
- Do not move channel-point redemption scripts from [Actions/Twitch Channel Points/](../Twitch%20Channel%20Points/) into this folder unless the operator explicitly requests a repo reorganization.
- Do not move non-Bits Twitch docs into this guide unless the operator explicitly requests a repo reorganization.

## Handoff Notes

After changes, follow these workflows:

- [change-summary](../../.agents/workflows/change-summary.md) — terminal summary with changed files, paste targets, setup steps, and validation output.
- [sync](../../.agents/workflows/sync.md) — repo-to-Streamer.bot manual paste expectations.
- [validation](../../.agents/workflows/validation.md) — validation command selection and failure reporting.

Paste targets are the edited `.cs` files under [Actions/Twitch Bits Integrations/](./). Operator must manually paste changed script contents into the matching Streamer.bot actions and verify cheer-tier or automatic-reward trigger filtering.

Public-copy handoff triggers: cheer/readout wording, automatic reward prompts, overlay message text, TTS text, or Mix It Up text branches. Include exactly which strings changed and whether `brand-steward` reviewed them.

---

## Script Reference

Shared constants reference: `Actions/SHARED-CONSTANTS.md`

### Script: `bits-tier-1.cs`

#### Purpose
Forwards Tier 1 cheer text to Mix It Up with sanitization and TTS pacing wait.

#### Expected Trigger / Input
- Tier 1 cheer trigger with message text (`messageStripped` fallback `message` then `rawInput`).

#### Required Runtime Variables
- None.

#### Key Outputs / Side Effects
- Forwards cheer text to Mix It Up.
- Prefers Streamer.bot's `messageStripped` value so cheer tokens are already removed.
- Uses the standardized bits readout helper/payload shape (`Platform`, `Arguments`, populated `SpecialIdentifiers`, `IgnoreRequirements`).
- Applies pacing wait to reduce overlap/cutoff.

#### Mix It Up Actions
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

#### OBS Interactions
- None.

#### Wait Behavior
- Wait after successful Mix It Up call: `3000ms + 400ms per word + 500ms buffer`.

#### Chat / Log Output
- Reads cheer text from trigger args.
- Sends no chat output.

#### Operator Notes
- Current command ID is configured from the saved Mix It Up command export.

---

### Script: `bits-tier-2.cs`

#### Purpose
Forwards Tier 2 cheer text to Mix It Up with a 250-word cap.

#### Expected Trigger / Input
- Tier 2 cheer trigger with message text (`messageStripped` fallback `message` then `rawInput`).

#### Required Runtime Variables
- None.

#### Key Outputs / Side Effects
- Forwards cheer text, caps to 250 words, and sends it to Mix It Up.
- Prefers Streamer.bot's `messageStripped` value so cheer tokens are already removed.
- Uses the standardized bits readout helper/payload shape (`Platform`, `Arguments`, populated `SpecialIdentifiers`, `IgnoreRequirements`).
- Applies pacing wait to reduce overlap/cutoff.

#### Mix It Up Actions
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

#### OBS Interactions
- None.

#### Wait Behavior
- Wait after successful Mix It Up call: `3000ms + 400ms per word + 500ms buffer`.

#### Chat / Log Output
- Reads cheer text from trigger args.
- Sends no chat output.

#### Operator Notes
- Current command ID is configured from the saved Mix It Up command export.

---

### Script: `bits-tier-3.cs`

#### Purpose
Forwards Tier 3 cheer text to Mix It Up with a 100-word cap.

#### Expected Trigger / Input
- Tier 3 cheer trigger with message text (`messageStripped` fallback `message` then `rawInput`).

#### Required Runtime Variables
- None.

#### Key Outputs / Side Effects
- Forwards cheer text, caps to 100 words, and sends it to Mix It Up.
- Prefers Streamer.bot's `messageStripped` value so cheer tokens are already removed.
- Uses the standardized bits readout helper/payload shape (`Platform`, `Arguments`, populated `SpecialIdentifiers`, `IgnoreRequirements`).
- Applies pacing wait to reduce overlap/cutoff.

#### Mix It Up Actions
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

#### OBS Interactions
- None.

#### Wait Behavior
- Wait after successful Mix It Up call: `3000ms + 400ms per word + 500ms buffer`.

#### Chat / Log Output
- Reads cheer text from trigger args.
- Sends no chat output.

#### Operator Notes
- Current command ID is configured.

---

### Script: `bits-tier-4.cs`

#### Purpose
Forwards Tier 4 cheer text to Mix It Up with a 10-word cap.

#### Expected Trigger / Input
- Tier 4 cheer trigger with message text (`messageStripped` fallback `message` then `rawInput`).

#### Required Runtime Variables
- None.

#### Key Outputs / Side Effects
- Forwards cheer text, caps to 10 words, and sends it to Mix It Up.
- Prefers Streamer.bot's `messageStripped` value so cheer tokens are already removed.
- Uses the standardized bits readout helper/payload shape (`Platform`, `Arguments`, populated `SpecialIdentifiers`, `IgnoreRequirements`).
- Applies pacing wait to reduce overlap/cutoff.

#### Mix It Up Actions
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

#### OBS Interactions
- None.

#### Wait Behavior
- Wait after successful Mix It Up call: `3000ms + 400ms per word + 500ms buffer`.

#### Chat / Log Output
- Reads cheer text from trigger args.
- Sends no chat output.

#### Operator Notes
- Current command ID is configured from the saved Mix It Up command export.

---

### Script: `gigantify-emote.cs`

#### Purpose
Handles the `gigantify emote` automatic reward redemption by calling a Mix It Up command with a fixed message, fixed type, and the gigantified emote details when available.

#### Expected Trigger / Input
- Streamer.bot trigger: `Twitch -> Channel Reward -> Automatic Reward Redemption`.
- For gigantify-style rewards, Streamer.bot may provide:
  - `gigantifiedEmoteId`
  - `gigantifiedEmoteName`
  - `gigantifiedEmoteUrl`

#### Required Runtime Variables
- None.

#### Key Outputs / Side Effects
- Calls Mix It Up Run Command API for the gigantify emote flow.
- Sends a fixed message payload of `whos that emote?`.
- Sends `SpecialIdentifiers.type = normal`.
- Forwards `emoteId`, `emoteName`, and `emoteUrl` into `SpecialIdentifiers` when available from the trigger.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_GIGANTIFY_EMOTE_COMMAND_ID` *(placeholder; must be replaced)*
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = "whos that emote?"`
  - `SpecialIdentifiers = { type = "normal", emoteId = "...", emoteName = "...", emoteUrl = "..." }`
  - `IgnoreRequirements = false`

#### OBS Interactions
- None.

#### Wait Behavior
- None.

#### Chat / Log Output
- No chat output.
- Logs warning/error messages when the Mix It Up call fails.

#### Operator Notes
- Replace the placeholder command ID after the matching Mix It Up action group is created and appears in the saved Mix It Up command export.
- Wire this script to `Twitch -> Channel Reward -> Automatic Reward Redemption`.
- Filter the action so it only runs for the `gigantify emote` automatic reward.

---

### Script: `message-effects.cs`

#### Purpose
Handles the `message effects` automatic reward redemption by forwarding the redeeming user's entered text to a Mix It Up command.

#### Expected Trigger / Input
- Streamer.bot trigger: `Twitch -> Channel Reward -> Automatic Reward Redemption`.
- Streamer.bot may provide the entered text under one of these args, depending on trigger wiring/version:
  - `userInput`
  - `input0`
  - `message`
  - `rawInput`

#### Required Runtime Variables
- None.

#### Key Outputs / Side Effects
- Calls Mix It Up Run Command API for the message effects flow.
- Sends the first non-empty value found in `userInput`, `input0`, `message`, or `rawInput` in `Arguments`.
- Sends populated `SpecialIdentifiers` for Mix It Up branching/text fields.
- Uses the same standardized bits readout helper/payload shape as the cheer-tier scripts.
- Waits after a successful Mix It Up call so TTS/message-effect readouts are less likely to overlap.
- Logs a warning if none of those args are populated.
- Logs which fallback arg was used when the text does not come from `userInput`.

#### Mix It Up Actions
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

#### OBS Interactions
- None.

#### Wait Behavior
- Wait after successful Mix It Up call: `3000ms + 400ms per word + 500ms buffer`.

#### Chat / Log Output
- No chat output.
- Logs warning/error messages when the Mix It Up call fails or when `userInput` is blank.

#### Operator Notes
- Replace the placeholder command ID after the matching Mix It Up action group is created and appears in the saved Mix It Up command export.
- Wire this script to `Twitch -> Channel Reward -> Automatic Reward Redemption`.
- Filter the action so it only runs for the `message effects` automatic reward.

---

### Script: `on-screen-celebration.cs`

#### Purpose
Handles the `on screen celebration` automatic reward redemption by calling a Mix It Up command with no `Arguments` text and populated celebration metadata identifiers.

#### Expected Trigger / Input
- Streamer.bot trigger: `Twitch -> Channel Reward -> Automatic Reward Redemption`.
- Streamer.bot may provide optional message text under `userInput`, `input0`, `message`, or `rawInput` depending on trigger wiring/version.

#### Required Runtime Variables
- None.

#### Key Outputs / Side Effects
- Calls Mix It Up Run Command API for the on-screen celebration flow.
- Sends an empty `Arguments` value for compatibility with the current Mix It Up command.
- Sends populated `SpecialIdentifiers` for Mix It Up branching/text fields.

#### Mix It Up Actions
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

#### OBS Interactions
- None.

#### Wait Behavior
- None.

#### Chat / Log Output
- No chat output.
- Logs warning/error messages when the Mix It Up call fails.

#### Operator Notes
- Replace the placeholder command ID after the matching Mix It Up action group is created and appears in the saved Mix It Up command export.
- Wire this script to `Twitch -> Channel Reward -> Automatic Reward Redemption`.
- Filter the action so it only runs for the `on screen celebration` automatic reward.
