---
id: triggers-twitch-emotes
type: reference
description: Streamer.bot Twitch Emotes trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: seeded
upstream: https://docs.streamer.bot/api/triggers/twitch/emotes
---

# Twitch — Emotes Triggers

## Betterttv Emote Added

- Path: Twitch -> Emotes -> Betterttv Emote Added
- Upstream: https://docs.streamer.bot/api/triggers/twitch/emotes/betterttv-emote-added
- Min SB version: v0.2.3
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `emote` | string | Name of the emote (e.g. `(ditto)`). |
| `imageUrl` | string | URL of the emote image. |

### Caveats

- Fires when a BetterTTV emote has been added to the channel.

### Used in repo

_Not yet wired._

---

## Betterttv Emote Removed

- Path: Twitch -> Emotes -> Betterttv Emote Removed
- Upstream: https://docs.streamer.bot/api/triggers/twitch/emotes/betterttv-emote-removed
- Min SB version: v0.2.3
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `emote` | string | Name of the emote (e.g. `(ditto)`). |
| `imageUrl` | string | URL of the emote image. |

### Caveats

- Fires when a BetterTTV emote has been removed from the channel.

### Used in repo

_Not yet wired._

---

## Seventv Emote Added

- Path: Twitch -> Emotes -> Seventv Emote Added
- Upstream: https://docs.streamer.bot/api/triggers/twitch/emotes/seventv-emote-added
- Min SB version: v0.2.3
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `emote` | string | Name of the emote. |
| `imageUrl` | string | URL of the emote image. |

### Caveats

- Fires when a SevenTV emote has been added to the channel.

### Used in repo

_Not yet wired._

---

## Seventv Emote Removed

- Path: Twitch -> Emotes -> Seventv Emote Removed
- Upstream: https://docs.streamer.bot/api/triggers/twitch/emotes/seventv-emote-removed
- Min SB version: v0.2.3
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `emote` | string | Name of the emote. |
| `imageUrl` | string | URL of the emote image. |

### Caveats

- Fires when a SevenTV emote has been removed from the channel.

### Used in repo

_Not yet wired._
