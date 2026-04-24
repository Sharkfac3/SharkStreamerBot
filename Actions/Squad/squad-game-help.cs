using System;
using System.Collections.Generic;

public class CPHInline
{
    private const string ARG_USER   = "user";
    private const string ARG_INPUT0 = "input0";

    /*
     * Purpose:
     * - !game          → lists all available squad mini-games in chat.
     * - !game <name>   → explains the rules of the named mini-game.
     *
     * Expected trigger/input:
     * - Chat command wired to !game.
     * - Reads: user, input0 (first word after command, lowercased by Streamer.bot).
     *
     * Key outputs/side effects:
     * - Sends 1 chat message (list or rules).
     *
     * Operator notes:
     * - No globals read or written.
     * - Add new games to the helpMessages dictionary as they are built.
     */
    public bool Execute()
    {
        string caller = GetArg(ARG_USER);
        string input  = GetArg(ARG_INPUT0).ToLowerInvariant();

        var helpMessages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["pedro"]    = $"@{caller} PEDRO: Type !pedro to open a call window. Chat has to say \"pedro\" 100+ times before the timer runs out. Hit the target and Pedro unlocks! Replay has a 5-minute cooldown. 🦀",
            ["duck"]     = $"@{caller} DUCK: When a Duck event is active, spam \"quack\" in chat! The hidden target scales up as more unique chatters join in. Hit the threshold before the timer and Duck unlocks! 🦆",
            ["clone"]    = $"@{caller} CLONE: When Clone starts, pick a position 1-5 with !rebel <number>. Each round one position is eliminated — move fast or get wiped. Survive all rounds and win! 🤖",
            ["toothless"] = $"@{caller} TOOTHLESS: Trigger a roll to try your luck at a Toothless rarity. Five rarities: regular, smol, long, flight, party. Each rarity can only be unlocked once per stream. Your boost stat affects the odds. 🐉",
        };

        if (string.IsNullOrWhiteSpace(input))
        {
            string gameList = string.Join(", ", helpMessages.Keys);
            CPH.SendMessage($"@{caller} Squad mini-games available: {gameList}. Type !game <name> to learn the rules!");
            return true;
        }

        if (helpMessages.TryGetValue(input, out string rules))
        {
            CPH.SendMessage(rules);
            return true;
        }

        string knownGames = string.Join(", ", helpMessages.Keys);
        CPH.SendMessage($"@{caller} \"{input}\" is not a known squad game. Available: {knownGames}. Try !game <name>.");
        return true;
    }

    private string GetArg(string key)
    {
        if (CPH.TryGetArg(key, out string value) && !string.IsNullOrWhiteSpace(value))
            return value.Trim();

        return string.Empty;
    }
}
