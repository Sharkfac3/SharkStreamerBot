# SharkStreamerBot — Agent Entry Point

## What This Project Is

The technical and creative infrastructure for SharkFac3's live stream — where an R&D company building off-road racing products operates in public. The stream is both the workspace and the marketing engine: products are developed live, knowledge is shared freely to establish authority, and a community is built that will eventually become the customer base.

The project spans Streamer.bot runtime actions, interactive entertainment features (mini-games, commanders, Legends of the ASCII Temple), brand identity, art generation, content repurposing, and future product documentation. Entertainment layers keep viewers engaged during real R&D work, and clip-worthy moments feed a content pipeline that drives discovery and growth.

## How to Navigate

1. Read `_shared/project.md` — project domains, priorities, scope rules.
2. Identify your role from the table below.
3. Read `roles/<role>/role.md` — confirms scope, trigger conditions, skill load order.
4. Load `roles/<role>/skills/core.md` — always required for that role.
5. Navigate into sub-skill folders only as the task demands.
6. Before starting any task: check `WORKING.md` at repo root for active conflicts.
7. After completing a task: add living notes to `roles/<role>/context/` if you discovered something future agents should know.

## Roles

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

## Shared Context

`_shared/` contains cross-role knowledge all agents may need:
- `project.md` — domains, priority order, scope rules
- `conventions.md` — git, naming, file routing
- `coordination.md` — WORKING.md protocol, conflict rules
- `mixitup-api.md` — Mix It Up API payload spec

## Adding a New Role

Copy `roles/_template/` to `roles/<new-role>/`. Follow the template. Update this file's Roles table.
