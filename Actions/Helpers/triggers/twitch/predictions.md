---
id: triggers-twitch-predictions
type: reference
description: Streamer.bot Twitch Predictions trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: seeded
upstream: https://docs.streamer.bot/api/triggers/twitch/predictions
---

# Twitch — Predictions Triggers

## Prediction Canceled

- Path: Twitch -> Predictions -> Prediction Canceled
- Upstream: https://docs.streamer.bot/api/triggers/twitch/predictions/prediction-canceled
- Min SB version: v0.0.50
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `prediction._json` | string | Full prediction object as JSON. |
| `prediction.CreatedAt` | DateTime | When the prediction was created. |
| `prediction.EndedAt` | DateTime | When the prediction was canceled. |
| `prediction.Id` | string | Unique prediction ID. |
| `prediction.LockedAt` | DateTime | When the prediction was locked (if it was). |
| `prediction.outcome#.badge.url` | string | Badge image URL for outcome at index #. |
| `prediction.outcome#.badge.version` | string | Badge version for outcome at index #. |
| `prediction.outcome#.channelPoints` | number | Channel points wagered on outcome at index #. |
| `prediction.outcome#.color` | string | Color of outcome at index # (`BLUE` or `PINK`). |
| `prediction.outcome#.id` | string | ID of outcome at index #. |
| `prediction.outcome#.title` | string | Title of outcome at index #. |
| `prediction.outcome#.topPredictors` | string | JSON array of top predictors for outcome at index #. |
| `prediction.outcome#.users` | number | Number of users who chose outcome at index #. |
| `prediction.PredictionWindow` | number | Prediction window duration in seconds. |
| `prediction.Title` | string | Prediction question text. |

### Caveats

- Canceled predictions refund all channel points wagered.
- No `prediction.winningOutcome.*` — no winner when canceled.
- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## Prediction Completed

- Path: Twitch -> Predictions -> Prediction Completed
- Upstream: https://docs.streamer.bot/api/triggers/twitch/predictions/prediction-completed
- Min SB version: v0.0.50
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `prediction._json` | string | Full prediction object as JSON. |
| `prediction.CreatedAt` | DateTime | When the prediction was created. |
| `prediction.EndedAt` | DateTime | When the prediction was resolved. |
| `prediction.Id` | string | Unique prediction ID. |
| `prediction.LockedAt` | DateTime | When the prediction was locked. |
| `prediction.outcome#.badge.url` | string | Badge image URL for outcome at index #. |
| `prediction.outcome#.badge.version` | string | Badge version for outcome at index #. |
| `prediction.outcome#.channelPoints` | number | Channel points wagered on outcome at index #. |
| `prediction.outcome#.color` | string | Color of outcome at index # (`BLUE` or `PINK`). |
| `prediction.outcome#.id` | string | ID of outcome at index #. |
| `prediction.outcome#.title` | string | Title of outcome at index #. |
| `prediction.outcome#.topPredictors` | string | JSON array of top predictors for outcome at index #. |
| `prediction.outcome#.users` | number | Number of users who chose outcome at index #. |
| `prediction.PredictionWindow` | number | Prediction window duration in seconds. |
| `prediction.Title` | string | Prediction question text. |
| `prediction.winningOutcome.channelPoints` | number | Channel points wagered on winning outcome. |
| `prediction.winningOutcome.color` | string | Color of winning outcome. |
| `prediction.winningOutcome.id` | string | ID of winning outcome. |
| `prediction.winningOutcome.title` | string | Title of winning outcome. |
| `prediction.winningOutcome.users` | number | Users who chose the winning outcome. |

### Caveats

- Use `prediction.winningOutcome.*` to branch on the result.
- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## Prediction Created

