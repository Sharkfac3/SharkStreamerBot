// ACTION-CONTRACT: Actions/Intros/AGENTS.md#first-chat-intro.cs
// ACTION-CONTRACT-SHA256: c24eddd42d08191fdbe084f2f92e987174da4b41ac8ac0cc85d8de415ad255bb

using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS — Info Service / Assets
    // Source of truth: Actions/SHARED-CONSTANTS.md §Info Service / Assets
    private const string INFO_SERVICE_URL    = "http://127.0.0.1:8766";
    private const string ASSETS_ROOT         = @"C:\Users\sharkfac3\Workspace\coding\SharkStreamerBot\Assets";
    private const string SOUND_SUBPATH       = @"user-intros\sound\";
    private const string COLLECTION_NAME     = "user-intros";

    // MixItUp integration pattern:
    // This script calls CPH.RunAction("Intros - Play Custom Intro", true) and sets
    // VAR_SOUND_FILE_PATH as a global var before calling.
    // The sub-action chain reads intro_sound_file_path and passes it to the MixItUp
    // "Custom Intro" command as the file path argument.
    // Direct MixItUp API integration is not used here because commandId lookup is
    // out of scope for this chunk; the indirection via RunAction keeps this script
    // decoupled from the MixItUp command registry.
    private const string MIXITUP_ACTION_NAME = "Intros - Play Custom Intro";
    private const string VAR_SOUND_FILE_PATH = "intro_sound_file_path";

    /*
     * Purpose:
     * - Fires on Streamer.bot native "First Chat" event (once per viewer per stream session).
     * - Looks up the viewer in the info-service user-intros collection.
     * - If a custom intro is configured and enabled, dispatches MixItUp "Custom Intro" command.
     *
     * Expected trigger/input:
     * - Streamer.bot "First Chat" event.
     * - Args: userId (Twitch user ID of the viewer).
     *
     * Required runtime:
     * - info-service running at http://127.0.0.1:8766
     *
     * Key outputs/side effects:
     * - Sets global var intro_sound_file_path (non-persisted) before calling RunAction.
     * - Calls CPH.RunAction("Intros - Play Custom Intro", true) when intro is active.
     * - All branches log to CPH.LogInfo for operator tracing in SB action log.
     */
    public bool Execute()
    {
        string userId = "";
        CPH.TryGetArg("userId", out userId);

        if (string.IsNullOrWhiteSpace(userId))
        {
            CPH.LogInfo("[first-chat-intro] No userId in trigger args — skipping.");
            return true;
        }

        CPH.LogInfo($"[first-chat-intro] First chat for userId={userId}");

        string url = $"{INFO_SERVICE_URL}/info/{COLLECTION_NAME}/{userId}";
        CPH.LogInfo($"[first-chat-intro] GET {url}");

        string body;
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            var response = client.GetAsync(url).GetAwaiter().GetResult();
            int status = (int)response.StatusCode;

            if (status == 404)
            {
                CPH.LogInfo($"[first-chat-intro] userId={userId} not found (404) — no-op.");
                return true;
            }

            if (status != 200)
            {
                CPH.LogInfo($"[first-chat-intro] Unexpected HTTP {status} for userId={userId} — no-op.");
                return true;
            }

            body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            CPH.LogInfo($"[first-chat-intro] HTTP error for userId={userId}: {ex.Message} — no-op.");
            return true;
        }

        bool enabled;
        string soundFile;
        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            if (!root.TryGetProperty("enabled", out var enabledProp))
            {
                CPH.LogInfo($"[first-chat-intro] userId={userId} — 'enabled' field missing — no-op.");
                return true;
            }
            enabled = enabledProp.GetBoolean();

            soundFile = root.TryGetProperty("soundFile", out var sfProp)
                ? sfProp.GetString() ?? ""
                : "";
        }
        catch (Exception ex)
        {
            CPH.LogInfo($"[first-chat-intro] JSON parse error for userId={userId}: {ex.Message} — no-op.");
            return true;
        }

        CPH.LogInfo($"[first-chat-intro] userId={userId} enabled={enabled} soundFile=\"{soundFile}\"");

        if (!enabled)
        {
            CPH.LogInfo($"[first-chat-intro] userId={userId} intro disabled — no-op.");
            return true;
        }

        if (string.IsNullOrWhiteSpace(soundFile))
        {
            CPH.LogInfo($"[first-chat-intro] userId={userId} intro enabled but soundFile empty — no-op.");
            return true;
        }

        string fullPath = System.IO.Path.Combine(ASSETS_ROOT, SOUND_SUBPATH, soundFile);
        CPH.LogInfo($"[first-chat-intro] userId={userId} dispatching intro. Path: {fullPath}");

        if (!System.IO.File.Exists(fullPath))
        {
            CPH.LogInfo($"[first-chat-intro] userId={userId} intro sound file missing at path: {fullPath} — no-op.");
            return true;
        }

        CPH.SetGlobalVar(VAR_SOUND_FILE_PATH, fullPath, false);
        CPH.RunAction(MIXITUP_ACTION_NAME, true);

        return true;
    }
}
