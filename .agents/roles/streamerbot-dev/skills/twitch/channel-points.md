# Twitch Channel Points

## Scripts

| Script | Path | Trigger |
|---|---|---|
| `disco-party.cs` | `Actions/Twitch Channel Points/` | Channel Reward → Reward Redemption |
| `explain-current-task.cs` | `Actions/Twitch Channel Points/` | Channel Reward → Reward Redemption |

## Detailed Docs

`Actions/Twitch Channel Points/README.md`

## Disco Party

- Switches OBS to a mode-specific disco scene using `CPH.ObsSetScene()`
- Reads `stream_mode` global to pick the correct scene:
  - `garage` → `Disco Party: Garage`
  - `workspace` → `Disco Party: Workspace`
  - `gamer` → `Disco Party: Gamer`
- Scene names must match exactly what exists in OBS

## Explain Current Task

- Ensures recording is active before triggering Mix It Up command
- No chat output unless explicitly requested

## Shared Rules

- Scripts are triggered by specific named channel point redeems — do not change reward names or IDs without updating Streamer.bot wiring
- Both scripts read `stream_mode` — keep the mode fallback to `workspace`

## Trigger Variables

Full trigger variable reference: `Actions/Twitch Channel Points/README.md`
