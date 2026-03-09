using System;

public class CPHInline
{
    // SYNC CONSTANTS (Duck feature)
    // Keep these names identical across:
    // - Actions/Squad/Duck/duck-main.cs
    // - Actions/Squad/Duck/duck-call.cs
    // - Actions/Squad/Duck/duck-resolve.cs
    // - Actions/Twitch Integration/stream-start.cs
    private const string VAR_DUCK_EVENT_ACTIVE = "duck_event_active";
    private const string VAR_DUCK_QUACK_COUNT = "duck_quack_count";

    /*
     * Purpose:
     * - Increments Duck event quack counter from chat activity.
     *
     * Expected trigger/input:
     * - Message/chat trigger while Duck event is active.
     * - Reads message (fallback rawInput).
     *
     * Required runtime variables:
     * - duck_event_active
     * - duck_quack_count
     *
     * Key outputs/side effects:
     * - Adds the number of "quack" occurrences found in message text.
     */
    public bool Execute()
    {
        // Count quacks only during active event window.
        bool active = (CPH.GetGlobalVar<bool?>(VAR_DUCK_EVENT_ACTIVE, false) ?? false);
        if (!active)
            return true;

        // Read message text; trigger payload varies by setup.
        string text = "";
        if (!CPH.TryGetArg("message", out text) || string.IsNullOrWhiteSpace(text))
        {
            CPH.TryGetArg("rawInput", out text);
        }

        if (string.IsNullOrWhiteSpace(text))
            return true;

        // Count substring matches of "quack" (case-insensitive).
        int hits = CountOccurrences(text, "quack");
        if (hits <= 0)
            return true;

        int current = (CPH.GetGlobalVar<int?>(VAR_DUCK_QUACK_COUNT, false) ?? 0);
        CPH.SetGlobalVar(VAR_DUCK_QUACK_COUNT, current + hits, false);

        return true;
    }

    /// <summary>
    /// Returns number of times word fragment appears in text.
    /// Uses simple substring search and allows repeated occurrences.
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
