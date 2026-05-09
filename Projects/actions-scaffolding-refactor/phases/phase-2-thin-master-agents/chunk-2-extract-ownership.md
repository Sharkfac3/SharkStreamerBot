---
id: phase-2-chunk-2-extract-ownership
type: refactor-prompt
phase: 2
chunk: 2
status: ready
---

# Phase 2 / Chunk 2 — Extract Ownership Model from Master AGENTS.md

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You reorganize agent scaffolding markdown files to improve navigability and reduce context load. You do not touch `.cs` runtime scripts, do not change Streamer.bot behavior, and do not alter what information exists — only where it lives.

## Repository Context

`Actions/AGENTS.md` is the master domain guide for `Actions/`. After Phase 2 Chunk 1, rules have been moved to `RULES.md`. The file still contains an ownership model section (role matrix, shared ownership rules, chain-to structure). This chunk extracts that.

## Prerequisite

Check `Projects/actions-scaffolding-refactor/progress.md`. Phase 2 Chunk 1 must show `done`. Phase 2 Chunk 2 must show `pending`.

## What This Chunk Does

Extract ownership-related sections from `Actions/AGENTS.md` into `Actions/OWNERSHIP.md`. Update AGENTS.md to reference it.

**Files touched:**
- `Actions/AGENTS.md` — edited
- `Actions/OWNERSHIP.md` — created

**Sections moving to OWNERSHIP.md:**
- `## Shared Ownership Rules` (or equivalent ownership section)
- Any role responsibility table or matrix
- Chain-to rules at the domain level

## What This Chunk Does NOT Do

- Touch RULES.md (created in chunk 1)
- Touch contract schema — that moves in chunk 3
- Touch the Folder Routing table — stays in AGENTS.md
- Touch any subfolder files
- Touch any `.cs` files

## The streamerbot-dev Nav Contract

After this chunk:

1. `Actions/AGENTS.md` still has Purpose, Start Here, Rules reference, Folder Routing, Runtime Integration Map
2. AGENTS.md clearly references `OWNERSHIP.md` for role matrix and ownership rules
3. `OWNERSHIP.md` contains complete ownership model — no information lost

## Files to Read Before Starting

1. `Actions/AGENTS.md` — current state after chunk 1 edits
2. `Projects/actions-scaffolding-refactor/progress.md` — confirm chunk 1 done, chunk 2 pending
3. `Projects/actions-scaffolding-refactor/AGENTS.md` — scaffolding builder constraints

## Step-by-Step Instructions

1. Read `Actions/AGENTS.md` in its current state (post-chunk-1). Identify:
   - The ownership/shared ownership section(s)
   - Any role responsibility tables
   - Any chain-to or secondary owner rules at the domain level

2. Create `Actions/OWNERSHIP.md`:

```markdown
---
id: actions-ownership
type: ownership
description: Role ownership model for all Actions/ scripts and scaffolding.
owner: ops
secondaryOwners: [streamerbot-dev, brand-steward]
parent: AGENTS.md
---

# Actions — Ownership Model

This file defines who owns what under `Actions/`. Per-folder ownership is defined in each folder's AGENTS.md. This file covers domain-level shared ownership rules.

[paste ## Shared Ownership Rules and all ownership content here]
```

3. Edit `Actions/AGENTS.md`:
   - Remove the ownership sections identified in step 1
   - Add a reference in the appropriate position:

```markdown
## Ownership

Domain-level ownership rules and role matrix live in [OWNERSHIP.md](OWNERSHIP.md). Per-folder ownership is in each folder's AGENTS.md.
```

4. Verify AGENTS.md is now shorter and clean. OWNERSHIP.md has all ownership content verbatim.

5. Update `progress.md`: Phase 2 chunk 2 → `done`, update "Last updated".

## Output Requirements

- `Actions/OWNERSHIP.md` exists with complete ownership model
- `Actions/AGENTS.md` references OWNERSHIP.md
- `Actions/AGENTS.md` has no inline role matrix or ownership rule lists
- `Actions/AGENTS.md` Folder Routing table still intact
- `progress.md` Phase 2 chunk 2 shows `done`

## Validation Checklist

- [ ] `OWNERSHIP.md` created with role ownership content
- [ ] `AGENTS.md` references `OWNERSHIP.md`
- [ ] `AGENTS.md` no inline ownership/role tables remaining
- [ ] `AGENTS.md` Folder Routing table intact
- [ ] `AGENTS.md` `RULES.md` reference from chunk 1 still intact
- [ ] `progress.md` updated
