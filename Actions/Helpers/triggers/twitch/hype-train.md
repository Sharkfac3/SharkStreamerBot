---
id: triggers-twitch-hype-train
type: reference
description: Streamer.bot Twitch Hype Train trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: seeded
upstream: https://docs.streamer.bot/api/triggers/twitch/hype-train
---

# Twitch — Hype Train Triggers

## End

- Path: Twitch -> Hype Train -> End
- Upstream: https://docs.streamer.bot/api/triggers/twitch/hype-train/end
- Min SB version: any
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `id` | string | Unique identifier of the hype train. |
| `isGoldenKappaTrain` | boolean | Whether this is a Golden Kappa Train. |
| `isInSharedChat` | boolean | Whether the broadcast is currently in a shared chat. |
| `isSharedChatHost` | boolean | Whether the broadcaster is the host of the shared chat. |
| `isSharedTrain` | boolean | Whether this is a Shared Chat Hype Train. |
| `isTreasureTrain` | boolean | Whether this is a Treasure Train. |
| `level` | number | Final Hype Train level reached. |
| `percent` | number | Final percentage of the current level. |
| `percentDecimal` | number | Final percentage as a decimal. |
| `startedAt` | DateTime | Time the hype train started. |
| `trainType` | string | Train classification: `regular`, `treasure`, `golden_kappa`. |
| `top.bits.total` | number | Bits from the top cheerer. |
| `top.bits.user` | string | Display name of the top cheerer. |
| `top.bits.userId` | number | User ID of the top cheerer. |
| `top.bits.userName` | string | Login name of the top cheerer. |
| `top.subscription.total` | number | Gift sub points from the top gifter (T1=500, T2=1000, T3=2500). |
| `top.subscription.user` | string | Display name of the top gifter. |
| `top.subscription.userId` | number | User ID of the top gifter. |
| `top.subscription.userName` | string | Login name of the top gifter. |
| `top.other.total` | number | Non-bits/sub amount from the top other contributor. |
| `top.other.user` | string | Display name of the top other contributor. |
| `top.other.userId` | number | User ID of the top other contributor. |
| `top.other.userName` | string | Login name of the top other contributor. |

### Caveats

- `top.*` variables may be empty if no contributions of that type occurred.

### Used in repo

- [Actions/Twitch Hype Train/hype-train-end.cs](../../Twitch%20Hype%20Train/hype-train-end.cs)

---

## Level Up

- Path: Twitch -> Hype Train -> Level Up
- Upstream: https://docs.streamer.bot/api/triggers/twitch/hype-train/level-up
- Min SB version: any
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `duration` | DateTime | Duration of the hype train. |
| `expiresAt` | DateTime | Time the hype train expires. |
| `id` | string | Unique identifier of the hype train. |
| `isGoldenKappaTrain` | boolean | Whether this is a Golden Kappa Train. |
| `isInSharedChat` | boolean | Whether the broadcast is in a shared chat. |
| `isSharedChatHost` | boolean | Whether the broadcaster hosts the shared chat. |
| `isSharedTrain` | boolean | Whether this is a Shared Chat Hype Train. |
| `isTreasureTrain` | boolean | Whether this is a Treasure Train. |
| `level` | number | Current Hype Train level. |
| `percent` | number | Percentage of the current level completed. |
| `percentDecimal` | number | Current level percentage as a decimal. |
| `prevLevel` | number | Previous Hype Train level (before this level-up). |
| `startedAt` | DateTime | Time the hype train started. |
| `trainType` | string | Train classification: `regular`, `treasure`, `golden_kappa`. |
| `top.bits.total` | number | Bits from the top cheerer. |
| `top.bits.user` | string | Display name of the top cheerer. |
| `top.bits.userId` | number | User ID of the top cheerer. |
| `top.bits.userName` | string | Login name of the top cheerer. |
| `top.subscription.total` | number | Gift sub points from the top gifter. |
| `top.subscription.user` | string | Display name of the top gifter. |
| `top.subscription.userId` | number | User ID of the top gifter. |
| `top.subscription.userName` | string | Login name of the top gifter. |
| `top.other.total` | number | Amount from the top other contributor. |
| `top.other.user` | string | Display name of the top other contributor. |
| `top.other.userId` | number | User ID of the top other contributor. |
| `top.other.userName` | string | Login name of the top other contributor. |

### Caveats

- `prevLevel` distinguishes this from Start (which has no prevLevel).

### Used in repo

