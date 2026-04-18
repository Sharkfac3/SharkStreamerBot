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
| 2026-04-15 | pi | Add Mix It Up placeholder checklist for humans | uncommitted |
| 2026-04-15 | pi | Reconcile Mix It Up IDs against export file | uncommitted |
| 2026-04-15 | pi | Update Pedro Mix It Up unlock ID | uncommitted |
| 2026-04-15 | pi | Update Twitch Bits Mix It Up IDs | uncommitted |
| 2026-04-14 | pi | Fix Twitch subscription Mix It Up command IDs | uncommitted |
| 2026-04-08 | claude | Stream overlay ecosystem complete — broker, Phaser overlay, LotAT/Squad visual layers, Streamer.bot scripts, agent docs (`Apps/stream-overlay/`, `Actions/Overlay/`, `Actions/LotAT/`, `Actions/Squad/*/`) | uncommitted |
| 2026-04-03 | pi | Add Mix It Up commander redeem triggers | uncommitted |
| 2026-04-03 | pi | Convert Mix It Up launcher from .sh to .bat and remove venv usage | uncommitted |
| 2026-04-03 | pi | Add root shell launcher for Mix It Up command export tool | uncommitted |
| 2026-04-02 | pi | Fix LotAT compile errors for missing System.IO imports | uncommitted |
| 2026-04-02 | pi | Make LotAT scripts Streamer.bot-compiler compatible | uncommitted |

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
