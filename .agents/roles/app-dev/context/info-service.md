# info-service — Agent Orientation

## Current state

All C1–C11 chunks complete as of 2026-04-25.

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
| C3 | Collection engine |
| C4 | `user-intros` collection + zod schema |
| C5 | REST routes |
| C6 | `production-manager` React admin app (skeleton) |
| C7 | `production-manager` user-intros page (table, form, CRUD) |
| C8 | `Actions/Intros/first-chat-intro.cs` — SB first-chat intro handler |
| C9 | MixItUp Custom Intro command spec (`.agents/_shared/mixitup-api.md`) |
| C10 | `pending-intros` collection + `Actions/Intros/redeem-capture.cs` |

## Key files

- `Apps/info-service/src/index.ts` — server entry, collections wired
- `Apps/info-service/src/store/collection.ts` — generic `Collection<T>` engine
- `Apps/info-service/src/store/schemas/` — zod schemas per collection
- `Apps/info-service/src/collections/` — collection instances
- `Apps/info-service/src/routes/read.ts` + `write.ts` — REST route plugins
- `Apps/production-manager/src/pages/UserIntrosPage.tsx` — user-intros CRUD UI
- `Actions/Intros/first-chat-intro.cs` — SB first-chat lookup
- `Actions/Intros/redeem-capture.cs` — SB channel-point redeem handler

## Schema mismatch policy

Hard stop on boot if `schemaVersion` in data file does not match code constant (Decision 23). Do not add silent fallbacks.

## Key references

- Architecture: `Docs/INFO-SERVICE-PLAN.md` §Tech Stack, §Schemas
- Decisions 1–28: `humans/info-service/COORDINATION.md`
- REST contract: `.agents/_shared/info-service-protocol.md`
- Port + URL constants: `Actions/SHARED-CONSTANTS.md` §Info Service / Assets
