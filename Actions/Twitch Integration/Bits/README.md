# Bits Script Reference

Shared constants reference: `Actions/SHARED-CONSTANTS.md`

## Script: `bits-tier-1.cs`

### Purpose
Forwards Tier 1 cheer text to Mix It Up with sanitization and TTS pacing wait.

### Expected Trigger / Input
- Tier 1 cheer trigger with message text (`message` fallback `rawInput`).

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Sanitizes cheer text and forwards it to Mix It Up.
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_TIER_1_COMMAND_ID` *(placeholder in script; not configured yet)*
- Payload `Arguments`: sanitized full cheer text (Cheer tokens removed, no word cap)

### OBS Interactions
- None.

### Wait Behavior
- Wait after successful Mix It Up call: `3000ms + 400ms per word + 500ms buffer`.

### Chat / Log Output
- Reads cheer text from trigger args.
- Sends no chat output.

### Operator Notes
- Replace placeholder command ID before production use.

---

## Script: `bits-tier-2.cs`

### Purpose
Forwards Tier 2 cheer text to Mix It Up with a 250-word cap.

### Expected Trigger / Input
- Tier 2 cheer trigger with message text (`message` fallback `rawInput`).

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Sanitizes cheer text, caps to 250 words, forwards to Mix It Up.
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_TIER_2_COMMAND_ID` *(placeholder in script; not configured yet)*
- Payload `Arguments`: sanitized cheer text capped to first 250 words

### OBS Interactions
- None.

### Wait Behavior
- Wait after successful Mix It Up call: `3000ms + 400ms per word + 500ms buffer`.

### Chat / Log Output
- Reads cheer text from trigger args.
- Sends no chat output.

### Operator Notes
- Replace placeholder command ID before production use.

---

## Script: `bits-tier-3.cs`

### Purpose
Forwards Tier 3 cheer text to Mix It Up with a 100-word cap.

### Expected Trigger / Input
- Tier 3 cheer trigger with message text (`message` fallback `rawInput`).

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Sanitizes cheer text, caps to 100 words, forwards to Mix It Up.
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `23f1afd1-7375-475d-afee-058ef4f7f68d`
- Payload `Arguments`: sanitized cheer text capped to first 100 words

### OBS Interactions
- None.

### Wait Behavior
- Wait after successful Mix It Up call: `3000ms + 400ms per word + 500ms buffer`.

### Chat / Log Output
- Reads cheer text from trigger args.
- Sends no chat output.

### Operator Notes
- Current command ID is configured.

---

## Script: `bits-tier-4.cs`

### Purpose
Forwards Tier 4 cheer text to Mix It Up with a 10-word cap.

### Expected Trigger / Input
- Tier 4 cheer trigger with message text (`message` fallback `rawInput`).

### Required Runtime Variables
- None.

### Key Outputs / Side Effects
- Sanitizes cheer text, caps to 10 words, forwards to Mix It Up.
- Applies pacing wait to reduce overlap/cutoff.

### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID: `REPLACE_WITH_TIER_4_COMMAND_ID` *(placeholder in script; not configured yet)*
- Payload `Arguments`: sanitized cheer text capped to first 10 words

### OBS Interactions
- None.

### Wait Behavior
- Wait after successful Mix It Up call: `3000ms + 400ms per word + 500ms buffer`.

### Chat / Log Output
- Reads cheer text from trigger args.
- Sends no chat output.

### Operator Notes
- Replace placeholder command ID before production use.
