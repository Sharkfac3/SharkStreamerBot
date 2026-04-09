using System;
using System.Collections.Generic;

// =============================================================================
// overlay-publish.cs — LotAT Overlay Publishing Reference
//
// PURPOSE:
//   Canonical reference for all PublishLotat* helper methods.
//   Each method builds the correct lotat.* broker message and sends it over
//   the WebSocket connection established by broker-connect.cs.
//
// HOW TO USE:
//   1. Copy the CONSTANTS BLOCK (below) into your LotAT engine script.
//   2. Copy the PublishBrokerMessage method from Actions/Overlay/broker-publish.cs.
//   3. Copy whichever PublishLotat* methods your script needs.
//
// INTEGRATION POINTS — which existing LotAT scripts call which methods:
//   lotat-start-main.cs      → PublishLotatSessionStart, PublishLotatJoinOpen
//   lotat-join.cs            → PublishLotatJoinUpdate
//   lotat-join-timeout.cs    → PublishLotatJoinClose
//   lotat-node-enter.cs      → PublishLotatNodeEnter, PublishLotatChaosUpdate
//   lotat-commander-input.cs → PublishLotatCommanderClose (outcome: success)
//   lotat-commander-timeout.cs → PublishLotatCommanderClose (outcome: skipped)
//   lotat-dice-roll.cs       → PublishLotatDiceRoll, PublishLotatDiceClose (success)
//   lotat-dice-timeout.cs    → PublishLotatDiceClose (failure)
//   lotat-decision-input.cs  → PublishLotatVoteCast
//   lotat-decision-resolve.cs → PublishLotatVoteClose
//   lotat-decision-timeout.cs → PublishLotatVoteClose (null winner)
//   lotat-end-session.cs     → PublishLotatSessionEnd
//
// WHY SEPARATE FROM THE ENGINE:
//   Streamer.bot C# scripts are isolated — no shared includes.
//   Each script that needs publishing copies in the methods it needs.
//   This file is the authoritative reference so all copies stay consistent.
//
// NOTE ON COMMANDER/DICE OPEN EVENTS:
//   PublishLotatCommanderOpen is called from lotat-node-enter.cs
//   AFTER publishing lotat.node.enter, when commander_moment.enabled = true.
//   PublishLotatDiceOpen is called similarly when dice_hook.enabled = true.
//   This matches the session-lifecycle.md contract where node_intro leads into
//   commander_open or dice_open before decision_open.
// =============================================================================

public class CPHInline
{
    // ── Copy these constants into any script that uses PublishBrokerMessage ──

    private const int    BROKER_WS_INDEX      = 0;
    private const string VAR_BROKER_CONNECTED = "broker_connected";
    private const string BROKER_CLIENT_NAME   = "streamerbot";
    private const int    WAIT_RECONNECT_MS    = 600;
    private const int    WAIT_HELLO_MS        = 200;

    // ── LotAT topic strings — must match @stream-overlay/shared/topics.ts ──

    private const string TOPIC_SESSION_START    = "lotat.session.start";
    private const string TOPIC_SESSION_END      = "lotat.session.end";
    private const string TOPIC_JOIN_OPEN        = "lotat.join.open";
    private const string TOPIC_JOIN_UPDATE      = "lotat.join.update";
    private const string TOPIC_JOIN_CLOSE       = "lotat.join.close";
    private const string TOPIC_NODE_ENTER       = "lotat.node.enter";
    private const string TOPIC_VOTE_OPEN        = "lotat.vote.open";
    private const string TOPIC_VOTE_CAST        = "lotat.vote.cast";
    private const string TOPIC_VOTE_CLOSE       = "lotat.vote.close";
    private const string TOPIC_DICE_OPEN        = "lotat.dice.open";
    private const string TOPIC_DICE_ROLL        = "lotat.dice.roll";
    private const string TOPIC_DICE_CLOSE       = "lotat.dice.close";
    private const string TOPIC_COMMANDER_OPEN   = "lotat.commander.open";
    private const string TOPIC_COMMANDER_CLOSE  = "lotat.commander.close";
    private const string TOPIC_CHAOS_UPDATE     = "lotat.chaos.update";

