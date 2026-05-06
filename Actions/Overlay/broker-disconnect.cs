// ACTION-CONTRACT: Actions/Overlay/AGENTS.md#broker-disconnect.cs
// ACTION-CONTRACT-SHA256: deb0534dcfb12787c7b1d7136077702c50ccd8e047ee5edf4c487106d3c84510

using System;

public class CPHInline
{
    // SYNC CONSTANTS (Overlay / Broker feature)
    // Keep these names identical across all broker/overlay scripts.
    // See Actions/SHARED-CONSTANTS.md → Overlay / Broker section.
    private const int    BROKER_WS_INDEX      = 0;
    private const string VAR_BROKER_CONNECTED = "broker_connected";

    /*
     * Purpose:
     * - Cleanly closes the WebSocket connection to the overlay broker.
     * - Should run as a sub-action inside stream-end (stream offline event),
     *   or triggered manually when shutting down the broker.
     *
     * Expected trigger/input:
     * - Sub-action of stream-end action (Twitch stream offline event).
     * - Can also be triggered manually via a mod command or hotkey.
     *
     * Required runtime variables:
     * - broker_connected (non-persisted) — read and cleared here.
     *
     * Key outputs/side effects:
     * - Calls WebsocketDisconnect() to close the socket gracefully.
     * - Sets global var `broker_connected` = false.
     * - Logs disconnect status to Streamer.bot log.
     *
     * Operator notes:
     * - A clean disconnect lets the broker publish system.client.disconnected
     *   to all subscribers (e.g. the overlay), which can use it to clear UI.
     * - If Streamer.bot crashes or the process is killed, the broker will
     *   detect the dropped connection and publish the disconnect event anyway.
     * - This action is safe to call even if the socket is already closed —
     *   WebsocketIsConnected() is checked first.
     */
    public bool Execute()
    {
        const string LOG_PREFIX = "[BrokerDisconnect]";

        // ── Guard: already disconnected ───────────────────────────────────────
        // Check live socket state — do not rely on the global flag alone, as it
        // may be stale if the connection dropped unexpectedly.
        if (!CPH.WebsocketIsConnected(BROKER_WS_INDEX))
        {
            CPH.LogWarn($"{LOG_PREFIX} Already disconnected from broker. No action needed.");
            CPH.SetGlobalVar(VAR_BROKER_CONNECTED, false, false);
            return true;
        }

        // ── Disconnect ────────────────────────────────────────────────────────
        // WebsocketDisconnect sends a clean WebSocket close frame before
        // tearing down the TCP connection. The broker will receive this and
        // publish system.client.disconnected to all remaining subscribers.
        CPH.LogWarn($"{LOG_PREFIX} Disconnecting from broker (WS client index {BROKER_WS_INDEX})...");
        CPH.WebsocketDisconnect(BROKER_WS_INDEX);

        // ── Update connection state ───────────────────────────────────────────
        CPH.SetGlobalVar(VAR_BROKER_CONNECTED, false, false);
        CPH.LogWarn($"{LOG_PREFIX} Disconnected from overlay broker.");

        return true;
    }
}
