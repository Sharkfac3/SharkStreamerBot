# Prompt C2 — info-service Skeleton

Paste everything below this line into a fresh coding-agent chat window.

---

You are working in the `SharkStreamerBot` repo. Fresh session — no memory of prior conversations.

## Read first

1. `CLAUDE.md`
2. `.agents/ENTRY.md`
3. `.agents/roles/app-dev/role.md`
4. `humans/info-service/COORDINATION.md`
5. `Docs/INFO-SERVICE-PLAN.md` — especially §Tech Stack, §Schemas, §Folder Layout, §Constants to Register
6. `Actions/SHARED-CONSTANTS.md` — for port values added in C1 (`INFO_SERVICE_PORT` = 8766)
7. `Apps/stream-overlay/packages/overlay/package.json` — reference for existing monorepo TypeScript conventions

Role: `app-dev`.

## Prereqs

Chunks merged: C1. Verify with `git log` before starting — `Assets/` folder and the `## Info Service / Assets (shared)` section in `Actions/SHARED-CONSTANTS.md` must exist.

## WORKING.md

Add row at start. Domain: `Apps/`, `.agents/`. Files: `Apps/info-service/package.json`, `Apps/info-service/tsconfig.json`, `Apps/info-service/src/index.ts`, `Apps/info-service/src/server.ts`, `Apps/info-service/README.md`, `.gitignore`, `.agents/roles/app-dev/role.md`, `.agents/roles/app-dev/context/info-service.md`, `.agents/_shared/info-service-protocol.md`. Remove + log at finish.

## Task

Scaffold `Apps/info-service/` — a standalone Fastify + TypeScript REST service. This chunk creates the skeleton only: HTTP server, one health route, correct loopback binding. No collection engine, no data persistence, no non-health routes.

### 1. `Apps/info-service/package.json`

- `name`: `@sharkstreamerbot/info-service`
- `version`: `0.1.0`
- `private`: `true`
- `main`: `dist/index.js`
- Scripts:
  - `"dev"`: `tsx src/index.ts` (match `tsx` if used in sibling packages; otherwise `ts-node src/index.ts`)
  - `"build"`: `tsc`
  - `"typecheck"`: `tsc --noEmit`
- Dependencies: `fastify`, `@fastify/type-provider-zod`, `zod`
- Dev dependencies: `typescript`, `@types/node`, `tsx` (or `ts-node` — match sibling packages)

Check `Apps/stream-overlay/packages/overlay/package.json` for the TypeScript version in use; match it.

### 2. `Apps/info-service/tsconfig.json`

- `target`: `ES2020`
- `module`: `commonjs`
- `strict`: `true`
- `outDir`: `dist`
- `rootDir`: `src`
- `esModuleInterop`: `true`
- `skipLibCheck`: `true`
- Do not extend from stream-overlay's tsconfig — this package is standalone.

### 3. `Apps/info-service/src/index.ts`

Entry point. Imports the Fastify app from `server.ts`, starts listening.

- Bind address: `127.0.0.1`
- Port: `8766` (matching `INFO_SERVICE_PORT` from `Actions/SHARED-CONSTANTS.md`)
- On successful listen: log `info-service listening on http://127.0.0.1:8766`
- On error: log the error and call `process.exit(1)`

```typescript
// rough shape — implement cleanly
import { buildServer } from './server';

const PORT = 8766;
const HOST = '127.0.0.1';

const app = buildServer();

app.listen({ port: PORT, host: HOST }, (err, address) => {
  if (err) {
    app.log.error(err);
    process.exit(1);
  }
  console.log(`info-service listening on ${address}`);
});
```

### 4. `Apps/info-service/src/server.ts`

Creates and exports a factory function `buildServer()` that returns a configured Fastify instance.

- Enable Fastify's built-in logger (`logger: true`).
- Register `@fastify/type-provider-zod` as the type provider.
- Register the health route:

```
GET /health
Response 200: { ok: true, uptime: <process.uptime() as number>, collections: [] }
```

  - `collections: []` is a typed placeholder (`string[]`). C3/C4 will populate it from loaded collection instances.
  - Return HTTP 200 always in this chunk.

- No other routes in this chunk.

Example shape:
```typescript
import Fastify from 'fastify';
import {
  serializerCompiler,
  validatorCompiler,
} from 'fastify-type-provider-zod';

export function buildServer() {
  const app = Fastify({ logger: true });

  app.setValidatorCompiler(validatorCompiler);
  app.setSerializerCompiler(serializerCompiler);

  app.get('/health', async (_req, _reply) => {
    return { ok: true, uptime: process.uptime(), collections: [] as string[] };
  });

  return app;
}
```

Adjust imports to match the installed package names. Verify types compile before reporting done.

### 5. `Apps/info-service/README.md`

Brief operator README covering:
- What info-service is (file-backed JSON REST service for per-viewer stream data)
- How to run: `npm install` then `npm run dev` from `Apps/info-service/`
- How to build: `npm run build`
- Port: 8766 (127.0.0.1 only — no LAN exposure)
- Health check: `GET http://127.0.0.1:8766/health`
- Data folder: `Apps/info-service/data/` (created at runtime, gitignored)
- What's coming: collection engine (C3), user-intros collection (C4), REST routes (C5)

### 6. `.gitignore` additions

Append to the root `.gitignore`:

```
# Apps/info-service
Apps/info-service/dist/
Apps/info-service/node_modules/
Apps/info-service/data/
```

