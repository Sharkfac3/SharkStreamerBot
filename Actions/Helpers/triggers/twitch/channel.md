---
id: triggers-twitch-channel
type: reference
description: Streamer.bot Twitch Channel trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: seeded
upstream: https://docs.streamer.bot/api/triggers/twitch/channel
---

# Twitch — Channel Triggers

## Follow

- Path: Twitch -> Channel -> Follow
- Upstream: https://docs.streamer.bot/api/triggers/twitch/channel/follow
- Min SB version: any
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

_No trigger-specific args. Shared variable groups apply (see upstream)._

### Caveats

- User identity available via Twitch User shared group.

### Used in repo

- [Actions/Twitch Core Integrations/follower-new.cs](../../Twitch%20Core%20Integrations/follower-new.cs)

---

## Stream Offline

- Path: Twitch -> Channel -> Stream Offline
- Upstream: https://docs.streamer.bot/api/triggers/twitch/channel/stream-offline
- Min SB version: v0.1.17
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `endedAt` | DateTime | Datetime the stream ended. |

### Caveats

- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## Stream Online

- Path: Twitch -> Channel -> Stream Online
- Upstream: https://docs.streamer.bot/api/triggers/twitch/channel/stream-online
- Min SB version: v0.1.17
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `game` | string | Category name (e.g. "Just Chatting"). |
| `gameId` | number | Category ID. |
| `startedAt` | DateTime | Datetime the stream started. |
| `tag#` | string | Each individual tag (indexed from 0). |
| `tagCount` | number | Number of tags. |
| `tags` | List\<string\> | C#-accessible list of tags. |
| `tagsDelimited` | string | Comma-delimited string of tags. |

### Caveats

- EventSub-based trigger.

### Used in repo

- [Actions/Twitch Core Integrations/stream-start.cs](../../Twitch%20Core%20Integrations/stream-start.cs)

---

## Viewer Count Update

- Path: Twitch -> Channel -> Viewer Count Update
- Upstream: https://docs.streamer.bot/api/triggers/twitch/channel/viewer-count-update
- Min SB version: v0.2.0
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `viewerCount` | number | New Twitch viewer count. |

### Caveats

- EventSub-based trigger.

### Used in repo

_Not yet wired._
