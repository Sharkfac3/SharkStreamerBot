# streamerbot-dev — Discovered Patterns & Gotchas

Agents: add notes here when you discover something that isn't obvious from the code and will save future agents time.

---

## OBS Scene Switching — Reflection Gotcha

`ObsSetCurrentScene` and `ObsSetProgramScene` do not exist in CPH. Reflection searching for a `(string)` overload of `ObsSetScene` silently fails. The real signature is `(string sceneName, int connection = 0)`. Use `CPH.ObsSetScene(targetScene)` directly.

## Mix It Up Unlock Wait — Standard Buffer

After triggering a Mix It Up command that queues audio/animation, wait **31 seconds** before the next action. This prevents overlap in the MixItUp queue. Pattern is in `Actions/HELPER-SNIPPETS.md`.

## Pedro Unlock — Multiple Fires Allowed

Pedro's secret unlock is intentionally allowed to trigger Mix It Up multiple times per stream. Do not treat repeated fires as a bug or add a one-per-stream guard unless the operator explicitly requests it.

## stream_mode Fallback

All scripts that read `stream_mode` should fall back to `"workspace"` if the value is null, empty, or unrecognized. Never hard-fail on an unknown mode value.
