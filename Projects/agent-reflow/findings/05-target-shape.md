# Findings 05 — Target Shape Design

Generated: 2026-04-26  
Source: prompt 05-design-target-shape  
Status: **RATIFIED — operator accepted all draft recommendations; prompt 06 may proceed**

## Operator Decisions Resolved

Operator response: "use the suggestions provided for all of them." Every item below is **RATIFIED** as recommended.

1. **RATIFIED — Manifest authority/location:** keep `legacy v1 routing manifest (retired)` as manifest v2 single source of truth and upgrade the schema.
2. **RATIFIED — Per-domain agent doc filename:** use `AGENTS.md`; root `CLAUDE.md` remains compatibility pointer only.
3. **RATIFIED — Per-domain doc coverage depth:** every first-level `Actions/`, `Apps/`, `Tools/`, and `Creative/` subfolder gets either `AGENTS.md` or an explicit manifest `coveredBy` entry.
4. **RATIFIED — Workflow layer location:** reusable procedures live under `.agents/workflows/<workflow-id>.md`.
5. **RATIFIED — Workflow taxonomy:** use one `.agents/workflows/canon-guardian.md` with role-specific checklists and manifest applicability.
6. **RATIFIED — Skill content location:** move domain/runtime knowledge beside the domain; keep central roles concise; keep reusable procedures in workflows.
7. **RATIFIED — App-dev taxonomy:** app-specific `Apps/<app>/AGENTS.md` docs own runtime/setup/validation; `stream-interactions` concepts are linked from `Apps/stream-overlay/AGENTS.md`.
8. **RATIFIED — Twitch granularity:** use one `streamerbot-dev` owner family with per-folder local docs; do not add four top-level Pi-style skill wrappers.
9. **RATIFIED — `Tools/LotAT/` owner:** `lotat-tech` primary; `ops` secondary for validator/tool execution mechanics.
10. **RATIFIED — `Creative/Marketing/` owner:** `brand-steward` primary; `content-repurposer` secondary for platform packaging and short-form repurposing.
11. **RATIFIED — `_shared/` disposition:** split by owner/audience; keep only repo-wide agent context central and move owner-specific protocol/API knowledge to domains.
12. **RATIFIED — `Docs/` disposition:** reduce `Docs/` to repo-wide human docs; move app/domain implementation plans beside their domains.
13. **RATIFIED — Naming convention:** use hierarchical content paths, kebab-case manifest/workflow IDs, and existing folder names unless a later rename is explicitly worth the churn.
14. **RATIFIED — `role.md` + `core.md` collapse:** collapse to one concise role overview per role after migrating detailed knowledge to domain docs/workflows.
15. **RATIFIED — Pi mirror disposition:** treat `retired Pi skill mirror/` as transitional compatibility; do not hand-maintain new wrappers; delete only after manifest/root-doc discovery replaces it.
16. **RATIFIED — Link convention for validators:** standardize on explicit real paths (`.agents/...`, `.pi/...`) and treat unnormalized paths as broken in validators.
17. **RATIFIED — Generated markers:** generated content must be visibly marked everywhere it is regenerated.

---

## Inputs Read

This design synthesizes all Phase A findings and handoffs present as of 2026-04-26:

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

Key evidence used:

- Findings 00: `.agents/` has 9 manifest roles, 12 canonical sub-skills, 14 aliases, 3 helpers, and 14 folder-vs-manifest mismatches.
- Findings 01: `retired Pi skill mirror/` has 38 wrappers; 22 are pure routing stubs, 14 are migrated aliases, and only 2 wrappers contain real Pi-specific content.
- Findings 02: many domain folders lack local/dedicated skill coverage, especially `Actions/Destroyer/`, `Actions/Intros/`, `Actions/Rest Focus Loop/`, `Actions/XJ Drivethrough/`, `Apps/info-service/`, `Apps/production-manager/`, `Tools/MixItUp/`, and `Tools/LotAT/`.
- Findings 03: routing text is duplicated; `app-dev` activation drift exists between manifest and generated docs; `retired Pi skill mirror/README.md ## Routing Table` is manual and unsynced.
- Findings 04: cross-reference graph found 256 broken doc/path references, 14 zero-inbound files, repeated path-normalization problems, and several lost progressive-disclosure candidates.

