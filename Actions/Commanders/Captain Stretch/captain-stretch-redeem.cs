// ACTION-CONTRACT: Actions/Commanders/Captain Stretch/AGENTS.md#captain-stretch-redeem.cs
// ACTION-CONTRACT-SHA256: 6f44931552c5f0f5ba41ab499a5ecf2ed98357683e0bac12b66cbca9d2ea9305

// Documented Mix It Up endpoint literal: http://localhost:8911/api/v2/commands/{commandId}
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // Runtime source of truth: Actions/Commanders/Captain Stretch/README.md
    // Shared names/constants reference: Actions/SHARED-CONSTANTS.md
    private const string ARG_USER = "user";
    private const string VAR_CURRENT_CAPTAIN_STRETCH = "current_captain_stretch";

    // Support-score tracking.
    private const string VAR_CAPTAIN_STRETCH_THANK_COUNT = "captain_stretch_thank_count";
    private const string VAR_CAPTAIN_STRETCH_THANK_HIGH_SCORE = "captain_stretch_thank_high_score";
    private const string VAR_CAPTAIN_STRETCH_THANK_HIGH_SCORE_USER = "captain_stretch_thank_high_score_user";

    // Command cooldown tracking.
    private const string VAR_CAPTAIN_STRETCH_STRETCH_NEXT_ALLOWED_UTC = "captain_stretch_stretch_next_allowed_utc";
    private const string VAR_CAPTAIN_STRETCH_SHRIMP_NEXT_ALLOWED_UTC = "captain_stretch_shrimp_next_allowed_utc";

    // Mix It Up redeem hook.
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_COMMAND_ID = "1facdbe6-f292-4d1b-9b66-ad71ae6de310";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";

    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    public bool Execute()
    {
        string newCaptain = string.Empty;
        CPH.TryGetArg(ARG_USER, out newCaptain);

        if (string.IsNullOrWhiteSpace(newCaptain))
            return true;

        newCaptain = newCaptain.Trim();

        // Finalize outgoing tenure before assigning the new captain.
        string previousCaptain = CPH.GetGlobalVar<string>(VAR_CURRENT_CAPTAIN_STRETCH, false) ?? string.Empty;
        int previousCount = CPH.GetGlobalVar<int?>(VAR_CAPTAIN_STRETCH_THANK_COUNT, false) ?? 0;

        int existingHighScore = CPH.GetGlobalVar<int?>(VAR_CAPTAIN_STRETCH_THANK_HIGH_SCORE, true) ?? 0;

        if (!string.IsNullOrWhiteSpace(previousCaptain) && previousCount > existingHighScore)
        {
            // Persisted high score.
            CPH.SetGlobalVar(VAR_CAPTAIN_STRETCH_THANK_HIGH_SCORE, previousCount, true);
            CPH.SetGlobalVar(VAR_CAPTAIN_STRETCH_THANK_HIGH_SCORE_USER, previousCaptain.Trim(), true);

            CPH.SendMessage($"🏆 New Captain Stretch thank record! {previousCaptain} finished with {previousCount} thank(s)! 💪");
        }

        CPH.SetGlobalVar(VAR_CURRENT_CAPTAIN_STRETCH, newCaptain, false);
        CPH.SetGlobalVar(VAR_CAPTAIN_STRETCH_THANK_COUNT, 0, false);

        // Fresh tenure starts with no Captain Stretch command cooldown debt.
        CPH.SetGlobalVar(VAR_CAPTAIN_STRETCH_STRETCH_NEXT_ALLOWED_UTC, 0L, false);
        CPH.SetGlobalVar(VAR_CAPTAIN_STRETCH_SHRIMP_NEXT_ALLOWED_UTC, 0L, false);

        // Trigger after state update so downstream logic sees the new captain.
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
