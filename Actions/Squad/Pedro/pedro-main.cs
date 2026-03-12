using System;
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
    // - Actions/Twitch Integration/stream-start.cs
    private const string VAR_PEDRO_GAME_ENABLED = "pedro_game_enabled";
    private const string VAR_PEDRO_MENTION_COUNT = "pedro_mention_count";
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
     * - shared lock: minigame_active/minigame_name
     *
     * Key outputs/side effects:
     * - Empty message: starts Pedro call window + timer.
     * - Secret message: calls Mix It Up unlock command only.
     */
    public bool Execute()
    {
        string pedroMessage = GetPedroCommandMessage();

        // Secret command path: do NOT start mini-game.
        // We only bridge to Mix It Up as requested.
        if (IsSecretPhrase(pedroMessage))
        {
            TriggerMixItUpUnlock();
            return true;
        }

        // Any non-empty argument that is not the secret phrase should not start the mini-game.
        if (!string.IsNullOrWhiteSpace(pedroMessage))
            return true;

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
    /// Pedro-specific wrapper that calls the generic Mix It Up helper.
    /// </summary>
    private void TriggerMixItUpUnlock()
    {
        TriggerMixItUpCommand(MIXITUP_PEDRO_UNLOCK_COMMAND_ID, "Squad Pedro");
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
