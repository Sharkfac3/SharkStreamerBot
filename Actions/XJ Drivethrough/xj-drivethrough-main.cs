// ACTION-CONTRACT: Actions/XJ Drivethrough/AGENTS.md#xj-drivethrough-main.cs
// ACTION-CONTRACT-SHA256: 437f251f7a493f09249451a4c3be925d450f72d6062dddf0caca153c1ddefc14

using System;
using System.Collections.Generic;

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
    private const int    XJ_WIDTH              = 400;
    private const int    XJ_HEIGHT             = 250;
    private const int    XJ_DEPTH              = 20;
    private const int    XJ_Y                  = 750;
    private const int    XJ_START_X            = -200;
    private const int    XJ_END_X              = 2120;
    private const int    XJ_DRIVE_DURATION_MS  = 10000;

    // Non-commander chance gate and timing.
    private const string VAR_XJ_ACTIVE = "xj_drivethrough_active";
    private const int XJ_CHANCE_MIN = 1;
    private const int XJ_CHANCE_MAX_EXCLUSIVE = 101;
    private const int XJ_TRIGGER_THRESHOLD = 20;
    private const int WAIT_SPAWN_SETTLE_MS = 750;
    private const int WAIT_POST_DRIVE_MS = 500;
    private static readonly Random ChanceRandom = new Random();

    // Commander slot globals.
    private const string VAR_CURRENT_WATER_WIZARD    = "current_water_wizard";
    private const string VAR_CURRENT_CAPTAIN_STRETCH = "current_captain_stretch";
    private const string VAR_CURRENT_THE_DIRECTOR    = "current_the_director";

    // Commander XJ piece assets.
    private const string ROLE_WATER_WIZARD    = "water_wizard";
    private const string ROLE_CAPTAIN_STRETCH = "captain_stretch";
    private const string ROLE_THE_DIRECTOR    = "the_director";
    private const string XJ_LEFT_ASSET_ID      = "xj-left";
    private const string XJ_CENTER_ASSET_ID    = "xj-center";
    private const string XJ_RIGHT_ASSET_ID     = "xj-right";
    private const string XJ_LEFT_SRC           = "images/xj-left.png";
    private const string XJ_CENTER_SRC         = "images/xj-middle.png";
    private const string XJ_RIGHT_SRC          = "images/xj-right.png";
    private const int XJ_COMMANDER_WIDTH       = 640;
    private const int XJ_COMMANDER_HEIGHT      = 250;
    private const int XJ_COMMANDER_LEFT_X      = 320;
    private const int XJ_COMMANDER_CENTER_X    = 960;
    private const int XJ_COMMANDER_RIGHT_X     = 1600;
    private const int XJ_COMMANDER_DISPLAY_MS  = 10000;

    // Commander triforce window state.
    private const string TIMER_XJ_COMMANDER_TRIFORCE_WINDOW = "XJ - Commander Triforce Window";
    private const string VAR_TRIFORCE_ACTIVE       = "xj_commander_triforce_active";
    private const string VAR_TRIFORCE_SEEN_JSON    = "xj_commander_triforce_seen_json";
    private const string VAR_TRIFORCE_DEADLINE_UTC = "xj_commander_triforce_deadline_utc";
    private const string VAR_TRIFORCE_COUNT        = "xj_commander_triforce_count";
    private const string VAR_TRIFORCE_HIGH_SCORE   = "xj_commander_triforce_high_score";
    private const int XJ_COMMANDER_TRIFORCE_WINDOW_MS = 5000;

    public bool Execute()
    {
        const string LOG_PREFIX = "[XJDrivethrough]";

        string user = GetTriggerUser();
        if (string.IsNullOrWhiteSpace(user))
        {
            HandleTriforceTimerIfExpired(LOG_PREFIX);
            return true;
        }

        CommanderPiece piece = GetCommanderPieceForUser(user);
        if (piece != null)
        {
            CPH.LogWarn($"{LOG_PREFIX} Active commander {user} triggered {piece.AssetId}.");
            return RunCommanderPiece(piece, LOG_PREFIX);
        }

        return RunNonCommanderDrivethrough(user, LOG_PREFIX);
    }

    private bool RunNonCommanderDrivethrough(string user, string logPrefix)
    {
        int chanceRoll;
        lock (ChanceRandom)
        {
            chanceRoll = ChanceRandom.Next(XJ_CHANCE_MIN, XJ_CHANCE_MAX_EXCLUSIVE);
        }

        CPH.SendMessage($"@{user} rolled {chanceRoll}/100 for !xj. Need over {XJ_TRIGGER_THRESHOLD}.");

        if (chanceRoll <= XJ_TRIGGER_THRESHOLD)
        {
            CPH.LogWarn($"{logPrefix} Chance roll {chanceRoll}/100 did not beat {XJ_TRIGGER_THRESHOLD}. No drivethrough queued.");
            return true;
        }

        CPH.LogWarn($"{logPrefix} Chance roll {chanceRoll}/100 beat {XJ_TRIGGER_THRESHOLD}. Queuing drivethrough.");

        bool alreadyRunning = (CPH.GetGlobalVar<bool?>(VAR_XJ_ACTIVE, false) ?? false);
        if (alreadyRunning)
        {
            CPH.LogWarn($"{logPrefix} Drivethrough already in progress. Ignoring duplicate request.");
            return true;
        }

        CPH.SetGlobalVar(VAR_XJ_ACTIVE, true, false);
        CPH.LogWarn($"{logPrefix} Starting XJ drivethrough sequence...");

        try
        {
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
                CPH.LogError($"{logPrefix} Failed to spawn XJ image. Broker unreachable. Aborting.");
                return true;
            }

            CPH.Wait(WAIT_SPAWN_SETTLE_MS);

            string movePayload =
                "{" +
                "\"assetId\":\"" + XJ_ASSET_ID + "\"," +
                "\"position\":{\"x\":" + XJ_END_X + ",\"y\":" + XJ_Y + "}," +
                "\"duration\":" + XJ_DRIVE_DURATION_MS +
                "}";
            PublishBrokerMessage(TOPIC_OVERLAY_MOVE, movePayload);

            PlayRevLimiter();
            CPH.LogWarn($"{logPrefix} Move and audio fired. Waiting {XJ_DRIVE_DURATION_MS}ms for drive to finish...");

            CPH.Wait(XJ_DRIVE_DURATION_MS + WAIT_POST_DRIVE_MS);
            StopRevLimiter();

            string removePayload =
                "{" +
                "\"assetId\":\"" + XJ_ASSET_ID + "\"," +
                "\"exitAnimation\":\"none\"," +
                "\"exitDuration\":0" +
                "}";
            PublishBrokerMessage(TOPIC_OVERLAY_REMOVE, removePayload);
            CPH.LogWarn($"{logPrefix} XJ drivethrough complete. Image removed.");
        }
        finally
        {
            CPH.SetGlobalVar(VAR_XJ_ACTIVE, false, false);
        }

        return true;
    }

    private bool RunCommanderPiece(CommanderPiece piece, string logPrefix)
    {
        try
        {
            HandleCommanderTriforce(piece.Role, logPrefix);

            string spawnPayload =
                "{" +
                "\"assetId\":\"" + piece.AssetId + "\"," +
                "\"src\":\"" + piece.Source + "\"," +
                "\"position\":{\"x\":" + piece.X + ",\"y\":" + XJ_Y + "}," +
                "\"width\":" + XJ_COMMANDER_WIDTH + "," +
                "\"height\":" + XJ_COMMANDER_HEIGHT + "," +
                "\"depth\":" + XJ_DEPTH + "," +
                "\"enterAnimation\":\"none\"," +
                "\"enterDuration\":0," +
                "\"lifetime\":" + XJ_COMMANDER_DISPLAY_MS + "," +
                "\"exitAnimation\":\"none\"," +
                "\"exitDuration\":0" +
                "}";
            if (!PublishBrokerMessage(TOPIC_OVERLAY_SPAWN, spawnPayload))
            {
                CPH.LogError($"{logPrefix} Failed to spawn commander XJ piece {piece.AssetId}.");
                return true;
            }

            CPH.LogWarn($"{logPrefix} Commander XJ piece {piece.AssetId} spawned with overlay lifetime {XJ_COMMANDER_DISPLAY_MS}ms.");
        }
        catch (Exception ex)
        {
            CPH.LogError($"{logPrefix} Commander XJ piece failed safely: {ex.Message}");
        }

        return true;
    }

    private void HandleCommanderTriforce(string role, string logPrefix)
    {
        bool active = (CPH.GetGlobalVar<bool?>(VAR_TRIFORCE_ACTIVE, false) ?? false);
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long deadline = CPH.GetGlobalVar<long?>(VAR_TRIFORCE_DEADLINE_UTC, false) ?? 0L;

        if (active && deadline > 0 && now > deadline)
        {
            ClearTriforceState();
            active = false;
            CPH.LogWarn($"{logPrefix} Commander triforce window expired before this command. Starting a new window.");
        }

        Dictionary<string, bool> seen = active ? ReadSeenMap() : new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        if (seen.ContainsKey(role))
        {
            CPH.LogWarn($"{logPrefix} Duplicate commander role {role} ignored for active triforce window.");
            return;
        }

        seen[role] = true;

        if (!active)
        {
            CPH.SetGlobalVar(VAR_TRIFORCE_ACTIVE, true, false);
            CPH.SetGlobalVar(VAR_TRIFORCE_DEADLINE_UTC, now + XJ_COMMANDER_TRIFORCE_WINDOW_MS, false);
            CPH.DisableTimer(TIMER_XJ_COMMANDER_TRIFORCE_WINDOW);
            CPH.EnableTimer(TIMER_XJ_COMMANDER_TRIFORCE_WINDOW);
            CPH.LogWarn($"{logPrefix} Commander triforce window started for {XJ_COMMANDER_TRIFORCE_WINDOW_MS}ms.");
        }

        CPH.SetGlobalVar(VAR_TRIFORCE_SEEN_JSON, WriteSeenMap(seen), false);

        if (seen.ContainsKey(ROLE_WATER_WIZARD) && seen.ContainsKey(ROLE_CAPTAIN_STRETCH) && seen.ContainsKey(ROLE_THE_DIRECTOR))
        {
            int count = (CPH.GetGlobalVar<int?>(VAR_TRIFORCE_COUNT, false) ?? 0) + 1;
            int highScore = CPH.GetGlobalVar<int?>(VAR_TRIFORCE_HIGH_SCORE, true) ?? 0;
            CPH.SetGlobalVar(VAR_TRIFORCE_COUNT, count, false);
            if (count > highScore)
                CPH.SetGlobalVar(VAR_TRIFORCE_HIGH_SCORE, count, true);

            PlayRevLimiter();
            ClearTriforceState();
            CPH.DisableTimer(TIMER_XJ_COMMANDER_TRIFORCE_WINDOW);
            CPH.LogWarn($"{logPrefix} Commander triforce complete. Current stream count={count}, high score={Math.Max(count, highScore)}.");
        }
    }

    private void HandleTriforceTimerIfExpired(string logPrefix)
    {
        bool active = (CPH.GetGlobalVar<bool?>(VAR_TRIFORCE_ACTIVE, false) ?? false);
        if (!active)
        {
            CPH.LogWarn($"{logPrefix} Timer/manual trigger arrived with no active commander triforce window. No-op.");
            return;
        }

        ClearTriforceState();
        CPH.LogWarn($"{logPrefix} Commander triforce timer fired without all three commanders. State cleared.");
    }

    private CommanderPiece GetCommanderPieceForUser(string user)
    {
        if (MatchesGlobalUser(user, VAR_CURRENT_WATER_WIZARD))
            return new CommanderPiece(ROLE_WATER_WIZARD, XJ_LEFT_ASSET_ID, XJ_LEFT_SRC, XJ_COMMANDER_LEFT_X);

        if (MatchesGlobalUser(user, VAR_CURRENT_CAPTAIN_STRETCH))
            return new CommanderPiece(ROLE_CAPTAIN_STRETCH, XJ_CENTER_ASSET_ID, XJ_CENTER_SRC, XJ_COMMANDER_CENTER_X);

        if (MatchesGlobalUser(user, VAR_CURRENT_THE_DIRECTOR))
            return new CommanderPiece(ROLE_THE_DIRECTOR, XJ_RIGHT_ASSET_ID, XJ_RIGHT_SRC, XJ_COMMANDER_RIGHT_X);

        return null;
    }

    private bool MatchesGlobalUser(string user, string globalVar)
    {
        string current = CPH.GetGlobalVar<string>(globalVar, false) ?? "";
        return !string.IsNullOrWhiteSpace(current) && string.Equals(user.Trim(), current.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private string GetTriggerUser()
    {
        string user = "";
        CPH.TryGetArg("user", out user);
        return user ?? "";
    }

    private Dictionary<string, bool> ReadSeenMap()
    {
        string json = CPH.GetGlobalVar<string>(VAR_TRIFORCE_SEEN_JSON, false) ?? "{}";
        var seen = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        if (json.IndexOf(ROLE_WATER_WIZARD, StringComparison.OrdinalIgnoreCase) >= 0)
            seen[ROLE_WATER_WIZARD] = true;
        if (json.IndexOf(ROLE_CAPTAIN_STRETCH, StringComparison.OrdinalIgnoreCase) >= 0)
            seen[ROLE_CAPTAIN_STRETCH] = true;
        if (json.IndexOf(ROLE_THE_DIRECTOR, StringComparison.OrdinalIgnoreCase) >= 0)
            seen[ROLE_THE_DIRECTOR] = true;
        return seen;
    }

    private string WriteSeenMap(Dictionary<string, bool> seen)
    {
        var parts = new List<string>();
        if (seen.ContainsKey(ROLE_WATER_WIZARD))
            parts.Add("\"" + ROLE_WATER_WIZARD + "\":true");
        if (seen.ContainsKey(ROLE_CAPTAIN_STRETCH))
            parts.Add("\"" + ROLE_CAPTAIN_STRETCH + "\":true");
        if (seen.ContainsKey(ROLE_THE_DIRECTOR))
            parts.Add("\"" + ROLE_THE_DIRECTOR + "\":true");
        return "{" + string.Join(",", parts.ToArray()) + "}";
    }

    private void ClearTriforceState()
    {
        CPH.SetGlobalVar(VAR_TRIFORCE_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_TRIFORCE_SEEN_JSON, "{}", false);
        CPH.SetGlobalVar(VAR_TRIFORCE_DEADLINE_UTC, 0L, false);
    }

    private void PlayRevLimiter()
    {
        string audioPlayPayload =
            "{" +
            "\"soundId\":\"" + XJ_SOUND_ID + "\"," +
            "\"src\":\"" + XJ_SOUND_SRC + "\"," +
            "\"volume\":1.0," +
            "\"loop\":false" +
            "}";
        PublishBrokerMessage(TOPIC_OVERLAY_AUDIO_PLAY, audioPlayPayload);
    }

    private void StopRevLimiter()
    {
        string audioStopPayload = "{\"soundId\":\"" + XJ_SOUND_ID + "\"}";
        PublishBrokerMessage(TOPIC_OVERLAY_AUDIO_STOP, audioStopPayload);
    }

    private class CommanderPiece
    {
        public string Role { get; private set; }
        public string AssetId { get; private set; }
        public string Source { get; private set; }
        public int X { get; private set; }

        public CommanderPiece(string role, string assetId, string source, int x)
        {
            Role = role;
            AssetId = assetId;
            Source = source;
            X = x;
        }
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
