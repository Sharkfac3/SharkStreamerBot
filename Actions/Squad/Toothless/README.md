# Toothless Script Reference

## Script: `toothless-main.cs`

### Purpose
Runs Toothless rarity roll and first-time unlock handling.

### Expected Trigger / Input
- Command/redeem/action trigger that performs a Toothless roll.

### Required Runtime Variables
- Reads/writes shared mini-game lock:
  - `minigame_active`
  - `minigame_name` (`toothless` during a roll)
- Reads/writes `boost_toothless_<userId>` (consumed/reset to `0` only on first-time unlock).
- Reads/writes `rarity_regular`.
- Reads/writes `rarity_smol`.
- Reads/writes `rarity_long`.
- Reads/writes `rarity_flight`.
- Reads/writes `rarity_party`.
- Writes `last_roll`.
- Writes `last_rarity`.
- Writes `last_user`.

### Key Outputs / Side Effects
- Determines rolled rarity and unlock status.
- Claims shared mini-game lock during roll execution, then releases it.
- Sets first-time unlock state for a rarity.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command IDs by rarity:
  - `regular` → `623c2524-0509-45fb-a759-362edf2543b3`
  - `smol` → `46ab64e0-6682-442a-83fa-d7e265274919`
  - `long` → `c00976c4-ddde-4f56-833a-7551fc106788`
  - `flight` → `2e0b400a-291c-4d8e-b97e-ced7c7b036e3`
  - `party` → `e57d1f2d-716d-41b2-bfbe-d9a8e7974ecb`
- Payload `Arguments`: empty string (`""`) in current script.
- Called only on first-time unlock.

### OBS Interactions
- On first-time unlock: shows `Toothless - Dancing - {rarity}` on `Disco Party: Workspace`.
- Current script does **not** restart unlock media on current scene.

### Wait Behavior
- On first-time rarity unlock, waits 19 seconds after the Mix It Up unlock command succeeds.
- That 19-second wait is intentionally composed as `3000ms` Mix It Up startup buffer + `16000ms` Toothless unlock playtime.
- Non-unlock rolls and blocked-by-other-mini-game paths do not wait.

### Chat / Log Output
- Sends mini-game-in-progress warning if another mini-game owns the lock.
- Sends new unlock message for first-time rarity unlocks.
- Sends regular roll result message when rarity is already unlocked.

### Operator Notes
- Keep rarity variable names and Mix It Up IDs stable for continuity.
- If shared keys/names change, sync with `Actions/SHARED-CONSTANTS.md` and `stream-start.cs`.
