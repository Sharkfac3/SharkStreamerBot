---
id: actions-twitch-hype-train
type: domain-route
description: Twitch hype train Streamer.bot event bridges, Mix It Up metadata payloads, paste targets, and brand handoffs.
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Twitch Hype Train — Agent Guide

## Purpose

This folder owns Streamer.bot C# scripts for Twitch hype train events. The scripts are lightweight event bridges that forward hype train metadata to Mix It Up through populated `SpecialIdentifiers` while keeping `Arguments` empty for compatibility with the current command pattern.

This is part of the ratified Twitch target shape: one `streamerbot-dev` owner family with folder-local Twitch guides. Do not recreate flat Twitch wrapper skills; keep hype-train event knowledge here.

## When to Activate

Use this guide when editing or reviewing files under [Actions/Twitch Hype Train/](./), including:

- [Actions/Twitch Hype Train/hype-train-start.cs](hype-train-start.cs)
- [Actions/Twitch Hype Train/hype-train-level-up.cs](hype-train-level-up.cs)
- [Actions/Twitch Hype Train/hype-train-end.cs](hype-train-end.cs)
- Script Reference or operator documentation in this folder

Activate `brand-steward` before changing public hype-train alert copy, overlay text, spoken/TTS text, reward/celebration wording, or Mix It Up message branches.

## Primary Owner

`streamerbot-dev` owns the C# runtime behavior, Streamer.bot hype-train trigger compatibility, Mix It Up API payload shape, safe placeholder-command handling, and manual paste readiness for this folder.

## Secondary Owners / Chain To

- `brand-steward` — chain for public hype-train copy, celebration wording, overlay copy, TTS/spoken text, or Mix It Up response wording.
- `ops` — chain only for validation, paste/sync workflow, or agent-tree maintenance beyond this local guide.

## Required Reading

Read the following before editing scripts:

- [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md)
- [Actions/Helpers/AGENTS.md](../Helpers/AGENTS.md)
- [Actions/Twitch Core Integrations/AGENTS.md](../Twitch%20Core%20Integrations/AGENTS.md) when changes affect shared Twitch event conventions
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) when public copy changes

## Local Workflow

1. Identify the hype train event: start, level up/progress, or end.
2. Preserve the bridge behavior: scripts should stay lightweight, avoid stateful mini-game behavior, and forward event metadata to Mix It Up.
3. Keep placeholder command ID handling safe. If a command ID is still a placeholder, log a warning and exit gracefully.
4. Preserve Mix It Up payload compatibility:
   - `Platform = "Twitch"`
   - `Arguments = ""`
   - Hype train metadata belongs in populated `SpecialIdentifiers`.
   - Identifier keys stay lowercase and no-space, using the documented `hypetrain*` naming pattern.
   - Missing trigger args resolve to empty strings, `0`, or `false` strings instead of throwing.
5. Do not add OBS interactions unless the operator explicitly requests them.
6. Keep scripts self-contained and paste-ready. Do not assume shared repo helper files can be imported by Streamer.bot.
7. Update the Script Reference section in this file if trigger variables, payload identifiers, command IDs, operator wiring, or event routing changes.
8. If a new global variable, OBS source, timer, or shared command contract is introduced, update [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) when in scope or flag it in the handoff when out of scope.

Current script map:

| Script | Runtime purpose | Event identifier |
|---|---|---|
| [hype-train-start.cs](hype-train-start.cs) | Hype train started event bridge to Mix It Up | `hypetrainevent = start` |
| [hype-train-level-up.cs](hype-train-level-up.cs) | Hype train progress/level-up event bridge to Mix It Up | `hypetrainevent = levelup` |
| [hype-train-end.cs](hype-train-end.cs) | Hype train ended event bridge to Mix It Up | `hypetrainevent = end` |

Key metadata pattern:

| Identifier group | Notes |
|---|---|
| `hypetrainlevel`, `hypetrainpercent`, `hypetrainpercentdecimal` | Current level/progress values |
| `hypetraintype`, `hypetraingoldenkappa`, `hypetraintreasure`, `hypetrainshared` | Train type and special train flags |
| `hypetrainstartedat`, `hypetrainexpiresat`, `hypetrainduration`, `hypetrainid` | Timing and identity fields, where available |
| `hypetraintopbits*`, `hypetraintopsub*`, `hypetraintopother*` | Top contributor metadata |
| `hypetrainprevlevel` | Level-up only |
| `hypetrainalltimehighlevel`, `hypetrainalltimehightotal` | Start/level-up all-time high metadata |

## Validation

For script changes, perform the narrowest safe validation available:

- Review edited C# for Streamer.bot paste readiness: one `Execute()` entry point, no unsupported imports, no repo-only runtime dependencies, and defensive arg handling.
- Verify any global names, OBS names, timers, and Mix It Up command contracts against [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md).
- For Mix It Up payload changes, confirm `Arguments` remains compatible and `SpecialIdentifiers` are documented in the Script Reference section in this file.
- Run shared-constants validation when constants or documented references change:

