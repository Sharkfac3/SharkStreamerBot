---
name: buildtools
description: External tooling for `Tools/` work — Mix It Up API command discovery, Python utilities, and StreamerBot validators. For art or worldbuilding tasks, use `creative-art` or `creative-worldbuilding` instead.
---

# BuildTools

## Scope

This skill covers local tooling under `Tools/`. It does **not** cover `Creative/` work — use `creative-art` or `creative-worldbuilding` for those tasks.

## Current Integrations

| Integration | Path | Description |
|---|---|---|
| Mix It Up | `Tools/MixItUp/Api/get_commands.py` | Fetches overlay commands from Mix It Up Developer API with pagination. Writes output to `Tools/MixItUp/Api/data/mixitup-commands.txt`. |

## Conventions

- Place operational utilities under `Tools/<Integration>/...`.
- Prefer Python for local tooling unless otherwise requested.
- Prefer stdlib-first solutions; avoid unnecessary external dependencies.
- For API integrations, support pagination (`skip` + `pageSize`) and write operator-readable output to file.

## Change Summary Format

For `Tools/` changes, provide **run instructions** (command + expected output file path). Validation checklist: see `sync-workflow` skill.
