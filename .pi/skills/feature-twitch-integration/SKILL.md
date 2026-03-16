---
name: feature-twitch-integration
description: Stream lifecycle, stream-start reset, Bits cheer forwarding, and core Twitch integration glue. Load when working on scripts under `Actions/Twitch Core Integrations/` or `Actions/Twitch Bits Integrations/`.
---

# Feature: Twitch Integrations

## Scope

These scripts handle stream lifecycle events and core Twitch behavior across startup resets and bits integrations.

## Scripts

| Script | Path |
|---|---|
| Stream Start | `Actions/Twitch Core Integrations/stream-start.cs` |
| Follower New | `Actions/Twitch Core Integrations/follower-new.cs` |
| Subscription New | `Actions/Twitch Core Integrations/subscription-new.cs` |
| Subscription Renewed | `Actions/Twitch Core Integrations/subscription-renewed.cs` |
| Subscription Gift Single | `Actions/Twitch Core Integrations/subscription-gift-single.cs` |
| Subscription Gift Multiple | `Actions/Twitch Core Integrations/subscription-gift-multiple.cs` |
| Bits Tier 1 | `Actions/Twitch Bits Integrations/bits-tier-1.cs` |
| Bits Tier 2 | `Actions/Twitch Bits Integrations/bits-tier-2.cs` |
| Bits Tier 3 | `Actions/Twitch Bits Integrations/bits-tier-3.cs` |
| Bits Tier 4 | `Actions/Twitch Bits Integrations/bits-tier-4.cs` |

## Detailed Docs

- `Actions/Twitch Core Integrations/README.md` (stream-start plus follow/subscription event docs)
- `Actions/Twitch Bits Integrations/README.md` (bits tier docs)

**Read the relevant README before making changes.**

## Stream-Start Reset

`stream-start.cs` is the central reset point for all session state. It resets globals for:

- Squad (Duck, Clone, Pedro, Toothless, offering/LotAT)
- OBS sources (hide dancing sources, cycle rarity sources)
- Timers (disable Duck Call Window, Clone Volley Timer)

**Any new global variable added by any feature must also be reset here** and added to `Actions/SHARED-CONSTANTS.md`.

## Bits Behavior

- Bits scripts forward cheer messages to Mix It Up overlay commands.
- They strip cheer tokens from message text before forwarding.
- They calculate a dynamic wait time based on word count to avoid overlapping TTS/readouts.

Shared Bits constants are in `Actions/SHARED-CONSTANTS.md`.

## Behavioral Expectations

- Favor reliability and idempotency.
- Fail safely (log useful info, avoid cascading failures).
- Preserve existing operator workflow unless asked to change it.
- For external TTS/readout integrations, include queue-safe timing/wait behavior so rapid triggers do not overlap/cut off active readouts.
- Keep `stream-start.cs` early in stream startup order so downstream scripts see clean state.
