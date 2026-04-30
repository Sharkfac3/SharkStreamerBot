# Shared Constants

Purpose: keep cross-script names synchronized for Streamer.bot copy/paste workflows.

When you rename any value below, update **all listed scripts** before syncing to Streamer.bot.

---

## OBS
- `OBS_SCENE_DISCO_GARAGE` = `Disco Party: Garage`
- `OBS_SCENE_DISCO_WORKSPACE` = `Disco Party: Workspace`
- `OBS_SCENE_DISCO_GAMER` = `Disco Party: Gamer`

Used in:
- `Actions/Twitch Core Integrations/stream-start.cs`
- `Actions/Twitch Channel Points/disco-party.cs`

---

## Stream Mode (shared)
- `VAR_STREAM_MODE` = `stream_mode`
- `MODE_GARAGE` = `garage`
- `MODE_WORKSPACE` = `workspace`
- `MODE_GAMER` = `gamer`

Used in:
- `Actions/Twitch Core Integrations/stream-start.cs`
- `Actions/Voice Commands/mode-garage.cs`
- `Actions/Voice Commands/mode-workspace.cs`
- `Actions/Voice Commands/mode-gamer.cs`
- `Actions/Twitch Channel Points/disco-party.cs`

---

## Disco Party (shared)
- `VAR_DISCO_PARTY_ACTIVE` = `disco_party_active` — re-entry guard; true while the 60s dance sequence is running
- `VAR_DISCO_PARTY_PREV_SCENE` = `disco_party_prev_scene` — OBS scene name saved before switching to Disco; used to return afterward

Used in:
- `Actions/Twitch Channel Points/disco-party.cs`
- `Actions/Twitch Core Integrations/stream-start.cs`

Operator notes:
- Both vars are non-persisted. `stream-start.cs` resets them to `false` / `""` at stream start.
- Dance command IDs (one per squad member/Toothless rarity) live as constants inside `disco-party.cs`.
  Replace all `REPLACE_WITH_*` placeholders after creating "Squad - \<Member\> - Dance" commands in Mix It Up.

---

## Mini-game Lock (shared)
- `VAR_MINIGAME_ACTIVE` = `minigame_active`
- `VAR_MINIGAME_NAME` = `minigame_name`

Used in:
- `Actions/Squad/Clone/clone-empire-main.cs`
- `Actions/Squad/Clone/clone-empire-move.cs`
- `Actions/Squad/Clone/clone-empire-tick.cs`
- `Actions/Squad/Duck/duck-main.cs`
- `Actions/Squad/Duck/duck-call.cs`
- `Actions/Squad/Duck/duck-resolve.cs`
- `Actions/Squad/Pedro/pedro-main.cs`
- `Actions/Squad/Toothless/toothless-main.cs`
- `Actions/Twitch Core Integrations/stream-start.cs`

---

## Duck (shared)
- `VAR_DUCK_EVENT_ACTIVE` = `duck_event_active`
- `VAR_DUCK_QUACK_COUNT` = `duck_quack_count`
- `VAR_DUCK_CALLER` = `duck_caller`
- `VAR_DUCK_UNLOCKED` = `duck_unlocked`
- `VAR_DUCK_TARGET_QUACKS` = `duck_target_quacks`
- `VAR_DUCK_UNIQUE_QUACKERS` = `duck_unique_quackers`
- `VAR_DUCK_UNIQUE_QUACKER_COUNT` = `duck_unique_quacker_count`
- `OBS_SOURCE_DUCK_DANCING` = `Duck - Dancing`
- `TIMER_DUCK_CALL_WINDOW` = `Duck - Call Window`

Used in:
- `Actions/Squad/Duck/duck-main.cs`
- `Actions/Squad/Duck/duck-call.cs`
- `Actions/Squad/Duck/duck-resolve.cs`
- `Actions/Twitch Core Integrations/stream-start.cs`

---

## Clone Grid Game (shared)

> The old Clone musical-chairs game has been replaced by the Empire Grid survival game.
> `clone_unlocked` and `OBS_SOURCE_CLONE_DANCING` are preserved for Disco Party integration.

