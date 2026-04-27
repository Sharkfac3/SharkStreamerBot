# Prompt 10-07 — apps-info-production-doc-folding

## Agent
Pi (manual copy/paste by operator)

## Preconditions
- Prompts 10-01 through 10-06 are complete.
- Read prior handoffs and findings 02, 04, 05, 06, 07, and 08.

## Scope
- Create `Apps/info-service/AGENTS.md`
- Create `Apps/production-manager/AGENTS.md`
- Move or split `Docs/INFO-SERVICE-PLAN.md` into app-local documentation under `Apps/info-service/` and/or `Apps/production-manager/`
- Read/migrate from `.agents/_shared/info-service-protocol.md`
- Read/migrate from `.agents/roles/app-dev/context/info-service.md`
- Read/migrate from `.agents/roles/app-dev/skills/core.md`
- Read `Apps/info-service/README.md`
- Read `Apps/production-manager/README.md`
- Update `.agents/manifest.json` only if co-location paths for the moved/split INFO-SERVICE plan need to be recorded
- Write `Projects/agent-reflow/handoffs/10-07-apps-info-production-doc-folding.handoff.md`

## Out-of-scope
- No stream-overlay docs.
- No action docs except links to `Actions/Intros/AGENTS.md` if present.
- No general role collapse.
- No git operations.

## Steps
1. Create `Apps/info-service/AGENTS.md` and `Apps/production-manager/AGENTS.md` with route IDs `apps-info-service` and `apps-production-manager`.
2. Fold `Docs/INFO-SERVICE-PLAN.md` content into app-local docs. Prefer one or more clearly named app-local markdown files; leave a short compatibility pointer at the old path if needed.
3. Move info-service protocol details out of `.agents/_shared/info-service-protocol.md` conceptually into the info-service local guide or adjacent app doc.
4. Normalize all app paths and links.
5. Include validation/build/test commands discovered from app READMEs/package files.
6. Run the validator and record output.
7. Write the handoff.

## Validator / Acceptance
- Run: `python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-07-validator.failures.txt`
- Expected cleared deltas:
  - `folder-coverage` and `stub-presence` clear for `apps-info-service` and `apps-production-manager`.
  - Link failures tied to `Docs/INFO-SERVICE-PLAN.md` and `.agents/_shared/info-service-protocol.md` should be reduced or eliminated if those files become pointers/stubs.
  - No new broken links from app-local docs.

## Handoff
Write `Projects/agent-reflow/handoffs/10-07-apps-info-production-doc-folding.handoff.md`. Include exact new app-local doc paths and whether the old Docs file remains as a pointer.
