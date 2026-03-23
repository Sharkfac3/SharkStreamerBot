# Core Skills — app-dev

## Business Context

This project supports a live R&D stream that is also a business — developing off-road racing products live while building community. Apps you build serve the content pipeline (overlays, dashboards, engagement tools) and may eventually serve the product side (customer-facing tools, community platforms). Build with extensibility in mind. Read `.agents/_shared/project.md` for the full business context and content pipeline.

## Status

Placeholder — populated as the first stream interaction apps are defined and built.

## What Goes Here

When the first app is planned, document here:
- Technology stack decisions
- Shared patterns across apps (auth, data flow, event handling)
- Integration points with Streamer.bot and Mix It Up
- Deployment/run model (local? hosted? Electron? web?)

## Known Integration Points

Apps in this project will likely need to integrate with:
- **Streamer.bot** — via its HTTP/WebSocket API for reading/triggering actions
- **Mix It Up** — via REST API (see `_shared/mixitup-api.md`)
- **Twitch** — via EventSub or PubSub for stream events
- **OBS** — via obs-websocket if the app needs scene/source control

## Repo Placement

New apps will live under `Apps/<AppName>/` at the repo root. This domain does not exist yet — create it when the first app is started and update this skill accordingly.
