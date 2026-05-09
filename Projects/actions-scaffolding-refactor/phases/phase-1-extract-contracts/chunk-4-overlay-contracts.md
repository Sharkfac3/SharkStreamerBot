---
id: phase-1-chunk-4-overlay-contracts
type: refactor-prompt
phase: 1
chunk: 4
status: ready
---

# Phase 1 / Chunk 4 — Extract Overlay Action Contracts

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You reorganize agent scaffolding markdown files to improve navigability and reduce context load. You do not touch `.cs` runtime scripts, do not change Streamer.bot behavior, and do not alter what information exists — only where it lives.

## Repository Context

SharkStreamerBot is a live-stream automation project. `Actions/Overlay/` contains the Streamer.bot → WebSocket broker bridge scripts. Its `AGENTS.md` is 150 lines, of which ~87 lines are action contract JSON for 4 scripts. Small but worth standardizing — all domain AGENTS.md files should follow the same pattern after Phase 1.

## Prerequisite

Check `Projects/actions-scaffolding-refactor/progress.md`. Chunks 1, 2, and 3 must show `done`. If not, stop.

## What This Chunk Does

Extract the `ACTION-CONTRACTS` block from `Actions/Overlay/AGENTS.md` into `Actions/Overlay/contracts.md`. Update AGENTS.md to reference it.

**Files touched:**
- `Actions/Overlay/AGENTS.md` — edited
- `Actions/Overlay/contracts.md` — created

## What This Chunk Does NOT Do

- Touch any `.cs` files (broker-connect.cs, broker-disconnect.cs, broker-publish.cs, test-overlay.cs)
- Change any contract content
- Touch `Actions/Helpers/overlay-broker.md`
- Touch any other folder

## The streamerbot-dev Nav Contract

After this chunk:

1. `Actions/Overlay/AGENTS.md` still has: Purpose, When to Activate, Ownership, Required Reading, Folder-Specific Rules, Script Summary
2. AGENTS.md clearly references `contracts.md` for contract details
3. `contracts.md` contains all 4 overlay script contracts intact

## Files to Read Before Starting

1. `Actions/Overlay/AGENTS.md` — understand full structure
2. `Projects/actions-scaffolding-refactor/progress.md` — confirm chunks 1–3 done, chunk 4 pending
3. `Projects/actions-scaffolding-refactor/AGENTS.md` — scaffolding builder constraints

## Step-by-Step Instructions

1. Read `Actions/Overlay/AGENTS.md` in full.

2. Locate the `## Action Contracts` heading and the contract block delimited by:
   - Start: `<!-- ACTION-CONTRACTS:START -->`
   - End: `<!-- ACTION-CONTRACTS:END -->`

   The file ends immediately after `<!-- ACTION-CONTRACTS:END -->`.

3. Create `Actions/Overlay/contracts.md`:

```markdown
---
id: overlay-contracts
type: contracts
description: Action contracts for Overlay WebSocket broker scripts.
owner: streamerbot-dev
secondaryOwners: [app-dev]
parent: AGENTS.md
---

# Overlay — Action Contracts

These contracts are maintained by `streamerbot-dev`. Load this file when validating or updating a script contract. Do not edit contracts without also updating the SHA256 stamp in the corresponding `.cs` file.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
[paste the full JSON block here exactly as it appears in AGENTS.md]
<!-- ACTION-CONTRACTS:END -->
```

   **Important:** Preserve both HTML comment markers. The validation tool uses them to locate the contract block.

4. Edit `Actions/Overlay/AGENTS.md`:
   - Delete the `## Action Contracts` heading and everything from `<!-- ACTION-CONTRACTS:START -->` to `<!-- ACTION-CONTRACTS:END -->` inclusive
   - Add at the end:

```markdown
## Action Contracts

Contracts for all Overlay scripts live in [contracts.md](contracts.md). Load it when validating or updating a script contract.
```

5. Verify both files.

6. Update `Projects/actions-scaffolding-refactor/progress.md`: chunk 4 → `done`, update "Last updated".

## Output Requirements

- `Actions/Overlay/contracts.md` exists with all 4 overlay script contracts
- `Actions/Overlay/AGENTS.md` has no JSON contract blocks
- `Actions/Overlay/AGENTS.md` references `contracts.md`
- All other AGENTS.md sections intact: Purpose, When to Activate, Ownership, Required Reading, Folder-Specific Rules, Script Summary
- `progress.md` chunk 4 shows `done`

## Validation Checklist

- [ ] `contracts.md` exists with 4 contracts (broker-connect, broker-disconnect, broker-publish, test-overlay)
- [ ] `AGENTS.md` references `contracts.md`
- [ ] `AGENTS.md` Script Summary table still present
- [ ] `AGENTS.md` Folder-Specific Rules still present
- [ ] `AGENTS.md` no JSON blocks remaining
- [ ] `progress.md` updated
