---
id: triggers-twitch-shared-chat
type: reference
description: Streamer.bot Twitch Shared Chat trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: partial
upstream: https://docs.streamer.bot/api/triggers/twitch/shared-chat
---

# Twitch — Shared Chat Triggers

All Shared Chat triggers require Streamer.bot v0.2.5+ and are EventSub-based unless noted. Most include the **Shared Chat Source** variable group (6 vars) identifying the originating channel within the shared chat session.

---

## Announcement

- Path: Twitch -> Shared Chat -> Announcement
- Upstream: https://docs.streamer.bot/api/triggers/twitch/shared-chat/announcement
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Twitch Chat, Shared Chat Source, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `announceColor` | string | Announcement color (e.g. `Green`). |
| `fromSharedChat` | boolean | Whether the announcement originated from the shared chat. |
| `isSubscribed` | boolean | Whether the poster is subscribed. |
| `message` | string | Full announcement text. |
| `messageStripped` | string | Announcement text with emote content removed. |
| `role` | string | Poster's role (e.g. `Broadcaster`). |

### Caveats

- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## Community Sub Gift

- Path: Twitch -> Shared Chat -> Community Sub Gift
- Upstream: https://docs.streamer.bot/api/triggers/twitch/shared-chat/community-sub-gift
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Twitch Chat, Shared Chat Source, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `anonymous` | boolean | Whether the gift bomb was anonymous. |
| `bonusGifts` | boolean | Whether Twitch added extra subscriptions to the bomb. |
| `gifts` | number | Number of subscriptions in this gift bomb. |
| `tier` | string | Subscription tier: `prime`, `tier 1`, `tier 2`, `tier 3`. |
| `totalGifts` | number | Total subscriptions the user has ever gifted. |

### Caveats

- EventSub-based trigger.
- Fires once for the entire bomb. Shared Chat Sub Gift fires individually for each recipient.

### Used in repo

_Not yet wired._

---

## Gift Paid Upgrade

- Path: Twitch -> Shared Chat -> Gift Paid Upgrade
- Upstream: https://docs.streamer.bot/api/triggers/twitch/shared-chat/gift-paid-upgrade
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Shared Chat Source, Twitch User
- Coverage: seeded

### Args

_No trigger-specific args identified in upstream docs. Shared variable groups apply (see upstream)._

### Caveats

- EventSub-based trigger.
- Fires when a user upgrades their gifted subscription to a paid one in a shared chat.

### Used in repo

_Not yet wired._

---

## Message Deleted

- Path: Twitch -> Shared Chat -> Message Deleted
- Upstream: https://docs.streamer.bot/api/triggers/twitch/shared-chat/message-deleted
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Shared Chat Source, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `message` | string | Text content of the deleted message. |
| `targetMessageId` | string | Unique ID of the deleted message. |

### Caveats

- EventSub-based trigger.
- Twitch User shared group identifies the user whose message was deleted.

### Used in repo

_Not yet wired._

---

## Pay It Forward

- Path: Twitch -> Shared Chat -> Pay It Forward
- Upstream: https://docs.streamer.bot/api/triggers/twitch/shared-chat/pay-it-forward
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Shared Chat Source, Twitch User
- Coverage: seeded

### Args

_No trigger-specific args identified in upstream docs. Shared variable groups apply (see upstream)._

### Caveats

- EventSub-based trigger.
- Fires when a user pays their gifted sub forward in a shared chat.

### Used in repo

_Not yet wired._

---

## Prime Paid Upgrade

- Path: Twitch -> Shared Chat -> Prime Paid Upgrade
- Upstream: https://docs.streamer.bot/api/triggers/twitch/shared-chat/prime-paid-upgrade
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Shared Chat Source, Twitch User
- Coverage: seeded

### Args

_No trigger-specific args identified in upstream docs. Shared variable groups apply (see upstream)._

### Caveats