`data/` is the runtime JSON store (gitignored per Decision 22 in COORDINATION.md).

### 7. Scaffolding updates

**`.agents/roles/app-dev/role.md`**

In the "Activate When" section (or add an explicit "Active Apps" note), record that `Apps/info-service/` is an active app under this role's scope. Keep the edit minimal — one bullet or note is sufficient.

**`.agents/roles/app-dev/context/info-service.md`** (new file)

One-page orientation for future agents working on info-service:

- What it is: file-backed JSON REST service replacing ad-hoc Streamer.bot user-variable storage for per-viewer data
- Port: 8766, binds 127.0.0.1 only — no LAN exposure, no auth needed day 1
- How to run locally: `npm run dev` from `Apps/info-service/`
- Where data lives: `Apps/info-service/data/` — runtime state, gitignored; operator is responsible for backup
- Single-writer policy: `production-manager` is the only write client by policy; Streamer.bot and overlay are read-only HTTP clients
- Extension points: collection engine (C3), user-intros + pending-intros schemas (C4), REST routes (C5), admin app (C6+)
- Key architecture decisions: see `Docs/INFO-SERVICE-PLAN.md` §Tech Stack and Decisions 1–28 in `humans/info-service/COORDINATION.md`
- Schema mismatch policy: hard stop on boot if `schemaVersion` in file does not match code constant (Decision 23)

**`.agents/_shared/info-service-protocol.md`** (new file)

Placeholder REST contract. Future agents expand this in C5 and C11.

```markdown
# Info Service — Protocol Reference

> Stub. Expands in C5 (routes) and C11 (final polish).
> Full schema definitions live in `Docs/INFO-SERVICE-PLAN.md §Schemas`.

## Health

`GET /health`

Response `200`:
\`\`\`json
{ "ok": true, "uptime": 12.345, "collections": [] }
\`\`\`

`collections` will list loaded collection names once C3/C4 are merged.

## Planned routes (C5 — not yet implemented)

- `GET /info/:collection` — return all records in collection
- `GET /info/:collection/:key` — return single record by key (Twitch userId for user-intros)
- `POST /info/:collection/:key` — create record (production-manager only)
- `PUT /info/:collection/:key` — update record (production-manager only)
- `DELETE /info/:collection/:key` — delete record (production-manager only)

## Envelope schema

All collection JSON files on disk follow the envelope defined in `Docs/INFO-SERVICE-PLAN.md §Schemas §1`.
Do not duplicate the schema here — reference the plan doc.

## Error conventions (C5 — not yet specified)

Placeholder. C5 prompt will define error shapes.
```

## Deliverables

- Files changed:
  - `Apps/info-service/package.json` (new)
  - `Apps/info-service/tsconfig.json` (new)
  - `Apps/info-service/src/index.ts` (new)
  - `Apps/info-service/src/server.ts` (new)
  - `Apps/info-service/README.md` (new)
  - `.gitignore` (new block appended)
  - `.agents/roles/app-dev/role.md` (updated — note new app in scope)
  - `.agents/roles/app-dev/context/info-service.md` (new)
  - `.agents/_shared/info-service-protocol.md` (new)
- Scaffolding updates: `.agents/roles/app-dev/role.md`, `.agents/roles/app-dev/context/info-service.md` (new), `.agents/_shared/info-service-protocol.md` (new)
- Shared constants: none — port values already added in C1
- Tests: N/A (no test infra yet; C11 may add)

## Forbidden in this chunk

- Collection engine (`Collection<T>` class, atomic write, schema loading) — that is C3
- Collection data files or per-collection zod schemas — C4
- Non-health HTTP routes (`/info/:collection`, write routes, etc.) — C5
- `production-manager` app or any React/Vite work — C6+
- Installing test frameworks or CI config
- Changing anything under `Apps/stream-overlay/`

## Finish

1. Run `npm install` then `npm run typecheck` from `Apps/info-service/`. Fix any TypeScript errors before reporting done.
2. Optionally start `npm run dev` and run `curl http://127.0.0.1:8766/health` — verify response is `{"ok":true,"uptime":<number>,"collections":[]}`.
3. Update `humans/info-service/COORDINATION.md` — chunk C2 status → `merged`, append Run Log row (Commit column: `uncommitted`).
4. `WORKING.md` — remove Active Work row, add Recently Completed row (trim to 10).
5. Load `ops-change-summary` skill, show output.

**Do NOT run `git commit` or `git add`. Operator commits manually.**

## Definition of done

- `Apps/info-service/` exists with `package.json`, `tsconfig.json`, `src/index.ts`, `src/server.ts`, `README.md`
- `npm run typecheck` passes with zero errors
- `GET /health` returns `{ ok: true, uptime: <number>, collections: [] }` when server is running
- Server binds `127.0.0.1:8766` — no other interface
- `fastify`, `zod`, `@fastify/type-provider-zod` are in `package.json` dependencies
- `.gitignore` excludes `dist/`, `node_modules/`, `data/` under `Apps/info-service/`
- `.agents/roles/app-dev/role.md` notes `Apps/info-service/` in active scope
- `.agents/roles/app-dev/context/info-service.md` exists with orientation content per Task §7
- `.agents/_shared/info-service-protocol.md` exists as placeholder stub with health route documented
- `COORDINATION.md` C2 row: status = `merged`, Prompt File = `11-info-service-skeleton.md`
- No git commit made by agent.
