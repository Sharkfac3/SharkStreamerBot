# The Director Script Reference

## Script: `the-director-redeem.cs`

### Purpose
Assigns the current The Director commander slot occupant.

### Expected Trigger / Input
- Redeem/action trigger for The Director role assignment.

### Required Runtime Variables
- Writes `current_the_director` (string username for the slot occupant).

### Key Outputs / Side Effects
- Updates active The Director commander assignment.

### Mix It Up Actions
- None.

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- None.

### Operator Notes
- Keep variable key `current_the_director` unchanged to preserve existing integrations.

---

## Script: `the-director-checkchat.cs`

### Purpose
Handles The Director-only `!checkchat` command usage.

### Expected Trigger / Input
- Chat command/action trigger for `!checkchat`.
- Accepts up to `10` words after the command (reads `rawInput`, then fallback `message`).
- Message text is optional, so `!checkchat` by itself is valid.

### Required Runtime Variables
- Reads `current_the_director` (active The Director username).
- Reads/writes `the_director_checkchat_next_allowed_utc` (Unix timestamp, UTC, used for 5-minute cooldown).

### Key Outputs / Side Effects
- If caller **is** current The Director and input is valid and off cooldown:
  - Triggers Mix It Up command and forwards the optional text (0 to 10 words) as payload `Arguments`.
  - Starts/refreshes 5-minute cooldown.
- If caller **is not** current The Director:
  - If The Director is active, sends Twitch chat instruction to type `!award`.
  - If no The Director is active, encourages caller to redeem and become The Director.
- If caller **is** current The Director but is on cooldown:
  - Sends cooldown remaining message in chat.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `REPLACE_WITH_DIRECTOR_CHECKCHAT_COMMAND_ID` *(placeholder; must be replaced)*
- Payload `Arguments`: validated `!checkchat` text (optional, max 10 words)

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Sends chat guidance/feedback for unauthorized use, invalid usage, and cooldown status.
- Logs warning/error if Mix It Up call fails.

### Operator Notes
- Replace `MIXITUP_COMMAND_ID` placeholder before production use.
- Wire this script to the `!checkchat` command trigger action.

---

## Script: `the-director-toad.cs`

### Purpose
Handles The Director-only `!toad` command usage.

### Expected Trigger / Input
- Chat command/action trigger for `!toad`.
- Accepts up to `30` words after the command (reads `rawInput`, then fallback `message`).
- Message text is optional, so `!toad` by itself is valid.

### Required Runtime Variables
- Reads `current_the_director` (active The Director username).
- Reads/writes `the_director_toad_next_allowed_utc` (Unix timestamp, UTC, used for 5-minute cooldown).

### Key Outputs / Side Effects
- If caller **is** current The Director and input is valid and off cooldown:
  - Triggers Mix It Up command and forwards the optional text (0 to 30 words) as payload `Arguments`.
  - Starts/refreshes 5-minute cooldown.
- If caller **is not** current The Director:
  - If The Director is active, sends Twitch chat instruction to type `!award`.
  - If no The Director is active, encourages caller to redeem and become The Director.
- If caller **is** current The Director but is on cooldown:
  - Sends cooldown remaining message in chat.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `REPLACE_WITH_DIRECTOR_TOAD_COMMAND_ID` *(placeholder; must be replaced)*
- Payload `Arguments`: validated `!toad` text (optional, max 30 words)

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Sends chat guidance/feedback for unauthorized use, invalid usage, and cooldown status.
- Logs warning/error if Mix It Up call fails.

### Operator Notes
- Replace `MIXITUP_COMMAND_ID` placeholder before production use.
- Wire this script to the `!toad` command trigger action.
