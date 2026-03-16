# Twitch Hype Train Script Reference

These scripts are base Twitch hype train bridges.
They are intentionally minimal so they can be expanded later.

## Shared Behavior
- All scripts are Streamer.bot C# action scripts.
- All scripts are prepared to call the Mix It Up Run Command API.
- All scripts currently use placeholder command IDs.
- All scripts currently send an empty `SpecialIdentifiers` object.
- No script in this folder interacts with OBS.
- If a command ID is still a placeholder, the script logs a warning and exits safely.

---

## Script: `hype-train-start.cs`

### Purpose
Base handler for a hype train start event.

### Expected Trigger / Input
- Wire to the Twitch hype train started event.

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Ready to call Mix It Up.
- Logs a warning until a real command ID is configured.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_HYPE_TRAIN_START_COMMAND_ID`
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

---

## Script: `hype-train-level-up.cs`

### Purpose
Base handler for a hype train level-up event.

### Expected Trigger / Input
- Wire to the Twitch hype train progress/level-up event.

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Ready to call Mix It Up.
- Logs a warning until a real command ID is configured.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_HYPE_TRAIN_LEVEL_UP_COMMAND_ID`
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

---

## Script: `hype-train-end.cs`

### Purpose
Base handler for a hype train end event.

### Expected Trigger / Input
- Wire to the Twitch hype train ended event.

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Ready to call Mix It Up.
- Logs a warning until a real command ID is configured.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_HYPE_TRAIN_END_COMMAND_ID`
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

---

## Trigger Variables

Access in C# via `CPH.TryGetArg("variableName", out T value)`.

### Hype Train Start

| Variable | Type | Description |
|---|---|---|
| `level` | number | Current hype train level |
| `percent` | number | Completion % of current level (e.g. 80) |
| `percentDecimal` | number | Completion as decimal (e.g. 0.8) |
| `trainType` | string | `regular`, `treasure`, or `golden_kappa` |
| `isGoldenKappaTrain` | bool | Whether this is a Golden Kappa Train |
| `isTreasureTrain` | bool | Whether this is a Treasure Train |
| `isSharedTrain` | bool | Whether this is a Shared Chat Hype Train |
| `startedAt` | DateTime | When the train started |
| `expiresAt` | DateTime | When the train expires |
| `duration` | number | Train duration in seconds |
| `allTimeHighLevel` | number | All-time peak level for this train type |
| `allTimeHighTotal` | number | All-time peak points for this train type |
| `id` | string | Unique hype train ID |
| `top.bits.user` | string | Top cheerer display name |
| `top.bits.userId` | number | Top cheerer user ID |
| `top.bits.userName` | string | Top cheerer login name |
| `top.bits.total` | number | Bits from top cheerer |
| `top.subscription.user` | string | Top gift sub giver display name |
| `top.subscription.userId` | number | Top gift sub giver user ID |
| `top.subscription.total` | number | Gift sub points from top giver |
| `top.other.user` | string | Top other contributor display name |
| `top.other.userId` | number | Top other contributor user ID |
| `top.other.total` | number | Points from top other contributor |

### Hype Train Level Up

Same variables as Start, plus:

| Variable | Type | Description |
|---|---|---|
| `prevLevel` | number | The level before this level-up |

### Hype Train End

| Variable | Type | Description |
|---|---|---|
| `level` | number | Final level reached |
| `percent` | number | Final completion % |
| `percentDecimal` | number | Final completion as decimal |
| `trainType` | string | `regular`, `treasure`, or `golden_kappa` |
| `isGoldenKappaTrain` | bool | Whether this was a Golden Kappa Train |
| `isTreasureTrain` | bool | Whether this was a Treasure Train |
| `isSharedTrain` | bool | Whether this was a Shared Chat Hype Train |
| `startedAt` | DateTime | When the train started |
| `id` | string | Unique hype train ID |
| `top.bits.user` | string | Top cheerer display name |
| `top.bits.userId` | number | Top cheerer user ID |
| `top.bits.total` | number | Bits from top cheerer |
| `top.subscription.user` | string | Top gift sub giver display name |
| `top.subscription.userId` | number | Top gift sub giver user ID |
| `top.subscription.total` | number | Gift sub points from top giver |
| `top.other.user` | string | Top other contributor display name |
| `top.other.userId` | number | Top other contributor user ID |
| `top.other.total` | number | Points from top other contributor |
