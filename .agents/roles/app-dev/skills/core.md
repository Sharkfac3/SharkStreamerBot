# Core Skills — app-dev

## Business Context

This project supports a live R&D stream that is also a business — developing off-road racing products live while building community. Apps you build serve the content pipeline (overlays, dashboards, engagement tools) and may eventually serve the product side (customer-facing tools, community platforms). Build with extensibility in mind. Read `.agents/_shared/project.md` for the full business context and content pipeline.

---

## Tech Stack (Confirmed 2026-04-06)

| Layer | Choice | Reason |
|---|---|---|
| Language | **TypeScript 5.x** (strict) | Shared types across packages; catch errors at compile time, not at stream time |
| Package manager | **pnpm** (workspaces) | Fastest for monorepos; strict dependency isolation; `workspace:*` protocol for internal packages |
| Bundler (overlay) | **Vite 5.x** | Standard for Phaser + TypeScript; fast HMR in dev; `base: './'` for OBS local file loading |
| Runtime framework (overlay) | **Phaser 3** (`^3.90.0`) + `phaser3-rex-plugins` (`^1.80.0`, declaration kept for reference) | GIF rendering uses a hidden `<img>` element + `CanvasTexture` approach; rex-plugins is not imported at runtime |
| Node runtime (broker) | **Node.js ESM** | `"type": "module"` throughout; `NodeNext` module resolution in broker tsconfig |

---

## Monorepo Structure

```
Apps/stream-overlay/
  package.json              # workspace root — scripts: dev:overlay, dev:broker, build, typecheck
  pnpm-workspace.yaml       # declares packages/* as workspaces
  tsconfig.base.json        # shared strict TypeScript config; each package extends this
  .gitignore                # node_modules/, dist/, .vite/, *.tsbuildinfo
  packages/
    shared/                 # @stream-overlay/shared — shared types and message protocol
      src/index.ts          # barrel export
    broker/                 # @stream-overlay/broker — WebSocket message broker (Node.js)
      src/index.ts          # entry point
    overlay/                # @stream-overlay/overlay — Phaser OBS browser source
      index.html            # 1920×1080, transparent bg, mounts Phaser canvas
      vite.config.ts        # base: './', port 5173, emptyOutDir: true
      src/main.ts           # entry point
```

Each package is a separate pnpm workspace. Internal dependencies use `"workspace:*"` — never hardcode versions for internal packages.

---

## Integration Points

```
Streamer.bot (Actions/)
    │  WebSocket client (sends events to broker)
    │
    ▼
┌─────────────┐
│   Broker    │  Node.js process — pub/sub hub, no business logic
│  (port 8765)│  routes messages by topic to all connected clients
└─────────────┘
    │         │         │
    ▼         ▼         ▼
 Overlay   Future    Future
(Phaser)  Web App   AI Agent
 OBS src  (Prompt   (TBD)
          N+)
```

- **Streamer.bot → Broker**: Streamer.bot sends stream events (subscriptions, alerts, squad actions) to the broker via WebSocket. The C# scripts in `Actions/` are responsible for this push. See `Actions/SHARED-CONSTANTS.md` for global variable names.
- **Broker → Overlay**: The overlay subscribes to topics and receives only the events relevant to rendering. It does not talk directly to Streamer.bot.
- **Broker → Future clients**: The same hub serves future web dashboards, AI agents, or mobile apps — they connect via WebSocket and subscribe to relevant topics.
- **Mix It Up**: Integrated via REST API from Streamer.bot side. The overlay does not call Mix It Up directly. See `.agents/_shared/mixitup-api.md`.

---

## Deployment Model

| Mode | How it works |
|---|---|
| **Development** | `pnpm dev:overlay` runs Vite at `http://localhost:5173`. Point OBS browser source here. |
| **Production** | `pnpm build` outputs `packages/overlay/dist/`. OBS loads `dist/index.html` as a local file. |
| **Broker** | `pnpm dev:broker` — starts with `tsx watch` on port `8765`. Runs as a local Node.js process on the stream PC. |

`base: './'` in `vite.config.ts` is required for OBS local file loading — all asset paths must be relative for `file://` context.

---

## TypeScript Configuration Pattern

- `tsconfig.base.json` (root): strict settings, no module/lib — inherited by all packages
- `packages/shared/tsconfig.json`: `module: ESNext`, `moduleResolution: bundler`
- `packages/broker/tsconfig.json`: `module: NodeNext`, `moduleResolution: NodeNext`
- `packages/overlay/tsconfig.json`: `moduleResolution: bundler`, `lib: [ES2022, DOM, DOM.Iterable]`, `noEmit: true` (Vite owns the build)

Always run `pnpm typecheck` before committing changes to any package.

---

## Prompt Sequence (Architecture Log)

| Prompt | Deliverable | Status |
|---|---|---|
| 00 — Architecture Bootstrap | Monorepo scaffolding, config files, agent docs | ✅ Done |
| 01 — Message Protocol | Shared type definitions, topic names, payload shapes | ✅ Done |
| 02 — Broker Implementation | WebSocket hub, client registry, pub/sub routing | ✅ Done |
| 03 — Overlay Scaffolding | Phaser game config, scene manager, broker connection | ✅ Done |
| 04 — Asset Management | AssetManager, AnimationSystem, AudioManager, GIF support | ✅ Done |
| 05 — Streamer.bot Broker Scripts | broker-connect, broker-publish template, test-overlay, broker-disconnect | ✅ Done |
| 06 — LotAT Visual Layer | 11 LotAT components, lotat-renderer, OverlayScene wiring, overlay-publish.cs, test session | ✅ Done |
| 07 — Squad Visual Layer | 4 squad game renderers, squad-renderer, overlay-publish.cs per game, test session | ✅ Done |
| 08 — Integration Test & Drift Check | End-to-end verification, doc drift fixes, STATUS.md | ✅ Done |

---

## Known Integration Points

Apps in this project integrate with:
- **Streamer.bot** — via WebSocket API; broker is the intermediary
- **Mix It Up** — via REST API (see `.agents/_shared/mixitup-api.md`); called from Streamer.bot side only
- **OBS** — overlay is an OBS browser source; no obs-websocket needed for the overlay itself
- **Twitch** — events arrive via Streamer.bot, not directly from the overlay
- **info-service** — REST API; Streamer.bot scripts call `GET /info/:collection/:key` (read-only) to look up per-viewer data. `production-manager` is the sole write client.

## Info Service + Production Manager

Two standalone apps added in the info-service build (C1–C11):

| App | Path | Port | Role |
|-----|------|------|------|
| `info-service` | `Apps/info-service/` | `8766` | File-backed JSON REST API for per-viewer data |
| `production-manager` | `Apps/production-manager/` | `5174` (dev) / `4174` (preview) | React admin app for managing info-service collections |

Both bind `127.0.0.1` only. No auth. No LAN exposure.

Key patterns:
- Collections use `Collection<T>` generic engine with atomic writes and zod schema validation.
- Single-writer: only `production-manager` calls write routes. Streamer.bot and overlay are read-only clients.
- Data lives in `Apps/info-service/data/` — gitignored, operator backs up manually.
- Binary assets (mp3, gif) live in `Assets/` at repo root — also gitignored.
- See `.agents/_shared/info-service-protocol.md` for REST route contracts and collection schemas.
- See `.agents/roles/app-dev/context/info-service.md` for key files and architecture orientation.
