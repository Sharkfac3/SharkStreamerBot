# Stream Overlay

A TypeScript monorepo that adds a live visual layer to the stream. Streamer.bot publishes events; the broker routes them; the Phaser overlay renders them in OBS.

```
Streamer.bot → WebSocket → Broker (port 8765) → WebSocket → Overlay (Phaser in OBS)
```

---

## Quick Start

### Prerequisites

- Node.js 18+ and pnpm installed
- `cd Apps/stream-overlay`

### Install dependencies

```bash
pnpm install
```

### Start the broker

```bash
pnpm dev:broker
# WebSocket hub running at ws://localhost:8765
# Health check: http://localhost:8765/health
```

### Start the overlay (dev mode)

```bash
pnpm dev:overlay
# Vite dev server at http://localhost:5173
# Debug mode: http://localhost:5173?debug=true
```

### Add as OBS browser source

| Setting | Value |
|---|---|
| URL | `http://localhost:5173` (dev) or local file `packages/overlay/dist/index.html` (prod) |
| Width | `1920` |
| Height | `1080` |
| Custom CSS | *(leave empty)* |

Place the browser source **above** your stream content in the OBS source list. The canvas is fully transparent — it composites over whatever is below it.

**Enable audio** if you plan to use sound alerts: in the browser source properties, check "Control audio via OBS".

### Connect Streamer.bot

1. In Streamer.bot: **Servers/Clients → WebSocket Clients → Add**
   - Host: `localhost`, Port: `8765`
   - This connection index defaults to `0` — match the constant in `Actions/Overlay/broker-connect.cs`
2. Add an action that triggers on stream start and runs `Actions/Overlay/broker-connect.cs`
3. Add an action that triggers on stream end and runs `Actions/Overlay/broker-disconnect.cs`

Test end-to-end: paste `Actions/Overlay/test-overlay.cs` into a Streamer.bot action and trigger it with `!testoverlay` in chat. You should see a test image appear on the overlay canvas.

---

## Architecture

### What each package does

| Package | Name | What it does |
|---|---|---|
| `packages/shared` | `@stream-overlay/shared` | TypeScript types, message protocol, and topic name constants. The source of truth for the data contract. |
| `packages/broker` | `@stream-overlay/broker` | Node.js WebSocket hub. Accepts connections, routes messages by topic to subscribers. No business logic. |
| `packages/overlay` | `@stream-overlay/overlay` | Phaser 3 browser source. Connects to broker, subscribes to topics, renders visual events. |

### How data flows

1. Something happens in the stream (viewer subscribes, LotAT session starts, squad game begins).
2. Streamer.bot fires an action that calls `PublishBrokerMessage(topic, payload)`.
3. The broker receives the message and delivers it to all clients subscribed to that topic.
4. The overlay receives the message and delegates to the appropriate renderer (LotAT, squad, asset system).
5. Phaser renders the visual on the 1920×1080 transparent canvas over the stream.

**Key rule:** The overlay is a dumb renderer. It does what the messages tell it. Business logic (when to show things, what values to use) stays in Streamer.bot.

### Where assets go

All local assets live under `packages/overlay/public/assets/`. Paths in broker messages are relative to `public/`:

```
public/assets/
  alerts/    ← stream event images (subs, raids, follows, bits)
  lotat/     ← LotAT character art, scene images
  squad/     ← squad mini-game images
  audio/     ← MP3 files for sound alerts
```

---

## Day-to-Day Operations

### Starting the system

```bash
# Terminal 1
cd Apps/stream-overlay && pnpm dev:broker

# Terminal 2
cd Apps/stream-overlay && pnpm dev:overlay
```

In Streamer.bot, trigger `broker-connect.cs` (or let stream-start do it automatically).

### Production build

```bash
cd Apps/stream-overlay
pnpm build
# Output: packages/overlay/dist/
```

In OBS, change the browser source URL to a local file pointing at `packages/overlay/dist/index.html`. The build uses relative paths so it works correctly as a `file://` URL.

### Adding new assets

Drop files into the appropriate subfolder under `packages/overlay/public/assets/`. No build step needed — Vite copies `public/` to `dist/` as-is. In dev mode, files are served directly.

