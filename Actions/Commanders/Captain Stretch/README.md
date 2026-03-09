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
- Reads/writes (persisted) `captain_stretch_thank_high_score`.
- Reads/writes (persisted) `captain_stretch_thank_high_score_user`.

### Key Outputs / Side Effects
- Updates active Captain Stretch commander assignment.
- Resets `captain_stretch_thank_count` to `0` for the new captain tenure.
- If outgoing captain beat the high score, announces the new record in chat.

### Mix It Up Actions
- None.

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
- Allows optional text after the command, up to `5` words max (reads `rawInput`, then fallback `message`).

### Required Runtime Variables
- Reads `current_captain_stretch` (active Captain Stretch username).
- Reads/writes `captain_stretch_stretch_next_allowed_utc` (Unix timestamp, UTC, used for 5-minute cooldown).

### Key Outputs / Side Effects
- If caller **is** current Captain Stretch and phrase is valid and off cooldown:
  - Triggers Mix It Up command and forwards the stretch phrase (0 to 5 words) as payload `Arguments`.
  - Starts/refreshes 5-minute cooldown.
- If caller **is not** current Captain Stretch:
  - If a Captain Stretch is active, sends Twitch chat instruction to type `!thank`.
  - If no Captain Stretch is active, encourages caller to redeem and become Captain Stretch.
- If caller **is** current Captain Stretch but is on cooldown:
  - Sends cooldown remaining message in chat.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `REPLACE_WITH_CAPTAIN_STRETCH_COMMAND_ID` *(placeholder; must be replaced)*
- Payload `Arguments`: the validated stretch phrase (0 to 5 words)

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Sends chat guidance/feedback for unauthorized use, invalid usage, and cooldown status.
- Logs warning/error if Mix It Up call fails.

### Operator Notes
- Replace `MIXITUP_COMMAND_ID` placeholder before production use.
- Wire this script to the `!stretch` command trigger action.

---

## Script: `captain-stretch-shrimp.cs`

### Purpose
Handles Captain Stretch-only `!shrimp` command usage.

### Expected Trigger / Input
- Chat command/action trigger for `!shrimp`.
- Allows optional text after the command, up to `30` words max (reads `rawInput`, then fallback `message`).

### Required Runtime Variables
- Reads `current_captain_stretch` (active Captain Stretch username).
- Reads/writes `captain_stretch_shrimp_next_allowed_utc` (Unix timestamp, UTC, used for 5-minute cooldown).

### Key Outputs / Side Effects
- If caller **is** current Captain Stretch and phrase is valid and off cooldown:
  - Triggers Mix It Up command and forwards the shrimp phrase (up to 30 words) as payload `Arguments`.
  - Starts/refreshes 5-minute cooldown.
- If caller **is not** current Captain Stretch:
  - If a Captain Stretch is active, sends Twitch chat instruction to type `!thank`.
  - If no Captain Stretch is active, encourages caller to redeem and become Captain Stretch.
- If caller **is** current Captain Stretch but is on cooldown:
  - Sends cooldown remaining message in chat.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `REPLACE_WITH_CAPTAIN_STRETCH_SHRIMP_COMMAND_ID` *(placeholder; must be replaced)*
- Payload `Arguments`: the validated shrimp phrase (up to 30 words)

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Sends chat guidance/feedback for unauthorized use, invalid usage, and cooldown status.
- Logs warning/error if Mix It Up call fails.

### Operator Notes
- Replace `MIXITUP_COMMAND_ID` placeholder before production use.
- Wire this script to the `!shrimp` command trigger action.
