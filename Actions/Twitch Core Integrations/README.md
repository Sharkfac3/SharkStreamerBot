# Twitch Core Integrations Script Reference

## Scope
This folder contains the core Twitch event and stream-state scripts that map to the Streamer.bot `Twitch Core Integrations` action group.

## Script: `stream-start.cs`

### Purpose
Runs stream-start reset logic for Squad, LotAT, and related Twitch integration state.

### Expected Trigger / Input
- Stream start action trigger.

### Required Runtime Variables
- Resets `minigame_active` (bool) to `false` and `minigame_name` (string) to `""` so no stale mini-game lock carries over.
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
- Preserves `clone_unlocked` (bool persisted) so Clone remains unlocked across stream restarts.
- Resets `empire_game_active` (bool) to false.
- Resets `empire_join_active` (bool) to false.
- Resets `empire_game_start_utc` (long) to `0`.
- Resets `empire_players_json` (string) to `[]`.
- Resets `empire_cells_json` (string) to `[]`.
- Resets `pedro_game_enabled` (bool) to false.
- Resets `pedro_mention_count` (int) to `0`.
- Resets `pedro_unlocked` (bool) to false.
- Resets `pedro_last_message_id` (string) to empty.
- Sets `stream_mode` (string) to `workspace`.

### Key Outputs / Side Effects
- Reinitializes session state for stream start.
- Disables timer `Duck - Call Window`.
- Disables timers `Clone - Join Window` and `Clone - Game Tick`.
- Disables timer `Pedro - Call Window`.

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
- The offering-related resets here exist because older experimental offering work shares some globals with LotAT naming.
- For current LotAT v1 planning, do **not** treat these offering resets as proof that offering is part of the active LotAT runtime contract.

## Core Event Scripts
These scripts are base Twitch event bridges for follows and subscriptions.
They are intentionally minimal so they can be expanded later.

### Shared Behavior
- All scripts are Streamer.bot C# action scripts.
- All scripts are prepared to call the Mix It Up Run Command API.
- Subscription scripts in this folder now use configured Mix It Up command IDs; some non-subscription helpers may still use placeholders until they are wired.
- Some scripts in this folder still send an empty `SpecialIdentifiers` object.
- `follower-new.cs`, all 5 dedicated subscription scripts, `subscription-counter-rollover.cs`, and `watch-streak.cs` now send populated special identifiers to Mix It Up.
- No script in this folder interacts with OBS.
- If a command ID is still a placeholder, the script logs a warning and exits safely.

## Dedicated Subscription Scripts (5 scripts, one per event)

Each of the 5 simple subscription events has its own dedicated script. Every script is self-contained — it names the exact event it handles, lists the trigger args available for that event, and has its own command ID constant. No substitutions or guesswork required when pasting.

### Shared behavior across all 5 scripts
- Wire each script to its Streamer.bot action and trigger (see table below).
- Each script already includes its configured Mix It Up command ID.
- All 5 scripts now forward their documented trigger data to Mix It Up as special identifiers.
- All 5 scripts keep `Arguments = ""` and use `SpecialIdentifiers` for structured subscription data.
- If a command ID is ever cleared or replaced with a placeholder, the script logs a warning and exits safely.

| Script | Streamer.bot Action | SB Trigger | Requires SB version |
|---|---|---|---|
| `subscription-new.cs` | Subscription New | Twitch → Subscriptions → Subscription | Any |
| `subscription-renewed.cs` | Subscription Renewed | Twitch → Subscriptions → Resubscription | Any |
| `subscription-prime-paid-upgrade.cs` | Prime Paid Upgrade | Twitch → Subscriptions → Prime Paid Upgrade | v0.2.5+ |
| `subscription-gift-paid-upgrade.cs` | Gift Paid Upgrade | Twitch → Subscriptions → Gift Paid Upgrade | v0.2.5+ |
| `subscription-pay-it-forward.cs` | Pay It Forward | Twitch → Subscriptions → Pay It Forward | v0.2.5+ |

