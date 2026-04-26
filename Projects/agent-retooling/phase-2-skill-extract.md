# Phase 2 — Skill atomization

## Before you start

1. Read `Projects/agent-retooling/PLAN.md`.
2. Read `Projects/agent-retooling/STATUS.md` — confirm every `phase-1a-spec-*.md` is checked off. If not, stop.

## Mission

Build the flat atomic skill library. Output `Projects/agent-retooling/skill-inventory.md`.

## Inputs

- Every `Projects/agent-retooling/workflow-spec-<cluster>.md` (from Phase 1).
- `Projects/agent-retooling/salvage-list.md` — files marked `keep` whose content is real skill knowledge are candidate skills.
- `Projects/agent-retooling/pi-skill-format.md` — confirms required frontmatter and shape.

## Steps

1. **Collect.** Walk every workflow spec. Pull every `[skill: <skill-name>]` reference into a flat list. Note which workflows reference each skill.
2. **Dedupe.** Merge near-duplicates (e.g. `validate-cs-script` and `cs-script-validate` collapse to one). Pick the clearer name. Record renames.
3. **Cross-reference salvage.** For each `keep`-tagged file in `salvage-list.md` whose content is technique knowledge, confirm a skill in the deduped list covers it. If not, add a new skill or flag in open questions.
4. **Spec each skill.** For every skill, produce the entry below.
5. **Sanity check.** Each skill should be:
   - Atomic — one capability, not a multi-step playbook
   - Agent-agnostic — works for both Pi and Claude
   - Useful in 2+ workflows OR clearly reusable, OR explicitly justified as workflow-private

## Required entry per skill

```markdown
### <skill-name>

**Purpose.** One sentence — what capability this skill provides.

**Used by workflows.** Comma-separated list of workflows that reference it.

**Inputs.** What the skill needs at invocation.

**Outputs.** What the skill produces.

**Source material.** Which existing files (`salvage-list.md` keeps) feed this skill's content. `(new)` if no existing source.

**Notes.** Constraints, gotchas, anti-patterns. Optional.
```

## Output format — `skill-inventory.md`

```markdown
# Atomic Skill Inventory

Flat list of skills referenced by Phase 1 workflow specs. One capability per skill. Agent-agnostic. Will become `.agents/skills/<skill>/SKILL.md` files in the new tree.

## Skills

(one H3 block per skill — sorted alphabetically by skill name)

## Renames applied during dedupe

| original ref | renamed to | reason |
|---|---|---|
| ... | ... | ... |

## Coverage check

- Workflows total: <n>
- Workflows fully covered by skills: <n>
- Workflows with unmapped steps: list them

## Open questions

- ...
```

## When done

1. Append to `STATUS.md` under `## Notes`: `- 2 complete: <n> atomic skills, <n> renames applied.`
2. Tick `[x] phase-2-skill-extract.md` in STATUS.md.
3. Next: `phase-3-tree-design.md`.
