using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    /*
     * Purpose:
     * - Base Twitch integration bridge for a Gift Paid Upgrade event.
     * - Fires when a user upgrades their gifted subscription to a paid tier.
     *
     * Expected trigger/input:
     * - Wire this script to the Streamer.bot Twitch event: Subscriptions → Gift Paid Upgrade.
     *
     * Required runtime variables:
     * - None.
     *
     * Key outputs/side effects:
     * - Calls the Mix It Up Run Command API when a real command ID is configured.
     * - Sends empty Arguments and empty SpecialIdentifiers for now.
     * - Does not interact with OBS.
     *
     * Trigger-specific arguments (available via CPH.TryGetArg):
     * - user   (string) : Display name of the user upgrading (from Twitch User group)
     * - userId (string) : Twitch user ID (from Twitch User group)
     * - (No additional documented trigger-specific variables beyond the shared groups.)
     *
     * Operator notes:
     * - Replace MIXITUP_COMMAND_ID before production use.
     * - Expand BuildArguments / BuildSpecialIdentifiers later when the final event fields are known.
     * - Requires Streamer.bot v0.2.5 or later.
     */

    private const string SCRIPT_NAME = "Core - Subscription Gift Paid Upgrade";
    private const string MIXITUP_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";
    private const string MIXITUP_COMMAND_ID = "REPLACE_WITH_CORE_SUBSCRIPTION_GIFT_PAID_UPGRADE_COMMAND_ID";

    private static readonly HttpClient Http = new HttpClient();

    public bool Execute()
    {
        try
        {
            if (HasPlaceholderCommandId(MIXITUP_COMMAND_ID))
            {
                CPH.LogWarn($"[{SCRIPT_NAME}] Mix It Up command ID is still a placeholder. Skipping call.");
                return true;
            }

            string arguments = BuildArguments();
            object specialIdentifiers = BuildSpecialIdentifiers();
            RunMixItUpCommand(arguments, specialIdentifiers);
        }
        catch (Exception ex)
        {
            CPH.LogError($"[{SCRIPT_NAME}] Exception while calling Mix It Up: {ex}");
        }

        return true;
    }

    private string BuildArguments()
    {
        // Expand this when the final event field contract is decided.
        // Available args: user, userId (and other Twitch User group fields).
        return string.Empty;
    }

    private object BuildSpecialIdentifiers()
    {
        // Expand this when the final event field contract is decided.
        return new { };
    }

    private void RunMixItUpCommand(string arguments, object specialIdentifiers)
    {
        string url = $"{MIXITUP_BASE_URL.TrimEnd('/')}/api/v2/commands/{MIXITUP_COMMAND_ID}";
        string payload = JsonSerializer.Serialize(new
        {
            Platform = MIXITUP_PLATFORM_TWITCH,
            Arguments = arguments ?? string.Empty,
            SpecialIdentifiers = specialIdentifiers ?? new { },
            IgnoreRequirements = false
        });

        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage response = Http.PostAsync(url, content).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            CPH.LogWarn($"[{SCRIPT_NAME}] Mix It Up call failed: {(int)response.StatusCode} {response.ReasonPhrase}");
        }
    }

    private bool HasPlaceholderCommandId(string commandId)
    {
        return string.IsNullOrWhiteSpace(commandId)
            || commandId.IndexOf("replace", StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