### Preserved from old Clone
- `VAR_CLONE_UNLOCKED` = `clone_unlocked` *(persisted bool; true once Clone is unlocked)*
- `OBS_SOURCE_CLONE_DANCING` = `Clone - Dancing` *(OBS source shown during unlock celebration)*

### Grid game — global state vars (non-persisted)
- `VAR_EMPIRE_GAME_ACTIVE`    = `empire_game_active`    — true while game is running (join or movement phase)
- `VAR_EMPIRE_JOIN_ACTIVE`    = `empire_join_active`    — true only during 60-second join window
- `VAR_EMPIRE_GAME_START_UTC` = `empire_game_start_utc` — Unix ms when movement phase opened (0 = not started)
- `VAR_EMPIRE_PLAYERS_JSON`   = `empire_players_json`   — JSON array of living players (see structure below)
- `VAR_EMPIRE_CELLS_JSON`     = `empire_cells_json`     — JSON array of empire cells (see structure below)

### Grid game — timers
- `TIMER_CLONE_JOIN_WINDOW` = `Clone - Join Window`   — 60-second one-shot; fires clone-empire-start.cs
- `TIMER_CLONE_GAME_TICK`   = `Clone - Game Tick`     — 5-second repeating; fires clone-empire-tick.cs

### Grid game — constants (not stored in globals; defined as C# constants in scripts)
- `EMPIRE_GRID_COLS`          = `32`
- `EMPIRE_GRID_ROWS`          = `18`
- `EMPIRE_SPAWN_COL`          = `16`   (center column)
- `EMPIRE_SPAWN_ROW`          = `9`    (center row)
- `EMPIRE_JOIN_WINDOW_S`      = `60`   (seconds)
- `EMPIRE_WIN_DURATION_MS`    = `300000` (5 minutes in ms)
- `EMPIRE_INACTIVITY_KILL_MS` = `30000`  (30 seconds in ms)
- `EMPIRE_INITIAL_COUNT`      = `5`   (empire ships spawned at game start)
- `EMPIRE_MOVE_COOLDOWN_MS`   = `1000` (1 second per-player move cooldown)
- `MINIGAME_NAME_CLONE_EMPIRE`= `clone_empire`

### JSON structures stored in global vars

**empire_players_json** — array of living player objects:
```json
[
  { "userId": "12345", "userName": "alice", "col": 16, "row": 9, "lastMoveUtc": 1714000000000 }
]
```
`lastMoveUtc` is set to `empire_game_start_utc` when movement opens. Used for inactivity detection.

**empire_cells_json** — array of empire cell positions:
```json
[
  { "col": 5, "row": 3 },
  { "col": 12, "row": 7 }
]
```

### Used in
- `Actions/Squad/Clone/clone-empire-main.cs`
- `Actions/Squad/Clone/clone-empire-join.cs`
- `Actions/Squad/Clone/clone-empire-start.cs`
- `Actions/Squad/Clone/clone-empire-move.cs`
- `Actions/Squad/Clone/clone-empire-tick.cs`
- `Actions/Twitch Core Integrations/stream-start.cs`

---

## Pedro (shared)
- `VAR_PEDRO_GAME_ENABLED` = `pedro_game_enabled`
- `VAR_PEDRO_MENTION_COUNT` = `pedro_mention_count`
- `VAR_PEDRO_UNLOCKED` = `pedro_unlocked`
- `VAR_PEDRO_NEXT_ALLOWED_UTC` = `pedro_next_allowed_utc`
- `VAR_PEDRO_SECRET_UNLOCK_ACTIVE` = `pedro_secret_unlock_active`
- `VAR_PEDRO_LAST_MESSAGE_ID` = `pedro_last_message_id`
- `OBS_SOURCE_PEDRO_DANCING` = `Pedro - Dancing`
- `TIMER_PEDRO_CALL_WINDOW` = `Pedro - Call Window`

Used in:
- `Actions/Squad/Pedro/pedro-main.cs`
- `Actions/Squad/Pedro/pedro-call.cs`
- `Actions/Squad/Pedro/pedro-resolve.cs`
- `Actions/Twitch Core Integrations/stream-start.cs`

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
- `Actions/Twitch Core Integrations/stream-start.cs`

