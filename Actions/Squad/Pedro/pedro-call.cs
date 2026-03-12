using System;

public class CPHInline
{
    // SYNC CONSTANTS (Pedro feature)
    // Keep these names identical across:
    // - Actions/Squad/Pedro/pedro-main.cs
    // - Actions/Squad/Pedro/pedro-call.cs
    // - Actions/Squad/Pedro/pedro-resolve.cs
    // - Actions/Twitch Integration/stream-start.cs
    private const string VAR_PEDRO_GAME_ENABLED = "pedro_game_enabled";
    private const string VAR_PEDRO_MENTION_COUNT = "pedro_mention_count";

    /*
     * Purpose:
     * - Counts "pedro" mentions from chat while Pedro event is active.
     *
     * Expected trigger/input:
     * - Message/chat trigger while Pedro event is active.
     * - Reads message (fallback rawInput).
     *
     * Required runtime variables:
     * - pedro_game_enabled
     * - pedro_mention_count
     *
     * Key outputs/side effects:
     * - Adds the number of "pedro" occurrences found in message text.
     */
    public bool Execute()
    {
        bool active = (CPH.GetGlobalVar<bool?>(VAR_PEDRO_GAME_ENABLED, false) ?? false);
        if (!active)
            return true;

        string text = GetMessageText();
        if (string.IsNullOrWhiteSpace(text))
            return true;

        // This counter command should ignore explicit !pedro command lines.
        // Command handling belongs to pedro-main.cs.
        if (IsPedroCommandMessage(text))
            return true;

        int hits = CountOccurrences(text, "pedro");
        if (hits <= 0)
            return true;

        int current = (CPH.GetGlobalVar<int?>(VAR_PEDRO_MENTION_COUNT, false) ?? 0);
        CPH.SetGlobalVar(VAR_PEDRO_MENTION_COUNT, current + hits, false);

        return true;
    }

    private string GetMessageText()
    {
        string text = "";
        if (!CPH.TryGetArg("message", out text) || string.IsNullOrWhiteSpace(text))
            CPH.TryGetArg("rawInput", out text);

        return text ?? "";
    }

    private bool IsPedroCommandMessage(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        return text.TrimStart().StartsWith("!pedro", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns number of non-overlapping occurrences of "word" in text (case-insensitive).
    /// </summary>
    private int CountOccurrences(string text, string word)
    {
        int count = 0;
        int index = 0;

        string t = text.ToLowerInvariant();
        string w = word.ToLowerInvariant();

        while ((index = t.IndexOf(w, index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += w.Length;
        }

        return count;
    }
}
