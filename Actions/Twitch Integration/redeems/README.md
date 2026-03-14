# Twitch Redeems Script Reference

Shared constants reference: `Actions/SHARED-CONSTANTS.md`

## Script: `explain-current-task.cs`

### Purpose
Handles the `explain-current-task` channel point redeem by ensuring recording is active and triggering a Mix It Up command.

### Expected Trigger / Input
- Channel point redeem action for `explain-current-task`.
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
