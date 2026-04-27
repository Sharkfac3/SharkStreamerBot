# Prompt 10-08 — tools-and-validator-coverage

## Agent
Pi (manual copy/paste by operator)

## Preconditions
- Prompts 10-01 through 10-07 are complete.
- Read prior handoffs and findings 02, 05, 06, 07, and 08.

## Scope
- Create `Tools/ArtPipeline/AGENTS.md`
- Create `Tools/ContentPipeline/AGENTS.md`
- Create `Tools/MixItUp/AGENTS.md`
- Create `Tools/StreamerBot/AGENTS.md`
- Add manifest coverage for `Tools/AgentTree/` and create `Tools/AgentTree/AGENTS.md` or declare it covered by `Tools/StreamerBot/AGENTS.md` if that is the better fit
- Read/migrate from `.agents/roles/art-director/skills/pipeline/_index.md`
- Read/migrate from `.agents/roles/content-repurposer/skills/pipeline/_index.md`
- Read/migrate from `.agents/roles/content-repurposer/skills/pipeline/phase-map.md`
- Read/migrate from `.agents/_shared/mixitup-api.md`
- Read `Tools/*/README.md` for in-scope tool folders
- Update `.agents/manifest.json` and `.agents/manifest.schema.json` only if needed for `Tools/AgentTree/` route/coveredBy support
- Write `Projects/agent-reflow/handoffs/10-08-tools-and-validator-coverage.handoff.md`

## Out-of-scope
- No Creative domain docs, except links to existing/future creative docs.
- No Streamer.bot action docs.
- No old source deletion.
- No git operations.

## Steps
1. Create local tool `AGENTS.md` docs with required frontmatter/sections for ArtPipeline, ContentPipeline, MixItUp, and StreamerBot.
2. Decide and implement explicit validator coverage for `Tools/AgentTree/` because it was introduced in Phase C and appears as a Phase 08 baseline gap.
3. If adding a new `tools-agent-tree` domain route, update `.agents/manifest.json` consistently: `skills`, `domains`, `co_locations`, and `quick_routing` only if appropriate.
4. Fold Mix It Up API reference into `Tools/MixItUp/AGENTS.md` or adjacent local doc; leave old shared source as pointer/stub if needed.
5. Normalize paths and commands in all new docs.
6. Run schema validation implicitly via the validator.
7. Run the validator and record output.
8. Write the handoff.

## Validator / Acceptance
- Run: `python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-08-validator.failures.txt`
- Expected cleared deltas:
  - `folder-coverage` and `stub-presence` clear for `tools-art-pipeline`, `tools-content-pipeline`, `tools-mix-it-up`, and `tools-streamer-bot`.
  - The baseline `Tools/AgentTree/: first-level domain folder has no manifest domain route` clears.
  - New tool docs and any manifest update produce no schema/naming failures.

## Handoff
Write `Projects/agent-reflow/handoffs/10-08-tools-and-validator-coverage.handoff.md`. Include the final disposition for `Tools/AgentTree/`.