```bash
python3 Tools/StreamerBot/validate-shared-constants.py
```

For agent-doc changes, follow [validation](../../.agents/workflows/validation.md) and run the agent-tree validator with the task-requested report path. Record command output in the handoff or final change summary.

## Boundaries / Out of Scope

- Do not introduce OBS scene changes, global state machines, or mini-game behavior unless explicitly requested.
- Do not rename the `hypetrain*` special identifier keys without updating Mix It Up commands and the Script Reference payload contract.
- Do not change public hype-train copy without `brand-steward` review.
- Do not migrate follow/sub/bits/channel-point behavior into this guide.

## Handoff Notes

After changes, follow these workflows:

- [change-summary](../../.agents/workflows/change-summary.md) — terminal summary with changed files, paste targets, setup steps, and validation output.
- [sync](../../.agents/workflows/sync.md) — repo-to-Streamer.bot manual paste expectations.
- [validation](../../.agents/workflows/validation.md) — validation command selection and failure reporting.

Paste targets are the edited `.cs` files under [Actions/Twitch Hype Train/](./). Operator must manually paste changed script contents into the matching Streamer.bot hype train actions and verify event trigger wiring.

Public-copy handoff triggers: hype-train alert wording, celebration copy, overlay messages, TTS/spoken responses, and Mix It Up text branches. Include exactly which strings changed and whether `brand-steward` reviewed them.

---

## Script Reference

These scripts are base Twitch hype train bridges.
They are intentionally minimal so they can be expanded later.

### Shared Behavior
- All scripts are Streamer.bot C# action scripts.
- All scripts are prepared to call the Mix It Up Run Command API.
- All scripts currently use placeholder command IDs.
- All scripts send populated `SpecialIdentifiers` for Mix It Up command logic.
- No script in this folder interacts with OBS.
- If a command ID is still a placeholder, the script logs a warning and exits safely.

---

### Script: `hype-train-start.cs`

#### Purpose
Handler for a hype train start event that forwards event metadata to Mix It Up.

#### Expected Trigger / Input
- Wire to the Twitch hype train started event.

#### Required Runtime Variables
- None.

#### Key Outputs / Side Effects
- Ready to call Mix It Up.
- Sends hype train start metadata through Mix It Up `SpecialIdentifiers`.
- Logs a warning until a real command ID is configured.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_HYPE_TRAIN_START_COMMAND_ID`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers` populated with the lowercase/no-space keys below
  - `IgnoreRequirements = false`

##### Special Identifiers

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

#### OBS Interactions
- None.

#### Wait Behavior
- None.

#### Chat / Log Output
- No chat output.
- Logs warning/error messages when configuration is incomplete or the Mix It Up call fails.

#### Operator Notes
- Replace the placeholder command ID when ready.
- `Arguments` intentionally remains empty for compatibility with the current Mix It Up command.
- Missing trigger args resolve to empty strings, `0`, or `false` strings instead of throwing.

---

### Script: `hype-train-level-up.cs`

#### Purpose
Handler for a hype train level-up event that forwards event metadata to Mix It Up.

#### Expected Trigger / Input
- Wire to the Twitch hype train progress/level-up event.

#### Required Runtime Variables
- None.

#### Key Outputs / Side Effects
- Ready to call Mix It Up.
- Sends hype train level-up metadata through Mix It Up `SpecialIdentifiers`.
- Logs a warning until a real command ID is configured.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_HYPE_TRAIN_LEVEL_UP_COMMAND_ID`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers` populated with the lowercase/no-space keys below
  - `IgnoreRequirements = false`

##### Special Identifiers

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

#### OBS Interactions
- None.

#### Wait Behavior
- None.

#### Chat / Log Output
- No chat output.
- Logs warning/error messages when configuration is incomplete or the Mix It Up call fails.

#### Operator Notes
- Replace the placeholder command ID when ready.
- `Arguments` intentionally remains empty for compatibility with the current Mix It Up command.
- Missing trigger args resolve to empty strings, `0`, or `false` strings instead of throwing.

---

### Script: `hype-train-end.cs`

#### Purpose
Handler for a hype train end event that forwards event metadata to Mix It Up.

#### Expected Trigger / Input
- Wire to the Twitch hype train ended event.

#### Required Runtime Variables
- None.

#### Key Outputs / Side Effects
- Ready to call Mix It Up.
- Sends hype train end metadata through Mix It Up `SpecialIdentifiers`.
- Logs a warning until a real command ID is configured.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_HYPE_TRAIN_END_COMMAND_ID`
- Payload shape:
  - `Platform = "Twitch"`
  - `Arguments = ""`
  - `SpecialIdentifiers` populated with the lowercase/no-space keys below
  - `IgnoreRequirements = false`

##### Special Identifiers

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

#### OBS Interactions
- None.

#### Wait Behavior
- None.

#### Chat / Log Output
- No chat output.
- Logs warning/error messages when configuration is incomplete or the Mix It Up call fails.

#### Operator Notes
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
