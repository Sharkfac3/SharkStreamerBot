# Captain Stretch Script Reference

## Script: `captain-stretch-redeem.cs`

### Purpose
Assigns the current Captain Stretch commander slot occupant, finalizes the outgoing captain's `!thank` score, and checks for a new all-time high score.

### Expected Trigger / Input
- Redeem/action trigger for Captain Stretch role assignment.
- Reads `user`.

### Required Runtime Variables
- Reads/writes `current_captain_stretch`.
- Reads/writes `captain_stretch_thank_count`.
- Reads/writes `captain_stretch_stretch_next_allowed_utc`.
- Reads/writes `captain_stretch_shrimp_next_allowed_utc`.
- Reads/writes (persisted) `captain_stretch_thank_high_score`.
- Reads/writes (persisted) `captain_stretch_thank_high_score_user`.

### Key Outputs / Side Effects
- Updates active Captain Stretch commander assignment.
- Resets `captain_stretch_thank_count` to `0` for the new captain tenure.
- Resets all Captain Stretch command cooldown vars so the new captain starts with no cooldown debt.
- If outgoing captain beat the high score, announces the new record in chat.
- Triggers Mix It Up command `Commander - Captain Stretch - Redeem` after the new captain is stored.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `1facdbe6-f292-4d1b-9b66-ad71ae6de310`
- Payload `Arguments`: new Captain Stretch username
- Payload `SpecialIdentifiers.user`: new Captain Stretch username
- Payload `SpecialIdentifiers.commander`: new Captain Stretch username

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Announces new high score when record is beaten.

### Operator Notes
- Keep variable key `current_captain_stretch` unchanged to preserve existing integrations.
- High score vars are intentionally persisted across Streamer.bot restarts.

---

## Script: `captain-stretch-thank.cs`

### Purpose
Handles public `!thank` support calls for the current Captain Stretch.

### Expected Trigger / Input
- Chat command/action trigger for `!thank`.
- Reads `user`.

### Required Runtime Variables
- Reads `current_captain_stretch`.
- Reads/writes `captain_stretch_thank_count`.

### Key Outputs / Side Effects
- Increments `captain_stretch_thank_count` by 1 per valid `!thank`.
- Blocks self-support (current Captain Stretch cannot `!thank` themselves).
- If no active Captain Stretch exists, responds with guidance.

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

## Script: `captain-stretch-stretch.cs`

### Purpose
Handles Captain Stretch-only `!stretch` command usage.

### Expected Trigger / Input
- Chat command/action trigger for `!stretch`.
- Allows optional text after the command, up to `10` words and `40` characters max (reads `rawInput`, then fallback `message`).

### Required Runtime Variables
- Reads `current_captain_stretch` (active Captain Stretch username).
- Reads/writes `captain_stretch_stretch_next_allowed_utc` (Unix timestamp, UTC, used for 5-minute cooldown).

### Key Outputs / Side Effects
- If caller **is** current Captain Stretch and phrase is valid and off cooldown:
  - Triggers Mix It Up command and forwards the stretch phrase (0 to 10 words, max 40 characters) as payload `Arguments`.
  - Starts/refreshes 5-minute cooldown.
- If caller **is not** current Captain Stretch:
  - If a Captain Stretch is active, sends Twitch chat instruction to type `!thank`.
  - If no Captain Stretch is active, encourages caller to redeem and become Captain Stretch.
- If caller **is** current Captain Stretch but is on cooldown:
  - Sends cooldown remaining message in chat.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `60b43da9-accb-4dbe-968a-d57846a7dc4c`
- Payload `Arguments`: the validated stretch phrase (0 to 10 words, max 40 characters)

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Sends chat guidance/feedback for unauthorized use, invalid usage, and cooldown status.
- Logs warning/error if Mix It Up call fails.

### Operator Notes
- Wire this script to the `!stretch` command trigger action.

---

## Script: `captain-stretch-generalfocus.cs`

### Purpose
Handles Captain Stretch-only `!generalfocus X` command usage during the rest/focus loop's pre-focus window.

