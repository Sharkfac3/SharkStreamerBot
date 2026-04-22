# Overlay Engine Setup

This is the operator guide for setting up the stream overlay pipeline end-to-end.

The pipeline is:

```
Streamer.bot → WebSocket → Broker (port 8765) → WebSocket → Overlay (Phaser in OBS)
```

All three pieces must be running and connected for anything to appear on screen.

---

## Prerequisites

- Node.js 18+ installed
- pnpm installed (`npm install -g pnpm` if missing)
- Dependencies installed: `cd Apps/stream-overlay && pnpm install`

---

## Step 1 — Start the Broker

Open a terminal and run:

```bash
cd Apps/stream-overlay
pnpm dev:broker
```

Expected output:
```
Broker listening   ws://localhost:8765
Health check       http://localhost:8765/health
```

Leave this terminal open. The broker must stay running during the stream.

Verify it is up: open `http://localhost:8765/health` in a browser. You should see JSON with `"status": "ok"`.

---

## Step 2 — Start the Overlay

Open a second terminal and run:

```bash
cd Apps/stream-overlay
pnpm dev:overlay
```

Expected output:
```
VITE ready in ...ms
➜  Local:   http://localhost:5173/
```

Leave this terminal open too.

Optional: open `http://localhost:5173?debug=true` in a browser to confirm the overlay loads. You should see a mostly-transparent canvas. A colored dot in the corner indicates broker connection status — orange means not connected yet, green means connected.

---

## Step 3 — OBS Browser Source

In OBS, add a **Browser Source** to your scene:

| Field | Value |
|---|---|
| URL | `http://localhost:5173` |
| Width | `1920` |
| Height | `1080` |
| Custom CSS | *(leave empty)* |
| Control audio via OBS | checked (required for sound alerts) |

Position the browser source **above all other sources** in the scene source list. The overlay canvas is fully transparent — it composites on top of whatever is beneath it.

Repeat this for every scene where you want overlay elements to appear.

---

## Step 4 — Streamer.bot WebSocket Client

This is a one-time setup. Do it once and leave it.

In Streamer.bot: **Servers/Clients → WebSocket Clients → right-click → Add**

| Field | Value |
|---|---|
| Name | `Overlay Broker` |
| Host | `localhost` |
| Port | `8765` |
| Endpoint | `/` |
| Scheme | `ws` |
| **Auto Connect** | **OFF — leave unchecked** |

**Critical:** Auto Connect must be OFF. If it is on, Streamer.bot will open the TCP connection at startup before the handshake script runs, and the broker will never register the client. The connection will appear live but no commands will reach the overlay.

This entry must be **first in the WebSocket Clients list** (index 0). If you add other WebSocket clients later, keep the broker entry at the top, or update `BROKER_WS_INDEX` in all broker scripts.

---

## Step 5 — Streamer.bot Actions

### broker-connect

Create an action in Streamer.bot:
- Name: `Overlay - Broker Connect` (or similar)
- Sub-action: **Execute C# Code** → paste contents of `Actions/Overlay/broker-connect.cs`

Wire this action as a sub-action inside your **stream-start action**, after the `broker_connected` reset (see below).

You can also trigger it manually to reconnect mid-stream if the broker restarts.

### broker-disconnect

Create an action:
- Name: `Overlay - Broker Disconnect`
- Sub-action: **Execute C# Code** → paste contents of `Actions/Overlay/broker-disconnect.cs`

Wire this as a sub-action inside your **stream-end action** (Twitch stream offline event).

### stream-start reset

In your stream-start action, **before** the broker-connect sub-action runs, add a **Set Global Variable** sub-action:

| Field | Value |
|---|---|
| Variable name | `broker_connected` |
| Value | `false` |
| Persisted | No |

This clears stale state from the previous session so broker-connect always runs the full handshake.

---

## Step 6 — Test the Pipeline

### Add a test image

Drop any PNG or JPG into:

```
Apps/stream-overlay/packages/overlay/public/images/test-overlay-ping.png
```

The filename must be exactly `test-overlay-ping.png`. The image content does not matter.

### Create the test command action

Create an action in Streamer.bot:
- Name: `Overlay - Test`
- Sub-action: **Execute C# Code** → paste contents of `Actions/Overlay/test-overlay.cs`

Add a chat command trigger:
- Command: `!testoverlay`
- Permission: Moderator or Broadcaster only

### Run the test

1. Make sure the broker and overlay dev server are running.
2. Trigger `broker-connect` (or let stream-start do it).
3. Check `http://localhost:8765/health` — `streamerbot` should appear in the clients list.
4. Type `!testoverlay` in chat.

Expected result: a test image appears at the center of the overlay canvas, stays for 3 seconds, then fades out. Chat receives: `✅ Overlay test complete — if you saw the image appear and disappear, the pipeline works.`

---

## Start Order Every Stream

```
1. Start the broker      →  pnpm dev:broker
2. Start the overlay     →  pnpm dev:overlay  (or use the prod build)
3. Launch OBS
4. Start Streamer.bot    →  stream-start fires broker-connect automatically
```

The overlay reconnects automatically if the broker restarts mid-stream, but Streamer.bot will not re-send ClientHello on its own. If the broker crashes, restart it and manually trigger the broker-connect action.

---

## Production Build (Optional)

If you prefer not to run the Vite dev server during streams:

```bash
cd Apps/stream-overlay
pnpm build
# Output: packages/overlay/dist/
```

In OBS, change the browser source URL to a local file path pointing at:
```
packages/overlay/dist/index.html
```

Use the full absolute path. The build uses relative paths so it works correctly as a `file://` URL.

You still need the broker running (`pnpm dev:broker`) — the broker has no production build, it just runs as a Node process.

---

## Troubleshooting

**Health endpoint shows no clients after broker-connect ran**
- Check Streamer.bot log for `[BrokerConnect] Already connected to broker. No action needed.`
- If you see that: the WebSocket client has Auto Connect turned on. Turn it off, disconnect manually in Streamer.bot, then trigger broker-connect again.
- If you see `[BrokerConnect] Connection failed`: the broker is not running. Start it first.

**Nothing appears on screen during `!testoverlay`**
1. `http://localhost:8765/health` — is `streamerbot` in the clients list?
2. Is the overlay loaded in OBS? Does `http://localhost:5173` load in a browser?
3. Does the test image file exist at `overlay/public/images/test-overlay-ping.png`?
4. Streamer.bot log — did `[BrokerPublish] Sent topic=overlay.spawn` appear?

**Overlay shows orange dot (not connected to broker)**
- Broker is not running. Start it with `pnpm dev:broker`.

**Broker starts but overlay won't connect**
- Port 8765 may be in use by something else. Check: `netstat -ano | findstr 8765`

**Image spawns but never disappears**
- The `overlay.remove` message used a different `assetId` than the spawn.
- To clear everything: publish `overlay.clear` with payload `{}`.

**`[BrokerPublish] Reconnect failed` in Streamer.bot log**
- Broker crashed or was never started. Restart it, then manually trigger broker-connect.
