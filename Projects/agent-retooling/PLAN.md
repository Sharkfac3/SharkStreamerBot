# Agent Retooling — Project Plan

This folder is the staging area for retooling the SharkStreamerBot agent scaffolding from a role-based tree to a workflow-based tree. The folder is ephemeral and gets deleted at the end of the retool.

## Goals

Replace `.agents/roles/*` and `.pi/skills/*` with a workflow + atomic skill model. End drift caused by per-role context folders and the three-tier mirror (`.agents/roles/`, `.pi/skills/`, `routing-manifest.json`).

## Locked design decisions

These were settled before Phase 0 began. Do not relitigate inside any phase.

- **Tree shape.**
  - `.agents/skills/<skill>/SKILL.md` — atomic, agent-agnostic skills in Agent Skills format (agentskills.io). One capability per skill. Pi discovers `.agents/skills/` natively; Claude loads via the Skill tool.
  - `.agents/workflows/<workflow>/WORKFLOW.md` — small playbooks. Each declares: trigger, inputs, ordered steps, exit criteria, which atomic skills it loads.
  - `.agents/_shared/` — canonical reference docs.
  - `.agents/ENTRY.md` — workflow + skill index, agent-agnostic.
  - `.pi/` — deleted entirely (Pi searches `.agents/skills/` natively).
- **Multi-agent.** Both Pi coding agent and Claude Code use this tree. Skills and workflows are agent-agnostic. Workflows do not declare a required agent. Hand-off mid-workflow is out of scope; chaining small workflows is the substitute.
- **Workflow size.** Workflows stay small. Major initiatives are sequences of chained workflows, not one giant workflow.
- **Anti-drift rule.** New discoveries do not accumulate in `context/` folders. Either the discovery is reusable (edit the canonical `_shared/` doc directly on discovery) or it is one-off (stays in the conversation / PR description, never written to scaffolding). No `context/` subdirs in the new tree.
- **Migration style.** Hard cutover. Old tree gets gutted, new tree gets built. No parallel trees.
- **Project folder.** All transitional artifacts live under `Projects/agent-retooling/`. Deleted at end of retool.

## Scope

- **In scope.** `.agents/`, `.pi/`, top-level `AGENTS.md`, `CLAUDE.md`, `WORKING.md`, `Docs/` (cleanup audit).
- **Out of scope.** `humans/` (deferred), `Actions/`, `Apps/`, `Tools/`, `Creative/`, `Assets/`, other `Projects/`.

## Phases (chunked into small self-perpetuating prompts)

The retool runs as a chain of small prompts. Each prompt:

- Reads `PLAN.md` (this file) and `STATUS.md` first.
- Reads any prior outputs it depends on.
- Writes its deliverable into `Projects/agent-retooling/`.
- Ticks its checkbox in `STATUS.md` and adds a one-line note.
- If the next phase's structure depends on this phase's data, generates the next batch of prompts.

Pre-writable prompts are written up front. Generator prompts (Phase 1, Phase 5) are written up front but spawn data-dependent child prompts at runtime.

| Phase | Prompt files | Deliverable |
|---|---|---|
| 0 — Discovery | `phase-0a-audit.md`, `phase-0b-pi-format.md`, `phase-0c-workflow-seeds.md`, `phase-0d-salvage.md` | `audit.md`, `pi-skill-format.md`, `workflow-seeds.md`, `salvage-list.md` |
| 1 — Workflow catalog | `phase-1-generator.md` (spawns `phase-1a-spec-<cluster>.md`) | `workflow-clusters.md`, `workflow-spec-<cluster>.md` per cluster |
| 2 — Skill atomization | `phase-2-skill-extract.md` | `skill-inventory.md` |
| 3 — New tree design | `phase-3-tree-design.md` | `new-tree.md` |
| 4 — Migration map | `phase-4-migration-map.md` | `migration-map.md`, `migration-order.md` |
| 5 — Execute migration | `phase-5-generator.md` (spawns `phase-5a-chunk-<n>.md`) | actual `.agents/` rebuilt; `.pi/` deleted |
| 6 — Smoke test + docs | `phase-6a-smoke-plan.md`, `phase-6b-smoke-tests.md`, `phase-6c-doc-catchup.md`, `phase-6d-cleanup.md` | smoke report, top-level doc updates, retool folder deleted |

Templates (referenced by generators, not run directly): `phase-1-template.md`, `phase-5-chunk-template.md`.

## Working rules across phases

- **Read-only against `.agents/` and `.pi/` until Phase 5.** Phases 0–4 only write into `Projects/agent-retooling/`.
- **No commits.** Operator commits manually after reviewing each phase.
- **Surface unknowns.** If a phase uncovers a question that changes later-phase design, write it under an `## Open questions` heading at the bottom of that phase's deliverable rather than guessing.
- **Caveman mode is operator preference for chat, not for these docs.** Phase prompts and deliverables are written in normal English so cold agents (Pi or Claude) read them cleanly.

## Status

See `STATUS.md` for current per-prompt progress.
