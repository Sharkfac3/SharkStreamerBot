---
id: actions-overlay
type: domain-route
description: Streamer.bot bridge scripts, paste targets, and publisher templates for the stream overlay broker.
owner: streamerbot-dev
secondaryOwners:
  - app-dev
workflows:
  - change-summary
  - sync
  - validation
status: active
path: Actions/Overlay/
---

# Actions/Overlay — Agent Guide

## Purpose

This folder owns the Streamer.bot-side bridge to the stream overlay ecosystem. These C# scripts connect Streamer.bot to the local overlay broker, publish broker-message envelopes, disconnect cleanly, and provide an end-to-end overlay test action.

Streamer.bot never talks directly to the Phaser overlay. It publishes to the broker; the broker routes to the overlay and any future clients.

```text
Streamer.bot C# action -> local broker WebSocket -> Phaser overlay in OBS
```

## When to Activate

Use this guide for changes under [Actions/Overlay/](./), especially:

- [broker-connect.cs](broker-connect.cs) — opens the broker WebSocket and sends the Streamer.bot hello frame.
- [broker-publish.cs](broker-publish.cs) — reference template for the `PublishBrokerMessage` helper.
- [broker-disconnect.cs](broker-disconnect.cs) — clean stream-end disconnect.
- [test-overlay.cs](test-overlay.cs) — manual end-to-end overlay smoke test.
- Any Streamer.bot setup, paste target, WebSocket client index, or broker-publish helper changes.

Also activate when app-side protocol changes in [Apps/stream-overlay/](../../Apps/stream-overlay/) require C# publisher changes.

## Primary Owner

`streamerbot-dev` owns this folder: C# action behavior, Streamer.bot UI setup assumptions, paste targets, global variables, and helper snippets copied into publishing actions.

## Secondary Owners / Chain To

- `app-dev` for broker protocol changes, topic taxonomy, TypeScript shared types, overlay rendering behavior, and app build/test commands.
- `lotat-tech` when `lotat.*` payloads or stage ordering are affected.
- `brand-steward` if chat-facing test messages or public overlay copy changes.
- `ops` for validation, sync, and final change-summary handoff.

Related guides:

- [Apps/stream-overlay/AGENTS.md](../../Apps/stream-overlay/AGENTS.md) — route, ownership, local workflow, and app validation.
- [Apps/stream-overlay/docs/protocol.md](../../Apps/stream-overlay/docs/protocol.md) — broker protocol, topics, subscriptions, and topic-change workflow.
- [Apps/stream-overlay/docs/asset-system.md](../../Apps/stream-overlay/docs/asset-system.md) — asset paths, asset IDs, lifecycle commands, and audio behavior.
- [Apps/stream-overlay/docs/rendering-notes.md](../../Apps/stream-overlay/docs/rendering-notes.md) — overlay renderer behavior, feature renderers, OBS caveats, and gotchas.
- [Actions/Squad/AGENTS.md](../Squad/AGENTS.md) — Squad publish-template integration.
- [Actions/LotAT/AGENTS.md](../LotAT/AGENTS.md) — LotAT publish-template integration.

## Required Reading

Before editing, read:

- [Apps/stream-overlay/AGENTS.md](../../Apps/stream-overlay/AGENTS.md) for route/ownership and [Apps/stream-overlay/docs/protocol.md](../../Apps/stream-overlay/docs/protocol.md) for app-side broker and protocol rules.
- [Apps/stream-overlay/packages/shared/src/topics.ts](../../Apps/stream-overlay/packages/shared/src/topics.ts) and [Apps/stream-overlay/packages/shared/src/protocol.ts](../../Apps/stream-overlay/packages/shared/src/protocol.ts) for the authoritative TypeScript contract.
- [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) before adding or renaming topic constants.
- [Actions/Helpers/json-no-external-libraries.md](../Helpers/json-no-external-libraries.md) when complex JSON serialization is needed.

## Local Workflow

1. Keep scripts paste-ready for Streamer.bot inline C# actions. Each deployed action needs a complete `CPHInline` class.
2. For any publishing action, copy the constants block and `PublishBrokerMessage` helper from [broker-publish.cs](broker-publish.cs) into the target action's class.
3. Build payloads as JSON strings or with the shared serialization helper from [Actions/Helpers/json-no-external-libraries.md](../Helpers/json-no-external-libraries.md).
4. Publish to topic names that match [Apps/stream-overlay/packages/shared/src/topics.ts](../../Apps/stream-overlay/packages/shared/src/topics.ts) and [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md).
5. Test with the broker and overlay running from [Apps/stream-overlay/](../../Apps/stream-overlay/), then run [test-overlay.cs](test-overlay.cs) via a mod/operator-only chat command.

## Validation

