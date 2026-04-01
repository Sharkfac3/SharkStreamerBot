using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class CPHInline
{
    // -------------------------------------------------
    // LotAT shared runtime constants
    // Keep these string names synchronized with Actions/SHARED-CONSTANTS.md.
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
    // LotAT timers and stage/window literals
    // -------------------------------------------------
    private const string TIMER_LOTAT_JOIN_WINDOW = "LotAT - Join Window";
    private const string TIMER_LOTAT_DECISION_WINDOW = "LotAT - Decision Window";
    private const string TIMER_LOTAT_COMMANDER_WINDOW = "LotAT - Commander Window";
    private const string TIMER_LOTAT_DICE_WINDOW = "LotAT - Dice Window";

    private const string STAGE_IDLE = "idle";
    private const string STAGE_JOIN_OPEN = "join_open";
    private const string STAGE_ENDED = "ended";
    private const string WINDOW_NONE = "none";
    private const string WINDOW_JOIN = "join";

    private const string END_STATE_FAULT_ABORT = "fault_abort";

    // Contract-owned default for v1. See implementation note in StartJoinTimer().
    private const int JOIN_WINDOW_SECONDS = 120;

    // Runtime source-of-truth story path for LotAT v1.
    private const string RUNTIME_STORY_RELATIVE_PATH = "Creative/WorldBuilding/Storylines/loaded/current-story.json";

    // Machine-local repo-root candidates.
    // Practical compromise: Streamer.bot actions do not automatically know the repo root,
    // so this script checks a small candidate list instead of assuming the process working
    // directory equals the repo directory.
    private static readonly string[] RUNTIME_STORY_CANDIDATE_PATHS =
    {
        "/mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot/Creative/WorldBuilding/Storylines/loaded/current-story.json",
        @"C:\Users\sharkfac3\Workspace\coding\SharkStreamerBot\Creative\WorldBuilding\Storylines\loaded\current-story.json"
    };

    /*
     * Purpose:
     * - Voice-triggered LotAT session bootstrap for v1.
     * - Loads the single runtime story file, performs only minimal-safe start checks,
     *   clears stale LotAT state, opens the join phase, and starts the join timer.
     *
     * Expected trigger/input:
     * - Voice command trigger in Streamer.bot.
     * - No chat payload is required or expected.
     * - This script intentionally does not read message/rawInput.
     *
     * Required runtime variables:
     * - Reads: lotat_active.
     * - Writes: fresh LotAT session bootstrap globals defined in Actions/SHARED-CONSTANTS.md.
     *
     * Key outputs/side effects:
     * - Refuses to start if a LotAT session is already active.
     * - Loads exactly Creative/WorldBuilding/Storylines/loaded/current-story.json.
     * - Fails closed if that runtime story file is missing, unreadable, malformed, or minimally unusable.
     * - Starts the LotAT join window and announces !join on success.
     *
     * Operator notes:
     * - The Streamer.bot timer named "LotAT - Join Window" must exist.
     * - Because CPH.SetTimerInterval(string, int) is still project-marked VERIFY, this script
     *   assumes the timer is configured to 120 seconds in the Streamer.bot UI and only enables it.
     * - The repo path candidates in this script should be updated if the local checkout moves.
     */
    public bool Execute()
    {
        const string logPrefix = "LotAT Start";

        try
        {
            if ((CPH.GetGlobalVar<bool?>(VAR_LOTAT_ACTIVE, false) ?? false))
            {
                string activeSessionId = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_ID, false) ?? "";
                string activeStage = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_STAGE, false) ?? "";

                CPH.LogWarn($"[{logPrefix}] Start blocked because a LotAT session is already active. sessionId='{activeSessionId}', stage='{activeStage}'.");
                CPH.SendMessage("LotAT is already running.");
                return true;
            }

            if (!TryLoadRuntimeStory(logPrefix, out RuntimeStoryBootstrap bootstrap, out string failureReason))
            {
                MarkFaultAbortState();
                CPH.LogWarn($"[{logPrefix}] Start aborted during runtime story load. {failureReason}");
                CPH.SendMessage("LotAT could not start because the runtime story is not ready.");
                return true;
            }

            ResetLotatRuntimeToIdle();

            string sessionId = Guid.NewGuid().ToString("N");
            CPH.SetGlobalVar(VAR_LOTAT_ACTIVE, true, false);
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_ID, sessionId, false);
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_STAGE, STAGE_JOIN_OPEN, false);
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_STORY_ID, bootstrap.StoryId, false);
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_CURRENT_NODE_ID, bootstrap.StartingNodeId, false);
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_CHAOS_TOTAL, 0, false);
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_ROSTER_FROZEN, false, false);
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_JOINED_ROSTER_JSON, "[]", false);
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_JOINED_COUNT, 0, false);
            CPH.SetGlobalVar(VAR_LOTAT_NODE_ACTIVE_WINDOW, WINDOW_JOIN, false);
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

            CPH.LogWarn($"[{logPrefix}] Session bootstrap succeeded. sessionId='{sessionId}', storyId='{bootstrap.StoryId}', startingNodeId='{bootstrap.StartingNodeId}', storyPath='{bootstrap.ResolvedPath}'.");

            CPH.SendMessage($"LotAT mission ready. Type !join in the next {JOIN_WINDOW_SECONDS} seconds to join this run.");
            StartJoinTimer(logPrefix, sessionId);
            return true;
        }
        catch (Exception ex)
        {
            MarkFaultAbortState();
            CPH.LogError($"[{logPrefix}] Unhandled exception during startup bootstrap: {ex}");
            CPH.SendMessage("LotAT hit a runtime problem and stopped safely.");
            return true;
        }
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

        // Minimal-safe runtime checks only.
        // This is intentionally not a second full schema or graph validation pass.
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

        CPH.LogWarn($"[{logPrefix}] Runtime story minimally loaded. storyId='{bootstrap.StoryId}', startingNodeId='{bootstrap.StartingNodeId}', path='{bootstrap.ResolvedPath}'.");
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
                // The caller logs the final failure once all candidates have been checked.
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

    private void StartJoinTimer(string logPrefix, string sessionId)
    {
        // Practical compromise:
        // The project still marks CPH.SetTimerInterval(string, int) as VERIFY/unconfirmed.
        // To keep this bootstrap script compile-safe in Streamer.bot, we do not call it here.
        // Instead, the operator must configure the named timer interval to 120 seconds in the UI.
        CPH.DisableTimer(TIMER_LOTAT_JOIN_WINDOW);
        CPH.EnableTimer(TIMER_LOTAT_JOIN_WINDOW);
        CPH.LogWarn($"[{logPrefix}] Join timer started. timer='{TIMER_LOTAT_JOIN_WINDOW}', expectedSeconds={JOIN_WINDOW_SECONDS}, sessionId='{sessionId}'. Ensure the Streamer.bot timer is configured to {JOIN_WINDOW_SECONDS}s in the UI.");
    }

    private void MarkFaultAbortState()
    {
        CPH.DisableTimer(TIMER_LOTAT_JOIN_WINDOW);
        CPH.DisableTimer(TIMER_LOTAT_DECISION_WINDOW);
        CPH.DisableTimer(TIMER_LOTAT_COMMANDER_WINDOW);
        CPH.DisableTimer(TIMER_LOTAT_DICE_WINDOW);

        CPH.SetGlobalVar(VAR_LOTAT_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_STAGE, STAGE_ENDED, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ACTIVE_WINDOW, WINDOW_NONE, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, true, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_LAST_END_STATE, END_STATE_FAULT_ABORT, false);
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
