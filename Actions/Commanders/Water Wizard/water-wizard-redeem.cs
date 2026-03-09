using System;

public class CPHInline
{
    /*
     * Purpose:
     * - Assigns the current Water Wizard commander slot to the redeeming user.
     *
     * Expected trigger/input:
     * - Commander redeem action for Water Wizard.
     * - Reads: user
     *
     * Required runtime variables:
     * - Writes current_water_wizard
     *
     * Key outputs/side effects:
     * - Updates global var current_water_wizard with latest valid username.
     */
    public bool Execute()
    {
        // Start with a safe default in case trigger does not include user.
        string user = string.Empty;
        CPH.TryGetArg("user", out user);

        // If missing/blank, no-op safely.
        if (string.IsNullOrWhiteSpace(user))
            return true;

        // Save current commander owner for Water Wizard slot.
        CPH.SetGlobalVar("current_water_wizard", user, false);
        return true;
    }
}
