# Twitch Core Integrations Script Reference

## Scope
This folder contains the core Twitch event and stream-state scripts that map to the Streamer.bot `Twitch Core Integrations` action group.

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

## Core Event Scripts
These scripts are base Twitch event bridges for follows and subscriptions.
They are intentionally minimal so they can be expanded later.

### Shared Behavior
- All scripts are Streamer.bot C# action scripts.
- All scripts are prepared to call the Mix It Up Run Command API.
- All scripts currently use placeholder command IDs.
- All scripts currently send an empty `SpecialIdentifiers` object.
- No script in this folder interacts with OBS.
- If a command ID is still a placeholder, the script logs a warning and exits safely.

## Script: `subscription-gift-single.cs`

### Purpose
Base handler for a single gifted subscription event.

### Expected Trigger / Input
- Wire to the Twitch subscription gift event that represents exactly 1 gifted sub.

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Ready to call Mix It Up.
- Logs a warning until a real command ID is configured.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_CORE_SUBSCRIPTION_GIFT_SINGLE_COMMAND_ID`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers = { }`
  - `IgnoreRequirements = false`

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs warning/error messages when configuration is incomplete or the Mix It Up call fails.

### Operator Notes
- Replace the placeholder command ID when ready.
- Add argument/special identifier mapping later when the event contract is finalized.

## Script: `subscription-gift-multiple.cs`

### Purpose
Base handler for a multi-gift subscription event.

### Expected Trigger / Input
- Wire to the Twitch subscription gift event that represents more than 1 gifted sub.

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Ready to call Mix It Up.
- Logs a warning until a real command ID is configured.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_CORE_SUBSCRIPTION_GIFT_MULTIPLE_COMMAND_ID`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers = { }`
  - `IgnoreRequirements = false`

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs warning/error messages when configuration is incomplete or the Mix It Up call fails.

### Operator Notes
- Replace the placeholder command ID when ready.
- Add argument/special identifier mapping later when the event contract is finalized.

## Script: `subscription-new.cs`

### Purpose
Base handler for a brand-new subscription event.

### Expected Trigger / Input
- Wire to the Twitch new subscription event.

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Ready to call Mix It Up.
- Logs a warning until a real command ID is configured.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_CORE_SUBSCRIPTION_NEW_COMMAND_ID`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers = { }`
  - `IgnoreRequirements = false`

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs warning/error messages when configuration is incomplete or the Mix It Up call fails.

### Operator Notes
- Replace the placeholder command ID when ready.
- Add argument/special identifier mapping later when the event contract is finalized.

## Script: `subscription-renewed.cs`

### Purpose
Base handler for a renewed subscription event.

### Expected Trigger / Input
- Wire to the Twitch subscription renewal event.

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Ready to call Mix It Up.
- Logs a warning until a real command ID is configured.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_CORE_SUBSCRIPTION_RENEWED_COMMAND_ID`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers = { }`
  - `IgnoreRequirements = false`

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs warning/error messages when configuration is incomplete or the Mix It Up call fails.

### Operator Notes
- Replace the placeholder command ID when ready.
- Add argument/special identifier mapping later when the event contract is finalized.

## Script: `follower-new.cs`

### Purpose
Base handler for a new follower event.

### Expected Trigger / Input
- Wire to the Twitch follow event.

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Ready to call Mix It Up.
- Logs a warning until a real command ID is configured.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_CORE_FOLLOWER_NEW_COMMAND_ID`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers = { }`
  - `IgnoreRequirements = false`

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs warning/error messages when configuration is incomplete or the Mix It Up call fails.

### Operator Notes
- Replace the placeholder command ID when ready.
- Add argument/special identifier mapping later when the event contract is finalized.

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
| `multiMonthDuration` | number | Total months in the multi-month subscription |
| `multiMonthTenure` | number | Months already completed |

> The sub/re-sub/gift-sub scripts are currently stubs — expand `BuildArguments()` and `BuildSpecialIdentifiers()` when event field contracts are finalized.
