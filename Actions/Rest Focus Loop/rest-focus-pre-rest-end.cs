// ACTION-CONTRACT: Actions/Rest Focus Loop/AGENTS.md#rest-focus-pre-rest-end.cs
// ACTION-CONTRACT-SHA256: 10d69fb7b1a6037533c91c6d3726ca9ec4b2934cb7619b49088eb2401fe50454

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (Rest / Focus Loop)
    private const string VAR_REST_FOCUS_LOOP_ACTIVE = "rest_focus_loop_active";
    private const string VAR_REST_FOCUS_LOOP_PHASE = "rest_focus_loop_phase";

    private const string PHASE_PRE_REST = "pre_rest";
    private const string PHASE_REST = "rest";

    private const string TIMER_PRE_REST = "Rest Focus - Pre Rest";
    private const string TIMER_REST = "Rest Focus - Rest";
    private const string TIMER_PRE_FOCUS = "Rest Focus - Pre Focus";
    private const string TIMER_FOCUS = "Rest Focus - Focus";

    private const int DEFAULT_REST_SECONDS = 120;

    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_WIZARDS_REST_COMMAND_ID = "REPLACE_WITH_WIZARDS_REST_COMMAND_ID";

    private static readonly HttpClient Http = new HttpClient();

    /*
     * Purpose:
     * - Handles the end of the pre-rest timer.
     * - If the Water Wizard did not cast !castrest in time, this starts the actual rest timer with the default 2-minute duration.
     *
     * Expected trigger/input:
     * - Streamer.bot timer-end trigger for timer: Rest Focus - Pre Rest.
     *
     * Required runtime variables:
     * - Reads rest_focus_loop_active.
     * - Reads/writes rest_focus_loop_phase.
     *
     * Key outputs/side effects:
     * - Stops the pre-rest timer.
     * - Starts the rest timer using the default duration.
     * - Triggers the placeholder Mix It Up Wizards Rest command with time in seconds.
     */
    public bool Execute()
    {
        const string logPrefix = "Rest Focus Pre Rest End";

        if (!IsLoopActive())
            return true;

        string currentPhase = GetCurrentPhase();
        if (!string.Equals(currentPhase, PHASE_PRE_REST, StringComparison.OrdinalIgnoreCase))
        {
            CPH.LogWarn($"[{logPrefix}] Ignoring stale timer fire because phase is '{currentPhase}'.");
            return true;
        }

        BeginRest(DEFAULT_REST_SECONDS, logPrefix);
        return true;
    }

    private void BeginRest(int restSeconds, string logPrefix)
    {
        if (restSeconds < 1)
            restSeconds = 1;

        // Update the phase before arming the next timer so any overlapping trigger sees the intended target state.
        CPH.SetGlobalVar(VAR_REST_FOCUS_LOOP_PHASE, PHASE_REST, false);

        TriggerMixItUpCommand(
            MIXITUP_WIZARDS_REST_COMMAND_ID,
            logPrefix,
            arguments: restSeconds.ToString(),
            specialIdentifiers: new { time = restSeconds.ToString() });

        if (!StartTargetTimer(TIMER_REST, restSeconds, logPrefix, PHASE_REST))
        {
            RecoverFromTimerStartFailure(logPrefix, PHASE_REST, TIMER_REST);
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
