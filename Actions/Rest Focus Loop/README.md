# Rest / Focus Loop Script Reference

Shared constants reference: `Actions/SHARED-CONSTANTS.md`

## Scope
This folder contains the repeating rest/focus loop controller for Streamer.bot.

The loop model is:
1. Voice command starts the loop and opens a 2-minute pre-rest window.
2. Water Wizard can run `!castrest X` to start the actual rest timer early.
3. If the pre-rest window expires, the loop falls back to a 2-minute rest timer.
4. When rest ends, the Captain's focus warning Mix It Up command fires and a 2-minute pre-focus window opens.
5. Captain Stretch can run `!generalfocus X` to start the actual focus timer early.
6. If the pre-focus window expires, the loop falls back to a 15-minute focus timer.
7. When focus ends, the Focus Timer End Mix It Up command fires and the loop returns to pre-rest.

## Hardened Transition Model
The current implementation is intentionally defensive:
- Every transition writes the next `rest_focus_loop_phase` **before** arming the next timer.
- Every timer-arm path disables the three non-target loop timers first, then disables and re-arms the target timer.
- Every timer-end script checks both `rest_focus_loop_active` and the expected phase, so stale timer fires are logged and ignored instead of advancing the loop again.
- If a timer cannot be armed, the scripts **fail closed**: all four loop timers are disabled and `rest_focus_loop_active` is set to `false` so the operator can restart from a known-good state.
- Mix It Up failures are logged, but they do **not** stop the loop by themselves. Only timer-arm failure drops the loop inactive.

---

## Script: `rest-focus-loop-start.cs`

### Purpose
Starts or restarts the loop from the pre-rest phase.

### Expected Trigger / Input
- Voice command, button, or manual chained action.

### Required Runtime Variables
- Writes `rest_focus_loop_active`
- Writes `rest_focus_loop_phase`

### Key Outputs / Side Effects
- Sets `rest_focus_loop_active = true` before attempting to arm the first timer.
- Sets `rest_focus_loop_phase = "pre_rest"` before timer arming so stale overlap sees the intended state.
- Defensively disables every non-target loop timer, then disables and re-arms timer `Rest Focus - Pre Rest` for 120 seconds.
- If timer arming fails, disables all four loop timers and sets `rest_focus_loop_active = false`.

### Mix It Up Actions
- None.

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs timer arming attempts.
- Logs timer-arm failure and fail-closed recovery if startup cannot arm the first timer.

### Operator Notes
- Wire this to your chosen voice command, button, or manual action that should begin the loop.
- This script does not send a chat confirmation; use logs to verify startup if needed.

---

## Script: `rest-focus-pre-rest-end.cs`

### Purpose
Handles the default path when pre-rest expires before Water Wizard calls `!castrest`.

### Expected Trigger / Input
- Timer end trigger for `Rest Focus - Pre Rest`.

### Required Runtime Variables
- Reads `rest_focus_loop_active`
- Reads/writes `rest_focus_loop_phase`

### Key Outputs / Side Effects
- Exits immediately if the loop is inactive.
- Ignores stale timer fires unless `rest_focus_loop_phase` is still `"pre_rest"`.
- Sets `rest_focus_loop_phase = "rest"` before arming the rest timer.
- Triggers Mix It Up command `Wizards Rest` placeholder with `time` in seconds.
- Defensively disables every non-target loop timer, then disables and re-arms timer `Rest Focus - Rest` for 120 seconds.
- If timer arming fails, disables all four loop timers and sets `rest_focus_loop_active = false`.

### Mix It Up Actions
- Placeholder ID: `REPLACE_WITH_WIZARDS_REST_COMMAND_ID`
- Sends `Arguments = "120"`
- Sends `SpecialIdentifiers.time = "120"`

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs stale-timer guards, timer arming attempts, Mix It Up failures, and fail-closed recovery if timer arming fails.

### Operator Notes
- Trigger only from the matching timer end event.
- If Mix It Up is unavailable or its command ID is still a placeholder, the loop continues; only the external call is skipped/failed.

---

## Script: `rest-focus-rest-end.cs`

### Purpose
Handles the transition from active rest into the pre-focus warning window.

### Expected Trigger / Input
- Timer end trigger for `Rest Focus - Rest`.

### Required Runtime Variables
- Reads `rest_focus_loop_active`
- Reads/writes `rest_focus_loop_phase`

