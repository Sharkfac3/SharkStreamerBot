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

    private const string STAGE_JOIN_OPEN = "join_open";
    private const string WINDOW_JOIN = "join";

    /*
     * Purpose:
     * - Handle !join during the LotAT join window.
     * - Build a session-scoped participant roster using lowercase username/login strings.
     *
     * Expected trigger/input:
     * - Chat command trigger for !join.
     * - Reads the sender from Streamer.bot args, primarily `user`.
     *
     * Required runtime variables:
     * - Reads: lotat_active, lotat_session_stage, lotat_session_roster_frozen,
     *   lotat_session_joined_roster_json, lotat_session_joined_count,
     *   lotat_node_active_window, lotat_node_window_resolved.
     * - Writes: lotat_session_joined_roster_json, lotat_session_joined_count.
     *
     * Key outputs/side effects:
     * - Accepts joins only during join_open.
     * - Deduplicates by lowercase username/login string.
     * - Ignores duplicate joins and out-of-phase joins without mutating runtime state.
     *
     * Operator notes:
     * - Wire only !join to this action.
     * - This script intentionally does not handle decision commands or late joins.
     */
    public bool Execute()
    {
        const string logPrefix = "LotAT Join";

        try
        {
            if (!IsJoinWindowOpen())
                return true;

            string rawUser = GetSenderUser();
            string participantKey = NormalizeParticipantIdentity(rawUser);
            if (string.IsNullOrWhiteSpace(participantKey))
            {
                CPH.LogWarn($"[{logPrefix}] Ignored !join because no usable user/login value was present.");
                return true;
            }

            List<string> roster = ReadRoster(logPrefix);
            if (roster.Contains(participantKey))
            {
                // Lightweight duplicate acknowledgement is okay, but keep it quiet.
                CPH.SendMessage($"@{rawUser.Trim()} you're already in the LotAT crew.");
                return true;
            }

            roster.Add(participantKey);
            string updatedRosterJson = JsonSerializer.Serialize(roster);
            int joinedCount = roster.Count;
            string sessionId = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_ID, false) ?? "";

            CPH.SetGlobalVar(VAR_LOTAT_SESSION_JOINED_ROSTER_JSON, updatedRosterJson, false);
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_JOINED_COUNT, joinedCount, false);

            CPH.LogWarn($"[{logPrefix}] Join accepted. sessionId='{sessionId}', participant='{participantKey}', joinedCount={joinedCount}.");
            CPH.SendMessage($"@{rawUser.Trim()} joined the LotAT crew. Crew count: {joinedCount}.");
            return true;
        }
        catch (Exception ex)
        {
            CPH.LogError($"[{logPrefix}] Unhandled exception while processing !join: {ex}");
            return true;
        }
    }

    private bool IsJoinWindowOpen()
    {
        bool active = CPH.GetGlobalVar<bool?>(VAR_LOTAT_ACTIVE, false) ?? false;
        if (!active)
            return false;

        string stage = (CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_STAGE, false) ?? "").Trim();
        if (!string.Equals(stage, STAGE_JOIN_OPEN, StringComparison.OrdinalIgnoreCase))
            return false;

        bool rosterFrozen = CPH.GetGlobalVar<bool?>(VAR_LOTAT_SESSION_ROSTER_FROZEN, false) ?? false;
        if (rosterFrozen)
            return false;

        string activeWindow = (CPH.GetGlobalVar<string>(VAR_LOTAT_NODE_ACTIVE_WINDOW, false) ?? "").Trim();
        if (!string.Equals(activeWindow, WINDOW_JOIN, StringComparison.OrdinalIgnoreCase))
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

        // Practical fallback in case a specific Streamer.bot trigger exposes a different field.
        if (CPH.TryGetArg("userName", out user) && !string.IsNullOrWhiteSpace(user))
            return user;

        return "";
    }

    private string NormalizeParticipantIdentity(string rawUser)
    {
        string normalized = (rawUser ?? "").Trim();
        if (normalized.StartsWith("@", StringComparison.Ordinal))
            normalized = normalized.Substring(1);

        return normalized.Trim().ToLowerInvariant();
    }

    private List<string> ReadRoster(string logPrefix)
    {
        string rosterJson = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_JOINED_ROSTER_JSON, false) ?? "[]";
        if (string.IsNullOrWhiteSpace(rosterJson))
            rosterJson = "[]";

        try
        {
            List<string> roster = JsonSerializer.Deserialize<List<string>>(rosterJson);
            if (roster == null)
                return new List<string>();

            var normalizedRoster = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string entry in roster)
            {
                string normalized = NormalizeParticipantIdentity(entry);
                if (string.IsNullOrWhiteSpace(normalized))
                    continue;

                if (seen.Add(normalized))
                    normalizedRoster.Add(normalized);
            }

            return normalizedRoster;
        }
        catch (Exception ex)
        {
            CPH.LogWarn($"[{logPrefix}] Failed to parse joined roster JSON. Resetting roster to empty for safety. exception='{ex.Message}'");
            return new List<string>();
        }
    }
}
