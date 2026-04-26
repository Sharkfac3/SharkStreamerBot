# Agent Retooling — Status

Source of truth for retool progress. Every prompt updates this file when it finishes.

## How to use this file

- Each phase prompt's first instruction is to read `STATUS.md` and confirm its dependencies are checked off.
- Each phase prompt's last instruction is to tick its own checkbox and add a one-line completion note under `## Notes`.
- Generator prompts (Phase 1, Phase 5) add new checkboxes for the prompts they generate.
- If a prompt fails or is partially complete, leave its box unchecked and add a `## Blocked` entry instead of a note.

## Phase 0 — Discovery (parallelizable; 0d depends on 0a + 0b)

- [ ] `phase-0a-audit.md` → `audit.md`
- [ ] `phase-0b-pi-format.md` → `pi-skill-format.md`
- [ ] `phase-0c-workflow-seeds.md` → `workflow-seeds.md`
- [ ] `phase-0d-salvage.md` → `salvage-list.md` (depends on 0a + 0b)

## Phase 1 — Workflow Catalog (1-generator runs first; spawns per-cluster prompts)

- [ ] `phase-1-generator.md` → `workflow-clusters.md` + per-cluster prompts (depends on 0c + 0d)
- (per-cluster prompts get added here when 1-generator runs)

## Phase 2 — Skill Atomization

- [ ] `phase-2-skill-extract.md` → `skill-inventory.md` (depends on all `workflow-spec-*.md`)

## Phase 3 — New Tree Design

- [ ] `phase-3-tree-design.md` → `new-tree.md` (depends on Phase 2)

## Phase 4 — Migration Map

- [ ] `phase-4-migration-map.md` → `migration-map.md` + `migration-order.md` (depends on Phase 3)

## Phase 5 — Execute Migration (5-generator runs first; spawns per-chunk prompts)

- [ ] `phase-5-generator.md` → per-chunk prompts (depends on Phase 4)
- (per-chunk prompts get added here when 5-generator runs)

## Phase 6 — Smoke Test & Docs

- [ ] `phase-6a-smoke-plan.md` → `smoke-plan.md` (depends on Phase 5 fully complete)
- [ ] `phase-6b-smoke-tests.md` → `smoke-tests.md` (test prompts for operator to run)
- [ ] `phase-6c-doc-catchup.md` → updates to `CLAUDE.md`, `README.md`, etc.
- [ ] `phase-6d-cleanup.md` → deletes `Projects/agent-retooling/`

## Notes

(prompt completion notes appended here)

## Blocked

(blockers appended here; remove entry when unblocked)
