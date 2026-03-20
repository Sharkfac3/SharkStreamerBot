---
name: app-dev
description: Stream interaction apps, dashboards, overlays, and future standalone tooling. Load when building or planning an app that runs outside Streamer.bot.
---

# app-dev

Full context: `.agents/roles/app-dev/role.md`

## Always Load

`.agents/roles/app-dev/skills/core.md`

## Then Navigate

| Task | Load |
|---|---|
| Stream-facing app concepts and architecture | `.agents/roles/app-dev/skills/stream-interactions/_index.md` |

## Purpose

This role exists so Pi can route future standalone app work into a dedicated lane instead of overloading `streamerbot-dev` or `ops`.

Use it for apps that:
- run outside Streamer.bot
- provide richer UI, dashboards, overlays, or persistent interaction state
- integrate with Streamer.bot, Mix It Up, Twitch, or OBS as external systems

Even while the app domain is still a placeholder, load this role first when the task is clearly about a standalone app rather than an `Actions/` script or local utility.
