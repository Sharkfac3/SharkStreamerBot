# The Director Script Reference

## Script: `the-director-redeem.cs`

### Purpose
Assigns the current The Director commander slot occupant, finalizes the outgoing director's `!award` score, and checks for a new all-time high score.

### Expected Trigger / Input
- Redeem/action trigger for The Director role assignment.
- Reads `user`.

### Required Runtime Variables
- Reads/writes `current_the_director`.
- Reads/writes `the_director_award_count`.
- Reads/writes `the_director_checkchat_next_allowed_utc`.
- Reads/writes `the_director_toad_next_allowed_utc`.
- Reads/writes (persisted) `the_director_award_high_score`.
- Reads/writes (persisted) `the_director_award_high_score_user`.

### Key Outputs / Side Effects
- Updates active The Director commander assignment.
- Resets `the_director_award_count` to `0` for the new director tenure.
- Resets all The Director command cooldown vars so the new director starts with no cooldown debt.
- If outgoing director beat the high score, announces the new record in chat.
- Triggers Mix It Up command `Commander - The Director - Redeem` after the new director is stored.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `51146998-c2bc-4f46-b6a8-13069565a562`
- Payload `Arguments`: new The Director username
- Payload `SpecialIdentifiers.user`: new The Director username
- Payload `SpecialIdentifiers.commander`: new The Director username

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Announces new high score when record is beaten.

### Operator Notes
- Keep variable key `current_the_director` unchanged to preserve existing integrations.
- High score vars are intentionally persisted across Streamer.bot restarts.

---

## Script: `the-director-award.cs`

### Purpose
Handles public `!award` support calls for the current The Director.

### Expected Trigger / Input
- Chat command/action trigger for `!award`.
- Reads `user`.

### Required Runtime Variables
- Reads `current_the_director`.
- Reads/writes `the_director_award_count`.

### Key Outputs / Side Effects
- Increments `the_director_award_count` by 1 per valid `!award`.
- Blocks self-support (current The Director cannot `!award` themselves).
- If no active Director exists, responds with guidance.

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
- Command ID in script: `06e3851f-81a2-40cb-a911-33c5ec04a3f2`
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

## Script: `the-director-primary.cs`

### Purpose
Lets the active The Director switch the current OBS scene to its primary source layout.

### Expected Trigger / Input
- Streamer.bot action trigger for `!primary`.
- Reads `user`.

### Required Runtime Variables
- Reads `current_the_director`.

### Key Outputs / Side Effects
- Shows the primary source and hides the secondary source in the current OBS scene.
- Source pairs per scene are defined in `SCENE_SOURCE_MAP` inside the script.
- If Mix It Up command ID is configured, triggers the primary switch command.
- No chat output.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Placeholder: `MIXITUP_COMMAND_ID_PRIMARY`
- Payload `Arguments`: `"primary"`
- Payload `SpecialIdentifiers.state`: `"primary"`
- **Not active until command ID is replaced.**

### OBS Interactions
- `ObsGetCurrentScene()` — reads the active scene. **VERIFY:** unconfirmed method signature — test before relying on in production.
- `ObsShowSource(scene, primarySource)` / `ObsHideSource(scene, secondarySource)`

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs warn on guard exits (wrong caller, no Director, unmapped scene).
- Logs warn on success showing which source was shown/hidden.

### Operator Notes
- Wire `!primary` action → run this script. No action argument needed.
- Add scene→source entries to `SCENE_SOURCE_MAP` inside the script as sources are confirmed.
- Keep `SCENE_SOURCE_MAP` in sync with `the-director-secondary.cs`.
- Currently mapped: `Workspace: Main` (primary: `Main Screen Capture`, secondary: `Quest POV`).
- Replace `REPLACE_WITH_PRIMARY_COMMAND_ID` when Mix It Up command exists.
- Verify `ObsGetCurrentScene()` resolves in Streamer.bot before shipping.

---

## Script: `the-director-secondary.cs`

### Purpose
Lets the active The Director switch the current OBS scene to its secondary source layout.

### Expected Trigger / Input
- Streamer.bot action trigger for `!secondary`.
- Reads `user`.

### Required Runtime Variables
- Reads `current_the_director`.

### Key Outputs / Side Effects
- Shows the secondary source and hides the primary source in the current OBS scene.
- Source pairs per scene are defined in `SCENE_SOURCE_MAP` inside the script.
- If Mix It Up command ID is configured, triggers the secondary switch command.
- No chat output.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Placeholder: `MIXITUP_COMMAND_ID_SECONDARY`
- Payload `Arguments`: `"secondary"`
- Payload `SpecialIdentifiers.state`: `"secondary"`
- **Not active until command ID is replaced.**

### OBS Interactions
- `ObsGetCurrentScene()` — reads the active scene. **VERIFY:** unconfirmed method signature — test before relying on in production.
- `ObsShowSource(scene, secondarySource)` / `ObsHideSource(scene, primarySource)`

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs warn on guard exits (wrong caller, no Director, unmapped scene).
- Logs warn on success showing which source was shown/hidden.

### Operator Notes
- Wire `!secondary` action → run this script. No action argument needed.
- Add scene→source entries to `SCENE_SOURCE_MAP` inside the script as sources are confirmed.
- Keep `SCENE_SOURCE_MAP` in sync with `the-director-primary.cs`.
- Currently mapped: `Workspace: Main` (primary: `Main Screen Capture`, secondary: `Quest POV`).
- Replace `REPLACE_WITH_SECONDARY_COMMAND_ID` when Mix It Up command exists.
- Verify `ObsGetCurrentScene()` resolves in Streamer.bot before shipping.

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
- Command ID in script: `5440fa4e-b84e-438a-a409-f398b637f3e7`
- Payload `Arguments`: validated `!toad` text (optional, max 30 words)
- Payload `SpecialIdentifiers.type`:
  - Default value: `"normal"`
  - Random variant: `"hypno"` with a 1-in-10 chance

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Sends chat guidance/feedback for unauthorized use, invalid usage, and cooldown status.
- Logs warning/error if Mix It Up call fails.

### Operator Notes
- Current command ID is configured from the saved Mix It Up command export.
- Wire this script to the `!toad` command trigger action.
