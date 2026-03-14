# Shared Constants

Purpose: keep cross-script names synchronized for Streamer.bot copy/paste workflows.

When you rename any value below, update **all listed scripts** before syncing to Streamer.bot.

---

## OBS
- `OBS_SCENE_DISCO_GARAGE` = `Disco Party: Garage`
- `OBS_SCENE_DISCO_WORKSPACE` = `Disco Party: Workspace`
- `OBS_SCENE_DISCO_GAMER` = `Disco Party: Gamer`

Used in:
- `Actions/Twitch Integration/stream-start.cs`
- `Actions/Twitch Integration/redeems/disco-party.cs`

---

## Stream Mode (shared)
- `VAR_STREAM_MODE` = `stream_mode`
- `MODE_GARAGE` = `garage`
- `MODE_WORKSPACE` = `workspace`
- `MODE_GAMER` = `gamer`

Used in:
- `Actions/Twitch Integration/stream-start.cs`
- `Actions/Twitch Integration/modes/mode-garage.cs`
- `Actions/Twitch Integration/modes/mode-workspace.cs`
- `Actions/Twitch Integration/modes/mode-gamer.cs`
- `Actions/Twitch Integration/redeems/disco-party.cs`

---

## Mini-game Lock (shared)
- `VAR_MINIGAME_ACTIVE` = `minigame_active`
- `VAR_MINIGAME_NAME` = `minigame_name`

Used in:
- `Actions/Squad/Clone/clone-main.cs`
- `Actions/Squad/Clone/clone-volley.cs`
- `Actions/Squad/Duck/duck-main.cs`
- `Actions/Squad/Duck/duck-resolve.cs`
- `Actions/Squad/Pedro/pedro-main.cs`
- `Actions/Squad/Toothless/toothless-main.cs`
- `Actions/Twitch Integration/stream-start.cs`

---

## Duck (shared)
- `VAR_DUCK_EVENT_ACTIVE` = `duck_event_active`
- `VAR_DUCK_QUACK_COUNT` = `duck_quack_count`
- `VAR_DUCK_CALLER` = `duck_caller`
- `VAR_DUCK_UNLOCKED` = `duck_unlocked`
- `OBS_SOURCE_DUCK_DANCING` = `Duck - Dancing`
- `TIMER_DUCK_CALL_WINDOW` = `Duck - Call Window`

Used in:
- `Actions/Squad/Duck/duck-main.cs`
- `Actions/Squad/Duck/duck-call.cs`
- `Actions/Squad/Duck/duck-resolve.cs`
- `Actions/Twitch Integration/stream-start.cs`

---

## Clone (shared)
- `VAR_CLONE_UNLOCKED` = `clone_unlocked`
- `VAR_CLONE_GAME_ACTIVE` = `clone_game_active`
- `VAR_CLONE_ROUND` = `clone_round`
- `VAR_CLONE_POSITIONS_COUNT` = `clone_positions_count`
- `VAR_CLONE_POSITIONS_OPEN` = `clone_positions_open`
- `VAR_CLONE_POSITION_REMOVED_LAST` = `clone_position_removed_last`
- `VAR_CLONE_WINNERS` = `clone_winners`
- `VAR_CLONE_ROUND1_POOL` = `clone_round1_pool`
- `OBS_SOURCE_CLONE_DANCING` = `Clone - Dancing`
- `TIMER_CLONE_VOLLEY` = `Clone - Volley Timer`

Used in:
- `Actions/Squad/Clone/clone-main.cs`
- `Actions/Squad/Clone/clone-position.cs`
- `Actions/Squad/Clone/clone-volley.cs`
- `Actions/Twitch Integration/stream-start.cs`

---

## Pedro (shared)
- `VAR_PEDRO_GAME_ENABLED` = `pedro_game_enabled`
- `VAR_PEDRO_MENTION_COUNT` = `pedro_mention_count`
- `VAR_PEDRO_UNLOCKED` = `pedro_unlocked`
- `VAR_PEDRO_LAST_MESSAGE_ID` = `pedro_last_message_id`
- `OBS_SOURCE_PEDRO_DANCING` = `Pedro - Dancing`
- `TIMER_PEDRO_CALL_WINDOW` = `Pedro - Call Window`

Used in:
- `Actions/Squad/Pedro/pedro-main.cs`
- `Actions/Squad/Pedro/pedro-call.cs`
- `Actions/Squad/Pedro/pedro-resolve.cs`
- `Actions/Twitch Integration/stream-start.cs`

---

## Toothless (shared)
- `PREFIX_RARITY` = `rarity_`
- `PREFIX_BOOST` = `boost_`
- `MEMBER_TOOTHLESS` = `toothless`
- `VAR_LAST_ROLL` = `last_roll`
- `VAR_LAST_RARITY` = `last_rarity`
- `VAR_LAST_USER` = `last_user`
- `OBS_SCENE_DISCO_WORKSPACE` = `Disco Party: Workspace`

Used in:
- `Actions/Squad/Toothless/toothless-main.cs`
- `Actions/Squad/offering.cs`
- `Actions/Twitch Integration/stream-start.cs`

---

