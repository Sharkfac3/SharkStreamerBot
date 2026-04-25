# Prompt: Explain Current Task Channel Point Redeem

You are working in `SharkStreamerBot`. Backfill Mix It Up special identifiers for this Streamer.bot action script.

Target script: `Actions/Twitch Channel Points/explain-current-task.cs`
Matching README: `Actions/Twitch Channel Points/README.md`

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

Update `Actions/Twitch Channel Points/explain-current-task.cs` so its Mix It Up payload sends populated `SpecialIdentifiers` instead of an empty object.

Recommended identifiers:

- `explaintaskuser` — `user` arg
- `explaintaskuserid` — `userId` arg
- `explaintasktype` — `explaincurrenttask`
- `explaintaskrewardid` — reward/rewardId arg if available, otherwise empty
- `explaintaskrewardname` — rewardName/rewardTitle arg if available, otherwise empty
- `explaintaskmessage` — optional userInput/input0/message/rawInput if available, otherwise empty
- `explaintaskmessagetype` — `message` when viewer text exists, otherwise `none`
- `explaintaskrecordingcheck` — `attempted` after the script runs its recording guard

## Notes

Preserve the OBS recording guard exactly unless you find a bug. The goal is only to enrich the Mix It Up payload.

## Acceptance criteria

- The script still compiles as a Streamer.bot C# inline action.
- Existing `Arguments` behavior is preserved.
- Mix It Up receives the listed identifiers under `SpecialIdentifiers`.
- Missing args produce empty strings, `0`, or `false` strings instead of exceptions.
- `Actions/Twitch Channel Points/README.md` documents the final payload shape and identifier list.
