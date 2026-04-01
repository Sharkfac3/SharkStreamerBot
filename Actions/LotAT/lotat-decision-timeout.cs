using System;

public class CPHInline
{
    // -------------------------------------------------
    // LotAT shared runtime constants
    // Keep these names synchronized with Actions/SHARED-CONSTANTS.md.
    // -------------------------------------------------
    private const string VAR_LOTAT_ACTIVE = "lotat_active";
    private const string VAR_LOTAT_SESSION_ID = "lotat_session_id";
    private const string VAR_LOTAT_SESSION_STAGE = "lotat_session_stage";
    private const string VAR_LOTAT_SESSION_STORY_ID = "lotat_session_story_id";
    private const string VAR_LOTAT_SESSION_CURRENT_NODE_ID = "lotat_session_current_node_id";
    private const string VAR_LOTAT_SESSION_CHAOS_TOTAL = "lotat_session_chaos_total";
    private const string VAR_LOTAT_SESSION_ROSTER_FROZEN = "lotat_session_roster_frozen";
    private const string VAR_LOTAT_SESSION_JOINED_ROSTER_JSON = "lotat_session_joined_roster_json";
    private const string VAR_LOTAT_SESSION_JOINED_COUNT = "lotat_session_joined_count";
    private const string VAR_LOTAT_NODE_ACTIVE_WINDOW = "lotat_node_active_window";
    private const string VAR_LOTAT_NODE_WINDOW_RESOLVED = "lotat_node_window_resolved";
    private const string VAR_LOTAT_NODE_ALLOWED_COMMANDS_JSON = "lotat_node_allowed_commands_json";
    private const string VAR_LOTAT_VOTE_MAP_JSON = "lotat_vote_map_json";
    private const string VAR_LOTAT_VOTE_VALID_COUNT = "lotat_vote_valid_count";
    private const string VAR_LOTAT_NODE_COMMANDER_NAME = "lotat_node_commander_name";
    private const string VAR_LOTAT_NODE_COMMANDER_TARGET_USER = "lotat_node_commander_target_user";
    private const string VAR_LOTAT_NODE_COMMANDER_ALLOWED_COMMANDS_JSON = "lotat_node_commander_allowed_commands_json";
    private const string VAR_LOTAT_NODE_DICE_SUCCESS_THRESHOLD = "lotat_node_dice_success_threshold";
    private const string VAR_LOTAT_SESSION_LAST_CHOICE_ID = "lotat_session_last_choice_id";
    private const string VAR_LOTAT_SESSION_LAST_END_STATE = "lotat_session_last_end_state";

    private const string TIMER_LOTAT_JOIN_WINDOW = "LotAT - Join Window";
    private const string TIMER_LOTAT_DECISION_WINDOW = "LotAT - Decision Window";
    private const string TIMER_LOTAT_COMMANDER_WINDOW = "LotAT - Commander Window";
    private const string TIMER_LOTAT_DICE_WINDOW = "LotAT - Dice Window";

    private const string STAGE_IDLE = "idle";
    private const string STAGE_DECISION_OPEN = "decision_open";
    private const string STAGE_ENDED = "ended";
    private const string WINDOW_NONE = "none";
    private const string WINDOW_DECISION = "decision";

    private const string END_STATE_UNRESOLVED = "unresolved";
    private const string END_STATE_FAULT_ABORT = "fault_abort";

