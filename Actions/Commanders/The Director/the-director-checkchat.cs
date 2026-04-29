using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // Runtime source of truth: Actions/Commanders/The Director/README.md
    // Shared names/constants reference: Actions/SHARED-CONSTANTS.md
    private const string ARG_USER = "user";
    private const string ARG_MESSAGE = "message";
    private const string ARG_RAW_INPUT = "rawInput";

    private const string VAR_CURRENT_THE_DIRECTOR = "current_the_director";
    private const string VAR_DIRECTOR_CHECKCHAT_NEXT_ALLOWED_UTC = "the_director_checkchat_next_allowed_utc";

    private const int CHECKCHAT_MAX_WORD_COUNT = 20;
    private const int CHECKCHAT_MAX_CHAR_COUNT = 40;
    private const int CHECKCHAT_COOLDOWN_MINUTES = 1;

    private const string MIXITUP_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_COMMAND_ID = "06e3851f-81a2-40cb-a911-33c5ec04a3f2";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";

    // Reuse one HttpClient instance for reliability/performance.
    private static readonly HttpClient Http = new HttpClient();

    public bool Execute()
    {
        string caller = GetArg(ARG_USER);
        if (string.IsNullOrWhiteSpace(caller))
            return true;

        string currentDirector = CPH.GetGlobalVar<string>(VAR_CURRENT_THE_DIRECTOR, false) ?? string.Empty;

        if (!IsSameUser(caller, currentDirector))
        {
            SendAwardPrompt(caller, currentDirector);
            return true;
        }

        bool tooManyWords;
        string checkchatText;
        TryParseCheckchatText(out checkchatText, out tooManyWords);

        if (tooManyWords)
        {
            CPH.SendMessage($"@{caller} !checkchat can include up to {CHECKCHAT_MAX_WORD_COUNT} words and {CHECKCHAT_MAX_CHAR_COUNT} characters (message is optional). Please shorten and try again. 🎬");
            return true;
        }

        long nowUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long nextAllowedUtc = (CPH.GetGlobalVar<long?>(VAR_DIRECTOR_CHECKCHAT_NEXT_ALLOWED_UTC, false) ?? 0L);

        if (nowUtc < nextAllowedUtc)
        {
            long remainingSeconds = Math.Max(1L, nextAllowedUtc - nowUtc);
            int remainingMinutes = (int)Math.Ceiling(remainingSeconds / 60.0);
            CPH.SendMessage($"@{caller} your direction cooldown is active. Try !checkchat again in about {remainingMinutes} minute(s). 🎬");
            return true;
        }

        bool mixitupOk = TriggerMixItUp(checkchatText);
        if (!mixitupOk)
        {
            // Preserve retry behavior when the external call fails.
            return true;
        }

        long newNextAllowedUtc = DateTimeOffset.UtcNow.AddMinutes(CHECKCHAT_COOLDOWN_MINUTES).ToUnixTimeSeconds();
        CPH.SetGlobalVar(VAR_DIRECTOR_CHECKCHAT_NEXT_ALLOWED_UTC, newNextAllowedUtc, false);

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

    private void SendAwardPrompt(string caller, string currentDirector)
    {
        if (!string.IsNullOrWhiteSpace(currentDirector))
        {
            CPH.SendMessage($"@{caller} only our current Director ({currentDirector}) can use !checkchat right now. Type !award to support them! 🎬");
            return;
        }

        CPH.SendMessage($"@{caller} there is no current Director right now—redeem to become The Director and unlock !checkchat! 🎬");
    }

    private void TryParseCheckchatText(out string text, out bool tooManyWords)
    {
        text = string.Empty;
        tooManyWords = false;

        string input = GetArg(ARG_RAW_INPUT);
        if (string.IsNullOrWhiteSpace(input))
            input = GetArg(ARG_MESSAGE);

        if (string.IsNullOrWhiteSpace(input))
            return;

        string[] parts = input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return;

        // Some triggers pass the full chat message instead of only command args.
        int startIndex = 0;
        if (parts[0].StartsWith("!checkchat", StringComparison.OrdinalIgnoreCase))
            startIndex = 1;

        int wordCount = parts.Length - startIndex;
        if (wordCount <= 0)
            return;

        if (wordCount > CHECKCHAT_MAX_WORD_COUNT)
        {
            tooManyWords = true;
            return;
        }

        text = string.Join(" ", parts, startIndex, wordCount);

        if (text.Length > CHECKCHAT_MAX_CHAR_COUNT)
        {
            tooManyWords = true;
            text = string.Empty;
        }
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
                CPH.LogWarn($"[The Director] Mix It Up call failed: {(int)response.StatusCode} {response.ReasonPhrase}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            CPH.LogError($"[The Director] Exception while calling Mix It Up: {ex}");
            return false;
        }
    }
}
