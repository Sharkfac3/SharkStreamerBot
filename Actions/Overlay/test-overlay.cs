// ACTION-CONTRACT: Actions/Overlay/AGENTS.md#test-overlay.cs
// ACTION-CONTRACT-SHA256: cb0b35783a7cf89b2abbc9beca40732f034acb82950598ef25d267c756162f57

using System;

public class CPHInline
{
    // SYNC CONSTANTS (Overlay / Broker feature)
    // Keep these names identical across all broker/overlay scripts.
    // See Actions/SHARED-CONSTANTS.md → Overlay / Broker section.
    private const int    BROKER_WS_INDEX      = 0;
    private const string VAR_BROKER_CONNECTED = "broker_connected";
    private const string BROKER_CLIENT_NAME   = "streamerbot";

    // Reconnect timing — copied from broker-publish.cs reference template.
    private const int WAIT_RECONNECT_MS = 600;
    private const int WAIT_HELLO_MS     = 200;

    // Test asset configuration.
    // Operator: drop a PNG or JPG into:
    //   Apps/stream-overlay/packages/overlay/public/images/test-overlay-ping.png
    // Any image works — the goal is to confirm the full pipeline, not to look good.
    private const string TEST_ASSET_ID  = "test-overlay-ping";
    private const string TEST_ASSET_SRC = "images/test-overlay-ping.png";

    // Center of the 1920×1080 overlay canvas.
    private const int TEST_ASSET_X     = 960;
    private const int TEST_ASSET_Y     = 540;
    private const int TEST_ASSET_WIDTH = 300;
    private const int TEST_ASSET_DEPTH = 50; // high depth so it renders on top of everything

    // How long the test image stays visible before being removed.
    private const int WAIT_VISIBLE_MS = 3000;

    // Topic strings — must match TOPICS constants in @stream-overlay/shared/topics.ts.
    private const string TOPIC_OVERLAY_SPAWN  = "overlay.spawn";
    private const string TOPIC_OVERLAY_REMOVE = "overlay.remove";

