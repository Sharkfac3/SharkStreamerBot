# WORKING.md — Agent Task Coordination

> **All agents: check this file before starting any task. Update it when you begin and when you finish.**
>
> This is the shared signal board for all contributors — human, pi, claude, diffusion, or otherwise. It prevents two agents from editing the same files at the same time and gives the operator a live snapshot of project state.
>
> For contribution rules (when to commit direct vs. use a branch, merge review template): see `Docs/AGENT-WORKFLOW.md`.

---

## Active Work

| Agent | Task | Domain | Files Being Edited | Started |
|-------|------|--------|--------------------|---------|

*Empty table = no active agent work. If you see entries here, check the files listed before editing anything in that domain.*

---

## Task Queue

Ordered by priority. Agents pull from the top. Operator manages this list.

1. (queue is clear)

---

## Recently Completed

| Date | Agent | Task | Commit |
|------|-------|------|--------|
| 2026-03-30 | pi | Create LotAT human implementation prompt pack | uncommitted |
| 2026-03-30 | pi | Add human startup prompt for Streamer.bot LotAT implementation planning | uncommitted |
| 2026-03-30 | pi | Follow-up pass for LotAT v1 offering boundary docs | uncommitted |
| 2026-03-30 | pi | Clarify LotAT v1 offering boundary as out-of-scope | uncommitted |
| 2026-03-30 | pi | Clean up LotAT writer contract drift after validation audit | uncommitted |
| 2026-03-30 | pi | Lock LotAT validation ownership and fatal/warning contract docs | uncommitted |
| 2026-03-29 | pi | Align LotAT unattended fail-closed recovery docs | uncommitted |
| 2026-03-29 | pi | Remove LotAT supported_mechanics from story contract | uncommitted |
| 2026-03-29 | pi | Clarify LotAT v1 schema hard requirements vs editorial guidance | uncommitted |
| 2026-03-29 | pi | Lock LotAT v1 story loading and preflight contract docs | uncommitted |
| 2026-03-29 | pi | Lock LotAT v1 runtime storage contract docs | uncommitted |

---

## Conventions

### When you START a task
Add a row to **Active Work** with:
- Your agent name (`pi`, `claude`, `diffusion`, or a descriptive name)
- Short task description (one line)
- Primary domain (`Actions/`, `Creative/`, `Docs/`, `Tools/`, `.pi/skills/`)
- Files you expect to touch (comma-separated paths, or `TBD` if scouting)
- Today's date (`YYYY-MM-DD`)

### When you FINISH a task
1. Remove your row from **Active Work**
2. Add a row to **Recently Completed** with the commit hash
3. Keep **Recently Completed** to the last 10 entries — drop the oldest when you add a new one

### File conflict rule
If the file you need to edit appears in the **Active Work** table under another agent, **stop**. Do not proceed. Flag this to the operator before making changes.

### Parallel work is fine — with boundaries
Multiple agents can work simultaneously as long as they are in **different domains** and their file lists do not overlap. Example: pi editing `Actions/Squad/duck-main.cs` while claude edits `Creative/Brand/BRAND-VOICE.md` is safe. Two agents both touching `Actions/SHARED-CONSTANTS.md` is not.

### Task Queue is operator-managed
Agents do not add tasks to the queue unilaterally. If you identify work that should be done, note it in your change summary output and let the operator decide whether to queue it.

### Diffusion / art workflow entries
Art generation agents should register the character and asset type being generated (e.g., `Captain Stretch emote sheet`) under `Creative/Art/`. This prevents duplicate generation runs for the same asset.
