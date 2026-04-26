# Phase 0c — Workflow seed list

## Before you start

1. Read `Projects/agent-retooling/PLAN.md`.
2. Read `Projects/agent-retooling/STATUS.md` and confirm `phase-0c-workflow-seeds.md` is not already checked off.

## Mission

Draft a candidate list of workflows. Phase 1 turns this into the formal catalog. This is rough — aim for ~12–20 candidate workflows that together cover the scope of work the current 9 roles handle.

## Inputs

- `Projects/agent-retooling/theory.md` — operator's prose hints at workflows.
- `.agents/ENTRY.md` Roles table.
- `.agents/routing-manifest.json` `quick_routing` table.
- High-level scan of `.agents/roles/<role>/role.md` for each role — what tasks does the role describe?
- `Actions/`, `Apps/`, `Tools/`, `Creative/`, `Docs/` directory listings (one level deep, do not recurse) — what kinds of work happen against these areas?
- Top-level `README.md` Docs Index for hints at feature areas.

## Required findings

For each candidate workflow, write a one-paragraph stub with:

- **name** — kebab-case, action-shaped (e.g. `streamerbot-script-write`, not `scripts`)
- **trigger** — what task or operator request invokes this workflow
- **rough scope** — what gets produced or changed
- **current roles spanned** — which existing roles touch this work today

Keep workflows **small**. PLAN.md decision: major initiatives are sequences of chained workflows, not one giant workflow. If a candidate feels like "build a whole new feature end to end", split it into smaller workflows.

## Seed examples (not the full list — derive the rest)

- `streamerbot-script-write` — write or edit a `.cs` action script. Spans streamerbot-dev, ops.
- `streamerbot-trigger-research` — investigate Streamer.bot trigger options for a new feature. Spans streamerbot-dev, possibly brand-steward.
- `content-pipeline-run` — run the content-repurposer pipeline end to end. Spans content-repurposer.
- `content-pipeline-tune` — adjust pipeline parameters or prompts after a run. Spans content-repurposer.
- `lotat-story-generate` — produce a new LotAT adventure pre-stream. Spans lotat-writer, lotat-tech.
- `lotat-engine-edit` — modify the LotAT C# engine. Spans lotat-tech, streamerbot-dev, ops.
- `art-asset-generate` — produce a diffusion-prompted asset for stream. Spans art-director, brand-steward.
- `brand-copy-write` — produce on-brand chat text, social copy, or stream titles. Spans brand-steward.
- `ops-paste-and-validate` — operator-side post-edit paste + smoke test ritual. Spans ops.

## Output format — `workflow-seeds.md`

```markdown
# Workflow Seed List

Candidate workflows for the new agent tree. Phase 1 formalizes these. Do not consider this list authoritative.

## Workflows

### <workflow-name>

- **Trigger:** ...
- **Scope:** ...
- **Current roles spanned:** ...

(repeat for each candidate)

## Cluster hints (informal)

Group candidates by domain — `streamerbot`, `lotat`, `content`, `brand`, `art`, `app`, `ops`, etc. Phase 1's generator uses these clusters to spawn per-cluster spec prompts.

- **streamerbot:** workflow-a, workflow-b
- **lotat:** workflow-c
- ...

## Open questions

- (candidates you weren't sure should be split or merged)
```

## When done

1. Append to `STATUS.md` under `## Notes`: `- 0c complete: <count> candidate workflows in <count> clusters.`
2. Tick `[x] phase-0c-workflow-seeds.md` in STATUS.md.
3. Phase 0c is independent of 0a, 0b, 0d.