---

## LotAT / Offering (shared)
- `VAR_LOTAT_ACTIVE` = `lotat_active` *(active LotAT session flag; do not treat as an offering toggle in LotAT v1 docs/implementation)*
- `VAR_LOTAT_SESSION_ID` = `lotat_session_id`
- `VAR_LOTAT_SESSION_STAGE` = `lotat_session_stage`
- `VAR_LOTAT_SESSION_STORY_ID` = `lotat_session_story_id`
- `VAR_LOTAT_SESSION_CURRENT_NODE_ID` = `lotat_session_current_node_id`
- `VAR_LOTAT_SESSION_CHAOS_TOTAL` = `lotat_session_chaos_total`
- `VAR_LOTAT_SESSION_ROSTER_FROZEN` = `lotat_session_roster_frozen`
- `VAR_LOTAT_SESSION_JOINED_ROSTER_JSON` = `lotat_session_joined_roster_json`
- `VAR_LOTAT_SESSION_JOINED_COUNT` = `lotat_session_joined_count`
- `VAR_LOTAT_NODE_ACTIVE_WINDOW` = `lotat_node_active_window`
- `VAR_LOTAT_NODE_WINDOW_RESOLVED` = `lotat_node_window_resolved`
- `VAR_LOTAT_NODE_ALLOWED_COMMANDS_JSON` = `lotat_node_allowed_commands_json`
- `VAR_LOTAT_VOTE_MAP_JSON` = `lotat_vote_map_json`
- `VAR_LOTAT_VOTE_VALID_COUNT` = `lotat_vote_valid_count`
- `VAR_LOTAT_NODE_COMMANDER_NAME` = `lotat_node_commander_name`
- `VAR_LOTAT_NODE_COMMANDER_TARGET_USER` = `lotat_node_commander_target_user`
- `VAR_LOTAT_NODE_COMMANDER_ALLOWED_COMMANDS_JSON` = `lotat_node_commander_allowed_commands_json`
- `VAR_LOTAT_NODE_DICE_SUCCESS_THRESHOLD` = `lotat_node_dice_success_threshold`
- `VAR_LOTAT_SESSION_LAST_CHOICE_ID` = `lotat_session_last_choice_id`
- `VAR_LOTAT_SESSION_LAST_END_STATE` = `lotat_session_last_end_state` *(recommended v1 history breadcrumb; safe idle default is empty string)*
- `VAR_LOTAT_ANNOUNCEMENT_SENT` = `lotat_announcement_sent` *(legacy / provisional offering-system latch)*
- `VAR_LOTAT_OFFERING_STEAL_CHANCE` = `lotat_offering_steal_chance` *(legacy / provisional offering variable; not active LotAT v1 engine contract)*
- `VAR_LOTAT_STEAL_MULTIPLIER` = `lotat_steal_multiplier` *(legacy / provisional offering variable; not active LotAT v1 engine contract)*
- `TIMER_LOTAT_JOIN_WINDOW` = `LotAT - Join Window`
- `TIMER_LOTAT_DECISION_WINDOW` = `LotAT - Decision Window`
- `TIMER_LOTAT_COMMANDER_WINDOW` = `LotAT - Commander Window`
- `TIMER_LOTAT_DICE_WINDOW` = `LotAT - Dice Window`
- `PREFIX_BOOST` = `boost_` *(external boost-system prefix; not LotAT v1 engine state by itself)*

Used in:
- `Actions/Squad/offering.cs`
- `Actions/Twitch Core Integrations/stream-start.cs`
- `Actions/LotAT/` *(planned engine implementation)*

