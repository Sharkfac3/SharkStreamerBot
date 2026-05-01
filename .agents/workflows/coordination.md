---
id: coordination
type: workflow
description: Multi-agent conflict avoidance and contribution rules.
status: active
owner: ops
appliesTo:
  - task-start
  - task-finish
  - handoff
skills:
  - ops
terminal: false
path: .agents/workflows/coordination.md
sourceOfTruth: true
---
# Workflow: coordination

## Purpose

Prevent file conflicts between agents and keep the operator informed about active work.

## When to Run

At task start and finish. Re-run if the planned edit set changes materially.

## Branch vs Direct-to-Main

Use direct-to-main for small, focused, single-domain changes with understood scope. Use a worktree branch for multi-file, experimental, risky, or parallel work. Agents do not perform git operations unless explicitly told to.

## Procedure

1. Before editing, identify the files you plan to touch.
2. If another agent is known to be editing the same file or domain, stop and notify the operator before making changes.
3. Do not add tasks to the task queue yourself. Flag future work in the handoff or change summary for the operator to decide.
4. Include coordination status in the final [change-summary workflow](change-summary.md) output when files changed.

## Done Criteria

- [ ] Planned files identified before editing.
- [ ] No known conflicting active edits, or operator resolved the conflict.
- [ ] Future work flagged to operator rather than self-queued.

## Output / Handoff

The final handoff or change summary should note:

- Whether potential conflicts were considered.
- Whether any conflicts were found and how they were resolved.
- Any unresolved coordination items for operator review.

For branch work, use the standard [change-summary workflow](change-summary.md) plus a one-paragraph branch purpose statement before merge review.

## Related Routes

- Ops role: [ops role overview](../roles/ops/role.md)
- Change Summary: [change-summary.md](change-summary.md)
- Validation: [validation.md](validation.md)
- Sync: [sync.md](sync.md)

## Conflict Resolution

Actions conflicts require operator final call. Creative brand or lore conflicts require canon review through [canon-guardian.md](canon-guardian.md). Shared constants conflicts should stop and be flagged to the operator rather than auto-resolved.
