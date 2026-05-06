// ACTION-CONTRACT: Actions/Twitch Core Integrations/AGENTS.md#subscription-renewed.cs
// ACTION-CONTRACT-SHA256: 57e6e0a3502fe9f63e87906bc2ff8b9db2d4e8e538d2ba92e6be939178b63fbc

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    /*
     * Script: subscription-renewed.cs
     * Event:  Resubscription — fires when a viewer renews a subscription they already had
     *         (they are continuing their streak, not subscribing for the first time)
     *
     * Streamer.bot trigger:
     *   Twitch → Subscriptions → Resubscription
     *
     * Available trigger args (read via CPH.TryGetArg):
     *   user               (string) : Display name of the resubscriber
     *   userId             (string) : Twitch user ID
     *   tier               (string) : "prime", "tier 1", "tier 2", or "tier 3"
     *   cumulative         (number) : Total cumulative months subscribed (all time)
     *   monthStreak        (number) : Current consecutive months without a gap
     *   streakShared       (bool)   : True if the viewer chose to publicly share their streak
     *   isMultiMonth       (bool)   : True if this is a multi-month resub purchase
     *   multiMonthDuration (number) : Total months in the multi-month package (3, 6, or 12)
     *   multiMonthTenure   (number) : Months already completed in the package
     *
     * What this script does:
     *   Calls the Mix It Up Run Command API so Mix It Up can handle the
     *   on-stream reaction (alert, chat message, milestone check, etc.).
     *   This script now forwards the resubscription details as Mix It Up
     *   special identifiers so the Mix It Up command can branch on tier,
     *   streak visibility, cumulative months, and multi-month status.
     *
     * Operator steps:
     *   1. Paste this script into the "Subscription Renewed" Streamer.bot action.
     *   2. Wire the action to: Twitch → Subscriptions → Resubscription
     *   3. Confirm MIXITUP_COMMAND_ID still matches your Mix It Up command export.
     *   4. In Mix It Up, reference these special identifiers:
     *      $subuser, $subuserid, $subtier, $subtype,
     *      $subcumulative, $submonthstreak, $substreakshared,
     *      $subismultimonth, $submultimonthduration, $submultimonthtenure
     */

    private const string SCRIPT_NAME = "Core - Subscription Renewed";

    // Mix It Up command ID for resubscriptions.
    private const string MIXITUP_COMMAND_ID = "4af70639-67b5-4d83-8da1-7be0afe1ce76";

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
        // Read every documented resubscription field defensively.
        string user = GetStringArg("user");
        string userId = GetStringArg("userId");
        string tier = GetStringArg("tier");
        int cumulative = GetIntArg("cumulative");
        int monthStreak = GetIntArg("monthStreak");
        bool streakShared = GetBoolArg("streakShared");
        bool isMultiMonth = GetBoolArg("isMultiMonth");
        int multiMonthDuration = GetIntArg("multiMonthDuration");
        int multiMonthTenure = GetIntArg("multiMonthTenure");

        // Mix It Up special identifier keys should stay lowercase with no spaces.
        // Send values as strings so Mix It Up can consume them consistently.
        return new
        {
            subuser = user,
            subuserid = userId,
            subtier = tier,
            subtype = "renewed",
            subcumulative = cumulative.ToString(),
            submonthstreak = monthStreak.ToString(),
            substreakshared = streakShared ? "true" : "false",
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
