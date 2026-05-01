---
id: actions-twitch-channel-points
type: domain-route
description: Twitch channel point redemption Streamer.bot actions, OBS/Mix It Up behavior, paste targets, and brand handoffs.
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Twitch Channel Points — Agent Guide

## Purpose

This folder owns Streamer.bot C# scripts for Twitch channel point redemptions that are maintained in the repo. Current actions cover Disco Party and Explain Current Task. These scripts bridge Twitch redemptions into OBS scene changes, recording checks, and Mix It Up commands.

This is part of the ratified Twitch target shape: one `streamerbot-dev` owner family with folder-local Twitch guides. Do not recreate flat Twitch wrapper skills; keep channel-point redemption knowledge here.

## When to Activate

Use this guide when editing or reviewing files under [Actions/Twitch Channel Points/](./), including:

- [Actions/Twitch Channel Points/disco-party.cs](disco-party.cs)
- [Actions/Twitch Channel Points/explain-current-task.cs](explain-current-task.cs)
- Script Reference or operator documentation in this folder

Activate `brand-steward` before changing public redemption copy, reward names/descriptions, chat messages, overlay text, spoken/TTS text, or any viewer-facing Mix It Up response text.

## Primary Owner

`streamerbot-dev` owns the C# runtime behavior, Streamer.bot reward wiring expectations, OBS interactions, Mix It Up API payload shape, global variable use, and manual paste readiness for this folder.

## Secondary Owners / Chain To

- `brand-steward` — chain for public redemption text, reward prompts, chat responses, overlay copy, or Mix It Up response wording.
- `ops` — chain only for validation, paste/sync workflow, or agent-tree maintenance beyond this local guide.

## Required Reading

Read the following before editing scripts:

- [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md)
- [Actions/Helpers/obs-scenes.md](../Helpers/obs-scenes.md)
- [Actions/Twitch Core Integrations/AGENTS.md](../Twitch%20Core%20Integrations/AGENTS.md) when changes add/reset shared session state
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) when public copy changes

## Local Workflow

1. Identify the redemption and its Streamer.bot action wiring.
2. Preserve existing reward names, reward IDs, command IDs, and trigger wiring unless the operator explicitly requests a migration.
3. Read Twitch reward args defensively with `CPH.TryGetArg`. Text-input rewards may expose viewer text through `userInput`, `input0`, `message`, or `rawInput` depending on trigger wiring/version.
4. Preserve `stream_mode` behavior for OBS scene routing. Unknown or missing mode should fall back safely to `workspace` behavior.
5. Use direct OBS methods from [Actions/Helpers/obs-scenes.md](../Helpers/obs-scenes.md). Do not use reflection to discover OBS methods.
6. Preserve Mix It Up payload compatibility:
   - Keep `Arguments` behavior compatible with the current Mix It Up command.
   - Put structured redemption metadata in populated `SpecialIdentifiers`.
   - Use lowercase, no-space special identifier keys.
7. Keep scripts self-contained and paste-ready. Do not assume shared repo helper files can be imported by Streamer.bot.
8. Update the Script Reference section in this file if trigger variables, payload identifiers, command IDs, OBS behavior, wait behavior, or operator wiring changes.
9. If a new global variable, OBS source, timer, or shared command contract is introduced, update [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) when in scope or flag it in the handoff when out of scope.

Current script map:

| Script | Runtime purpose | Important notes |
|---|---|---|
| [disco-party.cs](disco-party.cs) | Runs Disco Party start/end Mix It Up commands, switches to mode-matched Disco Party OBS scene, fires unlocked squad dance commands, then returns to the previous scene | Uses `stream_mode`, `disco_party_active`, `disco_party_prev_scene`, unlock flags, and exact OBS scene names |
| [explain-current-task.cs](explain-current-task.cs) | Ensures OBS recording is active, then triggers the Explain Current Task Mix It Up flow | Current Mix It Up command ID is a placeholder in the script contract |

## Validation

For script changes, perform the narrowest safe validation available:

- Review edited C# for Streamer.bot paste readiness: one `Execute()` entry point, no unsupported imports, no repo-only runtime dependencies, and defensive arg handling.
- Verify global names, OBS scenes, timers, and Mix It Up command contracts against [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md).
- For OBS changes, verify the exact scene names documented in the Script Reference section in this file: `Disco Party: Garage`, `Disco Party: Workspace`, and `Disco Party: Gamer`.
- For Mix It Up payload changes, confirm `Arguments` compatibility and populated `SpecialIdentifiers` in the Script Reference section in this file.
- Run shared-constants validation when constants or documented references change:

