---
id: apps-stream-overlay-rendering-notes
type: domain-reference
description: Phaser overlay renderer notes, feature renderer ownership, OBS caveats, and known gotchas for Apps/stream-overlay.
status: active
owner: app-dev
---

# Stream Overlay Rendering Notes

## Overlay Rules

- Development OBS/browser URL is http://localhost:5173.
- Production OBS loads the generated overlay HTML from the overlay package's generated dist directory after `pnpm build`.
- The overlay canvas is transparent, 1920 by 1080, and should sit above stream content in OBS.
- If overlay audio is used, the OBS browser source must have **Control audio via OBS** enabled.
- The overlay is not authoritative state. If a visual is wrong, inspect the publisher payload before adding renderer-side business rules.

Read [../packages/overlay/src/scenes/OverlayScene.ts](../packages/overlay/src/scenes/OverlayScene.ts) and [../packages/overlay/src/broker-client.ts](../packages/overlay/src/broker-client.ts) before changing subscriptions or render dispatch.

## LotAT Rendering

LotAT visuals are text/graphics-driven and live under [../packages/overlay/src/components/lotat/](../packages/overlay/src/components/lotat/) with orchestration in [../packages/overlay/src/systems/lotat-renderer.ts](../packages/overlay/src/systems/lotat-renderer.ts).

Keep renderer behavior aligned with the shared [LotAT contract](../../../Docs/Architecture/lotat-contract.md) and the authoritative TypeScript protocol files. This app owns visual layout and rendering implementation, not LotAT business rules.

LotAT C# publish templates live at [../../../Actions/LotAT/overlay-publish.cs](../../../Actions/LotAT/overlay-publish.cs). Runtime semantics are documented in [../../../Actions/LotAT/AGENTS.md](../../../Actions/LotAT/AGENTS.md).

## Squad Rendering

Squad visuals live under [../packages/overlay/src/components/squad/](../packages/overlay/src/components/squad/) with orchestration in [../packages/overlay/src/systems/squad-renderer.ts](../packages/overlay/src/systems/squad-renderer.ts).

Current behavior:

- Squad visuals occupy the top of the screen and can coexist with the LotAT lower third.
- Duck, Pedro, and Clone use a top-left panel; Toothless uses an upper-right popup.
- All four games use text/graphics primitives in v1; no image assets are required.
- Per-game messages are `squad.duck.*`, `squad.pedro.*`, `squad.clone.*`, and `squad.toothless.*`.

Squad C# publish templates live under [../../../Actions/Squad/](../../../Actions/Squad/) and are documented in [../../../Actions/Squad/AGENTS.md](../../../Actions/Squad/AGENTS.md).

## Known Gotchas

- Start order for live use is broker, then OBS/overlay, then Streamer.bot connection. The overlay reconnects, but clean startup is easier to debug.
- Streamer.bot must have its WebSocket client configured to the broker before C# publish scripts can send messages.
- Topic names must remain synchronized between [../packages/shared/src/topics.ts](../packages/shared/src/topics.ts) and [../../../Actions/SHARED-CONSTANTS.md](../../../Actions/SHARED-CONSTANTS.md).
- The broker health endpoint is the fastest way to see whether overlay and Streamer.bot are actually connected.
- Keep stream/game business logic in Streamer.bot scripts under [../../../Actions/](../../../Actions/), not in renderer code.
