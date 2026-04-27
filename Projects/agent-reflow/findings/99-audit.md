# Prompt 99 — Optimization Audit Findings

Date: 2026-04-27  
Scope: read-only audit of manifest v2, migrated role/workflow skills, co-located `AGENTS.md` docs, and root documentation. Dependency/vendor docs under `node_modules/` and migration prompt/handoff files under `Projects/agent-reflow/` were not treated as active tree content for optimization findings.

Validation observed:

```text
python3 Tools/AgentTree/validate.py
Agent tree validation summary ... Total failures: 0
```

## Summary

| Severity | Recommendation count |
|---|---:|
| High | 3 |
| Medium | 11 |
| Low | 6 |
| Total | 20 |

Operator triage: for each recommendation, decide: **act now** (spawn follow-up prompt 100+), **defer to backlog**, or **dismiss**.

Disposition reminder: after operator finishes triage and any follow-ups, run final disposition for this migration workspace: delete `Projects/agent-reflow/` or move it to `Docs/Archive/`.

---

## 1. Concept-Boundary Review

### Finding 1.1 — `Actions/HELPER-SNIPPETS.md` bundles unrelated reusable concepts

- **Severity:** high
- **Evidence:** `Actions/HELPER-SNIPPETS.md` is 1,044 lines and its H2 sections span independent helper domains:
  - `Mini-game Lock Helper`
  - `Mix It Up Command API Helper`
  - `Chat Message Input Helper`
  - `OBS Scene Switching`
  - `Verified CPH API Method Signatures`
  - `Timer Management`
  - `JSON Parse / Serialize Helper`
  - `Required mini-game contract checklist`
- **Recommended action:** Split by concept, not line count. Suggested split points:
  1. `Actions/Helpers/mini-game-lock.md`
  2. `Actions/Helpers/mixitup-command-api.md` or a cross-link to `Tools/MixItUp/AGENTS.md`
  3. `Actions/Helpers/chat-input.md`
  4. `Actions/Helpers/obs-scenes.md`
  5. `Actions/Helpers/cph-api-signatures.md`
  6. `Actions/Helpers/timers.md`
  7. `Actions/Helpers/json-no-external-libraries.md`
  8. Move the mini-game contract checklist to `Actions/Squad/AGENTS.md` or a dedicated mini-game contract doc.

### Finding 1.2 — `Actions/SHARED-CONSTANTS.md` is a constants registry plus feature ownership map

- **Severity:** medium
- **Evidence:** `Actions/SHARED-CONSTANTS.md` is 504 lines and mixes shared OBS/source constants, per-feature constants, setup notes, and operational sync notes. H2 sections include `OBS`, `Stream Mode`, `Mini-game Lock`, each Squad game, `LotAT / Offering`, `Commanders`, `Rest / Focus Loop`, `Bits`, `Overlay / Broker`, `XJ Drivethrough`, `Destroyer`, `Info Service / Assets`, and `Operator Sync Notes`.
- **Recommended action:** Keep `Actions/SHARED-CONSTANTS.md` as the canonical index, but extract per-domain constant reference files beside local guides for dense areas: `Actions/LotAT/constants.md`, `Actions/Squad/constants.md`, `Actions/Overlay/constants.md`, and `Actions/Twitch*/constants.md`. Link back to the canonical index for global names.

### Finding 1.3 — `Actions/LotAT/README.md` combines operator setup, runtime contract, implementation wiring, and roadmap boundaries

- **Severity:** medium
- **Evidence:** `Actions/LotAT/README.md` is 507 lines. H2 sections include implementation scope, action inventory, trigger/input matrix, runtime globals, timer setup, story prerequisites, commander prerequisites, path caveat, limitations, offering boundary, and operator checklist.
- **Recommended action:** Preserve `Actions/LotAT/README.md` as the overview, then split:
  - `runtime-contract.md` for globals, timers, commands, story file contract, v1 boundaries.
  - `operator-setup.md` for exact Streamer.bot timers/triggers and checklist.
  - `implementation-map.md` for action inventory and internal wiring.

### Finding 1.4 — `Apps/stream-overlay/AGENTS.md` is a domain guide plus protocol and renderer reference

