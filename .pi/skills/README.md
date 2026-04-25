---
name: skills
description: Overview of Pi's role-based skill tree and routing conventions for this project.
---

# Pi Skills

Pi uses a role-based skill tree. The full knowledge hierarchy lives in `.agents/roles/`. The `.pi/skills/` layer is a **flat routing and compatibility layer** that exposes Pi-safe wrapper names while preserving role ownership.

## How to Use

1. Identify the role for your task from the routing table below
2. Load the role's SKILL.md — it will point you to `.agents/roles/<role>/`
3. Always load `role.md` and `skills/core.md` from that agent role first
4. Load role-qualified flat sub-skill wrappers only as the task demands
5. For meta-operations on the agent tree itself: load `meta/SKILL.md`

## Naming Convention

- `.agents/` remains hierarchical and is the source of truth
- `.pi/skills/` wrapper names are flat and Pi-safe
- Role wrapper pattern: `<role>`
- Canonical sub-skill wrapper pattern: `<role>-<subskill>`
- Older flat names may remain as migrated compatibility aliases, but role-qualified flat names are canonical

## Roles

This table is sourced from the routing contract in `.agents/routing-manifest.json` and machine-validated against `.agents/ENTRY.md`. For each role below, the Pi role wrapper lives at `.pi/skills/<role>/SKILL.md`.

| Role | Folder | When to Activate |
|---|---|---|
| `streamerbot-dev` | `roles/streamerbot-dev/` | Any `.cs` script work under `Actions/` |
| `lotat-tech` | `roles/lotat-tech/` | LotAT story pipeline — C# engine, JSON schema, technical implementation |
| `lotat-writer` | `roles/lotat-writer/` | LotAT narrative — adventure design, lore, worldbuilding, story content |
| `art-director` | `roles/art-director/` | Diffusion model prompts, character art, stream visuals |
| `brand-steward` | `roles/brand-steward/` | Any public-facing output — chat text, titles, marketing, canon review |
| `content-repurposer` | `roles/content-repurposer/` | Short-form content repurposing — clip selection, captions, content calendars, platform formatting, and content-pipeline tooling |
| `app-dev` | `roles/app-dev/` | Stream overlay ecosystem (broker, Phaser overlay, web apps) — TypeScript under `Apps/` |
| `product-dev` | `roles/product-dev/` | Product documentation, technical knowledge articles, specifications, and future customer-facing content for stream-developed R&D products |
| `ops` | `roles/ops/` | Validation, sync workflow, change summaries, tooling |

## Meta Wrappers

Meta wrappers are Pi-only navigation helpers for the `.agents/` tree itself. They are not `.agents/roles/*` roles.

| Wrapper | Purpose |
|---|---|
| `meta/SKILL.md` | Meta entry point for agent-infrastructure work |
| `meta-agents-navigate/SKILL.md` | Navigate and read the `.agents/` tree |
| `meta-agents-update/SKILL.md` | Add or update `.agents/` content |

## Routing Table

