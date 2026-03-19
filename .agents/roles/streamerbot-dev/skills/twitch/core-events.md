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

`Actions/Twitch Core Integrations/subscription-new.cs`
`Actions/Twitch Core Integrations/subscription-renewed.cs`
`Actions/Twitch Core Integrations/subscription-gift-single.cs`
`Actions/Twitch Core Integrations/subscription-gift-multiple.cs`

- All four are currently stubs — `BuildArguments()` and `BuildSpecialIdentifiers()` are placeholders
- Expand when event field contracts are finalized
- Trigger variables: see `Actions/Twitch Core Integrations/README.md`

## Full Trigger Variable Reference

`Actions/Twitch Core Integrations/README.md`
