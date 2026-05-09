---
id: phase-1-chunk-1-squad-contracts
type: refactor-prompt
phase: 1
chunk: 1
status: ready
---

# Phase 1 / Chunk 1 — Extract Squad Action Contracts

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You reorganize agent scaffolding markdown files to improve navigability and reduce context load. You do not touch `.cs` runtime scripts, do not change Streamer.bot behavior, and do not alter what information exists — only where it lives.

## Repository Context

SharkStreamerBot is a live-stream automation project. The `Actions/` folder contains Streamer.bot C# scripts and the agent scaffolding markdown that governs them. Each subfolder has an `AGENTS.md` file that serves as a domain guide for AI agents working in that area. These files contain routing info, ownership, rules, and embedded action contract JSON blocks.

The problem: AGENTS.md files are bloated because action contracts are embedded directly in them. An agent opening `Squad/AGENTS.md` loads ~430 lines including ~330 lines of JSON contracts it usually doesn't need.

## What This Chunk Does

Extract the `ACTION-CONTRACTS` block from `Actions/Squad/AGENTS.md` into a new sibling file `Actions/Squad/contracts.md`. Update `Actions/Squad/AGENTS.md` to reference the new file.

**Files touched:**
- `Actions/Squad/AGENTS.md` — edited (contracts removed, reference added)
- `Actions/Squad/contracts.md` — created (contracts moved here)

## What This Chunk Does NOT Do

- Touch any `.cs` files
- Change any contract content — move only, no edits to contract data
- Touch any other AGENTS.md file
- Touch `Actions/AGENTS.md` (the master domain guide)
- Touch `Actions/SHARED-CONSTANTS.md`
- Touch anything outside `Actions/Squad/`

## The streamerbot-dev Nav Contract

`streamerbot-dev` is the primary role that navigates `Actions/Squad/AGENTS.md`. After this chunk runs, `streamerbot-dev` must still be able to:

1. Open `Actions/Squad/AGENTS.md` and find routing, ownership, workflow, and subfolder routing
2. Follow a clear reference from AGENTS.md to `contracts.md` to load contracts when needed
3. Find all 15+ squad script contracts in `contracts.md` intact and unmodified

Do not remove any section from AGENTS.md except the raw contract JSON block. All other sections stay.

## Files to Read Before Starting

Read these files in full before making any changes:

1. `Actions/Squad/AGENTS.md` — understand full current structure
2. `Projects/actions-scaffolding-refactor/progress.md` — confirm chunk 1 is `pending`
3. `Projects/actions-scaffolding-refactor/AGENTS.md` — understand scaffolding builder role and constraints

## Step-by-Step Instructions

1. Read `Actions/Squad/AGENTS.md` in full.

2. Locate the `## Action Contracts` section heading and the contract block. The block is delimited by HTML comment markers:
   - Start: `<!-- ACTION-CONTRACTS:START -->`
   - End: `<!-- ACTION-CONTRACTS:END -->`

3. Copy the `## Action Contracts` heading, the `<!-- ACTION-CONTRACTS:START -->` marker, the full JSON block, and the `<!-- ACTION-CONTRACTS:END -->` marker.

4. Create a new file `Actions/Squad/contracts.md` with the following structure:

```markdown
---
id: squad-contracts
type: contracts
description: Action contracts for all Squad mini-game scripts.
owner: streamerbot-dev
parent: AGENTS.md
---

# Squad — Action Contracts

These contracts are maintained by `streamerbot-dev`. Load this file when validating or updating a script contract. Do not edit contracts here without also updating the SHA256 stamp in the corresponding `.cs` file.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
[paste the full JSON block here exactly as it appears in AGENTS.md]
<!-- ACTION-CONTRACTS:END -->
```

   **Important:** Preserve both HTML comment markers in contracts.md. The validation tool (`action_contracts.py`) uses them to locate the contract block.

5. Edit `Actions/Squad/AGENTS.md`:
   - Delete the `## Action Contracts` heading and everything from `<!-- ACTION-CONTRACTS:START -->` to `<!-- ACTION-CONTRACTS:END -->` inclusive
   - Add the following section at the end of the file:

```markdown
## Action Contracts

Contracts for all 17 Squad scripts live in [contracts.md](contracts.md). Load it when validating or updating a script contract. Do not edit contracts without also updating the SHA256 stamp in the `.cs` file.
```

6. Verify: open both files. AGENTS.md should end with the reference section and contain no JSON. contracts.md should contain all 17 contract entries with both HTML markers.

7. Update `Projects/actions-scaffolding-refactor/progress.md`: change chunk 1 Status from `pending` to `done` and set Completed to today's date.

## Output Requirements

When done, the following must be true:

- `Actions/Squad/contracts.md` exists and contains the full original `## Action Contracts` content
- `Actions/Squad/AGENTS.md` no longer contains any action contract JSON
- `Actions/Squad/AGENTS.md` contains a `## Action Contracts` section that references `contracts.md`
- All other sections in `Actions/Squad/AGENTS.md` are unchanged
- `progress.md` chunk 1 shows `done`

## Validation Checklist

- [ ] `contracts.md` exists
- [ ] `contracts.md` contains all 17 squad script contracts (Clone ×5, Duck ×4, Pedro ×3, Toothless ×2, offering, squad-game-help, Duck/overlay-publish, Pedro/overlay-publish, Toothless/overlay-publish)
- [ ] `AGENTS.md` Action Contracts section now points to `contracts.md`
- [ ] `AGENTS.md` has no JSON contract blocks remaining
- [ ] `AGENTS.md` Purpose, Ownership, Required Reading, Subfolder Routing, Shared Scripts, Local Workflow, Squad-Specific Notes, Boundaries, Handoff sections all intact
- [ ] `progress.md` updated
