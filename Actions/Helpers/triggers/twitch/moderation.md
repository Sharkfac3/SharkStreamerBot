---
id: triggers-twitch-moderation
type: reference
description: Streamer.bot Twitch Moderation trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: partial
upstream: https://docs.streamer.bot/api/triggers/twitch/moderation
---

# Twitch — Moderation Triggers

## Automod Message Held

- Path: Twitch -> Moderation -> Automod Message Held
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/automod-message-held
- Min SB version: v0.2.4
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `category` | string | AutoMod category: `swearing`, `ableism`, `aggression`, `misogyny`, `namecalling`, `bullying`. |
| `heldAt` | datetime | When the message was held. |
| `input#` | string | Individual words from the held message, indexed from `input0`. |
| `level` | int | AutoMod filter level that caught the message (e.g. `1`). |
| `messageId` | string | Unique ID of the held message. |
| `rawInput` | string | Complete original message text. |

### Caveats

- EventSub-based trigger. v0.2.4+.
- Use `rawInput` for the full message and `input#` to inspect individual words.

### Used in repo

_Not yet wired._

---

## Automod Message Updated

- Path: Twitch -> Moderation -> Automod Message Updated
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/automod-message-updated
- Min SB version: v0.2.4
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `category` | string | AutoMod category: `swearing`, `ableism`, `aggression`, `misogyny`, `namecalling`, `bullying`. |
| `heldAt` | datetime | When the message was originally held. |
| `input#` | string | Individual words from the message, indexed from `input0`. |
| `level` | int | AutoMod filter level. |
| `messageId` | string | Unique ID of the held message. |
| `rawInput` | string | Complete original message text. |
| `status` | string | Resolution: `approved` or `denied`. |

### Caveats

- EventSub-based trigger. v0.2.4+.
- Fires when a moderator approves or denies a held AutoMod message. Use `status` to branch.

### Used in repo

_Not yet wired._

---

## Blocked Terms Added

- Path: Twitch -> Moderation -> Blocked Terms Added
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/blocked-terms-added
- Min SB version: v0.2.4
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `createdAt` | datetime | When the term(s) were added. |
| `fromAutomod` | bool | Whether AutoMod added the term (vs. a moderator). |
| `moderatorDisplayName` | string | Display name of the moderator who added the term. |
| `moderatorId` | string | User ID of the moderator. |
| `moderatorUsername` | string | Login name of the moderator. |
| `term.[#]` | string | Individual blocked terms, indexed from `term.[0]`. |
| `termCount` | int | Total number of terms added in this event. |

### Caveats

- EventSub-based trigger. v0.2.4+.
- No Twitch User shared group — moderator identity is via `moderator*` vars, not the shared group.

### Used in repo

_Not yet wired._

---

## Blocked Terms Deleted

- Path: Twitch -> Moderation -> Blocked Terms Deleted
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/blocked-terms-deleted
- Min SB version: v0.2.4
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `createdAt` | datetime | When the term(s) were originally added (not deletion timestamp). |
| `fromAutomod` | bool | Whether AutoMod originally added the term. |
| `moderatorDisplayName` | string | Display name of the moderator who removed the term. |
| `moderatorId` | string | User ID of the moderator. |
| `moderatorUsername` | string | Login name of the moderator. |
| `term.[#]` | string | Individual deleted terms, indexed from `term.[0]`. |
| `termCount` | int | Total number of terms deleted in this event. |

### Caveats

- EventSub-based trigger. v0.2.4+.
- Same variable structure as Blocked Terms Added — mirrors that trigger for removals.

### Used in repo

_Not yet wired._

---

## Chat Cleared

- Path: Twitch -> Moderation -> Chat Cleared
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/chat-cleared
- Min SB version: v0.1.18
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

_No trigger-specific args. Shared variable groups apply (see upstream)._

### Caveats

- Chat Client trigger.
- Fires when a moderator or broadcaster clears the entire chat.

### Used in repo

_Not yet wired._

---

## Chat Message Deleted

- Path: Twitch -> Moderation -> Chat Message Deleted
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/chat-message-deleted
- Min SB version: v0.1.18
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `message` | string | Text content of the deleted message. |
| `targetMessageId` | string | Unique ID of the deleted message. |

### Caveats

- Chat Client trigger.
- Twitch User shared group identifies the user whose message was deleted.

### Used in repo

_Not yet wired._

---

## Moderator Added

- Path: Twitch -> Moderation -> Moderator Added
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/moderator-added
- Min SB version: v0.2.3
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

_No trigger-specific args. Twitch User shared group identifies the user who was added as moderator._

### Caveats

- EventSub-based trigger. v0.2.3+.

### Used in repo

_Not yet wired._

---

## Moderator Removed

- Path: Twitch -> Moderation -> Moderator Removed
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/moderator-removed
- Min SB version: v0.2.3
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

_No trigger-specific args. Twitch User shared group identifies the user who was demodded._

