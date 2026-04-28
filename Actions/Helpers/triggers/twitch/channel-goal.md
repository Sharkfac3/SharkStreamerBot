---
id: triggers-twitch-channel-goal
type: reference
description: Streamer.bot Twitch Channel Goal trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: seeded
upstream: https://docs.streamer.bot/api/triggers/twitch/channel-goal
---

# Twitch — Channel Goal Triggers

## Goal Begin

- Path: Twitch -> Channel Goal -> Goal Begin
- Upstream: https://docs.streamer.bot/api/triggers/twitch/channel-goal/goal-begin
- Min SB version: v0.1.15
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `goal.currentAmount` | number | Current amount of the channel goal. |
| `goal.description` | string | Description of the channel goal. |
| `goal.id` | string | Unique ID of the channel goal. |
| `goal.startedAt` | DateTime | Timestamp the goal started. |
| `goal.targetAmount` | number | Target amount for the goal. |
| `goal.type` | string | Goal type: `follow`, `subscription`, `new_subscription`, `new_subscription_count`, `new_bit`, `new_cheerer`. |

### Caveats

- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## Goal End

- Path: Twitch -> Channel Goal -> Goal End
- Upstream: https://docs.streamer.bot/api/triggers/twitch/channel-goal/goal-end
- Min SB version: v0.1.15
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `goal.currentAmount` | number | Final amount of the channel goal. |
| `goal.description` | string | Description of the channel goal. |
| `goal.endedAt` | DateTime | Timestamp the goal ended. |
| `goal.id` | string | Unique ID of the channel goal. |
| `goal.startedAt` | DateTime | Timestamp the goal started. |
| `goal.targetAmount` | number | Target amount for the goal. |
| `goal.type` | string | Goal type: `follow`, `subscription`, `new_subscription`, `new_subscription_count`, `new_bit`, `new_cheerer`. |

### Caveats

- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## Goal Progress

- Path: Twitch -> Channel Goal -> Goal Progress
- Upstream: https://docs.streamer.bot/api/triggers/twitch/channel-goal/goal-progress
- Min SB version: v0.1.15
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `goal.currentAmount` | number | Current amount of the channel goal. |
| `goal.description` | string | Description of the channel goal. |
| `goal.id` | string | Unique ID of the channel goal. |
| `goal.startedAt` | DateTime | Timestamp the goal started. |
| `goal.targetAmount` | number | Target amount for the goal. |
| `goal.type` | string | Goal type: `follow`, `subscription`, `new_subscription`, `new_subscription_count`, `new_bit`, `new_cheerer`. |

### Caveats

- EventSub-based trigger.

### Used in repo

_Not yet wired._
