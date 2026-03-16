using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

public class CPHInline
{
    // SYNC CONSTANTS (Bits feature)
    // Keep shared names aligned across:
    // - Actions/Twitch Bits Integrations/bits-tier-1.cs
    // - Actions/Twitch Bits Integrations/bits-tier-2.cs
    // - Actions/Twitch Bits Integrations/bits-tier-3.cs
    // - Actions/Twitch Bits Integrations/bits-tier-4.cs
    private const string ARG_MESSAGE = "message";
    private const string ARG_RAW_INPUT = "rawInput";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";
    private const string CHEER_TOKEN_REGEX = @"\bcheer\d+\b";
    private const string WHITESPACE_REGEX = @"\s+";
    private const int WAIT_BASE_PREP_MS = 3000;
    private const int WAIT_MS_PER_WORD = 400;
    private const int WAIT_TAIL_BUFFER_MS = 500;

    /*
     * Purpose:
     * - Tier 4 (lowest) bits cheer bridge to Mix It Up command API.
     *
     * Expected trigger/input:
     * - Streamer.bot Trigger: Twitch -> Chat -> Cheer (Tier 4 action wiring).
     * - Reads: message (fallback: rawInput).
     *
     * Required runtime variables:
     * - None.
     *
     * Key outputs/side effects:
     * - POSTs sanitized cheer text to Mix It Up REST API command endpoint.
     * - Removes CheerXXX tokens before forwarding.
     * - Limits forwarded text to 10 words.
     * - Waits based on text length so TTS can finish before next queue item.
     *
     * Operator notes:
     * - Replace MIXITUP_COMMAND_ID with the Tier 4 command ID from Mix It Up.
     */

    // Mix It Up local API base URL (default local host/port for Mix It Up app).
    private const string MIXITUP_BASE_URL = "http://localhost:8911";

    // Placeholder command ID for this tier.
    // IMPORTANT: Replace before using in production.
    private const string MIXITUP_COMMAND_ID = "REPLACE_WITH_TIER_4_COMMAND_ID";

    // Tier 4 maximum number of words sent to Mix It Up.
    private const int MAX_WORDS = 10;

    // Reuse one HttpClient instance (best practice for repeated HTTP calls).
    private static readonly HttpClient Http = new HttpClient();

    public bool Execute()
    {
        try
        {
            // 1) Read cheer text from Streamer.bot args.
            string rawMessage = GetArg(ARG_MESSAGE);
            if (string.IsNullOrWhiteSpace(rawMessage))
            {
                rawMessage = GetArg(ARG_RAW_INPUT);
            }

            // 2) Remove Cheer### tokens and normalize spaces.
            string cleanedMessage = SanitizeCheerMessage(rawMessage);

            // 3) Enforce tier cap (Tier 4 => first 10 words only).
            string finalMessage = LimitToWordCount(cleanedMessage, MAX_WORDS);

            // 4) Build endpoint URL for Mix It Up command trigger.
            string url = $"{MIXITUP_BASE_URL.TrimEnd('/')}/api/v2/commands/{MIXITUP_COMMAND_ID}";

            // 5) Send payload to Mix It Up.
            string payload = JsonSerializer.Serialize(new
            {
                Platform = MIXITUP_PLATFORM_TWITCH,
                Arguments = finalMessage,
                IgnoreRequirements = false
            });
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = Http.PostAsync(url, content).GetAwaiter().GetResult();

            // 6) Log API failure, otherwise wait to avoid queue overlap.
            if (!response.IsSuccessStatusCode)
            {
                CPH.LogWarn($"[Bits Tier 4] Mix It Up call failed: {(int)response.StatusCode} {response.ReasonPhrase}");
            }
            else
            {
                int waitMs = CalculateReadoutWaitMs(finalMessage);
                CPH.Wait(waitMs);
            }
        }
        catch (Exception ex)
        {
            // Defensive catch: avoid crashing the action queue.
            CPH.LogError($"[Bits Tier 4] Exception while calling Mix It Up: {ex}");
        }

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

        string noCheerTokens = Regex.Replace(message, CHEER_TOKEN_REGEX, "", RegexOptions.IgnoreCase);
        string collapsedWhitespace = Regex.Replace(noCheerTokens, WHITESPACE_REGEX, " ").Trim();
        return collapsedWhitespace;
    }

    /// <summary>
    /// Returns first maxWords words from message.
    /// If message is shorter, returns original message unchanged.
    /// </summary>
    private string LimitToWordCount(string message, int maxWords)
    {
        if (string.IsNullOrWhiteSpace(message) || maxWords <= 0)
        {
            return string.Empty;
        }

        string[] words = message.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length <= maxWords)
        {
            return message;
        }

        return string.Join(" ", words, 0, maxWords);
    }

    /// <summary>
    /// Estimates wait duration for TTS so queue items don't overlap.
    /// Formula:
    /// - 3000ms prep time
    /// - 400ms per word
    /// - 500ms tail buffer
    /// </summary>
    private int CalculateReadoutWaitMs(string message)
    {
        int wordCount = CountWords(message);
        return WAIT_BASE_PREP_MS + (wordCount * WAIT_MS_PER_WORD) + WAIT_TAIL_BUFFER_MS;
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
