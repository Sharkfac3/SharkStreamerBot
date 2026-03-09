using System;

public class CPHInline
{
    // SYNC CONSTANTS (The Director support command)
    private const string ARG_USER = "user";
    private const string VAR_CURRENT_THE_DIRECTOR = "current_the_director";
    private const string VAR_THE_DIRECTOR_AWARD_COUNT = "the_director_award_count";

    /*
     * Purpose:
     * - Handles public !award support for the currently active The Director.
     * - Increments the current tenure award counter by 1 for each valid use.
     *
     * Expected trigger/input:
     * - Chat command/action trigger for !award.
     * - Reads: user
     *
     * Required runtime variables:
     * - Reads current_the_director
     * - Reads/Writes the_director_award_count
     *
     * Key outputs/side effects:
     * - Blocks self-support (current director cannot !award themselves).
     * - Sends chat feedback for success/failure paths.
     * - Future hook: Mix It Up support call can be added in success path.
     */
    public bool Execute()
    {
        string caller = GetArg(ARG_USER);
        if (string.IsNullOrWhiteSpace(caller))
            return true;

        string currentDirector = CPH.GetGlobalVar<string>(VAR_CURRENT_THE_DIRECTOR, false) ?? string.Empty;
        if (string.IsNullOrWhiteSpace(currentDirector))
        {
            CPH.SendMessage($"@{caller} there is no active Director to award right now. Redeem to cast the role! 🎬");
            return true;
        }

        if (IsSameUser(caller, currentDirector))
        {
            CPH.SendMessage($"@{caller} you cannot !award yourself while you are The Director. 🎬");
            return true;
        }

        int newCount = (CPH.GetGlobalVar<int?>(VAR_THE_DIRECTOR_AWARD_COUNT, false) ?? 0) + 1;
        CPH.SetGlobalVar(VAR_THE_DIRECTOR_AWARD_COUNT, newCount, false);

        CPH.SendMessage($"🎬 @{caller} awards The Director {currentDirector}! Current award count: {newCount}.");

        return true;
    }

    private string GetArg(string key)
    {
        if (CPH.TryGetArg(key, out string value) && !string.IsNullOrWhiteSpace(value))
            return value.Trim();

        return string.Empty;
    }

    private bool IsSameUser(string a, string b)
    {
        return !string.IsNullOrWhiteSpace(a)
            && !string.IsNullOrWhiteSpace(b)
            && string.Equals(a.Trim(), b.Trim(), StringComparison.OrdinalIgnoreCase);
    }
}
