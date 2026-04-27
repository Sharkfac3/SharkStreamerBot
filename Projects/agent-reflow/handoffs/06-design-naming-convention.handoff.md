# Prompt 06 handoff — design-naming-convention

Date: 2026-04-26
Agent: pi

## State changes

- Wrote and ratified `Projects/agent-reflow/findings/06-naming-convention.md`.
- Resolved the operator review items under delegated authority from: "do what you think is best to handle the review and unblock prompt 7."
- No edits were made to `.agents/`, `.pi/`, domain folders, root docs, or git state.

## Findings appended

- `findings/06-naming-convention.md`: ratified naming convention for manifest v2 and later migrations, including:
  - identifier namespaces and allowed characters
  - skill/domain/workflow/alias naming rules
  - canonical entry file conventions
  - `AGENTS.md` local agent doc convention
  - link conventions
  - heading/section templates
  - frontmatter schema
  - migration rules
  - final mapping tables for current `.agents/roles/<role>/skills/<feature>/` folders and `retired Pi skill mirror/<wrapper>/` folders

## Inputs read

- `Projects/agent-reflow/findings/05-target-shape.md`
- `Projects/agent-reflow/handoffs/05-design-target-shape.handoff.md`
- `Projects/agent-reflow/findings/00-current-tree.md`
- `Projects/agent-reflow/findings/01-pi-mirror.md`
- Current filesystem inventory of `.agents/roles/*/skills/*/`
- Current filesystem inventory of `retired Pi skill mirror/*/`

## Decisions resolved for prompt 07

- **RESOLVED — Workflow IDs:** no `wf-` prefix; workflow IDs are plain kebab-case and are distinguished by the manifest `workflows` namespace.
- **RESOLVED — Co-located agent docs:** use `AGENTS.md` as the only canonical local/domain agent doc filename.
- **RESOLVED — Entry files:** `_index.md` and `retired Pi skill mirror/<wrapper>/SKILL.md` are deprecated compatibility/migration sources only.
- **RESOLVED — Route ID normalization:** split CamelCase/PascalCase folder names into readable kebab-case route IDs.
  - `Tools/ArtPipeline/` → `tools-art-pipeline`
  - `Tools/ContentPipeline/` → `tools-content-pipeline`
- **RESOLVED — Prompt 07 unblock:** `findings/06-naming-convention.md` is ratified and may be used as an input to manifest schema v2.

## Validator status

Last run:

```bash
test -f Projects/agent-reflow/findings/06-naming-convention.md \
  && grep -q "Final Mapping Table" Projects/agent-reflow/findings/06-naming-convention.md \
  && grep -q "Status: **RATIFIED" Projects/agent-reflow/findings/06-naming-convention.md \
  && grep -q "Prompt 07 unblock" Projects/agent-reflow/findings/06-naming-convention.md \
  && test -f Projects/agent-reflow/handoffs/06-design-naming-convention.handoff.md \
  && echo ok
```

Exit code: 0

Key output: `ok`

## Open questions / blockers

- None. Prompt 07 may proceed.

## Next prompt entry point

Proceed per `Projects/agent-reflow/prompts/07-manifest-schema-v2.md`.

Use these ratified inputs:

1. `Projects/agent-reflow/findings/05-target-shape.md`
2. `Projects/agent-reflow/findings/06-naming-convention.md`
3. `Projects/agent-reflow/handoffs/06-design-naming-convention.handoff.md`