- **Severity:** medium
- **Evidence:** `Apps/stream-overlay/AGENTS.md` is 305 lines and includes agent routing, runtime notes, protocol details, asset system, rendering notes, build/test commands, and gotchas.
- **Recommended action:** Keep agent activation/routing in `AGENTS.md`; extract `protocol.md`, `asset-system.md`, and `rendering-notes.md` under `Apps/stream-overlay/docs/` or equivalent. Link the agent guide to those references.

### Finding 1.5 — LotAT writer experiment files are too coarse for active reuse

- **Severity:** medium
- **Evidence:**
  - `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` is 601 lines and mixes governance, tone, known cast, ship map, validation rules, story contract, commander moments, dice hooks, chaos, draft requirements, and output format.
  - `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` is 433 lines and mixes runtime principles, schema rules, engine architecture, presentation, persistence, expansion planning, and validation output.
- **Recommended action:** If these remain active references, promote and split stable content into `Creative/WorldBuilding/Franchises/StarshipShamples.md`, `Tools/LotAT/` schema/validation docs, and `Actions/LotAT/` runtime docs. If they are historical experiments, mark them explicitly as archival/non-load-bearing.

---

## 2. Deduplication Scan

### Finding 2.1 — Agent guide boilerplate repeats across every domain route

- **Severity:** medium
- **Evidence:** Most co-located `AGENTS.md` files repeat the same H2 framework: `Purpose`, `When to Activate`, `Primary Owner`, `Secondary Owners / Chain To`, `Required Reading`, `Local Workflow`, `Validation`, `Boundaries / Out of Scope`, and `Handoff Notes`. Duplicate clusters appear in `Actions/Commanders/AGENTS.md`, `Actions/Squad/AGENTS.md`, Twitch route docs, app docs, creative docs, and tool docs.
- **Recommended action:** Create a concise `.agents/_shared/domain-agent-doc-template.md` or document the template in `.agents/_shared/conventions.md`. Keep local guides focused on deviations and domain-specific facts; use the template as the single source for common section meanings.

### Finding 2.2 — Validation/migration command blocks are duplicated and stale-prone

- **Severity:** high
- **Evidence:** Repeated migration-era validation text appears in many agent docs, for example:
  - `Actions/Commanders/AGENTS.md:97`
  - `Actions/Squad/AGENTS.md:111`
  - `Actions/Twitch Bits Integrations/AGENTS.md:102`
  - `Actions/Twitch Channel Points/AGENTS.md:88`
  - `Actions/Twitch Core Integrations/AGENTS.md:106`
  - `Actions/Twitch Hype Train/AGENTS.md:101`
  - `Actions/Voice Commands/AGENTS.md:105`
  - `Tools/ArtPipeline/AGENTS.md:105`
  - `Tools/ContentPipeline/AGENTS.md:94`
  - `Tools/MixItUp/AGENTS.md:90`
  - `Tools/StreamerBot/AGENTS.md:78`
- **Recommended action:** Replace duplicated migration-specific validation language with a stable pointer to `.agents/workflows/validation.md` plus domain-specific commands only. The workflow should own any global validator selection guidance.

### Finding 2.3 — Art-agent style preambles are repeated

- **Severity:** low
- **Evidence:** `Creative/Art/Agents/captain-stretch-art-agent.md`, `Creative/Art/Agents/the-director-art-agent.md`, and `Creative/Art/Agents/water-wizard-art-agent.md` repeat the same `Style Reference` and background/detail requirement language pointing to `stream-style-art-agent.md`.
- **Recommended action:** Make `Creative/Art/Agents/stream-style-art-agent.md` the explicit base style contract and reduce character files to character-specific deltas plus one standard link.

### Finding 2.4 — Art pipeline full-run instructions are duplicated for agent and human audiences

- **Severity:** low
- **Evidence:** Similar dry-run and `commanders-redemption-v1` command sequences exist in:
  - `Tools/ArtPipeline/FULL-RUN.md`
  - `humans/art-pipeline/FULL-RUN.md`
  - `Tools/ArtPipeline/AGENTS.md`
- **Recommended action:** Choose `Tools/ArtPipeline/FULL-RUN.md` as the technical source and make the human doc a simplified wrapper. Keep `Tools/ArtPipeline/AGENTS.md` to routing plus links.

