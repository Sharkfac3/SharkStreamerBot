# Info Service — Architecture Plan

> **Read-only reference.** Process state lives in `humans/info-service/COORDINATION.md`. Schemas (detailed) land in a P2 update to this doc. Execution prompts are drafted lazily in P3.

---

## Goal

This body of work adds a lightweight, file-backed REST data service (`info-service`) and a React admin web app (`production-manager`) to the SharkStreamerBot ecosystem. Together they replace ad-hoc Streamer.bot user-variable storage for per-viewer data with a structured, queryable, and operator-editable store — starting with custom per-user stream intros. The architecture is designed to scale to additional data collections without requiring a database migration: each collection is a single JSON file governed by a shared envelope schema. Streamer.bot triggers look up viewer data at runtime via HTTP GET; MixItUp plays the resulting intro audio; the admin app lets the operator manage records without writing code.

---

## Non-Goals

- No SQLite or database migration tooling on day 1 (upgrade path exists if a collection crosses ~10k records or complex queries emerge)
- No authentication or authorization layer on day 1 (services bind `127.0.0.1` only)
- No LAN or external network exposure
- No new named cast members or characters
- No changes to overlay or broker behavior — both remain untouched
- No execution prompts in this document (drafted lazily in P3)
- No detailed zod schemas in this document (P2 adds a Schemas section here)

---

## Architecture Overview

```
Twitch Chat
    │
    │  First Chat event
    ▼
┌─────────────────────────────────┐
│         Streamer.bot            │
│  - "First Chat" trigger         │
│  - HTTP GET → info-service      │
│  - MixItUp command dispatch     │
└──────────┬──────────────────────┘
           │  GET /info/user-intros/:userId   │  POST/PUT/DELETE write routes
           ▼                                  ▼
┌──────────────────────┐        ┌──────────────────────────┐
│     info-service     │◄──────►│    production-manager    │
│  Node.js REST API    │        │  React + Vite admin app  │
│  file-backed JSON    │        │  write routes only       │
│  127.0.0.1 only      │        │  127.0.0.1 only          │
│  port TBD            │        │  dev port TBD            │
└──────────────────────┘        └──────────────────────────┘
           │
           │  reads/writes JSON files
           ▼
┌──────────────────────┐
│   Data/              │
│   (inside info-      │
│    service package)  │
│   user-intros.json   │
│   ...future colls... │
└──────────────────────┘

           │  filename only (no path)
           ▼
┌──────────────────────┐
│   Assets/            │  ← repo-root, gitignored
│   user-intros/       │     binary media (mp3, gif, etc.)
│     alice.mp3        │
│     bob.mp3          │
└──────────────────────┘
           ▲
           │  reads file by path = ASSETS_ROOT + filename
           │
┌──────────────────────┐
│       MixItUp        │
│  Custom Intro cmd    │
│  plays audio file    │
└──────────────────────┘

stream-overlay/broker  ──  unchanged, dumb WebSocket router
stream-overlay/overlay ──  unchanged, Phaser renderer
```

**Data flow for a First Chat event:**

1. Streamer.bot "First Chat" trigger fires.
2. SB script GETs `/info/user-intros/{userId}` from `info-service`.
3. If record found: SB dispatches MixItUp Custom Intro command with `filename` as a special identifier.
4. MixItUp constructs full path `ASSETS_ROOT/user-intros/{filename}` and plays the audio.
5. If no record: SB falls back to a default greeting (no-op for audio).

**Write flow (operator):**

1. Operator opens `production-manager` in browser.
2. Selects or creates a user-intros record, uploads/picks a sound file (stored to `Assets/user-intros/`).
3. `production-manager` POSTs/PUTs record to `info-service` write routes.
4. `info-service` atomically writes the updated collection JSON (temp-file rename).

---

## Folder Layout

