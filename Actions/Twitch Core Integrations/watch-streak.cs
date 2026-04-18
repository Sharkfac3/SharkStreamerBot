using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    /*
     * Script: watch-streak.cs
     * Event:  Watch Streak — fires when a viewer shares their consecutive
     *         stream watch streak in chat
     *
     * Streamer.bot trigger:
     *   Twitch → Chat → Watch Streak  (requires Chat Client v0.2.4+)
     *
     * Available trigger args (read via CPH.TryGetArg):
     *   user             (string) : Display name of the viewer
     *   userId           (string) : Twitch user ID
     *   watchStreak      (int)    : Number of consecutive streams watched
     *   watchStreakId     (string) : Unique identifier for this streak event
     *   copoReward       (int)    : Channel points Twitch awarded for the streak
     *   systemMessage    (string) : Twitch's system message (e.g. "User watched 5 consecutive streams...")
     *
     * What this script does:
     *   Reads the watch streak data from the trigger and forwards it to
     *   Mix It Up via special identifiers so Mix It Up can reference them
     *   by name in alerts, chat messages, or other actions.
     *
     * Special identifiers sent to Mix It Up:
     *   watchstreakuser    — display name of the viewer who shared the streak
     *   watchstreakmessage — Twitch's system message about the streak
     *   watchstreakcount   — the number of consecutive streams watched
     *
     * Operator steps:
     *   1. Paste this script into a new "Watch Streak" Streamer.bot action.
     *   2. Wire the action to: Twitch → Chat → Watch Streak
     *   3. Confirm MIXITUP_COMMAND_ID still matches the current Mix It Up export.
     *   4. In Mix It Up, create a command that references:
     *      $watchstreakuser, $watchstreakmessage, $watchstreakcount
     */

    private const string SCRIPT_NAME = "Core - Watch Streak";

    // Mix It Up command ID from the current Mix It Up export.
    private const string MIXITUP_COMMAND_ID = "d2d21a86-189c-4a52-8df2-f9d46141af3d";

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

            // Read streak data from trigger args.
            string user = "";
            CPH.TryGetArg("user", out user);
            user = user ?? "";

            int watchStreak = 0;
            CPH.TryGetArg("watchStreak", out watchStreak);

            string systemMessage = "";
            CPH.TryGetArg("systemMessage", out systemMessage);
            systemMessage = systemMessage ?? "";

            CPH.LogWarn($"[{SCRIPT_NAME}] {user} shared a watch streak of {watchStreak} streams.");

            // Build special identifiers so Mix It Up can reference them by name.
            // In Mix It Up, use: $watchstreakuser, $watchstreakmessage, $watchstreakcount
            // NOTE: Mix It Up special identifier keys must be lowercase with no spaces.
            object specialIdentifiers = new
            {
                watchstreakuser = user,
                watchstreakmessage = systemMessage,
                watchstreakcount = watchStreak.ToString()
            };

            RunMixItUpCommand(string.Empty, specialIdentifiers);
        }
        catch (Exception ex)
        {
            CPH.LogError($"[{SCRIPT_NAME}] Exception while calling Mix It Up: {ex}");
        }

        return true;
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
