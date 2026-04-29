---
id: triggers-twitch-subscriptions
type: reference
description: Streamer.bot Twitch Subscriptions trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: seeded
upstream: https://docs.streamer.bot/api/triggers/twitch/subscriptions
---

# Twitch — Subscriptions Triggers

## Gift Bomb

- Path: Twitch -> Subscriptions -> Gift Bomb
- Upstream: https://docs.streamer.bot/api/triggers/twitch/subscriptions/gift-bomb
- Min SB version: any
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `anonymous` | boolean | Whether the gift bomb is anonymous. |
| `bonusGifts` | boolean | Whether Twitch added additional subs to the gift bomb. |
| `gift.recipientId#` | string | ID of the gifted user at index # (0 to totalGifts - 1). |
| `gift.recipientUser#` | string | Display name of the gifted user at index #. |
| `gift.recipientUserName#` | string | Login name of the gifted user at index #. |
| `gifts` | number | Number of subscriptions in this gift bomb. |
| `systemMessage` | string | System message posted to Twitch chat. |
| `tier` | string | Subscription tier: `tier 1`, `tier 2`, `tier 3`. |
| `totalGifts` | number | Total subscriptions ever gifted by the user. |
| `totalGiftsShared` | boolean | Whether the user shares their total gift count publicly. |

### Caveats

- Recipient variables use indexed notation: `gift.recipientId0`, `gift.recipientId1`, … up to `totalGifts - 1`.
- A Gift Bomb triggers Gift Bomb once, then fires Gift Subscription N times (each with `fromGiftBomb = true`). Handle both to avoid double-counting; see [subscription-gift.cs](../../Twitch%20Core%20Integrations/subscription-gift.cs) for routing logic.

### Used in repo

- [Actions/Twitch Core Integrations/subscription-gift.cs](../../Twitch%20Core%20Integrations/subscription-gift.cs) — routes Gift Bomb and individual gift events.

---

## Gift Paid Upgrade

- Path: Twitch -> Subscriptions -> Gift Paid Upgrade
- Upstream: https://docs.streamer.bot/api/triggers/twitch/subscriptions/gift-paid-upgrade
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

_No trigger-specific args. Shared variable groups apply (see upstream)._

### Caveats

- EventSub-based trigger. v0.2.5+.
- Fires when a user upgrades their gifted subscription to a paid one.

### Used in repo

- [Actions/Twitch Core Integrations/subscription-gift-paid-upgrade.cs](../../Twitch%20Core%20Integrations/subscription-gift-paid-upgrade.cs)

---

## Gift Subscription

- Path: Twitch -> Subscriptions -> Gift Subscription
- Upstream: https://docs.streamer.bot/api/triggers/twitch/subscriptions/gift-subscription
- Min SB version: any
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `anonymous` | boolean | Whether the gift sub is anonymous. |
| `cumulativeMonths` | number | Months the recipient has been subscribed total. |
| `fromGiftBomb` | boolean | Whether this gift sub is part of a gift bomb. |
| `monthsGifted` | number | Prepaid months gifted: 1, 3, 6, or 12. |
| `random` | boolean | Whether the recipient was randomly chosen (vs. targeted). |
| `recipientId` | string | Recipient user's unique ID. |
| `recipientUser` | string | Recipient's display name. |
| `recipientUserName` | string | Recipient's login name. |
| `subBombCount` | number | Number of gift subs in the accompanying gift bomb. |
| `systemMessage` | string | System message posted to Twitch chat. |
| `tier` | string | Subscription tier: `tier 1`, `tier 2`, `tier 3`. |
| `totalSubsGifted` | number | Total subscriptions ever gifted by the gifting user. |
| `totalSubsGiftedShared` | boolean | Whether the gifting user shares their total gift count. |

### Caveats

- `monthsGifted` values: 1, 3, 6, or 12 only.
- When a Gift Bomb fires, this trigger fires N times (`fromGiftBomb = true`). Route via `fromGiftBomb` to avoid double-processing with Gift Bomb trigger; see [subscription-gift.cs](../../Twitch%20Core%20Integrations/subscription-gift.cs).

### Used in repo

- [Actions/Twitch Core Integrations/subscription-gift.cs](../../Twitch%20Core%20Integrations/subscription-gift.cs) — handles solo and bomb gift subs.

---

## Pay It Forward

