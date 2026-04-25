# info-service — Agent Orientation

## What it is

File-backed JSON REST service replacing ad-hoc Streamer.bot user-variable storage for per-viewer data. Lives at `Apps/info-service/` as a standalone Node/TypeScript app.

## Port + binding

`8766`, binds `127.0.0.1` only. No LAN exposure. No auth needed in v1 (Decision 10, 26).

## How to run locally

```bash
cd Apps/info-service
npm install
npm run dev
```

## Where data lives

`Apps/info-service/data/` — runtime state, gitignored (Decision 22). Operator is responsible for backup. Do not commit data files.

## Single-writer policy

`production-manager` is the only write client by policy. Streamer.bot and overlay are read-only HTTP clients. This keeps file I/O lock-free (Decision 9).

## Extension points

| Chunk | What it adds |
|-------|-------------|
| C3 | `Collection<T>` class, atomic write, schema version guard |
| C4 | `user-intros` + `pending-intros` collections with zod schemas |
| C5 | REST routes: `GET /info/:collection`, `GET /info/:collection/:key`, write routes |
| C6+ | `production-manager` React admin app |

## Schema mismatch policy

Hard stop on boot if `schemaVersion` in data file does not match code constant (Decision 23). Do not add silent fallbacks.

## Key references

- Architecture: `Docs/INFO-SERVICE-PLAN.md` §Tech Stack, §Schemas
- Decisions 1–28: `humans/info-service/COORDINATION.md`
- REST contract (stub): `.agents/_shared/info-service-protocol.md`
- Port + URL constants: `Actions/SHARED-CONSTANTS.md` §Info Service / Assets
