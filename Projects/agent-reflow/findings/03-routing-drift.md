# Prompt 03 findings — routing drift inventory

Date: 2026-04-26  
Agent: pi

## Scope and inputs

Read-only inventory of routing-table-like sources. No routing source files were edited and the sync script was not run.

Compared sources:

- `legacy v1 routing manifest (retired)` — declared canonical source
- `.agents/ENTRY.md` — `## Roles` table
- `AGENTS.md` — `## Quick Role Routing` generated block
- `retired Pi skill mirror/README.md` — `## Roles`, `## Routing Table`, `## Meta Wrappers`, `## Compatibility Aliases`
- `Tools/StreamerBot/Validation/retired-routing-doc-sync.py` — sync implementation

Prior handoffs read first:

- `Projects/agent-reflow/handoffs/00-inventory-agents-tree.handoff.md`
- `Projects/agent-reflow/handoffs/01-inventory-pi-mirror.handoff.md`

## Executive summary

- Role set is consistent across manifest, `.agents/ENTRY.md`, `AGENTS.md`, and `retired Pi skill mirror/README.md`: all list the same 9 roles.
- Severe canonical ambiguity: `app-dev` activation text differs between the declared canonical manifest and both `.agents/ENTRY.md` and `retired Pi skill mirror/README.md`.
- `.agents/ENTRY.md` and `retired Pi skill mirror/README.md` `## Roles` tables are identical today, including the same `app-dev` drift from manifest.
- `AGENTS.md` quick routing is not a role activation table; it contains shorter operator routing phrases from manifest `quick_routing`. All rows differ from the full role activation text by design, but that creates duplicated routing semantics.
- `retired Pi skill mirror/README.md` has a manual `## Routing Table` that is not synced by `retired-routing-doc-sync.py`; it duplicates and expands role/sub-skill activation logic.
- Manifest `canonical_children` are all represented somewhere in `retired Pi skill mirror/README.md` routing rows, but not as a generated or explicit child list.

## Role list by source

| Source | Roles found |
|---|---|
| `legacy v1 routing manifest (retired)` `roles[]` | `streamerbot-dev`, `lotat-tech`, `lotat-writer`, `art-director`, `brand-steward`, `content-repurposer`, `app-dev`, `product-dev`, `ops` |
| `.agents/ENTRY.md` `## Roles` | `streamerbot-dev`, `lotat-tech`, `lotat-writer`, `art-director`, `brand-steward`, `content-repurposer`, `app-dev`, `product-dev`, `ops` |
| `AGENTS.md` `## Quick Role Routing` | `streamerbot-dev`, `lotat-tech`, `lotat-writer`, `art-director`, `brand-steward`, `content-repurposer`, `app-dev`, `product-dev`, `ops` |
| `retired Pi skill mirror/README.md` `## Roles` | `streamerbot-dev`, `lotat-tech`, `lotat-writer`, `art-director`, `brand-steward`, `content-repurposer`, `app-dev`, `product-dev`, `ops` |
| `retired Pi skill mirror/README.md` `## Routing Table` | References the same 9 roles, plus canonical sub-skill wrappers and Pi meta wrappers |

No role is absent from any role-list source.

## Activation comparison matrix

Legend:

- `OK` = exact match to manifest role `when` text.
- `DRIFT` = differs from manifest role `when` text.
- `N/A quick` = `AGENTS.md` uses manifest `quick_routing.work`, not role `when`; included because it is still routing text users see.

