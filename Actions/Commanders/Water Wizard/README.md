# Water Wizard Script Reference

## Script: `water-wizard-redeem.cs`

### Purpose
Assigns the current Water Wizard commander slot occupant.

### Expected Trigger / Input
- Redeem/action trigger for Water Wizard role assignment.

### Required Runtime Variables
- Writes `current_water_wizard` (string username for the slot occupant).

### Key Outputs / Side Effects
- Updates active Water Wizard commander assignment.

### Mix It Up Actions
- None.

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- None.

### Operator Notes
- Keep variable key `current_water_wizard` unchanged to preserve existing integrations.

---

## Script: `wizard-hydrate.cs`

### Purpose
Handles Water Wizard-only `!hydrate X` command usage.

### Expected Trigger / Input
- Chat command/action trigger for `!hydrate`.
- Expects `X` to be a number from `1` to `10` (reads `input0` first, then falls back to `rawInput` / `message`).

### Required Runtime Variables
- Reads `current_water_wizard` (active Water Wizard username).
- Reads/writes `water_wizard_hydrate_next_allowed_utc` (Unix timestamp, UTC, used for 5-minute cooldown).

### Key Outputs / Side Effects
- If caller **is** current Water Wizard and `X` is valid and off cooldown:
  - Triggers Mix It Up command and forwards `X` as payload `Arguments`.
  - Starts/refreshes 5-minute cooldown.
- If caller **is not** current Water Wizard:
  - If a Water Wizard is active, sends Twitch chat instruction to type `!hail` for encouragement.
  - If no Water Wizard is active, encourages caller to redeem and become the Water Wizard.
- If caller **is** current Water Wizard but is on cooldown:
  - Sends cooldown remaining message in chat.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `REPLACE_WITH_WATER_WIZARD_HYDRATE_COMMAND_ID` *(placeholder; must be replaced)*
- Payload `Arguments`: the hydrate value `X` (as text)

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Sends chat guidance/feedback for unauthorized use, invalid usage, and cooldown status.
- Logs warning/error if Mix It Up call fails.

### Operator Notes
- Replace `MIXITUP_COMMAND_ID` placeholder before production use.
- Wire this script to the `!hydrate` command trigger action.
- Cooldown is role-specific and tracked globally by key `water_wizard_hydrate_next_allowed_utc`.
