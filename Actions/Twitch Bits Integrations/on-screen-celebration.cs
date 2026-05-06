// ACTION-CONTRACT: Actions/Twitch Bits Integrations/AGENTS.md#on-screen-celebration.cs
// ACTION-CONTRACT-SHA256: 13a52b9c9fb184a06ae9d535a912612f0161a9bfe2f6fb0e836df6b72d897aaf

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // Mix It Up API constants.
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";

    // Placeholder until this action group exists in Tools/MixItUp/Api/data/mixitup-commands.txt.
    // Action Group: Twitch - Bits - On Screen Celebration
    private const string MIXITUP_ON_SCREEN_CELEBRATION_COMMAND_ID = "REPLACE_WITH_ON_SCREEN_CELEBRATION_COMMAND_ID";

    // Reuse one HttpClient instance for reliability.
    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    /*
     * Purpose:
     * - Handles the Twitch automatic reward redemption for the on-screen celebration bits purchase.
     * - Triggers a Mix It Up command using the standard payload shape.
     * - Sends celebration metadata as Mix It Up special identifiers.
     *
     * Expected trigger/input:
     * - Streamer.bot action wired to:
     *   Twitch -> Channel Reward -> Automatic Reward Redemption
     *
     * Required runtime variables:
     * - None.
     *
     * Key outputs/side effects:
     * - POSTs to the Mix It Up command endpoint.
     * - Sends Arguments = "" for compatibility with the current Mix It Up command.
     * - Sends populated SpecialIdentifiers for Mix It Up branching/text fields.
     * - Logs warnings/errors instead of throwing, so the action queue stays stable.
     *
     * Operator notes:
     * - Replace the placeholder Mix It Up command ID before production use.
     * - Filter the action so it only runs for the on-screen celebration bits purchase.
     */
    public bool Execute()
    {
        TriggerMixItUpCommand(
            MIXITUP_ON_SCREEN_CELEBRATION_COMMAND_ID,
            "Twitch Automatic Reward: On-Screen Celebration",
            arguments: string.Empty,
            specialIdentifiers: BuildSpecialIdentifiers()
        );

        return true;
    }

    /// <summary>
    /// Builds the Mix It Up special identifier payload with stable lowercase keys.
    /// Values are strings so Mix It Up commands can consume them consistently.
    /// </summary>
    private object BuildSpecialIdentifiers()
    {
        string celebrationMessage = GetFirstStringArg("userInput", "input0", "message", "rawInput");

        return new
        {
            celebrationuser = GetStringArg("user"),
            celebrationuserid = GetStringArg("userId"),
            celebrationtype = "onscreencelebration",
            celebrationrewardid = GetFirstStringArg("reward", "rewardId"),
            celebrationrewardname = GetFirstStringArg("rewardName", "rewardTitle"),
            celebrationmessage = celebrationMessage,
            celebrationmessagetype = string.IsNullOrWhiteSpace(celebrationMessage) ? "none" : "message"
        };
    }

    /// <summary>
    /// Reads the first non-empty string arg from Streamer.bot using a defensive fallback chain.
    /// </summary>
    private string GetFirstStringArg(params string[] argNames)
    {
        foreach (string argName in argNames)
        {
            string value = GetStringArg(argName);
            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        return string.Empty;
    }

    /// <summary>
    /// Reads a string arg from Streamer.bot safely and returns a trimmed fallback empty string.
    /// </summary>
    private string GetStringArg(string argName)
    {
        if (CPH.TryGetArg(argName, out string value) && !string.IsNullOrWhiteSpace(value))
            return value.Trim();

        return string.Empty;
    }

    /// <summary>
    /// Reads an int arg from Streamer.bot safely and returns the provided fallback when missing/non-numeric.
    /// </summary>
    private int GetIntArg(string argName, int fallback = 0)
    {
        if (CPH.TryGetArg(argName, out int intValue))
            return intValue;

        string rawValue = GetStringArg(argName);
        if (int.TryParse(rawValue, out int parsedValue))
            return parsedValue;

        return fallback;
    }

    /// <summary>
    /// Reads a bool arg from Streamer.bot safely and returns the provided fallback when missing/unparseable.
    /// </summary>
    private bool GetBoolArg(string argName, bool fallback = false)
    {
        if (CPH.TryGetArg(argName, out bool boolValue))
            return boolValue;

        string rawValue = GetStringArg(argName);
        if (bool.TryParse(rawValue, out bool parsedValue))
            return parsedValue;

        return fallback;
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
