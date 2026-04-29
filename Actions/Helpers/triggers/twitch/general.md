---
id: triggers-twitch-general
type: reference
description: Streamer.bot Twitch General trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: seeded
upstream: https://docs.streamer.bot/api/triggers/twitch/general
---

# Twitch — General Triggers

## Announcement

- Path: Twitch -> General -> Announcement
- Upstream: https://docs.streamer.bot/api/triggers/twitch/general/announcement
- Min SB version: v0.1.9
- Shared groups: General, Twitch Broadcaster, Twitch Chat, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `announceColor` | string | Color of the announcement: `DEFAULT`, `BLUE`, `RED`, `ORANGE`, `PURPLE`. |

### Caveats

- Chat Client trigger.

### Used in repo

_Not yet wired._

---

## Present Viewers

- Path: Twitch -> General -> Present Viewers
- Upstream: https://docs.streamer.bot/api/triggers/twitch/general/present-viewers
- Min SB version: v0.0.50
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `isLive` | boolean | Whether the stream is live. |
| `isTest` | boolean | Whether this is a test event. |
| `users` | List\<Dictionary\<string, object\>\> | C#-accessible list of users present in chat. |

### Caveats

- Fires every 1–10 minutes (default 5 minutes); configurable at Platforms > Twitch > Settings.
- Live viewer data or artificial presence marking based on activity thresholds, depending on settings.

### Used in repo

_Not yet wired._

---

## Shoutout Received

- Path: Twitch -> General -> Shoutout Received
- Upstream: https://docs.streamer.bot/api/triggers/twitch/general/shoutout-received
- Min SB version: v0.1.17
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `viewerCount` | number | Viewer count of the user who created the shoutout. |

### Caveats

- Chat Client trigger.
- Twitch User shared group provides the identity of the channel that sent the shoutout.

### Used in repo

_Not yet wired._

---

## Stream Update

- Path: Twitch -> General -> Stream Update
- Upstream: https://docs.streamer.bot/api/triggers/twitch/general/stream-update
- Min SB version: v0.0.30
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `gameBoxArt` | string | Box art URL for the current game. |
| `gameId` | number | Current game/category ID. |
| `gameName` | string | Current game/category name. |
| `gameUpdate` | boolean | Whether the game was updated. |
| `oldGameBoxArt` | string | Box art URL of the previous game. |
| `oldGameId` | number | Previous game/category ID. |
| `oldGameName` | string | Previous game/category name. |
| `oldStatus` | string | Previous stream title. |
| `status` | string | Current stream title. |
| `statusUpdate` | boolean | Whether the stream title was updated. |

### Caveats

- EventSub-based trigger.

### Used in repo

_Not yet wired._
