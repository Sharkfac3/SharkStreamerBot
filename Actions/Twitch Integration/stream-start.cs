using System;
using System.Collections.Generic;

public class CPHInline
{
    public bool Execute()
    {
        // ---- Scenes ----
        string discoScene = "Disco Party: Workspace";
        // ---- Rarity list (Toothless) ----
        var toothlessRarities = new List<string>
        {
            "regular",
            "smol",
            "long",
            "flight",
            "party"
        };
        // =========================
        // OBS: Hide Toothless dancers + reset Toothless unlock flags
        // =========================
        foreach (var rarity in toothlessRarities)
        {
            string dancingSource = $"Toothless - Dancing - {rarity}";
            // Hide + refresh to avoid cached visibility issues
            CPH.ObsHideSource(discoScene, dancingSource);
            CPH.ObsShowSource(discoScene, dancingSource);
            CPH.ObsHideSource(discoScene, dancingSource);
            // Reset unlock flags for the stream
            CPH.SetGlobalVar($"rarity_{rarity}", false, false);
        }

        // =========================
        // Globals: Toothless roll bookkeeping
        // =========================
        CPH.SetGlobalVar("last_roll", 0, false);
        CPH.SetGlobalVar("last_rarity", "", false);
        CPH.SetGlobalVar("last_user", "", false);
        // NOTE: Per-user boosts are stored as boost_<member>_<userId>.
        // We cannot reliably clear all of those without a roster of userIds.
        // They are non-persisted anyway, and will naturally clear on Streamer.bot restart.
        // =========================
        // Offerings / LotAT state
        // =========================
        CPH.SetGlobalVar("lotat_active", false, false); // start stream with LotAT off (change if you prefer)
        CPH.SetGlobalVar("lotat_announcement_sent", false, false); // allow fresh announcement next time LotAT starts
        CPH.SetGlobalVar("lotat_offering_steal_chance", 0, false); // safe default (no steals unless you set it)
        CPH.SetGlobalVar("lotat_steal_multiplier", 1, false); // default multiplier
        // =========================
        // Duck game state
        // =========================
        // Stop any active duck window + clear counters
        CPH.SetGlobalVar("duck_event_active", false, false);
        CPH.SetGlobalVar("duck_quack_count", 0, false);
        CPH.SetGlobalVar("duck_caller", "", false);
        // Reset duck unlock flag
        CPH.SetGlobalVar("duck_unlocked", false, false);
        // Hide Duck dancer on Disco Party scene
        CPH.ObsHideSource(discoScene, "Duck - Dancing");
        // Ensure timer is disabled at stream start (prevents late resolves)
        CPH.DisableTimer("Duck - Call Window");
        // =========================
        // Clone game state
        // =========================
        CPH.ObsHideSource("Disco Party: Workspace", "Clone - Dancing");
        CPH.SetGlobalVar("clone_unlocked", false, false);
        CPH.SetGlobalVar("clone_game_active", false, false);
        CPH.SetGlobalVar("clone_round", 0, false);
        CPH.SetGlobalVar("clone_positions_open", "", false);
        CPH.SetGlobalVar("clone_winners", "", false);
        CPH.DisableTimer("Clone - Volley Timer");
        return true;
    }
}