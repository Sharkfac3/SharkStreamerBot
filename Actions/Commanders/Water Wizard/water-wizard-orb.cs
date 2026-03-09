using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (Water Wizard)
    // Keep these names aligned with:
    // - Actions/Commanders/Water Wizard/water-wizard-redeem.cs
    // - Actions/Commanders/Water Wizard/water-wizard-orb.cs
    private const string ARG_USER = "user";
    private const string ARG_MESSAGE = "message";
    private const string ARG_RAW_INPUT = "rawInput";

    private const string VAR_CURRENT_WATER_WIZARD = "current_water_wizard";
    private const string VAR_WIZARD_ORB_NEXT_ALLOWED_UTC = "water_wizard_orb_next_allowed_utc";

    private const int ORB_MAX_WORD_COUNT = 30;
    private const int ORB_COOLDOWN_MINUTES = 5;

    private const string MIXITUP_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_COMMAND_ID = "REPLACE_WITH_WATER_WIZARD_ORB_COMMAND_ID";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";

    private static readonly HttpClient Http = new HttpClient();

    /*
     * Purpose:
     * - Handles Water Wizard-only !orb command.
     * - Accepts up to 30 words after !orb (message text is optional).
     * - Applies a 5-minute cooldown for the active Water Wizard.
     * - For valid wizard usage, forwards optional orb text (0-30 words) to Mix It Up as Arguments.
     */
    public bool Execute()
    {
        string caller = GetArg(ARG_USER);
        if (string.IsNullOrWhiteSpace(caller))
            return true;

        string currentWizard = CPH.GetGlobalVar<string>(VAR_CURRENT_WATER_WIZARD, false) ?? string.Empty;

        if (!IsSameUser(caller, currentWizard))
        {
            SendHailPrompt(caller, currentWizard);
            return true;
        }

        string orbText = ParseCommandText("!orb", ORB_MAX_WORD_COUNT);
        if (orbText == null)
        {
            CPH.SendMessage($"@{caller} use !orb with optional text (0-30 words max). Example: !orb or !orb your message here 🔮");
            return true;
        }

        long nowUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long nextAllowedUtc = (CPH.GetGlobalVar<long?>(VAR_WIZARD_ORB_NEXT_ALLOWED_UTC, false) ?? 0L);

        if (nowUtc < nextAllowedUtc)
        {
            long remainingSeconds = Math.Max(1L, nextAllowedUtc - nowUtc);
            int remainingMinutes = (int)Math.Ceiling(remainingSeconds / 60.0);
            CPH.SendMessage($"@{caller} your orb is recharging. Try !orb again in about {remainingMinutes} minute(s). 🔮");
            return true;
        }

        bool mixitupOk = TriggerMixItUp(orbText);
        if (!mixitupOk)
            return true;

        long newNextAllowedUtc = DateTimeOffset.UtcNow.AddMinutes(ORB_COOLDOWN_MINUTES).ToUnixTimeSeconds();
        CPH.SetGlobalVar(VAR_WIZARD_ORB_NEXT_ALLOWED_UTC, newNextAllowedUtc, false);

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
            CPH.SendMessage($"@{caller} only our current Water Wizard ({currentWizard}) can cast !orb right now. Type !hail to encourage them! 🌊");
            return;
        }

        CPH.SendMessage($"@{caller} there is no current Water Wizard right now—redeem to become the Water Wizard and unlock !orb! 🌊");
    }

    private string ParseCommandText(string commandName, int maxWords)
    {
        string input = GetArg(ARG_RAW_INPUT);
        if (string.IsNullOrWhiteSpace(input))
            input = GetArg(ARG_MESSAGE);

        // If Streamer.bot did not provide raw/message input, treat it as no orb text.
        // No text is allowed, so this is valid and returns an empty payload string.
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        string[] parts = input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return string.Empty;

        int startIndex = 0;
        if (parts[0].StartsWith(commandName, StringComparison.OrdinalIgnoreCase))
            startIndex = 1;

        int wordCount = parts.Length - startIndex;
        if (wordCount < 0 || wordCount > maxWords)
            return null;

        if (wordCount == 0)
            return string.Empty;

        return string.Join(" ", parts, startIndex, wordCount);
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
                CPH.LogWarn($"[Water Wizard Orb] Mix It Up call failed: {(int)response.StatusCode} {response.ReasonPhrase}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            CPH.LogError($"[Water Wizard Orb] Exception while calling Mix It Up: {ex}");
            return false;
        }
    }
}
