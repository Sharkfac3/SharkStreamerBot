# Prompt 10-01 — workflows-foundation

## Agent
Pi (manual copy/paste by operator)

## Preconditions
- Read Projects/agent-reflow/handoffs/09-generation-summary.handoff.md if present; otherwise read Projects/agent-reflow/findings/09-generation-summary.md.
- Read Projects/agent-reflow/findings/05-target-shape.md, 06-naming-convention.md, 07-manifest-v2.md, and 08-validator.md.
- No other Phase E prompt has run yet.

## Scope
- Create `.agents/workflows/canon-guardian.md`
- Create `.agents/workflows/change-summary.md`
- Create `.agents/workflows/sync.md`
- Create `.agents/workflows/validation.md`
- Create `.agents/workflows/coordination.md`
- Read/migrate from `.agents/roles/brand-steward/skills/canon-guardian/_index.md`
- Read/migrate from `.agents/roles/lotat-writer/skills/canon-guardian/_index.md`
- Read/migrate from `.agents/roles/ops/skills/change-summary/_index.md`
- Read/migrate from `.agents/roles/ops/skills/sync/_index.md`
- Read/migrate from `.agents/roles/ops/skills/validation/_index.md`
- Read/migrate from `.agents/_shared/coordination.md`
- Read/migrate from `Docs/AGENT-WORKFLOW.md`
- Write `Projects/agent-reflow/handoffs/10-01-workflows-foundation.handoff.md`

## Out-of-scope
- No edits to domain folders.
- No role collapse yet; old workflow source files may remain until cleanup prompts.
- No `retired Pi skill mirror/` edits.
- No git operations.

## Steps
1. Read all source files listed in Scope.
2. Create `.agents/workflows/` if missing.
3. For each workflow, write YAML frontmatter with `id`, `type: workflow`, `description`, `status: active`, and any required workflow fields from prompt 06.
4. Use required workflow section order from findings/06.
5. Merge brand and LotAT canon procedures into one `canon-guardian` workflow with role-specific notes.
6. Convert change-summary, sync, validation, and coordination procedures into reusable workflows; use Markdown links for navigable file references and avoid ambiguous backtick paths.
7. Do not delete old source files in this prompt.
8. Run the validator and record output.
9. Write the handoff.

## Validator / Acceptance
- Run: `python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-01-validator.failures.txt`
- Expected cleared deltas from the Phase 08 baseline:
  - `folder-coverage` missing workflow location/co-location failures for all 5 workflows clear.
  - `stub-presence` missing workflow entry failures for all 5 workflows clear.
  - New workflow files produce no new `link-integrity` failures.
- Remaining failures for domain `AGENTS.md`, roles, root docs, old source files, and drift are expected.

## Handoff
Write `Projects/agent-reflow/handoffs/10-01-workflows-foundation.handoff.md` per the handoff template in `Projects/agent-reflow/README.md`. Include the validator summary and list any source workflow details that could not be cleanly merged.
