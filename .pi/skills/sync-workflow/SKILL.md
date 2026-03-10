---
name: sync-workflow
description: Repo-to-Streamer.bot sync workflow and manual validation checklists. Load when preparing to sync changed scripts into Streamer.bot or when running validation passes.
---

# Sync Workflow

## Source of Truth

This repo is the source-of-truth for all script text. Streamer.bot actions are updated by manual copy/paste.

## Sync Steps

### For Streamer.bot Actions

1. Update script file(s) in `Actions/<Feature Group>/<Subfeature>/...`.
2. Provide a copy/paste mapping:
   - Full file path (including subfeature folder).
   - Target Streamer.bot action/group.
   - Any required UI variable/trigger changes.
3. Paste updated scripts into Streamer.bot.
4. Run smoke tests.

### For BuildTools

1. Update file(s) in `BuildTools/<Integration>/...`.
2. Provide run instructions (command + expected output file path).

## Validation Checklists

### Streamer.bot Scripts (minimum after meaningful changes)

1. Syntax sanity check in script.
2. Paste script into the intended Streamer.bot action.
3. Trigger happy path once.
4. Trigger at least one edge case (missing input, repeated trigger, cooldown path, etc.).
5. Verify logs and chat output for clarity/safety.

### High-Impact Changes (startup/core, stateful mini-game)

All of the above, plus:
6. Test rollback/reset behavior.

### BuildTools Scripts

1. CLI help/syntax sanity check.
2. Happy-path API call against expected local service (when available).
3. Verify output file is written and readable.
4. Verify at least one edge case (service offline, malformed response, empty result set, pagination boundary).

## Commit Notes

Recommended commit note style includes: `synced-to-streamerbot: yes/no`.
