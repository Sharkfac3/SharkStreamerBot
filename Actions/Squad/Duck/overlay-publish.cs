using System;

// =============================================================================
// overlay-publish.cs (Duck) — Broker publishing reference template
//
// PURPOSE:
//   Provides publish methods for the Duck mini-game overlay.
//   This file is NOT a deployed action.  Copy the methods listed below
//   directly into the existing Duck scripts at the integration points shown.
//
// HOW TO INTEGRATE:
//   1. Copy the CONSTANTS BLOCK into each target script's CPHInline class.
//   2. Copy PublishBrokerMessage from Actions/Overlay/broker-publish.cs.
//   3. Copy only the Publish* methods the script needs (see map below).
//
// INTEGRATION MAP:
//   duck-main.cs    → PublishDuckStart()
//                     Call at the end of the "new event" path, after
//                     duck_event_active is set and the chat message is sent.
//
//   duck-call.cs    → PublishDuckUpdate(int quackCount, int uniqueQuackerCount)
//                     Call after incrementing the quack count, before the
//                     threshold check.  On the success/threshold path, call
//                     PublishDuckEndSuccess() after disabling the timer.
//
//   duck-resolve.cs → PublishDuckEndFailure(int finalQuackCount)
//                     Call on the timeout path, after setting
//                     duck_event_active = false.
//
// TOPIC STRINGS (from @stream-overlay/shared/topics.ts):
//   squad.duck.start   squad.duck.update   squad.duck.end
// =============================================================================

public class CPHInline
{
    // ── Constants — copy into any script that uses PublishBrokerMessage ──────
    private const int    BROKER_WS_INDEX      = 0;
    private const string VAR_BROKER_CONNECTED = "broker_connected";
    private const string BROKER_CLIENT_NAME   = "streamerbot";
    private const int WAIT_RECONNECT_MS = 600;
    private const int WAIT_HELLO_MS     = 200;

    // ── Global variable names (from SHARED-CONSTANTS.md) ────────────────────
    private const string VAR_DUCK_CALLER              = "duck_caller";
    private const string VAR_DUCK_QUACK_COUNT         = "duck_quack_count";
    private const string VAR_DUCK_UNIQUE_QUACKER_COUNT = "duck_unique_quacker_count";

    // ── Topic strings ────────────────────────────────────────────────────────
    private const string TOPIC_DUCK_START  = "squad.duck.start";
    private const string TOPIC_DUCK_UPDATE = "squad.duck.update";
    private const string TOPIC_DUCK_END    = "squad.duck.end";

    // ── Execute — not used as a deployed action ──────────────────────────────
    public bool Execute()
    {
        CPH.LogWarn("[DuckOverlay] Template — not a deployed action. Copy methods into duck scripts.");
        return true;
    }

    // ── PUBLISH METHODS — copy into duck scripts as described above ──────────

    // Call from duck-main.cs on the new-event path.
    // Reads duck_caller from global vars (set by duck-main.cs before this call).
    private void PublishDuckStart()
    {
        string caller = CPH.GetGlobalVar<string>(VAR_DUCK_CALLER) ?? "";
        string payload =
            "{\"game\":\"duck\"" +
            ",\"triggeredBy\":\"" + EscapeJson(caller) + "\"}";
        PublishBrokerMessage(TOPIC_DUCK_START, payload);
    }

    // Call from duck-call.cs after each quack count increment.
    // Pass the current running totals directly — they are already computed in the script.
    private void PublishDuckUpdate(int quackCount, int uniqueQuackerCount)
    {
        string payload =
            "{\"game\":\"duck\"" +
            ",\"state\":{" +
                "\"quackCount\":" + quackCount +
                ",\"uniqueQuackerCount\":" + uniqueQuackerCount +
            "}}";
        PublishBrokerMessage(TOPIC_DUCK_UPDATE, payload);
    }

    // Call from duck-call.cs on the threshold-reached path, after the timer is disabled.
    private void PublishDuckEndSuccess(int finalQuackCount, int uniqueQuackerCount)
    {
        string payload =
            "{\"game\":\"duck\"" +
            ",\"result\":{" +
                "\"success\":true" +
                ",\"finalQuackCount\":" + finalQuackCount +
                ",\"uniqueQuackerCount\":" + uniqueQuackerCount +
            "}}";
        PublishBrokerMessage(TOPIC_DUCK_END, payload);
    }

    // Call from duck-resolve.cs on the timeout path, after duck_event_active = false.
    private void PublishDuckEndFailure(int finalQuackCount)
    {
        string payload =
            "{\"game\":\"duck\"" +
            ",\"result\":{" +
                "\"success\":false" +
                ",\"finalQuackCount\":" + finalQuackCount +
                ",\"uniqueQuackerCount\":0" +
            "}}";
        PublishBrokerMessage(TOPIC_DUCK_END, payload);
    }

    // ── Shared helper — copy from Actions/Overlay/broker-publish.cs ─────────
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

    // ── JSON escaping helper ─────────────────────────────────────────────────
    private string EscapeJson(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
