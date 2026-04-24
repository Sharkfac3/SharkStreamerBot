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
    private const int HYDRATE_MAX_MESSAGE_WORDS = 5;
    private const int HYDRATE_MAX_MESSAGE_CHARS = 40;
    private const int HYDRATE_COOLDOWN_MINUTES = 5;

    private const string MIXITUP_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_COMMAND_ID = "53244f6a-6882-4457-bc9f-b429ecd9ce9d";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";
    private const string MIXITUP_TYPE_AMOUNT = "amount";
    private const string MIXITUP_TYPE_MESSAGE = "message";

    // Reuse one HttpClient instance for reliability/performance.
    private static readonly HttpClient Http = new HttpClient();

    private sealed class HydrateRequest
    {
        public string PayloadText { get; set; }
        public string PayloadType { get; set; }
    }

    /*
     * Purpose:
     * - Handles the Water Wizard-only !hydrate command.
     * - Accepts either:
     *   1) a whole number from 1 to 10, or
     *   2) a short custom message up to 5 words.
     * - Applies a 5-minute cooldown for the active Water Wizard.
     * - For valid wizard usage, forwards the payload to Mix It Up as Arguments,
     *   plus hydratemessage / hydratetype special identifiers.
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
     * - Wizard caller with valid input: triggers Mix It Up command with both
     *   the raw payload text and an explicit payload type.
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

        // Parse hydrate input. We support either a numeric intensity (1..10)
        // or a short custom message (up to 5 words).
        HydrateRequest hydrateRequest = ParseHydrateRequest();
        if (hydrateRequest == null)
        {
            CPH.SendMessage($"@{caller} use !hydrate <1-10> or !hydrate <short message> (up to {HYDRATE_MAX_MESSAGE_WORDS} words / {HYDRATE_MAX_MESSAGE_CHARS} chars). Example: !hydrate 7 or !hydrate hydrate the crew 💧");
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

        // Trigger Mix It Up with both the payload text and an explicit type marker
        // so Mix It Up can branch safely without guessing.
        bool mixitupOk = TriggerMixItUp(hydrateRequest);
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

    private HydrateRequest ParseHydrateRequest()
    {
        // Preferred path: first command argument (input0). This is the cleanest way
        // to capture a one-word number like !hydrate 7.
        string input0 = GetArg(ARG_INPUT0);
        if (int.TryParse(input0, out int parsedAmount)
            && parsedAmount >= HYDRATE_MIN_VALUE
            && parsedAmount <= HYDRATE_MAX_VALUE)
        {
            return new HydrateRequest
            {
                PayloadText = parsedAmount.ToString(),
                PayloadType = MIXITUP_TYPE_AMOUNT
            };
        }

        // Fallback path: parse the text after the command from rawInput/message.
        // This keeps the script working across different Streamer.bot trigger setups.
        string commandText = ParseCommandText("!hydrate");
        if (string.IsNullOrWhiteSpace(commandText))
            return null;

        if (int.TryParse(commandText, out parsedAmount)
            && parsedAmount >= HYDRATE_MIN_VALUE
            && parsedAmount <= HYDRATE_MAX_VALUE)
        {
            return new HydrateRequest
            {
                PayloadText = parsedAmount.ToString(),
                PayloadType = MIXITUP_TYPE_AMOUNT
            };
        }

        int wordCount = CountWords(commandText);
        if (wordCount < 1 || wordCount > HYDRATE_MAX_MESSAGE_WORDS)
            return null;

        // Also reject messages that exceed the character limit.
        if (commandText.Length > HYDRATE_MAX_MESSAGE_CHARS)
            return null;

        return new HydrateRequest
        {
            PayloadText = commandText,
            PayloadType = MIXITUP_TYPE_MESSAGE
        };
    }

    private string ParseCommandText(string commandName)
    {
        string input = GetArg(ARG_RAW_INPUT);
        if (string.IsNullOrWhiteSpace(input))
            input = GetArg(ARG_MESSAGE);

        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        string[] parts = input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return string.Empty;

        int startIndex = 0;
        if (parts[0].StartsWith(commandName, StringComparison.OrdinalIgnoreCase))
            startIndex = 1;

        if (startIndex >= parts.Length)
            return string.Empty;

        return string.Join(" ", parts, startIndex, parts.Length - startIndex).Trim();
    }

    private int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        return text.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private bool TriggerMixItUp(HydrateRequest hydrateRequest)
    {
        try
        {
            string url = $"{MIXITUP_BASE_URL.TrimEnd('/')}/api/v2/commands/{MIXITUP_COMMAND_ID}";

            string payloadText = hydrateRequest?.PayloadText ?? string.Empty;
            string payloadType = hydrateRequest?.PayloadType ?? MIXITUP_TYPE_MESSAGE;

            string payload = JsonSerializer.Serialize(new
            {
                Platform = MIXITUP_PLATFORM_TWITCH,
                Arguments = payloadText,
                SpecialIdentifiers = new
                {
                    hydratemessage = payloadText,
                    hydratetype = payloadType
                },
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
