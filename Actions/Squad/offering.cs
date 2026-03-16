using System;
using System.Collections.Generic;

public class CPHInline
{
    // SYNC CONSTANTS (Offering/LotAT + shared boost keys)
    // Keep these names identical across:
    // - Actions/Squad/offering.cs
    // - Actions/Squad/Toothless/toothless-main.cs
    // - Actions/Twitch Core Integrations/stream-start.cs
    private const string VAR_LOTAT_ACTIVE = "lotat_active";
    private const string VAR_LOTAT_ANNOUNCEMENT_SENT = "lotat_announcement_sent";
    private const string VAR_LOTAT_OFFERING_STEAL_CHANCE = "lotat_offering_steal_chance";
    private const string VAR_LOTAT_STEAL_MULTIPLIER = "lotat_steal_multiplier";
    private const string PREFIX_BOOST = "boost_";
    private const string MEMBER_TOOTHLESS = "toothless";
    private const string MEMBER_DUCK = "duck";

    /*
     * Purpose:
     * - Handles !offering token input and applies per-user boost changes.
     * - Supports optional LotAT steal behavior that can flip boosts negative.
     *
     * Expected trigger/input:
     * - Chat command/action that passes user, userId, and rawInput.
     * - rawInput should contain the offering token currently configured in offeringMap.
     *
     * Required runtime variables:
     * - lotat_active (bool)
     * - lotat_announcement_sent (bool)
     * - lotat_offering_steal_chance (int 0..100)
     * - lotat_steal_multiplier (int >= 1)
     * - boost_<member>_<userId> (int)
     *
     * Key outputs/side effects:
     * - Applies positive or negative delta to per-user boost key.
     * - Clamps final boost to range 0..30.
     * - Sends one-time LotAT active announcement when LotAT first used in a session.
     * - Sends chat feedback describing result.
     *
     * Operator notes:
     * - Add new offering tokens in BuildOfferingMap().
     * - This script does not expose secrets and does not require persisted vars.
     */

    // token -> (memberId, boostValue, flavor text)
    private Dictionary<string, (string MemberId, int BoostAdd, string Flavor)> offeringMap;

    public bool Execute()
    {
        string user = "";
        string userId = "";
        string rawInput = "";

        CPH.TryGetArg("user", out user);
        CPH.TryGetArg("userId", out userId);
        CPH.TryGetArg("rawInput", out rawInput);

        // If key inputs are missing, safely no-op.
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(rawInput))
            return true;

        // Fallback userId for triggers that do not provide one.
        if (string.IsNullOrWhiteSpace(userId))
            userId = user.ToLowerInvariant();

        // Extract token from command payload.
        // Current workflow expects token in first split segment.
        string[] parts = rawInput.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string token = parts.Length > 0 ? parts[0] : "";

        if (string.IsNullOrWhiteSpace(token))
            return true;

        // -------------------------------------------------
        // LotAT mode / one-time announcement
        // -------------------------------------------------
        bool lotatActive = (CPH.GetGlobalVar<bool?>(VAR_LOTAT_ACTIVE, false) ?? false);

        if (lotatActive)
        {
            bool announced = (CPH.GetGlobalVar<bool?>(VAR_LOTAT_ANNOUNCEMENT_SENT, false) ?? false);
            if (!announced)
            {
                CPH.SetGlobalVar(VAR_LOTAT_ANNOUNCEMENT_SENT, true, false);
                CPH.SendMessage("📜 Legends of the ASCII Temple (LotAT) is ACTIVE — offerings may have… consequences.");
            }
        }

        // Build lookup table for recognized offerings.
        BuildOfferingMap();

        // Unknown token: flavor miss message, no state changes.
        if (!offeringMap.TryGetValue(token, out var offer))
        {
            CPH.SendMessage($"❓ {user} makes an offering... but nothing seems to notice.");
            return true;
        }

        // -------------------------------------------------
        // LotAT steal settings (with defensive clamps)
        // -------------------------------------------------
        int stealChance = (CPH.GetGlobalVar<int?>(VAR_LOTAT_OFFERING_STEAL_CHANCE, false) ?? 15);
        int stealMultiplier = (CPH.GetGlobalVar<int?>(VAR_LOTAT_STEAL_MULTIPLIER, false) ?? 1);