### Mix It Up Actions (all 5 scripts)
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Configured command IDs:
  - `subscription-new.cs` → `4e5b04b7-b5c6-4b17-a6dd-e90efce4a591`
  - `subscription-renewed.cs` → `4af70639-67b5-4d83-8da1-7be0afe1ce76`
  - `subscription-prime-paid-upgrade.cs` → `eabfb607-a4f3-4c45-b26f-1968c3f3f1e7`
  - `subscription-gift-paid-upgrade.cs` → `0bcd98e5-79a4-4767-8e60-58a747d7b7a1`
  - `subscription-pay-it-forward.cs` → `f50ca2a7-cc1d-44c2-b0b8-f4abf9bf2207`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers = { ... }`
  - `IgnoreRequirements = false`
- Special identifiers by script:
  - `subscription-new.cs`
    - `subuser`
    - `subuserid`
    - `subtier`
    - `subtype` = `new`
    - `subismultimonth`
    - `submultimonthduration`
    - `submultimonthtenure`
  - `subscription-renewed.cs`
    - `subuser`
    - `subuserid`
    - `subtier`
    - `subtype` = `renewed`
    - `subcumulative`
    - `submonthstreak`
    - `substreakshared`
    - `subismultimonth`
    - `submultimonthduration`
    - `submultimonthtenure`
  - `subscription-prime-paid-upgrade.cs`
    - `subuser`
    - `subuserid`
    - `subtype` = `primepaidupgrade`
    - `subsystemmessage`
    - `subupgradetier`
    - `subupgradetierstring`
  - `subscription-gift-paid-upgrade.cs`
    - `subuser`
    - `subuserid`
    - `subtype` = `giftpaidupgrade`
  - `subscription-pay-it-forward.cs`
    - `subuser`
    - `subuserid`
    - `subtype` = `payitforward`

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs warning/error messages when configuration is incomplete or the Mix It Up call fails.

### Operator Notes
- `subscription-dispatcher.cs` is the old shared template these replaced — it can be deleted from Streamer.bot once all 5 dedicated actions are wired.
- See the Trigger Variables section below for the full list of available args per event.

---

## Script: `subscription-gift.cs`

### Purpose
Smart handler for all gift subscription events. Wired to TWO triggers in ONE Streamer.bot action. Routes between solo gifts, gift bomb aggregates, and per-recipient gift fires that occur during a bomb.

### How Twitch Gift Events Work
- **Solo gift (1 sub):** `Gift Subscription` fires once — `fromGiftBomb = false`
- **Gift bomb (N subs):** `Gift Bomb` fires once (aggregate) + `Gift Subscription` fires N times (`fromGiftBomb = true` on each)

### Routing Logic
| Condition | Result |
|---|---|
| `gifts` arg present | Gift Bomb event → calls `MIXITUP_COMMAND_ID_GIFT_BOMB` |
| `fromGiftBomb = false` | Solo Gift Subscription → calls `MIXITUP_COMMAND_ID_GIFT_SINGLE` |
| `fromGiftBomb = true` | Individual gift within a bomb → suppressed (logged only) |

Suppression is intentional: the Gift Bomb aggregate event handles the announcement. The N individual `Gift Subscription` events fired during a bomb are noise — they would produce duplicate alerts without suppression.

### Expected Trigger / Input
Wire ONE Streamer.bot action with TWO triggers:
- Trigger 1: Twitch → Subscriptions → Gift Subscription
- Trigger 2: Twitch → Subscriptions → Gift Bomb

Paste `subscription-gift.cs` into the action once. No Set Argument steps needed.

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Calls `MIXITUP_COMMAND_ID_GIFT_SINGLE` for solo gifts.
- Calls `MIXITUP_COMMAND_ID_GIFT_BOMB` for gift bomb aggregates.
- Suppresses (logs only) individual `Gift Subscription` events fired within a bomb.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command IDs:
  - Single gift → `ce197b79-89d1-4943-8f74-b1a690f5a8e4`
  - Gift bomb → `27111920-c34b-4991-b284-57d655a20195`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers = { ... }`
  - `IgnoreRequirements = false`
- Single gift special identifiers:
  - `subuser`
  - `subuserid`
  - `subtier`
  - `subtype` = `gift`
  - `subrecipientuser`
  - `subrecipientid`
  - `subrecipientusername`
  - `subanonymous`
  - `subrandom`
  - `subcumulativemonths`
  - `submonthsgifted`
  - `subfromgiftbomb`
  - `subbombcount`
  - `subsystemmessage`
  - `subtotalgifted`
  - `subtotalgiftedshared`
