---
id: phase-1-chunk-5-intros-contracts
type: refactor-prompt
phase: 1
chunk: 5
status: ready
---

# Phase 1 / Chunk 5 — Extract Intros Action Contracts

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You reorganize agent scaffolding markdown files to improve navigability and reduce context load. You do not touch `.cs` runtime scripts, do not change Streamer.bot behavior, and do not alter what information exists — only where it lives.

## Repository Context

SharkStreamerBot is a live-stream automation project. `Actions/Intros/` handles custom viewer intro capture and First-Chat playback. Its `AGENTS.md` is 151 lines, of which ~83 lines are action contract JSON for 2 scripts. This is the final chunk of Phase 1 — completing the contract extraction pattern across all domain folders.

## Prerequisite

Check `Projects/actions-scaffolding-refactor/progress.md`. Chunks 1 through 4 must all show `done`. If not, stop.

## What This Chunk Does

Extract the `ACTION-CONTRACTS` block from `Actions/Intros/AGENTS.md` into `Actions/Intros/contracts.md`. Update AGENTS.md to reference it.

**Files touched:**
- `Actions/Intros/AGENTS.md` — edited
- `Actions/Intros/contracts.md` — created

## What This Chunk Does NOT Do

- Touch any `.cs` files (first-chat-intro.cs, redeem-capture.cs)
- Change any contract content
- Touch any other folder

## The streamerbot-dev Nav Contract

After this chunk:

1. `Actions/Intros/AGENTS.md` still has: Purpose, Ownership, Required Reading, Local Workflow, Script Summary, Runtime Notes, Validation/Boundaries/Handoff
2. AGENTS.md clearly references `contracts.md`
3. `contracts.md` has both intro script contracts intact

## Files to Read Before Starting

1. `Actions/Intros/AGENTS.md` — understand full structure
2. `Projects/actions-scaffolding-refactor/progress.md` — confirm chunks 1–4 done, chunk 5 pending
3. `Projects/actions-scaffolding-refactor/AGENTS.md` — scaffolding builder constraints

## Step-by-Step Instructions

1. Read `Actions/Intros/AGENTS.md` in full.

2. Locate the `## Action Contracts` heading and the contract block delimited by:
   - Start: `<!-- ACTION-CONTRACTS:START -->`
   - End: `<!-- ACTION-CONTRACTS:END -->`

   **Important:** `## Runtime Notes` and `## Validation / Boundaries / Handoff` sections appear AFTER `<!-- ACTION-CONTRACTS:END -->`. Those sections stay in AGENTS.md — do not move them.

3. Create `Actions/Intros/contracts.md`:

```markdown
---
id: intros-contracts
type: contracts
description: Action contracts for Intros scripts.
owner: streamerbot-dev
secondaryOwners: [app-dev, brand-steward, ops]
parent: AGENTS.md
---

# Intros — Action Contracts

These contracts are maintained by `streamerbot-dev`. Load this file when validating or updating a script contract. Do not edit contracts without also updating the SHA256 stamp in the corresponding `.cs` file.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
[paste the full JSON block here exactly as it appears in AGENTS.md]
<!-- ACTION-CONTRACTS:END -->
```

   **Important:** Preserve both HTML comment markers. The validation tool uses them to locate the contract block.

4. Edit `Actions/Intros/AGENTS.md`:
   - Delete only the `## Action Contracts` heading and everything from `<!-- ACTION-CONTRACTS:START -->` to `<!-- ACTION-CONTRACTS:END -->` inclusive
   - The `## Runtime Notes` and `## Validation / Boundaries / Handoff` sections that follow must remain untouched
   - Replace the deleted block with:

```markdown
## Action Contracts

Contracts for all Intros scripts live in [contracts.md](contracts.md). Load it when validating or updating a script contract.
```

   - Ensure all sections after Action Contracts (Runtime Notes, Validation, Boundaries, Handoff) remain in AGENTS.md

5. Verify both files.

6. Update `Projects/actions-scaffolding-refactor/progress.md`: chunk 5 → `done`, update "Last updated".

   **Phase 1 is now complete.** Add a note in the Notes section of progress.md: "Phase 1 complete — all contract blocks extracted. Phase 2 may begin."

## Output Requirements

- `Actions/Intros/contracts.md` exists with both script contracts
- `Actions/Intros/AGENTS.md` has no JSON contract blocks
- `Actions/Intros/AGENTS.md` references `contracts.md`
- Runtime Notes, Validation, Boundaries, Handoff sections remain in AGENTS.md
- `progress.md` chunk 5 shows `done`, Phase 1 completion note added

## Validation Checklist

- [ ] `contracts.md` exists with 2 contracts (first-chat-intro, redeem-capture)
- [ ] `AGENTS.md` references `contracts.md`
- [ ] `AGENTS.md` Runtime Notes section still present
- [ ] `AGENTS.md` Script Summary table still present
- [ ] `AGENTS.md` no JSON blocks remaining
- [ ] `progress.md` updated with Phase 1 completion note

## Handoff to Phase 2

Phase 2 targets `Actions/AGENTS.md` (the master domain guide, 189 lines). It is independent of all Phase 1 work. No file from Phase 1 is a prerequisite for Phase 2 — only the completion status in `progress.md` gates it.
