using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

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
    private const string VAR_DUCK_UNLOCKED = "duck_unlocked";
    private const string TIMER_DUCK_CALL_WINDOW = "Duck - Call Window";
    private const string OBS_SCENE_DISCO_WORKSPACE = "Disco Party: Workspace";
    private const string OBS_SOURCE_DUCK_DANCING = "Duck - Dancing";

    /*
     * Purpose:
     * - Resolves Duck event when timer window ends.
     *
     * Expected trigger/input:
     * - Timer action: "Duck - Call Window"
     *
     * Required runtime variables:
     * - duck_event_active
     * - duck_quack_count
     * - duck_unlocked
     *
     * Key outputs/side effects:
     * - Ends active event window.
     * - Compares quack power vs random roll.
     * - On first unlock: sets duck_unlocked, shows OBS source, triggers Mix It Up command.
     */

    // Mix It Up unlock bridge for Duck unlock events.
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_DUCK_UNLOCK_COMMAND_ID = "d311b1c1-943a-44cb-9749-b189d1dbd08b";
    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    public bool Execute()
    {
        // If timer fired after event ended, just ensure timer is off and exit.
        bool active = (CPH.GetGlobalVar<bool?>(VAR_DUCK_EVENT_ACTIVE, false) ?? false);
        if (!active)
        {
            CPH.DisableTimer(TIMER_DUCK_CALL_WINDOW);
            return true;
        }

        // End event window now (no more quacks should count).
        CPH.SetGlobalVar(VAR_DUCK_EVENT_ACTIVE, false, false);
        CPH.DisableTimer(TIMER_DUCK_CALL_WINDOW);

        int quacks = (CPH.GetGlobalVar<int?>(VAR_DUCK_QUACK_COUNT, false) ?? 0);

        // Success target is dynamic each round.
        Random rnd = new Random();
        int roll = rnd.Next(1, 101);

        if (quacks > roll)
        {
            bool alreadyUnlocked = (CPH.GetGlobalVar<bool?>(VAR_DUCK_UNLOCKED, false) ?? false);

            if (!alreadyUnlocked)
            {
                // First-time unlock side effects.
                CPH.SetGlobalVar(VAR_DUCK_UNLOCKED, true, false);
                CPH.ObsShowSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_DUCK_DANCING);
                TriggerMixItUpUnlock("duck");

                CPH.SendMessage($"🦆✅ DUCK UNLOCKED! Quacks: {quacks} vs Roll: {roll} — Duck joins the disco!");
            }
            else
            {
                // Event succeeded, but unlock already owned.
                CPH.SendMessage($"🦆 Duck was already dancing. Quacks: {quacks} vs Roll: {roll}");
            }
        }
        else
        {
            CPH.SendMessage($"🦆❌ Not enough quack power. Quacks: {quacks} vs Roll: {roll}");
        }

        return true;
    }

    /// <summary>
    /// Sends Duck unlock event to Mix It Up API.
    /// </summary>
    private void TriggerMixItUpUnlock(string member)
    {
        if (string.IsNullOrWhiteSpace(MIXITUP_DUCK_UNLOCK_COMMAND_ID) ||
            MIXITUP_DUCK_UNLOCK_COMMAND_ID.StartsWith("REPLACE_WITH_", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        try
        {
            string url = $"{MIXITUP_API_BASE_URL.TrimEnd('/')}/api/v2/commands/{MIXITUP_DUCK_UNLOCK_COMMAND_ID}";
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
                CPH.LogWarn($"[Squad Duck] Mix It Up unlock call failed: {(int)response.StatusCode} {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            CPH.LogError($"[Squad Duck] Exception while calling Mix It Up unlock command: {ex}");
        }
    }
}
