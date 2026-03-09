# SharkStreamerBot

This repo stores **Streamer.bot C# action scripts**.

> Important: scripts are not auto-deployed. After edits, copy/paste each script into its matching Streamer.bot action.

## Mix It Up Command Call Map

These are the Mix It Up Desktop API commands currently called by scripts in this repo (`POST http://localhost:8911/api/v2/commands/{commandId}`).

- `Actions/Squad/Duck/duck-resolve.cs`
  - Command ID: `d311b1c1-943a-44cb-9749-b189d1dbd08b`
  - Arguments: `squad-unlock|duck`
- `Actions/Squad/Clone/clone-volley.cs`
  - Command ID: `4681be93-409a-4110-bfdb-7a7aa32df63a`
  - Arguments: `squad-unlock|clone`
- `Actions/Squad/Toothless/toothless-main.cs`
  - Regular: `623c2524-0509-45fb-a759-362edf2543b3` (`squad-unlock|toothless|regular`)
  - Smol: `46ab64e0-6682-442a-83fa-d7e265274919` (`squad-unlock|toothless|smol`)
  - Long: `c00976c4-ddde-4f56-833a-7551fc106788` (`squad-unlock|toothless|long`)
  - Flight: `2e0b400a-291c-4d8e-b97e-ced7c7b036e3` (`squad-unlock|toothless|flight`)
  - Party: `e57d1f2d-716d-41b2-bfbe-d9a8e7974ecb` (`squad-unlock|toothless|party`)
- `Actions/Twitch Integration/Bits/bits-tier-1.cs`
  - Command ID: `REPLACE_WITH_TIER_1_COMMAND_ID`
  - Arguments: sanitized full cheer text (no word cap)
- `Actions/Twitch Integration/Bits/bits-tier-2.cs`
  - Command ID: `REPLACE_WITH_TIER_2_COMMAND_ID`
  - Arguments: sanitized cheer text (first 250 words)
- `Actions/Twitch Integration/Bits/bits-tier-3.cs`
  - Command ID: `23f1afd1-7375-475d-afee-058ef4f7f68d`
  - Arguments: sanitized cheer text (first 100 words)
- `Actions/Twitch Integration/Bits/bits-tier-4.cs`
  - Command ID: `REPLACE_WITH_TIER_4_COMMAND_ID`
  - Arguments: sanitized cheer text (first 10 words)

## Script Reference (with Global Vars)

Below is each current script and the global vars it uses.

---

## `Actions/Squad/Clone/clone-main.cs`
**Purpose:** Starts the Clone event and initializes clean game state.

**Global vars written** (all non-persisted):
- `clone_game_active` (bool): true while a Clone match is running.
- `clone_positions_count` (int): total available positions (currently 5).
- `clone_round` (int): current round number (starts at 1).
- `clone_position_removed_last` (int): last position destroyed by volley.
- `clone_positions_open` (csv int list): currently valid positions, e.g. `1,2,3,4,5`.
- `clone_winners` (csv userId list): winner-eligible users (round-1 participants/alive intersection logic later).
- `clone_round1_pool` (csv userId list): frozen snapshot of round-1 participants (populated by volley script).
- `clone_pos_<n>` (csv userId list): roster standing in each position (e.g. `clone_pos_1`).

**Other side effects:**
- Enables timer `Clone - Volley Timer`.
- Sends event-start chat message.

---

## `Actions/Squad/Clone/clone-position.cs`
**Purpose:** Handles `!rebel` position picks/moves during Clone event.

**Global vars read/written:**
- `clone_game_active` (bool): gate to ignore command if event not running.
- `clone_positions_count` (int): valid position range.
- `clone_positions_open` (csv int list): only open positions are allowed.
- `clone_pos_<n>` (csv userId list): removes user from old roster, adds to new roster.
- `clone_player_pos_<userId>` (int): user’s current position (`0` means no active position).
- `clone_round` (int): determines round-1 behavior.
- `clone_winners` (csv userId list): round-1 joiners are added to winner-eligible pool.

---

## `Actions/Squad/Clone/clone-volley.cs`
**Purpose:** Timer resolver for Clone rounds (destroys a position each volley).

**Global vars read/written:**
- `clone_game_active` (bool): whether game is active.
- `clone_positions_count` (int): fallback board size.
- `clone_round` (int): increments every volley.
- `clone_positions_open` (csv int list): updated after removing one random position.
- `clone_position_removed_last` (int): position destroyed this volley.
- `clone_pos_<n>` (csv userId list): removed position roster cleared; surviving rosters scanned.
- `clone_player_pos_<userId>` (int): set to `0` when user is eliminated.
- `clone_round1_pool` (csv userId list): frozen once from early `clone_winners`.
- `clone_winners` (csv userId list): rebuilt as `alive ∩ round1_pool`.
- `clone_unlocked` (bool): set true on successful final-position win.
- `clone_rng_counter` (int): entropy helper for RNG seeding.

**Other side effects:**
- Shows `Clone - Dancing` OBS source on unlock.
- Calls Mix It Up unlock command for Clone.
- Disables/enables `Clone - Volley Timer` based on state.

