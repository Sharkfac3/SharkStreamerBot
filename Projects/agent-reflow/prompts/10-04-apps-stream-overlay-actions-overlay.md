# Prompt 10-04 — apps-stream-overlay-actions-overlay

## Agent
Pi (manual copy/paste by operator)

## Preconditions
- Prompts 10-01 through 10-03 are complete.
- Read prior handoffs and findings 02, 05, 06, and 08.

## Scope
- Create `Apps/stream-overlay/AGENTS.md`
- Create `Actions/Overlay/AGENTS.md`
- Read/migrate from `.agents/roles/app-dev/skills/stream-interactions/_index.md`
- Read/migrate from `.agents/roles/app-dev/skills/stream-interactions/asset-system.md`
- Read/migrate from `.agents/roles/app-dev/skills/stream-interactions/broker.md`
- Read/migrate from `.agents/roles/app-dev/skills/stream-interactions/lotat-rendering.md`
- Read/migrate from `.agents/roles/app-dev/skills/stream-interactions/overlay.md`
- Read/migrate from `.agents/roles/app-dev/skills/stream-interactions/protocol.md`
- Read/migrate from `.agents/roles/app-dev/skills/stream-interactions/squad-rendering.md`
- Read/migrate from `.agents/roles/streamerbot-dev/skills/overlay-integration.md`
- Read `Apps/stream-overlay/README.md`
- Read `Actions/Overlay/README.md`
- Write `Projects/agent-reflow/handoffs/10-04-apps-stream-overlay-actions-overlay.handoff.md`

## Out-of-scope
- No info-service or production-manager migration.
- No Squad/Commanders/Twitch action docs except links to already-created docs.
- No old source deletion.
- No git operations.

## Steps
1. Put app runtime, broker, protocol, asset, and rendering guidance in `Apps/stream-overlay/AGENTS.md`.
2. Put Streamer.bot publisher/paste target guidance in `Actions/Overlay/AGENTS.md`.
3. Use frontmatter route IDs `apps-stream-overlay` and `actions-overlay`.
4. Normalize paths to real repo paths under `Apps/stream-overlay/` and `Actions/Overlay/`.
5. Link `Actions/Squad/AGENTS.md` and `Actions/LotAT/AGENTS.md` if created by prior prompts; otherwise note deferred link in the handoff.
6. Run the validator and record output.
7. Write the handoff.

## Validator / Acceptance
- Run: `python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-04-validator.failures.txt`
- Expected cleared deltas:
  - `folder-coverage` and `stub-presence` clear for `apps-stream-overlay` and `actions-overlay`.
  - No broken links from the two new docs.
  - Existing old app-dev stream-interactions link failures may remain until cleanup.

## Handoff
Write `Projects/agent-reflow/handoffs/10-04-apps-stream-overlay-actions-overlay.handoff.md`. Include app build/test commands found and any protocol details moved.
