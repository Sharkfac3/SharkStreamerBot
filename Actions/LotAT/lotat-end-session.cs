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
            story = DeserializeJson<RuntimeStoryDefinition>(json);
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
        [DataMember(Name = "nodes")]
        public List<RuntimeStoryNode> Nodes { get; set; }
    }
    [DataContract]

    private sealed class RuntimeStoryNode
    {
        [DataMember(Name = "node_id")]
        public string NodeId { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "end_state")]
        public string EndState { get; set; }
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
