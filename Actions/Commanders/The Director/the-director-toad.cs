using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (The Director)
    // Keep these names aligned with:
    // - Actions/Commanders/The Director/the-director-redeem.cs
    // - Actions/Commanders/The Director/the-director-toad.cs
    private const string ARG_USER = "user";
    private const string ARG_MESSAGE = "message";
    private const string ARG_RAW_INPUT = "rawInput";

    private const string VAR_CURRENT_THE_DIRECTOR = "current_the_director";
    private const string VAR_DIRECTOR_TOAD_NEXT_ALLOWED_UTC = "the_director_toad_next_allowed_utc";

    private const int TOAD_MAX_WORD_COUNT = 30;
    private const int TOAD_COOLDOWN_MINUTES = 1;

    private const string MIXITUP_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_COMMAND_ID = "72622c77-8827-438a-9418-977d7bd03136";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";
    private const string MIXITUP_SPECIAL_TYPE_NORMAL = "normal";
    private const string MIXITUP_SPECIAL_TYPE_HYPNO = "hypno";
    private const int HYPNO_CHANCE_DENOMINATOR = 10; // 1-in-10 chance

    private static readonly HttpClient Http = new HttpClient();
    private static readonly Random Rng = new Random();

    /*
     * Purpose:
     * - Handles The Director-only !toad command.
     * - Accepts up to 30 words after !toad (message text is optional).
     * - Applies a 5-minute cooldown for the active Director.
     * - For valid director usage, forwards the optional text to Mix It Up as Arguments.
     */
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

        if (!TryParseCommandText("!toad", TOAD_MAX_WORD_COUNT, out string toadText))
        {
            CPH.SendMessage($"@{caller} !toad can include up to {TOAD_MAX_WORD_COUNT} words after the command (text is optional). 🎬");
            return true;
        }

        long nowUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long nextAllowedUtc = (CPH.GetGlobalVar<long?>(VAR_DIRECTOR_TOAD_NEXT_ALLOWED_UTC, false) ?? 0L);

        if (nowUtc < nextAllowedUtc)
        {
            long remainingSeconds = Math.Max(1L, nextAllowedUtc - nowUtc);
            int remainingMinutes = (int)Math.Ceiling(remainingSeconds / 60.0);
            CPH.SendMessage($"@{caller} your toad call is cooling down. Try !toad again in about {remainingMinutes} minute(s). 🎬");
            return true;
        }

        bool mixitupOk = TriggerMixItUp(toadText);
        if (!mixitupOk)
            return true;

        long newNextAllowedUtc = DateTimeOffset.UtcNow.AddMinutes(TOAD_COOLDOWN_MINUTES).ToUnixTimeSeconds();
        CPH.SetGlobalVar(VAR_DIRECTOR_TOAD_NEXT_ALLOWED_UTC, newNextAllowedUtc, false);

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
            CPH.SendMessage($"@{caller} only our current Director ({currentDirector}) can use !toad right now. Type !award to support them! 🎬");
            return;
        }

        CPH.SendMessage($"@{caller} there is no current Director right now—redeem to become The Director and unlock !toad! 🎬");
    }

    private bool TryParseCommandText(string commandName, int maxWords, out string commandText)
    {
        commandText = string.Empty;

        string input = GetArg(ARG_RAW_INPUT);
        if (string.IsNullOrWhiteSpace(input))
            input = GetArg(ARG_MESSAGE);

        // If no input is available from args, allow empty optional text.
        if (string.IsNullOrWhiteSpace(input))
            return true;

        string[] parts = input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return true;

        int startIndex = 0;
        if (parts[0].StartsWith(commandName, StringComparison.OrdinalIgnoreCase))
            startIndex = 1;

        int wordCount = Math.Max(0, parts.Length - startIndex);
        if (wordCount > maxWords)
            return false;

        if (wordCount == 0)
            return true;

        commandText = string.Join(" ", parts, startIndex, wordCount);
        return true;
    }

    private string GetToadTypeSpecialIdentifier()
    {
        // Most toads should be the standard behavior.
        // We occasionally send a "hypno" type for special handling in Mix It Up.
        // Next(10) returns 0..9, so 0 gives us an exact 1-in-10 chance.
        bool isHypno = Rng.Next(HYPNO_CHANCE_DENOMINATOR) == 0;
        return isHypno ? MIXITUP_SPECIAL_TYPE_HYPNO : MIXITUP_SPECIAL_TYPE_NORMAL;
    }

    private bool TriggerMixItUp(string argumentText)
    {
        try
        {
            string url = $"{MIXITUP_BASE_URL.TrimEnd('/')}/api/v2/commands/{MIXITUP_COMMAND_ID}";
            string toadType = GetToadTypeSpecialIdentifier();

            string payload = JsonSerializer.Serialize(new
            {
                Platform = MIXITUP_PLATFORM_TWITCH,
                Arguments = argumentText,
                SpecialIdentifiers = new
                {
                    type = toadType
                },
                IgnoreRequirements = false
            });

            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = Http.PostAsync(url, content).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                CPH.LogWarn($"[The Director Toad] Mix It Up call failed: {(int)response.StatusCode} {response.ReasonPhrase}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            CPH.LogError($"[The Director Toad] Exception while calling Mix It Up: {ex}");
            return false;
        }
    }
}
