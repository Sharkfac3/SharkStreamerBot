// ACTION-CONTRACT: Actions/Twitch Core Integrations/AGENTS.md#subscription-pay-it-forward.cs
// ACTION-CONTRACT-SHA256: c3f9eda58fa753c0958c94085eff936b738248a6028310492189b4316ff4479f

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    /*
     * Script: subscription-pay-it-forward.cs
     * Event:  Pay It Forward — fires when a viewer who received a gifted subscription
     *         turns around and gifts a subscription to someone else in the channel
     *         (they are passing the gift along rather than upgrading to paid themselves)
     *
     * Streamer.bot trigger:
     *   Twitch → Subscriptions → Pay It Forward
     *   Requires Streamer.bot v0.2.5 or later.
     *
     * Available trigger args (read via CPH.TryGetArg):
     *   user     (string) : Display name of the viewer paying it forward
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
     *   1. Paste this script into the "Subscription Pay It Forward" Streamer.bot action.
     *   2. Wire the action to: Twitch → Subscriptions → Pay It Forward
     *   3. Confirm your Streamer.bot version is 0.2.5 or later — this trigger does not
     *      exist in earlier versions.
     *   4. Confirm MIXITUP_COMMAND_ID still matches your Mix It Up command export.
     *   5. In Mix It Up, reference these special identifiers:
     *      $subuser, $subuserid, $subtype
     */

    private const string SCRIPT_NAME = "Core - Subscription Pay It Forward";

    // Mix It Up command ID for pay-it-forward events.
    private const string MIXITUP_COMMAND_ID = "f50ca2a7-cc1d-44c2-b0b8-f4abf9bf2207";

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
            subtype = "payitforward"
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
