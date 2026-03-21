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
| — | — | — | — | — |

*Empty table = no active agent work. If you see entries here, check the files listed before editing anything in that domain.*

---

## Task Queue

Ordered by priority. Agents pull from the top. Operator manages this list.

1. (queue is clear)

---

## Recently Completed

| Date | Agent | Task | Commit |
|------|-------|------|--------|
| 2026-03-20 | pi | Add temporary focus timer bridge scripts for voice command and timer end | uncommitted |
| 2026-03-20 | pi | Implement rest/focus loop with voice command, commander commands, timers, and Mix It Up hooks | uncommitted |
| 2026-03-20 | pi | Refactor Duck to immediate threshold unlock | uncommitted |
| 2026-03-20 | pi | Add remaining humans role starters | uncommitted |
| 2026-03-20 | pi | Add humans role kickoff prompts and README | uncommitted |
| 2026-03-20 | pi | Add humans prompt starter for streamerbot-dev role | uncommitted |
| 2026-03-20 | pi | Remove stale completed routing stabilization note | uncommitted |
| 2026-03-20 | pi | Add routing sweet-spot sanity checks and stale reference scanning | uncommitted |
| 2026-03-20 | pi | Partially generate AGENTS.md quick routing from routing-manifest.json | uncommitted |
| 2026-03-20 | pi | Generate routing tables from routing-manifest.json and add sync script | uncommitted |

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
