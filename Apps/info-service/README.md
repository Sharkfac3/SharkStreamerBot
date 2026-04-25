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
{ "ok": true, "uptime": 12.345, "collections": [] }
```

## Data folder

`Apps/info-service/data/` — created at runtime, gitignored. Operator is responsible for backup.

## What's coming

- **C3** — Collection engine (`Collection<T>`, atomic write, schema version guard)
- **C4** — `user-intros` collection + `pending-intros` collection
- **C5** — REST routes (`GET /info/:collection`, `GET /info/:collection/:key`, write routes)
