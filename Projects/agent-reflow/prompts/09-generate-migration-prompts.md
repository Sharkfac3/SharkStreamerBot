# Prompt 09 — Generate Migration Prompts

## Agent

Pi (manual copy/paste by operator).

## Purpose

Meta-prompt. Reads all Phase A findings + Phase B design + Phase C foundation, then generates the Phase E migration prompts (`prompts/10-NN-*.md`) sized to ground truth. Updates `PLAN.md` with new phase rows.

## Preconditions

- Phase A, B, C complete and operator-ratified
- Read findings/00–08
- Read PLAN.md

## Scope

Creates:
- One or more `prompts/10-NN-*.md` files, each sized for ~15-file batches (or fewer if natural concept boundaries are smaller)
- Updates `PLAN.md` Phase E rows

## Out-of-scope

- No actual migration work (the generated prompts do that)
- No edits to `.agents/`, `.pi/`, domain folders
- No git operations

## Steps

1. Read all findings + design + manifest + validator baseline.

2. Build migration backlog from validator failure list (prompt 08 baseline) + design deltas (target shape vs current).

3. Group migration tasks by:
   - **Co-location batches** — per-domain, e.g. `Actions/Squad/`, `Actions/Twitch *`, `Apps/info-service/`
   - **Skill rewrites** — collapse role.md + core.md, promote `_shared/`, etc.
   - **Workflow extraction** — canon-guardian, change-summary, sync, validation
   - **Coverage fills** — `Actions/Destroyer/`, `Actions/XJ Drivethrough/`, etc.
   - **Doc folding** — `Docs/INFO-SERVICE-PLAN.md` → `Apps/info-service/`, etc.
   - **Top-level doc updates** — root `CLAUDE.md`, `AGENTS.md`, `WORKING.md`, `ENTRY.md` (if it survives)

4. For each batch, draft a prompt file following the prompt template in `README.md`. Required sections per generated prompt:
   - Preconditions (which prior migration prompts must be done)
   - Scope (exact file list)
   - Out-of-scope (especially: no other batches, no git)
   - Steps (concrete edits)
   - Validator command (run validator, expect specific deltas to clear)
   - Handoff template

5. Update `PLAN.md` Phase E section with one row per generated prompt: prompt number, slug, status `drafted`, depends-on, notes.

6. Write `Projects/agent-reflow/findings/09-generation-summary.md` documenting: how many prompts generated, batching rationale, skipped concerns and why, expected order of execution.

## Validator / Acceptance

- All generated prompts conform to the prompt format in `README.md` (frontmatter / section headings)
- `PLAN.md` updated with one row per generated prompt
- Sum of generated prompts' validator-delta-clearing covers the full Phase 08 baseline failure list (no migration step missing)

## Handoff

Per template. Include the generated prompt index. Note any backlog items that could not be batched cleanly — flag for operator decision.
