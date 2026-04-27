# Prompt 08 — Validator

## Agent

Pi (manual copy/paste by operator).

## Purpose

Build the validator tool. Replaces / extends `Tools/StreamerBot/Validation/retired-routing-doc-sync.py`. Becomes the acceptance gate every Phase E migration prompt runs.

## Preconditions

- Prompt 07 done; manifest v2 + schema exist
- Read findings/07-manifest-v2.md

## Scope

Creates / modifies:
- New validator script (location per design — likely `Tools/AgentTree/validate.py` or similar)
- Replacement / deprecation note on `Tools/StreamerBot/Validation/retired-routing-doc-sync.py`
- Documentation for how to run the validator

## Out-of-scope

- No tree migrations (Phase E does that)
- No git operations
- Does not delete `retired-routing-doc-sync.py` yet (cutover prompt handles legacy cleanup)

## Steps

1. Implement these checks:

   **Schema check**
   - Manifest file validates against JSON Schema from prompt 07

   **Folder coverage**
   - Every domain folder listed in design has either a co-located agent doc OR an owning skill
   - Every skill in manifest has its declared file path actually exist
   - Every co-location declared in manifest exists on disk

   **Link integrity**
   - Every markdown link in agent docs resolves to a real file
   - Every backtick-wrapped path mention resolves
   - No broken outbound references

   **Drift detection**
   - All routing tables in markdown that are derived from manifest match what manifest currently declares
   - GENERATED blocks are in sync (or report which are stale)

   **Stub presence**
   - For every manifest skill, the entry file (`_index.md` or per design) exists and has required frontmatter

   **Orphan check**
   - Files in `.agents/` (or successor tree) that are not referenced by anything

   **Naming check**
   - All skill ids match the naming pattern from prompt 06
   - All file paths follow casing convention

2. Output format:
   - Exit code 0 = clean
   - Exit code 1 = validation failures
   - Stdout summary table of checks + counts
   - Stderr or report file listing each failure with file:line if applicable

3. Write run instructions in `Projects/agent-reflow/findings/08-validator.md`.

4. Run the validator against current tree state. Document expected failures (since migrations have not run yet, validator should fail loudly until Phase E lands). This baseline failure list becomes the migration to-do list.

## Validator / Acceptance

- Validator runs without crashing
- Schema check passes against the new manifest
- Every other check produces output (even if failing) — no silent skips

## Handoff

Per template. Include the baseline validator output (stdout + failure list) — this is the migration backlog Phase E must clear.