---

## Target Shape Summary

The target tree should become a **manifest-routed, domain-co-located, workflow-aware** agent system.

```text
.agents/
  ENTRY.md                         thin universal entry; generated/manifest-backed routing summary only
  legacy-v1-routing-manifest            manifest v2; single routing source of truth
  roles/
    <role>/
      role.md                      concise role overview after role/core collapse
      context/                     living notes only when not domain-local
  workflows/
    canon-guardian.md
    change-summary.md
    sync.md
    validation.md
    coordination.md                if WORKING.md protocol becomes workflow-level
  shared/ or _shared/              only repo-wide agent context after split

Actions/
  <feature>/AGENTS.md              local owner, load rules, paste targets, validation, workflow links
Apps/
  <app>/AGENTS.md                  runtime setup, architecture pointers, validation, owner
Tools/
  <tool-family>/AGENTS.md          tool owner, commands, validation, handoffs
Creative/
  <area>/AGENTS.md                 canon/brand/art/story owner, source docs, workflow links
Docs/
  ...                              repo-wide human docs only; no app/domain implementation plans

AGENTS.md                          root universal agent entry for all agents; generated quick routing block
CLAUDE.md                          root compatibility pointer only, unless operator chooses otherwise
retired Pi skill mirror/                        transitional Pi compatibility only until cutover
```

Key principle: **the manifest routes; local `AGENTS.md` files teach; workflow files describe repeatable procedures; root docs summarize.**

---

## Design Decisions

### 1. Routing single source of truth

**Decision**  
Upgrade `legacy v1 routing manifest (retired)` to manifest v2 and make it the single authoritative routing contract. Root `AGENTS.md`, `.agents/ENTRY.md`, `retired Pi skill mirror/README.md`, wrapper generation/cutover checks, local-domain coverage checks, and quick-routing summaries derive from it.

**Schema sketch**

```jsonc
{
  "version": 2,
  "generatedMarkersRequired": true,
  "entrypoints": {
    "rootAgentDoc": "AGENTS.md",
    "agentEntry": ".agents/ENTRY.md",
    "claudeCompatibility": "CLAUDE.md"
  },
  "roles": {
    "streamerbot-dev": {
      "description": "Streamer.bot C# runtime actions",
      "overview": ".agents/roles/streamerbot-dev/role.md",
      "when": "Any Streamer.bot C# action or runtime integration under Actions/",
      "owns": ["Actions/**/*.cs"],
      "defaultWorkflows": ["change-summary", "sync", "validation"],
      "chainsTo": ["brand-steward", "app-dev", "lotat-tech"]
    }
  },
  "domainRoutes": [
    {
      "id": "actions-squad",
      "path": "Actions/Squad/",
      "owner": "streamerbot-dev",
      "secondaryOwners": ["app-dev"],
      "agentDoc": "Actions/Squad/AGENTS.md",
      "status": "covered",
      "workflows": ["change-summary", "sync", "validation"]
    },
    {
      "id": "actions-temporary",
      "path": "Actions/Temporary/",
      "owner": "streamerbot-dev",
      "coveredBy": "Actions/Rest Focus Loop/AGENTS.md",
      "status": "covered-by-parent"
    }
  ],
  "workflows": {
    "change-summary": {
      "path": ".agents/workflows/change-summary.md",
      "terminal": true,
      "appliesTo": ["code-change", "doc-change"]
    }
  },
  "compatibilityAliases": {
    "feature-squad": {
      "targetRoute": "actions-squad",
      "status": "deprecated"
    }
  }
}
```

**Rationale**  
Addresses findings 00 and 03: current routing is duplicated across manifest, `.agents/ENTRY.md`, root `AGENTS.md`, `retired Pi skill mirror/README.md`, and wrapper descriptions. The `app-dev` drift proves the current contract is not reliably authoritative.

**Alternatives considered**

- Root `AGENTS.md` as source of truth: readable but hard to validate/generate from.
- New root `agent-routing.json`: clearer at repo root but adds churn and abandons current `.agents/` contract.
- Keep current manifest plus manual routing docs: lowest migration cost but preserves drift risk.