For documentation-only changes, run the agent-tree validator from the repository root:

```bash
python3 Tools/AgentTree/validate.py
```

For Streamer.bot script changes:

- Manually inspect C# syntax for Streamer.bot-compatible inline script style.
- Confirm the broker is running from [Apps/stream-overlay/](../../Apps/stream-overlay/) with `pnpm dev:broker`.
- Confirm the overlay is loaded with `pnpm dev:overlay` and debug mode visible in a browser or OBS.
- Trigger [broker-connect.cs](broker-connect.cs), then [test-overlay.cs](test-overlay.cs), and verify the broker health endpoint shows Streamer.bot connected.

## Boundaries / Out of Scope

- Do not add broker, protocol, or rendering logic here. That belongs in [Apps/stream-overlay/](../../Apps/stream-overlay/).
- Do not add feature-specific Squad or LotAT business logic here. Those publishers live beside their feature actions under [Actions/Squad/](../Squad/) and [Actions/LotAT/](../LotAT/).
- Do not deploy [broker-publish.cs](broker-publish.cs) as its own Streamer.bot action. It is a reference template.
- Do not hardcode new topic strings without updating the shared TypeScript source and [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md).
- Info-service and production-manager migration are out of scope for this route.

## Handoff Notes

Every code change in this folder needs a final handoff that includes:

- Streamer.bot paste target list for each edited `.cs` file.
- Whether the operator must update the WebSocket client index or broker host/port in Streamer.bot UI.
- Whether [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) needs matching updates.
- App-side validation output if the change affects [Apps/stream-overlay/](../../Apps/stream-overlay/).
- Manual smoke-test result for [test-overlay.cs](test-overlay.cs) when runtime behavior changes.

## Runtime Notes

### Streamer.bot WebSocket setup

Streamer.bot uses its built-in WebSocket client UI. Configure one client for the local broker:

| Setting | Value |
|---|---|
| Name | Overlay Broker |
| Host | localhost |
| Port | 8765 |
| Endpoint | / |
| Scheme | ws |

The scripts assume this client is connection index 0. If the operator changes the order in Streamer.bot, update the broker WebSocket index constant in all relevant publisher helpers.

### Connection lifecycle

1. Stream start triggers [broker-connect.cs](broker-connect.cs).
2. The script connects WebSocket client index 0 and sends a hello frame with client name `streamerbot` and no subscriptions.
3. Publisher actions call `PublishBrokerMessage(topic, payloadJson)`.
4. Stream end triggers [broker-disconnect.cs](broker-disconnect.cs), which closes the socket and clears `broker_connected`.

The C# helper uses Streamer.bot WebSocket APIs. Do not add raw .NET WebSocket implementations.

### Runtime variable

| Variable | Scope | Meaning |
|---|---|---|
| `broker_connected` | Non-persisted global | Best-effort flag set by connect/disconnect scripts and checked by publishers. The helper still checks the actual WebSocket state. |

Stream start should reset `broker_connected` to false before connecting. If the Twitch stream-start script owns that reset, keep the behavior documented in that folder's guide when it is created.

## Paste / Sync Targets

Each edited C# file in this folder is a Streamer.bot paste target:

| Repo file | Streamer.bot target |
|---|---|
| [broker-connect.cs](broker-connect.cs) | Stream-start sub-action, plus optional manual reconnect action. |
| [broker-disconnect.cs](broker-disconnect.cs) | Stream-end sub-action. |
| [test-overlay.cs](test-overlay.cs) | Moderator/broadcaster-only `!testoverlay` command action. |
| [broker-publish.cs](broker-publish.cs) | Not standalone; copy constants and helper into each action that publishes overlay messages. |

If you edit [broker-publish.cs](broker-publish.cs), find and update every copied helper in feature publisher templates or runtime actions as needed. Current known reference-template locations include [Actions/LotAT/overlay-publish.cs](../LotAT/overlay-publish.cs) and the per-game overlay-publish files under [Actions/Squad/](../Squad/).

## Protocol / Publisher Contract

`PublishBrokerMessage` accepts a topic string and a pre-serialized JSON payload string. It wraps them in a broker-message envelope with:

- generated ID,
- topic,
- sender `streamerbot`,
- current timestamp,
- payload object.

It then sends the message over WebSocket client index 0. If the socket is down, it attempts one reconnect and resends the hello frame before publishing. Failure is logged and returns false; publishing scripts should fail gracefully and avoid throwing during a live stream.

Common Streamer.bot-published namespaces:

| Namespace | Use |
|---|---|
| `overlay.*` | Generic canvas asset commands such as spawn, move, animate, remove, and clear. |
| `overlay.audio.*` | Sound playback control. |
| `lotat.*` | LotAT session visual lifecycle. |
| `squad.*` | Squad mini-game visual lifecycle. |
| `stream.*` | Platform event visual alerts. |

