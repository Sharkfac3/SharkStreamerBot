// ACTION-CONTRACT: Actions/Commanders/AGENTS.md#commanders.cs
// ACTION-CONTRACT-SHA256: f1d82177c03f60f6643e1ca525d332a517fdabbdd455ddb7e5f7e51705e17e6f

using System;
using System.Collections.Generic;

public class CPHInline
{
    // SYNC CONSTANTS (Commanders)
    // Keep these names aligned with Actions/SHARED-CONSTANTS.md.
    private const string VAR_CURRENT_CAPTAIN_STRETCH = "current_captain_stretch";
    private const string VAR_CURRENT_THE_DIRECTOR = "current_the_director";
    private const string VAR_CURRENT_WATER_WIZARD = "current_water_wizard";

    /*
     * Purpose:
     * - Tells chat who currently holds each commander slot.
     * - Keeps the commander roster read-only and safe when no commanders are active.
     *
     * Expected trigger/input:
     * - Chat command: !commanders
     *
     * Required runtime variables:
     * - Reads current_captain_stretch
     * - Reads current_the_director
     * - Reads current_water_wizard
     *
     * Key outputs/side effects:
     * - Sends one chat message listing active Captain Stretch, The Director, and Water Wizard slots.
     * - Prefixes listed commander usernames with @ so Twitch sends mention notifications.
     * - Sends a fallback message when all commander slots are open.
     * - Does not create or change any global variables.
     */
    public bool Execute()
    {
        string captainStretch = GetCommander(VAR_CURRENT_CAPTAIN_STRETCH);
        string director = GetCommander(VAR_CURRENT_THE_DIRECTOR);
        string waterWizard = GetCommander(VAR_CURRENT_WATER_WIZARD);

        var activeSlots = new List<string>();

        if (!string.IsNullOrWhiteSpace(captainStretch))
            activeSlots.Add($"Captain Stretch: {FormatMention(captainStretch)}");

        if (!string.IsNullOrWhiteSpace(director))
            activeSlots.Add($"The Director: {FormatMention(director)}");

        if (!string.IsNullOrWhiteSpace(waterWizard))
            activeSlots.Add($"Water Wizard: {FormatMention(waterWizard)}");

        if (activeSlots.Count == 0)
        {
            CPH.SendMessage("Commander deck is currently open: Captain Stretch, The Director, and Water Wizard are all awaiting redeems. 🛸");
            return true;
        }

        CPH.SendMessage($"Active commanders — {string.Join(" | ", activeSlots)}. Use !commanderhelp if you are on deck and need your briefing.");
        return true;
    }

    private string GetCommander(string globalName)
    {
        string value = CPH.GetGlobalVar<string>(globalName, false) ?? string.Empty;
        return value.Trim();
    }

    private string FormatMention(string userName)
    {
        string trimmed = (userName ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            return string.Empty;

        return "@" + trimmed.TrimStart('@');
    }
}

