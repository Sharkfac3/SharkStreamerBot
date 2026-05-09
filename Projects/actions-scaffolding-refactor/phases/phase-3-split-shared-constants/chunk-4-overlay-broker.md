---
id: phase-3-chunk-4-overlay-broker
type: refactor-prompt
phase: 3
chunk: 4
status: ready
---

# Phase 3 / Chunk 4 — Extract Overlay Broker Constants

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You reorganize agent scaffolding markdown files to improve navigability and reduce context load. You do not touch `.cs` runtime scripts and do not alter what information exists — only where it lives.

## Repository Context

`Actions/SHARED-CONSTANTS.md` is being split into domain-scoped files under `Actions/constants/`. This chunk extracts the Overlay/Broker section — WebSocket connection state, topic strings for overlay asset commands, LotAT session lifecycle, and Squad mini-game topics.

Note: `Actions/Helpers/overlay-broker.md` also exists as a helper pattern doc. That file is a C# usage guide, not a constants registry. Do not confuse them. This chunk only touches `SHARED-CONSTANTS.md`.

## Prerequisite

Phase 3 Chunks 1–3 must show `done` in `progress.md`. Chunk 4 must show `pending`.

## What This Chunk Does

Extract `## Overlay / Broker` from `Actions/SHARED-CONSTANTS.md` into `Actions/constants/overlay-broker.md`.

**Sections to extract:**
- `## Overlay / Broker` — connection state, topic strings for overlay commands, LotAT lifecycle topics, Squad mini-game topics

**Files touched:**
- `Actions/SHARED-CONSTANTS.md` — section replaced with index entry
- `Actions/constants/overlay-broker.md` — created

## What This Chunk Does NOT Do

- Touch `Actions/Helpers/overlay-broker.md` (that is a C# pattern guide, not a constants file)
- Touch `Actions/Overlay/AGENTS.md` or `contracts.md`
- Touch any other SHARED-CONSTANTS.md sections
- Touch any `.cs` files

## Files to Read Before Starting

1. `Actions/SHARED-CONSTANTS.md` — current state, locate `## Overlay / Broker` section
2. `Projects/actions-scaffolding-refactor/progress.md` — confirm chunks 1–3 done, chunk 4 pending

## Step-by-Step Instructions

1. Read `Actions/SHARED-CONSTANTS.md`. Locate `## Overlay / Broker` section in full.

2. Create `Actions/constants/overlay-broker.md`:

```markdown
---
id: constants-overlay-broker
type: constants
description: WebSocket broker connection state and overlay topic string constants.
owner: streamerbot-dev
secondaryOwners: [app-dev]
parent: ../SHARED-CONSTANTS.md
---

# Overlay / Broker Constants

Canonical broker connection state and topic strings. These are used by overlay publisher scripts and any script that sends events to the overlay. Topic strings here are the source of truth — do not hardcode them in scripts.

Note: for C# usage patterns (how to publish to the broker), see Actions/Helpers/overlay-broker.md.

[paste ## Overlay / Broker section here]
```

3. Edit `Actions/SHARED-CONSTANTS.md`:
   - Replace `## Overlay / Broker` with:

```markdown
## Overlay / Broker

WebSocket connection state and overlay topic strings → [constants/overlay-broker.md](constants/overlay-broker.md)

For C# usage patterns, see [Helpers/overlay-broker.md](Helpers/overlay-broker.md).
```

4. Verify both files.

5. Update `progress.md`: Phase 3 chunk 4 → `done`, update "Last updated".

## Output Requirements

- `constants/overlay-broker.md` exists with full Overlay/Broker section verbatim
- `SHARED-CONSTANTS.md` has Overlay/Broker index entry with note about Helpers file
- Previous index entries intact
- `progress.md` chunk 4 shows `done`

## Validation Checklist

- [ ] `constants/overlay-broker.md` created
- [ ] Contains connection state globals and topic string constants
- [ ] `SHARED-CONSTANTS.md` Overlay/Broker index entry present with Helpers cross-reference
- [ ] Previous index entries intact
- [ ] `progress.md` updated
