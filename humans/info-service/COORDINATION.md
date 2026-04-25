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
| 16 | HTTP framework for `info-service`: **Fastify** | 2026-04-24 | Q1 resolved. Best TS/zod integration, minimal footprint. |
| 17 | Schema validator: **zod** | 2026-04-24 | Q2 resolved. Mature, excellent TS inference, pairs with Fastify via `@fastify/type-provider-zod`. |
| 18 | `production-manager` data-fetching: **plain fetch** | 2026-04-24 | Q3 resolved. Operator-only local app; no caching/background-sync needed in v1. |
| 19 | `production-manager` component library: **shadcn/ui** (Radix UI + Tailwind) | 2026-04-24 | Q4 resolved. Good primitives without a runtime library dependency. |
| 20 | Port assignments: `info-service` = **8766**, pm-dev = **5174**, pm-prod-preview = **4174** | 2026-04-24 | Q5 resolved. 8766 adjacent to broker (8765); 5174/4174 follow overlay's 5173/4173. |
| 21 | `ASSETS_ROOT` constant: operator sets absolute path in `SHARED-CONSTANTS.md` | 2026-04-24 | Q6 resolved. Consistent with existing operator-set values in SHARED-CONSTANTS. |
| 22 | Data folder: `Apps/info-service/data/`, **gitignored** | 2026-04-24 | Q7 resolved. Runtime state; operator responsible for backup. |
| 23 | Schema version mismatch policy: **error out / refuse to start** | 2026-04-24 | Q8 resolved. Hard stop is safest signal for single-operator use. |
| 24 | First-chat 404 behavior: **silent no-op** | 2026-04-24 | Q9 resolved. No auto-pending; redeem flow (C10) handles explicit opt-in. |
| 25 | Redeem capture: **full automation target** — `pending-intros` collection in v1 scope | 2026-04-24 | Q10 resolved. SB redeem handler POSTs to `/info/pending-intros`; operator fulfills via production-manager. |
| 26 | `production-manager` auth: **none in v1** | 2026-04-24 | Q11 resolved. 127.0.0.1 binding sufficient; no sensitive PII beyond Twitch usernames. |
| 27 | Scaffolding: **extend `app-dev` role** — no new role | 2026-04-24 | Q12 resolved. Both services are TS/React under `Apps/`; fit existing activation criteria. |
| 28 | Commit discipline: **all chunks direct to `main`**, operator commits manually between runs | 2026-04-24 | Q13 resolved. Per project convention; WORKING.md prevents conflicts. |

## Open Questions (awaiting operator)

| # | Question | Date | Notes |
|---|----------|------|-------|
| Q14 | Should C10 include a `production-manager` pending-intros fulfillment page (table of pending records + fulfill/reject buttons), or is this a separate chunk (C10.5)? Precedent: C7 (user-intros PM page) was separate from C4 (user-intros collection). If separate, add C10.5 to the chunk list and update this file. | 2026-04-24 | `18-redeem-capture.md` explicitly marks PM UI out of scope pending this answer. |

---

## Chunk status

Status values: `not-started`, `prompt-ready`, `in-progress`, `merged`, `blocked`.

| # | Chunk | Prompt File | Status | Prereqs | Notes |
|---|-------|-------------|--------|---------|-------|
| P0 | Seed plan doc | `00-seed-plan.md` | merged | — | |
| P1 | Harden questions | `01-harden-questions.md` | merged | P0 | Wrote Q1–Q13 to Open Questions |
| P2 | Propose schemas | `02-schemas.md` | merged | P1 + operator answers | |
| P3 | Draft first exec prompts | `03-draft-exec-prompts.md` | merged | P2 | Writes prompts `10-*.md` and `11-*.md` |
| C1 | Assets folder + SHARED-CONSTANTS | `10-assets-folder-convention.md` | merged | P3 | No code — convention + docs only |
| C2 | info-service skeleton | `11-info-service-skeleton.md` | merged | C1 | Fastify, health route, bind 127.0.0.1 |
| C3 | Collection engine | `12-collection-engine.md` | merged | C2 | Generic `Collection<T>` + zod validator |
| C4 | user-intros collection | `13-user-intros-collection.md` | merged | C3 | |
| C5 | REST routes | `14-rest-routes.md` | merged | C4 | `/info/:collection`, `/info/:collection/:key` |
| C6 | production-manager skeleton | `15-production-manager-skeleton.md` | merged | C2 | React + Vite |
| C7 | production-manager: user-intros page | `17-production-manager-user-intros.md` | merged | C4, C6 | Table + form + file picker |
| C8 | SB first-chat script | `16-sb-first-chat.md` | merged | C5 | `Actions/Intros/first-chat-intro.cs` |
| C9 | MixItUp Custom Intro command spec | `17-mixitup-custom-intro.md` | merged | C8 | Updates `.agents/_shared/mixitup-api.md` |
| C10 | Redeem capture | `18-redeem-capture.md` | merged | C5 | Pending-intros collection |
| C11 | Docs + scaffolding sweep | `19-docs-scaffolding-sweep.md` | prompt-ready | all above | Final `app-dev` role update, `_shared/info-service-protocol.md` polish |

---

## Run log

Append a row each time a prompt runs. Newest first.