    /*
     * Execute — not triggered directly.
     * This script is a reference template; copy methods into engine scripts.
     */
    public bool Execute()
    {
        CPH.LogWarn("[LotATOverlayPublish] Template action executed directly. Copy the methods you need into your engine scripts.");
        return true;
    }

    // =========================================================================
    // lotat.session.start
    //
    // Call from lotat-start-main.cs after initialising session state.
    // Maps to: idle → join_open transition.
    //
    // Parameters:
    //   sessionId — new session UUID (from lotat_session_id global)
    //   storyId   — from loaded story JSON (story_id field)
    //   title     — from loaded story JSON (title field)
    // =========================================================================
    private void PublishLotatSessionStart(string sessionId, string storyId, string title)
    {
        string payload =
            "{\"sessionId\":\"" + EscapeJson(sessionId) + "\"" +
            ",\"storyId\":\"" + EscapeJson(storyId) + "\"" +
            ",\"title\":\"" + EscapeJson(title) + "\"}";
        PublishBrokerMessage(TOPIC_SESSION_START, payload);
    }

    // =========================================================================
    // lotat.session.end
    //
    // Call from lotat-end-session.cs after all state is cleared.
    // Maps to: ended → idle transition.
    //
    // reason values:
    //   "ending-reached" — normal story completion; endState required
    //   "zero-join"      — no participants joined
    //   "zero-votes"     — decision closed with zero valid votes
    //   "fault-abort"    — unrecoverable runtime error
    //
    // endState values (only when reason = "ending-reached"):
    //   "success" | "partial" | "failure"
    // =========================================================================
    private void PublishLotatSessionEnd(string sessionId, string reason, string endState)
    {
        // endState is only included when reason = "ending-reached"
        string endStatePart = "";
        if (!string.IsNullOrEmpty(endState) && reason == "ending-reached")
            endStatePart = ",\"endState\":\"" + EscapeJson(endState) + "\"";

        string payload =
            "{\"sessionId\":\"" + EscapeJson(sessionId) + "\"" +
            ",\"reason\":\"" + EscapeJson(reason) + "\"" +
            endStatePart + "}";
        PublishBrokerMessage(TOPIC_SESSION_END, payload);
    }

    // =========================================================================
    // lotat.join.open
    //
    // Call from lotat-start-main.cs immediately after session.start,
    // when the join timer is started.
    //
    // windowSeconds: fixed 120 in v1.
    // =========================================================================
    private void PublishLotatJoinOpen(string sessionId, int windowSeconds)
    {
        string payload =
            "{\"sessionId\":\"" + EscapeJson(sessionId) + "\"" +
            ",\"windowSeconds\":" + windowSeconds + "}";
        PublishBrokerMessage(TOPIC_JOIN_OPEN, payload);
    }

    // =========================================================================
    // lotat.join.update
    //
    // Call from lotat-join.cs each time a valid !join is processed.
    //
    // username:        lowercased viewer username (matches runtime identity rule)
    // participantCount: running count from lotat_session_joined_count global
    // =========================================================================
    private void PublishLotatJoinUpdate(string sessionId, string username, int participantCount)
    {
        string payload =
            "{\"sessionId\":\"" + EscapeJson(sessionId) + "\"" +
            ",\"username\":\"" + EscapeJson(username) + "\"" +
            ",\"participantCount\":" + participantCount + "}";
        PublishBrokerMessage(TOPIC_JOIN_UPDATE, payload);
    }

    // =========================================================================
    // lotat.join.close
    //
    // Call from lotat-join-timeout.cs when the join timer fires.
    // Also call if a future early-close trigger is added.
    //
    // participantCount: final frozen roster size (0 = zero-join path).
    // =========================================================================
    private void PublishLotatJoinClose(string sessionId, int participantCount)
    {
        string payload =
            "{\"sessionId\":\"" + EscapeJson(sessionId) + "\"" +
            ",\"participantCount\":" + participantCount + "}";
        PublishBrokerMessage(TOPIC_JOIN_CLOSE, payload);
    }

