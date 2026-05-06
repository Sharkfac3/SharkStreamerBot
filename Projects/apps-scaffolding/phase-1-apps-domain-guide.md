# Phase 1: Create Apps/AGENTS.md Domain Guide

## Context

You are working in the SharkStreamerBot repository. This repo uses structured agent scaffolding where every major domain folder has an `AGENTS.md` file that acts as a routing entrypoint for coding agents (Claude and Pi).

`Apps/` has 3 standalone TypeScript apps with their own `AGENTS.md` guides, but no domain-level entrypoint. An agent landing at `Apps/` has no routing surface. `Actions/AGENTS.md` is the gold-standard example of a domain-level guide — use it as your structural template.

## Required Reading

Read these files **before writing anything**:

1. `Actions/AGENTS.md` — **the template**. Match its structure: frontmatter, purpose, start-here steps, domain rules, folder routing, runtime integration map, shared references, validation, boundaries.
2. `Apps/info-service/AGENTS.md` — backend app guide.
3. `Apps/stream-overlay/AGENTS.md` — overlay app guide.
4. `Apps/production-manager/AGENTS.md` — admin UI guide.
5. `.agents/roles/app-dev/role.md` — role that owns Apps/.
6. `.agents/_shared/conventions.md` — repo-wide conventions.
7. `.agents/_shared/project.md` — repo-wide context.

## Pre-Digested App Inventory

Use this information directly — do not re-discover it:

| App | Path | Framework | Pkg Manager | Ports | Purpose |
|---|---|---|---|---|---|
| info-service | `Apps/info-service/` | Fastify + TypeScript | npm | 8766 | File-backed JSON REST API for per-viewer stream data |
| production-manager | `Apps/production-manager/` | React 18 + Vite 5 + Tailwind | npm | 5174 dev / 4174 preview | Local admin UI for managing info-service collections |
| stream-overlay | `Apps/stream-overlay/` | Phaser + pnpm monorepo (shared/broker/overlay) | pnpm | 5173 overlay / 8765 broker | WebSocket broker + Phaser browser overlay for OBS |

### Dependency Graph

```
stream-overlay (broker + overlay) — independent, connects to Streamer.bot via WebSocket
info-service — independent backend
production-manager — depends on info-service running at :8766
```

### Cross-App Integration

- Streamer.bot Actions publish to stream-overlay broker (WebSocket) and info-service (HTTP)
- production-manager is the preferred write client for info-service
- stream-overlay and info-service/production-manager are independent systems — no dependency between them

### Shared Conventions

- All localhost-only, no auth in v1
- All TypeScript
- Package manager differs: npm for info-service and production-manager, pnpm for stream-overlay
- Schema authority stays in info-service (zod schemas), not duplicated in UI
- Store asset filenames only, not absolute paths
- Use Twitch `userId` as stable key, not login/display name

## Expected Output Format

Your output must match the structural pattern of `Actions/AGENTS.md`. Here is the exact frontmatter format to use:

```yaml
---
id: apps-agent-guide
type: domain-guide
description: Standalone TypeScript apps for stream overlays, data services, and admin tooling.
status: active
owner: app-dev
---
```

**Important frontmatter rules:**
- `id` must be `apps-agent-guide` (the validator checks this naming pattern)
- `type` must be `domain-guide`
- `id` must be kebab-case
- All of `id`, `type`, `description`, `status` are required

## Required Sections (in this order)

1. **Purpose** — 2-3 sentences. What Apps/ contains and why.
2. **Start Here** — Numbered steps for an agent arriving at Apps/. Read this file → pick an app → read that app's AGENTS.md → follow workflows.
3. **App Inventory** — Table with: app name, path (as markdown link to local AGENTS.md), framework, package manager, ports, backend dependencies.
4. **Dependency and Startup Order** — Which apps depend on which. Clear startup order.
5. **Shared Conventions** — Bullet list of conventions from the "Shared Conventions" section above.
6. **Folder Routing** — Table mapping each app subdirectory to its local AGENTS.md (same pattern as `Actions/AGENTS.md` folder routing table).
7. **Runtime Integration Map** — How apps connect to each other and to Streamer.bot Actions. Use the "Cross-App Integration" info above.
8. **Shared References** — Table linking key cross-cutting docs (info-service plan, protocol docs, brand identity, shared constants).
9. **Validation** — Commands to validate across the domain. Include both app-specific commands and the agent-tree validator.
10. **Boundaries** — What does NOT belong in Apps/ (C# scripts → Actions/, brand content → Creative/, tooling → Tools/, etc.).

**Do NOT add sections beyond this list.**

## Length Target

Aim for roughly 80-120 lines of markdown (excluding frontmatter). `Actions/AGENTS.md` is ~188 lines but covers 14+ subfolders and action contracts. Apps/ has 3 apps and simpler rules — it should be shorter. Concise routing surface, not encyclopedia.

## Constraints

- **Only create `Apps/AGENTS.md`.** Do not modify any existing file.
- **Ignore `node_modules/` entirely.** Each app has a `node_modules/` directory with thousands of dependency files. Do not read, explore, reference, or link to anything inside `node_modules/`. They are not project source files.
- Use relative markdown links from `Apps/` (e.g., `[info-service/AGENTS.md](info-service/AGENTS.md)`).
- Do not invent information beyond what is in the required reading files and this prompt.
- Do not add commentary, explanations, or meta-notes inside the file — only the guide content.

## Validation

After creating the file, run:

```bash
python Tools/AgentTree/validate.py
```

**What the validator checks** (so you know what to fix if it fails):
- **frontmatter**: required fields `id`, `type`, `description`, `status` present
- **folder-coverage**: every first-level subdirectory under Apps/ has AGENTS.md coverage
- **link-integrity**: all markdown links and backtick paths resolve to real files
- **naming**: `id` is kebab-case and matches the domain pattern (`apps-*` for files under `Apps/`)
- **id-uniqueness**: no duplicate frontmatter IDs across all AGENTS.md files

Report the full validator output. If there are failures related to your new file, fix them before finishing.
