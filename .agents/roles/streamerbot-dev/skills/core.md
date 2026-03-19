# Core Skills — streamerbot-dev

## Execution Model

Scripts are **not auto-deployed**. Each `.cs` file is manually copy/pasted into a Streamer.bot action. All code must be:
- Copy/paste ready (self-contained per script when practical)
- Runtime-safe inside Streamer.bot (no traditional build/deploy assumptions)
- Defensively coded against null/empty variables from the runtime context

## Compatibility Rules

- Guard against null/empty variables from runtime context
- Use defensive error handling around external/event data
- Ensure chat-facing output is safe and intentional
- Prefer self-contained logic per script
- If a change depends on Streamer.bot UI setup (variables, trigger wiring, action ordering), note those dependencies clearly

## State Management

- Document where state is read/written (global vars, persisted vars, in-action flow)
- Preserve existing key names unless migration is explicitly requested
- Consider concurrent triggers and re-entry edge cases
- Add reset paths for long-running sessions (especially mini-games/CYOA)
- **Any new global variable added to any feature must also be reset in `Actions/Twitch Core Integrations/stream-start.cs` and added to `Actions/SHARED-CONSTANTS.md`**

## Safety

- Never expose secrets/tokens in code comments, logs, or chat output
- Sanitize or validate user input before using it in control flow
- Keep moderator/admin-only controls protected

## Commenting Standard

- Always include thorough comments explaining intent and flow
- Write comments in plain language so a beginner developer can follow along
- Prefer short, clear steps over dense or clever code

For non-trivial scripts, include/update header comments covering:
- Purpose
- Expected trigger/input
- Required runtime variables
- Key outputs/side effects
- Operator notes (cooldown, reset command, manual setup)

When script behavior changes, update the matching feature README (`Actions/**/README.md`) using the standard sections:
- Purpose / Expected Trigger / Input / Required Runtime Variables / Key Outputs / Side Effects / Mix It Up Actions / OBS Interactions / Wait Behavior / Chat / Log Output / Operator Notes

## Reusable Patterns

Before writing new utility code, check these references:
- `Actions/HELPER-SNIPPETS.md` — copy/paste code patterns (mini-game lock, Mix It Up API, OBS scene switching, chat input helpers). **Copy verbatim — do not rewrite.**
- `Actions/SHARED-CONSTANTS.md` — canonical string names. **Look up names here; do not hardcode string literals.** When renaming any value, update all listed scripts before syncing.

## Mini-Game Lock Contract

Any new mini-game must:
1. Acquire global lock at start (`minigame_active = true`, `minigame_name = <game_name>`)
2. Block with a clear chat message if another game owns the lock
3. Release lock on **every** terminal path (win, loss, timeout, cancel, manual stop, guard exit)
4. For single-action mini-games, release lock in a `finally` block
5. Keep lock vars/reset behavior synchronized with `Actions/Twitch Core Integrations/stream-start.cs`

## CPH API — Critical Notes

Verified method signatures are in `Actions/HELPER-SNIPPETS.md` § 5. Use those exact calls.

**Do not use reflection to call OBS methods.** `ObsSetCurrentScene` and `ObsSetProgramScene` do not exist. Reflection searching for a `(string)` overload of `ObsSetScene` silently fails — the real signature is `(string sceneName, int connection = 0)`.

Use: `CPH.ObsSetScene(targetScene)` — see `Actions/HELPER-SNIPPETS.md` for the exact call.

## File Naming

- Scripts: `<action>-main.cs`, `<action>-resolve.cs`, `<action>-call.cs`, etc.
- One nested folder level beneath feature group max — no deeper nesting
- Do not move scripts between feature groups unless explicitly requested
