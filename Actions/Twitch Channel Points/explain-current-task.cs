// ACTION-CONTRACT: Actions/Twitch Channel Points/AGENTS.md#explain-current-task.cs
// ACTION-CONTRACT-SHA256: d165d03e67eb83ae741cbae06f40b7f6ff35665ff240866b229aaa12dc5aec9f

using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // Mix It Up API constants.
    private const string MIXITUP_API_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";

    // TODO: Replace this with the real Mix It Up command ID when available.
    private const string MIXITUP_EXPLAIN_CURRENT_TASK_COMMAND_ID = "replace-with-actual-id-dyude-cmon";

    // Reuse one HttpClient instance for reliability.
    private static readonly HttpClient MIXITUP_HTTP_CLIENT = new HttpClient();

    /*
     * Purpose:
     * - Handles the explain-current-task channel point redeem.
     * - Ensures OBS recording is active (start if not already recording).
     * - Triggers a Mix It Up command that can handle the spoken/overlay response.
     *
     * Expected trigger/input:
     * - Streamer.bot action wired to the explain-current-task channel point redeem.
     * - No chat arguments required.
     *
     * Required runtime variables:
     * - None.
     *
     * Key outputs/side effects:
     * - Attempts to start OBS recording only when needed.
     * - POSTs to Mix It Up command endpoint using standard payload shape.
     * - Logs warnings/errors instead of throwing, so action queue stays stable.
     */
    public bool Execute()
    {
        EnsureRecordingIsActive();

        string user = GetStringArg("user");
        string userId = GetStringArg("userId");
        string rewardId = GetStringArg("reward", "rewardId");
        string rewardName = GetStringArg("rewardName", "rewardTitle");
        string message = GetStringArg("userInput", "input0", "message", "rawInput");
        string messageType = string.IsNullOrWhiteSpace(message) ? "none" : "message";

        TriggerMixItUpCommand(
            MIXITUP_EXPLAIN_CURRENT_TASK_COMMAND_ID,
            "Twitch Redeem: Explain Current Task",
            arguments: "",
            specialIdentifiers: new
            {
                explaintaskuser = user,
                explaintaskuserid = userId,
                explaintasktype = "explaincurrenttask",
                explaintaskrewardid = rewardId,
                explaintaskrewardname = rewardName,
                explaintaskmessage = message,
                explaintaskmessagetype = messageType,
                explaintaskrecordingcheck = "attempted"
            }
        );

        return true;
    }

    /// <summary>
    /// Reads the first available Streamer.bot argument as a string.
    /// Missing or null values are normalized to an empty string so the Mix It Up payload stays stable.
    /// </summary>
    private string GetStringArg(params string[] names)
    {
        foreach (string name in names)
        {
            if (CPH.TryGetArg(name, out object value) && value != null)
                return value.ToString() ?? "";
        }

        return "";
    }

    /// <summary>
    /// Reads the first available Streamer.bot argument as an integer.
    /// Missing or invalid values are normalized to 0.
    /// </summary>
    private int GetIntArg(params string[] names)
    {
        foreach (string name in names)
        {
            if (!CPH.TryGetArg(name, out object value) || value == null)
                continue;

            if (value is int intValue)
                return intValue;

            if (int.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedValue))
                return parsedValue;
        }

        return 0;
    }

    /// <summary>
    /// Reads the first available Streamer.bot argument as a boolean.
    /// Missing or invalid values are normalized to false.
    /// </summary>
    private bool GetBoolArg(params string[] names)
    {
        foreach (string name in names)
        {
            if (!CPH.TryGetArg(name, out object value) || value == null)
                continue;

            if (value is bool boolValue)
                return boolValue;

            if (bool.TryParse(value.ToString(), out bool parsedValue))
                return parsedValue;
        }

        return false;
    }

    /// <summary>
    /// Starts recording if OBS is not already recording.
    ///
    /// Note:
    /// We use reflection and try common Streamer.bot OBS method names so this script
    /// remains resilient across naming differences in Streamer.bot versions.
    /// </summary>
    private void EnsureRecordingIsActive()
    {
        try
        {
            bool? isRecording = TryInvokeBoolNoArgs("ObsIsRecording");

            // If we can confidently read recording state and it is already active, nothing to do.
            if (isRecording == true)
                return;

            // Preferred behavior: explicitly start recording.
            if (TryInvokeNoArg("ObsStartRecording") || TryInvokeNoArg("ObsStartRecord"))
                return;

            // Fallback: if start methods are unavailable, toggle recording.
            if (TryInvokeNoArg("ObsToggleRecording") || TryInvokeNoArg("ObsToggleRecord"))
                return;

            CPH.LogWarn("[Twitch Redeem: Explain Current Task] Could not find a compatible OBS recording method on CPH.");
        }
        catch (Exception ex)
        {
            CPH.LogError($"[Twitch Redeem: Explain Current Task] Failed to ensure recording is active: {ex}");
        }
    }

    /// <summary>
    /// Calls a parameterless CPH method by name and returns a bool result when available.
    /// Returns null when the method does not exist or does not return bool.
    /// </summary>
    private bool? TryInvokeBoolNoArgs(string methodName)
    {
        var method = CPH.GetType().GetMethod(methodName, Type.EmptyTypes);
        if (method == null)
            return null;

        object result = method.Invoke(CPH, null);
        if (result is bool b)
            return b;

        return null;
    }

    /// <summary>
    /// Calls a parameterless CPH method by name.
    /// Returns false when the method does not exist or throws.
    /// </summary>
    private bool TryInvokeNoArg(string methodName)
    {
        try
        {
            var method = CPH.GetType().GetMethod(methodName, Type.EmptyTypes);
            if (method == null)
                return false;

            object result = method.Invoke(CPH, null);

            // Treat bool return values as success/failure when provided.
            if (result is bool b)
                return b;

            // For void/non-bool methods, reaching this point means invoke succeeded.
            return true;
        }
        catch (Exception ex)
        {
            CPH.LogWarn($"[Twitch Redeem: Explain Current Task] OBS method '{methodName}' failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Triggers a Mix It Up command via local API.
    /// Uses the required payload convention for Streamer.bot scripts:
    /// Platform, Arguments, SpecialIdentifiers, IgnoreRequirements.
    /// </summary>
    private bool TriggerMixItUpCommand(
        string commandId,
        string logPrefix,
        string arguments,
        object specialIdentifiers)
    {
        if (string.IsNullOrWhiteSpace(commandId))
        {
            CPH.LogWarn($"[{logPrefix}] Mix It Up command ID is not configured.");
            return false;
        }

        try
        {
            string url = $"{MIXITUP_API_BASE_URL.TrimEnd('/')}/api/v2/commands/{commandId}";
            string payload = JsonSerializer.Serialize(new
            {
                Platform = MIXITUP_PLATFORM_TWITCH,
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
