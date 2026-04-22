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
    private const string TOPIC_OVERLAY_SPAWN = "overlay.spawn";

    // Destroyer asset configuration.
    // See Actions/SHARED-CONSTANTS.md → Destroyer section.
    private const string DESTROYER_ASSET_ID       = "destroyer";
    private const string DESTROYER_ASSET_SRC      = "images/destroyer.jpg";
    private const int    DESTROYER_START_X        = 960;
    private const int    DESTROYER_START_Y        = 540;
    private const int    DESTROYER_SIZE           = 200;
    private const int    DESTROYER_DEPTH          = 10;
    private const int    DESTROYER_LIFETIME_MS    = 300000; // 5 minutes
    private const string VAR_DESTROYER_ACTIVE     = "destroyer_active";
    private const string VAR_DESTROYER_X          = "destroyer_x";
    private const string VAR_DESTROYER_Y          = "destroyer_y";
    private const string VAR_DESTROYER_EXPIRE_UTC = "destroyer_expire_utc";

    /*
     * Purpose:
     * - Allows any chat viewer to summon the destroyer image onto the overlay.
     * - Image spawns at center screen and auto-despawns after 5 minutes.
     * - Re-entry guarded: !destroyer is ignored while the image is already on screen.
     *
     * Expected trigger/input:
     * - Chat command: !destroyer
     * - Open to all viewers (no permission restriction).
     *
     * Required runtime variables:
     * - broker_connected (non-persisted) — set by broker-connect.cs.
     * - WebSocket client index 0 configured in Streamer.bot UI.
     *
     * Key outputs/side effects:
     * - Publishes overlay.spawn → destroyer image appears at center, fades in.
     * - overlay auto-removes image after 5 minutes (lifetime field).
     * - Sets destroyer_active, destroyer_x, destroyer_y, destroyer_expire_utc globals.
     *
     * Operator notes:
     * - destroyer.jpg must exist at:
     *     Apps/stream-overlay/packages/overlay/public/images/destroyer.jpg
     * - Pair this action with destroyer-move.cs (triggered by !up/!down/!left/!right).
     */
    public bool Execute()
    {
        const string LOG_PREFIX = "[DestroyerSpawn]";

        // ── Re-entry guard ────────────────────────────────────────────────────
        // If destroyer is already on screen and hasn't expired, reject the spawn.
        bool active = (CPH.GetGlobalVar<bool?>(VAR_DESTROYER_ACTIVE, false) ?? false);
        if (active)
        {
            long expireUtc = CPH.GetGlobalVar<long>(VAR_DESTROYER_EXPIRE_UTC, false);
            long nowUtc    = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (nowUtc < expireUtc)
            {
                CPH.LogWarn($"{LOG_PREFIX} Destroyer already on screen. Ignoring spawn request.");
                return true;
            }
            // Expired — allow re-spawn (overlay already removed it via lifetime).
        }

        CPH.LogWarn($"{LOG_PREFIX} Spawning destroyer at center screen...");

        // ── Build overlay.spawn payload ───────────────────────────────────────
        // lifetime = DESTROYER_LIFETIME_MS — overlay auto-removes after 5 minutes.
        // exitAnimation = fade-out so it disappears cleanly when lifetime expires.
        string spawnPayload =
            "{" +
            "\"assetId\":\"" + DESTROYER_ASSET_ID + "\"," +
            "\"src\":\"" + DESTROYER_ASSET_SRC + "\"," +
            "\"position\":{\"x\":" + DESTROYER_START_X + ",\"y\":" + DESTROYER_START_Y + "}," +
            "\"width\":" + DESTROYER_SIZE + "," +
            "\"height\":" + DESTROYER_SIZE + "," +
            "\"depth\":" + DESTROYER_DEPTH + "," +
            "\"enterAnimation\":\"fade-in\"," +
            "\"enterDuration\":500," +
            "\"lifetime\":" + DESTROYER_LIFETIME_MS + "," +
            "\"exitAnimation\":\"fade-out\"," +
            "\"exitDuration\":500" +
            "}";

        bool sent = PublishBrokerMessage(TOPIC_OVERLAY_SPAWN, spawnPayload);
        if (!sent)
        {
            CPH.LogError($"{LOG_PREFIX} Failed to send overlay.spawn. Broker unreachable.");
            return true;
        }

        // ── Record spawn state ────────────────────────────────────────────────
        long expireAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + DESTROYER_LIFETIME_MS;
        CPH.SetGlobalVar(VAR_DESTROYER_ACTIVE,     true,              false);
        CPH.SetGlobalVar(VAR_DESTROYER_X,          DESTROYER_START_X, false);
        CPH.SetGlobalVar(VAR_DESTROYER_Y,          DESTROYER_START_Y, false);
        CPH.SetGlobalVar(VAR_DESTROYER_EXPIRE_UTC, expireAt,          false);

        CPH.LogWarn($"{LOG_PREFIX} Destroyer spawned. Expires at UTC ms={expireAt}.");
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
