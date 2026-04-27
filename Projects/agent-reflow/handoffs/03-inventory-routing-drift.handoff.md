# Prompt 03 handoff — inventory-routing-drift

Date: 2026-04-26  
Agent: pi

## State changes

- Created `Projects/agent-reflow/findings/03-routing-drift.md`
- Created `Projects/agent-reflow/handoffs/03-inventory-routing-drift.handoff.md`
- Updated `WORKING.md` coordination board: added active row at start, removed it at finish, and added a recently-completed row

## Findings appended

- `findings/03-routing-drift.md`: comparison of routing sources, activation drift matrix, canonical child/sub-skill coverage, sync script coverage, generated marker inventory, and routing duplication map.

## Key findings

- All role-list sources contain the same 9 roles.
- Severe/ambiguous drift: `app-dev` differs between canonical manifest and `.agents/ENTRY.md` / `retired Pi skill mirror/README.md`.
  - Manifest: `Stream interaction apps (expanding)`
  - Entry/README: `Stream overlay ecosystem (broker, Phaser overlay, web apps) — TypeScript under Apps/`
- `AGENTS.md` quick routing is generated from manifest `quick_routing[]`, not `roles[].when`; every row is shorter/different by design.
- `retired Pi skill mirror/README.md ## Routing Table` is manual and not synced by `retired-routing-doc-sync.py`.
- `retired-routing-doc-sync.py` syncs `AGENTS.md` quick routing, `.agents/ENTRY.md ## Roles`, `retired Pi skill mirror/README.md ## Roles`, `retired Pi skill mirror/README.md ## Meta Wrappers`, and `retired Pi skill mirror/README.md ## Compatibility Aliases`.
- Generated markers exist only around `AGENTS.md` quick routing; heading-based synced tables in ENTRY/README have no generated markers.

## Validator status

- Last run: manual read/parse only; sync script was not run per prompt.
- Files checked:
  - `legacy v1 routing manifest (retired)`
  - `.agents/ENTRY.md`
  - `AGENTS.md`
  - `retired Pi skill mirror/README.md`
  - `Tools/StreamerBot/Validation/retired-routing-doc-sync.py`
- Sanity command used: `grep -n "^## Roles\|^## Routing Table\|^## Meta Wrappers\|^## Compatibility Aliases\|GENERATED\|replace_table_under_heading\|replace_generated_block\|readme_raw\|entry_raw\|agents_raw" ...`

## Open questions / blockers

- Operator should decide canonical `app-dev` activation text before anyone re-runs `Tools/StreamerBot/Validation/retired-routing-doc-sync.py`; otherwise current manifest text will overwrite the more specific ENTRY/README text.
- Future manifest/schema work should decide whether `retired Pi skill mirror/README.md ## Routing Table` becomes generated from manifest task routes.

## Next prompt entry point

- Read `Projects/agent-reflow/findings/03-routing-drift.md`.
- Use the operator flags section to resolve `app-dev` canonical text before sync/cutover changes.
