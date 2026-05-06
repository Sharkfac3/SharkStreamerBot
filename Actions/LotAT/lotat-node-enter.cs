// ACTION-CONTRACT: Actions/LotAT/AGENTS.md#lotat-node-enter.cs
// ACTION-CONTRACT-SHA256: e0af705c5c3363ae61c2cb90b7b7191b4454217ee47dfb8e64e2e8c377964688

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
    private const string VAR_LOTAT_SESSION_LAST_END_STATE = "lotat_session_last_end_state";

    // Existing commander assignment globals owned by the commander system.
    private const string VAR_CURRENT_CAPTAIN_STRETCH = "current_captain_stretch";
    private const string VAR_CURRENT_THE_DIRECTOR = "current_the_director";
    private const string VAR_CURRENT_WATER_WIZARD = "current_water_wizard";

    // -------------------------------------------------
    // Timers and stage/window literals
    // -------------------------------------------------
    private const string TIMER_LOTAT_JOIN_WINDOW = "LotAT - Join Window";
    private const string TIMER_LOTAT_DECISION_WINDOW = "LotAT - Decision Window";
    private const string TIMER_LOTAT_COMMANDER_WINDOW = "LotAT - Commander Window";
    private const string TIMER_LOTAT_DICE_WINDOW = "LotAT - Dice Window";

    private const string STAGE_IDLE = "idle";
    private const string STAGE_NODE_INTRO = "node_intro";
    private const string STAGE_COMMANDER_OPEN = "commander_open";
    private const string STAGE_DICE_OPEN = "dice_open";
    private const string STAGE_DECISION_OPEN = "decision_open";
    private const string STAGE_ENDED = "ended";

    private const string WINDOW_NONE = "none";
    private const string WINDOW_COMMANDER = "commander";
    private const string WINDOW_DICE = "dice";
    private const string WINDOW_DECISION = "decision";

    // Join and decision windows are fixed runtime defaults in v1.
    private const int DECISION_WINDOW_SECONDS = 120;

    private const string END_REASON_FAULT_ABORT = "fault_abort";

    private const string RUNTIME_STORY_RELATIVE_PATH = "Creative/WorldBuilding/Storylines/loaded/current-story.json";
    private static readonly string[] RUNTIME_STORY_CANDIDATE_PATHS =
    {
        "/mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot/Creative/WorldBuilding/Storylines/loaded/current-story.json",
        @"C:\Users\sharkfac3\Workspace\coding\SharkStreamerBot\Creative\WorldBuilding\Storylines\loaded\current-story.json"
    };

    /*
     * Purpose:
     * - LotAT node-entry orchestration for v1.
     * - Resolves the active node, applies node-intro behavior, applies chaos,
     *   clears stale node-local state, stores allowed decision commands, and routes
     *   into ending / commander / dice / decision flow according to the runtime contract.
     *
     * Expected trigger/input:
     * - Internal follow-up action after join close or after a decision resolves.
     * - Optional `input0` may provide an explicit target node id.
     * - If `input0` is empty, the script uses lotat_session_current_node_id.
     *
     * Required runtime variables:
     * - Reads active LotAT session state, current node id, current chaos total,
     *   and commander assignment globals.
     * - Writes node_intro stage, cleared node/window/vote state, updated chaos total,
     *   allowed decision commands, commander/dice runtime state, and simple chat-facing intro output.
     *
     * Key outputs/side effects:
     * - Disables stale decision/commander/dice timers before opening the correct next window.
     * - Fails closed when the node cannot be safely resolved.
     * - Enforces the v1 guardrail that commander moments and dice hooks may not coexist.
     * - Dice windows hand off to lotat-dice-roll.cs / lotat-dice-timeout.cs, which then open the normal decision window.
     *
     * Operator notes:
     * - Place this action immediately after the successful join-close path and after decision resolve.
     * - Commander/dice windows rely on CPH.SetTimerInterval(string, int), which is still marked
     *   VERIFY/unconfirmed in project docs. Test those timer starts before live use.
     * - Ending nodes and fail-closed node-entry faults now mark stage = ended and rely on
     *   lotat-end-session.cs for the shared teardown back to idle.
     */
    public bool Execute()
    {
        const string logPrefix = "LotAT Node Enter";

        try
        {
            string sessionId = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_ID, false) ?? "";
            if (!(CPH.GetGlobalVar<bool?>(VAR_LOTAT_ACTIVE, false) ?? false))
            {
                CPH.LogWarn($"[{logPrefix}] Ignoring node-entry because no LotAT session is active.");
                return true;
            }

            string currentStage = (CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_STAGE, false) ?? "").Trim();
            if (string.Equals(currentStage, STAGE_ENDED, StringComparison.OrdinalIgnoreCase))
            {
                CPH.LogWarn($"[{logPrefix}] Ignoring node-entry because the session is already marked ended. sessionId='{sessionId}'.");
                return true;
            }

            string requestedNodeId = ReadRequestedNodeId();
            if (string.IsNullOrWhiteSpace(requestedNodeId))
            {
                FailClosed(logPrefix, sessionId, "No target node id was available from input0 or lotat_session_current_node_id.");
                return true;
            }

            if (!TryLoadRuntimeStory(logPrefix, out RuntimeStoryDefinition story, out string resolvedPath, out string loadFailure))
            {
                FailClosed(logPrefix, sessionId, loadFailure);
                return true;
            }

            RuntimeStoryNode node = FindNodeById(story, requestedNodeId);
            if (node == null)
            {
                FailClosed(logPrefix, sessionId, $"Target node could not be resolved. storyId='{story.StoryId}', nodeId='{requestedNodeId}', path='{resolvedPath}'.");
                return true;
            }

            if (string.IsNullOrWhiteSpace(node.NodeType))
            {
                FailClosed(logPrefix, sessionId, $"Resolved node is missing node_type. storyId='{story.StoryId}', nodeId='{requestedNodeId}'.");
                return true;
            }

            // Node-entry begins by clearing any stale node-local window and vote state from the prior node.
            DisableActiveNodeTimers();
            ClearPerNodeTransientState();

            // Persist the current node again so downstream handlers share the same source of truth
            // even when node-entry was invoked through input0.
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_STORY_ID, story.StoryId ?? "", false);
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_CURRENT_NODE_ID, node.NodeId ?? requestedNodeId, false);
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_STAGE, STAGE_NODE_INTRO, false);

            int updatedChaosTotal = ApplyNodeChaos(logPrefix, node, sessionId);
            List<string> allowedDecisionCommands = DeriveAllowedDecisionCommands(node, out string commandFailure);
            if (!string.IsNullOrWhiteSpace(commandFailure))
            {
                FailClosed(logPrefix, sessionId, commandFailure);
                return true;
            }

            CPH.SetGlobalVar(VAR_LOTAT_NODE_ALLOWED_COMMANDS_JSON, JsonSerializer.Serialize(allowedDecisionCommands), false);

            SurfaceNodeIntroToChat(node, updatedChaosTotal);

            bool commanderEnabled = node.CommanderMoment != null && node.CommanderMoment.Enabled;
            bool diceEnabled = node.DiceHook != null && node.DiceHook.Enabled;

            if (commanderEnabled && diceEnabled)
            {
                FailClosed(logPrefix, sessionId, $"Node '{node.NodeId}' enables both commander_moment and dice_hook, which is not allowed in v1.");
                return true;
            }

            string normalizedNodeType = (node.NodeType ?? "").Trim().ToLowerInvariant();
            if (normalizedNodeType == "ending")
            {
                RouteEndingNode(logPrefix, sessionId, node);
                return true;
            }

            if (normalizedNodeType != "stage")
            {
                FailClosed(logPrefix, sessionId, $"Node '{node.NodeId}' uses unsupported node_type '{node.NodeType}'.");
                return true;
            }

            if (commanderEnabled)
            {
                OpenCommanderWindow(logPrefix, sessionId, node);
                return true;
            }

            if (diceEnabled)
            {
                OpenDiceWindow(logPrefix, sessionId, node);
                return true;
            }

            OpenDecisionWindow(logPrefix, sessionId, node, allowedDecisionCommands);
            return true;
        }
        catch (Exception ex)
        {
            string sessionId = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_ID, false) ?? "";
            FailClosed("LotAT Node Enter", sessionId, $"Unhandled exception during node entry: {ex}");
            return true;
        }
    }

    private string ReadRequestedNodeId()
    {
        string nodeId = "";
        if (CPH.TryGetArg("input0", out nodeId) && !string.IsNullOrWhiteSpace(nodeId))
            return nodeId.Trim();

        return (CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_CURRENT_NODE_ID, false) ?? "").Trim();
    }

    private int ApplyNodeChaos(string logPrefix, RuntimeStoryNode node, string sessionId)
    {
        int currentChaos = CPH.GetGlobalVar<int?>(VAR_LOTAT_SESSION_CHAOS_TOTAL, false) ?? 0;
        int delta = 0;

        if (node.Chaos != null)
            delta = node.Chaos.Delta;

        if (delta < 0)
            throw new InvalidOperationException($"Node '{node.NodeId}' has invalid negative chaos delta {delta}.");

        int updatedChaos = currentChaos + delta;
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_CHAOS_TOTAL, updatedChaos, false);
        CPH.LogWarn($"[{logPrefix}] Applied chaos delta. sessionId='{sessionId}', nodeId='{node.NodeId}', delta={delta}, chaosTotal={updatedChaos}.");
        return updatedChaos;
    }

    private List<string> DeriveAllowedDecisionCommands(RuntimeStoryNode node, out string failureReason)
    {
        failureReason = "";
        var commands = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        string nodeType = (node.NodeType ?? "").Trim().ToLowerInvariant();

        if (nodeType == "ending")
            return commands;

        if (node.Choices == null)
        {
            failureReason = $"Stage node '{node.NodeId}' is missing choices.";
            return commands;
        }

        if (node.Choices.Count <= 0 || node.Choices.Count > 2)
        {
            failureReason = $"Stage node '{node.NodeId}' has invalid choice count {node.Choices.Count}. V1 requires 1 or 2 choices.";
            return commands;
        }

        foreach (RuntimeStoryChoice choice in node.Choices)
        {
            if (choice == null || string.IsNullOrWhiteSpace(choice.Command))
            {
                failureReason = $"Stage node '{node.NodeId}' has a choice with no command.";
                return new List<string>();
            }

            string command = choice.Command.Trim().ToLowerInvariant();
            if (seen.Add(command))
                commands.Add(command);
        }

        if (commands.Count <= 0)
            failureReason = $"Stage node '{node.NodeId}' produced no allowed decision commands.";

        return commands;
    }

    private void SurfaceNodeIntroToChat(RuntimeStoryNode node, int chaosTotal)
    {
        string header = BuildNodeHeader(node);
        if (!string.IsNullOrWhiteSpace(header))
            CPH.SendMessage(header);

        string readAloud = (node.ReadAloud ?? "").Trim();
        if (!string.IsNullOrWhiteSpace(readAloud))
            CPH.SendMessage(readAloud);

        if (node.Chaos != null && node.Chaos.Delta > 0)
            CPH.SendMessage($"Chaos rises by {node.Chaos.Delta}. Total chaos: {chaosTotal}.");
    }

    private string BuildNodeHeader(RuntimeStoryNode node)
    {
        string shipSection = (node.ShipSection ?? "").Trim();
        string title = (node.Title ?? "").Trim();

        if (!string.IsNullOrWhiteSpace(shipSection) && !string.IsNullOrWhiteSpace(title))
            return $"[{shipSection}] {title}";

        if (!string.IsNullOrWhiteSpace(title))
            return title;

        if (!string.IsNullOrWhiteSpace(shipSection))
            return $"[{shipSection}]";

        return "";
    }

    private void OpenCommanderWindow(string logPrefix, string sessionId, RuntimeStoryNode node)
    {
        if (node.CommanderMoment == null || !node.CommanderMoment.Enabled)
        {
            FailClosed(logPrefix, sessionId, $"Node '{node.NodeId}' attempted commander routing without an enabled commander_moment payload.");
            return;
        }

        string commanderName = (node.CommanderMoment.Commander ?? "").Trim();
        string prompt = (node.CommanderMoment.Prompt ?? "").Trim();
        int windowSeconds = node.CommanderMoment.WindowSeconds;
        string successText = (node.CommanderMoment.SuccessText ?? "").Trim();

        if (string.IsNullOrWhiteSpace(commanderName) || string.IsNullOrWhiteSpace(prompt) || string.IsNullOrWhiteSpace(successText) || windowSeconds <= 0)
        {
            FailClosed(logPrefix, sessionId, $"Node '{node.NodeId}' has malformed commander_moment payload.");
            return;
        }

        List<string> allowedCommanderCommands = GetAllowedCommanderCommands(commanderName);
        if (allowedCommanderCommands.Count <= 0)
        {
            FailClosed(logPrefix, sessionId, $"Node '{node.NodeId}' references unknown commander '{commanderName}'.");
            return;
        }

        string commanderTargetUser = GetAssignedCommanderUser(commanderName);
        if (string.IsNullOrWhiteSpace(commanderTargetUser))
        {
            CPH.LogWarn($"[{logPrefix}] Commander moment skipped because no assigned commander user is available. sessionId='{sessionId}', nodeId='{node.NodeId}', commander='{commanderName}'.");
            OpenDecisionWindow(logPrefix, sessionId, node, ReadAllowedDecisionCommandsFromState());
            return;
        }

        // Snapshot the assigned commander at window open so later commander-slot changes
        // cannot retarget this node mid-window.
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_STAGE, STAGE_COMMANDER_OPEN, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ACTIVE_WINDOW, WINDOW_COMMANDER, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, false, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_NAME, commanderName, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_TARGET_USER, commanderTargetUser, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_ALLOWED_COMMANDS_JSON, JsonSerializer.Serialize(allowedCommanderCommands), false);

        CPH.SendMessage(prompt);
        CPH.SendMessage($"Commander moment: {commanderName} has {windowSeconds} seconds. Allowed commands: {string.Join(" / ", allowedCommanderCommands)}.");

        // VERIFY: project docs still mark SetTimerInterval(string, int) unconfirmed.
        // This is the practical v1 way to honor authored commander window durations until a different verified pattern is documented.
        RestartTimerWithInterval(TIMER_LOTAT_COMMANDER_WINDOW, windowSeconds);
        CPH.LogWarn($"[{logPrefix}] Routed node to commander window. sessionId='{sessionId}', nodeId='{node.NodeId}', commander='{commanderName}', targetUser='{commanderTargetUser}', seconds={windowSeconds}.");
    }

    private void OpenDiceWindow(string logPrefix, string sessionId, RuntimeStoryNode node)
    {
        if (node.DiceHook == null || !node.DiceHook.Enabled)
        {
            FailClosed(logPrefix, sessionId, $"Node '{node.NodeId}' attempted dice routing without an enabled dice_hook payload.");
            return;
        }

        string purpose = (node.DiceHook.Purpose ?? "").Trim();
        string successText = (node.DiceHook.SuccessText ?? "").Trim();
        string failureText = (node.DiceHook.FailureText ?? "").Trim();
        int windowSeconds = node.DiceHook.RollWindowSeconds;
        int successThreshold = node.DiceHook.SuccessThreshold;

        if (string.IsNullOrWhiteSpace(purpose) || string.IsNullOrWhiteSpace(successText) || string.IsNullOrWhiteSpace(failureText) || windowSeconds <= 0 || successThreshold < 1 || successThreshold > 90)
        {
            FailClosed(logPrefix, sessionId, $"Node '{node.NodeId}' has malformed dice_hook payload.");
            return;
        }

        CPH.SetGlobalVar(VAR_LOTAT_SESSION_STAGE, STAGE_DICE_OPEN, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ACTIVE_WINDOW, WINDOW_DICE, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, false, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_DICE_SUCCESS_THRESHOLD, successThreshold, false);

        CPH.SendMessage($"Dice moment: {purpose}");
        CPH.SendMessage($"Anyone in chat can use !roll for the next {windowSeconds} seconds. Success on {successThreshold}+.");

        // VERIFY: project docs still mark SetTimerInterval(string, int) unconfirmed.
        RestartTimerWithInterval(TIMER_LOTAT_DICE_WINDOW, windowSeconds);
        CPH.LogWarn($"[{logPrefix}] Routed node to dice window. sessionId='{sessionId}', nodeId='{node.NodeId}', threshold={successThreshold}, seconds={windowSeconds}.");
    }

    private void OpenDecisionWindow(string logPrefix, string sessionId, RuntimeStoryNode node, List<string> allowedDecisionCommands)
    {
        if (allowedDecisionCommands == null || allowedDecisionCommands.Count <= 0)
        {
            FailClosed(logPrefix, sessionId, $"Node '{node.NodeId}' cannot open a decision window because no allowed decision commands were available.");
            return;
        }

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
        RestartTimerWithInterval(TIMER_LOTAT_DECISION_WINDOW, DECISION_WINDOW_SECONDS);
        CPH.LogWarn($"[{logPrefix}] Routed node to decision window. sessionId='{sessionId}', nodeId='{node.NodeId}', allowedCommands='{string.Join(",", allowedDecisionCommands)}', seconds={DECISION_WINDOW_SECONDS}.");
    }

    private string BuildChoicePrompt(RuntimeStoryNode node)
    {
        if (node.Choices == null || node.Choices.Count <= 0)
            return "No choices are available for this node.";

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

    private void RouteEndingNode(string logPrefix, string sessionId, RuntimeStoryNode node)
    {
        string endState = (node.EndState ?? "").Trim().ToLowerInvariant();
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_STAGE, STAGE_ENDED, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ACTIVE_WINDOW, WINDOW_NONE, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, true, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_LAST_END_STATE, endState, false);
        CPH.LogWarn($"[{logPrefix}] Ending node reached and marked for centralized cleanup. sessionId='{sessionId}', nodeId='{node.NodeId}', endState='{endState}'.");
    }

    private void FailClosed(string logPrefix, string sessionId, string detail)
    {
        CPH.LogError($"[{logPrefix}] Fail-closed. sessionId='{sessionId}'. {detail}");
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_STAGE, STAGE_ENDED, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ACTIVE_WINDOW, WINDOW_NONE, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, true, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_LAST_END_STATE, END_REASON_FAULT_ABORT, false);
    }

    private void DisableActiveNodeTimers()
    {
        CPH.DisableTimer(TIMER_LOTAT_COMMANDER_WINDOW);
        CPH.DisableTimer(TIMER_LOTAT_DICE_WINDOW);
        CPH.DisableTimer(TIMER_LOTAT_DECISION_WINDOW);
    }

    private void ClearPerNodeTransientState()
    {
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ACTIVE_WINDOW, WINDOW_NONE, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, false, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ALLOWED_COMMANDS_JSON, "[]", false);
        CPH.SetGlobalVar(VAR_LOTAT_VOTE_MAP_JSON, "{}", false);
        CPH.SetGlobalVar(VAR_LOTAT_VOTE_VALID_COUNT, 0, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_NAME, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_TARGET_USER, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_ALLOWED_COMMANDS_JSON, "[]", false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_DICE_SUCCESS_THRESHOLD, 0, false);
    }

    private List<string> ReadAllowedDecisionCommandsFromState()
    {
        string json = CPH.GetGlobalVar<string>(VAR_LOTAT_NODE_ALLOWED_COMMANDS_JSON, false) ?? "[]";
        if (string.IsNullOrWhiteSpace(json))
            return new List<string>();

        try
        {
            List<string> commands = JsonSerializer.Deserialize<List<string>>(json);
            return commands ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private string GetAssignedCommanderUser(string commanderName)
    {
        string raw = "";

        switch ((commanderName ?? "").Trim().ToLowerInvariant())
        {
            case "captain stretch":
                raw = CPH.GetGlobalVar<string>(VAR_CURRENT_CAPTAIN_STRETCH, false) ?? "";
                break;

            case "the director":
                raw = CPH.GetGlobalVar<string>(VAR_CURRENT_THE_DIRECTOR, false) ?? "";
                break;

            case "the water wizard":
                raw = CPH.GetGlobalVar<string>(VAR_CURRENT_WATER_WIZARD, false) ?? "";
                break;
        }

        return NormalizeUser(raw);
    }

    private List<string> GetAllowedCommanderCommands(string commanderName)
    {
        switch ((commanderName ?? "").Trim().ToLowerInvariant())
        {
            case "captain stretch":
                return new List<string> { "!stretch", "!shrimp" };

            case "the director":
                return new List<string> { "!checkchat", "!toad" };

            case "the water wizard":
                return new List<string> { "!hydrate", "!orb" };

            default:
                return new List<string>();
        }
    }

    private string NormalizeUser(string rawUser)
    {
        string normalized = (rawUser ?? "").Trim();
        if (normalized.StartsWith("@", StringComparison.Ordinal))
            normalized = normalized.Substring(1);

        return normalized.Trim().ToLowerInvariant();
    }

    private void RestartTimerWithInterval(string timerName, int seconds)
    {
        CPH.DisableTimer(timerName);

        // VERIFY: unconfirmed method signature per project helper docs.
        CPH.SetTimerInterval(timerName, seconds);
        CPH.EnableTimer(timerName);
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

        if (story == null)
        {
            failureReason = $"Runtime story JSON deserialized to null. path='{resolvedPath}'";
            return false;
        }

        if (string.IsNullOrWhiteSpace(story.StoryId) || story.Nodes == null || story.Nodes.Count <= 0)
        {
            failureReason = $"Runtime story is minimally unusable. storyId='{story?.StoryId ?? ""}', path='{resolvedPath}'.";
            return false;
        }

        CPH.LogWarn($"[{logPrefix}] Runtime story loaded for node entry. storyId='{story.StoryId}', path='{resolvedPath}'.");
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
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_LAST_END_STATE, "", false);
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

        [JsonPropertyName("node_type")]
        public string NodeType { get; set; }

        [JsonPropertyName("ship_section")]
        public string ShipSection { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("read_aloud")]
        public string ReadAloud { get; set; }

        [JsonPropertyName("chaos")]
        public RuntimeChaos Chaos { get; set; }

        [JsonPropertyName("dice_hook")]
        public RuntimeDiceHook DiceHook { get; set; }

        [JsonPropertyName("commander_moment")]
        public RuntimeCommanderMoment CommanderMoment { get; set; }

        [JsonPropertyName("choices")]
        public List<RuntimeStoryChoice> Choices { get; set; }

        [JsonPropertyName("end_state")]
        public string EndState { get; set; }
    }

    private sealed class RuntimeChaos
    {
        [JsonPropertyName("delta")]
        public int Delta { get; set; }
    }

    private sealed class RuntimeDiceHook
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("purpose")]
        public string Purpose { get; set; }

        [JsonPropertyName("roll_window_seconds")]
        public int RollWindowSeconds { get; set; }

        [JsonPropertyName("success_threshold")]
        public int SuccessThreshold { get; set; }

        [JsonPropertyName("failure_text")]
        public string FailureText { get; set; }

        [JsonPropertyName("success_text")]
        public string SuccessText { get; set; }
    }

    private sealed class RuntimeCommanderMoment
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("commander")]
        public string Commander { get; set; }

        [JsonPropertyName("prompt")]
        public string Prompt { get; set; }

        [JsonPropertyName("window_seconds")]
        public int WindowSeconds { get; set; }

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