    // =========================================================================
    // lotat.node.enter
    //
    // Call from lotat-node-enter.cs after loading the node and setting state.
    //
    // crewFocusJson: pre-serialised JSON for the crewFocus object, e.g.:
    //   "{\"commander\":\"The Water Wizard\",\"squadMember\":null}"
    //
    // choicesJson: pre-serialised JSON array of choice objects, e.g.:
    //   "[{\"choiceId\":\"c1\",\"label\":\"Scan area\",\"command\":\"!scan\"}]"
    //
    // diceHookJson: pre-serialised optional JSON object or "null"
    // commanderMomentJson: pre-serialised optional JSON object or "null"
    //
    // chaosDelta:  the chaos.delta value from the node (applied before publishing)
    // =========================================================================
    private void PublishLotatNodeEnter(
        string sessionId,
        string nodeId,
        string nodeType,
        string shipSection,
        string title,
        string readAloud,
        string sfxHint,
        string crewFocusJson,
        int    chaosDelta,
        string diceHookJson,
        string commanderMomentJson,
        string choicesJson,
        string endState)
    {
        // endState is null for stage nodes; non-null for ending nodes.
        string endStatePart = (endState == "null" || string.IsNullOrEmpty(endState))
            ? "null"
            : "\"" + EscapeJson(endState) + "\"";

        // sfxHint is null when absent
        string sfxPart = string.IsNullOrEmpty(sfxHint) ? "null" : "\"" + EscapeJson(sfxHint) + "\"";

        string payload =
            "{\"sessionId\":\"" + EscapeJson(sessionId) + "\"" +
            ",\"nodeId\":\"" + EscapeJson(nodeId) + "\"" +
            ",\"nodeType\":\"" + EscapeJson(nodeType) + "\"" +
            ",\"shipSection\":\"" + EscapeJson(shipSection) + "\"" +
            ",\"title\":\"" + EscapeJson(title) + "\"" +
            ",\"readAloud\":\"" + EscapeJson(readAloud) + "\"" +
            ",\"sfxHint\":" + sfxPart +
            ",\"crewFocus\":" + crewFocusJson +
            ",\"chaosDelta\":" + chaosDelta +
            ",\"diceHook\":" + (string.IsNullOrEmpty(diceHookJson) ? "null" : diceHookJson) +
            ",\"commanderMoment\":" + (string.IsNullOrEmpty(commanderMomentJson) ? "null" : commanderMomentJson) +
            ",\"choices\":" + (string.IsNullOrEmpty(choicesJson) ? "[]" : choicesJson) +
            ",\"endState\":" + endStatePart +
            "}";
        PublishBrokerMessage(TOPIC_NODE_ENTER, payload);
    }

    // =========================================================================
    // lotat.chaos.update
    //
    // Call from lotat-node-enter.cs when the node has a chaosDelta > 0.
    // Publish AFTER node.enter so the overlay has context before the meter updates.
    //
    // chaosTotal: updated lotat_session_chaos_total after applying delta
    // delta:      the node's chaos.delta value
    // =========================================================================
    private void PublishLotatChaosUpdate(string sessionId, int chaosTotal, int delta)
    {
        string payload =
            "{\"sessionId\":\"" + EscapeJson(sessionId) + "\"" +
            ",\"chaosTotal\":" + chaosTotal +
            ",\"delta\":" + delta + "}";
        PublishBrokerMessage(TOPIC_CHAOS_UPDATE, payload);
    }

    // =========================================================================
    // lotat.commander.open
    //
    // Call from lotat-node-enter.cs when a node has commander_moment.enabled = true,
    // after publishing node.enter.
    // =========================================================================
    private void PublishLotatCommanderOpen(
        string sessionId,
        string nodeId,
        string commander,
        string prompt,
        int    windowSeconds)
    {
        string payload =
            "{\"sessionId\":\"" + EscapeJson(sessionId) + "\"" +
            ",\"nodeId\":\"" + EscapeJson(nodeId) + "\"" +
            ",\"commander\":\"" + EscapeJson(commander) + "\"" +
            ",\"prompt\":\"" + EscapeJson(prompt) + "\"" +
            ",\"windowSeconds\":" + windowSeconds + "}";
        PublishBrokerMessage(TOPIC_COMMANDER_OPEN, payload);
    }

