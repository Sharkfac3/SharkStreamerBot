public class CPHInline
{
    // Shared mode variable key.
    // Keep this in sync with Actions/SHARED-CONSTANTS.md.
    private const string VAR_STREAM_MODE = "stream_mode";

    // Canonical mode value for gamer mode.
    private const string MODE_GAMER = "gamer";

    /*
     * Purpose:
     * - Switches the stream's global mode state to gamer.
     *
     * Expected trigger/input:
     * - Streamer.bot action trigger (manual button, hotkey, or chained action).
     * - No chat args required.
     *
     * Required runtime variables:
     * - Writes global var stream_mode.
     *
     * Key outputs/side effects:
     * - Sets stream_mode = "gamer".
     * - No chat output.
     */
    public bool Execute()
    {
        CPH.SetGlobalVar(VAR_STREAM_MODE, MODE_GAMER, false);
        return true;
    }
}
