// ACTION-CONTRACT: Actions/Voice Commands/AGENTS.md#mode-garage.cs
// ACTION-CONTRACT-SHA256: 09d0818bd315a97d9265f82c22f1cd25fee9b83d1796881c9cb83c399ba0b994

public class CPHInline
{
    // Shared mode variable key.
    // Keep this in sync with Actions/SHARED-CONSTANTS.md.
    private const string VAR_STREAM_MODE = "stream_mode";

    // Canonical mode value for garage mode.
    private const string MODE_GARAGE = "garage";

    /*
     * Purpose:
     * - Switches the stream's global mode state to garage.
     *
     * Expected trigger/input:
     * - Streamer.bot action trigger (manual button, hotkey, or chained action).
     * - No chat args required.
     *
     * Required runtime variables:
     * - Writes global var stream_mode.
     *
     * Key outputs/side effects:
     * - Sets stream_mode = "garage".
     * - No chat output.
     */
    public bool Execute()
    {
        CPH.SetGlobalVar(VAR_STREAM_MODE, MODE_GARAGE, false);
        return true;
    }
}