    /*
     * Purpose:
     * - End-to-end integration test for the Streamer.bot → broker → overlay pipeline.
     * - Spawns a test image at the center of the overlay canvas, waits 3 seconds,
     *   then removes it. Sends a chat confirmation when done.
     *
     * Expected trigger/input:
     * - Chat command: !testoverlay
     * - Restricted to moderators/operator (configure in Streamer.bot command settings).
     *
     * Required runtime variables:
     * - broker_connected (non-persisted) — set by broker-connect.cs.
     * - WebSocket client index 0 configured in Streamer.bot UI.
     *
     * Key outputs/side effects:
     * - Publishes overlay.spawn to the broker → image appears on screen.
     * - Waits 3 seconds.
     * - Publishes overlay.remove → image disappears.
     * - Sends a chat message confirming the test ran.
     *
     * Operator notes:
     * - The broker must be running and connected (run broker-connect.cs first).
     * - OBS must have the browser source for the overlay visible in the active scene.
     * - The test image must exist at:
     *     Apps/stream-overlay/packages/overlay/public/images/test-overlay-ping.png
     *   Drop any PNG/JPG there to make it work. The overlay serves files from public/.
     * - If nothing appears on screen, check:
     *     1. Is the broker running? (http://localhost:8765/health)
     *     2. Is the overlay loaded in OBS? (browser source pointing at overlay URL)
     *     3. Does the test image file exist in overlay/public/images/?
     *     4. Streamer.bot log — did PublishBrokerMessage log "Sent topic=overlay.spawn"?
     */
    public bool Execute()
    {
        const string LOG_PREFIX = "[TestOverlay]";

        // ── Broker connection guard ───────────────────────────────────────────
        // PublishBrokerMessage will auto-reconnect if needed, but we log here
        // so the operator has a clear signal before anything is sent.
        bool connected = (CPH.GetGlobalVar<bool?>(VAR_BROKER_CONNECTED, false) ?? false);
        if (!connected)
        {
            CPH.LogWarn($"{LOG_PREFIX} broker_connected is false. Will attempt reconnect on publish.");
        }

        CPH.LogWarn($"{LOG_PREFIX} Running overlay pipeline test...");

        // ── Build overlay.spawn payload ───────────────────────────────────────
        // Spawns the test image at center screen with a fade-in entry.
        // No lifetime set — we remove it explicitly to prove the remove path works.
        // Shape: OverlaySpawnPayload from @stream-overlay/shared/protocol.ts.
        string spawnPayload =
            "{" +
            "\"assetId\":\"" + TEST_ASSET_ID + "\"," +
            "\"src\":\"" + TEST_ASSET_SRC + "\"," +
            "\"position\":{\"x\":" + TEST_ASSET_X + ",\"y\":" + TEST_ASSET_Y + "}," +
            "\"width\":" + TEST_ASSET_WIDTH + "," +
            "\"depth\":" + TEST_ASSET_DEPTH + "," +
            "\"enterAnimation\":\"fade-in\"," +
            "\"enterDuration\":500" +
            "}";

        // ── Publish overlay.spawn ─────────────────────────────────────────────
        bool spawnSent = PublishBrokerMessage(TOPIC_OVERLAY_SPAWN, spawnPayload);
        if (!spawnSent)
        {
            CPH.LogError($"{LOG_PREFIX} Failed to send overlay.spawn. Aborting test.");
            CPH.SendMessage("⚠ Overlay test failed — broker not reachable. Check the log.");
            return true;
        }

        CPH.LogWarn($"{LOG_PREFIX} overlay.spawn sent. Waiting {WAIT_VISIBLE_MS}ms...");

        // ── Hold for WAIT_VISIBLE_MS ──────────────────────────────────────────
        // The image should be visible on screen during this window.
        CPH.Wait(WAIT_VISIBLE_MS);

        // ── Build overlay.remove payload ──────────────────────────────────────
        // Removes the test asset by ID with a fade-out exit animation.
        // Shape: OverlayRemovePayload from @stream-overlay/shared/protocol.ts.
        string removePayload =
            "{" +
            "\"assetId\":\"" + TEST_ASSET_ID + "\"," +
            "\"exitAnimation\":\"fade-out\"," +
            "\"exitDuration\":500" +
            "}";

        // ── Publish overlay.remove ────────────────────────────────────────────
        bool removeSent = PublishBrokerMessage(TOPIC_OVERLAY_REMOVE, removePayload);
        if (!removeSent)
        {
            CPH.LogError($"{LOG_PREFIX} Failed to send overlay.remove. Asset may be stuck on screen.");
            CPH.SendMessage("⚠ Overlay test: image appeared but remove failed. Check the log.");
            return true;
        }

        CPH.LogWarn($"{LOG_PREFIX} overlay.remove sent. Test complete.");

        // ── Chat confirmation ─────────────────────────────────────────────────
        CPH.SendMessage("✅ Overlay test complete — if you saw the image appear and disappear, the pipeline works.");
        return true;
    }

    // ── PublishBrokerMessage ─────────────────────────────────────────────────
    // Copied from Actions/Overlay/broker-publish.cs (reference template).
    // Wraps payloadJson in a BrokerMessage envelope and sends it to the broker.
    // Auto-reconnects once if the WebSocket is down.
    // Returns true if sent, false if the connection could not be established.
    private bool PublishBrokerMessage(string topic, string payloadJson)
    {
        const string LOG_PREFIX = "[BrokerPublish]";

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

        // Auto-reconnect if the socket is down.
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

            // Re-send ClientHello — required on every new connection.
            string hello =
                "{\"type\":\"client.hello\"," +
                "\"name\":\"" + BROKER_CLIENT_NAME + "\"," +
                "\"subscriptions\":[]}";
            CPH.WebsocketSend(hello, BROKER_WS_INDEX);
            CPH.Wait(WAIT_HELLO_MS);

            CPH.SetGlobalVar(VAR_BROKER_CONNECTED, true, false);
            CPH.LogWarn($"{LOG_PREFIX} Reconnected and handshake re-sent.");
        }

        // Build BrokerMessage envelope.
        // id: UUID v4 via Guid.NewGuid(). timestamp: Unix epoch ms.
        string id = Guid.NewGuid().ToString();
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        string message =
            "{\"id\":\"" + id + "\"" +
            ",\"topic\":\"" + topic + "\"" +
            ",\"sender\":\"" + BROKER_CLIENT_NAME + "\"" +
            ",\"timestamp\":" + timestamp +
            ",\"payload\":" + payloadJson +
            "}";

        CPH.WebsocketSend(message, BROKER_WS_INDEX);
        CPH.LogWarn($"{LOG_PREFIX} Sent topic={topic} id={id}");
        return true;
    }
}
