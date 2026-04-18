using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // Keep these constants synchronized with Actions/SHARED-CONSTANTS.md.

    // Stream mode
    private const string VAR_STREAM_MODE    = "stream_mode";
    private const string MODE_GARAGE        = "garage";
    private const string MODE_WORKSPACE     = "workspace";
    private const string MODE_GAMER         = "gamer";

    // OBS scene names for each stream mode's Disco Party scene.
    private const string OBS_SCENE_DISCO_GARAGE    = "Disco Party: Garage";
    private const string OBS_SCENE_DISCO_WORKSPACE = "Disco Party: Workspace";
    private const string OBS_SCENE_DISCO_GAMER     = "Disco Party: Gamer";

    // Re-entry guard: prevents the redeem from stacking on top of itself.
    private const string VAR_DISCO_PARTY_ACTIVE     = "disco_party_active";
    // Stores the scene we were on before switching to Disco so we can return afterward.
    private const string VAR_DISCO_PARTY_PREV_SCENE = "disco_party_prev_scene";

    // How long the disco party lasts before returning to the previous scene.
    private const int DISCO_PARTY_DURATION_MS = 60000;

    // Squad member unlock flags — checked before firing each dance command.
    private const string VAR_DUCK_UNLOCKED  = "duck_unlocked";
    private const string VAR_CLONE_UNLOCKED = "clone_unlocked";
    private const string VAR_PEDRO_UNLOCKED = "pedro_unlocked";

    // Toothless unlock flags use the "rarity_<name>" prefix.
    // Each rarity can be unlocked independently and has its own dance command.
    private const string PREFIX_RARITY = "rarity_";
    private static readonly string[] TOOTHLESS_RARITIES = { "regular", "smol", "long", "flight", "party" };

    // Mix It Up API base URL (Mix It Up must be running for these calls to work).
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    // Mix It Up dance command IDs.
    // Replace the REPLACE_WITH_* placeholders after creating "Squad - <Member> - Dance"
    // commands in Mix It Up, then run Tools/MixItUp/Api/get_commands.py to get their IDs.
    private const string MIXITUP_DUCK_DANCE_ID                 = "2aa776f8-a7f5-452d-bfe7-ef15a7f4df51";
    private const string MIXITUP_CLONE_DANCE_ID                = "decd533d-1263-4f38-ab99-2eacf7452c41";
    private const string MIXITUP_PEDRO_DANCE_ID                = "5d63ec44-90f8-47ea-bd4d-8aadb89df845";
    private const string MIXITUP_TOOTHLESS_DANCE_REGULAR_ID    = "2650f080-20af-4b9e-92a0-725259bd86a3";
    private const string MIXITUP_TOOTHLESS_DANCE_SMOL_ID       = "282cb38c-444f-4d78-b26f-acae7ee660aa";
    private const string MIXITUP_TOOTHLESS_DANCE_LONG_ID       = "a48fd2ef-6e9b-4481-839e-fdd94edd602d";
    private const string MIXITUP_TOOTHLESS_DANCE_FLIGHT_ID     = "56493860-2f38-4cbf-9482-80422e42f025";
    private const string MIXITUP_TOOTHLESS_DANCE_PARTY_ID      = "b3154c28-2c23-4f31-b9e7-58da52eb7913";

    /*
     * Purpose:
     * - Handles the "disco party" channel point redeem.
     * - Saves the current OBS scene, then switches to the Disco Party scene matching
     *   the active stream mode (Garage / Workspace / Gamer).
     * - Fires Mix It Up dance commands for every squad member that is currently unlocked.
     *   Duck, Clone, and Pedro each have a single unlock flag.
     *   Toothless has one flag per rarity — each unlocked rarity triggers its own dance command.
     * - After 60 seconds, returns to the original scene (unless the operator navigated away).
     *
     * Expected trigger/input:
     * - Streamer.bot action wired to the "disco party" channel point redeem.
     * - No chat args required.
     *
     * Required runtime variables (all global, non-persisted):
     * - stream_mode        — set by mode-garage / mode-workspace / mode-gamer scripts
     * - duck_unlocked      — set by duck unlock flow
     * - clone_unlocked     — set by clone unlock flow
     * - pedro_unlocked     — set by pedro unlock flow
     * - rarity_regular, rarity_smol, rarity_long, rarity_flight, rarity_party
     *                      — set by toothless-main.cs on first-time rarity unlock
     * - disco_party_active — set and cleared by this script (re-entry guard)
     * - disco_party_prev_scene — set and cleared by this script (scene memory)
     *
     * Key outputs/side effects:
     * - Switches OBS to the appropriate Disco Party scene.
     * - Calls Mix It Up for every dance command whose squad member is currently unlocked.
     * - Waits 60 seconds on the Disco Party scene.
     * - Switches OBS back to the previous scene (only if still on a Disco Party scene;
     *   if the operator manually navigated away, the auto-return is skipped).
     * - Sends a chat message when blocked by an already-running disco party.
     *
     * Operator notes:
     * - stream-start.cs resets disco_party_active and disco_party_prev_scene at stream start.
     * - Mix It Up dance commands fire simultaneously (no waits between them). Their
     *   internal duration / looping is handled inside Mix It Up, not here.
     */
    public bool Execute()
    {
        // Block if a disco party is already in progress (e.g. redeemed twice quickly).
        bool alreadyActive = (CPH.GetGlobalVar<bool?>(VAR_DISCO_PARTY_ACTIVE, false) ?? false);
        if (alreadyActive)
        {
            CPH.SendMessage("🕺 Disco party is already going! Wait for the current one to end.");
            return true;
        }

        // Claim the disco lock so concurrent redeems bail out above.
        CPH.SetGlobalVar(VAR_DISCO_PARTY_ACTIVE, true, false);

        try
        {
            // Save the active OBS scene so we can return to it after the party.
            string prevScene = CPH.ObsGetCurrentScene() ?? string.Empty;
            CPH.SetGlobalVar(VAR_DISCO_PARTY_PREV_SCENE, prevScene, false);

            // Pick the right Disco Party scene for the current stream mode.
            string mode = (CPH.GetGlobalVar<string>(VAR_STREAM_MODE, false) ?? string.Empty)
                .Trim()
                .ToLowerInvariant();
            string discoScene = ResolveDiscoScene(mode);

            CPH.ObsSetScene(discoScene);
            CPH.LogWarn($"[Twitch Redeem: Disco Party] Scene -> '{discoScene}'. Saved previous: '{prevScene}'.");

            // Fire dance commands for every squad member that is currently unlocked.
            // Commands are sent back-to-back (no waits); duration is controlled inside Mix It Up.
            FireDanceCommands();

            // Hold on the disco scene for the full party duration.
            CPH.Wait(DISCO_PARTY_DURATION_MS);

            // Only switch back if OBS is still on a Disco Party scene.
            // If the operator navigated away manually during the dance, respect that.
            string sceneAfterWait = CPH.ObsGetCurrentScene() ?? string.Empty;
            bool stillOnDiscoScene = sceneAfterWait.StartsWith("Disco Party", StringComparison.OrdinalIgnoreCase);

            if (stillOnDiscoScene && !string.IsNullOrWhiteSpace(prevScene))
            {
                CPH.ObsSetScene(prevScene);
                CPH.LogWarn($"[Twitch Redeem: Disco Party] Returned to '{prevScene}'.");
            }
            else if (!stillOnDiscoScene)
            {
                CPH.LogWarn($"[Twitch Redeem: Disco Party] Operator navigated away during party (current: '{sceneAfterWait}'). Skipping auto-return.");
            }
        }
        finally
        {
            // Always release the lock — even if an exception fires above.
            CPH.SetGlobalVar(VAR_DISCO_PARTY_ACTIVE, false, false);
            CPH.SetGlobalVar(VAR_DISCO_PARTY_PREV_SCENE, "", false);
        }

        return true;
    }

    // ─────────────────────────────────────────────────────────────────
    // Dance command dispatch
    // ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Checks unlock state for Duck, Clone, Pedro, and each Toothless rarity.
    /// Fires the corresponding Mix It Up dance command for every unlocked member/rarity.
    /// </summary>
    private void FireDanceCommands()
    {
        // Duck — single unlock flag.
        bool duckUnlocked = (CPH.GetGlobalVar<bool?>(VAR_DUCK_UNLOCKED, false) ?? false);
        if (duckUnlocked)
            TriggerMixItUpCommand(MIXITUP_DUCK_DANCE_ID, "Disco Party / Duck Dance");

        // Clone — single unlock flag.
        bool cloneUnlocked = (CPH.GetGlobalVar<bool?>(VAR_CLONE_UNLOCKED, false) ?? false);
        if (cloneUnlocked)
            TriggerMixItUpCommand(MIXITUP_CLONE_DANCE_ID, "Disco Party / Clone Dance");

        // Pedro — single unlock flag.
        bool pedroUnlocked = (CPH.GetGlobalVar<bool?>(VAR_PEDRO_UNLOCKED, false) ?? false);
        if (pedroUnlocked)
            TriggerMixItUpCommand(MIXITUP_PEDRO_DANCE_ID, "Disco Party / Pedro Dance");

        // Toothless — one flag and one command per rarity.
        // Only rarities that have been unlocked this session will dance.
        foreach (string rarity in TOOTHLESS_RARITIES)
        {
            string flagKey = $"{PREFIX_RARITY}{rarity}";
            bool rarityUnlocked = (CPH.GetGlobalVar<bool?>(flagKey, false) ?? false);
            if (rarityUnlocked)
            {
                string commandId = ResolveToothlessDanceCommandId(rarity);
                TriggerMixItUpCommand(commandId, $"Disco Party / Toothless Dance ({rarity})");
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the Disco Party OBS scene name for the given stream mode.
    /// Falls back to workspace if the mode is missing or unrecognized.
    /// </summary>
    private string ResolveDiscoScene(string mode)
    {
        switch (mode)
        {
            case MODE_GARAGE:    return OBS_SCENE_DISCO_GARAGE;
            case MODE_GAMER:     return OBS_SCENE_DISCO_GAMER;
            case MODE_WORKSPACE: return OBS_SCENE_DISCO_WORKSPACE;
            default:
                CPH.LogWarn($"[Twitch Redeem: Disco Party] Unknown stream_mode '{mode}'. Falling back to workspace scene.");
                return OBS_SCENE_DISCO_WORKSPACE;
        }
    }

    /// <summary>
    /// Maps a Toothless rarity name to its Mix It Up dance command ID.
    /// </summary>
    private string ResolveToothlessDanceCommandId(string rarity)
    {
        switch (rarity)
        {
            case "regular": return MIXITUP_TOOTHLESS_DANCE_REGULAR_ID;
            case "smol":    return MIXITUP_TOOTHLESS_DANCE_SMOL_ID;
            case "long":    return MIXITUP_TOOTHLESS_DANCE_LONG_ID;
            case "flight":  return MIXITUP_TOOTHLESS_DANCE_FLIGHT_ID;
            case "party":   return MIXITUP_TOOTHLESS_DANCE_PARTY_ID;
            default:
                CPH.LogWarn($"[Twitch Redeem: Disco Party] Unknown Toothless rarity '{rarity}' — no dance command mapped.");
                return "";
        }
    }

    /// <summary>
    /// POSTs to the Mix It Up API to trigger the command with the given ID.
    /// Silently skips if the ID is still a REPLACE_WITH_* placeholder.
    /// </summary>
    private bool TriggerMixItUpCommand(string commandId, string logPrefix)
    {
        if (string.IsNullOrWhiteSpace(commandId) ||
            commandId.StartsWith("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase))
        {
            CPH.LogWarn($"[{logPrefix}] Mix It Up command ID not configured yet — skipping.");
            return false;
        }

        try
        {
            string url = $"{MIXITUP_API_BASE_URL.TrimEnd('/')}/api/v2/commands/{commandId}";
            string payload = JsonSerializer.Serialize(new
            {
                Platform             = "Twitch",
                Arguments            = "",
                SpecialIdentifiers   = new { },
                IgnoreRequirements   = false
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
            CPH.LogError($"[{logPrefix}] Exception calling Mix It Up: {ex}");
            return false;
        }
    }
}
