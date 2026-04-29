// ACTION-CONTRACT: Actions/XJ Drivethrough/AGENTS.md#xj-drivethrough-main.cs
// ACTION-CONTRACT-SHA256: 7772c80676456d7f0732f0862695d9f9974bed90af108ffbd585f8605424de75

using System;

// =============================================================================
// xj-drivethrough-main.cs
//
// Purpose:
//   Rolls a 1-100 chance gate. On rolls above 85, drives a Jeep Cherokee XJ image
//   across the overlay from left to right over 10 seconds, simultaneously playing
//   a rev limiter sound effect.
//
// Expected trigger/input:
//   - Any trigger is fine: chat command, channel point, manual button, etc.
//   - No input arguments are read.
//
// Required runtime variables:
//   - xj_drivethrough_active (non-persisted bool) — re-entry guard managed here.
//   - broker_connected (non-persisted bool) — set by broker-connect.cs.
//   - WebSocket client index 0 must be configured in Streamer.bot UI.
//
// Key outputs/side effects:
//   - Publishes overlay.spawn → XJ image appears off-screen left.
//   - Publishes overlay.move  → XJ tweens to off-screen right over 10 seconds.
//   - Publishes overlay.audio.play → rev limiter sound fires at the same time.
//   - After 10 seconds: overlay.audio.stop + overlay.remove to clean up.
//
// Operator notes:
//   - Image file must exist at:
//       Apps/stream-overlay/packages/overlay/public/images/xj-drivethrough.png
//     Use a side-facing XJ image (facing right). Recommended size: 400×250 px.
//   - Audio file must exist at:
//       Apps/stream-overlay/packages/overlay/public/audio/xj-rev-limiter.mp3
//   - OBS browser source must have audio enabled for the sound to hit stream output.
//   - See Actions/SHARED-CONSTANTS.md → XJ Drivethrough section for all name constants.
// =============================================================================

public class CPHInline
{
    // ── Broker constants — identical in all broker/overlay scripts ─────────
    // See Actions/SHARED-CONSTANTS.md → Overlay / Broker section.
    private const int    BROKER_WS_INDEX      = 0;
    private const string VAR_BROKER_CONNECTED = "broker_connected";
    private const string BROKER_CLIENT_NAME   = "streamerbot";
    private const int    WAIT_RECONNECT_MS    = 600;
    private const int    WAIT_HELLO_MS        = 200;

    // ── Topic strings — must match TOPICS in @stream-overlay/shared/topics.ts ──
    private const string TOPIC_OVERLAY_SPAWN      = "overlay.spawn";
    private const string TOPIC_OVERLAY_MOVE       = "overlay.move";
    private const string TOPIC_OVERLAY_REMOVE     = "overlay.remove";
    private const string TOPIC_OVERLAY_AUDIO_PLAY = "overlay.audio.play";
    private const string TOPIC_OVERLAY_AUDIO_STOP = "overlay.audio.stop";

    // ── XJ Drivethrough constants ──────────────────────────────────────────
    // See Actions/SHARED-CONSTANTS.md → XJ Drivethrough section.
    private const string XJ_ASSET_ID           = "xj-drivethrough";
    private const string XJ_ASSET_SRC          = "images/xj-drivethrough.png";
    private const string XJ_SOUND_ID           = "xj-rev-limiter";
    private const string XJ_SOUND_SRC          = "audio/xj-rev-limiter.mp3";
    private const int    XJ_WIDTH              = 400;   // pixels
    private const int    XJ_HEIGHT             = 250;   // pixels
    private const int    XJ_DEPTH              = 20;    // render above most overlay content
    private const int    XJ_Y                  = 750;   // lower-third of 1920×1080 canvas

    // Start and end X positions use image center-origin (same as Phaser default).
    // XJ_START_X = -200 → center is 200px off the left edge; right edge sits at x=0 (canvas border).
    // XJ_END_X   = 2120 → center is 200px off the right edge; left edge sits at x=1920 (canvas border).
    // This means the Jeep enters from off-screen left and exits off-screen right — fully across.
    private const int    XJ_START_X            = -200;
    private const int    XJ_END_X              = 2120;

    // How long the drive takes in milliseconds (10 seconds).
    private const int    XJ_DRIVE_DURATION_MS  = 10000;

    // Re-entry guard variable name.
    private const string VAR_XJ_ACTIVE = "xj_drivethrough_active";

    // Chance gate: only rolls 86-100 trigger the overlay/audio sequence.
    private const int XJ_CHANCE_MIN = 1;
    private const int XJ_CHANCE_MAX_EXCLUSIVE = 101;
    private const int XJ_TRIGGER_THRESHOLD = 85;
    private static readonly Random ChanceRandom = new Random();

