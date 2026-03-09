using System;
using System.Collections.Generic;
using System.Linq;

public class CPHInline
{
    // SYNC CONSTANTS (Clone feature)
    // Keep these names identical across:
    // - Actions/Squad/Clone/clone-main.cs
    // - Actions/Squad/Clone/clone-position.cs
    // - Actions/Squad/Clone/clone-volley.cs
    // - Actions/Twitch Integration/stream-start.cs
    private const string VAR_CLONE_GAME_ACTIVE = "clone_game_active";
    private const string VAR_CLONE_POSITIONS_COUNT = "clone_positions_count";
    private const string VAR_CLONE_ROUND = "clone_round";
    private const string VAR_CLONE_POSITION_REMOVED_LAST = "clone_position_removed_last";
    private const string VAR_CLONE_POSITIONS_OPEN = "clone_positions_open";
    private const string VAR_CLONE_WINNERS = "clone_winners";
    private const string VAR_CLONE_ROUND1_POOL = "clone_round1_pool";
    private const string TIMER_CLONE_VOLLEY = "Clone - Volley Timer";

    /*
     * Purpose:
     * - Starts the Clone mini-game and initializes all runtime state.
     *
     * Expected trigger/input:
     * - Streamer.bot action/command that starts the Clone event.
     * - No required chat args.
     *
     * Required runtime variables:
     * - Uses non-persisted global vars prefixed with clone_*
     *   (set here and consumed by clone-position + clone-volley scripts).
     *
     * Key outputs/side effects:
     * - Enables timer: "Clone - Volley Timer"
     * - Sends chat start message.
     * - Resets game state so each run starts clean.
     *
     * Operator notes:
     * - Ensure timer "Clone - Volley Timer" exists and points to clone-volley logic.
     * - This script is copy/paste safe for Streamer.bot.
     */
    public bool Execute()
    {
        // If you want to scale game board size later, change this one value.
        int positionsCount = 5;

        // Guard: do not start a second game while one is active.
        bool active = (CPH.GetGlobalVar<bool?>(VAR_CLONE_GAME_ACTIVE, false) ?? false);
        if (active)
        {
            CPH.SendMessage($"🪖 The Empire is already advancing! Choose a position: !rebel 1-{positionsCount}");
            return true;
        }

        // ----- Core game state reset -----
        CPH.SetGlobalVar(VAR_CLONE_GAME_ACTIVE, true, false);
        CPH.SetGlobalVar(VAR_CLONE_POSITIONS_COUNT, positionsCount, false);

        // Round 1 is the open enrollment round.
        CPH.SetGlobalVar(VAR_CLONE_ROUND, 1, false);

        // Stores last removed position (diagnostics / overlays).
        CPH.SetGlobalVar(VAR_CLONE_POSITION_REMOVED_LAST, 0, false);

        // Open positions are stored as CSV (example: "1,2,3,4,5").
        string openCsv = string.Join(",", Enumerable.Range(1, positionsCount));
        CPH.SetGlobalVar(VAR_CLONE_POSITIONS_OPEN, openCsv, false);

        // Winners are stored as userId CSV.
        CPH.SetGlobalVar(VAR_CLONE_WINNERS, "", false);

        // Round 1 pool is frozen by clone-volley on first timer fire.
        CPH.SetGlobalVar(VAR_CLONE_ROUND1_POOL, "", false);

        // Clear roster for each position key: clone_pos_<n>
        for (int p = 1; p <= positionsCount; p++)
        {
            CPH.SetGlobalVar($"clone_pos_{p}", "", false);
        }

        // Note: we do not clear every clone_player_pos_<userId> key globally.
        // That would require scanning all users and is unnecessary.
        // Active rosters + round logic rebuild winner truth each volley.

        // Start volley cycle.
        CPH.EnableTimer(TIMER_CLONE_VOLLEY);

        // Notify chat the game has begun.
        CPH.SendMessage($"🧬 CLONE EVENT: The Empire arrives! You have 30 seconds — pick a position: !rebel 1-{positionsCount}");

        return true;
    }
}
