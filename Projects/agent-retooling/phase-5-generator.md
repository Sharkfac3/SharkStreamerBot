# Phase 5 — Generator: spawn per-chunk migration prompts

## Before you start

1. Read `Projects/agent-retooling/PLAN.md`.
2. Read `Projects/agent-retooling/STATUS.md` — confirm `phase-4-migration-map.md` is checked off.

## Mission

Read `Projects/agent-retooling/migration-order.md`. Generate one per-chunk prompt per chunk by cloning `phase-5-chunk-template.md`. Update STATUS.md.

## Steps

1. Read `migration-order.md` and list every chunk.
2. For each chunk, create `Projects/agent-retooling/phase-5a-chunk-<NN>-<chunk-name>.md` where `<NN>` is a zero-padded sequence (`01`, `02`, …) preserving execution order.
3. Cloning rules (same shape as `phase-1-generator.md`):
   - Read `phase-5-chunk-template.md`.
   - Strip everything above and including `## Template body (everything below this line is the per-chunk prompt)`.
   - Substitute `<CHUNK_NAME>`, `<CHUNK_FILES>`, `<CHUNK_PRE>`, `<CHUNK_POST>` with values from `migration-order.md`.
   - Save the result.
4. Update STATUS.md:
   - Under `## Phase 5 — Execute Migration`, add one checkbox per generated prompt:
     ```
     - [ ] `phase-5a-chunk-<NN>-<chunk-name>.md`
     ```
   - Note any chunks marked parallelizable so the operator can fan out.

## When done

1. Append to `STATUS.md` under `## Notes`: `- 5-generator complete: <count> chunk prompts generated; parallelizable groups: <list>.`
2. Tick `[x] phase-5-generator.md` in STATUS.md.
3. Next: run `phase-5a-chunk-01-*.md`, then `02`, etc., respecting parallelization.
4. After **all** chunks are checked off, run `phase-6a-smoke-plan.md`.