- EventSub-based trigger.
- Fires when a user upgrades their Prime Gaming subscription to Tier 1, 2, or 3 in a shared chat.

### Used in repo

_Not yet wired._

---

## Raid

- Path: Twitch -> Shared Chat -> Raid
- Upstream: https://docs.streamer.bot/api/triggers/twitch/shared-chat/raid
- Min SB version: _unknown_
- Shared groups: _unknown_
- Coverage: partial

### Args

_Upstream documentation not yet available (marked "Documentation Needed" on docs.streamer.bot). Variable list pending upstream update._

### Caveats

_None recorded yet._

### Used in repo

_Not yet wired._

---

## Resub

- Path: Twitch -> Shared Chat -> Resub
- Upstream: https://docs.streamer.bot/api/triggers/twitch/shared-chat/resub
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Twitch Chat, Shared Chat Source, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `cumulative` | number | Total cumulative months the user has been subscribed. |
| `monthStreak` | number | Current subscription streak in months. |
| `streakShared` | boolean | Whether the user shares their resub streak publicly. |
| `tier` | string | Subscription tier: `prime`, `tier 1`, `tier 2`, `tier 3`. |

### Caveats

- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## Session Begin

- Path: Twitch -> Shared Chat -> Session Begin
- Upstream: https://docs.streamer.bot/api/triggers/twitch/shared-chat/session-begin
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Twitch Chat, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `host.isModerator` | boolean | Whether the host is a moderator in the broadcaster's channel. |
| `host.isSubscribed` | boolean | Whether the host is subscribed to the broadcaster. |
| `host.isVip` | boolean | Whether the host has VIP status in the broadcaster's channel. |
| `host.userId` | string | Twitch user ID of the shared chat host. |
| `host.userLogin` | string | Login name of the host. |
| `host.userName` | string | Display name of the host. |

### Caveats

- Chat Client trigger.
- No Shared Chat Source group — this trigger is about the session itself, not a chat event.

### Used in repo

_Not yet wired._

---

## Session End

- Path: Twitch -> Shared Chat -> Session End
- Upstream: https://docs.streamer.bot/api/triggers/twitch/shared-chat/session-end
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Twitch Chat, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `host.isModerator` | boolean | Whether the host is a moderator in the broadcaster's channel. |
| `host.isSubscribed` | boolean | Whether the host is subscribed to the broadcaster. |
| `host.isVip` | boolean | Whether the host has VIP status in the broadcaster's channel. |
| `host.userId` | string | Twitch user ID of the shared chat host. |
| `host.userLogin` | string | Login name of the host. |
| `host.userName` | string | Display name of the host. |

### Caveats

- Chat Client trigger.
- No Shared Chat Source group — this trigger is about the session itself, not a chat event.

### Used in repo

_Not yet wired._

---

## Session Update

- Path: Twitch -> Shared Chat -> Session Update
- Upstream: https://docs.streamer.bot/api/triggers/twitch/shared-chat/session-update
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Twitch Chat, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `host.isModerator` | boolean | Whether the host is a moderator in the broadcaster's channel. |
| `host.isSubscribed` | boolean | Whether the host is subscribed to the broadcaster. |
| `host.isVip` | boolean | Whether the host has VIP status in the broadcaster's channel. |
| `host.userId` | string | Twitch user ID of the shared chat host. |
| `host.userLogin` | string | Login name of the host. |
| `host.userName` | string | Display name of the host. |

### Caveats

- Chat Client trigger.
- No Shared Chat Source group — this trigger is about the session itself, not a chat event.

### Used in repo

_Not yet wired._

---

## Sub

- Path: Twitch -> Shared Chat -> Sub
- Upstream: https://docs.streamer.bot/api/triggers/twitch/shared-chat/sub
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Twitch Chat, Shared Chat Source, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `tier` | string | Subscription tier: `prime`, `tier 1`, `tier 2`, `tier 3`. |

### Caveats

- EventSub-based trigger.
- Fires for new (first-time) subscriptions in a shared chat. Renewals use Resub.

