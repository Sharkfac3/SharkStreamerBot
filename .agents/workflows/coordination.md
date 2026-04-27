---
id: coordination
type: workflow
description: WORKING.md coordination protocol and multi-agent conflict avoidance procedure.
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

Use this workflow to prevent file conflicts between agents and to keep the operator informed about active work. It consolidates the shared coordination procedure and the contribution rules for direct-vs-branch work in this repo.

## When to Run

Run coordination at task start and task finish. Re-run it whenever the planned edit set changes materially or when another active agent may overlap your files.

Use direct-to-main for small, focused, single-domain changes where scope is understood. Use a worktree branch for multi-file, experimental, risky, or parallel work, following operator instructions. Agents in this reflow project do not perform git operations unless explicitly told otherwise.

## Inputs

Required references:

| Reference | Use |
|---|---|
| [WORKING.md](../../WORKING.md) | Active work table, task queue, conflict registry, and recently completed log. |
| [AGENTS.md](../../AGENTS.md) | Root agent routing and project coordination pointers. |
| [.agents/ENTRY.md](../ENTRY.md) | Agent-tree entrypoint and shared context index. |

## Procedure

1. Before editing, read [WORKING.md](../../WORKING.md).
2. Check the Active Work table for overlapping paths or domains.
3. If another agent is editing the same file or a conflicting area, stop and notify the operator before making changes.
4. Add your row to Active Work with:
   - agent name,
   - short task description,
   - primary domain,
   - expected files or a scouting placeholder,
   - start date.
5. Keep the Files Being Edited column current enough that another agent can detect conflicts.
6. During work, do not add tasks to the queue yourself. Flag future work in the handoff or change summary for the operator to decide.
7. At task finish, remove your Active Work row.
8. Add a Recently Completed row with date, agent, task, and commit value. If no commit was made by the agent, use the project’s current uncommitted convention.
9. Keep the Recently Completed section trimmed to the last ten rows when adding a new row.
10. Include coordination status in the final [change-summary workflow](change-summary.md) output when files changed.

## Validation / Done Criteria

Coordination is complete when:

- [ ] Active work was checked before edits.
- [ ] No conflicting active file edits were present, or the operator resolved the conflict.
- [ ] Active Work was updated at start.
- [ ] Active Work was cleared at finish.
- [ ] Recently Completed was updated when the task completed.
- [ ] Future work was reported to the operator rather than added to the task queue.

## Output / Handoff

At minimum, the final handoff or change summary should note:

- Whether [WORKING.md](../../WORKING.md) was checked.
- Whether conflicts were found.
- Whether the Active Work row was cleared.
- Any unresolved coordination or operator-review items.

For branch work, use the standard [change-summary workflow](change-summary.md) plus a one-paragraph branch purpose statement before merge review.

## Related Routes

- Ops role: [ops role overview](../roles/ops/role.md)
- Change Summary workflow: [change-summary.md](change-summary.md)
- Validation workflow: [validation.md](validation.md)
- Sync workflow: [sync.md](sync.md)
- Root coordination board: [WORKING.md](../../WORKING.md)

## Role-Specific Notes

### Conflict resolution

If two branches or agents changed the same file, the later branch resolves conflicts. Actions conflicts require operator final call. Creative brand or lore conflicts require canon review through [canon-guardian.md](canon-guardian.md). Shared constants conflicts should stop and be flagged to the operator rather than auto-resolved.

### Direct-to-main versus branch

Direct-to-main is the default for small and clear changes. Branch work is for multi-file, experimental, risky, or likely-conflicting work. Follow the operator’s chosen mode for prompt-driven migration tasks.

## Failure Modes

- Active Work already lists a conflicting file: stop before editing.
- Scope expands beyond registered files: update the Active Work row before continuing.
- Task discovers future work: report it in the final summary; do not modify the operator-managed queue.
