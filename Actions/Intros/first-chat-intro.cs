// ACTION-CONTRACT: Actions/Intros/contracts.md#first-chat-intro.cs
// ACTION-CONTRACT-SHA256: 338eab5660fcc76ce7f9b9111601b222c3a947a2d6c059cc65d4241e89088533

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // SYNC CONSTANTS — Info Service / Assets
    // Source of truth: Actions/SHARED-CONSTANTS.md §Info Service / Assets
    private const string INFO_SERVICE_URL = "http://127.0.0.1:8766";
    private const string ASSETS_ROOT = @"C:\Users\sharkfac3\Workspace\coding\SharkStreamerBot\Assets";
    private const string SOUND_SUBPATH = @"user-intros\sound\";
    private const string GIF_SUBPATH = @"user-intros\gif\";
    private const string COLLECTION_NAME = "user-intros";

    // Mix It Up API integration
    // Set this to the live Mix It Up command ID for command name: Custom Intro.
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_CUSTOM_INTRO_COMMAND_ID = "REPLACE_WITH_CUSTOM_INTRO_COMMAND_ID";
    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    /*
     * Purpose:
     * - Fires on Streamer.bot native "First Chat" event (once per viewer per stream session).
     * - Looks up the viewer in the info-service user-intros collection.
     * - If a custom intro is configured and enabled, dispatches Mix It Up "Custom Intro" directly via API.
     *
     * Expected trigger/input:
     * - Streamer.bot "First Chat" event.
     * - Args: userId (Twitch user ID of the viewer).
     *
     * Required runtime:
     * - info-service running at http://127.0.0.1:8766
     * - Mix It Up running locally with Developer API enabled
     * - MIXITUP_CUSTOM_INTRO_COMMAND_ID set to the live Custom Intro command ID
     *
     * Key outputs/side effects:
     * - Calls Mix It Up command "Custom Intro" directly when at least one
     *   configured asset resolves successfully.
     * - Sends resolved file paths in SpecialIdentifiers.intro_sound_file_path
     *   and SpecialIdentifiers.intro_gif_file_path.
     * - All branches log to CPH.LogInfo/LogWarn for operator tracing in SB action log.
     */
    public bool Execute()
    {
        const string logPrefix = "first-chat-intro";

        string userId = "";
        CPH.TryGetArg("userId", out userId);

        if (string.IsNullOrWhiteSpace(userId))
        {
            CPH.LogInfo($"[{logPrefix}] No userId in trigger args — skipping.");
            return true;
        }

        CPH.LogInfo($"[{logPrefix}] First chat for userId={userId}");

        string url = $"{INFO_SERVICE_URL}/info/{COLLECTION_NAME}/{userId}";
        CPH.LogInfo($"[{logPrefix}] GET {url}");

        string body;
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            var response = client.GetAsync(url).GetAwaiter().GetResult();
            int status = (int)response.StatusCode;

            if (status == 404)
            {
                CPH.LogInfo($"[{logPrefix}] userId={userId} not found (404) — no-op.");
                return true;
            }

            if (status != 200)
            {
                CPH.LogInfo($"[{logPrefix}] Unexpected HTTP {status} for userId={userId} — no-op.");
                return true;
            }

            body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            CPH.LogInfo($"[{logPrefix}] HTTP error for userId={userId}: {ex.Message} — no-op.");
            return true;
        }

        bool enabled;
        string soundFile;
        string gifFile;
        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            if (!root.TryGetProperty("enabled", out var enabledProp))
            {
                CPH.LogInfo($"[{logPrefix}] userId={userId} — 'enabled' field missing — no-op.");
                return true;
            }
            enabled = enabledProp.GetBoolean();

            soundFile = root.TryGetProperty("soundFile", out var sfProp)
                ? sfProp.GetString() ?? ""
                : "";
            gifFile = root.TryGetProperty("gifFile", out var gfProp)
                ? gfProp.GetString() ?? ""
                : "";
        }
        catch (Exception ex)
        {
            CPH.LogInfo($"[{logPrefix}] JSON parse error for userId={userId}: {ex.Message} — no-op.");
            return true;
        }

        CPH.LogInfo($"[{logPrefix}] userId={userId} enabled={enabled} soundFile=\"{soundFile}\" gifFile=\"{gifFile}\"");

        if (!enabled)
        {
            CPH.LogInfo($"[{logPrefix}] userId={userId} intro disabled — no-op.");
            return true;
        }

        if (string.IsNullOrWhiteSpace(soundFile) && string.IsNullOrWhiteSpace(gifFile))
        {
            CPH.LogInfo($"[{logPrefix}] userId={userId} intro enabled but no asset filename provided — no-op.");
            return true;
        }

        string resolvedSoundPath = "";
        string resolvedGifPath = "";

        if (!string.IsNullOrWhiteSpace(soundFile))
        {
            string normalizedSoundFile = System.IO.Path.GetFileName(soundFile);
            if (!string.Equals(soundFile, normalizedSoundFile, StringComparison.Ordinal))
            {
                CPH.LogInfo($"[{logPrefix}] userId={userId} soundFile included path segments; using filename-only value \"{normalizedSoundFile}\".");
            }

            if (string.IsNullOrWhiteSpace(normalizedSoundFile))
            {
                CPH.LogInfo($"[{logPrefix}] userId={userId} soundFile normalized to empty — continuing without sound.");
            }
            else
            {
                string fullSoundPath = System.IO.Path.Combine(ASSETS_ROOT, SOUND_SUBPATH, normalizedSoundFile);
                if (System.IO.File.Exists(fullSoundPath))
                {
                    resolvedSoundPath = fullSoundPath;
                }
                else
                {
                    CPH.LogInfo($"[{logPrefix}] userId={userId} intro sound file missing at path: {fullSoundPath} — continuing without sound.");
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(gifFile))
        {
            string normalizedGifFile = System.IO.Path.GetFileName(gifFile);
            if (!string.Equals(gifFile, normalizedGifFile, StringComparison.Ordinal))
            {
                CPH.LogInfo($"[{logPrefix}] userId={userId} gifFile included path segments; using filename-only value \"{normalizedGifFile}\".");
            }

            if (string.IsNullOrWhiteSpace(normalizedGifFile))
            {
                CPH.LogInfo($"[{logPrefix}] userId={userId} gifFile normalized to empty — continuing without gif.");
            }
            else
            {
                string fullGifPath = System.IO.Path.Combine(ASSETS_ROOT, GIF_SUBPATH, normalizedGifFile);
                if (System.IO.File.Exists(fullGifPath))
                {
                    resolvedGifPath = fullGifPath;
                }
                else
                {
                    CPH.LogInfo($"[{logPrefix}] userId={userId} intro gif file missing at path: {fullGifPath} — continuing without gif.");
                }
            }
        }

        if (string.IsNullOrWhiteSpace(resolvedSoundPath) && string.IsNullOrWhiteSpace(resolvedGifPath))
        {
            CPH.LogInfo($"[{logPrefix}] userId={userId} intro enabled but no configured assets resolved to local files — no-op.");
            return true;
        }

        CPH.LogInfo($"[{logPrefix}] userId={userId} dispatching Custom Intro directly via Mix It Up API. soundPath={resolvedSoundPath} gifPath={resolvedGifPath}");

        bool dispatched = TriggerMixItUpCommand(
            MIXITUP_CUSTOM_INTRO_COMMAND_ID,
            "first-chat-intro",
            arguments: "",
            specialIdentifiers: new
            {
                intro_sound_file_path = resolvedSoundPath,
                intro_gif_file_path = resolvedGifPath,
                userid = userId
            });

        if (!dispatched)
        {
            CPH.LogWarn($"[{logPrefix}] userId={userId} failed to dispatch Mix It Up Custom Intro command.");
        }

        return true;
    }

    private bool TriggerMixItUpCommand(
        string commandId,
        string logPrefix,
        string arguments = "",
        object specialIdentifiers = null)
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
                SpecialIdentifiers = specialIdentifiers ?? new { },
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
}
