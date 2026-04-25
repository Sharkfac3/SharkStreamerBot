You are working in the `SharkStreamerBot` repo. Fresh session — no memory of prior conversations.

## Read first

1. `CLAUDE.md`
2. `.agents/ENTRY.md`
3. `.agents/roles/app-dev/role.md`
4. `humans/info-service/README.md`
5. `humans/info-service/COORDINATION.md`
6. `Docs/INFO-SERVICE-PLAN.md` — §Chunk List, §Scaffolding Impact, §Schemas (routes + error conventions)
7. `.agents/_shared/info-service-protocol.md` — current state (stale placeholders to remove)
8. `.agents/roles/app-dev/context/info-service.md` — current state
9. `.agents/roles/app-dev/skills/core.md` — current state
10. `.agents/roles/app-dev/skills/stream-interactions/_index.md` — current state
11. `Apps/info-service/README.md` — current state (stale "What's coming" section)
12. `Apps/production-manager/README.md` — current state (stale "Coming next" section)
13. `Apps/info-service/src/routes/read.ts` — actual GET routes implemented
14. `Apps/info-service/src/routes/write.ts` — actual write routes implemented
15. `Apps/info-service/src/store/collection.ts` — error handling patterns
16. `Apps/info-service/src/index.ts` — current collections wired (user-intros + pending-intros)

Role: `app-dev`. This chunk is **docs + scaffolding only** — no code changes under `Apps/src/` or `Actions/`.

## Prereqs

Chunks merged: C1–C10. Verify by checking that all of the following exist:
- `Apps/info-service/src/routes/write.ts`
- `Apps/info-service/src/collections/pending-intros.ts`
- `Actions/Intros/first-chat-intro.cs`
- `Actions/Intros/redeem-capture.cs`

## WORKING.md

Add row at start. Domain: `.agents/`, `Apps/info-service/`, `Apps/production-manager/`. Files: `.agents/_shared/info-service-protocol.md`, `.agents/roles/app-dev/context/info-service.md`, `.agents/roles/app-dev/skills/core.md`, `.agents/roles/app-dev/skills/stream-interactions/_index.md`, `Apps/info-service/README.md`, `Apps/production-manager/README.md`. Remove + log at finish.

## Task

Final docs and scaffolding pass for the info-service build. All code is already merged (C1–C10). This chunk only updates documentation and agent scaffolding to reflect the completed state.

**Do NOT modify any `.ts`, `.tsx`, `.cs`, or `package.json` files. Docs and `.md` files only.**

### 1. `.agents/_shared/info-service-protocol.md`

Remove all stale placeholder text and update with real implemented behavior:

- **Top-note:** Remove the `> Final polish in C11.` note. Replace with nothing (or a brief context line if needed for the section).
- **Health route:** Update `collections` example to include both `"user-intros"` and `"pending-intros"` (both are registered in `src/index.ts`).
- **Routes section:** Replace the `## Planned routes (C5 — not yet implemented)` section with `## Routes` (implemented). Keep the same route table but remove the "not yet implemented" qualifier and the "(C5 —" prefix.
- **Error conventions:** Replace the `## Error conventions (C5 — not yet specified)` placeholder with a real section. Read `Apps/info-service/src/routes/read.ts` and `write.ts` to determine the actual error shapes returned. Document:
  - `404` — unknown collection name or missing record key
  - `400` — zod validation failure on write (include shape of zod error response if Fastify returns one)
  - `409` — if implemented for write conflicts (check write.ts; if not implemented, omit)
  - Format: brief description + example response body for each.
- **pending-intros note:** Remove the `> Note: production-manager UI for the pending-intros fulfillment queue is not yet implemented. Pending Open Question Q14...` note from the `pending-intros` section. Replace with: `> Note: production-manager PM UI for pending-intros fulfillment (C10.5) is pending operator decision on Open Question Q14 in \`humans/info-service/COORDINATION.md\`.`

