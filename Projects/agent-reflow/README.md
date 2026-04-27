# Agent Reflow

Ephemeral migration project. Reshapes agent scaffolding (`.agents/`, `retired Pi skill mirror/`, `Docs/`, root agent docs) into agent-agnostic, co-located, workflow-aware tree. Folder deletes at end.

## Mission

Solve documented scaffolding issues by running a series of Pi prompts in order. Each prompt is self-contained, leaves a handoff for the next, and gates on a validator once foundation lands.

## Problems Being Solved

1. Hidden progressive disclosure — domain knowledge lives in `.agents/roles/<role>/skills/<feature>/` instead of next to code that needs it
2. Roles model mixes domain + workflow — workflows like `canon-guardian` duplicated across roles
3. Two-tree adapter — `retired Pi skill mirror/` mirrors `.agents/roles/`, every leaf written twice
4. 4-source routing drift — `legacy-v1-routing-manifest`, `.agents/ENTRY.md`, `AGENTS.md`, `retired Pi skill mirror/README.md` all encode same table; sync script covers only one
5. Validator only syncs role table — does not check link integrity, folder coverage, stub presence
6. Migrated alias corpses — 14 alias folders in `retired Pi skill mirror/` for old names
7. Cross-role duplication — `canon-guardian` exists under `brand-steward` AND `lotat-writer`
8. Domain-to-role gaps — `Actions/Destroyer/`, `Actions/XJ Drivethrough/`, `Actions/Intros/`, `Actions/Overlay/`, `Actions/Rest Focus Loop/` have no skill leaf
9. `app-dev` collapses three different runtimes (info-service, production-manager, stream-overlay)
10. role.md + core.md redundancy at every role
11. `_shared/` not auto-loaded by either agent
12. WORKING.md coordination drifts (manual-only, last 10 entries all "uncommitted")
13. Adding new role = 5+ touch points, friction discourages coverage
14. `Docs/` folder may belong co-located with domains it documents

## Target Shape (Ratified in Phase B)

To be confirmed in `prompts/05-design-target-shape.md`. Provisional:

- Single source of truth for routing (manifest v2)
- Skill content co-located with code it describes (per-domain `CLAUDE.md` / `AGENTS.md`)
- Workflow layer separate from domain knowledge
- Agent-agnostic naming — no Pi-specific wrapper tree
- Validator covers link integrity, folder coverage, stub presence, drift detection

## Ground Rules

- **Pi runs every prompt.** Operator copy/pastes prompt text into Pi manually
- **No git operations by agent.** Operator commits manually. Agent finishes with dirty tree
- **One prompt at a time, serial.** No parallel prompt execution
- **No other agent work** in `.agents/`, `.pi/`, `Docs/`, `Projects/agent-reflow/` while reflow series is mid-flight
- **Self-contained prompts.** Pi must be able to execute each prompt cold by reading prompt file + prior handoff. No reliance on `retired Pi skill mirror/` routing wrappers
- **Ephemeral folder.** `Projects/agent-reflow/` deletes at end. Use freely as scratch space

## Folder Layout

```
Projects/agent-reflow/
  README.md                       this file
  PLAN.md                         phase table + status
  findings/                       info-gathering output
  handoffs/                       per-prompt handoff
  prompts/                        prompt files Pi runs in order
```

## Prompt File Format

Every prompt file follows this structure:

```
# Prompt NN — <slug>

## Agent
Pi (manual copy/paste by operator)

## Preconditions
- Read Projects/agent-reflow/handoffs/<N-1>-*.handoff.md (if exists)
- Read Projects/agent-reflow/findings/<relevant>.md (if relevant)
- <repo state expectations>

## Scope
- <files/dirs touched>

## Out-of-scope
- No git operations (operator commits manually)
- No edits outside listed scope
- <other forbidden actions>

## Steps
1. ...
2. ...

## Validator / Acceptance
- <command or manual check>
- <expected output>

## Handoff
Write Projects/agent-reflow/handoffs/NN-<slug>.handoff.md per the handoff template.
```

## Handoff Format

Every prompt writes one handoff file:

```
# Prompt NN handoff — <slug>

Date: YYYY-MM-DD
Agent: pi

## State changes
- <files created / modified / deleted>

## Findings appended
- findings/<file>.md: <summary of what was added>

## Assumptions for prompt N+1
- <fact prompt N+1 may rely on>

## Validator status
- Last run: <command + exit code + key output>

## Open questions / blockers
- <items needing operator decision before next prompt runs>

## Next prompt entry point
- Read this file first
- Then read findings/<relevant>.md
- Then proceed per prompts/<N+1>.md
```

## Acceptance Criteria for Whole Reflow

1. All 14 problems above resolved or explicitly deferred with rationale
2. Single routing source (manifest v2) — no duplicate role/skill tables
3. Validator green: link integrity 100%, folder coverage 100%, no orphan stubs
4. `retired Pi skill mirror/` mirror deleted
5. Domain knowledge co-located (per-domain agent docs in `Actions/<feature>/`, `Apps/<app>/`, etc.)
6. Workflow layer extracted (canon-guardian, change-summary, sync, etc.)
7. `Docs/` folder either folded into domain folders or reduced to repo-wide concerns only
8. `Projects/agent-reflow/` deleted (or moved to `Docs/Archive/` only if operator opts to keep history — default = delete)

## Disposition

Final prompt deletes `Projects/agent-reflow/` after operator confirms reflow merged and stable. Default = delete. Optional = move to `Docs/Archive/agent-reflow-2026-04/` if operator wants record.

## Status

See `PLAN.md` for live phase table.
