// ACTION-CONTRACT: Actions/Squad/AGENTS.md#Toothless/overlay-publish.cs
// ACTION-CONTRACT-SHA256: b40fd9bddd76fcd4d684ebd14b466f8b7a41fb87072e9b4dce046c070f970290

using System;

// =============================================================================
// overlay-publish.cs (Toothless) — Broker publishing reference template
//
// PURPOSE:
//   Provides publish methods for the Toothless mini-game overlay.
//   This file is NOT a deployed action.  Copy the methods listed below
//   directly into toothless-main.cs at the integration points shown.
//
// HOW TO INTEGRATE:
//   1. Copy the CONSTANTS BLOCK into toothless-main.cs CPHInline class.
//   2. Copy PublishBrokerMessage from Actions/Overlay/broker-publish.cs.
//   3. Copy PublishToothlessStart and PublishToothlessEnd into toothless-main.cs.
//
// INTEGRATION MAP (all in toothless-main.cs):
//   PublishToothlessStart(triggeredBy)
//     → Call AFTER acquiring the mini-game lock but BEFORE rolling.
//       Gives the overlay the "rolling…" state briefly.
//
//   PublishToothlessEnd(rarity, username, isFirstUnlock)
//     → Call AFTER the rarity is determined and last_rarity/last_user are set.
//       Call this on ALL paths — both first-unlock and non-unlock rolls.
//       On the first-unlock path, call it before the 19-second wait so the
//       overlay shows the result during the unlock animation.
//
// RARITY VALUES (must match ToothlessRarity in @stream-overlay/shared):
//   "regular" | "smol" | "long" | "flight" | "party"
//   These match the rarity variable names in SHARED-CONSTANTS.md.
//
// TOPIC STRINGS (from @stream-overlay/shared/topics.ts):
//   squad.toothless.start   squad.toothless.end
//   (No squad.toothless.update — roll is instant)
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
    private const string VAR_LAST_RARITY = "last_rarity";
    private const string VAR_LAST_USER   = "last_user";

    // ── Topic strings ─────────────────────────────────────────────────────────
    private const string TOPIC_TOOTHLESS_START = "squad.toothless.start";
    private const string TOPIC_TOOTHLESS_END   = "squad.toothless.end";

    // ── Execute — not a deployed action ───────────────────────────────────────
    public bool Execute()
    {
        CPH.LogWarn("[ToothlessOverlay] Template — not a deployed action. Copy methods into toothless-main.cs.");
        return true;
    }

    // ── PUBLISH METHODS ───────────────────────────────────────────────────────

    // Call from toothless-main.cs immediately after the mini-game lock is acquired.
    // triggeredBy — the user who triggered the roll (use CPH arg or global var).
    private void PublishToothlessStart(string triggeredBy)
    {
        string payload =
            "{\"game\":\"toothless\"" +
            ",\"triggeredBy\":\"" + EscapeJson(triggeredBy) + "\"}";
        PublishBrokerMessage(TOPIC_TOOTHLESS_START, payload);
    }

    // Call from toothless-main.cs after the rarity is determined.
    //   rarity        — one of: "regular", "smol", "long", "flight", "party"
    //   username      — the rolling user's display name or login
    //   isFirstUnlock — true if this rarity has never been unlocked before
    private void PublishToothlessEnd(string rarity, string username, bool isFirstUnlock)
    {
        string isFirstUnlockStr = isFirstUnlock ? "true" : "false";
        string payload =
            "{\"game\":\"toothless\"" +
            ",\"result\":{" +
                "\"rarity\":\"" + EscapeJson(rarity) + "\"" +
                ",\"username\":\"" + EscapeJson(username) + "\"" +
                ",\"isFirstUnlock\":" + isFirstUnlockStr +
            "}}";
        PublishBrokerMessage(TOPIC_TOOTHLESS_END, payload);
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
