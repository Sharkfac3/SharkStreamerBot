using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CPHInline
{
    /*
     * Purpose:
     * - Base Twitch integration bridge for a hype train end event.
     *
     * Expected trigger/input:
     * - Wire this script to the Streamer.bot Twitch event for hype train ended.
     * - Reads the hype train end trigger variables defensively when present.
     *
     * Required runtime variables:
     * - None.
     *
     * Key outputs/side effects:
     * - Calls the Mix It Up Run Command API when a real command ID is configured.
     * - Keeps Arguments empty for current Mix It Up command compatibility.
     * - Sends populated SpecialIdentifiers for shared Mix It Up hype train command logic.
     * - Does not interact with OBS.
     *
     * Operator notes:
     * - Replace MIXITUP_COMMAND_ID before production use.
     * - Mix It Up identifiers are lowercase/no-space keys and values are string-friendly.
     */

    private const string SCRIPT_NAME = "Hype Train - End";
    private const string MIXITUP_BASE_URL = "http://localhost:8911";
    private const string MIXITUP_PLATFORM_TWITCH = "Twitch";
    private const string MIXITUP_COMMAND_ID = "REPLACE_WITH_HYPE_TRAIN_END_COMMAND_ID";

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
        return new
        {
            hypetrainlevel = GetIntArg("level").ToString(CultureInfo.InvariantCulture),
            hypetrainpercent = GetIntArg("percent").ToString(CultureInfo.InvariantCulture),
            hypetrainpercentdecimal = GetStringArg("percentDecimal"),
            hypetraintype = GetStringArg("trainType"),
            hypetraingoldenkappa = GetBoolArg("isGoldenKappaTrain").ToString().ToLowerInvariant(),
            hypetraintreasure = GetBoolArg("isTreasureTrain").ToString().ToLowerInvariant(),
            hypetrainshared = GetBoolArg("isSharedTrain").ToString().ToLowerInvariant(),
            hypetrainstartedat = GetStringArg("startedAt"),
            hypetrainid = GetStringArg("id"),
            hypetraintopbitsuser = GetStringArg("top.bits.user"),
            hypetraintopbitsuserid = GetStringArg("top.bits.userId"),
            hypetraintopbitstotal = GetIntArg("top.bits.total").ToString(CultureInfo.InvariantCulture),
            hypetraintopsubuser = GetStringArg("top.subscription.user"),
            hypetraintopsubuserid = GetStringArg("top.subscription.userId"),
            hypetraintopsubtotal = GetIntArg("top.subscription.total").ToString(CultureInfo.InvariantCulture),
            hypetraintopotheruser = GetStringArg("top.other.user"),
            hypetraintopotheruserid = GetStringArg("top.other.userId"),
            hypetraintopothertotal = GetIntArg("top.other.total").ToString(CultureInfo.InvariantCulture),
            hypetrainevent = "end"
        };
    }

    private string GetStringArg(string name)
    {
        if (CPH.TryGetArg(name, out string stringValue))
        {
            return stringValue ?? string.Empty;
        }

        if (CPH.TryGetArg(name, out object objectValue) && objectValue != null)
        {
            return Convert.ToString(objectValue, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        return string.Empty;
    }

    private int GetIntArg(string name)
    {
        if (CPH.TryGetArg(name, out int intValue))
        {
            return intValue;
        }

        if (CPH.TryGetArg(name, out long longValue))
        {
            return (int)longValue;
        }

        if (CPH.TryGetArg(name, out double doubleValue))
        {
            return (int)doubleValue;
        }

        string stringValue = GetStringArg(name);
        if (int.TryParse(stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedInt))
        {
            return parsedInt;
        }

        if (double.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedDouble))
        {
            return (int)parsedDouble;
        }

        return 0;
    }

    private bool GetBoolArg(string name)
    {
        if (CPH.TryGetArg(name, out bool boolValue))
        {
            return boolValue;
        }

        string stringValue = GetStringArg(name);
        return bool.TryParse(stringValue, out bool parsedBool) && parsedBool;
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
}
