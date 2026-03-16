using System;
using System.Collections.Generic;

public class CPHInline
{
    // SYNC CONSTANTS (Clone feature)
    // Keep these names identical across:
    // - Actions/Squad/Clone/clone-main.cs
    // - Actions/Squad/Clone/clone-position.cs
    // - Actions/Squad/Clone/clone-volley.cs
    // - Actions/Twitch Core Integrations/stream-start.cs
    private const string VAR_CLONE_GAME_ACTIVE = "clone_game_active";
    private const string VAR_CLONE_POSITIONS_COUNT = "clone_positions_count";
    private const string VAR_CLONE_POSITIONS_OPEN = "clone_positions_open";
    private const string VAR_CLONE_ROUND = "clone_round";
    private const string VAR_CLONE_WINNERS = "clone_winners";

    /*
     * Purpose:
     * - Handles player position selection for the Clone mini-game (!rebel).
     *
     * Expected trigger/input:
     * - Chat command handler for !rebel.
     * - Reads user/userId + first numeric argument (position).
     *
     * Required runtime variables:
     * - clone_game_active
     * - clone_positions_count
     * - clone_positions_open
     * - clone_pos_<n>
     * - clone_player_pos_<userId>
     * - clone_round / clone_winners
     *
     * Key outputs/side effects:
     * - Moves player between position rosters.
     * - Updates per-player current position.
     * - During round 1 only: marks user as winner-eligible.
     */
    public bool Execute()
    {
        // Ignore command if no active game.
        bool active = (CPH.GetGlobalVar<bool?>(VAR_CLONE_GAME_ACTIVE, false) ?? false);
        if (!active)
            return true;

        // Standard user args from Streamer.bot.
        string user = "";
        string userId = "";
        CPH.TryGetArg("user", out user);
        CPH.TryGetArg("userId", out userId);

        if (string.IsNullOrWhiteSpace(user))
            return true;

        // Fallback if trigger didn't provide userId.
        if (string.IsNullOrWhiteSpace(userId))
            userId = user.ToLowerInvariant();

        int positionsCount = (CPH.GetGlobalVar<int?>(VAR_CLONE_POSITIONS_COUNT, false) ?? 5);

        // Parse desired position robustly across trigger variants.
        int desiredPos = 0;

        // Preferred arg: input0 (first command arg).
        string input0 = "";
        if (CPH.TryGetArg("input0", out input0))
            int.TryParse((input0 ?? "").Trim(), out desiredPos);

        // Fallback: parse first integer from rawInput.
        if (desiredPos == 0)
        {
            string rawInput = "";
            CPH.TryGetArg("rawInput", out rawInput);
            desiredPos = ExtractFirstInt(rawInput);
        }

        // Last fallback: parse from full message text.
        if (desiredPos == 0)
        {
            string message = "";
            CPH.TryGetArg("message", out message);
            desiredPos = ExtractFirstInt(message);
        }

        // Reject out-of-range picks silently.
        if (desiredPos < 1 || desiredPos > positionsCount)
            return true;

        // Position must still be open.
        var openPositions = ParseIntSet(CPH.GetGlobalVar<string>(VAR_CLONE_POSITIONS_OPEN, false) ?? "");
        if (!openPositions.Contains(desiredPos))
            return true;

        // Current tracked position for this user.
        // 0 means user is not currently standing anywhere.
        int currentPos = (CPH.GetGlobalVar<int?>($"clone_player_pos_{userId}", false) ?? 0);

        // Load target roster.
        var newRoster = ParseUserSet(CPH.GetGlobalVar<string>($"clone_pos_{desiredPos}", false) ?? "");

        // No-op only if state is already truly in sync.
        if (currentPos == desiredPos && newRoster.Contains(userId))
            return true;

        // Remove user from old roster if moving from another position.
        if (currentPos != 0 && currentPos != desiredPos)
        {
            var oldRoster = ParseUserSet(CPH.GetGlobalVar<string>($"clone_pos_{currentPos}", false) ?? "");
            oldRoster.Remove(userId);
            CPH.SetGlobalVar($"clone_pos_{currentPos}", string.Join(",", oldRoster), false);
        }

        // Add user to new roster and persist current position.
        newRoster.Add(userId);
        CPH.SetGlobalVar($"clone_pos_{desiredPos}", string.Join(",", newRoster), false);
        CPH.SetGlobalVar($"clone_player_pos_{userId}", desiredPos, false);

        // Round-1 eligibility rule:
        // only users who participated during round 1 can win the event.
        int round = (CPH.GetGlobalVar<int?>(VAR_CLONE_ROUND, false) ?? 1);
        if (round == 1)
        {
            var winners = ParseUserSet(CPH.GetGlobalVar<string>(VAR_CLONE_WINNERS, false) ?? "");
            winners.Add(userId);
            CPH.SetGlobalVar(VAR_CLONE_WINNERS, string.Join(",", winners), false);
        }

        return true;
    }

    /// <summary>
    /// Extracts the first integer token from input text.
    /// Examples:
    /// - "1" -> 1
    /// - "!rebel 3" -> 3
    /// - "abc" -> 0
    /// </summary>
    private int ExtractFirstInt(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        string[] parts = text.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var p in parts)
        {
            if (int.TryParse(p.Trim(), out int v))
                return v;
        }

        return 0;
    }

    /// <summary>
    /// Converts CSV ints ("1,2,5") into a HashSet for O(1) membership checks.
    /// </summary>
    private HashSet<int> ParseIntSet(string csv)
    {
        var set = new HashSet<int>();
        if (string.IsNullOrWhiteSpace(csv))
            return set;

        foreach (var part in csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            if (int.TryParse(part.Trim(), out int val))
                set.Add(val);
        }

        return set;
    }

    /// <summary>
    /// Converts CSV userIds into case-insensitive set to avoid duplicate casing issues.
    /// </summary>
    private HashSet<string> ParseUserSet(string csv)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(csv))
            return set;

        foreach (var part in csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var val = part.Trim();
            if (!string.IsNullOrEmpty(val))
                set.Add(val);
        }

        return set;
    }
}
