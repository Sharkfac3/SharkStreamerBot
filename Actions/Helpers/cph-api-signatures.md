---
id: actions-helper-cph-api-signatures
type: reference
description: Verified Streamer.bot CPH method signatures and policy for unverified API calls.
owner: streamerbot-dev
secondaryOwners:
  - ops
status: active
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
For the canonical args list per trigger, see the catalog at [triggers/](triggers/README.md). Feature READMEs and `.cs` headers describe `## Args Consumed` — the subset each script reads and how — not the full upstream args set.

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

## Related references

- [Timer Management](timers.md)
- [OBS Scene Switching](obs-scenes.md)

