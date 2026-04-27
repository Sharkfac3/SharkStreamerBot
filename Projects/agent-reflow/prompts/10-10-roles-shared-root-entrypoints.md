# Prompt 10-10 — roles-shared-root-entrypoints

## Agent
Pi (manual copy/paste by operator)

## Preconditions
- Prompts 10-01 through 10-09 are complete.
- Read all Phase E handoffs so far plus findings 03, 05, 06, 07, and 08.

## Scope
- Collapse/update `.agents/roles/streamerbot-dev/role.md`
- Collapse/update `.agents/roles/lotat-tech/role.md`
- Collapse/update `.agents/roles/lotat-writer/role.md`
- Collapse/update `.agents/roles/art-director/role.md`
- Collapse/update `.agents/roles/brand-steward/role.md`
- Collapse/update `.agents/roles/content-repurposer/role.md`
- Collapse/update `.agents/roles/app-dev/role.md`
- Collapse/update `.agents/roles/product-dev/role.md`
- Collapse/update `.agents/roles/ops/role.md`
- Update `.agents/_shared/project.md`
- Update `.agents/_shared/conventions.md`
- Update `.agents/_shared/coordination.md` into a pointer or manifest-declared shared/workflow doc
- Update `AGENTS.md`
- Update `.agents/ENTRY.md`
- Update `CLAUDE.md`
- Create `Docs/Architecture/AGENTS.md`
- Update `.agents/manifest.json` if shared docs, entrypoint status/locations, or Docs/Architecture routing need adjustment
- Write `Projects/agent-reflow/handoffs/10-10-roles-shared-root-entrypoints.handoff.md`

## Out-of-scope
- No `retired Pi skill mirror/` mirror deletion or wrapper cleanup.
- No domain `AGENTS.md` creation except fixing links to already-created docs.
- No old skill tree deletion; cleanup prompt follows.
- No git operations.

## Steps
1. For each role, collapse `role.md` and `skills/core.md` knowledge into one concise role overview following findings/06 required section order.
2. Keep domain-specific details out of role files; link to local `AGENTS.md` docs and workflows instead.
3. Create `Docs/Architecture/AGENTS.md` for the `docs-architecture` route. Keep it human/repo-architecture oriented and link to `Docs/Architecture/repo-structure.md` plus relevant workflows.
4. Add required frontmatter to all role files, shared docs, root `AGENTS.md`, `.agents/ENTRY.md`, and `CLAUDE.md` if manifest expects them as skills/co-locations.
5. Regenerate or hand-update manifest-backed routing summaries with visible generated markers where required by findings/05 and findings/08.
6. Make root `CLAUDE.md` a compatibility pointer only.
7. Normalize `.agents/_shared/*` content to repo-wide context only. If `.agents/_shared/coordination.md` is no longer canonical, turn it into a short pointer to `.agents/workflows/coordination.md` or declare/update it in manifest.
8. Run the validator and record output.
9. Write the handoff.

## Validator / Acceptance
- Run: `python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-10-validator.failures.txt`
- Expected cleared deltas:
  - `stub-presence` frontmatter failures clear for 9 role files, `AGENTS.md`, `.agents/ENTRY.md`, `.agents/_shared/project.md`, and `.agents/_shared/conventions.md`.
  - `folder-coverage` and `stub-presence` clear for `docs-architecture`.
  - `drift` failures clear for `AGENTS.md` and `.agents/ENTRY.md`; `retired Pi skill mirror/README.md` drift is expected to remain for prompt 10-11.
  - Root/shared link-integrity failures are reduced or eliminated.

## Handoff
Write `Projects/agent-reflow/handoffs/10-10-roles-shared-root-entrypoints.handoff.md`. Include a routing-surface diff summary and any `retired Pi skill mirror/README.md` drift left for the cutover prompt.
