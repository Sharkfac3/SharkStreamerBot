---
name: buildtools
description: External tooling and creative-scaffolding routing for `Tools/` and `Creative/`. Covers Mix It Up API command discovery and world-building/art agent files.
---

# BuildTools

## Scope

This skill covers local tooling and creative scaffolding outside Streamer.bot runtime actions.

## Current Integrations

| Integration | Path | Description |
|---|---|---|
| Mix It Up | `Tools/MixItUp/Api/get_commands.py` | Fetches overlay commands from Mix It Up Developer API with pagination. Writes output to `Tools/MixItUp/Api/data/mixitup-commands.txt`. |
| Art | `Creative/Art/Agents/` | Art-generation agent prompt files and related creative scaffolding. |
| WorldBuilding | `Creative/WorldBuilding/` | Narrative/world-building agent files and experiments. |

## Conventions

- Place operational utilities under `Tools/<Integration>/...`.
- Place art/world-building scaffolding under `Creative/...`.
- Prefer Python for local tooling unless otherwise requested.
- Prefer stdlib-first solutions; avoid unnecessary external dependencies.
- For API integrations, support pagination (`skip` + `pageSize`) and write operator-readable output to file.

## Validation Checklist (after meaningful changes)

1. CLI help/syntax sanity check.
2. Happy-path API call against expected local service (when available).
3. Verify output file is written and readable.
4. Verify at least one edge case (service offline, malformed response, empty result set, pagination boundary).

## Change Summary Format

For `Tools/` changes, provide **run instructions** (command + expected output file path). For `Creative/` moves/docs, use `N/A` paste targets.
