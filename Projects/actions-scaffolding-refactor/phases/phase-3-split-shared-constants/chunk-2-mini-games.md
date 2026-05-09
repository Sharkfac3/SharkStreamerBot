---
id: phase-3-chunk-2-mini-games
type: refactor-prompt
phase: 3
chunk: 2
status: ready
---

# Phase 3 / Chunk 2 — Extract Mini-Game Constants

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You reorganize agent scaffolding markdown files to improve navigability and reduce context load. You do not touch `.cs` runtime scripts and do not alter what information exists — only where it lives.

## Repository Context

`Actions/SHARED-CONSTANTS.md` is being split into domain-scoped files under `Actions/constants/`. This chunk extracts all mini-game constants (lock state, Duck, Clone Empire, Pedro, Toothless) — the largest single group at ~120 lines.

## Prerequisite

Phase 3 Chunk 1 must show `done` in `progress.md`. Chunk 2 must show `pending`.

## What This Chunk Does

Extract mini-game sections from `Actions/SHARED-CONSTANTS.md` into `Actions/constants/mini-games.md`.

**Sections to extract:**
- `## Mini-game Lock` — minigame_active, minigame_name globals
- `## Duck` — Duck game state
- `## Clone Grid Game` — Clone Empire state, empire_players_json, empire_cells_json
- `## Pedro` — Pedro game state
- `## Toothless` — Toothless game state

**Files touched:**
- `Actions/SHARED-CONSTANTS.md` — sections replaced with index entry
- `Actions/constants/mini-games.md` — created

## What This Chunk Does NOT Do

- Touch chunk 1's stream-core.md or its SHARED-CONSTANTS.md index entry
- Touch any other SHARED-CONSTANTS.md sections
- Touch any AGENTS.md files
- Touch any `.cs` files

## The streamerbot-dev Nav Contract

After this chunk, agents working on Squad scripts can follow SHARED-CONSTANTS.md index → `constants/mini-games.md` instead of loading all 511 lines.

## Files to Read Before Starting

1. `Actions/SHARED-CONSTANTS.md` — current state (after chunk 1 edits)
2. `Projects/actions-scaffolding-refactor/progress.md` — confirm chunk 1 done, chunk 2 pending

## Step-by-Step Instructions

1. Read `Actions/SHARED-CONSTANTS.md`. Identify the exact content for:
   - `## Mini-game Lock` section
   - `## Duck` section
   - `## Clone Grid Game` section (includes JSON structure examples)
   - `## Pedro` section
   - `## Toothless` section

2. Create `Actions/constants/mini-games.md`:

```markdown
---
id: constants-mini-games
type: constants
description: Mini-game lock state and per-game globals for Duck, Clone Empire, Pedro, and Toothless.
owner: streamerbot-dev
secondaryOwners: [app-dev]
parent: ../SHARED-CONSTANTS.md
---

# Mini-Game Constants

Canonical globals and state definitions for all Squad mini-games. The mini-game lock must be acquired before any game starts and released on all terminal paths.

[paste ## Mini-game Lock here]

[paste ## Duck here]

[paste ## Clone Grid Game here]

[paste ## Pedro here]

[paste ## Toothless here]
```

3. Edit `Actions/SHARED-CONSTANTS.md`:
   - Replace the 5 mini-game sections with a single index entry:

```markdown
## Mini-Games

Mini-game lock state and per-game globals (Duck, Clone Empire, Pedro, Toothless) → [constants/mini-games.md](constants/mini-games.md)
```

4. Verify both files.

5. Update `progress.md`: Phase 3 chunk 2 → `done`, update "Last updated".

## Output Requirements

- `constants/mini-games.md` exists with all 5 mini-game sections (~120 lines) verbatim
- `SHARED-CONSTANTS.md` has Mini-Games index entry, no inline mini-game sections
- All other sections unchanged
- `progress.md` chunk 2 shows `done`

## Validation Checklist

- [ ] `constants/mini-games.md` created with Mini-game Lock, Duck, Clone Grid Game, Pedro, Toothless
- [ ] Clone Empire JSON structure examples (empire_players_json, empire_cells_json) present in mini-games.md
- [ ] `SHARED-CONSTANTS.md` Mini-Games index entry present
- [ ] `SHARED-CONSTANTS.md` Stream Core index entry from chunk 1 still intact
- [ ] `progress.md` updated
