using System;
using System.Collections.Generic;
using System.Text.Json;

public class CPHInline
{
    // -------------------------------------------------
    // LotAT shared runtime constants
    // Keep these names synchronized with Actions/SHARED-CONSTANTS.md.
    // -------------------------------------------------
    private const string VAR_LOTAT_ACTIVE = "lotat_active";
    private const string VAR_LOTAT_SESSION_ID = "lotat_session_id";
    private const string VAR_LOTAT_SESSION_STAGE = "lotat_session_stage";
    private const string VAR_LOTAT_SESSION_ROSTER_FROZEN = "lotat_session_roster_frozen";
    private const string VAR_LOTAT_SESSION_JOINED_ROSTER_JSON = "lotat_session_joined_roster_json";
    private const string VAR_LOTAT_SESSION_JOINED_COUNT = "lotat_session_joined_count";
    private const string VAR_LOTAT_NODE_ACTIVE_WINDOW = "lotat_node_active_window";
    private const string VAR_LOTAT_NODE_WINDOW_RESOLVED = "lotat_node_window_resolved";
    private const string VAR_LOTAT_NODE_ALLOWED_COMMANDS_JSON = "lotat_node_allowed_commands_json";
    private const string VAR_LOTAT_VOTE_MAP_JSON = "lotat_vote_map_json";
    private const string VAR_LOTAT_VOTE_VALID_COUNT = "lotat_vote_valid_count";

    private const string TIMER_LOTAT_DECISION_WINDOW = "LotAT - Decision Window";

    private const string STAGE_DECISION_OPEN = "decision_open";
    private const string WINDOW_DECISION = "decision";

    /*
     * Purpose:
     * - Shared authored decision-command intake for LotAT v1.
     * - Accepts any authored decision command, validates it against the active node,
     *   records one active valid vote per joined participant, and supports latest-valid-vote replacement.
     *
     * Expected trigger/input:
     * - Streamer.bot chat command triggers for all authored decision commands:
     *   !scan, !target, !analyze, !reroute, !deploy, !contain, !inspect, !drink, !simulate
     * - Reads the sender from `user` / `userName` and command text from `command`, `message`, or `rawInput`.
     *
     * Required runtime variables:
     * - Reads: active/session/stage/window guards, frozen joined roster, allowed decision commands,
     *   and the current per-node vote map.
     * - Writes: lotat_vote_map_json, lotat_vote_valid_count, lotat_node_window_resolved.
     *
     * Key outputs/side effects:
     * - Counts votes only during decision_open.
     * - Counts only users in the frozen joined roster.
     * - Counts only commands present in lotat_node_allowed_commands_json.
     * - Keeps one valid vote per joined participant for the active node.
     * - Later valid votes replace earlier valid votes from the same participant.
     * - Disables the decision timer and marks the node resolved when every joined participant has a valid vote.
     *
     * Operator notes:
     * - Wire every authored decision command trigger to this shared action.
     * - For early-close to advance immediately, put lotat-decision-resolve.cs and then lotat-node-enter.cs
     *   immediately after this action in the same Streamer.bot action group.
     */
    public bool Execute()
    {
        const string logPrefix = "LotAT Decision Input";

        try
        {
            if (!IsDecisionWindowOpen())
                return true;

            string participantKey = NormalizeUser(GetSenderUser());
            if (string.IsNullOrWhiteSpace(participantKey))
                return true;

            string command = NormalizeCommand(GetInvokedCommandText());
            if (string.IsNullOrWhiteSpace(command))
                return true;

            List<string> joinedRoster = ReadNormalizedRoster(logPrefix);
            if (!joinedRoster.Contains(participantKey))
                return true;

            List<string> allowedCommands = ReadAllowedCommands(logPrefix);
            if (!allowedCommands.Contains(command))
                return true;

            Dictionary<string, string> voteMap = ReadVoteMap(logPrefix, joinedRoster, allowedCommands);
            string priorVote = voteMap.ContainsKey(participantKey) ? voteMap[participantKey] : "";
            voteMap[participantKey] = command;

            int validCount = voteMap.Count;
            int joinedCount = joinedRoster.Count;
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_JOINED_COUNT, joinedCount, false);

            CPH.SetGlobalVar(VAR_LOTAT_VOTE_MAP_JSON, JsonSerializer.Serialize(voteMap), false);
            CPH.SetGlobalVar(VAR_LOTAT_VOTE_VALID_COUNT, validCount, false);

            string sessionId = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_ID, false) ?? "";
            bool replacedVote = !string.IsNullOrWhiteSpace(priorVote) && !string.Equals(priorVote, command, StringComparison.OrdinalIgnoreCase);
            bool repeatedVote = string.Equals(priorVote, command, StringComparison.OrdinalIgnoreCase);
            CPH.LogWarn($"[{logPrefix}] Valid vote recorded. sessionId='{sessionId}', user='{participantKey}', command='{command}', prior='{priorVote}', repeated={repeatedVote}, replaced={replacedVote}, validCount={validCount}, joinedCount={joinedCount}.");

