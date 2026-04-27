# Prompt 100 — Manifest Status Normalization

## Agent

Pi (manual copy/paste by operator).

## Purpose

Align `.agents/manifest.json` `status` fields with active state of co-located `AGENTS.md` docs. Audit finding 4.1 + 9.1: manifest still marks several routes `planned` while their `AGENTS.md` files declare `status: active` and contain real migrated guidance. Drift weakens manifest semantics.

Bundles audit findings: 4.1, 9.1, parts of 6.3 (manifest migration ledger disposition).

## Preconditions

- Cutover (`10-13-cutover-pi-mirror`) complete
- Audit (`99-optimization-audit`) complete; `Projects/agent-reflow/findings/99-audit.md` exists
- Validator green: `python3 Tools/AgentTree/validate.py` returns 0 failures
- Read `Projects/agent-reflow/findings/99-audit.md` finding 4.1, 9.1, 6.3 first

## Scope

Edits:
- `.agents/manifest.json` — status fields for skills, domains, co_locations
- (Optional) split historical migration ledger out of active manifest if operator decides

Reads:
- All `AGENTS.md` files listed in finding 4.1
- `.agents/manifest.json` `aliases` + `migration.v1_entries` sections (per finding 6.3)

## Out-of-scope

- No edits to any `AGENTS.md` content body (frontmatter status only if downgrading)
- No splits / dedup / content moves (other prompts own those)
- No `.pi/skills/` work (already deleted)
- No git operations

## Steps

1. List all manifest entries (skills, domains, co_locations) where `status: planned`.

2. For each entry, read corresponding co-located `AGENTS.md` (or referenced file). Decide per entry:
   - **Promote to active** — file has real content, route is in use → set manifest `status: active`
   - **Downgrade doc** — manifest correct, file is placeholder → leave manifest `planned`, set AGENTS.md frontmatter `status: planned` to match
   - **Other status** — `deprecated`, `template`, etc. per existing schema

3. Confirmed-active candidates from audit finding 4.1:
   - `Actions/Destroyer/AGENTS.md`
   - `Actions/Intros/AGENTS.md`
   - `Actions/Rest Focus Loop/AGENTS.md`
   - `Actions/Temporary/AGENTS.md`
   - `Actions/XJ Drivethrough/AGENTS.md`
   - `Apps/info-service/AGENTS.md`
   - `Apps/production-manager/AGENTS.md`
   - `Tools/LotAT/AGENTS.md`

4. Update `.agents/manifest.json` accordingly.

5. **Migration ledger decision** (audit finding 6.3): inspect `.agents/manifest.json` `aliases` + `migration.v1_entries` sections. Operator-facing decision in this prompt:
   - **Keep in active manifest** — if any tooling still resolves v1 ids
   - **Move to archive** — relocate to `Projects/agent-reflow/findings/07-manifest-v2.md` appendix or new `Projects/agent-reflow/findings/100-migration-ledger-archive.md`; remove from active manifest
   
   If unclear which tooling depends on aliases, default: **keep**, document rationale in handoff. A later audit can revisit.

6. Run `python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/100-validator.failures.txt`. Expect 0 failures (status changes should not break schema, coverage, or drift checks).

7. Write handoff.

## Validator / Acceptance

- `python3 Tools/AgentTree/validate.py` exits 0
- No manifest entry remains `status: planned` while its target `AGENTS.md` has substantive content (>30 lines of migrated guidance)
- Migration ledger disposition recorded in handoff with rationale

## Handoff

Write `Projects/agent-reflow/handoffs/100-manifest-status-normalization.handoff.md` per template. Include:
- Per-entry decision table (entry → old status → new status → rationale)
- Migration ledger decision + rationale
- Final validator output
