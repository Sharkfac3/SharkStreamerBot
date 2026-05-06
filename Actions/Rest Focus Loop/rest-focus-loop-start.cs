// ACTION-CONTRACT: Actions/Rest Focus Loop/AGENTS.md#rest-focus-loop-start.cs
// ACTION-CONTRACT-SHA256: 4b641201e412d6014c53172ac08e409a8bce05c63211af1a7b1f45dffa8cd89f

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
        const string logPrefix = "Rest Focus Loop Start";

        CPH.SetGlobalVar(VAR_REST_FOCUS_LOOP_ACTIVE, true, false);

        // We enter pre_rest directly so loop state always reflects the timer we are about to arm.
        CPH.SetGlobalVar(VAR_REST_FOCUS_LOOP_PHASE, PHASE_PRE_REST, false);

        if (!StartTargetTimer(TIMER_PRE_REST, PRE_REST_SECONDS, logPrefix))
        {
            RecoverFromTimerStartFailure(logPrefix, PHASE_PRE_REST, TIMER_PRE_REST);
        }

        return true;
    }

    private bool StartTargetTimer(string targetTimerName, int seconds, string logPrefix)
    {
        if (seconds < 1)
            seconds = 1;

        try
        {
            // Clear every non-target timer first so a stale loop phase cannot keep running in parallel.
            DisableNonTargetTimers(targetTimerName);

            // Also reset the target timer itself before applying the new interval.
            CPH.DisableTimer(targetTimerName);

            CPH.LogWarn($"[{logPrefix}] Arming timer '{targetTimerName}' for {seconds} second(s).");
            CPH.SetTimerInterval(targetTimerName, seconds); // VERIFY: unconfirmed method signature
            CPH.EnableTimer(targetTimerName);
            return true;
        }
        catch (Exception ex)
        {
            CPH.LogError($"[{logPrefix}] Failed to arm timer '{targetTimerName}' for phase '{PHASE_PRE_REST}'. Exception: {ex}");
            return false;
        }
    }

    private void DisableNonTargetTimers(string targetTimerName)
    {
        if (!string.Equals(targetTimerName, TIMER_PRE_REST, StringComparison.Ordinal))
            CPH.DisableTimer(TIMER_PRE_REST);

        if (!string.Equals(targetTimerName, TIMER_REST, StringComparison.Ordinal))
            CPH.DisableTimer(TIMER_REST);

        if (!string.Equals(targetTimerName, TIMER_PRE_FOCUS, StringComparison.Ordinal))
            CPH.DisableTimer(TIMER_PRE_FOCUS);

        if (!string.Equals(targetTimerName, TIMER_FOCUS, StringComparison.Ordinal))
            CPH.DisableTimer(TIMER_FOCUS);
    }

    private void StopAllLoopTimers()
    {
        CPH.DisableTimer(TIMER_PRE_REST);
        CPH.DisableTimer(TIMER_REST);
        CPH.DisableTimer(TIMER_PRE_FOCUS);
        CPH.DisableTimer(TIMER_FOCUS);
    }

    private void RecoverFromTimerStartFailure(string logPrefix, string targetPhase, string targetTimerName)
    {
        // Safe recovery path: stop all loop timers and drop the active flag so the operator can restart cleanly.
        StopAllLoopTimers();
        CPH.SetGlobalVar(VAR_REST_FOCUS_LOOP_ACTIVE, false, false);

        CPH.LogError($"[{logPrefix}] Recovery triggered after failing to arm timer '{targetTimerName}' for phase '{targetPhase}'. The loop has been marked inactive and all loop timers were disabled. Verify the timer exists and that SetTimerInterval is supported in this Streamer.bot build, then restart the loop.");
    }
}
