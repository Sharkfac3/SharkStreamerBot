// ACTION-CONTRACT: Actions/Commanders/Captain Stretch/AGENTS.md#captain-stretch-stretch.cs
// ACTION-CONTRACT-SHA256: a8e672edb304f06c13b5d92e7a7546cf6605e53b07a9a8e75164051bd65c6b4a

// Documented Mix It Up endpoint literal: http://localhost:8911/api/v2/commands/{commandId}
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // Runtime source of truth: Actions/Commanders/Captain Stretch/README.md
    // Shared names/constants reference: Actions/SHARED-CONSTANTS.md
    private const string ARG_USER = "user";
    private const string ARG_MESSAGE = "message";
    private const string ARG_RAW_INPUT = "rawInput";

    private const string VAR_CURRENT_CAPTAIN_STRETCH = "current_captain_stretch";
    private const string VAR_CAPTAIN_STRETCH_NEXT_ALLOWED_UTC = "captain_stretch_stretch_next_allowed_utc";

    private const int STRETCH_MAX_WORD_COUNT = 10;
    private const int STRETCH_MAX_CHAR_COUNT = 40;
    private const int STRETCH_COOLDOWN_MINUTES = 5;

    private const string MIXITUP_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_COMMAND_ID = "60b43da9-accb-4dbe-968a-d57846a7dc4c";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";

    // Reuse one HttpClient instance for reliability/performance.
    private static readonly HttpClient Http = new HttpClient();

    public bool Execute()
    {
        string caller = GetArg(ARG_USER);
        if (string.IsNullOrWhiteSpace(caller))
            return true;

        string currentCaptain = CPH.GetGlobalVar<string>(VAR_CURRENT_CAPTAIN_STRETCH, false) ?? string.Empty;

        // Commander-only gate.
        if (!IsSameUser(caller, currentCaptain))
        {
            SendThankPrompt(caller, currentCaptain);
            return true;
        }

        // Validate optional command text before cooldown or external call.
        if (!TryParseStretchText(out string stretchText))
        {
            CPH.SendMessage($"@{caller} keep !stretch to {STRETCH_MAX_WORD_COUNT} words and {STRETCH_MAX_CHAR_COUNT} characters max (or no extra words at all). Example: !stretch shoulders up breathe 💪");
            return true;
        }

        long nowUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long nextAllowedUtc = (CPH.GetGlobalVar<long?>(VAR_CAPTAIN_STRETCH_NEXT_ALLOWED_UTC, false) ?? 0L);

        // Cooldown guard.
        if (nowUtc < nextAllowedUtc)
        {
            long remainingSeconds = Math.Max(1L, nextAllowedUtc - nowUtc);
            int remainingMinutes = (int)Math.Ceiling(remainingSeconds / 60.0);
            CPH.SendMessage($"@{caller} your stretching routine is still cooling down. Try !stretch again in about {remainingMinutes} minute(s). 💪");
            return true;
        }

        // Trigger Mix It Up with validated optional text.
        bool mixitupOk = TriggerMixItUp(stretchText);
        if (!mixitupOk)
        {
            // Do not start cooldown on failed external call.
            return true;
        }

        // Start cooldown only after successful trigger.
        long newNextAllowedUtc = DateTimeOffset.UtcNow.AddMinutes(STRETCH_COOLDOWN_MINUTES).ToUnixTimeSeconds();
        CPH.SetGlobalVar(VAR_CAPTAIN_STRETCH_NEXT_ALLOWED_UTC, newNextAllowedUtc, false);

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

    private void SendThankPrompt(string caller, string currentCaptain)
    {
        if (!string.IsNullOrWhiteSpace(currentCaptain))
        {
            CPH.SendMessage($"@{caller} only our current Captain Stretch ({currentCaptain}) can use !stretch right now. Type !thank to cheer them on! 💪");
            return;
        }

        CPH.SendMessage($"@{caller} there is no current Captain Stretch right now—redeem to become Captain Stretch and unlock !stretch! 💪");
    }

    private bool TryParseStretchText(out string stretchText)
    {
        stretchText = string.Empty;

        // Prefer command input, but tolerate full chat message input.
        string input = GetArg(ARG_RAW_INPUT);
        if (string.IsNullOrWhiteSpace(input))
            input = GetArg(ARG_MESSAGE);

        // Plain !stretch is valid.
        if (string.IsNullOrWhiteSpace(input))
            return true;

        string[] parts = input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return true;

        // If trigger passed the full chat message, drop command token.
        int startIndex = 0;
        if (parts[0].StartsWith("!stretch", StringComparison.OrdinalIgnoreCase))
            startIndex = 1;

        int wordCount = parts.Length - startIndex;
        if (wordCount <= 0)
            return true;

        if (wordCount > STRETCH_MAX_WORD_COUNT)
            return false;

        stretchText = string.Join(" ", parts, startIndex, wordCount);

        // Keep Mix It Up/TTS payload bounded.
        if (stretchText.Length > STRETCH_MAX_CHAR_COUNT)
            return false;

        return true;
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
                CPH.LogWarn($"[Captain Stretch] Mix It Up call failed: {(int)response.StatusCode} {response.ReasonPhrase}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            CPH.LogError($"[Captain Stretch] Exception while calling Mix It Up: {ex}");
            return false;
        }
    }
}
