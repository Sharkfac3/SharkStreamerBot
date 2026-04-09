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

        // ── Guard: already connected ─────────────────────────────────────────
        // WebsocketIsConnected() queries the live socket state, not our global var.
        // If Streamer.bot already has an open connection (e.g. manual retry),
        // update the flag and return immediately.
        if (CPH.WebsocketIsConnected(BROKER_WS_INDEX))
        {
            CPH.LogWarn($"{LOG_PREFIX} Already connected to broker. No action needed.");
            CPH.SetGlobalVar(VAR_BROKER_CONNECTED, true, false);
            return true;
        }

        // ── Mark disconnected before attempt ─────────────────────────────────
        // Ensures downstream publish helpers see a clean false state if we fail.
        CPH.SetGlobalVar(VAR_BROKER_CONNECTED, false, false);
        CPH.LogWarn($"{LOG_PREFIX} Connecting to broker (WS client index {BROKER_WS_INDEX})...");

        // ── Attempt connection ────────────────────────────────────────────────
        // WebsocketConnect is non-blocking — it opens the socket in the background.
        // We wait WAIT_CONNECT_MS milliseconds before checking whether it succeeded.
        CPH.WebsocketConnect(BROKER_WS_INDEX);
        CPH.Wait(WAIT_CONNECT_MS);

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
