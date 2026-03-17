using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (Bits feature)
    // Keep shared names aligned across:
    // - Actions/Twitch Bits Integrations/bits-tier-1.cs
    // - Actions/Twitch Bits Integrations/bits-tier-2.cs
    // - Actions/Twitch Bits Integrations/bits-tier-3.cs
    // - Actions/Twitch Bits Integrations/bits-tier-4.cs
    private const string ARG_MESSAGE_STRIPPED = "messageStripped";
    private const string ARG_MESSAGE = "message";
    private const string ARG_RAW_INPUT = "rawInput";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";
    private const int WAIT_BASE_PREP_MS = 3000;
    private const int WAIT_MS_PER_WORD = 400;
    private const int WAIT_TAIL_BUFFER_MS = 500;

    /*
     * Purpose:
     * - Tier 1 (highest) bits cheer bridge to Mix It Up command API.
     *
     * Expected trigger/input:
     * - Streamer.bot Trigger: Twitch -> Chat -> Cheer (Tier 1 action wiring).
     * - Reads: messageStripped (fallback: message, then rawInput).
     *
     * Required runtime variables:
     * - None.
     *
     * Key outputs/side effects:
     * - POSTs cheer text to Mix It Up REST API command endpoint.
     * - Prefers Streamer.bot's messageStripped value so CheerXXX tokens are already removed.
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
            // We prefer messageStripped because Streamer.bot already removes CheerXXX tokens for us.
            string finalMessage = GetArg(ARG_MESSAGE_STRIPPED);
            if (string.IsNullOrWhiteSpace(finalMessage))
            {
                finalMessage = GetArg(ARG_MESSAGE);
            }
            if (string.IsNullOrWhiteSpace(finalMessage))
            {
                finalMessage = GetArg(ARG_RAW_INPUT);
            }

            // 3) Build endpoint URL for Mix It Up command trigger.
            string url = $"{MIXITUP_BASE_URL.TrimEnd('/')}/api/v2/commands/{MIXITUP_COMMAND_ID}";

            // 4) Send payload to Mix It Up.
            // Per Mix It Up runCommand API, text must be sent in "Arguments".
            string payload = JsonSerializer.Serialize(new
            {
                Platform = MIXITUP_PLATFORM_TWITCH,
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
    /// Estimates wait duration for TTS so queue items don't overlap.
    /// Formula:
    /// - 3000ms prep time (Mix It Up needs startup time)
    /// - 400ms per word (approx speech time)
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
