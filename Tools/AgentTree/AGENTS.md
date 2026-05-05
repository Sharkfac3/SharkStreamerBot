---
id: tools-agent-tree
type: domain-route
description: Agent-tree validation tooling for domain coverage, stub presence, naming, and agent-doc link checks.
owner: ops
secondaryOwners: []
workflows:
  - change-summary
  - validation
status: active
---

# Tools/AgentTree — Agent Guide

## Purpose

[Tools/AgentTree/](./) contains validation tooling for the agent routing tree. It verifies first-level domain route coverage through `AGENTS.md` frontmatter scanning, local agent doc presence, entry-file frontmatter, naming conventions, and agent-doc link integrity.

This folder is its own `tools-agent-tree` route because it owns agent-tree validation. It is not covered by [Tools/StreamerBot/AGENTS.md](../StreamerBot/AGENTS.md); Streamer.bot tooling and agent-tree validation are related ops concerns but have different sources of truth.

## When to Activate

Use this guide when working on:

- [Tools/AgentTree/validate.py](./validate.py)
- validator reports requested by the active task
- domain route coverage and local `AGENTS.md` validation mechanics
- agent-tree acceptance checks

Do not activate this guide for Streamer.bot runtime script validation, Mix It Up API tooling, or content/art pipeline validators except where they appear as local domain docs.

## Primary Owner

Primary owner: `ops`.

`ops` owns validator execution, acceptance reports, routing checks, and handoff summaries for agent-tree work.

## Secondary Owners / Chain To

| Role | Chain when |
|---|---|
| `streamerbot-dev` | Validator changes affect action sync or Streamer.bot-specific local docs. |
| `app-dev` | Validator changes affect app-local docs or TypeScript app routing. |
| `art-director` | Validator changes affect art-pipeline or creative art route docs. |
| `content-repurposer` | Validator changes affect content-pipeline route docs. |
| `lotat-tech` | Validator changes affect LotAT tool/runtime route docs. |

## Required Reading

Read these first for agent-tree validation work:

1. [Tools/AgentTree/validate.py](./validate.py) — implementation and check list.
2. [.agents/_shared/conventions.md](../../.agents/_shared/conventions.md) — repo-wide naming and routing conventions.
3. [.agents/workflows/validation.md](../../.agents/workflows/validation.md) — reusable validation procedure and reporting expectations.

## Local Workflow

1. Confirm whether the task changes validator code or local agent docs.
2. If adding a first-level domain folder under Actions, Apps, Tools, or Creative, add or update the appropriate local `AGENTS.md` route doc with required frontmatter.
3. If adding a local route doc, use `AGENTS.md` and the required frontmatter fields.
4. Run the validator and save the failure report path requested by the active prompt.
5. Record the final disposition of any newly covered folder in the handoff.

## Validation

Compile the validator after Python edits:

```bash
python3 -m py_compile Tools/AgentTree/validate.py
```

Run the agent-tree validator with the report path named by the active task:

```bash
python3 Tools/AgentTree/validate.py --report <requested-report-path>
```

If no report path is requested, run `python3 Tools/AgentTree/validate.py`. Treat nonzero exits as blockers unless the task explicitly defines a narrower acceptance criterion.

## Boundaries / Out of Scope

Do not use this folder to:

- validate Streamer.bot runtime C# behavior directly
- replace app-specific typecheck or build commands
- store generated local agent docs
- edit creative, content, or runtime source only to satisfy unrelated validator failures
- delete or archive routing files during focused validation tasks unless explicitly requested

## Handoff Notes

After changes, report:

- changed files in [Tools/AgentTree/](./), [.agents/](../../.agents/), and local route docs
- validator command run and exact summary output
- failures that cleared
- remaining failures and whether they block handoff
- whether routing/frontmatter support was changed
- final disposition for any newly introduced first-level domain folder

## Runtime Notes

### Validator checks

[validate.py](./validate.py) currently checks:

| Check | Purpose |
|---|---|
| folder-coverage | Ensure domain folders and declared domain paths exist by scanning `AGENTS.md` frontmatter. |
| link-integrity | Check internal Markdown links and path-like backtick mentions in agent docs. |
| stub-presence | Require skill entry files and required frontmatter. |
| naming | Enforce kebab-case IDs, domain ID normalization, and `AGENTS.md` local doc names. |

### Coverage decision

`Tools/AgentTree/` has its own `AGENTS.md` because the folder owns a distinct validator, while [Tools/StreamerBot/](../StreamerBot/) owns sync utilities.

## Known Gotchas

- The validator is intentionally strict; address failures before handoff unless the active task defines a narrower acceptance criterion.
- Backtick path mentions are treated as path references by the current validator.
