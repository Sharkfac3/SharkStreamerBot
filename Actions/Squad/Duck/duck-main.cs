using System;

public class CPHInline
{
    // SYNC CONSTANTS (Duck feature)
    // Keep these names identical across:
    // - Actions/Squad/Duck/duck-main.cs
    // - Actions/Squad/Duck/duck-call.cs
    // - Actions/Squad/Duck/duck-resolve.cs
    // - Actions/Twitch Integration/stream-start.cs
    private const string VAR_DUCK_EVENT_ACTIVE = "duck_event_active";
    private const string VAR_DUCK_QUACK_COUNT = "duck_quack_count";
    private const string VAR_DUCK_CALLER = "duck_caller";
    private const string TIMER_DUCK_CALL_WINDOW = "Duck - Call Window";

    /*
     * Purpose:
     * - Starts the Duck mini-event (2-minute quack window).
     *
     * Expected trigger/input:
     * - Action/command to begin Duck event.
     * - Optional arg: user (caller for flavor tracking).
     *
     * Required runtime variables:
     * - duck_event_active
     * - duck_quack_count
     * - duck_caller
     *
     * Key outputs/side effects:
     * - Enables timer: "Duck - Call Window"
     * - Announces event in chat.
     */
    public bool Execute()
    {
        // Prevent overlapping events.
        bool active = (CPH.GetGlobalVar<bool?>(VAR_DUCK_EVENT_ACTIVE, false) ?? false);
        if (active)
        {
            CPH.SendMessage("🦆 Duck is already on the river... QUACK HARDER!");
            return true;
        }

        // Reset event state for new run.
        CPH.SetGlobalVar(VAR_DUCK_EVENT_ACTIVE, true, false);
        CPH.SetGlobalVar(VAR_DUCK_QUACK_COUNT, 0, false);

        // Optional flavor: who called Duck.
        string user = "";
        CPH.TryGetArg("user", out user);
        CPH.SetGlobalVar(VAR_DUCK_CALLER, user ?? "", false);

        // Start timer that will resolve event outcome.
        CPH.EnableTimer(TIMER_DUCK_CALL_WINDOW);

        // Notify chat.
        CPH.SendMessage("🦆 Duck has been called to the river! You have 2 minutes — spam **quack**!");
        return true;
    }
}
