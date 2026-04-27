# Prompt 06 — Design Naming Convention

## Agent

Pi (manual copy/paste by operator).

## Purpose

Lock the naming rules so Phase C foundation + Phase E migrations apply them consistently. Builds on operator-ratified target shape from prompt 05.

## Preconditions

- Prompt 05 done AND operator ratified target shape (handoff `Open questions` all `RESOLVED`)
- Read findings/05-target-shape.md and 05 handoff

## Scope

Writes `Projects/agent-reflow/findings/06-naming-convention.md`. No tree edits.

## Out-of-scope

- No edits to existing files
- No git operations

## Steps

1. Define naming rules for:
   - **Skill identifiers** (canonical names referenced by manifest): pattern, allowed characters, prefix rules
   - **Skill folder names** (filesystem): same as identifier or different?
   - **Skill entry file**: `_index.md`, `README.md`, `SKILL.md`, or other; pick one
   - **Co-located agent doc filename**: `CLAUDE.md`, `AGENTS.md`, both, or shared format
   - **Workflow names**: distinct prefix from skills? (e.g. `wf-canon-guardian`)
   - **Manifest IDs**: match folder names exactly, or schema-defined
   - **Cross-references**: relative paths, repo-rooted, or symbolic IDs resolved by validator

2. Define link conventions:
   - Use markdown links (`[text](path)`) vs backtick mentions
   - Relative or absolute paths
   - Whether links to validators / scripts use a different convention

3. Define heading/section conventions inside skill files:
   - Required sections (e.g. Purpose, When to Activate, Out of Scope, Chains To)
   - Optional sections
   - Section ordering

4. Define frontmatter convention:
   - Which fields are required (`name`, `description`, `type`, etc.)
   - Schema for description field (length, style)

5. Document migration rules: how existing names map to new names. Will inform prompt 09 (generator).

## Validator / Acceptance

Operator review. Once ratified, these rules become inputs to prompts 07–09.

## Handoff

Per template. Include final mapping table: every current `.agents/roles/<role>/skills/<feature>/` and every `retired Pi skill mirror/<wrapper>/` → proposed new identifier and location.