| Date | Prompt | Agent | Outcome | Commit |
|------|--------|-------|---------|--------|
| 2026-04-24 | 04-draft-next-exec-prompts.md (drip-meta, C11) | claude | Drafted `19-docs-scaffolding-sweep.md` (C11); C11 → prompt-ready | uncommitted |
| 2026-04-24 | 17-mixitup-custom-intro.md | claude | Appended `## Custom Intro Command` section to `.agents/_shared/mixitup-api.md`; covers command name, trigger action, argument format, MixItUp + SB setup checklists; C9 → merged | uncommitted |
| 2026-04-24 | 16-sb-first-chat.md | claude | Created `Actions/Intros/first-chat-intro.cs`; HTTP GET user-intros by userId; handles 404/non-200 as no-op; checks enabled + soundFile; constructs full asset path; dispatches via CPH.SetGlobalVar + CPH.RunAction("Intros - Play Custom Intro"); all branches logged; updated SHARED-CONSTANTS.md Used-in; C8 → merged; C9 → prompt-ready with `17-mixitup-custom-intro.md` | uncommitted |
| 2026-04-24 | 18-redeem-capture.md | claude | Created `pending-intros` schema + collection; wired into `src/index.ts`; created `Actions/Intros/redeem-capture.cs` (GET duplicate check, POST create, all branches logged); updated `SHARED-CONSTANTS.md` (COLLECTION_PENDING_INTROS); updated `_shared/info-service-protocol.md` (pending-intros section); tsc passes; C10 → merged | uncommitted |
| 2026-04-24 | 04-draft-next-exec-prompts.md (drip-meta) | claude | Drafted `18-redeem-capture.md` (C10); C10 → prompt-ready; added Open Question Q14 (PM pending-intros UI gap) | uncommitted |
| 2026-04-24 | 17-production-manager-user-intros.md | claude | Created `src/pages/UserIntrosPage.tsx` (table, modal form, create/edit/delete via plain fetch); updated `src/App.tsx` (nav + page switch health/user-intros); updated `src/index.css` (input-base component class); typecheck passes; C7 → merged | uncommitted |
| 2026-04-24 | 15-production-manager-skeleton.md | claude | Scaffolded `Apps/production-manager/` (package.json, tsconfig.json, vite.config.ts, index.html, src/main.tsx, src/App.tsx, src/pages/HealthPage.tsx, src/index.css, tailwind.config.ts, postcss.config.js, README.md); Tailwind v3 PostCSS path; `.gitignore` block; `.agents/roles/app-dev/role.md` active apps note; typecheck passes; C6 → merged | uncommitted |
| 2026-04-24 | 14-rest-routes.md | claude | Created `src/routes/read.ts` (GET /info/:collection, GET /info/:collection/:key); `src/routes/write.ts` (POST/PUT/DELETE /info/:collection/:key); updated `src/server.ts` (Collection registry + route plugin registration); updated `src/index.ts` (pass instances not names); typecheck passes; C5 → merged; C8 → prompt-ready; drafted `16-sb-first-chat.md` | uncommitted |
| 2026-04-24 | 13-user-intros-collection.md | claude | Created `src/store/schemas/user-intros.ts` (UserIntroRecordSchema); `src/collections/user-intros.ts` (Collection instance); updated `src/index.ts` (async main, load+wire); updated `_shared/info-service-protocol.md`; drafted `14-rest-routes.md`; C4 → merged; C5 → prompt-ready | uncommitted |
| 2026-04-24 | 12-collection-engine.md | claude | Created `src/store/collection.ts` (Collection<T> engine); updated `src/server.ts` (buildServer opts); drafted `13-user-intros-collection.md`; C3 → merged; C4 → prompt-ready | uncommitted |
| 2026-04-24 | 04-draft-next-exec-prompts.md | claude | Drafted `12-collection-engine.md` (C3) and `15-production-manager-skeleton.md` (C6); C3+C6 → prompt-ready | uncommitted |
| 2026-04-24 | 11-info-service-skeleton.md | claude | Scaffolded `Apps/info-service/` (package.json, tsconfig, src/index.ts, src/server.ts, README); `.gitignore` block; `.agents/roles/app-dev/role.md` active apps note; `context/info-service.md`; `_shared/info-service-protocol.md`; typecheck passes; C2 → merged | uncommitted |
| 2026-04-24 | 10-assets-folder-convention.md | claude | Created `Assets/` folder structure + `.gitkeep` files; `Assets/README.md`; `.gitignore` media block; `Actions/SHARED-CONSTANTS.md` §Info Service / Assets; C1 → merged | uncommitted |
| 2026-04-24 | 03-draft-exec-prompts.md | claude | Drafted `10-assets-folder-convention.md` and `11-info-service-skeleton.md`; C1/C2 → prompt-ready; P3 → merged | uncommitted |
| 2026-04-24 | 02-schemas.md | claude | Added §Schemas (items 1–8) to INFO-SERVICE-PLAN.md; flipped all OPEN Tech Stack items to RESOLVED; moved Q1–Q13 to Decisions (entries 16–28); P2 → merged | uncommitted |
| 2026-04-24 | 01-harden-questions.md | claude | Wrote Q1–Q13 to Open Questions in COORDINATION.md; P1 → merged | uncommitted |
| 2026-04-24 | 00-seed-plan.md | claude | Created `Docs/INFO-SERVICE-PLAN.md` with all 10 required sections | fa64c29 |
| 2026-04-24 | seed prompt series | claude | Created `humans/info-service/` with README, COORDINATION, P0-P3 prompts | uncommitted |
