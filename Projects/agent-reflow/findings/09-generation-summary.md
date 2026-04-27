# Findings 09 — Migration Prompt Generation Summary

Generated: 2026-04-26  
Source: prompt 09-generate-migration-prompts

## Inputs Read

- `Projects/agent-reflow/README.md`
- `Projects/agent-reflow/PLAN.md`
- `Projects/agent-reflow/findings/00-current-tree.md`
- `Projects/agent-reflow/findings/01-pi-mirror.md`
- `Projects/agent-reflow/findings/02-domain-coverage.md`
- `Projects/agent-reflow/findings/03-routing-drift.md`
- `Projects/agent-reflow/findings/04-cross-refs.md`
- `Projects/agent-reflow/findings/05-target-shape.md`
- `Projects/agent-reflow/findings/06-naming-convention.md`
- `Projects/agent-reflow/findings/07-manifest-v2.md`
- `Projects/agent-reflow/findings/08-validator.md`
- `Projects/agent-reflow/findings/08-validator.failures.txt`
- `.agents/manifest.json` route/domain/workflow inventory

## Generated Prompt Index

12 Phase E migration prompts were generated:

| Order | Prompt | Main purpose |
|---:|---|---|
| 10-01 | `Projects/agent-reflow/prompts/10-01-workflows-foundation.md` | Create `.agents/workflows/` and migrate reusable procedures. |
| 10-02 | `Projects/agent-reflow/prompts/10-02-actions-commanders-squad-voice.md` | Co-locate Commanders, Squad, and Voice Commands action guidance. |
| 10-03 | `Projects/agent-reflow/prompts/10-03-actions-lotat-tools-lotat.md` | Split LotAT runtime and story-pipeline/tooling guidance. |
| 10-04 | `Projects/agent-reflow/prompts/10-04-apps-stream-overlay-actions-overlay.md` | Co-locate stream-overlay app guidance and Streamer.bot overlay bridge guidance. |
| 10-05 | `Projects/agent-reflow/prompts/10-05-actions-twitch.md` | Co-locate four Twitch action folders. |
| 10-06 | `Projects/agent-reflow/prompts/10-06-actions-coverage-fills.md` | Fill uncovered Actions routes. |
| 10-07 | `Projects/agent-reflow/prompts/10-07-apps-info-production-doc-folding.md` | Co-locate info-service/production-manager docs and fold INFO-SERVICE plan/protocol. |
| 10-08 | `Projects/agent-reflow/prompts/10-08-tools-and-validator-coverage.md` | Co-locate tool docs and add/declare `Tools/AgentTree/` coverage. |
| 10-09 | `Projects/agent-reflow/prompts/10-09-creative-domains.md` | Co-locate Creative Art, Brand, Marketing, and WorldBuilding guidance. |
| 10-10 | `Projects/agent-reflow/prompts/10-10-roles-shared-root-entrypoints.md` | Collapse role/core docs, create Docs/Architecture route, and update shared/root entrypoints. |
| 10-11 | `Projects/agent-reflow/prompts/10-11-pi-meta-transition.md` | Normalize/migrate Pi meta helper content before mirror cutover. |
| 10-12 | `Projects/agent-reflow/prompts/10-12-retire-old-skill-tree-cleanup.md` | Retire old central skill tree and clear old link/orphan failures. |

## Batching Rationale

Batches were grouped by natural co-location and migration dependency rather than by strict line count:

1. **Workflow foundation first** because many later local docs need links to `canon-guardian`, `change-summary`, `sync`, `validation`, and `coordination`.
2. **Actions split into concept clusters**: high-signal existing coverage (`Commanders`, `Squad`, `Voice`), LotAT runtime, overlay bridge, Twitch family, then previously uncovered action folders.
3. **Apps split from stream overlay** because `Apps/stream-overlay/` already has deep stream-interactions source material, while `info-service`/`production-manager` are tied to doc folding and protocol relocation.
4. **Tools grouped together** because these docs primarily close validation/tooling route gaps and include the new `Tools/AgentTree/` gap from prompt 08.
5. **Creative grouped together** because the source skill files have dense cross-role relationships and rely on the canon workflow from 10-01.
6. **Role/shared/root updates are late** so role overviews can link to already-created domain docs instead of carrying domain details.
7. **Pi meta transition is isolated** so the two real Pi wrapper contents are handled before Phase F without touching the whole mirror.
8. **Cleanup last** because old `.agents/roles/*/skills/` files will continue to generate link/orphan failures until their content has been migrated and verified.

The intended batch sizes stay near or under the requested ~15-file working set, except where a natural family has many tiny source files. Those larger source sets are still conceptually bounded and explicitly scoped.

## Coverage of Phase 08 Baseline Failure Themes

| Baseline theme | Covering prompts |
|---|---|
| Missing workflow files and workflow stubs | 10-01 |
| Missing `Actions/*/AGENTS.md` files | 10-02 through 10-06 |
| Missing `Apps/*/AGENTS.md` files | 10-04 and 10-07 |
| Missing `Tools/*/AGENTS.md` files | 10-03 and 10-08 |
| Missing `Creative/*/AGENTS.md` files | 10-09 |
| Missing `Docs/Architecture/AGENTS.md` | 10-10 |
| New `Tools/AgentTree/` route gap | 10-08 |
| Role/root/shared missing frontmatter | 10-10 |
| Generated routing drift in root/entry docs | 10-10; Pi README drift in 10-11 |
| Pi meta wrapper frontmatter/link failures | 10-11 |
| Old central skill-source link failures | Content migrated in 10-02 through 10-09; old files retired in 10-12 |
| `.agents/` orphan failures | 10-10 and 10-12 |
| Info-service plan/protocol doc folding | 10-07 |
| Mix It Up shared reference relocation | 10-08 |

## Expected Execution Order

Run prompts serially in numeric order:

```text
10-01 -> 10-02 -> 10-03 -> 10-04 -> 10-05 -> 10-06 -> 10-07 -> 10-08 -> 10-09 -> 10-10 -> 10-11 -> 10-12 -> Phase F cutover -> 99 audit
```

Each prompt writes a handoff and validator report. Later prompts should read all immediately prior handoffs and may use earlier validator reports to tune cleanup.

## Skipped / Flagged Concerns

- `retired Pi skill mirror/` mirror deletion is intentionally not in Phase E. It remains Phase F (`NN-cutover-pi-mirror.md`).
- The generated prompts do not prescribe exact deletions of every old skill file before content is migrated. Prompt 10-12 owns verification and retirement after all target docs exist.
- Some validator link failures are from the validator treating code-ish backtick mentions as path references. The migration prompts prefer fixing source docs or retiring source files over weakening the validator.

## PLAN.md Update

`Projects/agent-reflow/PLAN.md` Phase E was updated with one `drafted` row per generated prompt, including dependencies and notes.
