# Phase 6c — Top-level doc catch-up

## Before you start

1. Read `Projects/agent-retooling/PLAN.md`.
2. Read `Projects/agent-retooling/STATUS.md` — confirm `phase-6b-smoke-tests.md` is checked off and `smoke-report.md` shows acceptable pass rate.
3. Read `Projects/agent-retooling/smoke-report.md` — note any test failures that require doc fixes.

## Mission

Update top-level documentation to match the new workflow-based tree.

## Files in scope

- `CLAUDE.md` (project instructions checked into repo)
- `AGENTS.md` (top-level)
- `WORKING.md` (active agent work protocol)
- `README.md` (top-level, contains Docs Index and routing pointers)
- `.agents/ENTRY.md` (already created in Phase 5; touch only if smoke tests revealed gaps)

## What to change

For each file:

1. Replace any reference to `.agents/roles/<role>/` with the equivalent `.agents/workflows/<workflow>/` or `.agents/skills/<skill>/` reference.
2. Replace any reference to `.pi/skills/` with the equivalent `.agents/skills/` reference (or remove if obsolete).
3. Remove any "Roles" table or list — replace with a "Workflows" or "Common tasks" table that points at workflow files.
4. Remove references to `routing-manifest.json`.
5. Confirm the "Before Starting Any Task" / "After Any Code Change" sections in `CLAUDE.md` still make sense — they may need to point at workflow chains rather than role skills.

## Anti-drift reminders

- The new tree's truth lives in `.agents/`. Top-level docs should point into the tree, not duplicate it.
- Do not invent new ceremony in these docs. If a section in `CLAUDE.md` is no longer applicable, delete it.

## Validation

After editing, grep the entire repo for `roles/`, `routing-manifest`, and `\.pi/` (excluding `Projects/agent-retooling/` and `.git/`). Any remaining match is an oversight — fix or document why it is intentional.

## When done

1. Append to `STATUS.md` under `## Notes`: `- 6c complete: <list of files updated>; <n> stale references fixed; grep clean.`
2. Tick `[x] phase-6c-doc-catchup.md` in STATUS.md.
3. Next: `phase-6d-cleanup.md`.
