// ACTION-CONTRACT: Actions/Squad/AGENTS.md#Pedro/overlay-publish.cs
// ACTION-CONTRACT-SHA256: 1cceee130637c4af85cfc3e3f16683367aa7ec353e2b9321f8e3aabc6512d2e3

using System;

// =============================================================================
// overlay-publish.cs (Pedro) — Broker publishing reference template
//
// PURPOSE:
//   Provides publish methods for the Pedro mini-game overlay.
//   This file is NOT a deployed action.  Copy the methods listed below
//   directly into the existing Pedro scripts at the integration points shown.
//
// HOW TO INTEGRATE:
//   1. Copy the CONSTANTS BLOCK into each target script's CPHInline class.
//   2. Copy PublishBrokerMessage from Actions/Overlay/broker-publish.cs.
//   3. Copy only the Publish* methods the script needs (see map below).
//
// INTEGRATION MAP:
//   pedro-main.cs    → PublishPedroStart()
//                      Call on the normal game-start path, after
//                      pedro_game_enabled = true and the chat message is sent.
//                      Do NOT call on the secret x500livepedro path
//                      (that path is instant, no overlay window needed).
//
//   pedro-call.cs    → PublishPedroUpdate(int mentionCount)
//                      Call after each successful mention count increment.
//
//   pedro-resolve.cs → PublishPedroEndSuccess(int finalMentionCount)
//                        Call on the success path (mentions > 100).
//                      PublishPedroEndFailure(int finalMentionCount)
//                        Call on the failure path (mentions <= 100).
//
// TOPIC STRINGS (from @stream-overlay/shared/topics.ts):
//   squad.pedro.start   squad.pedro.update   squad.pedro.end
// =============================================================================

public class CPHInline
{
    // ── Constants ─────────────────────────────────────────────────────────────
    private const int    BROKER_WS_INDEX      = 0;
    private const string VAR_BROKER_CONNECTED = "broker_connected";
    private const string BROKER_CLIENT_NAME   = "streamerbot";
    private const int WAIT_RECONNECT_MS = 600;
    private const int WAIT_HELLO_MS     = 200;

    // ── Topic strings ─────────────────────────────────────────────────────────
    private const string TOPIC_PEDRO_START  = "squad.pedro.start";
    private const string TOPIC_PEDRO_UPDATE = "squad.pedro.update";
    private const string TOPIC_PEDRO_END    = "squad.pedro.end";

    // ── Execute — not a deployed action ───────────────────────────────────────
    public bool Execute()
    {
        CPH.LogWarn("[PedroOverlay] Template — not a deployed action. Copy methods into pedro scripts.");
        return true;
    }

    // ── PUBLISH METHODS ───────────────────────────────────────────────────────

    // Call from pedro-main.cs on the normal game-start path.
    // triggeredBy should be the user who typed !pedro (CPH.GetGlobalVar "user" or similar).
    private void PublishPedroStart(string triggeredBy)
    {
        string payload =
            "{\"game\":\"pedro\"" +
            ",\"triggeredBy\":\"" + EscapeJson(triggeredBy) + "\"}";
        PublishBrokerMessage(TOPIC_PEDRO_START, payload);
    }

    // Call from pedro-call.cs after each mention count increment.
    private void PublishPedroUpdate(int mentionCount)
    {
        string payload =
            "{\"game\":\"pedro\"" +
            ",\"state\":{" +
                "\"mentionCount\":" + mentionCount +
            "}}";
        PublishBrokerMessage(TOPIC_PEDRO_UPDATE, payload);
    }

    // Call from pedro-resolve.cs on the success path (mentions > 100).
    private void PublishPedroEndSuccess(int finalMentionCount)
    {
        string payload =
            "{\"game\":\"pedro\"" +
            ",\"result\":{" +
                "\"success\":true" +
                ",\"finalMentionCount\":" + finalMentionCount +
            "}}";
        PublishBrokerMessage(TOPIC_PEDRO_END, payload);
    }

    // Call from pedro-resolve.cs on the failure path (mentions <= 100).
    private void PublishPedroEndFailure(int finalMentionCount)
    {
        string payload =
            "{\"game\":\"pedro\"" +
            ",\"result\":{" +
                "\"success\":false" +
                ",\"finalMentionCount\":" + finalMentionCount +
            "}}";
        PublishBrokerMessage(TOPIC_PEDRO_END, payload);
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
