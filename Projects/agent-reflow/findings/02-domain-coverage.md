# Findings 02 — Domain Coverage Inventory

Generated: 2026-04-26  
Source: prompt 02-inventory-domains  
Scope note: domain folders were inventoried read-only except for this findings file. Scope sizes exclude common generated/runtime folders such as `node_modules`, `dist`, `build`, `coverage`, `.venv`, `bin`, `obj`, and `__pycache__`.

## Inputs Read

- `Projects/agent-reflow/findings/00-current-tree.md`
- `Projects/agent-reflow/findings/01-pi-mirror.md`
- `legacy v1 routing manifest (retired)`
- `.agents/roles/*/role.md`
- Domain folder listings and README/CLAUDE/AGENTS presence checks

## Scope Legend

| Approx scope | Heuristic used here |
|---|---|
| small | < 1,000 lines, or very few files |
| medium | 1,000–4,999 lines |
| large | >= 5,000 lines |

`Skill leaf path` means the closest current skill file/path under `.agents/roles/<role>/skills/`. Several current mappings land on `_index.md` or `core.md` because no dedicated leaf exists yet.

---

## Actions/

Ownership baseline: `streamerbot-dev` owns `.cs` scripts and feature READMEs under `Actions/`; LotAT engine/story-pipeline work chains to `lotat-tech` when the task is specifically engine/schema/pipeline work.

| Subfolder | Current owning role | Current skill leaf path | README | CLAUDE/AGENTS | Approx scope | Coverage notes |
|---|---|---|---|---|---|---|
| `Actions/Commanders/` | `streamerbot-dev` | `.agents/roles/streamerbot-dev/skills/commanders/_index.md`; per-commander leaves: `captain-stretch.md`, `the-director.md`, `water-wizard.md` | yes | no/no | medium — 21 files, 3,618 lines | Good coverage; per-feature leaves exist. |
| `Actions/Destroyer/` | `streamerbot-dev` | `.agents/roles/streamerbot-dev/skills/core.md` only | no | no/no | small — 2 files, 372 lines | Gap: no `destroyer` feature skill leaf. |
| `Actions/Intros/` | `streamerbot-dev`; may chain to `app-dev`/`brand-steward` when info-service intro data or public intro text is involved | `.agents/roles/streamerbot-dev/skills/core.md` only | no | no/no | small — 2 files, 289 lines | Gap: no `intros` feature skill leaf; ownership may become cross-role with `Apps/info-service/`. |
| `Actions/LotAT/` | `lotat-tech` for engine/pipeline work; `streamerbot-dev` for general Streamer.bot runtime mechanics | `.agents/roles/streamerbot-dev/skills/lotat/_index.md`; plus `.agents/roles/lotat-tech/skills/engine/_index.md` and story-pipeline leaves when engine/schema work | yes | no/no | large — 14 files, 11,145 lines | Covered, but split ownership must remain explicit. Large enough to justify more direct per-runtime leaves if Phase B wants finer navigation. |
| `Actions/Overlay/` | `streamerbot-dev`; may chain to `app-dev` for broker/protocol/overlay runtime | `.agents/roles/streamerbot-dev/skills/overlay-integration.md`; app-side leaves under `.agents/roles/app-dev/skills/stream-interactions/` | yes | no/no | small — 5 files, 745 lines | Covered by a single leaf; acceptable, but bridge with app-dev should stay visible. |
| `Actions/Rest Focus Loop/` | `streamerbot-dev` | `.agents/roles/streamerbot-dev/skills/core.md` only | yes | no/no | medium — 6 files, 1,038 lines | Gap: no rest/focus feature skill leaf. |
| `Actions/Squad/` | `streamerbot-dev` | `.agents/roles/streamerbot-dev/skills/squad/_index.md`; per-game leaves: `clone.md`, `duck.md`, `pedro.md`, `toothless.md` | yes | no/no | large — 22 files, 6,472 lines | Good coverage for named games; `offering.cs` / LotAT steal behavior appears covered only at index level. |
| `Actions/Temporary/` | `streamerbot-dev` | `.agents/roles/streamerbot-dev/skills/core.md` only | no | no/no | small — 2 files, 171 lines | Gap/ambiguity: temporary focus timer scripts may belong with rest/focus if retained. |
| `Actions/Twitch Bits Integrations/` | `streamerbot-dev`; chain to `brand-steward` for public response text | `.agents/roles/streamerbot-dev/skills/twitch/bits.md` | yes | no/no | medium — 8 files, 1,907 lines | Covered. Part of Twitch many-to-one wrapper collapse. |
| `Actions/Twitch Channel Points/` | `streamerbot-dev`; chain to `brand-steward` for public redemption text | `.agents/roles/streamerbot-dev/skills/twitch/channel-points.md` | yes | no/no | small — 3 files, 726 lines | Covered. Part of Twitch many-to-one wrapper collapse. |
| `Actions/Twitch Core Integrations/` | `streamerbot-dev`; chain to `brand-steward` for public event text | `.agents/roles/streamerbot-dev/skills/twitch/core-events.md` | yes | no/no | medium — 11 files, 2,274 lines | Covered. Part of Twitch many-to-one wrapper collapse. |
| `Actions/Twitch Hype Train/` | `streamerbot-dev`; chain to `brand-steward` for public event text | `.agents/roles/streamerbot-dev/skills/twitch/hype-train.md` | yes | no/no | small — 4 files, 812 lines | Covered. Part of Twitch many-to-one wrapper collapse. |
| `Actions/Voice Commands/` | `streamerbot-dev` | `.agents/roles/streamerbot-dev/skills/voice-commands/_index.md` | yes | no/no | small — 8 files, 665 lines | Covered at index level only; no per-scene/per-mode leaves. |
| `Actions/XJ Drivethrough/` | `streamerbot-dev`; may chain to `brand-steward` or `product-dev` if it becomes product-facing | `.agents/roles/streamerbot-dev/skills/core.md` only | no | no/no | small — 1 file, 255 lines | Gap: no `xj-drivethrough` feature skill leaf; ownership intent ambiguous beyond C# runtime. |

