# Prompt 05 handoff — design-target-shape

Date: 2026-04-26
Agent: pi

## State changes
- Wrote `Projects/agent-reflow/findings/05-target-shape.md` as the Phase A synthesis / target-shape design draft.
- Updated `Projects/agent-reflow/handoffs/05-design-target-shape.handoff.md`.
- No edits were made to `.agents/`, `.pi/`, domain folders, root docs, or git state.

## Findings appended
- `findings/05-target-shape.md`: draft target tree shape covering routing single-source authority, manifest v2 schema sketch, skill content location, per-domain `AGENTS.md` convention, workflow layer, domain coverage map for `Actions/`, `Apps/`, `Tools/`, and `Creative/`, `_shared/` disposition, `Docs/` disposition, naming, `role.md` + `core.md` collapse, Pi mirror disposition, validator expectations, and migration principles.

## Inputs read
- `Projects/agent-reflow/findings/00-current-tree.md`
- `Projects/agent-reflow/findings/01-pi-mirror.md`
- `Projects/agent-reflow/findings/02-domain-coverage.md`
- `Projects/agent-reflow/findings/03-routing-drift.md`
- `Projects/agent-reflow/findings/04-cross-refs.md`
- `Projects/agent-reflow/handoffs/00-inventory-agents-tree.handoff.md`
- `Projects/agent-reflow/handoffs/01-inventory-pi-mirror.handoff.md`
- `Projects/agent-reflow/handoffs/02-inventory-domains.handoff.md`
- `Projects/agent-reflow/handoffs/03-inventory-routing-drift.handoff.md`
- `Projects/agent-reflow/handoffs/04-inventory-cross-refs.handoff.md`

## Assumptions for prompt 06
- **Prompt 06 may proceed.** The operator resolved the hard stop by accepting all draft recommendations in `findings/05-target-shape.md`.
- Domain folder names with spaces are preserved in the target shape; manifest IDs normalize them without requiring folder renames.
- `retired Pi skill mirror/` remains in place until a later explicit cutover after manifest/root-doc discovery is available.
- Later prompts should treat `Projects/agent-reflow/findings/05-target-shape.md` as ratified, not draft.

## Validator status
- Last run: `test -f Projects/agent-reflow/findings/05-target-shape.md && test -f Projects/agent-reflow/handoffs/05-design-target-shape.handoff.md && grep -q "Operator Decisions Resolved" Projects/agent-reflow/findings/05-target-shape.md && grep -q "Prompt 06 may proceed" Projects/agent-reflow/handoffs/05-design-target-shape.handoff.md && echo ok`
- Exit code: 0
- Key output: `ok`

## Open questions / blockers
- RESOLVED — Manifest authority/location: keep `legacy v1 routing manifest (retired)` as manifest v2 single source of truth and upgrade the schema.
- RESOLVED — Per-domain agent doc filename: use `AGENTS.md`; root `CLAUDE.md` remains compatibility pointer only.
- RESOLVED — Per-domain doc coverage depth: every first-level `Actions/`, `Apps/`, `Tools/`, and `Creative/` subfolder gets either local `AGENTS.md` or explicit manifest `coveredBy`.
- RESOLVED — Workflow layer location: reusable procedures live under `.agents/workflows/<workflow-id>.md`.
- RESOLVED — Workflow taxonomy: use one shared `.agents/workflows/canon-guardian.md` with role-specific sections.
- RESOLVED — Skill content location: move domain/runtime knowledge to domain-local docs; keep central roles concise; keep reusable procedures in workflows.
- RESOLVED — App-dev taxonomy: app-specific `Apps/<app>/AGENTS.md` docs own runtime/setup/validation; `stream-interactions` concepts link from `Apps/stream-overlay/AGENTS.md`.
- RESOLVED — Twitch granularity: one `streamerbot-dev` owner family with per-folder local docs; no four new top-level Pi-style skill wrappers.
- RESOLVED — `Tools/LotAT/` owner: `lotat-tech` primary; `ops` secondary for validator/tool execution mechanics.
- RESOLVED — `Creative/Marketing/` owner: `brand-steward` primary; `content-repurposer` secondary for platform packaging and short-form repurposing.
- RESOLVED — `_shared/` disposition: split by owner/audience; keep only repo-wide agent context central and move owner-specific protocol/API knowledge to domains.
- RESOLVED — `Docs/` disposition: reduce `Docs/` to repo-wide human docs; move app/domain implementation plans beside their domains.
- RESOLVED — Naming convention: hierarchical content paths, kebab-case manifest/workflow IDs, existing folder names unless later rename is explicitly worth the churn.
- RESOLVED — `role.md` + `core.md` collapse: collapse to one concise role overview per role after migrating detailed knowledge to domain docs/workflows.
- RESOLVED — Pi mirror disposition: treat `retired Pi skill mirror/` as transitional compatibility; do not hand-maintain new wrappers; delete only after manifest/root-doc discovery replaces it.
- RESOLVED — Link convention for validators: require explicit real paths (`.agents/...`, `.pi/...`) and treat unnormalized paths as broken.
- RESOLVED — Generated markers: require generated-block markers around every generated routing table.

## Next prompt entry point
- Proceed per `Projects/agent-reflow/prompts/06-design-naming-convention.md`.
- Load `Projects/agent-reflow/findings/05-target-shape.md` first and treat all target-shape recommendations as ratified.
- Continue to avoid tree edits outside the prompt scope.
