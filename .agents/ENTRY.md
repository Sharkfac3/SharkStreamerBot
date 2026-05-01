---
id: agent-entry
type: shared
description: Central .agents entrypoint with manifest-backed routing summary.
status: active
owner: ops
generated: false
---

# SharkStreamerBot — Agent Entry Point

## What This Project Is

SharkStreamerBot is the technical and creative infrastructure for SharkFac3's live stream: an R&D company building off-road racing products in public. The repo supports real product-development streams with Streamer.bot actions, standalone apps, local tooling, brand/canon references, art direction, LotAT stories, and content repurposing.

## How to Navigate

1. Read [project.md](_shared/project.md) for repo-wide context and priorities.
2. Choose a role from the manifest-backed table below.
3. Read that role overview.
4. Read the local `AGENTS.md` file for the folder you will edit, if one exists.
5. Follow required workflows under [workflows/](workflows/).
6. Add living context only when the discovery is not better stored in a local domain guide.

## Roles

<!-- GENERATED:agents-roles:start -->
| Role | Folder | When to Activate |
|---|---|---|
| `streamerbot-dev` | `.agents/roles/streamerbot-dev/` | Streamer.bot C# runtime actions and Streamer.bot-side integrations under Actions/. |
| `lotat-tech` | `.agents/roles/lotat-tech/` | LotAT technical pipeline, C# engine, JSON schema, and story runtime implementation. |
| `lotat-writer` | `.agents/roles/lotat-writer/` | LotAT narrative design, adventure content, lore, worldbuilding, and reusable story elements. |
| `art-director` | `.agents/roles/art-director/` | Diffusion prompts, character art, stream visuals, and art-pipeline guidance. |
| `brand-steward` | `.agents/roles/brand-steward/` | Brand consistency for public-facing copy, titles, marketing, canon, voice, and community messaging. |
| `content-repurposer` | `.agents/roles/content-repurposer/` | Short-form clips, captions, platform formatting, content calendars, and content-pipeline tooling. |
| `app-dev` | `.agents/roles/app-dev/` | Standalone stream interaction apps, dashboards, overlays, brokers, and TypeScript app tooling. |
| `product-dev` | `.agents/roles/product-dev/` | Product documentation, technical knowledge articles, specs, and future customer-facing product content. |
| `ops` | `.agents/roles/ops/` | Validation, sync workflow, change summaries, local tooling, and operational repo maintenance. |
<!-- GENERATED:agents-roles:end -->

## Shared Context

| File | Purpose |
|---|---|
| [project.md](_shared/project.md) | Repo-wide business context, domains, priorities, and scope rules. |
| [conventions.md](_shared/conventions.md) | Repo-wide file routing and documentation conventions. |
| [coordination.md](_shared/coordination.md) | Compatibility pointer to the canonical coordination workflow. |
| [coordination](workflows/coordination.md) | Coordination workflow and conflict avoidance. |
| [change-summary](workflows/change-summary.md) | Terminal handoff format after changed files. |
| [sync](workflows/sync.md) | Streamer.bot paste/sync procedure. |
| [validation](workflows/validation.md) | Validator and check-selection procedure. |

Domain/protocol knowledge now lives beside the domain it describes, for example [Apps/info-service/AGENTS.md](../Apps/info-service/AGENTS.md) and [Tools/MixItUp/AGENTS.md](../Tools/MixItUp/AGENTS.md).

## Manifest

The target routing source is [manifest.json](manifest.json). Generated routing summaries should stay aligned with that manifest.
