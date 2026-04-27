# Prompt 10-10 handoff — roles-shared-root-entrypoints

Date: 2026-04-26
Agent: pi

## State changes

- Collapsed the nine role overview files into concise prompt-06 section order with required frontmatter:
  - `.agents/roles/streamerbot-dev/role.md`
  - `.agents/roles/lotat-tech/role.md`
  - `.agents/roles/lotat-writer/role.md`
  - `.agents/roles/art-director/role.md`
  - `.agents/roles/brand-steward/role.md`
  - `.agents/roles/content-repurposer/role.md`
  - `.agents/roles/app-dev/role.md`
  - `.agents/roles/product-dev/role.md`
  - `.agents/roles/ops/role.md`
- Replaced role `skills/core.md` files with compatibility pointers to the collapsed role overview and local domain guides.
- Rewrote `.agents/_shared/project.md` and `.agents/_shared/conventions.md` as repo-wide shared context with required frontmatter.
- Replaced `.agents/_shared/coordination.md` with a compatibility pointer to `.agents/workflows/coordination.md`.
- Rewrote root `AGENTS.md` with required frontmatter and an updated manifest-backed quick routing generated block.
- Rewrote `.agents/ENTRY.md` with required frontmatter, generated role-table markers, and local-domain/workflow routing.
- Replaced root `CLAUDE.md` with a compatibility pointer only.
- Created `Docs/Architecture/AGENTS.md` for the `docs-architecture` route.
- Updated `.agents/manifest.json` to mark `docs-architecture` as covered/active and `coordination` as active now that their docs exist.
- Wrote validator output to `Projects/agent-reflow/findings/10-10-validator.failures.txt`.

## Routing-surface diff summary

| Surface | Before | After |
|---|---|---|
| Role files | Mixed role/core split; no target frontmatter; old load-order language pointed to central skill files. | Single concise role overview per role with purpose, ownership, activation, routes, workflows, chain rules, and living-context notes. |
| Role core files | Held broad domain/runtime details and stale path references. | Compatibility pointers to `../role.md` plus local route guides; no longer active startup targets. |
| `.agents/ENTRY.md` | Manual universal entry, unmarked roles table, shared API/protocol links. | Frontmatter, generated-marked roles table, workflow/shared/local-domain routing, no shared API/protocol duplication. |
| `AGENTS.md` | Root guide had stale generated quick-routing block target paths and no frontmatter. | Frontmatter, quick routing now points to role overview files, and root guidance points to manifest/local guides/workflows. |
| `CLAUDE.md` | Separate short Claude-specific rules. | Compatibility pointer only to `AGENTS.md` and `.agents/ENTRY.md`. |
| `.agents/_shared/project.md` | Repo-wide context plus some path mentions that validator treated as broken. | Repo-wide project context only, normalized links, required frontmatter. |
| `.agents/_shared/conventions.md` | Mixed git/file/agent conventions with broken illustrative backtick paths. | Repo-wide conventions only, workflow pointer, normalized links, required frontmatter. |
| `.agents/_shared/coordination.md` | Duplicated WORKING protocol. | Short compatibility pointer to `.agents/workflows/coordination.md`. |
| `Docs/Architecture/` | Manifest-declared route lacked local guide. | `Docs/Architecture/AGENTS.md` covers repo architecture docs and links `repo-structure.md`. |

## Manifest / generated-marker notes

- `.agents/manifest.json` already declared `root-agent-doc`, `agent-entry`, the nine role overview paths, shared project/conventions, and `docs-architecture`.
- This prompt changed only statuses for already-created docs/routes:
  - `docs-architecture`: skill active, domain covered, co-location active.
  - `coordination`: skill/workflow/co-location active.
- Root `AGENTS.md` retains the visible `GENERATED:agents-quick-role-routing` block.
- `.agents/ENTRY.md` now has visible `GENERATED:agents-roles` markers around its manifest-derived roles table.
- `retired Pi skill mirror/README.md` was intentionally not updated per prompt scope; drift remains for prompt 10-11/cutover.

## Validator status

Command run:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-10-validator.failures.txt
```

Exit code: `1`

Summary output:

```text
Agent tree validation summary
| Check | Checked | Failures | Status |
|---|---:|---:|---|
| schema | 1 | 0 | PASS |
| folder-coverage | 152 | 0 | PASS |
| link-integrity | 125 | 277 | FAIL |
| drift | 3 | 1 | FAIL |
| stub-presence | 49 | 3 | FAIL |
| orphan | 104 | 35 | FAIL |
| naming | 108 | 0 | PASS |

Total failures: 316
Failure report: /mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot/Projects/agent-reflow/findings/10-10-validator.failures.txt
```

## Prompt-specific acceptance deltas

Cleared:

- `folder-coverage` is now fully clear, including `docs-architecture`.
- `stub-presence` frontmatter failures are clear for all nine role files.
- `stub-presence` frontmatter failures are clear for `AGENTS.md`, `.agents/ENTRY.md`, `.agents/_shared/project.md`, `.agents/_shared/conventions.md`, and `Docs/Architecture/AGENTS.md`.
- `drift` failures are clear for `AGENTS.md` and `.agents/ENTRY.md`.
- No new link-integrity failures remain for root `AGENTS.md`, `CLAUDE.md`, `.agents/ENTRY.md`, `.agents/_shared/project.md`, `.agents/_shared/conventions.md`, `.agents/_shared/coordination.md`, `Docs/Architecture/AGENTS.md`, the nine role files, or the rewritten role `skills/core.md` compatibility pointers.

Remaining expected failures:

- `retired Pi skill mirror/README.md` drift remains intentionally for prompt 10-11.
- Three Pi meta wrapper `stub-presence` failures remain intentionally out of scope:
  - `retired Pi skill mirror/meta/SKILL.md`
  - `retired Pi skill mirror/meta-agents-navigate/SKILL.md`
  - `retired Pi skill mirror/meta-agents-update/SKILL.md`
- Link-integrity failures remain in old central skill/source files and Pi meta wrappers that are scheduled for later cleanup/cutover.
- Orphan failures remain for old central skill files, templates, and context files that were not deleted in this prompt.

## Old content intentionally not deleted

- No `retired Pi skill mirror/` wrapper cleanup or deletion was performed.
- No old central skill tree deletion was performed.
- No domain `AGENTS.md` files outside `Docs/Architecture/AGENTS.md` were created by this prompt.
- Old central sub-skill files remain as migration sources until the cleanup prompt.

## Open questions / blockers

- The role core compatibility pointers are still reported as orphans because they are not manifest-declared entrypoints. That is expected until the old central skill cleanup prompt retires or declares compatibility files.
- `CLAUDE.md` now has frontmatter even though the current validator does not enforce it as a manifest skill entry.
- If prompt 10-11 decides to update `retired Pi skill mirror/README.md`, it should derive the `## Roles` table from `.agents/manifest.json` so the remaining drift failure clears.
