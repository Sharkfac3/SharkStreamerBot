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

    private const string TIMER_LOTAT_COMMANDER_WINDOW = "LotAT - Commander Window";
    private const string TIMER_LOTAT_DECISION_WINDOW = "LotAT - Decision Window";

    private const string STAGE_COMMANDER_OPEN = "commander_open";
    private const string STAGE_DECISION_OPEN = "decision_open";
    private const string WINDOW_COMMANDER = "commander";
    private const string WINDOW_DECISION = "decision";

    private const int DECISION_WINDOW_SECONDS = 120;

    private const string RUNTIME_STORY_RELATIVE_PATH = "Creative/WorldBuilding/Storylines/loaded/current-story.json";
    private static readonly string[] RUNTIME_STORY_CANDIDATE_PATHS =
    {
        "/mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot/Creative/WorldBuilding/Storylines/loaded/current-story.json",
        @"C:\Users\sharkfac3\Workspace\coding\SharkStreamerBot\Creative\WorldBuilding\Storylines\loaded\current-story.json"
    };

    /*
     * Purpose:
     * - Shared commander-window input handler for LotAT v1.
     * - Listens to the existing commander command family during commander_open,
     *   accepts only the snapshotted assigned commander user, resolves on the first valid command,
     *   emits authored success_text, and then opens the normal decision window for the same node.
     *
     * Expected trigger/input:
     * - Shared Streamer.bot chat-command routing for:
     *   !stretch, !shrimp, !hydrate, !orb, !checkchat, !toad
     * - Reads sender from `user` / `userName`
     * - Reads invoked command text from `command`, `message`, or `rawInput`
     *
     * Required runtime variables:
     * - Reads: active/stage/window guards, current node id, snapshotted commander target user,
     *   commander name, and allowed commander command list.
     * - Writes: commander window resolved state, commander timer stop, and the normal decision-window globals.
     *
     * Key outputs/side effects:
     * - Ignores all input outside commander_open.
     * - Ignores non-assigned users silently.
     * - Ignores invalid commander commands silently.
     * - On first valid assigned-commander input, emits the authored success_text and continues into the normal decision window.
     *
     * Operator notes:
     * - Route all six existing commander commands to this single action.
     * - This action opens the decision window itself on success, so it does not require lotat-node-enter.cs after it.
     */
    public bool Execute()
    {
        const string logPrefix = "LotAT Commander Input";

        try
        {
            if (!IsCommanderWindowOpen())
                return true;

            string sender = NormalizeUser(GetSenderUser());
            if (string.IsNullOrWhiteSpace(sender))
                return true;

            string command = NormalizeCommand(GetInvokedCommandText());
            if (string.IsNullOrWhiteSpace(command))
                return true;

            string snapshottedTargetUser = NormalizeUser(CPH.GetGlobalVar<string>(VAR_LOTAT_NODE_COMMANDER_TARGET_USER, false) ?? "");
            if (string.IsNullOrWhiteSpace(snapshottedTargetUser))
                return true;

            // Silent ignore for anyone except the snapshotted commander target.
            if (!string.Equals(sender, snapshottedTargetUser, StringComparison.OrdinalIgnoreCase))
                return true;

            string commanderName = (CPH.GetGlobalVar<string>(VAR_LOTAT_NODE_COMMANDER_NAME, false) ?? "").Trim();
            List<string> allowedCommanderCommands = ReadAllowedCommanderCommands(logPrefix);
            if (allowedCommanderCommands.Count <= 0 || !allowedCommanderCommands.Contains(command))
                return true;

            string currentNodeId = (CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_CURRENT_NODE_ID, false) ?? "").Trim();
            if (string.IsNullOrWhiteSpace(currentNodeId))
                return true;

            if (!TryLoadRuntimeStory(logPrefix, out RuntimeStoryDefinition story, out string resolvedPath, out string loadFailure))
            {
                CPH.LogError($"[{logPrefix}] Runtime story load failed while resolving commander success. {loadFailure}");
                return true;
            }

            RuntimeStoryNode node = FindNodeById(story, currentNodeId);
            if (node == null)
            {
                CPH.LogError($"[{logPrefix}] Current node could not be resolved while handling commander success. nodeId='{currentNodeId}', path='{resolvedPath}'.");
                return true;
            }

            if (node.CommanderMoment == null || !node.CommanderMoment.Enabled)
            {
                CPH.LogWarn($"[{logPrefix}] Commander input ignored because the current node no longer has an active commander payload. nodeId='{currentNodeId}'.");
                return true;
            }

            // Re-validate the authored commander identity so stale or mismatched state cannot satisfy a later node.
            string authoredCommanderName = (node.CommanderMoment.Commander ?? "").Trim();
            if (!string.Equals(authoredCommanderName, commanderName, StringComparison.OrdinalIgnoreCase))
            {
                CPH.LogWarn($"[{logPrefix}] Commander input ignored because runtime commander state no longer matches the authored node commander. nodeId='{currentNodeId}', runtime='{commanderName}', authored='{authoredCommanderName}'.");
                return true;
            }

            List<string> allowedDecisionCommands = DeriveAllowedDecisionCommands(node, out string commandFailure);
            if (!string.IsNullOrWhiteSpace(commandFailure))
            {
                CPH.LogError($"[{logPrefix}] Could not open decision window after commander success. {commandFailure}");
                return true;
            }

            CPH.DisableTimer(TIMER_LOTAT_COMMANDER_WINDOW);
            CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, true, false);

            string successText = (node.CommanderMoment.SuccessText ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(successText))
                CPH.SendMessage(successText);

            OpenDecisionWindow(logPrefix, currentNodeId, allowedDecisionCommands, node);

            string sessionId = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_ID, false) ?? "";
            CPH.LogWarn($"[{logPrefix}] Commander moment succeeded. sessionId='{sessionId}', nodeId='{currentNodeId}', commander='{commanderName}', user='{sender}', command='{command}'.");
            return true;
        }
        catch (Exception ex)
        {
            string sessionId = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_ID, false) ?? "";
            CPH.LogError($"[LotAT Commander Input] Unhandled exception while processing commander input. sessionId='{sessionId}', exception={ex}");
            return true;
        }
    }

    private bool IsCommanderWindowOpen()
    {
        bool active = CPH.GetGlobalVar<bool?>(VAR_LOTAT_ACTIVE, false) ?? false;
        if (!active)
            return false;

        string stage = (CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_STAGE, false) ?? "").Trim();
        if (!string.Equals(stage, STAGE_COMMANDER_OPEN, StringComparison.OrdinalIgnoreCase))
            return false;

        string activeWindow = (CPH.GetGlobalVar<string>(VAR_LOTAT_NODE_ACTIVE_WINDOW, false) ?? "").Trim();
        if (!string.Equals(activeWindow, WINDOW_COMMANDER, StringComparison.OrdinalIgnoreCase))
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

    private List<string> ReadAllowedCommanderCommands(string logPrefix)
    {
        string allowedJson = CPH.GetGlobalVar<string>(VAR_LOTAT_NODE_COMMANDER_ALLOWED_COMMANDS_JSON, false) ?? "[]";
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
            CPH.LogWarn($"[{logPrefix}] Failed to parse commander allowed-command JSON. exception='{ex.Message}'");
            return new List<string>();
        }
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

    private void OpenDecisionWindow(string logPrefix, string currentNodeId, List<string> allowedDecisionCommands, RuntimeStoryNode node)
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

        CPH.SendMessage(BuildChoicePrompt(node));

        // The decision window uses the fixed runtime-owned v1 default of 120 seconds.
        // Reset by disable -> enable so the timer can use its configured UI interval safely.
        CPH.DisableTimer(TIMER_LOTAT_DECISION_WINDOW);
        CPH.EnableTimer(TIMER_LOTAT_DECISION_WINDOW);

        string sessionId = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_ID, false) ?? "";
        CPH.LogWarn($"[{logPrefix}] Commander window handed off to the normal decision window. sessionId='{sessionId}', nodeId='{currentNodeId}', seconds={DECISION_WINDOW_SECONDS}, allowedCommands='{string.Join(",", allowedDecisionCommands)}'.");
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

        CPH.LogWarn($"[{logPrefix}] Runtime story loaded for commander resolution. storyId='{story.StoryId}', path='{resolvedPath}'.");
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

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("commander_moment")]
        public RuntimeCommanderMoment CommanderMoment { get; set; }

        [JsonPropertyName("choices")]
        public List<RuntimeStoryChoice> Choices { get; set; }
    }

    private sealed class RuntimeCommanderMoment
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("commander")]
        public string Commander { get; set; }

        [JsonPropertyName("success_text")]
        public string SuccessText { get; set; }
    }

    private sealed class RuntimeStoryChoice
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("command")]
        public string Command { get; set; }
    }
}
