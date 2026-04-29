---
id: change-summary
type: workflow
description: Terminal handoff format for changed files, paste targets, setup steps, and validation output.
status: active
owner: ops
appliesTo:
  - code-change
  - doc-change
  - tooling-change
skills:
  - ops
terminal: true
path: .agents/workflows/change-summary.md
sourceOfTruth: true
---
# Workflow: change-summary

## Purpose

Use this terminal workflow after any code, tooling, app, agent-doc, or domain-doc change. It gives the operator a compact, repeatable summary of what changed, where any manual paste or setup work belongs, and which validation checks ran.

## When to Run

Run at the end of a task when any tracked project file changed. It is especially required after:

- Streamer.bot action script edits, including updates to the matching source-of-truth action contract in the nearest `AGENTS.md`.
- App or tool changes.
- Agent-tree migration edits.
- Docs that affect setup, runtime behavior, workflow, routing, canon, or validation.

For purely investigative tasks with no file changes, provide a findings-only summary instead.

## Inputs

Collect these before writing the final response:

- List of created, modified, or deleted files.
- Operator-facing behavior change or documentation effect.
- Paste targets for Streamer.bot action scripts, if any.
- Manual setup steps such as variables, triggers, action ordering, ports, or local services.
- Validation commands and results.
- Source-of-truth action contract status for Streamer.bot script changes: contract updated or reviewed unchanged, stamp refreshed, and validator result.
- Sync status when Streamer.bot actions changed; see the [sync workflow](sync.md).
- Coordination status for the shared task board; see the [coordination workflow](coordination.md).

## Procedure

1. Group changed files by domain.
2. Explain the practical outcome in operator-friendly language.
3. Choose the correct target section:
   - Streamer.bot script changes need paste targets.
   - Tools changes need run instructions and expected output locations.
   - Apps changes need start/build/type-check instructions and port notes.
   - Creative or docs-only changes usually use not applicable for paste targets.
4. List manual setup steps, or state None.
5. Report validation checks exactly as run. If a validator is expected to fail during a migration, include the command, exit code, summary, and expected failure themes.
6. Include known follow-ups, risks, or operator decisions without adding tasks to the queue yourself.
7. Keep the summary concise and avoid hiding failed validation.

## Validation / Done Criteria

A change summary is complete when it includes:

- [ ] Changed files with clear paths.
- [ ] Behavioral summary in plain language.
- [ ] Paste targets, run instructions, or not applicable status.
- [ ] Manual setup steps or None.
- [ ] Validation checklist with command output or explicit not-run rationale.
- [ ] Any known risks, expected failures, or follow-up prompts.

## Output / Handoff

Use this response shape:

### Changed files
- path — short description.

### Behavioral summary
One or two concise paragraphs.

### Streamer.bot paste targets
| File | Action/Group |
|---|---|
| path | target action or not applicable |

### Manual setup steps
None, or numbered steps.

### Validation checklist
- [x] Command or check — result.
- [ ] Manual check still needed — reason.

### Notes / follow-ups
Optional. Include only material items.

For worktree branch merges, extend this with a one-paragraph branch purpose statement and a merge-readiness checklist before requesting operator review.

## Related Routes

- Ops role: [ops role overview](../roles/ops/role.md)
- Sync workflow: [sync.md](sync.md)
- Validation workflow: [validation.md](validation.md)
- Coordination workflow: [coordination.md](coordination.md)

## Role-Specific Notes

### Streamer.bot actions

Include a paste mapping for each changed action script and note whether Streamer.bot has actually been updated. If the operator must copy manually, say so clearly.

Also state the matching local action contract status for every changed script: which `AGENTS.md` contract is the source of truth, whether it changed, whether the script stamp was refreshed, and the result of `python3 Tools/StreamerBot/Validation/action_contracts.py --changed` or the targeted equivalent.

### Tools changes

Provide the command to run, the expected output path if one exists, and any local service dependencies.

### Apps changes

State which app changed, the relevant start command and port, and whether type-check/build/start validation passed.
