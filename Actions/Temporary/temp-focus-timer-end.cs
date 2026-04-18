using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (Temporary timer bridge)
    // Keep these names aligned with:
    // - Actions/Temporary/temp-focus-timer-start.cs
    // - Actions/SHARED-CONSTANTS.md
    private const string TIMER_TEMP_FOCUS = "Temp Focus Timer";

    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_CAPTAIN_STRETCH_LOCK_IN_TIMER_END_COMMAND_ID = "REPLACE_WITH_CAPTAIN_STRETCH_LOCK_IN_TIMER_END_COMMAND_ID";

    private static readonly HttpClient Http = new HttpClient();

    /*
     * Purpose:
     * - Temporary timer-end bridge for the Temp Focus Timer.
     * - Runs the Mix It Up action group named "Commander - Captain Stretch - Lock In Timer - End".
     *
     * Expected trigger/input:
     * - Streamer.bot timer-end trigger for timer: Temp Focus Timer.
     * - No chat input required.
     *
     * Required runtime variables:
     * - None.
     *
     * Key outputs/side effects:
     * - POSTs to the local Mix It Up command API when the timer completes.
     *
     * Operator notes:
     * - Mix It Up action group ID was resolved from Tools/MixItUp/Api/data/mixitup-commands.txt.
     * - Wire this action to the timer-end event for Temp Focus Timer.
     */
    public bool Execute()
    {
        CPH.LogWarn("[Temporary Temp Focus Timer End] Temp Focus Timer completed.");
        TriggerMixItUpCommand(
            MIXITUP_CAPTAIN_STRETCH_LOCK_IN_TIMER_END_COMMAND_ID,
            "Temporary Temp Focus Timer End");
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
