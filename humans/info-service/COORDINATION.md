# Info Service — Coordination

Live state across fresh-chat runs. Operator + agents both read + write here.

This file is process. `Docs/INFO-SERVICE-PLAN.md` is architecture. Different lifetimes — do not mix.

---

## Decisions (resolved)

| # | Decision | Date | Notes |
|---|----------|------|-------|
| 1 | Option 2 from earlier analysis: extend ecosystem with persistent store + REST + admin UI | 2026-04-24 | Long-term scalable path chosen over SB-only user-vars |
| 2 | Store type: plain JSON files per collection, single-writer, atomic temp-file rename | 2026-04-24 | Upgrade to SQLite only if a collection crosses ~10k records or query needs emerge |
| 3 | Store lives in new service named `info-service`, not inside broker | 2026-04-24 | Broker stays "dumb router" by identity |
| 4 | `info-service` lives at `Apps/info-service/` (sibling to `Apps/stream-overlay/`), not nested under stream-overlay packages | 2026-04-24 | Generic data layer, not overlay-scoped |
| 5 | Admin web app named `production-manager`, lives at `Apps/production-manager/`, built with React | 2026-04-24 | Per-operator-directive |
| 6 | Binary assets (sound/gif files) live at repo-root `Assets/` folder, gitignored, outside all services | 2026-04-24 | MixItUp reads arbitrary paths; keeps services portable |
| 7 | Store records hold filenames only, consumers join with known asset-root constant | 2026-04-24 | Keeps data portable if asset folder moves |
| 8 | Every collection file uses envelope `{ schemaVersion, collection, updatedUtc, records }` | 2026-04-24 | Migration hook cheap day 1 |
| 9 | Single-writer: `production-manager` writes only. `info-service` REST exposes writes but by policy only admin app calls write routes. SB + overlay read-only. | 2026-04-24 | Keeps file I/O lock-free |
| 10 | Services bind `127.0.0.1` only | 2026-04-24 | No LAN exposure. No auth needed day 1. |
| 11 | First collection: `user-intros`. Records keyed by Twitch userId. | 2026-04-24 | |
| 12 | Intro audio plays via MixItUp command, not overlay | 2026-04-24 | Keeps overlay engine uncluttered |
| 13 | Streamer.bot native "First Chat" trigger fires the intro lookup | 2026-04-24 | |
| 14 | Prompt series lives in `humans/info-service/`, coordination in this file, architecture plan in `Docs/INFO-SERVICE-PLAN.md` | 2026-04-24 | |
| 15 | Execution prompts drafted lazily — one or two chunks ahead of current work, not all up front | 2026-04-24 | Drift-proof |

## Open Questions (awaiting operator)

_P1 will populate this section. Operator answers inline. Move to Decisions table when resolved._

---

## Chunk status

Status values: `not-started`, `prompt-ready`, `in-progress`, `merged`, `blocked`.

| # | Chunk | Prompt File | Status | Prereqs | Notes |
|---|-------|-------------|--------|---------|-------|
| P0 | Seed plan doc | `00-seed-plan.md` | merged | — | |
| P1 | Harden questions | `01-harden-questions.md` | prompt-ready | P0 | Writes Open Questions here |
| P2 | Propose schemas | `02-schemas.md` | prompt-ready | P1 + operator answers | |
| P3 | Draft first exec prompts | `03-draft-exec-prompts.md` | prompt-ready | P2 | Writes prompts `10-*.md` and `11-*.md` |
| C1 | Assets folder + SHARED-CONSTANTS | `10-*.md` (draft by P3) | not-started | P3 | No code — convention + docs only |
| C2 | info-service skeleton | `11-*.md` (draft by P3) | not-started | C1 | Express/Fastify (TBD in P1), health route, bind 127.0.0.1 |
| C3 | Collection engine | tbd | not-started | C2 | Generic `Collection<T>` + zod validator |
| C4 | user-intros collection | tbd | not-started | C3 | |
| C5 | REST routes | tbd | not-started | C4 | `/info/:collection`, `/info/:collection/:key` |
| C6 | production-manager skeleton | tbd | not-started | C2 | React + Vite |
| C7 | production-manager: user-intros page | tbd | not-started | C4, C6 | Table + form + file picker |
| C8 | SB first-chat script | tbd | not-started | C5 | `Actions/Intros/first-chat-intro.cs` |
| C9 | MixItUp Custom Intro command spec | tbd | not-started | C8 | Updates `.agents/_shared/mixitup-api.md` |
| C10 | Redeem capture | tbd | not-started | C5 | Pending-intros collection |
| C11 | Docs + scaffolding sweep | tbd | not-started | all above | Final `app-dev` role update, `_shared/info-service-protocol.md` polish |

---

## Run log

Append a row each time a prompt runs. Newest first.

| Date | Prompt | Agent | Outcome | Commit |
|------|--------|-------|---------|--------|
| 2026-04-24 | 00-seed-plan.md | claude | Created `Docs/INFO-SERVICE-PLAN.md` with all 10 required sections | fa64c29 |
| 2026-04-24 | seed prompt series | claude | Created `humans/info-service/` with README, COORDINATION, P0-P3 prompts | uncommitted |
