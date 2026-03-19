# Repository Structure

Five top-level working domains:

- `Actions/` — Streamer.bot runtime action source only
- `Tools/` — external/local utilities, integrations, validators, sync helpers
- `Creative/` — art, world-building, marketing, and reusable agent scaffolding
- `Docs/` — architecture, workflow, convention, and integration docs
- `.agents/` — agent knowledge tree (roles, skills, living context)

## Routing Rules

### `Actions/`
Belongs here:
- Streamer.bot C# action scripts
- Action-group docs tied directly to runtime behavior
- Shared runtime references (SHARED-CONSTANTS.md, HELPER-SNIPPETS.md)

Does not belong here:
- Local Python utilities
- Mix It Up export tools
- Creative prompt scaffolding
- Architecture notes unrelated to runtime actions

### `Tools/`
Belongs here:
- External/local support utilities
- Mix It Up API scripts, command helpers, overlay source
- Streamer.bot sync, validation, export/import helpers

Does not belong here:
- Streamer.bot runtime action source
- Art or story prompt systems
- Final creative assets

### `Creative/`
Belongs here:
- Reusable art-generation agents
- World-building and story scaffolding (LotAT, Starship Shamples)
- Marketing scaffolding
- References, experiments, projects, and approved assets

Does not belong here:
- Runtime action scripts
- Operational tooling scripts
- Repo-wide architecture docs

### `Docs/`
Belongs here:
- Repository structure docs
- Workflow docs
- Conventions and integration docs

Does not belong here:
- Runtime scripts
- Utility code
- Creative project outputs
- Agent skill content (that goes in `.agents/`)

### `.agents/`
Belongs here:
- Role definitions (`roles/<role>/role.md`)
- Skill content for each role (`roles/<role>/skills/`)
- Living context notes agents add over time (`roles/<role>/context/`)
- Shared cross-role knowledge (`_shared/`)
- Role template for adding new roles (`roles/_template/`)

Does not belong here:
- Runtime C# code (that goes in `Actions/`)
- Actual story content or lore (that goes in `Creative/WorldBuilding/`)
- Duplicate content from SHARED-CONSTANTS.md or HELPER-SNIPPETS.md (reference them)

## Current Ownership Notes

- Mix It Up tooling → `Tools/MixItUp/`
- Streamer.bot support tooling → `Tools/StreamerBot/`
- Art agent scaffolding → `Creative/Art/`
- LotAT/Starship Shamples worldbuilding → `Creative/WorldBuilding/`
- Agent skill tree → `.agents/roles/`
- Pi operational skill layer → `.pi/skills/`

`Actions/` should remain conceptually stable — runtime scripts only.
