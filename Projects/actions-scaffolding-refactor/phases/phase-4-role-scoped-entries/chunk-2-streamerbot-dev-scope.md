---
id: phase-4-chunk-2-streamerbot-dev-scope
type: refactor-prompt
phase: 4
chunk: 2
status: ready
---

# Phase 4 / Chunk 2 — Add streamerbot-dev Actions Scope

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You create and update agent scaffolding markdown files to improve navigability. You do not touch `.cs` runtime scripts and do not alter runtime behavior.

## Repository Context

`streamerbot-dev` is the primary role for all Streamer.bot C# action work. It has the widest scope of any role in `Actions/` — it owns or co-owns every subfolder. The goal is not to narrow its scope (it needs everything) but to give it a workflow-ordered reading list so it loads the right files for the task at hand rather than everything upfront.

`.agents/roles/streamerbot-dev/role.md` currently has a "Common Routes" section and "Living Context" section that partially do this. This chunk extends that with a structured `actions-scope.md`.

## Prerequisite

Phase 4 Chunk 1 must show `done`. Chunk 2 must show `pending`.

## What This Chunk Does

Create `.agents/roles/streamerbot-dev/actions-scope.md` — a task-oriented reading guide organized by what the agent is doing (navigating, writing a script, validating, reviewing a contract).

Update `.agents/roles/streamerbot-dev/role.md` to reference `actions-scope.md`.

**Files touched:**
- `.agents/roles/streamerbot-dev/actions-scope.md` — created
- `.agents/roles/streamerbot-dev/role.md` — updated (add reference)

## What This Chunk Does NOT Do

- Narrow streamerbot-dev's scope — it still owns everything it currently owns
- Remove anything from role.md — only add a reference
- Touch any Actions files

## Files to Read Before Starting

1. `.agents/roles/streamerbot-dev/role.md` — read carefully, understand current structure
2. `Actions/AGENTS.md` — lean router (post Phase 2)
3. `Actions/RULES.md` — domain rules
4. `Actions/OWNERSHIP.md` — ownership model
5. `Actions/CONTRACT-SCHEMA.md` — contract schema
6. `Actions/SHARED-CONSTANTS.md` — index (post Phase 3)
7. `Projects/actions-scaffolding-refactor/progress.md` — confirm chunk 1 done, chunk 2 pending

## Step-by-Step Instructions

1. Read all files above.

2. Create `.agents/roles/streamerbot-dev/actions-scope.md` organized by task type:

```markdown
---
id: streamerbot-dev-actions-scope
type: scope
description: Task-ordered reading guide for streamerbot-dev working in Actions/.
owner: streamerbot-dev
parent: role.md
---

# streamerbot-dev — Actions Scope

Load files in the order that matches your task. You do not need to load everything upfront.

## Starting Any Actions Task

Always load first:
1. [Actions/AGENTS.md](../../../Actions/AGENTS.md) — domain entry and folder routing
2. The local `AGENTS.md` for the specific folder you are editing

## Writing or Editing a Script

After the above:
3. The relevant constants file(s) from [Actions/SHARED-CONSTANTS.md](../../../Actions/SHARED-CONSTANTS.md) index — load only the domain you need
4. [Actions/Helpers/AGENTS.md](../../../Actions/Helpers/AGENTS.md) — check for reusable C# patterns
5. [Actions/Helpers/triggers/README.md](../../../Actions/Helpers/triggers/README.md) — canonical trigger args

## Validating or Updating a Contract

After the script step:
6. The local `contracts.md` for the folder you are in
7. [Actions/CONTRACT-SCHEMA.md](../../../Actions/CONTRACT-SCHEMA.md) — contract format spec
8. Run: `python3 Tools/StreamerBot/Validation/action_contracts.py --changed`

## Domain Rules or Ownership Questions

- [Actions/RULES.md](../../../Actions/RULES.md) — all 14 domain rules
- [Actions/OWNERSHIP.md](../../../Actions/OWNERSHIP.md) — role matrix and chain-to rules

## Folder Map (load local AGENTS.md for the folder you need)

| Folder | AGENTS.md | contracts.md |
|---|---|---|
| Squad | [Squad/AGENTS.md](../../../Actions/Squad/AGENTS.md) | [Squad/contracts.md](../../../Actions/Squad/contracts.md) |
| LotAT | [LotAT/AGENTS.md](../../../Actions/LotAT/AGENTS.md) | [LotAT/contracts.md](../../../Actions/LotAT/contracts.md) |
| Commanders | [Commanders/AGENTS.md](../../../Actions/Commanders/AGENTS.md) | [Commanders/contracts.md](../../../Actions/Commanders/contracts.md) |
| Overlay | [Overlay/AGENTS.md](../../../Actions/Overlay/AGENTS.md) | [Overlay/contracts.md](../../../Actions/Overlay/contracts.md) |
| Intros | [Intros/AGENTS.md](../../../Actions/Intros/AGENTS.md) | [Intros/contracts.md](../../../Actions/Intros/contracts.md) |
| (other folders) | local AGENTS.md | local contracts.md if present |
```

3. Edit `.agents/roles/streamerbot-dev/role.md`. In the `## Common Routes` section (or `## Living Context` section), add:

```markdown
For a task-ordered reading guide, see [actions-scope.md](actions-scope.md).
```

   Match the existing style of the role file. Do not restructure it.

4. Verify both files.

5. Update `progress.md`: Phase 4 chunk 2 → `done`, update "Last updated".

## Output Requirements

- `.agents/roles/streamerbot-dev/actions-scope.md` exists with task-ordered reading guide
- Guide covers: starting a task, writing a script, validating a contract, rules/ownership questions
- Folder map table lists all major folders with AGENTS.md and contracts.md links
- `streamerbot-dev/role.md` references `actions-scope.md`
- `progress.md` chunk 2 shows `done`

## Validation Checklist

- [ ] `actions-scope.md` created
- [ ] Organized by task type (start, write, validate, rules)
- [ ] Folder map table present and accurate
- [ ] `role.md` references `actions-scope.md`
- [ ] `role.md` not otherwise changed
- [ ] `progress.md` updated
