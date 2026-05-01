# Step 2: Rewrite validate.py to scan AGENTS.md frontmatter

## Context

We removed all doc references to `.agents/manifest.json` in step 1. Now rewrite `Tools/AgentTree/validate.py` so it derives routing info from AGENTS.md frontmatter instead of manifest.json.

## Current state

The existing validator at `Tools/AgentTree/validate.py` loads `.agents/manifest.json` and runs 7 checks: schema, folder-coverage, link-integrity, drift, stub-presence, orphan, naming. All routing data comes from manifest.json.

## What AGENTS.md frontmatter looks like

Every AGENTS.md in the repo has YAML frontmatter like:

```yaml
---
id: actions-commanders
type: domain-route
description: Streamer.bot commander role actions...
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---
```

Role overviews at `.agents/roles/<role>/role.md` have similar frontmatter with `type: role-overview`.

Shared docs at `.agents/_shared/*.md` have `type: shared`.

Workflow docs at `.agents/workflows/*.md` have `type: workflow`.

## New validator design

Rewrite `validate.py` to:

1. **Discover** — recursively find all `AGENTS.md` files and all `.md` files under `.agents/`. Parse YAML frontmatter from each.

2. **Keep these checks:**
   - `frontmatter` — every AGENTS.md must have frontmatter with required fields: `id`, `type`, `description`, `status`. Role overviews also need `owner`.
   - `folder-coverage` — every first-level subfolder under Actions/, Apps/, Tools/, Creative/ must have an AGENTS.md file (either directly or in a parent that covers it).
   - `link-integrity` — all internal markdown links and backtick path mentions in agent docs must resolve. Keep the existing link-checking logic — it works well and doesn't depend on manifest.
   - `naming` — frontmatter `id` fields must be kebab-case. Domain AGENTS.md ids should match normalized folder path (e.g., `Actions/Commanders/` -> `actions-commanders`).
   - `id-uniqueness` — no two AGENTS.md files should have the same frontmatter `id`.

3. **Drop these checks:**
   - `schema` — was manifest JSON Schema validation. No manifest, no schema.
   - `drift` — was checking generated blocks against manifest. Tables are now static.
   - `orphan` — was checking files against manifest declarations. Too noisy without a manifest to declare expected files.

4. **Keep the same CLI interface:**
   - `python3 Tools/AgentTree/validate.py` — run all checks
   - `--repo PATH` — override repo root
   - `--report PATH` — write failure report to file
   - Same exit code behavior: 0 = pass, 1 = failures

5. **Keep these utility functions as-is** (they don't depend on manifest):
   - `find_repo_root()`
   - `rel()`
   - `line_for_offset()`
   - `Failure` and `CheckResult` dataclasses
   - `path_exists_case_sensitive()`
   - `normalize_segment()`
   - `expected_domain_id()`
   - `is_internal_target()`
   - `split_target()`
   - `looks_like_path()`
   - `resolve_doc_target()`
   - `collect_doc_refs()`
   - `print_summary()`

6. **Remove these functions** (manifest-only):
   - `render_table()`
   - `render_quick_routing()`
   - `render_roles_table()`
   - `table_after_heading()`
   - All manifest loading logic in `run()`

7. **New frontmatter parser:** The existing `parse_frontmatter()` function handles simple `key: value` lines but not YAML lists (like `secondaryOwners` or `workflows`). Extend it to handle `- item` list syntax under a key, or use a simple YAML approach. Do NOT add a pyyaml dependency — keep it stdlib-only. The existing regex + line parsing approach is fine, just extend for lists.

## Implementation notes

- Remove `import json` (no longer needed unless you keep it for other purposes)
- Remove `jsonschema` import and try/except block
- Remove all `manifest`, `schema`, `skills`, `domains`, `co_locations`, `workflows`, `aliases` variables from `run()`
- The `DOMAIN_ROOTS` constant stays: `("Actions", "Apps", "Tools", "Creative")`
- The `PATH_ROOTS` constant stays
- Remove `DERIVED_BLOCKS` constant (was for generated block drift checking)
- Keep the docstring but update it: "Validate the agent routing tree using AGENTS.md frontmatter."

## Verification

After rewriting, run:

```bash
python3 -m py_compile Tools/AgentTree/validate.py
python3 Tools/AgentTree/validate.py
```

The validator should run without errors (it will likely report some failures — that's fine, it means it's finding real issues). It must NOT reference or try to load manifest.json.

## Do NOT

- Delete manifest.json yet (step 3)
- Change any AGENTS.md content or frontmatter in domain folders
- Add external dependencies (pyyaml, etc.)
- Change the CLI interface or output format
