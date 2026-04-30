---
id: sync
type: workflow
description: Repo-to-Streamer.bot paste and sync procedure for runtime scripts and generated docs.
status: active
owner: ops
appliesTo:
  - streamerbot-script-change
  - manual-paste
skills:
  - ops
  - streamerbot-dev
terminal: false
path: .agents/workflows/sync.md
sourceOfTruth: true
---
# Workflow: sync

## Purpose

Use this workflow when repository changes need to be copied into Streamer.bot or otherwise reconciled with local runtime state. The repository remains the source of truth; Streamer.bot actions are updated by manual operator copy and paste.

## When to Run

Run this workflow for:

- Any changed script under Actions.
- Any change that alters Streamer.bot action wiring, triggers, variables, timers, or action order.
- Any migration or docs update that changes the paste target or sync instructions for an action.

For Tools, Apps, Creative, and docs-only changes, use the relevant run instructions in the [change-summary workflow](change-summary.md) instead of Streamer.bot paste targets.

## Inputs

Prepare the following:

- Changed repository files.
- Target Streamer.bot group and action names.
- Any manual UI changes such as triggers, global variables, arguments, timers, or OBS source/action wiring.
- Smoke-test plan from the [validation workflow](validation.md).
- Constants references from [Actions/SHARED-CONSTANTS.md](../../Actions/SHARED-CONSTANTS.md) and helper patterns from [Actions/Helpers/AGENTS.md](../../Actions/Helpers/AGENTS.md) when scripts changed.

## Procedure

1. Confirm repository source files are final.
2. Build a paste mapping for every changed action script.
   - Include the full repository file path.
   - Include the Streamer.bot group and action target.
   - Include action folders or sub-actions when needed to disambiguate.
3. List required Streamer.bot UI changes.
   - New or changed triggers.
   - Global variables or persisted variables.
   - Action ordering.
   - Cooldowns, timers, or OBS/Mix It Up references.
4. Operator copies each script from the repository into the mapped Streamer.bot action.
5. Run smoke tests.
   - Trigger the happy path once.
   - Trigger at least one edge case such as missing input, repeated trigger, cooldown path, unavailable service, or malformed response.
   - Inspect logs and chat output for clarity and safety.
6. For high-impact startup, core, or stateful mini-game changes, test rollback or reset behavior.
7. Record sync status in the final [change-summary workflow](change-summary.md) output.

## Validation / Done Criteria

Sync is done when:

- [ ] Every changed action script has a paste target.
- [ ] Manual UI setup steps are listed or explicitly None.
- [ ] The operator has pasted the scripts or the handoff clearly says paste is still pending.
- [ ] Happy-path smoke test is complete, or a manual-test-pending note explains why not.
- [ ] At least one relevant edge case is tested for meaningful runtime changes.
- [ ] Logs and chat output look safe.
- [ ] High-impact changes include reset or rollback validation.

## Output / Handoff

Include this table in the final change summary when Actions files changed:

| File | Streamer.bot target | Sync status |
|---|---|---|
| path | group greater-than action | pending, pasted, or not applicable |

Also include:

- Manual setup steps.
- Smoke-test results.
- Whether the Streamer.bot runtime is now in sync with the repository.

## Related Routes

- Streamer.bot Dev role: [streamerbot-dev role overview](../roles/streamerbot-dev/role.md)
- Ops role: [ops role overview](../roles/ops/role.md)
- Change Summary workflow: [change-summary.md](change-summary.md)
- Validation workflow: [validation.md](validation.md)
- Actions shared constants: [Actions/SHARED-CONSTANTS.md](../../Actions/SHARED-CONSTANTS.md)
- Helper snippets: [Actions/Helpers/AGENTS.md](../../Actions/Helpers/AGENTS.md)

## Role-Specific Notes

### Tools and Creative files

Tools and Creative changes normally have no Streamer.bot paste target. Provide run instructions for tools and say not applicable for Creative scaffolding or docs.

### Apps files

App changes normally use local run/build validation instead of Streamer.bot paste. Include the affected app, command, and port in the change summary.

## Failure Modes

- Paste target is unknown: stop and ask the operator rather than guessing.
- A manual setup step is required but cannot be verified locally: record it as pending.
- Runtime smoke test fails: keep repository changes, report the failure, and do not claim Streamer.bot is synced.
