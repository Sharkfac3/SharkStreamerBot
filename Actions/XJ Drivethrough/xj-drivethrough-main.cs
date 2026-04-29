// ACTION-CONTRACT: Actions/XJ Drivethrough/AGENTS.md#xj-drivethrough-main.cs
// ACTION-CONTRACT-SHA256: 7772c80676456d7f0732f0862695d9f9974bed90af108ffbd585f8605424de75

using System;

// Runtime source of truth: Actions/XJ Drivethrough/AGENTS.md#Action-Contracts
// Shared names/constants reference: Actions/SHARED-CONSTANTS.md#xj-drivethrough

public class CPHInline
{
    // Broker constants; keep aligned with Actions/SHARED-CONSTANTS.md.
    private const int    BROKER_WS_INDEX      = 0;
    private const string VAR_BROKER_CONNECTED = "broker_connected";
    private const string BROKER_CLIENT_NAME   = "streamerbot";
    private const int    WAIT_RECONNECT_MS    = 600;
    private const int    WAIT_HELLO_MS        = 200;

    // Overlay broker topics.
    private const string TOPIC_OVERLAY_SPAWN      = "overlay.spawn";
    private const string TOPIC_OVERLAY_MOVE       = "overlay.move";
    private const string TOPIC_OVERLAY_REMOVE     = "overlay.remove";
    private const string TOPIC_OVERLAY_AUDIO_PLAY = "overlay.audio.play";
    private const string TOPIC_OVERLAY_AUDIO_STOP = "overlay.audio.stop";

    // XJ Drivethrough constants; behavior is defined by the action contract.
    private const string XJ_ASSET_ID           = "xj-drivethrough";
    private const string XJ_ASSET_SRC          = "images/xj-drivethrough.png";
    private const string XJ_SOUND_ID           = "xj-rev-limiter";
    private const string XJ_SOUND_SRC          = "audio/xj-rev-limiter.mp3";
    private const int    XJ_WIDTH              = 400;   // pixels
    private const int    XJ_HEIGHT             = 250;   // pixels
    private const int    XJ_DEPTH              = 20;    // render above most overlay content
    private const int    XJ_Y                  = 750;   // lower-third of 1920×1080 canvas

    // Center-origin positions that place the Jeep fully off-screen at start/end.
    private const int    XJ_START_X            = -200;
    private const int    XJ_END_X              = 2120;

    // Drive duration in milliseconds.
    private const int    XJ_DRIVE_DURATION_MS  = 10000;

    // Non-persisted re-entry guard.
    private const string VAR_XJ_ACTIVE = "xj_drivethrough_active";

    // Chance gate values from the action contract.
    private const int XJ_CHANCE_MIN = 1;
    private const int XJ_CHANCE_MAX_EXCLUSIVE = 101;
    private const int XJ_TRIGGER_THRESHOLD = 85;
    private static readonly Random ChanceRandom = new Random();

    // Publisher-side settle delay required by the action contract.
    private const int WAIT_SPAWN_SETTLE_MS = 750;

    // Cleanup buffer after the drive tween.
    private const int WAIT_POST_DRIVE_MS = 500;

