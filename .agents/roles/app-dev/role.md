# Role: app-dev

## What This Role Does

Builds and maintains standalone applications that support stream interactions — dashboards, overlays, chat integrations, and any other tooling that runs as an independent app rather than as a Streamer.bot script.

## Why This Role Matters

Apps extend the stream's capabilities beyond what Streamer.bot scripts can do alone. Overlays capture moments, dashboards track engagement, and future apps may serve the product side of the business — customer-facing tools, product configurators, or community platforms. This role builds the infrastructure that scales the content creator's capabilities into a full business operation.

## Activate When

- Building a new stream interaction application
- Maintaining or extending an existing app under `Apps/`
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

Active. The stream overlay ecosystem (`Apps/stream-overlay/`) is built and operational. The `Apps/` domain is established. Add new skills as new apps are added.

## Active Apps

- `Apps/stream-overlay/` — Phaser-based stream overlay (broker + overlay packages)
- `Apps/info-service/` — File-backed JSON REST service for per-viewer data (C2+); see `context/info-service.md`
- `Apps/production-manager/` — React + Vite admin app for managing info-service collections (C6+)

## Out of Scope

- Streamer.bot runtime scripts (those belong in `streamerbot-dev`)
- Python utilities and validators (those belong in `ops`)
