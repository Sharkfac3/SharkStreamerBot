You are working in the `SharkStreamerBot` repo. Fresh session — no memory of prior conversations.

## Read first

1. `CLAUDE.md`
2. `.agents/ENTRY.md`
3. `.agents/roles/streamerbot-dev/role.md` — C# script conventions
4. `humans/info-service/COORDINATION.md`
5. `Actions/SHARED-CONSTANTS.md` — §Info Service / Assets section (`INFO_SERVICE_URL`, `ASSETS_ROOT`, `ASSETS_USER_INTROS_SOUND_SUBPATH`)
6. An existing SB script for C# conventions — e.g. `Actions/Squad/Duck/duck-main.cs`

Role: `streamerbot-dev`.

## Prereqs

Chunks merged: C1–C5. Verify by checking that `Apps/info-service/src/routes/read.ts` exists.

## WORKING.md

Add row at start. Domain: `Actions/`. Files: `Actions/Intros/first-chat-intro.cs`. Remove + log at finish.

## Task

Implement `Actions/Intros/first-chat-intro.cs` — Streamer.bot C# inline script.

**Trigger:** Streamer.bot native "First Chat" event (fires once per viewer per stream session).

### Behavior

1. Read `INFO_SERVICE_URL` from SHARED-CONSTANTS (value: `http://127.0.0.1:8766`).
2. Read `ASSETS_ROOT` from SHARED-CONSTANTS (value: `C:\Users\sharkfac3\Workspace\coding\SharkStreamerBot\Assets`).
3. Read `ASSETS_USER_INTROS_SOUND_SUBPATH` from SHARED-CONSTANTS (value: `user-intros/sound/`).
4. Build request URL: `{INFO_SERVICE_URL}/info/user-intros/{userId}` where `userId` is the Twitch viewer's userId from the SB trigger args.
5. HTTP GET to that URL.
6. If response is `404` or any non-200: **silent no-op** — log to SB action log, return `true`.
7. If response is `200`: parse JSON. Check `enabled` field.
   - If `enabled === false`: silent no-op, log to SB action log, return `true`.
   - If `enabled === true` and `soundFile` field is present and non-empty:
     - Construct full asset path: `{ASSETS_ROOT}\{ASSETS_USER_INTROS_SOUND_SUBPATH}{soundFile}`
     - Log constructed path to SB action log (for operator debugging).
     - Dispatch MixItUp Custom Intro command with `soundFile` as argument (see notes below).
     - Return `true`.
   - If `enabled === true` but `soundFile` is empty/null: log + no-op, return `true`.

### MixItUp integration

Use `CPH.RunAction` to call a Streamer.bot sub-action chain that triggers the MixItUp Custom Intro command, passing the full asset path as the argument. Alternatively, if SB has direct MixItUp API integration available, use that. Check `.agents/_shared/mixitup-api.md` for the MixItUp API payload spec. The command name in MixItUp will be `Custom Intro`. The argument passed is the full absolute path to the sound file.

**If MixItUp direct API is not available in scope:** use `CPH.RunAction("Intros - Play Custom Intro", true)` and set `soundFilePath` as a global var before calling. Document this pattern clearly in the script header.

### Constants to define in script header

```csharp
private const string INFO_SERVICE_URL     = "http://127.0.0.1:8766";
private const string ASSETS_ROOT          = @"C:\Users\sharkfac3\Workspace\coding\SharkStreamerBot\Assets";
private const string SOUND_SUBPATH        = @"user-intros\sound\";
private const string COLLECTION_NAME      = "user-intros";
private const string MIXITUP_ACTION_NAME  = "Intros - Play Custom Intro";
private const string VAR_SOUND_FILE_PATH  = "intro_sound_file_path";
```

### Logging

Log every branch to `CPH.LogInfo(...)` so the operator can trace execution in SB's action log. Include: userId, URL called, response code, enabled field value, constructed path (when reached).

### Error handling

- Wrap HTTP call in try/catch — network failure = log + return `true` (silent no-op).
- JSON parse failure = log + return `true`.
- Never throw — SB actions must always return `true` or `false`, never crash.

## Deliverables

- `Actions/Intros/first-chat-intro.cs` (new file, new `Actions/Intros/` folder)
- `Actions/SHARED-CONSTANTS.md` — add `first-chat-intro.cs` to the "Used in" list under §Info Service / Assets
- `COORDINATION.md` — C8 status → `merged`, run log row appended (Commit: `uncommitted`)
- `WORKING.md` — active row removed, completed row added

## Forbidden in this chunk

- Any changes to `Apps/` TypeScript code
- Defining the MixItUp Custom Intro command itself — that is operator config, not script work
- `pending-intros` collection — C10

## Finish

1. Load `ops-change-summary` from `.agents/roles/ops/skills/change-summary/_index.md`, show output.
2. Check COORDINATION.md for next draftable chunks (C9 prereq = C8, now satisfied).
   - C9 (`MixItUp Custom Intro command spec`): prereq C8 now merged. Draft `17-mixitup-custom-intro.md`.
   - C9 task: document the MixItUp Custom Intro command spec in `.agents/_shared/mixitup-api.md`. Command name: `Custom Intro`. Argument: full absolute path to sound file. No C# work — this chunk is operator-facing documentation + shared context update only.
   - Update COORDINATION.md: C9 Prompt File → `17-mixitup-custom-intro.md`, status → `prompt-ready`.
3. **Do NOT run `git commit` or `git add`. Operator commits manually.**

## Definition of done

- `Actions/Intros/first-chat-intro.cs` exists
- Script: reads userId from SB trigger args, GETs `/info/user-intros/{userId}`, handles 404/non-200 as no-op, checks `enabled`, checks `soundFile`, constructs full path, dispatches MixItUp action
- All branches log to SB action log
- HTTP + parse errors caught, return `true` always
- `SHARED-CONSTANTS.md` §Info Service / Assets "Used in" updated
- `COORDINATION.md` C8 status = `merged`, run log row appended
- No git commit made by agent