| Role | Manifest `when` | `.agents/ENTRY.md` role text | `AGENTS.md` quick routing text | `retired Pi skill mirror/README.md` role text | `retired Pi skill mirror/README.md` routing-table task text |
|---|---|---|---|---|---|
| `streamerbot-dev` | Any `.cs` script work under `Actions/` | OK — Any `.cs` script work under `Actions/` | DRIFT / N/A quick — Any `.cs` script under `Actions/` | OK — Any `.cs` script work under `Actions/` | DRIFT — Any `.cs` script work; plus sub-routes for Squad, Commanders, Twitch, Voice, LotAT C# engine |
| `lotat-tech` | LotAT story pipeline — C# engine, JSON schema, technical implementation | OK — LotAT story pipeline — C# engine, JSON schema, technical implementation | DRIFT / N/A quick — LotAT C# engine / story pipeline | OK — LotAT story pipeline — C# engine, JSON schema, technical implementation | DRIFT — LotAT story JSON or schema; also secondary load for LotAT C# engine schema/pipeline context |
| `lotat-writer` | LotAT narrative — adventure design, lore, worldbuilding, story content | OK — LotAT narrative — adventure design, lore, worldbuilding, story content | DRIFT / N/A quick — LotAT adventure content / lore | OK — LotAT narrative — adventure design, lore, worldbuilding, story content | DRIFT — Write a new LotAT adventure; Review LotAT story for canon |
| `art-director` | Diffusion model prompts, character art, stream visuals | OK — Diffusion model prompts, character art, stream visuals | DRIFT / N/A quick — Art generation / diffusion prompts | OK — Diffusion model prompts, character art, stream visuals | DRIFT — Art generation / diffusion prompts |
| `brand-steward` | Any public-facing output — chat text, titles, marketing, canon review | OK — Any public-facing output — chat text, titles, marketing, canon review | DRIFT / N/A quick — Chat text, titles, canon, content strategy | OK — Any public-facing output — chat text, titles, marketing, canon review | DRIFT — Chat bot text, stream titles, announcements; Canon audit; Story tied to a specific build session |
| `content-repurposer` | Short-form content repurposing — clip selection, captions, content calendars, platform formatting, and content-pipeline tooling | OK — Short-form content repurposing — clip selection, captions, content calendars, platform formatting, and content-pipeline tooling | DRIFT / N/A quick — Short-form clips, captions, platform formatting, or content-pipeline tooling | OK — Short-form content repurposing — clip selection, captions, content calendars, platform formatting, and content-pipeline tooling | DRIFT — Short-form clips, captions, and content calendars; Content-pipeline tooling in `Tools/ContentPipeline/` |
| `app-dev` | Stream interaction apps (expanding) | **DRIFT — Stream overlay ecosystem (broker, Phaser overlay, web apps) — TypeScript under `Apps/`** | DRIFT / N/A quick — Stream interaction apps | **DRIFT — Stream overlay ecosystem (broker, Phaser overlay, web apps) — TypeScript under `Apps/`** | DRIFT — Standalone stream interaction app work; info-service lookup, collection queries; production-manager admin UI |
| `product-dev` | Product documentation, technical knowledge articles, specifications, and future customer-facing content for stream-developed R&D products | OK — Product documentation, technical knowledge articles, specifications, and future customer-facing content for stream-developed R&D products | DRIFT / N/A quick — Product docs, specs, knowledge articles, and customer-facing product content | OK — Product documentation, technical knowledge articles, specifications, and future customer-facing content for stream-developed R&D products | DRIFT — Product docs, specs, knowledge articles, and customer-facing product content |
| `ops` | Validation, sync workflow, change summaries, tooling | OK — Validation, sync workflow, change summaries, tooling | DRIFT / N/A quick — Validation, sync, change summary, tooling | OK — Validation, sync workflow, change summaries, tooling | DRIFT — Sync to Streamer.bot; After any code change; Run validation |

## Drift notes by source

### `.agents/ENTRY.md`

- Only role activation drift from manifest: `app-dev`.
- Exact table text says: `Stream overlay ecosystem (broker, Phaser overlay, web apps) — TypeScript under Apps/`.
- Manifest says: `Stream interaction apps (expanding)`.
- This is severe because `.agents/ENTRY.md` is the universal entry point and contradicts the declared canonical manifest.

### `AGENTS.md`

- Role set matches manifest.
- Every activation phrase differs from manifest `roles[].when` because the table is generated from manifest `quick_routing[].work`, not `roles[].when`.
- This is not necessarily stale, but it is a second canonical-ish vocabulary operators will see.
- It is synced by the script and wrapped in generated markers.

### `retired Pi skill mirror/README.md` `## Roles`

- Same status as `.agents/ENTRY.md`.
- Only direct role-table drift from manifest: `app-dev`.
- The README claims: `This table is sourced from the routing contract in legacy v1 routing manifest (retired) and machine-validated against .agents/ENTRY.md.` That claim is currently false for `app-dev` if the manifest is canonical, unless the sync script has not been run after manifest changed.

### `retired Pi skill mirror/README.md` `## Routing Table`

