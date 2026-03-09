using System;

public class CPHInline
{
    /*
     * Purpose:
     * - Assigns the current The Director commander slot to the redeeming user.
     *
     * Expected trigger/input:
     * - Commander redeem action for The Director.
     * - Reads: user
     *
     * Required runtime variables:
     * - Writes current_the_director
     *
     * Key outputs/side effects:
     * - Updates global var current_the_director with latest valid username.
     */
    public bool Execute()
    {
        // Start with a safe default in case trigger does not include user.
        string user = string.Empty;
        CPH.TryGetArg("user", out user);

        // If missing/blank, no-op safely.
        if (string.IsNullOrWhiteSpace(user))
            return true;

        // Save current commander owner for The Director slot.
        CPH.SetGlobalVar("current_the_director", user, false);
        return true;
    }
}
