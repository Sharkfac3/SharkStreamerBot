---
id: triggers-twitch-pyramid
type: reference
description: Streamer.bot Twitch Pyramid trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: seeded
upstream: https://docs.streamer.bot/api/triggers/twitch/pyramid
---

# Twitch — Pyramid Triggers

## Broken

- Path: Twitch -> Pyramid -> Broken
- Upstream: https://docs.streamer.bot/api/triggers/twitch/pyramid/broken
- Min SB version: any
- Shared groups: General, Twitch Broadcaster, Twitch Chat, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `pyramidEmote` | string | Emote used in the pyramid. |
| `pyramidOwnerDisplayName` | string | Display name of the user who started the pyramid. |
| `pyramidOwnerId` | string | User ID of the user who started the pyramid. |
| `pyramidOwnerUsername` | string | Login name of the user who started the pyramid. |
| `pyramidWidth` | number | Width of the pyramid at its peak (emotes in widest row). |
| `totalPyramidsBroken` | number | Running count of pyramids broken in the channel. |

### Caveats

- Twitch User shared group identifies the user who broke the pyramid (sent the interrupting message), not the pyramid builder.
- `pyramidOwner*` identifies the user who started the pyramid.

### Used in repo

_Not yet wired._

---

## Success

- Path: Twitch -> Pyramid -> Success
- Upstream: https://docs.streamer.bot/api/triggers/twitch/pyramid/success
- Min SB version: any
- Shared groups: General, Twitch Broadcaster, Twitch Chat, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `pyramidEmote` | string | Emote used in the pyramid. |
| `pyramidWidth` | number | Width of the completed pyramid (emotes in widest row). |
| `totalPyramids` | number | Running count of successful pyramids in the channel. |

### Caveats

- No `pyramidOwner*` vars — Twitch User shared group identifies the pyramid builder.
- Fires only when the full pyramid sequence completes without interruption.

### Used in repo

_Not yet wired._
