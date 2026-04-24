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

## Tech Stack

| Component | Choice | Status |
|---|---|---|
| `info-service` runtime | Node.js | RESOLVED |
| `info-service` language | TypeScript | RESOLVED |
| `info-service` HTTP framework | Fastify vs Express vs plain `http` | **OPEN** |
| `info-service` schema validator | zod (tentative) | OPEN — confirm in P1 |
| `info-service` port | TBD | **OPEN** |
| `info-service` file write strategy | atomic temp-file rename | RESOLVED |
| `production-manager` framework | React + Vite | RESOLVED |
| `production-manager` component library | TBD | **OPEN** |
| `production-manager` data-fetching library | TBD (fetch / react-query / swr) | **OPEN** |
| `production-manager` dev server port | TBD | **OPEN** |
| `Assets/` location | repo-root, gitignored | RESOLVED |
| Network binding | `127.0.0.1` only | RESOLVED |
| Auth | None day 1 | RESOLVED |

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
| `ASSETS_ROOT` | Absolute path to repo-root `Assets/` folder on operator machine | **TBD — operator must set** |
| `INFO_SERVICE_BASE_URL` | `http://127.0.0.1:{INFO_SERVICE_PORT}` | TBD (port OPEN) |
| `INFO_SERVICE_PORT` | TBD | **OPEN** |
| `PRODUCTION_MANAGER_PORT` | TBD | **OPEN** |
| `COLLECTION_USER_INTROS` | `"user-intros"` | RESOLVED |

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
