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

---

## Task Queue

Ordered by priority. Agents pull from the top. Operator manages this list.

1. (queue is clear)

---

## Recently Completed

| Date | Agent | Task | Commit |
|------|-------|------|--------|
| 2026-04-25 | pi | C11 — info-service docs + app-dev scaffolding sweep | uncommitted |
| 2026-04-24 | claude | Drip-meta: drafted C11 prompt `19-docs-scaffolding-sweep.md`; COORDINATION C11 → prompt-ready | uncommitted |
| 2026-04-24 | claude | C9 — MixItUp Custom Intro command spec (`.agents/_shared/mixitup-api.md`) | uncommitted |
| 2026-04-24 | claude | C8 — SB first-chat intro script (`Actions/Intros/first-chat-intro.cs`) | uncommitted |
| 2026-04-24 | claude | C10 — pending-intros collection + redeem-capture SB script | uncommitted |
| 2026-04-24 | claude | Draft C10 prompt (drip-meta) — `18-redeem-capture.md`, O14 open question | uncommitted |
| 2026-04-24 | claude | C7 — production-manager: user-intros page (table, modal form, CRUD, nav) | uncommitted |
| 2026-04-24 | claude | C6 — production-manager skeleton (React+Vite+Tailwind, HealthPage) | uncommitted |
| 2026-04-24 | claude | C5 — REST routes (read/write plugins, server registry, index wiring) | uncommitted |
| 2026-04-24 | claude | C4 — user-intros collection (schema + instance + index wiring) | uncommitted |

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