    // =========================================================================
    // lotat.commander.close
    //
    // Call from lotat-commander-input.cs on success or
    // lotat-commander-timeout.cs on timeout.
    //
    // outcome:     "success" | "skipped"
    // successText: the authored success_text from the node (only on success)
    // =========================================================================
    private void PublishLotatCommanderClose(
        string sessionId,
        string nodeId,
        string outcome,
        string successText)
    {
        string successPart = (!string.IsNullOrEmpty(successText) && outcome == "success")
            ? ",\"successText\":\"" + EscapeJson(successText) + "\""
            : "";

        string payload =
            "{\"sessionId\":\"" + EscapeJson(sessionId) + "\"" +
            ",\"nodeId\":\"" + EscapeJson(nodeId) + "\"" +
            ",\"outcome\":\"" + EscapeJson(outcome) + "\"" +
            successPart + "}";
        PublishBrokerMessage(TOPIC_COMMANDER_CLOSE, payload);
    }

    // =========================================================================
    // lotat.dice.open
    //
    // Call from lotat-node-enter.cs when a node has dice_hook.enabled = true,
    // after publishing node.enter.
    // =========================================================================
    private void PublishLotatDiceOpen(
        string sessionId,
        string nodeId,
        string purpose,
        int    successThreshold,
        int    windowSeconds)
    {
        string payload =
            "{\"sessionId\":\"" + EscapeJson(sessionId) + "\"" +
            ",\"nodeId\":\"" + EscapeJson(nodeId) + "\"" +
            ",\"purpose\":\"" + EscapeJson(purpose) + "\"" +
            ",\"successThreshold\":" + successThreshold +
            ",\"windowSeconds\":" + windowSeconds + "}";
        PublishBrokerMessage(TOPIC_DICE_OPEN, payload);
    }

    // =========================================================================
    // lotat.dice.roll
    //
    // Call from lotat-dice-roll.cs each time a viewer uses !roll.
    //
    // isSuccess: true when rollValue >= successThreshold (this roll closes the window)
    // =========================================================================
    private void PublishLotatDiceRoll(
        string sessionId,
        string nodeId,
        string username,
        int    rollValue,
        int    successThreshold,
        bool   isSuccess)
    {
        string payload =
            "{\"sessionId\":\"" + EscapeJson(sessionId) + "\"" +
            ",\"nodeId\":\"" + EscapeJson(nodeId) + "\"" +
            ",\"username\":\"" + EscapeJson(username) + "\"" +
            ",\"rollValue\":" + rollValue +
            ",\"successThreshold\":" + successThreshold +
            ",\"isSuccess\":" + (isSuccess ? "true" : "false") + "}";
        PublishBrokerMessage(TOPIC_DICE_ROLL, payload);
    }

    // =========================================================================
    // lotat.dice.close
    //
    // Call when the dice window resolves (success or failure).
    //   Success path: lotat-dice-roll.cs after first successful roll
    //   Failure path: lotat-dice-timeout.cs when timer expires with no success
    //
    // winningRoll / winningUsername only present on success.
    // outcomeText: authored success_text or failure_text from the node.
    // =========================================================================
    private void PublishLotatDiceClose(
        string sessionId,
        string nodeId,
        string outcome,
        int    winningRoll,
        string winningUsername,
        string outcomeText)
    {
        string winPart = (outcome == "success")
            ? ",\"winningRoll\":" + winningRoll +
              ",\"winningUsername\":\"" + EscapeJson(winningUsername) + "\""
            : "";

        string payload =
            "{\"sessionId\":\"" + EscapeJson(sessionId) + "\"" +
            ",\"nodeId\":\"" + EscapeJson(nodeId) + "\"" +
            ",\"outcome\":\"" + EscapeJson(outcome) + "\"" +
            winPart +
            ",\"outcomeText\":\"" + EscapeJson(outcomeText) + "\"}";
        PublishBrokerMessage(TOPIC_DICE_CLOSE, payload);
    }