| Task | Load |
|---|---|
| Any `.cs` script work | `streamerbot-dev/SKILL.md` |
| Squad mini-game (Clone, Duck, Pedro, Toothless) | `streamerbot-dev/SKILL.md` → `streamerbot-dev-squad/SKILL.md` |
| Commander scripts | `streamerbot-dev/SKILL.md` → `streamerbot-dev-commanders/SKILL.md` |
| Twitch events, bits, channel points, hype train | `streamerbot-dev/SKILL.md` → `streamerbot-dev-twitch/SKILL.md` |
| Voice command mode/scene scripts | `streamerbot-dev/SKILL.md` → `streamerbot-dev-voice-commands/SKILL.md` |
| LotAT C# engine work in `Actions/` | `streamerbot-dev/SKILL.md` → `streamerbot-dev-lotat/SKILL.md` + `lotat-tech/SKILL.md` when schema/pipeline context is needed |
| LotAT story JSON or schema | `lotat-tech/SKILL.md` |
| Write a new LotAT adventure | `lotat-writer/SKILL.md` |
| Review LotAT story for canon | `lotat-writer/SKILL.md` → `lotat-writer-canon-guardian/SKILL.md` |
| Art generation / diffusion prompts | `art-director/SKILL.md` |
| Chat bot text, stream titles, announcements | `brand-steward/SKILL.md` |
| Canon audit (new characters, franchise lore) | `brand-steward/SKILL.md` → `brand-steward-canon-guardian/SKILL.md` |
| Story tied to a specific build session | `brand-steward/SKILL.md` → `brand-steward-content-strategy/SKILL.md` |
| Short-form clips, captions, and content calendars | `content-repurposer/SKILL.md` |
| Content-pipeline tooling in `Tools/ContentPipeline/` | `content-repurposer/SKILL.md` → `content-repurposer-pipeline/SKILL.md` |
| Standalone stream interaction app work | `app-dev/SKILL.md` |
| info-service lookup, collection queries | `app-dev/SKILL.md` |
| production-manager admin UI | `app-dev/SKILL.md` |
| Product docs, specs, knowledge articles, and customer-facing product content | `product-dev/SKILL.md` |
| Sync to Streamer.bot | `ops/SKILL.md` → `ops-sync/SKILL.md` |
| After any code change | `ops-change-summary/SKILL.md` ← always terminal |
| Run validation | `ops/SKILL.md` → `ops-validation/SKILL.md` |
| Navigate .agents/ tree | `meta-agents-navigate/SKILL.md` |
| Add/update .agents/ content | `meta-agents-update/SKILL.md` |

## Adding a New Role

1. Follow `meta-agents-update/SKILL.md` to add the role to `.agents/`
2. Create `.pi/skills/<new-role>/SKILL.md` — thin wrapper pointing to `.agents/roles/<new-role>/`
3. Add a row to this README's routing table
4. Re-run `python3 Tools/StreamerBot/Validation/sync-routing-docs.py` so `AGENTS.md`, `.agents/ENTRY.md`, and this README stay aligned

## Adding a Pi Sub-Skill Wrapper

When a role gains a Pi-exposed sub-skill wrapper:

1. Keep the source-of-truth knowledge in `.agents/roles/<role>/skills/...`
2. Create a flat Pi wrapper at `.pi/skills/<role>-<subskill>/SKILL.md`
3. Point the wrapper at the appropriate `.agents/roles/<role>/skills/...` file or folder index
4. Update the parent role wrapper and this README routing table
5. If replacing an old name, keep a migrated flat alias wrapper if compatibility matters

## Compatibility Aliases

Older flat wrapper names may remain as migrated aliases for operator convenience and backward compatibility. This inventory is sourced from `.agents/routing-manifest.json` and machine-validated against it.

| Alias | Canonical wrapper |
|---|---|
| `change-summary` | `ops-change-summary` |
| `sync-workflow` | `ops-sync` |
| `brand-canon-guardian` | `brand-steward-canon-guardian` |
| `content-strategy` | `brand-steward-content-strategy` |
| `feature-commanders` | `streamerbot-dev-commanders` |
| `feature-squad` | `streamerbot-dev-squad` |
| `feature-twitch-integration` | `streamerbot-dev-twitch` |
| `feature-channel-points` | `streamerbot-dev-twitch` |
| `feature-hype-train` | `streamerbot-dev-twitch` |
| `feature-voice-commands` | `streamerbot-dev-voice-commands` |
| `streamerbot-scripting` | `streamerbot-dev` |
| `buildtools` | `ops` |
| `creative-art` | `art-director` |
| `creative-worldbuilding` | `lotat-writer` or `lotat-tech` |

## Naming Rules

- Role wrapper names: lowercase, hyphens, match `.agents/roles/<role>` naming
- Canonical sub-skill wrapper names: role-qualified flat names such as `ops-change-summary`
- Compatibility aliases, if kept, must also remain flat and Pi-safe
- Do not use slash-separated Pi skill names in frontmatter or wrapper references
