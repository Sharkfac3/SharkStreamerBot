using System;

public class CPHInline
{
    // Shared mode variable key.
    // Keep these constants synchronized with Actions/SHARED-CONSTANTS.md.
    private const string VAR_STREAM_MODE = "stream_mode";
    private const string MODE_GARAGE = "garage";
    private const string MODE_WORKSPACE = "workspace";
    private const string MODE_GAMER = "gamer";

    // OBS scene labels used by this action.
    private const string SCENE_SECTION_LABEL = "Main";
    private const string SCENE_PREFIX_GARAGE = "Garage";
    private const string SCENE_PREFIX_WORKSPACE = "Workspace";
    private const string SCENE_PREFIX_GAMER = "Gamer";

    /*
     * Purpose:
     * - Switches OBS to the Main scene for the current stream mode.
     *
     * Expected trigger/input:
     * - Streamer.bot action trigger (voice command, button, hotkey, or chained action).
     * - No chat args required.
     *
     * Required runtime variables:
     * - Reads global var stream_mode.
     *
     * Key outputs/side effects:
     * - stream_mode == garage    -> OBS scene "Garage: Main"
     * - stream_mode == workspace -> OBS scene "Workspace: Main"
     * - stream_mode == gamer     -> OBS scene "Gamer: Main"
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
            CPH.LogWarn("[Voice Commands: Scene Main] Could not resolve a target scene.");
            return true;
        }

        if (!TrySetObsScene(targetScene))
        {
            CPH.LogWarn($"[Voice Commands: Scene Main] Failed to switch OBS scene to '{targetScene}'.");
        }

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
                return BuildSceneName(SCENE_PREFIX_GARAGE);
            case MODE_GAMER:
                return BuildSceneName(SCENE_PREFIX_GAMER);
            case MODE_WORKSPACE:
                return BuildSceneName(SCENE_PREFIX_WORKSPACE);
            default:
                CPH.LogWarn($"[Voice Commands: Scene Main] Unknown stream_mode '{mode}'. Falling back to workspace scene.");
                return BuildSceneName(SCENE_PREFIX_WORKSPACE);
        }
    }

    /// <summary>
    /// Builds a full OBS scene name using the standard '<Mode>: <Section>' format.
    /// </summary>
    private string BuildSceneName(string modePrefix)
    {
        return $"{modePrefix}: {SCENE_SECTION_LABEL}";
    }

    /// <summary>
    /// Tries common Streamer.bot OBS scene switch method names using reflection,
    /// so this script remains resilient across API naming differences.
    /// </summary>
    private bool TrySetObsScene(string sceneName)
    {
        return TryInvokeStringArg("ObsSetCurrentScene", sceneName)
            || TryInvokeStringArg("ObsSetScene", sceneName)
            || TryInvokeStringArg("ObsSetProgramScene", sceneName);
    }

    /// <summary>
    /// Calls a single-string-argument CPH method by name.
    /// Returns false when the method does not exist or invocation fails.
    /// </summary>
    private bool TryInvokeStringArg(string methodName, string arg)
    {
        try
        {
            var method = CPH.GetType().GetMethod(methodName, new[] { typeof(string) });
            if (method == null)
                return false;

            object result = method.Invoke(CPH, new object[] { arg });

            // Respect bool success/failure return values when provided.
            if (result is bool b)
                return b;

            // Void/non-bool return means invoke succeeded.
            return true;
        }
        catch (Exception ex)
        {
            CPH.LogWarn($"[Voice Commands: Scene Main] OBS method '{methodName}' failed: {ex.Message}");
            return false;
        }
    }
}
