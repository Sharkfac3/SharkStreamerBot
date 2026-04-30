---
id: validation
type: workflow
description: Validator command selection, schema checks, and drift-check procedure.
status: active
owner: ops
appliesTo:
  - validation
  - schema-check
  - drift-check
skills:
  - ops
terminal: false
path: .agents/workflows/validation.md
sourceOfTruth: true
---
# Workflow: validation

## Purpose

Use this workflow to choose and report the checks that prove a change is safe or correctly migrated. It covers agent-tree validation, Streamer.bot runtime script checks, app checks, and tool/documentation validation.

## When to Run

Run validation before final handoff for any meaningful change. Select checks based on touched files and risk:

- Agent routing, workflows, or local agent docs: run the agent-tree validator.
- Streamer.bot action scripts: run action-contract validation, script sanity checks, and plan Streamer.bot smoke tests.
- Shared constants, timers, OBS names, or global variables: check constants and startup reset behavior.
- Apps: run type-check, build, and start checks when available.
- Tools: run CLI help, syntax, or happy-path commands when safe.
- Creative/canon docs: run canon review when content changes canon; see [canon-guardian.md](canon-guardian.md).

## Inputs

Use these references as applicable:

| Reference | Use |
|---|---|
| [Tools/AgentTree/validate.py](../../Tools/AgentTree/validate.py) | Agent-tree schema, coverage, link, frontmatter, orphan, and naming checks. |
| [Actions/SHARED-CONSTANTS.md](../../Actions/SHARED-CONSTANTS.md) | Canonical Streamer.bot globals, OBS source names, timer names, and reset expectations. |
| [Actions/Helpers/AGENTS.md](../../Actions/Helpers/AGENTS.md) | Reusable C# patterns and safe script idioms. |

## Procedure

1. Identify touched domains and choose checks.
2. Run automated checks from the repository root whenever possible.
3. For agent-tree routing, workflow, or local agent-doc changes, run:

```bash
python3 Tools/AgentTree/validate.py --report <requested-report-path>
```

4. Use the report path requested by the active task. If none is requested, omit `--report` or write to a task-specific path outside active docs.
5. For Streamer.bot scripts, validate local source-of-truth action contracts before handoff:

```bash
python3 Tools/StreamerBot/Validation/action_contracts.py --changed
```

If you intentionally changed an action contract, refresh the script stamp first:

```bash
python3 Tools/StreamerBot/Validation/action_contracts.py --script "Actions/<folder>/<script>.cs" --stamp
```

Then confirm syntax sanity and compare any globals, timers, OBS names, and persistent state against Actions shared constants.
6. For Apps changes, run the affected package type-check and build commands, then start the app if the prompt or local guide requires it.
7. For Tools changes, run the narrowest safe command that proves the tool still starts or produces expected output.
8. For Creative or canon changes, run the [canon-guardian workflow](canon-guardian.md) when the change affects reusable story, character, or brand canon.
9. Record exact command output summaries and exit codes. If a check fails, capture the failure category, report whether it blocks handoff, and escalate unexpected failures to the operator.

## Validation / Done Criteria

Validation is done when:

- [ ] Each touched domain has an appropriate check or a clear not-run rationale.
- [ ] Commands were run from the repository root unless otherwise stated.
- [ ] Exit codes and key output were captured.
- [ ] Any failures are categorized, with blocker status and escalation notes.
- [ ] Manual checks still needed by the operator are explicitly marked pending.

## Output / Handoff

In the final [change-summary workflow](change-summary.md) output, include:

- Command run.
- Exit code.
- Key stdout summary.
- Report path, if generated.
- Remaining failures or manual validation steps.

If the task asks for a handoff file, include validator output there as well.

## Related Routes

- Ops role: [ops role overview](../roles/ops/role.md)
- Change Summary workflow: [change-summary.md](change-summary.md)
- Sync workflow: [sync.md](sync.md)
- Canon Guardian workflow: [canon-guardian.md](canon-guardian.md)
- Agent-tree validator: [Tools/AgentTree/validate.py](../../Tools/AgentTree/validate.py)

## Role-Specific Notes

### Agent-tree routing

The agent-tree validator is the default gate for manifest, role, workflow, and local `AGENTS.md` changes. Treat nonzero validator exits as blockers unless the active task explicitly defines a narrower acceptance criterion.

### Streamer.bot scripts

Validation has three layers: source-of-truth action contract validation, repository-side script sanity, and Streamer.bot-side smoke testing after paste. Do not claim runtime validation passed if only repository-side checks ran.

Local Actions action contracts are authoritative. A script behavior change is not complete until the nearest action contract describes the behavior and the script carries a current `ACTION-CONTRACT-SHA256` stamp generated by `Tools/StreamerBot/Validation/action_contracts.py`.

### Apps

Type-check and build are the default baseline. Start the app when the change affects runtime wiring, ports, API routes, or UI behavior.

### Tools

Prefer deterministic local commands. Avoid destructive API calls unless the prompt explicitly asks for them or the operator approves.
