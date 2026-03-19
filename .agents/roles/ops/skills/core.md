# Core Skills — ops

## Tools/ Scope

Local utilities under `Tools/`. Does not cover `Creative/` work.

## Current Integrations

| Integration | Path | Description |
|---|---|---|
| Mix It Up command discovery | `Tools/MixItUp/Api/get_commands.py` | Fetches overlay commands from Mix It Up Developer API with pagination. Output: `Tools/MixItUp/Api/data/mixitup-commands.txt` |
| StreamerBot validator | `Tools/StreamerBot/Validation/validate.py` | Validates scripts against SHARED-CONSTANTS for drift |
| Pre-commit hooks | `Tools/StreamerBot/Validation/install-hooks.py` | Installs pre-commit validation hooks |

## Tools/ Conventions

- Place operational utilities under `Tools/<Integration>/...`
- Prefer Python for local tooling unless otherwise requested
- Prefer stdlib-first solutions; avoid unnecessary external dependencies
- For API integrations, support pagination (`skip` + `pageSize`) and write operator-readable output to file

## Change Control Rules

- Prefer small, targeted edits
- Preserve existing external behavior unless requested
- Do not rename files/actions casually when operators rely on them
- Highlight any breaking change before implementation
- If requirements are ambiguous for live behavior, ask before proceeding
