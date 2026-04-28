---
id: triggers-twitch-ads
type: reference
description: Streamer.bot Twitch Ads trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: seeded
upstream: https://docs.streamer.bot/api/triggers/twitch/ads
---

# Twitch — Ads Triggers

## Ad Run

- Path: Twitch -> Ads -> Ad Run
- Upstream: https://docs.streamer.bot/api/triggers/twitch/ads/ad-run
- Min SB version: v0.1.10
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `adLength` | number | Duration of the ad in seconds. |
| `adLengthMs` | number | Duration of the ad in milliseconds. |
| `adScheduled` | boolean | Whether the ad was scheduled. |

### Caveats

- Fires at the start of an ad break.
- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## Upcoming Ad

- Path: Twitch -> Ads -> Upcoming Ad
- Upstream: https://docs.streamer.bot/api/triggers/twitch/ads/upcoming-ad
- Min SB version: v0.2.3
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `adLength` | number | Length of the upcoming ad run in seconds. |
| `minutes` | number | Minutes until the ad run will start. |
| `nextAdAt` | DateTime | Exact UTC datetime of the next ad run. |
| `snoozesLeft` | number | Maximum number of snoozes remaining. |

### Caveats

- Fires at 1-minute intervals starting 5 minutes before the upcoming ad.

### Used in repo

_Not yet wired._