### Caveats

- EventSub-based trigger. v0.2.3+.
- Same variable structure as Moderator Added — mirrors that trigger for removals.

### Used in repo

_Not yet wired._

---

## Permitted Terms Added

- Path: Twitch -> Moderation -> Permitted Terms Added
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/permitted-terms-added
- Min SB version: v0.2.4
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `createdAt` | datetime | When the term(s) were added. |
| `fromAutomod` | bool | Whether the term originated from AutoMod. |
| `moderatorDisplayName` | string | Display name of the moderator who added the term. |
| `moderatorId` | string | User ID of the moderator. |
| `moderatorUsername` | string | Login name of the moderator. |
| `term.[#]` | string | Individual permitted terms, indexed from `term.[0]`. |
| `termCount` | int | Total number of terms added in this event. |

### Caveats

- EventSub-based trigger. v0.2.4+.
- Same variable structure as Blocked Terms Added but for the AutoMod permitted list.

### Used in repo

_Not yet wired._

---

## Permitted Terms Deleted

- Path: Twitch -> Moderation -> Permitted Terms Deleted
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/permitted-terms-deleted
- Min SB version: v0.2.4
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `createdAt` | datetime | When the term(s) were originally added (not deletion timestamp). |
| `fromAutomod` | bool | Whether the term originally came from AutoMod. |
| `moderatorDisplayName` | string | Display name of the moderator who removed the term. |
| `moderatorId` | string | User ID of the moderator. |
| `moderatorUsername` | string | Login name of the moderator. |
| `term.[#]` | string | Individual deleted terms, indexed from `term.[0]`. |
| `termCount` | int | Total number of terms deleted in this event. |

### Caveats

- EventSub-based trigger. v0.2.4+.
- Same variable structure as Permitted Terms Added — mirrors that trigger for removals.

### Used in repo

_Not yet wired._

---

## Shield Mode Begin

- Path: Twitch -> Moderation -> Shield Mode Begin
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/shield-mode-begin
- Min SB version: v0.1.15
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `startedAt` | DateTime | When Shield Mode was activated. |

### Caveats

- EventSub-based trigger. v0.1.15+.
- No Twitch User or Twitch Chat shared groups.

### Used in repo

_Not yet wired._

---

## Shield Mode End

- Path: Twitch -> Moderation -> Shield Mode End
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/shield-mode-end
- Min SB version: v0.1.15
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

_No trigger-specific args identified in upstream docs._

### Caveats

- EventSub-based trigger. v0.1.15+.
- Counterpart to Shield Mode Begin.
- No Twitch User or Twitch Chat shared groups.

### Used in repo

_Not yet wired._

---

## Shoutout Created

- Path: Twitch -> Moderation -> Shoutout Created
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/shoutout-created
- Min SB version: v0.1.14
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `cooldownEndsAt` | DateTime | When another shoutout can be sent. |
| `shoutoutId` | string | Twitch's internal shoutout identifier. |
| `targetCooldownEndsAt` | DateTime | When the same user can receive another shoutout. |
| `targetUserDisplayName` | string | Display name of the shoutout recipient. |
| `targetUserId` | string | User ID of the recipient. |
| `targetUserLogin` | string | Login name of the recipient. |
| `viewerCount` | string | Active viewer count at the time of the shoutout. |

### Caveats

- Chat Client trigger.
- Twitch User shared group identifies the user who created the shoutout (the mod/broadcaster), not the recipient.
- Use `targetUser*` vars for the recipient's identity.

### Used in repo

_Not yet wired._

---

## Suspicious User Message

- Path: Twitch -> Moderation -> Suspicious User Message
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/suspicious-user-message
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

## Suspicious User Update

- Path: Twitch -> Moderation -> Suspicious User Update
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/suspicious-user-update
- Min SB version: v0.2.4
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `lowTrustStatus` | string | Current treatment mode: `restricted`, `active_monitoring`, `no_treatment`. |
| `moderator.id` | string | User ID of the moderator who changed the status. |
| `moderator.login` | string | Login name of the moderator. |
| `moderator.userName` | string | Display name of the moderator. |

### Caveats

- EventSub-based trigger. v0.2.4+.
- Fires when a moderator changes a suspicious user's restriction or monitoring mode.
- Twitch User shared group identifies the suspicious user whose status was updated.

### Used in repo

_Not yet wired._

---

## Unban Request Approved

- Path: Twitch -> Moderation -> Unban Request Approved
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/unban-request-approved
- Min SB version: v0.2.4
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `createdAt` | datetime | When the request was approved. |
| `message` | string | The moderator's approval message. |
| `moderator.id` | string | User ID of the approving moderator. |
| `moderator.login` | string | Login name of the moderator. |
| `moderator.userName` | string | Display name of the moderator. |

### Caveats

- EventSub-based trigger. v0.2.4+.
- Twitch User shared group identifies the user whose unban request was approved.

### Used in repo