- [Actions/Twitch Hype Train/hype-train-level-up.cs](../../Twitch%20Hype%20Train/hype-train-level-up.cs)

---

## Start

- Path: Twitch -> Hype Train -> Start
- Upstream: https://docs.streamer.bot/api/triggers/twitch/hype-train/start
- Min SB version: any
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `allTimeHighLevel` | number | All-time high level this train type has reached. |
| `allTimeHighTotal` | number | All-time high total points this train type has reached. |
| `duration` | DateTime | Duration of the hype train. |
| `expiresAt` | DateTime | Time the hype train expires. |
| `id` | string | Unique identifier of the hype train. |
| `isGoldenKappaTrain` | boolean | Whether this is a Golden Kappa Train. |
| `isInSharedChat` | boolean | Whether the broadcast is in a shared chat. |
| `isSharedChatHost` | boolean | Whether the broadcaster hosts the shared chat. |
| `isSharedTrain` | boolean | Whether this is a Shared Chat Hype Train. |
| `isTreasureTrain` | boolean | Whether this is a Treasure Train. |
| `level` | number | Starting Hype Train level. |
| `percent` | number | Percentage of the current level completed. |
| `percentDecimal` | number | Current level percentage as a decimal. |
| `startedAt` | DateTime | Time the hype train started. |
| `trainType` | string | Train classification: `regular`, `treasure`, `golden_kappa`. |
| `top.bits.total` | number | Bits from the top cheerer. |
| `top.bits.user` | string | Display name of the top cheerer. |
| `top.bits.userId` | number | User ID of the top cheerer. |
| `top.bits.userName` | string | Login name of the top cheerer. |
| `top.subscription.total` | number | Gift sub points from the top gifter. |
| `top.subscription.user` | string | Display name of the top gifter. |
| `top.subscription.userId` | number | User ID of the top gifter. |
| `top.subscription.userName` | string | Login name of the top gifter. |
| `top.other.total` | number | Amount from the top other contributor. |
| `top.other.user` | string | Display name of the top other contributor. |
| `top.other.userId` | number | User ID of the top other contributor. |
| `top.other.userName` | string | Login name of the top other contributor. |

### Caveats

- `allTimeHighLevel` and `allTimeHighTotal` are unique to Start (not present in Level Up/Update).

### Used in repo

- [Actions/Twitch Hype Train/hype-train-start.cs](../../Twitch%20Hype%20Train/hype-train-start.cs)

---

## Update

- Path: Twitch -> Hype Train -> Update
- Upstream: https://docs.streamer.bot/api/triggers/twitch/hype-train/update
- Min SB version: any
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `duration` | DateTime | Duration of the hype train. |
| `expiresAt` | DateTime | Time the hype train expires. |
| `id` | string | Unique identifier of the hype train. |
| `isGoldenKappaTrain` | boolean | Whether this is a Golden Kappa Train. |
| `isInSharedChat` | boolean | Whether the broadcast is in a shared chat. |
| `isSharedChatHost` | boolean | Whether the broadcaster hosts the shared chat. |
| `isSharedTrain` | boolean | Whether this is a Shared Chat Hype Train. |
| `isTreasureTrain` | boolean | Whether this is a Treasure Train. |
| `level` | number | Current Hype Train level. |
| `percent` | number | Percentage of the current level completed. |
| `percentDecimal` | number | Current level percentage as a decimal. |
| `startedAt` | DateTime | Time the hype train started. |
| `trainType` | string | Train classification: `regular`, `treasure`, `golden_kappa`. |
| `top.bits.total` | number | Bits from the top cheerer. |
| `top.bits.user` | string | Display name of the top cheerer. |
| `top.bits.userId` | number | User ID of the top cheerer. |
| `top.bits.userName` | string | Login name of the top cheerer. |
| `top.subscription.total` | number | Gift sub points from the top gifter. |
| `top.subscription.user` | string | Display name of the top gifter. |
| `top.subscription.userId` | number | User ID of the top gifter. |
| `top.subscription.userName` | string | Login name of the top gifter. |
| `top.other.total` | number | Amount from the top other contributor. |
| `top.other.user` | string | Display name of the top other contributor. |
| `top.other.userId` | number | User ID of the top other contributor. |
| `top.other.userName` | string | Login name of the top other contributor. |

### Caveats

- Fires frequently during an active hype train — rate-control or de-dupe if needed.
- Same variable set as Start minus `allTimeHighLevel`/`allTimeHighTotal` and `prevLevel`.

### Used in repo

_Not yet wired._