- Path: Twitch -> Subscriptions -> Pay It Forward
- Upstream: https://docs.streamer.bot/api/triggers/twitch/subscriptions/pay-it-forward
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

_No trigger-specific args. Shared variable groups apply (see upstream)._

### Caveats

- EventSub-based trigger. v0.2.5+.
- Fires when a user pays forward their gifted subscription.

### Used in repo

- [Actions/Twitch Core Integrations/subscription-pay-it-forward.cs](../../Twitch%20Core%20Integrations/subscription-pay-it-forward.cs)

---

## Prime Paid Upgrade

- Path: Twitch -> Subscriptions -> Prime Paid Upgrade
- Upstream: https://docs.streamer.bot/api/triggers/twitch/subscriptions/prime-paid-upgrade
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `systemMessage` | string | Message Twitch posts to chat for this event. |
| `upgradeTier` | number | Tier upgraded to (numeric, e.g. `1000` for tier 1). |
| `upgradeTierString` | string | Tier upgraded to as text (e.g. `tier 1`). |

### Caveats

- EventSub-based trigger. v0.2.5+.
- Fires when a user upgrades their Prime Gaming subscription to tier 1, 2, or 3.

### Used in repo

- [Actions/Twitch Core Integrations/subscription-prime-paid-upgrade.cs](../../Twitch%20Core%20Integrations/subscription-prime-paid-upgrade.cs)

---

## Resubscription

- Path: Twitch -> Subscriptions -> Resubscription
- Upstream: https://docs.streamer.bot/api/triggers/twitch/subscriptions/resubscription
- Min SB version: any
- Shared groups: General, Twitch Broadcaster, Twitch Chat, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `cumulative` | number | Total cumulative months the user has been subscribed. |
| `isMultiMonth` | bool | Whether the resubscription covers 3, 6, or 12 months. |
| `monthStreak` | number | Current subscription streak in months. |
| `multiMonthDuration` | int | Length of multi-month subscription (3, 6, or 12). |
| `multiMonthTenure` | int | Elapsed months of the multi-month subscription. |
| `streakShared` | boolean | Whether the user shares their resub streak publicly. |
| `tier` | string | Subscription tier: `prime`, `tier 1`, `tier 2`, `tier 3`. |

### Caveats

- `multiMonthDuration` and `multiMonthTenure` are 0 when `isMultiMonth` is false.

### Used in repo

- [Actions/Twitch Core Integrations/subscription-renewed.cs](../../Twitch%20Core%20Integrations/subscription-renewed.cs)

---

<a id="sub-counter-rollover"></a>
## Sub Counter Rollover

- Path: Twitch -> Subscriptions -> Sub Counter Rollover
- Upstream: https://docs.streamer.bot/api/triggers/twitch/subscriptions/sub-counter-rollover
- Min SB version: any
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `rollover` | number | The configured rollover threshold (e.g. `50`). |
| `rolloverCount` | number | Number of times the rollover value has been reached (e.g. `3`). |
| `subCounter` | number | Current value of the sub counter (e.g. `20`). |

### Caveats

- **Twitch Chat and Twitch User shared groups are NOT available** — only General and Twitch Broadcaster apply.
- Rollover threshold is configured in Streamer.bot's sub counter settings, not Twitch.
- This is a Streamer.bot internal counter event, not a Twitch EventSub event.

### Used in repo

- [Actions/Twitch Core Integrations/subscription-counter-rollover.cs](../../Twitch%20Core%20Integrations/subscription-counter-rollover.cs)

---

## Subscription

- Path: Twitch -> Subscriptions -> Subscription
- Upstream: https://docs.streamer.bot/api/triggers/twitch/subscriptions/subscription
- Min SB version: any
- Shared groups: General, Twitch Broadcaster, Twitch Chat, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `isMultiMonth` | bool | Whether the subscription spans multiple months. |
| `multiMonthDuration` | int | Total months in a multi-month subscription. |
| `multiMonthTenure` | int | Elapsed months of the multi-month subscription. |
| `tier` | string | Subscription tier: `prime`, `tier 1`, `tier 2`, `tier 3`. |

### Caveats

- `multiMonthDuration` and `multiMonthTenure` are 0 when `isMultiMonth` is false.
- Fires for new (first-time) subscriptions only. Renewals use the Resubscription trigger.

### Used in repo

- [Actions/Twitch Core Integrations/subscription-new.cs](../../Twitch%20Core%20Integrations/subscription-new.cs)
