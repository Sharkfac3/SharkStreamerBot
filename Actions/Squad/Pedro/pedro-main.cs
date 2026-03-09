using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (Pedro feature)
    // Keep these names identical across:
    // - Actions/Squad/Pedro/pedro-main.cs
    // - Actions/Twitch Integration/stream-start.cs
    private const string VAR_PEDRO_GAME_ENABLED = "pedro_game_enabled";
    private const string VAR_PEDRO_MENTION_COUNT = "pedro_mention_count";
    private const string VAR_PEDRO_UNLOCKED = "pedro_unlocked";
    private const string VAR_PEDRO_LAST_MESSAGE_ID = "pedro_last_message_id";
    private const string OBS_SCENE_PEDRO = "Disco Party: Workspace";
    private const string OBS_SOURCE_PEDRO_DANCING = "Pedro - Dancing";

    // Shared mini-game lock (cross-feature).
    private const string VAR_MINIGAME_ACTIVE = "minigame_active";
    private const string VAR_MINIGAME_NAME = "minigame_name";
    private const string MINIGAME_NAME_PEDRO = "pedro";

    /*
     * Purpose:
     * - Handles all Pedro chat triggers in one action script:
     *   1) Secret Mix It Up trigger phrase (does not change Pedro game state).
     *   2) !pedro command to enable Pedro mention mini-game.
     *   3) Mention counting while mini-game is enabled.
     *
     * Expected trigger/input:
     * - Chat message trigger wired to run when:
     *   - message starts with "!pedro", OR
     *   - message contains "pedro" anywhere.
     * - Reads: message (fallback rawInput), optional msgId, optional user.
     *
     * Required runtime variables:
     * - pedro_game_enabled (bool)
     * - pedro_mention_count (int)
     * - pedro_unlocked (bool)
     * - pedro_last_message_id (string) for duplicate-trigger safety
     * - shared lock: minigame_active/minigame_name
     *
     * Key outputs/side effects:
     * - Enables mini-game on !pedro (only when no other mini-game is active).
     * - Counts "pedro" mentions until unlock threshold is reached.
     * - On unlock: shows OBS source "Pedro - Dancing" on the Disco workspace scene.
     * - On unlock: triggers Mix It Up command "Squad - Pedro - Unlock".
     * - On unlock: releases shared mini-game lock.
     *
     * Operator notes:
     * - Replace MIXITUP_PEDRO_UNLOCK_COMMAND_ID when available.
     * - Pedro source scene is fixed to: "Disco Party: Workspace".
     */

    private const int PEDRO_UNLOCK_THRESHOLD = 100;
    private const string PEDRO_SECRET_PHRASE = "x500livepedro";

    // Mix It Up unlock bridge for Pedro unlock events.
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_PEDRO_UNLOCK_COMMAND_ID = "REPLACE_WITH_PEDRO_UNLOCK_COMMAND_ID";
    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    public bool Execute()
    {
        string message = GetMessageText();
        if (string.IsNullOrWhiteSpace(message))
            return true;

        // Guard against accidental double-processing when multiple triggers fire for the same message.
        if (IsDuplicateMessage())
            return true;

        string trimmed = message.Trim();

        // 1) Secret override: !pedro x500liVePedro (case-insensitive).
        // This ONLY triggers Mix It Up and does NOT alter Pedro mini-game/unlock state.
        if (IsSecretUnlockCommand(trimmed))
        {
            TriggerMixItUpUnlock();
            return true;
        }

        // 2) !pedro enables the mini-game.
        if (string.Equals(trimmed, "!pedro", StringComparison.OrdinalIgnoreCase))
        {
            HandlePedroStartCommand();
            return true;
        }

        // 3) Normal mention counting only while game is active and still locked.
        bool gameEnabled = (CPH.GetGlobalVar<bool?>(VAR_PEDRO_GAME_ENABLED, false) ?? false);
        bool unlocked = (CPH.GetGlobalVar<bool?>(VAR_PEDRO_UNLOCKED, false) ?? false);
        if (!gameEnabled || unlocked)
            return true;

        int hits = CountOccurrences(message, "pedro");
        if (hits <= 0)
            return true;

        int currentCount = (CPH.GetGlobalVar<int?>(VAR_PEDRO_MENTION_COUNT, false) ?? 0);
        int newCount = currentCount + hits;
        CPH.SetGlobalVar(VAR_PEDRO_MENTION_COUNT, newCount, false);

        if (newCount >= PEDRO_UNLOCK_THRESHOLD)
        {
            UnlockPedro();
        }

        return true;
    }

    /// <summary>
    /// Reads chat text from common trigger args with safe fallback.
    /// </summary>
    private string GetMessageText()
    {
        string text = "";
        if (!CPH.TryGetArg("message", out text) || string.IsNullOrWhiteSpace(text))
            CPH.TryGetArg("rawInput", out text);

        return text ?? "";
    }

    /// <summary>
    /// Uses Twitch message ID when available to avoid handling the same chat line twice.
    /// </summary>
    private bool IsDuplicateMessage()
    {
        string msgId = "";
        if (!CPH.TryGetArg("msgId", out msgId) || string.IsNullOrWhiteSpace(msgId))
            return false;

        string lastId = CPH.GetGlobalVar<string>(VAR_PEDRO_LAST_MESSAGE_ID, false) ?? "";
        if (string.Equals(lastId, msgId, StringComparison.OrdinalIgnoreCase))
            return true;

        CPH.SetGlobalVar(VAR_PEDRO_LAST_MESSAGE_ID, msgId, false);
        return false;
    }

    /// <summary>
    /// Validates exact secret command format: !pedro x500liVePedro (case-insensitive token check).
    /// </summary>
    private bool IsSecretUnlockCommand(string trimmedMessage)
    {
        if (string.IsNullOrWhiteSpace(trimmedMessage))
            return false;

        string[] parts = trimmedMessage.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            return false;

        return string.Equals(parts[0], "!pedro", StringComparison.OrdinalIgnoreCase)
            && string.Equals(parts[1], PEDRO_SECRET_PHRASE, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Handles "!pedro" command to start/announce mini-game state.
    /// </summary>
    private void HandlePedroStartCommand()
    {
        bool unlocked = (CPH.GetGlobalVar<bool?>(VAR_PEDRO_UNLOCKED, false) ?? false);
        if (unlocked)
        {
            CPH.SendMessage("💃 Pedro is already unlocked and dancing!");
            return;
        }

        bool enabled = (CPH.GetGlobalVar<bool?>(VAR_PEDRO_GAME_ENABLED, false) ?? false);
        int count = (CPH.GetGlobalVar<int?>(VAR_PEDRO_MENTION_COUNT, false) ?? 0);

        if (!enabled)
        {
            if (!TryAcquireMiniGameLock())
            {
                string activeGame = CPH.GetGlobalVar<string>(VAR_MINIGAME_NAME, false) ?? "another mini-game";
                CPH.SendMessage($"🎮 A mini-game is already running ({activeGame}). Finish it before starting Pedro.");
                return;
            }

            CPH.SetGlobalVar(VAR_PEDRO_GAME_ENABLED, true, false);

            // Keep any existing count if operator paused/re-enabled manually.
            CPH.SendMessage($"🕺 Pedro mini-game is live! Mention 'pedro' in chat ({PEDRO_UNLOCK_THRESHOLD} total) to unlock him!");
            return;
        }

        CPH.SendMessage($"🕺 Pedro hunt already active: {count}/{PEDRO_UNLOCK_THRESHOLD} mentions.");
    }

    /// <summary>
    /// Applies first-time unlock state when mention threshold is reached.
    /// </summary>
    private void UnlockPedro()
    {
        bool alreadyUnlocked = (CPH.GetGlobalVar<bool?>(VAR_PEDRO_UNLOCKED, false) ?? false);
        if (alreadyUnlocked)
            return;

        CPH.SetGlobalVar(VAR_PEDRO_UNLOCKED, true, false);
        CPH.SetGlobalVar(VAR_PEDRO_GAME_ENABLED, false, false);
        CPH.SetGlobalVar(VAR_PEDRO_MENTION_COUNT, PEDRO_UNLOCK_THRESHOLD, false);

        ShowPedroSourceOnConfiguredScenes();
        TriggerMixItUpUnlock();
        ReleaseMiniGameLockIfOwned();

        CPH.SendMessage("💃✅ PEDRO UNLOCKED! Pedro joins the dance floor!");
    }

    /// <summary>
    /// Shows "Pedro - Dancing" on the fixed Pedro scene.
    /// </summary>
    private void ShowPedroSourceOnConfiguredScenes()
    {
        CPH.ObsShowSource(OBS_SCENE_PEDRO, OBS_SOURCE_PEDRO_DANCING);
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
    /// Releases the shared lock only if Pedro currently owns it.
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
    /// Calls Mix It Up command API for Pedro unlock side-effects.
    /// </summary>
    private void TriggerMixItUpUnlock()
    {
        if (string.IsNullOrWhiteSpace(MIXITUP_PEDRO_UNLOCK_COMMAND_ID) ||
            MIXITUP_PEDRO_UNLOCK_COMMAND_ID.StartsWith("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase))
        {
            CPH.LogWarn("[Squad Pedro] Mix It Up unlock command ID is not configured yet.");
            return;
        }

        try
        {
            string url = $"{MIXITUP_API_BASE_URL.TrimEnd('/')}/api/v2/commands/{MIXITUP_PEDRO_UNLOCK_COMMAND_ID}";
            string payload = JsonSerializer.Serialize(new
            {
                Platform = "Twitch",
                Arguments = "",
                IgnoreRequirements = false
            });

            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = MIXITUP_HTTP_CLIENT.PostAsync(url, content).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                CPH.LogWarn($"[Squad Pedro] Mix It Up unlock call failed: {(int)response.StatusCode} {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            CPH.LogError($"[Squad Pedro] Exception while calling Mix It Up unlock command: {ex}");
        }
    }

    /// <summary>
    /// Returns number of non-overlapping occurrences of "word" in text (case-insensitive).
    /// </summary>
    private int CountOccurrences(string text, string word)
    {
        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(word))
            return 0;

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
}
