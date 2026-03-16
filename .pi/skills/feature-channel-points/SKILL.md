---
name: feature-channel-points
description: Twitch Channel Point redeem scripts (Disco Party, Explain Current Task). Load when working on any script under Actions/Twitch Channel Points/.
---

# Feature: Channel Points

## When to Load

Load this skill for any work under `Actions/Twitch Channel Points/`.
Always pair it with `streamerbot-scripting` for `.cs` edits.
Load `change-summary` after making changes.

## Scope

| Script | Path | Trigger |
|---|---|---|
| Disco Party | `Actions/Twitch Channel Points/disco-party.cs` | Channel Reward → Reward Redemption |
| Explain Current Task | `Actions/Twitch Channel Points/explain-current-task.cs` | Channel Reward → Reward Redemption |

## Detailed Docs

- `Actions/Twitch Channel Points/README.md`

**Read the README before making changes.**

## Behavioral Expectations

- Scripts are triggered by specific named channel point redeems — do not change reward names or IDs without updating the Streamer.bot wiring.
- Disco Party switches OBS to a mode-specific scene; use `CPH.ObsSetScene(targetScene)` directly (no reflection).
- Explain Current Task ensures recording is active before triggering Mix It Up.
- Both scripts read `stream_mode` from global state — keep the mode fallback to `workspace`.
- No chat output from either script unless explicitly requested.

## OBS Dependency

Disco Party uses `stream_mode` to pick the target scene:
- `garage`    → `Disco Party: Garage`
- `workspace` → `Disco Party: Workspace`
- `gamer`     → `Disco Party: Gamer`

Scene names must match exactly what exists in OBS.

## Trigger Variables

Variable names confirmed from official Streamer.bot docs.
Access in C# via `CPH.TryGetArg("variableName", out T value)`.

### Channel Reward Redemption

| Variable | Type | Description |
|---|---|---|
| `user` | string | Display name of the redeeming user |
| `userId` | string | Twitch user ID of the redeeming user |
| `rewardName` | string | Name of the channel point reward |
| `rewardId` | string | Unique identifier for the reward |
| `rewardCost` | number | Point cost of the reward |
| `rewardPrompt` | string | Description/prompt text of the reward |
| `rawInput` | string | Text entered by the user (only if reward has text input enabled) |
| `rawInputEscaped` | string | Same as `rawInput` with special characters escaped |
| `redemptionId` | string | Unique identifier for this specific redemption |
| `counter` | number | Total number of times this reward has been redeemed |
| `userCounter` | number | Number of times this user has redeemed this reward |
