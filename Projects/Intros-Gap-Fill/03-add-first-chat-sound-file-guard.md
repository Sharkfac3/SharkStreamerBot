# Prompt 03 — Add missing sound-file guard to first-chat intro playback

You are working in the SharkStreamerBot repository as `streamerbot-dev`.

## Task

Update `Actions/Intros/first-chat-intro.cs` so it safely no-ops when the configured intro sound file does not exist on disk.

Known gap:

- `Actions/Intros/AGENTS.md` says missing sound files should no-op.
- Current code builds `fullPath`, logs it, sets `intro_sound_file_path`, and calls `Intros - Play Custom Intro` without checking file existence.

## Required reading before editing

1. `AGENTS.md`
2. `.agents/ENTRY.md`
3. `WORKING.md`
4. `.agents/roles/streamerbot-dev/role.md`
5. `Actions/AGENTS.md`
6. `Actions/Intros/AGENTS.md`
7. `Actions/SHARED-CONSTANTS.md` section `Info Service / Assets`
8. `Apps/info-service/INFO-SERVICE-PLAN.md` section `user-intros`

## Constraints

- Keep the change tiny and boring.
- Do not change `ASSETS_ROOT`, collection names, Mix It Up action name, or global var name.
- Do not implement direct Mix It Up API calls.
- Do not add broad filename sanitization unless you find a security/path traversal issue severe enough to ask the operator first.
- Do not alter behavior for disabled intros, empty `soundFile`, 404s, malformed JSON, or HTTP errors.

## Implementation expectation

- After constructing `fullPath`, check `System.IO.File.Exists(fullPath)`.
- If false, log a clear `[first-chat-intro]` no-op message including `userId` and the path, then return `true`.
- Only set `intro_sound_file_path` and call `CPH.RunAction("Intros - Play Custom Intro", true)` after the file exists.

## Validation

Run targeted validation:

```bash
python3 Tools/StreamerBot/Validation/action_contracts.py --script "Actions/Intros/first-chat-intro.cs"
```

If this fails only because Intros action contracts are missing, report that known repo-validation gap instead of solving it in this task.

Also manually review for Streamer.bot inline C# compatibility.

## Done criteria

- Missing sound file no longer calls the playback action.
- Existing happy path remains unchanged.
- No unrelated refactor.
- Handoff lists paste target and a smoke-test recommendation using one valid and one missing sound file.

Ask the operator before coding if a discovered issue would require changing the `user-intros` schema or asset path convention.
