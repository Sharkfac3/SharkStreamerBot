---
id: phase-1-chunk-3-commanders-contracts
type: refactor-prompt
phase: 1
chunk: 3
status: ready
---

# Phase 1 / Chunk 3 — Extract Commanders Contracts and Script Reference

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You reorganize agent scaffolding markdown files to improve navigability and reduce context load. You do not touch `.cs` runtime scripts, do not change Streamer.bot behavior, and do not alter what information exists — only where it lives.

## Repository Context

SharkStreamerBot is a live-stream automation project. `Actions/Commanders/` governs a 3-slot commander role system (Captain Stretch, The Director, Water Wizard). Its `AGENTS.md` is 312 lines containing both action contract JSON (~lines 96–159) and a long Script Reference section (~lines 175–312, 137 lines). Both are heavy and rarely needed together.

## Prerequisite

Check `Projects/actions-scaffolding-refactor/progress.md`. Chunks 1 and 2 must show `done`. If not, stop.

## What This Chunk Does

Two extractions from `Actions/Commanders/AGENTS.md`:

1. Move `## Action Contracts` block → `Actions/Commanders/contracts.md`
2. Move `## Script Reference` section → `Actions/Commanders/script-reference.md`
3. Update AGENTS.md with references to both new files

**Files touched:**
- `Actions/Commanders/AGENTS.md` — edited
- `Actions/Commanders/contracts.md` — created
- `Actions/Commanders/script-reference.md` — created

## What This Chunk Does NOT Do

- Touch the per-commander subfolders (Captain Stretch, The Director, Water Wizard) — each has its own AGENTS.md that is not touched here
- Touch any `.cs` files
- Change any contract content or script reference content — move only
- Touch any other folder

## The streamerbot-dev Nav Contract

After this chunk, `streamerbot-dev` must still be able to:

1. Open `Actions/Commanders/AGENTS.md` and find purpose, ownership, local workflow, SHARED-STATE, and subfolder routing
2. Follow a reference to `contracts.md` for commander script contracts
3. Follow a reference to `script-reference.md` for detailed script documentation
4. Find all 3 commander subfolder AGENTS.md files listed (those are untouched)

## Files to Read Before Starting

1. `Actions/Commanders/AGENTS.md` — understand full structure, identify exact section boundaries
2. `Projects/actions-scaffolding-refactor/progress.md` — confirm chunks 1–2 done, chunk 3 pending
3. `Projects/actions-scaffolding-refactor/AGENTS.md` — scaffolding builder constraints

## Step-by-Step Instructions

1. Read `Actions/Commanders/AGENTS.md` in full. Note these section boundaries:
   - `## Action Contracts` heading at line 96, contract block from `<!-- ACTION-CONTRACTS:START -->` (line 98) to `<!-- ACTION-CONTRACTS:END -->` (line 159)
   - `## Validation` section at line 161 — this STAYS in AGENTS.md
   - `## Boundaries / Out of Scope` section at line 165 — this STAYS in AGENTS.md
   - `## Handoff Notes` section at line 169 — this STAYS in AGENTS.md
   - `## Script Reference` section starting at line 175 — this MOVES to script-reference.md

2. Create `Actions/Commanders/contracts.md`:

```markdown
---
id: commanders-contracts
type: contracts
description: Action contracts for Commanders scripts.
owner: streamerbot-dev
secondaryOwners: [brand-steward]
parent: AGENTS.md
---

# Commanders — Action Contracts

These contracts are maintained by `streamerbot-dev`. Load this file when validating or updating a script contract. Do not edit contracts without also updating the SHA256 stamp in the corresponding `.cs` file.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
[paste the full JSON block here exactly as it appears in AGENTS.md]
<!-- ACTION-CONTRACTS:END -->
```

   **Important:** Preserve both HTML comment markers. The validation tool uses them to locate the contract block.

3. Create `Actions/Commanders/script-reference.md`:

```markdown
---
id: commanders-script-reference
type: reference
description: Detailed per-script documentation for Commanders scripts.
owner: streamerbot-dev
parent: AGENTS.md
---

# Commanders — Script Reference

[paste the full ## Script Reference content here, from line 175 to end of file]
```

4. Edit `Actions/Commanders/AGENTS.md` with TWO precise removals:

   **Removal 1:** Delete only the `## Action Contracts` heading and the contract block — from the `## Action Contracts` line through `<!-- ACTION-CONTRACTS:END -->` (lines 96–159). The `## Validation`, `## Boundaries / Out of Scope`, and `## Handoff Notes` sections that follow (lines 161–172) MUST remain in AGENTS.md.

   **Removal 2:** Delete the entire `## Script Reference` section (line 175 to end of file).

   **Addition:** After the Handoff Notes section, append:

```markdown
## Action Contracts

Contracts for all Commanders scripts live in [contracts.md](contracts.md). Load it when validating or updating a script contract.

## Script Reference

Detailed per-script documentation lives in [script-reference.md](script-reference.md). Load it when implementing or reviewing a specific commander script.
```

5. Verify all three files are correct.

6. Update `Projects/actions-scaffolding-refactor/progress.md`: chunk 3 → `done`, update "Last updated".

## Output Requirements

- `contracts.md` exists with commander script contracts
- `script-reference.md` exists with full Script Reference content
- `AGENTS.md` has no JSON contracts or Script Reference content inline
- `AGENTS.md` references both new files
- Core AGENTS.md sections intact: Purpose, When to Activate, Primary Owner, Secondary Owners, Required Reading, Local Workflow, SHARED-STATE, Validation, Boundaries / Out of Scope, Handoff Notes
- `progress.md` chunk 3 shows `done`

## Validation Checklist

- [ ] `contracts.md` created with contract JSON
- [ ] `script-reference.md` created with script reference content
- [ ] `AGENTS.md` references both files
- [ ] `AGENTS.md` SHARED-STATE section (commander slots, tenure counters) still present
- [ ] `AGENTS.md` Local Workflow rules still present
- [ ] `AGENTS.md` no raw JSON blocks remain
- [ ] `progress.md` updated
