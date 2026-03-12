# Pedro Script Reference

## Script: `pedro-main.cs`

### Purpose
Handles the `!pedro` command entrypoint.

### Expected Trigger / Input
- Command trigger wired to `!pedro`.
- Reads the trailing command message (`message`, fallback `rawInput`).

### Required Runtime Variables
- Reads/writes shared mini-game lock:
  - `minigame_active`
  - `minigame_name` (`pedro` while this mini-game owns the lock)
- Reads `pedro_game_enabled` to prevent overlap.
- Writes `pedro_game_enabled` = `true` on normal game start.
- Writes `pedro_mention_count` = `0` on normal game start.

### Key Outputs / Side Effects
- If command message is empty (`!pedro` only):
  - Starts Pedro event collection window,
  - Claims shared mini-game lock,
  - Enables timer `Pedro - Call Window`.
- If command message is exactly `x500livepedro`:
  - Does **not** start mini-game,
  - Does **not** claim lock,
  - Calls Mix It Up Pedro unlock command only.
- If command message is non-empty and not secret phrase:
  - Does nothing.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `a43a1ecd-1607-4dc2-9ae2-fe96f0566f39`
- Payload `Arguments`: empty string (`""`)
- Uses `IgnoreRequirements = false`
- Called only when command message is exactly `x500livepedro`.

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Sends mini-game-in-progress warning if another mini-game owns the lock.
- Sends already-active warning if event is already running.
- Sends event start message for new event.
- Secret path is silent in chat (Mix It Up call only).

### Operator Notes
- This script should be the direct command action for `!pedro`.

---

## Script: `pedro-call.cs`

### Purpose
Counts `pedro` occurrences in chat while Pedro event is active.

### Expected Trigger / Input
- Chat message trigger during active Pedro window.
- Your setup uses a trigger for messages containing `pedro`.

### Required Runtime Variables
- Reads `pedro_game_enabled` as a gate.
- Reads/writes `pedro_mention_count` (adds case-insensitive `pedro` hits in message text).

### Key Outputs / Side Effects
- Increments aggregate Pedro counter from incoming chat text.
- Ignores explicit `!pedro` command lines so command behavior stays in `pedro-main.cs`.

### Mix It Up Actions
- None.

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Reads incoming chat text (`message`, fallback `rawInput`).
- Sends no chat output during counting.

### Operator Notes
- Keep trigger broad enough to capture normal chat lines while event is active.

---

## Script: `pedro-resolve.cs`

### Purpose
Ends Pedro event and resolves success/failure at timer end.

### Expected Trigger / Input
- Timer or follow-up action trigger after call window ends (`Pedro - Call Window`).

### Required Runtime Variables
- Reads/writes shared mini-game lock:
  - `minigame_active`
  - `minigame_name` (released on resolve when owned by Pedro)
- Reads `pedro_game_enabled` for guard behavior.
- Writes `pedro_game_enabled` = `false` when resolving.
- Reads `pedro_mention_count` for threshold comparison.
- Writes `pedro_unlocked` = `true` on successful resolve.

### Key Outputs / Side Effects
- Disables timer `Pedro - Call Window` on guard/resolve paths.
- If mentions are greater than 100:
  - Sets unlock state,
  - Shows OBS source,
  - Triggers Mix It Up unlock command.
- Releases shared mini-game lock after resolve.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `a43a1ecd-1607-4dc2-9ae2-fe96f0566f39`
- Payload `Arguments`: empty string (`""`)
- Uses `IgnoreRequirements = false`
- Called on resolve success when mentions > 100.

### OBS Interactions
- On resolve success when mentions > 100:
  - `ObsShowSource("Disco Party: Workspace", "Pedro - Dancing")`

### Wait Behavior
- None.

### Chat / Log Output
- Sends unlock message on success.
- Sends failure message when mentions are 100 or lower.

### Operator Notes
- Keep timer wiring so this script runs exactly once after the call window.
