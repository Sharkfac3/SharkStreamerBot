# Prompt P0 — Seed Info-Service Plan Doc

Paste everything below this line into a fresh coding-agent chat window.

---

You are working in the `SharkStreamerBot` repo. This is a fresh session — you have no memory of prior planning conversations. All context lives in committed files.

## Read first (in this order)

1. `CLAUDE.md` — root project rules
2. `.agents/ENTRY.md` — role navigation
3. `.agents/_shared/project.md` — domains + priorities
4. `.agents/_shared/conventions.md` — git, naming, file routing
5. `humans/info-service/README.md` — this prompt series overview
6. `humans/info-service/COORDINATION.md` — decisions already made, chunk list, open questions
7. `Actions/SHARED-CONSTANTS.md` — just the "Overlay / Broker (shared)" section, to match style
8. `.agents/_shared/mixitup-api.md` — style reference for future `_shared/info-service-protocol.md`

Your role for this task: `app-dev` (see `.agents/roles/app-dev/role.md`).

## WORKING.md (mandatory)

Before editing anything, add a row to `WORKING.md` Active Work:

- Agent: `claude`
- Task: `Seed INFO-SERVICE-PLAN.md`
- Domain: `Docs/`
- Files: `Docs/INFO-SERVICE-PLAN.md`
- Started: today's date

When done, remove the row, add to Recently Completed.

## Task

Create a new file `Docs/INFO-SERVICE-PLAN.md` that captures the architecture plan for the info-service + production-manager work. This is the stable architecture artifact — it is consumed read-only by every future execution prompt.

Use every decision already recorded in `humans/info-service/COORDINATION.md` Decisions table as authoritative. Do not re-open resolved decisions. If you find contradictions between decisions, stop and flag in the chat.

## Required sections of the plan doc

Use these exact H2 headings in this order:

1. `## Goal` — one paragraph, what this body of work delivers + why.
2. `## Non-Goals` — bullet list, things explicitly out of scope (no SQLite day 1, no auth, no LAN exposure, no new cast, no overlay behavior changes).
3. `## Architecture Overview` — prose + a simple ASCII diagram showing the pieces and who talks to whom:
   - `Streamer.bot` (First Chat trigger, HTTP GET to info-service, MixItUp command dispatch)
   - `info-service` (Node.js REST, file-backed JSON, 127.0.0.1, port TBD)
   - `production-manager` (React admin, talks to info-service write routes)
   - `stream-overlay/broker` (unchanged, dumb router)
   - `stream-overlay/overlay` (unchanged)
   - `MixItUp` (plays intro sound, reads file from `Assets/user-intros/`)
   - `Assets/` repo-root folder (binary media, gitignored)
4. `## Folder Layout` — fenced tree showing where new packages + asset root land. Mirror structure agreed in COORDINATION.
5. `## Data Model (high level)` — describe the collection envelope shape `{ schemaVersion, collection, updatedUtc, records }` and the `user-intros` record fields at a high level. Leave concrete zod schema to P2.
6. `## Tech Stack` — list chosen stacks, BUT mark items as `RESOLVED` or `OPEN`:
   - `info-service` HTTP framework: OPEN (Fastify vs Express vs plain http)
   - `info-service` validator: tentative `zod`
   - `production-manager`: RESOLVED — React + Vite. Component library + data-fetching lib: OPEN.
   - Ports: OPEN (info-service, production-manager dev server)
7. `## Chunk List` — table with columns `#`, `Chunk`, `Prereqs`, `Summary`, `Scaffolding Updates`. Mirror COORDINATION chunk status but richer (one-line summary + which `.agents/` files each chunk must touch).
8. `## Scaffolding Impact` — enumerate every `.agents/` file that will be touched across the whole project (role.md updates, new skills files, new _shared protocol docs). One list, not per-chunk.
9. `## Constants to Register` — list every entry that will end up in `Actions/SHARED-CONSTANTS.md` (ASSETS_ROOT, INFO_SERVICE_URL, INFO_SERVICE_PORT, PRODUCTION_MANAGER_PORT, topic/event names if any). Mark values TBD where still open.
10. `## Risks & Mitigations` — at minimum: write amplification on large JSON, schema drift without migrations, prompt rot if execution prompts drafted too far ahead, coordination between operator-authored channel-point redeems and manual data entry.

## Do NOT in this prompt

- Do not add code.
- Do not create `Apps/info-service/` or `Apps/production-manager/` folders.
- Do not modify `.agents/` scaffolding yet — that happens inside execution chunks.
- Do not pick ports or the HTTP framework. Leave as OPEN.
- Do not draft execution prompts. That is P3.
- Do not write schemas in detail. That is P2.

## Finish

1. Save the plan doc.
2. Update `humans/info-service/COORDINATION.md`:
   - Chunk status row for P0 → `merged` (after commit)
   - Append a row to Run Log with today's date, prompt name, outcome, commit hash.
3. Update `WORKING.md` — remove Active Work row, add Recently Completed row (drop oldest entry if list exceeds 10).
4. Load `ops-change-summary` skill from `.agents/roles/ops/skills/change-summary/_index.md` and produce its paste targets + validation checklist.
5. Commit style: match recent commits in `git log`. Commit directly to `main` per `CLAUDE.md` (small focused change).

## Definition of done

- `Docs/INFO-SERVICE-PLAN.md` exists with all 10 required sections.
- Every decision from COORDINATION Decisions table is reflected.
- `COORDINATION.md` chunk status + run log updated.
- `WORKING.md` updated.
- `ops-change-summary` output shown in final chat message.
- Commit landed on `main`.
