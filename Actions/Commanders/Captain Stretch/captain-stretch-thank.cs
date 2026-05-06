// ACTION-CONTRACT: Actions/Commanders/Captain Stretch/AGENTS.md#captain-stretch-thank.cs
// ACTION-CONTRACT-SHA256: 5d5041632101af38c9f34cec59cd2e044a850d503a9da9085c20bfb9e1419121

using System;

public class CPHInline
{
    // Runtime source of truth: Actions/Commanders/Captain Stretch/README.md
    // Shared names/constants reference: Actions/SHARED-CONSTANTS.md
    private const string ARG_USER = "user";
    private const string VAR_CURRENT_CAPTAIN_STRETCH = "current_captain_stretch";
    private const string VAR_CAPTAIN_STRETCH_THANK_COUNT = "captain_stretch_thank_count";

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
