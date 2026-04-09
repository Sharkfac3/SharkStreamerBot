using System;

// =============================================================================
// overlay-publish.cs (Clone) — Broker publishing reference template
//
// PURPOSE:
//   Provides publish methods for the Clone mini-game overlay.
//   This file is NOT a deployed action.  Copy the methods listed below
//   directly into the existing Clone scripts at the integration points shown.
//
// HOW TO INTEGRATE:
//   1. Copy the CONSTANTS BLOCK into each target script's CPHInline class.
//   2. Copy PublishBrokerMessage from Actions/Overlay/broker-publish.cs.
//   3. Copy only the Publish* methods the script needs (see map below).
//
// INTEGRATION MAP:
//   clone-main.cs    → PublishCloneStart()
//                      Call after clone_game_active = true and the chat
//                      message is sent.  Reads clone_positions_open from
//                      global vars (initialised to "1,2,3,4,5").
//
//   clone-volley.cs  → PublishCloneUpdate(int round, string positionsOpen, int eliminatedPosition)
//                        Call on the CONTINUE path after the position is
//                        eliminated and clone_positions_open is updated.
//                      PublishCloneEndWin(int eliminatedPosition, string winners)
//                        Call on the WIN path.
//                      PublishCloneEndLoss(int eliminatedPosition)
//                        Call on the LOSS path.
//
// NOTE: clone-position.cs handles individual !rebel picks.
//   Player-level position state is not published to the overlay in v1 —
//   the overlay shows open/eliminated positions, not per-player assignment.
//
// TOPIC STRINGS (from @stream-overlay/shared/topics.ts):
//   squad.clone.start   squad.clone.update   squad.clone.end
// =============================================================================

public class CPHInline
{
    // ── Constants ─────────────────────────────────────────────────────────────
    private const int    BROKER_WS_INDEX      = 0;
    private const string VAR_BROKER_CONNECTED = "broker_connected";
    private const string BROKER_CLIENT_NAME   = "streamerbot";
    private const int WAIT_RECONNECT_MS = 600;
    private const int WAIT_HELLO_MS     = 200;

    // ── Global variable names (from SHARED-CONSTANTS.md) ─────────────────────
    private const string VAR_CLONE_POSITIONS_OPEN = "clone_positions_open";

    // ── Topic strings ─────────────────────────────────────────────────────────
    private const string TOPIC_CLONE_START  = "squad.clone.start";
    private const string TOPIC_CLONE_UPDATE = "squad.clone.update";
    private const string TOPIC_CLONE_END    = "squad.clone.end";

    // ── Execute — not a deployed action ───────────────────────────────────────
    public bool Execute()
    {
        CPH.LogWarn("[CloneOverlay] Template — not a deployed action. Copy methods into clone scripts.");
        return true;
    }

    // ── PUBLISH METHODS ───────────────────────────────────────────────────────

    // Call from clone-main.cs after game state is initialised.
    // triggeredBy should be the user who triggered the game.
    private void PublishCloneStart(string triggeredBy)
    {
        string payload =
            "{\"game\":\"clone\"" +
            ",\"triggeredBy\":\"" + EscapeJson(triggeredBy) + "\"}";
        PublishBrokerMessage(TOPIC_CLONE_START, payload);
    }

    // Call from clone-volley.cs on the CONTINUE path.
    //   round            — current round number (after increment)
    //   positionsOpen    — comma-separated remaining positions, e.g. "1,2,4,5"
    //   eliminatedPosition — the position just eliminated this volley
    private void PublishCloneUpdate(int round, string positionsOpen, int eliminatedPosition)
    {
        string payload =
            "{\"game\":\"clone\"" +
            ",\"state\":{" +
                "\"round\":" + round +
                ",\"positionsOpen\":\"" + EscapeJson(positionsOpen) + "\"" +
                ",\"eliminatedPosition\":" + eliminatedPosition +
            "}}";
        PublishBrokerMessage(TOPIC_CLONE_UPDATE, payload);
    }

    // Call from clone-volley.cs on the WIN path.
    //   eliminatedPosition — the final eliminated position
    //   winners            — comma-separated winner usernames from clone_winners var
    private void PublishCloneEndWin(int eliminatedPosition, string winners)
    {
        string payload =
            "{\"game\":\"clone\"" +
            ",\"result\":{" +
                "\"outcome\":\"win\"" +
                ",\"eliminatedPosition\":" + eliminatedPosition +
                ",\"winners\":\"" + EscapeJson(winners) + "\"" +
            "}}";
        PublishBrokerMessage(TOPIC_CLONE_END, payload);
    }

    // Call from clone-volley.cs on the LOSS path.
    //   eliminatedPosition — the final eliminated position
    private void PublishCloneEndLoss(int eliminatedPosition)
    {
        string payload =
            "{\"game\":\"clone\"" +
            ",\"result\":{" +
                "\"outcome\":\"loss\"" +
                ",\"eliminatedPosition\":" + eliminatedPosition +
                ",\"winners\":\"\"" +
            "}}";
        PublishBrokerMessage(TOPIC_CLONE_END, payload);
    }

    // ── Shared helper ─────────────────────────────────────────────────────────
    private bool PublishBrokerMessage(string topic, string payloadJson)
    {
        const string LOG_PREFIX = "[BrokerPublish]";
        if (string.IsNullOrWhiteSpace(topic)) return false;
        if (string.IsNullOrWhiteSpace(payloadJson)) return false;

        if (!CPH.WebsocketIsConnected(BROKER_WS_INDEX))
        {
            CPH.LogWarn($"{LOG_PREFIX} Not connected. Attempting reconnect for topic '{topic}'...");
            CPH.WebsocketConnect(BROKER_WS_INDEX);
            CPH.Wait(WAIT_RECONNECT_MS);
            if (!CPH.WebsocketIsConnected(BROKER_WS_INDEX))
            {
                CPH.LogError($"{LOG_PREFIX} Reconnect failed. Message for topic '{topic}' dropped.");
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

    private string EscapeJson(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
