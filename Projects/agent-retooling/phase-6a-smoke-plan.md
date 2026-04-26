# Phase 6a — Smoke test plan

## Before you start

1. Read `Projects/agent-retooling/PLAN.md`.
2. Read `Projects/agent-retooling/STATUS.md` — confirm every Phase 5 chunk is checked off.

## Mission

Design a smoke-test plan that exercises the new tree on real-shaped tasks. Output `Projects/agent-retooling/smoke-plan.md`.

A smoke test = a cold-agent task description that should be answerable using only the new `.agents/` tree.

## Inputs

- New `.agents/` tree (read it as a fresh agent would).
- Every workflow in `.agents/workflows/`.
- `Projects/agent-retooling/migration-validation.md` if it exists (open questions from Phase 5).

## Coverage rules

- At least one smoke test per workflow.
- At least two cross-workflow chains (e.g. `streamerbot-script-write` → `ops-paste-and-validate`).
- One "ambiguous task" test where the operator's request doesn't obviously map to a workflow — confirm `ENTRY.md` routes correctly.
- One "trivial fix" test where the task is too small to deserve a workflow — confirm the tree doesn't force ceremony onto small work.

## Output format — `smoke-plan.md`

```markdown
# Smoke Test Plan

## Per-workflow tests

### Test 1 — <workflow-name>

**Scenario.** ...

**Cold-agent prompt.** (paste-ready, includes only what a cold agent would have)

**Pass criteria.** What the agent's output should look like to count as pass.

(repeat for every workflow)

## Cross-workflow chain tests

### Test C1 — <chain-name>

(same shape, but exercises chaining)

## Routing edge cases

### Test R1 — ambiguous task

...

### Test R2 — trivial fix

...

## Open questions

- ...
```

## When done

1. Append to `STATUS.md` under `## Notes`: `- 6a complete: <n> per-workflow tests, <n> chain tests, <n> routing tests.`
2. Tick `[x] phase-6a-smoke-plan.md` in STATUS.md.
3. Next: `phase-6b-smoke-tests.md`.
