---
id: actions-helper-mixitup-command-api
type: reference
description: Streamer.bot C# helper for calling Mix It Up command API and pacing unlock effects.
owner: streamerbot-dev
secondaryOwners:
  - ops
status: active
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
            // Prefer passing useful event metadata for Mix It Up branching/logging.
            // Send an empty object only when there is truly no useful metadata yet.
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

// Prefer including useful metadata as lowercase/no-space special identifiers.
TriggerMixItUpCommand(
    MIXITUP_UNLOCK_COMMAND_ID,
    "Squad Duck",
    arguments: "hello chat",
    specialIdentifiers: new
    {
        squaduser = "viewername",
        squadtype = "duck",
        squadmessage = "hello chat"
    });

// If an event truly has no useful metadata yet, omit specialIdentifiers.
// The helper will still send SpecialIdentifiers = {} so the payload shape stays stable.
TriggerMixItUpCommand(MIXITUP_UNLOCK_COMMAND_ID, "Squad Duck");
```

### Optional user-message pattern
When a trigger includes **optional user-authored text** and Mix It Up needs to know both
"what the user wrote" and "whether they wrote anything at all," send two special identifiers:

- the message text itself (example: `watchstreakmessage`)
- a companion type/status field (example: `watchstreaktype`)

Preferred contract:
- message exists → keep the text unchanged and set type to `message`
- message missing/blank → send empty string and set type to `none`

Do **not** silently swap in `systemMessage` or another fallback string unless the operator
explicitly asks for that behavior.

Example:
```csharp
string userMessage = message ?? "";
string messageType = string.IsNullOrWhiteSpace(userMessage)
    ? "none"
    : "message";

TriggerMixItUpCommand(
    MIXITUP_UNLOCK_COMMAND_ID,
    "Core Watch Streak",
    arguments: "",
    specialIdentifiers: new
    {
        watchstreakmessage = userMessage,
        watchstreaktype = messageType
    });
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

## Related references

- [Tools/MixItUp/AGENTS.md](../../Tools/MixItUp/AGENTS.md) — payload/tooling authority for Mix It Up commands and discovery.
- [Actions/SHARED-CONSTANTS.md — Bits](../SHARED-CONSTANTS.md#bits)
- [Actions/SHARED-CONSTANTS.md — Mix It Up Unlock Pacing](../SHARED-CONSTANTS.md#mix-it-up-unlock-pacing)

