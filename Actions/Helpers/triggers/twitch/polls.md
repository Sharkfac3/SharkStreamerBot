---
id: triggers-twitch-polls
type: reference
description: Streamer.bot Twitch Polls trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: seeded
upstream: https://docs.streamer.bot/api/triggers/twitch/polls
---

# Twitch — Polls Triggers

## Poll Archived

- Path: Twitch -> Polls -> Poll Archived
- Upstream: https://docs.streamer.bot/api/triggers/twitch/polls/poll-archived
- Min SB version: v0.2.4
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `poll._json` | string | Full poll object as JSON. |
| `poll.choice#.channelPoints` | number | Channel points spent on choice at index #. |
| `poll.choice#.id` | string | ID of choice at index #. |
| `poll.choice#.title` | string | Title of choice at index #. |
| `poll.choice#.votes` | number | Votes for choice at index #. |
| `poll.choices.count` | number | Total number of choices. |
| `poll.Duration` | number | Poll duration in seconds. |
| `poll.DurationRemaining` | number | Seconds remaining when archived. |
| `poll.endedAt` | DateTime | When the poll ended. |
| `poll.Id` | string | Unique poll ID. |
| `poll.rewardVotes` | number | Total channel point votes cast. |
| `poll.StartedAt` | DateTime | When the poll started. |
| `poll.Title` | string | Poll question text. |
| `poll.totalVotes` | number | Total votes cast across all choices. |
| `poll.votes` | number | Total regular votes cast. |
| `poll.winningChoice.channelPoints` | number | Channel points spent on winning choice. |
| `poll.winningChoice.id` | string | ID of winning choice. |
| `poll.winningChoice.title` | string | Title of winning choice. |
| `poll.winningChoice.votes` | number | Votes for winning choice. |
| `poll.winningIndex` | number | Index of the winning choice. |

### Caveats

- Fires for polls that are abandoned before expiry. Adds `poll.endedAt` and `poll.winningChoice.*`.
- EventSub-based trigger. v0.2.4+.

### Used in repo

_Not yet wired._

---

## Poll Completed

- Path: Twitch -> Polls -> Poll Completed
- Upstream: https://docs.streamer.bot/api/triggers/twitch/polls/poll-completed
- Min SB version: v0.0.50
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `poll._json` | string | Full poll object as JSON. |
| `poll.choice#.channelPoints` | number | Channel points spent on choice at index #. |
| `poll.choice#.id` | string | ID of choice at index #. |
| `poll.choice#.title` | string | Title of choice at index #. |
| `poll.choice#.votes` | number | Votes for choice at index #. |
| `poll.choices.count` | number | Total number of choices. |
| `poll.Duration` | number | Poll duration in seconds. |
| `poll.DurationRemaining` | number | Seconds remaining when completed (typically 0). |
| `poll.endedAt` | DateTime | When the poll ended. |
| `poll.Id` | string | Unique poll ID. |
| `poll.rewardVotes` | number | Total channel point votes cast. |
| `poll.StartedAt` | DateTime | When the poll started. |
| `poll.Title` | string | Poll question text. |
| `poll.totalVotes` | number | Total votes cast across all choices. |
| `poll.votes` | number | Total regular votes cast. |
| `poll.winningChoice.channelPoints` | number | Channel points spent on winning choice. |
| `poll.winningChoice.id` | string | ID of winning choice. |
| `poll.winningChoice.title` | string | Title of winning choice. |
| `poll.winningChoice.votes` | number | Votes for winning choice. |
| `poll.winningIndex` | number | Index of the winning choice. |

### Caveats

- Use `poll.winningChoice.*` and `poll.winningIndex` to branch on the result.
- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## Poll Created

