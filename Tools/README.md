# Tools

`Tools/` is for external or local support utilities.

## Belongs here
- API scripts
- sync helpers
- validators
- exporters/importers
- Mix It Up command or overlay source used to operate stream tooling
- Streamer.bot support tooling that is not pasted into runtime actions

## Does not belong here
- Streamer.bot C# runtime action scripts
- creative prompt scaffolding
- lore, art direction, or marketing content
- repo architecture documentation

## Key subfolders
- `MixItUp/` — Mix It Up API, commands, overlays, and shared support files
- `StreamerBot/` — sync, validation, templates, and operator tooling

## Current structure
- `MixItUp/Api/`
- `MixItUp/Commands/`
- `MixItUp/Overlays/`
- `MixItUp/Shared/`
- `StreamerBot/Sync/`
- `StreamerBot/Validation/`
- `StreamerBot/Templates/`

## Common routing examples
- Need a Python script to query Mix It Up → `Tools/MixItUp/`
- Need overlay HTML/CSS/JS for Mix It Up → `Tools/MixItUp/Overlays/`
- Need a local validation or sync helper for Streamer.bot → `Tools/StreamerBot/`
