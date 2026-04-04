using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.IO;

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
            List<string> commands = DeserializeJson<List<string>>(allowedJson) ?? new List<string>();
            var normalized = new List<string>();
            var seen = new List<string>();

            foreach (string entry in commands)
            {
                string command = NormalizeCommand(entry);
                if (string.IsNullOrWhiteSpace(command))
                    continue;

                if (AddUniqueIgnoreCase(seen, command))
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
        var allowedSet = allowedCommands ?? new List<string>();

        try
        {
            Dictionary<string, string> rawMap = DeserializeJson<Dictionary<string, string>>(voteMapJson)
                ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<string, string> pair in rawMap)
            {
                string user = NormalizeUser(pair.Key);
                string command = NormalizeCommand(pair.Value);

                if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(command))
                    continue;

                if (allowedSet.Count > 0 && !ContainsIgnoreCase(allowedSet, command))
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
            story = DeserializeJson<RuntimeStoryDefinition>(json);
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
        var seen = new List<string>();

        foreach (string candidate in candidates)
        {
            if (string.IsNullOrWhiteSpace(candidate))
                continue;

            string normalized = candidate.Trim();
            if (AddUniqueIgnoreCase(seen, normalized))
                unique.Add(normalized);
        }

        return unique;
    }
    [DataContract]

    private sealed class RuntimeStoryDefinition
    {
        [DataMember(Name = "story_id")]
        public string StoryId { get; set; }

        [DataMember(Name = "nodes")]
        public List<RuntimeStoryNode> Nodes { get; set; }
    }
    [DataContract]

    private sealed class RuntimeStoryNode
    {
        [DataMember(Name = "node_id")]
        public string NodeId { get; set; }

        [DataMember(Name = "choices")]
        public List<RuntimeStoryChoice> Choices { get; set; }
    }
    [DataContract]

    private sealed class RuntimeStoryChoice
    {
        [DataMember(Name = "choice_id")]
        public string ChoiceId { get; set; }

        [DataMember(Name = "command")]
        public string Command { get; set; }

        [DataMember(Name = "result_flavor")]
        public string ResultFlavor { get; set; }

        [DataMember(Name = "next_node_id")]
        public string NextNodeId { get; set; }
    }

    private T DeserializeJson<T>(string json)
    {
        object parsed = ParseJsonRoot(json);
        object converted = ConvertParsedValueToType(parsed, typeof(T));
        if (converted == null)
            return default(T);

        return (T)converted;
    }

    private string SerializeJson<T>(T value)
    {
        return SerializeJsonValue(value);
    }

    private object ParseJsonRoot(string json)
    {
        int index = 0;
        string source = json ?? "";
        object value = ParseJsonValue(source, ref index);
        return value;
    }

    private object ParseJsonValue(string source, ref int index)
    {
        SkipJsonWhitespace(source, ref index);
        if (index >= source.Length)
            return null;

        char current = source[index];
        if (current == '{')
            return ParseJsonObject(source, ref index);

        if (current == '[')
            return ParseJsonArray(source, ref index);

        if (current == '"')
            return ParseJsonString(source, ref index);

        if (current == 't' || current == 'f')
            return ParseJsonBoolean(source, ref index);

        if (current == 'n')
        {
            ParseJsonNull(source, ref index);
            return null;
        }

        return ParseJsonNumber(source, ref index);
    }

    private Dictionary<string, object> ParseJsonObject(string source, ref int index)
    {
        var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        index++;
        SkipJsonWhitespace(source, ref index);

        if (index < source.Length && source[index] == '}')
        {
            index++;
            return result;
        }

        while (index < source.Length)
        {
            SkipJsonWhitespace(source, ref index);
            string key = ParseJsonString(source, ref index);
            SkipJsonWhitespace(source, ref index);

            if (index >= source.Length || source[index] != ':')
                throw new InvalidOperationException("Invalid JSON object: expected ':' after key.");

            index++;
            object value = ParseJsonValue(source, ref index);
            result[key] = value;

            SkipJsonWhitespace(source, ref index);
            if (index >= source.Length)
                break;

            if (source[index] == '}')
            {
                index++;
                break;
            }

            if (source[index] != ',')
                throw new InvalidOperationException("Invalid JSON object: expected ',' between properties.");

            index++;
        }

        return result;
    }

    private List<object> ParseJsonArray(string source, ref int index)
    {
        var result = new List<object>();
        index++;
        SkipJsonWhitespace(source, ref index);

        if (index < source.Length && source[index] == ']')
        {
            index++;
            return result;
        }

        while (index < source.Length)
        {
            object value = ParseJsonValue(source, ref index);
            result.Add(value);
            SkipJsonWhitespace(source, ref index);

            if (index >= source.Length)
                break;

            if (source[index] == ']')
            {
                index++;
                break;
            }

            if (source[index] != ',')
                throw new InvalidOperationException("Invalid JSON array: expected ',' between values.");

            index++;
        }

        return result;
    }

    private string ParseJsonString(string source, ref int index)
    {
        if (index >= source.Length || source[index] != '"')
            throw new InvalidOperationException("Invalid JSON string: expected opening quote.");

        index++;
        var builder = new StringBuilder();

        while (index < source.Length)
        {
            char current = source[index++];
            if (current == '"')
                return builder.ToString();

            if (current != '\\')
            {
                builder.Append(current);
                continue;
            }

            if (index >= source.Length)
                break;

            char escaped = source[index++];
            switch (escaped)
            {
                case '"': builder.Append('"'); break;
                case '\\': builder.Append('\\'); break;
                case '/': builder.Append('/'); break;
                case 'b': builder.Append('\b'); break;
                case 'f': builder.Append('\f'); break;
                case 'n': builder.Append('\n'); break;
                case 'r': builder.Append('\r'); break;
                case 't': builder.Append('\t'); break;
                case 'u':
                    if (index + 4 > source.Length)
                        throw new InvalidOperationException("Invalid JSON string: incomplete unicode escape.");

                    string hex = source.Substring(index, 4);
                    builder.Append((char)int.Parse(hex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture));
                    index += 4;
                    break;
                default:
                    builder.Append(escaped);
                    break;
            }
        }

        throw new InvalidOperationException("Invalid JSON string: missing closing quote.");
    }

    private bool ParseJsonBoolean(string source, ref int index)
    {
        if (source.Length >= index + 4 && string.Compare(source, index, "true", 0, 4, StringComparison.Ordinal) == 0)
        {
            index += 4;
            return true;
        }

        if (source.Length >= index + 5 && string.Compare(source, index, "false", 0, 5, StringComparison.Ordinal) == 0)
        {
            index += 5;
            return false;
        }

        throw new InvalidOperationException("Invalid JSON boolean value.");
    }

    private void ParseJsonNull(string source, ref int index)
    {
        if (source.Length >= index + 4 && string.Compare(source, index, "null", 0, 4, StringComparison.Ordinal) == 0)
        {
            index += 4;
            return;
        }

        throw new InvalidOperationException("Invalid JSON null value.");
    }

    private object ParseJsonNumber(string source, ref int index)
    {
        int start = index;
        while (index < source.Length)
        {
            char current = source[index];
            if ((current >= '0' && current <= '9') || current == '-' || current == '+' || current == '.' || current == 'e' || current == 'E')
            {
                index++;
                continue;
            }

            break;
        }

        string token = source.Substring(start, index - start);
        if (token.IndexOf('.') >= 0 || token.IndexOf('e') >= 0 || token.IndexOf('E') >= 0)
        {
            return double.Parse(token, System.Globalization.CultureInfo.InvariantCulture);
        }

        long longValue;
        if (long.TryParse(token, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out longValue))
        {
            if (longValue >= int.MinValue && longValue <= int.MaxValue)
                return (int)longValue;

            return longValue;
        }

        return 0;
    }

    private void SkipJsonWhitespace(string source, ref int index)
    {
        while (index < source.Length)
        {
            char current = source[index];
            if (current == ' ' || current == '\t' || current == '\n' || current == '\r')
            {
                index++;
                continue;
            }

            break;
        }
    }

    private object ConvertParsedValueToType(object value, Type targetType)
    {
        if (targetType == null)
            return null;

        Type nullableUnderlying = Nullable.GetUnderlyingType(targetType);
        if (nullableUnderlying != null)
            targetType = nullableUnderlying;

        if (value == null)
        {
            if (targetType.IsValueType)
                return Activator.CreateInstance(targetType);

            return null;
        }

        if (targetType == typeof(string))
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);

        if (targetType == typeof(int))
            return Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture);

        if (targetType == typeof(long))
            return Convert.ToInt64(value, System.Globalization.CultureInfo.InvariantCulture);

        if (targetType == typeof(bool))
            return Convert.ToBoolean(value, System.Globalization.CultureInfo.InvariantCulture);

        if (targetType == typeof(double))
            return Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);

        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
        {
            Type itemType = targetType.GetGenericArguments()[0];
            object listInstance = Activator.CreateInstance(targetType);
            var list = listInstance as System.Collections.IList;
            var rawList = value as List<object>;
            if (list == null || rawList == null)
                return listInstance;

            for (int i = 0; i < rawList.Count; i++)
                list.Add(ConvertParsedValueToType(rawList[i], itemType));

            return listInstance;
        }

        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            Type[] args = targetType.GetGenericArguments();
            if (args.Length == 2 && args[0] == typeof(string))
            {
                object dictInstance = Activator.CreateInstance(targetType);
                var dict = dictInstance as System.Collections.IDictionary;
                var rawDict = value as Dictionary<string, object>;
                if (dict == null || rawDict == null)
                    return dictInstance;

                foreach (KeyValuePair<string, object> pair in rawDict)
                    dict[pair.Key] = ConvertParsedValueToType(pair.Value, args[1]);

                return dictInstance;
            }
        }

        var objectMap = value as Dictionary<string, object>;
        if (objectMap == null)
            return Activator.CreateInstance(targetType);

        object instance = Activator.CreateInstance(targetType);
        System.Reflection.PropertyInfo[] properties = targetType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        for (int i = 0; i < properties.Length; i++)
        {
            System.Reflection.PropertyInfo property = properties[i];
            if (!property.CanWrite)
                continue;

            string jsonName = property.Name;
            object[] attributes = property.GetCustomAttributes(typeof(DataMemberAttribute), true);
            if (attributes != null && attributes.Length > 0)
            {
                DataMemberAttribute member = attributes[0] as DataMemberAttribute;
                if (member != null && !string.IsNullOrWhiteSpace(member.Name))
                    jsonName = member.Name;
            }

            object rawPropertyValue;
            if (!objectMap.TryGetValue(jsonName, out rawPropertyValue) && !objectMap.TryGetValue(property.Name, out rawPropertyValue))
                continue;

            property.SetValue(instance, ConvertParsedValueToType(rawPropertyValue, property.PropertyType), null);
        }

        return instance;
    }

    private string SerializeJsonValue(object value)
    {
        if (value == null)
            return "null";

        if (value is string)
            return "\"" + EscapeJsonString((string)value) + "\"";

        if (value is bool)
            return ((bool)value) ? "true" : "false";

        if (value is int || value is long || value is double || value is float || value is decimal || value is short || value is byte)
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);

        var dictionary = value as System.Collections.IDictionary;
        if (dictionary != null)
        {
            var parts = new List<string>();
            foreach (System.Collections.DictionaryEntry entry in dictionary)
            {
                string key = Convert.ToString(entry.Key, System.Globalization.CultureInfo.InvariantCulture) ?? "";
                parts.Add("\"" + EscapeJsonString(key) + "\":" + SerializeJsonValue(entry.Value));
            }

            return "{" + string.Join(",", parts.ToArray()) + "}";
        }

        if (!(value is string))
        {
            var enumerable = value as System.Collections.IEnumerable;
            if (enumerable != null)
            {
                var parts = new List<string>();
                foreach (object item in enumerable)
                    parts.Add(SerializeJsonValue(item));

                return "[" + string.Join(",", parts.ToArray()) + "]";
            }
        }

        var objectParts = new List<string>();
        System.Reflection.PropertyInfo[] properties = value.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        for (int i = 0; i < properties.Length; i++)
        {
            System.Reflection.PropertyInfo property = properties[i];
            if (!property.CanRead)
                continue;

            string jsonName = property.Name;
            object[] attributes = property.GetCustomAttributes(typeof(DataMemberAttribute), true);
            if (attributes != null && attributes.Length > 0)
            {
                DataMemberAttribute member = attributes[0] as DataMemberAttribute;
                if (member != null && !string.IsNullOrWhiteSpace(member.Name))
                    jsonName = member.Name;
            }

            object propertyValue = property.GetValue(value, null);
            objectParts.Add("\"" + EscapeJsonString(jsonName) + "\":" + SerializeJsonValue(propertyValue));
        }

        return "{" + string.Join(",", objectParts.ToArray()) + "}";
    }

    private string EscapeJsonString(string value)
    {
        string source = value ?? "";
        var builder = new StringBuilder();
        for (int i = 0; i < source.Length; i++)
        {
            char current = source[i];

            if (current == '\\')
            {
                builder.Append("\\\\");
            }
            else if (current == '"')
            {
                builder.Append("\\\"");
            }
            else if (current == '\b')
            {
                builder.Append("\\b");
            }
            else if (current == '\f')
            {
                builder.Append("\\f");
            }
            else if (current == '\n')
            {
                builder.Append("\\n");
            }
            else if (current == '\r')
            {
                builder.Append("\\r");
            }
            else if (current == '\t')
            {
                builder.Append("\\t");
            }
            else if (current < 32)
            {
                builder.Append("\\u" + ((int)current).ToString("x4", System.Globalization.CultureInfo.InvariantCulture));
            }
            else
            {
                builder.Append(current);
            }
        }

        return builder.ToString();
    }

    private bool ContainsIgnoreCase(List<string> values, string candidate)
    {
        if (values == null || string.IsNullOrWhiteSpace(candidate))
            return false;

        string normalizedCandidate = candidate.Trim();
        for (int i = 0; i < values.Count; i++)
        {
            string entry = values[i] ?? "";
            if (string.Equals(entry.Trim(), normalizedCandidate, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private bool AddUniqueIgnoreCase(List<string> values, string candidate)
    {
        if (values == null || string.IsNullOrWhiteSpace(candidate))
            return false;

        if (ContainsIgnoreCase(values, candidate))
            return false;

        values.Add(candidate);
        return true;
    }

}
