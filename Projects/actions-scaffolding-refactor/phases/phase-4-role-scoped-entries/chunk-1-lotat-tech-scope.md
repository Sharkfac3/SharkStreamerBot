---
id: phase-4-chunk-1-lotat-tech-scope
type: refactor-prompt
phase: 4
chunk: 1
status: ready
---

# Phase 4 / Chunk 1 — Add lotat-tech Actions Scope

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You create and update agent scaffolding markdown files to improve navigability. You do not touch `.cs` runtime scripts and do not alter runtime behavior.

## Repository Context

`.agents/roles/lotat-tech/role.md` defines the lotat-tech agent role — responsible for LotAT story schema, runtime engine, and tooling. When lotat-tech works in `Actions/`, it only needs LotAT-specific files. But there is no scoped reading list, so it currently navigates the entire Actions scaffolding.

This chunk adds `actions-scope.md` to the lotat-tech role folder — a targeted reading list for Actions work.

## Prerequisite

All Phase 1, 2, and 3 chunks must show `done` in `progress.md`. Phase 4 Chunk 1 must show `pending`.

## What This Chunk Does

Create `.agents/roles/lotat-tech/actions-scope.md` with a scoped reading list for lotat-tech's Actions work.

Optionally update `.agents/roles/lotat-tech/role.md` to reference `actions-scope.md` if it will improve navigation.

**Files touched:**
- `.agents/roles/lotat-tech/actions-scope.md` — created
- `.agents/roles/lotat-tech/role.md` — updated (add reference to actions-scope.md if beneficial)

## What This Chunk Does NOT Do

- Change what lotat-tech owns or is responsible for
- Remove any content from role.md — only add a reference
- Touch any Actions files
- Touch any other role folders

## Files to Read Before Starting

1. `.agents/roles/lotat-tech/role.md` — understand current role definition
2. `Actions/AGENTS.md` — current state (lean router after Phase 2)
3. `Actions/LotAT/AGENTS.md` — lotat-tech's primary Actions domain
4. `Actions/constants/lotat.md` — LotAT constants file
5. `Actions/constants/overlay-broker.md` — needed for LotAT overlay publishing
6. `Projects/actions-scaffolding-refactor/progress.md` — confirm prerequisites

## Step-by-Step Instructions

1. Read all files listed above.

2. Create `.agents/roles/lotat-tech/actions-scope.md`:

```markdown
---
id: lotat-tech-actions-scope
type: scope
description: Scoped reading list for lotat-tech working in Actions/.
owner: lotat-tech
parent: role.md
---

# lotat-tech — Actions Scope

When working in Actions/, load in this order. Stop loading when you have what you need for the task.

## Required for All LotAT Work

1. [Actions/AGENTS.md](../../../Actions/AGENTS.md) — domain entry, folder routing
2. [Actions/LotAT/AGENTS.md](../../../Actions/LotAT/AGENTS.md) — LotAT rules, ownership, workflow, phase map
3. [Actions/LotAT/contracts.md](../../../Actions/LotAT/contracts.md) — all 12 LotAT script contracts (load when validating or updating a contract)

## Constants Relevant to LotAT

- [Actions/constants/lotat.md](../../../Actions/constants/lotat.md) — session state, roster, dice, offering globals
- [Actions/constants/overlay-broker.md](../../../Actions/constants/overlay-broker.md) — broker topics used by LotAT overlay publish scripts

## Do Not Load for LotAT Work

- Squad contracts, mini-game constants — not relevant to LotAT
- Commander constants — not used by LotAT runtime
- XJ Drivethrough, Destroyer, Rest/Focus constants — not relevant

## If You Need to Validate

See [Actions/CONTRACT-SCHEMA.md](../../../Actions/CONTRACT-SCHEMA.md) for contract format spec.
Run: `python3 Tools/StreamerBot/Validation/action_contracts.py --changed`
```

3. Read `.agents/roles/lotat-tech/role.md`. If it has a section about navigating Actions/ or Common Routes, add a line:

```markdown
For Actions/ work, see [actions-scope.md](actions-scope.md) for a scoped reading list.
```

   If the role file has no existing Actions navigation guidance, add the reference under the `## Common Routes` section or equivalent. Do not restructure the role file.

4. Verify both files look correct.

5. Update `progress.md`: Phase 4 chunk 1 → `done`, update "Last updated".

## Output Requirements

- `.agents/roles/lotat-tech/actions-scope.md` exists with scoped reading list
- `.agents/roles/lotat-tech/role.md` references `actions-scope.md`
- lotat-tech role.md unchanged in structure/content except for the added reference
- `progress.md` chunk 1 shows `done`

## Validation Checklist

- [ ] `actions-scope.md` created in lotat-tech role folder
- [ ] Scoped list includes: Actions/AGENTS.md, LotAT/AGENTS.md, LotAT/contracts.md
- [ ] Scoped list includes: constants/lotat.md, constants/overlay-broker.md
- [ ] "Do Not Load" section present (keeps agent focused)
- [ ] `lotat-tech/role.md` references `actions-scope.md`
- [ ] role.md not otherwise changed
- [ ] `progress.md` updated
