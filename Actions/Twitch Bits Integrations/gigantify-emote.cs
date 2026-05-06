// ACTION-CONTRACT: Actions/Twitch Bits Integrations/AGENTS.md#gigantify-emote.cs
// ACTION-CONTRACT-SHA256: ab7ad5a76802761ec63e500450c1686659a65413ace9226617500adfacde5542

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // Trigger arg names exposed by Streamer.bot for:
    // Twitch -> Channel Reward -> Automatic Reward Redemption
    private const string ARG_GIGANTIFIED_EMOTE_ID = "gigantifiedEmoteId";
    private const string ARG_GIGANTIFIED_EMOTE_NAME = "gigantifiedEmoteName";
    private const string ARG_GIGANTIFIED_EMOTE_URL = "gigantifiedEmoteUrl";

    // Mix It Up API constants.
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";

    // Placeholder until this action group exists in Tools/MixItUp/Api/data/mixitup-commands.txt.
    // Action Group: Twitch - Bits- Gigantify Emote
    private const string MIXITUP_GIGANTIFY_EMOTE_COMMAND_ID = "REPLACE_WITH_GIGANTIFY_EMOTE_COMMAND_ID";

    // This action should always send the same message payload and the standard type.
    private const string MIXITUP_MESSAGE = "whos that emote?";
    private const string MIXITUP_TYPE_NORMAL = "normal";

    // Reuse one HttpClient instance for reliability.
    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    /*
     * Purpose:
     * - Handles the Twitch "gigantify emote" automatic reward redemption.
     * - Triggers a Mix It Up command using the same payload pattern as Director toad.
     * - Always sends a fixed message and a fixed type special identifier.
     * - Also forwards the redeemed gigantified emote details when Streamer.bot provides them.
     *
     * Expected trigger/input:
     * - Streamer.bot action wired to:
     *   Twitch -> Channel Reward -> Automatic Reward Redemption
     * - For gigantify emote rewards, Streamer.bot may provide:
     *   gigantifiedEmoteId, gigantifiedEmoteName, gigantifiedEmoteUrl
     *
     * Required runtime variables:
     * - None.
     *
     * Key outputs/side effects:
     * - POSTs to Mix It Up command endpoint using standard payload shape.
     * - Sends Arguments = "whos that emote?"
     * - Sends SpecialIdentifiers.type = "normal"
     * - Sends emoteId / emoteName / emoteUrl when available from the trigger.
     * - Logs warnings/errors instead of throwing, so action queue stays stable.
     */
    public bool Execute()
    {
        string gigantifiedEmoteId = GetArg(ARG_GIGANTIFIED_EMOTE_ID);
        string gigantifiedEmoteName = GetArg(ARG_GIGANTIFIED_EMOTE_NAME);
        string gigantifiedEmoteUrl = GetArg(ARG_GIGANTIFIED_EMOTE_URL);

        TriggerMixItUpCommand(
            MIXITUP_GIGANTIFY_EMOTE_COMMAND_ID,
            "Twitch Automatic Reward: Gigantify Emote",
            arguments: MIXITUP_MESSAGE,
            specialIdentifiers: new
            {
                type = MIXITUP_TYPE_NORMAL,
                emoteId = gigantifiedEmoteId,
                emoteName = gigantifiedEmoteName,
                emoteUrl = gigantifiedEmoteUrl
            }
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
