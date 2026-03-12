using System;

public class CPHInline
{
    // SYNC CONSTANTS (Commanders)
    // Keep these names aligned with:
    // - Actions/Commanders/Captain Stretch/captain-stretch-redeem.cs
    // - Actions/Commanders/The Director/the-director-redeem.cs
    // - Actions/Commanders/Water Wizard/water-wizard-redeem.cs
    private const string ARG_USER = "user";
    private const string VAR_CURRENT_THE_DIRECTOR = "current_the_director";

    // The Director support-score tracking.
    private const string VAR_THE_DIRECTOR_AWARD_COUNT = "the_director_award_count";
    private const string VAR_THE_DIRECTOR_AWARD_HIGH_SCORE = "the_director_award_high_score";
    private const string VAR_THE_DIRECTOR_AWARD_HIGH_SCORE_USER = "the_director_award_high_score_user";

    // The Director command cooldown tracking.
    private const string VAR_THE_DIRECTOR_CHECKCHAT_NEXT_ALLOWED_UTC = "the_director_checkchat_next_allowed_utc";
    private const string VAR_THE_DIRECTOR_TOAD_NEXT_ALLOWED_UTC = "the_director_toad_next_allowed_utc";

    /*
     * Purpose:
     * - Assigns the current The Director commander slot to the redeeming user.
     * - Before replacing the slot owner, finalizes the previous director's award score.
     * - If previous score beats the stored high score, writes a new persistent high score and announces it.
     *
     * Expected trigger/input:
     * - Commander redeem action for The Director.
     * - Reads: user
     *
     * Required runtime variables:
     * - Reads/Writes current_the_director
     * - Reads/Writes the_director_award_count
     * - Reads/Writes the_director_checkchat_next_allowed_utc
     * - Reads/Writes the_director_toad_next_allowed_utc
     * - Reads/Writes (persisted) the_director_award_high_score
     * - Reads/Writes (persisted) the_director_award_high_score_user
     *
     * Key outputs/side effects:
     * - Updates global var current_the_director with latest valid username.
     * - Resets the_director_award_count to 0 for the new director tenure.
     * - Resets The Director command cooldowns so the new director starts fresh.
     * - Announces new high score in chat when beaten.
     */
    public bool Execute()
    {
        // Start with a safe default in case trigger does not include user.
        string newDirector = string.Empty;
        CPH.TryGetArg(ARG_USER, out newDirector);

        // If missing/blank, no-op safely.
        if (string.IsNullOrWhiteSpace(newDirector))
            return true;

        newDirector = newDirector.Trim();

        // Finalize the outgoing director (if any) before assigning the new director.
        string previousDirector = CPH.GetGlobalVar<string>(VAR_CURRENT_THE_DIRECTOR, false) ?? string.Empty;
        int previousCount = CPH.GetGlobalVar<int?>(VAR_THE_DIRECTOR_AWARD_COUNT, false) ?? 0;

        int existingHighScore = CPH.GetGlobalVar<int?>(VAR_THE_DIRECTOR_AWARD_HIGH_SCORE, true) ?? 0;

        if (!string.IsNullOrWhiteSpace(previousDirector) && previousCount > existingHighScore)
        {
            // Persist high score data so it survives Streamer.bot restarts.
            CPH.SetGlobalVar(VAR_THE_DIRECTOR_AWARD_HIGH_SCORE, previousCount, true);
            CPH.SetGlobalVar(VAR_THE_DIRECTOR_AWARD_HIGH_SCORE_USER, previousDirector.Trim(), true);

            CPH.SendMessage($"🏆 New Director award record! {previousDirector} finished with {previousCount} award(s)! 🎬");
        }

        // Save current commander owner for The Director slot.
        CPH.SetGlobalVar(VAR_CURRENT_THE_DIRECTOR, newDirector, false);

        // Reset active tenure counter for the new director.
        CPH.SetGlobalVar(VAR_THE_DIRECTOR_AWARD_COUNT, 0, false);

        // Reset all Director command cooldowns for the new tenure.
        CPH.SetGlobalVar(VAR_THE_DIRECTOR_CHECKCHAT_NEXT_ALLOWED_UTC, 0L, false);
        CPH.SetGlobalVar(VAR_THE_DIRECTOR_TOAD_NEXT_ALLOWED_UTC, 0L, false);
        return true;
    }
}
