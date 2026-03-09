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

    // Water Wizard support-score tracking.
    private const string VAR_WATER_WIZARD_HAIL_COUNT = "water_wizard_hail_count";
    private const string VAR_WATER_WIZARD_HAIL_HIGH_SCORE = "water_wizard_hail_high_score";
    private const string VAR_WATER_WIZARD_HAIL_HIGH_SCORE_USER = "water_wizard_hail_high_score_user";

    /*
     * Purpose:
     * - Assigns the current Water Wizard commander slot to the redeeming user.
     * - Before replacing the slot owner, finalizes the previous wizard's hail score.
     * - If previous score beats the stored high score, writes a new persistent high score and announces it.
     *
     * Expected trigger/input:
     * - Commander redeem action for Water Wizard.
     * - Reads: user
     *
     * Required runtime variables:
     * - Reads/Writes current_water_wizard
     * - Reads/Writes water_wizard_hail_count
     * - Reads/Writes (persisted) water_wizard_hail_high_score
     * - Reads/Writes (persisted) water_wizard_hail_high_score_user
     *
     * Key outputs/side effects:
     * - Updates global var current_water_wizard with latest valid username.
     * - Resets water_wizard_hail_count to 0 for the new wizard tenure.
     * - Announces new high score in chat when beaten.
     */
    public bool Execute()
    {
        // Start with a safe default in case trigger does not include user.
        string newWizard = string.Empty;
        CPH.TryGetArg(ARG_USER, out newWizard);

        // If missing/blank, no-op safely.
        if (string.IsNullOrWhiteSpace(newWizard))
            return true;

        newWizard = newWizard.Trim();

        // Finalize the outgoing wizard (if any) before assigning the new wizard.
        string previousWizard = CPH.GetGlobalVar<string>(VAR_CURRENT_WATER_WIZARD, false) ?? string.Empty;
        int previousCount = CPH.GetGlobalVar<int?>(VAR_WATER_WIZARD_HAIL_COUNT, false) ?? 0;

        int existingHighScore = CPH.GetGlobalVar<int?>(VAR_WATER_WIZARD_HAIL_HIGH_SCORE, true) ?? 0;

        if (!string.IsNullOrWhiteSpace(previousWizard) && previousCount > existingHighScore)
        {
            // Persist high score data so it survives Streamer.bot restarts.
            CPH.SetGlobalVar(VAR_WATER_WIZARD_HAIL_HIGH_SCORE, previousCount, true);
            CPH.SetGlobalVar(VAR_WATER_WIZARD_HAIL_HIGH_SCORE_USER, previousWizard.Trim(), true);

            CPH.SendMessage($"🏆 New Water Wizard hail record! {previousWizard} finished with {previousCount} hail(s)! 🌊");
        }

        // Save current commander owner for Water Wizard slot.
        CPH.SetGlobalVar(VAR_CURRENT_WATER_WIZARD, newWizard, false);

        // Reset active tenure counter for the new wizard.
        CPH.SetGlobalVar(VAR_WATER_WIZARD_HAIL_COUNT, 0, false);
        return true;
    }
}
