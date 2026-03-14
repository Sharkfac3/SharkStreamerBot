# Stream Modes Script Reference

Shared constants reference: `Actions/SHARED-CONSTANTS.md`

## Script: `mode-garage.cs`

### Purpose
Sets the global stream mode to `garage`.

### Expected Trigger / Input
- Manual action trigger (button/hotkey/chain).

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
- Manual action trigger (button/hotkey/chain).

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
- Manual action trigger (button/hotkey/chain).

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
