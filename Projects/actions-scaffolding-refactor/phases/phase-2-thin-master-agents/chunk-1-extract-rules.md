---
id: phase-2-chunk-1-extract-rules
type: refactor-prompt
phase: 2
chunk: 1
status: ready
---

# Phase 2 / Chunk 1 — Extract Domain Rules from Master AGENTS.md

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You reorganize agent scaffolding markdown files to improve navigability and reduce context load. You do not touch `.cs` runtime scripts, do not change Streamer.bot behavior, and do not alter what information exists — only where it lives.

## Repository Context

`Actions/AGENTS.md` is the master domain guide for the entire `Actions/` folder. It is 189 lines mixing governance rules, ownership model, contract schema, and routing into one file. This phase splits it into focused sub-files. This chunk extracts the rules content.

## Prerequisite

Check `Projects/actions-scaffolding-refactor/progress.md`. All Phase 1 chunks (1–5) must show `done`. Phase 2 Chunk 1 must show `pending`.

## What This Chunk Does

Extract rules-related sections from `Actions/AGENTS.md` into a new file `Actions/RULES.md`. Update AGENTS.md to reference it.

**Files touched:**
- `Actions/AGENTS.md` — edited (rules sections removed, reference added)
- `Actions/RULES.md` — created

**Sections moving to RULES.md:**
- `## Domain Rules` (14 rules)
- `## Universal Script Rules`
- `## Boundaries` (if present)

## What This Chunk Does NOT Do

- Touch any subfolder AGENTS.md files
- Touch OWNERSHIP sections — those move in chunk 2
- Touch contract schema sections — those move in chunk 3
- Touch the Folder Routing table — stays in AGENTS.md
- Touch any `.cs` files

## The streamerbot-dev Nav Contract

After this chunk, `streamerbot-dev` must still find domain rules by following a reference from `Actions/AGENTS.md` to `Actions/RULES.md`. The rules content itself must be unchanged.

`Actions/AGENTS.md` must still have: Purpose, Start Here/Required Reading, Folder Routing table, Runtime Integration Map. These do not move in this chunk.

## Files to Read Before Starting

1. `Actions/AGENTS.md` — read in full, note exact section names and order
2. `Projects/actions-scaffolding-refactor/progress.md` — confirm prerequisites
3. `Projects/actions-scaffolding-refactor/AGENTS.md` — scaffolding builder constraints

## Step-by-Step Instructions

1. Read `Actions/AGENTS.md` in full. Identify:
   - The `## Domain Rules` section (14 numbered rules)
   - The `## Universal Script Rules` section
   - The `## Boundaries` section (if present — may be at end)
   - Everything else (Purpose, Start Here, Folder Routing, etc.) — these stay

2. Create `Actions/RULES.md`:

```markdown
---
id: actions-rules
type: rules
description: Domain rules and universal script rules for all Actions/ scripts.
owner: ops
secondaryOwners: [streamerbot-dev]
parent: AGENTS.md
---

# Actions — Domain Rules

These rules govern all agent and developer work under `Actions/`. They are enforced during validation and code review. When in doubt, these rules take precedence over convenience.

[paste ## Domain Rules section here]

[paste ## Universal Script Rules section here]

[paste ## Boundaries section here if present]
```

3. Edit `Actions/AGENTS.md`:
   - Remove the `## Domain Rules` section
   - Remove the `## Universal Script Rules` section
   - Remove the `## Boundaries` section if present
   - Add a reference after the Start Here section (or in a new `## Rules` section):

```markdown
## Rules

Domain rules and universal script rules for all work under `Actions/` live in [RULES.md](RULES.md). Read it before making any changes.
```

4. Verify: AGENTS.md should be noticeably shorter. RULES.md should contain all rule content verbatim.

5. Update `Projects/actions-scaffolding-refactor/progress.md`: Phase 2 chunk 1 → `done`, update "Last updated".

## Output Requirements

- `Actions/RULES.md` exists with all domain rules and universal script rules verbatim
- `Actions/AGENTS.md` references RULES.md
- `Actions/AGENTS.md` has no inline rule lists
- `Actions/AGENTS.md` Purpose, Start Here, Folder Routing, Runtime Integration Map still present
- `progress.md` Phase 2 chunk 1 shows `done`

## Validation Checklist

- [ ] `RULES.md` created
- [ ] `RULES.md` contains all 14 domain rules
- [ ] `RULES.md` contains Universal Script Rules
- [ ] `AGENTS.md` references `RULES.md`
- [ ] `AGENTS.md` Folder Routing table intact
- [ ] `AGENTS.md` no inline rule lists remaining
- [ ] `progress.md` updated
