# Prompt: Message Effects Automatic Reward

You are working in `SharkStreamerBot`. Backfill Mix It Up special identifiers for this Streamer.bot action script.

Target script: `Actions/Twitch Bits Integrations/message-effects.cs`
Matching README: `Actions/Twitch Bits Integrations/README.md`

## Shared requirements for the agent

Before editing:
1. Read `AGENTS.md`, `.agents/ENTRY.md`, `WORKING.md`, and load the `streamerbot-dev` skill.
2. Load any feature-specific skill matching the script path.
3. Check for active file conflicts in `WORKING.md`.

Implementation rules:
- Preserve existing behavior unless the prompt explicitly says to change it.
- Keep `Arguments` compatible with the current Mix It Up command. If it already sends useful text, keep it.
- Add populated `SpecialIdentifiers` with lowercase, no-space keys.
- Send values as strings where practical so Mix It Up can consume them consistently.
- Read Streamer.bot args defensively with helper methods (`GetStringArg`, `GetIntArg`, `GetBoolArg`) rather than assuming values exist.
- Do not expose secrets. Do not add chat output unless requested.
- Update the matching `Actions/**/README.md` section with the final identifier list.
- After code changes, load `ops-change-summary` and include paste target + validation checklist.


## Task

Update `Actions/Twitch Bits Integrations/message-effects.cs` so its Mix It Up payload sends populated `SpecialIdentifiers` instead of an empty object.

Recommended identifiers:

- `messageeffectsuser` — `user` arg
- `messageeffectsuserid` — `userId` arg
- `messageeffectsmessage` — final user text sent to `Arguments`
- `messageeffectstype` — `message` when text is non-empty, otherwise `none`
- `messageeffectssourcearg` — the arg name used (`userInput`, `input0`, `message`, `rawInput`, or empty)
- `messageeffectswordcount` — final message word count as string

## Notes

Keep the existing user-input fallback order. This action currently has a placeholder Mix It Up command ID; do not invent an ID unless the operator provides one.

## Acceptance criteria

- The script still compiles as a Streamer.bot C# inline action.
- Existing `Arguments` behavior is preserved.
- Mix It Up receives the listed identifiers under `SpecialIdentifiers`.
- Missing args produce empty strings, `0`, or `false` strings instead of exceptions.
- `Actions/Twitch Bits Integrations/README.md` documents the final payload shape and identifier list.
