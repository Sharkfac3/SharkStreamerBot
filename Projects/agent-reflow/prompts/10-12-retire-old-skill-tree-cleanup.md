# Prompt 10-12 — retire-old-skill-tree-cleanup

## Agent
Pi (manual copy/paste by operator)

## Preconditions
- Prompts 10-01 through 10-11 are complete.
- Read all Phase E handoffs.
- New workflow files, local domain `AGENTS.md` files, role overviews, shared/root entrypoints, and Pi meta transition are in place.

## Scope
- Retire or delete obsolete `.agents/roles/*/skills/core.md` files after their content is confirmed migrated
- Retire or delete obsolete `.agents/roles/*/skills/**/_index.md` and leaf skill files after their content is confirmed migrated
- Retire or delete obsolete `.agents/roles/*/context/.gitkeep` files and migrated context notes
- Retire or declare `.agents/roles/_template/` so it no longer fails orphan/link checks
- Clean up or convert `.agents/_shared/info-service-protocol.md` and `.agents/_shared/mixitup-api.md` after app/tool migration
- Update `.agents/manifest.json` for any compatibility stubs, archive status, or removed co-locations
- Update any links from root/domain/workflow docs that still point to deprecated `_index.md`, `skills/core.md`, or `retired Pi skill mirror/*/SKILL.md` sources
- Write `Projects/agent-reflow/handoffs/10-12-retire-old-skill-tree-cleanup.handoff.md`

## Out-of-scope
- Do not delete `retired Pi skill mirror/`; Phase F cutover owns that.
- Do not change runtime code in `Actions/`, `Apps/`, or `Tools/`.
- Do not perform git operations.

## Steps
1. Use `python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-12-precleanup.failures.txt` to identify remaining old-tree failures.
2. For each old `.agents/roles/*/skills/*` source, verify its content is represented in a local domain doc or workflow. Then either delete it or replace it with a minimal compatibility stub only if a surviving doc still needs it.
3. Ensure any retained compatibility stubs have valid frontmatter with `status: deprecated` or `template` and no broken links.
4. Handle `.agents/roles/_template/` by either manifest-declaring it as `template` or moving/deleting it per target shape.
5. Remove/convert orphan `.gitkeep` files only if empty context folders are no longer needed.
6. Re-run the validator. Iterate until every remaining Phase 08 baseline failure is either cleared or documented as intentionally deferred to Phase F/99.
7. Write the handoff.

## Validator / Acceptance
- Run: `python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-12-validator.failures.txt`
- Expected cleared deltas:
  - All baseline `orphan` failures under `.agents/` clear.
  - Old-source `link-integrity` failures from `.agents/roles/*/skills/**`, migrated shared protocol/API files, and obsolete context notes clear.
  - Remaining failures, if any, must be only `retired Pi skill mirror/` cutover issues explicitly assigned to Phase F or audit-only recommendations assigned to prompt 99.

## Handoff
Write `Projects/agent-reflow/handoffs/10-12-retire-old-skill-tree-cleanup.handoff.md`. Include a final Phase E validator summary and a short list of items intentionally left for Phase F cutover.
