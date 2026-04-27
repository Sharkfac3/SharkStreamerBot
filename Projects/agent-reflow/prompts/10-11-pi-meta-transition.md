# Prompt 10-11 — pi-meta-transition

## Agent
Pi (manual copy/paste by operator)

## Preconditions
- Prompts 10-01 through 10-10 are complete.
- Read `Projects/agent-reflow/handoffs/10-10-roles-shared-root-entrypoints.handoff.md` and findings 01, 04, 05, 06, 07, and 08.

## Scope
- Update `retired Pi skill mirror/meta/SKILL.md`
- Update `retired Pi skill mirror/meta-agents-navigate/SKILL.md`
- Update `retired Pi skill mirror/meta-agents-update/SKILL.md`
- Update `retired Pi skill mirror/README.md`
- Optionally create `.agents/meta/navigate.md` and `.agents/meta/update.md` if a non-Pi home is needed before cutover
- Update `.agents/manifest.json` if meta helper locations/disposition change
- Write `Projects/agent-reflow/handoffs/10-11-pi-meta-transition.handoff.md`

## Out-of-scope
- Do not delete `retired Pi skill mirror/`; Phase F cutover does that.
- Do not update every Pi wrapper; only meta wrappers and README drift needed for validator/cutover readiness.
- No domain docs or role collapse.
- No git operations.

## Steps
1. Resolve the two real-content Pi meta wrappers by moving or pointing their guidance to the target manifest/root-doc flow.
2. Normalize their links so they no longer produce baseline `link-integrity` failures.
3. Update `retired Pi skill mirror/README.md` only as transitional compatibility: it should point to manifest/root/local docs and avoid unsynced duplicated routing where possible.
4. If creating `.agents/meta/*`, give files valid frontmatter and manifest entries, or keep them out of `.agents/` if not manifest-declared.
5. Run the validator and record output.
6. Write the handoff.

## Validator / Acceptance
- Run: `python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-11-validator.failures.txt`
- Expected cleared deltas:
  - `stub-presence` frontmatter failures clear for the 3 meta skills or their manifest disposition changes.
  - Baseline `link-integrity` failures in `retired Pi skill mirror/meta*.md` clear.
  - `retired Pi skill mirror/README.md` drift clears or is explicitly deferred to Phase F with a validator note.

## Handoff
Write `Projects/agent-reflow/handoffs/10-11-pi-meta-transition.handoff.md`. Include exact cutover prerequisites for deleting `retired Pi skill mirror/`.
