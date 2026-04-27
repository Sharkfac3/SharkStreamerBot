---
id: tools-mix-it-up
type: domain-route
description: Mix It Up API helpers, command support files, overlays, and payload conventions for stream integrations.
owner: ops
secondaryOwners:
  - streamerbot-dev
  - app-dev
workflows:
  - change-summary
  - validation
status: active
---

# Tools/MixItUp — Agent Guide

## Purpose

[Tools/MixItUp/](./) contains local Mix It Up support tooling: API helpers, command-support files, overlay sources, shared support files, and exported/saved command data used by tooling.

This guide also owns the Mix It Up API payload convention that used to live in the shared agent tree. Keep Mix It Up operational knowledge here so Streamer.bot actions, app integrations, and local command tooling share one local source.

## When to Activate

Use this guide when working on:

- Mix It Up API scripts under [Tools/MixItUp/Api/](./Api/)
- command support files under [Tools/MixItUp/Commands/](./Commands/)
- Mix It Up overlay source under [Tools/MixItUp/Overlays/](./Overlays/)
- reusable support files under [Tools/MixItUp/Shared/](./Shared/)
- saved command/export data used by local tooling
- payload conventions for Streamer.bot actions that call Mix It Up commands

Do not activate this guide for Streamer.bot runtime C# scripts themselves, general Streamer.bot validators, or creative prompt/lore files.

## Primary Owner

Primary owner: `ops`.

`ops` owns local tooling layout, command discovery scripts, validation handoffs, and the shared Mix It Up API payload convention.

## Secondary Owners / Chain To

| Role | Chain when |
|---|---|
| `streamerbot-dev` | A Streamer.bot action builds or changes a Mix It Up command payload. |
| `app-dev` | A Mix It Up overlay or API helper needs app-side integration. |
| `brand-steward` | Public-facing command text, overlay copy, or alert language changes. |
| `content-repurposer` | Mix It Up output is being designed for short-form capture or repurposing. |

## Required Reading

Read these first for Mix It Up tooling work:

1. [Tools/MixItUp/README.md](./README.md) — folder purpose and routing examples.
2. [Tools/MixItUp/Api/README.md](./Api/README.md) — API helper area.
3. [Tools/MixItUp/Commands/README.md](./Commands/README.md) — command support area.
4. [Tools/MixItUp/Overlays/README.md](./Overlays/README.md) — overlay source area.
5. [Tools/MixItUp/Shared/README.md](./Shared/README.md) — reusable support area.
6. [Tools/MixItUp/Api/get_commands.py](./Api/get_commands.py) — command discovery helper.
7. [Actions/HELPER-SNIPPETS.md](../../Actions/HELPER-SNIPPETS.md) — shared wait pattern for Mix It Up unlock timing.

## Local Workflow

1. Confirm whether the task belongs in Mix It Up tooling, Streamer.bot runtime scripts, app overlays, or brand copy.
2. For API helper work, keep scripts under [Tools/MixItUp/Api/](./Api/).
3. For command source/support files, use [Tools/MixItUp/Commands/](./Commands/).
4. For overlay HTML, CSS, or JavaScript used by Mix It Up, use [Tools/MixItUp/Overlays/](./Overlays/).
5. For reusable support files, use [Tools/MixItUp/Shared/](./Shared/).
6. Keep command export or discovery data in the relevant tooling data folder and do not treat it as Streamer.bot runtime source.
7. If a Streamer.bot action changes a Mix It Up payload, verify the action README or local agent doc records the expected special identifiers.
8. Finish with validation output and a terminal change summary.

## Validation

Fetch available Mix It Up commands with:

```bash
python3 Tools/MixItUp/Api/get_commands.py
```

The command discovery output is written by the helper under the API tooling data area.

For syntax-level validation after Python script changes, run targeted compilation, for example:

```bash
python3 -m py_compile Tools/MixItUp/Api/*.py
```

For agent-doc changes, follow [.agents/workflows/validation.md](../../.agents/workflows/validation.md) and run the agent-tree validator with the task-requested report path.

