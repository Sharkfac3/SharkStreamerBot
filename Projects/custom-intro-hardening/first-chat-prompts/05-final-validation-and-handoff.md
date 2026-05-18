---
id: custom-intro-hardening-prompt-05
type: execution-prompt
description: Run the final hardening review, align docs/contracts, and produce the operator handoff.
owner: ops
secondaryOwners:
  - streamerbot-dev
status: active
---

# Prompt 05 — Final validation and handoff

## Hardening pass context

You are working inside the `custom-intro-hardening` project. This final chunk validates that the full first-chat intro hardening pass now supports the target flow:

1. `first-chat-intro.cs` resolves local intro assets and gates execution.
2. `Intros - Play Custom Intro` acts as a thin Streamer.bot wrapper.
3. Mix It Up `Custom Intro` performs sound and/or gif playback.

This chunk's deliverable is a final repo consistency pass plus a concise operator handoff note for implementing or verifying the remaining live Streamer.bot and Mix It Up setup.

## Required reading

Read in this order:
1. `Projects/AGENTS.md`
2. `Projects/custom-intro-hardening/README.md`
3. `Projects/custom-intro-hardening/AGENTS.md`
4. `Projects/custom-intro-hardening/progress.md`
5. `Projects/custom-intro-hardening/first-chat-prompts/01-audit-notes.md`
6. `Actions/Intros/AGENTS.md`
7. `Actions/Intros/contracts.md`
8. `Actions/Intros/first-chat-intro.cs`
9. `Actions/Intros/play-custom-intro-action-spec.md`
10. `Tools/MixItUp/AGENTS.md`

## Previous chunk outputs required

This is a fresh-chat chunk. Treat these as required inputs from earlier work:
- `Projects/custom-intro-hardening/first-chat-prompts/01-audit-notes.md`
- hardened resolver state from prompt 02
- `Actions/Intros/play-custom-intro-action-spec.md`
- hardened Mix It Up docs from prompt 04
- `Projects/custom-intro-hardening/progress.md` showing prompts 01–04 as done

## Stop conditions

Do not proceed with edits if either of these is true:
- one or more required prior-chunk artifacts are missing
- `progress.md` does not show prompts 01–04 as completed

If blocked, stop and report the missing prerequisite instead of continuing.

## Inspect before editing

First verify:
- prompts 01–04 are marked done in `progress.md`
- the resolver script, contracts, and docs all agree on the first-chat flow
- the wrapper-action spec and Mix It Up docs agree on variable names and branching rules
- no stale-variable guidance contradicts itself across files
- future-expansion notes do not conflict with the current contract

## Required edits

Make only the cleanup/alignment edits needed across this expected file set:
- `Projects/custom-intro-hardening/final-handoff.md` (new)
- `Projects/custom-intro-hardening/progress.md`
- any directly involved hardening files only if needed to remove drift

Then create:
- `Projects/custom-intro-hardening/final-handoff.md`

The handoff must include:
- final architecture recommendation summary
- step-by-step runtime flow summary
- variable contract summary
- operator checklist for Streamer.bot
- operator checklist for Mix It Up
- failure/no-op behavior summary
- any live setup work that still cannot be completed from repo alone

## Constraints

- Keep this as a final consistency and handoff pass.
- Do not start unrelated feature work.
- Only make targeted edits needed to align the completed hardening artifacts.

## Validation

Run the most relevant targeted validation for files you touch and summarize results.

## Progress update

Update `Projects/custom-intro-hardening/progress.md`:
- set this prompt to `done`
- include a short note about final validation/handoff completion

## Final handoff

Use exactly these headings in your final handoff:
- `Files Created/Edited`
- `Drift Fixed`
- `Validation`
- `Operator-Ready Next Steps`
- `Open Items`

Under `Open Items`, list only anything that still cannot be completed from repo alone.
