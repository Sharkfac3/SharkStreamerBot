---
id: triggers-twitch-chat
type: reference
description: Streamer.bot Twitch Chat trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: seeded
upstream: https://docs.streamer.bot/api/triggers/twitch/chat
---

# Twitch — Chat Triggers

## Bits Badge Tier

- Path: Twitch -> Chat -> Bits Badge Tier
- Upstream: https://docs.streamer.bot/api/triggers/twitch/chat/bits-badge-tier
- Min SB version: v0.2.5
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

_No trigger-specific args. Shared variable groups apply (see upstream)._

### Caveats

- EventSub-based trigger. v0.2.5+.
- Fires when a viewer's bits badge tier is upgraded (e.g. 100 bits → 1000 bits badge).

### Used in repo

_Not yet wired._

---

## Bot Whispers

- Path: Twitch -> Chat -> Bot Whispers
- Upstream: https://docs.streamer.bot/api/triggers/twitch/chat/bot-whispers
- Min SB version: v0.1.14
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

_No trigger-specific args. Shared variable groups apply (see upstream)._

### Caveats

- Chat Client trigger.
- Fires when the Streamer.bot bot account receives a whisper.

### Used in repo

_Not yet wired._

---

## Cheer

- Path: Twitch -> Chat -> Cheer
- Upstream: https://docs.streamer.bot/api/triggers/twitch/chat/cheer
- Min SB version: any
- Shared groups: General, Twitch Broadcaster, Twitch Chat, Twitch User
- Coverage: seeded

### Args

_No trigger-specific args beyond shared groups. Key fields available via Twitch Chat shared group: `bits`, `message`, `messageStripped`, `user`, `userId`._

### Caveats

- Supports a Min/Max range filter in SB trigger config to route tiers (e.g. 1–99, 100–999, 1000+).
- `bits` and cheer message available via Twitch Chat shared group variables.

### Used in repo

- [Actions/Twitch Bits Integrations/bits-tier-1.cs](../../Twitch%20Bits%20Integrations/bits-tier-1.cs) — Tier 1 (1–99 bits).
- [Actions/Twitch Bits Integrations/bits-tier-2.cs](../../Twitch%20Bits%20Integrations/bits-tier-2.cs) — Tier 2 (100–999 bits).
- [Actions/Twitch Bits Integrations/bits-tier-3.cs](../../Twitch%20Bits%20Integrations/bits-tier-3.cs) — Tier 3 (1000–9999 bits).
- [Actions/Twitch Bits Integrations/bits-tier-4.cs](../../Twitch%20Bits%20Integrations/bits-tier-4.cs) — Tier 4 (10000+ bits).

---

## First Words

- Path: Twitch -> Chat -> First Words
- Upstream: https://docs.streamer.bot/api/triggers/twitch/chat/first-words
- Min SB version: any
- Shared groups: General, Twitch Broadcaster, Twitch Chat, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `firstMessage` | bool | Whether this is the user's first-ever message in the channel. |

### Caveats

- Reset time is configurable at Platforms > Twitch > Settings (default: 12 hours).
- Recommended: pair a `Settings -> Reset First Words` subaction with the Stream Online trigger to auto-reset each stream.

### Used in repo

- [Actions/Intros/first-chat-intro.cs](../../Intros/first-chat-intro.cs)

---

## Message

- Path: Twitch -> Chat -> Message
- Upstream: https://docs.streamer.bot/api/triggers/twitch/chat/message
- Min SB version: v0.0.50
- Shared groups: General, Twitch Broadcaster, Twitch Chat, Twitch User
- Coverage: seeded

### Args

_No trigger-specific args. All message data (message, user, userId, etc.) is in the Twitch Chat shared group._

### Caveats

- Fires for every chat message. Use command triggers for filtered matching.

### Used in repo

_Not yet wired._

---

## Watch Streak

- Path: Twitch -> Chat -> Watch Streak
- Upstream: https://docs.streamer.bot/api/triggers/twitch/chat/watch-streak
- Min SB version: v0.2.4
- Shared groups: General, Twitch Broadcaster, Twitch Chat, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `copoReward` | int | Channel points awarded for the watch streak. |
| `systemMessage` | string | Twitch's internal watch streak message. |
| `watchStreak` | int | Current watch streak count. |
| `watchStreakId` | string | ID of this watch streak event. |

### Caveats

- Requires Chat Client. Available since v0.2.4.

### Used in repo

- [Actions/Twitch Core Integrations/watch-streak.cs](../../Twitch%20Core%20Integrations/watch-streak.cs)

---

## Whispers

- Path: Twitch -> Chat -> Whispers
- Upstream: https://docs.streamer.bot/api/triggers/twitch/chat/whispers
- Min SB version: v0.0.39
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

_No trigger-specific args. Shared variable groups apply (see upstream)._

### Caveats

- EventSub-based trigger. v0.0.39+.
- Sender must have a verified phone number.
- Rate limits: 40 unique recipients per day; 500 chars for new recipients, 10,000 for established conversations.
- Fires when the broadcaster's account receives a whisper (not the bot account — see Bot Whispers for that).

### Used in repo

_Not yet wired._
