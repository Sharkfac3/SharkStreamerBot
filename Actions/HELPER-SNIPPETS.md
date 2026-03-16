# Helper Snippets (Copy/Paste)

Purpose: reusable Streamer.bot C# snippet patterns for common tasks.

These are intentionally small and beginner-friendly so you can copy/paste into any feature script (Squad or non-Squad).

Reference implementation in repo:
- `Actions/Squad/Pedro/pedro-main.cs`

---

## 1) Mini-game Lock Helper (Global, cross-feature)

Use this when a script starts a mini-game so only one mini-game can run at a time.

### Constants
```csharp
// Shared mini-game lock globals.
private const string VAR_MINIGAME_ACTIVE = "minigame_active";
private const string VAR_MINIGAME_NAME = "minigame_name";

// Set this per feature/script.
private const string MINIGAME_NAME = "replace_with_game_name";
```

### Acquire lock (start path)
```csharp
private bool TryAcquireMiniGameLock()
{
    bool lockActive = (CPH.GetGlobalVar<bool?>(VAR_MINIGAME_ACTIVE, false) ?? false);
    string lockName = CPH.GetGlobalVar<string>(VAR_MINIGAME_NAME, false) ?? "";

    // Block when another game owns the lock.
    if (lockActive && !string.Equals(lockName, MINIGAME_NAME, StringComparison.OrdinalIgnoreCase))
        return false;

    // Claim lock for this mini-game.
    CPH.SetGlobalVar(VAR_MINIGAME_ACTIVE, true, false);
    CPH.SetGlobalVar(VAR_MINIGAME_NAME, MINIGAME_NAME, false);
    return true;
}
```

### Release lock (end path)
```csharp
private void ReleaseMiniGameLockIfOwned()
{
    string lockName = CPH.GetGlobalVar<string>(VAR_MINIGAME_NAME, false) ?? "";
    if (!string.Equals(lockName, MINIGAME_NAME, StringComparison.OrdinalIgnoreCase))
        return;

    CPH.SetGlobalVar(VAR_MINIGAME_ACTIVE, false, false);
    CPH.SetGlobalVar(VAR_MINIGAME_NAME, "", false);
}
```

### Example usage
```csharp
if (!TryAcquireMiniGameLock())
{
    string activeGame = CPH.GetGlobalVar<string>(VAR_MINIGAME_NAME, false) ?? "another mini-game";
    CPH.SendMessage($"🎮 A mini-game is already running ({activeGame}).");
    return true;
}

try
{
    // Your mini-game logic...
}
finally
{
    // Only for single-action mini-games.
    // For multi-step/timer mini-games, release on terminal resolve/win/loss paths instead.
    ReleaseMiniGameLockIfOwned();
}
```

Operator note:
- `Actions/Twitch Core Integrations/stream-start.cs` should reset these globals at stream start.

---

## 2) Mix It Up Command API Helper

Use this to avoid repeating the same HTTP request code in every script.

### Constants
```csharp
private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();
```

### Reusable method
```csharp
private bool TriggerMixItUpCommand(
    string commandId,
    string logPrefix,
    string arguments = "",
    object specialIdentifiers = null)
{
    if (string.IsNullOrWhiteSpace(commandId) ||
        commandId.StartsWith("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase))
    {
        CPH.LogWarn($"[{logPrefix}] Mix It Up command ID is not configured.");
        return false;
    }

    try
    {
        string url = $"{MIXITUP_API_BASE_URL.TrimEnd('/')}/api/v2/commands/{commandId}";
        string payload = JsonSerializer.Serialize(new
        {
            Platform = "Twitch",
            Arguments = arguments ?? "",
            // Keep SpecialIdentifiers present by default.
            // If no extra values are provided, send an empty object.
            SpecialIdentifiers = specialIdentifiers ?? new { },
            IgnoreRequirements = false
        });

        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage response = MIXITUP_HTTP_CLIENT.PostAsync(url, content).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            CPH.LogWarn($"[{logPrefix}] Mix It Up call failed: {(int)response.StatusCode} {response.ReasonPhrase}");
            return false;
        }

        return true;
    }
    catch (Exception ex)
    {
        CPH.LogError($"[{logPrefix}] Exception while calling Mix It Up: {ex}");
        return false;
    }
}
```

### Example usage
```csharp
private const string MIXITUP_UNLOCK_COMMAND_ID = "REPLACE_WITH_UNLOCK_COMMAND_ID";

// On unlock (no extra params): sends SpecialIdentifiers = {}
TriggerMixItUpCommand(MIXITUP_UNLOCK_COMMAND_ID, "Squad Duck");

// With extra params available in Mix It Up as special identifiers:
TriggerMixItUpCommand(
    MIXITUP_UNLOCK_COMMAND_ID,
    "Squad Duck",
    arguments: "hello chat",
    specialIdentifiers: new { test = "True" });
```

---

## 3) Chat Message Input Helper (message/rawInput)

Use this when parsing user chat text so scripts work across different trigger payloads.

```csharp
private string GetMessageText()
{
    string text = "";
    if (!CPH.TryGetArg("message", out text) || string.IsNullOrWhiteSpace(text))
        CPH.TryGetArg("rawInput", out text);

    return text ?? "";
}
```

Optional duplicate guard using message ID:
```csharp
private const string VAR_LAST_MESSAGE_ID = "replace_last_message_id_var";

private bool IsDuplicateMessage()
{
    string msgId = "";
    if (!CPH.TryGetArg("msgId", out msgId) || string.IsNullOrWhiteSpace(msgId))
        return false;

    string lastId = CPH.GetGlobalVar<string>(VAR_LAST_MESSAGE_ID, false) ?? "";
    if (string.Equals(lastId, msgId, StringComparison.OrdinalIgnoreCase))
        return true;

    CPH.SetGlobalVar(VAR_LAST_MESSAGE_ID, msgId, false);
    return false;
}
```

Optional sender helper:
```csharp
private (string User, string UserId) GetSender()
{
    string user = "";
    string userId = "";

    CPH.TryGetArg("user", out user);
    CPH.TryGetArg("userId", out userId);

    user = user ?? "";
    userId = string.IsNullOrWhiteSpace(userId) ? user.ToLowerInvariant() : userId;

    return (user, userId);
}
```

---

## Required mini-game contract checklist
1. Add lock constants + acquire/release methods.
2. Acquire lock at the mini-game start action.
3. Release lock on every terminal path (win/loss/timeout/cancel/manual stop/guard exits).
4. Add chat feedback when blocked by another active mini-game.
5. For single-action mini-games, use `finally` to guarantee lock release.
6. Add/update docs in feature README and `Actions/SHARED-CONSTANTS.md` when needed.
