# Prompt 01 — Inventory Pi Mirror

## Agent

Pi (manual copy/paste by operator).

## Purpose

Catalog every wrapper under `retired Pi skill mirror/`. Maps each wrapper to its source in `.agents/roles/` (or flags as orphan/migrated alias). Establishes the "double tree" overhead that Phase E migrations must eliminate.

## Preconditions

- Prompt 00 done; `Projects/agent-reflow/findings/00-current-tree.md` exists.
- Read `Projects/agent-reflow/handoffs/00-inventory-agents-tree.handoff.md` first.

## Scope

Read-only inventory of `retired Pi skill mirror/`. Writes `Projects/agent-reflow/findings/01-pi-mirror.md` and corresponding handoff.

## Out-of-scope

- No edits to `.pi/`
- No edits to `.agents/`
- No git operations
- No deletions

## Steps

1. Walk `retired Pi skill mirror/` recursively.
2. For each wrapper folder, capture: name, `SKILL.md` line count, frontmatter `name` + `description`, body content category (real content / pure-routing stub / migrated alias).
3. For pure-routing stubs, identify the `.agents/roles/<role>/...` target each points to.
4. For migrated aliases, identify both the canonical wrapper they redirect to and (if traceable) the original pre-migration name.
5. Cross-reference against findings/00-current-tree.md manifest data.
6. Flag: wrappers without a manifest entry, manifest entries without a wrapper, wrappers pointing to nonexistent agent-tree paths.
7. Write findings file with sections: Wrapper Inventory, Routing Stubs, Migrated Aliases, Orphans, Manifest Cross-Check, Summary Statistics.

## Validator / Acceptance

- `findings/01-pi-mirror.md` exists, lists every wrapper under `retired Pi skill mirror/`
- Counts match what's visible to operator
- All migrated aliases identified
- Orphan wrappers (no manifest entry) called out

## Handoff

Per template in `README.md`. Note any wrappers Pi itself currently relies on that will be affected by later cutover.
