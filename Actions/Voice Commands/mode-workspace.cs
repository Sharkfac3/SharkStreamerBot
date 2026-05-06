// ACTION-CONTRACT: Actions/Voice Commands/AGENTS.md#mode-workspace.cs
// ACTION-CONTRACT-SHA256: ec9b0811d175820eb774286bc23f119e5abe68d4b3900092a9f6d77c70df2a1c

public class CPHInline
{
    // Shared mode variable key.
    // Keep this in sync with Actions/SHARED-CONSTANTS.md.
    private const string VAR_STREAM_MODE = "stream_mode";

    // Canonical mode value for workspace mode.
    private const string MODE_WORKSPACE = "workspace";

    /*
     * Purpose:
     * - Switches the stream's global mode state to workspace.
     *
     * Expected trigger/input:
     * - Streamer.bot action trigger (manual button, hotkey, or chained action).
     * - No chat args required.
     *
     * Required runtime variables:
     * - Writes global var stream_mode.
     *
     * Key outputs/side effects:
     * - Sets stream_mode = "workspace".
     * - No chat output.
     */
    public bool Execute()
    {
        CPH.SetGlobalVar(VAR_STREAM_MODE, MODE_WORKSPACE, false);
        return true;
    }
}
