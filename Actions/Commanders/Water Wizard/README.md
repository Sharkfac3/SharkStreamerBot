# Water Wizard Script Reference

## Script: `water-wizard-redeem.cs`

### Purpose
Assigns the current Water Wizard commander slot occupant, finalizes the outgoing wizard's `!hail` score, and checks for a new all-time high score.

### Expected Trigger / Input
- Redeem/action trigger for Water Wizard role assignment.
- Reads `user`.

### Required Runtime Variables
- Reads/writes `current_water_wizard`.
- Reads/writes `water_wizard_hail_count`.
- Reads/writes `water_wizard_hydrate_next_allowed_utc`.
- Reads/writes `water_wizard_orb_next_allowed_utc`.
- Reads/writes (persisted) `water_wizard_hail_high_score`.
- Reads/writes (persisted) `water_wizard_hail_high_score_user`.

### Key Outputs / Side Effects
- Updates active Water Wizard commander assignment.
- Resets `water_wizard_hail_count` to `0` for the new wizard tenure.
- Resets all Water Wizard command cooldown vars so the new wizard starts with no cooldown debt.
- If outgoing wizard beat the high score, announces the new record in chat.

### Mix It Up Actions
- None.

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Announces new high score when record is beaten.

### Operator Notes
- Keep variable key `current_water_wizard` unchanged to preserve existing integrations.
- High score vars are intentionally persisted across Streamer.bot restarts.

---

## Script: `water-wizard-hail.cs`

### Purpose
Handles public `!hail` support calls for the current Water Wizard.

### Expected Trigger / Input
- Chat command/action trigger for `!hail`.
- Reads `user`.

### Required Runtime Variables
- Reads `current_water_wizard`.
- Reads/writes `water_wizard_hail_count`.

### Key Outputs / Side Effects
- Increments `water_wizard_hail_count` by 1 per valid `!hail`.
- Blocks self-support (current Water Wizard cannot `!hail` themselves).
- If no active Water Wizard exists, responds with guidance.

### Mix It Up Actions
- None currently.

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Sends success/failure guidance messages in chat.

### Operator Notes
- Future Mix It Up hook can be added to the success path.

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

---

## Script: `water-wizard-castrest.cs`

### Purpose
Handles Water Wizard-only `!castrest X` command usage during the rest/focus loop's pre-rest window.

### Expected Trigger / Input
- Chat command/action trigger for `!castrest`.
- Expects `X` to be a whole number of minutes from `1` to `999` (reads `input0` first, then falls back to `rawInput` / `message`).

### Required Runtime Variables
- Reads `current_water_wizard` (active Water Wizard username).
- Reads `rest_focus_loop_active`.
- Reads/writes `rest_focus_loop_phase`.

### Key Outputs / Side Effects
- If caller **is** current Water Wizard and the loop is in `pre_rest`:
  - Sets `rest_focus_loop_phase = "rest"` before arming the next timer so overlapping triggers see the intended target state.
  - Triggers Mix It Up placeholder command `Wizards Rest` with `Arguments = seconds` and `SpecialIdentifiers.time = seconds`.
  - Defensively disables every non-target rest/focus loop timer, then disables and re-arms timer `Rest Focus - Rest` using `X` minutes converted to seconds.
  - If timer arming fails, disables all four loop timers and marks `rest_focus_loop_active = false` so the operator can restart cleanly.
- If caller **is not** current Water Wizard:
  - If a Water Wizard is active, sends Twitch chat instruction to type `!hail`.
  - If no Water Wizard is active, explains that the default rest duration will be used instead.
- If caller **is** current Water Wizard but the loop is not active:
  - Sends a short chat message that the crew is not in a rest/focus loop right now.
- If caller **is** current Water Wizard but the loop is not in `pre_rest`:
  - Sends a short chat message explaining that the rest window already moved on.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `REPLACE_WITH_WIZARDS_REST_COMMAND_ID` *(placeholder; must be replaced)*
- Payload `Arguments`: rest duration in seconds
- Payload `SpecialIdentifiers.time`: rest duration in seconds

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Sends short guidance messages for unauthorized use, invalid usage, inactive loop state, and wrong loop phase.
- Logs warning/error if Mix It Up call fails.
- Logs timer arming attempts, timer-arm failures, and fail-closed loop recovery actions.

### Operator Notes
- Replace `REPLACE_WITH_WIZARDS_REST_COMMAND_ID` before production use.
- Wire this script to the `!castrest` command trigger action.
- This script depends on `CPH.SetTimerInterval(string, int)` to apply the operator-selected rest duration. Verify that method in your Streamer.bot build before production use.
- Mix It Up failure does **not** stop the loop. Timer-arm failure does.
- If timer arming fails, the script disables all four rest/focus timers and clears `rest_focus_loop_active`. Recovery is manual: fix the timer/setup problem, then restart the loop from its normal start action.

---

## Script: `water-wizard-orb.cs`

### Purpose
Handles Water Wizard-only `!orb` command usage.

### Expected Trigger / Input
- Chat command/action trigger for `!orb`.
- Accepts optional text after the command (`0` to `30` words; reads `rawInput`, then fallback `message`).

### Required Runtime Variables
- Reads `current_water_wizard` (active Water Wizard username).
- Reads/writes `water_wizard_orb_next_allowed_utc` (Unix timestamp, UTC, used for 5-minute cooldown).

### Key Outputs / Side Effects
- If caller **is** current Water Wizard and phrase is valid and off cooldown:
  - Triggers Mix It Up command and forwards optional orb text (`0` to `30` words) as payload `Arguments`.
  - Starts/refreshes 5-minute cooldown.
- If caller **is not** current Water Wizard:
  - If a Water Wizard is active, sends Twitch chat instruction to type `!hail` for encouragement.
  - If no Water Wizard is active, encourages caller to redeem and become the Water Wizard.
- If caller **is** current Water Wizard but is on cooldown:
  - Sends cooldown remaining message in chat.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `REPLACE_WITH_WATER_WIZARD_ORB_COMMAND_ID` *(placeholder; must be replaced)*
- Payload `Arguments`: validated orb text (optional, max 30 words)

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Sends chat guidance/feedback for unauthorized use, invalid usage, and cooldown status.
- Logs warning/error if Mix It Up call fails.

### Operator Notes
- Replace `MIXITUP_COMMAND_ID` placeholder before production use.
- Wire this script to the `!orb` command trigger action.
