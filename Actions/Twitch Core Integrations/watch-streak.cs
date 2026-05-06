// ACTION-CONTRACT: Actions/Twitch Core Integrations/AGENTS.md#watch-streak.cs
// ACTION-CONTRACT-SHA256: 5ea767abdcc719daf08ab5372e918cb8f50d7e0f27b9b71d41b6e9d3e5422411

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
     *   watchStreakId    (string) : Unique identifier for this streak event
     *   copoReward       (int)    : Channel points Twitch awarded for the streak
     *   message          (string) : The viewer's own shared watch streak chat message
     *   systemMessage    (string) : Twitch's system message (available from Twitch, but no longer forwarded)
     *
     * What this script does:
     *   Reads the watch streak data from the trigger and forwards it to
     *   Mix It Up via special identifiers so Mix It Up can reference them
     *   by name in alerts, chat messages, or other actions.
     *
     * Special identifiers sent to Mix It Up:
     *   watchstreakuser    — display name of the viewer who shared the streak
     *   watchstreakmessage — the viewer's own shared streak message (empty when no message was provided)
     *   watchstreaktype    — "message" when viewer text exists, otherwise "none"
     *   watchstreakcount   — the number of consecutive streams watched
     *
     * Operator steps:
     *   1. Paste this script into a new "Watch Streak" Streamer.bot action.
     *   2. Wire the action to: Twitch → Chat → Watch Streak
     *   3. Confirm MIXITUP_COMMAND_ID still matches the current Mix It Up export.
     *   4. In Mix It Up, create/update a command that references:
     *      $watchstreakuser, $watchstreakmessage, $watchstreaktype,
     *      $watchstreakcount
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

            string message = "";
            CPH.TryGetArg("message", out message);
            message = message ?? "";

            // Keep the user message exactly as provided.
            // If Twitch/Streamer.bot sends no viewer-authored message,
            // forward an empty string and mark the type as "none" so
            // Mix It Up can decide how to branch its response.
            string watchStreakMessage = message;
            string watchStreakType = string.IsNullOrWhiteSpace(watchStreakMessage)
                ? "none"
                : "message";

            CPH.LogWarn($"[{SCRIPT_NAME}] {user} shared a watch streak of {watchStreak} streams.");

            // Build special identifiers so Mix It Up can reference them by name.
            // In Mix It Up, use: $watchstreakuser, $watchstreakmessage,
            // $watchstreaktype, $watchstreakcount
            // NOTE: Mix It Up special identifier keys must be lowercase with no spaces.
            object specialIdentifiers = new
            {
                watchstreakuser = user,
                watchstreakmessage = watchStreakMessage,
                watchstreaktype = watchStreakType,
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
