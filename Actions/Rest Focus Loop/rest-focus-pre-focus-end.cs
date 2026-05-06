// ACTION-CONTRACT: Actions/Rest Focus Loop/AGENTS.md#rest-focus-pre-focus-end.cs
// ACTION-CONTRACT-SHA256: c6bdd7c923d5a32b1a6268bd7958155d16ac2de50dc879d3ae8e8ff8cc8f4ed8

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (Rest / Focus Loop)
    private const string VAR_REST_FOCUS_LOOP_ACTIVE = "rest_focus_loop_active";
    private const string VAR_REST_FOCUS_LOOP_PHASE = "rest_focus_loop_phase";

    private const string PHASE_PRE_FOCUS = "pre_focus";
    private const string PHASE_FOCUS = "focus";

    private const string TIMER_PRE_REST = "Rest Focus - Pre Rest";
    private const string TIMER_REST = "Rest Focus - Rest";
    private const string TIMER_PRE_FOCUS = "Rest Focus - Pre Focus";
    private const string TIMER_FOCUS = "Rest Focus - Focus";

    private const int DEFAULT_FOCUS_SECONDS = 900;

    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_CAPTAINS_FOCUS_COMMAND_ID = "REPLACE_WITH_CAPTAINS_FOCUS_COMMAND_ID";

    private static readonly HttpClient Http = new HttpClient();

    /*
     * Purpose:
     * - Handles the end of the pre-focus timer.
     * - If Captain Stretch did not run !generalfocus in time, this starts the actual focus timer with the default 15-minute duration.
     *
     * Expected trigger/input:
     * - Streamer.bot timer-end trigger for timer: Rest Focus - Pre Focus.
     *
     * Required runtime variables:
     * - Reads rest_focus_loop_active.
     * - Reads/writes rest_focus_loop_phase.
     */
    public bool Execute()
    {
        const string logPrefix = "Rest Focus Pre Focus End";

        if (!IsLoopActive())
            return true;

        string currentPhase = GetCurrentPhase();
        if (!string.Equals(currentPhase, PHASE_PRE_FOCUS, StringComparison.OrdinalIgnoreCase))
        {
            CPH.LogWarn($"[{logPrefix}] Ignoring stale timer fire because phase is '{currentPhase}'.");
            return true;
        }

        BeginFocus(DEFAULT_FOCUS_SECONDS, logPrefix);
        return true;
    }

    private void BeginFocus(int focusSeconds, string logPrefix)
    {
        if (focusSeconds < 1)
            focusSeconds = 1;

        // Update the phase before arming the next timer so any overlapping trigger sees the intended target state.
        CPH.SetGlobalVar(VAR_REST_FOCUS_LOOP_PHASE, PHASE_FOCUS, false);

        TriggerMixItUpCommand(
            MIXITUP_CAPTAINS_FOCUS_COMMAND_ID,
            logPrefix,
            arguments: focusSeconds.ToString(),
            specialIdentifiers: new { time = focusSeconds.ToString() });

        if (!StartTargetTimer(TIMER_FOCUS, focusSeconds, logPrefix, PHASE_FOCUS))
        {
            RecoverFromTimerStartFailure(logPrefix, PHASE_FOCUS, TIMER_FOCUS);
        }
    }

    private bool IsLoopActive()
    {
        return CPH.GetGlobalVar<bool?>(VAR_REST_FOCUS_LOOP_ACTIVE, false) ?? false;
    }

    private string GetCurrentPhase()
    {
        return CPH.GetGlobalVar<string>(VAR_REST_FOCUS_LOOP_PHASE, false) ?? string.Empty;
    }

    private bool StartTargetTimer(string targetTimerName, int seconds, string logPrefix, string targetPhase)
    {
        if (seconds < 1)
            seconds = 1;

        try
        {
            DisableNonTargetTimers(targetTimerName);
            CPH.DisableTimer(targetTimerName);

            CPH.LogWarn($"[{logPrefix}] Arming timer '{targetTimerName}' for phase '{targetPhase}' with duration {seconds} second(s).");
            CPH.SetTimerInterval(targetTimerName, seconds); // VERIFY: unconfirmed method signature
            CPH.EnableTimer(targetTimerName);
            return true;
        }
        catch (Exception ex)
        {
            CPH.LogError($"[{logPrefix}] Failed to arm timer '{targetTimerName}' for phase '{targetPhase}'. Exception: {ex}");
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
        StopAllLoopTimers();
        CPH.SetGlobalVar(VAR_REST_FOCUS_LOOP_ACTIVE, false, false);

        CPH.LogError($"[{logPrefix}] Recovery triggered after failing to arm timer '{targetTimerName}' for phase '{targetPhase}'. The loop has been marked inactive and all loop timers were disabled. Verify the timer exists and that SetTimerInterval is supported in this Streamer.bot build, then restart the loop.");
    }

    private bool TriggerMixItUpCommand(string commandId, string logPrefix, string arguments = "", object specialIdentifiers = null)
    {
        if (string.IsNullOrWhiteSpace(commandId) || commandId.StartsWith("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase))
        {
            CPH.LogWarn($"[{logPrefix}] Mix It Up command ID is not configured.");
            return false;
        }

        try
        {
            string url = $"{MIXITUP_API_BASE_URL.TrimEnd('/')}/api/v2/commands/{commandId}";
            string payload = JsonSerializer.Serialize(new
            {
                Platform = "Twitch",
                Arguments = arguments ?? string.Empty,
                SpecialIdentifiers = specialIdentifiers ?? new { },
                IgnoreRequirements = false
            });

            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = Http.PostAsync(url, content).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                CPH.LogWarn($"[{logPrefix}] Mix It Up call failed: {(int)response.StatusCode} {response.ReasonPhrase}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            CPH.LogError($"[{logPrefix}] Exception while calling Mix It Up: {ex}");
            return false;
        }
    }
}
