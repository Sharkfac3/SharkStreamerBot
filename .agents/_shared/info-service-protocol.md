# Info Service — Protocol Reference

> Full schema definitions live in `Docs/INFO-SERVICE-PLAN.md §Schemas`.

## Health

`GET /health`

Response `200`:
```json
{ "ok": true, "uptime": 12.345, "collections": ["user-intros", "pending-intros"] }
```

`collections` lists loaded collection names.

## Collections

### user-intros

| Property | Value |
|---|---|
| Collection name | `user-intros` |
| Record key | Twitch numeric `userId` as string (e.g., `"12345"`) |
| Schema version | `1` |
| Data file | `Apps/info-service/data/user-intros.json` (gitignored, created on first write) |
| Schema source | `Apps/info-service/src/store/schemas/user-intros.ts` |
| Collection instance | `Apps/info-service/src/collections/user-intros.ts` |

Record shape (zod source of truth: `UserIntroRecordSchema`):

| Field | Type | Required | Notes |
|---|---|---|---|
| `userId` | `string` | required | Twitch userId. Also the record key. |
| `userLogin` | `string` | required | Lowercase Twitch login at time of capture. Not used as key. |
| `soundFile` | `string \| undefined` | optional | Filename only. Resolves under `Assets/user-intros/sound/`. |
| `gifFile` | `string \| undefined` | optional | Filename only. Resolves under `Assets/user-intros/gif/`. |
| `enabled` | `boolean` | required | Soft-disable flag. `false` = skip intro without deleting. |
| `notes` | `string \| undefined` | optional | Operator-facing notes only. Never shown on stream. |
| `updatedUtc` | `number` | required | Unix milliseconds of last write to this record. |

### pending-intros

| Property | Value |
|---|---|
| Collection name | `pending-intros` |
| Record key | Twitch `redeemId` as string (stable per-redemption ID from SB event) |
| Schema version | `1` |
| Data file | `Apps/info-service/data/pending-intros.json` (gitignored, created on first write) |
| Schema source | `Apps/info-service/src/store/schemas/pending-intros.ts` |
| Collection instance | `Apps/info-service/src/collections/pending-intros.ts` |

Record shape (zod source of truth: `PendingIntroRecordSchema`):

| Field | Type | Required | Notes |
|---|---|---|---|
| `userId` | `string` | required | Twitch userId. |
| `userLogin` | `string` | required | Lowercase Twitch login at time of redeem. Not used as key. |
| `redeemId` | `string` | required | Channel-point redemption ID from SB event. Also the record key. |
| `redeemUtc` | `number` | required | Unix milliseconds when the redeem fired. |
| `rewardTitle` | `string` | required | Channel-point reward title as received from SB. |
| `userInput` | `string \| undefined` | optional | Raw user-supplied message from the redeem. Absent if reward has no user-input field. |
| `status` | `'pending' \| 'fulfilled' \| 'rejected'` | required | Lifecycle state. Initial value: `'pending'`. |
| `resolvedUtc` | `number \| undefined` | optional | Unix ms when operator marked the record fulfilled or rejected. |

Status lifecycle:
- `pending` → `fulfilled`: operator assigns assets in production-manager and promotes record to `user-intros`. PM sets `resolvedUtc`.
- `pending` → `rejected`: operator rejects the redeem. PM sets `resolvedUtc`.
- No reverse transition. If a promoted intro needs reworking, edit the `user-intros` record directly.

> Note: production-manager PM UI for pending-intros fulfillment (C10.5) is pending operator decision on Open Question Q14 in `humans/info-service/COORDINATION.md`.

## Routes

- `GET /info/:collection` — return all records in collection
- `GET /info/:collection/:key` — return single record by key
- `POST /info/:collection/:key` — create/replace record (production-manager only)
- `PUT /info/:collection/:key` — update record (same as POST in v1)
- `DELETE /info/:collection/:key` — delete record (production-manager only)

## Envelope schema

All collection JSON files on disk follow the envelope defined in `Docs/INFO-SERVICE-PLAN.md §Schemas §1`.

## Error conventions

### `404 Not Found`

Returned when the collection name is unknown, or when a known collection does not contain the requested key.

Unknown collection example:
```json
{ "error": "unknown collection" }
```

Missing record example:
```json
{ "error": "not found" }
```

### `400 Bad Request`

Returned by write routes when record validation fails inside `Collection<T>.set()`. In the current implementation, the response body is a plain object with a single `error` string containing zod's formatted validation message.

Example:
```json
{
  "error": "[\n  {\n    \"expected\": \"string\",\n    \"code\": \"invalid_type\",\n    \"path\": [\n      \"userId\"\n    ],\n    \"message\": \"Invalid input: expected string, received undefined\"\n  }\n]"
}
```

No `409` conflict response is currently implemented.

## Service constants

| Constant | Value |
|---|---|
| `INFO_SERVICE_BASE_URL` | `http://127.0.0.1:8766` |
| `INFO_SERVICE_PORT` | `8766` |
| Binding | `127.0.0.1` only |
