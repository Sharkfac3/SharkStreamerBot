# StreamerBot

`Tools/StreamerBot/` is for local support tooling around Streamer.bot operations.

## Belongs here
- sync helpers
- validation scripts
- export/import helpers
- reusable operator templates

## Does not belong here
- runtime action scripts from `Actions/`
- Mix It Up-specific tooling
- creative scaffolding

## Recommended subfolders
- `Sync/`
- `Validation/`
- `Templates/`

## Action contract validation

`Validation/action_contracts.py` enforces local `Actions/**/AGENTS.md` action contracts as the source of truth for Streamer.bot scripts.

Common commands:

```bash
# Validate changed action scripts against their nearest AGENTS.md contract.
python3 Tools/StreamerBot/Validation/action_contracts.py --changed

# After editing a contract, refresh the script's ACTION-CONTRACT hash stamp.
python3 Tools/StreamerBot/Validation/action_contracts.py --script "Actions/<folder>/<script>.cs" --stamp
```

The validator requires each selected script to have a matching contract block in the nearest local `AGENTS.md`, a current `ACTION-CONTRACT-SHA256` stamp, and documented literal values present in the script.
