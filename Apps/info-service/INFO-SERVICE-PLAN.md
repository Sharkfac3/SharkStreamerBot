# Info Service — Architecture, Protocol, and Operations

This is the app-local home for the former repository-level info-service plan and protocol notes. It covers the `Apps/info-service/` REST API, its file-backed collection model, and the integration contract used by Streamer.bot and `Apps/production-manager/`.

## Goal

`info-service` is a lightweight local REST service for per-viewer stream data. It replaces ad-hoc Streamer.bot user-variable storage with structured JSON collections that can be queried by Streamer.bot scripts and edited through `production-manager`.

Primary v1 use case: custom per-user stream intros.

## Non-Goals

- No SQLite or database migration tooling in v1.
- No authentication or authorization in v1.
- No LAN or external exposure; bind to `127.0.0.1` only.
- No overlay or broker behavior changes.
- No public chat copy ownership; chain to `brand-steward` for viewer-facing wording.

## Runtime Model

```text
Streamer.bot actions
  ├─ GET /info/user-intros/:userId for First Chat playback
  └─ POST /info/pending-intros/:redeemId for Custom Intro redemptions

production-manager
  └─ operator-only admin UI; sole planned write client for editing records

info-service
  └─ Fastify + TypeScript API backed by JSON files under data/
```

The service is intentionally local-only:

| Constant | Value |
|---|---|
| Base URL | `http://127.0.0.1:8766` |
| Port | `8766` |
| Binding | `127.0.0.1` only |
| Auth | none in v1 |

## Folder Layout

| Path | Purpose |
|---|---|
| [package.json](package.json) | npm scripts and dependencies. |
| [src/index.ts](src/index.ts) | Process entry point. |
| [src/server.ts](src/server.ts) | Fastify server construction and route registration. |
| [src/store/collection.ts](src/store/collection.ts) | Generic `Collection<T>` engine with validation and atomic writes. |
| [src/store/schemas/user-intros.ts](src/store/schemas/user-intros.ts) | `user-intros` zod schema. |
| [src/store/schemas/pending-intros.ts](src/store/schemas/pending-intros.ts) | `pending-intros` zod schema. |
| [src/collections/user-intros.ts](src/collections/user-intros.ts) | `user-intros` collection instance. |
| [src/collections/pending-intros.ts](src/collections/pending-intros.ts) | `pending-intros` collection instance. |
| [src/routes/read.ts](src/routes/read.ts) | `GET /info/:collection` and `GET /info/:collection/:key`. |
| [src/routes/write.ts](src/routes/write.ts) | `POST`, `PUT`, and `DELETE` collection write routes. |
| `data/` | Runtime JSON store, gitignored, created at runtime. |

## Related Local Systems

| System | Guide | Relationship |
|---|---|---|
| production-manager | [Apps/production-manager/PRODUCTION-MANAGER-GUIDE.md](../production-manager/PRODUCTION-MANAGER-GUIDE.md) | Operator UI and preferred write client. |
| Intros actions | [Actions/Intros/AGENTS.md](../../Actions/Intros/AGENTS.md) | Streamer.bot consumers/producers for intro data. |
| Shared constants | [Actions/SHARED-CONSTANTS.md](../../Actions/SHARED-CONSTANTS.md) | Streamer.bot base URL, collection names, and assets root constants. |

## Setup and Commands

```bash
cd Apps/info-service
npm install
npm run dev
npm run typecheck
npm run build
```

Health check after startup:

```text
GET http://127.0.0.1:8766/health
```

Expected shape:

```json
{ "ok": true, "uptime": 12.345, "collections": ["user-intros", "pending-intros"] }
```

## Data Ownership

`Apps/info-service/data/` contains runtime state and is gitignored. The operator is responsible for backups. Do not commit collection JSON files.

Binary intro assets live outside the service under repo-root `Assets/` and are also gitignored. Records store filenames only; consumers join those filenames with the configured assets root.

## Single-Writer Policy

`production-manager` is the only intended write client for operator edits. Streamer.bot has two exceptions:

1. `Actions/Intros/redeem-capture.cs` creates `pending-intros` records from Custom Intro redemptions.
2. Future Streamer.bot actions may write only when an app-local guide explicitly documents the exception.

Overlay clients and normal Streamer.bot playback scripts are read-only. Keep this policy unless a coordinated app-dev design adds locking or a database.

## Collection Envelope

Each collection file uses the same envelope shape:

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

Rules:

- `schemaVersion` starts at `1` and must match the collection code constant.
- `collection` must match the collection/file name.
- `updatedUtc` values are Unix milliseconds as numbers.
- `records` is keyed by the collection primary key.
- Optional fields are absent/undefined, never `null`.

## Schema Mismatch Policy

Hard stop on boot if an on-disk collection `schemaVersion` does not match the code's expected version. Do not add silent fallbacks. The operator must run a future migration command or manually repair data before serving records again.

