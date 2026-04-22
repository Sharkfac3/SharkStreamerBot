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

## Mix It Up Optional Message Pattern

When a Streamer.bot event includes an optional user-authored text field and that value needs to be forwarded to Mix It Up, prefer sending **both**:

- the user text itself as a special identifier (for example `watchstreakmessage`)
- a companion type/status special identifier (for example `watchstreaktype`)

Default contract:
- user text present → send the text unchanged and set type to `message`
- user text missing/blank → send empty string and set type to `none`

Do not silently swap in `systemMessage` or another fallback string unless the operator explicitly wants that behavior. This keeps Mix It Up branching explicit and preserves the difference between "viewer wrote something" and "no viewer message existed." 
