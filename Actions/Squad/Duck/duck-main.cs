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
    private const string VAR_DUCK_QUACK_COUNT = "duck_quack_count";
    private const string VAR_DUCK_CALLER = "duck_caller";
    private const string VAR_DUCK_TARGET_QUACKS = "duck_target_quacks";
    private const string VAR_DUCK_UNIQUE_QUACKERS = "duck_unique_quackers";
    private const string VAR_DUCK_UNIQUE_QUACKER_COUNT = "duck_unique_quacker_count";
    private const string TIMER_DUCK_CALL_WINDOW = "Duck - Call Window";

    // Shared mini-game lock (cross-feature).
    private const string VAR_MINIGAME_ACTIVE = "minigame_active";
    private const string VAR_MINIGAME_NAME = "minigame_name";
    private const string MINIGAME_NAME_DUCK = "duck";

    // Duck now unlocks immediately when chat reaches the live threshold.
    // The timer is only the maximum time allowed before the attempt fails.
    private const int DUCK_MINIMUM_TARGET_QUACKS = 12;

    /*
     * Purpose:
     * - Starts the Duck mini-event and opens the live quack race.
     *
     * Expected trigger/input:
     * - Action/command to begin Duck event.
     * - Optional arg: user (caller for flavor tracking).
     *
     * Required runtime variables:
     * - duck_event_active
     * - duck_quack_count
     * - duck_caller
     * - duck_target_quacks
     * - duck_unique_quackers
     * - duck_unique_quacker_count
     * - shared lock: minigame_active/minigame_name
     *
     * Key outputs/side effects:
     * - Enables timer: "Duck - Call Window"
     * - Announces the event in chat without exposing the exact quack target.
     */
    public bool Execute()
    {
        // Prevent overlap across all mini-games.
        if (!TryAcquireMiniGameLock())
        {
            string activeGame = CPH.GetGlobalVar<string>(VAR_MINIGAME_NAME, false) ?? "another mini-game";
            CPH.SendMessage($"🎮 A mini-game is already running ({activeGame}). Finish it before starting Duck.");
            return true;
        }

        // Prevent overlapping Duck events.
        bool active = (CPH.GetGlobalVar<bool?>(VAR_DUCK_EVENT_ACTIVE, false) ?? false);
        if (active)
        {
            CPH.SendMessage("🦆 Duck is already on the river. The crew is still quacking.");
            return true;
        }

        // Reset event state for a fresh run.
        CPH.SetGlobalVar(VAR_DUCK_EVENT_ACTIVE, true, false);
        CPH.SetGlobalVar(VAR_DUCK_QUACK_COUNT, 0, false);
        CPH.SetGlobalVar(VAR_DUCK_TARGET_QUACKS, DUCK_MINIMUM_TARGET_QUACKS, false);
        CPH.SetGlobalVar(VAR_DUCK_UNIQUE_QUACKERS, "|", false);
        CPH.SetGlobalVar(VAR_DUCK_UNIQUE_QUACKER_COUNT, 0, false);

        // Optional flavor: who called Duck.
        string user = "";
        CPH.TryGetArg("user", out user);
        CPH.SetGlobalVar(VAR_DUCK_CALLER, user ?? "", false);

        // Start timer that will fail the event if chat does not reach the target in time.
        CPH.EnableTimer(TIMER_DUCK_CALL_WINDOW);

        // Notify chat without revealing the exact threshold.
        // The quack requirement still scales with how many crew members join in.
        CPH.SendMessage("🦆 Duck has hit the river! Quack like you mean it before time runs out. The bigger the crew gets, the harder the duck is to corner.");
        return true;
    }

    /// <summary>
    /// Claims the shared mini-game lock when free.
    /// Allows re-entry only for Duck itself.
    /// </summary>
    private bool TryAcquireMiniGameLock()
    {
        bool lockActive = (CPH.GetGlobalVar<bool?>(VAR_MINIGAME_ACTIVE, false) ?? false);
        string lockName = CPH.GetGlobalVar<string>(VAR_MINIGAME_NAME, false) ?? "";

        if (lockActive && !string.Equals(lockName, MINIGAME_NAME_DUCK, StringComparison.OrdinalIgnoreCase))
            return false;

        CPH.SetGlobalVar(VAR_MINIGAME_ACTIVE, true, false);
        CPH.SetGlobalVar(VAR_MINIGAME_NAME, MINIGAME_NAME_DUCK, false);
        return true;
    }
}
