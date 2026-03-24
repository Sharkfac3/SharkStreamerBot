# Core Skills — ops

## Business Context

This project is the technical infrastructure for a live R&D stream that is also a business. Every tool, validation pass, and change summary you produce keeps the stream running reliably — and a reliable stream is the foundation of the content pipeline that builds the community that becomes the customer base. Read `.agents/_shared/project.md` for the full business context and content pipeline.

## Tools/ Scope

Local utilities under `Tools/`. Does not cover `Creative/` work.

## Current Integrations

| Integration | Path | Description |
|---|---|---|
| Mix It Up command discovery | `Tools/MixItUp/Api/get_commands.py` | Fetches overlay commands from Mix It Up Developer API with pagination. Output: `Tools/MixItUp/Api/data/mixitup-commands.txt` |
| Routing doc sync | `Tools/StreamerBot/Validation/sync-routing-docs.py` | Rewrites generated routing tables in `AGENTS.md`, `.agents/ENTRY.md`, and `.pi/skills/README.md` |

## Tools/ Conventions

- Place operational utilities under `Tools/<Integration>/...`
- Prefer Python for local tooling unless otherwise requested
- Prefer stdlib-first solutions; avoid unnecessary external dependencies
- For API integrations, support pagination (`skip` + `pageSize`) and write operator-readable output to file

## Agent Infrastructure Guardrails

- `.agents/` is the source of truth for role knowledge and hierarchy
- `.pi/skills/` is a flat compatibility/routing layer for Pi
- Every `.agents/roles/<role>/role.md` should have a matching `.pi/skills/<role>/SKILL.md`
- Pi-exposed sub-skills must use flat role-qualified names like `<role>-<subskill>`
- `.agents/routing-manifest.json` is the primary machine-readable routing contract
- `python3 Tools/StreamerBot/Validation/sync-routing-docs.py` rewrites generated routing tables in `AGENTS.md`, `.agents/ENTRY.md`, and `.pi/skills/README.md`
- `.pi/skills/README.md`'s generated tables and required wrapper surfacing must stay aligned with `.agents/routing-manifest.json`
- If a legacy flat Pi wrapper name is already in circulation, preserve it as a migrated compatibility alias rather than reintroducing nested wrappers
- When changing agent scaffolding, update validation and routing docs in the same task so future agents do not inherit drift

## Change Control Rules

- Prefer small, targeted edits
- Preserve existing external behavior unless requested
- Do not rename files/actions casually when operators rely on them
- Highlight any breaking change before implementation
- If requirements are ambiguous for live behavior, ask before proceeding
