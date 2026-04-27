---
id: shared-conventions
type: shared
description: Repo-wide conventions for agent and file routing.
status: active
owner: ops
---

# Shared Conventions

## Task Coordination

Read [WORKING.md](../../WORKING.md) before starting. Follow [coordination](../workflows/coordination.md) for active-work registration, conflict checks, and finish-state updates.

## File Routing

| Area | Rule |
|---|---|
| [Actions/](../../Actions/) | Streamer.bot C# runtime source and action-local docs. |
| [Apps/](../../Apps/) | Standalone TypeScript app code and app-local docs. |
| [Tools/](../../Tools/) | Local utilities, validators, sync helpers, and external integration scripts. |
| [Creative/](../../Creative/) | Brand, art, marketing, worldbuilding, and creative source docs. |
| [../](../) | Agent roles, workflows, manifest routing, and shared agent context. |
| Pi skill mirror | Retired 2026-04 at cutover; use local `AGENTS.md` guides and `.agents/` roles/workflows instead. |

## Naming

- Manifest IDs use kebab-case.
- Local domain agent guides are named `AGENTS.md`.
- Role overviews live at `.agents/roles/<role>/role.md`.
- Reusable procedures live at `.agents/workflows/<workflow-id>.md`.
- Existing domain folder names are not renamed during agent-tree reflow unless a prompt explicitly says so.

## Code and Docs Style

- Keep changes focused and preserve live-stream reliability.
- For `Actions/` work, use constants from [Actions/SHARED-CONSTANTS.md](../../Actions/SHARED-CONSTANTS.md) and reusable patterns from [Actions/HELPER-SNIPPETS.md](../../Actions/HELPER-SNIPPETS.md).
- Prefer local domain guides over old central role skill files.
- Use Markdown links for files the next agent should follow.
- Avoid adding new Pi wrapper skills; use manifest-backed local guides and role/workflow docs instead.

## Git and Sync Notes

Agents do not perform git operations unless explicitly asked. For Streamer.bot C# changes, document paste targets and sync status through [sync](../workflows/sync.md) and [change-summary](../workflows/change-summary.md).
