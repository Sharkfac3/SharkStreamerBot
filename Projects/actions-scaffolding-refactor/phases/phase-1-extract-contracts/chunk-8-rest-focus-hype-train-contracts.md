---
id: phase-1-chunk-8-rest-focus-hype-train-contracts
type: refactor-prompt
phase: 1
chunk: 8
status: ready
---

# Phase 1 / Chunk 8 — Extract Rest Focus Loop and Twitch Hype Train Contracts

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You reorganize agent scaffolding markdown files to improve navigability and reduce context load. You do not touch `.cs` runtime scripts and do not alter what information exists — only where it lives.

## Repository Context

Two medium-small folders, same end-of-file contract pattern:

- `Actions/Rest Focus Loop/AGENTS.md` — 5 contracts (loop phase scripts)
- `Actions/Twitch Hype Train/AGENTS.md` — 3 contracts (hype train lifecycle)

## Prerequisite

Chunk 7 must show `done` in `progress.md`. Chunk 8 must show `pending`.

## What This Chunk Does

For each folder: extract ACTION-CONTRACTS block → `contracts.md`, update AGENTS.md reference.

**Files touched:**
- `Actions/Rest Focus Loop/AGENTS.md` — edited
- `Actions/Rest Focus Loop/contracts.md` — created
- `Actions/Twitch Hype Train/AGENTS.md` — edited
- `Actions/Twitch Hype Train/contracts.md` — created

Note: folder names contain spaces — use exact names.

## Files to Read Before Starting

1. `Actions/Rest Focus Loop/AGENTS.md`
2. `Actions/Twitch Hype Train/AGENTS.md`
3. `Projects/actions-scaffolding-refactor/progress.md`

## Step-by-Step Instructions

### Rest Focus Loop

1. Read `Actions/Rest Focus Loop/AGENTS.md`. Locate `## Action Contracts` and the `<!-- ACTION-CONTRACTS:START -->` / `<!-- ACTION-CONTRACTS:END -->` block. File ends at the end marker.

2. Create `Actions/Rest Focus Loop/contracts.md`:

```markdown
---
id: rest-focus-contracts
type: contracts
description: Action contracts for Rest Focus Loop scripts.
owner: streamerbot-dev
parent: AGENTS.md
---

# Rest Focus Loop — Action Contracts

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
[paste full JSON block]
<!-- ACTION-CONTRACTS:END -->
```

3. Edit `Actions/Rest Focus Loop/AGENTS.md`: delete contracts block, add:

```markdown
## Action Contracts

Contracts for all 5 Rest Focus Loop scripts live in [contracts.md](contracts.md).
```

### Twitch Hype Train

4. Read `Actions/Twitch Hype Train/AGENTS.md`. Same process.

5. Create `Actions/Twitch Hype Train/contracts.md`:

```markdown
---
id: twitch-hype-train-contracts
type: contracts
description: Action contracts for Twitch Hype Train scripts.
owner: streamerbot-dev
parent: AGENTS.md
---

# Twitch Hype Train — Action Contracts

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
[paste full JSON block]
<!-- ACTION-CONTRACTS:END -->
```

6. Edit `Actions/Twitch Hype Train/AGENTS.md`: delete contracts block, add:

```markdown
## Action Contracts

Contracts for all 3 Twitch Hype Train scripts live in [contracts.md](contracts.md).
```

7. Update `progress.md`: chunk 8 → `done`, update "Last updated".

## Validation Checklist

- [ ] `Rest Focus Loop/contracts.md` created with 5 contracts (rest-focus-focus-end, rest-focus-loop-start, rest-focus-pre-focus-end, rest-focus-pre-rest-end, rest-focus-rest-end)
- [ ] `Twitch Hype Train/contracts.md` created with 3 contracts (hype-train-end, hype-train-level-up, hype-train-start)
- [ ] Both `contracts.md` files have HTML markers
- [ ] Both `AGENTS.md` files reference their `contracts.md`, no JSON remaining
- [ ] `progress.md` updated
