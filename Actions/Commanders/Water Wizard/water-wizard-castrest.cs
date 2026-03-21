using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (Water Wizard + Rest / Focus Loop)
    // Keep these names aligned with:
    // - Actions/Commanders/Water Wizard/water-wizard-redeem.cs
    // - Actions/Commanders/Water Wizard/water-wizard-castrest.cs
    // - Actions/Rest Focus Loop/*.cs
    private const string ARG_USER = "user";
    private const string ARG_INPUT0 = "input0";
    private const string ARG_MESSAGE = "message";
    private const string ARG_RAW_INPUT = "rawInput";

    private const string VAR_CURRENT_WATER_WIZARD = "current_water_wizard";
    private const string VAR_REST_FOCUS_LOOP_ACTIVE = "rest_focus_loop_active";
    private const string VAR_REST_FOCUS_LOOP_PHASE = "rest_focus_loop_phase";

    private const string PHASE_PRE_REST = "pre_rest";
    private const string PHASE_REST = "rest";

    private const string TIMER_PRE_REST = "Rest Focus - Pre Rest";
    private const string TIMER_REST = "Rest Focus - Rest";

    private const int MIN_ALLOWED_MINUTES = 1;
    private const int MAX_ALLOWED_MINUTES = 999;

    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_WIZARDS_REST_COMMAND_ID = "REPLACE_WITH_WIZARDS_REST_COMMAND_ID";

    private static readonly HttpClient Http = new HttpClient();

    /*
     * Purpose:
     * - Handles the Water Wizard-only !castrest X command during the pre-rest window.
     * - Ends the pre-rest timer early, starts the actual rest timer, and triggers the placeholder Mix It Up Wizards Rest command.
     *
     * Expected trigger/input:
     * - Chat command/action for !castrest.
     * - Reads user and rest minutes from input0 (fallback rawInput/message parsing).
     *
     * Required runtime variables:
     * - Reads current_water_wizard.
     * - Reads rest_focus_loop_active.
     * - Reads/writes rest_focus_loop_phase.
     *
     * Operator notes:
     * - This script assumes !castrest is wired as a chat command/action in Streamer.bot.
     * - This script uses CPH.SetTimerInterval(string, int). That method signature is still marked VERIFY in project docs.
     */
    public bool Execute()
    {
        string caller = GetArg(ARG_USER);
        if (string.IsNullOrWhiteSpace(caller))
            return true;

        string currentWizard = CPH.GetGlobalVar<string>(VAR_CURRENT_WATER_WIZARD, false) ?? string.Empty;
        if (!IsSameUser(caller, currentWizard))
        {
            SendWizardPrompt(caller, currentWizard);
            return true;
        }

        if (!(CPH.GetGlobalVar<bool?>(VAR_REST_FOCUS_LOOP_ACTIVE, false) ?? false))
        {
            CPH.SendMessage($"@{caller} the crew is not in a rest/focus loop right now.");
            return true;
        }

        string currentPhase = CPH.GetGlobalVar<string>(VAR_REST_FOCUS_LOOP_PHASE, false) ?? string.Empty;
        if (!string.Equals(currentPhase, PHASE_PRE_REST, StringComparison.OrdinalIgnoreCase))
        {
            CPH.SendMessage($"@{caller} the rest window has already moved on.");
            return true;
        }

        int requestedMinutes = ParseMinutes();
        if (requestedMinutes < MIN_ALLOWED_MINUTES || requestedMinutes > MAX_ALLOWED_MINUTES)
        {
            CPH.SendMessage($"@{caller} use !castrest <minutes> with a whole number from {MIN_ALLOWED_MINUTES} to {MAX_ALLOWED_MINUTES}.");
            return true;
        }

        int restSeconds = requestedMinutes * 60;
        BeginRest(restSeconds, "Water Wizard Cast Rest");
        return true;
    }

    private void BeginRest(int restSeconds, string logPrefix)
    {
        if (restSeconds < 1)
            restSeconds = 1;

        CPH.DisableTimer(TIMER_PRE_REST);
        TriggerMixItUpCommand(
            MIXITUP_WIZARDS_REST_COMMAND_ID,
            logPrefix,
            arguments: restSeconds.ToString(),
            specialIdentifiers: new { time = restSeconds.ToString() });

        StartTimer(TIMER_REST, restSeconds, logPrefix);
        CPH.SetGlobalVar(VAR_REST_FOCUS_LOOP_PHASE, PHASE_REST, false);
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

    private void SendWizardPrompt(string caller, string currentWizard)
    {
        if (!string.IsNullOrWhiteSpace(currentWizard))
        {
            CPH.SendMessage($"@{caller} only our current Water Wizard ({currentWizard}) can cast the rest window right now. Type !hail if you want to back the wizard. 🌊");
            return;
        }

        CPH.SendMessage($"@{caller} there is no current Water Wizard right now, so the ship will fall back to the default rest time. 🌊");
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
