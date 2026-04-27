# Prompt 103 — LotAT Contract Index

## Agent

Pi (manual copy/paste by operator).

## Purpose

Extract single canonical LotAT contract index. Audit finding 3.1: `Actions/LotAT/AGENTS.md`, `Tools/LotAT/AGENTS.md`, `Creative/WorldBuilding/AGENTS.md`, `Apps/stream-overlay/AGENTS.md` form a tight reference triangle with shared contract facts. Each domain currently restates parts of the contract; agents reading any one file get partial truth.

Bundles audit findings: 3.1, 1.3 (LotAT/README split if it falls naturally out of contract index).

## Preconditions

- Prompt 100 complete (manifest statuses aligned)
- Prompt 101 complete (stale cleanup done; this prompt should not have to dodge migration-era language)
- Validator green
- Read `findings/99-audit.md` findings 3.1 and 1.3 first

## Scope

Reads:
- `Actions/LotAT/AGENTS.md`
- `Actions/LotAT/README.md` (507 lines, audit finding 1.3)
- `Tools/LotAT/AGENTS.md`
- `Creative/WorldBuilding/AGENTS.md`
- `Apps/stream-overlay/AGENTS.md` (LotAT presentation handoff sections)

Writes:
- One new contract index file. Operator-decided location during prompt — defaults:
  - `Docs/Architecture/lotat-contract.md` (preferred — repo-wide architecture concern)
  - or `.agents/_shared/lotat-contract.md` (if contract is agent-only)
  - or `Creative/WorldBuilding/Franchises/StarshipShamples.md` extension (less likely; that doc is franchise-specific)
- Manifest entry for the new index in `.agents/manifest.json`

Edits:
- The four domain `AGENTS.md` files above — replace contract-restating sections with link to index, keep only domain-specific responsibilities
- (Optional, finding 1.3) Split `Actions/LotAT/README.md` 507 lines into:
  - `Actions/LotAT/README.md` — overview only
  - `Actions/LotAT/runtime-contract.md` — globals, timers, commands, story file contract, v1 boundaries
  - `Actions/LotAT/operator-setup.md` — Streamer.bot timers/triggers, checklist
  - `Actions/LotAT/implementation-map.md` — action inventory, internal wiring

## Out-of-scope

- No runtime code edits (`Actions/LotAT/*.cs`)
- No story JSON edits
- No worldbuilding lore content edits (only structural refs)
- No git operations

## Steps

1. Read all five source docs. Identify the **shared contract surface**: facts that are true across runtime, tooling, presentation, and worldbuilding. Examples (verify in source):
   - Story file JSON schema fields
   - Required runtime globals
   - Timer names + intervals
   - Command names
   - Offering / steal mechanic boundaries
   - Presentation handoff (overlay events, payload shape)
   - v1 limitations / future boundaries

2. Decide contract index location:
   - **Default to `Docs/Architecture/lotat-contract.md`** unless operator preference noted in handoff
   - Justification recorded in handoff

3. Write the contract index. Sections (suggest):
   - Purpose + scope (what the contract covers)
   - Story file schema
   - Runtime globals + timers
   - Commands
   - Offering / steal mechanic
   - Presentation events
   - v1 boundaries / future scope
   - Cross-references to runtime, tooling, presentation, lore docs

4. Add `.agents/manifest.json` entry for the new file (type per design — likely `shared` or new `contract` type if schema permits).

5. Update each domain doc:
   - Replace contract-restating prose with: "See [LotAT contract](path/to/lotat-contract.md) for shared facts."
   - Keep domain-specific content: runtime impl details in `Actions/LotAT/`, schema validation in `Tools/LotAT/`, lore/franchise refs in `Creative/WorldBuilding/`, presentation in `Apps/stream-overlay/`

6. Address audit finding 1.3 if natural: split `Actions/LotAT/README.md` into overview + 3 sub-docs. If the split is large (≥3 new files), defer to a separate execution prompt (`103b-lotat-readme-split`) and only design here. **Operator decision in step 0 of this prompt.** Default: include the split.

7. Run `python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/103-validator.failures.txt`. Expect 0 failures.

8. Write handoff.

## Validator / Acceptance

- `python3 Tools/AgentTree/validate.py` exits 0
- New contract index exists with manifest entry
- The four domain docs no longer restate contract facts; each contains link to index
- If split included: `Actions/LotAT/README.md` ≤150 lines and three sub-docs exist with manifest entries

## Handoff

Per template. Include:
- Contract index file location + rationale
- Per-domain doc summary of what was removed (contract restate) and what remained (domain specifics)
- Whether `Actions/LotAT/README.md` split executed in this prompt or deferred to `103b`
- Final validator output
