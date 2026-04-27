# Prompt 07 — Manifest Schema v2

## Agent

Pi (manual copy/paste by operator).

## Purpose

Build the new manifest format. Single source of truth for routing, skill identifiers, ownership, cross-references. Replaces `legacy v1 routing manifest (retired)` (which becomes redundant once schema v2 lands).

## Preconditions

- Prompts 05, 06 done and ratified
- Read findings/05-target-shape.md and findings/06-naming-convention.md

## Scope

Creates:
- New manifest file at the location decided in prompt 05 (e.g. `.agents/manifest.json` or repo root, per design)
- JSON Schema file describing the manifest structure
- (Optional, if design specifies) generator scripts that build derived markdown tables from the manifest

Reads existing `legacy v1 routing manifest (retired)` for content migration.

## Out-of-scope

- Does not delete old `legacy v1 routing manifest (retired)` yet (Phase F cutover handles that)
- No edits to `.pi/`, `.agents/roles/`, domain folders
- No git operations

## Steps

1. Define schema sections (informed by prompt 06):
   - **`skills`**: each skill's id, name, description, location (file path), domain owner, type (domain knowledge / workflow / shared / meta), depends-on, chains-to
   - **`workflows`**: distinct workflow definitions with skill compositions
   - **`domains`**: declared domain folders and their owning skills
   - **`co_locations`**: which agent docs live where (`Actions/Squad/CLAUDE.md` → skill X)
   - **`aliases`**: legacy name → new id (for transitional reference resolution)

2. Write JSON Schema (Draft 2020-12 or similar) that validates the manifest format.

3. Write the actual v2 manifest by migrating content from `legacy v1 routing manifest (retired)` + findings/02-domain-coverage.md + findings/06-naming-convention.md.

4. Validate the migrated manifest against the schema.

5. Write `Projects/agent-reflow/findings/07-manifest-v2.md` documenting:
   - Final schema location
   - Final manifest location
   - Key differences from v1 manifest
   - Migration mapping table (every v1 entry → v2 entry)

## Validator / Acceptance

- New manifest validates against schema (use any standard JSON Schema validator)
- Every entry in `legacy v1 routing manifest (retired)` accounted for in v2 manifest (or explicitly marked "removed: <reason>")

## Handoff

Per template. Note: until prompt 08 ships the validator tool, manifest v2 has no automated drift protection. Old manifest still in repo as reference.
