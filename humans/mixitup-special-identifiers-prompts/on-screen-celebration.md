# Prompt: On-Screen Celebration Automatic Reward

You are working in `SharkStreamerBot`. Backfill Mix It Up special identifiers for this Streamer.bot action script.

Target script: `Actions/Twitch Bits Integrations/on-screen-celebration.cs`
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

Update `Actions/Twitch Bits Integrations/on-screen-celebration.cs` so its Mix It Up payload sends populated `SpecialIdentifiers` instead of an empty object.

Recommended identifiers:

- `celebrationuser` — `user` arg
- `celebrationuserid` — `userId` arg
- `celebrationtype` — `onscreencelebration`
- `celebrationrewardid` — reward/rewardId arg if available, otherwise empty
- `celebrationrewardname` — rewardName/rewardTitle arg if available, otherwise empty
- `celebrationmessage` — optional userInput/input0/message/rawInput if available, otherwise empty
- `celebrationmessagetype` — `message` when celebration message is non-empty, otherwise `none`

## Notes

This is one of the true stub-like scripts. Add small helper methods for safe arg reads. Keep `Arguments = ""` unless the operator asks to route the message there.

## Acceptance criteria

- The script still compiles as a Streamer.bot C# inline action.
- Existing `Arguments` behavior is preserved.
- Mix It Up receives the listed identifiers under `SpecialIdentifiers`.
- Missing args produce empty strings, `0`, or `false` strings instead of exceptions.
- `Actions/Twitch Bits Integrations/README.md` documents the final payload shape and identifier list.
