---
id: actions-helper-timers
type: reference
description: Streamer.bot timer enable, disable, reset, interval-update, and verification patterns.
owner: streamerbot-dev
secondaryOwners:
  - ops
status: active
---

## 6) Timer Management

Use these patterns when a script needs to control Streamer.bot timers beyond basic enable/disable.

### Start a timer (enable)

```csharp
CPH.EnableTimer(string timerName);
```

Starts the timer if it is not already running. The countdown begins from the timer's configured interval.

### Stop a timer (disable)

```csharp
CPH.DisableTimer(string timerName);
```

Stops the timer immediately. The current countdown position is discarded.

### Reset a timer's countdown (restart from full interval)

Disable then immediately re-enable. This restarts the countdown from the full configured interval without changing the interval value.

```csharp
private void ResetTimer(string timerName)
{
    CPH.DisableTimer(timerName);
    CPH.EnableTimer(timerName);
}
```

Use this when you want to "push back" a timer deadline — for example, extending a call window each time the player does something valid.

Example:
```csharp
// Player submitted a valid action; restart the call window so they get the full window again.
ResetTimer(TIMER_DUCK_CALL_WINDOW);
```

### Update a timer's interval (change how long it runs)

Disable the timer, set the new interval, then re-enable it. The timer restarts from the new interval value.

```csharp
// VERIFY: unconfirmed method signature — test before relying on this in production.
private void SetTimerInterval(string timerName, int seconds)
{
    CPH.DisableTimer(timerName);
    CPH.SetTimerInterval(timerName, seconds); // VERIFY: unconfirmed method signature
    CPH.EnableTimer(timerName);
}
```

> **Operator note:** `CPH.SetTimerInterval` is not yet confirmed against Streamer.bot docs.
> Verify this method exists and accepts `(string, int)` before shipping any script that uses it.
> If it does not exist, the interval must be changed manually in the Streamer.bot UI and this
> method call removed.

### Safe enable/disable helpers with logging

Use these when you want a consistent log trail for timer state changes:

```csharp
private void StartTimer(string timerName, string logPrefix)
{
    CPH.LogWarn($"[{logPrefix}] Starting timer: {timerName}");
    CPH.EnableTimer(timerName);
}

private void StopTimer(string timerName, string logPrefix)
{
    CPH.LogWarn($"[{logPrefix}] Stopping timer: {timerName}");
    CPH.DisableTimer(timerName);
}
```

### Operator notes

- `CPH.EnableTimer` / `CPH.DisableTimer` are verified. The disable→enable trick for resetting the countdown is safe and relies only on those two verified calls.
- Timer names must match entries in `Actions/SHARED-CONSTANTS.md` exactly (case-sensitive).
- All timers should be disabled in [Actions/Twitch Core Integrations/stream-start.cs](../Twitch%20Core%20Integrations/stream-start.cs) at stream start to clear stale state from previous sessions.

## Related references

- [Verified CPH API Method Signatures](cph-api-signatures.md)
- [Actions/SHARED-CONSTANTS.md — timer names](../SHARED-CONSTANTS.md)

