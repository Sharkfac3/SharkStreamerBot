# Role: app-dev

## What This Role Does

Builds and maintains standalone applications that support stream interactions — dashboards, overlays, chat integrations, and any other tooling that runs as an independent app rather than as a Streamer.bot script.

## Activate When

- Building a new stream interaction application
- Maintaining or extending an existing app under `Apps/` (when that domain is created)
- Integrating external APIs or services into a standalone app
- Building UI for stream overlays that require their own runtime

## Do Not Activate When

- Task is a Streamer.bot C# script → use `streamerbot-dev`
- Task is a local Python utility or validator → use `ops`
- Task is narrative or brand content → use appropriate creative role

## Skill Load Order

1. `skills/core.md` — always load first
2. `skills/stream-interactions/_index.md` — when working on stream-facing features

## Status

This role is a placeholder. The `Apps/` domain and specific app architecture are defined as the first apps are built. Update `skills/core.md` and `skills/stream-interactions/_index.md` as decisions are made.

## Out of Scope

- Streamer.bot runtime scripts (those belong in `streamerbot-dev`)
- Python utilities and validators (those belong in `ops`)