### 2. `.agents/roles/app-dev/context/info-service.md`

Update to reflect all chunks merged:

- **Extension points table:** Fix incorrect entries. The table currently lists C4 as adding both `user-intros + pending-intros` — that is wrong. `user-intros` was C4; `pending-intros` was C10. Correct to:
  - C3: Collection engine
  - C4: `user-intros` collection + zod schema
  - C5: REST routes
  - C6: `production-manager` React admin app (skeleton)
  - C7: `production-manager` user-intros page (table, form, CRUD)
  - C8: `Actions/Intros/first-chat-intro.cs` — SB first-chat intro handler
  - C9: MixItUp Custom Intro command spec (`.agents/_shared/mixitup-api.md`)
  - C10: `pending-intros` collection + `Actions/Intros/redeem-capture.cs`
- Add a `## Current state` section at the top (above or below the existing opening paragraph) with a one-line status: "All C1–C10 chunks merged as of 2026-04-24. C11 docs sweep in progress."
- Add a `## Key files` section listing the most important files for a future agent picking up this domain:
  - `Apps/info-service/src/index.ts` — server entry, collections wired
  - `Apps/info-service/src/store/collection.ts` — generic Collection<T> engine
  - `Apps/info-service/src/store/schemas/` — zod schemas per collection
  - `Apps/info-service/src/collections/` — collection instances
  - `Apps/info-service/src/routes/read.ts` + `write.ts` — REST route plugins
  - `Apps/production-manager/src/pages/UserIntrosPage.tsx` — user-intros CRUD UI
  - `Actions/Intros/first-chat-intro.cs` — SB first-chat lookup
  - `Actions/Intros/redeem-capture.cs` — SB channel-point redeem handler

### 3. `.agents/roles/app-dev/skills/core.md`

Add a new section `## Info Service + Production Manager` after the existing `## Known Integration Points` section. Content:

```markdown
## Info Service + Production Manager

Two standalone apps added in the info-service build (C1–C11):

| App | Path | Port | Role |
|-----|------|------|------|
| `info-service` | `Apps/info-service/` | `8766` | File-backed JSON REST API for per-viewer data |
| `production-manager` | `Apps/production-manager/` | `5174` (dev) / `4174` (preview) | React admin app for managing info-service collections |

Both bind `127.0.0.1` only. No auth. No LAN exposure.

Key patterns:
- Collections use `Collection<T>` generic engine with atomic writes and zod schema validation.
- Single-writer: only `production-manager` calls write routes. Streamer.bot and overlay are read-only clients.
- Data lives in `Apps/info-service/data/` — gitignored, operator backs up manually.
- Binary assets (mp3, gif) live in `Assets/` at repo root — also gitignored.
- See `.agents/_shared/info-service-protocol.md` for REST route contracts and collection schemas.
- See `.agents/roles/app-dev/context/info-service.md` for key files and architecture orientation.
```

### 4. `.agents/roles/app-dev/skills/stream-interactions/_index.md`

Add a `## Info Service REST Interactions` section at the end of the file:

```markdown
## Info Service REST Interactions

`info-service` exposes REST routes consumed by Streamer.bot scripts (read-only) and `production-manager` (read + write).

Base URL: `http://127.0.0.1:8766`

| Pattern | Who calls it | Notes |
|---------|-------------|-------|
| `GET /health` | Anyone; health checks | Returns `{ ok, uptime, collections }` |
| `GET /info/:collection` | production-manager | All records in collection |
| `GET /info/:collection/:key` | SB scripts (read-only) | Single record by key |
| `POST /info/:collection/:key` | production-manager only | Create/replace record |
| `PUT /info/:collection/:key` | production-manager only | Update record |
| `DELETE /info/:collection/:key` | production-manager only | Delete record |

Collections registered at boot: `user-intros`, `pending-intros`.

Unknown collection name → `404`. Zod validation failure → `400`. Full contract in `.agents/_shared/info-service-protocol.md`.
```

### 5. `Apps/info-service/README.md`

Replace the stale `## What's coming` section (C3/C4/C5 items that are all done) with a `## Collections` section:

