# Step 3: Delete manifest.json and schema

## Context

Steps 1 and 2 removed all references to manifest.json from docs and rewrote the validator. Now delete the files.

## Tasks

### 1. Delete the files

```bash
git rm .agents/manifest.json
git rm .agents/manifest.schema.json
```

### 2. Final grep

Confirm no code or docs still import/reference these files:

```bash
grep -r "manifest\.json" --include="*.py" --include="*.md" --include="*.ts" --include="*.js" .
grep -r "manifest\.schema" --include="*.py" --include="*.md" .
```

Only hits should be in `Projects/kill-manifest/` (this project folder). If anything else references them, fix it before committing.

### 3. Commit

Commit message:

```
Remove manifest.json and manifest.schema.json

Routing info lives in AGENTS.md frontmatter. Validator now scans
frontmatter directly. No agent consumed the JSON at runtime.
```

## Do NOT

- Delete any AGENTS.md files
- Modify validate.py (already done in step 2)
- Remove the Projects/kill-manifest folder (keep for reference until project is closed)
