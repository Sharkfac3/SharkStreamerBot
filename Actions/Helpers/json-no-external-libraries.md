---
id: actions-helper-json-no-external-libraries
type: reference
description: No-external-library JSON parse/serialize helper for Streamer.bot inline C# scripts.
owner: streamerbot-dev
secondaryOwners:
  - lotat-tech
  - app-dev
  - ops
status: active
---

## 7) JSON Parse / Serialize Helper (No External Libraries)

Streamer.bot inline C# does not reliably expose `System.Text.Json` or `Newtonsoft.Json` for general use.
Use this hand-rolled parser/serializer when a script needs to read JSON files, store structured data
in global variables as JSON strings, or parse JSON payloads.

Reference implementation: `Actions/LotAT/lotat-commander-input.cs`

### Required using directives

```csharp
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
```

### Top-level helpers — copy these into your class

```csharp
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
```

### Defining data classes with [DataContract]

Use `[DataContract]` and `[DataMember(Name = "json_key")]` to map JSON keys
to C# properties. The parser uses these attributes for both reading and writing.

```csharp
[DataContract]
private class MyData
{
    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "label")]
    public string Label { get; set; }

    [DataMember(Name = "count")]
    public int Count { get; set; }

    [DataMember(Name = "tags")]
    public List<string> Tags { get; set; }
}
```

### Example usage

**Read a JSON file from disk:**
```csharp
string json = System.IO.File.ReadAllText(@"C:\path\to\data.json");
MyData data = DeserializeJson<MyData>(json);
CPH.LogWarn($"[Script] Loaded: id={data.Id}, label={data.Label}");
```

**Store/retrieve structured data in a global variable:**
```csharp
// Write
var scores = new Dictionary<string, int> { { "alice", 10 }, { "bob", 7 } };
CPH.SetGlobalVar("my_scores_json", SerializeJson(scores), false);

// Read
string raw = CPH.GetGlobalVar<string>("my_scores_json", false) ?? "{}";
Dictionary<string, int> loaded = DeserializeJson<Dictionary<string, int>>(raw);
```

**Parse a simple JSON array from a global variable:**
```csharp
string allowedJson = CPH.GetGlobalVar<string>("allowed_commands_json", false) ?? "[]";
List<string> commands = DeserializeJson<List<string>>(allowedJson);
```

### Full parser/serializer internals — copy verbatim

Copy **all** of the methods below into your class. They work together as a set.

```csharp
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
```

### What this supports

| Type | Deserialize | Serialize |
|---|---|---|
| `string`, `int`, `long`, `double`, `bool` | Yes | Yes |
| `List<T>` (any supported element type) | Yes | Yes |
| `Dictionary<string, T>` | Yes | Yes |
| `[DataContract]` classes with `[DataMember]` | Yes | Yes |
| `Nullable<T>` | Yes | — |
| `IEnumerable` (arrays, other collections) | — | Yes |

### Important notes

- Object key lookups are **case-insensitive** (uses `StringComparer.OrdinalIgnoreCase`).
- `[DataMember(Name = "...")]` controls the JSON key name. Without it, the property name is used.
- Numbers without decimals parse as `int` (if they fit) or `long`. Decimals parse as `double`.
- This parser handles standard JSON. It does **not** support comments, trailing commas, or single-quoted strings.
- For outbound HTTP payloads to Mix It Up, continue using `System.Text.Json` per §2 — that dependency is already proven for that path.

## Related references

- [Mix It Up Command API Helper](mixitup-command-api.md) — Mix It Up outbound HTTP still uses `System.Text.Json` for that proven path.
- [Actions/Overlay/AGENTS.md](../Overlay/AGENTS.md)

