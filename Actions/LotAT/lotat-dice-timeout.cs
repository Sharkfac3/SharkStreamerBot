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
    private const string VAR_LOTAT_SESSION_CURRENT_NODE_ID = "lotat_session_current_node_id";
    private const string VAR_LOTAT_NODE_ACTIVE_WINDOW = "lotat_node_active_window";
    private const string VAR_LOTAT_NODE_WINDOW_RESOLVED = "lotat_node_window_resolved";
    private const string VAR_LOTAT_NODE_ALLOWED_COMMANDS_JSON = "lotat_node_allowed_commands_json";
    private const string VAR_LOTAT_VOTE_MAP_JSON = "lotat_vote_map_json";
    private const string VAR_LOTAT_VOTE_VALID_COUNT = "lotat_vote_valid_count";
    private const string VAR_LOTAT_NODE_COMMANDER_NAME = "lotat_node_commander_name";
    private const string VAR_LOTAT_NODE_COMMANDER_TARGET_USER = "lotat_node_commander_target_user";
    private const string VAR_LOTAT_NODE_COMMANDER_ALLOWED_COMMANDS_JSON = "lotat_node_commander_allowed_commands_json";
    private const string VAR_LOTAT_NODE_DICE_SUCCESS_THRESHOLD = "lotat_node_dice_success_threshold";
    private const string VAR_LOTAT_SESSION_LAST_END_STATE = "lotat_session_last_end_state";

    private const string TIMER_LOTAT_DICE_WINDOW = "LotAT - Dice Window";
    private const string TIMER_LOTAT_DECISION_WINDOW = "LotAT - Decision Window";

    private const string STAGE_DICE_OPEN = "dice_open";
    private const string STAGE_DECISION_OPEN = "decision_open";
    private const string STAGE_ENDED = "ended";
    private const string WINDOW_NONE = "none";
    private const string WINDOW_DICE = "dice";
    private const string WINDOW_DECISION = "decision";

    private const string END_STATE_FAULT_ABORT = "fault_abort";

    private const int DECISION_WINDOW_SECONDS = 120;

    private const string RUNTIME_STORY_RELATIVE_PATH = "Creative/WorldBuilding/Storylines/loaded/current-story.json";
    private static readonly string[] RUNTIME_STORY_CANDIDATE_PATHS =
    {
        "/mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot/Creative/WorldBuilding/Storylines/loaded/current-story.json",
        @"C:\Users\sharkfac3\Workspace\coding\SharkStreamerBot\Creative\WorldBuilding\Storylines\loaded\current-story.json"
    };

    /*
     * Purpose:
     * - Dice-window timer-end handler for LotAT v1.
     * - No-ops safely on stale timer fires.
     * - On a relevant timeout, emits authored failure_text and continues into the normal decision window.
     *
     * Expected trigger/input:
     * - Streamer.bot timer-end trigger for "LotAT - Dice Window"
     *
     * Required runtime variables:
     * - Reads: active/stage/window guards, current node id, and dice threshold state.
     * - Writes: dice resolved state and the normal decision-window globals.
     *
     * Operator notes:
     * - Wire the timer directly to this script.
     * - This action opens the decision window itself on timeout, so it does not require lotat-node-enter.cs after it.
     */
    public bool Execute()
    {
        const string logPrefix = "LotAT Dice Timeout";

        try
        {
            string sessionId = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_ID, false) ?? "";
            if (!IsRelevantDiceTimerFire(logPrefix, sessionId))
            {
                CPH.DisableTimer(TIMER_LOTAT_DICE_WINDOW);
                return true;
            }

            CPH.DisableTimer(TIMER_LOTAT_DICE_WINDOW);
            CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, true, false);

            string currentNodeId = (CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_CURRENT_NODE_ID, false) ?? "").Trim();
            int thresholdFromState = CPH.GetGlobalVar<int?>(VAR_LOTAT_NODE_DICE_SUCCESS_THRESHOLD, false) ?? 0;
            if (string.IsNullOrWhiteSpace(currentNodeId) || thresholdFromState <= 0)
                return true;

            if (!TryLoadRuntimeStory(logPrefix, out RuntimeStoryDefinition story, out string resolvedPath, out string loadFailure))
            {
                FailClosed(logPrefix, sessionId, $"Runtime story load failed while processing dice timeout. {loadFailure}");
                return true;
            }

            RuntimeStoryNode node = FindNodeById(story, currentNodeId);
            if (node == null)
            {
                FailClosed(logPrefix, sessionId, $"Current node could not be resolved while handling dice timeout. nodeId='{currentNodeId}', path='{resolvedPath}'.");
                return true;
            }

            if (node.DiceHook == null || !node.DiceHook.Enabled)
            {
                CPH.LogWarn($"[{logPrefix}] Dice timeout ignored because the current node no longer has an active dice_hook payload. nodeId='{currentNodeId}'.");
                return true;
            }

            int authoredThreshold = node.DiceHook.SuccessThreshold;
            if (authoredThreshold <= 0 || authoredThreshold != thresholdFromState)
            {
                CPH.LogWarn($"[{logPrefix}] Dice timeout ignored because runtime dice threshold state no longer matches the authored node. nodeId='{currentNodeId}', runtimeThreshold={thresholdFromState}, authoredThreshold={authoredThreshold}.");
                return true;
            }

            List<string> allowedDecisionCommands = DeriveAllowedDecisionCommands(node, out string commandFailure);
            if (!string.IsNullOrWhiteSpace(commandFailure))
            {
                FailClosed(logPrefix, sessionId, $"Could not open decision window after dice timeout. {commandFailure}");
                return true;
            }

            string failureText = (node.DiceHook.FailureText ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(failureText))
                CPH.SendMessage(failureText);

            OpenDecisionWindow(logPrefix, sessionId, currentNodeId, allowedDecisionCommands, node);
            return true;
        }
        catch (Exception ex)
        {
            string sessionId = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_ID, false) ?? "";
            FailClosed(logPrefix, sessionId, $"Unhandled exception during dice timeout: {ex}");
            return true;
        }
    }

    private bool IsRelevantDiceTimerFire(string logPrefix, string sessionId)
    {
        bool active = CPH.GetGlobalVar<bool?>(VAR_LOTAT_ACTIVE, false) ?? false;
        if (!active)
        {
            CPH.LogWarn($"[{logPrefix}] Ignoring stale dice timer because LotAT is not active.");
            return false;
        }

        string stage = (CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_STAGE, false) ?? "").Trim();
        if (!string.Equals(stage, STAGE_DICE_OPEN, StringComparison.OrdinalIgnoreCase))
        {
            CPH.LogWarn($"[{logPrefix}] Ignoring stale dice timer because stage is '{stage}', not '{STAGE_DICE_OPEN}'. sessionId='{sessionId}'.");
            return false;
        }

        string activeWindow = (CPH.GetGlobalVar<string>(VAR_LOTAT_NODE_ACTIVE_WINDOW, false) ?? "").Trim();
        if (!string.Equals(activeWindow, WINDOW_DICE, StringComparison.OrdinalIgnoreCase))
        {
            CPH.LogWarn($"[{logPrefix}] Ignoring stale dice timer because activeWindow is '{activeWindow}', not '{WINDOW_DICE}'. sessionId='{sessionId}'.");
            return false;
        }

        bool windowResolved = CPH.GetGlobalVar<bool?>(VAR_LOTAT_NODE_WINDOW_RESOLVED, false) ?? false;
        if (windowResolved)
        {
            CPH.LogWarn($"[{logPrefix}] Ignoring stale dice timer because the dice window is already resolved. sessionId='{sessionId}'.");
            return false;
        }

        int threshold = CPH.GetGlobalVar<int?>(VAR_LOTAT_NODE_DICE_SUCCESS_THRESHOLD, false) ?? 0;
        if (threshold <= 0)
        {
            CPH.LogWarn($"[{logPrefix}] Ignoring stale dice timer because no active threshold is stored. sessionId='{sessionId}'.");
            return false;
        }

        return true;
    }

    private List<string> DeriveAllowedDecisionCommands(RuntimeStoryNode node, out string failureReason)
    {
        failureReason = "";
        var commands = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (node == null)
        {
            failureReason = "Current node was null while deriving decision commands.";
            return commands;
        }

        if (node.Choices == null || node.Choices.Count <= 0 || node.Choices.Count > 2)
        {
            failureReason = $"Stage node '{node.NodeId}' has invalid choice count for decision open.";
            return commands;
        }

        foreach (RuntimeStoryChoice choice in node.Choices)
        {
            if (choice == null || string.IsNullOrWhiteSpace(choice.Command))
            {
                failureReason = $"Stage node '{node.NodeId}' has a choice with no command.";
                return new List<string>();
            }

            string command = NormalizeCommand(choice.Command);
            if (seen.Add(command))
                commands.Add(command);
        }

        if (commands.Count <= 0)
            failureReason = $"Stage node '{node.NodeId}' produced no decision commands.";

        return commands;
    }

    private void OpenDecisionWindow(string logPrefix, string sessionId, string currentNodeId, List<string> allowedDecisionCommands, RuntimeStoryNode node)
    {
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_STAGE, STAGE_DECISION_OPEN, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ACTIVE_WINDOW, WINDOW_DECISION, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, false, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ALLOWED_COMMANDS_JSON, JsonSerializer.Serialize(allowedDecisionCommands), false);
        CPH.SetGlobalVar(VAR_LOTAT_VOTE_MAP_JSON, "{}", false);
        CPH.SetGlobalVar(VAR_LOTAT_VOTE_VALID_COUNT, 0, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_NAME, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_TARGET_USER, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_ALLOWED_COMMANDS_JSON, "[]", false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_DICE_SUCCESS_THRESHOLD, 0, false);

        CPH.SendMessage(BuildChoicePrompt(node));

        // The decision window uses the fixed runtime-owned v1 default of 120 seconds.
        // Reset by disable -> enable so the timer can use its configured UI interval safely.
        CPH.DisableTimer(TIMER_LOTAT_DECISION_WINDOW);
        CPH.EnableTimer(TIMER_LOTAT_DECISION_WINDOW);

        CPH.LogWarn($"[{logPrefix}] Dice window timed out and handed off to the normal decision window. sessionId='{sessionId}', nodeId='{currentNodeId}', seconds={DECISION_WINDOW_SECONDS}, allowedCommands='{string.Join(",", allowedDecisionCommands)}'.");
    }

    private void FailClosed(string logPrefix, string sessionId, string detail)
    {
        CPH.LogError($"[{logPrefix}] Fail-closed. sessionId='{sessionId}'. {detail}");
        CPH.DisableTimer(TIMER_LOTAT_DICE_WINDOW);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_STAGE, STAGE_ENDED, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ACTIVE_WINDOW, WINDOW_NONE, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, true, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ALLOWED_COMMANDS_JSON, "[]", false);
        CPH.SetGlobalVar(VAR_LOTAT_VOTE_MAP_JSON, "{}", false);
        CPH.SetGlobalVar(VAR_LOTAT_VOTE_VALID_COUNT, 0, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_NAME, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_TARGET_USER, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_ALLOWED_COMMANDS_JSON, "[]", false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_DICE_SUCCESS_THRESHOLD, 0, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_LAST_END_STATE, END_STATE_FAULT_ABORT, false);
    }

    private string BuildChoicePrompt(RuntimeStoryNode node)
    {
        if (node?.Choices == null || node.Choices.Count <= 0)
            return $"Choose now ({DECISION_WINDOW_SECONDS}s).";

        var parts = new List<string>();
        foreach (RuntimeStoryChoice choice in node.Choices)
        {
            if (choice == null)
                continue;

            string command = (choice.Command ?? "").Trim();
            string label = (choice.Label ?? "").Trim();
            if (string.IsNullOrWhiteSpace(command))
                continue;

            if (string.IsNullOrWhiteSpace(label))
                parts.Add(command);
            else
                parts.Add($"{command} = {label}");
        }

        return $"Choose now ({DECISION_WINDOW_SECONDS}s): {string.Join(" | ", parts)}";
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
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            story = JsonSerializer.Deserialize<RuntimeStoryDefinition>(json, options);
        }
        catch (Exception ex)
        {
            failureReason = $"Runtime story JSON parse failed. path='{resolvedPath}', exception='{ex.Message}'";
            return false;
        }

        if (story == null || story.Nodes == null || story.Nodes.Count <= 0)
        {
            failureReason = $"Runtime story is minimally unusable. path='{resolvedPath}'";
            return false;
        }

        CPH.LogWarn($"[{logPrefix}] Runtime story loaded for dice-timeout handoff. storyId='{story.StoryId}', path='{resolvedPath}'.");
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
        [JsonPropertyName("story_id")]
        public string StoryId { get; set; }

        [JsonPropertyName("nodes")]
        public List<RuntimeStoryNode> Nodes { get; set; }
    }

    private sealed class RuntimeStoryNode
    {
        [JsonPropertyName("node_id")]
        public string NodeId { get; set; }

        [JsonPropertyName("dice_hook")]
        public RuntimeDiceHook DiceHook { get; set; }

        [JsonPropertyName("choices")]
        public List<RuntimeStoryChoice> Choices { get; set; }
    }

    private sealed class RuntimeDiceHook
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("success_threshold")]
        public int SuccessThreshold { get; set; }

        [JsonPropertyName("failure_text")]
        public string FailureText { get; set; }
    }

    private sealed class RuntimeStoryChoice
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("command")]
        public string Command { get; set; }
    }
}
