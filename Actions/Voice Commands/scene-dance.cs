public class CPHInline
{
    // Shared mode variable key.
    // Keep these constants synchronized with Actions/SHARED-CONSTANTS.md.
    private const string VAR_STREAM_MODE = "stream_mode";
    private const string MODE_GARAGE = "garage";
    private const string MODE_WORKSPACE = "workspace";
    private const string MODE_GAMER = "gamer";

    // OBS scene labels used by this action.
    // Dance scenes use the "Disco Party" prefix instead of the standard '<Mode>: <Section>' pattern.
    private const string SCENE_PREFIX_DISCO_PARTY = "Disco Party";
    private const string SCENE_SUFFIX_GARAGE = "Garage";
    private const string SCENE_SUFFIX_WORKSPACE = "Workspace";
    private const string SCENE_SUFFIX_GAMER = "Gamer";

    /*
     * Purpose:
     * - Switches OBS to the Dance scene for the current stream mode.
     *
     * Expected trigger/input:
     * - Streamer.bot action trigger (voice command, button, hotkey, or chained action).
     * - No chat args required.
     *
     * Required runtime variables:
     * - Reads global var stream_mode.
     *
     * Key outputs/side effects:
     * - stream_mode == garage    -> OBS scene "Disco Party: Garage"
     * - stream_mode == workspace -> OBS scene "Disco Party: Workspace"
     * - stream_mode == gamer     -> OBS scene "Disco Party: gamer"
     * - Unknown/empty mode safely falls back to workspace.
     * - No chat output.
     *
     * Operator notes:
     * - Assumes the matching OBS scenes already exist with exact names.
     */
    public bool Execute()
    {
        string mode = (CPH.GetGlobalVar<string>(VAR_STREAM_MODE, false) ?? string.Empty)
            .Trim()
            .ToLowerInvariant();

        string targetScene = ResolveTargetScene(mode);
        if (string.IsNullOrWhiteSpace(targetScene))
        {
            CPH.LogWarn("[Voice Commands: Scene Dance] Could not resolve a target scene.");
            return true;
        }

        CPH.ObsSetScene(targetScene);

        return true;
    }

    /// <summary>
    /// Resolves the target OBS scene based on the shared stream mode global.
    /// Falls back to Workspace for safety if the mode is missing or unknown.
    /// </summary>
    private string ResolveTargetScene(string mode)
    {
        switch (mode)
        {
            case MODE_GARAGE:
                return BuildSceneName(SCENE_SUFFIX_GARAGE);
            case MODE_GAMER:
                return BuildSceneName(SCENE_SUFFIX_GAMER);
            case MODE_WORKSPACE:
                return BuildSceneName(SCENE_SUFFIX_WORKSPACE);
            default:
                CPH.LogWarn($"[Voice Commands: Scene Dance] Unknown stream_mode '{mode}'. Falling back to workspace scene.");
                return BuildSceneName(SCENE_SUFFIX_WORKSPACE);
        }
    }

    /// <summary>
    /// Builds a full OBS scene name using the disco party naming format.
    /// </summary>
    private string BuildSceneName(string modeSuffix)
    {
        return $"{SCENE_PREFIX_DISCO_PARTY}: {modeSuffix}";
    }
}
