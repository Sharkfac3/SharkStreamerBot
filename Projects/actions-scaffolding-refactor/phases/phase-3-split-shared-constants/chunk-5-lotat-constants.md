---
id: phase-3-chunk-5-lotat-constants
type: refactor-prompt
phase: 3
chunk: 5
status: ready
---

# Phase 3 / Chunk 5 — Extract LotAT Constants

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You reorganize agent scaffolding markdown files to improve navigability and reduce context load. You do not touch `.cs` runtime scripts and do not alter what information exists — only where it lives.

## Repository Context

`Actions/SHARED-CONSTANTS.md` is being split into domain-scoped files under `Actions/constants/`. This chunk extracts LotAT and Offering globals — session state, roster, dice, phase tracking, and offering state used by the Legends of the ASCII Temple runtime.

## Prerequisite

Phase 3 Chunks 1–4 must show `done` in `progress.md`. Chunk 5 must show `pending`.

## What This Chunk Does

Extract `## LotAT / Offering` (or equivalent) from `Actions/SHARED-CONSTANTS.md` into `Actions/constants/lotat.md`.

**Sections to extract:**
- `## LotAT / Offering` — session state, roster, dice results, phase globals, offering state

**Files touched:**
- `Actions/SHARED-CONSTANTS.md` — section replaced with index entry
- `Actions/constants/lotat.md` — created

## What This Chunk Does NOT Do

- Touch `Actions/LotAT/AGENTS.md`, `contracts.md`, `runtime-contract.md`, or any LotAT subfolder files
- Touch any other SHARED-CONSTANTS.md sections
- Touch any `.cs` files

## The lotat-tech / streamerbot-dev Nav Contract

After this chunk, agents working on LotAT scripts can follow `SHARED-CONSTANTS.md` → `constants/lotat.md` instead of loading all 511 lines.

## Files to Read Before Starting

1. `Actions/SHARED-CONSTANTS.md` — current state, locate LotAT/Offering section
2. `Projects/actions-scaffolding-refactor/progress.md` — confirm chunks 1–4 done, chunk 5 pending

## Step-by-Step Instructions

1. Read `Actions/SHARED-CONSTANTS.md`. Locate the LotAT / Offering section (may be labeled `## LotAT`, `## LotAT / Offering`, or similar).

2. Create `Actions/constants/lotat.md`:

```markdown
---
id: constants-lotat
type: constants
description: LotAT session state, roster, dice, and offering globals.
owner: lotat-tech
secondaryOwners: [streamerbot-dev]
parent: ../SHARED-CONSTANTS.md
---

# LotAT Constants

Canonical globals for the Legends of the ASCII Temple runtime. These names are the source of truth for all LotAT C# scripts.

[paste ## LotAT / Offering section here]
```

3. Edit `Actions/SHARED-CONSTANTS.md`:
   - Replace the LotAT section with:

```markdown
## LotAT / Offering

Session state, roster, dice result, and offering globals → [constants/lotat.md](constants/lotat.md)
```

4. Verify both files.

5. Update `progress.md`: Phase 3 chunk 5 → `done`, update "Last updated".

## Output Requirements

- `constants/lotat.md` exists with full LotAT/Offering section verbatim
- `SHARED-CONSTANTS.md` has LotAT/Offering index entry
- Previous index entries intact
- `progress.md` chunk 5 shows `done`

## Validation Checklist

- [ ] `constants/lotat.md` created
- [ ] Contains LotAT session/roster/dice/offering globals
- [ ] `SHARED-CONSTANTS.md` LotAT index entry present
- [ ] Previous index entries intact
- [ ] `progress.md` updated
