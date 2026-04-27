---
id: tools-agent-tree
type: domain-route
description: Agent manifest validation tooling for domain coverage, stub presence, naming, drift, and agent-doc link checks.
owner: ops
secondaryOwners: []
workflows:
  - change-summary
  - validation
status: active
---

# Tools/AgentTree — Agent Guide

## Purpose

[Tools/AgentTree/](./) contains validation tooling for the agent routing tree. It verifies manifest v2 schema compliance, first-level domain route coverage, local agent doc presence, generated-doc drift, entry-file frontmatter, naming conventions, and agent-doc link integrity.

This folder is its own `tools-agent-tree` route because it validates the manifest-backed agent tree. It is not covered by [Tools/StreamerBot/AGENTS.md](../StreamerBot/AGENTS.md); Streamer.bot tooling and agent-tree routing validation are related ops concerns but have different sources of truth.

## When to Activate

Use this guide when working on:

- [Tools/AgentTree/validate.py](./validate.py)
- [.agents/manifest.json](../../.agents/manifest.json) validation behavior
- [.agents/manifest.schema.json](../../.agents/manifest.schema.json) schema support
- validator reports requested by the active task
- domain route coverage and local `AGENTS.md` validation mechanics
- agent-tree acceptance checks

Do not activate this guide for Streamer.bot runtime script validation, Mix It Up API tooling, or content/art pipeline validators except where they appear as manifest-routed domain docs.

## Primary Owner

Primary owner: `ops`.

`ops` owns validator execution, acceptance reports, schema-aware routing checks, and handoff summaries for agent-tree work.

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
2. [.agents/manifest.json](../../.agents/manifest.json) — manifest v2 source of truth.
3. [.agents/manifest.schema.json](../../.agents/manifest.schema.json) — schema enforced by the validator.
4. [.agents/_shared/conventions.md](../../.agents/_shared/conventions.md) — repo-wide naming and routing conventions.
5. [.agents/workflows/validation.md](../../.agents/workflows/validation.md) — reusable validation procedure and reporting expectations.

## Local Workflow

1. Confirm whether the task changes validator code, manifest data, schema rules, or local agent docs.
2. If adding a first-level domain folder under Actions, Apps, Tools, or Creative, add an explicit manifest domain route.
3. If adding a local route doc, use `AGENTS.md` and the required frontmatter fields.
4. If changing schema structure, update both [.agents/manifest.schema.json](../../.agents/manifest.schema.json) and [.agents/manifest.json](../../.agents/manifest.json) consistently.
5. Run the validator and save the failure report path requested by the active prompt.
6. Record the final disposition of any newly covered folder in the handoff.

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
- whether schema support was changed
- final disposition for any newly introduced first-level domain folder

## Runtime Notes

### Validator checks

[validate.py](./validate.py) currently checks:

| Check | Purpose |
|---|---|
| schema | Validate manifest v2 against its JSON Schema. |
| folder-coverage | Ensure domain folders, skill locations, co-locations, and declared domain paths exist. |
| link-integrity | Check internal Markdown links and path-like backtick mentions in agent docs. |
| drift | Compare generated routing surfaces against the manifest. |
| stub-presence | Require manifest skill entry files and required frontmatter. |
| orphan | Report unreferenced or undeclared files under the agent tree. |
| naming | Enforce kebab-case IDs, domain ID normalization, and `AGENTS.md` local doc names. |

### Manifest coverage decision

`Tools/AgentTree/` has explicit manifest coverage through route `tools-agent-tree`. It is not declared `coveredBy` another route because the folder owns a distinct validator with manifest/schema semantics, while [Tools/StreamerBot/](../StreamerBot/) owns Streamer.bot sync and support utilities.

## Known Gotchas

- The validator is intentionally strict; address failures before handoff unless the active task defines a narrower acceptance criterion.
- Backtick path mentions are treated as path references by the current validator.
- Generated routing-table drift is checked against manifest v2.
- Manifest paths fail folder-coverage until files exist or coverage is represented differently.
