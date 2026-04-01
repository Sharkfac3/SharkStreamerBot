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
    private const string VAR_LOTAT_SESSION_LAST_CHOICE_ID = "lotat_session_last_choice_id";
    private const string VAR_LOTAT_SESSION_LAST_END_STATE = "lotat_session_last_end_state";

    private const string TIMER_LOTAT_DECISION_WINDOW = "LotAT - Decision Window";

    private const string STAGE_DECISION_OPEN = "decision_open";
    private const string STAGE_DECISION_RESOLVING = "decision_resolving";
    private const string STAGE_ENDED = "ended";
    private const string WINDOW_NONE = "none";
    private const string WINDOW_DECISION = "decision";

    private const string END_STATE_FAULT_ABORT = "fault_abort";

    private const string RUNTIME_STORY_RELATIVE_PATH = "Creative/WorldBuilding/Storylines/loaded/current-story.json";
    private static readonly string[] RUNTIME_STORY_CANDIDATE_PATHS =
    {
        "/mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot/Creative/WorldBuilding/Storylines/loaded/current-story.json",
        @"C:\Users\sharkfac3\Workspace\coding\SharkStreamerBot\Creative\WorldBuilding\Storylines\loaded\current-story.json"
    };

    /*
     * Purpose:
     * - Shared LotAT decision resolution path for v1.
     * - Resolves only when the active decision window has already been marked closed,
     *   tallies the current-node vote map, applies deterministic tie-break by authored choices order,
     *   emits result flavor, stores the winning choice, and advances the current node id.
     *
     * Expected trigger/input:
     * - Follow immediately after lotat-decision-input.cs in the authored command action group.
     * - Follow immediately after lotat-decision-timeout.cs in the decision timer-end action group.
     * - No required trigger args.
     *
     * Operator notes:
     * - Put lotat-node-enter.cs immediately after this action in both action groups.
     * - This action intentionally no-ops unless the decision window has already been marked resolved.
     */
    public bool Execute()
    {
        const string logPrefix = "LotAT Decision Resolve";

        try
        {
            string sessionId = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_ID, false) ?? "";
            if (!IsReadyToResolve(logPrefix, sessionId))
                return true;

            CPH.DisableTimer(TIMER_LOTAT_DECISION_WINDOW);

            string currentNodeId = (CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_CURRENT_NODE_ID, false) ?? "").Trim();
            if (string.IsNullOrWhiteSpace(currentNodeId))
            {
                CPH.LogWarn($"[{logPrefix}] Resolve skipped because no current node id was available. sessionId='{sessionId}'.");
                return true;
            }

            if (!TryLoadRuntimeStory(logPrefix, out RuntimeStoryDefinition story, out string resolvedPath, out string failureReason))
            {
                FailClosed(logPrefix, sessionId, $"Runtime story load failed during decision resolution. {failureReason}");
                return true;
            }

            RuntimeStoryNode node = FindNodeById(story, currentNodeId);
            if (node == null || node.Choices == null || node.Choices.Count <= 0)
            {
                FailClosed(logPrefix, sessionId, $"Resolve could not continue because the current node is missing or has no choices. nodeId='{currentNodeId}', path='{resolvedPath}'.");
                return true;
            }

            List<string> allowedCommands = ReadAllowedCommands(logPrefix);
            Dictionary<string, string> voteMap = ReadVoteMap(logPrefix, allowedCommands);
            if (voteMap.Count <= 0)
            {
                FailClosed(logPrefix, sessionId, $"Resolve found no valid votes after filtering despite entering the resolution path. nodeId='{currentNodeId}'.");
                return true;
            }

            RuntimeStoryChoice winningChoice = ResolveWinningChoice(node, voteMap, out int winningCount, out Dictionary<string, int> tallies);
            if (winningChoice == null)
            {
                FailClosed(logPrefix, sessionId, $"Resolve could not match the vote tallies to an authored choice. nodeId='{currentNodeId}'.");
                return true;
            }

            string nextNodeId = (winningChoice.NextNodeId ?? "").Trim();
            if (string.IsNullOrWhiteSpace(nextNodeId))
            {
                FailClosed(logPrefix, sessionId, $"Winning choice is missing next_node_id. nodeId='{currentNodeId}', choiceId='{winningChoice.ChoiceId}'.");
                return true;
            }

            CPH.SetGlobalVar(VAR_LOTAT_SESSION_STAGE, STAGE_DECISION_RESOLVING, false);
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_LAST_CHOICE_ID, winningChoice.ChoiceId ?? "", false);
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_CURRENT_NODE_ID, nextNodeId, false);
            CPH.SetGlobalVar(VAR_LOTAT_NODE_ACTIVE_WINDOW, WINDOW_NONE, false);
            CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, false, false);
            CPH.SetGlobalVar(VAR_LOTAT_NODE_ALLOWED_COMMANDS_JSON, "[]", false);
            CPH.SetGlobalVar(VAR_LOTAT_VOTE_MAP_JSON, "{}", false);
            CPH.SetGlobalVar(VAR_LOTAT_VOTE_VALID_COUNT, 0, false);

            string resultFlavor = (winningChoice.ResultFlavor ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(resultFlavor))
                CPH.SendMessage(resultFlavor);

            CPH.LogWarn($"[{logPrefix}] Decision resolved. sessionId='{sessionId}', nodeId='{currentNodeId}', choiceId='{winningChoice.ChoiceId}', command='{winningChoice.Command}', votes={winningCount}, tallies='{FormatTallies(tallies)}', nextNodeId='{nextNodeId}'.");
            return true;
        }
        catch (Exception ex)
        {
            string sessionId = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_ID, false) ?? "";
            FailClosed(logPrefix, sessionId, $"Unhandled exception during decision resolution: {ex}");
            return true;
        }
    }

    private bool IsReadyToResolve(string logPrefix, string sessionId)
    {
        bool active = CPH.GetGlobalVar<bool?>(VAR_LOTAT_ACTIVE, false) ?? false;
        if (!active)
            return false;

        string stage = (CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_STAGE, false) ?? "").Trim();
        if (!string.Equals(stage, STAGE_DECISION_OPEN, StringComparison.OrdinalIgnoreCase))
        {
            if (!string.Equals(stage, STAGE_DECISION_RESOLVING, StringComparison.OrdinalIgnoreCase))
                return false;

            CPH.LogWarn($"[{logPrefix}] Skipping duplicate resolve because stage is already '{STAGE_DECISION_RESOLVING}'. sessionId='{sessionId}'.");
            return false;
        }

        string activeWindow = (CPH.GetGlobalVar<string>(VAR_LOTAT_NODE_ACTIVE_WINDOW, false) ?? "").Trim();
        if (!string.Equals(activeWindow, WINDOW_DECISION, StringComparison.OrdinalIgnoreCase))
            return false;

        bool windowResolved = CPH.GetGlobalVar<bool?>(VAR_LOTAT_NODE_WINDOW_RESOLVED, false) ?? false;
        if (!windowResolved)
            return false;

        return true;
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
            CPH.LogWarn($"[{logPrefix}] Failed to parse allowed commands JSON during resolve. exception='{ex.Message}'");
            return new List<string>();
        }
    }

    private Dictionary<string, string> ReadVoteMap(string logPrefix, List<string> allowedCommands)
    {
        string voteMapJson = CPH.GetGlobalVar<string>(VAR_LOTAT_VOTE_MAP_JSON, false) ?? "{}";
        if (string.IsNullOrWhiteSpace(voteMapJson))
            voteMapJson = "{}";

        var filtered = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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

                if (allowedSet.Count > 0 && !allowedSet.Contains(command))
                    continue;

                filtered[user] = command;
            }
        }
        catch (Exception ex)
        {
            CPH.LogWarn($"[{logPrefix}] Failed to parse vote map JSON during resolve. exception='{ex.Message}'");
        }

        return filtered;
    }

    private RuntimeStoryChoice ResolveWinningChoice(RuntimeStoryNode node, Dictionary<string, string> voteMap, out int winningCount, out Dictionary<string, int> tallies)
    {
        winningCount = 0;
        tallies = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (string command in voteMap.Values)
        {
            string normalized = NormalizeCommand(command);
            if (string.IsNullOrWhiteSpace(normalized))
                continue;

            if (!tallies.ContainsKey(normalized))
                tallies[normalized] = 0;

            tallies[normalized]++;
            if (tallies[normalized] > winningCount)
                winningCount = tallies[normalized];
        }

        if (winningCount <= 0)
            return null;

        foreach (RuntimeStoryChoice choice in node.Choices)
        {
            if (choice == null)
                continue;

            string command = NormalizeCommand(choice.Command);
            if (string.IsNullOrWhiteSpace(command))
                continue;

            if (!tallies.ContainsKey(command))
                continue;

            if (tallies[command] == winningCount)
                return choice;
        }

        return null;
    }

    private string FormatTallies(Dictionary<string, int> tallies)
    {
        if (tallies == null || tallies.Count <= 0)
            return "";

        var parts = new List<string>();
        foreach (KeyValuePair<string, int> pair in tallies)
            parts.Add($"{pair.Key}:{pair.Value}");

        return string.Join(",", parts);
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
            failureReason = $"Runtime story is minimally unusable during resolve. path='{resolvedPath}'";
            return false;
        }

        CPH.LogWarn($"[{logPrefix}] Runtime story loaded for decision resolution. storyId='{story.StoryId}', path='{resolvedPath}'.");
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

    private void FailClosed(string logPrefix, string sessionId, string detail)
    {
        CPH.LogError($"[{logPrefix}] Fail-closed. sessionId='{sessionId}'. {detail}");
        CPH.DisableTimer(TIMER_LOTAT_DECISION_WINDOW);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_STAGE, STAGE_ENDED, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ACTIVE_WINDOW, WINDOW_NONE, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, true, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_LAST_CHOICE_ID, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_CURRENT_NODE_ID, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ALLOWED_COMMANDS_JSON, "[]", false);
        CPH.SetGlobalVar(VAR_LOTAT_VOTE_MAP_JSON, "{}", false);
        CPH.SetGlobalVar(VAR_LOTAT_VOTE_VALID_COUNT, 0, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_LAST_END_STATE, END_STATE_FAULT_ABORT, false);
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
                // Ignore path-probe exceptions here.
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
            // Ignore environment-probe issues.
        }

        try
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? "";
            if (!string.IsNullOrWhiteSpace(baseDirectory))
                candidates.Add(Path.Combine(baseDirectory, RUNTIME_STORY_RELATIVE_PATH));
        }
        catch
        {
            // Ignore app-domain-probe issues.
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

        [JsonPropertyName("choices")]
        public List<RuntimeStoryChoice> Choices { get; set; }
    }

    private sealed class RuntimeStoryChoice
    {
        [JsonPropertyName("choice_id")]
        public string ChoiceId { get; set; }

        [JsonPropertyName("command")]
        public string Command { get; set; }

        [JsonPropertyName("result_flavor")]
        public string ResultFlavor { get; set; }

        [JsonPropertyName("next_node_id")]
        public string NextNodeId { get; set; }
    }
}
