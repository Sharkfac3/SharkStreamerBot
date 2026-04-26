# Phase 3 — New tree design

## Before you start

1. Read `Projects/agent-retooling/PLAN.md`.
2. Read `Projects/agent-retooling/STATUS.md` — confirm `phase-2-skill-extract.md` is checked off.

## Mission

Design the full new `.agents/` tree. Every file gets a path and a content stub. Output `Projects/agent-retooling/new-tree.md`.

## Inputs

- `Projects/agent-retooling/skill-inventory.md`
- Every `Projects/agent-retooling/workflow-spec-<cluster>.md`
- `Projects/agent-retooling/salvage-list.md` (especially `keep` entries with proposed new paths)
- `Projects/agent-retooling/pi-skill-format.md` (for SKILL.md frontmatter shape)

## Locked layout (from PLAN.md)

```
.agents/
  ENTRY.md
  _shared/
    <preserved-or-new-reference-docs>.md
  workflows/
    <workflow-name>/
      WORKFLOW.md
  skills/
    <skill-name>/
      SKILL.md
```

No `roles/`, no `context/` subdirs, no `routing-manifest.json`. `.pi/` is removed entirely (confirmed by `pi-skill-format.md`).

## Steps

1. **`_shared/` design.** List every file that lands here, sourced from `salvage-list.md` keeps and merges. Each entry: final path, brief purpose, source files merged in.
2. **`workflows/` design.** For every workflow in Phase 1 specs, produce a `WORKFLOW.md` content stub. Stub includes: trigger, inputs, ordered steps with `[skill: ...]` refs, exit criteria, chains-to, out-of-scope. Use the workflow spec from Phase 1 directly.
3. **`skills/` design.** For every skill in `skill-inventory.md`, produce a `SKILL.md` content stub. Stub includes: frontmatter (`name`, `description`) per Pi format, body with purpose, inputs, outputs, steps (numbered), notes.
4. **`ENTRY.md` design.** Top-level index. Sections: project one-liner, how to navigate (read PLAN-equivalent if needed), workflow index table (workflow → trigger → cluster), skill index table (skill → purpose), `_shared/` index, anti-drift rule.
5. **Cross-check.** Every workflow's skill refs must resolve to a skill in the inventory. Every `_shared/` reference in a skill or workflow must exist in the `_shared/` design.

## Output format — `new-tree.md`

```markdown
# New Tree Design

Complete file layout for the new `.agents/`. Phase 4 maps old → new; Phase 5 creates these files.

## File tree

```
.agents/
  ENTRY.md
  _shared/
    project.md
    conventions.md
    ...
  workflows/
    streamerbot-script-write/
      WORKFLOW.md
    ...
  skills/
    validate-cs-script/
      SKILL.md
    ...
```

## Content stubs

### `.agents/ENTRY.md`

(full proposed content)

### `.agents/_shared/<file>.md`

(one section per file — purpose + outline of sections + which old files merge in)

### `.agents/workflows/<workflow>/WORKFLOW.md`

(one section per workflow — full content stub from Phase 1 spec, formatted as the file would appear)

### `.agents/skills/<skill>/SKILL.md`

(one section per skill — frontmatter + body)

## Cross-reference check

- Workflows: <n>. All skill refs resolve: yes/no. (If no, list mismatches.)
- Skills: <n>. All `_shared/` refs resolve: yes/no. (If no, list mismatches.)

## Open questions

- ...
```

## When done

1. Append to `STATUS.md` under `## Notes`: `- 3 complete: <n> shared / <n> workflows / <n> skills designed; cross-ref OK.`
2. Tick `[x] phase-3-tree-design.md` in STATUS.md.
3. Next: `phase-4-migration-map.md`.
