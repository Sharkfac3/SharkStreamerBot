using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

public class CPHInline
{
    private const string VAR_MINIGAME_ACTIVE    = "minigame_active";
    private const string VAR_MINIGAME_NAME      = "minigame_name";
    private const string MINIGAME_NAME          = "clone_empire";
    private const string VAR_EMPIRE_GAME_ACTIVE = "empire_game_active";
    private const string VAR_EMPIRE_JOIN_ACTIVE = "empire_join_active";
    private const string VAR_EMPIRE_GAME_START_UTC = "empire_game_start_utc";
    private const string VAR_EMPIRE_PLAYERS_JSON = "empire_players_json";
    private const string VAR_EMPIRE_CELLS_JSON  = "empire_cells_json";
    private const string TIMER_JOIN_WINDOW      = "Clone - Join Window";
    private const string TOPIC_CLONE_START      = "squad.clone.start";
    private const int    BROKER_WS_INDEX        = 0;
    private const int    EMPIRE_SPAWN_COL       = 16;
    private const int    EMPIRE_SPAWN_ROW       = 9;
    private const int    EMPIRE_JOIN_WINDOW_SECONDS = 60;
    private const int    EMPIRE_GRID_COLS       = 32;
    private const int    EMPIRE_GRID_ROWS       = 18;

    // Runtime source of truth: Actions/Squad/Clone/README.md
    // Shared names/constants reference: Actions/SHARED-CONSTANTS.md
    public bool Execute()
    {
        string user = "";
        string userId = "";
        CPH.TryGetArg("user", out user);
        CPH.TryGetArg("userId", out userId);

        user = (user ?? "").Trim();
        userId = NormalizeUserId(userId, user);

        bool lockActive = CPH.GetGlobalVar<bool?>(VAR_MINIGAME_ACTIVE, false) ?? false;
        string lockName = CPH.GetGlobalVar<string>(VAR_MINIGAME_NAME, false) ?? "";
        if (lockActive && !string.Equals(lockName, MINIGAME_NAME, StringComparison.OrdinalIgnoreCase))
        {
            CPH.SendMessage($"🎮 A mini-game is already running ({lockName}).");
            return true;
        }

        bool gameAlreadyActive = CPH.GetGlobalVar<bool?>(VAR_EMPIRE_GAME_ACTIVE, false) ?? false;
        if (gameAlreadyActive)
        {
            CPH.SendMessage("⚔️ The Empire is already advancing! Type !join to join the resistance.");
            return true;
        }

        TryAcquireMiniGameLock();

        var players = new List<EmpirePlayer>
        {
            new EmpirePlayer
            {
                UserId = userId,
                UserName = user,
                Col = EMPIRE_SPAWN_COL,
                Row = EMPIRE_SPAWN_ROW,
                LastMoveUtc = 0
            }
        };

        CPH.SetGlobalVar(VAR_EMPIRE_GAME_ACTIVE, true, false);
        CPH.SetGlobalVar(VAR_EMPIRE_JOIN_ACTIVE, true, false);
        CPH.SetGlobalVar(VAR_EMPIRE_GAME_START_UTC, 0L, false);
        CPH.SetGlobalVar(VAR_EMPIRE_PLAYERS_JSON, SerializeJson(players), false);
        CPH.SetGlobalVar(VAR_EMPIRE_CELLS_JSON, "[]", false);

        // Reset then enable so a stale countdown cannot instantly fire.
        CPH.DisableTimer(TIMER_JOIN_WINDOW);
        CPH.EnableTimer(TIMER_JOIN_WINDOW);

        PublishCloneStart(user, players);

        CPH.SendMessage($"⚔️ EMPIRE APPROACHES! Type !join in the next 60 seconds to join the Rebel defense! {user} leads the resistance! (1 rebel)");
        return true;
    }

    private string NormalizeUserId(string userId, string user)
    {
        string normalized = (userId ?? "").Trim();
        if (!string.IsNullOrWhiteSpace(normalized))
            return normalized;

        return (user ?? "").Trim().ToLowerInvariant();
    }

    private bool TryAcquireMiniGameLock()
    {
        bool lockActive = (CPH.GetGlobalVar<bool?>(VAR_MINIGAME_ACTIVE, false) ?? false);
        string lockName = CPH.GetGlobalVar<string>(VAR_MINIGAME_NAME, false) ?? "";

        if (lockActive && !string.Equals(lockName, MINIGAME_NAME, StringComparison.OrdinalIgnoreCase))
            return false;

        CPH.SetGlobalVar(VAR_MINIGAME_ACTIVE, true, false);
        CPH.SetGlobalVar(VAR_MINIGAME_NAME, MINIGAME_NAME, false);
        return true;
    }

    private void PublishCloneStart(string triggeredBy, List<EmpirePlayer> players)
    {
        var playerPayload = new List<Dictionary<string, object>>();
        for (int i = 0; i < players.Count; i++)
        {
            EmpirePlayer player = players[i];
            playerPayload.Add(new Dictionary<string, object>
            {
                { "userId", player.UserId ?? "" },
                { "userName", player.UserName ?? "" },
                { "col", player.Col },
                { "row", player.Row }
            });
        }

        var payload = new Dictionary<string, object>
        {
            { "game", "clone" },
            { "triggeredBy", triggeredBy ?? "" },
            { "phase", "join" },
            { "joinWindowSeconds", EMPIRE_JOIN_WINDOW_SECONDS },
            { "players", playerPayload },
            { "gridCols", EMPIRE_GRID_COLS },
            { "gridRows", EMPIRE_GRID_ROWS }
        };

        PublishBrokerMessage(TOPIC_CLONE_START, payload);
    }

    private bool PublishBrokerMessage(string topic, object payload)
    {
        try
        {
            var envelope = new Dictionary<string, object>
            {
                { "id", Guid.NewGuid().ToString() },
                { "topic", topic ?? "" },
                { "sender", "streamerbot" },
                { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
                { "payload", payload }
            };

            string json = SerializeJson(envelope);
            CPH.WebsocketSend(json, BROKER_WS_INDEX);
            return true;
        }
        catch (Exception ex)
        {
            CPH.LogError($"[Clone Empire Start] Failed to publish broker topic '{topic}': {ex}");
            return false;
        }
    }

    [DataContract]
    private class EmpirePlayer
    {
        [DataMember(Name = "userId")] public string UserId { get; set; }
        [DataMember(Name = "userName")] public string UserName { get; set; }
        [DataMember(Name = "col")] public int Col { get; set; }
        [DataMember(Name = "row")] public int Row { get; set; }
        [DataMember(Name = "lastMoveUtc")] public long LastMoveUtc { get; set; }
    }

    [DataContract]
    private class EmpireCell
    {
        [DataMember(Name = "col")] public int Col { get; set; }
        [DataMember(Name = "row")] public int Row { get; set; }
    }

    // JSON helpers: self-contained for Streamer.bot paste compatibility.
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
                builder.Append("\\\\");
            else if (current == '"')
                builder.Append("\\\"");
            else if (current == '\b')
                builder.Append("\\b");
            else if (current == '\f')
                builder.Append("\\f");
            else if (current == '\n')
                builder.Append("\\n");
            else if (current == '\r')
                builder.Append("\\r");
            else if (current == '\t')
                builder.Append("\\t");
            else if (current < 32)
                builder.Append("\\u" + ((int)current).ToString("x4", System.Globalization.CultureInfo.InvariantCulture));
            else
                builder.Append(current);
        }

        return builder.ToString();
    }
}
