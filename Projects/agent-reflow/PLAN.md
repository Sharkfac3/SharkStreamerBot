# Agent Reflow Plan

Phase table for `Projects/agent-reflow/`. Operator updates Status column as each prompt completes.

## Status Legend

- `pending` — not yet drafted in detail (stub only) or not yet run
- `drafted` — full prompt written, awaiting run
- `running` — operator currently has prompt loaded in Pi
- `done` — prompt completed, handoff written, validator passed (where applicable)
- `skipped` — operator decided to skip; rationale in Notes
- `blocked` — needs operator decision before proceeding

## Phases

### Phase A — Info Gathering (read-only, write findings/)

| # | Prompt | Status | Depends on | Notes |
|---|---|---|---|---|
| 00 | `00-inventory-agents-tree.md` | drafted | — | Walks `.agents/` tree, writes findings/00-current-tree.md |
| 01 | `01-inventory-pi-mirror.md` | pending | 00 | Walks `retired Pi skill mirror/`, maps each wrapper to source |
| 02 | `02-inventory-domains.md` | pending | 00 | Walks `Actions/`, `Apps/`, `Tools/`, `Creative/`, `Docs/`; maps each subfolder to current skill leaf or flags as gap |
| 03 | `03-inventory-routing-drift.md` | pending | 00, 01 | Compares routing tables across `legacy-v1-routing-manifest`, `ENTRY.md`, `AGENTS.md`, `retired Pi skill mirror/README.md` |
| 04 | `04-inventory-cross-refs.md` | pending | 00, 01, 02 | Catalogs every internal link between agent docs; builds reference graph |

### Phase B — Design Ratification

| # | Prompt | Status | Depends on | Notes |
|---|---|---|---|---|
| 05 | `05-design-target-shape.md` | pending | 00–04 | Reads all findings, drafts target tree shape; operator ratifies before continuing |
| 06 | `06-design-naming-convention.md` | pending | 05 | Flat naming rules, file extension convention, `_index.md` vs `README.md` decision |

### Phase C — Foundation

| # | Prompt | Status | Depends on | Notes |
|---|---|---|---|---|
| 07 | `07-manifest-schema-v2.md` | pending | 05, 06 | New manifest schema + JSON Schema validator |
| 08 | `08-validator.md` | pending | 07 | Tooling that checks link integrity, folder coverage, drift detection. Replaces / extends `Tools/StreamerBot/Validation/retired-routing-doc-sync.py` |

### Phase D — Generator

| # | Prompt | Status | Depends on | Notes |
|---|---|---|---|---|
| 09 | `09-generate-migration-prompts.md` | pending | 05–08 | Reads findings + design, writes `prompts/10-NN-*.md` migration files sized for ~15-file batches; updates this PLAN |

### Phase E — Generated Migrations

| # | Prompt | Status | Depends on | Notes |
|---|---|---|---|---|
| 10-01 | `10-01-workflows-foundation.md` | drafted | 09 | Creates workflow layer and clears workflow file/stub failures. |
| 10-02 | `10-02-actions-commanders-squad-voice.md` | drafted | 10-01 | Co-locates Commanders, Squad, and Voice Commands action guidance. |
| 10-03 | `10-03-actions-lotat-tools-lotat.md` | drafted | 10-02 | Splits LotAT runtime and story-pipeline/tooling guidance. |
| 10-04 | `10-04-apps-stream-overlay-actions-overlay.md` | drafted | 10-03 | Co-locates stream overlay app and Streamer.bot overlay bridge guidance. |
| 10-05 | `10-05-actions-twitch.md` | drafted | 10-04 | Co-locates four Twitch action folders without restoring flat Twitch wrappers. |
| 10-06 | `10-06-actions-coverage-fills.md` | drafted | 10-05 | Fills uncovered Actions routes: Destroyer, Intros, Rest Focus Loop, Temporary, XJ Drivethrough. |
| 10-07 | `10-07-apps-info-production-doc-folding.md` | drafted | 10-06 | Co-locates info-service and production-manager docs; folds INFO-SERVICE plan/protocol. |
| 10-08 | `10-08-tools-and-validator-coverage.md` | drafted | 10-07 | Co-locates tool docs and adds/declares Tools/AgentTree coverage. |
| 10-09 | `10-09-creative-domains.md` | drafted | 10-08 | Co-locates Creative Art, Brand, Marketing, and WorldBuilding guidance. |
| 10-10 | `10-10-roles-shared-root-entrypoints.md` | drafted | 10-09 | Collapses role/core docs, updates shared/root entrypoints, creates Docs/Architecture route, clears root drift except Pi README. |
| 10-11 | `10-11-pi-meta-transition.md` | drafted | 10-10 | Migrates or normalizes Pi meta helper content before mirror cutover. |
| 10-12 | `10-12-retire-old-skill-tree-cleanup.md` | drafted | 10-11 | Retires old central skill tree, clears old link/orphan failures, leaves only Phase F cutover items if any. |