## Schemas

### `user-intros`

| Property | Value |
|---|---|
| Collection name | `user-intros` |
| Record key | Twitch numeric `userId` as string |
| Schema version | `1` |
| Data file | `Apps/info-service/data/user-intros.json` |
| Source | [src/store/schemas/user-intros.ts](src/store/schemas/user-intros.ts) |
| Instance | [src/collections/user-intros.ts](src/collections/user-intros.ts) |

| Field | Type | Required | Notes |
|---|---|---|---|
| `userId` | `string` | yes | Twitch userId; also the record key. |
| `userLogin` | `string` | yes | Lowercase Twitch login at capture/edit time; not used as key. |
| `soundFile` | `string \| undefined` | no | Filename only; resolves under `Assets/user-intros/sound/`. |
| `gifFile` | `string \| undefined` | no | Filename only; resolves under `Assets/user-intros/gif/`. |
| `enabled` | `boolean` | yes | `false` skips intro without deleting the record. |
| `notes` | `string \| undefined` | no | Operator-facing notes only. Never show on stream. |
| `updatedUtc` | `number` | yes | Unix milliseconds of last write to this record. |

A first-chat playback consumer should require `enabled === true` and at least one usable asset field before dispatching an intro.

### `pending-intros`

| Property | Value |
|---|---|
| Collection name | `pending-intros` |
| Record key | Twitch redemption `redeemId` as string |
| Schema version | `1` |
| Data file | `Apps/info-service/data/pending-intros.json` |
| Source | [src/store/schemas/pending-intros.ts](src/store/schemas/pending-intros.ts) |
| Instance | [src/collections/pending-intros.ts](src/collections/pending-intros.ts) |

| Field | Type | Required | Notes |
|---|---|---|---|
| `userId` | `string` | yes | Twitch userId. |
| `userLogin` | `string` | yes | Lowercase Twitch login at redemption time. |
| `redeemId` | `string` | yes | Channel-point redemption ID; also the record key. |
| `redeemUtc` | `number` | yes | Unix milliseconds when the redeem fired. |
| `rewardTitle` | `string` | yes | Channel-point reward title as received by Streamer.bot. |
| `userInput` | `string \| undefined` | no | Raw user-supplied message, if present. |
| `status` | `'pending' \| 'fulfilled' \| 'rejected'` | yes | Lifecycle state. Initial value is `pending`. |
| `resolvedUtc` | `number \| undefined` | no | Unix milliseconds when fulfilled or rejected. |

Lifecycle:

- `pending` to `fulfilled`: operator assigns assets/promotes to `user-intros`; `resolvedUtc` is set.
- `pending` to `rejected`: operator rejects the redeem; `resolvedUtc` is set.
- No reverse transition. Edit the `user-intros` record directly if a fulfilled intro needs rework.

## REST Protocol

### Health

`GET /health`

Response `200`:

```json
{ "ok": true, "uptime": 12.345, "collections": ["user-intros", "pending-intros"] }
```

### Collection Routes

| Method | Path | Purpose |
|---|---|---|
| `GET` | `/info/:collection` | Return all records in a collection. |
| `GET` | `/info/:collection/:key` | Return one record by key. |
| `POST` | `/info/:collection/:key` | Create or replace a record. |
| `PUT` | `/info/:collection/:key` | Update a record; same replacement semantics as `POST` in v1. |
| `DELETE` | `/info/:collection/:key` | Delete a record. |

### Error Conventions

`404 Not Found` means either the collection is unknown or a known collection does not contain the requested key.

```json
{ "error": "unknown collection" }
```

```json
{ "error": "not found" }
```

`400 Bad Request` is returned by write routes when zod record validation fails. The current response body contains one `error` string with the validation detail.

No `409 Conflict` response is implemented in the current service.

## Validation Placement

| Checkpoint | When | What validates | Failure behavior |
|---|---|---|---|
| File load | Service boot and explicit reload paths | Full envelope and all records | Log and exit/refuse service. |
| Write request | POST/PUT before mutation | Per-record schema | Return `400`; do not write. |
| Manual data edits | Next boot/reload | Full envelope and all records | Refuse service until repaired. |

## Risks and Mitigations

| Risk | Mitigation |
|---|---|
| JSON write amplification as data grows | Accept for v1; consider SQLite if collections grow or queries become complex. |
| Schema drift | Hard-stop version checks and explicit future migrations. |
| Wrong machine-specific asset root | Keep `ASSETS_ROOT` in [Actions/SHARED-CONSTANTS.md](../../Actions/SHARED-CONSTANTS.md) and log constructed paths in Streamer.bot. |
| Multiple writers | Keep single-writer policy until locking/database work is explicitly designed. |

## Migration Notes

This file replaces the detailed plan formerly kept at the top-level Docs folder and the old agent-tree shared protocol pointer. New protocol details belong here.
