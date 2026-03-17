using System;
using System.Collections.Generic;

public class CPHInline
{
    // SYNC CONSTANTS (shared across features)
    // Keep these names identical across related feature scripts.
    private const string OBS_SCENE_DISCO_WORKSPACE = "Disco Party: Workspace";

    // Duck
    private const string VAR_DUCK_EVENT_ACTIVE = "duck_event_active";
    private const string VAR_DUCK_QUACK_COUNT = "duck_quack_count";
    private const string VAR_DUCK_CALLER = "duck_caller";
    private const string VAR_DUCK_UNLOCKED = "duck_unlocked";
    private const string OBS_SOURCE_DUCK_DANCING = "Duck - Dancing";
    private const string TIMER_DUCK_CALL_WINDOW = "Duck - Call Window";

    // Clone
    private const string VAR_CLONE_UNLOCKED = "clone_unlocked";
    private const string VAR_CLONE_GAME_ACTIVE = "clone_game_active";
    private const string VAR_CLONE_ROUND = "clone_round";
    private const string VAR_CLONE_POSITIONS_OPEN = "clone_positions_open";
    private const string VAR_CLONE_WINNERS = "clone_winners";
    private const string OBS_SOURCE_CLONE_DANCING = "Clone - Dancing";
    private const string TIMER_CLONE_VOLLEY = "Clone - Volley Timer";

    // Pedro
    private const string VAR_PEDRO_GAME_ENABLED = "pedro_game_enabled";
    private const string VAR_PEDRO_MENTION_COUNT = "pedro_mention_count";
    private const string VAR_PEDRO_UNLOCKED = "pedro_unlocked";
    private const string VAR_PEDRO_LAST_MESSAGE_ID = "pedro_last_message_id";
    private const string VAR_PEDRO_SECRET_NEXT_ALLOWED_UTC = "pedro_secret_next_allowed_utc";
    private const string OBS_SOURCE_PEDRO_DANCING = "Pedro - Dancing";
    private const string TIMER_PEDRO_CALL_WINDOW = "Pedro - Call Window";

    // Toothless
    private const string PREFIX_RARITY = "rarity_";
    private const string VAR_LAST_ROLL = "last_roll";
    private const string VAR_LAST_RARITY = "last_rarity";
    private const string VAR_LAST_USER = "last_user";

    // LotAT
    private const string VAR_LOTAT_ACTIVE = "lotat_active";
    private const string VAR_LOTAT_ANNOUNCEMENT_SENT = "lotat_announcement_sent";
    private const string VAR_LOTAT_OFFERING_STEAL_CHANCE = "lotat_offering_steal_chance";
    private const string VAR_LOTAT_STEAL_MULTIPLIER = "lotat_steal_multiplier";

    // Shared mini-game lock (cross-feature)
    private const string VAR_MINIGAME_ACTIVE = "minigame_active";
    private const string VAR_MINIGAME_NAME = "minigame_name";

    // Stream mode (shared across Twitch integration and mode actions)
    private const string VAR_STREAM_MODE = "stream_mode";
    private const string MODE_WORKSPACE = "workspace";

