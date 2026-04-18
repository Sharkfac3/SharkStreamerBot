# Clone Script Reference

## Script: `clone-main.cs`

### Purpose
Starts the Clone event and initializes clean game state.

### Expected Trigger / Input
- Event/action trigger that starts the Clone mini-game.

### Required Runtime Variables
- Reads/writes shared mini-game lock:
  - `minigame_active`
  - `minigame_name` (`clone` while this mini-game owns the lock)
- Writes `clone_game_active` = `true`.
- Writes `clone_positions_count` = `5`.
- Writes `clone_round` = `1`.
- Writes `clone_position_removed_last` = `0`.
- Writes `clone_positions_open` = `1,2,3,4,5`.
- Writes `clone_winners` = empty.
- Writes `clone_round1_pool` = empty.
- Writes `clone_pos_<n>` rosters = empty.
- Does **not** globally clear `clone_player_pos_<userId>` keys.

### Key Outputs / Side Effects
- Starts a fresh Clone run.
- Claims shared mini-game lock so no other mini-game can start.
- Enables timer `Clone - Volley Timer`.

### Mix It Up Actions
- None.

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Sends mini-game-in-progress warning if another mini-game owns the lock.
- Sends already-active warning if `clone_game_active` is already `true`.
- Sends Clone event start message.

### Operator Notes
- Use as the game-entry action before `clone-position.cs` and `clone-volley.cs` processing.

---

## Script: `clone-position.cs`

### Purpose
Handles `!rebel` position picks/moves while the Clone event is active.

### Expected Trigger / Input
- Chat command containing requested position.

### Required Runtime Variables
- Reads `clone_game_active` to gate command handling.
- Reads `clone_positions_count` to validate range.
- Reads `clone_positions_open` to ensure selected position is still open.
- Reads/writes `clone_pos_<n>` when moving a user between rosters.
- Reads/writes `clone_player_pos_<userId>` for current user position.
- Reads `clone_round` for round-1 eligibility rule.
- Reads/writes `clone_winners` (adds user during round 1 only).

### Key Outputs / Side Effects
- Moves/assigns player roster position when valid.
- Preserves silent reject behavior for invalid picks.

### Mix It Up Actions
- None.

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- None (silent accept/reject behavior).

### Operator Notes
- Keep this action wired to the `!rebel` command path only while game is active.

---

## Script: `clone-volley.cs`

### Purpose
Resolves each Clone volley timer tick by eliminating one open position.

### Expected Trigger / Input
- Timer trigger (`Clone - Volley Timer`).

### Required Runtime Variables
- Reads/writes shared mini-game lock:
  - `minigame_active`
  - `minigame_name` (released on Clone end when owned by Clone)
- Reads `clone_game_active` as execution guard.
- Reads `clone_positions_count` (fallback board size).
- Reads/writes `clone_round` (increments on continue path).
- Reads/writes `clone_positions_open` (removes one position each volley).
- Writes `clone_position_removed_last`.
- Reads/writes `clone_pos_<n>` (clears eliminated position and scans survivors).
- Writes `clone_player_pos_<userId>` = `0` for eliminated users.
- Reads/writes `clone_round1_pool` (frozen once on round 1).
- Reads/writes `clone_winners` (alive ∩ round1 pool).
- Writes `clone_unlocked` on win.
- Reads/writes `clone_rng_counter` for RNG seed entropy.
- Writes `clone_game_active` = `false` when game ends.

### Key Outputs / Side Effects
- Decides continue/loss/win path for each volley.
- Disables timer on inactive/loss/win end paths.
- Releases shared mini-game lock on loss/win end paths.
- Re-enables timer on continue path.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_CLONE_UNLOCK_COMMAND_ID` *(placeholder; must be replaced)*
- Payload `Arguments`: empty string (`""`)
- Called only on win path.

### OBS Interactions
- On win: visibility refresh `ObsHideSource("Disco Party: Workspace", "Clone - Dancing")` then `ObsShowSource(...)`.
- Logs an error if OBS call throws (helps diagnose scene/source mismatch).

### Wait Behavior
- On win, waits 11 seconds after the Mix It Up unlock command succeeds.
- That 11-second wait is intentionally composed as `3000ms` Mix It Up startup buffer + `8000ms` Clone unlock playtime.
- Continue, loss, and inactive-guard paths do not wait.

### Chat / Log Output
- Sends loss message when no winner-eligible survivors remain.
- Sends win message when final position resolves with survivors.
- Sends round update message on continue path.

### Operator Notes
- Ensure timer wiring points to this script and uses expected cadence.