        stealChance = Clamp(stealChance, 0, 100);
        stealMultiplier = Math.Max(1, stealMultiplier);

        bool stolen = false;
        if (lotatActive && stealChance > 0)
        {
            int roll = new Random().Next(1, 101);
            stolen = roll <= stealChance;
        }

        // Weighted table for LotAT steal flavor output.
        var evilTable = BuildEvilTable();

        // -------------------------------------------------
        // Apply boost delta
        // -------------------------------------------------
        const int BOOST_CAP = 30;

        string boostKey = $"{PREFIX_BOOST}{offer.MemberId}_{userId}";
        int currentBoost = (CPH.GetGlobalVar<int?>(boostKey, false) ?? 0);

        int delta = offer.BoostAdd;
        string flavor;

        if (stolen)
        {
            // On steal, invert to negative and apply multiplier.
            delta = -Math.Abs(delta) * stealMultiplier;
            var thief = PickWeighted(evilTable);
            flavor = thief.Flavor;
        }
        else
        {
            flavor = offer.Flavor;
        }

        int newBoost = currentBoost + delta;
        newBoost = Math.Min(newBoost, BOOST_CAP);
        newBoost = Math.Max(newBoost, 0);

        CPH.SetGlobalVar(boostKey, newBoost, false);

        string sign = delta >= 0 ? "+" : "";
        string favorWord = lotatActive ? "fate" : "favor";

        CPH.SendMessage($"{flavor} {user} gains {sign}{delta} {favorWord}. (Now {newBoost})");

        return true;
    }

    // =====================================================
    // Offering map configuration (easy place to add new tokens)
    // =====================================================
    private void BuildOfferingMap()
    {
        offeringMap = new Dictionary<string, (string MemberId, int BoostAdd, string Flavor)>(StringComparer.OrdinalIgnoreCase);

        // Toothless offerings
        AddOfferings(MEMBER_TOOTHLESS,
            ("sharkf4Rmp", 5, "🦐 A fancy shrimp offering slips into the dark..."),
            ("shrimp", 1, "🍤 A small shrimp is offered... something stirs."),
            ("fish", 1, "🐟 A fish is placed gently on the altar...")
        );

        // Duck offerings
        AddOfferings(MEMBER_DUCK,
            ("decoy", 1, "🦆 A decoy is placed upon the water... ripples spread.")
        );

        // Future squad members can be added here.
    }

    private void AddOfferings(string memberId, params (string Token, int BoostAdd, string Flavor)[] offerings)
    {
        foreach (var o in offerings)
            offeringMap[o.Token] = (memberId, o.BoostAdd, o.Flavor);
    }

    // =====================================================
    // LotAT steal flavor table (weighted)
    // =====================================================
    private List<(string Name, int Weight, string Flavor)> BuildEvilTable()
    {
        return new List<(string Name, int Weight, string Flavor)>
        {
            ("mimic", 50, "🫦 The offering is taken… and something smacks its lips."),
            ("wraith", 35, "🕯️ The air goes cold. The offering vanishes before it lands."),
            ("lich", 15, "💀 A dry chuckle echoes. Your offering is claimed.")
        };
    }

    private (string Name, int Weight, string Flavor) PickWeighted(List<(string Name, int Weight, string Flavor)> table)
    {
        int total = 0;
        foreach (var e in table)
            total += Math.Max(0, e.Weight);

        if (total <= 0)
            return ("unknown", 1, "🕯️ The offering vanishes before it lands... something else has noticed.");

        int roll = new Random().Next(1, total + 1);
        int running = 0;

        foreach (var e in table)
        {
            running += Math.Max(0, e.Weight);
            if (roll <= running)
                return e;
        }

        // Defensive fallback (should not be reached when total > 0).
        return table[0];
    }

    private int Clamp(int v, int min, int max)
    {
        if (v < min) return min;
        if (v > max) return max;
        return v;
    }
}
