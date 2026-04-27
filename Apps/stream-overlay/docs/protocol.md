---
id: apps-stream-overlay-protocol
type: domain-reference
description: Broker protocol, topic routing, message envelope, and package runtime notes for Apps/stream-overlay.
status: active
owner: app-dev
---

# Stream Overlay Protocol Reference

## Source of Truth

The authoritative protocol lives in [../packages/shared/src/protocol.ts](../packages/shared/src/protocol.ts) and [../packages/shared/src/topics.ts](../packages/shared/src/topics.ts). If this reference conflicts with those files, update this reference; do not change types to match stale docs.

For shared LotAT presentation facts such as `lotat.*` topics, broker envelope expectations, and payload handoff boundaries, see [.agents/_shared/lotat-contract.md](../../../.agents/_shared/lotat-contract.md). Keep this app reference focused on overlay implementation responsibilities.

Protocol evolution remains coordinated: removing fields/topics or changing field types is breaking and requires updates across shared types, overlay renderer, broker tests, and C# publishers.

## Packages

| Package | Role |
|---|---|
| [../packages/shared/](../packages/shared/) | Shared TypeScript types, broker message envelope, topic constants, client names, broker URL/port constants. This is the source of truth for protocol shape. |
| [../packages/broker/](../packages/broker/) | Node WebSocket hub. Accepts client hello frames, tracks subscriptions, routes messages by topic, exposes a health endpoint, and responds to system ping. |
| [../packages/overlay/](../packages/overlay/) | Phaser browser app for OBS. Connects to the broker, subscribes to rendering topics, and draws visuals/audio on a transparent 1920 by 1080 canvas. |

## Broker Rules

- Runtime is Node ESM compiled from TypeScript.
- Development command is `pnpm dev:broker` from [Apps/stream-overlay/](../).
- Default WebSocket port is 8765; default broker URL is ws://localhost:8765.
- Client names are stable strings exported from [../packages/shared/src/topics.ts](../packages/shared/src/topics.ts).
- The broker accepts a raw `client.hello` frame, returns `client.welcome`, then routes broker-message envelopes.
- Wildcard matching is single-level. For example, `squad.*` covers `squad.duck.start`; deeper nested topics need explicit handling if introduced.
- On malformed messages, use silent drop plus log. Do not crash or add application-level error semantics without a protocol revision.

## Overlay Subscription Rules

- Development OBS/browser URL is http://localhost:5173.
- Production OBS loads the generated overlay HTML from the overlay package's generated dist directory after `pnpm build`.
- The overlay subscribes to `overlay.*`, `overlay.audio.*`, `lotat.*`, `squad.*`, `stream.*`, and `system.*`.
- The overlay publishes only `system.ping` in v1.

## Topic Changes

When adding or changing topics:

1. Define or update payload types in [../packages/shared/src/protocol.ts](../packages/shared/src/protocol.ts).
2. Add topic constants in [../packages/shared/src/topics.ts](../packages/shared/src/topics.ts), including wildcard constants when needed.
3. Update overlay subscriptions in [../packages/overlay/src/broker-client.ts](../packages/overlay/src/broker-client.ts) if existing wildcards do not cover the new topic.
4. Update broker/overlay tests or test-session scripts when behavior changes.
5. Update C# publisher templates and [Actions/SHARED-CONSTANTS.md](../../../Actions/SHARED-CONSTANTS.md) when Streamer.bot publishes or depends on the topic.
