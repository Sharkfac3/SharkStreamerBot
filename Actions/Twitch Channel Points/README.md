# Twitch Channel Points Script Reference

Shared constants reference: `Actions/SHARED-CONSTANTS.md`

## Scope
This folder contains channel point redeem scripts that map to the Streamer.bot `Twitch Channel Points` action group.

## Script: `disco-party.cs`

### Purpose
Handles the `disco party` channel point redeem by switching to the correct Disco Party OBS scene based on the current stream mode.

### Expected Trigger / Input
- Channel point redeem action for `disco party`.
- No chat arguments required.

### Required Runtime Variables
- Reads `stream_mode`.

### Key Outputs / Side Effects
- If `stream_mode = garage`, switches to `Disco Party: Garage`.
- If `stream_mode = workspace`, switches to `Disco Party: Workspace`.
- If `stream_mode = gamer`, switches to `Disco Party: Gamer`.
- If `stream_mode` is missing/unknown, logs a warning and falls back to `Disco Party: Workspace`.

### Mix It Up Actions
- None.

### OBS Interactions
- Switches current OBS scene using available CPH OBS scene methods.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs warning messages if stream mode is unknown or OBS scene switching fails.

### Operator Notes
- Ensure the three OBS scenes exist with exact names:
  - `Disco Party: Garage`
  - `Disco Party: Workspace`
  - `Disco Party: Gamer`
- Wire this script into the Streamer.bot action tied to your `disco party` channel point redeem.

---

## Script: `explain-current-task.cs`

### Purpose
Handles the `Explain: Current Task` channel point redeem by ensuring recording is active and triggering a Mix It Up command.

### Expected Trigger / Input
- Channel point redeem action for `Explain: Current Task`.
- No chat arguments required.

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Checks whether OBS recording is already active.
- Starts recording when not already active.
- Falls back to toggling recording when explicit start methods are unavailable.
- Calls Mix It Up Run Command API for the explain task response flow.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `replace-with-actual-id-dyude-cmon` *(placeholder; replace with the real ID)*
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers = {}`
  - `IgnoreRequirements = false`

### OBS Interactions
- Uses CPH OBS recording methods (start/toggle as fallback) to ensure recording is active.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs warning/error messages when OBS or Mix It Up calls fail.

### Operator Notes
- Replace `replace-with-actual-id-dyude-cmon` with the actual Mix It Up command ID.
- Wire this script into the Streamer.bot action tied to your channel point redeem.

---

## Known Gap
- `Explain: Ask Away` exists in Streamer.bot, but there is currently no matching repo script under `Actions/Twitch Channel Points/` yet.
- Once that action is exported into the repo, place it in this folder so the project stays aligned with Streamer.bot.
