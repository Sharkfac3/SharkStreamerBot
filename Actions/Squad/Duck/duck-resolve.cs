// ACTION-CONTRACT: Actions/Squad/AGENTS.md#Duck/duck-resolve.cs
// ACTION-CONTRACT-SHA256: e8c9d37c2b85eec56c168dddd1e94c98015ab9ccfd3bd7bd42d2de1b1ac30a56

using System;

public class CPHInline
{
    // SYNC CONSTANTS (Duck feature)
    // Keep these names identical across:
    // - Actions/Squad/Duck/duck-main.cs
    // - Actions/Squad/Duck/duck-call.cs
    // - Actions/Squad/Duck/duck-resolve.cs
    // - Actions/Twitch Core Integrations/stream-start.cs
    private const string VAR_DUCK_EVENT_ACTIVE = "duck_event_active";
    private const string VAR_DUCK_UNIQUE_QUACKER_COUNT = "duck_unique_quacker_count";
    private const string TIMER_DUCK_CALL_WINDOW = "Duck - Call Window";

    // Shared mini-game lock (cross-feature).
    private const string VAR_MINIGAME_ACTIVE = "minigame_active";
    private const string VAR_MINIGAME_NAME = "minigame_name";
    private const string MINIGAME_NAME_DUCK = "duck";

    /*
     * Purpose:
     * - Fails the Duck event if chat does not reach the live target before time runs out.
     *
     * Expected trigger/input:
     * - Timer action: "Duck - Call Window"
     *
     * Required runtime variables:
     * - duck_event_active
     * - duck_unique_quacker_count
     * - shared lock: minigame_active/minigame_name
     *
     * Key outputs/side effects:
     * - Disables the Duck timer.
     * - Ends the active event if it timed out.
     * - Releases shared mini-game lock after the event ends.
     */
    public bool Execute()
    {
        // If the timer fires after a successful unlock or manual cleanup, make sure it stays off and exit.
        bool active = (CPH.GetGlobalVar<bool?>(VAR_DUCK_EVENT_ACTIVE, false) ?? false);
        if (!active)
        {
            CPH.DisableTimer(TIMER_DUCK_CALL_WINDOW);
            ReleaseMiniGameLockIfOwned();
            return true;
        }

        CPH.SetGlobalVar(VAR_DUCK_EVENT_ACTIVE, false, false);
        CPH.DisableTimer(TIMER_DUCK_CALL_WINDOW);

        int uniqueQuackers = (CPH.GetGlobalVar<int?>(VAR_DUCK_UNIQUE_QUACKER_COUNT, false) ?? 0);

        string failureMessage = uniqueQuackers <= 1
            ? "🦆❌ Duck got away. One crew member quacking into the void was not enough this time."
            : "🦆❌ Duck got away. The flock was strong, but not strong enough before time ran out.";

        CPH.SendMessage(failureMessage);

        ReleaseMiniGameLockIfOwned();
        return true;
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
}
