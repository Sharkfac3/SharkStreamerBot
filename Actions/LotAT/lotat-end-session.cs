using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

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
    private const string STAGE_ENDED = "ended";
    private const string WINDOW_NONE = "none";

    private const string END_STATE_ZERO_JOIN = "zero_join";
    private const string END_STATE_UNRESOLVED = "unresolved";
    private const string END_STATE_FAULT_ABORT = "fault_abort";

    private const string RUNTIME_STORY_RELATIVE_PATH = "Creative/WorldBuilding/Storylines/loaded/current-story.json";
    private static readonly string[] RUNTIME_STORY_CANDIDATE_PATHS =
    {
        "/mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot/Creative/WorldBuilding/Storylines/loaded/current-story.json",
        @"C:\Users\sharkfac3\Workspace\coding\SharkStreamerBot\Creative\WorldBuilding\Storylines\loaded\current-story.json"
    };

    /*
     * Purpose:
     * - Centralized LotAT session teardown for all terminal outcomes.
     * - Emits one simple terminal chat message, disables all LotAT timers,
     *   clears active session/node/vote globals, and returns runtime stage to idle.
     *
     * Expected trigger/input:
     * - Follow any action that may leave LotAT in stage = ended.
     * - Optional input0 may provide an explicit end reason, but the preferred source
     *   is lotat_session_last_end_state written by the terminal-path action.
     *
     * Supported reasons:
     * - success / partial / failure (normal ending reached)
     * - zero_join
     * - unresolved
     * - fault_abort
     *
     * Guard behavior:
     * - Safe to call repeatedly.
     * - No-ops when stage is not ended and no explicit terminal reason is present.
     */
    public bool Execute()
    {
        const string logPrefix = "LotAT End Session";

        try
        {
            string sessionId = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_ID, false) ?? "";
            string stage = (CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_STAGE, false) ?? "").Trim();
            string endReason = ReadEndReason();

            if (!string.Equals(stage, STAGE_ENDED, StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(endReason))
                return true;

            if (string.IsNullOrWhiteSpace(endReason))
                endReason = InferEndingReasonFromCurrentNode(logPrefix);

            string terminalMessage = BuildTerminalMessage(logPrefix, endReason);
            DisableAllLotatTimers();

            if (!string.IsNullOrWhiteSpace(terminalMessage))
                CPH.SendMessage(terminalMessage);

            ResetLotatRuntimeToIdle();
            CPH.LogWarn($"[{logPrefix}] Cleanup completed. sessionId='{sessionId}', endReason='{endReason}'.");
            return true;
        }
        catch (Exception ex)
        {
            DisableAllLotatTimers();
            ResetLotatRuntimeToIdle();
            CPH.LogError($"[{logPrefix}] Unhandled exception during cleanup. Runtime was still forced back to idle. exception={ex}");
            return true;
        }
    }

    private string ReadEndReason()
    {
        string inputReason = "";
        if (CPH.TryGetArg("input0", out inputReason) && !string.IsNullOrWhiteSpace(inputReason))
            return inputReason.Trim().ToLowerInvariant();

        return (CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_LAST_END_STATE, false) ?? "").Trim().ToLowerInvariant();
    }

    private string BuildTerminalMessage(string logPrefix, string endReason)
    {
        string normalizedReason = (endReason ?? "").Trim().ToLowerInvariant();
        switch (normalizedReason)
        {
            case END_STATE_ZERO_JOIN:
                return "No crew joined the LotAT mission, so the run is ending for now.";

            case END_STATE_UNRESOLVED:
                return "The crew never locked in a decision, so this LotAT mission ends unresolved.";

            case END_STATE_FAULT_ABORT:
                return "LotAT hit a runtime problem and stopped safely.";

            case "success":
            case "partial":
            case "failure":
                return BuildEndingNodeMessage(logPrefix, normalizedReason);

            default:
                return BuildFallbackEndingMessage(logPrefix, normalizedReason);
        }
    }

    private string BuildEndingNodeMessage(string logPrefix, string endReason)
    {
        string currentNodeId = (CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_CURRENT_NODE_ID, false) ?? "").Trim();
        if (!TryLoadRuntimeStory(logPrefix, out RuntimeStoryDefinition story, out string resolvedPath, out string failureReason))
        {
            CPH.LogWarn($"[{logPrefix}] Could not load runtime story while formatting ending output. {failureReason}");
            return BuildGenericNormalEndingMessage(endReason);
        }

        RuntimeStoryNode node = FindNodeById(story, currentNodeId);
        if (node == null)
        {
            CPH.LogWarn($"[{logPrefix}] Could not resolve ending node while formatting ending output. nodeId='{currentNodeId}', path='{resolvedPath}'.");
            return BuildGenericNormalEndingMessage(endReason);
        }

        string title = (node.Title ?? "").Trim();
        if (string.IsNullOrWhiteSpace(title))
            return BuildGenericNormalEndingMessage(endReason);

        return $"LotAT ending reached: {title}.";
    }

    private string BuildFallbackEndingMessage(string logPrefix, string endReason)
    {
        string inferredReason = InferEndingReasonFromCurrentNode(logPrefix);
        if (string.Equals(inferredReason, END_STATE_FAULT_ABORT, StringComparison.OrdinalIgnoreCase))
            return "LotAT hit a runtime problem and stopped safely.";

        if (string.Equals(inferredReason, END_STATE_ZERO_JOIN, StringComparison.OrdinalIgnoreCase))
            return "No crew joined the LotAT mission, so the run is ending for now.";

        if (string.Equals(inferredReason, END_STATE_UNRESOLVED, StringComparison.OrdinalIgnoreCase))
            return "The crew never locked in a decision, so this LotAT mission ends unresolved.";

        if (string.Equals(inferredReason, "success", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(inferredReason, "partial", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(inferredReason, "failure", StringComparison.OrdinalIgnoreCase))
        {
            return BuildEndingNodeMessage(logPrefix, inferredReason);
        }

        if (!string.IsNullOrWhiteSpace(endReason))
            CPH.LogWarn($"[{logPrefix}] Unknown end reason '{endReason}'. Falling back to generic completion message.");

        return "LotAT session complete.";
    }

    private string BuildGenericNormalEndingMessage(string endReason)
    {
        switch ((endReason ?? "").Trim().ToLowerInvariant())
        {
            case "success":
                return "LotAT mission complete. The crew reached a successful ending.";

            case "partial":
                return "LotAT mission complete. The crew reached a partial ending.";

            case "failure":
                return "LotAT mission complete. The crew reached a failure ending.";

            default:
                return "LotAT mission complete.";
        }
    }

    private string InferEndingReasonFromCurrentNode(string logPrefix)
    {
        string currentNodeId = (CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_CURRENT_NODE_ID, false) ?? "").Trim();
        if (string.IsNullOrWhiteSpace(currentNodeId))
            return "";

        if (!TryLoadRuntimeStory(logPrefix, out RuntimeStoryDefinition story, out _, out _))
            return "";

        RuntimeStoryNode node = FindNodeById(story, currentNodeId);
        if (node == null)
            return "";

        return (node.EndState ?? "").Trim().ToLowerInvariant();
    }

    private void DisableAllLotatTimers()
    {
        CPH.DisableTimer(TIMER_LOTAT_JOIN_WINDOW);
        CPH.DisableTimer(TIMER_LOTAT_DECISION_WINDOW);
        CPH.DisableTimer(TIMER_LOTAT_COMMANDER_WINDOW);
        CPH.DisableTimer(TIMER_LOTAT_DICE_WINDOW);
    }

    private void ResetLotatRuntimeToIdle()
    {
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

    private bool TryLoadRuntimeStory(string logPrefix, out RuntimeStoryDefinition story, out string resolvedPath, out string failureReason)
    {
        story = null;
        resolvedPath = "";
        failureReason = "Unknown runtime story load failure.";

        resolvedPath = ResolveRuntimeStoryPath();
        if (string.IsNullOrWhiteSpace(resolvedPath))
        {
            failureReason = $"Runtime story file was not found. Expected relative path '{RUNTIME_STORY_RELATIVE_PATH}'. Candidate paths checked: {string.Join(" | ", BuildPathSearchLog())}";
            return false;
        }

        string json;
        try
        {
            json = File.ReadAllText(resolvedPath);
        }
        catch (Exception ex)
        {
            failureReason = $"Runtime story file exists but could not be read. path='{resolvedPath}', exception='{ex.Message}'";
            return false;
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            failureReason = $"Runtime story file is empty. path='{resolvedPath}'";
            return false;
        }

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            story = JsonSerializer.Deserialize<RuntimeStoryDefinition>(json, options);
        }
        catch (Exception ex)
        {
            failureReason = $"Runtime story JSON parse failed. path='{resolvedPath}', exception='{ex.Message}'";
            return false;
        }

        if (story == null || story.Nodes == null || story.Nodes.Count <= 0)
        {
            failureReason = $"Runtime story is minimally unusable. path='{resolvedPath}'.";
            return false;
        }

        return true;
    }

    private RuntimeStoryNode FindNodeById(RuntimeStoryDefinition story, string nodeId)
    {
        if (story?.Nodes == null || string.IsNullOrWhiteSpace(nodeId))
            return null;

        foreach (RuntimeStoryNode node in story.Nodes)
        {
            if (node == null)
                continue;

            if (string.Equals((node.NodeId ?? "").Trim(), nodeId.Trim(), StringComparison.Ordinal))
                return node;
        }

        return null;
    }

    private string ResolveRuntimeStoryPath()
    {
        foreach (string candidatePath in BuildPathSearchLog())
        {
            try
            {
                if (File.Exists(candidatePath))
                    return candidatePath;
            }
            catch
            {
            }
        }

        return "";
    }

    private List<string> BuildPathSearchLog()
    {
        var candidates = new List<string>();

        foreach (string configuredCandidate in RUNTIME_STORY_CANDIDATE_PATHS)
            candidates.Add(configuredCandidate);

        try
        {
            string currentDirectory = Environment.CurrentDirectory ?? "";
            if (!string.IsNullOrWhiteSpace(currentDirectory))
                candidates.Add(Path.Combine(currentDirectory, RUNTIME_STORY_RELATIVE_PATH));
        }
        catch
        {
        }

        try
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? "";
            if (!string.IsNullOrWhiteSpace(baseDirectory))
                candidates.Add(Path.Combine(baseDirectory, RUNTIME_STORY_RELATIVE_PATH));
        }
        catch
        {
        }

        return DeduplicatePaths(candidates);
    }

    private List<string> DeduplicatePaths(List<string> candidates)
    {
        var unique = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (string candidate in candidates)
        {
            if (string.IsNullOrWhiteSpace(candidate))
                continue;

            string normalized = candidate.Trim();
            if (seen.Add(normalized))
                unique.Add(normalized);
        }

        return unique;
    }

    private sealed class RuntimeStoryDefinition
    {
        [JsonPropertyName("nodes")]
        public List<RuntimeStoryNode> Nodes { get; set; }
    }

    private sealed class RuntimeStoryNode
    {
        [JsonPropertyName("node_id")]
        public string NodeId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("end_state")]
        public string EndState { get; set; }
    }
}