    /*
     * Purpose:
     * - Handle the LotAT decision-window timer end.
     * - No-op safely on stale timer fires.
     * - End unresolved on a zero-valid-vote close.
     * - Otherwise mark the node ready for the single shared resolution path.
     *
     * Expected trigger/input:
     * - Streamer.bot timer-end trigger for "LotAT - Decision Window".
     *
     * Operator notes:
     * - Wire lotat-decision-timeout.cs first in the timer action group.
     * - Put lotat-decision-resolve.cs immediately after it, then lotat-node-enter.cs.
     *   That gives the timeout path and the early-close path one shared resolution implementation.
     */
    public bool Execute()
    {
        const string logPrefix = "LotAT Decision Timeout";

        try
        {
            string sessionId = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_ID, false) ?? "";
            if (!IsRelevantDecisionTimerFire(logPrefix, sessionId))
            {
                CPH.DisableTimer(TIMER_LOTAT_DECISION_WINDOW);
                return true;
            }

            CPH.DisableTimer(TIMER_LOTAT_DECISION_WINDOW);

            int validCount = CPH.GetGlobalVar<int?>(VAR_LOTAT_VOTE_VALID_COUNT, false) ?? 0;
            if (validCount <= 0)
            {
                CPH.LogWarn($"[{logPrefix}] Decision timer closed with zero valid votes. sessionId='{sessionId}'.");
                EndSessionUnresolved(logPrefix, sessionId);
                return true;
            }

            CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, true, false);
            CPH.LogWarn($"[{logPrefix}] Decision timer closed with recorded votes. sessionId='{sessionId}', validCount={validCount}. Routing through shared resolution path.");
            return true;
        }
        catch (Exception ex)
        {
            string sessionId = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_ID, false) ?? "";
            CPH.LogError($"[{logPrefix}] Unhandled exception during decision timeout. sessionId='{sessionId}', exception={ex}");
            MarkFaultAbortState(logPrefix, sessionId);
            return true;
        }
    }

    private bool IsRelevantDecisionTimerFire(string logPrefix, string sessionId)
    {
        bool active = CPH.GetGlobalVar<bool?>(VAR_LOTAT_ACTIVE, false) ?? false;
        if (!active)
        {
            CPH.LogWarn($"[{logPrefix}] Ignoring stale decision timer because LotAT is not active.");
            return false;
        }

        string stage = (CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_STAGE, false) ?? "").Trim();
        if (!string.Equals(stage, STAGE_DECISION_OPEN, StringComparison.OrdinalIgnoreCase))
        {
            CPH.LogWarn($"[{logPrefix}] Ignoring stale decision timer because stage is '{stage}', not '{STAGE_DECISION_OPEN}'. sessionId='{sessionId}'.");
            return false;
        }

        string activeWindow = (CPH.GetGlobalVar<string>(VAR_LOTAT_NODE_ACTIVE_WINDOW, false) ?? "").Trim();
        if (!string.Equals(activeWindow, WINDOW_DECISION, StringComparison.OrdinalIgnoreCase))
        {
            CPH.LogWarn($"[{logPrefix}] Ignoring stale decision timer because activeWindow is '{activeWindow}', not '{WINDOW_DECISION}'. sessionId='{sessionId}'.");
            return false;
        }

        bool windowResolved = CPH.GetGlobalVar<bool?>(VAR_LOTAT_NODE_WINDOW_RESOLVED, false) ?? false;
        if (windowResolved)
        {
            CPH.LogWarn($"[{logPrefix}] Ignoring stale decision timer because the decision window is already marked resolved. sessionId='{sessionId}'.");
            return false;
        }

        return true;
    }

    private void EndSessionUnresolved(string logPrefix, string sessionId)
    {
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_STAGE, STAGE_ENDED, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ACTIVE_WINDOW, WINDOW_NONE, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, true, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_LAST_END_STATE, END_STATE_UNRESOLVED, false);
        CPH.LogWarn($"[{logPrefix}] Unresolved zero-vote timeout marked for centralized cleanup. sessionId='{sessionId}'.");
    }

    private void MarkFaultAbortState(string logPrefix, string sessionId)
    {
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_STAGE, STAGE_ENDED, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ACTIVE_WINDOW, WINDOW_NONE, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, true, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_LAST_END_STATE, END_STATE_FAULT_ABORT, false);
        CPH.LogWarn($"[{logPrefix}] Fault-abort marked for centralized cleanup. sessionId='{sessionId}'.");
    }

    private void ResetLotatRuntimeToIdle()
    {
        CPH.DisableTimer(TIMER_LOTAT_JOIN_WINDOW);
        CPH.DisableTimer(TIMER_LOTAT_DECISION_WINDOW);
        CPH.DisableTimer(TIMER_LOTAT_COMMANDER_WINDOW);
        CPH.DisableTimer(TIMER_LOTAT_DICE_WINDOW);

        CPH.SetGlobalVar(VAR_LOTAT_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_ID, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_STAGE, STAGE_IDLE, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_STORY_ID, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_CURRENT_NODE_ID, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_CHAOS_TOTAL, 0, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_ROSTER_FROZEN, false, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_JOINED_ROSTER_JSON, "[]", false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_JOINED_COUNT, 0, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ACTIVE_WINDOW, WINDOW_NONE, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, false, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ALLOWED_COMMANDS_JSON, "[]", false);
        CPH.SetGlobalVar(VAR_LOTAT_VOTE_MAP_JSON, "{}", false);
        CPH.SetGlobalVar(VAR_LOTAT_VOTE_VALID_COUNT, 0, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_NAME, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_TARGET_USER, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_ALLOWED_COMMANDS_JSON, "[]", false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_DICE_SUCCESS_THRESHOLD, 0, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_LAST_CHOICE_ID, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_LAST_END_STATE, "", false);
    }
}
