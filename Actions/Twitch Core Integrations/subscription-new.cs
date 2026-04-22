using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    /*
     * Script: subscription-new.cs
     * Event:  Subscription New — fires when a viewer subscribes for the first time
     *         (not a resub, not a gift — the viewer paid directly right now)
     *
     * Streamer.bot trigger:
     *   Twitch → Subscriptions → Subscription
     *
     * Available trigger args (read via CPH.TryGetArg):
     *   user               (string) : Display name of the subscriber
     *   userId             (string) : Twitch user ID
     *   tier               (string) : "prime", "tier 1", "tier 2", or "tier 3"
     *   isMultiMonth       (bool)   : True if they bought multiple months upfront
     *   multiMonthDuration (number) : Total months in the multi-month package (3, 6, or 12)
     *   multiMonthTenure   (number) : Months already completed in the package
     *
     * What this script does:
     *   Calls the Mix It Up Run Command API so Mix It Up can handle the
     *   on-stream reaction (alert, chat message, sound, etc.).
     *   This script now forwards the new-subscription details as Mix It Up
     *   special identifiers so the Mix It Up command can branch on tier
     *   and multi-month status.
     *
     * Operator steps:
     *   1. Paste this script into the "Subscription New" Streamer.bot action.
     *   2. Wire the action to: Twitch → Subscriptions → Subscription
     *   3. Confirm MIXITUP_COMMAND_ID still matches your Mix It Up command export.
     *   4. In Mix It Up, reference these special identifiers:
     *      $subuser, $subuserid, $subtier, $subtype,
     *      $subismultimonth, $submultimonthduration, $submultimonthtenure
     */

    private const string SCRIPT_NAME = "Core - Subscription New";

    // Mix It Up command ID for new subscriptions.
    private const string MIXITUP_COMMAND_ID = "4e5b04b7-b5c6-4b17-a6dd-e90efce4a591";

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
        string tier = GetStringArg("tier");
        bool isMultiMonth = GetBoolArg("isMultiMonth");
        int multiMonthDuration = GetIntArg("multiMonthDuration");
        int multiMonthTenure = GetIntArg("multiMonthTenure");

        return new
        {
            subuser = user,
            subuserid = userId,
            subtier = tier,
            subtype = "new",
            subismultimonth = isMultiMonth ? "true" : "false",
            submultimonthduration = multiMonthDuration.ToString(),
            submultimonthtenure = multiMonthTenure.ToString()
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

    private int GetIntArg(string argName)
    {
        int value = 0;
        CPH.TryGetArg(argName, out value);
        return value;
    }

    private bool GetBoolArg(string argName)
    {
        bool value = false;
        CPH.TryGetArg(argName, out value);
        return value;
    }
}
