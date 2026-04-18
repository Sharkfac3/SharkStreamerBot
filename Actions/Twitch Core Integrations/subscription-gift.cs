using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    /*
     * Purpose:
     * - Handles all gift subscription events in a single script.
     * - Routes correctly between solo gifts, gift bomb aggregates, and the
     *   individual per-recipient Gift Subscription events that fire during a bomb.
     *
     * Background — how Twitch/Streamer.bot fires gift events:
     * - Solo gift (1 sub):
     *     Gift Subscription fires once (fromGiftBomb = false)
     * - Gift bomb (N subs):
     *     Gift Bomb fires once (aggregate — gifts = N, recipient list included)
     *     Gift Subscription fires N times (fromGiftBomb = true on each)
     *
     * Expected trigger/input:
     * - Wire this script to ONE Streamer.bot action with TWO triggers:
     *     Trigger 1: Twitch → Subscriptions → Gift Subscription
     *     Trigger 2: Twitch → Subscriptions → Gift Bomb
     * - Both triggers run this same script. The script detects which event fired
     *   by checking which runtime arguments Streamer.bot injected.
     *
     * Routing logic:
     * - Has "gifts" arg  → Gift Bomb aggregate event  → calls MIXITUP_COMMAND_ID_GIFT_BOMB
     * - fromGiftBomb = false → Solo Gift Subscription → calls MIXITUP_COMMAND_ID_GIFT_SINGLE
     * - fromGiftBomb = true  → Individual gift within bomb → SUPPRESSED
     *   (the Gift Bomb aggregate event already handles the announcement; these
     *    per-recipient fires are noise and would produce duplicate alerts)
     *
     * Required runtime variables:
     * - None.
     *
     * Key outputs/side effects:
     * - Calls one of two Mix It Up commands depending on detected event type.
     * - Suppresses (logs only) the per-recipient Gift Subscription fires from bombs.
     * - Does not interact with OBS.
     *
     * Operator notes:
     * - Confirm both Mix It Up command IDs still match your latest Mix It Up export.
     * - Expand BuildSingleArguments / BuildBombArguments when the final event
     *   field contracts are decided. Refer to README.md for available trigger args.
     * - The "gifts" arg is only injected by the Gift Bomb trigger — this is how
     *   the script distinguishes a bomb from a solo gift without any extra UI setup.
     */

    private const string SCRIPT_NAME = "Core - Subscription Gift";

    // Mix It Up command ID for a single gifted sub.
    private const string MIXITUP_COMMAND_ID_GIFT_SINGLE = "ce197b79-89d1-4943-8f74-b1a690f5a8e4";

    // Mix It Up command ID for a gift bomb aggregate.
    private const string MIXITUP_COMMAND_ID_GIFT_BOMB = "27111920-c34b-4991-b284-57d655a20195";

    private const string MIXITUP_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";

    private static readonly HttpClient Http = new HttpClient();

    public bool Execute()
    {
        try
        {
            // Step 1: Check if this is a Gift Bomb aggregate event.
            // Only the Gift Bomb trigger injects a "gifts" argument — Gift Subscription does not.
            bool isGiftBomb = CPH.TryGetArg("gifts", out int giftCount);

            if (isGiftBomb)
            {
                // This is the Gift Bomb aggregate event (fires once for the whole bomb).
                CPH.LogInfo($"[{SCRIPT_NAME}] Gift Bomb detected ({giftCount} gifts). Calling bomb command.");

                if (HasPlaceholderCommandId(MIXITUP_COMMAND_ID_GIFT_BOMB))
                {
                    CPH.LogWarn($"[{SCRIPT_NAME}] Gift bomb Mix It Up command ID is still a placeholder. Skipping call.");
                    return true;
                }

                string bombArgs = BuildBombArguments();
                object bombSpecialIds = BuildBombSpecialIdentifiers();
                RunMixItUpCommand(MIXITUP_COMMAND_ID_GIFT_BOMB, bombArgs, bombSpecialIds);
                return true;
            }

            // Step 2: This is a Gift Subscription event.
            // Check whether it was fired as part of a bomb or as a solo gift.
            CPH.TryGetArg("fromGiftBomb", out bool fromGiftBomb);

            if (fromGiftBomb)
            {
                // This is an individual per-recipient fire from a bomb event.
                // The Gift Bomb aggregate event (above) handles the announcement.
                // Suppress this to avoid N duplicate alerts for an N-sub bomb.
                CPH.LogInfo($"[{SCRIPT_NAME}] Gift Subscription within bomb — suppressed (handled by Gift Bomb event).");
                return true;
            }

            // Step 3: This is a solo Gift Subscription (fromGiftBomb = false).
            CPH.LogInfo($"[{SCRIPT_NAME}] Solo gift detected. Calling single-gift command.");

            if (HasPlaceholderCommandId(MIXITUP_COMMAND_ID_GIFT_SINGLE))
            {
                CPH.LogWarn($"[{SCRIPT_NAME}] Single-gift Mix It Up command ID is still a placeholder. Skipping call.");
                return true;
            }

            string singleArgs = BuildSingleArguments();
            object singleSpecialIds = BuildSingleSpecialIdentifiers();
            RunMixItUpCommand(MIXITUP_COMMAND_ID_GIFT_SINGLE, singleArgs, singleSpecialIds);
        }
        catch (Exception ex)
        {
            CPH.LogError($"[{SCRIPT_NAME}] Exception while processing gift event: {ex}");
        }

        return true;
    }

    // --- Solo Gift Subscription argument builders ---
    // Available args: user, userId, tier, recipientUser, recipientId, recipientUserName,
    //                 anonymous, random, cumulativeMonths, monthsGifted, fromGiftBomb (false),
    //                 subBombCount, systemMessage, totalSubsGifted, totalSubsGiftedShared.

    private string BuildSingleArguments()
    {
        // Expand this when the final event field contract is decided.
        return string.Empty;
    }

    private object BuildSingleSpecialIdentifiers()
    {
        // Expand this when the final event field contract is decided.
        return new { };
    }

    // --- Gift Bomb aggregate argument builders ---
    // Available args: user, userId, tier, anonymous, gifts, bonusGifts, systemMessage,
    //                 totalGifts, totalGiftsShared, gift.recipientId#, gift.recipientUser#,
    //                 gift.recipientUserName# (0-based index suffix).

    private string BuildBombArguments()
    {
        // Expand this when the final event field contract is decided.
        return string.Empty;
    }

    private object BuildBombSpecialIdentifiers()
    {
        // Expand this when the final event field contract is decided.
        return new { };
    }

    // --- Shared helpers ---

    private void RunMixItUpCommand(string commandId, string arguments, object specialIdentifiers)
    {
        string url = $"{MIXITUP_BASE_URL.TrimEnd('/')}/api/v2/commands/{commandId}";
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
