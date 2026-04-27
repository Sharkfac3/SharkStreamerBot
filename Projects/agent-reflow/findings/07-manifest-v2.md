# Findings 07 — Manifest Schema v2

Generated: 2026-04-26
Source: prompt 07-manifest-schema-v2
Status: **DRAFT — manifest v2 created and schema validation passing**

## Inputs Read

- `Projects/agent-reflow/findings/05-target-shape.md`
- `Projects/agent-reflow/findings/06-naming-convention.md`
- `Projects/agent-reflow/findings/02-domain-coverage.md`
- `legacy v1 routing manifest (retired)`

## Final Locations

| Artifact | Location | Notes |
|---|---|---|
| JSON Schema | `.agents/manifest.schema.json` | Draft 2020-12 schema for manifest v2. |
| Manifest v2 | `.agents/manifest.json` | New v2 routing source. The old `legacy v1 routing manifest (retired)` remains untouched as the v1 reference until cutover. |
| Findings / handoff note | `Projects/agent-reflow/findings/07-manifest-v2.md` | This document. |

## Key Differences from v1 Manifest

- v1 separated `roles`, `canonical_subskills`, `helpers`, `quick_routing`, and `aliases`; v2 normalizes all routeable concepts into typed `skills`, plus dedicated `workflows`, `domains`, `co_locations`, and `aliases` sections.
- v2 introduces explicit `domains` routes for first-level `Actions/`, `Apps/`, `Tools/`, `Creative/`, and `Docs/Architecture/` folders, including planned local `AGENTS.md` locations.
- v2 collapses procedure-like subskills into workflows: `canon-guardian`, `change-summary`, `sync`, `validation`, and planned `coordination`.
- v2 records co-location targets so future validators can check where agent docs should live.
- v2 aliases point directly to final role/domain/workflow IDs instead of alias-to-canonical-wrapper chains.
- v2 includes a `migration.v1_entries` section proving every v1 entry is accounted for while the old manifest remains in place as reference.
- v2 uses kebab-case IDs and keeps human-readable folder paths unchanged.

## Schema Sections

| Section | Purpose |
|---|---|
| `skills` | All routeable skill-like identifiers: roles, domain routes, workflows, shared docs, and transitional meta helpers. |
| `workflows` | Repeatable procedures with path, owner, composed skills, applicability, and terminal status. |
| `domains` | Declared domain folders, owners, secondary owners, local agent doc path, coverage status, and workflows. |
| `co_locations` | Agent docs and compatibility docs with exact paths and owning routes. |
| `aliases` | Legacy identifiers mapped to final role/domain/workflow IDs for transition. |
| `quick_routing` | Generated/root-doc friendly quick routing rows. |
| `migration` | Every v1 manifest entry mapped to a v2 entry or compatibility target. |

## Validation

Schema validation command used:

```bash
python3 - <<'PY'
import json, jsonschema
schema=json.load(open('.agents/manifest.schema.json'))
data=json.load(open('.agents/manifest.json'))
jsonschema.Draft202012Validator.check_schema(schema)
errors=sorted(jsonschema.Draft202012Validator(schema).iter_errors(data), key=lambda e:e.path)
print('schema ok' if not errors else errors[0].message)
PY
```

Result: `schema ok`.

V1 coverage check result: `missing []`.

## Migration Mapping Table

Every entry in `legacy v1 routing manifest (retired)` is listed below. No v1 entries were removed.

