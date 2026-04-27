# Prompt 04 — Inventory Cross-References

## Agent

Pi (manual copy/paste by operator).

## Purpose

Build a reference graph of every internal link between agent docs. Identifies orphan files (no inbound references), reference clusters (files always loaded together), and broken links.

## Preconditions

- Prompts 00, 01, 02 done; findings exist.
- Read prior handoffs.

## Scope

Read-only. Writes `Projects/agent-reflow/findings/04-cross-refs.md`.

In scope for link extraction:
- All `.md` files under `.agents/`
- All `.md` files under `retired Pi skill mirror/`
- `AGENTS.md`, `CLAUDE.md`, `WORKING.md`, `README.md` at repo root
- `Docs/` markdown
- Per-domain `README.md` files when they reference agent docs

Out of scope: links inside `Actions/**/*.cs`, `Apps/**/*.ts`, etc. — those are code references, not agent-doc references.

## Out-of-scope

- No edits, no git

## Steps

1. Extract every markdown link (`[text](path)`) and every backtick-wrapped path mention (e.g. `` `.agents/roles/...` ``) from in-scope files.
2. Resolve each link to a file path (relative or absolute repo path).
3. Build adjacency list: for each in-scope file, list outbound references and inbound references.
4. Flag broken links (target file does not exist).
5. Identify orphans (files with zero inbound references — may be unreachable).
6. Identify hubs (files with high inbound count — entry points worth preserving).
7. Identify clusters: file pairs where A references B and B references A (mutual reference — co-location candidates).
8. Identify chains: A → B → C → D linear reference paths (progressive disclosure pipelines).

## Validator / Acceptance

- Every in-scope file appears in the adjacency list
- Broken links explicitly listed
- Orphans listed
- Hubs (top 10 by inbound count) listed
- Mutual-reference pairs listed

## Handoff

Per template. Note: orphans that look like they should still be reachable (i.e. content suggests they're important but no one links to them) — flag as "lost progressive disclosure".
