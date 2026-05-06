// ACTION-CONTRACT: Actions/Overlay/AGENTS.md#broker-connect.cs
// ACTION-CONTRACT-SHA256: 7d948f63ef47fc0b3d176b78e5f0bb901e4fc8dbd2cf6a7ff663b4b0ccda2da4

using System;

public class CPHInline
{
    // SYNC CONSTANTS (Overlay / Broker feature)
    // Keep these names identical across:
    // - Actions/Overlay/broker-connect.cs
    // - Actions/Overlay/broker-publish.cs
    // - Actions/Overlay/broker-disconnect.cs
    // - Actions/Overlay/test-overlay.cs
    // - Actions/Twitch Core Integrations/stream-start.cs  (reset on stream start)
    // - Actions/SHARED-CONSTANTS.md
    //
    // BROKER_WS_INDEX: the zero-based position of the broker entry in
    // Streamer.bot → Servers/Clients → WebSocket Clients (top = 0).
    private const int    BROKER_WS_INDEX      = 0;
    private const string VAR_BROKER_CONNECTED = "broker_connected";

    // Client name registered with the broker.
    // Must match CLIENT_NAMES.STREAMERBOT in @stream-overlay/shared/topics.ts.
    private const string BROKER_CLIENT_NAME = "streamerbot";

    // How long (ms) to wait after WebsocketConnect() before checking status.
    // WebsocketConnect is non-blocking; the TCP handshake happens asynchronously,
    // so we pause briefly to let it complete before sending ClientHello.
    private const int WAIT_CONNECT_MS = 600;

    /*
     * Purpose:
     * - Connects Streamer.bot to the stream overlay broker via WebSocket.
     * - Sends the ClientHello handshake to register this client with the broker.
     * - Should run as a sub-action inside stream-start.cs (stream go-live event).
     *
     * Expected trigger/input:
     * - Sub-action of stream-start action (Twitch stream online event).
     * - Can also be triggered manually if the broker restarts mid-stream.
     *
     * Required runtime variables:
     * - None. Reads from constants only.
     *
     * Key outputs/side effects:
     * - Establishes WebSocket connection (Streamer.bot client index 0).
     * - Sets global var `broker_connected` = true/false (non-persisted).
     * - Sends ClientHello JSON frame to the broker.
     * - Logs connection result to Streamer.bot log.
     *
     * Operator notes:
     * - In Streamer.bot UI: Servers/Clients → WebSocket Clients → right-click → Add.
     *     Name:     Overlay Broker
     *     Host:     localhost
     *     Port:     8765
     *     Endpoint: /
     *     Scheme:   ws
     *   This entry must be the FIRST in the list (index 0), or adjust BROKER_WS_INDEX.
     * - The broker must be running BEFORE stream-start fires.
     *   Recommended start order: broker → OBS → Streamer.bot stream-start.
     * - If the broker is not running this action logs an error and returns true so
     *   stream-start continues. Overlay commands will be silently dropped (with a
     *   log warning) until the connection is restored.
     * - To reconnect mid-stream: trigger this action manually or add it as a
     *   sub-action on a mod command.
     * - stream-start.cs must reset `broker_connected` = false at startup so
     *   stale state from the previous session is cleared before this runs.
     */
    public bool Execute()
    {
        const string LOG_PREFIX = "[BrokerConnect]";

        // ── Guard: already connected AND hello already sent ──────────────────
        // WebsocketIsConnected() checks TCP state only — not whether ClientHello
        // was sent. Streamer.bot may auto-connect the socket at startup without
        // this script running, leaving the broker with a raw TCP connection but
        // no registered client. Only skip the full handshake if we know the
        // broker already has this client in its registry (VAR_BROKER_CONNECTED).
        bool alreadyConnected = CPH.WebsocketIsConnected(BROKER_WS_INDEX);
        bool helloSent = (CPH.GetGlobalVar<bool?>(VAR_BROKER_CONNECTED, false) ?? false);
        if (alreadyConnected && helloSent)
        {
            CPH.LogWarn($"{LOG_PREFIX} Already connected and registered with broker. No action needed.");
            return true;
        }

        // ── Mark disconnected before attempt ─────────────────────────────────
        CPH.SetGlobalVar(VAR_BROKER_CONNECTED, false, false);

        // ── Attempt connection (skip if TCP already open) ─────────────────────
        // WebsocketConnect is non-blocking. If Streamer.bot auto-connected the
        // socket at startup, the TCP channel is already open — calling Connect
        // again may be a no-op or cause an error depending on the version.
        if (!alreadyConnected)
        {
            CPH.LogWarn($"{LOG_PREFIX} Connecting to broker (WS client index {BROKER_WS_INDEX})...");
            CPH.WebsocketConnect(BROKER_WS_INDEX);
            CPH.Wait(WAIT_CONNECT_MS);
        }
        else
        {
            CPH.LogWarn($"{LOG_PREFIX} TCP already open. Sending ClientHello to register with broker...");
        }

        // ── Verify connection established ─────────────────────────────────────
        if (!CPH.WebsocketIsConnected(BROKER_WS_INDEX))
        {
            CPH.LogError(
                $"{LOG_PREFIX} Connection failed after {WAIT_CONNECT_MS}ms. " +
                "Is the broker running at ws://localhost:8765? " +
                "Start the broker before stream-start fires, then retry this action."
            );
            // Return true so stream-start continues. Mini-games and other features
            // are unaffected; only overlay commands will fail silently until fixed.
            return true;
        }

        // ── Send ClientHello handshake ────────────────────────────────────────
        // ClientHello must be the FIRST message sent on a new connection.
        // The broker expects: { type, name, subscriptions[] }.
        // Streamer.bot is publish-only in v1 — subscriptions is empty.
        // Shape defined in @stream-overlay/shared/protocol.ts → ClientHello.
        string hello =
            "{\"type\":\"client.hello\"," +
            "\"name\":\"" + BROKER_CLIENT_NAME + "\"," +
            "\"subscriptions\":[]}";

        CPH.WebsocketSend(hello, BROKER_WS_INDEX);

        // ── Update connection state ───────────────────────────────────────────
        CPH.SetGlobalVar(VAR_BROKER_CONNECTED, true, false);
        CPH.LogWarn($"{LOG_PREFIX} Connected and ClientHello sent. Overlay broker is ready.");

        return true;
    }
}