            if (joinedCount > 0 && validCount >= joinedCount)
            {
                CPH.DisableTimer(TIMER_LOTAT_DECISION_WINDOW);
                CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, true, false);
                CPH.LogWarn($"[{logPrefix}] Early close triggered because every joined participant has a valid vote. sessionId='{sessionId}', validCount={validCount}, joinedCount={joinedCount}.");
            }

            return true;
        }
        catch (Exception ex)
        {
            CPH.LogError($"[{logPrefix}] Unhandled exception while processing authored decision input: {ex}");
            return true;
        }
    }

    private bool IsDecisionWindowOpen()
    {
        bool active = CPH.GetGlobalVar<bool?>(VAR_LOTAT_ACTIVE, false) ?? false;
        if (!active)
            return false;

        string stage = (CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_STAGE, false) ?? "").Trim();
        if (!string.Equals(stage, STAGE_DECISION_OPEN, StringComparison.OrdinalIgnoreCase))
            return false;

        string activeWindow = (CPH.GetGlobalVar<string>(VAR_LOTAT_NODE_ACTIVE_WINDOW, false) ?? "").Trim();
        if (!string.Equals(activeWindow, WINDOW_DECISION, StringComparison.OrdinalIgnoreCase))
            return false;

        bool rosterFrozen = CPH.GetGlobalVar<bool?>(VAR_LOTAT_SESSION_ROSTER_FROZEN, false) ?? false;
        if (!rosterFrozen)
            return false;

        bool windowResolved = CPH.GetGlobalVar<bool?>(VAR_LOTAT_NODE_WINDOW_RESOLVED, false) ?? false;
        if (windowResolved)
            return false;

        return true;
    }

    private string GetSenderUser()
    {
        string user = "";
        if (CPH.TryGetArg("user", out user) && !string.IsNullOrWhiteSpace(user))
            return user;

        if (CPH.TryGetArg("userName", out user) && !string.IsNullOrWhiteSpace(user))
            return user;

        return "";
    }

    private string GetInvokedCommandText()
    {
        string text = "";
        if (CPH.TryGetArg("command", out text) && !string.IsNullOrWhiteSpace(text))
            return text;

        if (CPH.TryGetArg("message", out text) && !string.IsNullOrWhiteSpace(text))
            return text;

        if (CPH.TryGetArg("rawInput", out text) && !string.IsNullOrWhiteSpace(text))
            return text;

        return "";
    }

    private string NormalizeUser(string rawUser)
    {
        string normalized = (rawUser ?? "").Trim();
        if (normalized.StartsWith("@", StringComparison.Ordinal))
            normalized = normalized.Substring(1);

        return normalized.Trim().ToLowerInvariant();
    }

    private string NormalizeCommand(string rawCommand)
    {
        string normalized = (rawCommand ?? "").Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            return "";

        int firstSpace = normalized.IndexOf(' ');
        if (firstSpace >= 0)
            normalized = normalized.Substring(0, firstSpace);

        return normalized.Trim().ToLowerInvariant();
    }

    private List<string> ReadNormalizedRoster(string logPrefix)
    {
        string rosterJson = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_JOINED_ROSTER_JSON, false) ?? "[]";
        if (string.IsNullOrWhiteSpace(rosterJson))
            rosterJson = "[]";

        try
        {
            List<string> roster = JsonSerializer.Deserialize<List<string>>(rosterJson) ?? new List<string>();
            var normalizedRoster = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string entry in roster)
            {
                string normalized = NormalizeUser(entry);
                if (string.IsNullOrWhiteSpace(normalized))
                    continue;

                if (seen.Add(normalized))
                    normalizedRoster.Add(normalized);
            }

            return normalizedRoster;
        }
        catch (Exception ex)
        {
            CPH.LogWarn($"[{logPrefix}] Failed to parse joined roster JSON. Treating roster as empty for safety. exception='{ex.Message}'");
            return new List<string>();
        }
    }

    private List<string> ReadAllowedCommands(string logPrefix)
    {
        string allowedJson = CPH.GetGlobalVar<string>(VAR_LOTAT_NODE_ALLOWED_COMMANDS_JSON, false) ?? "[]";
        if (string.IsNullOrWhiteSpace(allowedJson))
            allowedJson = "[]";

        try
        {
            List<string> commands = JsonSerializer.Deserialize<List<string>>(allowedJson) ?? new List<string>();
            var normalized = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string entry in commands)
            {
                string command = NormalizeCommand(entry);
                if (string.IsNullOrWhiteSpace(command))
                    continue;

                if (seen.Add(command))
                    normalized.Add(command);
            }

            return normalized;
        }
        catch (Exception ex)
        {
            CPH.LogWarn($"[{logPrefix}] Failed to parse allowed commands JSON. Treating allowed commands as empty for safety. exception='{ex.Message}'");
            return new List<string>();
        }
    }

    private Dictionary<string, string> ReadVoteMap(string logPrefix, List<string> joinedRoster, List<string> allowedCommands)
    {
        string voteMapJson = CPH.GetGlobalVar<string>(VAR_LOTAT_VOTE_MAP_JSON, false) ?? "{}";
        if (string.IsNullOrWhiteSpace(voteMapJson))
            voteMapJson = "{}";

        var filteredMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var rosterSet = new HashSet<string>(joinedRoster, StringComparer.OrdinalIgnoreCase);
        var allowedSet = new HashSet<string>(allowedCommands, StringComparer.OrdinalIgnoreCase);

        try
        {
            Dictionary<string, string> rawMap = JsonSerializer.Deserialize<Dictionary<string, string>>(voteMapJson)
                ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<string, string> pair in rawMap)
            {
                string user = NormalizeUser(pair.Key);
                string command = NormalizeCommand(pair.Value);

                if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(command))
                    continue;

                if (!rosterSet.Contains(user))
                    continue;

                if (!allowedSet.Contains(command))
                    continue;

                filteredMap[user] = command;
            }
        }
        catch (Exception ex)
        {
            CPH.LogWarn($"[{logPrefix}] Failed to parse vote map JSON. Resetting vote map for safety. exception='{ex.Message}'");
        }

        return filteredMap;
    }
}
