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
    // - Actions/Twitch Integration/stream-start.cs
    private const string OBS_SCENE_DISCO_WORKSPACE = "Disco Party: Workspace";
    private const string MEMBER_TOOTHLESS = "toothless";
    private const string PREFIX_BOOST = "boost_";
    private const string PREFIX_RARITY = "rarity_";
    private const string VAR_LAST_ROLL = "last_roll";
    private const string VAR_LAST_RARITY = "last_rarity";
    private const string VAR_LAST_USER = "last_user";

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
     *
     * Key outputs/side effects:
     * - Rolls rarity and resolves unlock state.
     * - Shows unlock media on current scene + dancing source on Disco scene.
     * - Triggers per-rarity Mix It Up command ID on first-time unlock.
     */

    // Mix It Up unlock bridge for Toothless rarity unlock events.
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_TOOTHLESS_UNLOCK_COMMAND_ID_REGULAR = "623c2524-0509-45fb-a759-362edf2543b3";
    private const string MIXITUP_TOOTHLESS_UNLOCK_COMMAND_ID_SMOL = "46ab64e0-6682-442a-83fa-d7e265274919";
    private const string MIXITUP_TOOTHLESS_UNLOCK_COMMAND_ID_LONG = "c00976c4-ddde-4f56-833a-7551fc106788";
    private const string MIXITUP_TOOTHLESS_UNLOCK_COMMAND_ID_FLIGHT = "2e0b400a-291c-4d8e-b97e-ced7c7b036e3";
    private const string MIXITUP_TOOTHLESS_UNLOCK_COMMAND_ID_PARTY = "e57d1f2d-716d-41b2-bfbe-d9a8e7974ecb";
    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    public bool Execute()
    {
        // Required caller identity.
        string user = "";
        string userId = "";
        CPH.TryGetArg("user", out user);
        CPH.TryGetArg("userId", out userId);

        if (string.IsNullOrWhiteSpace(user))
            return true;

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

            // Make the unlocked variant visible in Disco workspace scene.
            string dancingSourceName = $"Toothless - Dancing - {rarity}";
            CPH.ObsShowSource(OBS_SCENE_DISCO_WORKSPACE, dancingSourceName);

            // Inform Mix It Up using per-rarity command mapping.
            TriggerMixItUpUnlock(rarity);

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

    /// <summary>
    /// Triggers the per-rarity Mix It Up command when a new rarity unlock occurs.
    /// </summary>
    private void TriggerMixItUpUnlock(string rarity)
    {
        string commandId = GetMixItUpCommandIdForRarity(rarity);
        if (string.IsNullOrWhiteSpace(commandId) ||
            commandId.StartsWith("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        try
        {
            string url = $"{MIXITUP_API_BASE_URL.TrimEnd('/')}/api/v2/commands/{commandId}";
            string payload = JsonSerializer.Serialize(new
            {
                Platform = "Twitch",
                Arguments = "",
                IgnoreRequirements = false
            });

            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = MIXITUP_HTTP_CLIENT.PostAsync(url, content).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                CPH.LogWarn($"[Squad Toothless] Mix It Up unlock call failed ({rarity}): {(int)response.StatusCode} {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            CPH.LogError($"[Squad Toothless] Exception while calling Mix It Up unlock command ({rarity}): {ex}");
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
