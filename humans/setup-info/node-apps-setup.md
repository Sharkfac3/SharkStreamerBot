# Node Apps Setup

This guide is for human operators setting up the repo's local Node/TypeScript apps on the stream PC.

## Prerequisites

- Node.js 18+ installed.
- npm is included with Node.js.
- pnpm installed for the stream overlay workspace:

```bash
npm install -g pnpm
```

Run commands from the repository root unless a step says to `cd` into an app folder.

---

## App Summary

| App | Folder | Package manager | Local URL / port | Purpose |
|---|---|---|---|---|
| Info Service | `Apps/info-service` | npm | `http://127.0.0.1:8766` | Local REST API and JSON store for stream info collections. |
| Production Manager | `Apps/production-manager` | npm | `http://127.0.0.1:5174` | Local admin UI for managing info-service data. |
| Stream Overlay Broker | `Apps/stream-overlay` | pnpm | `ws://localhost:8765`, health at `http://localhost:8765/health` | WebSocket message broker between Streamer.bot and the overlay. |
| Stream Overlay Renderer | `Apps/stream-overlay` | pnpm | `http://localhost:5173` | Phaser browser overlay for OBS. |

---

## Info Service

### Install

```bash
cd Apps/info-service
npm install
```

### Start for development

```bash
npm run dev
```

Expected service URL:

```text
http://127.0.0.1:8766
```

Health check:

```text
GET http://127.0.0.1:8766/health
```

### Build / check

```bash
npm run typecheck
npm run build
```

### Operator notes

- The service binds to `127.0.0.1` only.
- Runtime data is created under `Apps/info-service/data/` and is gitignored.
- Back up `Apps/info-service/data/` if the stream data should be preserved.

---

## Production Manager

Production Manager is the local admin UI for the info-service data. Start info-service first if you want live health and data pages to work.

### Install

```bash
cd Apps/production-manager
npm install
```

### Start for development

```bash
npm run dev
```

Open:

```text
http://127.0.0.1:5174
```

### Build / check

```bash
npm run typecheck
npm run build
```

Optional preview after building:

```bash
npm run preview
```

Preview URL:

```text
http://127.0.0.1:4174
```

### Operator notes

- If info-service is not running, the Health page will show an error. That is expected.
- This app is local-only and does not need LAN/public exposure.

---

## Stream Overlay

The stream overlay has two Node processes: the broker and the overlay dev server. For the full OBS and Streamer.bot setup, see `humans/setup-info/overlay-setup.md`.

### Install

```bash
cd Apps/stream-overlay
pnpm install
```

### Start the broker

Open terminal 1:

```bash
cd Apps/stream-overlay
pnpm dev:broker
```

Health check:

```text
http://localhost:8765/health
```

### Start the overlay renderer

Open terminal 2:

```bash
cd Apps/stream-overlay
pnpm dev:overlay
```

Open:

```text
http://localhost:5173
```

Debug URL:

```text
http://localhost:5173?debug=true
```

### Build / check

```bash
cd Apps/stream-overlay
pnpm typecheck
pnpm build
```

### OBS notes

Use an OBS Browser Source pointed at:

```text
http://localhost:5173
```

Recommended source size:

```text
1920 x 1080
```

Check **Control audio via OBS** if sound alerts are used.

---

## Typical Startup Order

For stream overlay work:

1. Start the overlay broker: `cd Apps/stream-overlay && pnpm dev:broker`
2. Start the overlay renderer: `cd Apps/stream-overlay && pnpm dev:overlay`
3. Launch OBS.
4. Launch Streamer.bot and trigger/connect the broker action.

For intro/admin data work:

1. Start info-service: `cd Apps/info-service && npm run dev`
2. Start production-manager: `cd Apps/production-manager && npm run dev`
3. Open `http://127.0.0.1:5174`

---

## Troubleshooting

- If a command says dependencies are missing, rerun `npm install` or `pnpm install` in that app folder.
- If a port is already in use, close the old terminal/process for that app and start it again.
- If Production Manager cannot load data, confirm info-service is running at `http://127.0.0.1:8766/health`.
- If the overlay shows disconnected, confirm the broker is running at `http://localhost:8765/health`.
