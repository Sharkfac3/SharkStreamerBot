---
id: apps-stream-overlay
type: domain-route
description: Phaser stream overlay, WebSocket broker, shared protocol, asset system, and visual renderers.
owner: app-dev
secondaryOwners:
  - streamerbot-dev
  - lotat-tech
workflows:
  - change-summary
  - validation
status: active
path: Apps/stream-overlay/
---

# Apps/stream-overlay — Agent Route

## Purpose

This folder owns the TypeScript stream overlay ecosystem: a shared message protocol package, a Node WebSocket broker, and a Phaser browser overlay used as an OBS source. Streamer.bot actions publish events to the broker; the overlay subscribes and renders visual/audio output.

```text
Streamer.bot Actions -> broker on local WebSocket port 8765 -> Phaser overlay in OBS
```

The broker routes messages only. Streamer.bot owns stream/game business logic. The overlay is a dumb renderer that displays what broker messages request.

## When to Activate

Use this guide for changes under [Apps/stream-overlay/](./), including:

- Shared message types and topic constants in [packages/shared/src/protocol.ts](packages/shared/src/protocol.ts) and [packages/shared/src/topics.ts](packages/shared/src/topics.ts).
- Broker routing, client lifecycle, health checks, and test clients in [packages/broker/src/](packages/broker/src/).
- Phaser overlay rendering in [packages/overlay/src/](packages/overlay/src/).
- Asset handling, animation presets, audio playback, LotAT UI, Squad UI, and OBS browser-source behavior.

Also activate when app-side changes require matching Streamer.bot publisher changes under [Actions/Overlay/](../../Actions/Overlay/), [Actions/Squad/](../../Actions/Squad/), or [Actions/LotAT/](../../Actions/LotAT/).

## Primary Owner

`app-dev` owns this folder: TypeScript app architecture, broker behavior, shared protocol types, Phaser overlay rendering, build/test commands, and OBS browser-source setup guidance.

## Secondary Owners / Chain To

- `streamerbot-dev` for C# publishers, WebSocket client setup, Streamer.bot paste targets, and global constants in [Actions/SHARED-CONSTANTS.md](../../Actions/SHARED-CONSTANTS.md).
- `lotat-tech` for LotAT runtime stage ordering and story-engine semantics that affect `lotat.*` payloads.
- `brand-steward` for new public-facing UI text, new character/brand visuals, or changes to the stream's public tone.
- `ops` for validation/handoff formatting and local tooling failures.

Related local action guides:

- [Actions/Overlay/AGENTS.md](../../Actions/Overlay/AGENTS.md) — Streamer.bot broker bridge and paste targets.
- [Actions/Squad/AGENTS.md](../../Actions/Squad/AGENTS.md) — Squad game publishers and runtime boundaries.
- [Actions/LotAT/AGENTS.md](../../Actions/LotAT/AGENTS.md) — LotAT engine publishers and runtime boundaries.

## Required Reading

Before making changes, read:

- [README.md](README.md) for quick start, OBS setup, day-to-day operations, and developer workflow.
- [package.json](package.json) for workspace scripts.
- [docs/protocol.md](docs/protocol.md) for broker protocol, package roles, routing rules, and topic-change workflow.
- [docs/asset-system.md](docs/asset-system.md) for public asset paths, asset IDs, lifecycle commands, audio behavior, and depth conventions.
- [docs/rendering-notes.md](docs/rendering-notes.md) for overlay renderer rules, LotAT/Squad renderer notes, OBS caveats, and gotchas.
- [Docs/Architecture/lotat-contract.md](../../Docs/Architecture/lotat-contract.md) before changing LotAT presentation topics, payload expectations, or renderer behavior.

## Local Workflow / Build and Test

```bash
cd Apps/stream-overlay
pnpm install
pnpm dev:broker
pnpm dev:overlay
pnpm typecheck
pnpm build
```

Open the overlay in a browser at http://localhost:5173?debug=true and confirm the connection HUD turns green after the broker is running. Use the broker health endpoint at http://localhost:8765/health to inspect connected clients and subscriptions.

Useful end-to-end checks:

```bash
cd Apps/stream-overlay/packages/broker
npx tsx src/test-client.ts
npx tsx src/lotat-test-session.ts
npx tsx src/squad-test-session.ts
```

For agent-doc changes, run the validator from the repository root:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/104-validator.failures.txt
```

## Boundaries / Out of Scope

- Do not put stream/game business logic in the broker or overlay. Business rules stay in Streamer.bot scripts under [Actions/](../../Actions/).
- Do not make the broker publicly exposed or authenticated without a separate security design. It is a local-only service for the stream PC.
- Do not treat the broker as persistent storage. Messages are fire-and-forget; no queue, replay, or stream state authority.
- Do not deploy C# reference publisher templates as standalone actions. They are copy/paste sources for target Streamer.bot actions.
- Info-service and production-manager are separate app routes and are intentionally out of scope for this guide.

## Handoff Notes

When app changes affect Streamer.bot publishers:

- Update [Actions/Overlay/AGENTS.md](../../Actions/Overlay/AGENTS.md) expectations if broker connection or publish helper behavior changes.
- Update [Actions/Squad/AGENTS.md](../../Actions/Squad/AGENTS.md) or [Actions/LotAT/AGENTS.md](../../Actions/LotAT/AGENTS.md) if topic payloads or event order change for those features.
- Mirror topic names in [Actions/SHARED-CONSTANTS.md](../../Actions/SHARED-CONSTANTS.md) if [packages/shared/src/topics.ts](packages/shared/src/topics.ts) changes.
- Include build/typecheck output and any required OBS/Streamer.bot setup changes in the final handoff.