Reference them in broker messages with paths relative to `public/`, e.g. `"assets/lotat/archivist.png"`.

### Stopping

Close the two terminal processes. The overlay will show an empty canvas and reconnect automatically when the broker comes back.

### Troubleshooting

**Nothing appears on screen:**
1. Is the broker running? Check `http://localhost:8765/health` — you should see the overlay listed as a connected client.
2. Is the OBS browser source pointed at the right URL?
3. Does the asset file exist at the path specified in the `src` field?
4. Check the Streamer.bot log — did `[BrokerPublish]` log `Sent topic=...`?

**Overlay shows orange dot (not connecting):**
- The broker is not running. Start it with `pnpm dev:broker`.

**Broker starts but overlay won't connect:**
- Check if port 8765 is in use by something else: `netstat -ano | findstr 8765`

**`[BrokerPublish] Reconnect failed` in Streamer.bot log:**
- The broker crashed or was never started. Restart it and then manually trigger `broker-connect.cs`.

**Image spawns but never disappears:**
- Check that the `overlay.remove` message used the same `assetId` as the spawn.
- You can clear everything by publishing `overlay.clear` with payload `{}`.

---

## For Developers (Future Agents)

### Adding a new overlay feature

1. Define any new payload types in `packages/shared/src/protocol.ts`.
2. Add topic constants to `packages/shared/src/topics.ts` (and `WILDCARD_*` if needed).
3. Add the topic to the overlay's `ClientHello` subscriptions in `packages/overlay/src/broker-client.ts` (or it's already covered by an existing wildcard subscription).
4. Add a handler in `packages/overlay/src/scenes/OverlayScene.ts` that calls your new renderer.
5. Build the renderer in `packages/overlay/src/` — use `AssetManager` and `AnimationSystem` for visuals.
6. Add a publish method in the appropriate `Actions/*/overlay-publish.cs` reference template.
7. Run `pnpm typecheck` to verify zero type errors.

### Adding a new broker topic

Topics follow the `<namespace>.<event>` naming pattern (e.g. `lotat.node.enter`, `squad.duck.start`). Add the constant to `topics.ts` and update `SHARED-CONSTANTS.md` in `Actions/` with the equivalent C# constant.

### Adding a new Streamer.bot integration

Create a new `overlay-publish.cs` reference template in the relevant `Actions/` subfolder. Follow the pattern in `Actions/Overlay/broker-publish.cs` — inline `PublishBrokerMessage` into the class, define topic constants at the top, and add one method per topic.

**Never deploy `overlay-publish.cs` as a standalone action** — it is a reference template. Copy the methods into the target engine script.

### Where to find things

| What | Where |
|---|---|
| All topic names and payload types | `packages/shared/src/topics.ts`, `packages/shared/src/protocol.ts` |
| Broker message routing | `packages/broker/src/message-router.ts`, `topic-matcher.ts` |
| Overlay scene + subscriptions | `packages/overlay/src/scenes/OverlayScene.ts` |
| Asset spawn/move/animate/remove | `packages/overlay/src/systems/asset-manager.ts` |
| Animation presets | `packages/overlay/src/systems/animation-system.ts` |
| LotAT visual components | `packages/overlay/src/lotat/` |
| Squad visual components | `packages/overlay/src/squad/` |
| Agent route, ownership, and workflow | `Apps/stream-overlay/AGENTS.md` |
| Protocol and topic rules | `Apps/stream-overlay/docs/protocol.md` |
| Asset paths, lifecycle, and audio | `Apps/stream-overlay/docs/asset-system.md` |
| Renderer notes and OBS gotchas | `Apps/stream-overlay/docs/rendering-notes.md` |
| Streamer.bot integration guide | `Actions/Overlay/AGENTS.md` |

### Testing without Streamer.bot

The broker includes two test session scripts:

```bash
cd Apps/stream-overlay/packages/broker

# Run a full LotAT session simulation
npx tsx src/lotat-test-session.ts

# Run all four squad game simulations
npx tsx src/squad-test-session.ts
```

For manual message injection, run the interactive test client:

```bash
npx tsx src/test-client.ts
```
