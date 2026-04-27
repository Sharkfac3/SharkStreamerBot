# Prompt 00 — Inventory Agents Tree

## Agent

Pi (manual copy/paste by operator).

## Purpose

First read-only inventory pass. Catalogs every file under `.agents/` so later phases can plan migrations against ground truth instead of assumptions. Writes findings, makes no edits.

## Preconditions

- Working directory: repo root (`SharkStreamerBot/`).
- Branch: `feature/agent-retool-via-reflow`.
- No prior prompt run; this is the first.
- `Projects/agent-reflow/findings/` may not exist yet — create it if missing.
- `Projects/agent-reflow/handoffs/` may not exist yet — create it if missing.

## Scope

Read-only inventory of `.agents/` only. Writes one new file: `Projects/agent-reflow/findings/00-current-tree.md`.

Files in scope (read):
- Every file under `.agents/` (recursive)
- `legacy v1 routing manifest (retired)` (parse for cross-reference)

Files in scope (write):
- `Projects/agent-reflow/findings/00-current-tree.md` (new)
- `Projects/agent-reflow/handoffs/00-inventory-agents-tree.handoff.md` (new)

## Out-of-scope

- No edits to any file under `.agents/`
- No edits to `.pi/`, `Actions/`, `Apps/`, `Creative/`, `Docs/`, `Tools/`, root markdown
- No git operations (operator commits manually)
- No deletion of any file
- No creation of files outside `Projects/agent-reflow/findings/` and `Projects/agent-reflow/handoffs/`

## Steps

1. Confirm `Projects/agent-reflow/findings/` and `Projects/agent-reflow/handoffs/` exist. Create if missing.

2. Walk `.agents/` recursively. For each file, capture:
   - Path relative to repo root
   - Line count
   - File type (`role.md`, `core.md`, `_index.md`, leaf skill `.md`, `legacy-v1-routing-manifest`, other)
   - Top-level frontmatter `name` field if present
   - Top-level frontmatter `description` field if present
   - First H1 heading text if present

3. For each role folder under `.agents/roles/<role>/`, capture:
   - Role name (folder name)
   - Whether `role.md` exists
   - Whether `skills/core.md` exists
   - List of sub-skill folders under `skills/`
   - List of leaf skills (`.md` files directly under `skills/`)
   - Whether `context/` folder exists and what's inside
   - Total line count for the role (sum of all `.md` files in the role tree)

4. Parse `legacy v1 routing manifest (retired)`. Capture:
   - Every role declared
   - Every canonical sub-skill declared
   - Every alias declared
   - For each canonical sub-skill: `required_surfaces` list
   - For each helper: `name`, `purpose`, `canonical_children`

5. Cross-check: for every role + sub-skill in the manifest, verify the corresponding folder/file exists under `.agents/roles/`. Flag mismatches (manifest says X exists but folder missing, or folder exists but manifest does not declare).

6. List `.agents/_shared/` contents and line counts.

7. Write findings to `Projects/agent-reflow/findings/00-current-tree.md` using the template below.

## Findings File Template

```
# Findings 00 — Current `.agents/` Tree

Generated: <YYYY-MM-DD>
Source: prompt 00-inventory-agents-tree

## Top-Level Layout

| Path | Type | Lines | Notes |
|---|---|---|---|
| .agents/ENTRY.md | entry doc | <N> | <H1> |
| legacy v1 routing manifest (retired) | manifest | <N> | <count of roles, sub-skills, aliases> |
| .agents/_shared/ | shared dir | <total lines> | <list of files> |
| .agents/roles/ | roles dir | <total lines> | <count of roles> |

## Roles

For each role:

### Role: <role-name>

- Folder: `.agents/roles/<role-name>/`
- Total lines: <N>
- `role.md`: <yes/no>, <line count>
- `skills/core.md`: <yes/no>, <line count>
- Sub-skill folders: <list>
- Leaf skill files: <list>
- `context/`: <yes/no>, <contents>
- Manifest declares this role: <yes/no>
- Manifest canonical sub-skills for this role: <list>
- Mismatches: <any folder-vs-manifest discrepancies>

## `_shared/` Contents

| File | Lines | First H1 |
|---|---|---|
| .agents/_shared/<file> | <N> | <heading> |

## Manifest Cross-Check

### Manifest says exists, folder missing
- <list or "none">

### Folder exists, manifest does not declare
- <list or "none">

### Required surfaces declared, file missing
- <list or "none">

## Aliases

| Alias | Targets | Targets exist? |
|---|---|---|
| <alias-name> | <list> | <yes/no per target> |

## Helpers (`meta/`, etc.)

| Name | Folder under `retired Pi skill mirror/`? | Manifest required_surfaces present? |
|---|---|---|

## Summary Statistics

- Total `.md` files in `.agents/`: <N>
- Total lines across all `.md` files: <N>
- Roles count: <N>
- Total sub-skill folders: <N>
- Total leaf skill files: <N>
- Manifest-vs-folder mismatches: <N>
- Largest single file: <path>, <lines>
- Smallest non-empty file: <path>, <lines>
```

## Validator / Acceptance

Manual check by operator:
- `Projects/agent-reflow/findings/00-current-tree.md` exists and is non-empty
- Roles listed match what the operator can see in `.agents/roles/` (visual spot check)
- Manifest cross-check section is populated (even if "none" in each subsection)
- No edits outside the two new files (operator can confirm with `git status` showing only `Projects/agent-reflow/` additions)

## Handoff

Write `Projects/agent-reflow/handoffs/00-inventory-agents-tree.handoff.md`:

```
# Prompt 00 handoff — inventory-agents-tree

Date: <YYYY-MM-DD>
Agent: pi

## State changes
- Created Projects/agent-reflow/findings/00-current-tree.md
- Created Projects/agent-reflow/handoffs/00-inventory-agents-tree.handoff.md

## Findings appended
- findings/00-current-tree.md: full inventory of `.agents/` tree, manifest cross-check, summary stats

## Assumptions for prompt 01
- `.agents/` tree state captured at <date>; if operator edits `.agents/` between this prompt and prompt 01, re-run prompt 00 first
- Manifest declarations vs folder reality is now documented; prompt 01 (Pi mirror inventory) can compare against these baselines

## Validator status
- N/A (read-only inventory; manual operator check only)

## Open questions / blockers
- <any roles where role.md or core.md is missing>
- <any manifest-vs-folder mismatches that need operator attention>
- <any `_shared/` files that look stale or unused>

## Next prompt entry point
- Read this handoff
- Then read findings/00-current-tree.md
- Then proceed per prompts/01-inventory-pi-mirror.md
```
