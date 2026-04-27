---
id: actions-helper-obs-scenes
type: reference
description: Streamer.bot C# OBS scene switching helper and direct-call guidance.
owner: streamerbot-dev
secondaryOwners:
  - ops
status: active
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

## Related references

- [Verified CPH API Method Signatures](cph-api-signatures.md)
- [Actions/SHARED-CONSTANTS.md — OBS](../SHARED-CONSTANTS.md#obs)
- [Actions/SHARED-CONSTANTS.md — Stream Mode](../SHARED-CONSTANTS.md#stream-mode)

