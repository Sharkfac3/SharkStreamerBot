# C9 — MixItUp Custom Intro Command Spec

Role: `streamerbot-dev` (docs-only pass — no `.cs` changes).

## Prereqs

C8 merged. `Actions/Intros/first-chat-intro.cs` exists and dispatches via:
```csharp
CPH.SetGlobalVar("intro_sound_file_path", fullPath, false);
CPH.RunAction("Intros - Play Custom Intro", true);
```

## WORKING.md

Add row at start. Domain: `Docs/`. Files: `.agents/_shared/mixitup-api.md`. Remove + log at finish.

## Task

Document the MixItUp `Custom Intro` command spec in `.agents/_shared/mixitup-api.md`.

This is **operator-facing documentation only** — no C# work, no TypeScript work.

### What to add

Append a new section `## Custom Intro Command` to `.agents/_shared/mixitup-api.md`:

```markdown
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
- The path is constructed in `first-chat-intro.cs` from `ASSETS_ROOT + SOUND_SUBPATH + soundFile`.
- Operator is responsible for ensuring the sound file actually exists at the constructed path before
  enabling an intro in the production-manager.
- No overlay work — audio playback is fully owned by Mix It Up.
```

### Also update COORDINATION.md

C9 status → `merged`, run log row appended (Commit: `uncommitted`).

## Deliverables

- `.agents/_shared/mixitup-api.md` — `## Custom Intro Command` section appended
- `COORDINATION.md` — C9 status → `merged`, run log row appended

## Forbidden in this chunk

- Any C# code changes
- Any TypeScript code changes
- Defining Mix It Up command internals (operator config only)

## Finish

1. Load `ops-change-summary` from `.agents/roles/ops/skills/change-summary/_index.md`, show output.
2. Check COORDINATION.md for next draftable chunks.
   - C11 (`Docs + scaffolding sweep`) prereq = all above. Still `not-started` pending operator review.
3. **Do NOT run `git commit` or `git add`. Operator commits manually.**

## Definition of done

- `.agents/_shared/mixitup-api.md` has `## Custom Intro Command` section
- Section covers: command name, triggering action, argument format, operator setup checklist
- `COORDINATION.md` C9 = `merged`
- No git commit made by agent