### Actions/ gaps and granularity issues

- **No dedicated skill leaf:** `Actions/Destroyer/`, `Actions/Intros/`, `Actions/Rest Focus Loop/`, `Actions/Temporary/`, `Actions/XJ Drivethrough/`.
- **Known gap list confirmed:** `Actions/Destroyer/`, `Actions/XJ Drivethrough/`, `Actions/Intros/`, `Actions/Overlay/`, `Actions/Rest Focus Loop/`. Note: `Actions/Overlay/` does have `overlay-integration.md`, but not a folder-style feature sub-skill.
- **Many-to-one collapse:** four `Actions/Twitch *` folders are represented under one canonical wrapper, `streamerbot-dev-twitch`, even though internal leaves exist (`bits.md`, `channel-points.md`, `core-events.md`, `hype-train.md`). Pi discovery exposes one Twitch wrapper rather than four feature wrappers.
- **Large feature area:** `Actions/LotAT/` is large and cross-role. Current coverage is usable, but Phase B should decide whether `streamerbot-dev/skills/lotat/_index.md` needs leaves matching runtime files/stages.
- **Ambiguous ownership for Phase B:** `Actions/Intros/` (Streamer.bot runtime + info-service + public intro text), `Actions/XJ Drivethrough/` (C# runtime but product/content associations possible), `Actions/Temporary/` (temporary vs. retained feature).

---

## Apps/

Ownership baseline: `app-dev` owns standalone apps under `Apps/`. `ops` validates/summarizes app changes. `brand-steward` may chain in for public-facing UI text.

| Subfolder | Current owning role | Current skill leaf path | README | CLAUDE/AGENTS | Approx scope | Coverage notes |
|---|---|---|---|---|---|---|
| `Apps/info-service/` | `app-dev`; `ops` for validation/summaries | `.agents/roles/app-dev/skills/core.md`; living context at `.agents/roles/app-dev/context/info-service.md`; shared protocol `.agents/_shared/info-service-protocol.md` | yes | no/no | medium — 13 files, 1,702 lines | Gap: no app-specific skill leaf. Context/protocol docs exist but are not a skill leaf. |
| `Apps/production-manager/` | `app-dev`; `brand-steward` if UI copy changes | `.agents/roles/app-dev/skills/core.md` only | yes | no/no | medium — 13 files, 3,291 lines | Gap: no app-specific skill leaf for React admin runtime. |
| `Apps/stream-overlay/` | `app-dev`; chain to `streamerbot-dev` for C# publishers | `.agents/roles/app-dev/skills/stream-interactions/_index.md`; leaves: `overlay.md`, `broker.md`, `protocol.md`, `asset-system.md`, `lotat-rendering.md`, `squad-rendering.md` | yes | no/no | large — 62 files, 10,320 lines | Best-covered app, though coverage is feature/protocol oriented rather than a single app leaf. |

### Apps/ gaps and granularity issues

- **One-to-many app-dev gap:** `app-dev` currently covers three different runtimes under one role/core path: `info-service` (Node REST API), `production-manager` (React/Vite admin app), and `stream-overlay` (Phaser overlay + broker ecosystem).
- **No app-specific leaves:** `Apps/info-service/` and `Apps/production-manager/` lack dedicated skill leaves despite having distinct architecture, setup, validation, and runtime behavior.
- **Stream overlay coverage is deep but not mirrored in manifest:** `.agents/roles/app-dev/skills/stream-interactions/` exists and Pi wrappers route to it, but findings/00 showed it is not declared as a canonical manifest sub-skill.
- **Phase B operator decision:** choose whether app-dev should grow app leaves (`skills/info-service.md`, `skills/production-manager.md`, `skills/stream-overlay/`) or keep feature-oriented `stream-interactions/*` and add missing runtime leaves beneath it.

---

## Tools/

Ownership baseline: `ops` owns local utilities and validators under `Tools/`, except role-specific pipelines where role activation rules explicitly name them.

| Subfolder | Current owning role | Current skill leaf path | README | CLAUDE/AGENTS | Approx scope | Coverage notes |
|---|---|---|---|---|---|---|
| `Tools/ArtPipeline/` | `art-director` | `.agents/roles/art-director/skills/pipeline/_index.md` | yes | no/no | medium — 18 files, 4,124 lines | Covered at pipeline index level; no per-command leaves. |
| `Tools/ContentPipeline/` | `content-repurposer` | `.agents/roles/content-repurposer/skills/pipeline/_index.md`; `pipeline/phase-map.md` | yes | no/no | large — 18 files, 5,505 lines | Covered; large enough to justify more implementation-specific leaves if pipeline grows. |
| `Tools/LotAT/` | `lotat-tech`; possibly `ops` for local utility concerns | `.agents/roles/lotat-tech/skills/story-pipeline/_index.md`; `story-pipeline/json-schema.md` if schema validation involved | yes | no/no | medium — 3 files, 1,626 lines | Partially covered; no explicit Tools/LotAT utility leaf. Ownership ambiguous between LotAT pipeline and ops tooling. |
| `Tools/MixItUp/` | `ops` | `.agents/roles/ops/skills/core.md` only; shared reference `.agents/_shared/mixitup-api.md` | yes | no/no | small — 7 files, 501 lines | Gap: no dedicated Mix It Up tooling/integration skill leaf under ops. |
| `Tools/StreamerBot/` | `ops` | `.agents/roles/ops/skills/sync/_index.md` and `.agents/roles/ops/skills/validation/_index.md` | yes | no/no | small — 5 files, 169 lines | Covered by sync/validation skills. |

### Tools/ gaps and granularity issues

- **No dedicated skill leaf:** `Tools/MixItUp/` and `Tools/LotAT/` do not have explicit leaves matching their folder identity.
- **Role-specific tool exceptions:** `Tools/ArtPipeline/` activates `art-director`, not `ops`; `Tools/ContentPipeline/` activates `content-repurposer`, not generic ops. This should be preserved in routing.
- **Many-to-one tooling collapse:** ops covers `Tools/MixItUp/`, `Tools/StreamerBot/`, generic validators, change summaries, sync, and agent-tree operations. Current leaves cover sync/validation but not all utility families.
- **Ambiguous ownership for Phase B:** `Tools/LotAT/` should be assigned either as a `lotat-tech` tool leaf or an `ops` validator/tool leaf with LotAT chain rules.

---

## Creative/

Ownership baseline: creative domain routes by content type: art assets/prompts to `art-director`, brand and public messaging to `brand-steward`, LotAT/lore/story content to `lotat-writer` with `lotat-tech` for schema/pipeline review.

| Subfolder | Current owning role | Current skill leaf path | README | CLAUDE/AGENTS | Approx scope | Coverage notes |
|---|---|---|---|---|---|---|
| `Creative/Art/` | `art-director` | `.agents/roles/art-director/skills/core.md`; `characters/_index.md` and character leaves; `stream-style/_index.md`; `pipeline/_index.md` when tooling involved | yes | no/no | small — 10 files, 637 lines | Covered, but folder maps to multiple visual skill areas rather than one leaf. |
| `Creative/Brand/` | `brand-steward` | `.agents/roles/brand-steward/skills/core.md`; `voice/_index.md`; `canon-guardian/_index.md`; `content-strategy/_index.md` as needed | no | no/no | small — 3 files, 642 lines | Covered by role skills, but no direct `brand-docs` leaf. |
| `Creative/Marketing/` | `brand-steward`; `content-repurposer` for platform/short-form outputs | `.agents/roles/brand-steward/skills/community-growth/_index.md`; `content-strategy/_index.md`; possible `.agents/roles/content-repurposer/skills/platforms/_index.md` | yes | no/no | small — 5 files, 48 lines | Partial/ambiguous coverage; marketing is currently scaffolding, no dedicated marketing skill leaf. |
| `Creative/WorldBuilding/` | `lotat-writer`; `lotat-tech` for story JSON schema/pipeline; `brand-steward` for franchise-wide canon | `.agents/roles/lotat-writer/skills/adventures/_index.md`; `franchises/starship-shamples.md`; `universe/_index.md`; `canon-guardian/_index.md` | yes | no/no | medium — 15 files, 3,757 lines | Covered, though multiple skill areas are required depending on story/lore/schema task. |

### Creative/ gaps and granularity issues

- **No direct folder leaves:** `Creative/Brand/` and `Creative/Marketing/` map to role/core or broad sub-skill indexes, not folder-specific leaves.
- **Many-to-one / one-to-many:** `Creative/WorldBuilding/` maps to several LotAT writer leaves and sometimes `lotat-tech`/`brand-steward`; this is correct but needs clear routing examples in Phase B.
- **Marketing ambiguity:** `Creative/Marketing/` could belong primarily to `brand-steward`, with `content-repurposer` for short-form platform packaging. Phase B should decide whether to add a dedicated brand marketing/community leaf or a cross-role handoff note.

---

## Docs/

Ownership baseline: `Docs/` is cross-role documentation. `product-dev` owns future product docs/specs/knowledge articles; `ops` owns workflow/validation/sync/agent operations; app docs should generally co-locate with their app where possible. Current immediate subfolders/files are inventoried below.

### Docs subfolders

| Subfolder | Current owning role | Current skill leaf path | README | CLAUDE/AGENTS | Approx scope | Coverage notes |
|---|---|---|---|---|---|---|
| `Docs/Architecture/` | `ops` for repo/process architecture; possibly `product-dev` for future product docs architecture | `.agents/roles/ops/skills/core.md` only | no | no/no | small — 1 file, 95 lines | Gap: no docs/architecture skill leaf. Current file is repo-structure documentation. |

### Docs file co-location proposals

| Current file | Lines | Proposed co-location target | Confidence | Rationale |
|---|---:|---|---|---|
| `Docs/AGENT-WORKFLOW.md` | 146 | `.agents/_shared/coordination.md` or a new `.agents/_shared/workflow.md` | medium | It is primarily agent/contributor coordination, not product/domain documentation. Could remain in `Docs/` if kept as human-facing workflow; Phase B should decide whether `.agents` becomes the single home for agent workflow. |
| `Docs/INFO-SERVICE-PLAN.md` | 450 | `Apps/info-service/INFO-SERVICE-PLAN.md` or split between `Apps/info-service/` and `Apps/production-manager/` | high | The document is the architecture plan for info-service and production-manager; app-level docs tied directly to those apps belong under `Apps/`. |
| `Docs/ONBOARDING.md` | 180 | uncertain — candidate `.agents/_shared/project.md` / `.agents/ENTRY.md`, or keep as repo-level `Docs/ONBOARDING.md` | uncertain | It is broad project orientation for agents. Some content overlaps `.agents/ENTRY.md` and `_shared/project.md`, but it may still be useful as human-facing onboarding. |
| `Docs/Architecture/repo-structure.md` | 95 | `Docs/Architecture/repo-structure.md` or `.agents/_shared/conventions.md` | uncertain | It documents top-level repo structure and routing rules. This can remain valid as repo architecture, but its routing content overlaps agent shared conventions. |

### Docs/ gaps and granularity issues

- **No Docs role:** There is no dedicated docs role. Ownership must be inferred from content (`ops` for workflow/repo docs, `app-dev` for app docs, `product-dev` for future product docs, `brand-steward` for public copy).
- **Co-location drift:** `Docs/INFO-SERVICE-PLAN.md` is a strong candidate to move under `Apps/info-service/` because it is app architecture, not repo-wide documentation.
- **Agent workflow duplication risk:** `Docs/AGENT-WORKFLOW.md`, `.agents/_shared/coordination.md`, and `WORKING.md` all describe coordination. Phase B should decide the source of truth.
- **Ambiguous ownership for Phase B:** `Docs/ONBOARDING.md` and `Docs/Architecture/repo-structure.md` could stay in Docs as human-facing docs or move/split into `.agents/_shared/` for agent routing.

---

## Consolidated Coverage Gaps

### Subfolders with no dedicated skill leaf

- `Actions/Destroyer/`
- `Actions/Intros/`
- `Actions/Rest Focus Loop/`
- `Actions/Temporary/`
- `Actions/XJ Drivethrough/`
- `Apps/info-service/`
- `Apps/production-manager/`
- `Tools/LotAT/`
- `Tools/MixItUp/`
- `Creative/Brand/` (covered by role skills but no folder leaf)
- `Creative/Marketing/` (partial/ambiguous)
- `Docs/Architecture/`

### Subfolders covered only by broad index/core skills

- `Actions/Voice Commands/` — index only, no per-mode/per-scene leaves
- `Tools/ArtPipeline/` — pipeline index only
- `Creative/Art/` — multiple art-director skill areas, no direct folder leaf
- `Creative/WorldBuilding/` — multiple lotat-writer/lotat-tech/brand-steward pathways, no single folder leaf

### Many-to-one collapses

- `Actions/Twitch Bits Integrations/`, `Actions/Twitch Channel Points/`, `Actions/Twitch Core Integrations/`, and `Actions/Twitch Hype Train/` all expose through one canonical Pi wrapper: `streamerbot-dev-twitch`, despite internal leaves.
- `Tools/` generic utilities largely collapse into `ops`, with only sync/validation/change-summary as explicit ops leaves.
- `Creative/Brand/` and `Creative/Marketing/` collapse into broad `brand-steward` skills instead of folder-level leaves.

### One-to-many gaps

- `app-dev` covers three app runtimes with different stacks and contracts: `info-service`, `production-manager`, and `stream-overlay`.
- `Creative/WorldBuilding/` can require `lotat-writer`, `lotat-tech`, `brand-steward`, and `art-director` handoffs depending on whether the task is story authoring, schema/runtime validation, canon/marketing, or asset extraction.
- `Actions/Intros/` likely spans `streamerbot-dev` scripts, `Apps/info-service/` data, Mix It Up audio playback conventions, and public-facing intro copy.

## Phase B Operator Decisions Needed

1. Should gaps in `Actions/` become new streamerbot-dev feature folders/leaves (`destroyer`, `intros`, `rest-focus-loop`, `xj-drivethrough`) or stay covered by `core.md` until they grow?
2. Should Twitch folders get separate canonical wrappers, or is `streamerbot-dev-twitch` with internal leaves sufficient?
3. Should `app-dev` add app-specific leaves for `info-service`, `production-manager`, and `stream-overlay`, or retain the current stream-interactions feature taxonomy?
4. Should `Tools/MixItUp/` get an ops skill leaf, given repeated Mix It Up API/special identifier work?
5. Should `Tools/LotAT/` be owned primarily by `lotat-tech` or `ops`?
6. Should `Creative/Marketing/` become a `brand-steward` marketing/community skill, a `content-repurposer` platform skill area, or a handoff boundary between both?
7. Should repo/agent workflow docs in `Docs/` be co-located into `.agents/_shared/`, or remain as human-facing docs with `.agents` linking to them?
