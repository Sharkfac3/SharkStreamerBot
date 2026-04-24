using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

public class CPHInline
{
    private const string VAR_MINIGAME_ACTIVE       = "minigame_active";
    private const string VAR_MINIGAME_NAME         = "minigame_name";
    private const string MINIGAME_NAME             = "clone_empire";
    private const string VAR_EMPIRE_GAME_ACTIVE    = "empire_game_active";
    private const string VAR_EMPIRE_JOIN_ACTIVE    = "empire_join_active";
    private const string VAR_EMPIRE_GAME_START_UTC = "empire_game_start_utc";
    private const string VAR_EMPIRE_PLAYERS_JSON   = "empire_players_json";
    private const string VAR_EMPIRE_CELLS_JSON     = "empire_cells_json";
    private const string TIMER_GAME_TICK           = "Clone - Game Tick";
    private const string TOPIC_CLONE_UPDATE        = "squad.clone.update";
    private const string TOPIC_CLONE_END           = "squad.clone.end";
    private const int    BROKER_WS_INDEX           = 0;
    private const int    EMPIRE_GRID_COLS          = 32;
    private const int    EMPIRE_GRID_ROWS          = 18;
    private const long   EMPIRE_MOVE_COOLDOWN_MS   = 1000L;
    private const string MIXITUP_CLONE_UNLOCK_COMMAND_ID = "REPLACE_WITH_CLONE_UNLOCK_COMMAND_ID";
    private const int    WAIT_MIXITUP_UNLOCK_STARTUP_MS  = 3000;

    /*
     * Purpose:
     * - Handles Clone Empire movement commands during the live movement phase.
     * - Applies movement cooldown, grid boundaries, death-by-empire, empire growth,
     *   and local Rule 3 empire collapse checks.
     *
     * Expected trigger/input:
     * - Streamer.bot command triggers for !up, !down, !left, and !right.
     * - This script uses one shared action for all four commands.
     * - Direction is read from the rawInput arg (example: !up).
     * - Required args: user, userId, rawInput.
     *
     * Required runtime variables:
     * - Reads shared mini-game lock globals: minigame_active, minigame_name.
     * - Reads/writes empire_game_active, empire_join_active, empire_players_json,
     *   empire_cells_json, empire_game_start_utc.
     *
     * Key outputs/side effects:
     * - Moves a living player one cell when valid.
     * - Silently ignores invalid directions, cooldown hits, and off-grid moves.
     * - Removes players who enter empire territory.
     * - Grows empire territory from the player's departure cell when Rule 2 is met.
     * - Publishes squad.clone.update and squad.clone.end broker events.
     *
     * Operator notes:
     * - Add this action as an additional action on the existing !up / !down / !left / !right triggers.
     * - Do not remove the existing Destroyer movement action.
     * - No extra Streamer.bot direction arg is needed because this script reads rawInput directly.
     */
    public bool Execute()
    {
        string lockName = CPH.GetGlobalVar<string>(VAR_MINIGAME_NAME, false) ?? "";
        if (!string.Equals(lockName, MINIGAME_NAME, StringComparison.OrdinalIgnoreCase))
            return true;

        bool gameActive = CPH.GetGlobalVar<bool?>(VAR_EMPIRE_GAME_ACTIVE, false) ?? false;
        bool joinActive = CPH.GetGlobalVar<bool?>(VAR_EMPIRE_JOIN_ACTIVE, false) ?? false;

        string user = "";
        string userId = "";
        string rawInput = "";
        CPH.TryGetArg("user", out user);
        CPH.TryGetArg("userId", out userId);
        CPH.TryGetArg("rawInput", out rawInput);

        user = (user ?? "").Trim();
        userId = NormalizeUserId(userId, user);

        if (!gameActive)
            return true;

        if (joinActive)
        {
            CPH.SendMessage($"@{user} join phase is still open — movement starts soon!");
            return true;
        }

        string direction = ParseDirection(rawInput);
        if (string.IsNullOrWhiteSpace(direction))
            return true;

        List<EmpirePlayer> players = LoadPlayers();
        int playerIndex = players.FindIndex(p => string.Equals(p.UserId ?? "", userId, StringComparison.OrdinalIgnoreCase));
        if (playerIndex < 0)
        {
            CPH.SendMessage($"@{user} you're not in this game!");
            return true;
        }

        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        EmpirePlayer player = players[playerIndex];
        if (now - player.LastMoveUtc < EMPIRE_MOVE_COOLDOWN_MS)
            return true;

        int newCol = player.Col;
        int newRow = player.Row;
        ApplyDirection(direction, ref newCol, ref newRow);

        if (newCol < 1 || newCol > EMPIRE_GRID_COLS || newRow < 1 || newRow > EMPIRE_GRID_ROWS)
            return true;

        List<EmpireCell> empire = LoadEmpire();

        // Rule 1: entering empire territory is instant death.
        if (ContainsEmpireCell(empire, newCol, newRow))
        {
            players.RemoveAt(playerIndex);
            SavePlayers(players);

            CPH.SendMessage($"💀 {player.UserName} flew into Empire territory and was destroyed!");
            PublishCloneUpdate("player_died", players, empire, player.UserName);

            if (players.Count == 0)
                TriggerLoss();

            return true;
        }

        int oldCol = player.Col;
        int oldRow = player.Row;

        player.Col = newCol;
        player.Row = newRow;
        player.LastMoveUtc = now;
        players[playerIndex] = player;

        bool empireAdded = false;
        if (ShouldGrowEmpire(empire, oldCol, oldRow))
        {
            empireAdded = AddEmpireCellIfMissing(empire, oldCol, oldRow);
            if (empireAdded)
                Rule3Check(empire, players, new List<int> { oldCol }, new List<int> { oldRow });
        }

        SavePlayers(players);
        SaveEmpire(empire);

        if (empireAdded)
            PublishCloneUpdate("empire_spawned", players, empire, oldCol + "," + oldRow);
        else
            PublishCloneUpdate("player_moved", players, empire, player.UserName);

        if (players.Count == 0)
            TriggerLoss();

        return true;
    }

