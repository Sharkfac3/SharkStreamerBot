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
     *
     * Operator steps:
     *   1. Paste this script into the "Subscription New" Streamer.bot action.
     *   2. Wire the action to: Twitch → Subscriptions → Subscription
     *   3. Replace MIXITUP_COMMAND_ID below with your real Mix It Up command ID.
     *   4. Expand BuildArguments / BuildSpecialIdentifiers when you decide
     *      which trigger args to forward to Mix It Up.
     */

    private const string SCRIPT_NAME = "Core - Subscription New";

    // OPERATOR: Replace with your real Mix It Up command ID for new subscriptions.
    private const string MIXITUP_COMMAND_ID = "REPLACE_WITH_CORE_SUBSCRIPTION_NEW_COMMAND_ID";

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
        // Available args: user, userId, tier, isMultiMonth, multiMonthDuration, multiMonthTenure.
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
