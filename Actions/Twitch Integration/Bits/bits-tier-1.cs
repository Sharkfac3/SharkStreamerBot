using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

public class CPHInline
{
    /*
     * Purpose:
     * - Tier 1 (highest) bits cheer bridge to Mix It Up command API.
     *
     * Expected trigger/input:
     * - Streamer.bot Trigger: Twitch -> Chat -> Cheer (Tier 1 action wiring).
     * - Reads: message (fallback: rawInput).
     *
     * Required runtime variables:
     * - None.
     *
     * Key outputs/side effects:
     * - POSTs sanitized cheer text to Mix It Up REST API command endpoint.
     * - Removes CheerXXX tokens before forwarding.
     * - Waits based on text length so TTS can finish before next queue item.
     *
     * Operator notes:
     * - Replace MIXITUP_COMMAND_ID with the Tier 1 command ID from Mix It Up.
     */

    // Mix It Up local API base URL (default local host/port for Mix It Up app).
    private const string MIXITUP_BASE_URL = "http://localhost:8911";

    // Placeholder command ID for this tier.
    // IMPORTANT: Replace before using in production.
    private const string MIXITUP_COMMAND_ID = "REPLACE_WITH_TIER_1_COMMAND_ID";

    // Reuse one HttpClient instance (best practice for repeated HTTP calls).
    private static readonly HttpClient Http = new HttpClient();

    public bool Execute()
    {
        try
        {
            // 1) Read cheer text from Streamer.bot args.
            // "message" is expected from chat triggers, but we keep "rawInput" as a safe fallback.
            string rawMessage = GetArg("message");
            if (string.IsNullOrWhiteSpace(rawMessage))
            {
                rawMessage = GetArg("rawInput");
            }

            // 2) Remove Twitch Cheer tokens like Cheer100, Cheer5000, etc.
            // Tier 1 forwards full sanitized message (no word cap).
            string finalMessage = SanitizeCheerMessage(rawMessage);

            // 3) Build endpoint URL for Mix It Up command trigger.
            string url = $"{MIXITUP_BASE_URL.TrimEnd('/')}/api/v2/commands/{MIXITUP_COMMAND_ID}";

            // 4) Send payload to Mix It Up.
            // Per Mix It Up runCommand API, text must be sent in "Arguments".
            string payload = JsonSerializer.Serialize(new
            {
                Platform = "Twitch",
                Arguments = finalMessage,
                IgnoreRequirements = false
            });
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = Http.PostAsync(url, content).GetAwaiter().GetResult();

            // 5) On failure: log warning (do not throw). Keep action flow stable.
            if (!response.IsSuccessStatusCode)
            {
                CPH.LogWarn($"[Bits Tier 1] Mix It Up call failed: {(int)response.StatusCode} {response.ReasonPhrase}");
            }
            else
            {
                // 6) On success: wait long enough for Mix It Up prep + speech.
                // This helps prevent overlap when many cheers fire quickly.
                int waitMs = CalculateReadoutWaitMs(finalMessage);
                CPH.Wait(waitMs);
            }
        }
        catch (Exception ex)
        {
            // Defensive catch: avoid crashing the action queue.
            CPH.LogError($"[Bits Tier 1] Exception while calling Mix It Up: {ex}");
        }

        // Always return true so Streamer.bot can continue action processing.
        return true;
    }

    /// <summary>
    /// Safely reads an argument from Streamer.bot.
    /// Returns empty string when missing/null.
    /// </summary>
    private string GetArg(string key)
    {
        if (CPH.TryGetArg(key, out string value) && !string.IsNullOrEmpty(value))
        {
            return value;
        }

        return string.Empty;
    }

    /// <summary>
    /// Removes all Cheer### tokens (case-insensitive),
    /// then normalizes whitespace to a single space.
    /// </summary>
    private string SanitizeCheerMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return string.Empty;
        }

        // Remove words like Cheer100, cheer500, CHEER1000, etc.
        string noCheerTokens = Regex.Replace(message, @"\bcheer\d+\b", "", RegexOptions.IgnoreCase);

        // Collapse repeated spaces/tabs/newlines and trim ends.
        string collapsedWhitespace = Regex.Replace(noCheerTokens, @"\s+", " ").Trim();
        return collapsedWhitespace;
    }

    /// <summary>
    /// Estimates wait duration for TTS so queue items don't overlap.
    /// Formula:
    /// - 3000ms prep time (Mix It Up needs startup time)
    /// - 400ms per word (approx speech time)
    /// - 500ms tail buffer
    /// </summary>
    private int CalculateReadoutWaitMs(string message)
    {
        const int basePrepMs = 3000;
        const int msPerWord = 400;
        const int tailBufferMs = 500;

        int wordCount = CountWords(message);
        return basePrepMs + (wordCount * msPerWord) + tailBufferMs;
    }

    /// <summary>
    /// Counts words by splitting on spaces and ignoring empty entries.
    /// </summary>
    private int CountWords(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return 0;
        }

        return message.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