### Expected Trigger / Input
- Chat command/action trigger for `!generalfocus`.
- Expects `X` to be a whole number of minutes from `1` to `999` (reads `input0` first, then falls back to `rawInput` / `message`).

### Required Runtime Variables
- Reads `current_captain_stretch` (active Captain Stretch username).
- Reads `rest_focus_loop_active`.
- Reads/writes `rest_focus_loop_phase`.

### Key Outputs / Side Effects
- If caller **is** current Captain Stretch and the loop is in `pre_focus`:
  - Sets `rest_focus_loop_phase = "focus"` before arming the next timer so overlapping triggers see the intended target state.
  - Triggers Mix It Up placeholder command `Captain's Focus` with `Arguments = seconds` and `SpecialIdentifiers.time = seconds`.
  - Defensively disables every non-target rest/focus loop timer, then disables and re-arms timer `Rest Focus - Focus` using `X` minutes converted to seconds.
  - If timer arming fails, disables all four loop timers and marks `rest_focus_loop_active = false` so the operator can restart cleanly.
- If caller **is not** current Captain Stretch:
  - If a Captain Stretch is active, sends Twitch chat instruction to type `!thank`.
  - If no Captain Stretch is active, explains that the default focus duration will be used instead.
- If caller **is** current Captain Stretch but the loop is not active:
  - Sends a short chat message that the crew is not in a rest/focus loop right now.
- If caller **is** current Captain Stretch but the loop is not in `pre_focus`:
  - Sends a short chat message explaining that the focus window is not open right now.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `REPLACE_WITH_CAPTAINS_FOCUS_COMMAND_ID` *(placeholder; must be replaced)*
- Payload `Arguments`: focus duration in seconds
- Payload `SpecialIdentifiers.time`: focus duration in seconds

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Sends short guidance messages for unauthorized use, invalid usage, inactive loop state, and wrong loop phase.
- Logs warning/error if Mix It Up call fails.
- Logs timer arming attempts, timer-arm failures, and fail-closed loop recovery actions.

### Operator Notes
- Replace `REPLACE_WITH_CAPTAINS_FOCUS_COMMAND_ID` before production use.
- Wire this script to the `!generalfocus` command trigger action.
- This script depends on `CPH.SetTimerInterval(string, int)` to apply the operator-selected focus duration. Verify that method in your Streamer.bot build before production use.
- Mix It Up failure does **not** stop the loop. Timer-arm failure does.
- If timer arming fails, the script disables all four rest/focus timers and clears `rest_focus_loop_active`. Recovery is manual: fix the timer/setup problem, then restart the loop from its normal start action.

---

## Script: `captain-stretch-shrimp.cs`

### Purpose
Handles Captain Stretch-only `!shrimp` command usage.

### Expected Trigger / Input
- Chat command/action trigger for `!shrimp`.
- Allows optional text after the command, up to `30` words max (reads `rawInput`, then fallback `message`).

### Required Runtime Variables
- Reads `current_captain_stretch` (active Captain Stretch username).
- Reads/writes `captain_stretch_shrimp_next_allowed_utc` (Unix timestamp, UTC, used for 1-minute cooldown).

### Key Outputs / Side Effects
- If caller **is** current Captain Stretch and phrase is valid and off cooldown:
  - Triggers Mix It Up command and forwards the shrimp phrase (up to 30 words) as payload `Arguments`.
  - Starts/refreshes 1-minute cooldown.
- If caller **is not** current Captain Stretch:
  - If a Captain Stretch is active, sends Twitch chat instruction to type `!thank`.
  - If no Captain Stretch is active, encourages caller to redeem and become Captain Stretch.
- If caller **is** current Captain Stretch but is on cooldown:
  - Sends cooldown remaining message in chat.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `af5567d1-ac94-49bf-ad7b-0b7e034cb05d`
- Payload `Arguments`: the validated shrimp phrase (up to 30 words)

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Sends chat guidance/feedback for unauthorized use, invalid usage, and cooldown status.
- Logs warning/error if Mix It Up call fails.

### Operator Notes
- Wire this script to the `!shrimp` command trigger action.