- Gift bomb special identifiers:
  - `subuser`
  - `subuserid`
  - `subtier`
  - `subtype` = `giftbomb`
  - `subanonymous`
  - `subgifts`
  - `subbonusgifts`
  - `subsystemmessage`
  - `subtotalgifted`
  - `subtotalgiftedshared`

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs info messages identifying which routing branch executed.
- Logs warning/error messages when configuration is incomplete or the Mix It Up call fails.

### Operator Notes
- `subscription-gift.cs` now uses the same `sub*` special identifier naming pattern as the other subscription scripts.
- Use `subtype` inside Mix It Up to branch between `gift` and `giftbomb` behavior.
- The old "Subscription Gift Multiple" Streamer.bot action can be deleted — its function is now handled by this script's Gift Bomb branch.

---

## Script: `follower-new.cs`

### Purpose
Forwards new follower event data to Mix It Up.

### Expected Trigger / Input
- Wire to the Twitch follow event.
- Reads `user` and `userId` from Streamer.bot trigger args.

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Ready to call Mix It Up.
- Keeps `Arguments = ""` for compatibility with the current Mix It Up command.
- Sends follower metadata as Mix It Up special identifiers.
- Logs a warning until a real command ID is configured.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_CORE_FOLLOWER_NEW_COMMAND_ID`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers = { followuser, followuserid, followtype }`
  - `IgnoreRequirements = false`

### Special Identifiers (Mix It Up)
| Key | Source | Mix It Up usage |
|---|---|---|
| `followuser` | `user` trigger arg (empty when missing) | `$followuser` |
| `followuserid` | `userId` trigger arg (empty when missing) | `$followuserid` |
| `followtype` | Literal `new` | `$followtype` |

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs warning/error messages when configuration is incomplete or the Mix It Up call fails.

### Operator Notes
- Replace the placeholder command ID when ready.
- In Mix It Up, create/update the command so it references `$followuser`, `$followuserid`, and `$followtype`.

## Script: `subscription-counter-rollover.cs`

### Purpose
Base handler for a Sub Counter Rollover event — fires when Streamer.bot's internal sub counter hits a configured rollover threshold.

### Expected Trigger / Input
- Wire to Streamer.bot: Twitch → Subscriptions → Sub Counter Rollover.
- Configure the rollover threshold value in the Streamer.bot sub counter settings UI.

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Ready to call Mix It Up.
- Logs a warning until a real command ID is configured.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_CORE_SUBSCRIPTION_COUNTER_ROLLOVER_COMMAND_ID`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers = { subtype, subrollover, subrollovercount, subcounter }`
  - `IgnoreRequirements = false`

### Special Identifiers (Mix It Up)
| Key | Source | Mix It Up usage |
|---|---|---|
| `subtype` | Literal `counterrollover` | `$subtype` |
| `subrollover` | `rollover` trigger arg (as string; `0` when missing) | `$subrollover` |
| `subrollovercount` | `rolloverCount` trigger arg (as string; `0` when missing) | `$subrollovercount` |
| `subcounter` | `subCounter` trigger arg (as string; `0` when missing) | `$subcounter` |

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs warning/error messages when configuration is incomplete or the Mix It Up call fails.

### Operator Notes
- Replace the placeholder command ID when ready.
- In Mix It Up, create/update the command so it references `$subtype`, `$subrollover`, `$subrollovercount`, and `$subcounter`.
- This is a counter event, not a per-user event — Twitch Chat and Twitch User variable groups are NOT available.
- Available trigger args: `rollover` (number — configured threshold), `rolloverCount` (number — times threshold hit), `subCounter` (number — current counter value).

---

## Script: `watch-streak.cs`

### Purpose
Forwards Twitch watch streak data to Mix It Up when a viewer shares their consecutive stream watch streak in chat. This is the first script to use **populated special identifiers** rather than an empty object.

