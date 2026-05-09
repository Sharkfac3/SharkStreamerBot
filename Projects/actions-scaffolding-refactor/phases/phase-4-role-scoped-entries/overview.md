---
id: phase-4-role-scoped-entries-overview
type: phase-overview
phase: 4
status: active
---

# Phase 4 — Role-Scoped Entry Points

## Problem

`.agents/roles/` already defines what each role owns and does. But when a role agent starts working in `Actions/`, it still reads a general entry chain (AGENTS.md → SHARED-CONSTANTS.md → all subfolder guides). A `lotat-tech` agent loads Commander constants and Squad contracts it will never need. A `brand-steward` agent loads the full trigger catalog.

## Solution

Add an `actions-scope.md` file to each relevant role under `.agents/roles/<role>/`. This file gives that role a scoped reading list — only the Actions folders, constants files, and contracts files it actually needs.

This does not change what any role *can* access. It gives them a faster on-ramp.

## What Changes

Each `actions-scope.md` file:
- Lists the Actions subfolders this role works in
- Lists the constants files relevant to this role
- Lists the contracts files relevant to this role
- Points to the lean `Actions/AGENTS.md` (now a router after Phase 2) as the domain entry

`role.md` files are updated only if adding a reference to `actions-scope.md` makes navigation meaningfully better. The role file itself does not change structure.

## Chunks

| Chunk | Role | Rationale |
|---|---|---|
| 1 | lotat-tech | Narrowest scope — only LotAT folder + lotat/overlay-broker constants |
| 2 | streamerbot-dev | Widest scope — all folders, but ordered by typical workflow |
| 3 | brand-steward + ops | Secondary owners who touch Actions only for review/validation |

## Phase Gate

This is the final phase. When all 3 chunks show `done`, the project is complete.
