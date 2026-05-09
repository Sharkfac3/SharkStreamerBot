---
id: actions-scaffolding-refactor-agents
type: domain-route
description: Agent guide for the actions-scaffolding-refactor project.
owner: ops
status: active
---

# Agent Guide — actions-scaffolding-refactor

## Who Works Here

The agent running chunks in this project is a **scaffolding builder**. It understands agent architecture patterns, file organization, and markdown-based routing systems. It does not need to understand Streamer.bot runtime behavior.

## What the Scaffolding Builder Does

- Reads existing AGENTS.md and markdown scaffolding files
- Reorganizes content into better-structured files
- Creates new files, moves content, updates cross-references
- Does NOT touch `.cs` script files
- Does NOT change runtime behavior or action contracts — only their location

## The streamerbot-dev Nav Contract

Every chunk must preserve the `streamerbot-dev` role's ability to navigate. After any chunk runs, the following must still resolve:

1. `Actions/<folder>/AGENTS.md` exists for every Actions subfolder
2. Action contracts for every script are reachable — either in AGENTS.md or clearly referenced from it
3. `Actions/SHARED-CONSTANTS.md` exists — even if it becomes an index
4. `Actions/Helpers/AGENTS.md` exists and indexes helper patterns
5. `Actions/Helpers/triggers/README.md` exists (trigger catalog)
6. `.agents/roles/streamerbot-dev/role.md` is accurate and not broken

If a chunk creates a new file location, it must update any AGENTS.md that previously pointed to the old location.

## Permission to Update streamerbot-dev Role

If the new structure improves navigation for `streamerbot-dev` but requires updating `.agents/roles/streamerbot-dev/role.md` (e.g., adding a reference to a new contracts.md file), the scaffolding builder is authorized to make that update.

## Execution Order

Always run phases in order. Within a phase, run chunks in order. Do not parallelize — later chunks depend on earlier ones having completed.

Check `Projects/actions-scaffolding-refactor/progress.md` before starting. Update it as the last step of every chunk.

## Out of Scope

- Any file outside `Actions/`, `.agents/roles/`, and `Projects/actions-scaffolding-refactor/`
- `.cs` script files
- `Tools/` directory
- `Apps/` directory
- Workflow files under `.agents/workflows/` (unless a chunk explicitly says otherwise)
