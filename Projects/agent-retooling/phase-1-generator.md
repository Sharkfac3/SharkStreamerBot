# Phase 1 — Generator: cluster workflows and spawn per-cluster prompts

## Before you start

1. Read `Projects/agent-retooling/PLAN.md`.
2. Read `Projects/agent-retooling/STATUS.md` — confirm `phase-0c-workflow-seeds.md` and `phase-0d-salvage.md` are both checked off. If not, stop and wait.

## Mission

1. Cluster the candidate workflows from `workflow-seeds.md` into formal cluster groups.
2. For each cluster, generate a per-cluster spec prompt by cloning `phase-1-template.md`.
3. Update `STATUS.md` with the new prompt checkboxes.

## Inputs

- `Projects/agent-retooling/workflow-seeds.md`
- `Projects/agent-retooling/salvage-list.md`
- `Projects/agent-retooling/phase-1-template.md` (template file — do not run, only clone)

## Step 1 — Cluster the workflows

Pick a small number of clusters (target 4–7). Cluster by domain. Use the "Cluster hints" section in `workflow-seeds.md` as a starting point. Examples (not prescriptive):

- `streamerbot` — `.cs` script work, triggers, runtime actions
- `lotat` — adventure stories, engine, narrative
- `content` — content pipeline run, tune, clip selection, captions
- `brand` — copy, social, canon
- `art` — asset generation, prompt design
- `app` — TypeScript apps under `Apps/`
- `ops` — paste, validate, change summary

A workflow may belong to exactly one cluster. If a workflow plausibly belongs to two, pick the cluster whose owning role would have produced it most often.

Output `Projects/agent-retooling/workflow-clusters.md`:

```markdown
# Workflow Clusters

| cluster | workflows |
|---|---|
| streamerbot | streamerbot-script-write, streamerbot-trigger-research, ... |
| ...
```

## Step 2 — Generate per-cluster prompts

For each cluster, create `Projects/agent-retooling/phase-1a-spec-<cluster>.md` by:

1. Reading `phase-1-template.md`.
2. Removing everything above and including the line `## Template body (everything below this line is the per-cluster prompt)`.
3. Substituting `<CLUSTER>` with the cluster name (kebab-case).
4. Substituting `<WORKFLOW_LIST>` with the bullet list of workflows for that cluster, formatted as:

   ```
   - workflow-a
   - workflow-b
   ```

5. Saving the result.

## Step 3 — Update STATUS.md

Under `## Phase 1 — Workflow Catalog`, add one new checkbox per generated prompt:

```
- [ ] `phase-1a-spec-<cluster>.md` → `workflow-spec-<cluster>.md`
```

## When done

1. Append to `STATUS.md` under `## Notes`: `- 1-generator complete: <count> clusters, <count> per-cluster prompts generated.`
2. Tick `[x] phase-1-generator.md` in STATUS.md.
3. The per-cluster prompts can run in parallel. When **all** are checked off, the next prompt is `phase-2-skill-extract.md`.
