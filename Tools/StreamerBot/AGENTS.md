---
id: tools-streamer-bot
type: domain-route
description: Local Streamer.bot support tooling for sync helpers, validators, export/import utilities, and templates.
owner: ops
secondaryOwners:
  - streamerbot-dev
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Tools/StreamerBot — Agent Guide

## Purpose

[Tools/StreamerBot/](./) contains local support tooling around Streamer.bot operations. It is for sync helpers, validation scripts, export/import helpers, reusable operator templates, and related local utilities that are not pasted directly into Streamer.bot.

This folder supports the Streamer.bot runtime source in [Actions/](../../Actions/) but does not contain runtime action scripts.

## When to Activate

Use this guide when working on:

- sync helpers under [Tools/StreamerBot/Sync/](./Sync/)
- validation scripts under [Tools/StreamerBot/Validation/](./Validation/)
- operator templates under [Tools/StreamerBot/Templates/](./Templates/)
- Streamer.bot export/import helper tooling

Do not activate this guide for Streamer.bot runtime C# action edits; those route to the relevant [Actions/](../../Actions/) local agent guide.

## Primary Owner

Primary owner: `ops`.

`ops` owns local tooling, validation command selection, sync workflow support, and terminal handoff requirements.

## Secondary Owners / Chain To

| Role | Chain when |
|---|---|
| `streamerbot-dev` | A tool changes how action scripts are pasted, synced, validated, or exported. |
| `app-dev` | A Streamer.bot helper affects app integrations such as overlay broker publishing. |
| `lotat-tech` | A validation or sync helper affects LotAT story-engine runtime handoff. |
| `brand-steward` | Public-facing generated text or operator-facing presentation copy changes. |

## Required Reading

Read these first for Streamer.bot tooling work:

1. [Tools/StreamerBot/README.md](./README.md) — folder purpose and boundaries.
2. [Actions/SHARED-CONSTANTS.md](../../Actions/SHARED-CONSTANTS.md) — canonical global variable, OBS source, and timer names.
3. [Actions/Helpers/AGENTS.md](../../Actions/Helpers/AGENTS.md) — reusable C# patterns for action scripts.
4. [.agents/workflows/coordination.md](../../.agents/workflows/coordination.md) — contribution workflow and coordination expectations.

## Local Workflow

1. Confirm whether the work is local tooling or runtime action source.
2. Keep runtime C# scripts in [Actions/](../../Actions/), not in this folder.
3. Keep Streamer.bot-specific support tools in the matching subfolder:
   - [Sync/](./Sync/) for repo-to-Streamer.bot paste or sync helpers.
   - [Validation/](./Validation/) for local validation scripts.
   - [Templates/](./Templates/) for reusable operator templates.
4. If a tool is legacy or deprecated, mark that clearly in the tool doc/header instead of deleting it during unrelated work.
5. If a tooling change affects action paste targets, include operator copy/paste impact in the handoff.
6. Finish with validation output and a terminal change summary.

## Validation

For syntax-level validation after Python script changes, run targeted compilation, for example:

```bash
python3 -m py_compile Tools/StreamerBot/Validation/*.py
```

For agent-doc changes, follow [.agents/workflows/validation.md](../../.agents/workflows/validation.md) and run the agent-tree validator with the task-requested report path.

For action-script changes outside this folder, use the local [Actions/](../../Actions/) guide and the relevant Streamer.bot validation checklist rather than only this tools guide. The source-of-truth contract validator is:

```bash
python3 Tools/StreamerBot/Validation/action_contracts.py --changed
```

After intentionally changing a local action contract in an Actions `AGENTS.md`, refresh the affected script stamp with:

```bash
python3 Tools/StreamerBot/Validation/action_contracts.py --script "Actions/<folder>/<script>.cs" --stamp
```

## Boundaries / Out of Scope

Do not use this folder to:

- store runtime action scripts from [Actions/](../../Actions/)
- store Mix It Up-specific API helpers or overlays; those belong in [Tools/MixItUp/](../MixItUp/)
- store creative scaffolding, lore, art prompts, or marketing content
- make behavior-changing runtime edits without routing through `streamerbot-dev`
- treat deprecated v1 routing tools as the source of truth for current routing docs

## Handoff Notes

After changes, report:

- changed files in [Tools/StreamerBot/](./)
- setup or dependency changes
- validation commands run and exact output
- whether operator paste/sync steps changed
- whether runtime [Actions/](../../Actions/) docs or constants need follow-up
- whether any legacy tooling was touched

## Runtime Notes

### Current subfolder intent

| Subfolder | Purpose |
|---|---|
| [Sync/](./Sync/) | Repo-to-Streamer.bot sync helpers and paste-support tooling. |
| [Validation/](./Validation/) | Validation scripts and transitional routing validation support. |
| [Templates/](./Templates/) | Reusable operator templates for Streamer.bot support workflows. |

### Relationship to Actions

[Actions/](../../Actions/) remains the source for Streamer.bot runtime scripts. Tools here may inspect, validate, sync, export, or template action content, but they should not become runtime paste targets themselves.

## Known Gotchas

- Streamer.bot action behavior is manually synced into Streamer.bot; local tool changes may still require operator paste or setup instructions.
- `Actions/SHARED-CONSTANTS.md` is canonical for global variable, OBS source, and timer names.
