---
id: phase-3-chunk-1-stream-core
type: refactor-prompt
phase: 3
chunk: 1
status: ready
---

# Phase 3 / Chunk 1 — Extract Stream Core Constants

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You reorganize agent scaffolding markdown files to improve navigability and reduce context load. You do not touch `.cs` runtime scripts, do not change Streamer.bot behavior, and do not alter what information exists — only where it lives.

## Repository Context

`Actions/SHARED-CONSTANTS.md` is 511 lines covering 18 domain sections. Every agent that touches any script loads all 511 lines. This phase splits the file into domain-scoped files under `Actions/constants/`. This chunk extracts OBS scenes, stream mode, and Disco Party sections.

## Prerequisite

Check `Projects/actions-scaffolding-refactor/progress.md`. All Phase 1 and Phase 2 chunks must show `done`. Phase 3 Chunk 1 must show `pending`.

## What This Chunk Does

Extract the stream-core sections from `Actions/SHARED-CONSTANTS.md` into `Actions/constants/stream-core.md`.

**Sections to extract:**
- `## OBS` — scene names
- `## Stream Mode` — garage, workspace, gamer globals
- `## Disco Party` — active flag, previous scene

**Files touched:**
- `Actions/SHARED-CONSTANTS.md` — sections replaced with index entry
- `Actions/constants/stream-core.md` — created

## What This Chunk Does NOT Do

- Touch any other sections of SHARED-CONSTANTS.md
- Remove the Operator Sync Notes from SHARED-CONSTANTS.md
- Touch any AGENTS.md files (cross-reference updates happen in Phase 4)
- Touch any `.cs` files

## The streamerbot-dev Nav Contract

After this chunk:

1. `Actions/SHARED-CONSTANTS.md` still exists and now has an index entry pointing to `constants/stream-core.md` for the extracted sections
2. `Actions/constants/stream-core.md` contains the extracted content verbatim
3. Any agent that loaded SHARED-CONSTANTS.md before can now find stream-core constants via the index entry

## Files to Read Before Starting

1. `Actions/SHARED-CONSTANTS.md` — read in full, note exact section boundaries for OBS, Stream Mode, Disco Party
2. `Projects/actions-scaffolding-refactor/progress.md` — confirm prerequisites
3. `Projects/actions-scaffolding-refactor/AGENTS.md` — scaffolding builder constraints

## Step-by-Step Instructions

1. Read `Actions/SHARED-CONSTANTS.md` in full. Identify the exact content for:
   - `## OBS` section
   - `## Stream Mode` section
   - `## Disco Party` section

2. Create the `Actions/constants/` directory if it doesn't exist.

3. Create `Actions/constants/stream-core.md`:

```markdown
---
id: constants-stream-core
type: constants
description: OBS scene names, stream mode globals, and Disco Party state constants.
owner: streamerbot-dev
parent: ../SHARED-CONSTANTS.md
---

# Stream Core Constants

Canonical OBS scene names, stream mode globals, and Disco Party state for use across all Actions scripts. Names here are the source of truth — do not hardcode these strings in scripts.

[paste ## OBS section here]

[paste ## Stream Mode section here]

[paste ## Disco Party section here]
```

4. Edit `Actions/SHARED-CONSTANTS.md`:
   - Replace the `## OBS`, `## Stream Mode`, and `## Disco Party` sections with a single index entry:

```markdown
## Stream Core

OBS scene names, stream mode globals, and Disco Party state → [constants/stream-core.md](constants/stream-core.md)
```

5. Verify: SHARED-CONSTANTS.md is shorter. constants/stream-core.md has the full extracted content.

6. Update `progress.md`: Phase 3 chunk 1 → `done`, update "Last updated".

## Output Requirements

- `Actions/constants/stream-core.md` exists with OBS, Stream Mode, Disco Party sections verbatim
- `Actions/SHARED-CONSTANTS.md` has an index entry pointing to `constants/stream-core.md` in place of the three sections
- All other SHARED-CONSTANTS.md sections unchanged
- `progress.md` chunk 1 shows `done`

## Validation Checklist

- [ ] `constants/` directory created
- [ ] `constants/stream-core.md` created with 3 sections
- [ ] `SHARED-CONSTANTS.md` Stream Core index entry present
- [ ] `SHARED-CONSTANTS.md` OBS/Stream Mode/Disco Party sections no longer inline
- [ ] All other SHARED-CONSTANTS.md sections untouched
- [ ] `progress.md` updated
