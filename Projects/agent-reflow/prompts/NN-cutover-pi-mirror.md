# Prompt NN — Cutover Pi Mirror

## Agent

Pi (manual copy/paste by operator).

## Note

Prompt number `NN` will be assigned by prompt 09 generator (immediately after the last Phase E migration prompt). Replace `NN` in filename and validator commands once assigned.

## Purpose

Delete the `retired Pi skill mirror/` mirror tree now that all content has migrated. Final cleanup of legacy routing layer. Updates root agent docs to remove references to `retired Pi skill mirror/`.

## Preconditions

- Every Phase E migration prompt complete
- Validator passing (per current rules) for everything except Pi-mirror cleanup
- Pi has run any prompt that depended on `retired Pi skill mirror/` already
- Read all Phase E handoffs

## Scope

Deletes:
- `retired Pi skill mirror/` (entire tree)
- Old `legacy v1 routing manifest (retired)` (if Phase E did not already)
- `Tools/StreamerBot/Validation/retired-routing-doc-sync.py` (if superseded by new validator)

Updates:
- `AGENTS.md` — remove `retired Pi skill mirror/` references, remove old GENERATED block if obsolete
- Root `CLAUDE.md` — remove any `retired Pi skill mirror/` references
- `WORKING.md` — remove `retired Pi skill mirror/` from domain list
- Any other doc that references the deleted paths

## Out-of-scope

- No new skill content
- No reorganization beyond removing the mirror
- No git operations

## Steps

1. Confirm validator is green except for Pi-mirror-related orphan/drift checks.
2. Delete `retired Pi skill mirror/` recursively.
3. Delete old `legacy v1 routing manifest (retired)` if v2 manifest fully replaced it (per prompt 07 / Phase E).
4. Delete `Tools/StreamerBot/Validation/retired-routing-doc-sync.py` if validator from prompt 08 covers its functionality.
5. Grep entire repo for remaining string matches: `retired Pi skill mirror`, `legacy-v1-routing-manifest` (old name), `retired-routing-doc-sync`.
6. Update / remove every match found in step 5.
7. Re-run validator. Expect green.

## Validator / Acceptance

- Validator exits 0
- No remaining repo references to `retired Pi skill mirror/` (grep returns no hits)
- No broken links introduced

## Handoff

Per template. Note: this is the last cleanup before audit. Include final validator stdout for record.
