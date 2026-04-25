# Prompt P2 — Propose Schemas (Envelope + First Collection)

Paste everything below this line into a fresh coding-agent chat window.

---

You are working in the `SharkStreamerBot` repo. Fresh session — no memory of prior conversations. All context is in committed files.

## Read first

1. `CLAUDE.md`
2. `.agents/ENTRY.md`
3. `humans/info-service/README.md`
4. `humans/info-service/COORDINATION.md` — **especially the Decisions table AND every `Operator answer` filled in under Open Questions.** If any Open Question has a blank `Operator answer`, STOP and report which questions are unanswered. Do not proceed until operator has answered all relevant questions.
5. `Docs/INFO-SERVICE-PLAN.md` — the architecture plan
6. `.agents/roles/app-dev/role.md` — role for this task
7. `.agents/_shared/mixitup-api.md` — style reference for schema doc
8. `Actions/SHARED-CONSTANTS.md` — to understand the constants discipline
9. Any `package.json` in `Apps/stream-overlay/packages/` if a schema lib is already present there (zod, typebox) — reuse if so

Role: `app-dev`.

## WORKING.md

Add row at start, remove + log at end. Domain: `Docs/`. Files: `Docs/INFO-SERVICE-PLAN.md`.

## Task

Two outputs:

### Output A — update `Docs/INFO-SERVICE-PLAN.md`

Add a new H2 section `## Schemas` immediately after the existing `## Data Model (high level)` section. Contents:

1. **Envelope schema** — shape of every `<collection>.json` file on disk. Include:
   - `schemaVersion: number` — integer starting at 1
   - `collection: string` — must match filename base
   - `updatedUtc: number` — Unix ms of last write
   - `records: Record<string, T>` — keyed by stable record key
   Present as zod (or chosen validator) pseudocode AND as a canonical JSON example with one placeholder record.

2. **Record key policy** — what is used as the `records` key per collection. For `user-intros`: Twitch numeric userId as string. Explicit rule: keys must be stable identifiers, never display names.

3. **Timestamp policy** — all timestamps are Unix milliseconds in number fields. UTC. No ISO strings.

4. **Optional field policy** — explicit `T | undefined` modeling. Never null.

5. **`user-intros` record schema** — full field list with types, required/optional, constraints. Base fields (confirm + refine based on operator answers):
   - `userId: string` (required, numeric string)
   - `userLogin: string` (required, lowercase Twitch login at time of capture; stored but display name can change)
   - `soundFile?: string` — filename only, no path, must resolve under `Assets/user-intros/sound/`
   - `gifFile?: string` — filename only, under `Assets/user-intros/gif/`
   - `notes?: string`
   - `updatedUtc: number`
   - any additional fields the operator surfaced in P1 answers

6. **`pending-intros` record schema** (if operator answer in P1 confirmed future redeem capture) — proposed fields:
   - `userId: string`
   - `userLogin: string`
   - `redeemUtc: number`
   - `rewardTitle: string`
   - `userInput?: string` (raw redeem message from user)
   - `status: 'pending' | 'fulfilled' | 'rejected'`
   - `resolvedUtc?: number`
   - any fields operator requested

   If operator deferred `pending-intros`, mark this subsection `DEFERRED — not in v1 scope` and do not include schema.

7. **Migration policy** — what happens when `schemaVersion` on disk is older than code version. Match the operator answer from P1. Include the specific rule (error vs auto-migrate vs log-and-continue).

8. **Validation placement** — where zod runs: on load (reject bad file), on every POST/PUT write (reject bad payload before replacing in-memory record), on every read from disk after edit-by-hand. Make this explicit so execution chunks have one story.

### Output B — update `Tech Stack` section of the plan

Flip every item from `OPEN` to `RESOLVED` using the operator answers. If any item is still `OPEN` because operator deferred it, leave it OPEN and note the deferral.

## Do NOT in this prompt

- Do not create any code file.
- Do not create `Apps/info-service/` or `Apps/production-manager/` folders.
- Do not add new `.agents/` files — scaffolding updates happen inside execution chunks.
- Do not write execution prompts. That is P3.
- Do not introduce new decisions beyond what operator answered. If a field feels necessary but no operator answer covers it, flag it as a new Open Question in `COORDINATION.md` and stop that subsection.

## Finish

1. Save updated plan doc.
2. Update `COORDINATION.md`:
   - Move every resolved Open Question into the Decisions table with today's date.
   - Leave unresolved questions in place (flagged with note).
   - Chunk status P2 → `merged`. Append Run Log row (Commit column: `uncommitted`).
3. `WORKING.md` update.
4. Load `ops-change-summary` skill, show output.

**Do NOT run `git commit` or `git add`. Operator handles commits manually.**

## Definition of done

- `Docs/INFO-SERVICE-PLAN.md` has a complete `## Schemas` section covering items 1-8 above.
- `## Tech Stack` section has no `OPEN` items except explicitly deferred ones.
- Resolved questions migrated to Decisions. Unresolved ones flagged.
- Run log + chunk status updated.
- No git commit made by you.