### Key Outputs / Side Effects
- Exits immediately if the loop is inactive.
- Ignores stale timer fires unless `rest_focus_loop_phase` is still `"rest"`.
- Sets `rest_focus_loop_phase = "pre_focus"` before arming the pre-focus timer.
- Triggers Captain's focus warning Mix It Up placeholder.
- Defensively disables every non-target loop timer, then disables and re-arms timer `Rest Focus - Pre Focus` for 120 seconds.
- If timer arming fails, disables all four loop timers and sets `rest_focus_loop_active = false`.

### Mix It Up Actions
- Placeholder ID: `REPLACE_WITH_CAPTAINS_FOCUS_WARNING_COMMAND_ID`

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs stale-timer guards, timer arming attempts, Mix It Up failures, and fail-closed recovery if timer arming fails.

---

## Script: `rest-focus-pre-focus-end.cs`

### Purpose
Handles the default path when pre-focus expires before Captain Stretch calls `!generalfocus`.

### Expected Trigger / Input
- Timer end trigger for `Rest Focus - Pre Focus`.

### Required Runtime Variables
- Reads `rest_focus_loop_active`
- Reads/writes `rest_focus_loop_phase`

### Key Outputs / Side Effects
- Exits immediately if the loop is inactive.
- Ignores stale timer fires unless `rest_focus_loop_phase` is still `"pre_focus"`.
- Sets `rest_focus_loop_phase = "focus"` before arming the focus timer.
- Triggers Mix It Up command `Captain's Focus` placeholder with `time` in seconds.
- Defensively disables every non-target loop timer, then disables and re-arms timer `Rest Focus - Focus` for 900 seconds.
- If timer arming fails, disables all four loop timers and sets `rest_focus_loop_active = false`.

### Mix It Up Actions
- Placeholder ID: `REPLACE_WITH_CAPTAINS_FOCUS_COMMAND_ID`
- Sends `Arguments = "900"`
- Sends `SpecialIdentifiers.time = "900"`

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs stale-timer guards, timer arming attempts, Mix It Up failures, and fail-closed recovery if timer arming fails.

---

## Script: `rest-focus-focus-end.cs`

### Purpose
Handles the end of the active focus timer and loops back to pre-rest.

### Expected Trigger / Input
- Timer end trigger for `Rest Focus - Focus`.

### Required Runtime Variables
- Reads `rest_focus_loop_active`
- Reads/writes `rest_focus_loop_phase`

### Key Outputs / Side Effects
- Exits immediately if the loop is inactive.
- Ignores stale timer fires unless `rest_focus_loop_phase` is still `"focus"`.
- Sets `rest_focus_loop_phase = "pre_rest"` before arming the next pre-rest timer.
- Triggers Mix It Up `Focus Timer End` placeholder.
- Defensively disables every non-target loop timers, then disables and re-arms timer `Rest Focus - Pre Rest` for 120 seconds.
- If timer arming fails, disables all four loop timers and sets `rest_focus_loop_active = false`.

### Mix It Up Actions
- Placeholder ID: `REPLACE_WITH_FOCUS_TIMER_END_COMMAND_ID`

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs stale-timer guards, timer arming attempts, Mix It Up failures, and fail-closed recovery if timer arming fails.

---

## General Operator Notes
- Create four Streamer.bot timers with these exact names:
  - `Rest Focus - Pre Rest`
  - `Rest Focus - Rest`
  - `Rest Focus - Pre Focus`
  - `Rest Focus - Focus`
- Wire each timer's end event to the matching script above.
- Replace all placeholder Mix It Up command IDs before production use.
- `CPH.SetTimerInterval(string, int)` is still effectively a dependency for this implementation:
  - The start script uses it for the pre-rest timer.
  - The loop transition scripts use it for default rest/focus durations.
  - The commander overrides use it for operator-selected `!castrest` / `!generalfocus` durations.
- `CPH.SetTimerInterval(string, int)` is still marked VERIFY in project docs. Test it in your Streamer.bot build before relying on this loop in production.
- If `SetTimerInterval` is missing or throws in your build, the current scripts do **not** fall back to fixed UI timer lengths automatically. They fail closed by disabling all four loop timers and clearing `rest_focus_loop_active`.
- Manual recovery after a timer-arm failure:
  1. Check Streamer.bot logs for the script name and the timer that failed to arm.
  2. Verify the timer exists with the exact expected name.
  3. Verify your Streamer.bot build supports `CPH.SetTimerInterval(string, int)`.
  4. Re-run your loop start action after correcting the timer/setup problem.
- Manual sync reminder: these scripts are repo source only. After doc or code changes, keep the matching Streamer.bot actions and timer-end wiring in sync manually.
