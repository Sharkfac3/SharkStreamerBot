# Prompt 10-03 — actions-lotat-tools-lotat

## Agent
Pi (manual copy/paste by operator)

## Preconditions
- Prompts 10-01 and 10-02 are complete.
- Read their handoffs plus findings 02, 05, 06, 07, and 08.

## Scope
- Create `Actions/LotAT/AGENTS.md`
- Create `Tools/LotAT/AGENTS.md`
- Read/migrate from `.agents/roles/streamerbot-dev/skills/lotat/_index.md`
- Read/migrate from `.agents/roles/lotat-tech/skills/engine/_index.md`
- Read/migrate from `.agents/roles/lotat-tech/skills/engine/commands.md`
- Read/migrate from `.agents/roles/lotat-tech/skills/engine/docs-map.md`
- Read/migrate from `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md`
- Read/migrate from `.agents/roles/lotat-tech/skills/engine/state-and-voting.md`
- Read/migrate from `.agents/roles/lotat-tech/skills/story-pipeline/_index.md`
- Read/migrate from `.agents/roles/lotat-tech/skills/story-pipeline/json-schema.md`
- Read `Actions/LotAT/README.md`
- Read `Tools/LotAT/README.md`
- Write `Projects/agent-reflow/handoffs/10-03-actions-lotat-tools-lotat.handoff.md`

## Out-of-scope
- No LotAT story-content/worldbuilding migration; that belongs to the Creative prompt.
- No stream-overlay visual rendering migration; that belongs to the overlay/app prompt.
- No role collapse or old source deletion.
- No git operations.

## Steps
1. Split runtime engine guidance into `Actions/LotAT/AGENTS.md` and story pipeline/schema/tooling guidance into `Tools/LotAT/AGENTS.md`.
2. Use frontmatter route IDs `actions-lotat` and `tools-lotat` with owners and secondary owners from manifest v2.
3. Preserve explicit handoff rules among `lotat-tech`, `streamerbot-dev`, `lotat-writer`, `app-dev`, and `ops`.
4. Convert old shorthand file names into valid links to `Actions/LotAT/*.cs`, `Tools/LotAT/*`, and related creative/app docs where they exist.
5. Link the relevant workflows: `change-summary`, `sync`, `validation`, and `canon-guardian` where story/canon review is mentioned.
6. Run the validator and record output.
7. Write the handoff.

## Validator / Acceptance
- Run: `python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-03-validator.failures.txt`
- Expected cleared deltas:
  - `folder-coverage` and `stub-presence` clear for `actions-lotat` and `tools-lotat`.
  - New docs add no broken links.
  - Baseline link failures in old LotAT skill source files are allowed to remain until cleanup.

## Handoff
Write `Projects/agent-reflow/handoffs/10-03-actions-lotat-tools-lotat.handoff.md`. Include any ambiguous ownership choices or links deferred because a later Creative/App prompt will create the target doc.
