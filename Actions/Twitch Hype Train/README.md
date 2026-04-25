# Twitch Hype Train Script Reference

These scripts are base Twitch hype train bridges.
They are intentionally minimal so they can be expanded later.

## Shared Behavior
- All scripts are Streamer.bot C# action scripts.
- All scripts are prepared to call the Mix It Up Run Command API.
- All scripts currently use placeholder command IDs.
- All scripts send populated `SpecialIdentifiers` for Mix It Up command logic.
- No script in this folder interacts with OBS.
- If a command ID is still a placeholder, the script logs a warning and exits safely.

---

## Script: `hype-train-start.cs`

### Purpose
Handler for a hype train start event that forwards event metadata to Mix It Up.

### Expected Trigger / Input
- Wire to the Twitch hype train started event.

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Ready to call Mix It Up.
- Sends hype train start metadata through Mix It Up `SpecialIdentifiers`.
- Logs a warning until a real command ID is configured.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_HYPE_TRAIN_START_COMMAND_ID`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers` populated with the lowercase/no-space keys below
  - `IgnoreRequirements = false`

#### Special Identifiers

| Identifier | Source |
|---|---|
| `hypetrainlevel` | `level` as string; defaults to `0` |
| `hypetrainpercent` | `percent` as string; defaults to `0` |
| `hypetrainpercentdecimal` | `percentDecimal` as string; defaults to empty string |
| `hypetraintype` | `trainType`; defaults to empty string |
| `hypetraingoldenkappa` | `isGoldenKappaTrain` as `true`/`false`; defaults to `false` |
| `hypetraintreasure` | `isTreasureTrain` as `true`/`false`; defaults to `false` |
| `hypetrainshared` | `isSharedTrain` as `true`/`false`; defaults to `false` |
| `hypetrainstartedat` | `startedAt` as string; defaults to empty string |
| `hypetrainid` | `id`; defaults to empty string |
| `hypetraintopbitsuser` | `top.bits.user`; defaults to empty string |
| `hypetraintopbitsuserid` | `top.bits.userId` as string; defaults to empty string |
| `hypetraintopbitstotal` | `top.bits.total` as string; defaults to `0` |
| `hypetraintopsubuser` | `top.subscription.user`; defaults to empty string |
| `hypetraintopsubuserid` | `top.subscription.userId` as string; defaults to empty string |
| `hypetraintopsubtotal` | `top.subscription.total` as string; defaults to `0` |
| `hypetraintopotheruser` | `top.other.user`; defaults to empty string |
| `hypetraintopotheruserid` | `top.other.userId` as string; defaults to empty string |
| `hypetraintopothertotal` | `top.other.total` as string; defaults to `0` |
| `hypetrainevent` | Literal `start` |
| `hypetrainexpiresat` | `expiresAt` as string; defaults to empty string |
| `hypetrainduration` | `duration` as string; defaults to `0` |
| `hypetrainalltimehighlevel` | `allTimeHighLevel` as string; defaults to `0` |
| `hypetrainalltimehightotal` | `allTimeHighTotal` as string; defaults to `0` |

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs warning/error messages when configuration is incomplete or the Mix It Up call fails.

### Operator Notes
- Replace the placeholder command ID when ready.
- `Arguments` intentionally remains empty for compatibility with the current Mix It Up command.
- Missing trigger args resolve to empty strings, `0`, or `false` strings instead of throwing.

---

## Script: `hype-train-level-up.cs`

### Purpose
Handler for a hype train level-up event that forwards event metadata to Mix It Up.

### Expected Trigger / Input
- Wire to the Twitch hype train progress/level-up event.

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Ready to call Mix It Up.
- Sends hype train level-up metadata through Mix It Up `SpecialIdentifiers`.
- Logs a warning until a real command ID is configured.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_HYPE_TRAIN_LEVEL_UP_COMMAND_ID`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers` populated with the lowercase/no-space keys below
  - `IgnoreRequirements = false`

#### Special Identifiers

