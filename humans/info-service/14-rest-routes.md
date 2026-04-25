You are working in the `SharkStreamerBot` repo. Fresh session — no memory of prior conversations.

## Read first

1. `CLAUDE.md`
2. `.agents/ENTRY.md`
3. `.agents/roles/app-dev/role.md`
4. `humans/info-service/COORDINATION.md`
5. `Docs/INFO-SERVICE-PLAN.md` — §Schemas §1 (Envelope), §Folder Layout (`src/routes/read.ts`, `src/routes/write.ts`), §Chunk List row C5
6. `Apps/info-service/src/store/collection.ts` — `Collection<T>` API (`getAll`, `get`, `set`, `delete`, `name`)
7. `Apps/info-service/src/collections/user-intros.ts` — the registered collection instance
8. `Apps/info-service/src/server.ts` — current `buildServer(opts)` signature
9. `.agents/_shared/info-service-protocol.md` — planned route contracts

Role: `app-dev`.

## Prereqs

Chunks merged: C1, C2, C3, C4. Verify by checking that `Apps/info-service/src/collections/user-intros.ts` exists. Run `npm run typecheck` from `Apps/info-service/` before starting — must pass clean.

## WORKING.md

Add row at start. Domain: `Apps/info-service/`. Files: `Apps/info-service/src/routes/read.ts`, `Apps/info-service/src/routes/write.ts`, `Apps/info-service/src/server.ts`. Remove + log at finish.

## Task

Implement REST routes for `info-service`. Routes registered in `buildServer()`. Use a collection registry (map of name → `Collection` instance) passed via `opts` or built inside `buildServer`.

### Route contracts

| Method | Path | Success | Failure |
|---|---|---|---|
| `GET` | `/info/:collection` | `200` — `{ records: Record<string, T> }` | `404` unknown collection |
| `GET` | `/info/:collection/:key` | `200` — single record object | `404` unknown collection or key |
| `POST` | `/info/:collection/:key` | `201` — `{ ok: true }` | `400` schema fail, `404` unknown collection |
| `PUT` | `/info/:collection/:key` | `200` — `{ ok: true }` | `400` schema fail, `404` unknown collection |
| `DELETE` | `/info/:collection/:key` | `200` — `{ ok: true }` | `404` unknown collection or key |

### 1. `Apps/info-service/src/routes/read.ts` (new file)

Register read routes. Plugin receives the collection registry.

```typescript
import { FastifyInstance } from 'fastify';
import { Collection } from '../store/collection';
import { z } from 'zod';

type Registry = Map<string, Collection<z.ZodTypeAny>>;

export async function readRoutes(app: FastifyInstance, opts: { registry: Registry }) {
  const { registry } = opts;

  app.get('/info/:collection', async (req, reply) => {
    const { collection } = req.params as { collection: string };
    const col = registry.get(collection);
    if (!col) return reply.status(404).send({ error: 'unknown collection' });
    return { records: col.getAll() };
  });

  app.get('/info/:collection/:key', async (req, reply) => {
    const { collection, key } = req.params as { collection: string; key: string };
    const col = registry.get(collection);
    if (!col) return reply.status(404).send({ error: 'unknown collection' });
    const record = col.get(key);
    if (record === undefined) return reply.status(404).send({ error: 'not found' });
    return record;
  });
}
```

### 2. `Apps/info-service/src/routes/write.ts` (new file)

Register write routes. Validate body with the collection's record schema — `400` on zod failure.

```typescript
import { FastifyInstance } from 'fastify';
import { Collection } from '../store/collection';
import { z } from 'zod';

type Registry = Map<string, Collection<z.ZodTypeAny>>;

export async function writeRoutes(app: FastifyInstance, opts: { registry: Registry }) {
  const { registry } = opts;

  app.post('/info/:collection/:key', async (req, reply) => {
    const { collection, key } = req.params as { collection: string; key: string };
    const col = registry.get(collection);
    if (!col) return reply.status(404).send({ error: 'unknown collection' });
    try {
      await col.set(key, req.body);
    } catch (err: unknown) {
      return reply.status(400).send({ error: err instanceof Error ? err.message : String(err) });
    }
    return reply.status(201).send({ ok: true });
  });

  app.put('/info/:collection/:key', async (req, reply) => {
    const { collection, key } = req.params as { collection: string; key: string };
    const col = registry.get(collection);
    if (!col) return reply.status(404).send({ error: 'unknown collection' });
    try {
      await col.set(key, req.body);
    } catch (err: unknown) {
      return reply.status(400).send({ error: err instanceof Error ? err.message : String(err) });
    }
    return { ok: true };
  });

  app.delete('/info/:collection/:key', async (req, reply) => {
    const { collection, key } = req.params as { collection: string; key: string };
    const col = registry.get(collection);
    if (!col) return reply.status(404).send({ error: 'unknown collection' });
    const existing = col.get(key);
    if (existing === undefined) return reply.status(404).send({ error: 'not found' });
    await col.delete(key);
    return { ok: true };
  });
}
```