### Used in repo

_Not yet wired._

---

## Sub Gift

- Path: Twitch -> Shared Chat -> Sub Gift
- Upstream: https://docs.streamer.bot/api/triggers/twitch/shared-chat/sub-gift
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Twitch Chat, Shared Chat Source, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `anonymous` | boolean | Whether the gift was anonymous. |
| `cumulativeMonths` | number | Recipient's total cumulative subscription months. |
| `fromGiftBomb` | boolean | Whether this gift is part of a Community Sub Gift bomb. |
| `monthsGifted` | number | Prepaid months gifted: `1`, `3`, `6`, or `12`. |
| `recipientId` | string | Recipient's user ID. |
| `recipientUser` | string | Recipient's display name. |
| `recipientUserName` | string | Recipient's login name. |
| `subBombCount` | number | Total gift subs in the accompanying bomb. |
| `tier` | string | Subscription tier: `prime`, `tier 1`, `tier 2`, `tier 3`. |
| `totalSubsGifted` | number | Total subscriptions ever gifted by the gifting user. |

### Caveats

- EventSub-based trigger.
- When a Community Sub Gift fires, this trigger fires N times (`fromGiftBomb = true`).

### Used in repo

_Not yet wired._

---

## User Banned

- Path: Twitch -> Shared Chat -> User Banned
- Upstream: https://docs.streamer.bot/api/triggers/twitch/shared-chat/user-banned
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Shared Chat Source, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `createdAt` | DateTime | When the ban occurred. |
| `createdByDisplayName` | string | Display name of the moderator who issued the ban. |
| `createdById` | string | User ID of the moderator. |
| `createdByUsername` | string | Login name of the moderator. |
| `reason` | string | Reason provided for the ban. |
| `user` | string | Display name of the banned user (only if present in chat). |
| `userId` | string | Twitch ID of the banned user. |
| `userName` | string | Login name of the banned user. |

### Caveats

- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## User Timed Out

- Path: Twitch -> Shared Chat -> User Timed Out
- Upstream: https://docs.streamer.bot/api/triggers/twitch/shared-chat/user-timed-out
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Shared Chat Source, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `createdAt` | DateTime | When the timeout was issued. |
| `createdByDisplayName` | string | Display name of the moderator who issued the timeout. |
| `createdById` | string | User ID of the moderator. |
| `createdByUsername` | string | Login name of the moderator. |
| `duration` | number | Timeout length in seconds. |
| `reason` | string | Reason provided for the timeout. |
| `user` | string | Display name of the timed-out user (only if present in chat). |
| `userId` | string | Twitch ID of the timed-out user. |
| `userName` | string | Login name of the timed-out user. |

### Caveats

- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## User Unbanned

- Path: Twitch -> Shared Chat -> User Unbanned
- Upstream: https://docs.streamer.bot/api/triggers/twitch/shared-chat/user-unbanned
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `createdAt` | datetime | When the unban occurred. |
| `createdByDisplayName` | string | Display name of the moderator who unbanned the user. |
| `createdById` | string | User ID of the moderator. |
| `createdByUsername` | string | Login name of the moderator. |

### Caveats

- EventSub-based trigger.
- Twitch User shared group identifies the user who was unbanned.

### Used in repo

_Not yet wired._

---

## User Untimed Out

- Path: Twitch -> Shared Chat -> User Untimed Out
- Upstream: https://docs.streamer.bot/api/triggers/twitch/shared-chat/user-untimed-out
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Shared Chat Source, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `createdAt` | datetime | When the timeout was lifted. |
| `createdByDisplayName` | string | Display name of the moderator who removed the timeout. |
| `createdById` | string | User ID of the moderator. |
| `createdByUsername` | string | Login name of the moderator. |

### Caveats

- EventSub-based trigger.
- Twitch User shared group identifies the user whose timeout was removed.

### Used in repo

_Not yet wired._
