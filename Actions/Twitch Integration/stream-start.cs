using System;
using System.Collections.Generic;

public class CPHInline
{
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
     * - Resets Toothless rarity unlock flags + last roll tracking.
     * - Resets LotAT mode + offering steal settings.
     * - Resets Duck and Clone runtime state.
     * - Hides Duck/Clone/Toothless dance sources in OBS.
     * - Disables Duck and Clone timers to prevent stale timer fires.
     *
     * Operator notes:
     * - Keep scene/source names in sync with OBS.
     * - This script is safe to run repeatedly (idempotent reset behavior).
     */
    public bool Execute()
    {
        // Central scene used by dancing sources.
        const string DISCO_SCENE = "Disco Party: Workspace";

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
            CPH.ObsHideSource(DISCO_SCENE, dancingSource);
            CPH.ObsShowSource(DISCO_SCENE, dancingSource);
            CPH.ObsHideSource(DISCO_SCENE, dancingSource);

            // Clear unlock flags so each stream starts fresh.
            CPH.SetGlobalVar($"rarity_{rarity}", false, false);
        }

        // Reset latest-roll breadcrumbs used by overlays/debug.
        CPH.SetGlobalVar("last_roll", 0, false);
        CPH.SetGlobalVar("last_rarity", "", false);
        CPH.SetGlobalVar("last_user", "", false);

        // Note: per-user boosts are stored as boost_<member>_<userId>.
        // We intentionally do not iterate/clear unknown user IDs here.

        // -------------------------------------------------
        // LotAT + offerings reset
        // -------------------------------------------------
        CPH.SetGlobalVar("lotat_active", false, false);
        CPH.SetGlobalVar("lotat_announcement_sent", false, false);
        CPH.SetGlobalVar("lotat_offering_steal_chance", 0, false);
        CPH.SetGlobalVar("lotat_steal_multiplier", 1, false);

        // -------------------------------------------------
        // Duck reset
        // -------------------------------------------------
        CPH.SetGlobalVar("duck_event_active", false, false);
        CPH.SetGlobalVar("duck_quack_count", 0, false);
        CPH.SetGlobalVar("duck_caller", "", false);
        CPH.SetGlobalVar("duck_unlocked", false, false);

        CPH.ObsHideSource(DISCO_SCENE, "Duck - Dancing");
        CPH.DisableTimer("Duck - Call Window");

        // -------------------------------------------------
        // Clone reset
        // -------------------------------------------------
        CPH.ObsHideSource(DISCO_SCENE, "Clone - Dancing");
        CPH.SetGlobalVar("clone_unlocked", false, false);
        CPH.SetGlobalVar("clone_game_active", false, false);
        CPH.SetGlobalVar("clone_round", 0, false);
        CPH.SetGlobalVar("clone_positions_open", "", false);
        CPH.SetGlobalVar("clone_winners", "", false);

        CPH.DisableTimer("Clone - Volley Timer");

        return true;
    }
}
