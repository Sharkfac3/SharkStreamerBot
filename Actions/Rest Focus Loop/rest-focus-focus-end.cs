using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (Rest / Focus Loop)
    private const string VAR_REST_FOCUS_LOOP_ACTIVE = "rest_focus_loop_active";
    private const string VAR_REST_FOCUS_LOOP_PHASE = "rest_focus_loop_phase";

    private const string PHASE_FOCUS = "focus";
    private const string PHASE_PRE_REST = "pre_rest";

    private const string TIMER_FOCUS = "Rest Focus - Focus";
    private const string TIMER_PRE_REST = "Rest Focus - Pre Rest";

    private const int PRE_REST_SECONDS = 120;

    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_FOCUS_TIMER_END_COMMAND_ID = "REPLACE_WITH_FOCUS_TIMER_END_COMMAND_ID";

    private static readonly HttpClient Http = new HttpClient();

    /*
     * Purpose:
     * - Handles the end of the active focus timer.
     * - Fires the Focus Timer End Mix It Up command, then loops back to the pre-rest window.
     *
     * Expected trigger/input:
     * - Streamer.bot timer-end trigger for timer: Rest Focus - Focus.
     */
    public bool Execute()
    {
        if (!IsLoopActive())
            return true;

        string currentPhase = GetCurrentPhase();
        if (!string.Equals(currentPhase, PHASE_FOCUS, StringComparison.OrdinalIgnoreCase))
        {
            CPH.LogWarn($"[Rest Focus Focus End] Ignoring stale timer fire because phase is '{currentPhase}'.");
            return true;
        }

        CPH.DisableTimer(TIMER_FOCUS);
        TriggerMixItUpCommand(MIXITUP_FOCUS_TIMER_END_COMMAND_ID, "Rest Focus Focus End");

        StartTimer(TIMER_PRE_REST, PRE_REST_SECONDS, "Rest Focus Focus End");
        CPH.SetGlobalVar(VAR_REST_FOCUS_LOOP_PHASE, PHASE_PRE_REST, false);
        return true;
    }

    private bool IsLoopActive()
    {
        return CPH.GetGlobalVar<bool?>(VAR_REST_FOCUS_LOOP_ACTIVE, false) ?? false;
    }

    private string GetCurrentPhase()
    {
        return CPH.GetGlobalVar<string>(VAR_REST_FOCUS_LOOP_PHASE, false) ?? string.Empty;
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
