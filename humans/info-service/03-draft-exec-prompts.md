# Prompt P3 — Draft First Two Execution Prompts

Paste everything below this line into a fresh coding-agent chat window.

---

You are working in the `SharkStreamerBot` repo. Fresh session — no memory of prior conversations. All context is in committed files.

## Read first

1. `CLAUDE.md`
2. `.agents/ENTRY.md`
3. `humans/info-service/README.md`
4. `humans/info-service/COORDINATION.md` — current Decisions, chunk status, run log
5. `Docs/INFO-SERVICE-PLAN.md` — hardened architecture plan (must include `## Schemas` section from P2)
6. `humans/info-service/00-seed-plan.md`, `01-harden-questions.md`, `02-schemas.md` — prior prompts, for style + structure
7. `.agents/roles/app-dev/role.md`
8. `.agents/_shared/conventions.md`
9. `Actions/SHARED-CONSTANTS.md`

Role: `app-dev`.

## WORKING.md

Add row, remove + log at end. Domain: `humans/`. Files: `humans/info-service/10-*.md`, `11-*.md`.

## Task

Draft exactly **two** execution-prompt files for the next two chunks in `COORDINATION.md` chunk status table that are in `not-started` state:

- C1 → `humans/info-service/10-<slug>.md`
- C2 → `humans/info-service/11-<slug>.md`

Use a short lowercase-hyphenated slug based on the chunk name (e.g. `10-assets-folder-convention.md`, `11-info-service-skeleton.md`).

Drip discipline: do NOT draft prompts beyond C2. Later chunks get drafted in the wake of earlier ones, when the current state of the repo is visible.

## Required structure for every execution-prompt file

Use this template. Adapt contents per chunk.

```
# Prompt C<n> — <Chunk Title>

Paste everything below this line into a fresh coding-agent chat window.

---

You are working in the `SharkStreamerBot` repo. Fresh session — no memory of prior conversations.

## Read first

1. `CLAUDE.md`
2. `.agents/ENTRY.md`
3. `.agents/roles/<role>/role.md`
4. `humans/info-service/COORDINATION.md`
5. `Docs/INFO-SERVICE-PLAN.md`
6. <any chunk-specific files>

Role: `<role>`.

## Prereqs

Chunks merged: <list>. Verify with `git log` before starting.

## WORKING.md

Add row at start. Domain: `<domain>`. Files: `<file list>`. Remove + log at finish.

## Task

<concrete task description, scope-locked>

## Deliverables

- Files changed: <exact list>
- Scaffolding updates: <exact .agents/ files + what to add>
- Shared constants: <exact SHARED-CONSTANTS.md additions if any>
- Tests: <list or N/A>

## Forbidden in this chunk

- <explicit out-of-scope items>

## Finish

1. Run any build/typecheck commands relevant to the package.
2. Update `humans/info-service/COORDINATION.md` — chunk status → `merged`, append Run Log row (Commit column: `uncommitted`).
3. `WORKING.md` — remove Active Work row, add Recently Completed row (trim to 10).
4. Load `ops-change-summary` skill, show output.
5. **Draft the next chunk's prompt file** (self-propagation). Rules:
   - Find the next chunk in `COORDINATION.md` with status `not-started`, Prompt File `tbd`, and all prereqs `merged` (including the one just merged).
   - If such a chunk exists, create `humans/info-service/1<N>-<slug>.md` using this same template (including this Finish step).
   - If no such chunk exists (prereq blocked), skip this step and note the blocker in chat.
   - Update `COORDINATION.md` Prompt File column + set that chunk's status to `prompt-ready`.

**Do NOT run `git commit` or `git add`. Operator commits manually.**

## Definition of done

<concrete, verifiable list, ending with "Next-chunk prompt drafted OR blocker reported." and "No git commit made by agent.">
```

## Chunk-specific contents (fill in per the two chunks)

### C1 — Assets Folder Convention + SHARED-CONSTANTS

- Role: `app-dev` (or `ops` — pick whichever makes more sense; `app-dev` probably fits since it establishes app-asset convention).
- No code. Documentation + folder scaffold only.
- Deliverables:
  - `Assets/` folder at repo root with `user-intros/sound/.gitkeep` and `user-intros/gif/.gitkeep`
  - `Assets/README.md` explaining: asset root purpose, folder convention, gitignore rule, MixItUp path expectation
  - `.gitignore` addition — ignore binary extensions under `Assets/` but keep `.gitkeep`, `README.md`
  - `Actions/SHARED-CONSTANTS.md` new section `## Info Service / Assets (shared)` listing `ASSETS_ROOT`, `INFO_SERVICE_URL`, `INFO_SERVICE_PORT`, `PRODUCTION_MANAGER_PORT`, `ASSETS_USER_INTROS_SOUND_SUBPATH`, `ASSETS_USER_INTROS_GIF_SUBPATH` — use values from plan. Mark any still-TBD values clearly.
- Forbidden: creating `Apps/info-service/` or `Apps/production-manager/` (later chunks).
- Scaffolding updates: none in this chunk.

### C2 — info-service skeleton

- Role: `app-dev`.
- Prereqs: C1 merged.
- Deliverables:
  - `Apps/info-service/` with `package.json`, `tsconfig.json`, `src/index.ts`, `src/server.ts`, `README.md`
  - HTTP framework per plan Tech Stack decision
  - Health route `GET /health` returning `{ ok: true, uptime, collections: [] }`
  - Binds `127.0.0.1` on port from SHARED-CONSTANTS
  - zod (or chosen validator) added as dep
  - `npm run dev` + `npm run build` scripts
  - No collection engine yet (C3), no collection data (C4), no non-health routes (C5)
  - Scaffolding updates:
    - `.agents/roles/app-dev/role.md` — add `Apps/info-service/` to scope
    - `.agents/roles/app-dev/context/info-service.md` (new) — one-page orientation for future agents: what info-service is, port, how to run, extension points
    - `.agents/_shared/info-service-protocol.md` (new, placeholder shape) — initial section documenting health route + future route slots; expands in C5
- Forbidden: collection engine, data persistence, routes beyond `/health`, React work.

## Do NOT in this prompt

- Do not draft execution prompts beyond C1 and C2.
- Do not modify `Docs/INFO-SERVICE-PLAN.md`.
- Do not write any code for the chunks themselves.
- Do not skip the shared template.

## Finish

1. Save `humans/info-service/10-<slug>.md` and `humans/info-service/11-<slug>.md`.
2. Update `COORDINATION.md`:
   - Fill the `Prompt File` column for C1 and C2 with the real filenames.
   - Chunk status C1, C2 → `prompt-ready`. P3 status → `merged`.
   - Append Run Log row (Commit column: `uncommitted`).
3. `WORKING.md` update.
4. Load `ops-change-summary` skill, show output.

**Do NOT run `git commit` or `git add`. Operator commits manually. This applies both to P3 itself AND to every execution prompt you draft — the template above already omits the commit step, keep it that way.**

## Definition of done

- Exactly two new files: `humans/info-service/10-*.md` and `humans/info-service/11-*.md`.
- Both follow the required template (no commit step).
- Both align with the plan doc's Tech Stack and Schemas decisions.
- COORDINATION chunk status reflects new prompt filenames.
- No git commit made by you.
