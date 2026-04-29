using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // Runtime source of truth: Actions/Commanders/The Director/README.md
    // Shared names/constants reference: Actions/SHARED-CONSTANTS.md
    private const string ARG_USER = "user";
    private const string VAR_CURRENT_THE_DIRECTOR = "current_the_director";

    private const string VAR_THE_DIRECTOR_AWARD_COUNT = "the_director_award_count";
    private const string VAR_THE_DIRECTOR_AWARD_HIGH_SCORE = "the_director_award_high_score";
    private const string VAR_THE_DIRECTOR_AWARD_HIGH_SCORE_USER = "the_director_award_high_score_user";

    private const string VAR_THE_DIRECTOR_CHECKCHAT_NEXT_ALLOWED_UTC = "the_director_checkchat_next_allowed_utc";
    private const string VAR_THE_DIRECTOR_TOAD_NEXT_ALLOWED_UTC = "the_director_toad_next_allowed_utc";

    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_COMMAND_ID = "51146998-c2bc-4f46-b6a8-13069565a562";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";

    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    public bool Execute()
    {
        string newDirector = string.Empty;
        CPH.TryGetArg(ARG_USER, out newDirector);

        if (string.IsNullOrWhiteSpace(newDirector))
            return true;

        newDirector = newDirector.Trim();

        string previousDirector = CPH.GetGlobalVar<string>(VAR_CURRENT_THE_DIRECTOR, false) ?? string.Empty;
        int previousCount = CPH.GetGlobalVar<int?>(VAR_THE_DIRECTOR_AWARD_COUNT, false) ?? 0;

        int existingHighScore = CPH.GetGlobalVar<int?>(VAR_THE_DIRECTOR_AWARD_HIGH_SCORE, true) ?? 0;

        if (!string.IsNullOrWhiteSpace(previousDirector) && previousCount > existingHighScore)
        {
            // Persist high score data so it survives Streamer.bot restarts.
            CPH.SetGlobalVar(VAR_THE_DIRECTOR_AWARD_HIGH_SCORE, previousCount, true);
            CPH.SetGlobalVar(VAR_THE_DIRECTOR_AWARD_HIGH_SCORE_USER, previousDirector.Trim(), true);

            CPH.SendMessage($"🏆 New Director award record! {previousDirector} finished with {previousCount} award(s)! 🎬");
        }

        CPH.SetGlobalVar(VAR_CURRENT_THE_DIRECTOR, newDirector, false);
        CPH.SetGlobalVar(VAR_THE_DIRECTOR_AWARD_COUNT, 0, false);
        CPH.SetGlobalVar(VAR_THE_DIRECTOR_CHECKCHAT_NEXT_ALLOWED_UTC, 0L, false);
        CPH.SetGlobalVar(VAR_THE_DIRECTOR_TOAD_NEXT_ALLOWED_UTC, 0L, false);

        // Fire the Mix It Up redeem command after state is updated so downstream logic sees the new director.
        TriggerMixItUpCommand(
            MIXITUP_COMMAND_ID,
            "The Director Redeem",
            arguments: newDirector,
            specialIdentifiers: new { user = newDirector, commander = newDirector });

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
