# Step 1: Remove manifest references from docs

## Context

We are removing `.agents/manifest.json` from the repo. It duplicates routing info already in AGENTS.md frontmatter. This step updates all docs that reference it before the file is deleted.

## Tasks

### 1. Edit `AGENTS.md` (repo root)

- Remove the `<!-- GENERATED:agents-quick-role-routing:start -->` and `<!-- GENERATED:agents-quick-role-routing:end -->` comment markers around the Quick Role Routing table. Keep the table itself — it's useful, just no longer generated from manifest.
- In the Project Domains table, change the Agent Tree row description from "Manifest, role overviews, workflows, and shared agent context." to "Role overviews, workflows, and shared agent context."
- In Key References table, update the Tools/AgentTree row purpose from "Manifest/validator route for agent-tree tooling." to "Validator route for agent-tree tooling."

### 2. Edit `.agents/ENTRY.md`

- In frontmatter line 4, change `description` from "Central .agents entrypoint with manifest-backed routing summary." to "Central .agents entrypoint with routing summary."
- Line 19: change "Choose a role from the manifest-backed table below." to "Choose a role from the table below."
- Remove the `<!-- GENERATED:agents-roles:start -->` and `<!-- GENERATED:agents-roles:end -->` comment markers around the Roles table. Keep the table.
- Delete the entire `## Manifest` section at the bottom (lines 55-57), which says "The target routing source is [manifest.json](manifest.json)..."

### 3. Edit `.agents/_shared/conventions.md`

- Line 24: change "Agent roles, workflows, manifest routing, and shared agent context." to "Agent roles, workflows, and shared agent context."
- Line 27: change "Manifest IDs use kebab-case." to "Frontmatter IDs use kebab-case."
- Line 39: change "Avoid adding new Pi wrapper skills; use manifest-backed local guides and role/workflow docs instead." to "Avoid adding new Pi wrapper skills; use local AGENTS.md guides and role/workflow docs instead."

### 4. Edit `Tools/AgentTree/AGENTS.md`

This file heavily references manifest.json. Rewrite these sections:

- Frontmatter line 4: change description to "Agent-tree validation tooling for domain coverage, stub presence, naming, and agent-doc link checks."
- Purpose section: replace "manifest v2 schema compliance" language with "AGENTS.md frontmatter scanning". Remove mentions of manifest.json throughout.
- "When to Activate" list: remove the manifest.json and manifest.schema.json bullet points.
- Required Reading: remove items 2 and 3 (manifest.json and manifest.schema.json references).
- Local Workflow: remove step 4 about schema structure. Update other steps to reference AGENTS.md frontmatter instead of manifest.
- Validation section: commands stay the same (python3 Tools/AgentTree/validate.py).
- Runtime Notes > Validator checks table: remove "schema" and "drift" rows. Update "folder-coverage" description to say it scans AGENTS.md frontmatter. Remove "orphan" row (was manifest-dependent).
- Runtime Notes > Manifest coverage decision: rewrite to say "Tools/AgentTree/ has its own AGENTS.md because the folder owns a distinct validator, while Tools/StreamerBot/ owns sync utilities."
- Known Gotchas: remove manifest-specific gotchas. Keep the general strictness warning.

### 5. Edit `AGENTS.md` (root) frontmatter

- Line 12: change "Start Here" section text from "manifest-backed universal entry point" to "universal entry point"

## Verification

After edits, grep the entire repo for "manifest" to confirm no stale references remain in docs:

```bash
grep -ri "manifest" --include="*.md" .
```

Any remaining hits should only be in `Projects/kill-manifest/` (this project folder).

## Do NOT

- Delete manifest.json or manifest.schema.json yet (step 3 handles that)
- Modify validate.py (step 2 handles that)
- Change any AGENTS.md frontmatter fields in domain folders — those are already correct
