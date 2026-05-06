// ACTION-CONTRACT: Actions/Squad/AGENTS.md#Clone/clone-empire-start.cs
// ACTION-CONTRACT-SHA256: 486e85f6c2e6225b72cb94eb854cd406ac2bd480e04ae5365b4a0f0f4b49e1d3

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

public class CPHInline
{
    private const string VAR_MINIGAME_ACTIVE     = "minigame_active";
    private const string VAR_MINIGAME_NAME       = "minigame_name";
    private const string MINIGAME_NAME           = "clone_empire";
    private const string VAR_EMPIRE_GAME_ACTIVE  = "empire_game_active";
    private const string VAR_EMPIRE_JOIN_ACTIVE  = "empire_join_active";
    private const string VAR_EMPIRE_GAME_START_UTC = "empire_game_start_utc";
    private const string VAR_EMPIRE_PLAYERS_JSON = "empire_players_json";
    private const string VAR_EMPIRE_CELLS_JSON   = "empire_cells_json";
    private const string TIMER_JOIN_WINDOW       = "Clone - Join Window";
    private const string TIMER_GAME_TICK         = "Clone - Game Tick";
    private const string TOPIC_CLONE_UPDATE      = "squad.clone.update";
    private const int    BROKER_WS_INDEX         = 0;
    private const int    EMPIRE_GRID_COLS        = 32;
    private const int    EMPIRE_GRID_ROWS        = 18;
    private const int    EMPIRE_SPAWN_COL        = 16;
    private const int    EMPIRE_SPAWN_ROW        = 9;
    private const int    EMPIRE_INITIAL_COUNT    = 5;
    private const int    EMPIRE_SPAWN_ATTEMPT_LIMIT = 1000;

    /*
     * Purpose:
     * - Ends the Clone Empire join window and opens the movement phase.
     * - Spawns Empire ships, stamps the movement start time, updates all
     *   player inactivity clocks, starts the repeating game tick, and
     *   publishes the opening full-state update.
     *
     * Expected trigger/input:
     * - Streamer.bot timer trigger for Clone - Join Window.
     * - No trigger args required.
     *
     * Required runtime variables:
     * - Reads/writes shared mini-game lock: minigame_active, minigame_name.
     * - Reads/writes empire_game_active, empire_join_active, empire_game_start_utc,
     *   empire_players_json, empire_cells_json.
     * - Disables timer: Clone - Join Window.
     * - Enables timer: Clone - Game Tick.
     *
     * Key outputs/side effects:
     * - Cancels the game cleanly when no players remain.
     * - Spawns up to 5 unique Empire cells away from the center spawn.
     * - Publishes broker topic squad.clone.update with event game_start.
     * - Sends chat feedback for cancel and movement-open paths.
     *
     * Operator notes:
     * - The timer Clone - Game Tick should already exist in Streamer.bot but stay disabled until this script enables it.
     * - Prompt 03 will provide the Clone - Empire - Tick action that consumes the repeating timer.
     */
    public bool Execute()
    {
        // Stop the one-shot timer first so a duplicate fire cannot reopen this path.
        CPH.DisableTimer(TIMER_JOIN_WINDOW);
        CPH.SetGlobalVar(VAR_EMPIRE_JOIN_ACTIVE, false, false);

        List<EmpirePlayer> players = LoadPlayers();
        if (players.Count <= 0)
        {
            ReleaseMiniGameLockIfOwned();
            CPH.SetGlobalVar(VAR_EMPIRE_GAME_ACTIVE, false, false);
            CPH.SetGlobalVar(VAR_EMPIRE_GAME_START_UTC, 0L, false);
            CPH.SetGlobalVar(VAR_EMPIRE_PLAYERS_JSON, "[]", false);
            CPH.SetGlobalVar(VAR_EMPIRE_CELLS_JSON, "[]", false);
            CPH.SendMessage("⚔️ No rebels showed up. The Empire claims this sector unopposed.");
            return true;
        }

        List<EmpireCell> empireCells = SpawnEmpireCells();
        long gameStartUtc = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        for (int i = 0; i < players.Count; i++)
            players[i].LastMoveUtc = gameStartUtc;

        CPH.SetGlobalVar(VAR_EMPIRE_GAME_START_UTC, gameStartUtc, false);
        CPH.SetGlobalVar(VAR_EMPIRE_PLAYERS_JSON, SerializeJson(players), false);
        CPH.SetGlobalVar(VAR_EMPIRE_CELLS_JSON, SerializeJson(empireCells), false);

        // Reset before enable so a stale repeating timer state cannot carry over.
        CPH.DisableTimer(TIMER_GAME_TICK);
        CPH.EnableTimer(TIMER_GAME_TICK);

        PublishGameStart(players, empireCells);

        int rebelCount = players.Count;
        int empireCount = empireCells.Count;
        CPH.SendMessage($"⚔️ {rebelCount} Rebels on the grid! The Empire has deployed {empireCount} ships. MOVE with !up !down !left !right — survive 5 minutes to unlock Clone! ☠️ Danger: standing still for 30s = death!");
        return true;
    }

