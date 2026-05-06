// ACTION-CONTRACT: Actions/Commanders/The Director/AGENTS.md#the-director-primary.cs
// ACTION-CONTRACT-SHA256: 7f4dadced40e618af4c57477b61c5f78e8da3a37301178dbeb15d6090969bf2a

// Documented Mix It Up endpoint literal: http://localhost:8911/api/v2/commands/{commandId}
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    /*
     * Purpose:
     * - Lets the active The Director switch the current OBS scene to its primary source layout.
     *
     * Expected trigger/input:
     * - Streamer.bot action trigger for !primary command.
     * - Reads trigger arg: user
     *
     * Required runtime variables:
     * - Reads global var current_the_director
     *
     * Key outputs/side effects:
     * - Shows the primary source and hides the secondary source in the current OBS scene.
     * - If Mix It Up command ID is configured, triggers the primary switch command.
     * - No chat output.
     *
     * Operator notes:
     * - ObsGetCurrentScene() is flagged VERIFY — test before relying on in production.
     * - Add scene entries to SCENE_SOURCE_MAP as sources are confirmed.
     * - Replace MIXITUP_COMMAND_ID_PRIMARY placeholder when ready.
     */

    private const string LOG_PREFIX = "The Director: Primary";

    private const string VAR_CURRENT_THE_DIRECTOR = "current_the_director";

    // Replace with the Mix It Up command ID for the primary switch effect when ready.
    private const string MIXITUP_API_BASE_URL       = "http://localhost:8911";
    private const string MIXITUP_COMMAND_ID_PRIMARY = "REPLACE_WITH_PRIMARY_COMMAND_ID";
    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    // Scene source map: OBS scene name -> (primary source name, secondary source name).
    // Add an entry here for each scene as sources are confirmed.
    // Source names must exactly match the OBS source names inside each scene.
    private static readonly Dictionary<string, SceneSources> SCENE_SOURCE_MAP =
        new Dictionary<string, SceneSources>(StringComparer.OrdinalIgnoreCase)
    {
        { "Workspace: Main", new SceneSources("Main Screen Capture", "Quest POV") },
        // TODO: { "Workspace: Chat",         new SceneSources("REPLACE", "REPLACE") },
        // TODO: { "Workspace: Housekeeping",  new SceneSources("REPLACE", "REPLACE") },
        // TODO: { "Disco Party: Workspace",   new SceneSources("REPLACE", "REPLACE") },
        // TODO: { "Garage: Main",             new SceneSources("REPLACE", "REPLACE") },
        // TODO: { "Garage: Chat",             new SceneSources("REPLACE", "REPLACE") },
        // TODO: { "Garage: Housekeeping",     new SceneSources("REPLACE", "REPLACE") },
        // TODO: { "Disco Party: Garage",      new SceneSources("REPLACE", "REPLACE") },
        // TODO: { "Gamer: Main",              new SceneSources("REPLACE", "REPLACE") },
        // TODO: { "Gamer: Chat",              new SceneSources("REPLACE", "REPLACE") },
        // TODO: { "Gamer: Housekeeping",      new SceneSources("REPLACE", "REPLACE") },
        // TODO: { "Disco Party: Gamer",       new SceneSources("REPLACE", "REPLACE") },
    };

    public bool Execute()
    {
        // Read the caller's username from the trigger.
        string caller = "";
        CPH.TryGetArg("user", out caller);
        caller = (caller ?? "").Trim();

        // Guard: only the active The Director may use this command.
        string currentDirector = (CPH.GetGlobalVar<string>(VAR_CURRENT_THE_DIRECTOR, false) ?? "").Trim();

        if (string.IsNullOrWhiteSpace(currentDirector))
        {
            CPH.LogWarn($"[{LOG_PREFIX}] No active Director. Ignoring !primary from '{caller}'.");
            return true;
        }

        if (!string.Equals(caller, currentDirector, StringComparison.OrdinalIgnoreCase))
        {
            CPH.LogWarn($"[{LOG_PREFIX}] '{caller}' is not the current Director ('{currentDirector}'). Ignoring.");
            return true;
        }

        // Get the current OBS scene.
        // VERIFY: ObsGetCurrentScene() — unconfirmed method signature.
        // Test this call manually before relying on it in production.
        string currentScene = CPH.ObsGetCurrentScene();

        if (string.IsNullOrWhiteSpace(currentScene))
        {
            CPH.LogWarn($"[{LOG_PREFIX}] Could not determine the current OBS scene. ObsGetCurrentScene() may need verification.");
            return true;
        }

        // Look up primary/secondary sources for this scene.
        SceneSources sources;
        if (!SCENE_SOURCE_MAP.TryGetValue(currentScene, out sources))
        {
            CPH.LogWarn($"[{LOG_PREFIX}] No source mapping for scene '{currentScene}'. Add it to SCENE_SOURCE_MAP in this script.");
            return true;
        }

        // Show primary, hide secondary.
        CPH.ObsShowSource(currentScene, sources.Primary);
        CPH.ObsHideSource(currentScene, sources.Secondary);

        CPH.LogWarn($"[{LOG_PREFIX}] '{currentScene}' -> primary. Show: '{sources.Primary}' | Hide: '{sources.Secondary}'.");

        TriggerMixItUpCommand(MIXITUP_COMMAND_ID_PRIMARY);

        return true;
    }

    private bool TriggerMixItUpCommand(string commandId)
    {
        if (string.IsNullOrWhiteSpace(commandId) ||
            commandId.StartsWith("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        try
        {
            string url     = $"{MIXITUP_API_BASE_URL.TrimEnd('/')}/api/v2/commands/{commandId}";
            string payload = JsonSerializer.Serialize(new
            {
                Platform           = "Twitch",
                Arguments          = "primary",
                SpecialIdentifiers = new { state = "primary" },
                IgnoreRequirements = false
            });

            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = MIXITUP_HTTP_CLIENT.PostAsync(url, content).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                CPH.LogWarn($"[{LOG_PREFIX}] Mix It Up call failed: {(int)response.StatusCode} {response.ReasonPhrase}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            CPH.LogError($"[{LOG_PREFIX}] Exception calling Mix It Up: {ex}");
            return false;
        }
    }

    private class SceneSources
    {
        public string Primary   { get; }
        public string Secondary { get; }

        public SceneSources(string primary, string secondary)
        {
            Primary   = primary;
            Secondary = secondary;
        }
    }
}
