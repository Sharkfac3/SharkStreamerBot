---
id: phase-1-extract-contracts-overview
type: phase-overview
phase: 1
status: active
---

# Phase 1 — Extract Contracts

## Problem

Action contracts are embedded inside AGENTS.md files. Every time an agent opens a domain AGENTS.md, it loads the full contract block even if it only needs routing or ownership info.

| File | Total Lines | Contract Lines |
|---|---|---|
| Squad/AGENTS.md | 430 | ~330 |
| LotAT/AGENTS.md | 365 | ~255 |
| Commanders/AGENTS.md | 312 | ~180 |
| Overlay/AGENTS.md | 150 | ~87 |
| Intros/AGENTS.md | 151 | ~83 |

Total contract lines loaded on every domain read: ~935 lines agents often don't need.

## Solution

Move the `ACTION-CONTRACTS` block out of each AGENTS.md into a sibling `contracts.md` file. Update AGENTS.md to reference `contracts.md` instead.

After this phase:
- AGENTS.md files are lean routers (~60–100 lines each)
- contracts.md files hold the full JSON blocks
- Agents load contracts only when validating or updating a script

## Output Shape Per Folder

```
Actions/<folder>/
├── AGENTS.md          ← routing, ownership, rules, workflow — NO contracts
└── contracts.md       ← ACTION-CONTRACTS block only
```

AGENTS.md must include a clear reference:

```markdown
## Action Contracts

Contracts for all scripts in this folder live in [contracts.md](contracts.md).
Load it when validating or updating a script contract.
```

## Important: Action Contract Markers

All contract blocks use HTML comment markers that must be preserved in `contracts.md`:
```
<!-- ACTION-CONTRACTS:START -->
...JSON...
<!-- ACTION-CONTRACTS:END -->
```
The validation tool (`action_contracts.py`) uses these markers. Every chunk prompt specifies this — do not strip the markers.

## Chunks

| Chunk | Target | Contracts |
|---|---|---|
| 1 | Squad/AGENTS.md | 17 |
| 2 | LotAT/AGENTS.md | 13 |
| 3 | Commanders/AGENTS.md + Script Reference | 2 + reference doc |
| 4 | Overlay/AGENTS.md | 4 |
| 5 | Intros/AGENTS.md | 2 |
| 6 | Twitch Core Integrations/AGENTS.md | 10 |
| 7 | Twitch Bits Integrations + Voice Commands | 7 + 7 |
| 8 | Rest Focus Loop + Twitch Hype Train | 5 + 3 |
| 9 | XJ Drivethrough + Destroyer + Twitch Channel Points + Temporary | 1+2+2+2 |

Run chunks in order.

## Phase Gate

Phase 2 may begin once all 9 chunks show `done` in `progress.md`.
