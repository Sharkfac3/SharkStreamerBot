---
id: phase-3-chunk-6-effects
type: refactor-prompt
phase: 3
chunk: 6
status: ready
---

# Phase 3 / Chunk 6 — Extract Effects Constants and Finalize Index

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You reorganize agent scaffolding markdown files to improve navigability and reduce context load. You do not touch `.cs` runtime scripts and do not alter what information exists — only where it lives.

## Repository Context

This is the final chunk of Phase 3. It extracts the remaining SHARED-CONSTANTS.md sections (Rest/Focus Loop, Bits, Mix It Up pacing, XJ Drivethrough, Destroyer, Info Service, Temporary) into `Actions/constants/effects.md`. It then finalizes `SHARED-CONSTANTS.md` as a clean index with operator sync notes.

## Prerequisite

Phase 3 Chunks 1–5 must all show `done` in `progress.md`. Chunk 6 must show `pending`.

## What This Chunk Does

Extract remaining non-indexed sections from `Actions/SHARED-CONSTANTS.md` into `Actions/constants/effects.md`, then rewrite SHARED-CONSTANTS.md as a proper index.

**Sections to extract:**
- `## Rest / Focus Loop` — phase loop constants
- `## Temporary` — short-lived constants
- `## Bits` — cheer/bits tier constants
- `## Mix It Up Unlock Pacing` — unlock pacing constants
- `## XJ Drivethrough` — asset/sound IDs, display dimensions, movement parameters
- `## Destroyer` — position state, lifetime, movement step
- `## Info Service / Assets` — asset paths, port, collection names

**Files touched:**
- `Actions/SHARED-CONSTANTS.md` — remaining sections replaced with index entries, file restructured as index
- `Actions/constants/effects.md` — created

## What This Chunk Does NOT Do

- Touch any previous chunk's constants files
- Touch any AGENTS.md files
- Touch any `.cs` files
- Remove Operator Sync Notes — those stay in SHARED-CONSTANTS.md

## Files to Read Before Starting

1. `Actions/SHARED-CONSTANTS.md` — current state (5 sections already replaced with index entries by chunks 1–5)
2. `Projects/actions-scaffolding-refactor/progress.md` — confirm chunks 1–5 done, chunk 6 pending

## Step-by-Step Instructions

1. Read `Actions/SHARED-CONSTANTS.md` in full. Identify all remaining inline sections (not yet replaced with index entries).

2. Create `Actions/constants/effects.md`:

```markdown
---
id: constants-effects
type: constants
description: Rest/Focus loop, Bits/unlock pacing, XJ Drivethrough, Destroyer, Info Service, and Temporary constants.
owner: streamerbot-dev
parent: ../SHARED-CONSTANTS.md
---

# Effects Constants

Canonical constants for stream effects, overlays, and auxiliary systems.

[paste ## Rest / Focus Loop here]

[paste ## Temporary here]

[paste ## Bits here]

[paste ## Mix It Up Unlock Pacing here]

[paste ## XJ Drivethrough here]

[paste ## Destroyer here]

[paste ## Info Service / Assets here]
```

3. Replace the remaining inline sections of `Actions/SHARED-CONSTANTS.md` with index entries — **one section at a time, in order**. Do NOT rewrite the file from scratch. For each remaining section:
   - Find the section in the current file
   - Replace it with a single index entry line pointing to `constants/effects.md`
   - Preserve the Operator Sync Notes section exactly as-is

   After all replacements, the file should contain only:
   - The frontmatter (if any) or title
   - 5 existing index entries from chunks 1–5 (Stream Core, Mini-Games, Commanders, Overlay/Broker, LotAT/Offering)
   - 1 new Effects index entry
   - The original Operator Sync Notes section verbatim

   Add the Effects index entry:

```markdown
## Effects

Rest/Focus loop, Bits/unlock pacing, XJ Drivethrough, Destroyer, Info Service, Temporary → [constants/effects.md](constants/effects.md)
```

   Then add a top-level index table after the title/intro:

```markdown
## Index

| Domain | File | Contents |
|---|---|---|
| Stream Core | [constants/stream-core.md](constants/stream-core.md) | OBS scenes, stream mode, Disco Party |
| Mini-Games | [constants/mini-games.md](constants/mini-games.md) | Lock state, Duck, Clone Empire, Pedro, Toothless |
| Commanders | [constants/commanders.md](constants/commanders.md) | Commander slots, tenure, high-scores |
| Overlay / Broker | [constants/overlay-broker.md](constants/overlay-broker.md) | Connection state, topic strings |
| LotAT / Offering | [constants/lotat.md](constants/lotat.md) | Session state, roster, dice, offering |
| Effects | [constants/effects.md](constants/effects.md) | Rest/Focus loop, Bits, XJ Drivethrough, Destroyer, Info Service |
```

4. Verify by reading `SHARED-CONSTANTS.md` after edits: confirm no inline constant blocks remain, only index entries + the index table + Operator Sync Notes. All 6 constants files exist under `Actions/constants/`.

5. Update `progress.md`: Phase 3 chunk 6 → `done`, update "Last updated". Add note: "Phase 3 complete — SHARED-CONSTANTS.md is now a clean index. Phase 4 may begin."

## Output Requirements

- `constants/effects.md` exists with all 7 remaining sections verbatim
- `Actions/SHARED-CONSTANTS.md` is a clean index table (~30–40 lines) + Operator Sync Notes
- All 6 constants files exist under `Actions/constants/`
- `progress.md` chunk 6 shows `done`, Phase 3 completion note added

## Validation Checklist

- [ ] `constants/effects.md` created with Rest/Focus, Temporary, Bits, Mix It Up, XJ, Destroyer, Info Service
- [ ] `SHARED-CONSTANTS.md` is an index table — no inline constant blocks remain
- [ ] `SHARED-CONSTANTS.md` Operator Sync Notes preserved
- [ ] All 6 constants files exist: stream-core, mini-games, commanders, overlay-broker, lotat, effects
- [ ] `progress.md` updated with Phase 3 completion note

## Handoff to Phase 4

Phase 4 adds role-scoped entry points. It reads the new constants structure and domain AGENTS.md files to build scoped views for each role. No constants files are modified in Phase 4.