_Not yet wired._

---

## Unban Request Created

- Path: Twitch -> Moderation -> Unban Request Created
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/unban-request-created
- Min SB version: v0.2.4
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `createdAt` | datetime | When the unban request was submitted. |
| `text` | string | The user's unban request message. |

### Caveats

- EventSub-based trigger. v0.2.4+.
- Twitch User shared group identifies the user who submitted the request.

### Used in repo

_Not yet wired._

---

## Unban Request Denied

- Path: Twitch -> Moderation -> Unban Request Denied
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/unban-request-denied
- Min SB version: v0.2.4
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `createdAt` | datetime | When the request was denied. |
| `message` | string | The moderator's denial message. |
| `moderator.id` | string | User ID of the denying moderator. |
| `moderator.login` | string | Login name of the moderator. |
| `moderator.userName` | string | Display name of the moderator. |

### Caveats

- EventSub-based trigger. v0.2.4+.
- Same variable structure as Unban Request Approved — mirrors that trigger for denials.
- Twitch User shared group identifies the user whose request was denied.

### Used in repo

_Not yet wired._

---

## User Banned

- Path: Twitch -> Moderation -> User Banned
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/user-banned
- Min SB version: any
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `createdAt` | DateTime | When the ban was issued. |
| `createdByDisplayName` | string | Display name of the moderator who issued the ban. |
| `createdById` | string | User ID of the moderator. |
| `createdByUsername` | string | Login name of the moderator. |
| `reason` | string | Reason provided for the ban. |
| `user` | string | Display name of the banned user (only if present in chat). |
| `userId` | string | Twitch ID of the banned user. |
| `userName` | string | Login name of the banned user. |

### Caveats

- EventSub-based trigger.
- No Twitch User shared group — banned user identity is via `user*` vars directly.
- `user` (display name) may be empty if the user was not present in chat.

### Used in repo

_Not yet wired._

---

## User Timed Out

- Path: Twitch -> Moderation -> User Timed Out
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/user-timed-out
- Min SB version: any
- Shared groups: General, Twitch Broadcaster
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
- No Twitch User shared group — timed-out user identity is via `user*` vars directly.
- `user` (display name) may be empty if the user was not present in chat.

### Used in repo

_Not yet wired._

---

## User Unbanned

- Path: Twitch -> Moderation -> User Unbanned
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/user-unbanned
- Min SB version: v0.2.4
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

- EventSub-based trigger. v0.2.4+.
- Twitch User shared group identifies the user who was unbanned.

### Used in repo

_Not yet wired._

---

## User Untimed Out

- Path: Twitch -> Moderation -> User Untimed Out
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/user-untimed-out
- Min SB version: v0.2.4
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `createdAt` | datetime | When the timeout was lifted. |
| `createdByDisplayName` | string | Display name of the moderator who removed the timeout. |
| `createdById` | string | User ID of the moderator. |
| `createdByUsername` | string | Login name of the moderator. |

### Caveats

- EventSub-based trigger. v0.2.4+.
- Same variable structure as User Unbanned — mirrors that trigger for timeout removals.
- Twitch User shared group identifies the user whose timeout was removed.

### Used in repo

_Not yet wired._

---

## Vip Added

- Path: Twitch -> Moderation -> Vip Added
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/vip-added
- Min SB version: v0.2.3
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

_No trigger-specific args. Twitch User shared group identifies the user who was given VIP._

### Caveats

- EventSub-based trigger. v0.2.3+.

### Used in repo

_Not yet wired._

---

## Vip Removed

- Path: Twitch -> Moderation -> Vip Removed
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/vip-removed
- Min SB version: v0.2.3
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

_No trigger-specific args. Twitch User shared group identifies the user whose VIP was removed._

### Caveats

- EventSub-based trigger. v0.2.3+.
- Same variable structure as Vip Added — mirrors that trigger for removals.

### Used in repo

_Not yet wired._

---

## Warned User

- Path: Twitch -> Moderation -> Warned User
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/warned-user
- Min SB version: v0.2.4
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `moderator.id` | string | User ID of the moderator who issued the warning. |
| `moderator.login` | string | Login name of the moderator. |
| `moderator.userName` | string | Display name of the moderator. |
| `reason` | string | Reason provided for the warning. |

### Caveats

- EventSub-based trigger. v0.2.4+.
- Twitch User shared group identifies the user who was warned.

### Used in repo

_Not yet wired._

---

## Warning Acknowledged

- Path: Twitch -> Moderation -> Warning Acknowledged
- Upstream: https://docs.streamer.bot/api/triggers/twitch/moderation/warning-acknowledged
- Min SB version: v0.2.4
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

_No trigger-specific args. Shared variable groups apply (see upstream)._

### Caveats

- EventSub-based trigger. v0.2.4+.
- Fires when a warned user acknowledges the warning in chat.
- Twitch User shared group identifies the user who acknowledged.

### Used in repo

_Not yet wired._
