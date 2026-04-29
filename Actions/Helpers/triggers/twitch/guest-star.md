---
id: triggers-twitch-guest-star
type: reference
description: Streamer.bot Twitch Guest Star trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: seeded
upstream: https://docs.streamer.bot/api/triggers/twitch/guest-star
---

# Twitch — Guest Star Triggers

## Guest Update

- Path: Twitch -> Guest Star -> Guest Update
- Upstream: https://docs.streamer.bot/api/triggers/twitch/guest-star/guest-update
- Min SB version: v0.2.3
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `guestUserId` | string | User ID of the guest. |
| `guestUserLogin` | string | Login name of the guest. |
| `guestUserName` | string | Display name of the guest. |
| `hostAudioEnabled` | bool | Audio enabled status (null if guest not yet joined). |
| `hostVideoEnabled` | bool | Video enabled status (null if guest not yet joined). |
| `hostVolume` | number | Volume level (null if guest not yet joined). |
| `moderatorUserId` | string | User ID of the moderator who updated guest settings. |
| `moderatorUserLogin` | string | Login name of the moderator. |
| `moderatorUserName` | string | Display name of the moderator. |
| `sessionId` | string | ID of the current Guest Star session. |
| `slotId` | string | ID of the guest slot (null if guest not yet joined). |
| `state` | string | Guest update state: `invited`, `backstage`, `ready`. |

### Caveats

- EventSub-based trigger.
- `hostAudioEnabled`, `hostVideoEnabled`, `hostVolume`, `slotId` are null until guest joins a slot.

### Used in repo

_Not yet wired._

---

## Session Begin

- Path: Twitch -> Guest Star -> Session Begin
- Upstream: https://docs.streamer.bot/api/triggers/twitch/guest-star/session-begin
- Min SB version: v0.2.3
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `sessionId` | string | ID of the Guest Star session. |
| `startedAt` | datetime | Datetime the session started. |

### Caveats

- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## Session End

- Path: Twitch -> Guest Star -> Session End
- Upstream: https://docs.streamer.bot/api/triggers/twitch/guest-star/session-end
- Min SB version: v0.2.3
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `endedAt` | datetime | Datetime the session ended. |
| `sessionId` | string | ID of the Guest Star session. |
| `startedAt` | datetime | Datetime the session started. |

### Caveats

- EventSub-based trigger.

### Used in repo

_Not yet wired._

---

## Settings Update

- Path: Twitch -> Guest Star -> Settings Update
- Upstream: https://docs.streamer.bot/api/triggers/twitch/guest-star/settings-update
- Min SB version: v0.2.3
- Shared groups: General, Twitch Broadcaster
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `groupLayout` | string | Layout of guests in browser source: `tiled`, `screenshare`, `horizontal`, `vertical`. |
| `isBrowserSourceAudioEnabled` | bool | Whether browser sources should output audio. |
| `isModeratorSendLiveEnabled` | bool | Whether moderators can control guest "live" status. |
| `slotCount` | int | Number of guest slots (1–6). |

### Caveats

- EventSub-based trigger.

### Used in repo

_Not yet wired._
