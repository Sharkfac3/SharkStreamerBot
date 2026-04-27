---
id: app-dev
type: role
description: Standalone stream interaction apps, dashboards, overlays, brokers, and TypeScript app tooling.
status: active
owner: app-dev
workflows: change-summary, validation
---

# Role: app-dev

## Purpose

Own standalone apps that run outside Streamer.bot, including overlays, brokers, REST services, and admin dashboards.

## Owns

- TypeScript app work under [Apps/](../../../Apps/).
- Stream-overlay route guidance in [Apps/stream-overlay/AGENTS.md](../../../Apps/stream-overlay/AGENTS.md), with broker protocol details in [Apps/stream-overlay/docs/protocol.md](../../../Apps/stream-overlay/docs/protocol.md) and renderer details in [Apps/stream-overlay/docs/rendering-notes.md](../../../Apps/stream-overlay/docs/rendering-notes.md).
- Info-service and production-manager app contracts in their local app guides.

## When to Activate

Activate for stream interaction apps, dashboards, overlays, brokers, TypeScript build/runtime behavior, REST service changes, or app-side protocol work.

## Do Not Activate For

- Streamer.bot C# scripts unless app protocol/bridge behavior is involved; use `streamerbot-dev`.
- Public copy without app UX implementation; use `brand-steward`.
- Pure operational validation tooling; use `ops`.

## Common Routes

Use [Apps/stream-overlay/AGENTS.md](../../../Apps/stream-overlay/AGENTS.md), [Apps/info-service/AGENTS.md](../../../Apps/info-service/AGENTS.md), and [Apps/production-manager/AGENTS.md](../../../Apps/production-manager/AGENTS.md). Use [Actions/Overlay/AGENTS.md](../../../Actions/Overlay/AGENTS.md) when app work requires Streamer.bot bridge updates.

## Required Workflows

- [coordination](../../workflows/coordination.md) before starting.
- [validation](../../workflows/validation.md) for build/typecheck/test selection.
- [change-summary](../../workflows/change-summary.md) after changed files.
- [sync](../../workflows/sync.md) only when companion C# scripts are edited.

## Chain To

- `streamerbot-dev` for C# publishers, event bridges, and paste targets.
- `brand-steward` for UI/public copy and reward-facing text.
- `lotat-tech` for LotAT protocol/runtime contract changes.
- `ops` for tool/environment validation support.

## Living Context

Use local app guides first. Treat app-dev context notes as supplemental background when local guides do not answer the question.
