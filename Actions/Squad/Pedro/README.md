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
- Reads `pedro_unlocked` to block the normal mini-game after Pedro has been unlocked.
- Reads `pedro_next_allowed_utc` to enforce the 5-minute replay cooldown.
- Reads/writes `pedro_secret_unlock_active` to serialize the secret unlock path.
- Writes `pedro_game_enabled` = `true` on normal game start.
- Writes `pedro_mention_count` = `0` on normal game start.

### Key Outputs / Side Effects
- If command message is empty (`!pedro` only):
  - Starts Pedro event collection window,
  - Claims shared mini-game lock,
  - Enables timer `Pedro - Call Window`,
  - Only when Pedro is not already unlocked and not on the 5-minute replay cooldown.
- If command message is exactly `x500livepedro`:
  - Does **not** start mini-game,
  - Does **not** claim the normal mini-game lock,
  - Uses a dedicated secret-unlock guard so only one secret run can play at a time,
  - Calls Mix It Up Pedro unlock command,
  - Waits 28 seconds after a successful command call,
  - Still works even if Pedro is already unlocked.
- If command message is non-empty and not secret phrase:
  - Does not start the mini-game,
  - Sends a cryptic in-world hint that the phrase was not the one Pedro was listening for.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `a43a1ecd-1607-4dc2-9ae2-fe96f0566f39`
- Payload `Arguments`: empty string (`""`)
- Uses `IgnoreRequirements = false`
- Called only when command message is exactly `x500livepedro`.

### OBS Interactions
- None.

### Wait Behavior
- Secret phrase path waits 28 seconds after a successful Mix It Up unlock command call.
- While that secret wait is active, additional secret phrase attempts are blocked so the Mix It Up unlock sequence cannot overlap with itself.
- Normal Pedro game start path does not wait.
- Cooldown/unlocked/ignored-input guard paths do not wait.

### Chat / Log Output
- Sends mini-game-in-progress warning if another mini-game owns the lock.
- Sends already-active warning if event is already running.
- Sends unlocked warning if Pedro has already been unlocked this stream.
- Sends cooldown warning if Pedro is still on the 5-minute replay cooldown.
- Sends event start message for new event.
- Sends a cryptic invalid-phrase warning for non-secret `!pedro <text>` usage.
- Sends a secret-path warning if another secret unlock sequence is already playing.

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
- Writes `pedro_next_allowed_utc` = current UTC + 5 minutes on every real resolve.

### Key Outputs / Side Effects
- Disables timer `Pedro - Call Window` on guard/resolve paths.
- Sets a 5-minute replay cooldown on every real resolve.
- If mentions are greater than 100:
  - Sets unlock state,
  - Shows OBS source,
  - Triggers Mix It Up unlock command,
  - Waits 28 seconds before the resolve action finishes.
- Releases shared mini-game lock after resolve.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `a43a1ecd-1607-4dc2-9ae2-fe96f0566f39`
- Payload `Arguments`: empty string (`""`)
- Uses `IgnoreRequirements = false`
- Called on resolve success when mentions > 100.

### OBS Interactions
- On resolve success when mentions > 100:
  - Visibility refresh `ObsHideSource("Disco Party: Workspace", "Pedro - Dancing")` then `ObsShowSource(...)`
- Logs an error if OBS call throws (helps diagnose scene/source mismatch).

### Wait Behavior
- On successful unlock resolve, waits 28 seconds after the Mix It Up unlock command succeeds.
- Failure/guard paths do not wait.

### Chat / Log Output
- Sends unlock message on success.
- Sends failure message when mentions are 100 or lower.

### Operator Notes
- Keep timer wiring so this script runs exactly once after the call window.
- After Pedro unlocks, the normal `!pedro` mini-game should stay unavailable until the next stream reset.
