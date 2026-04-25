# Prompt: Hype Train Level Up

You are working in `SharkStreamerBot`. Backfill Mix It Up special identifiers for this Streamer.bot action script.

Target script: `Actions/Twitch Hype Train/hype-train-level-up.cs`
Matching README: `Actions/Twitch Hype Train/README.md`

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

Update `Actions/Twitch Hype Train/hype-train-level-up.cs` so its Mix It Up payload sends populated `SpecialIdentifiers` instead of an empty object.

Recommended identifiers:

- `hypetrainlevel` — `level` arg as string
- `hypetrainpercent` — `percent` arg as string
- `hypetrainpercentdecimal` — `percentDecimal` arg as string
- `hypetraintype` — `trainType` arg
- `hypetraingoldenkappa` — `isGoldenKappaTrain` as `true`/`false`
- `hypetraintreasure` — `isTreasureTrain` as `true`/`false`
- `hypetrainshared` — `isSharedTrain` as `true`/`false`
- `hypetrainstartedat` — `startedAt` arg as string
- `hypetrainid` — `id` arg
- `hypetraintopbitsuser` — `top.bits.user` arg
- `hypetraintopbitsuserid` — `top.bits.userId` arg as string
- `hypetraintopbitstotal` — `top.bits.total` arg as string
- `hypetraintopsubuser` — `top.subscription.user` arg
- `hypetraintopsubuserid` — `top.subscription.userId` arg as string
- `hypetraintopsubtotal` — `top.subscription.total` arg as string
- `hypetraintopotheruser` — `top.other.user` arg
- `hypetraintopotheruserid` — `top.other.userId` arg as string
- `hypetraintopothertotal` — `top.other.total` arg as string
- `hypetrainevent` — `levelup`
- `hypetrainprevlevel` — `prevLevel` arg as string
- `hypetrainexpiresat` — `expiresAt` arg as string
- `hypetrainduration` — `duration` arg as string
- `hypetrainalltimehighlevel` — `allTimeHighLevel` arg as string
- `hypetrainalltimehightotal` — `allTimeHighTotal` arg as string

## Notes

Use the same helper and key names as the start/end scripts so Mix It Up can share command logic.

## Acceptance criteria

- The script still compiles as a Streamer.bot C# inline action.
- Existing `Arguments` behavior is preserved.
- Mix It Up receives the listed identifiers under `SpecialIdentifiers`.
- Missing args produce empty strings, `0`, or `false` strings instead of exceptions.
- `Actions/Twitch Hype Train/README.md` documents the final payload shape and identifier list.