```markdown
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
```

Also update the health check example response to show real collections:
```json
{ "ok": true, "uptime": 12.345, "collections": ["user-intros", "pending-intros"] }
```

### 6. `Apps/production-manager/README.md`

Replace the `## What's here (C6)` + `## Coming next` sections with a `## Pages` section reflecting current state:

```markdown
## Pages

| Page | Path | Purpose |
|------|------|---------|
| Health | `/` | Fetches `GET /health` from info-service; shows status, uptime, and collections. |
| User Intros | `/user-intros` | Table of all user-intros records; create/edit/delete via modal form; soft-disable toggle. |

## Pending

- **Pending Intros fulfillment page** (C10.5): table of `pending-intros` records + fulfill/reject workflow. Blocked on operator decision for Open Question Q14 in `humans/info-service/COORDINATION.md`.
```

## Deliverables

- Files changed:
  - `.agents/_shared/info-service-protocol.md` (modified — remove placeholders, fill error conventions, update routes)
  - `.agents/roles/app-dev/context/info-service.md` (modified — fix extension points, add current state + key files)
  - `.agents/roles/app-dev/skills/core.md` (modified — add Info Service + PM section)
  - `.agents/roles/app-dev/skills/stream-interactions/_index.md` (modified — add REST interactions section)
  - `Apps/info-service/README.md` (modified — add Collections + Routes sections, remove "coming" items)
  - `Apps/production-manager/README.md` (modified — update pages section, add pending section)
- Scaffolding updates: all `.agents/` files listed above
- Shared constants: none
- Tests: N/A (docs only)

## Forbidden in this chunk

- Any changes to `.ts`, `.tsx`, `.cs`, `package.json`, `tsconfig.json`, or any other source/config files
- `Docs/INFO-SERVICE-PLAN.md` modifications
- Adding new chunks to COORDINATION.md without operator instruction
- Answering or resolving Open Question Q14 — flag it but do not act on it
- No `git commit` or `git add`

## Finish

1. No build step needed (docs only). Verify no source files were modified.
2. Update `humans/info-service/COORDINATION.md` — C11 status → `merged`, append Run Log row (Commit: `uncommitted`).
3. `WORKING.md` — remove Active Work row, add Recently Completed row (trim to 10).
4. Load `ops-change-summary` from `.agents/roles/ops/skills/change-summary/_index.md`, show output.
5. **Self-propagation check:**
   - C11 is the **last chunk** in the COORDINATION.md chunk list. All chunks C1–C11 will be merged.
   - No next chunk exists. Report in chat: "C11 is the final chunk. Info-service build complete. No further prompts to draft."
   - If Open Question Q14 has been resolved by the operator and a C10.5 row has been added to COORDINATION.md with status `not-started` and Prompt File `tbd` — draft that prompt instead of reporting no-op.

**Do NOT run `git commit` or `git add`. Operator commits manually.**

## Definition of done

- `.agents/_shared/info-service-protocol.md` has no stale placeholder text; routes section is marked implemented; error conventions filled; collections list updated
- `.agents/roles/app-dev/context/info-service.md` has correct extension points table (C4 = user-intros only; C10 = pending-intros); current state note; key files list
- `.agents/roles/app-dev/skills/core.md` has Info Service + PM section
- `.agents/roles/app-dev/skills/stream-interactions/_index.md` has REST interactions section
- `Apps/info-service/README.md` has Collections + Routes sections; no "What's coming" stale items
- `Apps/production-manager/README.md` has Pages table reflecting current C7 state; Pending section for Q14
- No source files (`.ts`, `.tsx`, `.cs`) modified
- `COORDINATION.md` C11 status = `merged`, run log row appended (Commit: `uncommitted`)
- Next-chunk prompt drafted OR "C11 is final chunk" reported in chat
- No git commit made by agent
