using System;

public class CPHInline
{
    // SYNC CONSTANTS (Rest / Focus Loop)
    // Keep these names aligned with:
    // - Actions/Rest Focus Loop/rest-focus-loop-start.cs
    // - Actions/Rest Focus Loop/rest-focus-pre-rest-end.cs
    // - Actions/Rest Focus Loop/rest-focus-rest-end.cs
    // - Actions/Rest Focus Loop/rest-focus-pre-focus-end.cs
    // - Actions/Rest Focus Loop/rest-focus-focus-end.cs
    // - Actions/Commanders/Water Wizard/water-wizard-castrest.cs
    // - Actions/Commanders/Captain Stretch/captain-stretch-generalfocus.cs
    // - Actions/Twitch Core Integrations/stream-start.cs
    private const string VAR_REST_FOCUS_LOOP_ACTIVE = "rest_focus_loop_active";
    private const string VAR_REST_FOCUS_LOOP_PHASE = "rest_focus_loop_phase";

    private const string PHASE_IDLE = "idle";
    private const string PHASE_PRE_REST = "pre_rest";

    private const string TIMER_PRE_REST = "Rest Focus - Pre Rest";
    private const string TIMER_REST = "Rest Focus - Rest";
    private const string TIMER_PRE_FOCUS = "Rest Focus - Pre Focus";
    private const string TIMER_FOCUS = "Rest Focus - Focus";

    private const int PRE_REST_SECONDS = 120;

    /*
     * Purpose:
     * - Starts or restarts the repeating rest/focus loop from the pre-rest window.
     * - Intended to be triggered by a voice command or a manually chained action.
     *
     * Expected trigger/input:
     * - Voice command action.
     * - No chat input required.
     *
     * Required runtime variables:
     * - Writes rest_focus_loop_active.
     * - Writes rest_focus_loop_phase.
     *
     * Key outputs/side effects:
     * - Stops any currently running loop timers.
     * - Marks the loop as active.
     * - Starts the 2-minute pre-rest timer.
     *
     * Operator notes:
     * - This script assumes the four timers already exist in Streamer.bot with the exact names above.
     * - This script uses CPH.SetTimerInterval(string, int). That method signature is still marked VERIFY in project docs.
     * - If SetTimerInterval is unavailable in your Streamer.bot build, set the timer duration in the UI and remove that call.
     */
    public bool Execute()
    {
        StopAllLoopTimers();

        CPH.SetGlobalVar(VAR_REST_FOCUS_LOOP_ACTIVE, true, false);
        CPH.SetGlobalVar(VAR_REST_FOCUS_LOOP_PHASE, PHASE_IDLE, false);

        StartTimer(TIMER_PRE_REST, PRE_REST_SECONDS, "Rest Focus Loop Start");
        CPH.SetGlobalVar(VAR_REST_FOCUS_LOOP_PHASE, PHASE_PRE_REST, false);

        return true;
    }

    private void StopAllLoopTimers()
    {
        CPH.DisableTimer(TIMER_PRE_REST);
        CPH.DisableTimer(TIMER_REST);
        CPH.DisableTimer(TIMER_PRE_FOCUS);
        CPH.DisableTimer(TIMER_FOCUS);
    }

    private void StartTimer(string timerName, int seconds, string logPrefix)
    {
        if (seconds < 1)
            seconds = 1;

        CPH.LogWarn($"[{logPrefix}] Starting timer '{timerName}' for {seconds} second(s).");
        CPH.DisableTimer(timerName);
        CPH.SetTimerInterval(timerName, seconds); // VERIFY: unconfirmed method signature
        CPH.EnableTimer(timerName);
    }
}
