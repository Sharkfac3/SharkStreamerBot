# Phase 3: Consistency Pass Across Domain Guides

## Context

You are working in the SharkStreamerBot repository. Phases 1, 2a, and 2b created domain-level `AGENTS.md` guides for `Apps/`, `Tools/`, and `Creative/`. This phase audits all 4 domain guides for consistency and updates root routing files.

## Required Reading

Read **all 4 domain guides**:

1. `Actions/AGENTS.md` — the original (template).
2. `Apps/AGENTS.md` — Phase 1 output.
3. `Tools/AGENTS.md` — Phase 2a output.
4. `Creative/AGENTS.md` — Phase 2b output.

Read **root routing files**:

5. `AGENTS.md` (repo root) — root agent entry and quick routing surface.
6. `.agents/ENTRY.md` — central agent entrypoint.
7. `.agents/_shared/project.md` — repo-wide context with domain table.
8. `.agents/_shared/conventions.md` — file routing conventions.

## Reference Structure Spec

All 4 domain guides should follow this structure. Use this as your comparison baseline:

```
---
id: <domain>-agent-guide        # e.g., apps-agent-guide, tools-agent-guide
type: domain-guide
description: <one-line summary>
status: active
owner: <primary-role>
---

# <Domain> — <Subtitle>

## Purpose                      # 2-3 sentences
## Start Here                   # numbered agent arrival steps
## <Inventory Table>            # domain-specific name; table with paths, owners, purpose
## Folder Routing               # table mapping subdirs to local AGENTS.md
## <Domain-Specific Sections>   # e.g., Dependency Order, Cross-Cutting Refs, Canon Guardian
## Shared References            # table of cross-cutting docs
## Validation                   # validator command + domain-specific checks
## Boundaries                   # what does NOT belong here
```

### Frontmatter Requirements (validator-enforced)

| Field | Required | Rule |
|---|---|---|
| `id` | Yes | kebab-case, pattern `<domain>-agent-guide` for top-level domain guides |
| `type` | Yes | `domain-guide` for domain-level files |
| `description` | Yes | One-line summary |
| `status` | Yes | `active` |
| `owner` | Recommended | Primary role |

## Audit Checklist

Compare all 4 domain guides. For each check, note the file path and issue if it fails.

### Structural Consistency
- [ ] All 4 have frontmatter with `id`, `type`, `description`, `status`
- [ ] All 4 have these sections in order: Purpose, Start Here, Inventory/Routing, Validation, Boundaries
- [ ] Section heading style is consistent (e.g., all use `## Section Name`, not mixed `##` and `###`)
- [ ] Markdown link format is consistent (relative paths, same style)

### Content Consistency
- [ ] Each domain guide's self-description matches what root `AGENTS.md` and `.agents/_shared/project.md` say about that domain
- [ ] Boundary statements are symmetric (Apps says "C# → Actions", Actions says "apps → Apps")
- [ ] Cross-domain references are bidirectional where appropriate
- [ ] Validation commands use correct Python detection per `.agents/_shared/conventions.md` (detect `python` vs `python3`)

### Package Manager Accuracy
- [ ] `Apps/AGENTS.md` documents which apps use npm vs pnpm
- [ ] Each app-level AGENTS.md validation section uses correct package manager
- [ ] No guide says `npm` for a `pnpm` project or vice versa

### Root Routing Completeness
- [ ] Root `AGENTS.md` Project Domains table links to each domain's AGENTS.md (not just the folder)
- [ ] `.agents/_shared/project.md` Repository Domains table is consistent with the new domain guides

## Deliverables

### 1. Audit Report

**Before making any changes**, write a brief report listing:
- Issues found (file path, line number, description)
- Missing cross-references
- Recommended fixes

Present the report, then proceed with fixes.

### 2. Fixes

Apply fixes for issues found. Likely changes:

- **`AGENTS.md` (root)**: Update Project Domains table. Currently it has:
  ```
  | Apps | [Apps/](Apps/) | Standalone TypeScript apps and app-local guides. |
  ```
  It should reference `Apps/AGENTS.md` as the local routing guide, same pattern for Tools and Creative.

- **`.agents/_shared/project.md`**: Update Repository Domains table if it doesn't reference the new domain guides.

- **Domain guides**: Fix any structural inconsistencies, missing sections, broken links, or contradictory boundaries found in the audit.

- **App-level AGENTS.md files**: Fix package manager commands only if they are factually wrong.

### 3. Validation

After all changes, run:

```bash
python Tools/AgentTree/validate.py
```

Report full output. Fix any failures.

## Constraints

- Do NOT restructure or rename existing folders.
- Do NOT change app behavior, code, or configuration.
- Only modify documentation/routing files.
- Keep changes minimal — fix inconsistencies, don't rewrite working guides.
- Preserve all existing frontmatter fields; only add missing ones.
- For every file you modify: state what changed and why in your response.

## What the Validator Checks

So you can interpret failures correctly:

| Check | What it does |
|---|---|
| frontmatter | Required fields `id`, `type`, `description`, `status` present in all AGENTS.md and role.md files |
| folder-coverage | Every first-level subdirectory under Actions/, Apps/, Tools/, Creative/ has AGENTS.md coverage |
| link-integrity | All markdown links and backtick paths in agent docs resolve to real files |
| naming | IDs are kebab-case; domain route IDs match normalized path pattern |
| id-uniqueness | No duplicate frontmatter IDs across all AGENTS.md files |
