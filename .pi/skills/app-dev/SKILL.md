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

Use this role for standalone apps under `Apps/` that:
- run outside Streamer.bot
- provide richer UI, dashboards, overlays, or persistent interaction state
- integrate with Streamer.bot, Mix It Up, Twitch, or OBS as external systems

Active apps: `Apps/stream-overlay/` (Phaser overlay + broker), `Apps/info-service/` (per-viewer JSON REST service), `Apps/production-manager/` (React admin UI).