## Build / Test Commands

The C# actions do not have a local compiler in this repo. Use app-side commands for end-to-end testing:

```bash
cd Apps/stream-overlay
pnpm dev:broker
pnpm dev:overlay
pnpm typecheck
pnpm build
```

Manual runtime smoke test:

1. Start the broker and overlay.
2. Trigger [broker-connect.cs](broker-connect.cs) from Streamer.bot.
3. Run [test-overlay.cs](test-overlay.cs) through a restricted `!testoverlay` command.
4. Verify a test image appears and disappears, and Streamer.bot logs show successful `overlay.spawn` and `overlay.remove` sends.

## Known Gotchas

- The broker must be running before [broker-connect.cs](broker-connect.cs) succeeds.
- The overlay can reconnect automatically, but Streamer.bot publishers rely on the configured WebSocket client and helper reconnect logic.
- [test-overlay.cs](test-overlay.cs) requires a test image at the path documented in the Script Reference section.
- A true `broker_connected` flag does not guarantee the socket is still open; the helper checks the actual WebSocket state on publish.
- Payload JSON must be valid. Use the serialization helper for nested or user-provided strings to avoid quote escaping bugs.

---

## Script Reference

Scripts that connect Streamer.bot to the stream overlay broker and publish overlay commands.

---

### `broker-connect.cs`

**Purpose:** Connects Streamer.bot to the overlay broker WebSocket and sends the ClientHello handshake.

**Expected trigger:** Optional manual reconnect action. The normal stream-start path now has equivalent connect/register logic inline in `Actions/Twitch Core Integrations/stream-start.cs`.

**Input:** None.

**Required runtime variables:**
- None on input. Sets `broker_connected` (non-persisted).

**Key outputs/side effects:**
- Opens WebSocket connection to `ws://localhost:8765` (client index 0).
- Sets `broker_connected` = `true` on success, `false` on failure.
- Sends `ClientHello` JSON frame: `{ type: "client.hello", name: "streamerbot", subscriptions: [] }`.

**Log output:**
- `[BrokerConnect] Connecting to broker...`
- `[BrokerConnect] Connected and ClientHello sent. Overlay broker is ready.`
- `[BrokerConnect] Connection failed...` (if broker is not running)

**Operator notes:**
- The broker must be running before this fires. Start order: broker → OBS → Streamer.bot.
- If connection fails, stream-start continues; only overlay commands are affected.
- To retry mid-stream: trigger this action from a mod command or hotkey.
- Normal stream-start no longer needs this as a separate sub-action if the updated `stream-start.cs` is pasted.

---

### `broker-publish.cs`

**Purpose:** Reference template for the `PublishBrokerMessage` helper method. **Not a standalone action.**

**How to use:** Copy the constants block and `PublishBrokerMessage` method into any `CPHInline` class that needs to publish overlay commands. See `test-overlay.cs` for a working example.

**What the helper does:**
- Accepts a topic string and a pre-serialized JSON payload string.
- Wraps them in a `BrokerMessage` envelope (`id`, `topic`, `sender`, `timestamp`, `payload`).
- Calls `CPH.WebsocketSend(message, 0)` to publish via the broker WebSocket client.
- Auto-reconnects once (with re-sent ClientHello) if the connection is down.
- Returns `true` if sent, `false` if the connection could not be established.

**Operator notes:**
- This file exists as a copy/paste source. Do not add it to Streamer.bot as its own action.

---

### `test-overlay.cs`

**Purpose:** End-to-end pipeline test. Spawns a test image at canvas center, waits 3 seconds, removes it.

**Expected trigger:** Chat command `!testoverlay`. Restrict to mods/operator in Streamer.bot command settings.

**Input:** None (no args read from trigger).

**Required runtime variables:**
- `broker_connected` (non-persisted) — set by `broker-connect.cs`.
- WebSocket client index 0 configured in Streamer.bot UI.

**Key outputs/side effects:**
- Publishes `overlay.spawn` → image appears at canvas center (960, 540).
- Waits 3 seconds.
- Publishes `overlay.remove` → image disappears with fade-out.
- Sends chat: `✅ Overlay test complete — if you saw the image appear and disappear, the pipeline works.`

**Wait behavior:** 3-second wait between spawn and remove. Chat confirmation comes after remove.

**Chat:**
- On success: `✅ Overlay test complete — if you saw the image appear and disappear, the pipeline works.`
- On broker failure (spawn): `⚠ Overlay test failed — broker not reachable. Check the log.`
- On broker failure (remove): `⚠ Overlay test: image appeared but remove failed. Check the log.`