**Tradeoff**  
Manifest JSON is less pleasant for humans, so generated/linked Markdown views remain necessary. The benefit is validators can prove route coverage and detect drift.

---

### 2. Skill content location

**Decision**  
Move domain/runtime-specific skill content beside the domain it describes. Keep central role files short and role-oriented. Move reusable procedure content to workflows.

**Central tree keeps**

- Role identities, trigger boundaries, anti-triggers, and chain rules.
- Cross-role workflows.
- Repo-wide coordination and conventions.
- Context notes that are genuinely cross-domain or not tied to a stable folder.

**Domain folders keep**

- Setup/build/test commands for that domain.
- Paste/sync notes for Streamer.bot actions.
- Protocol/runtime notes for apps/tools.
- Feature-specific gotchas.
- Local ownership and handoff rules.

**Rationale**  
Addresses findings 02 and 04: many folders are covered only by broad `core.md` or `_index.md`; important docs are zero-inbound or hidden behind wrapper load paths.

**Alternatives considered**

- Add missing `.agents/roles/*/skills/` leaves only.
- Move all role docs into domain folders.
- Keep `retired Pi skill mirror/` as the primary discovery layer.

**Tradeoff**  
More local docs means more files, but lower task-start friction and clearer ownership at the point of work.

---

### 3. Per-domain agent doc convention

**Decision**  
Use `AGENTS.md` as the canonical domain/subdomain agent doc filename. Use `CLAUDE.md` only for compatibility pointers where tools require it; draft default is root-only.

**Local `AGENTS.md` template**

1. Primary owner role and secondary/chained roles.
2. Activation conditions for this folder.
3. Required local references.
4. Setup/build/validation commands.
5. Runtime/paste/sync notes where applicable.
6. Linked workflows.
7. Local gotchas and boundaries.
8. Canon/public-copy/product handoff triggers where applicable.

**Rationale**  
Addresses findings 02: no immediate domain subfolder currently has `CLAUDE.md` or nested `AGENTS.md`, so agents must infer ownership from central docs.

**Alternatives considered**

- `CLAUDE.md` everywhere: useful for Claude but less agent-agnostic.
- Both everywhere: maximum compatibility but doubles maintenance unless one is a stub.
- `README.agent.md`: explicit but not a common convention in this repo.

**Tradeoff**  
Some tools may not auto-load nested `AGENTS.md`; manifest-driven routing/validation and root pointers must compensate.

---

### 4. Workflow layer

**Decision**  
Create `.agents/workflows/` for reusable procedures that are not roles and not domain-local implementation knowledge.

**Initial workflows**

| Workflow ID | Canonical path | Notes |
|---|---|---|
| `canon-guardian` | `.agents/workflows/canon-guardian.md` | Shared review procedure; sections for brand-level and LotAT-level canon. |
| `change-summary` | `.agents/workflows/change-summary.md` | Terminal response format after changes. |
| `sync` | `.agents/workflows/sync.md` | Repo-to-Streamer.bot paste/sync process. |
| `validation` | `.agents/workflows/validation.md` | Validator command selection and expected outputs. |
| `coordination` | `.agents/workflows/coordination.md` | Ratified default if WORKING.md protocol is extracted as a reusable procedure. |
| `content-strategy` | `Creative/Marketing/AGENTS.md` by default | Promote to `.agents/workflows/content-strategy.md` only if later migration proves it is reusable outside brand/marketing work. |

**Rationale**  
Addresses findings 00/01: current canonical sub-skills and Pi wrappers conflate roles, feature knowledge, and procedures (`ops-change-summary`, `ops-sync`, `ops-validation`, canon guardian variants).

**Alternatives considered**

- Keep workflows under `.agents/roles/ops/skills/`.
- Use `Docs/Workflows/` for human visibility.
- Use root `Workflows/`.

**Tradeoff**  
Agent-facing workflows become less visible to casual repo browsers; root docs can link to generated workflow summaries if needed.

---

### 5. Domain coverage ownership

