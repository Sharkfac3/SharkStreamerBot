# Overlay Integration — streamerbot-dev

## What This Is

The bridge between Streamer.bot's C# runtime and the Phaser overlay displayed in OBS.
Streamer.bot publishes commands; the overlay renders what it receives.

Streamer.bot → WebSocket → **broker** → WebSocket → Overlay (Phaser in OBS browser source)

The broker at `ws://localhost:8765` handles all routing. Streamer.bot never connects directly to the overlay.

---

## How the Connection Works

Streamer.bot uses its built-in WebSocket client support (configured via UI, not code):

1. **One-time UI setup**: Add an entry in **Servers/Clients → WebSocket Clients** pointing at `ws://localhost:8765`. This becomes the **connection index** (default: `0`).
2. **stream-start** fires → `broker-connect.cs` runs → calls `CPH.WebsocketConnect(0)` → sends `ClientHello`.
3. **Any publish action** calls `PublishBrokerMessage(topic, payloadJson)` → `CPH.WebsocketSend(message, 0)`.
4. **stream-end** fires → `broker-disconnect.cs` runs → calls `CPH.WebsocketDisconnect(0)`.

CPH manages the socket. Scripts never touch raw .NET WebSocket types.

---

## The PublishBrokerMessage Pattern

Every action that sends overlay commands uses the same helper method, inlined directly into that action's `CPHInline` class. Copy from `Actions/Overlay/broker-publish.cs`.

```csharp
// Constants (copy into your CPHInline class)
private const int    BROKER_WS_INDEX      = 0;
private const string VAR_BROKER_CONNECTED = "broker_connected";
private const string BROKER_CLIENT_NAME   = "streamerbot";
private const int    WAIT_RECONNECT_MS    = 600;
private const int    WAIT_HELLO_MS        = 200;

// Usage
string payload = "{\"assetId\":\"my-asset\",\"src\":\"images/foo.png\",\"position\":{\"x\":960,\"y\":540}}";
PublishBrokerMessage("overlay.spawn", payload);
```

The method:
- Checks `WebsocketIsConnected(0)` — auto-reconnects once with re-sent ClientHello if down.
- Builds `BrokerMessage` envelope: `{ id, topic, sender, timestamp, payload }`.
- Calls `CPH.WebsocketSend(message, 0)`.
- Returns `true` if sent, `false` if the connection could not be established (logs error, does not throw).

**Why inline instead of a separate action?** Streamer.bot scripts are isolated per action — there is no include/import mechanism. Inlining keeps each script copy/paste-ready, which is the deployment model.

---

## Topic Strings

Always use the string constants from `Actions/SHARED-CONSTANTS.md → Overlay / Broker`. Never hardcode topic strings. The canonical source is `@stream-overlay/shared/topics.ts`.

Common topics used by Streamer.bot:

