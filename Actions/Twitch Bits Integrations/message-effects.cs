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

    // SYNC CONSTANTS (Bits feature)
    // Keep readout pacing aligned with the cheer-tier scripts.
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";
    private const int WAIT_BASE_PREP_MS = 3000;
    private const int WAIT_MS_PER_WORD = 400;
    private const int WAIT_TAIL_BUFFER_MS = 500;

    // Mix It Up API constants.
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";

    // Placeholder until this action group exists in Tools/MixItUp/Api/data/mixitup-commands.txt.
    // Action Group: Twitch - Bits - Message Effects
    private const string MIXITUP_MESSAGE_EFFECTS_COMMAND_ID = "REPLACE_WITH_MESSAGE_EFFECTS_COMMAND_ID";

    // Reuse one HttpClient instance for reliability.
    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    /*
     * Purpose:
     * - Handles the Twitch automatic reward redemption for the message effects bits purchase.
     * - Reads the user's entered message from Streamer.bot using a fallback chain
     *   (userInput -> input0 -> message -> rawInput).
     * - Forwards that message to a Mix It Up command using the standard payload shape.
     * - Waits after a successful call so TTS/message-effect readouts do not overlap as easily.
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
     * - Uses the same 3000ms + 400ms/word + 500ms pacing wait as the bits-tier cheer scripts.
     * - Logs warnings/errors instead of throwing, so the action queue stays stable.
     *
     * Operator notes:
     * - Current Mix It Up command ID is configured.
     * - Filter the action so it only runs for the message effects bits purchase.
     */
    public bool Execute()
    {
        try
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

            bool mixItUpTriggered = TriggerMixItUpReadout(
                MIXITUP_MESSAGE_EFFECTS_COMMAND_ID,
                "Twitch Automatic Reward: Message Effects",
                userInput
            );

            // Only pause the action when Mix It Up actually accepted the request.
            if (mixItUpTriggered)
            {
                int waitMs = CalculateReadoutWaitMs(userInput);
                CPH.Wait(waitMs);
            }
        }
        catch (Exception ex)
        {
            CPH.LogError($"[Twitch Automatic Reward: Message Effects] Exception while calling Mix It Up: {ex}");
        }

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
    /// Triggers a Mix It Up readout-style command via local API.
    /// Uses the same payload convention and success/failure behavior as the bits-tier scripts.
    /// </summary>
    private bool TriggerMixItUpReadout(string commandId, string logPrefix, string arguments)
    {
        if (string.IsNullOrWhiteSpace(commandId) ||
            commandId.StartsWith("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase))
        {
            CPH.LogWarn($"[{logPrefix}] Mix It Up command ID is not configured.");
            return false;
        }

        string url = $"{MIXITUP_API_BASE_URL.TrimEnd('/')}/api/v2/commands/{commandId}";
        string payload = JsonSerializer.Serialize(new
        {
            Platform = MIXITUP_PLATFORM_TWITCH,
            Arguments = arguments ?? string.Empty,
            SpecialIdentifiers = new { },
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

    /// <summary>
    /// Estimates wait duration for TTS so queue items don't overlap.
    /// Formula:
    /// - 3000ms prep time
    /// - 400ms per word
    /// - 500ms tail buffer
    /// </summary>
    private int CalculateReadoutWaitMs(string message)
    {
        int wordCount = CountWords(message);
        return WAIT_BASE_PREP_MS + (wordCount * WAIT_MS_PER_WORD) + WAIT_TAIL_BUFFER_MS;
    }

    /// <summary>
    /// Counts words by splitting on spaces and ignoring empty entries.
    /// </summary>
    private int CountWords(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return 0;
        }

        return message.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
