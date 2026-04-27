using System;

// =============================================================================
// broker-publish.cs — Reference Template
//
// This file is NOT a standalone action. It is the canonical source for the
// PublishBrokerMessage helper method.
//
// HOW TO USE:
//   Copy the private constants block and the PublishBrokerMessage method
//   directly into any CPHInline class that needs to publish overlay commands.
//   See test-overlay.cs for a complete working example.
//
// WHY INLINE INSTEAD OF A SEPARATE ACTION:
//   Streamer.bot C# scripts are isolated per action. There is no shared
//   include mechanism. Inlining keeps each script self-contained and
//   copy/paste-ready, which is the Streamer.bot deployment model.
//
// WHAT THIS METHOD DOES:
//   Wraps any JSON payload in a BrokerMessage envelope and sends it over
//   the pre-configured WebSocket client (index BROKER_WS_INDEX).
//   If the connection is down, it attempts one reconnect before giving up.
//
// BROKER MESSAGE ENVELOPE (from @stream-overlay/shared/protocol.ts):
//   {
//     "id":        "<UUID v4>",        // generated here via Guid.NewGuid()
//     "topic":     "<dot.notation>",   // e.g. "overlay.spawn"
//     "sender":    "streamerbot",      // CLIENT_NAMES.STREAMERBOT
//     "timestamp": <unix ms>,          // DateTimeOffset.UtcNow
//     "payload":   { ... }             // caller-supplied JSON string, inlined as-is
//   }
// =============================================================================

public class CPHInline
{
    // ── Constants — copy these into any script that uses PublishBrokerMessage ──

    // Index of the broker entry in Streamer.bot → Servers/Clients → WebSocket Clients.
    // First entry in the list = 0.
    private const int    BROKER_WS_INDEX      = 0;
    private const string VAR_BROKER_CONNECTED = "broker_connected";
    private const string BROKER_CLIENT_NAME   = "streamerbot";

    // How long (ms) to wait after a reconnect attempt before checking status.
    private const int WAIT_RECONNECT_MS = 600;
    // How long (ms) to wait after re-sending ClientHello on a reconnect.
    private const int WAIT_HELLO_MS = 200;

    // ── Execute — not used when copied as a helper; acts as a usage example ──

    /*
     * Purpose:
     * - Canonical reference for the PublishBrokerMessage helper method.
     * - Copy the constants block and PublishBrokerMessage into any CPHInline
     *   class that needs to send overlay commands.
     *
     * Expected trigger/input:
     * - Not triggered directly. Copy the method into other actions.
     *
     * Required runtime variables:
     * - broker_connected (non-persisted) — set by broker-connect.cs.
     * - WebSocket client index 0 configured in Streamer.bot UI.
     */
    public bool Execute()
    {
        // This action is a template only. When copied into a real action,
        // replace this Execute() body with the actual feature logic.
        CPH.LogWarn("[BrokerPublish] Template action executed directly. Copy this method into your feature script.");
        return true;
    }

    // ── PublishBrokerMessage — COPY THIS METHOD INTO ANY PUBLISHING SCRIPT ───
    //
    // Parameters:
    //   topic       — dot-notation topic string, e.g. "overlay.spawn"
    //                 Use the string constants from @stream-overlay/shared/topics.ts.
    //   payloadJson — the payload object already serialized to a JSON string.
    //                 Build this with string concatenation or the SerializeJson
    //                 helper from Actions/Helpers/json-no-external-libraries.md.
    //
    // Returns true if the message was sent, false if the connection is unavailable.
    //
    // Auto-reconnect: if the socket is down, this method attempts one reconnect
    // and re-sends ClientHello before publishing. If the reconnect fails, it logs
    // an error and returns false without crashing the calling action.
    private bool PublishBrokerMessage(string topic, string payloadJson)
    {
        const string LOG_PREFIX = "[BrokerPublish]";

        // ── Guard: null/empty inputs ──────────────────────────────────────────
        if (string.IsNullOrWhiteSpace(topic))
        {
            CPH.LogWarn($"{LOG_PREFIX} Topic is null or empty. Message not sent.");
            return false;
        }
        if (string.IsNullOrWhiteSpace(payloadJson))
        {
            CPH.LogWarn($"{LOG_PREFIX} Payload JSON is null or empty for topic '{topic}'. Message not sent.");
            return false;
        }

        // ── Auto-reconnect if connection dropped ──────────────────────────────
        // Check live socket state, not just our global flag (the flag can lag
        // if the connection dropped without a clean disconnect event).
        if (!CPH.WebsocketIsConnected(BROKER_WS_INDEX))
        {
            CPH.LogWarn($"{LOG_PREFIX} Not connected. Attempting reconnect for topic '{topic}'...");
            CPH.WebsocketConnect(BROKER_WS_INDEX);
            CPH.Wait(WAIT_RECONNECT_MS);

            if (!CPH.WebsocketIsConnected(BROKER_WS_INDEX))
            {
                CPH.LogError(
                    $"{LOG_PREFIX} Reconnect failed. Message for topic '{topic}' dropped. " +
                    "Check that the broker is running and retry broker-connect.cs."
                );
                CPH.SetGlobalVar(VAR_BROKER_CONNECTED, false, false);
                return false;
            }

            // Re-send ClientHello — the broker requires the handshake on every new connection.
            string hello =
                "{\"type\":\"client.hello\"," +
                "\"name\":\"" + BROKER_CLIENT_NAME + "\"," +
                "\"subscriptions\":[]}";
            CPH.WebsocketSend(hello, BROKER_WS_INDEX);
            CPH.Wait(WAIT_HELLO_MS);

            CPH.SetGlobalVar(VAR_BROKER_CONNECTED, true, false);
            CPH.LogWarn($"{LOG_PREFIX} Reconnected and handshake re-sent.");
        }

        // ── Build BrokerMessage envelope ──────────────────────────────────────
        // Fields: id (UUID), topic, sender (fixed "streamerbot"), timestamp (unix ms), payload.
        // payloadJson is inlined directly — no extra escaping needed because it is
        // already valid JSON. Do not wrap it in quotes.
        string id = Guid.NewGuid().ToString();
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        string message =
            "{\"id\":\"" + id + "\"" +
            ",\"topic\":\"" + topic + "\"" +
            ",\"sender\":\"" + BROKER_CLIENT_NAME + "\"" +
            ",\"timestamp\":" + timestamp +
            ",\"payload\":" + payloadJson +
            "}";

        // ── Send ──────────────────────────────────────────────────────────────
        CPH.WebsocketSend(message, BROKER_WS_INDEX);
        CPH.LogWarn($"{LOG_PREFIX} Sent topic={topic} id={id}");
        return true;
    }
}
