# Prompt 10-02 — actions-commanders-squad-voice

## Agent
Pi (manual copy/paste by operator)

## Preconditions
- Prompt 10-01 is complete.
- Read `Projects/agent-reflow/handoffs/10-01-workflows-foundation.handoff.md`.
- Read findings 02, 05, 06, and 08.

## Scope
- Create `Actions/Commanders/AGENTS.md`
- Create `Actions/Squad/AGENTS.md`
- Create `Actions/Voice Commands/AGENTS.md`
- Read/migrate from `.agents/roles/streamerbot-dev/skills/commanders/_index.md`
- Read/migrate from `.agents/roles/streamerbot-dev/skills/commanders/captain-stretch.md`
- Read/migrate from `.agents/roles/streamerbot-dev/skills/commanders/the-director.md`
- Read/migrate from `.agents/roles/streamerbot-dev/skills/commanders/water-wizard.md`
- Read/migrate from `.agents/roles/streamerbot-dev/skills/squad/_index.md`
- Read/migrate from `.agents/roles/streamerbot-dev/skills/squad/clone.md`
- Read/migrate from `.agents/roles/streamerbot-dev/skills/squad/duck.md`
- Read/migrate from `.agents/roles/streamerbot-dev/skills/squad/pedro.md`
- Read/migrate from `.agents/roles/streamerbot-dev/skills/squad/toothless.md`
- Read/migrate from `.agents/roles/streamerbot-dev/skills/voice-commands/_index.md`
- Read/migrate from `.agents/roles/streamerbot-dev/context/patterns.md`
- Write `Projects/agent-reflow/handoffs/10-02-actions-commanders-squad-voice.handoff.md`

## Out-of-scope
- No Twitch, LotAT, Overlay, app, tool, or creative migrations.
- No deletion of old `.agents/roles/streamerbot-dev/skills/` sources yet unless the validator requires a temporary frontmatter stub; cleanup is later.
- No git operations.

## Steps
1. Read the local `README.md` files under the three target folders, if present.
2. Create each local `AGENTS.md` using the required domain-doc section order from findings/06.
3. Use frontmatter `type: domain-route`, route IDs `actions-commanders`, `actions-squad`, and `actions-voice-commands`, correct owners, secondary owners, workflows, and `status: active`.
4. Convert relative script-name mentions from old skills into valid paths under the target folder (for example `Actions/Squad/...`).
5. Include Streamer.bot paste/sync expectations and links to `.agents/workflows/change-summary.md`, `.agents/workflows/sync.md`, and `.agents/workflows/validation.md`.
6. Include brand/app handoff triggers from findings/02 where applicable.
7. Run the validator and record output.
8. Write the handoff.

## Validator / Acceptance
- Run: `python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-02-validator.failures.txt`
- Expected cleared deltas:
  - `folder-coverage` and `stub-presence` clear for `actions-commanders`, `actions-squad`, and `actions-voice-commands`.
  - No new link failures in the three new `AGENTS.md` files.
  - Old-source link/orphan failures for commander/squad/voice skill files may remain until cleanup.

## Handoff
Write `Projects/agent-reflow/handoffs/10-02-actions-commanders-squad-voice.handoff.md`. Include paste targets and any old skill content that was intentionally not migrated.
