# MixItUp

`Tools/MixItUp/` is the home for Mix It Up operational tooling and source.

## Belongs here
- API scripts
- command-support files
- overlay HTML/CSS/JS
- shared Mix It Up support files
- saved command/export data used by tooling

## Does not belong here
- Streamer.bot runtime action scripts
- general Streamer.bot tooling unrelated to Mix It Up
- art or world-building prompt files

## Recommended subfolders
- `Api/` — scripts that call Mix It Up APIs
- `Commands/` — command-related source or support files
- `Overlays/` — HTML/CSS/JS overlay source
- `Shared/` — reusable support files

## Common routing examples
- Fetch command definitions from Mix It Up → `Api/`
- Store command export text/data → `Api/data/`
- Build or edit an overlay page → `Overlays/`