```
SharkStreamerBot/
├── Apps/
│   ├── stream-overlay/          ← existing, untouched
│   ├── info-service/            ← NEW (C2+)
│   │   ├── package.json
│   │   ├── src/
│   │   │   ├── index.ts         ← server entry, binds 127.0.0.1
│   │   │   ├── routes/
│   │   │   │   ├── read.ts      ← GET /info/:collection, /info/:collection/:key
│   │   │   │   └── write.ts     ← POST/PUT/DELETE /info/:collection/:key
│   │   │   ├── store/
│   │   │   │   ├── collection.ts  ← generic Collection<T> + atomic write
│   │   │   │   └── schemas/
│   │   │   │       └── user-intros.ts  ← zod schema (C4)
│   │   │   └── collections/
│   │   │       └── user-intros.ts      ← collection instance (C4)
│   │   └── data/                ← runtime JSON store (gitignored binary aside)
│   │       └── user-intros.json ← created on first write
│   └── production-manager/      ← NEW (C6+)
│       ├── package.json
│       ├── vite.config.ts
│       └── src/
│           ├── main.tsx
│           ├── App.tsx
│           └── pages/
│               └── UserIntros.tsx  ← table + form + file picker (C7)
├── Assets/                      ← NEW (C1) — repo-root, gitignored
│   └── user-intros/             ← mp3/gif files, named by filename field
├── Actions/
│   └── Intros/                  ← NEW (C8)
│       └── first-chat-intro.cs
├── Docs/
│   └── INFO-SERVICE-PLAN.md     ← this file
└── humans/
    └── info-service/            ← prompt series + coordination
```

---

## Data Model (high level)

All collection files use the same envelope shape:

```json
{
  "schemaVersion": 1,
  "collection": "user-intros",
  "updatedUtc": "2026-04-24T00:00:00Z",
  "records": { ... }
}
```

| Field | Type | Purpose |
|---|---|---|
| `schemaVersion` | integer | Migration hook — bump when record shape changes |
| `collection` | string | Self-describing collection name |
| `updatedUtc` | ISO 8601 string | Last write timestamp; useful for cache/debug |
| `records` | object (keyed map) | Collection-specific records, keyed by primary key |

**`user-intros` record shape (high level — detailed zod schema in P2):**

| Field | Type | Notes |
|---|---|---|
| `userId` | string | Twitch userId — primary key |
| `userName` | string | Display name at time of last edit |
| `filename` | string | Filename only — consumer joins with `ASSETS_ROOT/user-intros/` |
| `enabled` | boolean | Soft-disable without deleting the record |
| `updatedUtc` | ISO 8601 string | Per-record last-write timestamp |

Records stored as a keyed object: `{ "12345": { "userId": "12345", ... } }`. Lookup by key is O(1) for the read path SB uses.

---

## Schemas

All schemas use **zod** for runtime validation. The `Collection<T>` generic in `info-service` accepts a zod schema for `T` and validates at load and write time.

---

### 1. Envelope schema

Every `<collection>.json` file on disk uses this outer envelope. `T` is the per-collection record type.

**Zod pseudocode:**

```typescript
const EnvelopeSchema = <T extends z.ZodTypeAny>(recordSchema: T) =>
  z.object({
    schemaVersion: z.number().int().positive(),
    collection:    z.string().min(1),
    updatedUtc:    z.number().int().positive(),
    records:       z.record(z.string(), recordSchema),
  });
```

**Canonical JSON example (`user-intros.json` with one placeholder record):**

```json
{
  "schemaVersion": 1,
  "collection": "user-intros",
  "updatedUtc": 1745500000000,
  "records": {
    "12345": {
      "userId": "12345",
      "userLogin": "alice",
      "soundFile": "alice.mp3",
      "gifFile": "alice.gif",
      "enabled": true,
      "updatedUtc": 1745500000000
    }
  }
}
```

Field rules:

| Field | Type | Notes |
|---|---|---|
| `schemaVersion` | `number` (integer) | Starts at `1`. Bump on breaking record shape changes. |
| `collection` | `string` | Must match filename base (e.g., `user-intros` for `user-intros.json`). Validated on load. |
| `updatedUtc` | `number` | Unix milliseconds of last write to this file. |
| `records` | `Record<string, T>` | Keyed map of record objects. Key rules are per-collection (see §2). |

---

### 2. Record key policy

| Collection | Key | Why |
|---|---|---|
| `user-intros` | Twitch numeric `userId` as string (e.g., `"12345"`) | Stable — does not change when a user renames their account |
| `pending-intros` | Twitch `redeemId` as string | Stable per-redemption identifier from the SB event payload |

**Rule:** Keys must be stable identifiers. Never use Twitch login names or display names as record keys — both can change.

---

### 3. Timestamp policy

- All timestamps are **Unix milliseconds** stored as `number`.
- UTC always. No timezone offsets.
- No ISO 8601 strings in any schema field. The high-level Data Model section above used ISO strings as a placeholder; this section supersedes that.
- Both envelope `updatedUtc` and every per-record `updatedUtc` follow this policy.
- Example: `1745500000000` — not `"2025-04-24T00:00:00Z"`.

---

### 4. Optional field policy

