// ACTION-CONTRACT: Actions/Commanders/Water Wizard/AGENTS.md#water-wizard-orb.cs
// ACTION-CONTRACT-SHA256: 573cfd8b7fad7c00829ffa704cadee6f75adc82a68f57a0845a9520b9c4b070d

// Documented Mix It Up endpoint literal: http://localhost:8911/api/v2/commands/{commandId}
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // Runtime source of truth: Actions/Commanders/Water Wizard/README.md
    // Shared names/constants reference: Actions/SHARED-CONSTANTS.md
    private const string ARG_USER = "user";
    private const string ARG_MESSAGE = "message";
    private const string ARG_RAW_INPUT = "rawInput";

    private const string VAR_CURRENT_WATER_WIZARD = "current_water_wizard";
    private const string VAR_WIZARD_ORB_NEXT_ALLOWED_UTC = "water_wizard_orb_next_allowed_utc";

    private const int ORB_MAX_WORD_COUNT = 30;
    private const int ORB_COOLDOWN_MINUTES = 1;

    private const string MIXITUP_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_COMMAND_ID = "6b00a684-8fd4-404c-81b0-c279f241af73";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";
    private const string MIXITUP_TYPE_NONE = "none";
    private const string MIXITUP_TYPE_MESSAGE = "message";
    private const string MIXITUP_TYPE_SPECIAL = "special";
    private const string SPECIAL_ORB_PHRASE_BOW_TO_ME = "bowtome";

    private static readonly HttpClient Http = new HttpClient();

    private sealed class OrbRequest
    {
        public string PayloadText { get; set; }
        public string PayloadType { get; set; }
    }

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

        OrbRequest orbRequest = ParseOrbRequest();
        if (orbRequest == null)
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

        bool mixitupOk = TriggerMixItUp(orbRequest);
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

    private OrbRequest ParseOrbRequest()
    {
        string orbText = ParseCommandText("!orb", ORB_MAX_WORD_COUNT);
        if (orbText == null)
            return null;

        string orbType = ResolveOrbType(orbText);
        return new OrbRequest
        {
            PayloadText = orbText,
            PayloadType = orbType
        };
    }

    private string ParseCommandText(string commandName, int maxWords)
    {
        string input = GetArg(ARG_RAW_INPUT);
        if (string.IsNullOrWhiteSpace(input))
            input = GetArg(ARG_MESSAGE);

        // Missing Streamer.bot input is valid: !orb can send an empty payload.
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

        return string.Join(" ", parts, startIndex, wordCount).Trim();
    }

    private string ResolveOrbType(string orbText)
    {
        if (string.IsNullOrWhiteSpace(orbText))
            return MIXITUP_TYPE_NONE;

        if (string.Equals(orbText.Trim(), SPECIAL_ORB_PHRASE_BOW_TO_ME, StringComparison.OrdinalIgnoreCase))
            return MIXITUP_TYPE_SPECIAL;

        return MIXITUP_TYPE_MESSAGE;
    }

    private bool TriggerMixItUp(OrbRequest orbRequest)
    {
        try
        {
            string url = $"{MIXITUP_BASE_URL.TrimEnd('/')}/api/v2/commands/{MIXITUP_COMMAND_ID}";

            string payloadText = orbRequest?.PayloadText ?? string.Empty;
            string payloadType = orbRequest?.PayloadType ?? MIXITUP_TYPE_NONE;

            string payload = JsonSerializer.Serialize(new
            {
                Platform = MIXITUP_PLATFORM_TWITCH,
                Arguments = payloadText,
                SpecialIdentifiers = new
                {
                    orbmessage = payloadText,
                    orbtype = payloadType
                },
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
