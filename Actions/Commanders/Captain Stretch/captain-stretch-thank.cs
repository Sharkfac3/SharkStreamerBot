using System;

public class CPHInline
{
    // SYNC CONSTANTS (Captain Stretch support command)
    private const string ARG_USER = "user";
    private const string VAR_CURRENT_CAPTAIN_STRETCH = "current_captain_stretch";
    private const string VAR_CAPTAIN_STRETCH_THANK_COUNT = "captain_stretch_thank_count";

    /*
     * Purpose:
     * - Handles public !thank support for the currently active Captain Stretch.
     * - Increments the current tenure thank counter by 1 for each valid use.
     *
     * Expected trigger/input:
     * - Chat command/action trigger for !thank.
     * - Reads: user
     *
     * Required runtime variables:
     * - Reads current_captain_stretch
     * - Reads/Writes captain_stretch_thank_count
     *
     * Key outputs/side effects:
     * - Blocks self-support (current captain cannot !thank themselves).
     * - Sends chat feedback for success/failure paths.
     * - Future hook: Mix It Up support call can be added in success path.
     */
    public bool Execute()
    {
        string caller = GetArg(ARG_USER);
        if (string.IsNullOrWhiteSpace(caller))
            return true;

        string currentCaptain = CPH.GetGlobalVar<string>(VAR_CURRENT_CAPTAIN_STRETCH, false) ?? string.Empty;
        if (string.IsNullOrWhiteSpace(currentCaptain))
        {
            CPH.SendMessage($"@{caller} there is no active Captain Stretch to thank right now. Redeem to appoint one! 💪");
            return true;
        }

        if (IsSameUser(caller, currentCaptain))
        {
            CPH.SendMessage($"@{caller} you cannot !thank yourself while you are Captain Stretch. 💪");
            return true;
        }

        int newCount = (CPH.GetGlobalVar<int?>(VAR_CAPTAIN_STRETCH_THANK_COUNT, false) ?? 0) + 1;
        CPH.SetGlobalVar(VAR_CAPTAIN_STRETCH_THANK_COUNT, newCount, false);

        CPH.SendMessage($"💪 @{caller} thanks Captain Stretch {currentCaptain}! Current thank count: {newCount}.");

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
