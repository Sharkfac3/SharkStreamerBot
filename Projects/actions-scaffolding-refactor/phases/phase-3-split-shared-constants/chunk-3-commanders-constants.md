---
id: phase-3-chunk-3-commanders-constants
type: refactor-prompt
phase: 3
chunk: 3
status: ready
---

# Phase 3 / Chunk 3 — Extract Commander Constants

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You reorganize agent scaffolding markdown files to improve navigability and reduce context load. You do not touch `.cs` runtime scripts and do not alter what information exists — only where it lives.

## Repository Context

`Actions/SHARED-CONSTANTS.md` is being split into domain-scoped files under `Actions/constants/`. This chunk extracts the Commanders section — globals for 3 commander slots, tenure counters, and high-score tracking.

## Prerequisite

Phase 3 Chunks 1 and 2 must show `done` in `progress.md`. Chunk 3 must show `pending`.

## What This Chunk Does

Extract `## Commanders` from `Actions/SHARED-CONSTANTS.md` into `Actions/constants/commanders.md`.

**Sections to extract:**
- `## Commanders` — current slots, tenure counters, high-score tracking for 3 commanders

**Files touched:**
- `Actions/SHARED-CONSTANTS.md` — section replaced with index entry
- `Actions/constants/commanders.md` — created

## What This Chunk Does NOT Do

- Touch `Actions/Commanders/AGENTS.md` or any subfolder files (those were handled in Phase 1)
- Note: `Actions/Commanders/AGENTS.md` has a SHARED-STATE section that mirrors some of this data. Do NOT merge or reconcile them — they serve different purposes. Leave both as-is.
- Touch any other SHARED-CONSTANTS.md sections or previous chunk index entries
- Touch any `.cs` files

## Files to Read Before Starting

1. `Actions/SHARED-CONSTANTS.md` — current state
2. `Projects/actions-scaffolding-refactor/progress.md` — confirm chunks 1–2 done, chunk 3 pending

## Step-by-Step Instructions

1. Read `Actions/SHARED-CONSTANTS.md`. Locate `## Commanders` section.

2. Create `Actions/constants/commanders.md`:

```markdown
---
id: constants-commanders
type: constants
description: Commander slot globals, tenure counters, and high-score tracking constants.
owner: streamerbot-dev
secondaryOwners: [brand-steward]
parent: ../SHARED-CONSTANTS.md
---

# Commander Constants

Canonical globals for the 3-slot commander system. These names are the source of truth for all commander scripts.

[paste ## Commanders section here]
```

3. Edit `Actions/SHARED-CONSTANTS.md`:
   - Replace `## Commanders` with:

```markdown
## Commanders

Commander slot globals, tenure counters, and high-score tracking → [constants/commanders.md](constants/commanders.md)
```

4. Verify both files.

5. Update `progress.md`: Phase 3 chunk 3 → `done`, update "Last updated".

## Output Requirements

- `constants/commanders.md` exists with full Commanders section verbatim
- `SHARED-CONSTANTS.md` has Commanders index entry
- Previous chunk index entries intact (Stream Core, Mini-Games)
- `progress.md` chunk 3 shows `done`

## Validation Checklist

- [ ] `constants/commanders.md` created
- [ ] Contains commander slot globals, tenure counters, high-score keys
- [ ] `SHARED-CONSTANTS.md` Commanders index entry present
- [ ] Previous index entries (Stream Core, Mini-Games) still intact
- [ ] `progress.md` updated
