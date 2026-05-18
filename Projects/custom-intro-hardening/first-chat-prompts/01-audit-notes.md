---
id: custom-intro-hardening-audit-notes-01
type: audit-note
description: Current-state audit for the first-chat custom intro hardening pass.
owner: streamerbot-dev
status: active
---

# Audit Notes — Prompt 01

## Short current-state summary

Repo state already points at the intended first-chat architecture: `Actions/Intros/first-chat-intro.cs` is the first-word gatekeeper, `Intros - Play Custom Intro` is named as the playback handoff, and `Tools/MixItUp/AGENTS.md` documents a `Custom Intro` Mix It Up command that consumes `intro_sound_file_path` and `intro_gif_file_path`.

What is still missing is hardening around stale variable handling plus repo-tracked operator specs for the live Streamer.bot wrapper action and the live Mix It Up command internals.

## Matches target already

### Step 1: `first-chat-intro.cs`

`Actions/Intros/first-chat-intro.cs` already largely matches the target step-1 role:
- reads `userId` from the First Words trigger
- GETs `user-intros` from info-service
- requires `enabled = true`
- reads optional `soundFile` / `gifFile`
- resolves filenames to local absolute paths under `Assets/user-intros/sound/` and `Assets/user-intros/gif/`
- no-ops on missing user, 404, non-200 HTTP, JSON errors, disabled record, missing filenames, or no locally resolvable assets
- supports sound-only, gif-only, and sound+gif
- sets `intro_sound_file_path` / `intro_gif_file_path`
- calls `CPH.RunAction("Intros - Play Custom Intro", true)` only when at least one asset resolved

### Wrapper role is already implied in repo docs

The repo already says the wrapper should stay thin, but not yet in one dedicated spec:
- `Actions/Intros/AGENTS.md` says playback should stay delegated to `Intros - Play Custom Intro`
- `first-chat-intro.cs` comments describe that action as the handoff point after path resolution
- `Actions/Intros/contracts.md` says the resolver sets globals and then runs `Intros - Play Custom Intro`

### Mix It Up role is already partially defined

`Tools/MixItUp/AGENTS.md` already defines the basic role for `Custom Intro`:
- command name is `Custom Intro`
- triggered by `Intros - Play Custom Intro`
- consumes `intro_sound_file_path` and/or `intro_gif_file_path`
- should keep filename resolution in Streamer.bot
- includes a basic operator checklist for sound and gif playback

## Gaps / hardening targets

1. **Stale-global risk is not hardened yet.**
   - `first-chat-intro.cs` only writes globals on the dispatch path.
   - If an earlier run set one or both globals, later no-op paths do not clear them.
   - The live wrapper action is not repo-tracked, so its current clear/snapshot behavior is unknown.

2. **Wrapper behavior is documented only indirectly.**
   - The repo names `Intros - Play Custom Intro`, but does not yet contain a dedicated operator-facing spec for exactly how it should read, snapshot, branch, call Mix It Up, and clear variables.

3. **Mix It Up command behavior is documented only at a high level.**
   - The repo has a checklist, but not yet a hardened contract for sound-only, gif-only, sound+gif, and neither/no-op handling.
   - There is no repo-tracked confirmation of how the live command currently branches.

4. **Live internals are not source-controlled here.**
   - Streamer.bot sub-action internals for `Intros - Play Custom Intro` are operator-configured, not stored as code in this repo.
   - Mix It Up `Custom Intro` command internals are likewise not directly represented as runnable source here.
   - That means repo docs can define desired behavior, but cannot prove the live setup currently matches it.

5. **Failure boundaries between step 2 and step 3 still need explicit ownership.**
   - Step 1 already decides whether anything should run.
   - Step 2 and step 3 still need clearer repo guidance that they should stay playback-focused and not re-own filename resolution or business logic.

## Repo-editable vs operator-config-only

### Repo-editable
- `Actions/Intros/first-chat-intro.cs`
- `Actions/Intros/contracts.md`
- `Actions/Intros/AGENTS.md`
- `Tools/MixItUp/AGENTS.md`
- new repo-tracked specs/checklists for wrapper and Mix It Up setup
- project audit/progress/handoff notes

### Operator-config-only
- live Streamer.bot internals of `Intros - Play Custom Intro`
- live Mix It Up internals of `Custom Intro`
- validating whether production Streamer.bot and Mix It Up currently match repo docs
- any First Words reset / per-stream operator procedure

## Recommended execution order for prompts 02–05

1. **Prompt 02 — Harden the resolver first.**
   Make step 1 authoritative and explicitly safe around stale global reuse, then update contracts/docs to match.

2. **Prompt 03 — Spec the Streamer.bot wrapper next.**
   Once resolver behavior is stable, define the exact wrapper responsibilities: snapshot current values, no-op when both are empty, pass through to Mix It Up, and clear/reset defensively.

3. **Prompt 04 — Harden the Mix It Up command contract after the wrapper spec exists.**
   Document the exact playback expectations for sound-only, gif-only, sound+gif, and neither/no-op behavior.

4. **Prompt 05 — Final consistency pass last.**
   Align resolver contract, wrapper spec, Mix It Up docs, and operator-only setup steps into one handoff.
