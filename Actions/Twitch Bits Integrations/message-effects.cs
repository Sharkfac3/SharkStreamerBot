using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // Trigger arg names we have seen used by Streamer.bot-style reward flows.
    // The automatic bits reward appears to provide free-form user text under different
    // keys depending on trigger wiring/version, so we check several safe fallbacks.
    private const string ARG_USER_INPUT = "userInput";
    private const string ARG_INPUT0 = "input0";
    private const string ARG_MESSAGE = "message";
    private const string ARG_RAW_INPUT = "rawInput";

    // Mix It Up API constants.
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";

    // Verified Mix It Up command ID from Tools/MixItUp/Api/data/mixitup-commands.txt
    // Action Group: Twitch - Bits - Message Effects
    private const string MIXITUP_MESSAGE_EFFECTS_COMMAND_ID = "28397ebb-7a68-4a52-b448-3044a811c008";

    // Reuse one HttpClient instance for reliability.
    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    /*
     * Purpose:
     * - Handles the Twitch automatic reward redemption for the message effects bits purchase.
     * - Reads the user's entered message from Streamer.bot using a fallback chain
     *   (userInput -> input0 -> message -> rawInput).
     * - Forwards that message to a Mix It Up command using the standard payload shape.
     *
     * Expected trigger/input:
     * - Streamer.bot action wired to:
     *   Twitch -> Channel Reward -> Automatic Reward Redemption
     * - Streamer.bot may expose the entered message under userInput, input0,
     *   message, or rawInput depending on trigger wiring/version.
     *
     * Required runtime variables:
     * - None.
     *
     * Key outputs/side effects:
     * - POSTs to the Mix It Up command endpoint.
     * - Sends Arguments = the trimmed userInput value.
     * - Sends SpecialIdentifiers = { } for now.
     * - Logs warnings/errors instead of throwing, so the action queue stays stable.
     *
     * Operator notes:
     * - Replace the placeholder Mix It Up command ID before production use.
     * - Filter the action so it only runs for the message effects bits purchase.
     */
    public bool Execute()
    {
        string inputSource = string.Empty;
        string userInput = GetUserInput(out inputSource);

        if (string.IsNullOrWhiteSpace(userInput))
        {
            CPH.LogWarn("[Twitch Automatic Reward: Message Effects] No message input was provided by Streamer.bot. Checked userInput, input0, message, and rawInput.");
        }
        else if (!string.Equals(inputSource, ARG_USER_INPUT, StringComparison.Ordinal))
        {
            // Temporary diagnostic: if a fallback arg is the real source, keep one clear log line
            // so the operator can confirm which trigger field Streamer.bot is actually populating.
            CPH.LogWarn($"[Twitch Automatic Reward: Message Effects] Using fallback arg '{inputSource}' for redeem text.");
        }

        TriggerMixItUpCommand(
            MIXITUP_MESSAGE_EFFECTS_COMMAND_ID,
            "Twitch Automatic Reward: Message Effects",
            arguments: userInput,
            specialIdentifiers: new { }
        );

        return true;
    }

    /// <summary>
    /// Reads the redeem text using a defensive fallback chain.
    /// This helps when different Streamer.bot reward triggers expose the same user-entered
    /// text under different argument names.
    /// </summary>
    private string GetUserInput(out string sourceArg)
    {
        string[] candidateArgs = new[] { ARG_USER_INPUT, ARG_INPUT0, ARG_MESSAGE, ARG_RAW_INPUT };

        foreach (string argName in candidateArgs)
        {
            string value = GetArg(argName);
            if (!string.IsNullOrWhiteSpace(value))
            {
                sourceArg = argName;
                return value;
            }
        }

        sourceArg = string.Empty;
        return string.Empty;
    }

    /// <summary>
    /// Reads a string arg from Streamer.bot safely and returns a trimmed fallback empty string.
    /// </summary>
    private string GetArg(string argName)
    {
        if (CPH.TryGetArg(argName, out string value) && !string.IsNullOrWhiteSpace(value))
            return value.Trim();

        return string.Empty;
    }

    /// <summary>
    /// Triggers a Mix It Up command via local API.
    /// Uses the required payload convention for Streamer.bot scripts:
    /// Platform, Arguments, SpecialIdentifiers, IgnoreRequirements.
    /// </summary>
    private bool TriggerMixItUpCommand(
        string commandId,
        string logPrefix,
        string arguments,
        object specialIdentifiers)
    {
        if (string.IsNullOrWhiteSpace(commandId))
        {
            CPH.LogWarn($"[{logPrefix}] Mix It Up command ID is not configured.");
            return false;
        }

        try
        {
            string url = $"{MIXITUP_API_BASE_URL.TrimEnd('/')}/api/v2/commands/{commandId}";
            string payload = JsonSerializer.Serialize(new
            {
                Platform = MIXITUP_PLATFORM_TWITCH,
                Arguments = arguments ?? string.Empty,
                SpecialIdentifiers = specialIdentifiers ?? new { },
                IgnoreRequirements = false
            });

            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = MIXITUP_HTTP_CLIENT.PostAsync(url, content).GetAwaiter().GetResult();

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
