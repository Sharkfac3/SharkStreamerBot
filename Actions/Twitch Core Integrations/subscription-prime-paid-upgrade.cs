using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    /*
     * Script: subscription-prime-paid-upgrade.cs
     * Event:  Prime Paid Upgrade — fires when a viewer who was subscribed via Amazon Prime
     *         upgrades to a paid Twitch subscription tier
     *         (they are converting their free Prime sub into a real paid one)
     *
     * Streamer.bot trigger:
     *   Twitch → Subscriptions → Prime Paid Upgrade
     *   Requires Streamer.bot v0.2.5 or later.
     *
     * Available trigger args (read via CPH.TryGetArg):
     *   user               (string) : Display name of the upgrading viewer
     *   userId             (string) : Twitch user ID
     *   systemMessage      (string) : The Twitch chat system message for this event
     *   upgradeTier        (number) : Numeric tier value (e.g. 1000 = Tier 1, 2000 = Tier 2)
     *   upgradeTierString  (string) : Human-readable tier name (e.g. "tier 1")
     *
     * What this script does:
     *   Calls the Mix It Up Run Command API so Mix It Up can handle the
     *   on-stream reaction (alert, chat callout, etc.).
     *
     * Operator steps:
     *   1. Paste this script into the "Subscription Prime Paid Upgrade" Streamer.bot action.
     *   2. Wire the action to: Twitch → Subscriptions → Prime Paid Upgrade
     *   3. Confirm your Streamer.bot version is 0.2.5 or later — this trigger does not
     *      exist in earlier versions.
     *   4. Replace MIXITUP_COMMAND_ID below with your real Mix It Up command ID.
     *   5. Expand BuildArguments / BuildSpecialIdentifiers when you decide
     *      which trigger args to forward (e.g. user, upgradeTierString).
     */

    private const string SCRIPT_NAME = "Core - Subscription Prime Paid Upgrade";

    // OPERATOR: Replace with your real Mix It Up command ID for Prime-to-paid upgrades.
    private const string MIXITUP_COMMAND_ID = "REPLACE_WITH_CORE_SUBSCRIPTION_PRIME_PAID_UPGRADE_COMMAND_ID";

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
        // Available args: user, userId, systemMessage, upgradeTier, upgradeTierString.
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