**Decision**  
Every first-level subfolder under `Actions/`, `Apps/`, `Tools/`, and `Creative/` must have an explicit manifest route and either a local `AGENTS.md` or explicit `coveredBy` entry. Findings 02 gaps should be closed, not left implicit in role `core.md`.

#### Actions/

| Subfolder | Primary owner | Secondary / chain roles | Target doc |
|---|---|---|---|
| `Actions/Commanders/` | `streamerbot-dev` | `brand-steward` for public text | `Actions/Commanders/AGENTS.md` |
| `Actions/Destroyer/` | `streamerbot-dev` | `brand-steward` if public text | `Actions/Destroyer/AGENTS.md` |
| `Actions/Intros/` | `streamerbot-dev` | `app-dev`, `brand-steward`, `ops` for Mix It Up/audio/API conventions | `Actions/Intros/AGENTS.md` |
| `Actions/LotAT/` | `lotat-tech` | `streamerbot-dev`, `lotat-writer`, `app-dev` | `Actions/LotAT/AGENTS.md` |
| `Actions/Overlay/` | `streamerbot-dev` | `app-dev` | `Actions/Overlay/AGENTS.md` |
| `Actions/Rest Focus Loop/` | `streamerbot-dev` | `brand-steward` for chat copy | `Actions/Rest Focus Loop/AGENTS.md` |
| `Actions/Squad/` | `streamerbot-dev` | `app-dev` for overlay rendering | `Actions/Squad/AGENTS.md` |
| `Actions/Temporary/` | `streamerbot-dev` | none, or merge/cover by Rest Focus if retained | `Actions/Temporary/AGENTS.md` or manifest `coveredBy` |
| `Actions/Twitch Bits Integrations/` | `streamerbot-dev` | `brand-steward` | `Actions/Twitch Bits Integrations/AGENTS.md` |
| `Actions/Twitch Channel Points/` | `streamerbot-dev` | `brand-steward` | `Actions/Twitch Channel Points/AGENTS.md` |
| `Actions/Twitch Core Integrations/` | `streamerbot-dev` | `brand-steward` | `Actions/Twitch Core Integrations/AGENTS.md` |
| `Actions/Twitch Hype Train/` | `streamerbot-dev` | `brand-steward` | `Actions/Twitch Hype Train/AGENTS.md` |
| `Actions/Voice Commands/` | `streamerbot-dev` | none | `Actions/Voice Commands/AGENTS.md` |
| `Actions/XJ Drivethrough/` | `streamerbot-dev` | `product-dev`, `brand-steward` if product-facing | `Actions/XJ Drivethrough/AGENTS.md` |

#### Apps/

| Subfolder | Primary owner | Secondary / chain roles | Target doc |
|---|---|---|---|
| `Apps/info-service/` | `app-dev` | `streamerbot-dev`, `ops` | `Apps/info-service/AGENTS.md` |
| `Apps/production-manager/` | `app-dev` | `brand-steward` for UI copy | `Apps/production-manager/AGENTS.md` |
| `Apps/stream-overlay/` | `app-dev` | `streamerbot-dev`, `lotat-tech` | `Apps/stream-overlay/AGENTS.md` |

#### Tools/

| Subfolder | Primary owner | Secondary / chain roles | Target doc |
|---|---|---|---|
| `Tools/ArtPipeline/` | `art-director` | `ops` for environment validation | `Tools/ArtPipeline/AGENTS.md` |
| `Tools/ContentPipeline/` | `content-repurposer` | `ops` for tooling validation | `Tools/ContentPipeline/AGENTS.md` |
| `Tools/LotAT/` | `lotat-tech` | `ops` | `Tools/LotAT/AGENTS.md` |
| `Tools/MixItUp/` | `ops` | `streamerbot-dev`, `app-dev` when API usage touches runtime | `Tools/MixItUp/AGENTS.md` |
| `Tools/StreamerBot/` | `ops` | `streamerbot-dev` | `Tools/StreamerBot/AGENTS.md` |

#### Creative/

