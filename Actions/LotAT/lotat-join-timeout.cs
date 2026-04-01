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

    // -------------------------------------------------
    // Timers and stage/window literals
    // -------------------------------------------------
    private const string TIMER_LOTAT_JOIN_WINDOW = "LotAT - Join Window";
    private const string TIMER_LOTAT_DECISION_WINDOW = "LotAT - Decision Window";
    private const string TIMER_LOTAT_COMMANDER_WINDOW = "LotAT - Commander Window";
    private const string TIMER_LOTAT_DICE_WINDOW = "LotAT - Dice Window";

    private const string STAGE_IDLE = "idle";
    private const string STAGE_JOIN_OPEN = "join_open";
    private const string STAGE_NODE_INTRO = "node_intro";
    private const string STAGE_ENDED = "ended";
    private const string WINDOW_NONE = "none";
    private const string WINDOW_JOIN = "join";

    private const string END_STATE_ZERO_JOIN = "zero_join";
    private const string END_STATE_FAULT_ABORT = "fault_abort";

    private const string RUNTIME_STORY_RELATIVE_PATH = "Creative/WorldBuilding/Storylines/loaded/current-story.json";
    private static readonly string[] RUNTIME_STORY_CANDIDATE_PATHS =
    {
        "/mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot/Creative/WorldBuilding/Storylines/loaded/current-story.json",
        @"C:\Users\sharkfac3\Workspace\coding\SharkStreamerBot\Creative\WorldBuilding\Storylines\loaded\current-story.json"
    };

    /*
     * Purpose:
     * - Handle the LotAT join-window timer end.
     * - Either end the run cleanly when nobody joined or freeze the roster and
     *   transition into the story starting node.
     *
     * Expected trigger/input:
     * - Streamer.bot timer-end trigger for "LotAT - Join Window".
     * - No chat payload required.
     *
     * Required runtime variables:
     * - Reads: active/session/stage/window guards, joined roster state, story bootstrap globals.
     * - Writes: roster frozen flag, node/window state, current node id, end-state breadcrumbs,
     *   or a full idle reset on terminal paths.
     *
     * Operator notes:
     * - Wire only the timer "LotAT - Join Window" to this script.
     * - This script intentionally stops at node_intro; full node-entry orchestration belongs
     *   to the later lotat-node-enter.cs implementation pass.
     */
    public bool Execute()
    {
        const string logPrefix = "LotAT Join Timeout";

        try
        {
            string sessionId = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_ID, false) ?? "";

            if (!IsRelevantJoinTimerFire(logPrefix, sessionId))
            {
                CPH.DisableTimer(TIMER_LOTAT_JOIN_WINDOW);
                return true;
            }

            CPH.DisableTimer(TIMER_LOTAT_JOIN_WINDOW);
            CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, true, false);

            List<string> roster = ReadRoster(logPrefix);
            int joinedCount = roster.Count;
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_JOINED_ROSTER_JSON, JsonSerializer.Serialize(roster), false);
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_JOINED_COUNT, joinedCount, false);

            if (joinedCount <= 0)
            {
                CPH.LogWarn($"[{logPrefix}] Join window closed with zero participants. sessionId='{sessionId}'.");
                EndSessionZeroJoin(logPrefix, sessionId);
                return true;
            }

            if (!TryLoadRuntimeStory(logPrefix, out RuntimeStoryBootstrap bootstrap, out string failureReason))
            {
                CPH.LogWarn($"[{logPrefix}] Runtime story reload failed at join close. sessionId='{sessionId}'. {failureReason}");
                FailClosed(logPrefix, sessionId);
                return true;
            }

            CPH.SetGlobalVar(VAR_LOTAT_SESSION_ROSTER_FROZEN, true, false);
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_CURRENT_NODE_ID, bootstrap.StartingNodeId, false);
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_STAGE, STAGE_NODE_INTRO, false);
            CPH.SetGlobalVar(VAR_LOTAT_NODE_ACTIVE_WINDOW, WINDOW_NONE, false);
            CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, false, false);
            CPH.SetGlobalVar(VAR_LOTAT_NODE_ALLOWED_COMMANDS_JSON, "[]", false);
            CPH.SetGlobalVar(VAR_LOTAT_VOTE_MAP_JSON, "{}", false);
            CPH.SetGlobalVar(VAR_LOTAT_VOTE_VALID_COUNT, 0, false);
            CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_NAME, "", false);
            CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_TARGET_USER, "", false);
            CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_ALLOWED_COMMANDS_JSON, "[]", false);
            CPH.SetGlobalVar(VAR_LOTAT_NODE_DICE_SUCCESS_THRESHOLD, 0, false);

            CPH.LogWarn($"[{logPrefix}] Join window closed with participants. sessionId='{sessionId}', joinedCount={joinedCount}, storyId='{bootstrap.StoryId}', startingNodeId='{bootstrap.StartingNodeId}'.");
            CPH.SendMessage($"LotAT crew locked in: {joinedCount}. Mission starting now.");
            return true;
        }
        catch (Exception ex)
        {
            string sessionId = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_ID, false) ?? "";
            CPH.LogError($"[LotAT Join Timeout] Unhandled exception. sessionId='{sessionId}', exception={ex}");
            FailClosed("LotAT Join Timeout", sessionId);
            return true;
        }
    }

    private bool IsRelevantJoinTimerFire(string logPrefix, string sessionId)
    {
        bool active = CPH.GetGlobalVar<bool?>(VAR_LOTAT_ACTIVE, false) ?? false;
        if (!active)
        {
            CPH.LogWarn($"[{logPrefix}] Ignoring stale join timer because LotAT is not active.");
            return false;
        }

        string stage = (CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_STAGE, false) ?? "").Trim();
        if (!string.Equals(stage, STAGE_JOIN_OPEN, StringComparison.OrdinalIgnoreCase))
        {
            CPH.LogWarn($"[{logPrefix}] Ignoring stale join timer because stage is '{stage}', not '{STAGE_JOIN_OPEN}'. sessionId='{sessionId}'.");
            return false;
        }

        string activeWindow = (CPH.GetGlobalVar<string>(VAR_LOTAT_NODE_ACTIVE_WINDOW, false) ?? "").Trim();
        if (!string.Equals(activeWindow, WINDOW_JOIN, StringComparison.OrdinalIgnoreCase))
        {
            CPH.LogWarn($"[{logPrefix}] Ignoring stale join timer because activeWindow is '{activeWindow}', not '{WINDOW_JOIN}'. sessionId='{sessionId}'.");
            return false;
        }

        bool rosterFrozen = CPH.GetGlobalVar<bool?>(VAR_LOTAT_SESSION_ROSTER_FROZEN, false) ?? false;
        if (rosterFrozen)
        {
            CPH.LogWarn($"[{logPrefix}] Ignoring stale join timer because the roster is already frozen. sessionId='{sessionId}'.");
            return false;
        }

        bool windowResolved = CPH.GetGlobalVar<bool?>(VAR_LOTAT_NODE_WINDOW_RESOLVED, false) ?? false;
        if (windowResolved)
        {
            CPH.LogWarn($"[{logPrefix}] Ignoring stale join timer because the join window is already marked resolved. sessionId='{sessionId}'.");
            return false;
        }

        return true;
    }

    private void EndSessionZeroJoin(string logPrefix, string sessionId)
    {
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_STAGE, STAGE_ENDED, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ACTIVE_WINDOW, WINDOW_NONE, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, true, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_LAST_END_STATE, END_STATE_ZERO_JOIN, false);
        CPH.LogWarn($"[{logPrefix}] Zero-join path marked for centralized cleanup. sessionId='{sessionId}'.");
    }

    private void FailClosed(string logPrefix, string sessionId)
    {
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_STAGE, STAGE_ENDED, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ACTIVE_WINDOW, WINDOW_NONE, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, true, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_LAST_END_STATE, END_STATE_FAULT_ABORT, false);
        CPH.LogWarn($"[{logPrefix}] Fault-abort path marked for centralized cleanup. sessionId='{sessionId}'.");
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
            CPH.LogWarn($"[{logPrefix}] Failed to parse joined roster JSON at join close. Using empty roster for safety. exception='{ex.Message}'");
            return new List<string>();
        }
    }

    private string NormalizeParticipantIdentity(string rawUser)
    {
        string normalized = (rawUser ?? "").Trim();
        if (normalized.StartsWith("@", StringComparison.Ordinal))
            normalized = normalized.Substring(1);

        return normalized.Trim().ToLowerInvariant();
    }

    private bool TryLoadRuntimeStory(string logPrefix, out RuntimeStoryBootstrap bootstrap, out string failureReason)
    {
        bootstrap = null;
        failureReason = "Unknown runtime story load failure.";

        string resolvedPath = ResolveRuntimeStoryPath();
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

        RuntimeStoryDefinition story;
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

        if (story == null)
        {
            failureReason = $"Runtime story JSON deserialized to null. path='{resolvedPath}'";
            return false;
        }

        if (string.IsNullOrWhiteSpace(story.StoryId))
        {
            failureReason = $"Runtime story missing story_id. path='{resolvedPath}'";
            return false;
        }

        if (string.IsNullOrWhiteSpace(story.StartingNodeId))
        {
            failureReason = $"Runtime story missing starting_node_id. storyId='{story.StoryId}', path='{resolvedPath}'";
            return false;
        }

        if (story.Nodes == null || story.Nodes.Count == 0)
        {
            failureReason = $"Runtime story missing nodes array content. storyId='{story.StoryId}', path='{resolvedPath}'";
            return false;
        }

        RuntimeStoryNode startingNode = null;
        foreach (RuntimeStoryNode node in story.Nodes)
        {
            if (node == null)
                continue;

            if (string.Equals(node.NodeId ?? "", story.StartingNodeId, StringComparison.Ordinal))
            {
                startingNode = node;
                break;
            }
        }

        if (startingNode == null)
        {
            failureReason = $"Runtime story starting node could not be resolved. storyId='{story.StoryId}', startingNodeId='{story.StartingNodeId}', path='{resolvedPath}'";
            return false;
        }

        if (string.IsNullOrWhiteSpace(startingNode.NodeType))
        {
            failureReason = $"Runtime story starting node is missing node_type. storyId='{story.StoryId}', startingNodeId='{story.StartingNodeId}', path='{resolvedPath}'";
            return false;
        }

        bootstrap = new RuntimeStoryBootstrap
        {
            StoryId = story.StoryId.Trim(),
            StartingNodeId = story.StartingNodeId.Trim(),
            ResolvedPath = resolvedPath
        };

        CPH.LogWarn($"[{logPrefix}] Runtime story minimally reloaded at join close. storyId='{bootstrap.StoryId}', startingNodeId='{bootstrap.StartingNodeId}', path='{bootstrap.ResolvedPath}'.");
        return true;
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

    private sealed class RuntimeStoryBootstrap
    {
        public string StoryId { get; set; }
        public string StartingNodeId { get; set; }
        public string ResolvedPath { get; set; }
    }

    private sealed class RuntimeStoryDefinition
    {
        [JsonPropertyName("story_id")]
        public string StoryId { get; set; }

        [JsonPropertyName("starting_node_id")]
        public string StartingNodeId { get; set; }

        [JsonPropertyName("nodes")]
        public List<RuntimeStoryNode> Nodes { get; set; }
    }

    private sealed class RuntimeStoryNode
    {
        [JsonPropertyName("node_id")]
        public string NodeId { get; set; }

        [JsonPropertyName("node_type")]
        public string NodeType { get; set; }
    }
}
