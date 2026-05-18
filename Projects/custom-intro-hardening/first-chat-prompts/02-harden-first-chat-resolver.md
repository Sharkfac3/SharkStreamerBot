---
id: custom-intro-hardening-prompt-02
type: execution-prompt
description: Harden the Streamer.bot first-chat intro resolver script and its contract/docs.
owner: streamerbot-dev
status: active
---

# Prompt 02 — Harden first-chat resolver

## Hardening pass context

You are working inside the `custom-intro-hardening` project. This chunk hardens step 1 of the target flow:

1. `first-chat-intro.cs` resolves local intro assets and decides whether anything should run.
2. `Intros - Play Custom Intro` acts as a thin Streamer.bot wrapper.
3. Mix It Up `Custom Intro` performs playback.

This chunk's deliverable is the repo-side hardening of the resolver script and its declared contract so later chunks can treat step 1 as stable.

## Required reading

Read in this order:
1. `Projects/AGENTS.md`
2. `Projects/custom-intro-hardening/README.md`
3. `Projects/custom-intro-hardening/AGENTS.md`
4. `Projects/custom-intro-hardening/progress.md`
5. `Projects/custom-intro-hardening/first-chat-prompts/01-audit-notes.md`
6. `.agents/ENTRY.md`
7. `.agents/workflows/coordination.md`
8. `.agents/roles/streamerbot-dev/role.md`
9. `Actions/Intros/AGENTS.md`
10. `Actions/Intros/contracts.md`
11. `Actions/Intros/first-chat-intro.cs`
12. `Tools/MixItUp/AGENTS.md`

## Previous chunk outputs required

This is a fresh-chat chunk. Treat these as required inputs from earlier work:
- `Projects/custom-intro-hardening/first-chat-prompts/01-audit-notes.md`
- `Projects/custom-intro-hardening/progress.md` showing prompt 01 as done

## Stop conditions

Do not proceed with edits if either of these is true:
- `Projects/custom-intro-hardening/first-chat-prompts/01-audit-notes.md` does not exist
- `progress.md` does not show prompt 01 as completed

If blocked, stop and report the missing prerequisite instead of continuing.

## Inspect before editing

First confirm what the script already does today:
- user lookup
- enabled check
- `soundFile` / `gifFile` filename handling
- local file resolution
- no-op behavior when no usable assets exist
- setting `intro_sound_file_path` / `intro_gif_file_path`
- dispatch to `Intros - Play Custom Intro`

Then compare current behavior to the hardening goals below.

## Hardening goals

Harden `first-chat-intro.cs` so step 1 is explicit and resilient:
- it should remain the authoritative gatekeeper for whether an intro should run at all
- it should explicitly avoid stale asset-path reuse risk
- it should continue to support all 4 cases:
  - sound only
  - gif only
  - sound + gif
  - neither asset present
- it should log enough to explain why an intro did or did not dispatch
- its contract/docs should describe the behavior clearly enough that later prompts do not need to rediscover it

## Required edits

Inspect current state first, then make only the edits needed across this expected file set:
- `Actions/Intros/first-chat-intro.cs`
- `Actions/Intros/contracts.md`
- `Actions/Intros/AGENTS.md`
- `Projects/custom-intro-hardening/progress.md`

Do not expand beyond this file set unless a tightly related doc cross-reference is required for accuracy; if you must expand, justify it in the final handoff.

Specific expectations:
- if you identify a stale-global risk, harden against it in the script
- preserve local-only operation
- preserve no-op behavior when nothing resolves
- keep filename resolution in Streamer.bot, not Mix It Up
- update the contract and folder guide to match any behavior changes
- if the script body changes, update the ACTION-CONTRACT SHA stamp correctly

## Constraints

- Do not try to implement the live `Intros - Play Custom Intro` Streamer.bot action internals here.
- Do not redesign the intake/redemption path.
- Keep changes tightly scoped to first-chat playback hardening.

## Validation

Run whatever targeted validation is appropriate for the touched files. At minimum:
- verify the contract text matches the script behavior
- verify the SHA stamp matches if the script changed

## Progress update

Update `Projects/custom-intro-hardening/progress.md`:
- set this prompt to `done`
- include a short note about what was hardened

## Final handoff

Use exactly these headings in your final handoff:
- `Files Edited`
- `Current Behavior After Hardening`
- `Key Changes Made`
- `Validation`
- `Prompt 03 Assumptions / Follow-ons`

Under `Prompt 03 Assumptions / Follow-ons`, explicitly call out any wrapper-action assumptions that the next chunk must honor.