**Log output:**
- `[TestOverlay] Running overlay pipeline test...`
- `[BrokerPublish] Sent topic=overlay.spawn id=<uuid>`
- `[BrokerPublish] Sent topic=overlay.remove id=<uuid>`
- `[TestOverlay] Test complete.`

**Operator notes:**
- The test image must exist at:
  `Apps/stream-overlay/packages/overlay/public/images/test-overlay-ping.png`
  Drop any PNG/JPG there. The exact appearance doesn't matter — the goal is to confirm the pipeline.
- If nothing appears on screen, check:
  1. Is the broker running? (`http://localhost:8765/health` — should show Streamer.bot in clients list)
  2. Is the overlay loaded in OBS? (browser source URL matches overlay dev/prod URL)
  3. Does the test image file exist under `overlay/public/images/`?
  4. Streamer.bot log — did `[BrokerPublish]` log `Sent topic=overlay.spawn`?

---

### `broker-disconnect.cs`

**Purpose:** Cleanly closes the WebSocket connection to the overlay broker.

**Expected trigger:** Sub-action inside the stream-end action (Twitch stream offline event). Can also be called manually.

**Input:** None.

**Required runtime variables:**
- `broker_connected` (non-persisted) — cleared here.

**Key outputs/side effects:**
- Sends a WebSocket close frame (allows broker to publish `system.client.disconnected` to subscribers).
- Sets `broker_connected` = `false`.

**Log output:**
- `[BrokerDisconnect] Disconnecting from broker...`
- `[BrokerDisconnect] Disconnected from overlay broker.`

**Operator notes:**
- Safe to call even if already disconnected — guarded by `WebsocketIsConnected()` check.

---

## Dependency: Broker

All scripts in this folder depend on the overlay broker (`Apps/stream-overlay/packages/broker/`) being reachable at `ws://localhost:8765`.

- **Dev mode:** `pnpm dev:broker` from `Apps/stream-overlay/`
- **Health check:** `http://localhost:8765/health` — shows connected clients and subscriptions
- **Start order:** broker → OBS → Streamer.bot stream-start

If the broker is not running, `broker-connect.cs` will log an error and all publish calls will fail silently (logged but not chat-visible).

---

## Operator Setup (One-Time)

### 1. Add WebSocket Client in Streamer.bot

In Streamer.bot UI: **Servers/Clients → WebSocket Clients → right-click → Add**

| Field    | Value             |
|----------|-------------------|
| Name     | Overlay Broker    |
| Host     | localhost         |
| Port     | 8765              |
| Endpoint | /                 |
| Scheme   | ws                |

This entry must be **first in the list** (index 0). If you add other clients, adjust `BROKER_WS_INDEX` in the scripts.

### 2. Use updated `stream-start.cs` for normal startup connection

Paste the updated `Actions/Twitch Core Integrations/stream-start.cs` into the Streamer.bot stream-start action. It now connects/registers with the overlay broker automatically if needed.

Keep `broker-connect.cs` available only as an optional manual reconnect action or hotkey for mid-stream broker restarts.

### 3. Add `broker-disconnect.cs` to stream-end

In stream-end action (Twitch stream offline): add **Execute C# Code** sub-action → paste `broker-disconnect.cs` content.

### 4. Add `!testoverlay` chat command

Create a new command action:
- Command: `!testoverlay`
- Permission: Moderator or Broadcaster only
- Sub-action: **Execute C# Code** → paste `test-overlay.cs` content

### 5. Add test image

Drop any PNG/JPG at:
```
Apps/stream-overlay/packages/overlay/public/images/test-overlay-ping.png
```

### 6. Broker connection state

No separate reset snippet is needed after pasting the updated `Actions/Twitch Core Integrations/stream-start.cs`. The stream-start script clears stale `broker_connected` state before a connect/register attempt and sets it to `true` after sending ClientHello.

---

## Writing New Overlay-Publishing Actions

1. Copy the constants block and `PublishBrokerMessage` method from `broker-publish.cs`.
2. Paste into your `CPHInline` class.
3. Build your payload as a JSON string (string concatenation or the `SerializeJson` helper from `Actions/Helpers/json-no-external-libraries.md`).
4. Call `PublishBrokerMessage("overlay.spawn", payloadJson)`.
5. Topic strings: use the constants from `@stream-overlay/shared/topics.ts` — see `Actions/SHARED-CONSTANTS.md → Overlay / Broker` for the C# string equivalents.

For the full list of topics and payload shapes, see:
- `Apps/stream-overlay/docs/protocol.md`
- `Apps/stream-overlay/packages/shared/src/protocol.ts`
- `Actions/Helpers/overlay-broker.md` for the reusable connect/register helper if `broker-connect.cs` is retired later.
