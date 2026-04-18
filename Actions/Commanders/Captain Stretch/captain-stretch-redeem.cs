using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (Commanders)
    // Keep these names aligned with:
    // - Actions/Commanders/Captain Stretch/captain-stretch-redeem.cs
    // - Actions/Commanders/The Director/the-director-redeem.cs
    // - Actions/Commanders/Water Wizard/water-wizard-redeem.cs
    private const string ARG_USER = "user";
    private const string VAR_CURRENT_CAPTAIN_STRETCH = "current_captain_stretch";

    // Captain Stretch support-score tracking.
    private const string VAR_CAPTAIN_STRETCH_THANK_COUNT = "captain_stretch_thank_count";
    private const string VAR_CAPTAIN_STRETCH_THANK_HIGH_SCORE = "captain_stretch_thank_high_score";
    private const string VAR_CAPTAIN_STRETCH_THANK_HIGH_SCORE_USER = "captain_stretch_thank_high_score_user";

    // Captain Stretch command cooldown tracking.
    private const string VAR_CAPTAIN_STRETCH_STRETCH_NEXT_ALLOWED_UTC = "captain_stretch_stretch_next_allowed_utc";
    private const string VAR_CAPTAIN_STRETCH_SHRIMP_NEXT_ALLOWED_UTC = "captain_stretch_shrimp_next_allowed_utc";

    // Mix It Up wiring for the commander redeem celebration.
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_COMMAND_ID = "REPLACE_WITH_CAPTAIN_STRETCH_REDEEM_COMMAND_ID";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";

    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    /*
     * Purpose:
     * - Assigns the current Captain Stretch commander slot to the redeeming user.
     * - Before replacing the slot owner, finalizes the previous captain's thank score.
     * - If previous score beats the stored high score, writes a new persistent high score and announces it.
     * - Triggers the Captain Stretch Mix It Up redeem command for the new commander takeover.
     *
     * Expected trigger/input:
     * - Commander redeem action for Captain Stretch.
     * - Reads: user
     *
     * Required runtime variables:
     * - Reads/Writes current_captain_stretch
     * - Reads/Writes captain_stretch_thank_count
     * - Reads/Writes captain_stretch_stretch_next_allowed_utc
     * - Reads/Writes captain_stretch_shrimp_next_allowed_utc
     * - Reads/Writes (persisted) captain_stretch_thank_high_score
     * - Reads/Writes (persisted) captain_stretch_thank_high_score_user
     *
     * Key outputs/side effects:
     * - Updates global var current_captain_stretch with latest valid username.
     * - Resets captain_stretch_thank_count to 0 for the new captain tenure.
     * - Resets Captain Stretch command cooldowns so the new captain starts fresh.
     * - Announces new high score in chat when beaten.
     * - Calls Mix It Up command "Commander - Captain Stretch - Redeem" with the new captain name.
     */
    public bool Execute()
    {
        // Start with a safe default in case trigger does not include user.
        string newCaptain = string.Empty;
        CPH.TryGetArg(ARG_USER, out newCaptain);

        // If missing/blank, no-op safely.
        if (string.IsNullOrWhiteSpace(newCaptain))
            return true;

        newCaptain = newCaptain.Trim();

        // Finalize the outgoing captain (if any) before assigning the new captain.
        string previousCaptain = CPH.GetGlobalVar<string>(VAR_CURRENT_CAPTAIN_STRETCH, false) ?? string.Empty;
        int previousCount = CPH.GetGlobalVar<int?>(VAR_CAPTAIN_STRETCH_THANK_COUNT, false) ?? 0;

        int existingHighScore = CPH.GetGlobalVar<int?>(VAR_CAPTAIN_STRETCH_THANK_HIGH_SCORE, true) ?? 0;

        if (!string.IsNullOrWhiteSpace(previousCaptain) && previousCount > existingHighScore)
        {
            // Persist high score data so it survives Streamer.bot restarts.
            CPH.SetGlobalVar(VAR_CAPTAIN_STRETCH_THANK_HIGH_SCORE, previousCount, true);
            CPH.SetGlobalVar(VAR_CAPTAIN_STRETCH_THANK_HIGH_SCORE_USER, previousCaptain.Trim(), true);

            CPH.SendMessage($"🏆 New Captain Stretch thank record! {previousCaptain} finished with {previousCount} thank(s)! 💪");
        }

        // Save current commander owner for Captain Stretch slot.
        CPH.SetGlobalVar(VAR_CURRENT_CAPTAIN_STRETCH, newCaptain, false);

        // Reset active tenure counter for the new captain.
        CPH.SetGlobalVar(VAR_CAPTAIN_STRETCH_THANK_COUNT, 0, false);

        // Reset all Captain Stretch command cooldowns for the new tenure.
        CPH.SetGlobalVar(VAR_CAPTAIN_STRETCH_STRETCH_NEXT_ALLOWED_UTC, 0L, false);
        CPH.SetGlobalVar(VAR_CAPTAIN_STRETCH_SHRIMP_NEXT_ALLOWED_UTC, 0L, false);

        // Fire the Mix It Up redeem command after state is updated so downstream logic sees the new captain.
        TriggerMixItUpCommand(
            MIXITUP_COMMAND_ID,
            "Captain Stretch Redeem",
            arguments: newCaptain,
            specialIdentifiers: new { user = newCaptain, commander = newCaptain });

        return true;
    }

    private bool TriggerMixItUpCommand(
        string commandId,
        string logPrefix,
        string arguments = "",
        object specialIdentifiers = null)
    {
        if (string.IsNullOrWhiteSpace(commandId) ||
            commandId.StartsWith("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase))
        {
            CPH.LogWarn($"[{logPrefix}] Mix It Up command ID is not configured.");
            return false;
        }

        try
        {
            string url = $"{MIXITUP_API_BASE_URL.TrimEnd('/')}/api/v2/commands/{commandId}";
            string payload = JsonSerializer.Serialize(new
            {
                Platform = MIXITUP_PLATFORM_TWITCH,
                Arguments = arguments ?? "",
                SpecialIdentifiers = specialIdentifiers ?? new { },
                IgnoreRequirements = false
            });

            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = MIXITUP_HTTP_CLIENT.PostAsync(url, content).GetAwaiter().GetResult();

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
