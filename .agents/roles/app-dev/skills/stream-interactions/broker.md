# Broker

## What It Is

The broker (`packages/broker/`) is a Node.js WebSocket server that acts as the message hub for the entire stream overlay ecosystem. It is the single connection point for all clients — Streamer.bot, the Phaser overlay, and any future clients (web dashboards, AI agents, etc.).

The broker's only job is **routing**. It receives a message on one connection and delivers it to all connections that have subscribed to that message's topic. That's it.

---

## Why It Exists

Without a broker, every client would need to know about every other client. Streamer.bot would need to know the overlay's address. Adding a new client (say, a web dashboard) would require changes to Streamer.bot scripts. This is fragile.

The broker decouples producers from consumers:
- Streamer.bot publishes an event. It doesn't care who's listening.
- The overlay subscribes to the events it needs. It doesn't care where they come from.
- A new client can connect without touching any existing code.

---

## Architecture Position

```
Streamer.bot ──publish──▶ BROKER ──deliver──▶ Overlay
                            │
                            └──deliver──▶ Future Web App
                            └──deliver──▶ Future AI Agent
```

The broker is a **dumb pipe**. Intelligence lives at the edges (Streamer.bot decides what to send; the overlay decides how to render it).

---

## Runtime

- **Language**: Node.js ESM (TypeScript compiled via `tsc`, run with `node dist/index.js`)
- **Dev mode**: `tsx watch src/index.ts` — hot reload, no compile step
- **Port**: `8765` — defined as `BROKER_PORT` in `@stream-overlay/shared`; override with `BROKER_PORT` env var
- **URL**: `ws://localhost:8765` — defined as `BROKER_URL` in `@stream-overlay/shared`

---

## How to Run

### Development (hot reload)

```sh
# From Apps/stream-overlay/
pnpm dev:broker

# Or directly in packages/broker/
pnpm dev
```

Starts with `tsx watch` — restarts automatically on file changes. Logs go to stdout.

### Production

```sh
# From Apps/stream-overlay/
pnpm --filter broker build   # compiles TypeScript to dist/
pnpm --filter broker start   # node dist/index.js
```

Start the broker **before** OBS and the overlay. The overlay will reconnect automatically if the broker starts after it, but the cleanest flow is: broker → OBS → overlay.

### Environment Variables

| Variable | Default | Description |
|---|---|---|
| `BROKER_PORT` | `8765` | WebSocket port to listen on |
| `LOG_LEVEL` | `standard` | `minimal`, `standard`, or `verbose` |

Example:
```sh
LOG_LEVEL=verbose pnpm dev
```

---

## How to Debug

### Health Check

Open in a browser or `curl`:

```
http://localhost:8765/health
```

Returns:
```json
{
  "status": "ok",
  "uptime": 42,
  "clients": [
    { "name": "streamerbot", "subscriptions": [] },
    { "name": "overlay", "subscriptions": ["overlay.*", "overlay.audio.*", "lotat.*", "squad.*", "stream.*", "system.*"] }
  ]
}
```

This is the fastest way to confirm what's connected and what each client is subscribed to.

### Test Client

```sh
# From Apps/stream-overlay/packages/broker/
pnpm test-client
```

Connects as `"test-client"`, subscribes to all topics, and prints every message it receives. Also accepts typed input to publish test messages:

```
> overlay.spawn {"assetId":"test","src":"test.png","position":{"x":100,"y":100}}
> lotat.session.start {"sessionId":"abc","storyId":"story-01","title":"Test"}
> system.ping {}
```

Format: `<topic> <json-payload>` then Enter.

### Verbose Logging

```sh
LOG_LEVEL=verbose pnpm dev
```

Logs every routed message with topic, sender, and recipient count.

---

## Module Breakdown

```
packages/broker/src/
  index.ts           — entry point; creates registry and server, calls listen()
  server.ts          — HTTP+WebSocket server; connection handling; handshake; ping/pong
  client-registry.ts — Map<clientId, ConnectedClient>; add/remove/query connected clients
  message-router.ts  — delivers a BrokerMessage to all subscribed clients
  topic-matcher.ts   — wildcard topic matching logic (e.g. "lotat.*" matches "lotat.session.start")
  logger.ts          — structured log output with minimal/standard/verbose levels
  config.ts          — port and log level from env vars, with defaults from @stream-overlay/shared
  test-client.ts     — dev/debug CLI tool; not part of production build
```

**Why each module exists:**
- `server.ts` is separated from `index.ts` so the server can be tested or restarted without touching the entry point.
- `client-registry.ts` is a single `Map` wrapper because the registry is the only shared mutable state in the broker. Keeping it isolated prevents logic from leaking into routing or connection handling.
- `topic-matcher.ts` is extracted because wildcard matching has its own rules and is the most likely thing to need a unit test.
- `logger.ts` is extracted so verbosity can be controlled without console.log calls scattered everywhere.

---

## Responsibilities

