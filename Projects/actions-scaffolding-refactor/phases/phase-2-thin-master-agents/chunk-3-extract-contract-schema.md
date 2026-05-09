---
id: phase-2-chunk-3-extract-contract-schema
type: refactor-prompt
phase: 2
chunk: 3
status: ready
---

# Phase 2 / Chunk 3 — Extract Contract Schema from Master AGENTS.md

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You reorganize agent scaffolding markdown files to improve navigability and reduce context load. You do not touch `.cs` runtime scripts, do not change Streamer.bot behavior, and do not alter what information exists — only where it lives.

## Repository Context

`Actions/AGENTS.md` is the master domain guide for `Actions/`. After chunks 1 and 2, rules and ownership have moved to their own files. The remaining heavy content is the action contract schema — the JSON template defining what a valid contract looks like, plus validation and sync/handoff instructions. This is the final chunk of Phase 2.

## Prerequisite

Check `Projects/actions-scaffolding-refactor/progress.md`. Phase 2 Chunks 1 and 2 must show `done`. Chunk 3 must show `pending`.

## What This Chunk Does

Extract contract schema and validation content from `Actions/AGENTS.md` into `Actions/CONTRACT-SCHEMA.md`. Update AGENTS.md to reference it. After this chunk, AGENTS.md becomes a lean router.

**Files touched:**
- `Actions/AGENTS.md` — edited (final thinning)
- `Actions/CONTRACT-SCHEMA.md` — created

**Sections moving to CONTRACT-SCHEMA.md:**
- `## Action Contracts` (the schema/template section — not individual script contracts, those are in subfolder contracts.md files)
- `## Validation and Handoff`
- `## Sync and Handoff Expectations`
- Any contract field definitions or JSON template block

## What This Chunk Does NOT Do

- Touch any subfolder `contracts.md` files created in Phase 1
- Touch `RULES.md` or `OWNERSHIP.md` from chunks 1–2
- Touch the Folder Routing table — stays in AGENTS.md
- Touch any `.cs` files

## The streamerbot-dev Nav Contract

After this chunk:

1. `Actions/AGENTS.md` is a lean router: Purpose, Start Here/Required Reading chain, Folder Routing table, Runtime Integration Map, and references to RULES.md / OWNERSHIP.md / CONTRACT-SCHEMA.md
2. Any agent needing to write or validate a contract can follow the reference to `CONTRACT-SCHEMA.md`
3. All contract schema and validation instructions are in `CONTRACT-SCHEMA.md` verbatim

**Also update `.agents/roles/streamerbot-dev/role.md` if needed:** If the role file references `Actions/AGENTS.md` as the place to find contract schema or validation instructions, add a note pointing to `Actions/CONTRACT-SCHEMA.md`. The role file already references `Actions/SHARED-CONSTANTS.md` and `Actions/Helpers/AGENTS.md` explicitly — match that pattern.

## Files to Read Before Starting

1. `Actions/AGENTS.md` — current state after chunks 1 and 2
2. `.agents/roles/streamerbot-dev/role.md` — check whether it needs updating
3. `Projects/actions-scaffolding-refactor/progress.md` — confirm chunks 1–2 done, chunk 3 pending
4. `Projects/actions-scaffolding-refactor/AGENTS.md` — scaffolding builder constraints

## Step-by-Step Instructions

1. Read `Actions/AGENTS.md` in its current state. Identify the contract schema section and validation/sync sections.

2. Create `Actions/CONTRACT-SCHEMA.md`:

```markdown
---
id: actions-contract-schema
type: schema
description: Action contract format specification and validation instructions for Actions/ scripts.
owner: ops
secondaryOwners: [streamerbot-dev]
parent: AGENTS.md
---

# Actions — Contract Schema

This file defines the required format for action contracts across all `Actions/` scripts. Contracts live in each folder's `contracts.md` file. This file tells you what a valid contract looks like and how to validate one.

[paste ## Action Contracts schema/template section here]

[paste ## Validation and Handoff section here]

[paste ## Sync and Handoff Expectations section here]
```

3. Edit `Actions/AGENTS.md`:
   - Remove all sections moved to CONTRACT-SCHEMA.md
   - Add a reference:

```markdown
## Contract Schema and Validation

Contract format specification, field definitions, and validation instructions live in [CONTRACT-SCHEMA.md](CONTRACT-SCHEMA.md). Load it when writing or validating a script contract.
```

4. Review the final `Actions/AGENTS.md`. It should now be a lean router, roughly:
   - Purpose
   - Start Here / Required Reading chain (SHARED-CONSTANTS.md, Helpers/AGENTS.md, local AGENTS.md)
   - Rules reference → RULES.md
   - Ownership reference → OWNERSHIP.md
   - Contract schema reference → CONTRACT-SCHEMA.md
   - Folder Routing table
   - Runtime Integration Map

   If any other heavy sections remain, note them in the progress.md Notes section for future cleanup.

5. Check `.agents/roles/streamerbot-dev/role.md`. If it describes finding contract schema or validation in `Actions/AGENTS.md`, update the relevant line to reference `Actions/CONTRACT-SCHEMA.md` instead. Do not change any other part of the role file.

6. Update `progress.md`: Phase 2 chunk 3 → `done`, update "Last updated". Add note: "Phase 2 complete — Actions/AGENTS.md is now a lean router. Phase 3 may begin."

## Output Requirements

- `Actions/CONTRACT-SCHEMA.md` exists with full contract schema and validation instructions
- `Actions/AGENTS.md` is a lean router (~50–80 lines) with references to RULES.md, OWNERSHIP.md, CONTRACT-SCHEMA.md, and the Folder Routing table
- `.agents/roles/streamerbot-dev/role.md` updated if it referenced schema/validation location
- `progress.md` Phase 2 chunk 3 shows `done`, Phase 2 completion note added

## Validation Checklist

- [ ] `CONTRACT-SCHEMA.md` created with schema template + validation instructions
- [ ] `AGENTS.md` references `CONTRACT-SCHEMA.md`
- [ ] `AGENTS.md` is now a lean router (no inline rule lists, no inline ownership, no inline schema)
- [ ] `AGENTS.md` Folder Routing table still present
- [ ] `AGENTS.md` Start Here / Required Reading chain still present
- [ ] `streamerbot-dev/role.md` checked and updated if needed
- [ ] `progress.md` updated with Phase 2 completion note