    private string NormalizeUserId(string userId, string user)
    {
        string normalized = (userId ?? "").Trim();
        if (!string.IsNullOrWhiteSpace(normalized))
            return normalized;

        return (user ?? "").Trim().ToLowerInvariant();
    }

    private string ParseDirection(string rawInput)
    {
        string value = (rawInput ?? "").Trim().ToLowerInvariant();
        if (value.StartsWith("!", StringComparison.Ordinal))
            value = value.Substring(1);

        if (value == "up" || value == "down" || value == "left" || value == "right")
            return value;

        return "";
    }

    private void ApplyDirection(string direction, ref int col, ref int row)
    {
        switch (direction)
        {
            case "up":
                row -= 1;
                break;
            case "down":
                row += 1;
                break;
            case "left":
                col -= 1;
                break;
            case "right":
                col += 1;
                break;
        }
    }

    private bool ShouldGrowEmpire(List<EmpireCell> empire, int oldCol, int oldRow)
    {
        int[] dCol = { 0, 0, -1, 1 };
        int[] dRow = { -1, 1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int neighborCol = oldCol + dCol[i];
            int neighborRow = oldRow + dRow[i];
            if (!ContainsEmpireCell(empire, neighborCol, neighborRow))
                continue;

            for (int j = 0; j < 4; j++)
            {
                int otherCol = neighborCol + dCol[j];
                int otherRow = neighborRow + dRow[j];
                if (otherCol == oldCol && otherRow == oldRow)
                    continue;

                if (ContainsEmpireCell(empire, otherCol, otherRow))
                    return true;
            }
        }

        return false;
    }

