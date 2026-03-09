using System;

public class CPHInline
{
    // SYNC CONSTANTS (Commanders)
    // Keep these names aligned with:
    // - Actions/Commanders/Captain Stretch/captain-stretch-redeem.cs
    // - Actions/Commanders/The Director/the-director-redeem.cs
    // - Actions/Commanders/Water Wizard/water-wizard-redeem.cs
    private const string ARG_USER = "user";
    private const string VAR_CURRENT_CAPTAIN_STRETCH = "current_captain_stretch";

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
        CPH.TryGetArg(ARG_USER, out user);

        // If missing/blank, no-op safely.
        if (string.IsNullOrWhiteSpace(user))
            return true;

        // Save current commander owner for Captain Stretch slot.
        CPH.SetGlobalVar(VAR_CURRENT_CAPTAIN_STRETCH, user, false);
        return true;
    }
}
