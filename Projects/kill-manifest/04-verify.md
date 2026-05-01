# Step 4: Verify clean state

## Context

Steps 1-3 removed manifest.json, updated docs, and rewrote the validator. This step confirms everything is clean.

## Tasks

### 1. Run the validator

```bash
python3 Tools/AgentTree/validate.py --report Projects/kill-manifest/validation-report.txt
```

Review the output. Expected checks: frontmatter, folder-coverage, link-integrity, naming, id-uniqueness. All should pass or have only pre-existing issues unrelated to this project.

### 2. Check for stale references

```bash
grep -ri "manifest" --include="*.md" --include="*.py" --include="*.json" . | grep -v "Projects/kill-manifest"
```

Should return zero results. If any remain, fix them.

### 3. Check link integrity manually

Spot-check that these files have no broken links:

- `AGENTS.md` (root)
- `.agents/ENTRY.md`
- `.agents/_shared/conventions.md`
- `Tools/AgentTree/AGENTS.md`

### 4. Confirm frontmatter consistency

Spot-check 3-4 AGENTS.md files across different domains to confirm frontmatter `id` fields follow the `<root>-<folder>` pattern:

- `Actions/Commanders/AGENTS.md` should have `id: actions-commanders`
- `Apps/stream-overlay/AGENTS.md` should have `id: apps-stream-overlay`
- `Tools/AgentTree/AGENTS.md` should have `id: tools-agent-tree`
- `Creative/Brand/AGENTS.md` should have `id: creative-brand`

### 5. Report

If everything passes, this project is done. The validation report at `Projects/kill-manifest/validation-report.txt` is the acceptance artifact.

If failures exist, document them and determine whether they are blockers or pre-existing issues.
