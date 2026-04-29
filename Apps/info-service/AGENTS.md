---
id: apps-info-service
type: domain-route
description: Local Fastify/TypeScript REST API for per-viewer stream data and custom-intro collections.
owner: app-dev
secondaryOwners:
  - streamerbot-dev
  - ops
workflows:
  - change-summary
  - validation
status: active
---

# Info Service — Agent Guide

## Purpose

This folder owns `info-service`, the local file-backed JSON REST API for per-viewer stream data. It currently serves the `user-intros` and `pending-intros` collections used by custom-intro Streamer.bot actions and edited by `production-manager`.

## When to Activate

Use this guide for work under [Apps/info-service/](./), including:

- Fastify server startup, binding, ports, and health checks.
- Collection engine behavior, atomic JSON writes, and schema validation.
- `user-intros` and `pending-intros` schemas or route behavior.
- REST contract changes that affect [Apps/production-manager/](../production-manager/) or [Actions/Intros/AGENTS.md](../../Actions/Intros/AGENTS.md).
- App-local documentation replacing old shared/repo-level info-service plan docs.

## Primary Owner

`app-dev` owns the Node/TypeScript app, REST API contract, schema placement, local runtime model, and app documentation.

## Secondary Owners / Chain To

- `streamerbot-dev` — chain when route/schema changes require C# updates in [Actions/Intros/](../../Actions/Intros/) or constants in [Actions/SHARED-CONSTANTS.md](../../Actions/SHARED-CONSTANTS.md).
- `ops` — chain for validation runs, local backup/operator checklists, and handoff formatting.
- `brand-steward` — chain only if public-facing intro wording, reward text, or moderation policy is changed elsewhere.

## Required Reading

Read these before changing service behavior:

- [README.md](README.md) for quick startup and current route summary.
- [INFO-SERVICE-PLAN.md](INFO-SERVICE-PLAN.md) for architecture, protocol, collection schemas, and migration policy.
- [package.json](package.json) for available npm scripts.
- [src/store/collection.ts](src/store/collection.ts) before changing persistence behavior.
- [src/store/schemas/user-intros.ts](src/store/schemas/user-intros.ts) and [src/store/schemas/pending-intros.ts](src/store/schemas/pending-intros.ts) before changing records.
- [Apps/production-manager/PRODUCTION-MANAGER-GUIDE.md](../production-manager/PRODUCTION-MANAGER-GUIDE.md) when UI behavior or write flows are affected.
- [Actions/Intros/AGENTS.md](../../Actions/Intros/AGENTS.md) when Streamer.bot intro scripts are affected.

## Local Workflow

1. Identify whether the change is server/runtime, persistence, schema, REST protocol, or documentation-only.
2. Preserve `127.0.0.1` binding and port `8766` unless the operator explicitly approves a coordinated port migration.
3. Keep zod schemas as the runtime validation source of truth.
4. Preserve hard-stop behavior for schema-version mismatch; do not add silent fallback/migration behavior without an explicit migration design.
5. Preserve atomic write semantics in the collection engine.
6. Keep runtime data under the local data folder and binary assets under repo-root [Assets/](../../Assets/); neither should be committed.
7. If schemas or routes change, update [INFO-SERVICE-PLAN.md](INFO-SERVICE-PLAN.md), [README.md](README.md), and affected clients in the same coordinated change when in scope.
8. Run validation and record output in the handoff.

## Validation

Service commands:

```bash
cd Apps/info-service
npm install
npm run typecheck
npm run build
npm run dev
```

Smoke check while the dev server is running:

```text
GET http://127.0.0.1:8766/health
```

Agent-tree validation for doc/routing changes:

```bash
python3 Tools/AgentTree/validate.py
```

## Boundaries / Out of Scope

- Do not expose the service outside localhost in v1.
- Do not add auth unless the local-only deployment model changes.
- Do not replace the JSON store with SQLite/database work without a dedicated design prompt.
- Do not edit Streamer.bot scripts here; link to [Actions/Intros/AGENTS.md](../../Actions/Intros/AGENTS.md) and chain to `streamerbot-dev`.
- Do not change production-manager UI copy or public reward text without the appropriate owner.

## Handoff Notes

For code or protocol changes, include:

- Changed routes and collection schemas.
- Whether existing data files require manual repair or migration.
- Commands run from this folder and their outputs.
- Client impacts for [Apps/production-manager/](../production-manager/) and [Actions/Intros/](../../Actions/Intros/).
- Any operator backup/setup steps for the local data folder or [Assets/](../../Assets/).

## Runtime Notes

Important constants and policies:

| Item | Current value / policy |
|---|---|
| Base URL | `http://127.0.0.1:8766` |
| Data folder | Apps/info-service/data/ (runtime-created, gitignored) |
| Write model | Single-writer by policy; production-manager is the preferred write client, with documented Streamer.bot capture exceptions. |
| Collections | `user-intros`, `pending-intros` |
| Schema source | zod files under [src/store/schemas/](src/store/schemas/) |
| Protocol source | [INFO-SERVICE-PLAN.md](INFO-SERVICE-PLAN.md) |
