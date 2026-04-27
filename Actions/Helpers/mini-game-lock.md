---
id: actions-helper-mini-game-lock
type: reference
description: Reusable Streamer.bot C# mini-game lock constants and acquire/release snippets.
owner: streamerbot-dev
secondaryOwners:
  - ops
status: active
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

## Related references

- [Mini-game contract checklist](mini-game-contract.md)
- [Actions/SHARED-CONSTANTS.md — Mini-game Lock](../SHARED-CONSTANTS.md#mini-game-lock)
- [Actions/Twitch Core Integrations/stream-start.cs](../Twitch%20Core%20Integrations/stream-start.cs)