- Optional fields are typed `T | undefined` in TypeScript and `z.T().optional()` in zod.
- **Never use `null`** for absent optional fields. If a field is absent it is `undefined` in memory and omitted from the serialized JSON (zod `.optional()` omits on round-trip).
- Callers must guard `field !== undefined` before use. No silent coalescing to empty string unless explicitly documented per field.

---

### 5. `user-intros` record schema

**Zod schema:**

```typescript
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

**Field reference:**

| Field | Type | Required | Constraints |
|---|---|---|---|
| `userId` | `string` | required | Numeric Twitch userId. Also the record key in `records`. Never empty. |
| `userLogin` | `string` | required | Lowercase Twitch login at time of capture. Stored for human readability; not used as key. May become stale if user renames. |
| `soundFile` | `string \| undefined` | optional | Filename only — no path, no leading slash. Resolves under `Assets/user-intros/sound/`. |
| `gifFile` | `string \| undefined` | optional | Filename only — no path. Resolves under `Assets/user-intros/gif/`. |
| `enabled` | `boolean` | required | Soft-disable flag. `false` = skip intro without deleting the record. |
| `notes` | `string \| undefined` | optional | Operator-facing notes. Never shown on stream. |
| `updatedUtc` | `number` | required | Unix ms of last write to this record. |

Notes:
- Both `soundFile` and `gifFile` may be absent if the operator has not yet assigned assets. The SB first-chat script must check `enabled === true` and that at least one asset field is present before dispatching.
- Asset paths are resolved by consumers as `ASSETS_ROOT + "/user-intros/sound/" + soundFile` and `ASSETS_ROOT + "/user-intros/gif/" + gifFile`.

---

### 6. `pending-intros` record schema

Confirmed in scope (Q10 = A — full automation target). Populated by the SB channel-point redeem handler; fulfilled or rejected via production-manager.

**Zod schema:**

```typescript
export const PendingIntroRecordSchema = z.object({
  userId:       z.string().min(1),
  userLogin:    z.string().min(1),
  redeemId:     z.string().min(1),
  redeemUtc:    z.number().int().positive(),
  rewardTitle:  z.string().min(1),
  userInput:    z.string().optional(),
  status:       z.enum(['pending', 'fulfilled', 'rejected']),
  resolvedUtc:  z.number().int().positive().optional(),
});