```bash
python3 Tools/StreamerBot/validate-shared-constants.py
```

For agent-doc changes, follow [validation](../../.agents/workflows/validation.md) and run the agent-tree validator with the task-requested report path. Record command output in the handoff or final change summary.

## Boundaries / Out of Scope

- Do not rename channel point rewards, reward IDs, OBS scenes, or Mix It Up identifier keys unless explicitly requested.
- Do not add public redemption copy without `brand-steward` review.
- Do not move automatic rewards from [Actions/Twitch Bits Integrations/](../Twitch%20Bits%20Integrations/) into this folder unless the operator explicitly requests a repo reorganization.
- Custom intro redemptions are handled by [Actions/Intros/](../Intros/) and are not owned here.
- `Explain: Ask Away` is a known gap in Streamer.bot but has no matching repo script in this folder yet; do not invent the script unless requested.

## Handoff Notes

After changes, follow these workflows:

- [change-summary](../../.agents/workflows/change-summary.md) — terminal summary with changed files, paste targets, setup steps, and validation output.
- [sync](../../.agents/workflows/sync.md) — repo-to-Streamer.bot manual paste expectations.
- [validation](../../.agents/workflows/validation.md) — validation command selection and failure reporting.

Paste targets are the edited `.cs` files under [Actions/Twitch Channel Points/](./). Operator must manually paste changed script contents into the matching Streamer.bot channel-point actions and verify reward trigger wiring.

Public-copy handoff triggers: reward names/descriptions, redemption prompts, chat messages, overlay messages, TTS/spoken responses, and Mix It Up text branches. Include exactly which strings changed and whether `brand-steward` reviewed them.

---

## Script Reference

Shared constants reference: `Actions/SHARED-CONSTANTS.md`

### Scope
This folder contains channel point redeem scripts that map to the Streamer.bot `Twitch Channel Points` action group.

### Script: `disco-party.cs`

#### Purpose
Handles the `disco party` channel point redeem by running Mix It Up intro/outro commands, switching to the correct Disco Party OBS scene based on the current stream mode, and firing dance commands for unlocked squad members.

#### Expected Trigger / Input
- Channel point redeem action for `disco party`.
- No chat arguments required.
- Reads the standard Twitch reward redemption `user` trigger variable and forwards it to Mix It Up as a special identifier.

#### Required Runtime Variables
- Reads `stream_mode`.
- Reads unlock flags for Duck / Clone / Pedro / Toothless rarities.
- Uses `disco_party_active` and `disco_party_prev_scene` for re-entry protection and scene return.

#### Key Outputs / Side Effects
- Runs `Twitch - Channel Points - Disco Party - Start` before switching OBS scenes.
- Waits 5 seconds for the start command to finish when the Mix It Up API call succeeds.
- If `stream_mode = garage`, switches to `Disco Party: Garage`.
- If `stream_mode = workspace`, switches to `Disco Party: Workspace`.
- If `stream_mode = gamer`, switches to `Disco Party: Gamer`.
- If `stream_mode` is missing/unknown, logs a warning and falls back to `Disco Party: Workspace`.
- Fires Mix It Up dance commands for every unlocked squad member / Toothless rarity.
- Waits 60 seconds on the disco scene.
- Returns to the previous scene if OBS is still on a `Disco Party*` scene.
- Runs `Twitch - Channel Points - Disco Party - End` after the OBS return step.
- Waits 5 seconds for the end command to finish when the Mix It Up API call succeeds.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Start command:
  - Name: `Twitch - Channel Points - Disco Party - Start`
  - ID: `9d642983-c438-4b4d-85f9-eccf49251a68`
- End command:
  - Name: `Twitch - Channel Points - Disco Party - End`
  - ID: `d8af386f-e3f5-4eb0-9b53-ca3aaa6853fd`
- Special identifiers sent to both commands:
  - `user = <redeeming Twitch display name>`

#### OBS Interactions
- Saves the current OBS scene before takeover.
- Switches to the mode-matched Disco Party scene.
- Switches back to the saved scene after the party only if the operator has not manually navigated away.

