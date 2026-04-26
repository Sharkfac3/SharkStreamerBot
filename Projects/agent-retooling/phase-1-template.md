# Phase 1 — Per-cluster spec prompt template

This file is **not run directly**. `phase-1-generator.md` clones it once per workflow cluster, filling in `<CLUSTER>` and the workflow list.

When generated, the cloned file goes to `Projects/agent-retooling/phase-1a-spec-<cluster>.md`.

---

## Template body (everything below this line is the per-cluster prompt)

# Phase 1a — Workflow spec for cluster: `<CLUSTER>`

## Before you start

1. Read `Projects/agent-retooling/PLAN.md`.
2. Read `Projects/agent-retooling/STATUS.md` — confirm `phase-1-generator.md` is checked off and your prompt's checkbox exists.
3. Read `Projects/agent-retooling/workflow-seeds.md` and `Projects/agent-retooling/workflow-clusters.md`.

## Workflows in this cluster

`<WORKFLOW_LIST>` — formatted as a bullet list of workflow names from the cluster.

## Mission

Produce a formal spec for every workflow listed above. Output `Projects/agent-retooling/workflow-spec-<CLUSTER>.md`.

## Inputs to read for each workflow

- The seed entry in `workflow-seeds.md`.
- Any current `.agents/roles/<role>/skills/...` files relevant to this workflow's domain.
- `theory.md` for context.
- Domain folders (e.g. `Actions/`, `Apps/`, `Creative/`) at top level — what artifacts does this workflow produce or modify?

## Required spec fields per workflow

```markdown
### <workflow-name>

**Trigger.** What operator request or upstream event invokes this workflow.

**Inputs.** What the workflow needs to start (e.g. a `.cs` file path, a story prompt, a clip timestamp).

**Steps.** Ordered list of actions. Each step is one sentence. Steps name the atomic skill they use, in the form `[skill: <skill-name>]`. Skill names are kebab-case and may not exist yet — Phase 2 reconciles the full skill set.

**Exit criteria.** What "done" looks like — observable artifacts or operator-visible state.

**Chains to.** Other workflows commonly run after this one. Use kebab-case names. Empty if this is a leaf workflow.

**Out of scope.** What this workflow explicitly does not do (helps anti-drift).

**Operator notes.** Any manual step the operator must perform that the agent cannot. (Examples: paste into Streamer.bot, restart OBS source.)
```

## Anti-drift reminders

- Keep workflows **small**. If a workflow has more than ~6 steps, split it.
- Workflows are **agent-agnostic** — do not name Pi or Claude in the spec.
- Skill references like `[skill: validate-cs-script]` are fine even if the skill doesn't exist yet. Phase 2 reconciles.

## Output format — `workflow-spec-<CLUSTER>.md`

```markdown
# Workflow Specs — Cluster: <CLUSTER>

(one H3 block per workflow, matching the schema above)

## Skills referenced (raw)

Flat list of every `[skill: ...]` reference used in the specs above. Phase 2 dedupes across clusters.

- skill-name-1
- skill-name-2
- ...

## Open questions

- ...
```

## When done

1. Append to `STATUS.md` under `## Notes`: `- 1a-<CLUSTER> complete: <count> workflows, <count> skill refs.`
2. Tick your prompt's checkbox in STATUS.md.
3. When **all** per-cluster specs are checked off, the next prompt is `phase-2-skill-extract.md`.