| Identifier | Source |
|---|---|
| `hypetrainlevel` | `level` as string; defaults to `0` |
| `hypetrainpercent` | `percent` as string; defaults to `0` |
| `hypetrainpercentdecimal` | `percentDecimal` as string; defaults to empty string |
| `hypetraintype` | `trainType`; defaults to empty string |
| `hypetraingoldenkappa` | `isGoldenKappaTrain` as `true`/`false`; defaults to `false` |
| `hypetraintreasure` | `isTreasureTrain` as `true`/`false`; defaults to `false` |
| `hypetrainshared` | `isSharedTrain` as `true`/`false`; defaults to `false` |
| `hypetrainstartedat` | `startedAt` as string; defaults to empty string |
| `hypetrainid` | `id`; defaults to empty string |
| `hypetraintopbitsuser` | `top.bits.user`; defaults to empty string |
| `hypetraintopbitsuserid` | `top.bits.userId` as string; defaults to empty string |
| `hypetraintopbitstotal` | `top.bits.total` as string; defaults to `0` |
| `hypetraintopsubuser` | `top.subscription.user`; defaults to empty string |
| `hypetraintopsubuserid` | `top.subscription.userId` as string; defaults to empty string |
| `hypetraintopsubtotal` | `top.subscription.total` as string; defaults to `0` |
| `hypetraintopotheruser` | `top.other.user`; defaults to empty string |
| `hypetraintopotheruserid` | `top.other.userId` as string; defaults to empty string |
| `hypetraintopothertotal` | `top.other.total` as string; defaults to `0` |
| `hypetrainevent` | Literal `levelup` |
| `hypetrainprevlevel` | `prevLevel` as string; defaults to `0` |
| `hypetrainexpiresat` | `expiresAt` as string; defaults to empty string |
| `hypetrainduration` | `duration` as string; defaults to `0` |
| `hypetrainalltimehighlevel` | `allTimeHighLevel` as string; defaults to `0` |
| `hypetrainalltimehightotal` | `allTimeHighTotal` as string; defaults to `0` |

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs warning/error messages when configuration is incomplete or the Mix It Up call fails.

### Operator Notes
- Replace the placeholder command ID when ready.
- `Arguments` intentionally remains empty for compatibility with the current Mix It Up command.
- Missing trigger args resolve to empty strings, `0`, or `false` strings instead of throwing.

---

## Script: `hype-train-end.cs`

### Purpose
Handler for a hype train end event that forwards event metadata to Mix It Up.

### Expected Trigger / Input
- Wire to the Twitch hype train ended event.

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Ready to call Mix It Up.
- Sends hype train end metadata through Mix It Up `SpecialIdentifiers`.
- Logs a warning until a real command ID is configured.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_HYPE_TRAIN_END_COMMAND_ID`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers` populated with the lowercase/no-space keys below
  - `IgnoreRequirements = false`

#### Special Identifiers

| Identifier | Source |
|---|---|
| `hypetrainlevel` | `level` as string; defaults to `0` |
| `hypetrainpercent` | `percent` as string; defaults to `0` |
| `hypetrainpercentdecimal` | `percentDecimal` as string; defaults to empty string |
| `hypetraintype` | `trainType`; defaults to empty string |
| `hypetraingoldenkappa` | `isGoldenKappaTrain` as `true`/`false`; defaults to `false` |
| `hypetraintreasure` | `isTreasureTrain` as `true`/`false`; defaults to `false` |
| `hypetrainshared` | `isSharedTrain` as `true`/`false`; defaults to `false` |
| `hypetrainstartedat` | `startedAt` as string; defaults to empty string |
| `hypetrainid` | `id`; defaults to empty string |
| `hypetraintopbitsuser` | `top.bits.user`; defaults to empty string |
| `hypetraintopbitsuserid` | `top.bits.userId` as string; defaults to empty string |
| `hypetraintopbitstotal` | `top.bits.total` as string; defaults to `0` |
| `hypetraintopsubuser` | `top.subscription.user`; defaults to empty string |
| `hypetraintopsubuserid` | `top.subscription.userId` as string; defaults to empty string |
| `hypetraintopsubtotal` | `top.subscription.total` as string; defaults to `0` |
| `hypetraintopotheruser` | `top.other.user`; defaults to empty string |
| `hypetraintopotheruserid` | `top.other.userId` as string; defaults to empty string |
| `hypetraintopothertotal` | `top.other.total` as string; defaults to `0` |
| `hypetrainevent` | Literal `end` |

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs warning/error messages when configuration is incomplete or the Mix It Up call fails.

### Operator Notes
- Replace the placeholder command ID when ready.
- `Arguments` intentionally remains empty for compatibility with the current Mix It Up command.
- Missing trigger args resolve to empty strings, `0`, or `false` strings instead of throwing.

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