---

## `Actions/Squad/Duck/duck-main.cs`
**Purpose:** Starts Duck event quack window.

**Global vars written:**
- `duck_event_active` (bool): true during active 2-minute window.
- `duck_quack_count` (int): current number of detected `quack` hits.
- `duck_caller` (string): user who started/called the event.

**Other side effects:**
- Enables timer `Duck - Call Window`.
- Sends event-start chat message.

---

## `Actions/Squad/Duck/duck-call.cs`
**Purpose:** Counts `quack` occurrences in chat while Duck event is active.

**Global vars read/written:**
- `duck_event_active` (bool): only count during active window.
- `duck_quack_count` (int): incremented by count of `quack` occurrences.

---

## `Actions/Squad/Duck/duck-resolve.cs`
**Purpose:** Ends Duck event and resolves success/failure.

**Global vars read/written:**
- `duck_event_active` (bool): guard and close event window.
- `duck_quack_count` (int): compared against random roll.
- `duck_unlocked` (bool): set true on first successful unlock.

**Other side effects:**
- Shows `Duck - Dancing` OBS source on first unlock.
- Calls Mix It Up unlock command for Duck.
- Disables `Duck - Call Window` timer.

---

## `Actions/Squad/Pedro/pedro-main.cs`
**Purpose:** Placeholder/no-op action for Pedro.

**Global vars:**
- None currently.

---

## `Actions/Squad/Toothless/toothless-main.cs`
**Purpose:** Runs Toothless rarity roll + unlock handling.

**Global vars read/written:**
- `boost_toothless_<userId>` (int): optional per-user boost added to roll (consumed/reset on new unlock).
- `rarity_regular` (bool): whether regular rarity unlocked.
- `rarity_smol` (bool): whether smol rarity unlocked.
- `rarity_long` (bool): whether long rarity unlocked.
- `rarity_flight` (bool): whether flight rarity unlocked.
- `rarity_party` (bool): whether party rarity unlocked.
- `last_roll` (int): last resolved boosted roll.
- `last_rarity` (string): last resolved rarity name.
- `last_user` (string): last user who rolled.

**Other side effects:**
- Shows unlock media source on current scene.
- Shows rarity dancing source on Disco scene.
- Calls per-rarity Mix It Up unlock command on first unlock.

---

## `Actions/Squad/offering.cs`
**Purpose:** Handles offering tokens and applies boost changes (with optional LotAT steal behavior).

**Global vars read/written:**
- `lotat_active` (bool): enables LotAT behavior.
- `lotat_announcement_sent` (bool): one-time session announcement latch.
- `lotat_offering_steal_chance` (int 0-100): chance offering is stolen while LotAT active.
- `lotat_steal_multiplier` (int >=1): multiplier for negative stolen penalty.
- `boost_<member>_<userId>` (int): per-user favor/fate boost for target member (clamped 0..30).

---

## `Actions/Commanders/Captain Stretch/captain-stretch-redeem.cs`
**Purpose:** Assigns current Captain Stretch commander.

**Global vars written:**
- `current_captain_stretch` (string): username currently occupying that commander slot.

---

## `Actions/Commanders/The Director/the-director-redeem.cs`
**Purpose:** Assigns current The Director commander.

**Global vars written:**
- `current_the_director` (string): username currently occupying that commander slot.

---

## `Actions/Commanders/Water Wizard/water-wizard-redeem.cs`
**Purpose:** Assigns current Water Wizard commander.

**Global vars written:**
- `current_water_wizard` (string): username currently occupying that commander slot.

---

## `Actions/Twitch Integration/stream-start.cs`
**Purpose:** Stream-start reset script for Squad/LotAT/Twitch state.

**Global vars written/reset:**
- `rarity_regular`, `rarity_smol`, `rarity_long`, `rarity_flight`, `rarity_party` (bool): all reset false.
- `last_roll` (int): reset `0`.
- `last_rarity` (string): reset empty.
- `last_user` (string): reset empty.
- `lotat_active` (bool): reset false by default.
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

**Other side effects:**
- Hides Toothless and Duck/Clone dancing sources in OBS.
- Disables Duck and Clone timers.

---

## `Actions/Twitch Integration/Bits/bits-tier-1.cs`
**Purpose:** Forwards Tier 1 cheer text to Mix It Up with sanitize + TTS pacing wait.

**Global vars:**
- None.

---

## `Actions/Twitch Integration/Bits/bits-tier-2.cs`
**Purpose:** Forwards Tier 2 cheer text to Mix It Up (250-word cap).

**Global vars:**
- None.

---

## `Actions/Twitch Integration/Bits/bits-tier-3.cs`
**Purpose:** Forwards Tier 3 cheer text to Mix It Up (100-word cap).

**Global vars:**
- None.

---

## `Actions/Twitch Integration/Bits/bits-tier-4.cs`
**Purpose:** Forwards Tier 4 cheer text to Mix It Up (10-word cap).

**Global vars:**
- None.

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
