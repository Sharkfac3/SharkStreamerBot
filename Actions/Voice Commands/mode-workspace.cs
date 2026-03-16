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
