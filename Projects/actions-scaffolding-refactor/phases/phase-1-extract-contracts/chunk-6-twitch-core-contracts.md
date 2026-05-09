---
id: phase-1-chunk-6-twitch-core-contracts
type: refactor-prompt
phase: 1
chunk: 6
status: ready
---

# Phase 1 / Chunk 6 — Extract Twitch Core Integrations Contracts

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You reorganize agent scaffolding markdown files to improve navigability and reduce context load. You do not touch `.cs` runtime scripts and do not alter what information exists — only where it lives.

## Repository Context

`Actions/Twitch Core Integrations/` contains scripts for stream start/reset, follows, subscriptions, gift subs, and watch streaks. Its `AGENTS.md` contains an ACTION-CONTRACTS block with 10 script contracts spanning lines 76–276 — the largest single contract block after Squad.

## Prerequisite

Check `progress.md`. Chunks 1–5 must show `done`. Chunk 6 must show `pending`.

## What This Chunk Does

Extract the ACTION-CONTRACTS block from `Actions/Twitch Core Integrations/AGENTS.md` into `Actions/Twitch Core Integrations/contracts.md`.

**Files touched:**
- `Actions/Twitch Core Integrations/AGENTS.md` — edited
- `Actions/Twitch Core Integrations/contracts.md` — created

Note: the folder name contains spaces. Use the full name exactly as-is in all file paths.

## What This Chunk Does NOT Do

- Touch any `.cs` files
- Touch any other folder
- Change any contract content

## Files to Read Before Starting

1. `Actions/Twitch Core Integrations/AGENTS.md`
2. `Projects/actions-scaffolding-refactor/progress.md`
3. `Projects/actions-scaffolding-refactor/AGENTS.md`

## Step-by-Step Instructions

1. Read `Actions/Twitch Core Integrations/AGENTS.md` in full.

2. Locate the `## Action Contracts` heading and the block bounded by:
   - `<!-- ACTION-CONTRACTS:START -->`
   - `<!-- ACTION-CONTRACTS:END -->`

   The file ends at `<!-- ACTION-CONTRACTS:END -->`.

3. Create `Actions/Twitch Core Integrations/contracts.md`:

```markdown
---
id: twitch-core-contracts
type: contracts
description: Action contracts for Twitch Core Integration scripts.
owner: streamerbot-dev
parent: AGENTS.md
---

# Twitch Core Integrations — Action Contracts

These contracts are maintained by `streamerbot-dev`. Load this file when validating or updating a script contract. Do not edit contracts without also updating the SHA256 stamp in the corresponding `.cs` file.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
[paste the full JSON block here exactly as it appears in AGENTS.md]
<!-- ACTION-CONTRACTS:END -->
```

   Preserve both HTML comment markers.

4. Edit `Actions/Twitch Core Integrations/AGENTS.md`:
   - Delete the `## Action Contracts` heading and everything from `<!-- ACTION-CONTRACTS:START -->` to `<!-- ACTION-CONTRACTS:END -->` inclusive
   - Add at the end:

```markdown
## Action Contracts

Contracts for all 10 Twitch Core Integration scripts live in [contracts.md](contracts.md). Load it when validating or updating a script contract.
```

5. Verify: `contracts.md` has 10 contracts (follower-new, stream-start, subscription-counter-rollover, subscription-gift-paid-upgrade, subscription-gift, subscription-new, subscription-pay-it-forward, subscription-prime-paid-upgrade, subscription-renewed, watch-streak). AGENTS.md has no JSON.

6. Update `progress.md`: chunk 6 → `done`, update "Last updated".

## Validation Checklist

- [ ] `contracts.md` created with 10 contracts
- [ ] Both HTML markers present in `contracts.md`
- [ ] `AGENTS.md` references `contracts.md`
- [ ] `AGENTS.md` has no JSON blocks remaining
- [ ] `progress.md` updated
