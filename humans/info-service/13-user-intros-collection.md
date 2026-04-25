You are working in the `SharkStreamerBot` repo. Fresh session — no memory of prior conversations.

## Read first

1. `CLAUDE.md`
2. `.agents/ENTRY.md`
3. `.agents/roles/app-dev/role.md`
4. `humans/info-service/COORDINATION.md`
5. `Docs/INFO-SERVICE-PLAN.md` — especially §Schemas §5 (`UserIntroRecordSchema`), §Folder Layout, §Chunk List row C4, §Scaffolding Impact
6. `Apps/info-service/src/store/collection.ts` — understand `Collection<T>` and `EnvelopeSchema`
7. `Apps/info-service/src/index.ts` — current entry point
8. `Apps/info-service/src/server.ts` — `buildServer(opts)` signature
9. `.agents/_shared/info-service-protocol.md` — current protocol doc

Role: `app-dev`.

## Prereqs

Chunks merged: C1, C2, C3. Verify by checking that `Apps/info-service/src/store/collection.ts` exists and exports `Collection`, `EnvelopeSchema`, `CollectionRecord`. Run `npm run typecheck` from `Apps/info-service/` before starting — must pass clean.

## WORKING.md

Add row at start. Domain: `Apps/info-service/`. Files: `Apps/info-service/src/store/schemas/user-intros.ts`, `Apps/info-service/src/collections/user-intros.ts`, `Apps/info-service/src/index.ts`, `.agents/_shared/info-service-protocol.md`. Remove + log at finish.

## Task

Wire the `user-intros` collection into `info-service`. This includes the zod schema file, the collection instance, and entry-point wiring. No HTTP routes yet — that is C5.

### 1. `Apps/info-service/src/store/schemas/user-intros.ts` (new file)

```typescript
import { z } from 'zod';

export const SCHEMA_VERSION = 1;

export const UserIntroRecordSchema = z.object({
  userId:     z.string().min(1),
  userLogin:  z.string().min(1),
  soundFile:  z.string().min(1).optional(),
  gifFile:    z.string().min(1).optional(),
  enabled:    z.boolean(),
  notes:      z.string().optional(),
  updatedUtc: z.number().int().positive(),
});

export type UserIntroRecord = z.infer<typeof UserIntroRecordSchema>;
```

### 2. `Apps/info-service/src/collections/user-intros.ts` (new file)

Instantiates `Collection<typeof UserIntroRecordSchema>`. The `filePath` must be resolved relative to this file's location so it always points to `Apps/info-service/data/user-intros.json` regardless of working directory.

```typescript
import * as path from 'node:path';
import { Collection } from '../store/collection';
import { UserIntroRecordSchema, SCHEMA_VERSION } from '../store/schemas/user-intros';

export const userIntros = new Collection({
  filePath: path.resolve(__dirname, '../../data/user-intros.json'),
  collectionName: 'user-intros',
  schemaVersion: SCHEMA_VERSION,
  recordSchema: UserIntroRecordSchema,
});
```

> Note: `__dirname` works with `tsx` (dev) and compiled output. Confirm `tsconfig.json` has `"module": "commonjs"` or equivalent so `__dirname` is available. If the project uses ESM, use `import.meta.url` + `fileURLToPath` instead — check `Apps/info-service/tsconfig.json` and adjust if needed.

### 3. `Apps/info-service/src/index.ts` — update entry point

Load the collection before starting the server. Pass the collection name to `buildServer`.

```typescript
import { buildServer } from './server';
import { userIntros } from './collections/user-intros';

const PORT = 8766;
const HOST = '127.0.0.1';

async function main() {
  await userIntros.load();

  const app = buildServer({ collections: [userIntros.name()] });

  await app.listen({ port: PORT, host: HOST });
  console.log(`info-service listening on ${HOST}:${PORT}`);
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
```

### 4. `.agents/_shared/info-service-protocol.md` — add collection entry

Read the existing file. Append or update to include:

- Collection name: `user-intros`
- Record key: Twitch numeric `userId` as string
- Schema version: `1`
- Data file: `Apps/info-service/data/user-intros.json` (gitignored, created on first write)
- Schema source: `Apps/info-service/src/store/schemas/user-intros.ts`
- Collection instance: `Apps/info-service/src/collections/user-intros.ts`

## Deliverables

- Files changed:
  - `Apps/info-service/src/store/schemas/user-intros.ts` (new)
  - `Apps/info-service/src/collections/user-intros.ts` (new)
  - `Apps/info-service/src/index.ts` (updated — async main, load + wire)
  - `.agents/_shared/info-service-protocol.md` (updated — collection entry)

## Forbidden in this chunk

- HTTP routes beyond `/health` — C5
- `pending-intros` collection — C10
- Any `production-manager` work — C6+
- Installing new npm packages

## Finish

1. Run `npm run typecheck` from `Apps/info-service/`. Zero errors required. Fix any before reporting done.
2. Update `humans/info-service/COORDINATION.md` — chunk C4 status → `merged`, append Run Log row (Commit column: `uncommitted`).
3. `WORKING.md` — remove Active Work row, add Recently Completed row (trim to 10).
4. Load `ops-change-summary` skill from `.agents/roles/ops/skills/change-summary/_index.md`, show output.
5. **Draft the next chunk's prompt file** (self-propagation):
   - Check `COORDINATION.md` for chunks with status `not-started`, Prompt File `tbd`, and all prereqs `merged` (C4 now counts as merged).
   - C5 (`REST routes`) has prereqs C4 — now satisfied. It is the next chunk to draft.
   - Create `humans/info-service/14-rest-routes.md` using this same template (including this Finish step).
   - Pull C5 scope from `Docs/INFO-SERVICE-PLAN.md` §Chunk List row C5, §Folder Layout (`src/routes/read.ts`, `src/routes/write.ts`).
   - C5 task: implement `GET /info/:collection` (returns all records), `GET /info/:collection/:key` (returns single record or 404), `POST /info/:collection/:key` (create/replace), `PUT /info/:collection/:key` (update — same as POST for now), `DELETE /info/:collection/:key` (delete). Routes registered in `buildServer()`. Use a collection registry (map of name → Collection instance) passed via `opts` or a dedicated registry module. 404 on unknown collection. Validate write body with the collection's record schema (zod) — 400 on failure.
   - Update `COORDINATION.md`: C5 Prompt File → `14-rest-routes.md`, status → `prompt-ready`.

**Do NOT run `git commit` or `git add`. Operator commits manually.**

## Definition of done

- `Apps/info-service/src/store/schemas/user-intros.ts` exists with `UserIntroRecordSchema`, `UserIntroRecord`, and `SCHEMA_VERSION = 1`
- `Apps/info-service/src/collections/user-intros.ts` exists — instantiates `Collection` with correct `filePath`, `collectionName: 'user-intros'`, `schemaVersion: 1`
- `Apps/info-service/src/index.ts` calls `userIntros.load()` before `app.listen()`; passes `collections: [userIntros.name()]` to `buildServer()`
- `.agents/_shared/info-service-protocol.md` updated with collection entry
- `npm run typecheck` passes with zero errors in `Apps/info-service/`
- `COORDINATION.md` C4 status = `merged`, run log row appended
- `humans/info-service/14-rest-routes.md` drafted with full template OR blocker reported
- No git commit made by agent
