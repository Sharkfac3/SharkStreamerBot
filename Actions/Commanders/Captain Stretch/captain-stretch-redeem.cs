using System;

public class CPHInline
{
    /*
     * Purpose:
     * - Assigns the current Captain Stretch commander slot to the redeeming user.
     *
     * Expected trigger/input:
     * - Commander redeem action for Captain Stretch.
     * - Reads: user
     *
     * Required runtime variables:
     * - Writes current_captain_stretch
     *
     * Key outputs/side effects:
     * - Updates global var current_captain_stretch with latest valid username.
     */
    public bool Execute()
    {
        // Start with a safe default in case trigger does not include user.
        string user = string.Empty;
        CPH.TryGetArg("user", out user);

        // If missing/blank, no-op safely.
        if (string.IsNullOrWhiteSpace(user))
            return true;

        // Save current commander owner for Captain Stretch slot.
        CPH.SetGlobalVar("current_captain_stretch", user, false);
        return true;
    }
}
