# SharkStreamerBot

## Start Here

Read `.agents/ENTRY.md` before any task. It identifies your role, explains how to navigate the skill tree, and points to shared project context.

## Before Starting Any Task

Check `WORKING.md` at repo root for active agent work and file conflicts.

## After Any Code Change

Load `ops/change-summary` from `.agents/roles/ops/skills/change-summary/_index.md` to produce paste targets and validation checklist for the operator.

## Key Facts

- Streamer.bot scripts are **not auto-deployed** — all `Actions/` changes require manual copy/paste into Streamer.bot
- Commit directly to `main` for small focused changes; use a worktree branch for multi-file or parallel work
- Cast is fixed — no new named characters without operator approval
- `Actions/SHARED-CONSTANTS.md` is the canonical source for all global variable, OBS source, and timer names — never hardcode these
