# Twitch Bits Integrations

## Scripts

| Script | Path | Tier Threshold |
|---|---|---|
| `bits-tier-1.cs` | `Actions/Twitch Bits Integrations/` | See SHARED-CONSTANTS |
| `bits-tier-2.cs` | `Actions/Twitch Bits Integrations/` | See SHARED-CONSTANTS |
| `bits-tier-3.cs` | `Actions/Twitch Bits Integrations/` | See SHARED-CONSTANTS |
| `bits-tier-4.cs` | `Actions/Twitch Bits Integrations/` | See SHARED-CONSTANTS |

## Bits Scripts Also In This Feature Group

| Script | Path | Purpose |
|---|---|---|
| `gigantify-emote.cs` | `Actions/Twitch Bits Integrations/` | Giant emote overlay |
| `message-effects.cs` | `Actions/Twitch Bits Integrations/` | Message effect trigger |
| `on-screen-celebration.cs` | `Actions/Twitch Bits Integrations/` | Celebration overlay |

## Behavior

- Each tier script forwards the cheer message to a Mix It Up command
- Strip cheer tokens (`Cheer100`, `PogChamp50`, etc.) from message text before forwarding
- Calculate dynamic wait time from word count to prevent overlapping TTS readouts
- Pacing wait added after message-effects trigger to prevent audio overlap

## Tier Thresholds

Defined in `Actions/SHARED-CONSTANTS.md` — do not hardcode bit values in scripts.

## Detailed Docs

`Actions/Twitch Bits Integrations/README.md`
