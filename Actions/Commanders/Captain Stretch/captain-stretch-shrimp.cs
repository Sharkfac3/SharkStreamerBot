using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (Captain Stretch)
    // Keep these names aligned with:
    // - Actions/Commanders/Captain Stretch/captain-stretch-redeem.cs
    // - Actions/Commanders/Captain Stretch/captain-stretch-shrimp.cs
    private const string ARG_USER = "user";
    private const string ARG_MESSAGE = "message";
    private const string ARG_RAW_INPUT = "rawInput";

    private const string VAR_CURRENT_CAPTAIN_STRETCH = "current_captain_stretch";
    private const string VAR_CAPTAIN_SHRIMP_NEXT_ALLOWED_UTC = "captain_stretch_shrimp_next_allowed_utc";

    private const int SHRIMP_MAX_WORD_COUNT = 30;
    private const int SHRIMP_COOLDOWN_MINUTES = 1;

    private const string MIXITUP_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_COMMAND_ID = "af5567d1-ac94-49bf-ad7b-0b7e034cb05d";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";

    private static readonly HttpClient Http = new HttpClient();

    /*
     * Purpose:
     * - Handles Captain Stretch-only !shrimp command.
     * - Allows optional shrimp text after !shrimp (30 words max).
     * - Applies a 5-minute cooldown for the active Captain Stretch.
     * - For valid captain usage, forwards the shrimp text (up to 30 words) to Mix It Up as Arguments.
     */
    public bool Execute()
    {
        string caller = GetArg(ARG_USER);
        if (string.IsNullOrWhiteSpace(caller))
            return true;

        string currentCaptain = CPH.GetGlobalVar<string>(VAR_CURRENT_CAPTAIN_STRETCH, false) ?? string.Empty;

        if (!IsSameUser(caller, currentCaptain))
        {
            SendThankPrompt(caller, currentCaptain);
            return true;
        }

        string shrimpText = ParseCommandText("!shrimp", SHRIMP_MAX_WORD_COUNT);
        if (shrimpText == null)
        {
            CPH.SendMessage($"@{caller} use !shrimp with optional text (0-30 words max). Example: !shrimp or !shrimp your message here 🍤");
            return true;
        }

        long nowUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long nextAllowedUtc = (CPH.GetGlobalVar<long?>(VAR_CAPTAIN_SHRIMP_NEXT_ALLOWED_UTC, false) ?? 0L);

        if (nowUtc < nextAllowedUtc)
        {
            long remainingSeconds = Math.Max(1L, nextAllowedUtc - nowUtc);
            int remainingMinutes = (int)Math.Ceiling(remainingSeconds / 60.0);
            CPH.SendMessage($"@{caller} your shrimp signal is cooling down. Try !shrimp again in about {remainingMinutes} minute(s). 🍤");
            return true;
        }

        bool mixitupOk = TriggerMixItUp(shrimpText);
        if (!mixitupOk)
            return true;

        long newNextAllowedUtc = DateTimeOffset.UtcNow.AddMinutes(SHRIMP_COOLDOWN_MINUTES).ToUnixTimeSeconds();
        CPH.SetGlobalVar(VAR_CAPTAIN_SHRIMP_NEXT_ALLOWED_UTC, newNextAllowedUtc, false);

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
            CPH.SendMessage($"@{caller} only our current Captain Stretch ({currentCaptain}) can use !shrimp right now. Type !thank to cheer them on! 💪");
            return;
        }

        CPH.SendMessage($"@{caller} there is no current Captain Stretch right now—redeem to become Captain Stretch and unlock !shrimp! 💪");
    }

    private string ParseCommandText(string commandName, int maxWords)
    {
        string input = GetArg(ARG_RAW_INPUT);
        if (string.IsNullOrWhiteSpace(input))
            input = GetArg(ARG_MESSAGE);

        // If Streamer.bot did not provide raw/message input, treat it as no shrimp text.
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
                CPH.LogWarn($"[Captain Stretch Shrimp] Mix It Up call failed: {(int)response.StatusCode} {response.ReasonPhrase}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            CPH.LogError($"[Captain Stretch Shrimp] Exception while calling Mix It Up: {ex}");
            return false;
        }
    }
}