| Subfolder | Primary owner | Secondary / chain roles | Target doc |
|---|---|---|---|
| `Creative/Art/` | `art-director` | `brand-steward`, `lotat-writer` as needed | `Creative/Art/AGENTS.md` |
| `Creative/Brand/` | `brand-steward` | `lotat-writer`, `art-director` for canon/assets | `Creative/Brand/AGENTS.md` |
| `Creative/Marketing/` | `brand-steward` | `content-repurposer` for platform packaging | `Creative/Marketing/AGENTS.md` |
| `Creative/WorldBuilding/` | `lotat-writer` | `lotat-tech`, `brand-steward`, `art-director` | `Creative/WorldBuilding/AGENTS.md` |

**Rationale**  
Directly addresses findings 02 coverage gaps and the many-to-one/one-to-many ambiguity lists.

**Alternatives considered**

- Add local docs only for current gaps.
- Use manifest-only routes without local files.
- Preserve existing broad role/core coverage.

**Tradeoff**  
Comprehensive local docs create boilerplate. `coveredBy` allows intentionally trivial folders to avoid unnecessary files while remaining validator-visible.

---

### 6. `_shared/` disposition

**Decision**  
Split `_shared/` by audience and owner. Keep a central agent shared folder only for repo-wide agent context; move owner-specific API/protocol knowledge to owning domains or route it explicitly in manifest.

| Current file | Target disposition |
|---|---|
| `.agents/_shared/project.md` | Keep central; optionally rename to `.agents/shared/project.md`. |
| `.agents/_shared/conventions.md` | Keep central for repo-wide conventions; split workflow/procedure details if present. |
| `.agents/_shared/coordination.md` | Merge/split with `.agents/workflows/coordination.md` and root `WORKING.md` protocol; one source of truth. |
| `.agents/_shared/info-service-protocol.md` | Move/own under `Apps/info-service/` or explicitly route as app-dev-owned shared reference. |
| `.agents/_shared/mixitup-api.md` | Move/own under `Tools/MixItUp/` or explicitly route as ops-owned shared reference. |

**Rationale**  
Addresses findings 04 broken-link/path-normalization issues and findings 02 co-location needs. `_shared/` currently mixes truly global context with domain API contracts.

**Alternatives considered**

- Keep `_shared/` unchanged.
- Promote all shared content into root `AGENTS.md`.
- Move all shared content to `Docs/`.

**Tradeoff**  
Splitting reduces accidental global load and clarifies ownership, but requires careful link updates and may temporarily create compatibility stubs.

---

### 7. `Docs/` disposition

**Decision**  
Reduce `Docs/` to repo-wide, human-facing documentation. Move domain implementation plans beside their domains. Agent-facing workflows live under `.agents/workflows/`.

| Current file | Target disposition |
|---|---|
| `Docs/AGENT-WORKFLOW.md` | Split: agent procedure to `.agents/workflows/coordination.md` or `.agents/shared/coordination.md`; human contribution overview may remain. |
| `Docs/INFO-SERVICE-PLAN.md` | Move/split into `Apps/info-service/` and `Apps/production-manager/` architecture docs. |
| `Docs/ONBOARDING.md` | Keep if human onboarding; remove duplicated routing and link to manifest-backed entries. |
| `Docs/Architecture/repo-structure.md` | Keep as human repo architecture if useful; remove or generate duplicated routing content. |

**Rationale**  
Addresses findings 02 Docs co-location proposals and findings 04 broken path patterns in docs that reference agent/domain files.

**Alternatives considered**

- Keep `Docs/` unchanged and add links.
- Move all `Docs/` into `.agents/` or domains.
- Make `Docs/Workflows/` canonical for workflows.

**Tradeoff**  
This preserves human-facing documentation while preventing app/domain implementation plans from drifting away from code.

---

### 8. Naming convention

**Decision**  
Use hierarchical file paths for content and kebab-case stable IDs in manifest/workflows. Preserve existing human-readable folder names unless a later prompt explicitly approves folder renames.

**Examples**

| Thing | Example |
|---|---|
| Role ID | `streamerbot-dev` |
| Workflow ID | `change-summary` |
| Domain route ID | `actions-rest-focus-loop` |
| Existing path | `Actions/Rest Focus Loop/` |
| Local doc | `Actions/Rest Focus Loop/AGENTS.md` |

**Rationale**  
Addresses findings 01 alias/wrapper proliferation and findings 04 path parsing problems. IDs can be normalized without renaming folders with spaces.

