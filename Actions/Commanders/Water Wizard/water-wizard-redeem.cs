using System;

public class CPHInline
{
    // SYNC CONSTANTS (Commanders)
    // Keep these names aligned with:
    // - Actions/Commanders/Captain Stretch/captain-stretch-redeem.cs
    // - Actions/Commanders/The Director/the-director-redeem.cs
    // - Actions/Commanders/Water Wizard/water-wizard-redeem.cs
    private const string ARG_USER = "user";
    private const string VAR_CURRENT_WATER_WIZARD = "current_water_wizard";

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
        CPH.TryGetArg(ARG_USER, out user);

        // If missing/blank, no-op safely.
        if (string.IsNullOrWhiteSpace(user))
            return true;

        // Save current commander owner for Water Wizard slot.
        CPH.SetGlobalVar(VAR_CURRENT_WATER_WIZARD, user, false);
        return true;
    }
}
