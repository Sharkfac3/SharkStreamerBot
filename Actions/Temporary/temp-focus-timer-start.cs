using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (Temporary timer bridge)
    // Keep these names aligned with:
    // - Actions/Temporary/temp-focus-timer-end.cs
    // - Actions/Twitch Core Integrations/stream-start.cs
    // - Actions/SHARED-CONSTANTS.md
    private const string TIMER_TEMP_FOCUS = "Temp Focus Timer";

    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_TEMPORARY_LOCK_IN_TIMER_COMMAND_ID = "REPLACE_WITH_TEMPORARY_LOCK_IN_TIMER_COMMAND_ID";

    private static readonly HttpClient Http = new HttpClient();

    /*
     * Purpose:
     * - Temporary voice-command bridge that kicks off the lock-in sequence.
     * - Runs the Mix It Up action group named "Temporary Lock In Timer".
     * - Enables the existing Streamer.bot timer named "Temp Focus Timer".
     *
     * Expected trigger/input:
     * - Voice command action in Streamer.bot.
     * - No chat input or timer arguments required.
     *
     * Required runtime variables:
     * - None.
     *
     * Key outputs/side effects:
     * - POSTs to the local Mix It Up command API.
     * - Enables the Temp Focus Timer countdown using its already-configured interval.
     *
     * Operator notes:
     * - Mix It Up action group ID was resolved from Tools/MixItUp/Api/data/mixitup-commands.txt.
     * - Tie this action to the intended voice command.
     */
    public bool Execute()
    {
        TriggerMixItUpCommand(
            MIXITUP_TEMPORARY_LOCK_IN_TIMER_COMMAND_ID,
            "Temporary Temp Focus Timer Start");

        CPH.LogWarn("[Temporary Temp Focus Timer Start] Enabling timer 'Temp Focus Timer'.");
        CPH.EnableTimer(TIMER_TEMP_FOCUS);
        return true;
    }

    private bool TriggerMixItUpCommand(string commandId, string logPrefix, string arguments = "", object specialIdentifiers = null)
    {
        if (string.IsNullOrWhiteSpace(commandId) || commandId.StartsWith("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase))
        {
            CPH.LogWarn($"[{logPrefix}] Mix It Up command ID is not configured.");
            return false;
        }

        try
        {
            string url = $"{MIXITUP_API_BASE_URL.TrimEnd('/')}/api/v2/commands/{commandId}";
            string payload = JsonSerializer.Serialize(new
            {
                Platform = "Twitch",
                Arguments = arguments ?? string.Empty,
                SpecialIdentifiers = specialIdentifiers ?? new { },
                IgnoreRequirements = false
            });

            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = Http.PostAsync(url, content).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                CPH.LogWarn($"[{logPrefix}] Mix It Up call failed: {(int)response.StatusCode} {response.ReasonPhrase}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            CPH.LogError($"[{logPrefix}] Exception while calling Mix It Up: {ex}");
            return false;
        }
    }
}
