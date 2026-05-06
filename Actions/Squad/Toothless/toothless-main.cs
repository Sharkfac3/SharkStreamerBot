// ACTION-CONTRACT: Actions/Squad/AGENTS.md#Toothless/toothless-main.cs
// ACTION-CONTRACT-SHA256: 132082626b04224a467928d7222a9c0e8b9b4ebf80a9bdd167dbc83f05cc44ef

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS (Toothless feature)
    // Keep these names identical across:
    // - Actions/Squad/Toothless/toothless-main.cs
    // - Actions/Squad/offering.cs
    // - Actions/Twitch Core Integrations/stream-start.cs
    private const string MEMBER_TOOTHLESS = "toothless";
    private const string PREFIX_BOOST = "boost_";
    private const string PREFIX_RARITY = "rarity_";
    private const string VAR_LAST_ROLL = "last_roll";
    private const string VAR_LAST_RARITY = "last_rarity";
    private const string VAR_LAST_USER = "last_user";

    // Shared mini-game lock (cross-feature).
    private const string VAR_MINIGAME_ACTIVE = "minigame_active";
    private const string VAR_MINIGAME_NAME = "minigame_name";
    private const string MINIGAME_NAME_TOOTHLESS = "toothless";

    /*
     * Purpose:
     * - Handles Toothless roll logic, rarity unlocks, OBS reactions, and Mix It Up unlock callouts.
     *
     * Expected trigger/input:
     * - Chat/action trigger for Toothless roll.
     * - Reads: user, userId.
     *
     * Required runtime variables:
     * - boost_toothless_<userId> (optional boost value)
     * - rarity_<rarityName> (unlock flags)
     * - last_roll / last_rarity / last_user (debug/display breadcrumbs)
     * - shared lock: minigame_active/minigame_name
     *
     * Key outputs/side effects:
     * - Rolls rarity and resolves unlock state.
     * - Shows unlock media on current scene + dancing source on Disco scene.
     * - Triggers per-rarity Mix It Up command ID on first-time unlock.
     * - Uses and then releases shared mini-game lock during each roll.
     */

    // Mix It Up unlock bridge for Toothless rarity unlock events.
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_TOOTHLESS_UNLOCK_COMMAND_ID_REGULAR = "d391a388-3891-49ad-ba04-0ccc37f6c329";
    private const string MIXITUP_TOOTHLESS_UNLOCK_COMMAND_ID_SMOL = "dd0224e2-88fc-4eb2-90ae-976d0fffe410";
    private const string MIXITUP_TOOTHLESS_UNLOCK_COMMAND_ID_LONG = "d82e7462-7e78-4dc4-b19d-e989001c9f6e";
    private const string MIXITUP_TOOTHLESS_UNLOCK_COMMAND_ID_FLIGHT = "47027e14-e971-4db0-b129-b2adf79c65d0";
    private const string MIXITUP_TOOTHLESS_UNLOCK_COMMAND_ID_PARTY = "a71a89cc-a255-4f00-a8f2-5f61a33e7da5";
    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    // Shared unlock pacing rule:
    // Mix It Up usually needs a small startup window before the visible unlock payoff begins.
    // All Toothless unlock commands currently play for 16 seconds, so the roll should hold
    // the mini-game lock for a full 19 seconds when a first-time unlock fires.
    private const int WAIT_MIXITUP_UNLOCK_STARTUP_MS = 3000;
    private const int TOOTHLESS_UNLOCK_DURATION_MS = 16000;
    private const int TOOTHLESS_UNLOCK_WAIT_MS = WAIT_MIXITUP_UNLOCK_STARTUP_MS + TOOTHLESS_UNLOCK_DURATION_MS;

    public bool Execute()
    {
        // Required caller identity.
        string user = "";
        string userId = "";
        CPH.TryGetArg("user", out user);
        CPH.TryGetArg("userId", out userId);

        if (string.IsNullOrWhiteSpace(user))
            return true;

        // Prevent overlap with other mini-games.
        if (!TryAcquireMiniGameLock())
        {
            string activeGame = CPH.GetGlobalVar<string>(VAR_MINIGAME_NAME, false) ?? "another mini-game";
            CPH.SendMessage($"🎮 A mini-game is already running ({activeGame}). Finish it before rolling Toothless.");
            return true;
        }

        try
        {
            // Fallback if trigger did not include userId.
            if (string.IsNullOrWhiteSpace(userId))
                userId = user.ToLowerInvariant();

            // Base roll: 1-100 inclusive.
            Random rnd = new Random();
            int roll = rnd.Next(1, 101);

            // Optional favor/boost, tracked per user.
            string boostKey = $"{PREFIX_BOOST}{MEMBER_TOOTHLESS}_{userId}";
            int boost = (CPH.GetGlobalVar<int?>(boostKey, false) ?? 0);
            int boostedRoll = Math.Min(100, roll + boost);

            // Rarity bands are ordered from lowest threshold upward.
            var rarityTable = new List<(string Name, int MaxRoll)>
            {
                ("regular", 40),
                ("smol", 70),
                ("long", 88),
                ("flight", 96),
                ("party", 100)
            };

            // First matching MaxRoll determines rarity.
            string rarity = rarityTable.First(r => boostedRoll <= r.MaxRoll).Name;

            // Unlock state key by rarity name.
            string unlockFlagKey = $"{PREFIX_RARITY}{rarity}";
            bool alreadyUnlocked = (CPH.GetGlobalVar<bool?>(unlockFlagKey, false) ?? false);

            if (!alreadyUnlocked)
            {
                // Persist first-time unlock.
                CPH.SetGlobalVar(unlockFlagKey, true, false);

                // Boost is consumed on successful new unlock.
                CPH.SetGlobalVar(boostKey, 0, false);

                // Inform Mix It Up using per-rarity command mapping.
                bool unlockTriggered = TriggerMixItUpUnlock(rarity);

                if (unlockTriggered)
                    CPH.Wait(TOOTHLESS_UNLOCK_WAIT_MS);

                CPH.SendMessage($"🎉 NEW TOOTHLESS FORM! {rarity.ToUpper()} — {user} rolled {roll}" +
                                (boost > 0 ? $" +{boost} favor = {boostedRoll}" : $" = {boostedRoll}"));
            }
            else
            {
                // Non-unlock roll feedback.
                CPH.SendMessage($"{user} rolled {roll}" +
                                (boost > 0 ? $" +{boost} favor = {boostedRoll}" : $" = {boostedRoll}") +
                                $" — {rarity}");
            }

            // Store latest roll info for diagnostics/overlays.
            CPH.SetGlobalVar(VAR_LAST_ROLL, boostedRoll, false);
            CPH.SetGlobalVar(VAR_LAST_RARITY, rarity, false);
            CPH.SetGlobalVar(VAR_LAST_USER, user, false);

            return true;
        }
        finally
        {
            ReleaseMiniGameLockIfOwned();
        }
    }

    /// <summary>
    /// Claims the shared mini-game lock when free.
    /// Allows re-entry only for Toothless itself.
    /// </summary>
    private bool TryAcquireMiniGameLock()
    {
        bool lockActive = (CPH.GetGlobalVar<bool?>(VAR_MINIGAME_ACTIVE, false) ?? false);
        string lockName = CPH.GetGlobalVar<string>(VAR_MINIGAME_NAME, false) ?? "";

        if (lockActive && !string.Equals(lockName, MINIGAME_NAME_TOOTHLESS, StringComparison.OrdinalIgnoreCase))
            return false;

        CPH.SetGlobalVar(VAR_MINIGAME_ACTIVE, true, false);
        CPH.SetGlobalVar(VAR_MINIGAME_NAME, MINIGAME_NAME_TOOTHLESS, false);
        return true;
    }

    /// <summary>
    /// Releases the shared lock only if Toothless currently owns it.
    /// </summary>
    private void ReleaseMiniGameLockIfOwned()
    {
        string lockName = CPH.GetGlobalVar<string>(VAR_MINIGAME_NAME, false) ?? "";
        if (!string.Equals(lockName, MINIGAME_NAME_TOOTHLESS, StringComparison.OrdinalIgnoreCase))
            return;

        CPH.SetGlobalVar(VAR_MINIGAME_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_MINIGAME_NAME, "", false);
    }

    /// <summary>
    /// Triggers the per-rarity Mix It Up command when a new rarity unlock occurs.
    /// </summary>
    private bool TriggerMixItUpUnlock(string rarity)
    {
        string commandId = GetMixItUpCommandIdForRarity(rarity);
        return TriggerMixItUpCommand(commandId, "Squad Toothless");
    }

    /// <summary>
    /// Generic Mix It Up command trigger helper.
    /// </summary>
    private bool TriggerMixItUpCommand(string commandId, string logPrefix, string arguments = "")
    {
        if (string.IsNullOrWhiteSpace(commandId) ||
            commandId.StartsWith("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase))
        {
            CPH.LogWarn($"[{logPrefix}] Mix It Up command ID is not configured.");
            return false;
        }

        try
        {
            string url = $"{MIXITUP_API_BASE_URL.TrimEnd('/')}/api/v2/commands/{commandId}";
            string payload = JsonSerializer.Serialize(new
            {
                Platform = "Twitch",
                Arguments = arguments ?? "",
                IgnoreRequirements = false
            });

            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = MIXITUP_HTTP_CLIENT.PostAsync(url, content).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                CPH.LogWarn($"[{logPrefix}] Mix It Up call failed: {(int)response.StatusCode} {response.ReasonPhrase}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            CPH.LogError($"[{logPrefix}] Exception while calling Mix It Up: {ex}");
            return false;
        }
    }

    /// <summary>
    /// Maps rarity name to dedicated Mix It Up command ID.
    /// </summary>
    private string GetMixItUpCommandIdForRarity(string rarity)
    {
        switch ((rarity ?? string.Empty).Trim().ToLowerInvariant())
        {
            case "regular": return MIXITUP_TOOTHLESS_UNLOCK_COMMAND_ID_REGULAR;
            case "smol": return MIXITUP_TOOTHLESS_UNLOCK_COMMAND_ID_SMOL;
            case "long": return MIXITUP_TOOTHLESS_UNLOCK_COMMAND_ID_LONG;
            case "flight": return MIXITUP_TOOTHLESS_UNLOCK_COMMAND_ID_FLIGHT;
            case "party": return MIXITUP_TOOTHLESS_UNLOCK_COMMAND_ID_PARTY;
            default: return string.Empty;
        }
    }
}
