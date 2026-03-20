# AGENTS.md

## Start Here

Read `.agents/ENTRY.md` — it is the universal entry point for all agents (Claude, Pi, or any future agent).

`.agents/ENTRY.md` contains:
- Project description
- Full roles table
- Navigation guide to shared context and role skill trees

## Quick Role Routing

<!-- GENERATED:agents-quick-role-routing:start -->
| You're working on | Role | Agent Tree |
|---|---|---|
| Any `.cs` script under `Actions/` | `streamerbot-dev` | `.agents/roles/streamerbot-dev/` |
| LotAT C# engine / story pipeline | `lotat-tech` | `.agents/roles/lotat-tech/` |
| LotAT adventure content / lore | `lotat-writer` | `.agents/roles/lotat-writer/` |
| Art generation / diffusion prompts | `art-director` | `.agents/roles/art-director/` |
| Chat text, titles, canon, content strategy | `brand-steward` | `.agents/roles/brand-steward/` |
| Stream interaction apps | `app-dev` | `.agents/roles/app-dev/` |
| Validation, sync, change summary, tooling | `ops` | `.agents/roles/ops/` |
<!-- GENERATED:agents-quick-role-routing:end -->

## Coordination

Before starting any task: **check `WORKING.md`** for active agent work and file conflicts.

After completing any code task: load `ops-change-summary` to produce the paste targets and validation checklist the operator needs.

## Pi Routing

Pi's skill tree is at `.pi/skills/`. Pi uses `.pi/skills/README.md` for its routing table. Pi meta-skills are exposed through the flat wrappers `meta`, `meta-agents-navigate`, and `meta-agents-update`.

## Project Domains

| Domain | Path | Contains |
|---|---|---|
| Actions | `Actions/` | Streamer.bot C# runtime scripts |
| Tools | `Tools/` | Local utilities, validators, API helpers |
| Creative | `Creative/` | Brand docs, character art, worldbuilding, lore |
| Docs | `Docs/` | Architecture, workflow, onboarding |
| Agent Tree | `.agents/` | Shared role/skill knowledge tree |

## Key References

| File | Purpose |
|---|---|
| `WORKING.md` | Active work, task queue, conflict registry — check first |
| `Actions/SHARED-CONSTANTS.md` | Canonical global variable, OBS source, timer names |
| `Actions/HELPER-SNIPPETS.md` | Reusable C# patterns — copy verbatim |
| `Creative/Brand/BRAND-IDENTITY.md` | Brand vision, mission, values, neurodivergent metaphor |
| `Creative/Brand/CHARACTER-CODEX.md` | Canonical character identities |
| `Docs/AGENT-WORKFLOW.md` | When to commit direct vs. use a branch; merge review template |
| `Docs/ONBOARDING.md` | Start here if new to the project |
