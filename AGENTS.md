---
id: root-agent-doc
type: shared
description: Root universal agent entry and generated quick routing surface.
status: active
owner: ops
generated: false
---

# SharkStreamerBot — Agent Entry

## Start Here

Read [.agents/ENTRY.md](.agents/ENTRY.md) first. It is the manifest-backed universal entry point for Claude, Pi, and future agents.

Then:

1. Check [WORKING.md](WORKING.md) for active work and file conflicts.
2. Pick the correct role or local domain route from the routing summary below.
3. Read the role overview and any local `AGENTS.md` guide for the folder you will edit.
4. After changed files, follow [change-summary](.agents/workflows/change-summary.md) and relevant validation/sync workflows.

## Quick Role Routing

<!-- GENERATED:agents-quick-role-routing:start -->
| You're working on | Role | Agent Tree |
|---|---|---|
| Any `.cs` script under `Actions/` | `streamerbot-dev` | `.agents/roles/streamerbot-dev/role.md` |
| LotAT C# engine / story pipeline | `lotat-tech` | `.agents/roles/lotat-tech/role.md` |
| LotAT adventure content / lore | `lotat-writer` | `.agents/roles/lotat-writer/role.md` |
| Art generation / diffusion prompts | `art-director` | `.agents/roles/art-director/role.md` |
| Chat text, titles, canon, content strategy | `brand-steward` | `.agents/roles/brand-steward/role.md` |
| Short-form clips, captions, platform formatting, or content-pipeline tooling | `content-repurposer` | `.agents/roles/content-repurposer/role.md` |
| Stream interaction apps | `app-dev` | `.agents/roles/app-dev/role.md` |
| Product docs, specs, knowledge articles, and customer-facing product content | `product-dev` | `.agents/roles/product-dev/role.md` |
| Validation, sync, change summary, tooling | `ops` | `.agents/roles/ops/role.md` |
<!-- GENERATED:agents-quick-role-routing:end -->

## Project Domains

| Domain | Path | Local routing |
|---|---|---|
| Actions | [Actions/](Actions/) | Streamer.bot C# runtime scripts and folder-local action guides. |
| Apps | [Apps/](Apps/) | Standalone TypeScript apps and app-local guides. |
| Tools | [Tools/](Tools/) | Local utilities, validators, Mix It Up helpers, and sync tooling. |
| Creative | [Creative/](Creative/) | Brand, art, marketing, worldbuilding, and creative guides. |
| Agent Tree | [.agents/](.agents/) | Manifest, role overviews, workflows, and shared agent context. |

## Coordination

Before starting: read [WORKING.md](WORKING.md) and follow [coordination](.agents/workflows/coordination.md).

After code changes: include paste targets, setup steps, and validation output via [change-summary](.agents/workflows/change-summary.md).

## Key References

| File | Purpose |
|---|---|
| [Actions/SHARED-CONSTANTS.md](Actions/SHARED-CONSTANTS.md) | Canonical globals, OBS sources, timer names. |
| [Actions/Helpers/AGENTS.md](Actions/Helpers/AGENTS.md) | Reusable C# patterns. |
| [Creative/Brand/BRAND-IDENTITY.md](Creative/Brand/BRAND-IDENTITY.md) | Brand vision, mission, values, and metaphor. |
| [Creative/Brand/CHARACTER-CODEX.md](Creative/Brand/CHARACTER-CODEX.md) | Canonical character identities. |
| [Tools/AgentTree/AGENTS.md](Tools/AgentTree/AGENTS.md) | Manifest/validator route for agent-tree tooling. |