- Path: Twitch -> Predictions -> Prediction Created
- Upstream: https://docs.streamer.bot/api/triggers/twitch/predictions/prediction-created
- Min SB version: v0.0.50
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `prediction._json` | string | Full prediction object as JSON. |
| `prediction.CreatedAt` | DateTime | When the prediction was created. |
| `prediction.Id` | string | Unique prediction ID. |
| `prediction.outcome#.badge.url` | string | Badge image URL for outcome at index #. |
| `prediction.outcome#.badge.version` | string | Badge version for outcome at index #. |
| `prediction.outcome#.channelPoints` | number | Channel points wagered (0 at creation). |
| `prediction.outcome#.color` | string | Color of outcome at index # (`BLUE` or `PINK`). |
| `prediction.outcome#.id` | string | ID of outcome at index #. |
| `prediction.outcome#.title` | string | Title of outcome at index #. |
| `prediction.outcome#.users` | number | Users who chose this outcome (0 at creation). |
| `prediction.PredictionWindow` | number | Prediction window duration in seconds. |
| `prediction.Title` | string | Prediction question text. |

### Caveats

- No `prediction.EndedAt`, `prediction.LockedAt`, or `prediction.winningOutcome.*` — prediction not yet locked or resolved.
- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## Prediction Locked

- Path: Twitch -> Predictions -> Prediction Locked
- Upstream: https://docs.streamer.bot/api/triggers/twitch/predictions/prediction-locked
- Min SB version: v0.0.50
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `prediction._json` | string | Full prediction object as JSON. |
| `prediction.CreatedAt` | DateTime | When the prediction was created. |
| `prediction.Id` | string | Unique prediction ID. |
| `prediction.LockedAt` | DateTime | When the prediction was locked. |
| `prediction.outcome#.badge.url` | string | Badge image URL for outcome at index #. |
| `prediction.outcome#.badge.version` | string | Badge version for outcome at index #. |
| `prediction.outcome#.channelPoints` | number | Channel points wagered on outcome at index #. |
| `prediction.outcome#.color` | string | Color of outcome at index # (`BLUE` or `PINK`). |
| `prediction.outcome#.id` | string | ID of outcome at index #. |
| `prediction.outcome#.title` | string | Title of outcome at index #. |
| `prediction.outcome#.topPredictors` | string | JSON array of top predictors for outcome at index #. |
| `prediction.outcome#.users` | number | Number of users who chose outcome at index #. |
| `prediction.PredictionWindow` | number | Prediction window duration in seconds. |
| `prediction.Title` | string | Prediction question text. |

### Caveats

- Locked = voting closed; no more entries. Adds `prediction.LockedAt` vs. Created.
- No `prediction.EndedAt` or `prediction.winningOutcome.*` — result not yet resolved.
- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## Prediction Updated

- Path: Twitch -> Predictions -> Prediction Updated
- Upstream: https://docs.streamer.bot/api/triggers/twitch/predictions/prediction-updated
- Min SB version: v0.0.50
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `prediction._json` | string | Full prediction object as JSON. |
| `prediction.CreatedAt` | DateTime | When the prediction was created. |
| `prediction.Id` | string | Unique prediction ID. |
| `prediction.outcome#.badge.url` | string | Badge image URL for outcome at index #. |
| `prediction.outcome#.badge.version` | string | Badge version for outcome at index #. |
| `prediction.outcome#.channelPoints` | number | Channel points wagered on outcome at index #. |
| `prediction.outcome#.color` | string | Color of outcome at index # (`BLUE` or `PINK`). |
| `prediction.outcome#.id` | string | ID of outcome at index #. |
| `prediction.outcome#.title` | string | Title of outcome at index #. |
| `prediction.outcome#.topPredictors` | string | JSON array of top predictors for outcome at index #. |
| `prediction.outcome#.users` | number | Number of users who chose outcome at index #. |
| `prediction.PredictionWindow` | number | Prediction window duration in seconds. |
| `prediction.Title` | string | Prediction question text. |

### Caveats

- Fires each time a user places or changes a prediction vote.
- No `prediction.LockedAt`, `prediction.EndedAt`, or `prediction.winningOutcome.*` — prediction still open.
- EventSub-based trigger.

### Used in repo

_Not yet wired._