    private List<EmpirePlayer> LoadPlayers()
    {
        string rawJson = CPH.GetGlobalVar<string>(VAR_EMPIRE_PLAYERS_JSON, false) ?? "[]";
        if (string.IsNullOrWhiteSpace(rawJson))
            rawJson = "[]";

        try
        {
            return DeserializeJson<List<EmpirePlayer>>(rawJson) ?? new List<EmpirePlayer>();
        }
        catch (Exception ex)
        {
            CPH.LogWarn($"[Clone Empire Start] Failed to parse player JSON. Resetting to empty list. {ex.Message}");
            return new List<EmpirePlayer>();
        }
    }

    private List<EmpireCell> SpawnEmpireCells()
    {
        var empireCells = new List<EmpireCell>();
        Random random = new Random();
        int attempts = 0;

        while (empireCells.Count < EMPIRE_INITIAL_COUNT && attempts < EMPIRE_SPAWN_ATTEMPT_LIMIT)
        {
            attempts++;

            int col = random.Next(1, EMPIRE_GRID_COLS + 1);
            int row = random.Next(1, EMPIRE_GRID_ROWS + 1);

            // Never let Empire spawn on the shared player center spawn.
            if (col == EMPIRE_SPAWN_COL && row == EMPIRE_SPAWN_ROW)
                continue;

            bool duplicate = false;
            for (int i = 0; i < empireCells.Count; i++)
            {
                if (empireCells[i].Col == col && empireCells[i].Row == row)
                {
                    duplicate = true;
                    break;
                }
            }

            if (duplicate)
                continue;

            empireCells.Add(new EmpireCell { Col = col, Row = row });
        }

        if (empireCells.Count < EMPIRE_INITIAL_COUNT)
            CPH.LogWarn($"[Clone Empire Start] Empire spawn attempt limit reached. Spawned {empireCells.Count} of {EMPIRE_INITIAL_COUNT} ships.");

        return empireCells;
    }

    private void ReleaseMiniGameLockIfOwned()
    {
        string lockName = CPH.GetGlobalVar<string>(VAR_MINIGAME_NAME, false) ?? "";
        if (!string.Equals(lockName, MINIGAME_NAME, StringComparison.OrdinalIgnoreCase))
            return;

        CPH.SetGlobalVar(VAR_MINIGAME_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_MINIGAME_NAME, "", false);
    }

    private void PublishGameStart(List<EmpirePlayer> players, List<EmpireCell> empireCells)
    {
        var state = new Dictionary<string, object>
        {
            { "event", "game_start" },
            { "players", players },
            { "empire", empireCells },
            { "gridCols", EMPIRE_GRID_COLS },
            { "gridRows", EMPIRE_GRID_ROWS },
            { "elapsedSeconds", 0 },
            { "survivorCount", players.Count },
            { "empireCount", empireCells.Count },
            { "eventDetail", "Movement phase open" }
        };

        var payload = new Dictionary<string, object>
        {
            { "game", "clone" },
            { "state", state }
        };

        PublishBrokerMessage(TOPIC_CLONE_UPDATE, payload);
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

    // ── Deserialize ──────────────────────────────────────────────────────
    // Parses a JSON string and converts it to the requested type T.
    // Supports: primitives, strings, List<T>, Dictionary<string,T>, and
    // [DataContract]/[DataMember] classes.
    private T DeserializeJson<T>(string json)
    {
        object parsed = ParseJsonRoot(json);
        object converted = ConvertParsedValueToType(parsed, typeof(T));
        if (converted == null)
            return default(T);

        return (T)converted;
    }

    // ── Serialize ────────────────────────────────────────────────────────
    // Converts an object to a JSON string. Supports primitives, strings,
    // dictionaries, lists/enumerables, and [DataContract]/[DataMember] classes.
    private string SerializeJson<T>(T value)
    {
        return SerializeJsonValue(value);
    }

    // ── Root parser ──────────────────────────────────────────────────────
    private object ParseJsonRoot(string json)
    {
        int index = 0;
        string source = json ?? "";
        object value = ParseJsonValue(source, ref index);
        return value;
    }

    // ── Value dispatcher ─────────────────────────────────────────────────
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

    // ── Object ───────────────────────────────────────────────────────────
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

    // ── Array ────────────────────────────────────────────────────────────
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

    // ── String ───────────────────────────────────────────────────────────
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

    // ── Boolean ──────────────────────────────────────────────────────────
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

    // ── Null ─────────────────────────────────────────────────────────────
    private void ParseJsonNull(string source, ref int index)
    {
        if (source.Length >= index + 4 && string.Compare(source, index, "null", 0, 4, StringComparison.Ordinal) == 0)
        {
            index += 4;
            return;
        }

        throw new InvalidOperationException("Invalid JSON null value.");
    }

    // ── Number ───────────────────────────────────────────────────────────
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

    // ── Whitespace ───────────────────────────────────────────────────────
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

    // ── Type converter ───────────────────────────────────────────────────
    // Maps parsed Dictionary<string,object> / List<object> / primitives
    // into strongly-typed C# objects using [DataContract] attributes.
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

        // List<T>
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

        // Dictionary<string, T>
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

        // [DataContract] object mapping
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

    // ── Serializer ───────────────────────────────────────────────────────
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

        // Fall back to [DataContract] object serialization.
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

    // ── String escaping ──────────────────────────────────────────────────
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