---

## 3. Cross-Reference Clustering

### Finding 3.1 — LotAT docs form a strong reference triangle across runtime, tooling, and worldbuilding

- **Severity:** medium
- **Evidence:** Mutual/coupled references exist among:
  - `Actions/LotAT/AGENTS.md`
  - `Tools/LotAT/AGENTS.md`
  - `Creative/WorldBuilding/AGENTS.md`
  - plus `Apps/stream-overlay/AGENTS.md` for presentation handoff.
- **Recommended action:** Extract a single LotAT contract index, e.g. `Docs/Architecture/lotat-contract.md` or `Creative/WorldBuilding/Franchises/StarshipShamples.md` plus explicit runtime/tooling sections. Each domain guide should link to that index for shared contract boundaries and keep only local responsibilities.

### Finding 3.2 — Info-service, production-manager, and intros are tightly coupled

- **Severity:** medium
- **Evidence:** Mutual/coupled references appear between:
  - `Actions/Intros/AGENTS.md`
  - `Apps/info-service/AGENTS.md`
  - `Apps/info-service/INFO-SERVICE-PLAN.md`
  - `Apps/production-manager/AGENTS.md`
  - `Apps/production-manager/PRODUCTION-MANAGER-GUIDE.md`
- **Recommended action:** Co-locate or extract a shared `Docs/Architecture/custom-intros-contract.md` covering collection names, REST contract, Streamer.bot action expectations, and UI ownership. Leave app/action guides as implementation-specific entry points.

### Finding 3.3 — Workflow files are densely cross-linked

- **Severity:** low
- **Evidence:** Mutual links exist among `.agents/workflows/change-summary.md`, `coordination.md`, `sync.md`, and `validation.md`; root entry docs also link back into coordination.
- **Recommended action:** Do not merge automatically. Add a short `.agents/workflows/README.md` or workflow index if operators want a single overview. This would reduce directory-link ambiguity and explain load order.

---

## 4. Naming Consistency

### Finding 4.1 — Manifest status values disagree with co-located agent-doc front matter

- **Severity:** high
- **Evidence:** Manifest v2 still marks these skills/domains as `planned`, while the corresponding `AGENTS.md` front matter says `status: active` and the files contain real migrated guidance:
  - `Actions/Destroyer/AGENTS.md`
  - `Actions/Intros/AGENTS.md`
  - `Actions/Rest Focus Loop/AGENTS.md`
  - `Actions/Temporary/AGENTS.md`
  - `Actions/XJ Drivethrough/AGENTS.md`
  - `Apps/info-service/AGENTS.md`
  - `Apps/production-manager/AGENTS.md`
  - `Tools/LotAT/AGENTS.md`
- **Recommended action:** Spawn a manifest normalization follow-up to decide whether these routes are truly active/covered or planned, then align `skills`, `domains`, and `co_locations` status fields with front matter.

### Finding 4.2 — Folder casing/spacing convention remains mixed for human-named action folders

- **Severity:** low
- **Evidence:** Manifest ids use slug casing (`actions-rest-focus-loop`, `actions-twitch-channel-points`), while paths retain spaces and title casing (`Actions/Rest Focus Loop/`, `Actions/Twitch Channel Points/`, `Actions/XJ Drivethrough/`). This is likely intentional for existing Streamer.bot folder names but increases link escaping (`Rest%20Focus%20Loop`).
- **Recommended action:** Document this as an explicit exception in `.agents/_shared/conventions.md`: manifest ids are lowercase slugs; legacy Streamer.bot folder paths may keep operator-facing names and spaces.

### Finding 4.3 — Root link text sometimes names broad folders without an index file

- **Severity:** low
- **Evidence:** `AGENTS.md` and `.agents/ENTRY.md` link to folder roots such as `Actions/`, `Apps/`, `Docs/`, `.agents/`, and `.agents/workflows/`. The validator accepts directory links, but readers may expect a folder README.
- **Recommended action:** Either add lightweight folder index docs where missing (`Actions/README.md`, `Apps/README.md`, `.agents/workflows/README.md`) or change link text to clarify that the target is a folder, not a document.

---

## 5. Skill Granularity

