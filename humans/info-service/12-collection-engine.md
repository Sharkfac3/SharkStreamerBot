# Prompt C3 — Collection Engine

Paste everything below this line into a fresh coding-agent chat window.

---

You are working in the `SharkStreamerBot` repo. Fresh session — no memory of prior conversations.

## Read first

1. `CLAUDE.md`
2. `.agents/ENTRY.md`
3. `.agents/roles/app-dev/role.md`
4. `humans/info-service/COORDINATION.md`
5. `Docs/INFO-SERVICE-PLAN.md` — especially §Schemas (all 8 subsections), §Folder Layout
6. `Apps/info-service/src/server.ts` — understand current health route and `buildServer()` shape
7. `Apps/info-service/src/index.ts` — understand the entry point
8. `Apps/info-service/package.json` — confirm `zod` and `fastify` are already installed

Role: `app-dev`.

## Prereqs

Chunks merged: C1, C2. Verify by reading `Apps/info-service/src/server.ts` — `buildServer()` must exist and export a Fastify app. Run `npm run typecheck` from `Apps/info-service/` before starting — must pass clean.

## WORKING.md

Add row at start. Domain: `Apps/info-service/`. Files: `Apps/info-service/src/store/collection.ts`, `Apps/info-service/src/server.ts`. Remove + log at finish.

## Task

Implement the generic `Collection<T>` engine. This is the runtime engine all future collections (`user-intros`, `pending-intros`) will use. No collection instances are created in this chunk — that is C4.

### 1. `Apps/info-service/src/store/collection.ts` (new file)

Create this file. It must export three things: `EnvelopeSchema`, `Collection<T>`, and `CollectionRecord`.

#### `EnvelopeSchema` factory

```typescript
import { z } from 'zod';
import * as fs from 'node:fs/promises';
import * as path from 'node:path';

export const EnvelopeSchema = <T extends z.ZodTypeAny>(recordSchema: T) =>
  z.object({
    schemaVersion: z.number().int().positive(),
    collection:    z.string().min(1),
    updatedUtc:    z.number().int().positive(),
    records:       z.record(z.string(), recordSchema),
  });
```

#### `CollectionOptions` interface

```typescript
interface CollectionOptions<T extends z.ZodTypeAny> {
  filePath: string;        // absolute path to the .json file on disk
  collectionName: string;  // must match the `collection` field in the envelope
  schemaVersion: number;   // expected version — mismatch = hard stop (process.exit(1))
  recordSchema: T;         // zod schema for a single record
}
```

#### `Collection<T>` class

```typescript
export class Collection<T extends z.ZodTypeAny> {
  private records: Record<string, z.infer<T>> = {};
  // ...constructor and methods
}
```

**Methods:**

| Method | Signature | Behaviour |
|--------|-----------|-----------|
| `load()` | `async load(): Promise<void>` | Reads file if it exists; validates; checks version. If file absent, keeps `records` as `{}` (no file write). |
| `getAll()` | `getAll(): Record<string, z.infer<T>>` | Returns shallow copy of in-memory records map (`{ ...this.records }`). |
| `get(key)` | `get(key: string): z.infer<T> \| undefined` | Returns single record or `undefined`. |
| `set(key, record)` | `async set(key: string, record: z.infer<T>): Promise<void>` | Validates record schema. Mutates in-memory map. Writes atomically. |
| `delete(key)` | `async delete(key: string): Promise<void>` | Removes key from in-memory map. Writes atomically. No-op if key absent. |
| `name()` | `name(): string` | Returns `collectionName`. Used by the health route. |

**`load()` detail:**

1. Use `fs.access(filePath)` (or `fs.readFile` with try/catch on ENOENT) to detect whether the file exists.
2. If file does not exist → leave `this.records` as `{}`. Do NOT create the file. Return.
3. Read and `JSON.parse` the file contents.
4. Validate with `EnvelopeSchema(recordSchema).safeParse(raw)`.
5. If parse fails → `console.error` the zod issues and `process.exit(1)`.
6. If `parsed.data.schemaVersion !== schemaVersion` → log the mismatch (collection name, on-disk version, expected version) and `process.exit(1)`.
7. If `parsed.data.collection !== collectionName` → log the mismatch and `process.exit(1)`.
8. Store `parsed.data.records` in `this.records`.

**Atomic write (used by `set()` and `delete()`):**