| v1 type | v1 entry | v2 type | v2 entry | Status | Reason |
|---|---|---|---|---|---|
| `role` | `streamerbot-dev` | `role` | `streamerbot-dev` | migrated | Role preserved as v2 skill/role entry. |
| `role` | `lotat-tech` | `role` | `lotat-tech` | migrated | Role preserved as v2 skill/role entry. |
| `role` | `lotat-writer` | `role` | `lotat-writer` | migrated | Role preserved as v2 skill/role entry. |
| `role` | `art-director` | `role` | `art-director` | migrated | Role preserved as v2 skill/role entry. |
| `role` | `brand-steward` | `role` | `brand-steward` | migrated | Role preserved as v2 skill/role entry. |
| `role` | `content-repurposer` | `role` | `content-repurposer` | migrated | Role preserved as v2 skill/role entry. |
| `role` | `app-dev` | `role` | `app-dev` | migrated | Role preserved as v2 skill/role entry. |
| `role` | `product-dev` | `role` | `product-dev` | migrated | Role preserved as v2 skill/role entry. |
| `role` | `ops` | `role` | `ops` | migrated | Role preserved as v2 skill/role entry. |
| `canonical_subskill` | `brand-steward-canon-guardian` | `workflow` | `canon-guardian` | collapsed | V1 canonical subskill converted to workflow or domain route. |
| `canonical_subskill` | `brand-steward-content-strategy` | `domain` | `creative-marketing` | collapsed | V1 canonical subskill converted to workflow or domain route. |
| `canonical_subskill` | `content-repurposer-pipeline` | `domain` | `tools-content-pipeline` | migrated | V1 canonical subskill converted to workflow or domain route. |
| `canonical_subskill` | `lotat-writer-canon-guardian` | `workflow` | `canon-guardian` | collapsed | V1 canonical subskill converted to workflow or domain route. |
| `canonical_subskill` | `ops-change-summary` | `workflow` | `change-summary` | migrated | V1 canonical subskill converted to workflow or domain route. |
| `canonical_subskill` | `ops-sync` | `workflow` | `sync` | migrated | V1 canonical subskill converted to workflow or domain route. |
| `canonical_subskill` | `ops-validation` | `workflow` | `validation` | migrated | V1 canonical subskill converted to workflow or domain route. |
| `canonical_subskill` | `streamerbot-dev-commanders` | `domain` | `actions-commanders` | migrated | V1 canonical subskill converted to workflow or domain route. |
| `canonical_subskill` | `streamerbot-dev-lotat` | `domain` | `actions-lotat` | migrated | V1 canonical subskill converted to workflow or domain route. |
| `canonical_subskill` | `streamerbot-dev-squad` | `domain` | `actions-squad` | migrated | V1 canonical subskill converted to workflow or domain route. |
| `canonical_subskill` | `streamerbot-dev-twitch` | `domain` | `actions-twitch-core-integrations` | split | V1 canonical subskill converted to workflow or domain route. |
| `canonical_subskill` | `streamerbot-dev-voice-commands` | `domain` | `actions-voice-commands` | migrated | V1 canonical subskill converted to workflow or domain route. |
| `helper` | `meta` | `meta` | `meta` | compatibility | Transitional Pi helper retained until cutover. |
| `helper` | `meta-agents-navigate` | `meta` | `meta-agents-navigate` | compatibility | Transitional Pi helper retained until cutover. |
| `helper` | `meta-agents-update` | `meta` | `meta-agents-update` | compatibility | Transitional Pi helper retained until cutover. |
| `alias` | `change-summary` | `workflow` | `change-summary` | compatibility | v1 alias now points directly to workflow |
| `alias` | `sync-workflow` | `workflow` | `sync` | compatibility | v1 alias now points directly to workflow |
| `alias` | `brand-canon-guardian` | `workflow` | `canon-guardian` | compatibility | legacy Pi alias collapsed into shared canon workflow |
| `alias` | `content-strategy` | `domain` | `creative-marketing` | compatibility | legacy alias now routes to Creative/Marketing domain |
| `alias` | `feature-commanders` | `domain` | `actions-commanders` | compatibility | legacy feature alias now domain route |
| `alias` | `feature-squad` | `domain` | `actions-squad` | compatibility | legacy feature alias now domain route |
| `alias` | `feature-twitch-integration` | `domain` | `actions-twitch-core-integrations` | compatibility | broad Twitch alias defaults to core route; folder routes handle specific Twitch areas |
| `alias` | `feature-channel-points` | `domain` | `actions-twitch-channel-points` | compatibility | legacy feature alias now domain route |
| `alias` | `feature-hype-train` | `domain` | `actions-twitch-hype-train` | compatibility | legacy feature alias now domain route |
| `alias` | `feature-voice-commands` | `domain` | `actions-voice-commands` | compatibility | legacy feature alias now domain route |
| `alias` | `streamerbot-scripting` | `role` | `streamerbot-dev` | compatibility | legacy scripting alias collapsed to role |
| `alias` | `buildtools` | `role` | `ops` | compatibility | legacy buildtools alias collapsed to ops role |
| `alias` | `creative-art` | `domain` | `creative-art` | compatibility | legacy alias now explicit Creative/Art route |
| `alias` | `creative-worldbuilding` | `domain` | `creative-worldbuilding` | compatibility | legacy alias now explicit Creative/WorldBuilding route |
| `quick_routing` | `Any &#96;.cs&#96; script under &#96;Actions/&#96;` | `role` | `streamerbot-dev` | migrated | Quick routing row preserved as manifest quick_routing entry. |
| `quick_routing` | `LotAT C# engine / story pipeline` | `role` | `lotat-tech` | migrated | Quick routing row preserved as manifest quick_routing entry. |
| `quick_routing` | `LotAT adventure content / lore` | `role` | `lotat-writer` | migrated | Quick routing row preserved as manifest quick_routing entry. |
| `quick_routing` | `Art generation / diffusion prompts` | `role` | `art-director` | migrated | Quick routing row preserved as manifest quick_routing entry. |
| `quick_routing` | `Chat text, titles, canon, content strategy` | `role` | `brand-steward` | migrated | Quick routing row preserved as manifest quick_routing entry. |
| `quick_routing` | `Short-form clips, captions, platform formatting, or content-pipeline tooling` | `role` | `content-repurposer` | migrated | Quick routing row preserved as manifest quick_routing entry. |
| `quick_routing` | `Stream interaction apps` | `role` | `app-dev` | migrated | Quick routing row preserved as manifest quick_routing entry. |
| `quick_routing` | `Product docs, specs, knowledge articles, and customer-facing product content` | `role` | `product-dev` | migrated | Quick routing row preserved as manifest quick_routing entry. |
| `quick_routing` | `Validation, sync, change summary, tooling` | `role` | `ops` | migrated | Quick routing row preserved as manifest quick_routing entry. |
## Handoff

### State changes

- Created `.agents/manifest.schema.json`.
- Created `.agents/manifest.json`.
- Created `Projects/agent-reflow/findings/07-manifest-v2.md`.
- Did not delete or modify `legacy v1 routing manifest (retired)`.
- Did not edit `.pi/`, `.agents/roles/`, or domain folders.

### Validator status

- `.agents/manifest.json` validates against `.agents/manifest.schema.json`.
- Every v1 manifest role, canonical subskill, helper, quick-routing row, and alias is accounted for in `migration.v1_entries`.

### Open questions / blockers

- Until prompt 08 ships the validator tool, manifest v2 has no automated drift protection beyond ad hoc schema validation.
- Several `agentDoc` and workflow paths are marked `planned`; later migration prompts still need to create or move those docs.
- `legacy v1 routing manifest (retired)` remains the legacy reference until Phase F cutover decides whether to delete, replace, or generate it from v2.

### Next prompt entry point

Proceed to prompt 08 to implement automated validation for `.agents/manifest.json`, schema compliance, v1 migration coverage, domain coverage, and generated-doc drift checks.