export type PendingIntroRecord = z.infer<typeof PendingIntroRecordSchema>;
```

**Field reference:**

| Field | Type | Required | Constraints |
|---|---|---|---|
| `userId` | `string` | required | Twitch userId. |
| `userLogin` | `string` | required | Lowercase Twitch login at time of redeem. |
| `redeemId` | `string` | required | Channel-point redemption ID from SB event. Also used as the record key. |
| `redeemUtc` | `number` | required | Unix ms when the redeem fired. |
| `rewardTitle` | `string` | required | Channel-point reward title as received from SB. |
| `userInput` | `string \| undefined` | optional | Raw user-supplied message from the redeem. Absent if the reward has no user-input field. |
| `status` | `'pending' \| 'fulfilled' \| 'rejected'` | required | Lifecycle state. Initial value: `'pending'`. |
| `resolvedUtc` | `number \| undefined` | optional | Unix ms when operator marked the record fulfilled or rejected. |

Status lifecycle:
- `pending` → `fulfilled`: operator assigns `soundFile`/`gifFile` in production-manager and promotes the record to `user-intros`. production-manager sets `resolvedUtc`.
- `pending` → `rejected`: operator rejects the redeem (duplicate or ineligible). production-manager sets `resolvedUtc`.
- No `fulfilled → pending` transition. If a promoted intro needs reworking, edit the `user-intros` record directly.

---

### 7. Migration policy

**Rule (Q8 = A — error out on mismatch):** On boot, `info-service` loads each collection file and checks `file.schemaVersion` against the code's expected version constant. If they do not match, the service throws and refuses to start. No data is served.

Implementation contract:
- Each collection module exports a `SCHEMA_VERSION` constant (e.g., `export const SCHEMA_VERSION = 1`).
- `Collection<T>.load()` reads the file, parses the envelope schema, then asserts `file.schemaVersion === SCHEMA_VERSION`.
- On mismatch: log the collection name, on-disk version, and expected version, then `process.exit(1)`.
- Resolution: operator runs a CLI migration command (to be added in a future chunk) or manually edits the file and bumps the version.

Rationale: with a single collection and a single operator, a hard stop is the safest signal. Prevents silent schema drift that could cause missed intros or corrupted records.

---

### 8. Validation placement

Three mandatory checkpoints:

| Checkpoint | When | What validates | On failure |
|---|---|---|---|
| **On file load** | Service boot; any `Collection.reload()` call | Full envelope schema + record schema for every record in the file | Log error + `process.exit(1)` |
| **On write** | Every POST/PUT request, before in-memory record is updated | Per-record schema (Fastify route handler via `@fastify/type-provider-zod`) | Return `400 Bad Request` with zod error details |
| **On disk read after hand-edit** | `GET` request if file mtime changed (hot-reload path) or explicit reload endpoint | Full envelope + record schemas | Log warning + return `503` until operator resolves |

Design notes:
- Validate at the boundary, not mid-flow. Write validation fires before any in-memory mutation — if zod rejects, the file is never touched.
- On-load checkpoint ensures the service never starts in a broken state even after manual JSON edits.
- A future chunk may add a `POST /admin/reload` endpoint to re-run the on-load checkpoint without restarting the process.

---

## Tech Stack

| Component | Choice | Status |
|---|---|---|
| `info-service` runtime | Node.js | RESOLVED |
| `info-service` language | TypeScript | RESOLVED |
| `info-service` HTTP framework | Fastify | RESOLVED — Q1 |
| `info-service` schema validator | zod | RESOLVED — Q2 |
| `info-service` port | 8766 | RESOLVED — Q5 |
| `info-service` file write strategy | atomic temp-file rename | RESOLVED |
| `info-service` data folder | `Apps/info-service/data/`, gitignored | RESOLVED — Q7 |
| `info-service` schema mismatch policy | error out / refuse to start | RESOLVED — Q8 |
| `production-manager` framework | React + Vite | RESOLVED |
| `production-manager` component library | shadcn/ui (Radix UI + Tailwind) | RESOLVED — Q4 |
| `production-manager` data-fetching library | plain fetch | RESOLVED — Q3 |
| `production-manager` dev server port | 5174 | RESOLVED — Q5 |
| `production-manager` prod-preview port | 4174 | RESOLVED — Q5 |
| `production-manager` auth | None in v1 | RESOLVED — Q11 |
| `Assets/` location | repo-root, gitignored | RESOLVED |
| `ASSETS_ROOT` constant | operator-set absolute path in `SHARED-CONSTANTS.md` | RESOLVED — Q6 |
| Network binding | `127.0.0.1` only | RESOLVED |
| Auth | None day 1 | RESOLVED |
| Scaffolding role | extend `app-dev` | RESOLVED — Q12 |
| Commit discipline | direct to `main`, operator commits manually | RESOLVED — Q13 |

---

## Chunk List

| # | Chunk | Prereqs | Summary | Scaffolding Updates |
|---|-------|---------|---------|---------------------|
| P0 | Seed plan doc | — | Create this file; update COORDINATION + WORKING | None |
| P1 | Harden questions | P0 | Agent reads plan, surfaces architecture gaps as open questions for operator | `COORDINATION.md` Open Questions |
| P2 | Propose schemas | P1 + operator answers | Add detailed envelope + user-intros zod schemas to this doc | `INFO-SERVICE-PLAN.md` §Schemas |
| P3 | Draft first exec prompts | P2 | Write `humans/info-service/10-*.md` and `11-*.md` | `humans/info-service/` |
| C1 | Assets folder + SHARED-CONSTANTS | P3 | Create `Assets/` with `.gitignore`, add constants block to `Actions/SHARED-CONSTANTS.md`. No code. | `.agents/roles/app-dev/skills/core.md` (note new domain) |
| C2 | info-service skeleton | C1 | `Apps/info-service/` with package.json, TS config, health route, server binding 127.0.0.1 | `app-dev` role.md, `.agents/roles/app-dev/skills/stream-interactions/_index.md` |
| C3 | Collection engine | C2 | Generic `Collection<T>` class + atomic write + zod validation hookpoint | None expected |
| C4 | user-intros collection | C3 | Zod schema instance + collection wiring for user-intros | `.agents/_shared/info-service-protocol.md` (new) |
| C5 | REST routes | C4 | GET `/info/:collection`, `/info/:collection/:key`; POST/PUT/DELETE write routes | None expected |
| C6 | production-manager skeleton | C2 | `Apps/production-manager/` React + Vite scaffold, dev server, health check page | `app-dev` role.md |
| C7 | production-manager: user-intros page | C4 + C6 | Table listing records, form for create/edit, file picker for `Assets/user-intros/` | None expected |
| C8 | SB first-chat script | C5 | `Actions/Intros/first-chat-intro.cs` — GET lookup, MixItUp dispatch, fallback | `Actions/SHARED-CONSTANTS.md` (verify constants present), `Actions/Intros/README.md` |
| C9 | MixItUp Custom Intro command spec | C8 | Document the MixItUp command structure; update `.agents/_shared/mixitup-api.md` or add an Intros section | `.agents/_shared/mixitup-api.md` |
| C10 | Redeem capture | C5 | Pending-intros collection: channel-point redeem creates a pending record for operator to fulfill | `COORDINATION.md` (design decisions), `Actions/SHARED-CONSTANTS.md` |
| C11 | Docs + scaffolding sweep | all above | Polish `_shared/info-service-protocol.md`, update `app-dev` role skills index, final README pass | `.agents/roles/app-dev/`, `.agents/_shared/info-service-protocol.md` |

---

## Scaffolding Impact

All `.agents/` files expected to be touched across the full project:

- `.agents/roles/app-dev/role.md` — add `info-service` and `production-manager` to active-app list (C2, C6)
- `.agents/roles/app-dev/skills/core.md` — note `Assets/` domain, `info-service` patterns (C1, C2)
- `.agents/roles/app-dev/skills/stream-interactions/_index.md` — add info-service interaction patterns (C2)
- `.agents/_shared/info-service-protocol.md` — new file: REST route contracts, envelope schema, topic/event names if any, SB usage patterns (C4, C11)
- `.agents/_shared/mixitup-api.md` — add Intros section documenting Custom Intro command shape (C9)
- `.agents/roles/streamerbot-dev/` — may need note about `first-chat-intro.cs` pattern (C8, at agent's discretion)

---

## Constants to Register

These entries will be added to `Actions/SHARED-CONSTANTS.md` under a new **Info Service (shared)** section:

| Constant | Value | Status |
|---|---|---|
| `ASSETS_ROOT` | Absolute path to repo-root `Assets/` folder on operator machine | **TBD — operator must set in `SHARED-CONSTANTS.md`** |
| `INFO_SERVICE_BASE_URL` | `http://127.0.0.1:8766` | RESOLVED — Q5 |
| `INFO_SERVICE_PORT` | `8766` | RESOLVED — Q5 |
| `PRODUCTION_MANAGER_PORT` | `5174` | RESOLVED — Q5 |
| `COLLECTION_USER_INTROS` | `"user-intros"` | RESOLVED |
| `COLLECTION_PENDING_INTROS` | `"pending-intros"` | RESOLVED — Q10 |

