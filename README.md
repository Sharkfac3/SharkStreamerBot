# SharkStreamerBot

This repo stores **Streamer.bot C# action scripts**.

> Important: scripts are not auto-deployed. After edits, copy/paste each script into its matching Streamer.bot action.

## Script Reference (audited against current code)

Below is each current script and what it actually does today: global variables, Mix It Up usage, OBS interaction, waits, and Twitch chat interaction.

---

## `Actions/Squad/Clone/clone-main.cs`
**Purpose:** Starts the Clone event and initializes clean game state.

**Global vars written** (all non-persisted):
- `clone_game_active` (bool): set `true` at event start.
- `clone_positions_count` (int): set to `5`.
- `clone_round` (int): set to `1`.
- `clone_position_removed_last` (int): reset to `0`.
- `clone_positions_open` (string csv): set to `1,2,3,4,5`.
- `clone_winners` (string csv): reset empty.
- `clone_round1_pool` (string csv): reset empty.
- `clone_pos_<n>` (string csv): each position roster reset empty.
- `clone_player_pos_<userId>`: **not** globally cleared by this script.

**Mix It Up actions:**
- None.

**OBS interactions:**
- None.

**Waits:**
- None.

**Twitch chat interactions:**
- Sends already-active warning if `clone_game_active` is already `true`.
- Sends Clone event start message.

**Other side effects:**
- Enables timer `Clone - Volley Timer`.

---

## `Actions/Squad/Clone/clone-position.cs`
**Purpose:** Handles `!rebel` position picks/moves during Clone event.

**Global vars read/written:**
- Reads `clone_game_active` to gate command handling.
- Reads `clone_positions_count` to validate range.
- Reads `clone_positions_open` to ensure selected position is still open.
- Reads/writes `clone_pos_<n>` when moving user between rosters.
- Reads/writes `clone_player_pos_<userId>` for user’s current slot.
- Reads `clone_round` for round-1 eligibility rule.
- Reads/writes `clone_winners` (adds user during round 1 only).

**Mix It Up actions:**
- None.

**OBS interactions:**
- None.

**Waits:**
- None.

**Twitch chat interactions:**
- None (silent accept/reject behavior).

---

## `Actions/Squad/Clone/clone-volley.cs`
**Purpose:** Resolves each Clone volley timer tick (eliminates one open position).

**Global vars read/written:**
- Reads `clone_game_active` to guard execution.
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
- Writes `clone_game_active` to `false` when game ends.

**Mix It Up actions:**
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `4681be93-409a-4110-bfdb-7a7aa32df63a`
- Payload `Arguments`: `squad-unlock|clone`
- Called only on win path.

**OBS interactions:**
- On win: `ObsShowSource("Disco Party: Workspace", "Clone - Dancing")`.

**Waits:**
- None.

**Twitch chat interactions:**
- Sends loss message when no winner-eligible survivors remain.
- Sends win message when final position resolves with survivors.
- Sends round update message on continue path.

**Other side effects:**
- Disables timer when inactive/loss/win end paths.
- Re-enables timer on continue path.

---

## `Actions/Squad/Duck/duck-main.cs`
**Purpose:** Starts Duck event quack window.

**Global vars read/written:**
- Reads `duck_event_active` to prevent overlapping events.
- Writes `duck_event_active` = `true`.
- Writes `duck_quack_count` = `0`.
- Writes `duck_caller` from `user` arg (or empty).

**Mix It Up actions:**
- None.

**OBS interactions:**
- None.

**Waits:**
- None.

**Twitch chat interactions:**
- Sends already-active warning message if event already running.
- Sends event start message when new event begins.

**Other side effects:**
- Enables timer `Duck - Call Window`.

---

## `Actions/Squad/Duck/duck-call.cs`
**Purpose:** Counts `quack` occurrences in chat while Duck event is active.

**Global vars read/written:**
- Reads `duck_event_active` to gate counting.
- Reads/writes `duck_quack_count` (adds number of case-insensitive `quack` hits in message text).

**Mix It Up actions:**
- None.

**OBS interactions:**
- None.

**Waits:**
- None.

**Twitch chat interactions:**
- Reads incoming chat text (`message` fallback `rawInput`).
- Sends no chat output.

---

## `Actions/Squad/Duck/duck-resolve.cs`
**Purpose:** Ends Duck event and resolves success/failure.

**Global vars read/written:**
- Reads `duck_event_active` for guard behavior.
- Writes `duck_event_active` = `false` when resolving.
- Reads `duck_quack_count` for comparison vs random roll.
- Reads/writes `duck_unlocked` (set true first successful unlock).