## LotAT / Offering (shared)
- `VAR_LOTAT_ACTIVE` = `lotat_active`
- `VAR_LOTAT_ANNOUNCEMENT_SENT` = `lotat_announcement_sent`
- `VAR_LOTAT_OFFERING_STEAL_CHANCE` = `lotat_offering_steal_chance`
- `VAR_LOTAT_STEAL_MULTIPLIER` = `lotat_steal_multiplier`
- `PREFIX_BOOST` = `boost_`

Used in:
- `Actions/Squad/offering.cs`
- `Actions/Twitch Integration/stream-start.cs`

---

## Commanders (shared)
- `ARG_USER` = `user`
- `VAR_CURRENT_CAPTAIN_STRETCH` = `current_captain_stretch`
- `VAR_CURRENT_THE_DIRECTOR` = `current_the_director`
- `VAR_CURRENT_WATER_WIZARD` = `current_water_wizard`
- `VAR_CAPTAIN_STRETCH_THANK_COUNT` = `captain_stretch_thank_count`
- `VAR_CAPTAIN_STRETCH_STRETCH_NEXT_ALLOWED_UTC` = `captain_stretch_stretch_next_allowed_utc`
- `VAR_CAPTAIN_STRETCH_SHRIMP_NEXT_ALLOWED_UTC` = `captain_stretch_shrimp_next_allowed_utc`
- `VAR_CAPTAIN_STRETCH_THANK_HIGH_SCORE` = `captain_stretch_thank_high_score` *(persisted)*
- `VAR_CAPTAIN_STRETCH_THANK_HIGH_SCORE_USER` = `captain_stretch_thank_high_score_user` *(persisted)*
- `VAR_THE_DIRECTOR_AWARD_COUNT` = `the_director_award_count`
- `VAR_THE_DIRECTOR_CHECKCHAT_NEXT_ALLOWED_UTC` = `the_director_checkchat_next_allowed_utc`
- `VAR_THE_DIRECTOR_TOAD_NEXT_ALLOWED_UTC` = `the_director_toad_next_allowed_utc`
- `VAR_THE_DIRECTOR_AWARD_HIGH_SCORE` = `the_director_award_high_score` *(persisted)*
- `VAR_THE_DIRECTOR_AWARD_HIGH_SCORE_USER` = `the_director_award_high_score_user` *(persisted)*
- `VAR_WATER_WIZARD_HAIL_COUNT` = `water_wizard_hail_count`
- `VAR_WATER_WIZARD_HYDRATE_NEXT_ALLOWED_UTC` = `water_wizard_hydrate_next_allowed_utc`
- `VAR_WATER_WIZARD_ORB_NEXT_ALLOWED_UTC` = `water_wizard_orb_next_allowed_utc`
- `VAR_WATER_WIZARD_HAIL_HIGH_SCORE` = `water_wizard_hail_high_score` *(persisted)*
- `VAR_WATER_WIZARD_HAIL_HIGH_SCORE_USER` = `water_wizard_hail_high_score_user` *(persisted)*

Used in:
- `Actions/Commanders/Captain Stretch/captain-stretch-redeem.cs`
- `Actions/Commanders/Captain Stretch/captain-stretch-thank.cs`
- `Actions/Commanders/Captain Stretch/captain-stretch-stretch.cs`
- `Actions/Commanders/Captain Stretch/captain-stretch-shrimp.cs`
- `Actions/Commanders/The Director/the-director-redeem.cs`
- `Actions/Commanders/The Director/the-director-award.cs`
- `Actions/Commanders/The Director/the-director-checkchat.cs`
- `Actions/Commanders/The Director/the-director-toad.cs`
- `Actions/Commanders/Water Wizard/water-wizard-redeem.cs`
- `Actions/Commanders/Water Wizard/water-wizard-hail.cs`
- `Actions/Commanders/Water Wizard/wizard-hydrate.cs`
- `Actions/Commanders/Water Wizard/water-wizard-orb.cs`

---

## Bits (shared)
- `ARG_MESSAGE` = `message`
- `ARG_RAW_INPUT` = `rawInput`
- `MIXITUP_PLATFORM_TWITCH` = `Twitch`
- `CHEER_TOKEN_REGEX` = `\bcheer\d+\b`
- `WHITESPACE_REGEX` = `\s+`
- `WAIT_BASE_PREP_MS` = `3000`
- `WAIT_MS_PER_WORD` = `400`
- `WAIT_TAIL_BUFFER_MS` = `500`

Used in:
- `Actions/Twitch Integration/Bits/bits-tier-1.cs`
- `Actions/Twitch Integration/Bits/bits-tier-2.cs`
- `Actions/Twitch Integration/Bits/bits-tier-3.cs`
- `Actions/Twitch Integration/Bits/bits-tier-4.cs`

---

## Operator Sync Notes
1. Update constants in source files in this repo.
2. Paste updated scripts into Streamer.bot actions.
3. Run smoke tests:
   - stream-start reset,
   - stream mode switching (garage/workspace/gamer),
   - Duck start/call/resolve,
   - Clone start/position/volley,
   - Pedro start/mentions/unlock,
   - Toothless roll + first-time unlock,
   - Offering happy path + LotAT steal path,
   - Commander redeems + !hail/!thank/!award scoring for all three slots,
   - Bits tiers 1-4 cheer forwarding + wait behavior.
