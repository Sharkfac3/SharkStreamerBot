# Mix It Up API

Use this folder for Mix It Up API scripts and saved API-derived data.

## Belongs here
- scripts that call the Mix It Up Desktop API
- saved command export data used by tooling

## Does not belong here
- overlay HTML/CSS/JS
- non-Mix It Up tooling
- Streamer.bot runtime action scripts

## Current files
- `get_commands.py` — fetches command data with pagination and writes an action-script-friendly lookup export
- `data/mixitup-commands.txt` — saved Action Group lookup export for agent/operator use

## Action script authoring workflow
When an agent or operator needs a Mix It Up command ID for a Streamer.bot action script:
1. Run `python3 Tools/MixItUp/Api/get_commands.py`
2. Open `Tools/MixItUp/Api/data/mixitup-commands.txt`
3. Check `Action Group Lookup (Exact Name, Alphabetical)` for the intended command name
4. If needed, use `Action Group Lookup (Normalized Name)` for fuzzy lookup
5. If the command is not present in the Action Group sections, treat that ID as not yet available in Mix It Up

Only `Action Group` commands should be used as authoritative lookup data for Streamer.bot action integration in this repo.
The export is intentionally summary/lookup-focused and no longer includes the full raw JSON payload.
