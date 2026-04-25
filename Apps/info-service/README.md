# info-service

File-backed JSON REST service for per-viewer stream data. Replaces ad-hoc Streamer.bot user-variable storage with a queryable HTTP API.

## Run (dev)

```bash
cd Apps/info-service
npm install
npm run dev
```

## Build

```bash
npm run build
```

## Port

`8766` — binds `127.0.0.1` only. No LAN exposure. No auth required.

## Health check

```
GET http://127.0.0.1:8766/health
```

Response:

```json
{ "ok": true, "uptime": 12.345, "collections": ["user-intros", "pending-intros"] }
```

## Data folder

`Apps/info-service/data/` — created at runtime, gitignored. Operator is responsible for backup.

## Collections

| Collection | Key | Data file |
|------------|-----|-----------|
| `user-intros` | Twitch `userId` | `data/user-intros.json` |
| `pending-intros` | Twitch `redeemId` | `data/pending-intros.json` |

See `.agents/_shared/info-service-protocol.md` for field reference and schema details.

## Routes

| Method | Path | Purpose |
|--------|------|---------|
| `GET` | `/health` | Service health + loaded collections |
| `GET` | `/info/:collection` | All records in collection |
| `GET` | `/info/:collection/:key` | Single record by key |
| `POST` | `/info/:collection/:key` | Create/replace record |
| `PUT` | `/info/:collection/:key` | Update record |
| `DELETE` | `/info/:collection/:key` | Delete record |
