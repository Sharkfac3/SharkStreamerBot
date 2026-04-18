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
     *
     * Operator steps:
     *   1. Paste this script into the "Subscription Renewed" Streamer.bot action.
     *   2. Wire the action to: Twitch → Subscriptions → Resubscription
     *   3. Confirm MIXITUP_COMMAND_ID still matches your Mix It Up command export.
     *   4. Expand BuildArguments / BuildSpecialIdentifiers when you decide
     *      which trigger args to forward (e.g. cumulative, monthStreak, tier).
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
        // Expand this when you decide what data to forward.
        // Available args: user, userId, tier, cumulative, monthStreak, streakShared,
        //                 isMultiMonth, multiMonthDuration, multiMonthTenure.
        return string.Empty;
    }

    private object BuildSpecialIdentifiers()
    {
        // Expand this when you decide what data to forward.
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
