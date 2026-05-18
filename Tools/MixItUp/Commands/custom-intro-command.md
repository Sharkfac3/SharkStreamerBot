---
id: mixitup-custom-intro-command
type: operator-spec
description: Repo-tracked contract and operator checklist for the live Mix It Up `Custom Intro` command.
owner: ops
secondaryOwners:
  - streamerbot-dev
status: active
---

# Custom Intro — Mix It Up Command Spec

## Purpose

`Custom Intro` is the playback-only Mix It Up command for the first-chat intro flow:

1. `first-chat-intro.cs` decides whether an intro should run and resolves local asset paths.
2. `first-chat-intro.cs` calls Mix It Up `Custom Intro` directly through the Developer API.
3. Mix It Up command `Custom Intro` plays the sound and/or gif for that one run.

This command must not query info-service, resolve filenames, decide whether a user is eligible, or invent fallback business logic.

## Expected variables

`Custom Intro` must honor these Mix It Up payload special identifiers:

- `intro_sound_file_path`
- `intro_gif_file_path`

Upstream may also include additional metadata such as `userid`. That metadata is optional and should not change playback behavior unless a future repo-tracked contract explicitly says otherwise.

Expected value shape:

- each value is either `""` or a full local absolute file path
- sound and gif are independent
- either one or both may be populated
- paths are already normalized/resolved upstream

Example values:

- `C:\Users\sharkfac3\Workspace\coding\SharkStreamerBot\Assets\user-intros\sound\viewer.mp3`
- `C:\Users\sharkfac3\Workspace\coding\SharkStreamerBot\Assets\user-intros\gif\viewer.gif`

## Command behavior contract

### Sound-only

If `intro_sound_file_path` is non-empty and `intro_gif_file_path` is empty:

- play the sound
- skip gif/visual steps cleanly
- do not fail just because gif is absent

### Gif-only

If `intro_gif_file_path` is non-empty and `intro_sound_file_path` is empty:

- play/show the gif
- skip sound steps cleanly
- do not fail just because sound is absent

### Sound + gif

If both variables are non-empty:

- play the sound
- play/show the gif
- keep both branches in the same command run

### Neither asset

If both variables are empty:

- no-op
- do not attempt fallback playback
- do not try to resolve older values or default media

## Branching guidance

Keep Mix It Up branching simple:

- branch only on whether each variable is empty or non-empty
- treat sound and gif as optional independent branches
- do not move filename resolution, file existence checks, user gating, or intro policy into Mix It Up
- do not add extra decision layers unless a new repo-tracked contract explicitly requires them

## Local-only operator setup

Inside Mix It Up:

1. Create or open a command named exactly `Custom Intro`.
2. Make sure the command can read `intro_sound_file_path` and `intro_gif_file_path` from the incoming API payload special identifiers.
3. Add one simple sound branch that plays only when `intro_sound_file_path` is non-empty.
4. Add one simple gif/visual branch that shows only when `intro_gif_file_path` is non-empty.
5. Keep both branches local-path compatible; assume absolute Windows file paths.
6. Do not add file-name rewriting, path concatenation, or info-service lookups in Mix It Up.
7. Keep cooldowns/requirements disabled or harmless for first-chat playback.
8. Test all four cases: sound-only, gif-only, sound+gif, neither.

## Failure / no-op expectations

- Missing sound path: gif branch may still run.
- Missing gif path: sound branch may still run.
- Both missing: command should no-op.
- If a playback action cannot use the provided path, that run should fail only for that asset branch; do not introduce fallback media automatically.
- Logs/operator-facing command layout should make it obvious whether the command ran sound, gif, both, or neither.

## Payload expectations

Upstream callers should send the standard Mix It Up API payload shape:

- `Platform = "Twitch"`
- `Arguments = ""`
- `SpecialIdentifiers.intro_sound_file_path`
- `SpecialIdentifiers.intro_gif_file_path`
- optional metadata may be present in `SpecialIdentifiers` (for example `userid`)
- `IgnoreRequirements = false`

## Future expansion guidance

If intros expand later, add new explicit contract fields instead of overloading the current two variables.

Guidelines for expansion:

- keep filename/path resolution upstream
- keep new variables explicit and narrowly named
- update `Actions/Intros/contracts.md`, `Tools/MixItUp/AGENTS.md`, and this file together
- preserve the playback-focused responsibility split
