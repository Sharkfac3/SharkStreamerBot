using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // Runtime source of truth: Actions/Commanders/Water Wizard/README.md
    // Shared names/constants reference: Actions/SHARED-CONSTANTS.md
    private const string ARG_USER = "user";
    private const string VAR_CURRENT_WATER_WIZARD = "current_water_wizard";

    private const string VAR_WATER_WIZARD_HAIL_COUNT = "water_wizard_hail_count";
    private const string VAR_WATER_WIZARD_HAIL_HIGH_SCORE = "water_wizard_hail_high_score";
    private const string VAR_WATER_WIZARD_HAIL_HIGH_SCORE_USER = "water_wizard_hail_high_score_user";

    private const string VAR_WATER_WIZARD_HYDRATE_NEXT_ALLOWED_UTC = "water_wizard_hydrate_next_allowed_utc";
    private const string VAR_WATER_WIZARD_ORB_NEXT_ALLOWED_UTC = "water_wizard_orb_next_allowed_utc";

    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_COMMAND_ID = "d5452a4f-1bf3-4ce8-a6d8-dd7a74887752";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";

    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    public bool Execute()
    {
        string newWizard = string.Empty;
        CPH.TryGetArg(ARG_USER, out newWizard);

        if (string.IsNullOrWhiteSpace(newWizard))
            return true;

        newWizard = newWizard.Trim();

        // Finalize outgoing tenure before replacing the slot owner.
        string previousWizard = CPH.GetGlobalVar<string>(VAR_CURRENT_WATER_WIZARD, false) ?? string.Empty;
        int previousCount = CPH.GetGlobalVar<int?>(VAR_WATER_WIZARD_HAIL_COUNT, false) ?? 0;

        int existingHighScore = CPH.GetGlobalVar<int?>(VAR_WATER_WIZARD_HAIL_HIGH_SCORE, true) ?? 0;

        if (!string.IsNullOrWhiteSpace(previousWizard) && previousCount > existingHighScore)
        {
            // Persist high score data so it survives Streamer.bot restarts.
            CPH.SetGlobalVar(VAR_WATER_WIZARD_HAIL_HIGH_SCORE, previousCount, true);
            CPH.SetGlobalVar(VAR_WATER_WIZARD_HAIL_HIGH_SCORE_USER, previousWizard.Trim(), true);

            CPH.SendMessage($"🏆 New Water Wizard hail record! {previousWizard} finished with {previousCount} hail(s)! 🌊");
        }

        CPH.SetGlobalVar(VAR_CURRENT_WATER_WIZARD, newWizard, false);
        CPH.SetGlobalVar(VAR_WATER_WIZARD_HAIL_COUNT, 0, false);

        // New commander starts without Water Wizard cooldown debt.
        CPH.SetGlobalVar(VAR_WATER_WIZARD_HYDRATE_NEXT_ALLOWED_UTC, 0L, false);
        CPH.SetGlobalVar(VAR_WATER_WIZARD_ORB_NEXT_ALLOWED_UTC, 0L, false);

        // Fire the Mix It Up redeem command after state is updated so downstream logic sees the new wizard.
        TriggerMixItUpCommand(
            MIXITUP_COMMAND_ID,
            "Water Wizard Redeem",
            arguments: newWizard,
            specialIdentifiers: new { user = newWizard, commander = newWizard });

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
