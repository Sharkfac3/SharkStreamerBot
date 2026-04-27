# Prompt 10-06 — actions-coverage-fills

## Agent
Pi (manual copy/paste by operator)

## Preconditions
- Prompts 10-01 through 10-05 are complete.
- Read prior handoffs and findings 02, 05, 06, and 08.

## Scope
- Create `Actions/Destroyer/AGENTS.md`
- Create `Actions/Intros/AGENTS.md`
- Create `Actions/Rest Focus Loop/AGENTS.md`
- Create `Actions/Temporary/AGENTS.md`
- Create `Actions/XJ Drivethrough/AGENTS.md`
- Read folder contents/READMEs for each target folder where present.
- Read `.agents/roles/streamerbot-dev/skills/core.md`
- Read `.agents/_shared/mixitup-api.md`
- Read `.agents/_shared/info-service-protocol.md`
- Read `Actions/SHARED-CONSTANTS.md`
- Read `Actions/HELPER-SNIPPETS.md`
- Write `Projects/agent-reflow/handoffs/10-06-actions-coverage-fills.handoff.md`

## Out-of-scope
- No app-side info-service or Mix It Up tool docs; this prompt may link/defer those docs only.
- No Twitch, Squad, Commanders, Voice, LotAT, or Overlay docs.
- No old source deletion.
- No git operations.

## Steps
1. Create local `AGENTS.md` files for the five previously uncovered action folders.
2. Use route IDs `actions-destroyer`, `actions-intros`, `actions-rest-focus-loop`, `actions-temporary`, and `actions-xj-drivethrough`.
3. For `Actions/Intros/`, include chain rules to `app-dev`, `brand-steward`, and `ops`, and note dependency on later `Apps/info-service/AGENTS.md` and `Tools/MixItUp/AGENTS.md` docs if not yet created.
4. For `Actions/Temporary/`, document whether it is standalone or operationally related to Rest Focus Loop without changing manifest coverage.
5. Normalize all links and avoid shorthand backtick paths.
6. Run the validator and record output.
7. Write the handoff.

## Validator / Acceptance
- Run: `python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-06-validator.failures.txt`
- Expected cleared deltas:
  - `folder-coverage` and `stub-presence` clear for five action coverage-fill routes.
  - New docs add no broken links.
  - Old core/shared link failures may remain until role/shared cleanup.

## Handoff
Write `Projects/agent-reflow/handoffs/10-06-actions-coverage-fills.handoff.md`. Flag any action folder whose ownership remains ambiguous for operator review.
