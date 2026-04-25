using System;
using System.Net.Http;
using System.Text;

public class CPHInline
{
    // SYNC CONSTANTS (Info Service)
    // Keep these in sync with Actions/SHARED-CONSTANTS.md §Info Service / Assets
    private const string INFO_SERVICE_URL = "http://127.0.0.1:8766";
    private const string COLLECTION_NAME  = "pending-intros";

    /*
     * Purpose:
     * - Captures a "Custom Intro" channel-point redemption into the pending-intros collection.
     * - Idempotent: duplicate redeemId is a no-op (safe to re-run if SB retries).
     *
     * Expected trigger:
     * - Streamer.bot channel-point redemption trigger for the "Custom Intro" reward.
     *   Operator must wire this action to that trigger in SB.
     *
     * Required SB trigger args:
     * - userId       — Twitch numeric userId (string)
     * - userLogin    — Twitch login (lowercase string)
     * - redeemId     — channel-point redemption ID (string)
     * - rewardTitle  — reward display name (string)
     * - rawInput     — user-supplied message; may be empty/null
     *
     * Key outputs/side effects:
     * - POSTs a new pending-intros record to info-service (status = "pending").
     * - Logs every branch to SB action log for operator tracing.
     */
    public bool Execute()
    {
        string userId      = "";
        string userLogin   = "";
        string redeemId    = "";
        string rewardTitle = "";
        string userInput   = "";

        CPH.TryGetArg("userId",      out userId);
        CPH.TryGetArg("userLogin",   out userLogin);
        CPH.TryGetArg("redeemId",    out redeemId);
        CPH.TryGetArg("rewardTitle", out rewardTitle);
        CPH.TryGetArg("rawInput",    out userInput);

        userId      = userId      ?? "";
        userLogin   = userLogin   ?? "";
        redeemId    = redeemId    ?? "";
        rewardTitle = rewardTitle ?? "";
        userInput   = userInput   ?? "";

        if (string.IsNullOrWhiteSpace(redeemId))
        {
            CPH.LogInfo($"[redeem-capture] Missing redeemId — cannot capture redeem. userId={userId}");
            return true;
        }

        string recordUrl = $"{INFO_SERVICE_URL}/info/{COLLECTION_NAME}/{redeemId}";

        // Duplicate check — GET existing record
        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(5);

            CPH.LogInfo($"[redeem-capture] GET {recordUrl} (duplicate check)");
            var getResponse = httpClient.GetAsync(recordUrl).GetAwaiter().GetResult();

            if (getResponse.IsSuccessStatusCode)
            {
                CPH.LogInfo($"[redeem-capture] Duplicate redeemId: {redeemId} — record already exists, skipping. userId={userId}");
                return true;
            }

            CPH.LogInfo($"[redeem-capture] GET returned {(int)getResponse.StatusCode} — proceeding to create record. redeemId={redeemId}");
        }
        catch (Exception ex)
        {
            CPH.LogInfo($"[redeem-capture] GET error for redeemId={redeemId}: {ex.Message} — proceeding to create.");
        }

        // Build JSON body
        long redeemUtc = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        string userInputJson = string.IsNullOrWhiteSpace(userInput)
            ? ""
            : $",\n  \"userInput\": {EscapeJsonString(userInput)}";

        string jsonBody = "{"
            + $"\n  \"userId\": {EscapeJsonString(userId)},"
            + $"\n  \"userLogin\": {EscapeJsonString(userLogin)},"
            + $"\n  \"redeemId\": {EscapeJsonString(redeemId)},"
            + $"\n  \"redeemUtc\": {redeemUtc},"
            + $"\n  \"rewardTitle\": {EscapeJsonString(rewardTitle)}"
            + userInputJson
            + ",\n  \"status\": \"pending\""
            + "\n}";

        // POST new record
        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(5);

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            CPH.LogInfo($"[redeem-capture] POST {recordUrl} — creating pending record. userId={userId} redeemId={redeemId}");
            var postResponse = httpClient.PostAsync(recordUrl, content).GetAwaiter().GetResult();
            int statusCode   = (int)postResponse.StatusCode;

            if (statusCode == 200 || statusCode == 201)
            {
                CPH.LogInfo($"[redeem-capture] Success ({statusCode}) — pending record created. redeemId={redeemId} userId={userId}");
            }
            else
            {
                string body = "";
                try { body = postResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult(); } catch { }
                string excerpt = body.Length > 200 ? body.Substring(0, 200) : body;
                CPH.LogInfo($"[redeem-capture] POST error ({statusCode}) for redeemId={redeemId}: {excerpt}");
            }
        }
        catch (Exception ex)
        {
            CPH.LogInfo($"[redeem-capture] POST exception for redeemId={redeemId}: {ex.Message}");
        }

        return true;
    }

    private static string EscapeJsonString(string value)
    {
        if (value == null) return "\"\"";
        var sb = new StringBuilder("\"");
        foreach (char c in value)
        {
            switch (c)
            {
                case '"':  sb.Append("\\\""); break;
                case '\\': sb.Append("\\\\"); break;
                case '\n': sb.Append("\\n");  break;
                case '\r': sb.Append("\\r");  break;
                case '\t': sb.Append("\\t");  break;
                default:
                    if (c < 0x20)
                        sb.Append($"\\u{(int)c:x4}");
                    else
                        sb.Append(c);
                    break;
            }
        }
        sb.Append('"');
        return sb.ToString();
    }
}
