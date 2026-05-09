---
id: phase-1-chunk-9-xj-destroyer-channel-points-temporary-contracts
type: refactor-prompt
phase: 1
chunk: 9
status: ready
---

# Phase 1 / Chunk 9 — Extract XJ Drivethrough, Destroyer, Twitch Channel Points, and Temporary Contracts

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You reorganize agent scaffolding markdown files to improve navigability and reduce context load. You do not touch `.cs` runtime scripts and do not alter what information exists — only where it lives.

## Repository Context

This final Phase 1 chunk handles four small folders. Two are simple end-of-file patterns; two have sections after the contract block that must stay in AGENTS.md.

**XJ Drivethrough** — 1 contract. Has `## Runtime Notes` and `## Validation / Boundaries / Handoff` AFTER the contract block. Those sections stay in AGENTS.md.

**Destroyer** — 2 contracts. File ends at `<!-- ACTION-CONTRACTS:END -->`. Simple pattern.

**Twitch Channel Points** — 2 contracts. File ends at `<!-- ACTION-CONTRACTS:END -->`. Simple pattern.

**Temporary** — 2 contracts. File ends at `<!-- ACTION-CONTRACTS:END -->`. Simple pattern.

## Prerequisite

Chunk 8 must show `done` in `progress.md`. Chunk 9 must show `pending`.

## What This Chunk Does

For each of the 4 folders: extract ACTION-CONTRACTS block → `contracts.md`, update AGENTS.md reference.

**Files touched (8 total):**
- `Actions/XJ Drivethrough/AGENTS.md` — edited
- `Actions/XJ Drivethrough/contracts.md` — created
- `Actions/Destroyer/AGENTS.md` — edited
- `Actions/Destroyer/contracts.md` — created
- `Actions/Twitch Channel Points/AGENTS.md` — edited
- `Actions/Twitch Channel Points/contracts.md` — created
- `Actions/Temporary/AGENTS.md` — edited
- `Actions/Temporary/contracts.md` — created

## Files to Read Before Starting

1. `Actions/XJ Drivethrough/AGENTS.md`
2. `Actions/Destroyer/AGENTS.md`
3. `Actions/Twitch Channel Points/AGENTS.md`
4. `Actions/Temporary/AGENTS.md`
5. `Projects/actions-scaffolding-refactor/progress.md`

## Step-by-Step Instructions

### XJ Drivethrough (special: sections follow contracts)

1. Read `Actions/XJ Drivethrough/AGENTS.md`. Locate:
   - `<!-- ACTION-CONTRACTS:START -->` / `<!-- ACTION-CONTRACTS:END -->` block
   - `## Runtime Notes` section that follows — **stays in AGENTS.md**
   - `## Validation / Boundaries / Handoff` section that follows — **stays in AGENTS.md**

2. Create `Actions/XJ Drivethrough/contracts.md`:

```markdown
---
id: xj-drivethrough-contracts
type: contracts
description: Action contracts for XJ Drivethrough scripts.
owner: streamerbot-dev
parent: AGENTS.md
---

# XJ Drivethrough — Action Contracts

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
[paste full JSON block]
<!-- ACTION-CONTRACTS:END -->
```

3. Edit `Actions/XJ Drivethrough/AGENTS.md`:
   - Delete only `## Action Contracts` heading + `<!-- ACTION-CONTRACTS:START -->` through `<!-- ACTION-CONTRACTS:END -->`
   - The `## Runtime Notes` and `## Validation / Boundaries / Handoff` sections that follow MUST remain
   - Insert before Runtime Notes:

```markdown
## Action Contracts

Contracts for xj-drivethrough-main.cs live in [contracts.md](contracts.md).
```

### Destroyer (end-of-file pattern)

4. Read `Actions/Destroyer/AGENTS.md`. Create `contracts.md`, update AGENTS.md with reference. Same pattern as previous chunks.

```markdown
---
id: destroyer-contracts
type: contracts
description: Action contracts for Destroyer scripts.
owner: streamerbot-dev
parent: AGENTS.md
---

# Destroyer — Action Contracts

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
[paste full JSON block]
<!-- ACTION-CONTRACTS:END -->
```

AGENTS.md reference: `Contracts for destroyer-spawn.cs and destroyer-move.cs live in [contracts.md](contracts.md).`

### Twitch Channel Points (end-of-file pattern)

5. Same process. 2 contracts: disco-party.cs, explain-current-task.cs.

```markdown
---
id: twitch-channel-points-contracts
type: contracts
description: Action contracts for Twitch Channel Points scripts.
owner: streamerbot-dev
parent: AGENTS.md
---

# Twitch Channel Points — Action Contracts

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
[paste full JSON block]
<!-- ACTION-CONTRACTS:END -->
```

### Temporary (end-of-file pattern)

6. Same process. 2 contracts: temp-focus-timer-end.cs, temp-focus-timer-start.cs.

```markdown
---
id: temporary-contracts
type: contracts
description: Action contracts for Temporary scripts.
owner: streamerbot-dev
parent: AGENTS.md
---

# Temporary — Action Contracts

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
[paste full JSON block]
<!-- ACTION-CONTRACTS:END -->
```

7. Update `progress.md`: chunk 9 → `done`, update "Last updated".

   **Phase 1 is now complete.** Add a note in the Notes section: "Phase 1 complete — all contract blocks extracted from all 13 Action subfolders. Phase 2 may begin."

## Validation Checklist

- [ ] `XJ Drivethrough/contracts.md` created with 1 contract
- [ ] `XJ Drivethrough/AGENTS.md` Runtime Notes and Validation sections still present
- [ ] `Destroyer/contracts.md` created with 2 contracts (destroyer-move, destroyer-spawn)
- [ ] `Twitch Channel Points/contracts.md` created with 2 contracts (disco-party, explain-current-task)
- [ ] `Temporary/contracts.md` created with 2 contracts (temp-focus-timer-end, temp-focus-timer-start)
- [ ] All 4 `contracts.md` files have HTML markers
- [ ] All 4 `AGENTS.md` files reference `contracts.md`, no JSON remaining
- [ ] `progress.md` chunk 9 done, Phase 1 completion note added
