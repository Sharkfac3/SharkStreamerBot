---
id: custom-intro-hardening-final-handoff
type: handoff
description: Final validation summary and operator handoff for the first-chat custom intro hardening pass.
owner: ops
status: active
---

# Final Handoff — Custom Intro Hardening

## Final architecture recommendation summary

Keep the first-chat intro flow split into three clear responsibilities:

1. `Actions/Intros/first-chat-intro.cs` is the authoritative resolver and gatekeeper.
2. Streamer.bot action `Intros - Play Custom Intro` is a thin wrapper that snapshots, clears, forwards, and clears again.
3. Mix It Up command `Custom Intro` is playback-only and branches only on provided sound/gif variables.

This is the recommended steady-state architecture because it keeps intro eligibility, asset resolution, shared-state defense, and media playback separated cleanly.

## Step-by-step runtime flow summary

1. Streamer.bot `Twitch -> Chat -> First Words` triggers `first-chat-intro.cs`.
2. `first-chat-intro.cs` clears `intro_sound_file_path` and `intro_gif_file_path` immediately.
3. The resolver reads `userId` and fetches the viewer record from `user-intros` in info-service.
4. The resolver no-ops for missing user, missing record, disabled intro, bad HTTP/JSON, missing asset names, or no resolvable local assets.
5. If `soundFile` and/or `gifFile` exist, the resolver normalizes them to filename-only values.
6. The resolver resolves local full paths under `Assets/user-intros/sound/` and `Assets/user-intros/gif/`.
7. If at least one local asset exists, the resolver sets the globals to resolved path-or-empty values and runs `Intros - Play Custom Intro` synchronously.
8. The wrapper snapshots both globals into local action values.
9. The wrapper clears both shared globals immediately.
10. If both snapshots are empty, the wrapper logs/no-ops and stops.
11. Otherwise, the wrapper re-sets the globals from the snapshots for the current dispatch window.
12. The wrapper triggers Mix It Up command `Custom Intro`.
13. Mix It Up plays sound and/or gif based only on whether each variable is empty.
14. After handoff completes, the wrapper clears both globals again.

## Variable contract summary

Shared variable names:

- `intro_sound_file_path`
- `intro_gif_file_path`

Value contract:

- each value must be either `""` or a full local absolute Windows file path
- sound and gif are independent
- either one or both may be populated
- upstream resolver owns filename normalization and local path resolution

Responsibility split:

- resolver: clear early, resolve assets, set current-run values, decide dispatch vs no-op
- wrapper: snapshot, clear, branch on snapshots, re-set for dispatch, clear again
- Mix It Up: playback only; no filename resolution, no user gating, no fallback business logic

## Operator checklist for Streamer.bot

1. Confirm `Intros - First Chat Intro` uses `Actions/Intros/first-chat-intro.cs`.
2. Confirm the action is wired to `Twitch -> Chat -> First Words`.
3. Confirm the live action `Intros - Play Custom Intro` exists.
4. In that wrapper action, snapshot `intro_sound_file_path` and `intro_gif_file_path` into local action values.
5. Clear both globals immediately after snapshot.
6. If both snapshots are empty, log/no-op and stop.
7. If either snapshot is non-empty, re-set both globals from the snapshot values.
8. Trigger Mix It Up command `Custom Intro` exactly once.
9. Clear both globals again after the handoff step.
10. Keep the wrapper ordered/synchronous so cleanup happens after the current run.
11. Reset Streamer.bot First Words state each stream/session as needed so approved viewers can trigger their first-chat intro once per stream.

## Operator checklist for Mix It Up

1. Confirm a command named exactly `Custom Intro` exists.
2. Add a sound branch that plays `intro_sound_file_path` only when non-empty.
3. Add a gif/visual branch that shows `intro_gif_file_path` only when non-empty.
4. Support sound-only, gif-only, and sound+gif in one command run.
5. If both variables are empty, the command should no-op safely.
6. Do not add filename rewriting, path concatenation, info-service lookups, or user gating here.
7. Keep command requirements/cooldowns disabled or harmless for first-chat playback.
8. Test four cases: sound-only, gif-only, sound+gif, neither.

## Failure/no-op behavior summary

`first-chat-intro.cs` no-ops when:

- `userId` is missing
- info-service returns 404 or unexpected HTTP
- JSON is malformed
- `enabled` is false or missing
- both configured asset names are empty
- configured assets do not resolve to local files

`Intros - Play Custom Intro` no-ops when:

- both snapshot values are empty after normalization

`Custom Intro` should remain safe when:

- sound is missing but gif is present
- gif is missing but sound is present
- both are missing
- one playback branch fails and the other still has usable media

## Live setup work that still cannot be completed from repo alone

- Building or verifying the live internal sub-actions of Streamer.bot action `Intros - Play Custom Intro`
- Building or verifying the live internal branches/actions of Mix It Up command `Custom Intro`
- Confirming the exact live command/action behavior in production after manual operator testing
- Resetting/operating Streamer.bot First Words state during actual stream runs
