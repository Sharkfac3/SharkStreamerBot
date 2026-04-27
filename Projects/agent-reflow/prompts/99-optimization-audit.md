# Prompt 99 — Optimization Audit

## Agent

Pi (manual copy/paste by operator).

## Purpose

Review-only post-migration audit. Surfaces optimization opportunities discovered after the new tree is in place. Outputs **recommendations only** — operator triages each recommendation into a follow-up prompt if action is desired.

This is **not** a line-count split pass. Focus: concept boundaries, deduplication, cross-reference clustering, naming consistency, skill granularity, stale content, reference integrity.

## Preconditions

- All Phase E migrations complete
- Cutover prompt complete (`retired Pi skill mirror/` deleted, legacy artifacts removed)
- Validator green
- Read final state of: manifest v2, all migrated skills, all co-located agent docs, root docs

## Scope

Read-only audit. Writes one report file: `Projects/agent-reflow/findings/99-audit.md`. Makes no edits to tree.

## Out-of-scope

- **No automatic fixes.** Audit produces a list; operator decides what to act on
- No git operations
- No new prompts (operator decides whether to spawn `100-*.md` follow-ups)

## Steps

Run each audit category. For each, output a section in the report with: finding, evidence (file paths, examples), severity (low / medium / high), recommended action.

### 1. Concept-Boundary Review

Goal: find files that bundle multiple distinct concepts that a junior reader could not separate without reading the whole file.

- For each `.md` over a non-trivial size, scan H2 sections
- Flag files where H2 sections cover unrelated topics (e.g. `Actions/HELPER-SNIPPETS.md` mixing mini-game-lock + Mix-It-Up + OBS scenes + chat input)
- For each flagged file, propose conceptual split points (by section, not line count)

### 2. Deduplication Scan

Goal: find near-identical text across multiple files.

- Look for repeated business-context blurbs, repeated load instructions, repeated rules statements
- For each duplicate cluster, propose a single source location + reference pattern from the others

### 3. Cross-Reference Clustering

Goal: find file pairs that always reference each other → candidates for merge or co-location.

- Identify mutual-reference pairs (A links B, B links A, no other strong links)
- Identify reference triangles
- Recommend: merge, extract shared content, or co-locate in same folder

### 4. Naming Consistency

Goal: enforce conventions from prompt 06.

- Skill ids match pattern
- File extensions consistent
- Markdown link text matches target file purpose
- Casing consistent

### 5. Skill Granularity

Goal: find skills that are too coarse (covering multiple distinct concepts) or too fine (always loaded with a sibling).

- Coarse: large skill file with 4+ distinct concepts → split candidate
- Fine: small skill never referenced independently → merge candidate

### 6. Stale Content

Goal: surface content that is no longer load-bearing.

- TODO / FIXME / REMOVED comments
- Date-stamped notes older than 6 months
- "Phase 1" / "v1" / migration-era markers that should now be gone
- References to deleted files or old skill names

### 7. Reference Integrity (final pass)

Goal: confirm post-migration tree has no rot.

- Every internal link resolves
- No orphan files (every file has at least one inbound reference, except documented entry points)
- No circular reference loops in load chains

### 8. Workflow vs Domain Knowledge Separation

Goal: confirm Phase E correctly extracted workflows from domain knowledge.

- Workflow files contain compositional logic, not domain facts
- Domain skill files contain domain facts, not workflow steps
- Flag bleed-through

### 9. Co-location Coverage

Goal: confirm every domain folder has appropriate agent doc co-location.

- Every domain subfolder has the doc convention from prompt 06
- No domain folder has stale or empty agent doc
- No agent doc points to skills that no longer exist

## Validator / Acceptance

- `findings/99-audit.md` written
- Every audit category has a section (even if "no findings")
- Recommendations include severity + concrete next step

## Handoff

Per template. Include:
- Total recommendations by severity
- Operator triage instructions: "for each recommendation, decide: act now (spawn follow-up prompt 100+), defer to backlog, or dismiss"
- Disposition reminder: after operator finishes triage and any follow-ups, run final disposition step (delete `Projects/agent-reflow/` or move to `Docs/Archive/`).
