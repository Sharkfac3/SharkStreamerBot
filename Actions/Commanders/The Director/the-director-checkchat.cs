using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (The Director)
    // Keep these names aligned with:
    // - Actions/Commanders/The Director/the-director-redeem.cs
    // - Actions/Commanders/The Director/the-director-checkchat.cs
    private const string ARG_USER = "user";
    private const string ARG_MESSAGE = "message";
    private const string ARG_RAW_INPUT = "rawInput";

    private const string VAR_CURRENT_THE_DIRECTOR = "current_the_director";
    private const string VAR_DIRECTOR_CHECKCHAT_NEXT_ALLOWED_UTC = "the_director_checkchat_next_allowed_utc";

    private const int CHECKCHAT_MAX_WORD_COUNT = 20;
    private const int CHECKCHAT_COOLDOWN_MINUTES = 1;

    private const string MIXITUP_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_COMMAND_ID = "231c06fc-30f7-4891-974c-41db9b12c68e";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";

    // Reuse one HttpClient instance for reliability/performance.
    private static readonly HttpClient Http = new HttpClient();

    /*
     * Purpose:
     * - Handles The Director-only !checkchat command.
     * - Allows optional text after !checkchat (0 to 10 words max).
     * - Applies a 5-minute cooldown for the active Director.
     * - For valid Director usage, forwards the optional text to Mix It Up API as Arguments.
     *
     * Expected trigger/input:
     * - Chat command/action for !checkchat.
     * - Reads: user, rawInput (fallback message).
     *
     * Required runtime variables:
     * - Reads current_the_director
     * - Reads/Writes the_director_checkchat_next_allowed_utc (Unix seconds, UTC)
     *
     * Key outputs/side effects:
     * - Non-director caller with active director: sends instruction to type !award.
     * - Non-director caller with no active director: encourages redeem to become The Director.
     * - Director caller on cooldown: sends remaining cooldown message.
     * - Director caller with valid optional text (up to 10 words): triggers Mix It Up command.
     */
    public bool Execute()
    {
        string caller = GetArg(ARG_USER);
        if (string.IsNullOrWhiteSpace(caller))
            return true;

        string currentDirector = CPH.GetGlobalVar<string>(VAR_CURRENT_THE_DIRECTOR, false) ?? string.Empty;

        // Only the currently assigned The Director can use !checkchat.
        if (!IsSameUser(caller, currentDirector))
        {
            SendAwardPrompt(caller, currentDirector);
            return true;
        }

        // Parse optional checkchat text (allowed: 0 through CHECKCHAT_MAX_WORD_COUNT words).
        bool tooManyWords;
        string checkchatText;
        TryParseCheckchatText(out checkchatText, out tooManyWords);

        if (tooManyWords)
        {
            CPH.SendMessage($"@{caller} !checkchat can include up to {CHECKCHAT_MAX_WORD_COUNT} words (message text is optional). Please shorten your message and try again. 🎬");
            return true;
        }

        long nowUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long nextAllowedUtc = (CPH.GetGlobalVar<long?>(VAR_DIRECTOR_CHECKCHAT_NEXT_ALLOWED_UTC, false) ?? 0L);

        // Cooldown guard for active director.
        if (nowUtc < nextAllowedUtc)
        {
            long remainingSeconds = Math.Max(1L, nextAllowedUtc - nowUtc);
            int remainingMinutes = (int)Math.Ceiling(remainingSeconds / 60.0);
            CPH.SendMessage($"@{caller} your direction cooldown is active. Try !checkchat again in about {remainingMinutes} minute(s). 🎬");
            return true;
        }

        // Trigger Mix It Up with the validated optional text (can be blank).
        bool mixitupOk = TriggerMixItUp(checkchatText);
        if (!mixitupOk)
        {
            // Do not start cooldown on failed external call.
            return true;
        }

        // Start new cooldown only after successful trigger.
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

        // Prefer rawInput from command trigger, then fallback to message.
        string input = GetArg(ARG_RAW_INPUT);
        if (string.IsNullOrWhiteSpace(input))
            input = GetArg(ARG_MESSAGE);

        // No message text is valid for !checkchat.
        if (string.IsNullOrWhiteSpace(input))
            return;

        string[] parts = input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return;

        // If trigger passed full chat message, drop command token.
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
