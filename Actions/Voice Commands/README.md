# Voice Commands Script Reference

Shared constants reference: `Actions/SHARED-CONSTANTS.md`

## Scope
This folder contains voice-command-driven scripts that map to the Streamer.bot `voice commands` action group.

## Script: `mode-garage.cs`

### Purpose
Sets the global stream mode to `garage`.

### Expected Trigger / Input
- Voice command, button, hotkey, or chained action.

### Required Runtime Variables
- Writes `stream_mode`.

### Key Outputs / Side Effects
- `stream_mode = "garage"`

### Mix It Up Actions
- None.

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- None.

### Operator Notes
- Use this action whenever you switch to garage stream context.

---

## Script: `mode-workspace.cs`

### Purpose
Sets the global stream mode to `workspace`.

### Expected Trigger / Input
- Voice command, button, hotkey, or chained action.

### Required Runtime Variables
- Writes `stream_mode`.

### Key Outputs / Side Effects
- `stream_mode = "workspace"`

### Mix It Up Actions
- None.

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- None.

### Operator Notes
- `stream-start.cs` also sets this as the default mode at stream start.

---

## Script: `mode-gamer.cs`

### Purpose
Sets the global stream mode to `gamer`.

### Expected Trigger / Input
- Voice command, button, hotkey, or chained action.

### Required Runtime Variables
- Writes `stream_mode`.

### Key Outputs / Side Effects
- `stream_mode = "gamer"`

### Mix It Up Actions
- None.

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- None.

### Operator Notes
- Use this action whenever you switch to gamer stream context.

---

## Script: `scene-chat.cs`

### Purpose
Switches OBS to the `Chat` scene that matches the current `stream_mode`.

### Expected Trigger / Input
- Voice command, button, hotkey, or chained action.

### Required Runtime Variables
- Reads `stream_mode`.

### Key Outputs / Side Effects
- `stream_mode = "garage"` -> OBS scene `Garage: Chat`
- `stream_mode = "workspace"` -> OBS scene `Workspace: Chat`
- `stream_mode = "gamer"` -> OBS scene `Gamer: Chat`
- Unknown/empty mode falls back to `Workspace: Chat`

### Mix It Up Actions
- None.

### OBS Interactions
- Switches the current OBS scene using common Streamer.bot OBS scene setter methods.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs a warning if the mode is unknown or OBS scene switching fails.

### Operator Notes
- Assumes the matching OBS scenes already exist with exact names.

---

## Script: `scene-main.cs`

### Purpose
Switches OBS to the `Main` scene that matches the current `stream_mode`.

### Expected Trigger / Input
- Voice command, button, hotkey, or chained action.

### Required Runtime Variables
- Reads `stream_mode`.

### Key Outputs / Side Effects
- `stream_mode = "garage"` -> OBS scene `Garage: Main`
- `stream_mode = "workspace"` -> OBS scene `Workspace: Main`
- `stream_mode = "gamer"` -> OBS scene `Gamer: Main`
- Unknown/empty mode falls back to `Workspace: Main`

### Mix It Up Actions
- None.

### OBS Interactions
- Switches the current OBS scene using common Streamer.bot OBS scene setter methods.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs a warning if the mode is unknown or OBS scene switching fails.

### Operator Notes
- Assumes the matching OBS scenes already exist with exact names.

---

## Script: `scene-housekeeping.cs`

### Purpose
Switches OBS to the `Housekeeping` scene that matches the current `stream_mode`.

### Expected Trigger / Input
- Voice command, button, hotkey, or chained action.

### Required Runtime Variables
- Reads `stream_mode`.

### Key Outputs / Side Effects
- `stream_mode = "garage"` -> OBS scene `Garage: Housekeeping`
- `stream_mode = "workspace"` -> OBS scene `Workspace: Housekeeping`
- `stream_mode = "gamer"` -> OBS scene `Gamer: Housekeeping`
- Unknown/empty mode falls back to `Workspace: Housekeeping`

### Mix It Up Actions
- None.

### OBS Interactions
- Switches the current OBS scene using common Streamer.bot OBS scene setter methods.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs a warning if the mode is unknown or OBS scene switching fails.

### Operator Notes
- Assumes the matching OBS scenes already exist with exact names.

---

## Script: `scene-dance.cs`

### Purpose
Switches OBS to the disco-party scene that matches the current `stream_mode`.

### Expected Trigger / Input
- Voice command, button, hotkey, or chained action.

### Required Runtime Variables
- Reads `stream_mode`.

### Key Outputs / Side Effects
- `stream_mode = "garage"` -> OBS scene `Disco Party: Garage`
- `stream_mode = "workspace"` -> OBS scene `Disco Party: Workspace`
- `stream_mode = "gamer"` -> OBS scene `Disco Party: Gamer`
- Unknown/empty mode falls back to `Disco Party: Workspace`

### Mix It Up Actions
- None.

### OBS Interactions
- Switches the current OBS scene using common Streamer.bot OBS scene setter methods.

### Wait Behavior
- None.

### Chat / Log Output
- No chat output.
- Logs a warning if the mode is unknown or OBS scene switching fails.

### Operator Notes
- Assumes the matching OBS scenes already exist with exact names.
- This action uses the custom `Disco Party: <Mode>` scene naming instead of the normal `<Mode>: <Section>` pattern.

---

## General Operator Notes
- The mode scripts in this folder are the backbone for voice-command flows that depend on the current stream context.
- Keep `stream_mode` usage aligned with `Actions/Twitch Core Integrations/stream-start.cs`, `Actions/Twitch Channel Points/disco-party.cs`, and any future voice-command actions.
- Shared scene scripts in this folder usually assume OBS scene names follow the exact `<Mode>: <Section>` convention, such as `Garage: Chat`, `Workspace: Main`, and `Gamer: Housekeeping`.
- `scene-dance.cs` is the exception and uses `Disco Party: Garage`, `Disco Party: Workspace`, and `Disco Party: Gamer`.
