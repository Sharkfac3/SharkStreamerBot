// ACTION-CONTRACT: Actions/Commanders/Captain Stretch/AGENTS.md#captain-stretch-generalfocus.cs
// ACTION-CONTRACT-SHA256: 6ba2dc2b41555af902d3af88d9978c58e68519f6ee55b92a16e0a3a9396ab678

// Documented Mix It Up endpoint literal: http://localhost:8911/api/v2/commands/{commandId}
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (Captain Stretch + Rest / Focus Loop)
    // Keep these names aligned with:
    // - Actions/Commanders/Captain Stretch/captain-stretch-redeem.cs
    // - Actions/Commanders/Captain Stretch/captain-stretch-generalfocus.cs
    // - Actions/Rest Focus Loop/*.cs
    private const string ARG_USER = "user";
    private const string ARG_INPUT0 = "input0";
    private const string ARG_MESSAGE = "message";
    private const string ARG_RAW_INPUT = "rawInput";

    private const string VAR_CURRENT_CAPTAIN_STRETCH = "current_captain_stretch";
    private const string VAR_REST_FOCUS_LOOP_ACTIVE = "rest_focus_loop_active";
    private const string VAR_REST_FOCUS_LOOP_PHASE = "rest_focus_loop_phase";

    private const string PHASE_PRE_FOCUS = "pre_focus";
    private const string PHASE_FOCUS = "focus";

    private const string TIMER_PRE_REST = "Rest Focus - Pre Rest";
    private const string TIMER_REST = "Rest Focus - Rest";
    private const string TIMER_PRE_FOCUS = "Rest Focus - Pre Focus";
    private const string TIMER_FOCUS = "Rest Focus - Focus";

    private const int MIN_ALLOWED_MINUTES = 1;
    private const int MAX_ALLOWED_MINUTES = 999;

    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_CAPTAINS_FOCUS_COMMAND_ID = "REPLACE_WITH_CAPTAINS_FOCUS_COMMAND_ID";

    private static readonly HttpClient Http = new HttpClient();

    /*
     * Purpose:
     * - Handles the Captain Stretch-only !generalfocus X command during the pre-focus window.
     * - Ends the pre-focus timer early, starts the actual focus timer, and triggers the placeholder Mix It Up Captain's Focus command.
     *
     * Expected trigger/input:
     * - Chat command/action for !generalfocus.
     * - Reads user and focus minutes from input0 (fallback rawInput/message parsing).
     */
    public bool Execute()
    {
        string caller = GetArg(ARG_USER);
        if (string.IsNullOrWhiteSpace(caller))
            return true;

        string currentCaptain = CPH.GetGlobalVar<string>(VAR_CURRENT_CAPTAIN_STRETCH, false) ?? string.Empty;
        if (!IsSameUser(caller, currentCaptain))
        {
            SendCaptainPrompt(caller, currentCaptain);
            return true;
        }

        if (!(CPH.GetGlobalVar<bool?>(VAR_REST_FOCUS_LOOP_ACTIVE, false) ?? false))
        {
            CPH.SendMessage($"@{caller} the crew is not in a rest/focus loop right now.");
            return true;
        }

        string currentPhase = CPH.GetGlobalVar<string>(VAR_REST_FOCUS_LOOP_PHASE, false) ?? string.Empty;
        if (!string.Equals(currentPhase, PHASE_PRE_FOCUS, StringComparison.OrdinalIgnoreCase))
        {
            CPH.SendMessage($"@{caller} the focus window is not open right now.");
            return true;
        }

        int requestedMinutes = ParseMinutes();
        if (requestedMinutes < MIN_ALLOWED_MINUTES || requestedMinutes > MAX_ALLOWED_MINUTES)
        {
            CPH.SendMessage($"@{caller} use !generalfocus <minutes> with a whole number from {MIN_ALLOWED_MINUTES} to {MAX_ALLOWED_MINUTES}.");
            return true;
        }

        int focusSeconds = requestedMinutes * 60;
        BeginFocus(focusSeconds, "Captain Stretch General Focus");
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

    private string GetArg(string key)
    {
        if (CPH.TryGetArg(key, out string value) && !string.IsNullOrWhiteSpace(value))
            return value.Trim();

        return string.Empty;
    }

    private bool IsSameUser(string a, string b)
    {
        return !string.IsNullOrWhiteSpace(a)
            && !string.IsNullOrWhiteSpace(b)
            && string.Equals(a.Trim(), b.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private void SendCaptainPrompt(string caller, string currentCaptain)
    {
        if (!string.IsNullOrWhiteSpace(currentCaptain))
        {
            CPH.SendMessage($"@{caller} only our current Captain Stretch ({currentCaptain}) can call the focus window right now. Type !thank if you want to back the captain. 💪");
            return;
        }

        CPH.SendMessage($"@{caller} there is no current Captain Stretch right now, so the ship will fall back to the default focus time. 💪");
    }

    private int ParseMinutes()
    {
        string input0 = GetArg(ARG_INPUT0);
        if (int.TryParse(input0, out int parsed))
            return parsed;

        string rawInput = GetArg(ARG_RAW_INPUT);
        parsed = ExtractFirstInt(rawInput);
        if (parsed != 0)
            return parsed;

        string message = GetArg(ARG_MESSAGE);
        return ExtractFirstInt(message);
    }

    private int ExtractFirstInt(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        string[] parts = text.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string part in parts)
        {
            if (int.TryParse(part.Trim(), out int value))
                return value;
        }

        return 0;
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
