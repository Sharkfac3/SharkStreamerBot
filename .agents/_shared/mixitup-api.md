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
- Any new or modified Streamer.bot action that calls Mix It Up should build populated `SpecialIdentifiers` for useful event metadata and branching values.
- Use lowercase, no-space special identifier keys.
- Send special identifier values as strings where practical for consistent Mix It Up consumption.
- Empty `SpecialIdentifiers = new { }` is acceptable only when the event truly has no useful metadata or the field contract is intentionally unresolved; document that in the matching `Actions/**/README.md`.

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

---

## Custom Intro Command

**Command name in Mix It Up:** `Custom Intro`

**Triggered by:** Streamer.bot action `Intros - Play Custom Intro`

**How the argument arrives:**
Streamer.bot sets global var `intro_sound_file_path` (non-persisted) immediately before calling
`CPH.RunAction("Intros - Play Custom Intro", true)`. The sub-action chain must read this global var
and pass it as the argument to the Mix It Up command.

**Argument:** Full absolute path to the sound file.
Example: `C:\Users\sharkfac3\Workspace\coding\SharkStreamerBot\Assets\user-intros\sound\alice.mp3`

**Mix It Up command setup (operator checklist):**
1. Create a command named exactly `Custom Intro` in Mix It Up.
2. Add a "Play Sound" action that reads the path from `$specialidentifier[intro_sound_file_path]`
   (or the equivalent Streamer.bot → Mix It Up variable passthrough mechanism).
3. No cooldowns required — SB "First Chat" trigger fires at most once per viewer per stream session.
4. `IgnoreRequirements = false` (default) is fine.

**Streamer.bot sub-action chain setup (operator checklist):**
1. Create action named `Intros - Play Custom Intro` in Streamer.bot.
2. Add sub-action: Read global var `intro_sound_file_path` → store as local arg.
3. Add sub-action: Call Mix It Up command `Custom Intro`, passing the path as argument.

**Notes:**
- Path constructed in `first-chat-intro.cs` from `ASSETS_ROOT + SOUND_SUBPATH + soundFile`.
- Operator must ensure the sound file exists at the constructed path before enabling an intro in production-manager.
- No overlay work — audio playback fully owned by Mix It Up.