Operator note:
- These are timer **names only**. V1 timer durations stay in the runtime contract / implementation layer.
- Runtime defaults to preserve when implementation begins: join = `120s`, decision = `120s`.
- Stream start should disable all four LotAT timers to clear stale state before returning LotAT to `idle`.
- Current LotAT v1 contract boundary: `!offering` remains out of scope until a future explicit integration decision is documented.

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
- `Actions/Commanders/commander-help.cs`
- `Actions/Commanders/commanders.cs`
- `Actions/Commanders/Captain Stretch/captain-stretch-redeem.cs`
- `Actions/Commanders/Captain Stretch/captain-stretch-thank.cs`
- `Actions/Commanders/Captain Stretch/captain-stretch-stretch.cs`
- `Actions/Commanders/Captain Stretch/captain-stretch-shrimp.cs`
- `Actions/Commanders/Captain Stretch/captain-stretch-generalfocus.cs`
- `Actions/Commanders/The Director/the-director-redeem.cs`
- `Actions/Commanders/The Director/the-director-award.cs`
- `Actions/Commanders/The Director/the-director-checkchat.cs`
- `Actions/Commanders/The Director/the-director-toad.cs`
- `Actions/Commanders/The Director/the-director-primary.cs`
- `Actions/Commanders/The Director/the-director-secondary.cs`

### The Director — Scene Source Map

`the-director-primary.cs` and `the-director-secondary.cs` each contain a `SCENE_SOURCE_MAP` dictionary
that maps OBS scene names to a `(primarySource, secondarySource)` pair.

When The Director uses `!primary` or `!secondary`, the script reads the current OBS scene,
looks it up in the map, then shows one source and hides the other.

**Before adding any new scene source switching logic**, check this map first.
**When adding a new scene entry**, update `SCENE_SOURCE_MAP` in **both** scripts and add the
source names to the table below.

| OBS Scene | Primary Source | Secondary Source |
|---|---|---|
| `Workspace: Main` | `Main Screen Capture` | `Quest POV` |
| *(others TBD — add rows here as sources are confirmed)* | | |
- `Actions/Commanders/Water Wizard/water-wizard-redeem.cs`
- `Actions/Commanders/Water Wizard/water-wizard-hail.cs`
- `Actions/Commanders/Water Wizard/wizard-hydrate.cs`
- `Actions/Commanders/Water Wizard/water-wizard-orb.cs`
- `Actions/Commanders/Water Wizard/water-wizard-castrest.cs`

---

## Rest / Focus Loop (shared)
- `VAR_REST_FOCUS_LOOP_ACTIVE` = `rest_focus_loop_active`
- `VAR_REST_FOCUS_LOOP_PHASE` = `rest_focus_loop_phase`
- `PHASE_IDLE` = `idle`
- `PHASE_PRE_REST` = `pre_rest`
- `PHASE_REST` = `rest`
- `PHASE_PRE_FOCUS` = `pre_focus`
- `PHASE_FOCUS` = `focus`
- `TIMER_REST_FOCUS_PRE_REST` = `Rest Focus - Pre Rest`
- `TIMER_REST_FOCUS_REST` = `Rest Focus - Rest`
- `TIMER_REST_FOCUS_PRE_FOCUS` = `Rest Focus - Pre Focus`
- `TIMER_REST_FOCUS_FOCUS` = `Rest Focus - Focus`

Used in:
- `Actions/Rest Focus Loop/rest-focus-loop-start.cs`
- `Actions/Rest Focus Loop/rest-focus-pre-rest-end.cs`
- `Actions/Rest Focus Loop/rest-focus-rest-end.cs`
- `Actions/Rest Focus Loop/rest-focus-pre-focus-end.cs`
- `Actions/Rest Focus Loop/rest-focus-focus-end.cs`
- `Actions/Commanders/Water Wizard/water-wizard-castrest.cs`
- `Actions/Commanders/Captain Stretch/captain-stretch-generalfocus.cs`
- `Actions/Twitch Core Integrations/stream-start.cs`

---

## Temporary (shared)
- `TIMER_TEMP_FOCUS` = `Temp Focus Timer`

Used in:
- `Actions/Temporary/temp-focus-timer-start.cs`
- `Actions/Temporary/temp-focus-timer-end.cs`
- `Actions/Twitch Core Integrations/stream-start.cs`

---

