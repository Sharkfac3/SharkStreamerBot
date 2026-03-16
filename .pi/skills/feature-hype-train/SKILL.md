---
name: feature-hype-train
description: Twitch Hype Train event scripts (start, level-up, end). Load when working on any script under Actions/Twitch Hype Train/.
---

# Feature: Hype Train

## When to Load

Load this skill for any work under `Actions/Twitch Hype Train/`.
Always pair it with `streamerbot-scripting` for `.cs` edits.
Load `change-summary` after making changes.

## Scope

| Script | Path | Trigger |
|---|---|---|
| Hype Train Start | `Actions/Twitch Hype Train/hype-train-start.cs` | Hype Train → Start |
| Hype Train Level Up | `Actions/Twitch Hype Train/hype-train-level-up.cs` | Hype Train → Level Up |
| Hype Train End | `Actions/Twitch Hype Train/hype-train-end.cs` | Hype Train → End |

## Detailed Docs

- `Actions/Twitch Hype Train/README.md`

**Read the README before making changes.**

## Current State

All three scripts are **stubs** — they call a Mix It Up command but pass empty `Arguments` and `SpecialIdentifiers`. The `BuildArguments()` and `BuildSpecialIdentifiers()` methods are placeholders with a comment to expand once the final event field contract is decided.

When expanding a script to use trigger variables, add them to `BuildArguments()` or `BuildSpecialIdentifiers()` as described in the operator notes inside each script.

## Behavioral Expectations

- Keep hype train scripts lightweight — they are notification bridges, not stateful mini-games.
- All three scripts must skip the Mix It Up call gracefully when the command ID is still a placeholder.
- Log warnings instead of throwing exceptions so the action queue stays stable.
- Do not introduce OBS interactions unless explicitly requested.

## Trigger Variables

Variable names confirmed from official Streamer.bot docs.
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
