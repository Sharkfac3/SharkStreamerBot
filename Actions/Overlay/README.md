# Actions/Overlay

Scripts that connect Streamer.bot to the stream overlay broker and publish overlay commands.

---

## Scripts

### `broker-connect.cs`

**Purpose:** Connects Streamer.bot to the overlay broker WebSocket and sends the ClientHello handshake.

**Expected trigger:** Sub-action inside the stream-start action (Twitch stream online event). Can also be called manually to reconnect mid-stream.

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

### 2. Add `broker-connect.cs` to stream-start

In stream-start action: add **Execute C# Code** sub-action → paste `broker-connect.cs` content.

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

### 6. Reset `broker_connected` in stream-start

Add to `Actions/Twitch Core Integrations/stream-start.cs` (before broker-connect runs):
```csharp
CPH.SetGlobalVar("broker_connected", false, false);
```

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
