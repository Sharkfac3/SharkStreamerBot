---
id: triggers-twitch-charity
type: reference
description: Streamer.bot Twitch Charity trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: seeded
upstream: https://docs.streamer.bot/api/triggers/twitch/charity
---

# Twitch — Charity Triggers

## Completed

- Path: Twitch -> Charity -> Completed
- Upstream: https://docs.streamer.bot/api/triggers/twitch/charity/completed
- Min SB version: v0.1.14
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `charity.currentAmount.currency` | string | ISO 4217 currency code of amount raised. |
| `charity.currentAmount.value` | number | Current amount raised. |
| `charity.currentAmount.valueMinor` | number | Current amount raised in minor format. |
| `charity.description` | string | Description of the charity campaign. |
| `charity.id` | string | Unique ID of the charity campaign. |
| `charity.logo` | string | Image URL of the charity logo. |
| `charity.name` | string | Name of the charity campaign. |
| `charity.stoppedAt` | DateTime | Datetime the charity campaign completed. |
| `charity.targetAmount.currency` | string | ISO 4217 currency code of target amount. |
| `charity.targetAmount.value` | number | Target amount to raise. |
| `charity.targetAmount.valueMinor` | number | Target amount in minor format. |
| `charity.website` | string | Website URL of the charity campaign. |

### Caveats

- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## Donation

- Path: Twitch -> Charity -> Donation
- Upstream: https://docs.streamer.bot/api/triggers/twitch/charity/donation
- Min SB version: v0.1.14
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `charity.amount.currency` | string | ISO 4217 currency code. |
| `charity.amount.value` | number | Donation amount. |
| `charity.amount.valueMinor` | number | Donation amount in minor format. |
| `charity.campaignId` | string | Unique ID of the charity campaign. |
| `charity.description` | string | Description of the charity campaign. |
| `charity.logo` | string | Image URL of the charity logo. |
| `charity.name` | string | Name of the charity campaign. |
| `charity.website` | string | Website URL of the charity campaign. |

### Caveats

- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## Progress

- Path: Twitch -> Charity -> Progress
- Upstream: https://docs.streamer.bot/api/triggers/twitch/charity/progress
- Min SB version: v0.1.15
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `charity.currentAmount.currency` | string | ISO 4217 currency code of amount raised. |
| `charity.currentAmount.value` | number | Current amount raised. |
| `charity.currentAmount.valueMinor` | number | Current amount raised in minor format. |
| `charity.description` | string | Description of the charity campaign. |
| `charity.id` | string | Unique ID of the charity campaign. |
| `charity.logo` | string | Image URL of the charity logo. |
| `charity.name` | string | Name of the charity campaign. |
| `charity.targetAmount.currency` | string | ISO 4217 currency code of target amount. |
| `charity.targetAmount.value` | number | Target amount to raise. |
| `charity.targetAmount.valueMinor` | number | Target amount in minor format. |
| `charity.website` | string | Website URL of the charity campaign. |

### Caveats

- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## Started

- Path: Twitch -> Charity -> Started
- Upstream: https://docs.streamer.bot/api/triggers/twitch/charity/started
- Min SB version: v0.1.15
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `charity.currentAmount.currency` | string | ISO 4217 currency code of amount raised so far. |
| `charity.currentAmount.value` | number | Current amount raised. |
| `charity.currentAmount.valueMinor` | number | Current amount raised in minor format. |
| `charity.description` | string | Description of the charity campaign. |
| `charity.id` | string | Unique ID of the charity campaign. |
| `charity.logo` | string | Image URL of the charity logo. |
| `charity.name` | string | Name of the charity campaign. |
| `charity.startedAt` | DateTime | Datetime the charity campaign started. |
| `charity.targetAmount.currency` | string | ISO 4217 currency code of target amount. |
| `charity.targetAmount.value` | number | Target amount to raise. |
| `charity.targetAmount.valueMinor` | number | Target amount in minor format. |
| `charity.website` | string | Website URL of the charity campaign. |

### Caveats

- EventSub-based trigger.

### Used in repo

_Not yet wired._
