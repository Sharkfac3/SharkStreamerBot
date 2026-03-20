# Duck Script Reference

## Script: `duck-main.cs`

### Purpose
Starts the Duck event and opens the live quack race.

### Expected Trigger / Input
- Event/action trigger that starts the Duck mini-game.

### Required Runtime Variables
- Reads/writes shared mini-game lock:
  - `minigame_active`
  - `minigame_name` (`duck` while this mini-game owns the lock)
- Reads `duck_event_active` to prevent overlap.
- Writes `duck_event_active` = `true`.
- Writes `duck_quack_count` = `0`.
- Writes `duck_target_quacks` = starting threshold.
- Writes `duck_unique_quackers` = round participant registry.
- Writes `duck_unique_quacker_count` = `0`.
- Writes `duck_caller` from `user` arg (or empty).

### Key Outputs / Side Effects
- Opens Duck event collection window.
- Claims shared mini-game lock so no other mini-game can start.
- Enables timer `Duck - Call Window` as the maximum allowed attempt window.
- Announces the event in chat without exposing the exact quack goal.

### Mix It Up Actions
- None.

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Sends mini-game-in-progress warning if another mini-game owns the lock.
- Sends already-active warning if event is already running.
- Sends event start message for new event.

### Operator Notes
- This should always run before `duck-call.cs`/`duck-resolve.cs` in an event cycle.
- Duck no longer waits for timer resolve to succeed. It unlocks immediately when chat reaches the live threshold.

---

## Script: `duck-call.cs`

### Purpose
Counts `quack` occurrences in chat while Duck event is active and unlocks Duck immediately once the threshold is reached.

### Expected Trigger / Input
- Chat message trigger during active Duck window.

### Required Runtime Variables
- Reads `duck_event_active` as a gate.
- Reads/writes `duck_quack_count` (adds case-insensitive `quack` hits in message text).
- Reads/writes `duck_target_quacks` (live threshold).
- Reads/writes `duck_unique_quackers` and `duck_unique_quacker_count` for participation scaling.
- Reads/writes `duck_unlocked` for first-time unlock behavior.

### Key Outputs / Side Effects
- Increments aggregate quack counter from incoming chat text.
- Tracks unique participants using `userId` when available.
- Recalculates the hidden live quack target as more unique chatters join in.
- On threshold reached: ends event immediately, disables timer, triggers unlock flow, releases mini-game lock.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `d311b1c1-943a-44cb-9749-b189d1dbd08b`
- Payload `Arguments`: empty string (`""`)
- Called only on first-time unlock.

### OBS Interactions
- On first-time unlock: visibility refresh `ObsHideSource("Disco Party: Workspace", "Duck - Dancing")` then `ObsShowSource(...)`.
- Logs an error if OBS call throws (helps diagnose scene/source mismatch).

### Wait Behavior
- On first-time unlock, waits 21 seconds after the Mix It Up unlock command succeeds.
- That 21-second wait is intentionally composed as `3000ms` Mix It Up startup buffer + `18000ms` Duck unlock playtime.
- Already-unlocked and non-success paths do not wait.

### Chat / Log Output
- Reads incoming chat text (`message`, fallback `rawInput`).
- Sends unlock message when chat reaches the hidden threshold.
- Sends success-but-already-unlocked message if Duck was already owned.
- Does not reveal the exact required quack count in chat output.

### Operator Notes
- Keep trigger broad enough to capture normal messages while event is active.
- `userId` is preferred for unique participant tracking; fallback `user` is supported.

---

## Script: `duck-resolve.cs`

### Purpose
Fails the Duck event if chat does not reach the live threshold before the timer expires.

### Expected Trigger / Input
- Timer or follow-up action trigger after call window ends.

### Required Runtime Variables
- Reads/writes shared mini-game lock:
  - `minigame_active`
  - `minigame_name` (released on resolve when owned by Duck)
- Reads `duck_event_active` for guard behavior.
- Writes `duck_event_active` = `false` when timing out.
- Reads `duck_unique_quacker_count` for timeout flavor.

### Key Outputs / Side Effects
- Disables timer `Duck - Call Window` on guard/resolve paths.
- Releases shared mini-game lock after timeout.

### Mix It Up Actions
- None.

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Sends failure message when quacks do not reach the hidden threshold before timeout.

### Operator Notes
- This script is now timeout/failure-only.
- Success happens inside `duck-call.cs` the moment the threshold is reached.