- Manual table, not generated by current sync script.
- It expands routing into task-level and sub-skill-level activation text.
- It is useful operationally but duplicates role/sub-skill routing semantics that are partly also in manifest `roles`, `canonical_children`, `helpers`, and wrapper descriptions.

## Canonical children / sub-skill coverage

Manifest `canonical_children` vs `retired Pi skill mirror/README.md` routing-table references:

| Owner role | Manifest `canonical_children` | Referenced in `retired Pi skill mirror/README.md` routing table? | Notes |
|---|---|---|---|
| `streamerbot-dev` | `streamerbot-dev-commanders`, `streamerbot-dev-lotat`, `streamerbot-dev-squad`, `streamerbot-dev-twitch`, `streamerbot-dev-voice-commands` | Yes, all 5 | README has task rows for Commander, LotAT C# engine, Squad, Twitch, Voice. |
| `lotat-tech` | none | N/A | README has role route rows only. |
| `lotat-writer` | `lotat-writer-canon-guardian` | Yes | README has canon review row. |
| `art-director` | none | N/A | README has role route row only. |
| `brand-steward` | `brand-steward-canon-guardian`, `brand-steward-content-strategy` | Yes, both | README has canon audit and build-session story rows. |
| `content-repurposer` | `content-repurposer-pipeline` | Yes | README has content-pipeline tooling row. |
| `app-dev` | none | N/A | README has expanded app task rows not represented as canonical children. |
| `product-dev` | none | N/A | README has role route row only. |
| `ops` | `ops-change-summary`, `ops-sync`, `ops-validation` | Yes, all 3 | `ops-change-summary` is loaded directly as terminal, not via `ops/SKILL.md`. |

Manifest `canonical_subskills[]` contains the same 12 canonical sub-skill wrappers implied by the role `canonical_children` lists. `retired Pi skill mirror/README.md` does not contain an explicit generated child-list table; coverage is inferred from the manual `## Routing Table`.

## Meta/helper and alias routing data

| Data set | Manifest | `retired Pi skill mirror/README.md` | Other source presence | Sync status |
|---|---|---|---|---|
| Helpers / meta wrappers | `helpers[]` with purpose and children | `## Meta Wrappers` table | `AGENTS.md` prose mentions Pi meta-skills; wrapper files also exist | Synced into README `## Meta Wrappers` only |
| Compatibility aliases | `aliases[]` | `## Compatibility Aliases` table | Alias wrapper files exist under `retired Pi skill mirror/` | Synced into README `## Compatibility Aliases` only |
| Helper `canonical_children` | `meta` has `meta-agents-navigate`, `meta-agents-update` | Implied by routing table and prose | Wrapper files | Not rendered as explicit child list |

## `retired-routing-doc-sync.py` coverage

The script reads `legacy v1 routing manifest (retired)` and constructs:

- `role_table` from `manifest["roles"]`
- `meta_table` from `manifest["helpers"]`
- `alias_table` from `manifest["aliases"]`
- `quick_routing_table` from `manifest["quick_routing"]`

It writes:

| Target | What is synced | How |
|---|---|---|
| `AGENTS.md` | `## Quick Role Routing` table | Replaces generated block named `agents-quick-role-routing` |
| `.agents/ENTRY.md` | `## Roles` table | Replaces first markdown table under heading `## Roles` |
| `retired Pi skill mirror/README.md` | `## Roles` table | Replaces first markdown table under heading `## Roles` |
| `retired Pi skill mirror/README.md` | `## Meta Wrappers` table | Replaces first markdown table under heading `## Meta Wrappers` |
| `retired Pi skill mirror/README.md` | `## Compatibility Aliases` table | Replaces first markdown table under heading `## Compatibility Aliases` |

Explicit sync gaps:

- Does **not** sync `retired Pi skill mirror/README.md` `## Routing Table`.
- Does **not** render manifest `canonical_children` or `canonical_subskills[]` into a dedicated table.
- Does **not** sync `retired Pi skill mirror/*/SKILL.md` wrapper descriptions or parent-role wrapper sub-skill lists.
- Does **not** sync `.agents/roles/*/role.md` trigger conditions or skill load orders.
- Does **not** sync narrative prose in `AGENTS.md`, `.agents/ENTRY.md`, or `retired Pi skill mirror/README.md` outside the targeted tables.
- Does **not** place generated markers around `.agents/ENTRY.md` or `retired Pi skill mirror/README.md` tables; those replacements are heading-based.

