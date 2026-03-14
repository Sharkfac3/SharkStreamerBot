# Twitch Integration Script Reference

## Script: `stream-start.cs`

### Purpose
Runs stream-start reset logic for Squad, LotAT, and related Twitch integration state.

### Expected Trigger / Input
- Stream start action trigger.

### Required Runtime Variables
- Resets `rarity_regular`, `rarity_smol`, `rarity_long`, `rarity_flight`, `rarity_party` (bool) to false.
- Resets `last_roll` (int) to `0`.
- Resets `last_rarity` (string) to empty.
- Resets `last_user` (string) to empty.
- Resets `lotat_active` (bool) to false.
- Resets `lotat_announcement_sent` (bool) to false.
- Resets `lotat_offering_steal_chance` (int) to `0`.
- Resets `lotat_steal_multiplier` (int) to `1`.
- Resets `duck_event_active` (bool) to false.
- Resets `duck_quack_count` (int) to `0`.
- Resets `duck_caller` (string) to empty.
- Resets `duck_unlocked` (bool) to false.
- Resets `clone_unlocked` (bool) to false.
- Resets `clone_game_active` (bool) to false.
- Resets `clone_round` (int) to `0`.
- Resets `clone_positions_open` (string) to empty.
- Resets `clone_winners` (string) to empty.
- Resets `pedro_game_enabled` (bool) to false.
- Resets `pedro_mention_count` (int) to `0`.
- Resets `pedro_unlocked` (bool) to false.
- Resets `pedro_last_message_id` (string) to empty.
- Sets `stream_mode` (string) to `workspace`.

### Key Outputs / Side Effects
- Reinitializes session state for stream start.
- Disables timer `Duck - Call Window`.
- Disables timer `Clone - Volley Timer`.

### Mix It Up Actions
- None.

### OBS Interactions
- For each Toothless rarity source: hide → show → hide on `Disco Party: Workspace`.
- Hides `Duck - Dancing` on `Disco Party: Workspace`.
- Hides `Clone - Dancing` on `Disco Party: Workspace`.
- Hides `Pedro - Dancing` on `Disco Party: Workspace`.

### Wait Behavior
- None.

### Chat / Log Output
- None.

### Operator Notes
- Keep this action early in stream startup order so downstream scripts see clean state.
- Keep shared key/timer/OBS names aligned with `Actions/SHARED-CONSTANTS.md`.

## Mode Scripts
- See `modes/README.md`.

## Bits Docs
- See `Bits/README.md`.

## Redeem Scripts
- See `redeems/README.md`.
