---
id: custom-intro-hardening-prompt-03
type: execution-prompt
description: Create the repo-tracked spec for the Streamer.bot wrapper action Intros - Play Custom Intro.
owner: ops
secondaryOwners:
  - streamerbot-dev
status: active
---

# Prompt 03 — Spec the Streamer.bot wrapper action

## Hardening pass context

You are working inside the `custom-intro-hardening` project. This chunk hardens step 2 of the target flow:

1. `first-chat-intro.cs` resolves local intro assets and gates execution.
2. `Intros - Play Custom Intro` acts as a thin Streamer.bot wrapper.
3. Mix It Up `Custom Intro` performs sound and/or gif playback.

This chunk's deliverable is a repo-tracked implementation spec/checklist for the Streamer.bot action `Intros - Play Custom Intro`, since the live action internals are configured in Streamer.bot rather than stored directly in source here.

## Required reading

Read in this order:
1. `Projects/AGENTS.md`
2. `Projects/custom-intro-hardening/README.md`
3. `Projects/custom-intro-hardening/AGENTS.md`
4. `Projects/custom-intro-hardening/progress.md`
5. `Projects/custom-intro-hardening/first-chat-prompts/01-audit-notes.md`
6. `.agents/ENTRY.md`
7. `.agents/workflows/coordination.md`
8. `Actions/Intros/AGENTS.md`
9. `Tools/MixItUp/AGENTS.md`
10. `Actions/Intros/contracts.md`
11. `Actions/Intros/first-chat-intro.cs`

## Previous chunk outputs required

This is a fresh-chat chunk. Treat these as required inputs from earlier work:
- `Projects/custom-intro-hardening/first-chat-prompts/01-audit-notes.md`
- hardened step-1 repo state from prompt 02
- `Projects/custom-intro-hardening/progress.md` showing prompts 01 and 02 as done

## Stop conditions

Do not proceed with edits if either of these is true:
- `Projects/custom-intro-hardening/first-chat-prompts/01-audit-notes.md` does not exist
- `progress.md` does not show prompts 01 and 02 as completed

If blocked, stop and report the missing prerequisite instead of continuing.

## Inspect before editing

First verify what repo state already tells you about `Intros - Play Custom Intro`:
- where it is named
- what variables it is expected to consume
- what no-op behavior is already implied
- what stale-variable risks the wrapper action should explicitly defend against

## Required edits

Make only the edits needed across this expected file set:
- `Actions/Intros/play-custom-intro-action-spec.md` (new)
- `Actions/Intros/AGENTS.md`
- `Projects/custom-intro-hardening/progress.md`
- `Tools/MixItUp/AGENTS.md` only if a cross-reference update is truly needed

Create a new repo-tracked spec at:
- `Actions/Intros/play-custom-intro-action-spec.md`

The spec must define:
- purpose of `Intros - Play Custom Intro`
- inputs consumed
- exact variable contract
- wrapper responsibilities
- when to no-op
- when to call Mix It Up `Custom Intro`
- how to snapshot and clear values to avoid stale-global reuse
- expected operator build steps inside Streamer.bot
- assumptions/dependencies on step 1 and step 3

Then update any relevant docs that should point to this spec, likely including:
- `Actions/Intros/AGENTS.md`
- `Tools/MixItUp/AGENTS.md` only if cross-reference clarity is needed

## Constraints

- Do not invent direct code exports for the live Streamer.bot action if they do not exist in repo.
- Keep the wrapper simple: read variables, decide if anything should run, pass through to Mix It Up, clear/reset as appropriate.
- Do not move filename resolution into the wrapper.

## Validation

Check that the new spec is consistent with:
- the current `first-chat-intro.cs` behavior
- the documented Mix It Up payload conventions

## Progress update

Update `Projects/custom-intro-hardening/progress.md`:
- set this prompt to `done`
- include a short note about the wrapper-action spec deliverable

## Final handoff

Use exactly these headings in your final handoff:
- `Files Created/Edited`
- `Wrapper Action Contract Summary`
- `Key Spec Decisions`
- `Validation`
- `Operator-Only Streamer.bot Steps`

Under `Operator-Only Streamer.bot Steps`, list only the live steps that still must happen inside Streamer.bot.