No broker topics or overlay events are introduced by this feature (audio plays via MixItUp, not overlay).

---

## Risks & Mitigations

| Risk | Likelihood | Mitigation |
|---|---|---|
| **Write amplification on large JSON** — rewriting the full collection file on every record update is cheap at hundreds of records but degrades at scale | Low day 1 | Envelope includes `schemaVersion`; upgrade path to SQLite documented. Monitor file size. Alert operator if collection grows past ~1k records. |
| **Schema drift without migrations** — `user-intros` record shape may evolve; old files will have missing fields | Medium | `schemaVersion` field in envelope. Collection engine checks version at load and can apply migrations at read time. Detailed migration strategy deferred to P2. |
| **Prompt rot** — execution prompts drafted too far ahead will diverge from actual implemented state | Medium | Decision 15: prompts drafted lazily, one or two chunks ahead. P3 only drafts C1+C2 prompts. Each chunk's prompt is written after its predecessor merges. |
| **Operator coordination: channel-point redeems vs. manual data entry** — a viewer redeems a custom-intro channel point while the operator hasn't fulfilled past pending records yet, leading to confusing state | Medium | C10 introduces a `pending-intros` collection as a queue. Operator fulfills from `production-manager`. Redeem script checks `pending-intros` to avoid duplicate pending entries. Design details in C10 prompt. |
| **`Assets/` path is machine-specific** — `ASSETS_ROOT` must be set per operator machine; if wrong, MixItUp plays nothing | Low | `ASSETS_ROOT` registered in `SHARED-CONSTANTS.md` as a TBD operator-set value. SB script should log the constructed path to SB action log for debugging. |
| **Single-writer assumption breaks** — if a future agent or script calls write routes concurrently, the atomic rename may still race | Low | Document single-writer policy in `info-service-protocol.md`. Write routes return 409 if a write lock file exists (to be specified in P2). |
