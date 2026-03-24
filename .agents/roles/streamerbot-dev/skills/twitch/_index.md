# Twitch Integrations — Feature Overview

## Scope

Stream lifecycle events and core Twitch behavior: startup resets, bits integrations, follows, subscriptions.

## Scripts

| Script | Path | Trigger |
|---|---|---|
| Stream Start | `Actions/Twitch Core Integrations/stream-start.cs` | Stream → Go Live |
| Follower New | `Actions/Twitch Core Integrations/follower-new.cs` | Follow event |
| Subscription Dispatcher | `Actions/Twitch Core Integrations/subscription-dispatcher.cs` | Subscription event routing |
| Subscription Gift | `Actions/Twitch Core Integrations/subscription-gift.cs` | Gift sub event |
| Subscription Counter Rollover | `Actions/Twitch Core Integrations/subscription-counter-rollover.cs` | Subscription counter maintenance |
| Bits Tier 1–4 | `Actions/Twitch Bits Integrations/bits-tier-*.cs` | Cheer event (by tier) |

## Detailed Docs

- `Actions/Twitch Core Integrations/README.md` — stream-start, follow, sub trigger variables
- `Actions/Twitch Bits Integrations/README.md` — bits tier thresholds and behavior

## Stream-Start Reset

`Actions/Twitch Core Integrations/stream-start.cs` is the **central reset point** for all session state. Resets:
- Squad (Duck, Clone, Pedro, Toothless, offering/LotAT)
- OBS sources (hide dancing sources, cycle rarity sources)
- Timers (disable Duck Call Window, Clone Volley Timer)

**Any new global variable added by any feature must also be reset here and added to `Actions/SHARED-CONSTANTS.md`.**

## Bits Behavior

- Scripts forward cheer messages to Mix It Up overlay commands
- Strip cheer tokens from message text before forwarding
- Calculate dynamic wait time based on word count (avoids overlapping TTS/readouts)
- Shared bits constants in `Actions/SHARED-CONSTANTS.md`

## Stubs

Sub/re-sub/gift-sub scripts and hype train scripts are currently stubs. Expand `BuildArguments()` and `BuildSpecialIdentifiers()` when event field contracts are finalized.

## Behavioral Expectations

- Favor reliability and idempotency
- Fail safely (log useful info, avoid cascading failures)
- For TTS/readout integrations: include queue-safe timing/wait behavior so rapid triggers do not overlap
- Keep `Actions/Twitch Core Integrations/stream-start.cs` early in stream startup order so downstream scripts see clean state

## Sub-Skills

- `core-events.md` — stream-start, follow, sub event details
- `bits.md` — bits tier details
- `channel-points.md` — channel point redeems
- `hype-train.md` — hype train events
