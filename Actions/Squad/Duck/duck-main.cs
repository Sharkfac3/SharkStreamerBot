using System;

public class CPHInline
{
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
        const string TIMER_NAME = "Duck - Call Window";

        // Prevent overlapping events.
        bool active = (CPH.GetGlobalVar<bool?>("duck_event_active", false) ?? false);
        if (active)
        {
            CPH.SendMessage("🦆 Duck is already on the river... QUACK HARDER!");
            return true;
        }

        // Reset event state for new run.
        CPH.SetGlobalVar("duck_event_active", true, false);
        CPH.SetGlobalVar("duck_quack_count", 0, false);

        // Optional flavor: who called Duck.
        string user = "";
        CPH.TryGetArg("user", out user);
        CPH.SetGlobalVar("duck_caller", user ?? "", false);

        // Start timer that will resolve event outcome.
        CPH.EnableTimer(TIMER_NAME);

        // Notify chat.
        CPH.SendMessage("🦆 Duck has been called to the river! You have 2 minutes — spam **quack**!");
        return true;
    }
}
