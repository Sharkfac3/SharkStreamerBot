# Sync Workflow

## Source of Truth

This repo is the source-of-truth for all script text. Streamer.bot actions are updated by manual copy/paste.

## Sync Steps — Streamer.bot Actions

1. Update script file(s) in `Actions/<Feature Group>/...`
2. Provide a copy/paste mapping:
   - Full file path (including action folder when present)
   - Target Streamer.bot action/group
   - Any required UI variable/trigger changes
3. Paste updated scripts into Streamer.bot
4. Run smoke tests (see validation checklists below)

## Sync Steps — Tools / Creative

1. Update file(s) in `Tools/<Integration>/...` or `Creative/...`
2. Provide run instructions for `Tools/` scripts, or note `N/A` for creative scaffolding/docs

## Validation Checklists

### Streamer.bot Scripts (minimum after meaningful changes)

1. Syntax sanity check
2. Paste script into the intended Streamer.bot action
3. Trigger happy path once
4. Trigger at least one edge case (missing input, repeated trigger, cooldown path)
5. Verify logs and chat output for clarity/safety

### High-Impact Changes (startup/core, stateful mini-game)

All of the above, plus:
6. Test rollback/reset behavior

### Tools / Creative Scripts

1. CLI help/syntax sanity check
2. Happy-path API call against expected local service (when available)
3. Verify output file is written and readable
4. Verify at least one edge case (service offline, malformed response, empty result, pagination boundary)
5. For `Creative/` moves/docs: paste target is `N/A`

## Commit Notes

Include `synced-to-streamerbot: yes/no` in commit notes for `Actions/` changes.
