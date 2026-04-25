using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    /*
     * Purpose:
     * - Base Twitch integration bridge for a new follower event.
     *
     * Expected trigger/input:
     * - Wire this script to the Streamer.bot Twitch event for a new follow.
     * - Reads the new follower's display name and Twitch user ID from trigger args.
     *
     * Required runtime variables:
     * - None.
     *
     * Key outputs/side effects:
     * - Calls the Mix It Up Run Command API when a real command ID is configured.
     * - Sends populated follower SpecialIdentifiers for Mix It Up branching.
     * - Does not interact with OBS.
     *
     * Operator notes:
     * - Replace MIXITUP_COMMAND_ID before production use.
     * - In Mix It Up, reference: $followuser, $followuserid, $followtype.
     */

    private const string SCRIPT_NAME = "Core - Follower New";
    private const string MIXITUP_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";
    private const string MIXITUP_COMMAND_ID = "REPLACE_WITH_CORE_FOLLOWER_NEW_COMMAND_ID";

    private static readonly HttpClient Http = new HttpClient();

    public bool Execute()
    {
        try
        {
            if (HasPlaceholderCommandId(MIXITUP_COMMAND_ID))
            {
                CPH.LogWarn($"[{SCRIPT_NAME}] Mix It Up command ID is still a placeholder. Skipping call.");
                return true;
            }

            string arguments = BuildArguments();
            object specialIdentifiers = BuildSpecialIdentifiers();
            RunMixItUpCommand(arguments, specialIdentifiers);
        }
        catch (Exception ex)
        {
            CPH.LogError($"[{SCRIPT_NAME}] Exception while calling Mix It Up: {ex}");
        }

        return true;
    }

    private string BuildArguments()
    {
        return string.Empty;
    }

    private object BuildSpecialIdentifiers()
    {
        string user = GetStringArg("user");
        string userId = GetStringArg("userId");

        return new
        {
            followuser = user,
            followuserid = userId,
            followtype = "new"
        };
    }

    private void RunMixItUpCommand(string arguments, object specialIdentifiers)
    {
        string url = $"{MIXITUP_BASE_URL.TrimEnd('/')}/api/v2/commands/{MIXITUP_COMMAND_ID}";
        string payload = JsonSerializer.Serialize(new
        {
            Platform = MIXITUP_PLATFORM_TWITCH,
            Arguments = arguments ?? string.Empty,
            SpecialIdentifiers = specialIdentifiers ?? new { },
            IgnoreRequirements = false
        });

        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage response = Http.PostAsync(url, content).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            CPH.LogWarn($"[{SCRIPT_NAME}] Mix It Up call failed: {(int)response.StatusCode} {response.ReasonPhrase}");
        }
    }

    private bool HasPlaceholderCommandId(string commandId)
    {
        return string.IsNullOrWhiteSpace(commandId)
            || commandId.IndexOf("replace", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private string GetStringArg(string argName)
    {
        string value = "";
        CPH.TryGetArg(argName, out value);
        return value ?? "";
    }

    private int GetIntArg(string argName)
    {
        int value = 0;
        CPH.TryGetArg(argName, out value);
        return value;
    }

    private bool GetBoolArg(string argName)
    {
        bool value = false;
        CPH.TryGetArg(argName, out value);
        return value;
    }
}
