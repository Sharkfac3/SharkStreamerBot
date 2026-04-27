# Prompt 104 — Apps Stream-Overlay Doc Split

## Agent

Pi (manual copy/paste by operator).

## Purpose

Split `Apps/stream-overlay/AGENTS.md` (305 lines) into thin agent route + subreference docs. Audit findings 1.4 + 5.1: current file mixes agent activation/routing with broker protocol details, asset system, rendering notes, build/test commands, gotchas. Skill is too coarse; reader looking for protocol detail must scan whole doc.

Bundles audit findings: 1.4, 5.1.

## Preconditions

- Prompt 103 complete (LotAT contract index done; presentation refs in `Apps/stream-overlay/AGENTS.md` already simplified to point at contract)
- Validator green
- Read `findings/99-audit.md` findings 1.4 + 5.1 first

## Scope

Reads:
- `Apps/stream-overlay/AGENTS.md` (full)
- `Apps/stream-overlay/README.md` (for cross-reference)
- Any in-tree references to `Apps/stream-overlay/AGENTS.md` (grep first)

Writes / creates:
- `Apps/stream-overlay/docs/protocol.md` — broker/protocol details
- `Apps/stream-overlay/docs/asset-system.md` — asset registry, conventions
- `Apps/stream-overlay/docs/rendering-notes.md` — overlay renderer, feature renderers, gotchas
- (Optional) `Apps/stream-overlay/docs/build-test.md` — build/test commands if substantial; otherwise keep in AGENTS.md

Edits:
- `Apps/stream-overlay/AGENTS.md` — reduce to agent activation, ownership, routing, links to docs/* subreferences
- `.agents/manifest.json` — add co_location entries for new sub-docs (type per existing schema; likely `domain-reference` or extension)
- Any doc that references specific sections of the old `AGENTS.md` (per grep result) — update links to point at appropriate new sub-doc

## Out-of-scope

- No runtime code edits (`Apps/stream-overlay/src/*.ts`)
- No `package.json` / build config changes
- No `Apps/stream-overlay/README.md` rewrite (only minor link updates if needed)
- No git operations

## Steps

1. Grep for all references to `Apps/stream-overlay/AGENTS.md` across `Actions/`, `Apps/`, `Tools/`, `Creative/`, `Docs/`, `.agents/`, root markdown. Capture exact link text + section anchors used. These need updating after split.

2. Read `Apps/stream-overlay/AGENTS.md` end-to-end. Map H2/H3 sections to target sub-docs:
   - **Agent activation, ownership, routing, primary/secondary owners, when-to-activate, boundaries** → stay in `AGENTS.md`
   - **Broker protocol, message shapes, channels** → `docs/protocol.md`
   - **Asset registry, conventions, asset paths** → `docs/asset-system.md`
   - **Renderer notes, feature renderers, gotchas, OBS source caveats** → `docs/rendering-notes.md`
   - **Build/test commands, dev workflow** → keep in `AGENTS.md` if short; split to `docs/build-test.md` if substantial

3. Create `Apps/stream-overlay/docs/` directory.

4. Write each sub-doc with required frontmatter per `.agents/_shared/conventions.md`. Each sub-doc gets:
   - `id` per naming convention
   - `type` (likely `domain-reference`)
   - `status: active`
   - `description`
   - `owner: app-dev`

5. Rewrite `Apps/stream-overlay/AGENTS.md` to thin route: keep frontmatter + agent activation + ownership + workflow links + links to each new sub-doc with one-line description.

6. Add manifest entries for new sub-docs in `.agents/manifest.json` co_locations.

7. Update inbound references identified in step 1 to point at correct sub-doc.

8. Run `python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/104-validator.failures.txt`. Expect 0 failures.

9. Write handoff.

## Validator / Acceptance

- `python3 Tools/AgentTree/validate.py` exits 0
- `Apps/stream-overlay/AGENTS.md` ≤120 lines (down from 305)
- Three sub-docs (`protocol.md`, `asset-system.md`, `rendering-notes.md`) exist under `Apps/stream-overlay/docs/`
- Manifest declares each sub-doc as a co-location
- All inbound references resolve to correct sub-doc (no broken anchor links)

## Handoff

Per template. Include:
- Section-to-file mapping table
- List of inbound references updated
- Whether `docs/build-test.md` was split out or kept inline (with rationale)
- Final validator output
