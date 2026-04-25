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
  - `regular` → `d391a388-3891-49ad-ba04-0ccc37f6c329`
  - `smol` → `dd0224e2-88fc-4eb2-90ae-976d0fffe410`
  - `long` → `d82e7462-7e78-4dc4-b19d-e989001c9f6e`
  - `flight` → `47027e14-e971-4db0-b129-b2adf79c65d0`
  - `party` → `a71a89cc-a255-4f00-a8f2-5f61a33e7da5`
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

---

## Script: `overlay-publish.cs`

**Reference template — not a standalone deployed action.**

Provides `PublishToothlessStart(triggeredBy)` and `PublishToothlessEnd(rarity, username, isFirstUnlock)` helper methods for publishing Toothless overlay events to the broker.

Copy the constants block and both `Publish*` methods into `toothless-main.cs`. Copy `PublishBrokerMessage` from `Actions/Overlay/broker-publish.cs`.

Integration map (all in `toothless-main.cs`): call `PublishToothlessStart` after acquiring the mini-game lock; call `PublishToothlessEnd` after rarity is determined, on all paths including non-unlock rolls.
