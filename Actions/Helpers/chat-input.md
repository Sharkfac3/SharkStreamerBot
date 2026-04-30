---
id: actions-helper-chat-input
type: reference
description: Streamer.bot C# snippets for defensive chat text, duplicate message, and sender argument handling.
owner: streamerbot-dev
secondaryOwners:
  - ops
status: active
---

## 3) Chat Message Input Helper (message/rawInput)

Use this when parsing user chat text so scripts work across different trigger payloads.

```csharp
private string GetMessageText()
{
    string text = "";
    if (!CPH.TryGetArg("message", out text) || string.IsNullOrWhiteSpace(text))
        CPH.TryGetArg("rawInput", out text);

    return text ?? "";
}
```

Optional duplicate guard using message ID:
```csharp
private const string VAR_LAST_MESSAGE_ID = "replace_last_message_id_var";

private bool IsDuplicateMessage()
{
    string msgId = "";
    if (!CPH.TryGetArg("msgId", out msgId) || string.IsNullOrWhiteSpace(msgId))
        return false;

    string lastId = CPH.GetGlobalVar<string>(VAR_LAST_MESSAGE_ID, false) ?? "";
    if (string.Equals(lastId, msgId, StringComparison.OrdinalIgnoreCase))
        return true;

    CPH.SetGlobalVar(VAR_LAST_MESSAGE_ID, msgId, false);
    return false;
}
```

Optional sender helper:
```csharp
private (string User, string UserId) GetSender()
{
    string user = "";
    string userId = "";

    CPH.TryGetArg("user", out user);
    CPH.TryGetArg("userId", out userId);

    user = user ?? "";
    userId = string.IsNullOrWhiteSpace(userId) ? user.ToLowerInvariant() : userId;

    return (user, userId);
}
```

## Chat mentions / user notifications

When a Streamer.bot action sends a chat message that directly addresses, thanks, warns, assigns, or lists a specific Twitch user, format that username as `@username`. Twitch uses the `@` mention form for the expected user notification/highlight behavior.

Use this helper for any chat-facing user mention. It trims stored display names and prevents accidental double-`@@` if a value already includes the prefix.

```csharp
private string FormatMention(string userName)
{
    string trimmed = (userName ?? string.Empty).Trim();
    if (string.IsNullOrWhiteSpace(trimmed))
        return string.Empty;

    return "@" + trimmed.TrimStart('@');
}
```

Do not add `@` for role names, character names, generic labels, or non-chat identifiers. Keep it for actual Twitch users the message is talking to or about.

## Related references

- [Verified CPH API Method Signatures](cph-api-signatures.md)