### 3. `Apps/info-service/src/server.ts` — update `buildServer`

Accept `collections` as `Collection<z.ZodTypeAny>[]` (instances, not names). Build registry map. Register route plugins.

```typescript
import Fastify from 'fastify';
import {
  serializerCompiler,
  validatorCompiler,
} from 'fastify-type-provider-zod';
import { z } from 'zod';
import { Collection } from './store/collection';
import { readRoutes } from './routes/read';
import { writeRoutes } from './routes/write';

export function buildServer(opts: { collections?: Collection<z.ZodTypeAny>[] } = {}) {
  const app = Fastify({ logger: true });

  app.setValidatorCompiler(validatorCompiler);
  app.setSerializerCompiler(serializerCompiler);

  const registry = new Map<string, Collection<z.ZodTypeAny>>(
    (opts.collections ?? []).map((c) => [c.name(), c])
  );

  app.get('/health', async (_req, _reply) => {
    return { ok: true, uptime: process.uptime(), collections: [...registry.keys()] };
  });

  app.register(readRoutes, { registry });
  app.register(writeRoutes, { registry });

  return app;
}
```

### 4. `Apps/info-service/src/index.ts` — update wiring

Pass collection instances (not names) to `buildServer`.

```typescript
import { buildServer } from './server';
import { userIntros } from './collections/user-intros';

const PORT = 8766;
const HOST = '127.0.0.1';

async function main() {
  await userIntros.load();

  const app = buildServer({ collections: [userIntros] });

  await app.listen({ port: PORT, host: HOST });
  console.log(`info-service listening on ${HOST}:${PORT}`);
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
```

## Deliverables

- Files changed:
  - `Apps/info-service/src/routes/read.ts` (new)
  - `Apps/info-service/src/routes/write.ts` (new)
  - `Apps/info-service/src/server.ts` (updated — registry + route registration)
  - `Apps/info-service/src/index.ts` (updated — pass instances not names)

## Forbidden in this chunk

- `pending-intros` collection — C10
- Any `production-manager` work — C6+
- Installing new npm packages
- Admin/reload endpoints — future chunk

## Finish

1. Run `npm run typecheck` from `Apps/info-service/`. Zero errors required. Fix any before reporting done.
2. Update `humans/info-service/COORDINATION.md` — chunk C5 status → `merged`, append Run Log row (Commit column: `uncommitted`).
3. `WORKING.md` — remove Active Work row, add Recently Completed row (trim to 10).
4. Load `ops-change-summary` skill from `.agents/roles/ops/skills/change-summary/_index.md`, show output.
5. **Draft the next chunk's prompt file** (self-propagation):
   - Check `COORDINATION.md` for chunks with status `not-started`, Prompt File `tbd`, and all prereqs `merged` (C5 now counts as merged).
   - C6 (`production-manager skeleton`) already has Prompt File `15-production-manager-skeleton.md` — no draft needed.
   - C7 (`production-manager: user-intros page`) has prereqs C4 + C6 — C4 merged, C6 prompt-ready (not yet merged). C7 cannot be drafted yet.
   - C8 (`SB first-chat script`) has prereq C5 — now satisfied. Draft `humans/info-service/16-sb-first-chat.md`.
   - C8 task: implement `Actions/Intros/first-chat-intro.cs` — Streamer.bot C# script. On "First Chat" trigger: GET `/info/user-intros/{userId}` from `INFO_SERVICE_BASE_URL`. If `200` and `enabled === true` and `soundFile` present: dispatch MixItUp Custom Intro command with `soundFile` as argument. If `404` or `enabled === false`: silent no-op. Log constructed asset path to SB action log for debugging. Read `Actions/SHARED-CONSTANTS.md` for constants (`INFO_SERVICE_BASE_URL`, `ASSETS_ROOT`, `COLLECTION_USER_INTROS`). Read `.agents/roles/streamerbot-dev/` for C# script conventions.
   - Update `COORDINATION.md`: C8 Prompt File → `16-sb-first-chat.md`, status → `prompt-ready`.

**Do NOT run `git commit` or `git add`. Operator commits manually.**

## Definition of done

- `Apps/info-service/src/routes/read.ts` exists — registers `GET /info/:collection` and `GET /info/:collection/:key`
- `Apps/info-service/src/routes/write.ts` exists — registers `POST`, `PUT`, `DELETE /info/:collection/:key`; validates body via `col.set()` zod path; returns `400` on failure
- `Apps/info-service/src/server.ts` builds collection registry from instances; registers both route plugins
- `Apps/info-service/src/index.ts` passes `[userIntros]` (instance) to `buildServer`
- `GET /health` still returns `{ ok, uptime, collections }` with correct collection names
- `npm run typecheck` passes with zero errors in `Apps/info-service/`
- `COORDINATION.md` C5 status = `merged`, run log row appended
- `humans/info-service/16-sb-first-chat.md` drafted OR blocker reported
- No git commit made by agent