| Constant (C# name)      | String               | Use |
|-------------------------|----------------------|-----|
| `TOPIC_OVERLAY_SPAWN`   | `overlay.spawn`      | Show image/GIF on canvas |
| `TOPIC_OVERLAY_MOVE`    | `overlay.move`       | Tween asset to new position |
| `TOPIC_OVERLAY_ANIMATE` | `overlay.animate`    | Apply animation preset |
| `TOPIC_OVERLAY_REMOVE`  | `overlay.remove`     | Remove specific asset |
| `TOPIC_OVERLAY_CLEAR`   | `overlay.clear`      | Remove all (or prefixed) assets |

LotAT topics (`lotat.*`) and Squad topics (`squad.*`) follow the same publish pattern — see `protocol.md` for full payload shapes.

---

## Payload Building

Payloads are built as JSON strings using string concatenation. No external library needed.

```csharp
// overlay.spawn — show image at canvas center
string spawnPayload =
    "{" +
    "\"assetId\":\"my-asset\"," +
    "\"src\":\"images/my-image.png\"," +
    "\"position\":{\"x\":960,\"y\":540}," +
    "\"width\":300," +
    "\"depth\":10," +
    "\"enterAnimation\":\"fade-in\"," +
    "\"enterDuration\":500" +
    "}";
PublishBrokerMessage("overlay.spawn", spawnPayload);

// overlay.remove — remove by asset ID
string removePayload =
    "{" +
    "\"assetId\":\"my-asset\"," +
    "\"exitAnimation\":\"fade-out\"," +
    "\"exitDuration\":500" +
    "}";
PublishBrokerMessage("overlay.remove", removePayload);
```

For complex payloads (e.g. `lotat.node.enter` with nested objects), use the `SerializeJson` helper from `Actions/HELPER-SNIPPETS.md § 7`.

### Asset ID Strategy

| Asset type | ID format | Example |
|---|---|---|
| Ephemeral (one-shot alert) | Any unique string — use `Guid.NewGuid().ToString()` | `"abc123-..."` |
| Named/persistent (HUD widget) | kebab-case slug | `"chaos-meter"`, `"lotat-hud"` |

Spawning with an existing `assetId` **replaces** that asset. Named assets are safe to update without tracking UUIDs.

---

## Actions That Publish Overlay Messages

| Script | Topics Published | Trigger |
|---|---|---|
| `Actions/Overlay/test-overlay.cs` | `overlay.spawn`, `overlay.remove` | `!testoverlay` chat command |
| `Actions/LotAT/overlay-publish.cs` | All `lotat.*` topics (15 methods) | Reference template — copy methods into LotAT engine scripts |
| `Actions/Squad/Duck/overlay-publish.cs` | `squad.duck.start`, `squad.duck.update`, `squad.duck.end` | Reference template — copy methods into duck-main/call/resolve.cs |
| `Actions/Squad/Clone/overlay-publish.cs` | `squad.clone.start`, `squad.clone.update`, `squad.clone.end` | Reference template — copy methods into clone scripts |
| `Actions/Squad/Pedro/overlay-publish.cs` | `squad.pedro.start`, `squad.pedro.update`, `squad.pedro.end` | Reference template — copy methods into pedro scripts |
| `Actions/Squad/Toothless/overlay-publish.cs` | `squad.toothless.start`, `squad.toothless.end` | Reference template — copy methods into toothless scripts |

Each overlay-publish.cs file is a **reference template only** — not deployed as its own action. Copy the publish methods and `PublishBrokerMessage` helper into the target engine script's `CPHInline` class.

---

## Troubleshooting

**Nothing appears on screen:**
1. Is the broker running? Check `http://localhost:8765/health` — Streamer.bot should appear in the clients list with `"subscriptions":[]`.
2. Is the overlay loaded in OBS? Browser source must point at the overlay URL (dev: `localhost:5173`).
3. Does the asset file exist? `Apps/stream-overlay/packages/overlay/public/<src path>`.
4. Check the Streamer.bot log — did `[BrokerPublish]` log `Sent topic=...`? If not, the WebSocket connection failed.

**`[BrokerPublish] Reconnect failed`:**
- The broker is not running or crashed.
- Start the broker: `pnpm dev:broker` from `Apps/stream-overlay/`.
- Then trigger `broker-connect.cs` manually or restart the stream.

**`[BrokerConnect] Connection failed`:**
- Same as above. Check broker is running on port 8765.
- Check the WebSocket client in Streamer.bot UI has the correct host/port.

**Image spawns but never disappears:**
- The `overlay.remove` publish failed (check log). Or the asset ID in remove doesn't match spawn.
- You can manually clear the overlay by publishing `overlay.clear` with an empty payload `{}`.

**broker_connected is true but WebsocketIsConnected() returns false:**
- The connection dropped without a clean close (network blip, broker restart).
- `PublishBrokerMessage` will auto-reconnect on the next publish attempt.
- If that also fails, trigger `broker-connect.cs` manually.

---

## Related Files

- `Actions/Overlay/broker-connect.cs` — connects + sends ClientHello
- `Actions/Overlay/broker-publish.cs` — reference template for `PublishBrokerMessage`
- `Actions/Overlay/broker-disconnect.cs` — clean close
- `Actions/Overlay/test-overlay.cs` — `!testoverlay` end-to-end test
- `Actions/Overlay/README.md` — operator setup guide
- `Actions/SHARED-CONSTANTS.md` → Overlay / Broker section
- `.agents/roles/app-dev/skills/stream-interactions/protocol.md` — full topic/payload reference
- `.agents/roles/app-dev/skills/stream-interactions/broker.md` — broker architecture