### Expected Trigger / Input
- Wire to: Twitch → Chat → Watch Streak (requires Chat Client v0.2.4+)

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Reads `user`, `watchStreak`, and `message` from Streamer.bot trigger args.
- Forwards the viewer's `message` to Mix It Up unchanged.
- Sends a companion `watchstreaktype` special identifier so Mix It Up can branch safely:
  - `message` when viewer text exists
  - `none` when the viewer message is blank or missing

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `d2d21a86-189c-4a52-8df2-f9d46141af3d`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers = { watchstreakuser, watchstreakmessage, watchstreaktype, watchstreakcount }`
  - `IgnoreRequirements = false`

### Special Identifiers (Mix It Up)
| Key | Source | Mix It Up usage |
|---|---|---|
| `watchstreakuser` | `user` trigger arg | `$watchstreakuser` |
| `watchstreakmessage` | `message` trigger arg (empty when missing) | `$watchstreakmessage` |
| `watchstreaktype` | `message` when viewer text exists, otherwise `none` | `$watchstreaktype` |
| `watchstreakcount` | `watchStreak` trigger arg (as string) | `$watchstreakcount` |

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs info message with the user and streak count.
- Logs warning/error messages when configuration is incomplete or the Mix It Up call fails.

### Operator Notes
- Current command ID is configured from the saved Mix It Up command export.
- In Mix It Up, create/update the command so it references `$watchstreakuser`, `$watchstreakmessage`, `$watchstreaktype`, and `$watchstreakcount`.
- `$watchstreakmessage` is now intentionally empty when Twitch does not provide a viewer-authored message.
- Use `$watchstreaktype` to branch inside Mix It Up instead of relying on a fallback system message.
- This is the first Twitch Core script to use populated special identifiers — use it as a reference pattern for backfilling the subscription and follower scripts.

---

## Related Voice Command Scripts
- Stream mode helpers now live in `Actions/Voice Commands/README.md`.

---

## Trigger Variables

Access in C# via `CPH.TryGetArg("variableName", out T value)`.

### Cheer (Bits)

Triggered under Twitch → Chat → Cheer.

| Variable | Type | Notes |
|---|---|---|
| `user` | string | Display name of the cheering user |
| `userId` | string | Twitch user ID |
| `message` | string | Full chat message including `CheerXXX` tokens |
| `rawInput` | string | Same as `message` — use as fallback when `message` is empty |
| `bits` | number | Amount of bits cheered (use to determine tier thresholds) |

> Scripts strip `CheerXXX` tokens from `message` before forwarding to Mix It Up. Tier thresholds (1–4) are defined in `Actions/SHARED-CONSTANTS.md`.

### Follow

Triggered under Twitch → Channel → Follow.

| Variable | Type | Notes |
|---|---|---|
| `user` | string | Display name of the new follower |
| `userId` | string | Twitch user ID |

### Subscription (New)

Triggered under Twitch → Subscriptions → Subscription.

| Variable | Type | Notes |
|---|---|---|
| `user` | string | Display name of the subscriber |
| `userId` | string | Twitch user ID |
| `tier` | string | `prime`, `tier 1`, `tier 2`, or `tier 3` |
| `isMultiMonth` | bool | Whether this is a multi-month subscription |
| `multiMonthDuration` | number | Total months in the multi-month subscription (3, 6, or 12) |
| `multiMonthTenure` | number | Months already completed in the multi-month subscription |

### Resubscription

Triggered under Twitch → Subscriptions → Resubscription.

| Variable | Type | Notes |
|---|---|---|
| `user` | string | Display name of the resubscriber |
| `userId` | string | Twitch user ID |
| `tier` | string | `prime`, `tier 1`, `tier 2`, or `tier 3` |
| `cumulative` | number | Total cumulative months subscribed |
| `monthStreak` | number | Current consecutive subscription streak |
| `streakShared` | bool | Whether the user publicly shares their streak |
| `isMultiMonth` | bool | Whether this is a multi-month resub |
| `multiMonthDuration` | number | Length of the multi-month resub (3, 6, or 12) |
| `multiMonthTenure` | number | Months already elapsed in the multi-month resub |

### Gift Subscription

Triggered under Twitch → Subscriptions → Gift Subscription. Fires once per individual gifted sub.

| Variable | Type | Notes |
|---|---|---|
| `user` | string | Display name of the gifter |
| `userId` | string | Twitch user ID of the gifter |
| `tier` | string | `tier 1`, `tier 2`, or `tier 3` |
| `recipientUser` | string | Display name of the recipient |
| `recipientId` | string | Twitch user ID of the recipient |
| `recipientUserName` | string | Login name (lowercase) of the recipient |
| `anonymous` | bool | Whether the gift was anonymous |
| `random` | bool | Whether the recipient was randomly selected |
| `cumulativeMonths` | number | Total months the recipient has been subscribed |
| `monthsGifted` | number | Prepaid months gifted (1, 3, 6, or 12) |
| `fromGiftBomb` | bool | Whether this gift was part of a gift bomb event |
| `subBombCount` | number | Total gifts in the associated bomb |
| `systemMessage` | string | Twitch chat system message |
| `totalSubsGifted` | number | Cumulative subs ever gifted by this user |
| `totalSubsGiftedShared` | bool | Whether the gifter publicly shares their total |

### Gift Bomb

Triggered under Twitch → Subscriptions → Gift Bomb. Fires once for the entire multi-gift event.

| Variable | Type | Notes |
|---|---|---|
| `user` | string | Display name of the gifter |
| `userId` | string | Twitch user ID of the gifter |
| `tier` | string | `tier 1`, `tier 2`, or `tier 3` |
| `anonymous` | bool | Whether the bomb was anonymous |
| `gifts` | number | Count of subs in this specific bomb |
| `bonusGifts` | bool | Whether Twitch added bonus subs to the bomb |
| `systemMessage` | string | Twitch chat system message |
| `totalGifts` | number | Total subs ever gifted by this user |
| `totalGiftsShared` | bool | Whether the gifter's total is publicly visible |
| `gift.recipientId#` | string | User ID of recipient at index `#` (0-based) |
| `gift.recipientUser#` | string | Display name of recipient at index `#` |
| `gift.recipientUserName#` | string | Login name of recipient at index `#` |