    // =========================================================================
    // lotat.vote.open
    //
    // Call from lotat-node-enter.cs (or lotat-commander/dice close path)
    // when the decision window opens.
    //
    // choicesJson: pre-serialised JSON array (same format as in node.enter)
    // =========================================================================
    private void PublishLotatVoteOpen(
        string sessionId,
        string nodeId,
        string choicesJson,
        int    windowSeconds,
        int    participantCount)
    {
        string payload =
            "{\"sessionId\":\"" + EscapeJson(sessionId) + "\"" +
            ",\"nodeId\":\"" + EscapeJson(nodeId) + "\"" +
            ",\"choices\":" + choicesJson +
            ",\"windowSeconds\":" + windowSeconds +
            ",\"participantCount\":" + participantCount + "}";
        PublishBrokerMessage(TOPIC_VOTE_OPEN, payload);
    }

    // =========================================================================
    // lotat.vote.cast
    //
    // Call from lotat-decision-input.cs each time a valid vote is received.
    //
    // voteTotalsJson: pre-serialised JSON object mapping command→count, e.g.:
    //   "{\"!scan\":3,\"!reroute\":1,\"!drink\":0}"
    // =========================================================================
    private void PublishLotatVoteCast(
        string sessionId,
        string nodeId,
        string username,
        string command,
        string voteTotalsJson,
        int    votedCount,
        int    participantCount)
    {
        string payload =
            "{\"sessionId\":\"" + EscapeJson(sessionId) + "\"" +
            ",\"nodeId\":\"" + EscapeJson(nodeId) + "\"" +
            ",\"username\":\"" + EscapeJson(username) + "\"" +
            ",\"command\":\"" + EscapeJson(command) + "\"" +
            ",\"voteTotals\":" + voteTotalsJson +
            ",\"votedCount\":" + votedCount +
            ",\"participantCount\":" + participantCount + "}";
        PublishBrokerMessage(TOPIC_VOTE_CAST, payload);
    }

    // =========================================================================
    // lotat.vote.close
    //
    // Call from lotat-decision-resolve.cs or lotat-decision-timeout.cs.
    //
    // winningCommand:   the winning !command, or "null" string if unresolved
    // winningChoiceId:  matching choice_id from story JSON, or "null"
    // resultFlavor:     authored result_flavor text, or "null"
    // nextNodeId:       the next node to enter, or "null" if session is ending
    // voteTotalsJson:   final vote map as JSON object
    // =========================================================================
    private void PublishLotatVoteClose(
        string sessionId,
        string nodeId,
        string winningCommand,
        string winningChoiceId,
        string resultFlavor,
        string voteTotalsJson,
        string nextNodeId)
    {
        // Null-safe serialisation helpers
        string WrapStr(string s) =>
            (s == null || s == "null" || string.IsNullOrEmpty(s))
                ? "null"
                : "\"" + EscapeJson(s) + "\"";

        string payload =
            "{\"sessionId\":\"" + EscapeJson(sessionId) + "\"" +
            ",\"nodeId\":\"" + EscapeJson(nodeId) + "\"" +
            ",\"winningCommand\":" + WrapStr(winningCommand) +
            ",\"winningChoiceId\":" + WrapStr(winningChoiceId) +
            ",\"resultFlavor\":" + WrapStr(resultFlavor) +
            ",\"voteTotals\":" + voteTotalsJson +
            ",\"nextNodeId\":" + WrapStr(nextNodeId) + "}";
        PublishBrokerMessage(TOPIC_VOTE_CLOSE, payload);
    }

    // =========================================================================
    // PublishBrokerMessage
    //
    // Copied verbatim from Actions/Overlay/broker-publish.cs.
    // Wraps any JSON payload in a BrokerMessage envelope and sends it.
    // =========================================================================
    private bool PublishBrokerMessage(string topic, string payloadJson)
    {
        const string LOG_PREFIX = "[LotATPublish]";

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
            ",\"payload\":" + payloadJson + "}";

        CPH.WebsocketSend(message, BROKER_WS_INDEX);
        CPH.LogWarn($"{LOG_PREFIX} Sent topic={topic} id={id}");
        return true;
    }

    // =========================================================================
    // EscapeJson
    //
    // Escapes a string value for safe inline JSON embedding.
    // Handles the most common characters.  Long strings with unusual characters
    // may need the full implementation from lotat-start-main.cs.
    // =========================================================================
    private string EscapeJson(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";

        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
}