### Finding 5.1 — `apps-stream-overlay` skill is too coarse for current content

- **Severity:** medium
- **Evidence:** `Apps/stream-overlay/AGENTS.md` covers broker protocol, overlay rendering, asset conventions, LotAT presentation, Squad rendering, build/test commands, and app ownership in one 305-line skill guide.
- **Recommended action:** Split into a thin domain route plus subreferences: broker/protocol, overlay renderer, asset registry, and feature renderers. Keep the manifest skill as the route entry point.

### Finding 5.2 — `tools-mix-it-up` skill is broad but internally has natural subdomains

- **Severity:** medium
- **Evidence:** `Tools/MixItUp/AGENTS.md` covers API helpers, commands, overlays, shared payload references, placeholder/command IDs, Streamer.bot integration boundaries, and app integration boundaries. The folder already has `Api/`, `Commands/`, `Overlays/`, and `Shared/` README files.
- **Recommended action:** Keep `tools-mix-it-up` as the skill route but push subdomain facts to the existing subfolder READMEs. The AGENTS doc should become an ownership and routing shell.

### Finding 5.3 — Fine-grained Twitch route skills may always load as a cluster

- **Severity:** low
- **Evidence:** `Actions/Twitch Bits Integrations/AGENTS.md`, `Actions/Twitch Channel Points/AGENTS.md`, `Actions/Twitch Core Integrations/AGENTS.md`, and `Actions/Twitch Hype Train/AGENTS.md` share owner, workflows, brand handoff, Mix It Up concerns, and validation pattern.
- **Recommended action:** Do not merge unless operator friction confirms it. Consider adding `Actions/Twitch/README.md` as a cluster index that points to each route and centralizes common Twitch/Mix It Up conventions.

---

## 6. Stale Content

### Finding 6.1 — Migration-era references remain in active workflow and agent docs

- **Severity:** medium
- **Evidence:** Active docs still mention migration phase assumptions, including:
  - `.agents/workflows/validation.md:43`, `:59`, `:81`, `:93-95` reference `Projects/agent-reflow/findings/08-validator.md`, migration prompts, and Phase E expected failures.
  - `Tools/AgentTree/AGENTS.md:78`, `:94`, `:129`, `:131` reference prompt-specific migration validation and legacy routing.
  - Multiple `Actions/*/AGENTS.md` files say not to delete old central skill files during migration even though cutover is complete.
- **Recommended action:** Spawn a post-cutover cleanup prompt to remove or archive migration-era instructions from active docs. Keep historical context only in `Projects/agent-reflow/` or `Docs/Archive/`.

### Finding 6.2 — Deleted `retired Pi skill mirror/` is still linked from shared conventions

- **Severity:** medium
- **Evidence:** `.agents/_shared/conventions.md:25` links to `../../retired Pi skill mirror/` as transitional Pi wrappers. That folder was deleted during cutover. Independent link scan found this as the only truly nonexistent internal link outside migration files.
- **Recommended action:** Remove the row or replace it with a historical note that the mirror was retired at cutover. This is safe to prioritize because it is a direct stale reference to a deleted path.

### Finding 6.3 — Active manifest retains large migration/compatibility payload

- **Severity:** medium
- **Evidence:** `.agents/manifest.json` includes extensive `aliases` and `migration.v1_entries` sections with deprecated/compatibility references to v1 ids, old helpers, and legacy aliases.
- **Recommended action:** Decide whether manifest v2 should remain a migration ledger. If not, move historical mapping to `Projects/agent-reflow/findings/07-manifest-v2.md` or an archive doc, and keep the active manifest to current routing plus any compatibility aliases still needed by tooling.

### Finding 6.4 — Test art project appears orphaned and date/status unclear

- **Severity:** low
- **Evidence:** `Creative/Art/Projects/test-v1-captain-stretch-redemption-overlay.md` has no inbound references from active docs in the scan and is named as a test/v1 artifact.
- **Recommended action:** Decide whether it is an approved reference, a sample output, or disposable test residue. Add a link from `Creative/Art/README.md`/`AGENTS.md` if retained, or archive/delete in a follow-up.

---

## 7. Reference Integrity (Final Pass)

### Finding 7.1 — Validator is green, but general Markdown link scan still finds one deleted-path reference