    public bool Execute()
    {
        const string LOG_PREFIX = "[XJDrivethrough]";

        // Chance gate: see AGENTS.md action contract.
        int chanceRoll;
        lock (ChanceRandom)
        {
            chanceRoll = ChanceRandom.Next(XJ_CHANCE_MIN, XJ_CHANCE_MAX_EXCLUSIVE);
        }

        if (chanceRoll <= XJ_TRIGGER_THRESHOLD)
        {
            CPH.LogWarn($"{LOG_PREFIX} Chance roll {chanceRoll}/100 did not beat {XJ_TRIGGER_THRESHOLD}. No drivethrough queued.");
            return true;
        }

        CPH.LogWarn($"{LOG_PREFIX} Chance roll {chanceRoll}/100 beat {XJ_TRIGGER_THRESHOLD}. Queuing drivethrough.");

        // Re-entry guard: see AGENTS.md action contract.
        bool alreadyRunning = (CPH.GetGlobalVar<bool?>(VAR_XJ_ACTIVE, false) ?? false);
        if (alreadyRunning)
        {
            CPH.LogWarn($"{LOG_PREFIX} Drivethrough already in progress. Ignoring duplicate request.");
            return true;
        }

        // Claim the active slot before broker work.
        CPH.SetGlobalVar(VAR_XJ_ACTIVE, true, false);
        CPH.LogWarn($"{LOG_PREFIX} Starting XJ drivethrough sequence...");

        try
        {
            // Spawn XJ off-screen left.
            string spawnPayload =
                "{" +
                "\"assetId\":\"" + XJ_ASSET_ID + "\"," +
                "\"src\":\"" + XJ_ASSET_SRC + "\"," +
                "\"position\":{\"x\":" + XJ_START_X + ",\"y\":" + XJ_Y + "}," +
                "\"width\":" + XJ_WIDTH + "," +
                "\"height\":" + XJ_HEIGHT + "," +
                "\"depth\":" + XJ_DEPTH + "," +
                "\"enterAnimation\":\"none\"," +
                "\"enterDuration\":0" +
                "}";

            bool spawned = PublishBrokerMessage(TOPIC_OVERLAY_SPAWN, spawnPayload);
            if (!spawned)
            {
                CPH.LogError($"{LOG_PREFIX} Failed to spawn XJ image. Broker unreachable. Aborting.");
                return true; // finally releases the guard
            }

            // Let the overlay register the spawned asset before moving it.
            CPH.Wait(WAIT_SPAWN_SETTLE_MS);

            // Move XJ across the overlay.
            string movePayload =
                "{" +
                "\"assetId\":\"" + XJ_ASSET_ID + "\"," +
                "\"position\":{\"x\":" + XJ_END_X + ",\"y\":" + XJ_Y + "}," +
                "\"duration\":" + XJ_DRIVE_DURATION_MS +
                "}";

            PublishBrokerMessage(TOPIC_OVERLAY_MOVE, movePayload);

            // Play rev-limiter audio with the drive.
            string audioPlayPayload =
                "{" +
                "\"soundId\":\"" + XJ_SOUND_ID + "\"," +
                "\"src\":\"" + XJ_SOUND_SRC + "\"," +
                "\"volume\":1.0," +
                "\"loop\":false" +
                "}";

            PublishBrokerMessage(TOPIC_OVERLAY_AUDIO_PLAY, audioPlayPayload);
            CPH.LogWarn($"{LOG_PREFIX} Move and audio fired. Waiting {XJ_DRIVE_DURATION_MS}ms for drive to finish...");

            // Wait for the drive to complete.
            CPH.Wait(XJ_DRIVE_DURATION_MS + WAIT_POST_DRIVE_MS);

            // Stop audio if it is still playing.
            string audioStopPayload =
                "{\"soundId\":\"" + XJ_SOUND_ID + "\"}";

            PublishBrokerMessage(TOPIC_OVERLAY_AUDIO_STOP, audioStopPayload);

            // Remove the off-screen XJ image.
            string removePayload =
                "{" +
                "\"assetId\":\"" + XJ_ASSET_ID + "\"," +
                "\"exitAnimation\":\"none\"," +
                "\"exitDuration\":0" +
                "}";

            PublishBrokerMessage(TOPIC_OVERLAY_REMOVE, removePayload);
            CPH.LogWarn($"{LOG_PREFIX} XJ drivethrough complete. Image removed.");
        }
        finally
        {
            // Always release the guard after a claimed run.
            CPH.SetGlobalVar(VAR_XJ_ACTIVE, false, false);
        }

        return true;
    }

    // Broker publish helper; keep behavior aligned with the action contract and broker template.
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

            // Re-send ClientHello after reconnect.
            string hello =
                "{\"type\":\"client.hello\"," +
                "\"name\":\"" + BROKER_CLIENT_NAME + "\"," +
                "\"subscriptions\":[]}";
            CPH.WebsocketSend(hello, BROKER_WS_INDEX);
            CPH.Wait(WAIT_HELLO_MS);

            CPH.SetGlobalVar(VAR_BROKER_CONNECTED, true, false);
            CPH.LogWarn($"{LOG_PREFIX} Reconnected and handshake re-sent.");
        }

        // Build BrokerMessage envelope; payloadJson is already valid JSON.
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