    // Checks newly added empire cells for encirclement. Modifies empireList in-place.
    // Returns list of cells removed.
    private List<EmpireCell> Rule3Check(
        List<EmpireCell> empire,
        List<EmpirePlayer> players,
        List<int> checkCols,
        List<int> checkRows)
    {
        var removed = new List<EmpireCell>();
        var toCheck = new Queue<Tuple<int, int>>();
        for (int i = 0; i < checkCols.Count; i++)
            toCheck.Enqueue(Tuple.Create(checkCols[i], checkRows[i]));

        int iterations = 0;
        while (toCheck.Count > 0 && iterations < 100)
        {
            iterations++;
            Tuple<int, int> current = toCheck.Dequeue();
            int col = current.Item1;
            int row = current.Item2;

            if (!ContainsEmpireCell(empire, col, row))
                continue;

            int[] dCol = { 0, 0, -1, 1 };
            int[] dRow = { -1, 1, 0, 0 };
            bool allOccupied = true;

            for (int d = 0; d < 4; d++)
            {
                int nc = col + dCol[d];
                int nr = row + dRow[d];

                bool isEdge = nc < 1 || nc > EMPIRE_GRID_COLS || nr < 1 || nr > EMPIRE_GRID_ROWS;
                bool isEmpire = ContainsEmpireCell(empire, nc, nr);
                bool isPlayer = ContainsPlayer(players, nc, nr);

                if (!isEdge && !isEmpire && !isPlayer)
                {
                    allOccupied = false;
                    break;
                }
            }

            if (allOccupied)
            {
                empire.RemoveAll(e => e.Col == col && e.Row == row);
                removed.Add(new EmpireCell { Col = col, Row = row });

                for (int d = 0; d < 4; d++)
                {
                    int nc = col + dCol[d];
                    int nr = row + dRow[d];
                    if (nc >= 1 && nc <= EMPIRE_GRID_COLS && nr >= 1 && nr <= EMPIRE_GRID_ROWS)
                    {
                        if (ContainsEmpireCell(empire, nc, nr))
                            toCheck.Enqueue(Tuple.Create(nc, nr));
                    }
                }
            }
        }

        return removed;
    }

    private bool ContainsPlayer(List<EmpirePlayer> players, int col, int row)
    {
        return players.Exists(p => p.Col == col && p.Row == row);
    }

    private bool ContainsEmpireCell(List<EmpireCell> empire, int col, int row)
    {
        return empire.Exists(e => e.Col == col && e.Row == row);
    }

