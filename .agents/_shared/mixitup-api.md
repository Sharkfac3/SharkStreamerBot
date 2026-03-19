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

## Timing / Wait Behavior

- After calling a Mix It Up command that triggers TTS or an overlay animation, insert a wait proportional to the content length to avoid overlap with subsequent actions.
- Shared wait pattern: see `Actions/HELPER-SNIPPETS.md` § Mix It Up unlock wait.
- Standard unlock buffer: **31 seconds** for Mix It Up commands that trigger animations with queued audio.

## Command Discovery

To fetch available Mix It Up commands: `Tools/MixItUp/Api/get_commands.py`
Output written to: `Tools/MixItUp/Api/data/mixitup-commands.txt`
