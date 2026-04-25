using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    /*
     * Purpose:
     * - Base Twitch integration bridge for a Sub Counter Rollover event.
     * - Fires when Streamer.bot's internal sub counter reaches a configured rollover threshold.
     *
     * Expected trigger/input:
     * - Wire this script to the Streamer.bot Twitch event: Subscriptions → Sub Counter Rollover.
     * - Configure the rollover threshold in Streamer.bot's sub counter settings.
     *
     * Required runtime variables:
     * - None.
     *
     * Key outputs/side effects:
     * - Calls the Mix It Up Run Command API when a real command ID is configured.
     * - Sends empty Arguments and populated SpecialIdentifiers for Mix It Up branching.
     * - Does not interact with OBS.
     *
     * Trigger-specific arguments (available via CPH.TryGetArg):
     * - rollover      (number) : The configured rollover threshold (e.g. 50)
     * - rolloverCount (number) : How many times the threshold has been reached (e.g. 3)
     * - subCounter    (number) : The current subscription counter value (e.g. 20)
     *
     * Note: This trigger does NOT include Twitch Chat or Twitch User variable groups —
     *       it is a counter event, not a per-user event. Only General and Twitch Broadcaster
     *       shared groups are available.
     *
     * Operator notes:
     * - Replace MIXITUP_COMMAND_ID before production use.
     * - In Mix It Up, reference $subtype, $subrollover, $subrollovercount, and $subcounter.
     * - Set your rollover threshold in Streamer.bot UI: Actions → Sub Counter settings.
     */

    private const string SCRIPT_NAME = "Core - Subscription Counter Rollover";
    private const string MIXITUP_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";
    private const string MIXITUP_COMMAND_ID = "REPLACE_WITH_CORE_SUBSCRIPTION_COUNTER_ROLLOVER_COMMAND_ID";

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
        // Preserve existing Mix It Up command compatibility: this counter event
        // does not need a plain-text argument string. Structured values are sent
        // through SpecialIdentifiers instead.
        return string.Empty;
    }

    private object BuildSpecialIdentifiers()
    {
        // This trigger is a counter event, not a user event. Do not include user
        // identifiers unless Streamer.bot later documents user args for it.
        int rollover = GetIntArg("rollover");
        int rolloverCount = GetIntArg("rolloverCount");
        int subCounter = GetIntArg("subCounter");

        return new
        {
            subtype = "counterrollover",
            subrollover = rollover.ToString(),
            subrollovercount = rolloverCount.ToString(),
            subcounter = subCounter.ToString()
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