- Path: Twitch -> Polls -> Poll Created
- Upstream: https://docs.streamer.bot/api/triggers/twitch/polls/poll-created
- Min SB version: v0.0.50
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `poll._json` | string | Full poll object as JSON. |
| `poll.choice#.channelPoints` | number | Channel points spent on choice at index # (0 at creation). |
| `poll.choice#.id` | string | ID of choice at index #. |
| `poll.choice#.title` | string | Title of choice at index #. |
| `poll.choice#.votes` | number | Votes for choice at index # (0 at creation). |
| `poll.choices.count` | number | Total number of choices. |
| `poll.Duration` | number | Poll duration in seconds. |
| `poll.DurationRemaining` | number | Seconds remaining. |
| `poll.Id` | string | Unique poll ID. |
| `poll.rewardVotes` | number | Total channel point votes cast (0 at creation). |
| `poll.StartedAt` | DateTime | When the poll started. |
| `poll.Title` | string | Poll question text. |
| `poll.totalVotes` | number | Total votes cast (0 at creation). |
| `poll.votes` | number | Total regular votes cast (0 at creation). |

### Caveats

- No `poll.endedAt` or `poll.winningChoice.*` — poll has not ended yet.
- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## Poll Terminated

- Path: Twitch -> Polls -> Poll Terminated
- Upstream: https://docs.streamer.bot/api/triggers/twitch/polls/poll-terminated
- Min SB version: v0.0.50
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `poll._json` | string | Full poll object as JSON. |
| `poll.choice#.channelPoints` | number | Channel points spent on choice at index #. |
| `poll.choice#.id` | string | ID of choice at index #. |
| `poll.choice#.title` | string | Title of choice at index #. |
| `poll.choice#.votes` | number | Votes for choice at index #. |
| `poll.choices.count` | number | Total number of choices. |
| `poll.Duration` | number | Poll duration in seconds. |
| `poll.DurationRemaining` | number | Seconds remaining when terminated. |
| `poll.endedAt` | DateTime | When the poll was terminated. |
| `poll.Id` | string | Unique poll ID. |
| `poll.rewardVotes` | number | Total channel point votes cast. |
| `poll.StartedAt` | DateTime | When the poll started. |
| `poll.Title` | string | Poll question text. |
| `poll.totalVotes` | number | Total votes cast. |
| `poll.votes` | number | Total regular votes cast. |
| `poll.winningChoice.channelPoints` | number | Channel points spent on winning choice. |
| `poll.winningChoice.id` | string | ID of winning choice. |
| `poll.winningChoice.title` | string | Title of winning choice. |
| `poll.winningChoice.votes` | number | Votes for winning choice. |
| `poll.winningIndex` | number | Index of the winning choice. |

### Caveats

- Terminated polls are ended early by a moderator or broadcaster. Still produces a winner.
- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## Poll Updated

- Path: Twitch -> Polls -> Poll Updated
- Upstream: https://docs.streamer.bot/api/triggers/twitch/polls/poll-updated
- Min SB version: v0.0.50
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `poll._json` | string | Full poll object as JSON. |
| `poll.choice#.channelPoints` | number | Channel points spent on choice at index #. |
| `poll.choice#.id` | string | ID of choice at index #. |
| `poll.choice#.title` | string | Title of choice at index #. |
| `poll.choice#.votes` | number | Votes for choice at index #. |
| `poll.choices.count` | number | Total number of choices. |
| `poll.Duration` | number | Poll duration in seconds. |
| `poll.DurationRemaining` | number | Seconds remaining. |
| `poll.Id` | string | Unique poll ID. |
| `poll.rewardVotes` | number | Total channel point votes cast. |
| `poll.StartedAt` | DateTime | When the poll started. |
| `poll.Title` | string | Poll question text. |
| `poll.totalVotes` | number | Total votes cast. |
| `poll.votes` | number | Total regular votes cast. |

### Caveats

- No `poll.endedAt` or `poll.winningChoice.*` — poll is still in progress.
- Fires on each vote increment during an active poll.
- EventSub-based trigger.

### Used in repo

_Not yet wired._
