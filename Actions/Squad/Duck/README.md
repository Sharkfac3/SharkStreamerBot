# Duck Script Reference

## Script: `duck-main.cs`

### Purpose
Starts the Duck event quack window.

### Expected Trigger / Input
- Event/action trigger that starts the Duck mini-game.

### Required Runtime Variables
- Reads/writes shared mini-game lock:
  - `minigame_active`
  - `minigame_name` (`duck` while this mini-game owns the lock)
- Reads `duck_event_active` to prevent overlap.
- Writes `duck_event_active` = `true`.
- Writes `duck_quack_count` = `0`.
- Writes `duck_caller` from `user` arg (or empty).

### Key Outputs / Side Effects
- Opens Duck event collection window.
- Claims shared mini-game lock so no other mini-game can start.
- Enables timer `Duck - Call Window`.

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

---

## Script: `duck-call.cs`

### Purpose
Counts `quack` occurrences in chat while Duck event is active.

### Expected Trigger / Input
- Chat message trigger during active Duck window.

### Required Runtime Variables
- Reads `duck_event_active` as a gate.
- Reads/writes `duck_quack_count` (adds case-insensitive `quack` hits in message text).

### Key Outputs / Side Effects
- Increments aggregate quack counter from incoming chat text.

### Mix It Up Actions
- None.

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Reads incoming chat text (`message`, fallback `rawInput`).
- Sends no chat output.

### Operator Notes
- Keep trigger broad enough to capture normal messages while event is active.

---

## Script: `duck-resolve.cs`

### Purpose
Ends Duck event and resolves success/failure.

### Expected Trigger / Input
- Timer or follow-up action trigger after call window ends.

### Required Runtime Variables
- Reads/writes shared mini-game lock:
  - `minigame_active`
  - `minigame_name` (released on resolve when owned by Duck)
- Reads `duck_event_active` for guard behavior.
- Writes `duck_event_active` = `false` when resolving.
- Reads `duck_quack_count` for success comparison.
- Reads/writes `duck_unlocked` (set `true` on first successful unlock).

### Key Outputs / Side Effects
- Disables timer `Duck - Call Window` on guard/resolve paths.
- Sets unlock state for Duck when successful.
- Releases shared mini-game lock after resolve.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `d311b1c1-943a-44cb-9749-b189d1dbd08b`
- Payload `Arguments`: empty string (`""`)
- Called only on first-time unlock.

### OBS Interactions
- On first-time unlock: visibility refresh `ObsHideSource("Disco Party: Workspace", "Duck - Dancing")` then `ObsShowSource(...)`.
- Logs an error if OBS call throws (helps diagnose scene/source mismatch).

### Wait Behavior
- None.

### Chat / Log Output
- Sends unlock message (first-time unlock).
- Sends success-but-already-unlocked message.
- Sends failure message when quacks do not beat roll.

### Operator Notes
- Preserve timer disable behavior to avoid overlapping resolve runs.