## Bits (shared)
- `ARG_MESSAGE_STRIPPED` = `messageStripped`
- `ARG_MESSAGE` = `message`
- `ARG_RAW_INPUT` = `rawInput`
- `MIXITUP_PLATFORM_TWITCH` = `Twitch`
- `WAIT_BASE_PREP_MS` = `3000`
- `WAIT_MS_PER_WORD` = `400`
- `WAIT_TAIL_BUFFER_MS` = `500`

## Mix It Up Unlock Pacing (shared guidance)
- `WAIT_MIXITUP_UNLOCK_STARTUP_MS` = `3000`

Use in:
- Any `Actions/Squad/**` unlock flow that triggers a Mix It Up command and then waits for an unlock animation, dance, TTS, or other time-based payoff.
- Other non-Squad unlock/reveal flows when the wait should include Mix It Up startup time before the visible/audible effect begins.

Operator rule:
- Default to including this extra `3000ms` startup buffer whenever a script triggers a Mix It Up unlock command and then waits for the unlock moment to finish.
- If a future flow proves it does **not** need the startup buffer, document that exception in the feature README instead of silently omitting it.

Used in:
- `Actions/Twitch Bits Integrations/bits-tier-1.cs`
- `Actions/Twitch Bits Integrations/bits-tier-2.cs`
- `Actions/Twitch Bits Integrations/bits-tier-3.cs`
- `Actions/Twitch Bits Integrations/bits-tier-4.cs`

---

## Overlay / Broker (shared)

- `BROKER_WS_INDEX` = `0` *(integer — position of the broker entry in Streamer.bot → Servers/Clients → WebSocket Clients; first entry = 0)*
- `VAR_BROKER_CONNECTED` = `broker_connected` *(non-persisted boolean; true when WebSocket to broker is live)*
- `BROKER_CLIENT_NAME` = `"streamerbot"` *(sent in ClientHello; must match CLIENT_NAMES.STREAMERBOT in @stream-overlay/shared/topics.ts)*

Topic string constants used in C# scripts (mirror TOPICS in @stream-overlay/shared/topics.ts).
Each script defines these as private constants locally — copy them from here to stay in sync.

**Overlay — generic asset commands** (used in `Actions/Overlay/`):
- `TOPIC_OVERLAY_SPAWN`          = `"overlay.spawn"`
- `TOPIC_OVERLAY_MOVE`           = `"overlay.move"`
- `TOPIC_OVERLAY_ANIMATE`        = `"overlay.animate"`
- `TOPIC_OVERLAY_REMOVE`         = `"overlay.remove"`
- `TOPIC_OVERLAY_CLEAR`          = `"overlay.clear"`

**Overlay — audio** (used in any action that plays/stops sounds):
- `TOPIC_OVERLAY_AUDIO_PLAY`     = `"overlay.audio.play"`
- `TOPIC_OVERLAY_AUDIO_STOP`     = `"overlay.audio.stop"`

**LotAT session lifecycle** (used in `Actions/LotAT/overlay-publish.cs`):
- `TOPIC_LOTAT_SESSION_START`    = `"lotat.session.start"`
- `TOPIC_LOTAT_SESSION_END`      = `"lotat.session.end"`
- `TOPIC_LOTAT_JOIN_OPEN`        = `"lotat.join.open"`
- `TOPIC_LOTAT_JOIN_UPDATE`      = `"lotat.join.update"`
- `TOPIC_LOTAT_JOIN_CLOSE`       = `"lotat.join.close"`
- `TOPIC_LOTAT_NODE_ENTER`       = `"lotat.node.enter"`
- `TOPIC_LOTAT_VOTE_OPEN`        = `"lotat.vote.open"`
- `TOPIC_LOTAT_VOTE_CAST`        = `"lotat.vote.cast"`
- `TOPIC_LOTAT_VOTE_CLOSE`       = `"lotat.vote.close"`
- `TOPIC_LOTAT_DICE_OPEN`        = `"lotat.dice.open"`
- `TOPIC_LOTAT_DICE_ROLL`        = `"lotat.dice.roll"`
- `TOPIC_LOTAT_DICE_CLOSE`       = `"lotat.dice.close"`
- `TOPIC_LOTAT_COMMANDER_OPEN`   = `"lotat.commander.open"`
- `TOPIC_LOTAT_COMMANDER_CLOSE`  = `"lotat.commander.close"`
- `TOPIC_LOTAT_CHAOS_UPDATE`     = `"lotat.chaos.update"`

