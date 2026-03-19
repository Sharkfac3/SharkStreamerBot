---
name: skills
description: Overview of Pi's role-based skill tree and routing conventions for this project.
---

# Pi Skills

Pi uses a role-based skill tree. Skills are organized by job role — each role maps to a domain in `.agents/roles/`. Pi's SKILL.md files are thin routing wrappers; the full knowledge content lives in `.agents/`.

## How to Use

1. Identify the role for your task from the routing table below
2. Load the role's SKILL.md — it will point you to `.agents/roles/<role>/`
3. Always load `role.md` and `skills/core.md` from that agent role first
4. Navigate into sub-skills only as the task demands
5. For meta-operations on the agent tree itself: load `meta/SKILL.md`

## Roles

| Role | SKILL.md | When to Activate |
|---|---|---|
| `streamerbot-dev` | `streamerbot-dev/SKILL.md` | Any `.cs` script work under `Actions/` |
| `lotat-tech` | `lotat-tech/SKILL.md` | LotAT engine, JSON schema, technical pipeline |
| `lotat-writer` | `lotat-writer/SKILL.md` | LotAT adventures, narrative, lore, worldbuilding |
| `art-director` | `art-director/SKILL.md` | Art generation, diffusion prompts, stream visuals |
| `brand-steward` | `brand-steward/SKILL.md` | Chat text, titles, canon review, content strategy |
| `ops` | `ops/SKILL.md` | Change summaries, sync, validation, tooling |
| `meta` | `meta/SKILL.md` | Navigating or updating the `.agents/` tree |

## Routing Table

| Task | Load |
|---|---|
| Any `.cs` script work | `streamerbot-dev/SKILL.md` |
| Squad mini-game (Clone, Duck, Pedro, Toothless) | `streamerbot-dev/SKILL.md` → `streamerbot-dev/squad/SKILL.md` |
| Commander scripts | `streamerbot-dev/SKILL.md` → `streamerbot-dev/commanders/SKILL.md` |
| Twitch events, bits, channel points, hype train | `streamerbot-dev/SKILL.md` → `streamerbot-dev/twitch/SKILL.md` |
| Voice command mode/scene scripts | `streamerbot-dev/SKILL.md` → `streamerbot-dev/voice-commands/SKILL.md` |
| LotAT C# engine work | `streamerbot-dev/SKILL.md` + `lotat-tech/SKILL.md` |
| LotAT story JSON or schema | `lotat-tech/SKILL.md` |
| Write a new LotAT adventure | `lotat-writer/SKILL.md` |
| Review story for canon | `lotat-writer/SKILL.md` → `lotat-writer/canon-guardian/SKILL.md` |
| Art generation / diffusion prompts | `art-director/SKILL.md` |
| Chat bot text, stream titles, announcements | `brand-steward/SKILL.md` |
| Canon audit (new characters, lore) | `brand-steward/SKILL.md` → `brand-steward/canon-guardian/SKILL.md` |
| Story tied to a specific build session | `brand-steward/SKILL.md` → `brand-steward/content-strategy/SKILL.md` |
| Sync to Streamer.bot | `ops/SKILL.md` → `ops/sync/SKILL.md` |
| After any code change | `ops/change-summary/SKILL.md` ← always terminal |
| Run validation | `ops/SKILL.md` → `ops/validation/SKILL.md` |
| Navigate .agents/ tree | `meta/agents-navigate/SKILL.md` |
| Add/update .agents/ content | `meta/agents-update/SKILL.md` |

## Adding a New Role

1. Follow `meta/agents-update/SKILL.md` to add the role to `.agents/`
2. Create `.pi/skills/<new-role>/SKILL.md` — thin wrapper pointing to `.agents/roles/<new-role>/`
3. Add a row to this README's routing table
4. Update `AGENTS.md` at repo root

## Naming Rules

- Role folder names: lowercase, hyphens, match `.agents/roles/<role>` naming
- Sub-skill folders: match the sub-skill path in `.agents/`
- SKILL.md frontmatter `name` field: `<role>` or `<role>/<sub-skill>`