1. Build the envelope: `{ schemaVersion, collection: collectionName, updatedUtc: Date.now(), records: this.records }`.
2. Ensure the parent directory of `filePath` exists: `await fs.mkdir(path.dirname(filePath), { recursive: true })`.
3. Write JSON to a temp file: `filePath + '.tmp'`. Use `JSON.stringify(envelope, null, 2)`.
4. Rename temp to final: `await fs.rename(tmpPath, filePath)`. This is atomic within the same directory/filesystem.

**`set()` validation:**

Before mutating `this.records`, call `recordSchema.safeParse(record)`. If it fails, throw `new Error(...)` with the zod error message. Do NOT mutate state or write to disk on failure.

#### `CollectionRecord` type helper

```typescript
export type CollectionRecord<T extends z.ZodTypeAny> = z.infer<T>;
```

### 2. `Apps/info-service/src/server.ts` — update health route

Update `buildServer()` to accept a `collections` option so C4 can pass loaded collection names. Keep backward compatibility: default to `[]`.

```typescript
export function buildServer(opts: { collections?: string[] } = {}) {
  // ...
  app.get('/health', async (_req, _reply) => {
    return { ok: true, uptime: process.uptime(), collections: opts.collections ?? [] };
  });
  return app;
}
```

`src/index.ts` calls `buildServer()` with no args — that still compiles. Do NOT wire up collection instances in `index.ts` in this chunk.

## Deliverables

- Files changed:
  - `Apps/info-service/src/store/collection.ts` (new)
  - `Apps/info-service/src/server.ts` (updated — `buildServer` accepts `opts`)
- Scaffolding updates: none
- Shared constants: none
- Tests: N/A

## Forbidden in this chunk

- Creating collection instances (`user-intros`, `pending-intros`) — that is C4
- Per-collection zod schema files under `src/store/schemas/` — C4
- Wiring collection instances into `src/index.ts` — C4
- HTTP routes beyond `/health` — C5
- Any `production-manager` work — C6+
- Installing new npm packages (`zod` already in `package.json`)

## Finish

1. Run `npm run typecheck` from `Apps/info-service/`. Zero errors required. Fix any before reporting done.
2. Update `humans/info-service/COORDINATION.md` — chunk C3 status → `merged`, append Run Log row (Commit column: `uncommitted`).
3. `WORKING.md` — remove Active Work row, add Recently Completed row (trim to 10).
4. Load `ops-change-summary` skill from `.agents/roles/ops/skills/change-summary/_index.md`, show output.
5. **Draft the next chunk's prompt file** (self-propagation):
   - Check `COORDINATION.md` for chunks with status `not-started`, Prompt File `tbd`, and all prereqs `merged` (C3 now counts as merged).
   - C4 (`user-intros collection`) has prereq C3 — now satisfied. It is the next chunk to draft.
   - Create `humans/info-service/13-user-intros-collection.md` using this same template (including this Finish step).
   - Pull C4 scope from `Docs/INFO-SERVICE-PLAN.md` §Chunk List row C4, §Schemas §5 (`UserIntroRecordSchema`), §Folder Layout (`src/store/schemas/user-intros.ts`, `src/collections/user-intros.ts`), and §Scaffolding Impact entry for C4.
   - C4 task: add `UserIntroRecordSchema` zod schema + `UserIntroRecord` type, create `src/store/schemas/user-intros.ts`, create `src/collections/user-intros.ts` (instantiates `Collection<typeof UserIntroRecordSchema>` with `schemaVersion: 1` and `filePath` resolved to `Apps/info-service/data/user-intros.json`), wire the collection into `src/index.ts` so `buildServer({ collections: ['user-intros'] })` is called, and update `_shared/info-service-protocol.md` with the collection name.
   - Update `COORDINATION.md`: C4 Prompt File → `13-user-intros-collection.md`, status → `prompt-ready`.

**Do NOT run `git commit` or `git add`. Operator commits manually.**

## Definition of done

- `Apps/info-service/src/store/collection.ts` exists and exports `EnvelopeSchema`, `Collection<T>`, `CollectionRecord`
- `Collection<T>` implements all six methods: `load()`, `getAll()`, `get()`, `set()`, `delete()`, `name()`
- `load()` calls `process.exit(1)` on zod parse failure, schema version mismatch, or collection name mismatch
- `set()` validates record with `recordSchema.safeParse()` before mutating; throws on failure
- Atomic write: temp file + `fs.rename` in same directory; parent dir auto-created
- `buildServer()` accepts `opts?: { collections?: string[] }` — health route returns the list
- `npm run typecheck` passes with zero errors in `Apps/info-service/`
- `COORDINATION.md` C3 status = `merged`, run log row appended
- `humans/info-service/13-user-intros-collection.md` drafted with full template OR blocker reported
- No git commit made by agent
