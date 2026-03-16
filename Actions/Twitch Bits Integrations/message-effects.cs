using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // Trigger arg name exposed by Streamer.bot for automatic rewards that include
    // free-form user-entered text. The docs describe this as the redeeming user's input.
    private const string ARG_USER_INPUT = "userInput";

    // Mix It Up API constants.
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";

    // Placeholder command ID for the message effects bits purchase flow.
    // Replace this with the real Mix It Up command ID when ready.
    private const string MIXITUP_MESSAGE_EFFECTS_COMMAND_ID = "replace-this-id-with-bits-message-effects";

    // Reuse one HttpClient instance for reliability.
    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    /*
     * Purpose:
     * - Handles the Twitch automatic reward redemption for the message effects bits purchase.
     * - Reads the user's entered message from Streamer.bot's userInput arg.
     * - Forwards that message to a Mix It Up command using the standard payload shape.
     *
     * Expected trigger/input:
     * - Streamer.bot action wired to:
     *   Twitch -> Channel Reward -> Automatic Reward Redemption
     * - Streamer.bot should provide userInput for this reward type.
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
        string userInput = GetArg(ARG_USER_INPUT);

        if (string.IsNullOrWhiteSpace(userInput))
        {
            CPH.LogWarn("[Twitch Automatic Reward: Message Effects] No userInput value was provided by Streamer.bot.");
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
