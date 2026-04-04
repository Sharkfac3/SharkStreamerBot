using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

public class CPHInline
{
    // -------------------------------------------------
    // LotAT shared runtime constants
    // Keep these names synchronized with Actions/SHARED-CONSTANTS.md.
    // -------------------------------------------------
    private const string VAR_LOTAT_ACTIVE = "lotat_active";
    private const string VAR_LOTAT_SESSION_ID = "lotat_session_id";
    private const string VAR_LOTAT_SESSION_STAGE = "lotat_session_stage";
    private const string VAR_LOTAT_SESSION_ROSTER_FROZEN = "lotat_session_roster_frozen";
    private const string VAR_LOTAT_SESSION_JOINED_ROSTER_JSON = "lotat_session_joined_roster_json";
    private const string VAR_LOTAT_SESSION_JOINED_COUNT = "lotat_session_joined_count";
    private const string VAR_LOTAT_NODE_ACTIVE_WINDOW = "lotat_node_active_window";
    private const string VAR_LOTAT_NODE_WINDOW_RESOLVED = "lotat_node_window_resolved";
    private const string VAR_LOTAT_NODE_ALLOWED_COMMANDS_JSON = "lotat_node_allowed_commands_json";
    private const string VAR_LOTAT_VOTE_MAP_JSON = "lotat_vote_map_json";
    private const string VAR_LOTAT_VOTE_VALID_COUNT = "lotat_vote_valid_count";

    private const string TIMER_LOTAT_DECISION_WINDOW = "LotAT - Decision Window";

    private const string STAGE_DECISION_OPEN = "decision_open";
    private const string WINDOW_DECISION = "decision";

    /*
     * Purpose:
     * - Shared authored decision-command intake for LotAT v1.
     * - Accepts any authored decision command, validates it against the active node,
     *   records one active valid vote per joined participant, and supports latest-valid-vote replacement.
     *
     * Expected trigger/input:
     * - Streamer.bot chat command triggers for all authored decision commands:
     *   !scan, !target, !analyze, !reroute, !deploy, !contain, !inspect, !drink, !simulate
     * - Reads the sender from `user` / `userName` and command text from `command`, `message`, or `rawInput`.
     *
     * Required runtime variables:
     * - Reads: active/session/stage/window guards, frozen joined roster, allowed decision commands,
     *   and the current per-node vote map.
     * - Writes: lotat_vote_map_json, lotat_vote_valid_count, lotat_node_window_resolved.
     *
     * Key outputs/side effects:
     * - Counts votes only during decision_open.
     * - Counts only users in the frozen joined roster.
     * - Counts only commands present in lotat_node_allowed_commands_json.
     * - Keeps one valid vote per joined participant for the active node.
     * - Later valid votes replace earlier valid votes from the same participant.
     * - Disables the decision timer and marks the node resolved when every joined participant has a valid vote.
     *
     * Operator notes:
     * - Wire every authored decision command trigger to this shared action.
     * - For early-close to advance immediately, put lotat-decision-resolve.cs and then lotat-node-enter.cs
     *   immediately after this action in the same Streamer.bot action group.
     */
    public bool Execute()
    {
        const string logPrefix = "LotAT Decision Input";

        try
        {
            if (!IsDecisionWindowOpen())
                return true;

            string participantKey = NormalizeUser(GetSenderUser());
            if (string.IsNullOrWhiteSpace(participantKey))
                return true;

            string command = NormalizeCommand(GetInvokedCommandText());
            if (string.IsNullOrWhiteSpace(command))
                return true;

            List<string> joinedRoster = ReadNormalizedRoster(logPrefix);
            if (!joinedRoster.Contains(participantKey))
                return true;

            List<string> allowedCommands = ReadAllowedCommands(logPrefix);
            if (!allowedCommands.Contains(command))
                return true;

            Dictionary<string, string> voteMap = ReadVoteMap(logPrefix, joinedRoster, allowedCommands);
            string priorVote = voteMap.ContainsKey(participantKey) ? voteMap[participantKey] : "";
            voteMap[participantKey] = command;

            int validCount = voteMap.Count;
            int joinedCount = joinedRoster.Count;
            CPH.SetGlobalVar(VAR_LOTAT_SESSION_JOINED_COUNT, joinedCount, false);

            CPH.SetGlobalVar(VAR_LOTAT_VOTE_MAP_JSON, SerializeJson(voteMap), false);
            CPH.SetGlobalVar(VAR_LOTAT_VOTE_VALID_COUNT, validCount, false);

            string sessionId = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_ID, false) ?? "";
            bool replacedVote = !string.IsNullOrWhiteSpace(priorVote) && !string.Equals(priorVote, command, StringComparison.OrdinalIgnoreCase);
            bool repeatedVote = string.Equals(priorVote, command, StringComparison.OrdinalIgnoreCase);
            CPH.LogWarn($"[{logPrefix}] Valid vote recorded. sessionId='{sessionId}', user='{participantKey}', command='{command}', prior='{priorVote}', repeated={repeatedVote}, replaced={replacedVote}, validCount={validCount}, joinedCount={joinedCount}.");

            if (joinedCount > 0 && validCount >= joinedCount)
            {
                CPH.DisableTimer(TIMER_LOTAT_DECISION_WINDOW);
                CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, true, false);
                CPH.LogWarn($"[{logPrefix}] Early close triggered because every joined participant has a valid vote. sessionId='{sessionId}', validCount={validCount}, joinedCount={joinedCount}.");
            }

