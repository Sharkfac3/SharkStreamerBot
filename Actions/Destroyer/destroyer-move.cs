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

    // Topic strings — must match TOPICS constants in @stream-overlay/shared/topics.ts.
    private const string TOPIC_OVERLAY_MOVE = "overlay.move";

    // Destroyer movement configuration.
    // See Actions/SHARED-CONSTANTS.md → Destroyer section.
    private const string DESTROYER_ASSET_ID       = "destroyer";
    private const int    DESTROYER_STEP            = 50;
    private const int    DESTROYER_TWEEN_MS        = 150;
    private const int    DESTROYER_HALF_SIZE       = 100; // half of 200px — used for boundary clamping
    private const int    CANVAS_WIDTH              = 1920;
    private const int    CANVAS_HEIGHT             = 1080;
    private const string VAR_DESTROYER_ACTIVE      = "destroyer_active";
    private const string VAR_DESTROYER_X           = "destroyer_x";
    private const string VAR_DESTROYER_Y           = "destroyer_y";
    private const string VAR_DESTROYER_EXPIRE_UTC  = "destroyer_expire_utc";

    /*
     * Purpose:
     * - Moves the destroyer image one step in the requested direction.
     * - Image cannot move off screen (clamped to canvas bounds accounting for image size).
     * - Silently ignored when the destroyer is not on screen.
     *
     * Expected trigger/input:
     * - Chat commands: !up / !down / !left / !right
     * - Open to all viewers (no permission restriction).
     * - All four commands should point to this single action in Streamer.bot.
     *   The action reads %command% to determine direction.
     *
     * Required runtime variables:
     * - destroyer_active (non-persisted bool) — set by destroyer-spawn.cs.
     * - destroyer_x, destroyer_y (non-persisted int) — current canvas position.
     * - destroyer_expire_utc (non-persisted long) — unix ms when auto-despawn occurs.
     * - broker_connected (non-persisted bool) — set by broker-connect.cs.
     *
     * Key outputs/side effects:
     * - Publishes overlay.move → destroyer tweens 50px in the commanded direction.
     * - Updates destroyer_x / destroyer_y globals with the new clamped position.
     *
     * Operator notes:
     * - Pair this action with destroyer-spawn.cs (triggered by !destroyer).
     * - Boundary min/max derived from canvas size (1920×1080) and image half-size (100px).
     *   Adjust DESTROYER_HALF_SIZE if you change the image dimensions in destroyer-spawn.cs.
     */
    public bool Execute()
    {
        const string LOG_PREFIX = "[DestroyerMove]";

        // ── Active guard ──────────────────────────────────────────────────────
        // Drop move silently if destroyer isn't on screen or has expired.
        bool active = (CPH.GetGlobalVar<bool?>(VAR_DESTROYER_ACTIVE, false) ?? false);
        if (!active)
        {
            return true;
        }

        long expireUtc = CPH.GetGlobalVar<long>(VAR_DESTROYER_EXPIRE_UTC, false);
        long nowUtc    = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (nowUtc >= expireUtc)
        {
            // Lifetime elapsed — clear the active flag so spawn guard resets.
            CPH.SetGlobalVar(VAR_DESTROYER_ACTIVE, false, false);
            return true;
        }

        // ── Read direction from command arg ───────────────────────────────────
        // Streamer.bot populates %command% with the matched command (e.g. "!up").
        if (!CPH.TryGetArg("command", out string command))
        {
            CPH.LogWarn($"{LOG_PREFIX} No 'command' arg found. Cannot determine direction.");
            return true;
        }

        int dx = 0;
        int dy = 0;

        switch (command.ToLowerInvariant())
        {
            case "!up":    dy = -DESTROYER_STEP; break;
            case "!down":  dy =  DESTROYER_STEP; break;
            case "!left":  dx = -DESTROYER_STEP; break;
            case "!right": dx =  DESTROYER_STEP; break;
            default:
                CPH.LogWarn($"{LOG_PREFIX} Unrecognized command '{command}'. Expected !up/!down/!left/!right.");
                return true;
        }

        // ── Read current position ─────────────────────────────────────────────
        int currentX = CPH.GetGlobalVar<int>(VAR_DESTROYER_X, false);
        int currentY = CPH.GetGlobalVar<int>(VAR_DESTROYER_Y, false);

        // ── Calculate and clamp new position ─────────────────────────────────
        // Phaser Image origin = center (0.5, 0.5) by default.
        // Canvas edges: image center must stay at least HALF_SIZE from each edge.
        int minX = DESTROYER_HALF_SIZE;
        int maxX = CANVAS_WIDTH  - DESTROYER_HALF_SIZE;
        int minY = DESTROYER_HALF_SIZE;
        int maxY = CANVAS_HEIGHT - DESTROYER_HALF_SIZE;

        int newX = Math.Max(minX, Math.Min(maxX, currentX + dx));
        int newY = Math.Max(minY, Math.Min(maxY, currentY + dy));

        // ── Build overlay.move payload ────────────────────────────────────────
        string movePayload =
            "{" +
            "\"assetId\":\"" + DESTROYER_ASSET_ID + "\"," +
            "\"position\":{\"x\":" + newX + ",\"y\":" + newY + "}," +
            "\"duration\":" + DESTROYER_TWEEN_MS +
            "}";

        bool sent = PublishBrokerMessage(TOPIC_OVERLAY_MOVE, movePayload);
        if (!sent)
        {
            CPH.LogError($"{LOG_PREFIX} Failed to send overlay.move. Broker unreachable.");
            return true;
        }

        // ── Save updated position ─────────────────────────────────────────────
        CPH.SetGlobalVar(VAR_DESTROYER_X, newX, false);
        CPH.SetGlobalVar(VAR_DESTROYER_Y, newY, false);

        CPH.LogWarn($"{LOG_PREFIX} Moved {command} → ({newX}, {newY})");
        return true;
    }

    // ── PublishBrokerMessage ─────────────────────────────────────────────────
    // Copied from Actions/Overlay/broker-publish.cs (reference template).
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

            string hello =
                "{\"type\":\"client.hello\"," +
                "\"name\":\"" + BROKER_CLIENT_NAME + "\"," +
                "\"subscriptions\":[]}";
            CPH.WebsocketSend(hello, BROKER_WS_INDEX);
            CPH.Wait(WAIT_HELLO_MS);

            CPH.SetGlobalVar(VAR_BROKER_CONNECTED, true, false);
            CPH.LogWarn($"{LOG_PREFIX} Reconnected and handshake re-sent.");
        }

        string id        = Guid.NewGuid().ToString();
        long   timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

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
