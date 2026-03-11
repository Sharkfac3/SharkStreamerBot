using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (Water Wizard)
    // Keep these names aligned with:
    // - Actions/Commanders/Water Wizard/water-wizard-redeem.cs
    // - Actions/Commanders/Water Wizard/wizard-hydrate.cs
    private const string ARG_USER = "user";
    private const string ARG_INPUT0 = "input0";
    private const string ARG_MESSAGE = "message";
    private const string ARG_RAW_INPUT = "rawInput";

    private const string VAR_CURRENT_WATER_WIZARD = "current_water_wizard";
    private const string VAR_WIZARD_HYDRATE_NEXT_ALLOWED_UTC = "water_wizard_hydrate_next_allowed_utc";

    private const int HYDRATE_MIN_VALUE = 1;
    private const int HYDRATE_MAX_VALUE = 10;
    private const int HYDRATE_COOLDOWN_MINUTES = 5;

    private const string MIXITUP_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_COMMAND_ID = "248ea7cf-e9c9-4fd1-add7-4b37e63dc805";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";

    // Reuse one HttpClient instance for reliability/performance.
    private static readonly HttpClient Http = new HttpClient();

    /*
     * Purpose:
     * - Handles the Water Wizard-only !hydrate X command (X must be 1..10).
     * - Applies a 5-minute cooldown for the active Water Wizard.
     * - For valid wizard usage, forwards X to Mix It Up command API as Arguments.
     *
     * Expected trigger/input:
     * - Chat command/action for !hydrate.
     * - Reads: user, input0 (fallback parse from rawInput/message).
     *
     * Required runtime variables:
     * - Reads current_water_wizard
     * - Reads/Writes water_wizard_hydrate_next_allowed_utc (Unix seconds, UTC)
     *
     * Key outputs/side effects:
     * - Non-wizard caller: sends encouragement chat instruction to use !hail.
     * - Wizard caller on cooldown: sends remaining cooldown message.
     * - Wizard caller with valid X: triggers Mix It Up command with X as message text.
     */
    public bool Execute()
    {
        string caller = GetArg(ARG_USER);
        if (string.IsNullOrWhiteSpace(caller))
            return true;

        string currentWizard = CPH.GetGlobalVar<string>(VAR_CURRENT_WATER_WIZARD, false) ?? string.Empty;

        // Only the currently assigned Water Wizard can use !hydrate.
        if (!IsSameUser(caller, currentWizard))
        {
            SendHailPrompt(caller, currentWizard);
            return true;
        }

        // Parse hydrate amount. Must be integer 1..10.
        int hydrateAmount = ParseHydrateAmount();
        if (hydrateAmount < HYDRATE_MIN_VALUE || hydrateAmount > HYDRATE_MAX_VALUE)
        {
            CPH.SendMessage($"@{caller} use !hydrate <1-10> (example: !hydrate 7). 💧");
            return true;
        }

        long nowUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long nextAllowedUtc = (CPH.GetGlobalVar<long?>(VAR_WIZARD_HYDRATE_NEXT_ALLOWED_UTC, false) ?? 0L);

        // Cooldown guard for active wizard.
        if (nowUtc < nextAllowedUtc)
        {
            long remainingSeconds = Math.Max(1L, nextAllowedUtc - nowUtc);
            int remainingMinutes = (int)Math.Ceiling(remainingSeconds / 60.0);
            CPH.SendMessage($"@{caller} your water magic is recharging. Try !hydrate again in about {remainingMinutes} minute(s). 💧");
            return true;
        }

        // Trigger Mix It Up with X as Arguments/message text.
        bool mixitupOk = TriggerMixItUp(hydrateAmount.ToString());
        if (!mixitupOk)
        {
            // Do not start cooldown on failed external call.
            return true;
        }

        // Start new cooldown only after successful trigger.
        long newNextAllowedUtc = DateTimeOffset.UtcNow.AddMinutes(HYDRATE_COOLDOWN_MINUTES).ToUnixTimeSeconds();
        CPH.SetGlobalVar(VAR_WIZARD_HYDRATE_NEXT_ALLOWED_UTC, newNextAllowedUtc, false);

        return true;
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

    private void SendHailPrompt(string caller, string currentWizard)
    {
        if (!string.IsNullOrWhiteSpace(currentWizard))
        {
            CPH.SendMessage($"@{caller} only our current Water Wizard ({currentWizard}) can cast !hydrate right now. Type !hail to encourage them! 🌊");
            return;
        }

        CPH.SendMessage($"@{caller} there is no current Water Wizard right now—redeem to become the Water Wizard and unlock !hydrate! 🌊");
    }

    private int ParseHydrateAmount()
    {
        // Preferred path: first command argument (input0).
        string input0 = GetArg(ARG_INPUT0);
        if (int.TryParse(input0, out int parsed))
            return parsed;

        // Fallback path: parse first integer from raw input or full message.
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

    private bool TriggerMixItUp(string argumentText)
    {
        try
        {
            string url = $"{MIXITUP_BASE_URL.TrimEnd('/')}/api/v2/commands/{MIXITUP_COMMAND_ID}";

            string payload = JsonSerializer.Serialize(new
            {
                Platform = MIXITUP_PLATFORM_TWITCH,
                Arguments = argumentText,
                IgnoreRequirements = false
            });

            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = Http.PostAsync(url, content).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                CPH.LogWarn($"[Water Wizard Hydrate] Mix It Up call failed: {(int)response.StatusCode} {response.ReasonPhrase}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            CPH.LogError($"[Water Wizard Hydrate] Exception while calling Mix It Up: {ex}");
            return false;
        }
    }
}
