---
id: custom-intro-hardening-prompt-01
type: execution-prompt
description: Audit the current first-chat custom intro flow against the hardening target before edits begin.
owner: ops
status: active
---

# Prompt 01 — Audit current state

## Hardening pass context

You are working inside the `custom-intro-hardening` project. This prompt is the audit chunk for the first-chat intro hardening pass.

Target flow:
1. `first-chat-intro.cs` resolves local intro assets and decides whether anything should run.
2. Streamer.bot action `Intros - Play Custom Intro` acts as a thin playback wrapper.
3. Mix It Up command `Custom Intro` performs sound and/or gif playback from passed-through variables.

This chunk's deliverable is a repo-tracked audit note that records what already exists, what is still missing, and what later prompts need to harden.

## Required reading

Read in this order:
1. `Projects/AGENTS.md`
2. `Projects/custom-intro-hardening/README.md`
3. `Projects/custom-intro-hardening/AGENTS.md`
4. `Projects/custom-intro-hardening/progress.md`
5. `.agents/ENTRY.md`
6. `.agents/workflows/coordination.md`
7. `.agents/roles/streamerbot-dev/role.md`
8. `Actions/Intros/AGENTS.md`
9. `Tools/MixItUp/AGENTS.md`
10. `Actions/Intros/contracts.md`
11. `Actions/Intros/first-chat-intro.cs`

## What to inspect first

Before editing anything, inspect and summarize:
- whether `first-chat-intro.cs` already matches step 1
- whether repo docs already define the `Intros - Play Custom Intro` wrapper role
- whether repo docs already define the Mix It Up `Custom Intro` role
- any stale-variable risks or no-op/failure gaps you can see from current repo state
- any repo-tracked gaps caused by the fact that Streamer.bot action internals and live Mix It Up command internals are not stored directly in code here

## Required edits

Create a new audit note at:
- `Projects/custom-intro-hardening/first-chat-prompts/01-audit-notes.md`

The audit note must include:
- a short current-state summary
- a "matches target already" section
- a "gaps / hardening targets" section
- a "repo-editable vs operator-config-only" section
- a recommended execution order for prompts 02–05

Then update:
- `Projects/custom-intro-hardening/progress.md`

Set this prompt to `done` and add a brief note.

## Constraints

- Do not change runtime behavior in this chunk.
- This is an audit/documentation pass only.
- Keep the audit focused on the first-chat intro flow, not the redemption flow.

## Validation

No special validator is required beyond checking that the new audit file exists and the progress row is updated.

## Final handoff

Report:
- files created/edited
- key current-state findings
- the top hardening priorities for the next prompt
