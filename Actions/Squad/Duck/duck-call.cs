using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (Duck feature)
    // Keep these names identical across:
    // - Actions/Squad/Duck/duck-main.cs
    // - Actions/Squad/Duck/duck-call.cs
    // - Actions/Squad/Duck/duck-resolve.cs
    // - Actions/Twitch Core Integrations/stream-start.cs
    private const string VAR_DUCK_EVENT_ACTIVE = "duck_event_active";
    private const string VAR_DUCK_QUACK_COUNT = "duck_quack_count";
    private const string VAR_DUCK_UNLOCKED = "duck_unlocked";
    private const string VAR_DUCK_TARGET_QUACKS = "duck_target_quacks";
    private const string VAR_DUCK_UNIQUE_QUACKERS = "duck_unique_quackers";
    private const string VAR_DUCK_UNIQUE_QUACKER_COUNT = "duck_unique_quacker_count";
    private const string TIMER_DUCK_CALL_WINDOW = "Duck - Call Window";

    // Shared mini-game lock (cross-feature).
    private const string VAR_MINIGAME_ACTIVE = "minigame_active";
    private const string VAR_MINIGAME_NAME = "minigame_name";
    private const string MINIGAME_NAME_DUCK = "duck";

    // Mix It Up unlock bridge for Duck unlock events.
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_DUCK_UNLOCK_COMMAND_ID = "c77405db-ac86-454e-bc86-5ff262da0a9a";
    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    // Shared unlock pacing rule:
    // Mix It Up usually needs a small startup window before the visible unlock payoff begins.
    // Duck's unlock sequence plays for 18 seconds, so we keep the startup buffer separate
    // from the effect duration to make the total 21-second wait obvious.
    private const int WAIT_MIXITUP_UNLOCK_STARTUP_MS = 3000;
    private const int DUCK_UNLOCK_DURATION_MS = 18000;
    private const int DUCK_UNLOCK_WAIT_MS = WAIT_MIXITUP_UNLOCK_STARTUP_MS + DUCK_UNLOCK_DURATION_MS;

    // Duck difficulty tuning.
    // The flock grows with participation, but we keep a minimum threshold so one chatter
    // still has to work for it. Every unique quacker raises the target a little.
    private const int DUCK_MINIMUM_TARGET_QUACKS = 12;
    private const int DUCK_QUACKS_PER_UNIQUE_CHATTER = 4;

    /*
     * Purpose:
     * - Counts quacks during the active Duck window and unlocks Duck immediately
     *   once the live threshold is reached.
     *
     * Expected trigger/input:
     * - Message/chat trigger while Duck event is active.
     * - Reads message (fallback rawInput).
     * - Reads userId (fallback user) to track unique participants.
     *
     * Required runtime variables:
     * - duck_event_active
     * - duck_quack_count
     * - duck_target_quacks
     * - duck_unique_quackers
     * - duck_unique_quacker_count
     * - duck_unlocked
     *
     * Key outputs/side effects:
     * - Adds the number of "quack" occurrences found in message text.
     * - Tracks unique quackers for live scaling.
     * - Ends the event immediately on success.
     */
    public bool Execute()
    {
        // Count quacks only during the active event window.
        bool active = (CPH.GetGlobalVar<bool?>(VAR_DUCK_EVENT_ACTIVE, false) ?? false);
        if (!active)
            return true;

        // Read message text; trigger payload varies by setup.
        string text = "";
        if (!CPH.TryGetArg("message", out text) || string.IsNullOrWhiteSpace(text))
        {
            CPH.TryGetArg("rawInput", out text);
        }

        if (string.IsNullOrWhiteSpace(text))
            return true;

        // Count substring matches of "quack" (case-insensitive).
        int hits = CountOccurrences(text, "quack");
        if (hits <= 0)
            return true;

        // Register unique participant before we update the live target.
        string participantKey = GetParticipantKey();
        int uniqueQuackers = RegisterUniqueQuacker(participantKey);

        int liveTarget = CalculateTarget(uniqueQuackers);
        CPH.SetGlobalVar(VAR_DUCK_TARGET_QUACKS, liveTarget, false);

        int current = (CPH.GetGlobalVar<int?>(VAR_DUCK_QUACK_COUNT, false) ?? 0);
        int updatedQuacks = current + hits;
        CPH.SetGlobalVar(VAR_DUCK_QUACK_COUNT, updatedQuacks, false);

        // Success is now immediate. Turn off the event first so near-simultaneous messages
        // do not double-trigger the unlock path.
        if (updatedQuacks < liveTarget)
            return true;

        CPH.SetGlobalVar(VAR_DUCK_EVENT_ACTIVE, false, false);
        CPH.DisableTimer(TIMER_DUCK_CALL_WINDOW);

        bool alreadyUnlocked = (CPH.GetGlobalVar<bool?>(VAR_DUCK_UNLOCKED, false) ?? false);
        if (!alreadyUnlocked)
        {
            CPH.SetGlobalVar(VAR_DUCK_UNLOCKED, true, false);

            bool unlockTriggered = TriggerMixItUpUnlock();
            if (unlockTriggered)
                CPH.Wait(DUCK_UNLOCK_WAIT_MS);

            CPH.SendMessage("🦆✅ DUCK UNLOCKED! The crew bullied the river into cooperating.");
        }
        else
        {
            CPH.SendMessage("🦆 Duck was already dancing. The river has accepted your nonsense.");
        }

        ReleaseMiniGameLockIfOwned();
        return true;
    }

    /// <summary>
    /// Calculates the live quack target from the number of unique chatters who have joined in.
    /// The minimum target keeps solo spam from unlocking too cheaply.
    /// </summary>
    private int CalculateTarget(int uniqueQuackers)
    {
        int safeUniqueCount = Math.Max(1, uniqueQuackers);
        return Math.Max(DUCK_MINIMUM_TARGET_QUACKS, safeUniqueCount * DUCK_QUACKS_PER_UNIQUE_CHATTER);
    }

    /// <summary>
    /// Stores the participant if we have not seen them this round yet.
    /// Returns the updated unique participant count.
    /// </summary>
    private int RegisterUniqueQuacker(string participantKey)
    {
        int currentCount = (CPH.GetGlobalVar<int?>(VAR_DUCK_UNIQUE_QUACKER_COUNT, false) ?? 0);
        if (string.IsNullOrWhiteSpace(participantKey))
            return currentCount;

        string normalizedKey = participantKey.Trim();
        if (normalizedKey.Contains("|"))
            normalizedKey = normalizedKey.Replace("|", "");

        if (string.IsNullOrWhiteSpace(normalizedKey))
            return currentCount;

        string registry = CPH.GetGlobalVar<string>(VAR_DUCK_UNIQUE_QUACKERS, false) ?? "|";
        string token = $"|{normalizedKey}|";
        if (registry.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0)
            return currentCount;

        registry += normalizedKey + "|";
        currentCount++;

        CPH.SetGlobalVar(VAR_DUCK_UNIQUE_QUACKERS, registry, false);
        CPH.SetGlobalVar(VAR_DUCK_UNIQUE_QUACKER_COUNT, currentCount, false);
        return currentCount;
    }

    /// <summary>
    /// Prefers userId because it is stable if a chatter renames themselves.
    /// Falls back to user if the trigger does not expose userId.
    /// </summary>
    private string GetParticipantKey()
    {
        string participantKey = "";
        if (CPH.TryGetArg("userId", out participantKey) && !string.IsNullOrWhiteSpace(participantKey))
            return participantKey;

        CPH.TryGetArg("user", out participantKey);
        return participantKey ?? "";
    }

    /// <summary>
    /// Returns number of times word fragment appears in text.
    /// Uses simple substring search and allows repeated occurrences.
    /// </summary>
    private int CountOccurrences(string text, string word)
    {
        int count = 0;
        int index = 0;

        string t = text.ToLowerInvariant();
        string w = word.ToLowerInvariant();

        while ((index = t.IndexOf(w, index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += w.Length;
        }

        return count;
    }

    /// <summary>
    /// Releases the shared lock only if Duck currently owns it.
    /// </summary>
    private void ReleaseMiniGameLockIfOwned()
    {
        string lockName = CPH.GetGlobalVar<string>(VAR_MINIGAME_NAME, false) ?? "";
        if (!string.Equals(lockName, MINIGAME_NAME_DUCK, StringComparison.OrdinalIgnoreCase))
            return;

        CPH.SetGlobalVar(VAR_MINIGAME_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_MINIGAME_NAME, "", false);
    }

    /// <summary>
    /// Duck-specific wrapper that calls the generic Mix It Up helper.
    /// </summary>
    private bool TriggerMixItUpUnlock()
    {
        return TriggerMixItUpCommand(MIXITUP_DUCK_UNLOCK_COMMAND_ID, "Squad Duck");
    }

    /// <summary>
    /// Generic Mix It Up command trigger helper.
    /// </summary>
    private bool TriggerMixItUpCommand(string commandId, string logPrefix, string arguments = "")
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
                Platform = "Twitch",
                Arguments = arguments ?? "",
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
