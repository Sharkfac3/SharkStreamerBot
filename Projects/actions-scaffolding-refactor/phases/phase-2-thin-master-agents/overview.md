---
id: phase-2-thin-master-agents-overview
type: phase-overview
phase: 2
status: active
---

# Phase 2 — Thin Master AGENTS.md

## Problem

`Actions/AGENTS.md` is 189 lines mixing four distinct concerns in one file:
- Governance rules (domain rules + universal script rules)
- Ownership model (role matrix, shared ownership)
- Contract schema (what a valid contract looks like + validation instructions)
- Routing (folder table, runtime integration map)

An agent navigating to a Squad script loads all four concerns even though it only needs routing. An agent updating a contract loads everything when it only needs the schema.

## Solution

Split `Actions/AGENTS.md` by concern into focused sub-files. The master file becomes a thin router (~50 lines) that points to the right file.

After this phase:

```
Actions/
├── AGENTS.md           ← router only: purpose, folder routing table, required reading chain
├── RULES.md            ← 14 domain rules + universal script rules
├── OWNERSHIP.md        ← role matrix, shared ownership, chain-to rules
└── CONTRACT-SCHEMA.md  ← contract format spec, field definitions, validation instructions
```

## What Stays in AGENTS.md

- Purpose statement (what this folder is)
- Start Here / required reading chain
- Folder Routing table (13 rows)
- Runtime Integration Map
- Short pointers to RULES.md, OWNERSHIP.md, CONTRACT-SCHEMA.md

## What Moves Out

| Section | Destination |
|---|---|
| Domain Rules (14 rules) | RULES.md |
| Universal Script Rules | RULES.md |
| Shared Ownership Rules | OWNERSHIP.md |
| Shared Ownership table | OWNERSHIP.md |
| Action Contracts schema | CONTRACT-SCHEMA.md |
| Validation and Handoff | CONTRACT-SCHEMA.md |
| Sync and Handoff Expectations | CONTRACT-SCHEMA.md |
| Boundaries | RULES.md or OWNERSHIP.md (chunk decides) |

## Chunks

| Chunk | Creates | Moves From AGENTS.md |
|---|---|---|
| 1 | RULES.md | Domain Rules + Universal Script Rules + Boundaries |
| 2 | OWNERSHIP.md | Shared Ownership Rules + role table |
| 3 | CONTRACT-SCHEMA.md | Contract format spec + Validation + Sync/Handoff |

Run in order — each chunk edits AGENTS.md after the previous chunk narrowed it.

## Phase Gate

Phase 3 may begin once all 3 chunks show `done` in `progress.md`.
