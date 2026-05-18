---
id: custom-intro-hardening-prompt-04
type: execution-prompt
description: Harden the repo-tracked Mix It Up Custom Intro command contract and operator setup.
owner: ops
secondaryOwners:
  - streamerbot-dev
status: active
---

# Prompt 04 — Spec the Mix It Up command

## Hardening pass context

You are working inside the `custom-intro-hardening` project. This chunk hardens step 3 of the target flow:

1. `first-chat-intro.cs` resolves local intro assets and gates execution.
2. `Intros - Play Custom Intro` acts as a thin Streamer.bot wrapper.
3. Mix It Up `Custom Intro` performs sound and/or gif playback.

This chunk's deliverable is a clear repo-tracked Mix It Up command contract and operator checklist for local-only operation.

## Required reading

Read in this order:
1. `Projects/AGENTS.md`
2. `Projects/custom-intro-hardening/README.md`
3. `Projects/custom-intro-hardening/AGENTS.md`
4. `Projects/custom-intro-hardening/progress.md`
5. `Projects/custom-intro-hardening/first-chat-prompts/01-audit-notes.md`
6. `Actions/Intros/AGENTS.md`
7. `Actions/Intros/play-custom-intro-action-spec.md`
8. `Tools/MixItUp/AGENTS.md`
9. `Actions/Helpers/mixitup-command-api.md`

## Previous chunk outputs required

This is a fresh-chat chunk. Treat these as required inputs from earlier work:
- `Projects/custom-intro-hardening/first-chat-prompts/01-audit-notes.md`
- `Actions/Intros/play-custom-intro-action-spec.md`
- `Projects/custom-intro-hardening/progress.md` showing prompts 01–03 as done

## Stop conditions

Do not proceed with edits if any of these is true:
- `Projects/custom-intro-hardening/first-chat-prompts/01-audit-notes.md` does not exist
- `Actions/Intros/play-custom-intro-action-spec.md` does not exist
- `progress.md` does not show prompts 01–03 as completed

If blocked, stop and report the missing prerequisite instead of continuing.

## Inspect before editing

First confirm what the repo already documents for Mix It Up:
- `Custom Intro` command name
- current variable names
- current operator checklist
- current future-direction notes

Then compare that to the target flow and identify what needs to be clarified or hardened.

## Required edits

Inspect first, then update the repo docs needed to make step 3 explicit and operator-usable.

Make only the edits needed across this expected file set:
- `Tools/MixItUp/AGENTS.md`
- `Tools/MixItUp/Commands/` only if a focused support doc is genuinely helpful
- `Projects/custom-intro-hardening/progress.md`

At minimum, update:
- `Tools/MixItUp/AGENTS.md`

If helpful, also create a focused support doc under:
- `Tools/MixItUp/Commands/`

The resulting docs must cover:
- exact `Custom Intro` command purpose
- sound-only behavior
- gif-only behavior
- sound+gif behavior
- neither-asset/no-op behavior
- which variables are expected
- why branching should stay simple in Mix It Up
- local-only operator setup steps
- failure/no-op behavior
- stale-variable avoidance expectations from upstream callers
- future expansion guidance for `videoFile`, overlay params, and richer interactions

## Constraints

- Do not pretend the live Mix It Up command was edited if you only changed repo docs.
- Keep Mix It Up as playback-focused, not the source of filename resolution or business logic.
- Keep the design compatible with local absolute file paths.

## Validation

Check that the final docs are consistent with:
- `first-chat-intro.cs`
- `play-custom-intro-action-spec.md`
- Mix It Up payload conventions in repo docs

## Progress update

Update `Projects/custom-intro-hardening/progress.md`:
- set this prompt to `done`
- include a short note about the Mix It Up contract/setup hardening

## Final handoff

Use exactly these headings in your final handoff:
- `Files Created/Edited`
- `Mix It Up Command Contract Summary`
- `Key Documentation Decisions`
- `Validation`
- `Remaining Live Mix It Up Setup Steps`

Under `Remaining Live Mix It Up Setup Steps`, list only the operator work that still must be done inside Mix It Up itself.