    // Delay after spawn before sending the move, so the overlay has time to load
    // and register the image before we try to tween it. The overlay also queues
    // early move commands, but this keeps the normal path simple and reliable.
    private const int WAIT_SPAWN_SETTLE_MS = 750;

    // Buffer after the drive tween before sending remove, to let the tween fully finish.
    private const int WAIT_POST_DRIVE_MS = 500;

    public bool Execute()
    {
        const string LOG_PREFIX = "[XJDrivethrough]";

        // ── Chance gate ───────────────────────────────────────────────────────
        // Roll 1-100 inclusive. Only rolls above 85 continue into the overlay/audio sequence.
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

        // ── Re-entry guard ────────────────────────────────────────────────────
        // If a drivethrough is already running, silently drop this request.
        // This prevents two simultaneous sequences from fighting over the same asset ID.
        bool alreadyRunning = (CPH.GetGlobalVar<bool?>(VAR_XJ_ACTIVE, false) ?? false);
        if (alreadyRunning)
        {
            CPH.LogWarn($"{LOG_PREFIX} Drivethrough already in progress. Ignoring duplicate request.");
            return true;
        }

        // Claim the active slot before doing any async work.
        CPH.SetGlobalVar(VAR_XJ_ACTIVE, true, false);
        CPH.LogWarn($"{LOG_PREFIX} Starting XJ drivethrough sequence...");

        try
        {
            // ── Step 1: Spawn XJ off-screen left ─────────────────────────────
            // No enter animation — the Jeep just appears at the edge and immediately
            // starts moving. The movement itself is the visual "entrance."
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
                return true; // finally block will release the guard
            }

            // Give the overlay a moment to register the asset before we send the move.
            // Without this, the move might arrive before the spawn is processed.
            CPH.Wait(WAIT_SPAWN_SETTLE_MS);

            // ── Step 2: Drive XJ across to the right (10 seconds) ────────────
            // The overlay engine tweens the image from XJ_START_X to XJ_END_X
            // using Phaser's built-in tween system. Duration = 10000ms.
            string movePayload =
                "{" +
                "\"assetId\":\"" + XJ_ASSET_ID + "\"," +
                "\"position\":{\"x\":" + XJ_END_X + ",\"y\":" + XJ_Y + "}," +
                "\"duration\":" + XJ_DRIVE_DURATION_MS +
                "}";

            PublishBrokerMessage(TOPIC_OVERLAY_MOVE, movePayload);

            // ── Step 3: Play rev limiter sound ────────────────────────────────
            // Fired right after the move command so audio and motion start together.
            // loop: false = plays once. If the audio file is shorter than 10 seconds,
            // it will finish naturally. If longer, we stop it after the drive completes.
            string audioPlayPayload =
                "{" +
                "\"soundId\":\"" + XJ_SOUND_ID + "\"," +
                "\"src\":\"" + XJ_SOUND_SRC + "\"," +
                "\"volume\":1.0," +
                "\"loop\":false" +
                "}";

            PublishBrokerMessage(TOPIC_OVERLAY_AUDIO_PLAY, audioPlayPayload);
            CPH.LogWarn($"{LOG_PREFIX} Move and audio fired. Waiting {XJ_DRIVE_DURATION_MS}ms for drive to finish...");

            // ── Step 4: Wait for the drive to complete ────────────────────────
            // Total wait = drive duration + small post-drive buffer.
            CPH.Wait(XJ_DRIVE_DURATION_MS + WAIT_POST_DRIVE_MS);

            // ── Step 5: Stop audio ────────────────────────────────────────────
            // Stops the rev limiter sound if it is still playing (e.g., if the
            // audio file is longer than 10 seconds, or if loop was accidentally set).
            string audioStopPayload =
                "{\"soundId\":\"" + XJ_SOUND_ID + "\"}";

            PublishBrokerMessage(TOPIC_OVERLAY_AUDIO_STOP, audioStopPayload);

            // ── Step 6: Remove the XJ image ───────────────────────────────────
            // No exit animation — the image is already off-screen, so no need for a fade.
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
            // Always release the guard, even if something throws mid-sequence.
            // This ensures a future trigger can run after a partial failure.
            CPH.SetGlobalVar(VAR_XJ_ACTIVE, false, false);
        }

        return true;
    }

    // ── PublishBrokerMessage ─────────────────────────────────────────────────
    // Copied from Actions/Overlay/broker-publish.cs (reference template).
    // Wraps payloadJson in a BrokerMessage envelope and sends it to the broker.
    // Auto-reconnects once if the WebSocket is down.
    // Returns true if the message was sent; false if the connection failed.
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

        // Build the BrokerMessage envelope. payloadJson is inlined directly
        // (already valid JSON — no extra escaping needed).
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
