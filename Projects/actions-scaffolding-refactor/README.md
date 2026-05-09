---
id: actions-scaffolding-refactor
type: project
description: Refactor Actions folder agent scaffolding for better navigability and reduced context cost.
owner: ops
status: active
---

# Project: Actions Scaffolding Refactor

## Goal

Reduce context load and improve navigability of the agent scaffolding under `Actions/` without breaking any existing role's ability to find what it needs.

The current structure works but is heavy. Key problems:
- Action contracts are embedded in AGENTS.md files — agents load 300–400 lines just to navigate
- `SHARED-CONSTANTS.md` is 511 lines covering 18 unrelated domains
- `Actions/AGENTS.md` mixes governance, routing, rules, and contract schema in one file
- No role-scoped entry points — every agent loads everything

## What This Project Is NOT Doing

- Changing how Streamer.bot actions work at runtime
- Touching any `.cs` script files
- Altering the `streamerbot-dev` role's responsibilities or ownership
- Removing any information — only reorganizing it

## Phases

| Phase | Name | Goal | Chunks |
|---|---|---|---|
| 1 | Extract Contracts | Move action contract JSON out of AGENTS.md into sibling contracts.md files | 5 |
| 2 | Thin Master AGENTS.md | Split Actions/AGENTS.md governance into focused sub-files | 3 |
| 3 | Split SHARED-CONSTANTS.md | Break 511-line constants file into domain-scoped files | 6 |
| 4 | Role-Scoped Entries | Add scoped Actions views per agent role | 3 |

## Execution Model

Each chunk is a self-contained prompt run in a separate chat session. Run chunks in phase order. Do not skip phases. Check `progress.md` before starting any chunk to confirm its prerequisites are complete.

## Files

```
phases/
├── phase-1-extract-contracts/
│   ├── overview.md
│   ├── chunk-1-squad-contracts.md
│   ├── chunk-2-lotat-contracts.md
│   ├── chunk-3-commanders-contracts.md
│   ├── chunk-4-overlay-contracts.md
│   └── chunk-5-intros-contracts.md
├── phase-2-thin-master-agents/
│   ├── overview.md
│   ├── chunk-1-extract-rules.md
│   ├── chunk-2-extract-ownership.md
│   └── chunk-3-extract-contract-schema.md
├── phase-3-split-shared-constants/
│   ├── overview.md
│   ├── chunk-1-stream-core.md
│   ├── chunk-2-mini-games.md
│   ├── chunk-3-commanders-constants.md
│   ├── chunk-4-overlay-broker.md
│   ├── chunk-5-lotat-constants.md
│   └── chunk-6-effects.md
└── phase-4-role-scoped-entries/
    ├── overview.md
    ├── chunk-1-lotat-tech-scope.md
    ├── chunk-2-streamerbot-dev-scope.md
    └── chunk-3-secondary-roles-scope.md
```

## Key Paths

- Actions folder: `Actions/`
- Shared constants: `Actions/SHARED-CONSTANTS.md`
- Master domain guide: `Actions/AGENTS.md`
- streamerbot-dev role: `.agents/roles/streamerbot-dev/role.md`
- Progress tracking: `Projects/actions-scaffolding-refactor/progress.md`
