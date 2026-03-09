using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (Clone feature)
    // Keep these names identical across:
    // - Actions/Squad/Clone/clone-main.cs
    // - Actions/Squad/Clone/clone-position.cs
    // - Actions/Squad/Clone/clone-volley.cs
    // - Actions/Twitch Integration/stream-start.cs
    private const string VAR_CLONE_UNLOCKED = "clone_unlocked";
    private const string VAR_CLONE_GAME_ACTIVE = "clone_game_active";
    private const string VAR_CLONE_ROUND = "clone_round";
    private const string VAR_CLONE_POSITIONS_COUNT = "clone_positions_count";
    private const string VAR_CLONE_POSITIONS_OPEN = "clone_positions_open";
    private const string VAR_CLONE_POSITION_REMOVED_LAST = "clone_position_removed_last";
    private const string VAR_CLONE_WINNERS = "clone_winners";
    private const string VAR_CLONE_ROUND1_POOL = "clone_round1_pool";
    private const string TIMER_CLONE_VOLLEY = "Clone - Volley Timer";
    private const string OBS_SCENE_DISCO_WORKSPACE = "Disco Party: Workspace";
    private const string OBS_SOURCE_CLONE_DANCING = "Clone - Dancing";

    /*
     * Purpose:
     * - Resolves each Clone volley tick (timer-driven elimination round).
     *
     * Expected trigger/input:
     * - Timer action: "Clone - Volley Timer"
     *
     * Required runtime variables:
     * - clone_game_active, clone_round, clone_positions_open, clone_positions_count
     * - clone_pos_<n>, clone_player_pos_<userId>
     * - clone_winners, clone_round1_pool
     *
     * Key outputs/side effects:
     * - Removes one open position per volley.
     * - Rebuilds alive/winners set deterministically from rosters.
     * - Ends game on loss/win.
     * - On win: sets clone_unlocked, shows OBS source, triggers Mix It Up unlock command.
     */

    // RNG is static to reduce repeated seed patterns when actions run close together.
    private static Random _rng = null;

    // Mix It Up unlock bridge for Clone unlock events.
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_CLONE_UNLOCK_COMMAND_ID = "4681be93-409a-4110-bfdb-7a7aa32df63a";
    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    public bool Execute()
    {
        // Guard: if game already ended, ensure timer is disabled and exit safely.
        bool active = (CPH.GetGlobalVar<bool?>(VAR_CLONE_GAME_ACTIVE, false) ?? false);
        if (!active)
        {
            CPH.DisableTimer(TIMER_CLONE_VOLLEY);
            return true;
        }

        // Load persistent round state.
        int positionsCount = (CPH.GetGlobalVar<int?>(VAR_CLONE_POSITIONS_COUNT, false) ?? 5);
        int round = (CPH.GetGlobalVar<int?>(VAR_CLONE_ROUND, false) ?? 1);
        List<int> openPositions = ParseIntList(CPH.GetGlobalVar<string>(VAR_CLONE_POSITIONS_OPEN, false) ?? "");

        // Defensive fallback if open list becomes missing/corrupt.
        if (openPositions.Count == 0)
        {
            openPositions = Enumerable.Range(1, positionsCount).ToList();
            CPH.SetGlobalVar(VAR_CLONE_POSITIONS_OPEN, string.Join(",", openPositions), false);
        }

        // Terminal safety: if list is already collapsed, end game cleanly.
        if (openPositions.Count <= 1)
        {
            EndGame(TIMER_CLONE_VOLLEY);
            return true;
        }

        EnsureRandomInitialized();

        // Freeze round-1 participation once on first volley tick.
        // This enforces: only round-1 participants can ever be winners.
        if (round == 1)
        {
            string poolCsv = CPH.GetGlobalVar<string>(VAR_CLONE_ROUND1_POOL, false) ?? "";
            if (string.IsNullOrWhiteSpace(poolCsv))
            {
                string round1Csv = CPH.GetGlobalVar<string>(VAR_CLONE_WINNERS, false) ?? "";
                CPH.SetGlobalVar(VAR_CLONE_ROUND1_POOL, round1Csv, false);
            }
        }

        // 1) Destroy one random open position.
        int removedPosition = PickRandom(openPositions);
        openPositions.Remove(removedPosition);

        CPH.SetGlobalVar(VAR_CLONE_POSITION_REMOVED_LAST, removedPosition, false);
        CPH.SetGlobalVar(VAR_CLONE_POSITIONS_OPEN, string.Join(",", openPositions), false);

        // 2) Eliminate anyone standing in removed position.
        HashSet<string> removedUsers = ParseUserSet(CPH.GetGlobalVar<string>($"clone_pos_{removedPosition}", false) ?? "");
        foreach (string uid in removedUsers)
        {
            CPH.SetGlobalVar($"clone_player_pos_{uid}", 0, false);
        }

        // Position is gone; wipe roster key.
        CPH.SetGlobalVar($"clone_pos_{removedPosition}", "", false);

        // 3) Rebuild alive set from remaining open rosters.
        HashSet<string> alive = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (int pos in openPositions)
        {
            HashSet<string> roster = ParseUserSet(CPH.GetGlobalVar<string>($"clone_pos_{pos}", false) ?? "");
            foreach (string uid in roster)
                alive.Add(uid);
        }

        // 4) Winners = alive ∩ round1Pool.
        HashSet<string> round1Pool = ParseUserSet(CPH.GetGlobalVar<string>(VAR_CLONE_ROUND1_POOL, false) ?? "");
        HashSet<string> winners = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (string uid in alive)
        {
            if (round1Pool.Contains(uid))
                winners.Add(uid);
        }

        CPH.SetGlobalVar(VAR_CLONE_WINNERS, string.Join(",", winners), false);

        // Loss condition: no winner-eligible survivors while multiple positions still remain.
        if (winners.Count == 0)
        {
            CPH.SendMessage($"🧬 Position {removedPosition} has fallen! The Empire wipes out the rebellion... no survivors remain.");
            EndGame(TIMER_CLONE_VOLLEY);
            return true;
        }

        // Win condition: one position left and at least one winner-eligible survivor.
        if (openPositions.Count == 1)
        {
            CPH.SendMessage("🧬✅ The rebellion holds the final position… CLONE JOINS THE DISCO!");

            CPH.SetGlobalVar(VAR_CLONE_UNLOCKED, true, false);
            CPH.ObsShowSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_CLONE_DANCING);

            // Notify Mix It Up for unlock side-effects.
            TriggerMixItUpUnlock("clone");

            EndGame(TIMER_CLONE_VOLLEY);
            return true;
        }

        // Continue game.
        round += 1;
        CPH.SetGlobalVar(VAR_CLONE_ROUND, round, false);
        CPH.SendMessage($"🧬 Round {round}: Position {removedPosition} has fallen! You have 30 seconds to move — !rebel {string.Join("/", openPositions)}");

        // Re-enable timer for next volley (robust with one-shot timer configurations).
        CPH.EnableTimer(TIMER_CLONE_VOLLEY);
        return true;
    }

    /// <summary>
    /// Initializes RNG with time+counter seed to reduce repeated outcomes across rapid executions.
    /// </summary>
    private void EnsureRandomInitialized()
    {
        if (_rng != null)
            return;

        int counter = (CPH.GetGlobalVar<int?>("clone_rng_counter", false) ?? 0);
        counter++;
        CPH.SetGlobalVar("clone_rng_counter", counter, false);

        int seed = Environment.TickCount ^ (counter * 7919);
        _rng = new Random(seed);
    }

    /// <summary>
    /// Cleanly ends the game and disables future timer fires.
    /// </summary>
    private void EndGame(string timerName)
    {
        CPH.SetGlobalVar(VAR_CLONE_GAME_ACTIVE, false, false);
        CPH.DisableTimer(timerName);
    }

    /// <summary>
    /// Picks one item from a non-empty list.
    /// </summary>
    private int PickRandom(List<int> list)
    {
        int index = _rng.Next(0, list.Count);
        return list[index];
    }

    /// <summary>
    /// Parses CSV integer list ("1,2,5") into a strongly-typed List<int>.
    /// </summary>
    private List<int> ParseIntList(string csv)
    {
        var list = new List<int>();
        if (string.IsNullOrWhiteSpace(csv))
            return list;

        foreach (var part in csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            if (int.TryParse(part.Trim(), out int val))
                list.Add(val);
        }

        return list;
    }

    /// <summary>
    /// Parses CSV userId list into case-insensitive HashSet for stable dedupe/comparison.
    /// </summary>
    private HashSet<string> ParseUserSet(string csv)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(csv))
            return set;

        foreach (var part in csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            string val = part.Trim();
            if (!string.IsNullOrEmpty(val))
                set.Add(val);
        }

        return set;
    }

    /// <summary>
    /// Sends Clone unlock event to Mix It Up command API.
    /// Uses a fixed command ID dedicated to Clone unlock behavior.
    /// </summary>
    private void TriggerMixItUpUnlock(string member)
    {
        if (string.IsNullOrWhiteSpace(MIXITUP_CLONE_UNLOCK_COMMAND_ID) ||
            MIXITUP_CLONE_UNLOCK_COMMAND_ID.StartsWith("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        try
        {
            string url = $"{MIXITUP_API_BASE_URL.TrimEnd('/')}/api/v2/commands/{MIXITUP_CLONE_UNLOCK_COMMAND_ID}";
            string payload = JsonSerializer.Serialize(new
            {
                Platform = "Twitch",
                Arguments = $"squad-unlock|{member}",
                IgnoreRequirements = false
            });

            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = MIXITUP_HTTP_CLIENT.PostAsync(url, content).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                CPH.LogWarn($"[Squad Clone] Mix It Up unlock call failed: {(int)response.StatusCode} {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            CPH.LogError($"[Squad Clone] Exception while calling Mix It Up unlock command: {ex}");
        }
    }
}
