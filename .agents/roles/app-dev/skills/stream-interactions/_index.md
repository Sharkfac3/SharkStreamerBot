# Stream Interactions — Overview

## What This Is

The stream overlay ecosystem is a multi-package TypeScript monorepo (`Apps/stream-overlay/`) that extends the stream's capabilities beyond what Streamer.bot scripts can do alone. The primary deliverable is a Phaser-based OBS browser source that renders live stream events as visual overlays — but the architecture is designed to support future clients (web dashboards, AI agents, mobile) through the same message broker.

---

## Architecture

```
┌──────────────────────────────────────────────────────────────────────────────┐
│                          STREAM OVERLAY ECOSYSTEM                            │
└──────────────────────────────────────────────────────────────────────────────┘

  ┌─────────────────────────────────────────┐
  │           Streamer.bot (Actions/)        │
  │  C# scripts — business logic, triggers   │
  │  Sends events to broker via WebSocket    │
  └────────────────────┬────────────────────┘
                       │ WebSocket (push)
                       ▼
          ┌────────────────────────┐
          │         BROKER         │
          │  @stream-overlay/broker│
          │  Node.js WebSocket hub │
          │  pub/sub, topic routing│
          │  NO business logic     │
          └─────────┬──────────────┘
          ┌─────────┤──────────────┬─────────────────┐
          │         │              │                  │
          ▼         ▼              ▼                  ▼
   ┌──────────┐ ┌──────────┐ ┌──────────┐     ┌──────────┐
   │ OVERLAY  │ │ Future   │ │ Future   │     │ Future   │
   │ (Phaser) │ │ Web App  │ │ AI Agent │     │ Mobile   │
   │ OBS src  │ │dashboard │ │          │     │          │
   └──────────┘ └──────────┘ └──────────┘     └──────────┘
```

**Hub-and-spoke:** The broker is the only component all clients talk to. No client talks directly to another client. Streamer.bot is the sole producer of business events; all other components are consumers (or future producers for things like viewer commands).

---

## Packages

| Package | Name | Role |
|---|---|---|
| `packages/shared` | `@stream-overlay/shared` | Shared TypeScript types, message protocol, topic names. The source of truth for the data contract. |
| `packages/broker` | `@stream-overlay/broker` | Node.js WebSocket broker. Accepts connections, routes messages by topic. No business logic. |
| `packages/overlay` | `@stream-overlay/overlay` | Phaser 3 browser source. Connects to broker, renders events. Single OBS browser source. |

---

## Services

| Service | Built In | Owner | Notes |
|---|---|---|---|
| Broker | `packages/broker/` | `app-dev` | Hub for all message routing — complete |
| Overlay (Phaser) | `packages/overlay/` | `app-dev` | OBS browser source — complete |
| Asset system | `packages/overlay/src/systems/` | `app-dev` | AssetManager, AnimationSystem, AudioManager — complete |
| Streamer.bot broker scripts | `Actions/Overlay/` | `streamerbot-dev` | broker-connect, broker-publish, test-overlay, broker-disconnect — complete |
| LotAT visual layer | `packages/overlay/src/lotat/` | `app-dev` | 11 components + lotat-renderer + OverlayScene wiring — complete |
| Squad visual layer | `packages/overlay/src/squad/` | `app-dev` | 4 game renderers + squad-renderer + per-game overlay-publish.cs — complete |
| Future: Web dashboard | TBD | `app-dev` | Operator-facing view of stream state |
| Future: AI agent client | TBD | `app-dev` | Connects to broker, reacts to events autonomously |

---

## Role Boundaries

| Concern | Owner | Notes |
|---|---|---|
| Message protocol definition | `app-dev` | Defined in `@stream-overlay/shared` (`packages/shared/src/protocol.ts`, `topics.ts`) |
| Broker implementation | `app-dev` | `packages/broker/` — pure routing, no business logic |
| Overlay rendering | `app-dev` | `packages/overlay/` — Phaser scenes, animations |
| Streamer.bot event push | `streamerbot-dev` | C# script in `Actions/` that connects to broker |
| Business logic (what triggers what) | `streamerbot-dev` | Stays in Streamer.bot scripts, never in broker/overlay |
| OBS scene configuration | Operator | OBS browser source setup, layer ordering |
| Brand/character identity in overlays | `brand-steward` | Review any new visual character or text before shipping |

---

## Skill Docs

- [`broker.md`](broker.md) — What the broker is, how it works, what it explicitly does NOT do
- [`overlay.md`](overlay.md) — What the Phaser overlay is, OBS integration model, rendering principles
