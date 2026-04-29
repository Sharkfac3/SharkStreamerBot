using System;

public class CPHInline
{
    // Runtime source of truth: Actions/Commanders/Water Wizard/README.md
    // Shared names/constants reference: Actions/SHARED-CONSTANTS.md
    private const string ARG_USER = "user";
    private const string VAR_CURRENT_WATER_WIZARD = "current_water_wizard";
    private const string VAR_WATER_WIZARD_HAIL_COUNT = "water_wizard_hail_count";

    public bool Execute()
    {
        string caller = GetArg(ARG_USER);
        if (string.IsNullOrWhiteSpace(caller))
            return true;

        string currentWizard = CPH.GetGlobalVar<string>(VAR_CURRENT_WATER_WIZARD, false) ?? string.Empty;
        if (string.IsNullOrWhiteSpace(currentWizard))
        {
            CPH.SendMessage($"@{caller} there is no active Water Wizard to hail right now. Redeem to crown one! 🌊");
            return true;
        }

        if (IsSameUser(caller, currentWizard))
        {
            CPH.SendMessage($"@{caller} you cannot !hail yourself while you are the Water Wizard. 🌊");
            return true;
        }

        int newCount = (CPH.GetGlobalVar<int?>(VAR_WATER_WIZARD_HAIL_COUNT, false) ?? 0) + 1;
        CPH.SetGlobalVar(VAR_WATER_WIZARD_HAIL_COUNT, newCount, false);

        CPH.SendMessage($"🌊 @{caller} hails our Water Wizard {currentWizard}! Current hail count: {newCount}.");

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