**Mix It Up actions:**
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `d311b1c1-943a-44cb-9749-b189d1dbd08b`
- Payload `Arguments`: `squad-unlock|duck`
- Called only on first-time unlock.

**OBS interactions:**
- On first-time unlock: `ObsShowSource("Disco Party: Workspace", "Duck - Dancing")`.

**Waits:**
- None.

**Twitch chat interactions:**
- Sends unlock message (first-time unlock).
- Sends success-but-already-unlocked message.
- Sends failure message when quacks do not beat roll.

**Other side effects:**
- Disables timer `Duck - Call Window` on guard and resolve paths.

---

## `Actions/Squad/Pedro/pedro-main.cs`
**Purpose:** Placeholder/no-op action for Pedro.

**Global vars:**
- None.

**Mix It Up actions:**
- None.

**OBS interactions:**
- None.

**Waits:**
- None.

**Twitch chat interactions:**
- None.

---

## `Actions/Squad/Toothless/toothless-main.cs`
**Purpose:** Runs Toothless rarity roll + unlock handling.

**Global vars read/written:**
- Reads/writes `boost_toothless_<userId>` (consumed/reset to `0` only on first-time unlock).
- Reads/writes `rarity_regular`.
- Reads/writes `rarity_smol`.
- Reads/writes `rarity_long`.
- Reads/writes `rarity_flight`.
- Reads/writes `rarity_party`.
- Writes `last_roll`.
- Writes `last_rarity`.
- Writes `last_user`.

**Mix It Up actions:**
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command IDs by rarity:
  - `regular` → `623c2524-0509-45fb-a759-362edf2543b3`
  - `smol` → `46ab64e0-6682-442a-83fa-d7e265274919`
  - `long` → `c00976c4-ddde-4f56-833a-7551fc106788`
  - `flight` → `2e0b400a-291c-4d8e-b97e-ced7c7b036e3`
  - `party` → `e57d1f2d-716d-41b2-bfbe-d9a8e7974ecb`
- Payload `Arguments`: empty string (`""`) in current script.
- Called only on first-time unlock.

**OBS interactions:**
- On first-time unlock: shows `Toothless - Dancing - {rarity}` on `Disco Party: Workspace`.
- Current script does **not** trigger unlock media restart on current scene.

**Waits:**
- None.

**Twitch chat interactions:**
- Sends new unlock message for first-time rarity unlocks.
- Sends regular roll result message when rarity is already unlocked.

---

## `Actions/Squad/offering.cs`
**Purpose:** Handles offering tokens and applies boost changes (with optional LotAT steal behavior).

**Global vars read/written:**
- Reads `lotat_active`.
- Reads/writes `lotat_announcement_sent` (one-time LotAT announcement latch).
- Reads `lotat_offering_steal_chance` (clamped 0..100).
- Reads `lotat_steal_multiplier` (min 1).
- Reads/writes `boost_<member>_<userId>` (clamped final value 0..30).

**Mix It Up actions:**
- None.

**OBS interactions:**
- None.

**Waits:**
- None.

**Twitch chat interactions:**
- Sends one-time LotAT active announcement.
- Sends unknown-token flavor message.
- Sends result message with delta and resulting boost.

---

## `Actions/Commanders/Captain Stretch/captain-stretch-redeem.cs`
**Purpose:** Assigns current Captain Stretch commander.

**Global vars written:**
- `current_captain_stretch` (string): username currently occupying that commander slot.

**Mix It Up actions:**
- None.

**OBS interactions:**
- None.

**Waits:**
- None.

**Twitch chat interactions:**
- None.

---

## `Actions/Commanders/The Director/the-director-redeem.cs`
**Purpose:** Assigns current The Director commander.

**Global vars written:**
- `current_the_director` (string): username currently occupying that commander slot.

**Mix It Up actions:**
- None.

**OBS interactions:**
- None.

**Waits:**
- None.

**Twitch chat interactions:**
- None.

---

## `Actions/Commanders/Water Wizard/water-wizard-redeem.cs`
**Purpose:** Assigns current Water Wizard commander.

**Global vars written:**
- `current_water_wizard` (string): username currently occupying that commander slot.

**Mix It Up actions:**
- None.

**OBS interactions:**
- None.

**Waits:**
- None.

**Twitch chat interactions:**
- None.

---

## `Actions/Twitch Integration/stream-start.cs`
**Purpose:** Stream-start reset script for Squad/LotAT/Twitch state.

