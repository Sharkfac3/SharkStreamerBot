---
id: constants-effects
type: constants
description: Rest/Focus loop, Bits/unlock pacing, XJ Drivethrough, Destroyer, Info Service, and Temporary constants.
owner: streamerbot-dev
parent: ../SHARED-CONSTANTS.md
---

# Effects Constants

Canonical constants for stream effects, overlays, and auxiliary systems.

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
- `XJ_COMMANDER_WIDTH`    = `640` *(commander piece display width in pixels; matches 640×640 source assets)*
- `XJ_COMMANDER_HEIGHT`   = `640` *(commander piece display height in pixels; matches 640×640 source assets)*
- `XJ_TRIGGER_THRESHOLD`  = `75` *(roll must be greater than this; 76-100 trigger the drivethrough)*

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