**Squad mini-games** (used in `Actions/Squad/*/overlay-publish.cs`):
- `TOPIC_SQUAD_DUCK_START`       = `"squad.duck.start"`
- `TOPIC_SQUAD_DUCK_UPDATE`      = `"squad.duck.update"`
- `TOPIC_SQUAD_DUCK_END`         = `"squad.duck.end"`
- `TOPIC_SQUAD_CLONE_START`      = `"squad.clone.start"`
- `TOPIC_SQUAD_CLONE_UPDATE`     = `"squad.clone.update"`
- `TOPIC_SQUAD_CLONE_END`        = `"squad.clone.end"`
- `TOPIC_SQUAD_PEDRO_START`      = `"squad.pedro.start"`
- `TOPIC_SQUAD_PEDRO_UPDATE`     = `"squad.pedro.update"`
- `TOPIC_SQUAD_PEDRO_END`        = `"squad.pedro.end"`
- `TOPIC_SQUAD_TOOTHLESS_START`  = `"squad.toothless.start"`
- `TOPIC_SQUAD_TOOTHLESS_UPDATE` = `"squad.toothless.update"`
- `TOPIC_SQUAD_TOOTHLESS_END`    = `"squad.toothless.end"`

Used in:
- `Actions/Overlay/broker-connect.cs`
- `Actions/Overlay/broker-publish.cs`
- `Actions/Overlay/broker-disconnect.cs`
- `Actions/Overlay/test-overlay.cs`
- `Actions/LotAT/overlay-publish.cs`
- `Actions/Squad/Duck/overlay-publish.cs`
- `Actions/Squad/Pedro/overlay-publish.cs`
- `Actions/Squad/Toothless/overlay-publish.cs`
- `Actions/Twitch Core Integrations/stream-start.cs` *(ensures broker connection/ClientHello at startup)*

Operator notes:
- `broker_connected` is non-persisted. It is set to `false` before a Streamer.bot-side connect/register attempt and to `true`
  when the broker ClientHello handshake is sent successfully.
- `stream-start.cs` now includes the broker connect/register logic directly, so a separate broker-connect sub-action is optional for manual reconnects rather than required for normal stream start.
- `BROKER_WS_INDEX` must match the position of the "Overlay Broker" entry in the Streamer.bot
  WebSocket Clients list (Servers/Clients → WebSocket Clients). Default is 0 (first entry).

---

## XJ Drivethrough (shared)
- `XJ_ASSET_ID`           = `xj-drivethrough`
- `XJ_ASSET_SRC`          = `images/xj-drivethrough.png`
- `XJ_SOUND_ID`           = `xj-rev-limiter`
- `XJ_SOUND_SRC`          = `audio/xj-rev-limiter.mp3`
- `XJ_WIDTH`              = `400` *(display width in pixels)*
- `XJ_HEIGHT`             = `250` *(display height in pixels)*
- `XJ_DEPTH`              = `20`
- `XJ_Y`                  = `750` *(vertical canvas position — lower-third road position on 1920×1080)*
- `XJ_START_X`            = `-200` *(image center starts off-screen left; right edge at x=0)*
- `XJ_END_X`              = `2120` *(image center exits off-screen right; left edge at x=1920)*
- `XJ_DRIVE_DURATION_MS`  = `10000` *(10 seconds left-to-right tween)*
- `VAR_XJ_ACTIVE`         = `xj_drivethrough_active` *(non-persisted bool; true while sequence is running)*
- `XJ_CHANCE_MIN`         = `1` *(inclusive lower bound for random chance rolls)*
- `XJ_CHANCE_MAX_EXCLUSIVE` = `101` *(exclusive upper bound; produces rolls 1-100)*
- `XJ_TRIGGER_THRESHOLD`  = `20` *(roll must be greater than this; 21-100 trigger the drivethrough)*
- `XJ_COMMANDER_WIDTH`    = `640` *(commander piece display width in pixels; matches 640×640 source assets)*
- `XJ_COMMANDER_HEIGHT`   = `640` *(commander piece display height in pixels; matches 640×640 source assets)*

