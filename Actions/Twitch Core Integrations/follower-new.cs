// ACTION-CONTRACT: Actions/Twitch Core Integrations/AGENTS.md#follower-new.cs
// ACTION-CONTRACT-SHA256: 5b3c20a3a4aab5f75e7a1d2a005b278f6d13453ecb0b38649ae4967666af5969

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    // Runtime source of truth: Actions/Twitch Core Integrations/AGENTS.md
    // Shared names/constants reference: Actions/SHARED-CONSTANTS.md

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
