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
    private const string VAR_PEDRO_UNLOCKED = "pedro_unlocked";
    private const string TIMER_PEDRO_CALL_WINDOW = "Pedro - Call Window";
    private const string OBS_SCENE_DISCO_WORKSPACE = "Disco Party: Workspace";
    private const string OBS_SOURCE_PEDRO_DANCING = "Pedro - Dancing";

    // Shared mini-game lock (cross-feature).
    private const string VAR_MINIGAME_ACTIVE = "minigame_active";
    private const string VAR_MINIGAME_NAME = "minigame_name";
    private const string MINIGAME_NAME_PEDRO = "pedro";

    // Pedro success threshold for mention counting.
    private const int PEDRO_MENTION_THRESHOLD = 100;

    // Mix It Up unlock bridge for Pedro unlock events.
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_PEDRO_UNLOCK_COMMAND_ID = "a43a1ecd-1607-4dc2-9ae2-fe96f0566f39";
    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    /*
     * Purpose:
     * - Resolves Pedro event when timer window ends.
     *
     * Expected trigger/input:
     * - Timer action: "Pedro - Call Window"
     *
     * Required runtime variables:
     * - pedro_game_enabled
     * - pedro_mention_count
     * - pedro_unlocked
     * - shared lock: minigame_active/minigame_name
     *
     * Key outputs/side effects:
     * - Ends active event window.
     * - If mentions are greater than 100: shows OBS source and triggers Mix It Up command.
     * - Releases shared mini-game lock when event ends.
     */
    public bool Execute()
    {
        // If timer fired after event ended, just ensure timer is off and exit.
        bool active = (CPH.GetGlobalVar<bool?>(VAR_PEDRO_GAME_ENABLED, false) ?? false);
        if (!active)
        {
            CPH.DisableTimer(TIMER_PEDRO_CALL_WINDOW);
            ReleaseMiniGameLockIfOwned();
            return true;
        }

        // End event window now (no more mention counting should happen).
        CPH.SetGlobalVar(VAR_PEDRO_GAME_ENABLED, false, false);
        CPH.DisableTimer(TIMER_PEDRO_CALL_WINDOW);

        int mentions = (CPH.GetGlobalVar<int?>(VAR_PEDRO_MENTION_COUNT, false) ?? 0);

        if (mentions > PEDRO_MENTION_THRESHOLD)
        {
            CPH.SetGlobalVar(VAR_PEDRO_UNLOCKED, true, false);
            CPH.ObsShowSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_PEDRO_DANCING);
            TriggerMixItUpUnlock();

            CPH.SendMessage($"💃✅ PEDRO UNLOCKED! Mentions: {mentions} (needed more than {PEDRO_MENTION_THRESHOLD}).");
        }
        else
        {
            CPH.SendMessage($"💃❌ Not enough Pedro power. Mentions: {mentions} (need more than {PEDRO_MENTION_THRESHOLD}).");
        }

        ReleaseMiniGameLockIfOwned();
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
