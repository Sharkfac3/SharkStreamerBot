// ACTION-CONTRACT: Actions/Commanders/The Director/AGENTS.md#the-director-award.cs
// ACTION-CONTRACT-SHA256: 6a391a07b67a5a8e69d33cfb15bdb0d36b40a471fe2c4ac41ef8f59472c9f4f0

using System;

public class CPHInline
{
    // Runtime source of truth: Actions/Commanders/The Director/README.md
    // Shared names/constants reference: Actions/SHARED-CONSTANTS.md
    private const string ARG_USER = "user";
    private const string VAR_CURRENT_THE_DIRECTOR = "current_the_director";
    private const string VAR_THE_DIRECTOR_AWARD_COUNT = "the_director_award_count";

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
