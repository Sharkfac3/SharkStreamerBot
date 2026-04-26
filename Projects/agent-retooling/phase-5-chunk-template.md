# Phase 5 — Per-chunk migration prompt template

This file is **not run directly**. `phase-5-generator.md` clones it once per migration chunk, filling in `<CHUNK_NAME>`, `<CHUNK_FILES>`, `<CHUNK_PRE>`, `<CHUNK_POST>`.

When generated, the cloned file goes to `Projects/agent-retooling/phase-5a-chunk-<n>-<chunk-name>.md`.

---

## Template body (everything below this line is the per-chunk prompt)

# Phase 5a — Migration chunk: `<CHUNK_NAME>`

## Before you start

1. Read `Projects/agent-retooling/PLAN.md`.
2. Read `Projects/agent-retooling/STATUS.md` — confirm pre-condition chunks are checked off.
3. Read `Projects/agent-retooling/migration-map.md` and `Projects/agent-retooling/new-tree.md`.

## This chunk

- **Touches:** `<CHUNK_FILES>`
- **Pre-conditions:** `<CHUNK_PRE>`
- **Post-conditions:** `<CHUNK_POST>`

## Mission

Execute this chunk only. Do not touch files outside the chunk's scope, even if you see something that needs fixing — flag it as an open question instead.

This is the **first phase that touches the production tree**. All prior phases were read-only. Be careful.

## Steps

1. Confirm pre-conditions in STATUS.md. If not met, stop.
2. For each file in this chunk's scope, perform the action in `migration-map.md`:
   - `move` — copy content to new path with `Read` + `Write`. Do not delete the source yet (the gut-old-tree chunk handles deletions).
   - `move+rename` — same as move, but new path differs from old.
   - `merge` — read source, integrate into target file with `Edit` or `Write`.
   - `rewrite` — author new content using the stub from `new-tree.md`.
   - `delete` — only execute if this chunk is `gut-old-tree` or similar. Otherwise leave for later chunk.
3. After each file, confirm content is correct. Check frontmatter on `SKILL.md` files matches Pi format from `pi-skill-format.md`.
4. Cross-check: every `[skill: ...]` reference in any new `WORKFLOW.md` resolves to an existing `SKILL.md` (or one in a not-yet-built chunk — note these in open questions for the validator).

## Constraints

- **Hard cutover but staged.** Old files stay in place until the explicit gut-old-tree chunk runs. This chunk only adds or rewrites in the new tree (unless the chunk's purpose IS deletion).
- **No commits.** Operator commits manually.
- **Do not run smoke tests.** Phase 6 handles validation.

## When done

1. Verify all post-conditions are met by inspecting the touched files.
2. Append to `STATUS.md` under `## Notes`: `- 5a-<CHUNK_NAME> complete: <count> files written / merged / deleted.`
3. Tick your prompt's checkbox in STATUS.md.
4. If any cross-references couldn't be resolved (skill in another chunk not yet built), record under `## Open questions` in `migration-validation.md` (create the file if it doesn't exist).
5. Identify the next chunk from `migration-order.md` and run it. When all chunks done, run `phase-6a-smoke-plan.md`.
