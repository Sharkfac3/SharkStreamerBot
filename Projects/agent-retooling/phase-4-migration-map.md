# Phase 4 — Migration map

## Before you start

1. Read `Projects/agent-retooling/PLAN.md`.
2. Read `Projects/agent-retooling/STATUS.md` — confirm `phase-3-tree-design.md` is checked off.

## Mission

Produce two files:

- `Projects/agent-retooling/migration-map.md` — for every current file under `.agents/` and `.pi/`, an entry that maps it to a new path, a merge target, or the graveyard.
- `Projects/agent-retooling/migration-order.md` — execution chunks ordered for safe hard-cutover migration.

## Inputs

- `Projects/agent-retooling/audit.md` (full file list)
- `Projects/agent-retooling/salvage-list.md` (verdicts and proposed new paths)
- `Projects/agent-retooling/new-tree.md` (final file layout)

## migration-map.md format

For every file in `audit.md`, exactly one row:

| current path | action | target | content transform |
|---|---|---|---|
| .agents/_shared/project.md | move | .agents/_shared/project.md | none |
| .agents/roles/streamerbot-dev/skills/squad/_index.md | merge | .agents/_shared/squad-patterns.md | extract checklist section |
| .agents/roles/_template/role.md | delete | (graveyard) | role concept retired |
| .pi/skills/streamerbot-dev/SKILL.md | delete | (graveyard) | role wrapper, redundant in new tree |

`action` is one of: `move`, `move+rename`, `merge`, `rewrite`, `delete`.

Group rows under H2 sections: `### Move`, `### Move + rename`, `### Merge`, `### Rewrite`, `### Delete`.

Add a `## Net new files` section listing `.agents/` files that have no current source — entirely new files Phase 5 will author from scratch (most workflows + skills + new ENTRY.md).

## migration-order.md format

Order chunks so each chunk leaves the repo in a coherent state. Recommended order — adjust if Phase 3 design changes the dependency shape:

1. **Build new `_shared/`.** Copy preserved docs; perform merges. After this chunk: new `_shared/` is complete; old `_shared/` still exists in parallel. (Hard cutover means we won't keep both at end, but during this chunk both can coexist briefly.)
2. **Build `skills/<skill>/SKILL.md`.** All atomic skills. May split into sub-chunks (e.g. 5 skills per chunk).
3. **Build `workflows/<workflow>/WORKFLOW.md`.** All workflows. Split similarly.
4. **Write new `ENTRY.md`.**
5. **Delete `.agents/roles/`, `.agents/routing-manifest.json`, old `.agents/_shared/` files that were replaced.**
6. **Delete `.pi/` entirely.**
7. **Update top-level references.** `CLAUDE.md`, `AGENTS.md`, `WORKING.md`, `README.md` — any pointer at `.agents/roles/` or `.pi/` updated to new tree.

Each chunk in `migration-order.md` should specify:

- Chunk name (kebab-case, e.g. `build-shared`, `build-skills-batch-1`)
- Files touched (paths)
- Pre-conditions (other chunks that must be done first)
- Post-conditions (what state the repo is in after)
- Estimated size (small / medium / large — used by Phase 5 generator to pick batch granularity)

## Output format — `migration-order.md`

```markdown
# Migration Order

Hard-cutover migration plan. Each chunk is self-contained. Chunks must run in order unless marked parallelizable.

## Chunks

### chunk-1: build-shared

- **Touches:** `.agents/_shared/<files>`
- **Pre:** none
- **Post:** new `_shared/` exists alongside old structure
- **Size:** medium

### chunk-2: build-skills-batch-1

- **Touches:** `.agents/skills/<n>/SKILL.md` × ~5
- **Pre:** chunk-1
- **Post:** first batch of skill files exist
- **Size:** medium
- **Parallelizable with:** chunk-3, chunk-4 (other skill batches)

(repeat)

### chunk-N: gut-old-tree

- **Touches:** delete `.agents/roles/`, delete `.agents/routing-manifest.json`, delete superseded `_shared/` files, delete `.pi/`
- **Pre:** all build chunks complete
- **Post:** old tree fully removed
- **Size:** small

### chunk-N+1: top-level-doc-fix

- **Touches:** `CLAUDE.md`, `AGENTS.md`, `WORKING.md`, `README.md`
- **Pre:** chunk-N
- **Post:** no references to old tree remain anywhere
- **Size:** small

## Parallelization map

(which chunks can run in parallel — Phase 5 generator uses this)
```

## When done

1. Append to `STATUS.md` under `## Notes`: `- 4 complete: <n> mapped files, <n> chunks (<n> parallelizable groups).`
2. Tick `[x] phase-4-migration-map.md` in STATUS.md.
3. Next: `phase-5-generator.md`.
