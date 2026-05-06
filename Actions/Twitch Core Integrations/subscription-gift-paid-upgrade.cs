// ACTION-CONTRACT: Actions/Twitch Core Integrations/AGENTS.md#subscription-gift-paid-upgrade.cs
// ACTION-CONTRACT-SHA256: 3a9c989984b9db2f62502eced2f94876c1b13b23f625995f9223259a3b431cc6

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    /*
     * Script: subscription-gift-paid-upgrade.cs
     * Event:  Gift Paid Upgrade — fires when a viewer who received a gifted subscription
     *         upgrades to their own paid subscription to keep their sub going
     *         (they are taking over payment for a sub they originally received as a gift)
     *
     * Streamer.bot trigger:
     *   Twitch → Subscriptions → Gift Paid Upgrade
     *   Requires Streamer.bot v0.2.5 or later.
     *
     * Available trigger args (read via CPH.TryGetArg):
     *   user     (string) : Display name of the viewer upgrading to paid
     *   userId   (string) : Twitch user ID
     *
     *   Note: No additional event-specific args are documented for this trigger beyond
     *         the standard Twitch User shared variable group (user, userId, etc.).
     *
     * What this script does:
     *   Calls the Mix It Up Run Command API so Mix It Up can handle the
     *   on-stream reaction (alert, chat callout, etc.).
     *   This script now forwards the documented user details as Mix It Up
     *   special identifiers so the Mix It Up command can distinguish this
     *   event from other subscription flows.
     *
     * Operator steps:
     *   1. Paste this script into the "Subscription Gift Paid Upgrade" Streamer.bot action.
     *   2. Wire the action to: Twitch → Subscriptions → Gift Paid Upgrade
     *   3. Confirm your Streamer.bot version is 0.2.5 or later — this trigger does not
     *      exist in earlier versions.
     *   4. Confirm MIXITUP_COMMAND_ID still matches your Mix It Up command export.
     *   5. In Mix It Up, reference these special identifiers:
     *      $subuser, $subuserid, $subtype
     */

    private const string SCRIPT_NAME = "Core - Subscription Gift Paid Upgrade";

    // Mix It Up command ID for gift-to-paid upgrades.
    private const string MIXITUP_COMMAND_ID = "0bcd98e5-79a4-4767-8e60-58a747d7b7a1";

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
        // We are using special identifiers for structured subscription data.
        // Keep Arguments empty unless a future Mix It Up command needs a
        // plain-text argument string as well.
        return string.Empty;
    }

    private object BuildSpecialIdentifiers()
    {
        string user = GetStringArg("user");
        string userId = GetStringArg("userId");

        return new
        {
            subuser = user,
            subuserid = userId,
            subtype = "giftpaidupgrade"
        };
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

    private string GetStringArg(string argName)
    {
        string value = "";
        CPH.TryGetArg(argName, out value);
        return value ?? "";
    }
}
