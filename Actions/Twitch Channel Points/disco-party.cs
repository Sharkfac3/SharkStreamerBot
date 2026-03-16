public class CPHInline
{
    // Shared mode variable key.
    // Keep these constants synchronized with Actions/SHARED-CONSTANTS.md.
    private const string VAR_STREAM_MODE = "stream_mode";
    private const string MODE_GARAGE = "garage";
    private const string MODE_WORKSPACE = "workspace";
    private const string MODE_GAMER = "gamer";

    // OBS scene names for the Disco Party redeem per stream mode.
    private const string OBS_SCENE_DISCO_GARAGE = "Disco Party: Garage";
    private const string OBS_SCENE_DISCO_WORKSPACE = "Disco Party: Workspace";
    private const string OBS_SCENE_DISCO_GAMER = "Disco Party: Gamer";

    /*
     * Purpose:
     * - Handles the "disco party" channel point redeem.
     * - Switches OBS to the Disco Party scene that matches the current stream mode.
     *
     * Expected trigger/input:
     * - Streamer.bot action wired to the "disco party" channel point redeem.
     * - No chat args required.
     *
     * Required runtime variables:
     * - Reads global var stream_mode.
     *
     * Key outputs/side effects:
     * - stream_mode == garage   -> OBS scene "Disco Party: Garage"
     * - stream_mode == workspace -> OBS scene "Disco Party: Workspace"
     * - stream_mode == gamer    -> OBS scene "Disco Party: Gamer"
     * - Unknown/empty mode safely falls back to workspace scene.
     * - No chat output.
     */
    public bool Execute()
    {
        string mode = (CPH.GetGlobalVar<string>(VAR_STREAM_MODE, false) ?? string.Empty)
            .Trim()
            .ToLowerInvariant();

        string targetScene = ResolveTargetScene(mode);
        if (string.IsNullOrWhiteSpace(targetScene))
        {
            CPH.LogWarn("[Twitch Redeem: Disco Party] Could not resolve a target scene.");
            return true;
        }

        CPH.ObsSetScene(targetScene);

        return true;
    }

    /// <summary>
    /// Resolves the disco party scene based on the shared stream mode global.
    /// Falls back to workspace for safety if mode is missing or unknown.
    /// </summary>
    private string ResolveTargetScene(string mode)
    {
        switch (mode)
        {
            case MODE_GARAGE:
                return OBS_SCENE_DISCO_GARAGE;
            case MODE_GAMER:
                return OBS_SCENE_DISCO_GAMER;
            case MODE_WORKSPACE:
                return OBS_SCENE_DISCO_WORKSPACE;
            default:
                CPH.LogWarn($"[Twitch Redeem: Disco Party] Unknown stream_mode '{mode}'. Falling back to workspace scene.");
                return OBS_SCENE_DISCO_WORKSPACE;
        }
    }
}