- **Severity:** medium
- **Evidence:** `python3 Tools/AgentTree/validate.py` reports zero failures. A broader Markdown scan that includes shared docs found `.agents/_shared/conventions.md` linking to `../../retired Pi skill mirror/`, which no longer exists.
- **Recommended action:** Either expand `Tools/AgentTree/validate.py` link coverage to include `.agents/_shared/*.md`, or fix the stale link directly in the stale-content cleanup follow-up.

### Finding 7.2 — No actionable circular load-chain problem found

- **Severity:** low
- **Evidence:** Mutual links exist among root entry, coordination, and workflow docs, plus role/workflow pairs. These are navigational references rather than mandatory recursive load instructions. Validator orphan check passes.
- **Recommended action:** No merge required. If future agents over-load workflows recursively, add explicit `load once` language to `.agents/ENTRY.md` or workflow index.

---

## 8. Workflow vs Domain Knowledge Separation

### Finding 8.1 — Validation workflow still contains migration baseline/domain-era expectations

- **Severity:** medium
- **Evidence:** `.agents/workflows/validation.md` includes prompt/migration-specific guidance and an expected-failure baseline for Phase E. That is historical project state, not reusable validation workflow logic.
- **Recommended action:** Move migration-era expected failures and prompt handoff details to archived migration docs. Keep `.agents/workflows/validation.md` focused on choosing checks, running validators, reporting exit codes, and escalating failures.

### Finding 8.2 — Several domain agent docs contain workflow procedure rather than domain facts

- **Severity:** medium
- **Evidence:** Many domain `AGENTS.md` files duplicate procedural validation instructions, migration acceptance commands, and handoff language. Examples include `Actions/Commanders/AGENTS.md`, `Actions/Squad/AGENTS.md`, `Tools/ArtPipeline/AGENTS.md`, `Tools/MixItUp/AGENTS.md`, and `Tools/StreamerBot/AGENTS.md`.
- **Recommended action:** Keep only domain-specific validation commands or paste targets in local docs. Move common procedure to `.agents/workflows/validation.md`, `.agents/workflows/sync.md`, and `.agents/workflows/change-summary.md`.

---

## 9. Co-location Coverage

### Finding 9.1 — Manifest co-location coverage exists and validator passes, but manifest status drift weakens coverage semantics

- **Severity:** medium
- **Evidence:** Validator checks pass for folder coverage, stub presence, naming, and orphan checks. However, manifest statuses still say `planned` for several domains that now have active `AGENTS.md` guides, including `Actions/Destroyer`, `Actions/Intros`, `Apps/info-service`, and `Tools/LotAT`.
- **Recommended action:** Treat status normalization as the primary co-location follow-up. After status alignment, rerun `python3 Tools/AgentTree/validate.py` and verify generated quick-routing drift remains zero.

### Finding 9.2 — Some active linked folders intentionally lack local index docs

- **Severity:** low
- **Evidence:** Co-located docs link to folder roots that exist but do not have local `README.md`/`AGENTS.md` index docs in some cases, such as `.agents/workflows/`, `Actions/Intros/`, `Actions/Temporary/`, `Actions/XJ Drivethrough/`, and several creative subfolders (`Creative/Art/Agents/`, `Creative/Art/Projects/`, `Creative/Marketing/Social/`, `Creative/WorldBuilding/Storylines/`).
- **Recommended action:** Decide convention: either directory links are acceptable and need no index, or every linked domain subfolder should have a tiny README. If choosing the latter, spawn targeted coverage prompts for high-traffic folders first.

---

## Recommended Follow-Up Prompt Candidates

1. **100-manifest-status-normalization** — align manifest `planned` vs active doc statuses.
2. **101-post-cutover-stale-cleanup** — remove Phase E/migration/retired mirror references from active docs.
3. **102-actions-helper-snippets-split-design** — design conceptual split for `Actions/HELPER-SNIPPETS.md` without changing code snippets.
4. **103-lotat-contract-index** — create/choose a shared LotAT contract index and reduce cross-domain duplication.
5. **104-apps-stream-overlay-doc-split** — extract protocol/assets/rendering details from `Apps/stream-overlay/AGENTS.md`.