**Global vars written/reset:**
- `rarity_regular`, `rarity_smol`, `rarity_long`, `rarity_flight`, `rarity_party` (bool): reset false.
- `last_roll` (int): reset `0`.
- `last_rarity` (string): reset empty.
- `last_user` (string): reset empty.
- `lotat_active` (bool): reset false.
- `lotat_announcement_sent` (bool): reset false.
- `lotat_offering_steal_chance` (int): reset `0`.
- `lotat_steal_multiplier` (int): reset `1`.
- `duck_event_active` (bool): reset false.
- `duck_quack_count` (int): reset `0`.
- `duck_caller` (string): reset empty.
- `duck_unlocked` (bool): reset false.
- `clone_unlocked` (bool): reset false.
- `clone_game_active` (bool): reset false.
- `clone_round` (int): reset `0`.
- `clone_positions_open` (string): reset empty.
- `clone_winners` (string): reset empty.

**Mix It Up actions:**
- None.

**OBS interactions:**
- For each Toothless rarity source: hide → show → hide on `Disco Party: Workspace`.
- Hides `Duck - Dancing` on `Disco Party: Workspace`.
- Hides `Clone - Dancing` on `Disco Party: Workspace`.

**Waits:**
- None.

**Twitch chat interactions:**
- None.

**Other side effects:**
- Disables timer `Duck - Call Window`.
- Disables timer `Clone - Volley Timer`.

---

## `Actions/Twitch Integration/Bits/bits-tier-1.cs`
**Purpose:** Forwards Tier 1 cheer text to Mix It Up with sanitize + TTS pacing wait.

**Global vars:**
- None.

**Mix It Up actions:**
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_TIER_1_COMMAND_ID` *(placeholder in script; not configured yet)*
- Payload `Arguments`: sanitized full cheer text (Cheer tokens removed, no word cap)

**OBS interactions:**
- None.

**Waits:**
- Wait after successful Mix It Up call: `3000ms + 400ms per word + 500ms buffer`.

**Twitch chat interactions:**
- Reads cheer text from trigger args (`message`, fallback `rawInput`).
- Sends no chat output.

---

## `Actions/Twitch Integration/Bits/bits-tier-2.cs`
**Purpose:** Forwards Tier 2 cheer text to Mix It Up (250-word cap).

**Global vars:**
- None.

**Mix It Up actions:**
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_TIER_2_COMMAND_ID` *(placeholder in script; not configured yet)*
- Payload `Arguments`: sanitized cheer text capped to first 250 words

**OBS interactions:**
- None.

**Waits:**
- Wait after successful Mix It Up call: `3000ms + 400ms per word + 500ms buffer`.

**Twitch chat interactions:**
- Reads cheer text from trigger args (`message`, fallback `rawInput`).
- Sends no chat output.

---

## `Actions/Twitch Integration/Bits/bits-tier-3.cs`
**Purpose:** Forwards Tier 3 cheer text to Mix It Up (100-word cap).

**Global vars:**
- None.

**Mix It Up actions:**
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `23f1afd1-7375-475d-afee-058ef4f7f68d`
- Payload `Arguments`: sanitized cheer text capped to first 100 words

**OBS interactions:**
- None.

**Waits:**
- Wait after successful Mix It Up call: `3000ms + 400ms per word + 500ms buffer`.

**Twitch chat interactions:**
- Reads cheer text from trigger args (`message`, fallback `rawInput`).
- Sends no chat output.

---

## `Actions/Twitch Integration/Bits/bits-tier-4.cs`
**Purpose:** Forwards Tier 4 cheer text to Mix It Up (10-word cap).

**Global vars:**
- None.

**Mix It Up actions:**
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_TIER_4_COMMAND_ID` *(placeholder in script; not configured yet)*
- Payload `Arguments`: sanitized cheer text capped to first 10 words

**OBS interactions:**
- None.

**Waits:**
- Wait after successful Mix It Up call: `3000ms + 400ms per word + 500ms buffer`.

**Twitch chat interactions:**
- Reads cheer text from trigger args (`message`, fallback `rawInput`).
- Sends no chat output.

---

## Commander Model Reminder
- Three commander slots exist (Captain Stretch, The Director, Water Wizard).
- All three can be active at the same time.
- Each commander can have separate command scripts in their folder.

## Operator Sync Reminder
1. Edit scripts in this repo.
2. Copy/paste each changed script into Streamer.bot.
3. Run one happy-path and one edge-case test.
4. Confirm chat/log/OBS behavior is correct.
