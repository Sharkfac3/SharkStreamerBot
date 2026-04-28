---
id: trigger-catalog-phase-01-skeleton
type: project-phase
description: Scaffold the catalog folder tree, READMEs, and per-subcategory files with frontmatter, trigger headings, and upstream URL stubs. No args yet.
status: active
owner: streamerbot-dev
phase: 1
depends_on:
  - 00-pre-seed.md
---

# Phase 1 — Skeleton

## Goal

From `manifest.json`, scaffold the entire `Actions/Helpers/triggers/` tree with:

- Top-level README (platform index + lookup-order rule).
- One README per platform (subcategory index + coverage flags).
- One file per subcategory (frontmatter + trigger headings + upstream URL per trigger + `coverage: stub`).

Every trigger gets a placeholder section. No args content yet — that's Phase 3+.

## Prerequisites

1. Read `Projects/streamerbot-trigger-catalog/README.md` (conventions, slug rules, file schemas).
2. Read `manifest.json`.
3. Read [.agents/ENTRY.md](../../.agents/ENTRY.md) and [.agents/roles/streamerbot-dev/role.md](../../.agents/roles/streamerbot-dev/role.md).
4. Coordination check via [WORKING.md](../../WORKING.md).
5. Glance at [Actions/Helpers/README.md](../../Actions/Helpers/README.md) and [Actions/Helpers/cph-api-signatures.md](../../Actions/Helpers/cph-api-signatures.md) so the new tree fits the existing Helpers pattern.

## Steps

1. Create folder `Actions/Helpers/triggers/` if missing.
2. Write `Actions/Helpers/triggers/README.md`:
   - Frontmatter (`type: reference`, `id: triggers-index`, `owner: streamerbot-dev`, `status: active`).
   - Section: "Why this exists" — short paragraph pointing to project rationale.
   - Section: "Lookup order" — verbatim from project README.
   - Table of platforms with link to each platform README and a coverage flag column (initial value: `stub`).
   - Section: "How to add a new trigger entry" — pointer to project README schemas.
3. For each platform in `manifest.json`:
   - Create folder `Actions/Helpers/triggers/<platform-slug>/`.
   - Write `Actions/Helpers/triggers/<platform-slug>/README.md`:
     - Frontmatter (`id: triggers-<platform-slug>`, `coverage: stub`, `upstream: <platform upstream URL>`).
     - Table of subcategories with link to each subcategory file and a coverage flag column.
4. For each subcategory in each platform:
   - Write `Actions/Helpers/triggers/<platform-slug>/<subcategory-slug>.md`:
     - Frontmatter per project README schema (`coverage: stub`, `upstream: <subcategory URL>`).
     - One `## <Trigger Name>` heading per trigger.
     - Under each heading: `Path:`, `Upstream:`, `Min SB version: _unknown — fill in Phase 3+_`, `Coverage: stub`.
     - Empty `### Args` section with placeholder `_Pending — see Phase 3+ for content fill._`.
     - Empty `### Caveats` section with placeholder `_None recorded yet._`.
     - Empty `### Used in repo` section with placeholder `_Not yet wired._` (Phase 3+ backfills via grep).

## Anchor IDs

Trigger headings produce GitHub-style anchors automatically. Confirm anchors match the slug from the manifest by writing trigger names exactly as upstream words them. If a trigger name contains a slash, dot, or other punctuation, the auto-anchor may differ from the manifest slug — in that case, explicitly add an HTML anchor before the heading:

```markdown
<a id="sub-counter-rollover"></a>
## Sub Counter Rollover
```

## Validation

1. File count check: manifest subcategory count + manifest platform count + 1 (top README) = total markdown files created under `Actions/Helpers/triggers/`.
2. Every subcategory file has a heading per trigger in its manifest entry.
3. `grep -r "coverage: stub" Actions/Helpers/triggers/ | wc -l` matches subcategory count.
4. Spot-check 5 random subcategory files: frontmatter parses, anchors are deterministic, upstream URLs match manifest.
5. Top README's platform table renders.

## Exit

- Dirty tree. Do not commit.
- Change summary: file count created, breakdown per platform, link to top-level catalog README so operator can spot-check.

## Next phase

`02-wiring.md` — wire pointers into existing repo so agents discover the catalog from the streamerbot-dev role and Helpers index.
