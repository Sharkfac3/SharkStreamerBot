# Prompt P4 — Drip-Draft Next Execution Prompts (reusable)

This prompt is **reusable**. Re-run it any time `humans/info-service/COORDINATION.md` shows `not-started` chunks without a prompt file, after their prereqs merge. It drafts the next two prompts in line. Self-propagation (Finish step in each drafted prompt) should normally keep the chain moving; this prompt is the fallback / catch-up tool when a chain breaks or chunks lack prereq completion.

Paste everything below this line into a fresh coding-agent chat window.

---

You are working in the `SharkStreamerBot` repo. Fresh session — no memory of prior conversations. All context is in committed files.

## Read first

1. `CLAUDE.md`
2. `.agents/ENTRY.md`
3. `humans/info-service/README.md` — especially the "Prompt authoring rules" section
4. `humans/info-service/COORDINATION.md` — Decisions, chunk status, run log
5. `Docs/INFO-SERVICE-PLAN.md` — architecture plan + Schemas section
6. `humans/info-service/03-draft-exec-prompts.md` — prompt template reference
7. The **two most recent** execution prompt files already drafted under `humans/info-service/1*.md` — match their style + structure
8. `.agents/roles/app-dev/role.md` and `.agents/roles/streamerbot-dev/role.md` — role references for chunks that may fall to either role
9. Current state of `Apps/info-service/`, `Apps/production-manager/`, and `Actions/Intros/` — whatever exists already, so drafted prompts match reality not plan

Role for this task: `app-dev`.

## WORKING.md

Add row at start. Domain: `humans/`. Files: `humans/info-service/<new prompt filenames>`. Remove + log at finish.

## Task

Scan `humans/info-service/COORDINATION.md` chunk status table. Find the next chunks matching ALL of:

- Status is `not-started`
- Prompt File column is `tbd` (no file drafted yet)
- Every chunk listed in Prereqs column has status `merged`

Draft prompts for up to **two** such chunks — the two with lowest chunk numbers. If fewer than two qualify (e.g. only one prereq-satisfied chunk exists), draft just that one. If zero qualify, stop and report why in the chat (likely: in-flight chunk still `not-started` blocks progress, or all chunks merged).

File naming: `1<N>-<short-slug>.md` where `<N>` matches the chunk number offset (C3 → `12-`, C4 → `13-`, C5 → `14-`, C6 → `15-`, C7 → `16-`, C8 → `17-`, C9 → `18-`, C10 → `19-`, C11 → `20-`). Slug lowercase-hyphenated from chunk name.

## Template every drafted prompt must follow

Use the template below. Adapt `Task`, `Deliverables`, `Forbidden`, `Definition of done` per chunk. Keep all other sections.

```
# Prompt C<n> — <Chunk Title>

Paste everything below this line into a fresh coding-agent chat window.

---

You are working in the `SharkStreamerBot` repo. Fresh session — no memory of prior conversations.

## Read first

1. `CLAUDE.md`
2. `.agents/ENTRY.md`
3. `.agents/roles/<role>/role.md`
4. `humans/info-service/README.md`
5. `humans/info-service/COORDINATION.md`
6. `Docs/INFO-SERVICE-PLAN.md`
7. <any chunk-specific files — existing code this chunk builds on>

Role: `<role>`.

## Prereqs

Chunks merged: <list>. Verify with `git log` and by inspecting files before starting.

## WORKING.md

Add row at start. Domain: `<domain>`. Files: `<file list>`. Remove + log at finish.

## Task

<concrete task, scope-locked to this chunk>

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
4. Load `ops-change-summary` skill from `.agents/roles/ops/skills/change-summary/_index.md`, show output.
5. **Draft the next chunk's prompt file** (self-propagation). Rules:
   - Find the next chunk in `COORDINATION.md` that has status `not-started`, Prompt File `tbd`, and all prereqs `merged` (including the one this prompt just merged).
   - If such a chunk exists, create `humans/info-service/1<N>-<slug>.md` using this same template (including this Finish step).
   - If no such chunk exists (prereq blocked, or all chunks merged), skip this step and note in the chat which chunk is blocked and why.
   - Update `COORDINATION.md` Prompt File column + set that chunk's status to `prompt-ready`.

**Do NOT run `git commit` or `git add`. Operator commits manually.**

## Definition of done

<concrete, verifiable list. Include: "Next-chunk prompt drafted OR blocker reported." and "No git commit made by agent.">
```

## Source-of-truth for chunk details

Do not invent scope. Pull each chunk's `Task`, `Deliverables`, `Forbidden` from:

- The chunk's row in `COORDINATION.md` chunk status table (gives one-line summary + prereqs)
- The relevant section of `Docs/INFO-SERVICE-PLAN.md` (Chunk List table + Scaffolding Impact + Constants to Register sections)
- The Schemas section of the plan doc (for any chunk that handles a collection)
- Existing repo state (what already exists, what naming/style conventions are established)

If any chunk detail is ambiguous or contradicts the plan, STOP and append an Open Question to `COORDINATION.md` instead of guessing. Do not proceed to draft that prompt until operator resolves.

## Do NOT in this prompt

- Do not draft more than two prompts in one run. Drip discipline.
- Do not modify `Docs/INFO-SERVICE-PLAN.md`.
- Do not modify any code under `Apps/` or `Actions/`.
- Do not commit, do not stage (`git add`). Operator commits.
- Do not omit the self-propagation Finish step from any drafted prompt.

## Finish (for this meta prompt)

1. Save the up-to-two new prompt files.
2. Update `humans/info-service/COORDINATION.md`:
   - Fill Prompt File column for drafted chunks with new filenames.
   - Set drafted chunks' status to `prompt-ready`.
   - Append a Run Log row (newest first) describing which chunks got prompts. Commit column: `uncommitted`.
3. `WORKING.md` — remove Active Work row, add Recently Completed row (trim to 10).
4. Load `ops-change-summary` skill, show output.

**Do NOT run `git commit` or `git add`. Operator commits manually.**

## Definition of done

- Up to two new prompt files saved under `humans/info-service/1*.md`.
- Each drafted prompt follows the full template including the self-propagation Finish step.
- Each drafted prompt's scope pulled from plan + COORDINATION, not invented.
- COORDINATION chunk status + run log updated.
- No git commit made by you.
