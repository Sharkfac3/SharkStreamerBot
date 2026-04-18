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
    // - Actions/Twitch Bits Integrations/message-effects.cs
    private const string ARG_MESSAGE_STRIPPED = "messageStripped";
    private const string ARG_MESSAGE = "message";
    private const string ARG_RAW_INPUT = "rawInput";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";
    private const int WAIT_BASE_PREP_MS = 3000;
    private const int WAIT_MS_PER_WORD = 400;
    private const int WAIT_TAIL_BUFFER_MS = 500;

    /*
     * Purpose:
     * - Tier 4 (lowest) bits cheer bridge to Mix It Up command API.
     *
     * Expected trigger/input:
     * - Streamer.bot Trigger: Twitch -> Chat -> Cheer (Tier 4 action wiring).
     * - Reads: messageStripped (fallback: message, then rawInput).
     *
     * Required runtime variables:
     * - None.
     *
     * Key outputs/side effects:
     * - POSTs cheer text to Mix It Up REST API command endpoint.
     * - Prefers Streamer.bot's messageStripped value so CheerXXX tokens are already removed.
     * - Limits forwarded text to 10 words.
     * - Waits based on text length so TTS can finish before next queue item.
     *
     * Operator notes:
     * - MIXITUP_COMMAND_ID is configured for the current Tier 4 command.
     */

    // Mix It Up local API base URL (default local host/port for Mix It Up app).
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";

    // Verified Mix It Up command ID from Tools/MixItUp/Api/data/mixitup-commands.txt
    // Action Group: Twitch - Bits - Tier 4
    private const string MIXITUP_COMMAND_ID = "35405bfe-660f-46f2-bec6-8a1da9ec1af2";

    // Tier 4 maximum number of words sent to Mix It Up.
    private const int MAX_WORDS = 10;

    // Reuse one HttpClient instance (best practice for repeated HTTP calls).
    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    public bool Execute()
    {
        try
        {
            // 1) Read cheer text from Streamer.bot args.
            // We prefer messageStripped because Streamer.bot already removes CheerXXX tokens for us.
            string cleanedMessage = GetArg(ARG_MESSAGE_STRIPPED);
            if (string.IsNullOrWhiteSpace(cleanedMessage))
            {
                cleanedMessage = GetArg(ARG_MESSAGE);
            }
            if (string.IsNullOrWhiteSpace(cleanedMessage))
            {
                cleanedMessage = GetArg(ARG_RAW_INPUT);
            }

            // 2) Enforce tier cap (Tier 4 => first 10 words only).
            string finalMessage = LimitToWordCount(cleanedMessage, MAX_WORDS);

            // 3) Forward the cheer text to Mix It Up.
            bool mixItUpTriggered = TriggerMixItUpReadout(
                MIXITUP_COMMAND_ID,
                "Bits Tier 4",
                finalMessage
            );

            // 4) Only wait when Mix It Up accepted the request.
            if (mixItUpTriggered)
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
    /// Sends a readout-style request to Mix It Up using the standard payload shape.
    /// This helper is intentionally kept in sync with the other bits-tier scripts so
    /// all cheer readouts follow the same API and waiting behavior.
    /// </summary>
    private bool TriggerMixItUpReadout(string commandId, string logPrefix, string arguments)
    {
        if (string.IsNullOrWhiteSpace(commandId) ||
            commandId.StartsWith("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase))
        {
            CPH.LogWarn($"[{logPrefix}] Mix It Up command ID is not configured.");
            return false;
        }

        string url = $"{MIXITUP_API_BASE_URL.TrimEnd('/')}/api/v2/commands/{commandId}";
        string payload = JsonSerializer.Serialize(new
        {
            Platform = MIXITUP_PLATFORM_TWITCH,
            Arguments = arguments ?? string.Empty,
            SpecialIdentifiers = new { },
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
