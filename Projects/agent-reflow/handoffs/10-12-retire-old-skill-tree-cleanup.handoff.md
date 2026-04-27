# Prompt 10-12 handoff — retire-old-skill-tree-cleanup

## State changes

- Retired the obsolete central `.agents/roles/*/skills/` tree by deleting the migrated role skill folders and leaf skill files.
- Removed obsolete `.agents/roles/*/context/` folders and `.gitkeep` files after their useful content was confirmed migrated to local domain guides, workflows, or role overviews.
- Removed `.agents/roles/_template/` instead of manifest-declaring it, because the target shape no longer uses central role skill templates.
- Deleted the migrated compatibility pointers:
  - `.agents/_shared/info-service-protocol.md`
  - `.agents/_shared/mixitup-api.md`
- Updated surviving references away from deprecated central skill/shared docs in:
  - `Docs/ONBOARDING.md`
  - `Docs/Architecture/repo-structure.md`
  - `Apps/stream-overlay/README.md`
  - `Apps/info-service/INFO-SERVICE-PLAN.md`
  - `Actions/Overlay/README.md`
  - `Actions/Intros/AGENTS.md`
  - `Actions/Temporary/AGENTS.md`
  - `Creative/WorldBuilding/Franchises/StarshipShamples.md`
  - `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`
  - `.agents/roles/streamerbot-dev/role.md`
- Updated `.agents/manifest.json` role-overview notes to reflect that old `core.md` content is already collapsed.
- Added `legacy v1 routing manifest (retired)` to `.agents/manifest.json` as a deprecated `shared-reference` co-location so the retained v1 manifest is intentional and no longer orphaned.
- Wrote validator reports:
  - `Projects/agent-reflow/findings/10-12-precleanup.failures.txt`
  - `Projects/agent-reflow/findings/10-12-validator.failures.txt`

## Migration source disposition

All old `.agents/roles/*/skills/core.md`, nested `_index.md`, leaf skill, and migrated context files covered by the Phase E handoffs are now retired. Their content was represented by prior prompts in:

- role overview files under `.agents/roles/*/role.md`
- workflows under `.agents/workflows/`
- local route docs named `AGENTS.md` under `Actions/`, `Apps/`, `Tools/`, `Creative/`, and `Docs/Architecture/`
- app/tool local reference docs such as `Apps/info-service/INFO-SERVICE-PLAN.md`, `Apps/production-manager/PRODUCTION-MANAGER-GUIDE.md`, `Tools/MixItUp/AGENTS.md`, and `Apps/stream-overlay/AGENTS.md`

No compatibility stubs were retained in the old `.agents/roles/*/skills/` tree because no surviving root/domain/workflow docs need to load them.

## Link cleanup summary

A targeted search over root/domain/workflow docs found no remaining active links or path references to:

- `.agents/roles/*/skills/...`
- deprecated `_index.md` skill sources
- `.agents/_shared/info-service-protocol.md`
- `.agents/_shared/mixitup-api.md`
- `retired Pi skill mirror/*/SKILL.md` sources

Historical handoff files under `Projects/agent-reflow/handoffs/` still mention old migration sources as provenance only.

## Validator status

Pre-cleanup command:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-12-precleanup.failures.txt
```

Pre-cleanup result:

```text
Total failures: 283
schema: 0
folder-coverage: 0
link-integrity: 248
stub-presence: 0
orphan: 35
naming: 0
```

Final command:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-12-validator.failures.txt
```

Final result:

```text
Agent tree validation summary
| Check | Checked | Failures | Status |
|---|---:|---:|---|
| schema | 1 | 0 | PASS |
| folder-coverage | 153 | 0 | PASS |
| link-integrity | 51 | 0 | PASS |
| drift | 3 | 0 | PASS |
| stub-presence | 49 | 0 | PASS |
| orphan | 21 | 0 | PASS |
| naming | 108 | 0 | PASS |

Total failures: 0
```

## Cleared Phase E deltas

- All baseline `orphan` failures under `.agents/` are cleared.
- Old-source `link-integrity` failures from `.agents/roles/*/skills/**` are cleared by deletion.
- Old-source link failures from migrated shared protocol/API files are cleared by routing surviving docs to app/tool-local references and deleting the shared pointers.
- Obsolete context-note and `.gitkeep` orphan failures are cleared.
- `_template/` no longer fails orphan/link checks because it was removed.
- Drift remains clear for root generated surfaces and `retired Pi skill mirror/README.md`.
- Schema, folder coverage, stub presence, naming, link integrity, orphan, and drift all pass in the final Phase E run.

## Items intentionally left for Phase F / later audit

- `retired Pi skill mirror/` was not deleted per prompt scope. Phase F owns Pi wrapper deletion or final conversion.
- Many non-meta `retired Pi skill mirror/*/SKILL.md` wrappers are still transitional compatibility files and may reference retired `.agents` paths in their own text. They are outside the Phase E validator pass and should be removed or rewritten during Phase F cutover.
- `legacy v1 routing manifest (retired)` is retained as a deprecated v1 provenance file, now manifest-declared. Phase F or prompt 99 can decide whether to delete it after final audit.
- No runtime code under `Actions/`, `Apps/`, or `Tools/` was intentionally changed by this prompt.

## Next prompt entry point

Proceed to Phase F cutover or prompt 99 audit using `Projects/agent-reflow/findings/10-12-validator.failures.txt` as the clean Phase E baseline.