            return true;
        }
        catch (Exception ex)
        {
            CPH.LogError($"[{logPrefix}] Unhandled exception while processing authored decision input: {ex}");
            return true;
        }
    }

    private bool IsDecisionWindowOpen()
    {
        bool active = CPH.GetGlobalVar<bool?>(VAR_LOTAT_ACTIVE, false) ?? false;
        if (!active)
            return false;

        string stage = (CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_STAGE, false) ?? "").Trim();
        if (!string.Equals(stage, STAGE_DECISION_OPEN, StringComparison.OrdinalIgnoreCase))
            return false;

        string activeWindow = (CPH.GetGlobalVar<string>(VAR_LOTAT_NODE_ACTIVE_WINDOW, false) ?? "").Trim();
        if (!string.Equals(activeWindow, WINDOW_DECISION, StringComparison.OrdinalIgnoreCase))
            return false;

        bool rosterFrozen = CPH.GetGlobalVar<bool?>(VAR_LOTAT_SESSION_ROSTER_FROZEN, false) ?? false;
        if (!rosterFrozen)
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

    private List<string> ReadNormalizedRoster(string logPrefix)
    {
        string rosterJson = CPH.GetGlobalVar<string>(VAR_LOTAT_SESSION_JOINED_ROSTER_JSON, false) ?? "[]";
        if (string.IsNullOrWhiteSpace(rosterJson))
            rosterJson = "[]";

        try
        {
            List<string> roster = DeserializeJson<List<string>>(rosterJson) ?? new List<string>();
            var normalizedRoster = new List<string>();
            var seen = new List<string>();

            foreach (string entry in roster)
            {
                string normalized = NormalizeUser(entry);
                if (string.IsNullOrWhiteSpace(normalized))
                    continue;

                if (AddUniqueIgnoreCase(seen, normalized))
                    normalizedRoster.Add(normalized);
            }

            return normalizedRoster;
        }
        catch (Exception ex)
        {
            CPH.LogWarn($"[{logPrefix}] Failed to parse joined roster JSON. Treating roster as empty for safety. exception='{ex.Message}'");
            return new List<string>();
        }
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
            CPH.LogWarn($"[{logPrefix}] Failed to parse allowed commands JSON. Treating allowed commands as empty for safety. exception='{ex.Message}'");
            return new List<string>();
        }
    }

    private Dictionary<string, string> ReadVoteMap(string logPrefix, List<string> joinedRoster, List<string> allowedCommands)
    {
        string voteMapJson = CPH.GetGlobalVar<string>(VAR_LOTAT_VOTE_MAP_JSON, false) ?? "{}";
        if (string.IsNullOrWhiteSpace(voteMapJson))
            voteMapJson = "{}";

        var filteredMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var rosterSet = joinedRoster ?? new List<string>();
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

                if (!ContainsIgnoreCase(rosterSet, user))
                    continue;

                if (!ContainsIgnoreCase(allowedSet, command))
                    continue;

                filteredMap[user] = command;
            }
        }
        catch (Exception ex)
        {
            CPH.LogWarn($"[{logPrefix}] Failed to parse vote map JSON. Resetting vote map for safety. exception='{ex.Message}'");
        }

        return filteredMap;
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