### Phase F — Cutover

| # | Prompt | Status | Depends on | Notes |
|---|---|---|---|---|
| NN | `NN-cutover-pi-mirror.md` | pending | All Phase E | Deletes `retired Pi skill mirror/` mirror once content fully migrated. Run AFTER all migration prompts |

### Phase G — Audit

| # | Prompt | Status | Depends on | Notes |
|---|---|---|---|---|
| 99 | `99-optimization-audit.md` | pending | NN | Review-only optimization pass; outputs recommendations operator triages into follow-up prompts if needed |

### Phase I — Audit Follow-ups (drafted from 99 audit recommendations)

| # | Prompt | Status | Depends on | Notes |
|---|---|---|---|---|
| 100 | `100-manifest-status-normalization.md` | drafted | 99 | Aligns manifest `planned` vs active doc statuses; bundles audit 4.1, 9.1, 6.3 |
| 101 | `101-post-cutover-stale-cleanup.md` | drafted | 100 | Removes Phase E/migration/retired-mirror refs; dedupes validation procedure into workflow; bundles audit 6.1, 6.2, 7.1, 8.1, 8.2, 2.2 |
| 102 | `102-actions-helper-snippets-split-design.md` | drafted | 101 | Design only for splitting `Actions/HELPER-SNIPPETS.md` by concept; execution deferred to `102b` if approved; bundles audit 1.1 |
| 103 | `103-lotat-contract-index.md` | drafted | 101 | Extracts canonical LotAT contract index; trims contract-restating prose from four domain docs; bundles audit 3.1, 1.3 |
| 104 | `104-apps-stream-overlay-doc-split.md` | drafted | 103 | Splits `Apps/stream-overlay/AGENTS.md` 305 lines into thin route + subrefs under `docs/`; bundles audit 1.4, 5.1 |

### Phase H — Disposition

| # | Prompt | Status | Depends on | Notes |
|---|---|---|---|---|
| ZZ | (in audit handoff) | pending | 99 + Phase I | Operator decides: delete `Projects/agent-reflow/` or move to `Docs/Archive/`. Default = delete |

## Dependency Diagram

```
00 ──┬─> 03 ──┐
01 ──┘        │
00,01,02 ──> 04
00,01,02,03,04 ──> 05 ──> 06 ──> 07 ──> 08 ──> 09 ──> 10..NN ──> NN-cutover ──> 99 ──> 100 ──> 101 ──> 102 ──> 103 ──> 104 ──> ZZ
```

## Update Protocol

1. Operator finishes a prompt run
2. Pi writes handoff to `handoffs/NN-<slug>.handoff.md`
3. Operator updates Status column in this file
4. Operator commits (if desired)
5. Operator loads next prompt into Pi

## Decisions Made (Pre-flight)

- Pi runs all prompts; operator copies prompts manually
- No-git rule: agent never runs `git`; operator commits
- Branch: stay on `feature/agent-retool-via-reflow`
- Serial only — no parallel prompt execution
- Hardening = optimization audit (concept boundaries, dedup, cross-refs), not line-count splits
- `Docs/` folder content may flow into other folders during Phase E
- Cutover (kill `.pi/` mirror) runs late — after migrations, before audit
- `Projects/agent-reflow/` deletes at end; archive optional

## Open Decisions (Logged for Phase B)

- Per-domain agent doc filename: `CLAUDE.md`, `AGENTS.md`, both, or new convention?
- Workflow layer location: `.agents/workflows/`, repo-root `Workflows/`, or other?
- Skill name pattern: `<domain>-<feature>` (e.g. `actions-squad`) or domain-prefix-free flat names?
- `_shared/` content: promote into root agent doc, or keep separate?
