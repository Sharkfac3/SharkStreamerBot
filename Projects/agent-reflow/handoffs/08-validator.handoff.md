# Prompt 08 handoff — validator

Date: 2026-04-26
Agent: pi

## State changes

- Created `Tools/AgentTree/validate.py` as the manifest v2 agent-tree validator.
- Added a deprecation header to `Tools/StreamerBot/Validation/retired-routing-doc-sync.py`, marking it as legacy v1 routing-table sync tooling.
- Wrote `Projects/agent-reflow/findings/08-validator.md` with run instructions, check descriptions, baseline output, and expected failure themes.
- Wrote `Projects/agent-reflow/findings/08-validator.failures.txt` with the full baseline failure list.
- Updated `WORKING.md` to record completion.

## Findings appended

- `findings/08-validator.md`: validator summary, including:
  - artifact locations
  - run commands
  - exit-code contract
  - implemented check categories
  - baseline stdout
  - expected failure themes before Phase E
  - full baseline failure list embedded for handoff readability
- `findings/08-validator.failures.txt`: full machine-readable/text failure backlog from the baseline validator run.

## Inputs read

- `.agents/ENTRY.md`
- `WORKING.md`
- `retired Pi skill mirror/ops/SKILL.md`
- `retired Pi skill mirror/ops-validation/SKILL.md`
- `retired Pi skill mirror/ops-change-summary/SKILL.md`
- `.agents/_shared/project.md`
- `.agents/roles/ops/role.md`
- `.agents/roles/ops/skills/core.md`
- `.agents/roles/ops/skills/validation/_index.md`
- `.agents/roles/ops/skills/change-summary/_index.md`
- `Projects/agent-reflow/findings/05-target-shape.md`
- `Projects/agent-reflow/findings/06-naming-convention.md`
- `Projects/agent-reflow/findings/07-manifest-v2.md`
- `.agents/manifest.json`
- `.agents/manifest.schema.json`
- `Tools/StreamerBot/Validation/retired-routing-doc-sync.py`

## Validator contents

`Tools/AgentTree/validate.py` implements these checks:

| Check | Purpose |
|---|---|
| `schema` | Validates `.agents/manifest.json` against `.agents/manifest.schema.json`. |
| `folder-coverage` | Confirms domain folders are routed and declared skill/co-location/domain paths exist. |
| `link-integrity` | Checks markdown links and backtick path mentions in agent docs. |
| `drift` | Checks generated/derived routing surfaces against manifest v2. |
| `stub-presence` | Requires every manifest skill entry file to exist and have required frontmatter when Markdown. |
| `orphan` | Reports `.agents/` files not declared or referenced. |
| `naming` | Enforces prompt 06 ID/path naming conventions. |

## Decisions / implementation notes

- Placed the validator at `Tools/AgentTree/validate.py` to separate agent-tree validation from Streamer.bot runtime validation.
- Left `Tools/StreamerBot/Validation/retired-routing-doc-sync.py` in place, but marked it deprecated rather than deleting it, per prompt scope.
- Used `.agents/manifest.json` as the validator source of truth, not legacy `legacy v1 routing manifest (retired)`.
- Made the validator strict by design: planned paths in the manifest fail until Phase E creates docs or marks coverage differently.
- Saved the full baseline failure report so Phase E prompts can use it as a migration backlog.
- Preserved approved project-specific normalized names such as `lotat` and `worldbuilding` in the naming check.

## Validator status

Syntax check:

```bash
python3 -m py_compile Tools/AgentTree/validate.py
```

Exit code: 0

Baseline run:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/08-validator.failures.txt
```

Exit code: 1

Stdout:

```text
Agent tree validation summary
| Check | Checked | Failures | Status |
|---|---:|---:|---|
| schema | 1 | 0 | PASS |
| folder-coverage | 149 | 65 | FAIL |
| link-integrity | 92 | 331 | FAIL |
| drift | 3 | 3 | FAIL |
| stub-presence | 48 | 48 | FAIL |
| orphan | 99 | 18 | FAIL |
| naming | 106 | 0 | PASS |

Total failures: 465
Failure report: /mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot/Projects/agent-reflow/findings/08-validator.failures.txt
```

Expected result: failing until Phase E migrations create/move target docs and normalize references.

## Open questions / blockers

- `Tools/AgentTree/` is now a new first-level `Tools/` folder and the current manifest does not route it, so the validator reports it under `folder-coverage`.
- Many declared workflow and co-located `AGENTS.md` paths do not exist yet; Phase E must create/move them.
- Existing role/core/skill docs do not yet have the prompt 06 frontmatter.
- Existing generated tables still reflect v1 routing and are stale against manifest v2.
- Existing links/backtick path mentions contain old shorthand and placeholder patterns; Phase E should normalize or reclassify them.

## Next prompt entry point

Proceed per `Projects/agent-reflow/prompts/09-generate-migration-prompts.md`.

Use these inputs:

1. `Tools/AgentTree/validate.py`
2. `Projects/agent-reflow/findings/08-validator.md`
3. `Projects/agent-reflow/findings/08-validator.failures.txt`
4. `Projects/agent-reflow/handoffs/08-validator.handoff.md`
5. `.agents/manifest.json`
6. `.agents/manifest.schema.json`
7. `Projects/agent-reflow/findings/05-target-shape.md`
8. `Projects/agent-reflow/findings/06-naming-convention.md`
9. `Projects/agent-reflow/findings/07-manifest-v2.md`