- Accept WebSocket connections from any client
- Manage client registration (connect, disconnect, reconnect)
- Accept topic subscription requests from clients
- Route incoming messages to all subscribers of that topic
- Handle disconnections gracefully without crashing other clients
- Log connection events and routing activity for debugging
- Respond to `system.ping` with `system.pong`
- Expose `/health` HTTP endpoint for at-a-glance debugging

---

## What It Does NOT Do

This is as important as what it does:

- ❌ **No business logic** — it does not decide what events mean or what to do with them
- ❌ **No state management** — it does not track stream state (squad, commanders, chaos meter)
- ❌ **No persistence** — messages are fire-and-forget; there is no queue or replay
- ❌ **No authentication** — it is a local-only service on the stream PC; no public exposure
- ❌ **No Twitch/Mix It Up integration** — those integrations belong in Streamer.bot (`Actions/`)
- ❌ **No transformation** — messages pass through as-is; format is defined in `@stream-overlay/shared`

If you find yourself adding business logic to the broker, stop. That logic belongs in a Streamer.bot script.

---

## Message Protocol

All messages follow the protocol defined in `@stream-overlay/shared`. See [`protocol.md`](protocol.md) for the full specification.

The broker does **not** validate message payloads — it trusts the sender. Shape enforcement is a TypeScript compile-time concern, not a broker runtime concern.

On malformed or unroutable messages: **silent drop + log**. The broker never sends error responses.

---

## Topic Taxonomy

```
system.*          — broker infrastructure (broker generates these)
overlay.*         — generic canvas asset commands
overlay.audio.*   — sound playback
lotat.*           — LotAT session lifecycle events
squad.*           — squad mini-game events (all games)
squad.duck.*      — Duck mini-game
squad.clone.*     — Clone mini-game
squad.pedro.*     — Pedro mini-game
squad.toothless.* — Toothless mini-game
stream.*          — raw Twitch platform events
```

All topic strings are constants in `TOPICS` (exported from `@stream-overlay/shared`). Never hardcode topic strings in broker code.

---

## Client Lifecycle

```
1. Client opens WebSocket connection to broker (ws://localhost:8765)
2. Client sends ClientHello immediately:
      { type: "client.hello", name: "streamerbot", subscriptions: ["lotat.*", "system.*"] }
3. Broker sends ClientWelcome back:
      { type: "client.welcome", clientId: "<uuid>", connectedClients: ["overlay"] }
4. Broker publishes system.client.connected to all OTHER subscribers (not the new client)
5. Client begins publishing BrokerMessages and/or receiving deliveries
6. Client disconnects (clean close or unexpected drop)
7. Broker publishes system.client.disconnected to all remaining subscribers
8. Broker removes client from all subscription lists
```

**Reconnect behavior:** Clients are responsible for reconnecting after a dropped connection. On reconnect, the full handshake repeats from step 1. The broker has no persistent memory of prior connections.

---

## File Location

```
Apps/stream-overlay/packages/broker/
  package.json        # @stream-overlay/broker; scripts: dev, start, build, typecheck, test-client
  tsconfig.json       # NodeNext module resolution
  src/
    index.ts          # entry point
    server.ts         # WebSocket + HTTP server setup
    client-registry.ts
    message-router.ts
    topic-matcher.ts
    logger.ts
    config.ts
    test-client.ts    # dev/debug tool only
```

---

## Prerequisites

Node.js must be available in PATH for `pnpm run` scripts to work. pnpm's standalone executable does not expose its bundled Node.js to scripts. Install Node.js from [nodejs.org](https://nodejs.org) (LTS) and verify with `node --version`.

---

## Known Clients

### Streamer.bot

- **Client name:** `"streamerbot"` (matches `CLIENT_NAMES.STREAMERBOT` in `@stream-overlay/shared`)
- **Subscriptions:** `[]` — publish-only in v1. Streamer.bot does not subscribe to any topics.
- **Connection lifecycle:** connects at stream start via `broker-connect.cs`; disconnects at stream end via `broker-disconnect.cs`.
- **Connection mechanism:** Uses Streamer.bot's built-in WebSocket client (configured via UI as a `ws://localhost:8765` entry, referenced by index `0` in CPH calls).
- **Publish implementation:** `PublishBrokerMessage(topic, payloadJson)` helper inlined into each publishing action. Source template: `Actions/Overlay/broker-publish.cs`.
- **Topics it publishes:** `overlay.*`, `overlay.audio.*`, `lotat.*`, `squad.*`, `stream.*`
- **Implementation docs:** `.agents/roles/streamerbot-dev/skills/overlay-integration.md`

### Overlay

- **Client name:** `"overlay"` (matches `CLIENT_NAMES.OVERLAY` in `@stream-overlay/shared`)
- **Subscriptions:** `overlay.*`, `overlay.audio.*`, `lotat.*`, `squad.*`
- **Connection lifecycle:** connects when the OBS browser source loads; auto-reconnects if broker restarts.
- **Publish behavior:** v1 publishes `system.ping` only — overlay is a pure subscriber for all application topics.
