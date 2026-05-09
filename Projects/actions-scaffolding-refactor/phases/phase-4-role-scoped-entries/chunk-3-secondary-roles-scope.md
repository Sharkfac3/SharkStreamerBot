---
id: phase-4-chunk-3-secondary-roles-scope
type: refactor-prompt
phase: 4
chunk: 3
status: ready
---

# Phase 4 / Chunk 3 — Add Secondary Role Scopes (brand-steward, ops)

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You create and update agent scaffolding markdown files to improve navigability. You do not touch `.cs` runtime scripts and do not alter runtime behavior.

## Repository Context

`brand-steward` and `ops` are secondary owners in `Actions/`. They don't write scripts but they review and validate specific parts:

- `brand-steward`: reviews public chat output, TTS, command names, reward text in Actions scripts
- `ops`: runs validation, maintains sync workflow, updates routing docs

Neither has a scoped Actions reading list. Without one, they load the full domain scaffolding to find the narrow slice they need.

This is the final chunk of the project.

## Prerequisite

Phase 4 Chunks 1 and 2 must show `done`. Chunk 3 must show `pending`.

## What This Chunk Does

Create `actions-scope.md` for both roles:
- `.agents/roles/brand-steward/actions-scope.md`
- `.agents/roles/ops/actions-scope.md`

Optionally update each role.md to reference its scope file.

**Files touched:**
- `.agents/roles/brand-steward/actions-scope.md` — created
- `.agents/roles/brand-steward/role.md` — updated (add reference if beneficial)
- `.agents/roles/ops/actions-scope.md` — created
- `.agents/roles/ops/role.md` — updated (add reference if beneficial)

## What This Chunk Does NOT Do

- Change what either role owns or is responsible for
- Touch any Actions files
- Touch streamerbot-dev or lotat-tech role folders (done in chunks 1–2)

## Files to Read Before Starting

1. `.agents/roles/brand-steward/role.md`
2. `.agents/roles/ops/role.md`
3. `Actions/AGENTS.md`
4. `Actions/OWNERSHIP.md`
5. `Actions/RULES.md`
6. `Actions/CONTRACT-SCHEMA.md`
7. `Projects/actions-scaffolding-refactor/progress.md` — confirm chunks 1–2 done, chunk 3 pending

## Step-by-Step Instructions

### brand-steward scope

1. Read `.agents/roles/brand-steward/role.md`. Understand what it reviews in Actions.

2. Create `.agents/roles/brand-steward/actions-scope.md`:

```markdown
---
id: brand-steward-actions-scope
type: scope
description: Scoped reading list for brand-steward reviewing Actions scripts.
owner: brand-steward
parent: role.md
---

# brand-steward — Actions Scope

brand-steward reviews Actions scripts for public-facing copy, tone, and naming — not for C# correctness. Load only what you need for review.

## For Any Actions Review

1. [Actions/AGENTS.md](../../../Actions/AGENTS.md) — domain entry
2. [Actions/OWNERSHIP.md](../../../Actions/OWNERSHIP.md) — understand what brand-steward reviews vs. what streamerbot-dev owns
3. The local `AGENTS.md` for the folder being reviewed — check the "Secondary Owners" section for brand-steward responsibilities

## What brand-steward Reviews in Actions

- Public chat output text (CPH.SendMessage calls, formatted strings)
- TTS content
- Reward/command names (channel point names, !command names)
- Commander character copy (Captain Stretch, The Director, Water Wizard)

## What brand-steward Does NOT Review

- C# logic, globals, timer names, OBS source names
- Contract JSON structure or SHA256 stamps
- Trigger configuration

## Domain Rules for Review Work

See [Actions/RULES.md](../../../Actions/RULES.md) — specifically the rules about public chat formatting and @mention conventions.
```

3. Add a reference in `.agents/roles/brand-steward/role.md` if it has a `## Common Routes` or Actions-relevant section:

```markdown
For Actions review work, see [actions-scope.md](actions-scope.md).
```

### ops scope

4. Read `.agents/roles/ops/role.md`. Understand what ops does in Actions.

5. Create `.agents/roles/ops/actions-scope.md`:

```markdown
---
id: ops-actions-scope
type: scope
description: Scoped reading list for ops validating and maintaining Actions scaffolding.
owner: ops
parent: role.md
---

# ops — Actions Scope

ops handles validation, sync workflow, and routing doc maintenance in Actions/. Load in this order for Actions work.

## Validation Run

1. [Actions/AGENTS.md](../../../Actions/AGENTS.md) — domain entry
2. [Actions/CONTRACT-SCHEMA.md](../../../Actions/CONTRACT-SCHEMA.md) — contract format spec
3. The local `contracts.md` for the folder being validated
4. Run: `python3 Tools/StreamerBot/Validation/action_contracts.py --changed`

## Routing / Scaffolding Maintenance

1. [Actions/AGENTS.md](../../../Actions/AGENTS.md) — master router
2. [Actions/RULES.md](../../../Actions/RULES.md) — domain rules
3. [Actions/OWNERSHIP.md](../../../Actions/OWNERSHIP.md) — role matrix
4. The local `AGENTS.md` for the folder being updated

## Sync / Paste Workflow

Follow [.agents/workflows/sync.md](../../../.agents/workflows/sync.md).
After changes: [.agents/workflows/change-summary.md](../../../.agents/workflows/change-summary.md).

## Constants Index

[Actions/SHARED-CONSTANTS.md](../../../Actions/SHARED-CONSTANTS.md) — index of all constants files.
```

6. Add a reference in `.agents/roles/ops/role.md` under `## Common Routes` or equivalent:

```markdown
For Actions validation and maintenance, see [actions-scope.md](actions-scope.md).
```

7. Verify all 4 files (2 scope files + 2 role.md updates).

8. Update `progress.md`: Phase 4 chunk 3 → `done`, update "Last updated". Add note: "Phase 4 complete — project done. All scaffolding refactor chunks complete."

## Output Requirements

- `.agents/roles/brand-steward/actions-scope.md` exists with review-focused reading list
- `.agents/roles/ops/actions-scope.md` exists with validation/maintenance reading list
- Both role.md files reference their scope file
- `progress.md` chunk 3 shows `done`, project completion note added

## Validation Checklist

- [ ] `brand-steward/actions-scope.md` created
- [ ] `ops/actions-scope.md` created
- [ ] `brand-steward/role.md` references `actions-scope.md`
- [ ] `ops/role.md` references `actions-scope.md`
- [ ] brand-steward scope includes "What brand-steward Does NOT Review" (keeps it focused)
- [ ] ops scope covers: validation run, routing maintenance, sync workflow, constants index
- [ ] `progress.md` updated with project completion note

## Project Complete

When this chunk is done, the actions-scaffolding-refactor project is complete. The Actions/ scaffolding now has:

- Lean domain AGENTS.md files (~60–100 lines each) with contracts in sibling files
- A lean master Actions/AGENTS.md (~50–80 lines) pointing to RULES.md, OWNERSHIP.md, CONTRACT-SCHEMA.md
- A clean SHARED-CONSTANTS.md index pointing to 6 domain-scoped constants files
- Role-scoped Actions reading lists for lotat-tech, streamerbot-dev, brand-steward, and ops