    /*
     * Purpose:
     * - Runs at stream start to reset shared state for Squad, LotAT, and Twitch integrations.
     *
     * Expected trigger/input:
     * - Streamer.bot stream-start action (manual or event wired).
     * - No chat input required.
     *
     * Required runtime variables:
     * - Reads none.
     * - Writes/reset many global vars used by Squad mini-games and LotAT offering logic.
     *
     * Key outputs/side effects:
     * - Clears global mini-game lock state.
     * - Resets Toothless rarity unlock flags + last roll tracking.
     * - Resets LotAT mode + offering steal settings.
     * - Resets Duck, Clone, and Pedro runtime state.
     * - Sets stream mode to workspace as the default start-of-stream mode.
     * - Hides Duck/Clone/Pedro/Toothless dance sources in OBS.
     * - Disables Duck, Clone, and Pedro timers to prevent stale timer fires.
     *
     * Operator notes:
     * - Keep scene/source names in sync with OBS.
     * - This script is safe to run repeatedly (idempotent reset behavior).
     */
    public bool Execute()
    {
        // Clear shared lock so no stale mini-game blocks the new stream.
        CPH.SetGlobalVar(VAR_MINIGAME_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_MINIGAME_NAME, "", false);

        // Default stream mode at stream start.
        // This gives downstream actions/commands a known baseline mode.
        CPH.SetGlobalVar(VAR_STREAM_MODE, MODE_WORKSPACE, false);

        // Toothless rarity list used for both source names and unlock flag keys.
        var toothlessRarities = new List<string>
        {
            "regular",
            "smol",
            "long",
            "flight",
            "party"
        };

        // -------------------------------------------------
        // Toothless reset
        // -------------------------------------------------
        foreach (string rarity in toothlessRarities)
        {
            string dancingSource = $"Toothless - Dancing - {rarity}";

            // Hide -> show -> hide sequence helps clear any stale visibility cache.
            CPH.ObsHideSource(OBS_SCENE_DISCO_WORKSPACE, dancingSource);
            CPH.ObsShowSource(OBS_SCENE_DISCO_WORKSPACE, dancingSource);
            CPH.ObsHideSource(OBS_SCENE_DISCO_WORKSPACE, dancingSource);

            // Clear unlock flags so each stream starts fresh.
            CPH.SetGlobalVar($"{PREFIX_RARITY}{rarity}", false, false);
        }

        // Reset latest-roll breadcrumbs used by overlays/debug.
        CPH.SetGlobalVar(VAR_LAST_ROLL, 0, false);
        CPH.SetGlobalVar(VAR_LAST_RARITY, "", false);
        CPH.SetGlobalVar(VAR_LAST_USER, "", false);

        // Note: per-user boosts are stored as boost_<member>_<userId>.
        // We intentionally do not iterate/clear unknown user IDs here.

        // -------------------------------------------------
        // LotAT + offerings reset
        // -------------------------------------------------
        CPH.SetGlobalVar(VAR_LOTAT_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_LOTAT_ANNOUNCEMENT_SENT, false, false);
        CPH.SetGlobalVar(VAR_LOTAT_OFFERING_STEAL_CHANCE, 0, false);
        CPH.SetGlobalVar(VAR_LOTAT_STEAL_MULTIPLIER, 1, false);

        // -------------------------------------------------
        // Duck reset
        // -------------------------------------------------
        CPH.SetGlobalVar(VAR_DUCK_EVENT_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_DUCK_QUACK_COUNT, 0, false);
        CPH.SetGlobalVar(VAR_DUCK_CALLER, "", false);
        CPH.SetGlobalVar(VAR_DUCK_UNLOCKED, false, false);

        CPH.ObsHideSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_DUCK_DANCING);
        CPH.ObsShowSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_DUCK_DANCING);
        CPH.ObsHideSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_DUCK_DANCING);
        CPH.DisableTimer(TIMER_DUCK_CALL_WINDOW);

        // -------------------------------------------------
        // Clone reset
        // -------------------------------------------------
        CPH.ObsHideSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_CLONE_DANCING);
        CPH.ObsShowSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_CLONE_DANCING);
        CPH.ObsHideSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_CLONE_DANCING);
        CPH.SetGlobalVar(VAR_CLONE_UNLOCKED, false, false);
        CPH.SetGlobalVar(VAR_CLONE_GAME_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_CLONE_ROUND, 0, false);
        CPH.SetGlobalVar(VAR_CLONE_POSITIONS_OPEN, "", false);
        CPH.SetGlobalVar(VAR_CLONE_WINNERS, "", false);

        CPH.DisableTimer(TIMER_CLONE_VOLLEY);

        // -------------------------------------------------
        // Pedro reset
        // -------------------------------------------------
        CPH.SetGlobalVar(VAR_PEDRO_GAME_ENABLED, false, false);
        CPH.SetGlobalVar(VAR_PEDRO_MENTION_COUNT, 0, false);
        CPH.SetGlobalVar(VAR_PEDRO_UNLOCKED, false, false);
        CPH.SetGlobalVar(VAR_PEDRO_LAST_MESSAGE_ID, "", false);
        CPH.SetGlobalVar(VAR_PEDRO_SECRET_NEXT_ALLOWED_UTC, 0L, false);

        CPH.ObsHideSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_PEDRO_DANCING);
        CPH.ObsShowSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_PEDRO_DANCING);
        CPH.ObsHideSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_PEDRO_DANCING);
        CPH.DisableTimer(TIMER_PEDRO_CALL_WINDOW);

        return true;
    }
}
