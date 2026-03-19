# Agent Coordination

## WORKING.md — Check First

`WORKING.md` at repo root is the shared signal board. **Read it before starting any task.**

It contains:
- **Active Work** — agents currently editing files (conflict check)
- **Task Queue** — operator-managed priority list
- **Recently Completed** — last 10 completed tasks with commit hashes

## When You Start a Task

Add a row to **Active Work** in `WORKING.md`:
- Agent name (`pi`, `claude`, or descriptive name)
- Short task description (one line)
- Domain (`Actions/`, `Creative/`, `Docs/`, `Tools/`, `.agents/`)
- Files you expect to touch (or `TBD` if scouting)
- Today's date (`YYYY-MM-DD`)

## When You Finish a Task

1. Remove your row from **Active Work**
2. Add a row to **Recently Completed** with the commit hash
3. Keep **Recently Completed** to the last 10 entries

## File Conflict Rule

If the file you need to edit appears in **Active Work** under another agent: **stop**. Flag to operator before making changes.

## Parallel Work

Multiple agents can work simultaneously in **different domains** with **non-overlapping file lists**.

Safe: pi editing `Actions/Squad/duck-main.cs` while claude edits `Creative/Brand/BRAND-VOICE.md`
Not safe: two agents both touching `Actions/SHARED-CONSTANTS.md`

## Conflict Resolution (Branch Work)

When two branches have modified the same file:
1. The **later branch** resolves conflicts
2. `Actions/` conflicts → operator makes the final call
3. `Creative/` brand/lore conflicts → `brand-steward` canon-guardian review required
4. `Actions/SHARED-CONSTANTS.md` conflicts → stop and flag to operator; do not auto-resolve

## Task Queue

Agents do not add tasks to the queue unilaterally. Flag identified work in your change summary and let the operator decide.
