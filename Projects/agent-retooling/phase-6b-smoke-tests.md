# Phase 6b — Smoke test execution package

## Before you start

1. Read `Projects/agent-retooling/PLAN.md`.
2. Read `Projects/agent-retooling/STATUS.md` — confirm `phase-6a-smoke-plan.md` is checked off.

## Mission

Turn the smoke plan into a paste-ready execution package. Output `Projects/agent-retooling/smoke-tests.md` (operator runs the tests) and seed `Projects/agent-retooling/smoke-report.md` (operator records results).

The operator runs each test by pasting the cold-agent prompt into a fresh Pi or Claude session against the new tree, then records pass/fail in `smoke-report.md`. This phase produces both files; it does not run the tests itself.

## Inputs

- `Projects/agent-retooling/smoke-plan.md`

## Steps

1. For each test in `smoke-plan.md`, format a clearly delimited "paste this" block in `smoke-tests.md`:

   ````markdown
   ### Test 1 — <workflow-name>

   **Scenario.** ...

   **Paste-ready prompt:**

   ```
   <full prompt the operator pastes into a fresh agent session>
   ```

   **Pass criteria.** ...

   **Notes for operator.** Any setup the operator must do first (e.g. confirm `.agents/` is in repo root, no leftover `.pi/`).
   ````

2. Create `smoke-report.md` with one stub per test:

   ```markdown
   # Smoke Test Report

   ## Test 1 — <workflow-name>

   - **Run by:** Pi / Claude
   - **Result:** pass / fail
   - **Notes:** ...

   (repeat)

   ## Summary

   - Pass: <n>
   - Fail: <n>
   - Blocked: <n>

   ## Failures requiring fix before Phase 6c

   - ...
   ```

3. Both files use stable test numbering matching `smoke-plan.md`.

## When done

1. Append to `STATUS.md` under `## Notes`: `- 6b complete: <n> tests packaged for operator execution; smoke-report stub seeded.`
2. Tick `[x] phase-6b-smoke-tests.md` in STATUS.md.
3. Operator runs tests against the new tree, fills in `smoke-report.md`.
4. After operator records results, next prompt is `phase-6c-doc-catchup.md` (only if Pass ≥ Fail; otherwise re-enter Phase 5 for fixes).
