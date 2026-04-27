---
id: actions-helper-overlay-broker
type: reference
description: Copy/paste helpers for connecting Streamer.bot inline C# actions to the stream-overlay broker.
owner: streamerbot-dev
secondaryOwners:
  - app-dev
status: active
---

# Overlay Broker Helper

Use this helper when a Streamer.bot inline C# action needs to ensure the custom stream-overlay broker WebSocket is connected and registered before publishing messages.

Streamer.bot cannot import helper files at runtime. Copy the needed constants and method directly into the target `CPHInline` class.

## Runtime assumptions

Streamer.bot must have one WebSocket client configured for the local broker:

| Setting | Value |
|---|---|
| Name | Overlay Broker |
| Host | localhost |
| Port | 8765 |
| Endpoint | `/` |
| Scheme | `ws` |

The snippets below assume this client is index `0` in Streamer.bot's WebSocket Clients list.

## Constants

Copy these into any script that calls `EnsureOverlayBrokerConnected()`:

```csharp
private const int    BROKER_WS_INDEX      = 0;
private const string VAR_BROKER_CONNECTED = "broker_connected";
private const string BROKER_CLIENT_NAME   = "streamerbot";
private const int    WAIT_BROKER_CONNECT_MS = 600;
```

## Ensure connected and registered

This is the reusable version of the former `Actions/Overlay/broker-connect.cs` behavior. It is now also embedded in `Actions/Twitch Core Integrations/stream-start.cs` for normal stream-start startup.

```csharp
private bool EnsureOverlayBrokerConnected()
{
    const string LOG_PREFIX = "[OverlayBroker]";

    // WebsocketIsConnected() checks TCP state only — not whether ClientHello
    // was sent. Only skip when both the socket is open and our non-persisted
    // registration flag says the broker hello already succeeded.
    bool alreadyConnected = CPH.WebsocketIsConnected(BROKER_WS_INDEX);
    bool helloSent = (CPH.GetGlobalVar<bool?>(VAR_BROKER_CONNECTED, false) ?? false);
    if (alreadyConnected && helloSent)
    {
        CPH.LogWarn($"{LOG_PREFIX} Overlay broker already connected and registered.");
        return true;
    }

    // Clear stale state before trying to connect/register.
    CPH.SetGlobalVar(VAR_BROKER_CONNECTED, false, false);

    if (!alreadyConnected)
    {
        CPH.LogWarn($"{LOG_PREFIX} Connecting to overlay broker (WS client index {BROKER_WS_INDEX})...");
        CPH.WebsocketConnect(BROKER_WS_INDEX);
        CPH.Wait(WAIT_BROKER_CONNECT_MS);
    }
    else
    {
        CPH.LogWarn($"{LOG_PREFIX} TCP socket already open. Sending ClientHello to register with broker...");
    }

    if (!CPH.WebsocketIsConnected(BROKER_WS_INDEX))
    {
        CPH.LogError(
            $"{LOG_PREFIX} Connection failed after {WAIT_BROKER_CONNECT_MS}ms. " +
            "Is the stream-overlay broker running at ws://localhost:8765? " +
            "Overlay broker publishes may fail until reconnected."
        );
        return false;
    }

    string hello =
        "{\"type\":\"client.hello\"," +
        "\"name\":\"" + BROKER_CLIENT_NAME + "\"," +
        "\"subscriptions\":[]}";

    CPH.WebsocketSend(hello, BROKER_WS_INDEX);
    CPH.SetGlobalVar(VAR_BROKER_CONNECTED, true, false);
    CPH.LogWarn($"{LOG_PREFIX} Overlay broker connected and ClientHello sent.");
    return true;
}
```

## Usage in `Execute()`

Call the helper before publishing overlay/broker messages. For startup scripts, keep failure non-fatal so unrelated stream-start reset logic still runs.

```csharp
public bool Execute()
{
    bool brokerReady = EnsureOverlayBrokerConnected();
    if (!brokerReady)
    {
        // Choose behavior per action:
        // - Startup/reset actions: log and continue.
        // - Overlay-only actions: return after logging or send an operator-facing warning.
        CPH.LogWarn("[OverlayBroker] Broker unavailable; continuing without overlay publish.");
    }

    // ...action logic here...
    return true;
}
```

## Relationship to publishing helpers

- Use this helper to connect/register the Streamer.bot client with the broker.
- Use `Actions/Overlay/broker-publish.cs` as the canonical `PublishBrokerMessage(topic, payloadJson)` template for sending broker-message envelopes.
- Feature publisher templates under `Actions/LotAT/` and `Actions/Squad/` may include their own reconnect-and-hello path so live features can recover if the broker drops mid-stream.

## Gotchas

- `broker_connected` is non-persisted and best-effort. Always check `CPH.WebsocketIsConnected(BROKER_WS_INDEX)` before sending critical messages.
- ClientHello must be sent after each new WebSocket connection so the broker registers the client name `streamerbot`.
- If Streamer.bot auto-opens the TCP socket, this helper still sends ClientHello when `broker_connected` is false.
- Keep `BROKER_WS_INDEX` synchronized across connection and publish helpers if the WebSocket Clients list order changes.