## Boundaries / Out of Scope

Do not use this folder to:

- store Streamer.bot runtime C# scripts from [Actions/](../../Actions/)
- store general Streamer.bot sync or validation helpers that are not Mix It Up-specific
- store art prompts, lore, worldbuilding, or marketing strategy source
- silently bypass Mix It Up command requirements or cooldowns unless the operator explicitly requests it
- invent top-level payload fields for command variables when `SpecialIdentifiers` should be used

## Handoff Notes

After changes, report:

- changed files in [Tools/MixItUp/](./)
- commands or setup steps the operator must perform inside Mix It Up
- API helper commands run and output locations
- changed Streamer.bot payload conventions or special identifier names
- whether any related action docs need updates under [Actions/](../../Actions/)
- validation output

## Runtime Notes

### API endpoint

Run a Mix It Up command through:

```text
POST /api/v2/commands/{commandId}
```

### Payload convention

Use this C# payload shape from Streamer.bot-side callers:

```csharp
var payload = new {
    Platform = "Twitch",
    Arguments = "optional message text",
    SpecialIdentifiers = new {
        myvar = "value",
        anothervar = "value"
    },
    IgnoreRequirements = false
};
```

Payload rules:

- Keep standard fields at top level: `Platform`, `Arguments`, and `IgnoreRequirements`.
- Put extra command variables inside `SpecialIdentifiers`, not as top-level fields.
- Use `Arguments` for the primary string payload, such as display text or a chat message.
- Keep `IgnoreRequirements = false` unless the operator explicitly asks to bypass cooldowns or conditions.
- Populate useful metadata and branching values for new or modified Streamer.bot actions that call Mix It Up.
- Use lowercase, no-space special identifier keys.
- Send special identifier values as strings where practical.
- Empty `SpecialIdentifiers` is acceptable only when the event truly has no useful metadata or the field contract is intentionally unresolved; document that in the matching action README or local agent guide.

### Optional user message pattern

When forwarding an optional user-authored message:

- Send raw user text in a dedicated special identifier.
- Send a companion type/status identifier.
- If user text exists, preserve it unchanged and set the type to `message`.
- If user text is blank or missing, send an empty message identifier and set the type to `none`.
- Do not silently replace an empty user message with fallback/system text unless explicitly requested.

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

### Timing and wait behavior

After calling a Mix It Up command that triggers TTS or an overlay animation, insert a wait proportional to content length to avoid overlap with subsequent actions. Use the Mix It Up unlock wait pattern in [Actions/HELPER-SNIPPETS.md](../../Actions/HELPER-SNIPPETS.md).

Standard unlock buffer for commands that trigger animations with queued audio: 31 seconds.

### Custom Intro command

Mix It Up command name: `Custom Intro`.

Triggered by Streamer.bot action: `Intros - Play Custom Intro`.

Argument contract:

- Streamer.bot sets global variable `intro_sound_file_path` immediately before calling the intro playback action.
- The sub-action chain reads that global variable and passes the full absolute sound-file path to the Mix It Up command.
- Mix It Up plays the sound file through a Play Sound action that reads the passed special identifier or equivalent passthrough value.
- No overlay work is involved; audio playback is owned by Mix It Up.
- `IgnoreRequirements = false` is the default.

Operator checklist:

1. Create a Mix It Up command named exactly `Custom Intro`.
2. Add a Play Sound action that reads the passed intro sound path.
3. Ensure the audio file exists before enabling an intro in production-manager.
4. Keep cooldowns disabled or harmless; first-chat behavior should fire at most once per viewer per stream session.

## Known Gotchas

- Mix It Up payload variables belong in `SpecialIdentifiers` unless they are one of the standard top-level fields.
- Optional user messages should not be overwritten by fallback text without explicit operator approval.
- Command discovery data can drift; fetch commands before relying on command IDs.
- Mix It Up-specific overlay source belongs here, not under general Streamer.bot tooling.