    private bool AddEmpireCellIfMissing(List<EmpireCell> empire, int col, int row)
    {
        if (ContainsEmpireCell(empire, col, row))
            return false;

        empire.Add(new EmpireCell { Col = col, Row = row });
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
            CPH.LogWarn($"[Clone Empire Move] Failed to parse player JSON. {ex.Message}");
            return new List<EmpirePlayer>();
        }
    }

    private List<EmpireCell> LoadEmpire()
    {
        string rawJson = CPH.GetGlobalVar<string>(VAR_EMPIRE_CELLS_JSON, false) ?? "[]";
        if (string.IsNullOrWhiteSpace(rawJson))
            rawJson = "[]";

        try
        {
            return DeserializeJson<List<EmpireCell>>(rawJson) ?? new List<EmpireCell>();
        }
        catch (Exception ex)
        {
            CPH.LogWarn($"[Clone Empire Move] Failed to parse empire JSON. {ex.Message}");
            return new List<EmpireCell>();
        }
    }

    private void SavePlayers(List<EmpirePlayer> players)
    {
        CPH.SetGlobalVar(VAR_EMPIRE_PLAYERS_JSON, SerializeJson(players ?? new List<EmpirePlayer>()), false);
    }

    private void SaveEmpire(List<EmpireCell> empire)
    {
        CPH.SetGlobalVar(VAR_EMPIRE_CELLS_JSON, SerializeJson(empire ?? new List<EmpireCell>()), false);
    }

    private void TriggerLoss()
    {
        CPH.DisableTimer(TIMER_GAME_TICK);
        CPH.SetGlobalVar(VAR_EMPIRE_GAME_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_EMPIRE_JOIN_ACTIVE, false, false);
        ReleaseMiniGameLockIfOwned();

        PublishCloneEnd("loss", new List<EmpirePlayer>());

        CPH.SendMessage("☠️ All Rebel fighters have been eliminated. The Empire controls this sector.");
    }

    private void ReleaseMiniGameLockIfOwned()
    {
        string lockName = CPH.GetGlobalVar<string>(VAR_MINIGAME_NAME, false) ?? "";
        if (!string.Equals(lockName, MINIGAME_NAME, StringComparison.OrdinalIgnoreCase))
            return;

        CPH.SetGlobalVar(VAR_MINIGAME_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_MINIGAME_NAME, "", false);
    }

    private void PublishBrokerMessage(string topic, string payloadJson)
    {
        try
        {
            string envelopeJson = "{"
                + "\"id\":" + SerializeJson(Guid.NewGuid().ToString()) + ","
                + "\"topic\":" + SerializeJson(topic ?? "") + ","
                + "\"sender\":" + SerializeJson("streamerbot") + ","
                + "\"timestamp\":" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + ","
                + "\"payload\":" + (payloadJson ?? "null")
                + "}";

            CPH.WebsocketSend(envelopeJson, BROKER_WS_INDEX);
        }
        catch (Exception ex)
        {
            CPH.LogError($"[Clone Empire Move] Failed to publish broker topic '{topic}': {ex}");
        }
    }

    private void PublishCloneUpdate(string eventName, List<EmpirePlayer> players, List<EmpireCell> empire, string detail = "")
    {
        var state = new Dictionary<string, object>
        {
            { "event", eventName ?? "" },
            { "players", players ?? new List<EmpirePlayer>() },
            { "empire", empire ?? new List<EmpireCell>() },
            { "gridCols", EMPIRE_GRID_COLS },
            { "gridRows", EMPIRE_GRID_ROWS },
            { "elapsedSeconds", GetElapsedSeconds() },
            { "survivorCount", players == null ? 0 : players.Count },
            { "empireCount", empire == null ? 0 : empire.Count },
            { "eventDetail", detail ?? "" }
        };

        var payload = new Dictionary<string, object>
        {
            { "game", "clone" },
            { "state", state }
        };

        PublishBrokerMessage(TOPIC_CLONE_UPDATE, SerializeJson(payload));
    }

    private void PublishCloneEnd(string outcome, List<EmpirePlayer> survivors)
    {
        var result = new Dictionary<string, object>
        {
            { "outcome", outcome ?? "" },
            { "survivors", survivors ?? new List<EmpirePlayer>() }
        };

        var payload = new Dictionary<string, object>
        {
            { "game", "clone" },
            { "result", result }
        };

        PublishBrokerMessage(TOPIC_CLONE_END, SerializeJson(payload));
    }

    private long GetElapsedSeconds()
    {
        long startUtc = CPH.GetGlobalVar<long?>(VAR_EMPIRE_GAME_START_UTC, false) ?? 0L;
        if (startUtc <= 0)
            return 0L;

        long elapsedMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startUtc;
        if (elapsedMs < 0)
            elapsedMs = 0;

        return elapsedMs / 1000L;
    }

    [DataContract]
    private class EmpirePlayer
    {
        [DataMember(Name = "userId")]      public string UserId      { get; set; }
        [DataMember(Name = "userName")]    public string UserName    { get; set; }
        [DataMember(Name = "col")]         public int    Col         { get; set; }
        [DataMember(Name = "row")]         public int    Row         { get; set; }
        [DataMember(Name = "lastMoveUtc")] public long   LastMoveUtc { get; set; }
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
