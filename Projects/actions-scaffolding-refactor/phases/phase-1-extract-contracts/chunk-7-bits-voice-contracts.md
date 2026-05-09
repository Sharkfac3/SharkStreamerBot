---
id: phase-1-chunk-7-bits-voice-contracts
type: refactor-prompt
phase: 1
chunk: 7
status: ready
---

# Phase 1 / Chunk 7 — Extract Twitch Bits and Voice Commands Contracts

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You reorganize agent scaffolding markdown files to improve navigability and reduce context load. You do not touch `.cs` runtime scripts and do not alter what information exists — only where it lives.

## Repository Context

This chunk handles two medium-sized folders in one pass. Both follow the same simple pattern: contracts are at the end of AGENTS.md with nothing after them.

- `Actions/Twitch Bits Integrations/AGENTS.md` — 7 contracts (bits tiers + effects)
- `Actions/Voice Commands/AGENTS.md` — 7 contracts (mode and scene scripts)

## Prerequisite

Chunk 6 must show `done` in `progress.md`. Chunk 7 must show `pending`.

## What This Chunk Does

For EACH of the two folders:
1. Extract ACTION-CONTRACTS block from AGENTS.md
2. Create `contracts.md` with the block
3. Update AGENTS.md to reference it

**Files touched:**
- `Actions/Twitch Bits Integrations/AGENTS.md` — edited
- `Actions/Twitch Bits Integrations/contracts.md` — created
- `Actions/Voice Commands/AGENTS.md` — edited
- `Actions/Voice Commands/contracts.md` — created

Note: folder names contain spaces — use exact names in all paths.

## What This Chunk Does NOT Do

- Touch any `.cs` files
- Touch any other folder

## Files to Read Before Starting

1. `Actions/Twitch Bits Integrations/AGENTS.md`
2. `Actions/Voice Commands/AGENTS.md`
3. `Projects/actions-scaffolding-refactor/progress.md`

## Step-by-Step Instructions

Complete Twitch Bits Integrations first, then Voice Commands. Same pattern for both.

### Twitch Bits Integrations

1. Read `Actions/Twitch Bits Integrations/AGENTS.md`. Locate `## Action Contracts` heading and the `<!-- ACTION-CONTRACTS:START -->` / `<!-- ACTION-CONTRACTS:END -->` block. File ends at the end marker.

2. Create `Actions/Twitch Bits Integrations/contracts.md`:

```markdown
---
id: twitch-bits-contracts
type: contracts
description: Action contracts for Twitch Bits Integration scripts.
owner: streamerbot-dev
parent: AGENTS.md
---

# Twitch Bits Integrations — Action Contracts

These contracts are maintained by `streamerbot-dev`. Load this file when validating or updating a script contract.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
[paste full JSON block]
<!-- ACTION-CONTRACTS:END -->
```

3. Edit `Actions/Twitch Bits Integrations/AGENTS.md`: delete the contracts block, add reference:

```markdown
## Action Contracts

Contracts for all 7 Twitch Bits Integration scripts live in [contracts.md](contracts.md).
```

### Voice Commands

4. Read `Actions/Voice Commands/AGENTS.md`. Same process.

5. Create `Actions/Voice Commands/contracts.md`:

```markdown
---
id: voice-commands-contracts
type: contracts
description: Action contracts for Voice Command scripts.
owner: streamerbot-dev
parent: AGENTS.md
---

# Voice Commands — Action Contracts

These contracts are maintained by `streamerbot-dev`. Load this file when validating or updating a script contract.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
[paste full JSON block]
<!-- ACTION-CONTRACTS:END -->
```

6. Edit `Actions/Voice Commands/AGENTS.md`: delete the contracts block, add reference:

```markdown
## Action Contracts

Contracts for all 7 Voice Command scripts live in [contracts.md](contracts.md).
```

7. Update `progress.md`: chunk 7 → `done`, update "Last updated".

## Validation Checklist

- [ ] `Twitch Bits Integrations/contracts.md` created with 7 contracts (bits-tier-1 through 4, gigantify-emote, message-effects, on-screen-celebration)
- [ ] `Voice Commands/contracts.md` created with 7 contracts (mode-gamer, mode-garage, mode-workspace, scene-chat, scene-dance, scene-housekeeping, scene-main)
- [ ] Both `contracts.md` files have HTML markers
- [ ] Both `AGENTS.md` files reference their `contracts.md`
- [ ] Neither `AGENTS.md` has remaining JSON blocks
- [ ] `progress.md` updated
