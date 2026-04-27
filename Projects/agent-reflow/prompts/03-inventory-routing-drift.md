# Prompt 03 — Inventory Routing Drift

## Agent

Pi (manual copy/paste by operator).

## Purpose

Compare every routing-table-like document and report drift. Establishes the size of the duplication problem and pinpoints which sources are stale.

## Preconditions

- Prompts 00, 01 done; their findings exist.
- Read both prior handoffs first.

## Scope

Read-only. Writes `Projects/agent-reflow/findings/03-routing-drift.md`.

Sources to compare:
- `legacy v1 routing manifest (retired)` (declared canonical)
- `.agents/ENTRY.md` (Roles table)
- `AGENTS.md` (Quick Role Routing — has `<!-- GENERATED -->` block)
- `retired Pi skill mirror/README.md` (Roles table + Routing Table)
- `Tools/StreamerBot/Validation/retired-routing-doc-sync.py` (what it actually syncs)

## Out-of-scope

- No edits to any of these files
- No re-run of the sync script
- No git operations

## Steps

1. Extract role list from each source.
2. Extract activation/when-to-use text from each source.
3. Extract sub-skill / canonical_children list from manifest + `retired Pi skill mirror/README.md`.
4. Build a comparison matrix: rows = roles, columns = each source, cells = activation text or "absent".
5. Flag every cell where activation text differs across sources.
6. Read `retired-routing-doc-sync.py` and document: which tables it currently syncs from manifest, which sources it leaves manual.
7. Document the GENERATED markers (where they exist, what content they wrap).
8. Identify: routing data that lives in only one source vs. duplicated across N sources.

## Validator / Acceptance

- Comparison matrix complete for every role in any source
- Drift cells flagged (highlighted or marked `DRIFT`)
- Sync coverage gap explicit ("sync script handles X but not Y, Z")

## Handoff

Per template. Note: any drift severe enough that the canonical answer is genuinely ambiguous (i.e. manifest disagrees with everyone else) — flag for operator.
