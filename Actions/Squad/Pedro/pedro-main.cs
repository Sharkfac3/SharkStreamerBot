using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (Pedro feature)
    // Keep these names identical across:
    // - Actions/Squad/Pedro/pedro-main.cs
    // - Actions/Squad/Pedro/pedro-call.cs
    // - Actions/Squad/Pedro/pedro-resolve.cs
    // - Actions/Twitch Core Integrations/stream-start.cs
    private const string VAR_PEDRO_GAME_ENABLED = "pedro_game_enabled";
    private const string VAR_PEDRO_MENTION_COUNT = "pedro_mention_count";
    private const string VAR_PEDRO_UNLOCKED = "pedro_unlocked";
    private const string VAR_PEDRO_NEXT_ALLOWED_UTC = "pedro_next_allowed_utc";
    private const string VAR_PEDRO_SECRET_UNLOCK_ACTIVE = "pedro_secret_unlock_active";
    private const string TIMER_PEDRO_CALL_WINDOW = "Pedro - Call Window";

    // Shared mini-game lock (cross-feature).
    private const string VAR_MINIGAME_ACTIVE = "minigame_active";
    private const string VAR_MINIGAME_NAME = "minigame_name";
    private const string MINIGAME_NAME_PEDRO = "pedro";

    // Secret unlock text allowed through the !pedro command.
    // Example: !pedro x500livepedro
    private const string PEDRO_SECRET_PHRASE = "x500livepedro";

    // Mix It Up unlock bridge for Pedro unlock events.
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_PEDRO_UNLOCK_COMMAND_ID = "a43a1ecd-1607-4dc2-9ae2-fe96f0566f39";
    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    // Every successful Pedro unlock command trigger should hold the action open long enough
    // for the Mix It Up sequence to play out.
    private const int PEDRO_UNLOCK_WAIT_MS = 28000;

    /*
     * Purpose:
     * - Handles the !pedro command entrypoint.
     * - Starts Pedro mini-game only when !pedro has no trailing message.
     * - If !pedro has the exact secret message, only triggers Mix It Up unlock command.
     *
     * Expected trigger/input:
     * - Streamer.bot command trigger for !pedro
     *
     * Required runtime variables:
     * - pedro_game_enabled
     * - pedro_mention_count
     * - pedro_unlocked
     * - pedro_next_allowed_utc
     * - pedro_secret_unlock_active
     * - shared lock: minigame_active/minigame_name
     *
     * Key outputs/side effects:
     * - Empty message: starts Pedro call window + timer when Pedro is not unlocked and not on cooldown.
     * - Secret message: calls Mix It Up unlock command, waits 28 seconds on success,
     *   and blocks overlapping secret unlock runs until the active secret sequence finishes.
     */
    public bool Execute()
    {
        string pedroMessage = GetPedroCommandMessage();

        // Secret command path: do NOT start mini-game.
        // We only bridge to Mix It Up as requested, and we still allow this path
        // even if Pedro is already unlocked or the normal game is on cooldown.
        if (IsSecretPhrase(pedroMessage))
        {
            if (!TryAcquireSecretUnlockGuard())
            {
                CPH.SendMessage("💃 Pedro's secret unlock is already playing. Let this one finish first.");
                return true;
            }

            try
            {
                bool unlockTriggered = TriggerMixItUpUnlock();
                if (unlockTriggered)
                    CPH.Wait(PEDRO_UNLOCK_WAIT_MS);
            }
            finally
            {
                ReleaseSecretUnlockGuard();
            }

            return true;
        }

        // Any non-empty argument that is not the secret phrase should not start the mini-game.
        // Give chat a slightly cryptic nudge so the command feels in-world instead of error-like.
        if (!string.IsNullOrWhiteSpace(pedroMessage))
        {
            CPH.SendMessage("🦝 Pedro rattles a wrench inside the walls, then goes quiet. That phrase wasn't the one he was listening for.");
            return true;
        }

        // Once Pedro is unlocked, the normal mini-game should stay unavailable for the rest of the stream.
        bool unlocked = (CPH.GetGlobalVar<bool?>(VAR_PEDRO_UNLOCKED, false) ?? false);
        if (unlocked)
        {
            CPH.SendMessage("💃 Pedro is already unlocked, so this mini-game is resting for the rest of the stream.");
            return true;
        }

        // Respect the 5-minute replay cooldown after the previous Pedro game resolved.
        DateTime? nextAllowedUtc = GetPedroNextAllowedUtc();
        if (nextAllowedUtc.HasValue && DateTime.UtcNow < nextAllowedUtc.Value)
        {
            TimeSpan remaining = nextAllowedUtc.Value - DateTime.UtcNow;
            if (remaining < TimeSpan.Zero)
                remaining = TimeSpan.Zero;

            int secondsRemaining = (int)Math.Ceiling(remaining.TotalSeconds);
            CPH.SendMessage($"💃 Pedro needs a breather. Try again in about {secondsRemaining} seconds.");
            return true;
        }

        // Prevent overlap across all mini-games.
        if (!TryAcquireMiniGameLock())
        {
            string activeGame = CPH.GetGlobalVar<string>(VAR_MINIGAME_NAME, false) ?? "another mini-game";
            CPH.SendMessage($"🎮 A mini-game is already running ({activeGame}). Finish it before starting Pedro.");
            return true;
        }

        // Prevent overlapping Pedro events.
        bool active = (CPH.GetGlobalVar<bool?>(VAR_PEDRO_GAME_ENABLED, false) ?? false);
        if (active)
        {
            ReleaseMiniGameLockIfOwned();
            CPH.SendMessage("💃 Pedro is already on the dance floor... summon harder!");
            return true;
        }

        // Reset event state for a new run.
        CPH.SetGlobalVar(VAR_PEDRO_GAME_ENABLED, true, false);
        CPH.SetGlobalVar(VAR_PEDRO_MENTION_COUNT, 0, false);

        // Start timer that will resolve event outcome.
        CPH.EnableTimer(TIMER_PEDRO_CALL_WINDOW);

        // Notify chat.
        CPH.SendMessage("💃 Pedro has entered the arena! You have 2 minutes — spam **pedro**!");
        return true;
    }

    private string GetPedroCommandMessage()
    {
        // In command triggers, `message` is usually the text after !pedro.
        string messageArg = "";
        if (CPH.TryGetArg("message", out messageArg))
            return (messageArg ?? "").Trim();

        // Fallback for setups that pass only rawInput.
        // We strip the !pedro prefix if present.
        string rawInput = "";
        if (!CPH.TryGetArg("rawInput", out rawInput))
            return "";

        rawInput = (rawInput ?? "").Trim();
        if (rawInput.StartsWith("!pedro", StringComparison.OrdinalIgnoreCase))
            return rawInput.Substring("!pedro".Length).Trim();

        return rawInput;
    }

    private bool IsSecretPhrase(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        return string.Equals(text.Trim(), PEDRO_SECRET_PHRASE, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Secret unlock runs are intentionally single-file so two users cannot fire the
    /// Mix It Up unlock sequence on top of each other.
    /// </summary>
    private bool TryAcquireSecretUnlockGuard()
    {
        bool active = (CPH.GetGlobalVar<bool?>(VAR_PEDRO_SECRET_UNLOCK_ACTIVE, false) ?? false);
        if (active)
            return false;

        CPH.SetGlobalVar(VAR_PEDRO_SECRET_UNLOCK_ACTIVE, true, false);
        return true;
    }

    /// <summary>
    /// Clears the secret unlock guard once the Mix It Up sequence finishes or errors.
    /// </summary>
    private void ReleaseSecretUnlockGuard()
    {
        CPH.SetGlobalVar(VAR_PEDRO_SECRET_UNLOCK_ACTIVE, false, false);
    }

    /// <summary>
    /// Claims the shared mini-game lock when free.
    /// Allows re-entry only for Pedro itself.
    /// </summary>
    private bool TryAcquireMiniGameLock()
    {
        bool lockActive = (CPH.GetGlobalVar<bool?>(VAR_MINIGAME_ACTIVE, false) ?? false);
        string lockName = CPH.GetGlobalVar<string>(VAR_MINIGAME_NAME, false) ?? "";

        if (lockActive && !string.Equals(lockName, MINIGAME_NAME_PEDRO, StringComparison.OrdinalIgnoreCase))
            return false;

        CPH.SetGlobalVar(VAR_MINIGAME_ACTIVE, true, false);
        CPH.SetGlobalVar(VAR_MINIGAME_NAME, MINIGAME_NAME_PEDRO, false);
        return true;
    }

    /// <summary>
    /// Reads the next allowed Pedro start time from the shared cooldown variable.
    /// Empty/invalid values are treated as no cooldown.
    /// </summary>
    private DateTime? GetPedroNextAllowedUtc()
    {
        string rawValue = CPH.GetGlobalVar<string>(VAR_PEDRO_NEXT_ALLOWED_UTC, false) ?? "";
        if (string.IsNullOrWhiteSpace(rawValue))
            return null;

        if (DateTime.TryParse(
                rawValue,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                out DateTime parsedUtc))
        {
            return parsedUtc;
        }

        CPH.LogWarn($"[Squad Pedro] Invalid cooldown timestamp in '{VAR_PEDRO_NEXT_ALLOWED_UTC}': '{rawValue}'.");
        return null;
    }

    /// <summary>
    /// Releases the shared lock only if Pedro currently owns it.
    /// This is mainly used for defensive guard exits after the lock has been claimed.
    /// </summary>
    private void ReleaseMiniGameLockIfOwned()
    {
        string lockName = CPH.GetGlobalVar<string>(VAR_MINIGAME_NAME, false) ?? "";
        if (!string.Equals(lockName, MINIGAME_NAME_PEDRO, StringComparison.OrdinalIgnoreCase))
            return;

        CPH.SetGlobalVar(VAR_MINIGAME_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_MINIGAME_NAME, "", false);
    }

    /// <summary>
    /// Pedro-specific wrapper that calls the generic Mix It Up helper.
    /// </summary>
    private bool TriggerMixItUpUnlock()
    {
        return TriggerMixItUpCommand(MIXITUP_PEDRO_UNLOCK_COMMAND_ID, "Squad Pedro");
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