Important implication: If the script is run now, it should overwrite the `app-dev` role text in `.agents/ENTRY.md` and `retired Pi skill mirror/README.md` with manifest text `Stream interaction apps (expanding)`, unless the manifest is updated first.

## Generated marker inventory

| File | Markers found | Wrapped content |
|---|---|---|
| `AGENTS.md` | `<!-- GENERATED:agents-quick-role-routing:start -->` through `<!-- GENERATED:agents-quick-role-routing:end -->` | The entire `## Quick Role Routing` markdown table: header plus 9 quick-routing rows. |
| `.agents/ENTRY.md` | none | `## Roles` table is syncable by heading replacement, but not visibly marked generated. |
| `retired Pi skill mirror/README.md` | none | `## Roles`, `## Meta Wrappers`, and `## Compatibility Aliases` are syncable by heading replacement, but not visibly marked generated. `## Routing Table` is manual. |
| `Tools/StreamerBot/Validation/retired-routing-doc-sync.py` | template strings only | Contains the generated-block regex/template implementation. |

## Routing data duplication map

| Routing data | Sources where it lives | Count | Notes |
|---|---:|---:|---|
| Role list and full `when` activation text | Manifest `roles[]`, `.agents/ENTRY.md ## Roles`, `retired Pi skill mirror/README.md ## Roles` | 3 | Intended to be synced; currently `app-dev` drift shows sync is stale or manifest is stale. |
| Quick role routing text | Manifest `quick_routing[]`, `AGENTS.md ## Quick Role Routing` | 2 | Script syncs this generated block. Distinct shorter text from role `when`. |
| Pi task/sub-skill routing table | `retired Pi skill mirror/README.md ## Routing Table` | 1 | Manual-only, high drift risk because it duplicates role and child activation semantics. |
| Canonical role child lists | Manifest `roles[].canonical_children` | 1 explicit | README routing table implies all children but does not list them explicitly. |
| Canonical sub-skill inventory | Manifest `canonical_subskills[]` | 1 explicit | Wrapper files and README routes mirror it indirectly. |
| Meta wrappers | Manifest `helpers[]`, `retired Pi skill mirror/README.md ## Meta Wrappers`, `AGENTS.md` prose, `retired Pi skill mirror/meta*` wrappers | 4 | Only README Meta Wrappers table is script-synced. |
| Compatibility aliases | Manifest `aliases[]`, `retired Pi skill mirror/README.md ## Compatibility Aliases`, alias wrapper files | 3 | Only README alias table is script-synced. |
| Domain routing (`Actions`, `Tools`, etc.) | `AGENTS.md ## Project Domains`, likely shared docs | 1 in this prompt scope | Not manifest-backed. |
| Key reference routing | `AGENTS.md ## Key References` | 1 | Not manifest-backed. |

## Operator flags

1. **Ambiguous canonical answer: `app-dev`**
   - Manifest: `Stream interaction apps (expanding)`
   - `.agents/ENTRY.md` and `retired Pi skill mirror/README.md`: `Stream overlay ecosystem (broker, Phaser overlay, web apps) — TypeScript under Apps/`
   - `AGENTS.md` quick route: `Stream interaction apps`
   - Since the manifest is declared canonical, the docs are stale by contract. But the docs are more specific and may reflect newer intent. Operator should decide which text is canonical before re-running sync.

2. **Manual Pi routing table is the largest unsynced duplication point**
   - It is the only place with task-level routes for app-dev internals (`info-service`, `production-manager`) and many sub-skill trigger phrases.
   - If manifest v2 is planned, this table should probably be generated from explicit route/task entries.

3. **Generated-marker inconsistency**
   - `AGENTS.md` clearly marks generated quick routing.
   - `.agents/ENTRY.md` and `retired Pi skill mirror/README.md` are machine-edited but lack generated markers, making manual edits appear safe when they may be overwritten.

## Acceptance checklist

- [x] Extracted role list from every requested source.
- [x] Extracted activation / when-to-use text from every requested source.
- [x] Extracted `canonical_children` / sub-skill coverage from manifest and `retired Pi skill mirror/README.md`.
- [x] Built role comparison matrix with drift cells marked.
- [x] Read and summarized `retired-routing-doc-sync.py` behavior.
- [x] Documented generated markers and wrapped content.
- [x] Identified single-source vs duplicated routing data.
