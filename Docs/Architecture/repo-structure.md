# Repository Structure

This repo is being organized into four top-level working domains:

- `Actions/` — Streamer.bot runtime action source only
- `Tools/` — external/local utilities, integrations, validators, sync helpers
- `Creative/` — art, world-building, marketing, and reusable agent scaffolding
- `Docs/` — architecture, workflow, convention, and integration docs

## Routing rules

### `Actions/`
Belongs here:
- Streamer.bot C# action scripts
- action-group docs tied directly to runtime behavior
- shared runtime references such as constants and helper snippets

Does not belong here:
- local Python utilities
- Mix It Up export tools
- creative prompt scaffolding
- architecture notes unrelated to runtime actions

### `Tools/`
Belongs here:
- external/local support utilities
- Mix It Up API scripts, command helpers, overlay source
- Streamer.bot sync, validation, export/import helpers

Does not belong here:
- Streamer.bot runtime action source
- art or story prompt systems
- final creative assets

### `Creative/`
Belongs here:
- reusable art-generation agents
- world-building and story scaffolding
- marketing scaffolding
- references, experiments, projects, and approved assets

Does not belong here:
- runtime action scripts
- operational tooling scripts
- repo-wide architecture docs

### `Docs/`
Belongs here:
- repository structure docs
- workflow docs
- conventions and integration docs

Does not belong here:
- runtime scripts
- utility code
- creative project outputs

## Current ownership notes

- Mix It Up tooling belongs under `Tools/MixItUp/`
- Streamer.bot support tooling belongs under `Tools/StreamerBot/`
- art agent scaffolding belongs under `Creative/Art/`
- world-building scaffolding belongs under `Creative/WorldBuilding/`

`Actions/` should remain conceptually stable.
