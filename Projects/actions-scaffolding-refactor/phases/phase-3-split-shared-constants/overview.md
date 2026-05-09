---
id: phase-3-split-shared-constants-overview
type: phase-overview
phase: 3
status: active
---

# Phase 3 — Split SHARED-CONSTANTS.md

## Problem

`Actions/SHARED-CONSTANTS.md` is 511 lines covering 18 unrelated domain sections. Every agent that touches any constant-referencing script loads the entire file — including sections for domains it will never touch.

Example: an agent fixing a Duck mini-game script loads XJ Drivethrough constants, Destroyer state, Info Service paths, and Commander tenure counters — none of which are relevant.

## Solution

Split into domain-scoped constant files under `Actions/constants/`. The original `SHARED-CONSTANTS.md` becomes a lightweight index + operator sync notes.

After this phase:

```
Actions/
├── SHARED-CONSTANTS.md          ← index only + operator sync notes
└── constants/
    ├── stream-core.md           ← OBS scenes, stream mode, Disco Party
    ├── mini-games.md            ← lock state, Duck, Clone Empire, Pedro, Toothless
    ← commanders.md              ← commander slots, tenure, high-score tracking
    ├── overlay-broker.md        ← broker connection state, topic strings
    ├── lotat.md                 ← LotAT/offering globals
    └── effects.md               ← Rest/Focus loop, Bits/unlock pacing, XJ Drivethrough, Destroyer, Info Service
```

## Cross-Reference Update Rule

Any AGENTS.md that currently says "see SHARED-CONSTANTS.md" must be updated to point to the specific constants file it actually needs. This update happens within the chunk that creates that constants file.

`SHARED-CONSTANTS.md` stays as a file — it becomes the index. Any agent that loads it gets the full map to all constants files.

## Chunks

| Chunk | Creates | Sections Moved |
|---|---|---|
| 1 | constants/stream-core.md | OBS, Stream Mode, Disco Party |
| 2 | constants/mini-games.md | Mini-game Lock, Duck, Clone Empire, Pedro, Toothless |
| 3 | constants/commanders.md | Commanders (slots, tenure, high-scores) |
| 4 | constants/overlay-broker.md | Overlay/Broker topics and connection state |
| 5 | constants/lotat.md | LotAT/Offering globals |
| 6 | constants/effects.md | Rest/Focus Loop, Bits, Mix It Up pacing, XJ Drivethrough, Destroyer, Info Service, Temporary |

After chunk 6, `SHARED-CONSTANTS.md` is updated to be an index pointing to all 6 files plus operator sync notes.

Chunk 7 then audits all AGENTS.md files that reference `SHARED-CONSTANTS.md` and updates domain-specific references to point directly to the relevant `constants/<domain>.md` file.

## Phase Gate

Phase 4 may begin once all 7 chunks show `done` in `progress.md`.
