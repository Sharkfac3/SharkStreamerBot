using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    /*
     * Purpose:
     * - Shared dispatcher template for simple subscription events that each map
     *   1-to-1 with a single Mix It Up command.
     *
     * Supported events (one paste per Streamer.bot action):
     * - Subscription New          → Twitch → Subscriptions → Subscription
     * - Subscription Renewed      → Twitch → Subscriptions → Resubscription
     * - Prime Paid Upgrade        → Twitch → Subscriptions → Prime Paid Upgrade
     * - Gift Paid Upgrade         → Twitch → Subscriptions → Gift Paid Upgrade
     * - Pay It Forward            → Twitch → Subscriptions → Pay It Forward
     *
     * Operator paste instructions:
     * - Paste this file into the target Streamer.bot action.
     * - Replace SCRIPT_NAME with the human-readable name for that event
     *   (used in log output so you can tell which event fired).
     * - Replace MIXITUP_COMMAND_ID with the real Mix It Up command ID for that event.
     * - Do NOT change any other logic — the structure is identical for all events.
     *
     * Expected trigger/input:
     * - Wire to the appropriate Streamer.bot subscription event trigger.
     * - No extra sub-actions or Set Argument steps required.
     *
     * Required runtime variables:
     * - None.
     *
     * Key outputs/side effects:
     * - Calls the Mix It Up Run Command API when a real command ID is configured.
     * - Sends empty Arguments and empty SpecialIdentifiers for now.
     * - Does not interact with OBS.
     *
     * Operator notes:
     * - Replace MIXITUP_COMMAND_ID before production use.
     * - Expand BuildArguments / BuildSpecialIdentifiers when the final event
     *   field contract for this specific event is decided.
     * - Requires Streamer.bot v0.2.5 or later for Prime Paid Upgrade,
     *   Gift Paid Upgrade, and Pay It Forward triggers.
     */

    // OPERATOR: Replace with the human-readable name for this event.
    // Examples: "Core - Subscription New", "Core - Subscription Renewed",
    //           "Core - Subscription Prime Paid Upgrade", etc.
    private const string SCRIPT_NAME = "Core - Subscription [REPLACE_WITH_EVENT_NAME]";

    // OPERATOR: Replace with the real Mix It Up command ID for this event.
    private const string MIXITUP_COMMAND_ID = "REPLACE_WITH_COMMAND_ID";

    private const string MIXITUP_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";

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
        // Expand this when the final event field contract for this specific event is decided.
        // Refer to Actions/Twitch Core Integrations/README.md for available trigger args per event.
        return string.Empty;
    }

    private object BuildSpecialIdentifiers()
    {
        // Expand this when the final event field contract for this specific event is decided.
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
