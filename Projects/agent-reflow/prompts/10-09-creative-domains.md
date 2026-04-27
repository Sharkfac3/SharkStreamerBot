# Prompt 10-09 — creative-domains

## Agent
Pi (manual copy/paste by operator)

## Preconditions
- Prompts 10-01 through 10-08 are complete.
- Read prior handoffs and findings 02, 05, 06, and 08.

## Scope
- Create `Creative/Art/AGENTS.md`
- Create `Creative/Brand/AGENTS.md`
- Create `Creative/Marketing/AGENTS.md`
- Create `Creative/WorldBuilding/AGENTS.md`
- Read/migrate from `.agents/roles/art-director/skills/characters/_index.md`
- Read/migrate from `.agents/roles/art-director/skills/characters/captain-stretch.md`
- Read/migrate from `.agents/roles/art-director/skills/characters/the-director.md`
- Read/migrate from `.agents/roles/art-director/skills/characters/water-wizard.md`
- Read/migrate from `.agents/roles/art-director/skills/stream-style/_index.md`
- Read/migrate from `.agents/roles/brand-steward/skills/community-growth/_index.md`
- Read/migrate from `.agents/roles/brand-steward/skills/content-strategy/_index.md`
- Read/migrate from `.agents/roles/brand-steward/skills/voice/_index.md`
- Read/migrate from `.agents/roles/content-repurposer/skills/clip-strategy/_index.md`
- Read/migrate from `.agents/roles/content-repurposer/skills/platforms/_index.md`
- Read/migrate from `.agents/roles/lotat-writer/skills/adventures/`, `franchises/`, and `universe/`
- Read `Creative/*/README.md` and canonical brand/worldbuilding docs as needed
- Write `Projects/agent-reflow/handoffs/10-09-creative-domains.handoff.md`

## Out-of-scope
- No tool/app/action docs.
- No role collapse beyond using source content.
- No old source deletion.
- No git operations.

## Steps
1. Create four Creative local `AGENTS.md` docs with route IDs `creative-art`, `creative-brand`, `creative-marketing`, and `creative-worldbuilding`.
2. Preserve owner/secondary-owner boundaries from manifest and findings/05.
3. Link the `canon-guardian` workflow wherever canon review is triggered.
4. Move brand voice/community/content strategy guidance into `Creative/Brand/AGENTS.md` and `Creative/Marketing/AGENTS.md` according to findings/06 mapping.
5. Move LotAT writer adventure/franchise/universe routing into `Creative/WorldBuilding/AGENTS.md`.
6. Normalize all links to source docs under `Creative/`.
7. Run the validator and record output.
8. Write the handoff.

## Validator / Acceptance
- Run: `python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-09-validator.failures.txt`
- Expected cleared deltas:
  - `folder-coverage` and `stub-presence` clear for all four Creative routes.
  - New Creative docs add no broken links.
  - Old creative skill source link/orphan failures may remain until cleanup.

## Handoff
Write `Projects/agent-reflow/handoffs/10-09-creative-domains.handoff.md`. Include any canon-sensitive content that needs operator review.
