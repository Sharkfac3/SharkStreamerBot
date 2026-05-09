---
id: phase-1-chunk-2-lotat-contracts
type: refactor-prompt
phase: 1
chunk: 2
status: ready
---

# Phase 1 / Chunk 2 — Extract LotAT Action Contracts

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You reorganize agent scaffolding markdown files to improve navigability and reduce context load. You do not touch `.cs` runtime scripts, do not change Streamer.bot behavior, and do not alter what information exists — only where it lives.

## Repository Context

SharkStreamerBot is a live-stream automation project. The `Actions/` folder contains Streamer.bot C# scripts and the agent scaffolding markdown that governs them. Each subfolder has an `AGENTS.md` file that serves as a domain guide for AI agents. These files contain routing info, ownership, rules, and embedded action contract JSON blocks.

`Actions/LotAT/` is the runtime engine for "Legends of the ASCII Temple" (LotAT), a live Twitch mini-RPG. Its `AGENTS.md` is 365 lines, of which ~255 lines are action contract JSON.

## Prerequisite

Check `Projects/actions-scaffolding-refactor/progress.md` before starting. Phase 1 Chunk 1 must show `done`. If it does not, stop and run chunk 1 first.

## What This Chunk Does

Extract the `ACTION-CONTRACTS` block from `Actions/LotAT/AGENTS.md` into a new sibling file `Actions/LotAT/contracts.md`. Update `Actions/LotAT/AGENTS.md` to reference the new file.

**Files touched:**
- `Actions/LotAT/AGENTS.md` — edited (contracts removed, reference added)
- `Actions/LotAT/contracts.md` — created (contracts moved here)

## What This Chunk Does NOT Do

- Touch any `.cs` files
- Change any contract content — move only
- Touch `Actions/LotAT/runtime-contract.md`, `implementation-map.md`, or `operator-setup.md`
- Touch any other folder's files
- Touch `Actions/AGENTS.md` or `Actions/SHARED-CONSTANTS.md`

## The streamerbot-dev / lotat-tech Nav Contract

`Actions/LotAT/AGENTS.md` is used by both `lotat-tech` (primary) and `streamerbot-dev` (secondary). After this chunk runs, both must still be able to:

1. Open `Actions/LotAT/AGENTS.md` and find purpose, ownership, required reading, runtime phase map, gotchas, and workflow
2. Follow a clear reference from AGENTS.md to `contracts.md` to load the 12 LotAT script contracts

## Files to Read Before Starting

1. `Actions/LotAT/AGENTS.md` — understand full current structure
2. `Projects/actions-scaffolding-refactor/progress.md` — confirm chunk 1 done, chunk 2 pending
3. `Projects/actions-scaffolding-refactor/AGENTS.md` — scaffolding builder constraints

## Step-by-Step Instructions

1. Read `Actions/LotAT/AGENTS.md` in full.

2. Locate the `## Action Contracts` section heading and the contract block delimited by:
   - Start: `<!-- ACTION-CONTRACTS:START -->`
   - End: `<!-- ACTION-CONTRACTS:END -->`

3. Copy the `## Action Contracts` heading, both markers, and the full JSON block between them.

4. Create `Actions/LotAT/contracts.md`:

```markdown
---
id: lotat-contracts
type: contracts
description: Action contracts for all LotAT runtime scripts.
owner: lotat-tech
secondaryOwners: [streamerbot-dev]
parent: AGENTS.md
---

# LotAT — Action Contracts

These contracts are maintained by `lotat-tech`. Load this file when validating or updating a script contract. Do not edit contracts without also updating the SHA256 stamp in the corresponding `.cs` file.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
[paste the full JSON block here exactly as it appears in AGENTS.md]
<!-- ACTION-CONTRACTS:END -->
```

   **Important:** Preserve both HTML comment markers. The validation tool uses them to locate the contract block.

5. Edit `Actions/LotAT/AGENTS.md`:
   - Delete the `## Action Contracts` heading and everything from `<!-- ACTION-CONTRACTS:START -->` to `<!-- ACTION-CONTRACTS:END -->` inclusive
   - Add at the end:

```markdown
## Action Contracts

Contracts for all LotAT scripts live in [contracts.md](contracts.md). Load it when validating or updating a script contract. Do not edit contracts without also updating the SHA256 stamp in the `.cs` file.
```

6. Verify both files look correct.

7. Update `Projects/actions-scaffolding-refactor/progress.md`: chunk 2 → `done`, set Completed to today's date. Also update "Last updated" at top of progress.md.

## Output Requirements

- `Actions/LotAT/contracts.md` exists with all 12 script contracts
- `Actions/LotAT/AGENTS.md` has no JSON contract blocks
- `Actions/LotAT/AGENTS.md` `## Action Contracts` section references `contracts.md`
- All other AGENTS.md sections intact: Purpose, Ownership, When to Activate, Required Reading, Local Workflow, Runtime Phase Map, LotAT-Specific Notes, Known Gotchas, Validation/Boundaries/Handoff
- `progress.md` chunk 2 shows `done`

## Validation Checklist

- [ ] `contracts.md` exists
- [ ] `contracts.md` has 13 LotAT script contracts (lotat-commander-input, lotat-commander-timeout, lotat-decision-input, lotat-decision-resolve, lotat-decision-timeout, lotat-dice-roll, lotat-dice-timeout, lotat-end-session, lotat-join-timeout, lotat-join, lotat-node-enter, lotat-start-main, overlay-publish)
- [ ] `AGENTS.md` Action Contracts section now points to `contracts.md`
- [ ] `AGENTS.md` has no JSON contract blocks remaining
- [ ] `AGENTS.md` Runtime Phase Map section intact
- [ ] `AGENTS.md` Known Gotchas section intact
- [ ] `progress.md` updated
