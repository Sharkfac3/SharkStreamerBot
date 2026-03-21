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
private const int WAIT_MIXITUP_UNLOCK_STARTUP_MS = 3000;

// On unlock (no extra params): sends SpecialIdentifiers = {}
TriggerMixItUpCommand(MIXITUP_UNLOCK_COMMAND_ID, "Squad Duck");

// With extra params available in Mix It Up as special identifiers:
TriggerMixItUpCommand(
    MIXITUP_UNLOCK_COMMAND_ID,
    "Squad Duck",
    arguments: "hello chat",
    specialIdentifiers: new { test = "True" });
```

### Unlock pacing guidance
When a script triggers a **Mix It Up unlock command** and then waits for the unlock sequence to finish,
include a **3000ms startup buffer** before the effect's own duration.

Why:
- Mix It Up usually needs a short startup window before the visible/audible payoff actually begins.
- Without that buffer, Streamer.bot can release locks or finish an action a little too early.
- This matters most for **Squad unlock flows** (Pedro, Duck, Clone, future mini-games), but the same rule is a good default for any unlock/reveal flow driven by Mix It Up.

Default rule:
- Wait time = `WAIT_MIXITUP_UNLOCK_STARTUP_MS` + effect/animation/TTS duration.
- If you intentionally skip the startup buffer for a specific feature, document the reason in that feature's README.

Example:
```csharp
private const int WAIT_MIXITUP_UNLOCK_STARTUP_MS = 3000;
private const int PEDRO_DANCE_DURATION_MS = 28000;
private const int PEDRO_UNLOCK_WAIT_MS = WAIT_MIXITUP_UNLOCK_STARTUP_MS + PEDRO_DANCE_DURATION_MS;

bool unlockTriggered = TriggerMixItUpCommand(MIXITUP_UNLOCK_COMMAND_ID, "Squad Pedro");
if (unlockTriggered)
    CPH.Wait(PEDRO_UNLOCK_WAIT_MS);
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

## 4) OBS Scene Switching

Use this when a script needs to switch the active OBS scene based on the current stream mode.

**Confirmed correct method** (verified against Streamer.bot API docs):

```csharp
CPH.ObsSetScene(string sceneName, int connection = 0);
```

Do **not** use reflection to look up OBS scene methods. `ObsSetCurrentScene` and
`ObsSetProgramScene` do not exist. Reflection searching for a `(string)` overload
of `ObsSetScene` silently returns null — the real signature is `(string, int)`.

### Example (mode-aware scene switch)

```csharp
private const string VAR_STREAM_MODE = "stream_mode";

private string ResolveTargetScene(string mode)
{
    switch (mode)
    {
        case "garage":    return "Garage: Main";
        case "gamer":     return "Gamer: Main";
        case "workspace": return "Workspace: Main";
        default:
            CPH.LogWarn($"[Script] Unknown stream_mode '{mode}'. Falling back to workspace.");
            return "Workspace: Main";
    }
}

// In Execute():
string mode = (CPH.GetGlobalVar<string>(VAR_STREAM_MODE, false) ?? string.Empty)
    .Trim().ToLowerInvariant();
CPH.ObsSetScene(ResolveTargetScene(mode));
```

---

## 5) Verified CPH API Method Signatures

