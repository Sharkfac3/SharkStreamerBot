---
id: triggers-twitch-raid
type: reference
description: Streamer.bot Twitch Raid trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: seeded
upstream: https://docs.streamer.bot/api/triggers/twitch/raid
---

# Twitch — Raid Triggers

## Cancelled

- Path: Twitch -> Raid -> Cancelled
- Upstream: https://docs.streamer.bot/api/triggers/twitch/raid/cancelled
- Min SB version: v0.0.36
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `raidUser` | string | Display name of the raid target. |
| `raidUserId` | string | User ID of the raid target. |
| `raidUserName` | string | Login name of the raid target. |
| `raidUserProfileImageEscaped` | string | URL-escaped profile image URL of the raid target. |
| `raidUserProfileImageURL` | string | Profile image URL of the raid target. |

### Caveats

- Chat Client trigger.
- Fires when the broadcaster cancels an outgoing raid they initiated.
- `raidUser*` identifies the target of the cancelled raid, not the broadcaster.
- No `viewers` var — raid was not sent.

### Used in repo

_Not yet wired._

---

## Raid

- Path: Twitch -> Raid -> Raid
- Upstream: https://docs.streamer.bot/api/triggers/twitch/raid/raid
- Min SB version: any
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `viewers` | number | Number of viewers in the incoming raid. |

### Caveats

- Chat Client trigger.
- Fires when the broadcaster's channel receives an incoming raid.
- Twitch User shared group identifies the raiding channel.

### Used in repo

_Not yet wired._

---

## Send

- Path: Twitch -> Raid -> Send
- Upstream: https://docs.streamer.bot/api/triggers/twitch/raid/send
- Min SB version: v0.0.36
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `raidUser` | string | Display name of the raid target. |
| `raidUserId` | string | User ID of the raid target. |
| `raidUserName` | string | Login name of the raid target. |
| `raidUserProfileImageEscaped` | string | URL-escaped profile image URL of the raid target. |
| `raidUserProfileImageURL` | string | Profile image URL of the raid target. |
| `viewers` | number | Number of viewers sent in the raid. |

### Caveats

- Chat Client trigger.
- Fires when the broadcaster's raid is dispatched (viewers leave the channel).
- Comes after Start; Start = initiated, Send = dispatched.

### Used in repo

_Not yet wired._

---

## Start

- Path: Twitch -> Raid -> Start
- Upstream: https://docs.streamer.bot/api/triggers/twitch/raid/start
- Min SB version: v0.0.33
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `raidUser` | string | Display name of the raid target. |
| `raidUserId` | string | User ID of the raid target. |
| `raidUserName` | string | Login name of the raid target. |
| `raidUserProfileImageEscaped` | string | URL-escaped profile image URL of the raid target. |
| `raidUserProfileImageURL` | string | Profile image URL of the raid target. |
| `viewers` | number | Number of viewers included in the raid. |

### Caveats

- Chat Client trigger.
- Fires when the broadcaster initiates an outgoing raid (before it is sent).
- `raidUser*` identifies the target channel.

### Used in repo

_Not yet wired._