**Alternatives considered**

- Continue flat Pi skill names as primary IDs.
- Rename all folders to kebab-case paths.
- Use snake_case or PascalCase IDs.

**Tradeoff**  
Separating IDs from paths adds schema complexity, but avoids broad path-churn and keeps manifest entries stable.

---

### 9. `role.md` + `core.md` collapse

**Decision**  
Collapse each role's `role.md` and `skills/core.md` into one concise role overview after detailed domain/procedure content has a new home.

**New role overview responsibilities**

- What the role owns.
- When to activate and when not to activate.
- Which domain routes commonly apply.
- Which workflows are terminal/required.
- Which secondary roles to chain to.
- Links to living context if any.

**Rationale**  
Addresses findings 00 structure: every role currently has both `role.md` and `skills/core.md`, which increases startup load and update burden. Findings 02 show detailed knowledge is often better located by domain.

**Alternatives considered**

- Keep both with stricter separation.
- Delete central role docs entirely.
- Rename `core.md` to role-level `AGENTS.md`.

**Tradeoff**  
Collapse reduces reading friction, but migration must avoid losing details currently buried in `core.md` files.

---

### 10. Pi mirror and alias disposition

**Decision**  
Treat `retired Pi skill mirror/` as transitional compatibility. Do not add new manually maintained wrappers. After manifest v2, local domain docs, workflows, and validators prove coverage, execute a cutover that removes or replaces the mirror.

**Rationale**  
Addresses findings 01: all 38 wrappers are manifest-backed; most are stubs or aliases. The mirror is adapter debt once Pi can route from manifest/root docs.

**Alternatives considered**

- Keep wrappers permanently.
- Generate wrappers indefinitely from manifest v2.
- Move Pi meta content into an `ops` role.

**Tradeoff**  
Deletion cannot happen until Pi discovery has an equivalent replacement. The two real Pi meta wrappers need explicit migration or replacement before cutover.

---

### 11. Cross-reference and validator expectations

**Decision**  
Manifest v2 and validators should enforce routing and doc graph health.

Minimum validator expectations:

- Every first-level domain subfolder has a domain route.
- Every domain route has a valid owner role.
- Every route has `agentDoc` or `coveredBy`.
- Every generated routing table has generated markers.
- No manual duplicate routing table remains outside manifest/generator control.
- No broken internal Markdown/path references under in-scope agent docs.
- No orphan role/workflow/domain docs unless marked `template`, `archive`, or `external-entry`.
- Deprecated aliases are generated compatibility shims or absent.
- Path references use real repo paths (`.agents/...`, `.pi/...`) rather than ambiguous `agents/...`/`pi/...` shorthand.

**Rationale**  
Addresses findings 03 sync gaps and findings 04 cross-reference failures: 256 broken references, 14 zero-inbound files, and repeated path-normalization issues.

**Alternatives considered**

- Manual checklist only.
- Link checker only.
- Extend only current `retired-routing-doc-sync.py` without schema v2.

**Tradeoff**  
Better validators require schema and migration work, but they prevent the same drift from recurring.

---

## Migration Principles for Later Phases

1. **Stop before changing tree shape.** This document requires operator ratification before prompt 06/Phase C.
2. **Move; do not copy.** Use compatibility stubs only where needed and mark them clearly.
3. **Close coverage before deleting wrappers.** `retired Pi skill mirror/` can only be removed after manifest/local docs/workflows cover its routes.
4. **Keep root docs thin and generated where possible.** Root `AGENTS.md` should route, not teach every domain.
5. **Make handoffs explicit.** Cross-role boundaries like `Actions/Intros/`, `Actions/LotAT/`, `Creative/WorldBuilding/`, and `Tools/LotAT/` need local handoff rules.
6. **Normalize links during moves.** Fix broken `agents/...`, `pi/...`, and shorthand paths as files move.
7. **Do not rename domain folders casually.** Normalize manifest IDs first; folder renames are separate operator decisions.

## Stop Condition

Hard stop resolved on 2026-04-26: the operator ratified all draft recommendations. Prompt 06 may proceed using the decisions recorded in **Operator Decisions Resolved**.
