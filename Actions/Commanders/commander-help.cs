using System;
using System.Collections.Generic;

public class CPHInline
{
    // SYNC CONSTANTS (Commanders)
    // Keep these names aligned with Actions/SHARED-CONSTANTS.md.
    private const string ARG_USER = "user";

    private const string VAR_CURRENT_CAPTAIN_STRETCH = "current_captain_stretch";
    private const string VAR_CURRENT_THE_DIRECTOR = "current_the_director";
    private const string VAR_CURRENT_WATER_WIZARD = "current_water_wizard";

    /*
     * Purpose:
     * - Gives the caller a short commander-specific help message in chat.
     * - If the caller currently holds one or more commander roles, the script lists
     *   the commands available to that commander role.
     * - If the caller is not an active commander, the script gives short guidance
     *   instead of staying silent.
     *
     * Expected trigger/input:
     * - Chat command or action trigger for a help command such as !commanderhelp.
     * - Reads: user
     *
     * Required runtime variables:
     * - Reads current_captain_stretch
     * - Reads current_the_director
     * - Reads current_water_wizard
     *
     * Key outputs/side effects:
     * - Sends 1 chat message per commander role the caller currently holds.
     * - Sends a short fallback message when the caller is not an active commander.
     *
     * Operator notes:
     * - This script does not create or change any globals.
     * - Wire this to the chat command name you want to use.
     */
    public bool Execute()
    {
        string caller = GetArg(ARG_USER);
        if (string.IsNullOrWhiteSpace(caller))
            return true;

        string currentCaptainStretch = CPH.GetGlobalVar<string>(VAR_CURRENT_CAPTAIN_STRETCH, false) ?? string.Empty;
        string currentDirector = CPH.GetGlobalVar<string>(VAR_CURRENT_THE_DIRECTOR, false) ?? string.Empty;
        string currentWaterWizard = CPH.GetGlobalVar<string>(VAR_CURRENT_WATER_WIZARD, false) ?? string.Empty;

        var helpMessages = new List<string>();

        // A caller may theoretically hold more than one commander role,
        // so check each role independently and send help for all matches.
        if (IsSameUser(caller, currentCaptainStretch))
        {
            helpMessages.Add(
                $"@{caller} Captain Stretch briefing: use !stretch [up to 5 words] and !shrimp [up to 30 words]. The crew can back your command with !thank. 💪");
        }

        if (IsSameUser(caller, currentDirector))
        {
            helpMessages.Add(
                $"@{caller} The Director briefing: use !checkchat [optional text], !toad [optional text], and !primary / !secondary to swap the mapped OBS layout for the current scene. The crew can support the board with !award. 🎬");
        }

        if (IsSameUser(caller, currentWaterWizard))
        {
            helpMessages.Add(
                $"@{caller} Water Wizard briefing: use !hydrate <1-10 or short message> and !orb [optional message]. The crew can encourage your wisdom with !hail. 🌊");
        }

        if (helpMessages.Count == 0)
        {
            CPH.SendMessage($"@{caller} you are not on the commander deck right now. Redeem a commander role first, then run this help command again for your bridge briefing. 🛸");
            return true;
        }

        // Keep each response short and role-specific so chat stays readable.
        for (int i = 0; i < helpMessages.Count; i++)
            CPH.SendMessage(helpMessages[i]);

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
