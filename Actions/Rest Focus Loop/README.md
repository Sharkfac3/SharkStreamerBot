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
- Disables all four loop timers to clear stale state.
- Sets the loop active flag.
- Starts timer `Rest Focus - Pre Rest` for 120 seconds.
- Sets `rest_focus_loop_phase = "pre_rest"`.

### Mix It Up Actions
- None.

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs timer start.

### Operator Notes
- Wire this to your chosen voice command that should begin the loop.

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
- Starts timer `Rest Focus - Rest` for 120 seconds.
- Sets `rest_focus_loop_phase = "rest"`.
- Triggers Mix It Up command `Wizards Rest` placeholder with `time` in seconds.

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
- Logs stale-timer guards and timer start.

### Operator Notes
- Trigger only from the matching timer end event.

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
- Triggers Captain's focus warning Mix It Up placeholder.
- Starts timer `Rest Focus - Pre Focus` for 120 seconds.
- Sets `rest_focus_loop_phase = "pre_focus"`.

### Mix It Up Actions
- Placeholder ID: `REPLACE_WITH_CAPTAINS_FOCUS_WARNING_COMMAND_ID`

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs stale-timer guards and timer start.

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
- Starts timer `Rest Focus - Focus` for 900 seconds.
- Sets `rest_focus_loop_phase = "focus"`.
- Triggers Mix It Up command `Captain's Focus` placeholder with `time` in seconds.

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
- Logs stale-timer guards and timer start.

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
- Triggers Mix It Up `Focus Timer End` placeholder.
- Starts timer `Rest Focus - Pre Rest` for 120 seconds.
- Sets `rest_focus_loop_phase = "pre_rest"`.

### Mix It Up Actions
- Placeholder ID: `REPLACE_WITH_FOCUS_TIMER_END_COMMAND_ID`

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs stale-timer guards and timer start.

---

## General Operator Notes
- Create four Streamer.bot timers with these exact names:
  - `Rest Focus - Pre Rest`
  - `Rest Focus - Rest`
  - `Rest Focus - Pre Focus`
  - `Rest Focus - Focus`
- Wire each timer's end event to the matching script above.
- Replace all placeholder Mix It Up command IDs before production use.
- `CPH.SetTimerInterval(string, int)` is still marked VERIFY in project docs. Test it in Streamer.bot before relying on dynamic durations.
- If `SetTimerInterval` is unavailable, the operator will need to swap to fixed-duration timers or another UI-driven timing setup.