Confirmed against the official Streamer.bot docs (https://docs.streamer.bot/api/csharp/methods).
Use these exact calls — do not substitute reflection-based fallbacks or guessed method names.

### OBS Studio

```csharp
CPH.ObsSetScene(string sceneName, int connection = 0);
CPH.ObsShowSource(string scene, string source, int connection = 0);
CPH.ObsHideSource(string scene, string source, int connection = 0);
```

> **Do not use reflection for OBS.** `ObsSetCurrentScene` and `ObsSetProgramScene` do not exist.
> Reflection searching for a `(string)` overload of `ObsSetScene` silently fails — the real signature is `(string, int)`.

### Chat

```csharp
CPH.SendMessage(string message, bool useBot = true, bool fallback = true);
```

### Global Variables

```csharp
CPH.GetGlobalVar<T>(string varName, bool persisted = true);
CPH.SetGlobalVar(string varName, object value, bool persisted = true);
CPH.UnsetGlobalVar(string varName, bool persisted = true);
```

### Timers

```csharp
CPH.EnableTimer(string timerName);
CPH.DisableTimer(string timerName);
CPH.SetTimerInterval(string timerName, int seconds); // VERIFY: unconfirmed method signature
```

See **§6 Timer Management** for full patterns including countdown reset and interval update.

### Trigger Arguments

```csharp
// Read a trigger argument into an out variable. Returns false if the key is missing.
bool CPH.TryGetArg<T>(string varName, out T value);
```

Common keys: `user`, `userId`, `message`, `rawInput`, `msgId`, `input0`, `bits`, `rewardName`, `rewardId`.
See each feature README's "Trigger Variables" section for the full list per event type.

### Misc

```csharp
CPH.Wait(int milliseconds);
CPH.LogWarn(string logLine);
CPH.LogError(string logLine);
```

### Using a CPH method not listed here

If you need a CPH method that isn't in this list:
1. Mark it with `// VERIFY: unconfirmed method signature` in a comment above the call.
2. Flag it in your change summary under Manual Setup Steps so the operator can test it before relying on it.
3. Do **not** guess at method names or use reflection to find them.

---

## 6) Timer Management

Use these patterns when a script needs to control Streamer.bot timers beyond basic enable/disable.

### Start a timer (enable)

```csharp
CPH.EnableTimer(string timerName);
```

Starts the timer if it is not already running. The countdown begins from the timer's configured interval.

### Stop a timer (disable)

```csharp
CPH.DisableTimer(string timerName);
```

Stops the timer immediately. The current countdown position is discarded.

### Reset a timer's countdown (restart from full interval)

Disable then immediately re-enable. This restarts the countdown from the full configured interval without changing the interval value.

```csharp
private void ResetTimer(string timerName)
{
    CPH.DisableTimer(timerName);
    CPH.EnableTimer(timerName);
}
```

Use this when you want to "push back" a timer deadline — for example, extending a call window each time the player does something valid.

Example:
```csharp
// Player submitted a valid action; restart the call window so they get the full window again.
ResetTimer(TIMER_DUCK_CALL_WINDOW);
```

### Update a timer's interval (change how long it runs)

Disable the timer, set the new interval, then re-enable it. The timer restarts from the new interval value.

```csharp
// VERIFY: unconfirmed method signature — test before relying on this in production.
private void SetTimerInterval(string timerName, int seconds)
{
    CPH.DisableTimer(timerName);
    CPH.SetTimerInterval(timerName, seconds); // VERIFY: unconfirmed method signature
    CPH.EnableTimer(timerName);
}
```

> **Operator note:** `CPH.SetTimerInterval` is not yet confirmed against Streamer.bot docs.
> Verify this method exists and accepts `(string, int)` before shipping any script that uses it.
> If it does not exist, the interval must be changed manually in the Streamer.bot UI and this
> method call removed.

### Safe enable/disable helpers with logging

Use these when you want a consistent log trail for timer state changes:

```csharp
private void StartTimer(string timerName, string logPrefix)
{
    CPH.LogWarn($"[{logPrefix}] Starting timer: {timerName}");
    CPH.EnableTimer(timerName);
}

private void StopTimer(string timerName, string logPrefix)
{
    CPH.LogWarn($"[{logPrefix}] Stopping timer: {timerName}");
    CPH.DisableTimer(timerName);
}
```

### Operator notes

- `CPH.EnableTimer` / `CPH.DisableTimer` are verified. The disable→enable trick for resetting the countdown is safe and relies only on those two verified calls.
- Timer names must match entries in `Actions/SHARED-CONSTANTS.md` exactly (case-sensitive).
- All timers should be disabled in `stream-start.cs` at stream start to clear stale state from previous sessions.

---

## Required mini-game contract checklist
1. Add lock constants + acquire/release methods.
2. Acquire lock at the mini-game start action.
3. Release lock on every terminal path (win/loss/timeout/cancel/manual stop/guard exits).
4. Add chat feedback when blocked by another active mini-game.
5. For single-action mini-games, use `finally` to guarantee lock release.
6. Add/update docs in feature README and `Actions/SHARED-CONSTANTS.md` when needed.