Used in:
- `Actions/XJ Drivethrough/xj-drivethrough-main.cs`
- `Actions/Twitch Core Integrations/stream-start.cs`

Operator notes:
- Image must exist at: `Apps/stream-overlay/packages/overlay/public/images/xj-drivethrough.png`
  Use a side-facing XJ image (facing right). 400×250 px recommended.
- Commander piece images must exist at `Apps/stream-overlay/packages/overlay/public/images/xj-left.png`, `xj-middle.png`, and `xj-right.png`. Source and display size should be 640×640 px to avoid distortion.
- Audio must exist at: `Apps/stream-overlay/packages/overlay/public/audio/xj-rev-limiter.mp3`
- OBS browser source must have audio enabled for the rev limiter sound to hit stream output.

---

## Destroyer (shared)
- `DESTROYER_ASSET_ID`       = `destroyer`
- `DESTROYER_ASSET_SRC`      = `images/destroyer.jpg`
- `DESTROYER_START_X`        = `960`
- `DESTROYER_START_Y`        = `540`
- `DESTROYER_SIZE`           = `200` *(width and height in pixels)*
- `DESTROYER_HALF_SIZE`      = `100` *(half of DESTROYER_SIZE; used for boundary clamping)*
- `DESTROYER_STEP`           = `50` *(pixels per !up/!down/!left/!right command)*
- `DESTROYER_TWEEN_MS`       = `150` *(move tween duration in ms)*
- `DESTROYER_LIFETIME_MS`    = `300000` *(5 minutes; overlay auto-removes after this)*
- `DESTROYER_DEPTH`          = `10`
- `VAR_DESTROYER_ACTIVE`     = `destroyer_active` *(non-persisted bool; true while on screen)*
- `VAR_DESTROYER_X`          = `destroyer_x` *(non-persisted int; current canvas x)*
- `VAR_DESTROYER_Y`          = `destroyer_y` *(non-persisted int; current canvas y)*
- `VAR_DESTROYER_EXPIRE_UTC` = `destroyer_expire_utc` *(non-persisted long; unix ms when lifetime ends)*

Used in:
- `Actions/Destroyer/destroyer-spawn.cs`
- `Actions/Destroyer/destroyer-move.cs`

Operator notes:
- `!destroyer` spawns the image; `!up` / `!down` / `!left` / `!right` move it.
- All four move commands should trigger the **same** destroyer-move.cs action in Streamer.bot.
- `destroyer_active` resets to false automatically when the first move command fires after expiry.
- Image is 200×200px at Phaser center-origin. Boundary clamp keeps the full image on canvas.
- `destroyer.jpg` must exist at `Apps/stream-overlay/packages/overlay/public/images/destroyer.jpg`.

---

## Info Service / Assets (shared)

- `ASSETS_ROOT` = `C:\Users\sharkfac3\Workspace\coding\SharkStreamerBot\Assets`
- `INFO_SERVICE_URL` = `http://127.0.0.1:8766` — base URL for all info-service HTTP requests from Streamer.bot
- `INFO_SERVICE_PORT` = `8766` — info-service listen port
- `PRODUCTION_MANAGER_PORT` = `5174` — production-manager dev server port
- `ASSETS_USER_INTROS_SOUND_SUBPATH` = `user-intros/sound/` — relative path from `ASSETS_ROOT` to user-intro sound files
- `ASSETS_USER_INTROS_GIF_SUBPATH` = `user-intros/gif/` — relative path from `ASSETS_ROOT` to user-intro gif files
- `COLLECTION_USER_INTROS` = `"user-intros"` — collection name for the user-intros collection
- `COLLECTION_PENDING_INTROS` = `"pending-intros"` — collection name for the pending-intros collection

Operator notes:
- `ASSETS_ROOT` is machine-specific. Value above is set for this machine. If deploying on a different machine, update to that machine's absolute path. No trailing slash.

Used in:
- `Actions/Intros/first-chat-intro.cs`
- `Actions/Intros/redeem-capture.cs`

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
