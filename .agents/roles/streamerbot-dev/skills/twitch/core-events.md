# Twitch Core Events

## Stream Start

`Actions/Twitch Core Integrations/stream-start.cs`

- Runs at stream go-live
- Resets all session state (Squad locks, OBS sources, timers)
- Must run before any feature script that depends on clean global state
- Any new feature global variable must be added to this reset

## Follow

`Actions/Twitch Core Integrations/follower-new.cs`

- Triggers on new follow event
- Forwards to Mix It Up for overlay handling
- Trigger variables: see `Actions/Twitch Core Integrations/README.md`

## Subscriptions

Each subscription event has its own dedicated script. All are currently stubs —
`BuildArguments()` and `BuildSpecialIdentifiers()` return empty until event field contracts are finalized.

| Script | Event |
|---|---|
| `Actions/Twitch Core Integrations/subscription-new.cs` | First-time sub (viewer paid directly) |
| `Actions/Twitch Core Integrations/subscription-renewed.cs` | Resub (viewer renewing their existing sub) |
| `Actions/Twitch Core Integrations/subscription-prime-paid-upgrade.cs` | Prime sub converted to paid tier (SB v0.2.5+) |
| `Actions/Twitch Core Integrations/subscription-gift-paid-upgrade.cs` | Gifted sub converted to paid by recipient (SB v0.2.5+) |
| `Actions/Twitch Core Integrations/subscription-pay-it-forward.cs` | Gifted sub recipient gifts to someone else (SB v0.2.5+) |
| `Actions/Twitch Core Integrations/subscription-gift.cs` | Gift subs — smart router handling solo gifts, gift bombs, and bomb deduplication |
| `Actions/Twitch Core Integrations/subscription-counter-rollover.cs` | Sub counter milestone — counter event, no user context |

- Each script has its own `MIXITUP_COMMAND_ID` constant — replace before production use
- Trigger variables for each event: see `Actions/Twitch Core Integrations/README.md`

## Full Trigger Variable Reference

`Actions/Twitch Core Integrations/README.md`
