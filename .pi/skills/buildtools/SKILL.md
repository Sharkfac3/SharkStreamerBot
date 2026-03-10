---
name: buildtools
description: External Python/local tooling that runs outside Streamer.bot. Currently covers Mix It Up API command discovery and WorldBuilding agent prompts. Load when working on anything under BuildTools/.
---

# BuildTools

## Scope

BuildTools are terminal-run utilities for external integrations. They are **not** pasted into Streamer.bot actions.

## Current Integrations

| Integration | Path | Description |
|---|---|---|
| Mix It Up | `BuildTools/MixItUp/get_commands.py` | Fetches overlay commands from Mix It Up Developer API with pagination. Writes output to `mixitup-commands.txt`. |
| WorldBuilding | `BuildTools/WorldBuilding/` | Agent prompt files for art generation and narrative workflows. Not pi skills yet. |

## Conventions

- Place utilities under `BuildTools/<Integration>/...`.
- Prefer Python for local tooling unless otherwise requested.
- Prefer stdlib-first solutions; avoid unnecessary external dependencies.
- For API integrations, support pagination (`skip` + `pageSize`) and write operator-readable output to file.

## Validation Checklist (after meaningful changes)

1. CLI help/syntax sanity check.
2. Happy-path API call against expected local service (when available).
3. Verify output file is written and readable.
4. Verify at least one edge case (service offline, malformed response, empty result set, pagination boundary).

## Change Summary Format

For BuildTools changes, provide **run instructions** (command + expected output file path) instead of Streamer.bot paste targets.
