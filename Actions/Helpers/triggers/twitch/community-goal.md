---
id: triggers-twitch-community-goal
type: reference
description: Streamer.bot Twitch Community Goal trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: seeded
upstream: https://docs.streamer.bot/api/triggers/twitch/community-goal
---

# Twitch — Community Goal Triggers

> **Deprecated:** Both triggers removed in Streamer.bot v1.0.0. Twitch deprecated the PubSub API these relied on. Triggers will not function until Twitch re-implements them via EventSub.

## Contribution

- Path: Twitch -> Community Goal -> Contribution
- Upstream: https://docs.streamer.bot/api/triggers/twitch/community-goal/contribution
- Min SB version: any (removed in v1.0.0)
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `contributed` | number | Total amount contributed to the goal so far. |
| `contributedFormatted` | number | Total contribution as a formatted number. |
| `daysLeft` | number | Days remaining in the goal. |
| `durationDays` | number | Total goal duration in days. |
| `goalAmount` | number | Total amount required to complete the goal. |
| `goalAmountFormatted` | number | Goal amount as a formatted number. |
| `percentComplete` | string | Progress percentage (e.g. `73%`). |
| `percentDecimal` | number | Progress as a decimal (e.g. `0.73`). |
| `title` | string | Name of the community goal (e.g. `My community goal`). |
| `userContributed` | number | This user's contribution amount. |
| `userContribFormatted` | number | This user's contribution as a formatted number. |
| `userTotalContributed` | number | This user's lifetime total contribution. |
| `userTotalContribFormatted` | number | Lifetime total as a formatted number. |

### Caveats

- **Removed in Streamer.bot v1.0.0.** Twitch deprecated PubSub. Non-functional until Twitch re-implements via EventSub.

### Used in repo

_Not yet wired._

---

## Ended

- Path: Twitch -> Community Goal -> Ended
- Upstream: https://docs.streamer.bot/api/triggers/twitch/community-goal/ended
- Min SB version: any (removed in v1.0.0)
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `contributed` | number | Total amount contributed to the goal. |
| `contributedFormatted` | number | Total contribution as a formatted number. |
| `goalAmount` | number | Total amount required to complete the goal. |
| `goalAmountFormatted` | number | Goal amount as a formatted number. |
| `percentComplete` | string | Final progress percentage (e.g. `73%`). |
| `percentDecimal` | number | Final progress as a decimal (e.g. `0.73`). |
| `title` | string | Name of the community goal. |

### Caveats

- **Removed in Streamer.bot v1.0.0.** Twitch deprecated PubSub. Non-functional until Twitch re-implements via EventSub.

### Used in repo

_Not yet wired._
