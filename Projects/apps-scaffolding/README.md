# Apps Agent Scaffolding Project

Improve agent navigation and understanding across the `Apps/` domain (and other domains missing top-level AGENTS.md guides).

## Problem

`Apps/` has 3 interdependent TypeScript apps with individual AGENTS.md guides, but no domain-level entrypoint. An agent landing at `Apps/` has no routing surface, no dependency graph, no shared conventions doc. `Actions/` has this solved with `Actions/AGENTS.md` — Apps does not.

`Tools/` (6 tools) and `Creative/` (4 areas) have the same gap.

## Phases

| Phase | Prompt | Deliverable | Depends on |
|---|---|---|---|
| 1 | [phase-1-apps-domain-guide.md](phase-1-apps-domain-guide.md) | `Apps/AGENTS.md` — domain entrypoint | — |
| 2a | [phase-2a-tools-domain-guide.md](phase-2a-tools-domain-guide.md) | `Tools/AGENTS.md` | Phase 1 |
| 2b | [phase-2b-creative-domain-guide.md](phase-2b-creative-domain-guide.md) | `Creative/AGENTS.md` | Phase 1 |
| 3 | [phase-3-consistency-pass.md](phase-3-consistency-pass.md) | Cross-domain audit + root routing updates | All above |

## How to Run

Each phase is a self-contained prompt for a Pi coding agent chat (ChatGPT-backed). Each prompt is a new chat session — no prior context assumed.

**Run order:** Phase 1 first. Then 2a and 2b can run in parallel. Phase 3 last (needs all domain guides to exist).

## Prompt Design

Each prompt includes:
- Pre-digested inventories (agent does not need to discover what exists)
- Exact frontmatter to use (validator-enforced naming rules)
- Required sections list with order and length targets
- Explicit "do NOT add sections beyond this list" constraint
- Validator check descriptions so the agent can interpret and fix failures
- Reference to `Actions/AGENTS.md` as the structural template

## Validation

After each phase and after all phases, run from repo root:

```bash
python Tools/AgentTree/validate.py
```

The validator checks: frontmatter fields, folder coverage, link integrity, naming conventions, and ID uniqueness.
