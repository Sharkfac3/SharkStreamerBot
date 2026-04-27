# Prompt 10-05 — actions-twitch

## Agent
Pi (manual copy/paste by operator)

## Preconditions
- Prompts 10-01 through 10-04 are complete.
- Read prior handoffs and findings 02, 05, 06, and 08.

## Scope
- Create `Actions/Twitch Bits Integrations/AGENTS.md`
- Create `Actions/Twitch Channel Points/AGENTS.md`
- Create `Actions/Twitch Core Integrations/AGENTS.md`
- Create `Actions/Twitch Hype Train/AGENTS.md`
- Read/migrate from `.agents/roles/streamerbot-dev/skills/twitch/_index.md`
- Read/migrate from `.agents/roles/streamerbot-dev/skills/twitch/bits.md`
- Read/migrate from `.agents/roles/streamerbot-dev/skills/twitch/channel-points.md`
- Read/migrate from `.agents/roles/streamerbot-dev/skills/twitch/core-events.md`
- Read/migrate from `.agents/roles/streamerbot-dev/skills/twitch/hype-train.md`
- Read `Actions/Twitch Bits Integrations/README.md`
- Read `Actions/Twitch Channel Points/README.md`
- Read `Actions/Twitch Core Integrations/README.md`
- Read `Actions/Twitch Hype Train/README.md`
- Write `Projects/agent-reflow/handoffs/10-05-actions-twitch.handoff.md`

## Out-of-scope
- No non-Twitch action folders.
- No brand voice rewrite beyond noting brand-steward handoff triggers.
- No old source deletion.
- No git operations.

## Steps
1. Create one local `AGENTS.md` per Twitch folder, using route IDs from manifest v2.
2. Preserve the ratified decision: one `streamerbot-dev` owner family with folder-local docs; do not recreate flat Twitch wrappers.
3. Include secondary `brand-steward` handoff for public event/redemption text.
4. Convert script references to valid paths under each Twitch folder.
5. Link workflows `change-summary`, `sync`, and `validation`.
6. Run the validator and record output.
7. Write the handoff.

## Validator / Acceptance
- Run: `python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-05-validator.failures.txt`
- Expected cleared deltas:
  - `folder-coverage` and `stub-presence` clear for all four Twitch routes.
  - New docs add no broken links.
  - Old twitch skill source link/orphan failures may remain until cleanup.

## Handoff
Write `Projects/agent-reflow/handoffs/10-05-actions-twitch.handoff.md`. Include the generated Twitch prompt index and any public-copy handoff notes.
