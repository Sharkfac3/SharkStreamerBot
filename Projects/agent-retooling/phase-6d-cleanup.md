# Phase 6d — Cleanup

## Before you start

1. Read `Projects/agent-retooling/PLAN.md`.
2. Read `Projects/agent-retooling/STATUS.md` — confirm every prior phase prompt is checked off and `smoke-report.md` shows the operator-accepted pass rate.

## Mission

The retool is complete. Tear down the transitional folder.

## Steps

1. Confirm the new tree is in place:
   - `.agents/ENTRY.md` exists.
   - `.agents/workflows/` has at least one `WORKFLOW.md`.
   - `.agents/skills/` has at least one `SKILL.md`.
   - `.agents/roles/` does **not** exist.
   - `.agents/routing-manifest.json` does **not** exist.
   - `.pi/` does **not** exist.
2. Confirm STATUS.md shows every prompt checked.
3. Write a final summary to `Projects/agent-retooling/RETOOL-SUMMARY.md`:

   ```markdown
   # Retool — Final Summary

   ## What changed

   - Roles tree replaced with workflow + atomic skill tree.
   - Drift sources eliminated: per-role context folders, `.pi/skills/` mirror, `routing-manifest.json`.
   - Anti-drift rule active: discoveries edit canonical `_shared/` docs or stay in conversation; no new context dirs.

   ## Final tree shape

   - `.agents/ENTRY.md`
   - `.agents/_shared/` — <n> reference docs
   - `.agents/workflows/` — <n> workflows
   - `.agents/skills/` — <n> skills

   ## Smoke test result

   - Pass: <n> / <total>
   - Notable issues fixed during smoke: ...

   ## Operator follow-ups (deferred)

   - `humans/` retooling
   - <other deferred items>
   ```

4. Save `RETOOL-SUMMARY.md` somewhere durable **outside** `Projects/agent-retooling/` — copy it to `Docs/Architecture/agent-retool-2026.md` (or similar dated path) so the change is documented after the transitional folder is deleted.
5. Delete `Projects/agent-retooling/` entirely.

   Use `rm -rf Projects/agent-retooling` (Bash) — but **before deleting**, confirm the operator has reviewed `RETOOL-SUMMARY.md` and the durable copy. The deletion is irreversible without git history; the operator may want to commit first.
6. **Stop.** Do not commit. Operator commits manually per repo convention.

## Done state

- Transitional folder gone.
- New tree in production.
- Summary documented at `Docs/Architecture/agent-retool-2026.md` (or the chosen durable path).
- Operator commits the full retool as one or more PRs.
