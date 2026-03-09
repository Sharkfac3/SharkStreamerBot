using System;
using System.Collections.Generic;

public class CPHInline
{
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

        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(rawInput))
            return true;

        if (string.IsNullOrWhiteSpace(userId))
            userId = user.ToLowerInvariant();

        // Extract token after !offering
        string[] parts = rawInput.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string token = parts.Length > 0 ? parts[0] : "";

        if (string.IsNullOrWhiteSpace(token))
            return true;

        // =========================
        // LotAT Mode Check
        // =========================
        bool lotatActive = (CPH.GetGlobalVar<bool?>("lotat_active", false) ?? false);

        // Send announcement once per session
        if (lotatActive)
        {
            bool announced = (CPH.GetGlobalVar<bool?>("lotat_announcement_sent", false) ?? false);

            if (!announced)
            {
                CPH.SetGlobalVar("lotat_announcement_sent", true, false);
                CPH.SendMessage("📜 Legends of the ASCII Temple (LotAT) is ACTIVE — offerings may have… consequences.");
            }
        }

        // =========================
        // Build Offering Map
        // =========================
        BuildOfferingMap();

        if (!offeringMap.TryGetValue(token, out var offer))
        {
            CPH.SendMessage($"❓ {user} makes an offering... but nothing seems to notice.");
            return true;
        }

        // =========================
        // LotAT Steal Configuration
        // =========================
        int stealChance = (CPH.GetGlobalVar<int?>("lotat_offering_steal_chance", false) ?? 15);
        int stealMultiplier = (CPH.GetGlobalVar<int?>("lotat_steal_multiplier", false) ?? 1);

        stealChance = Clamp(stealChance, 0, 100);
        stealMultiplier = Math.Max(1, stealMultiplier);

        bool stolen = false;

        if (lotatActive && stealChance > 0)
        {
            int roll = new Random().Next(1, 101);
            stolen = roll <= stealChance;
        }

        // =========================
        // Evil Table (weighted)
        // =========================
        var evilTable = BuildEvilTable();

        // =========================
        // Apply Boost
        // =========================
        const int boostCap = 30;

        string boostKey = $"boost_{offer.MemberId}_{userId}";
        int currentBoost = (CPH.GetGlobalVar<int?> (boostKey, false) ?? 0);

        int delta = offer.BoostAdd;
        string flavor;

        if (stolen)
        {
            delta = -Math.Abs(delta) * stealMultiplier;

            var thief = PickWeighted(evilTable);
            flavor = thief.Flavor;
        }
        else
        {
            flavor = offer.Flavor;
        }

        int newBoost = currentBoost + delta;
        newBoost = Math.Min(newBoost, boostCap);
        newBoost = Math.Max(newBoost, 0);

        CPH.SetGlobalVar(boostKey, newBoost, false);

        string sign = delta >= 0 ? "+" : "";
        string favorWord = lotatActive ? "fate" : "favor";

        CPH.SendMessage($"{flavor} {user} gains {sign}{delta} {favorWord}. (Now {newBoost})");

        return true;
    }

    // =====================================================
    // Offering Map (add squad members here)
    // =====================================================

    private void BuildOfferingMap()
    {
        offeringMap = new Dictionary<string, (string MemberId, int BoostAdd, string Flavor)>(StringComparer.OrdinalIgnoreCase);

        // Toothless offerings
        AddOfferings("toothless",
            ("sharkf4Rmp", 5, "🦐 A fancy shrimp offering slips into the dark..."),
            ("shrimp", 1, "🍤 A small shrimp is offered... something stirs."),
            ("fish", 1, "🐟 A fish is placed gently on the altar...")
        );

        // Duck offerings
        AddOfferings("duck",
            ("decoy", 1, "🦆 A decoy is placed upon the water... ripples spread.")
        );

        // Future squad members go here
        // Example:
        // AddOfferings("frog",
        //     ("fly", 2, "🪰 Something buzzing disappears into the reeds...")
        // );
    }

    private void AddOfferings(string memberId, params (string Token, int BoostAdd, string Flavor)[] offerings)
    {
        foreach (var o in offerings)
            offeringMap[o.Token] = (memberId, o.BoostAdd, o.Flavor);
    }

    // =====================================================
    // Evil Table
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

        return table[0];
    }

    private int Clamp(int v, int min, int max)
    {
        if (v < min) return min;
        if (v > max) return max;
        return v;
    }
}