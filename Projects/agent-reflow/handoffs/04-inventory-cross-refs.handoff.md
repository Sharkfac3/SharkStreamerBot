# Prompt 04 handoff — inventory-cross-refs

Date: 2026-04-26
Agent: pi

## State changes

- Rebuilt `Projects/agent-reflow/findings/04-cross-refs.md`.
- Created `Projects/agent-reflow/handoffs/04-inventory-cross-refs.handoff.md`.
- Did not edit `WORKING.md` because the prompt was scoped as read-only except for findings/handoff output and an existing active `pi` row was present for prompt 05.

## Findings appended

- `findings/04-cross-refs.md`: cross-reference graph for in-scope agent docs, including broken links, zero-inbound files, top inbound hubs, mutual-reference pairs, candidate linear chains, and a full adjacency list.

## Key findings

- In-scope files: 144.
- Total resolved doc/path references recorded: 702.
- Broken doc/path references: 256.
- Orphans / zero inbound references: 14.
- Mutual-reference pairs: 6.
- Candidate linear chains listed: 3.
- Adjacency validator: every in-scope file appears in the adjacency list.

## Notable lost progressive disclosure candidates

The report flags all zero-inbound files whose contents look like routing, protocol, workflow, role, or skill material. Important examples include:

- `.agents/_shared/conventions.md`
- `.agents/_shared/coordination.md`
- `.agents/_shared/info-service-protocol.md`
- `.agents/_shared/mixitup-api.md`
- `.agents/roles/app-dev/skills/stream-interactions/lotat-rendering.md`
- `.agents/roles/app-dev/skills/stream-interactions/squad-rendering.md`
- `.agents/roles/content-repurposer/context/pipeline-dev-notes.md`
- `.agents/roles/streamerbot-dev/context/patterns.md`
- `.agents/roles/streamerbot-dev/skills/overlay-integration.md`
- `retired Pi skill mirror/README.md`

## Methodology notes / caveats

- Backtick extraction was intentionally filtered to doc-like paths; commands, API routes, package names, globs, obvious code/runtime assets, and template placeholders were excluded from broken-link findings where possible.
- Existing references to non-in-scope docs are recorded as outbound references but do not count as inbound for the in-scope graph.
- Many broken links are repeated patterns, especially missing leading dots such as `agents/...` / `pi/...`, relative links that should probably be repo-root links, and `/core.md`-style skill-relative shorthand.
- Paths with spaces inside backtick mentions may need a manual spot-check if the operator cares about exact non-agent doc targets; the cross-ref report is optimized for agent-doc graph shape, not every repository doc path.

## Validator status

- Last run: Python inventory script over `.agents/**/*.md`, `retired Pi skill mirror/**/*.md`, root `AGENTS.md`/`CLAUDE.md`/`WORKING.md`/`README.md`, `Docs/**/*.md`, and per-domain `README.md` files under `Actions/`, `Apps/`, `Tools/`, and `Creative/` that reference agent docs.
- Exit code: 0.
- Sanity check: adjacency headings = 144; in-scope files = 144; match = true.

## Open questions / blockers

- Decide whether references like `agents/_shared/project.md` and `pi/skills/README.md` should be treated as broken (current report) or normalized to `.agents/...` / `.pi/...` in future validators.
- Decide whether `/core.md` and `/characters/_index.md` shorthand in skill indexes is an accepted convention or should be rewritten as explicit relative paths.
- Decide whether zero-inbound `_shared` files should receive explicit links from `.agents/ENTRY.md` beyond the current shared-context prose.

## Next prompt entry point

- Read `Projects/agent-reflow/findings/04-cross-refs.md`.
- Use the broken-link patterns and lost progressive-disclosure candidates when ratifying or revising the prompt 05 target shape.
