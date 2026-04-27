# Prompt 05 — Design Target Shape

## Agent

Pi (manual copy/paste by operator).

## Purpose

Synthesize all Phase A findings into a target tree shape. Drafts design doc; **operator must ratify before Phase C runs**. This is a stop-and-review checkpoint.

## Preconditions

- Phase A complete (prompts 00–04, all findings written)
- Read all four findings files + four handoffs

## Scope

Read findings, write design doc to `Projects/agent-reflow/findings/05-target-shape.md`. No tree edits.

## Out-of-scope

- No edits to `.agents/`, `.pi/`, domain folders, or root docs
- No git operations
- No prompts/10-NN generation (that's prompt 09)

## Steps

1. Read findings 00–04.

2. Draft target shape covering:
   - **Routing**: where the single source of truth lives, schema sketch
   - **Skill content location**: which knowledge stays in a central tree, which co-locates with code
   - **Per-domain agent doc convention**: filename (`CLAUDE.md` / `AGENTS.md` / both), what goes in each
   - **Workflow layer**: where workflows live, naming pattern, examples (canon-guardian, change-summary, sync, validation)
   - **Domain coverage**: how every Actions/, Apps/, Tools/, Creative/ subfolder gets a clear owner
   - **`_shared/` disposition**: promote into root, keep separate, or split
   - **`Docs/` disposition**: which files fold into domains, which stay as repo-wide concerns
   - **Naming**: flat vs hierarchical, kebab-case vs other
   - **role.md + core.md collapse**: how the redundancy resolves

3. For each design decision, document:
   - **Decision**
   - **Rationale** (which finding it addresses)
   - **Alternatives considered**
   - **Tradeoff**

4. Document open decisions where operator must choose (filename convention, workflow location, etc.). List as numbered questions.

5. Write `findings/05-target-shape.md` with a clear "Operator Decisions Needed" section at top.

## Validator / Acceptance

**Hard stop after this prompt.** Operator reads the design doc, marks each decision as ratified / changed / deferred, and responds before prompt 06 runs. Operator may rewrite parts of the doc directly.

## Handoff

Per template. The handoff must list every operator decision as `OPEN` until the operator returns and ratifies. After ratification, operator updates the handoff `Open questions` section to `RESOLVED` with the decision recorded.
