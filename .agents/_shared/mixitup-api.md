# Mix It Up API — Run Command Convention

## Endpoint

`POST /api/v2/commands/{commandId}`

## Payload Shape

```csharp
var payload = new {
    Platform = "Twitch",
    Arguments = "optional message text",
    SpecialIdentifiers = new {
        myVar = "value",
        anotherVar = "value"
    },
    IgnoreRequirements = false
};
```

## Rules

- Standard fields: `Platform`, `Arguments`, `IgnoreRequirements` — keep these at top level.
- Extra variables for Mix It Up command usage go inside `SpecialIdentifiers`, not as top-level fields.
- `Arguments` carries the primary string payload (e.g., a chat message or display text).
- `IgnoreRequirements = false` unless the operator explicitly asks to bypass cooldowns/conditions.

## Preferred Pattern — Optional User Message + Explicit Type

When forwarding an **optional** user-authored message to Mix It Up, prefer this pattern:

- Send the raw user text as a dedicated special identifier such as `watchstreakmessage`.
- Send a companion type/status identifier such as `watchstreaktype`.
- If the user text is present:
  - keep the message text unchanged
  - set the type to `"message"`
- If the user text is blank or missing:
  - send the message identifier as an empty string
  - set the type to `"none"`
- Do **not** silently replace the empty user message with a fallback/system message unless the operator explicitly asks for that behavior.

Why this pattern is preferred:
- Mix It Up can branch intentionally on the explicit type instead of guessing from empty/non-empty text.
- The original user-authored message is preserved exactly when it exists.
- The no-message path stays clean and predictable for future alert logic.

Example:

```csharp
string userMessage = message ?? "";
string messageType = string.IsNullOrWhiteSpace(userMessage)
    ? "none"
    : "message";

var payload = new {
    Platform = "Twitch",
    Arguments = "",
    SpecialIdentifiers = new {
        watchstreakmessage = userMessage,
        watchstreaktype = messageType
    },
    IgnoreRequirements = false
};
```

## Timing / Wait Behavior

- After calling a Mix It Up command that triggers TTS or an overlay animation, insert a wait proportional to the content length to avoid overlap with subsequent actions.
- Shared wait pattern: see `Actions/HELPER-SNIPPETS.md` § Mix It Up unlock wait.
- Standard unlock buffer: **31 seconds** for Mix It Up commands that trigger animations with queued audio.

## Command Discovery

To fetch available Mix It Up commands: `Tools/MixItUp/Api/get_commands.py`
Output written to: `Tools/MixItUp/Api/data/mixitup-commands.txt`