### Prime Paid Upgrade

Triggered under Twitch → Subscriptions → Prime Paid Upgrade (v0.2.5+).

| Variable | Type | Notes |
|---|---|---|
| `user` | string | Display name of the upgrading user |
| `userId` | string | Twitch user ID |
| `systemMessage` | string | Twitch chat system message |
| `upgradeTier` | number | Numeric tier (e.g. `1000` for Tier 1) |
| `upgradeTierString` | string | Text tier name (e.g. `tier 1`) |

### Gift Paid Upgrade

Triggered under Twitch → Subscriptions → Gift Paid Upgrade (v0.2.5+).

| Variable | Type | Notes |
|---|---|---|
| `user` | string | Display name of the upgrading user |
| `userId` | string | Twitch user ID |

> No additional documented trigger-specific variables beyond the Twitch User shared group.

### Pay It Forward

Triggered under Twitch → Subscriptions → Pay It Forward (v0.2.5+).

| Variable | Type | Notes |
|---|---|---|
| `user` | string | Display name of the user paying it forward |
| `userId` | string | Twitch user ID |

> No additional documented trigger-specific variables beyond the Twitch User shared group.

### Sub Counter Rollover

Triggered under Twitch → Subscriptions → Sub Counter Rollover.

| Variable | Type | Notes |
|---|---|---|
| `rollover` | number | The configured rollover threshold (e.g. `50`) |
| `rolloverCount` | number | How many times the threshold has been reached (e.g. `3`) |
| `subCounter` | number | The current sub counter value (e.g. `20`) |

> This is a counter event — Twitch Chat and Twitch User variable groups are NOT available. Only General and Twitch Broadcaster shared groups apply.

### Watch Streak

Triggered under Twitch → Chat → Watch Streak (Chat Client v0.2.4+).

| Variable | Type | Notes |
|---|---|---|
| `user` | string | Display name of the viewer sharing their streak |
| `userId` | string | Twitch user ID |
| `watchStreak` | int | Number of consecutive streams watched |
| `watchStreakId` | string | Unique identifier for this streak event |
| `copoReward` | int | Channel points Twitch awarded for the streak |
| `message` | string | The viewer's own shared watch streak chat message |
| `systemMessage` | string | Twitch's system message (e.g. "User watched 5 consecutive streams...") |

> All 5 dedicated subscription scripts now forward their documented trigger fields to Mix It Up via special identifiers. Use the per-script lists above when building or updating the matching Mix It Up commands.