#### Wait Behavior
- 5000ms after the Mix It Up start command when the API call succeeds.
- 60000ms on the Disco Party scene.
- 5000ms after the Mix It Up end command when the API call succeeds.

#### Chat / Log Output
- Sends chat only when a second redeem tries to start while a disco party is already active.
- Logs warning messages if stream mode is unknown, OBS is manually changed during the party, or Mix It Up calls fail.

#### Operator Notes
- Ensure the three OBS scenes exist with exact names:
  - `Disco Party: Garage`
  - `Disco Party: Workspace`
  - `Disco Party: Gamer`
- Ensure Mix It Up is running locally and both disco-party transition commands exist with the IDs above.
- Wire this script into the Streamer.bot action tied to your `disco party` channel point redeem.

---

### Script: `explain-current-task.cs`

#### Purpose
Handles the `Explain: Current Task` channel point redeem by ensuring recording is active and triggering a Mix It Up command.

#### Expected Trigger / Input
- Channel point redeem action for `Explain: Current Task`.
- No chat arguments required.
- Reads Twitch reward redemption args defensively; optional viewer text is accepted from `userInput`, `input0`, `message`, or `rawInput` when present.

#### Required Runtime Variables
- None.

#### Key Outputs / Side Effects
- Checks whether OBS recording is already active.
- Starts recording when not already active.
- Falls back to toggling recording when explicit start methods are unavailable.
- Calls Mix It Up Run Command API for the explain task response flow.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `replace-with-actual-id-dyude-cmon` *(placeholder; replace with the real ID)*
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers = { ... }`
  - `IgnoreRequirements = false`
- Special identifiers:
  - `explaintaskuser = <user arg or empty string>`
  - `explaintaskuserid = <userId arg or empty string>`
  - `explaintasktype = "explaincurrenttask"`
  - `explaintaskrewardid = <reward arg, rewardId arg, or empty string>`
  - `explaintaskrewardname = <rewardName arg, rewardTitle arg, or empty string>`
  - `explaintaskmessage = <userInput/input0/message/rawInput arg or empty string>`
  - `explaintaskmessagetype = "message" when viewer text exists, otherwise "none"`
  - `explaintaskrecordingcheck = "attempted"`

#### OBS Interactions
- Uses CPH OBS recording methods (start/toggle as fallback) to ensure recording is active.

#### Wait Behavior
- None.

#### Chat / Log Output
- No chat output.
- Logs warning/error messages when OBS or Mix It Up calls fail.

#### Operator Notes
- Replace `replace-with-actual-id-dyude-cmon` with the actual Mix It Up command ID.
- Wire this script into the Streamer.bot action tied to your channel point redeem.

---

## Trigger Variables

Access in C# via `CPH.TryGetArg("variableName", out T value)`.

### Channel Reward Redemption

Both scripts are triggered via Twitch → Channel Reward → Reward Redemption.

| Variable | Type | Description |
|---|---|---|
| `user` | string | Display name of the redeeming user |
| `userId` | string | Twitch user ID of the redeeming user |
| `rewardName` | string | Name of the channel point reward |
| `rewardId` | string | Unique identifier for the reward |
| `rewardCost` | number | Point cost of the reward |
| `rewardPrompt` | string | Description/prompt text of the reward |
| `rawInput` | string | Text entered by the user (only if reward has text input enabled) |
| `rawInputEscaped` | string | Same as `rawInput` with special characters escaped |
| `redemptionId` | string | Unique identifier for this specific redemption |
| `counter` | number | Total number of times this reward has been redeemed |
| `userCounter` | number | Number of times this user has redeemed this reward |

---

## Known Gap
- `Explain: Ask Away` exists in Streamer.bot, but there is currently no matching repo script under `Actions/Twitch Channel Points/` yet.
- Once that action is exported into the repo, place it in this folder so the project stays aligned with Streamer.bot.

## Related: Custom Intro Redemptions
Custom Intro channel-point redemptions are **not** handled in this folder. [redeem-capture.cs](../Intros/redeem-capture.cs) captures the redemption and writes a pending-intro record to the info-service. Playback is triggered later via [first-chat-intro.cs](../Intros/first-chat-intro.cs) on the viewer's next first-chat event.
