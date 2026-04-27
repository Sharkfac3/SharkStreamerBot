# Findings 06 — Naming Convention

Generated: 2026-04-26  
Source: prompt 06-design-naming-convention  
Status: **RATIFIED — operator delegated review/unblock decisions to Pi; prompt 07 may proceed**

## Inputs Read

- `Projects/agent-reflow/findings/05-target-shape.md`
- `Projects/agent-reflow/handoffs/05-design-target-shape.handoff.md`
- `Projects/agent-reflow/findings/00-current-tree.md`
- `Projects/agent-reflow/findings/01-pi-mirror.md`
- Current filesystem inventory of `.agents/roles/*/skills/*/`
- Current filesystem inventory of `retired Pi skill mirror/*/`

## Operator Decisions Resolved

Operator instruction after draft: "do what you think is best to handle the review and unblock prompt 7." Pi resolved the pending review items as follows:

1. **RATIFIED — Workflow IDs:** no `wf-` prefix. Workflows are distinguished by manifest namespace and live at `.agents/workflows/<workflow-id>.md`.
2. **RATIFIED — Co-located agent doc filename:** `AGENTS.md` is the canonical local/domain agent doc filename.
3. **RATIFIED — Deprecated entry files:** `_index.md` and `retired Pi skill mirror/<wrapper>/SKILL.md` are compatibility/migration sources only, not target entrypoints.
4. **RATIFIED — Camel/Pascal path normalization:** split CamelCase/PascalCase folder names into readable kebab-case route IDs. Examples: `Tools/ArtPipeline/` → `tools-art-pipeline`, `Tools/ContentPipeline/` → `tools-content-pipeline`.
5. **RATIFIED — Prompt 07 unblock:** prompt 07 should treat this file as ratified and use the conventions below.

## Naming Rules

### 1. Identifier namespaces

Use explicit namespaces in manifest v2. IDs are stable strings; paths are separately declared.

| Namespace | Pattern | Examples | Notes |
|---|---|---|---|
| Role ID | `^[a-z][a-z0-9]*(?:-[a-z0-9]+)*$` | `streamerbot-dev`, `brand-steward` | Must match `.agents/roles/<role>/` folder name exactly. |
| Domain route ID | `^(actions|apps|tools|creative|docs|root)-[a-z0-9]+(?:-[a-z0-9]+)*$` | `actions-rest-focus-loop`, `apps-info-service` | Schema-defined; derived from repo path but does not need to match filesystem folder names exactly. |
| Workflow ID | `^[a-z][a-z0-9]*(?:-[a-z0-9]+)*$` | `change-summary`, `canon-guardian` | No `wf-` prefix. Type/namespace distinguishes workflows from skills/routes. |
| Compatibility alias ID | `^[a-z][a-z0-9]*(?:-[a-z0-9]+)*$` | `feature-squad`, `sync-workflow` | Deprecated IDs only; never add for new work unless required for cutover compatibility. |
| Helper/tooling ID | `^[a-z][a-z0-9]*(?:-[a-z0-9]+)*$` | `meta-agents-navigate` | Transitional/Pi-only until cutover. Prefer workflows or ops docs for new reusable procedures. |

Allowed characters for all IDs: lowercase ASCII letters, digits, and single hyphens. IDs must not contain spaces, underscores, slashes, dots, leading/trailing hyphens, or repeated hyphens.

### 2. Skill identifiers

**Canonical rule:** do not create new flat Pi-style skill identifiers for domain work.

In manifest v2, what used to be called a skill becomes one of:

1. a **role** (`roles.<role-id>`),
2. a **domain route** (`domainRoutes[].id`),
3. a **workflow** (`workflows.<workflow-id>`), or
4. a deprecated **compatibility alias** (`compatibilityAliases.<alias-id>`).

When a skill-like concept must be referred to generically, use the manifest ID in backticks and include its type in nearby prose: e.g. `actions-squad` route, `change-summary` workflow, `streamerbot-dev` role.

### 3. Skill folder names

**Target rule:** no new `.agents/roles/<role>/skills/<feature>/` folders.

- Domain/runtime knowledge moves to local domain docs such as `Actions/Squad/AGENTS.md` or `Apps/info-service/AGENTS.md`.
- Reusable procedures move to `.agents/workflows/<workflow-id>.md`.
- Role-level identity stays in `.agents/roles/<role>/role.md` after the `role.md` + `core.md` collapse.
- Existing skill folders may remain only as migration sources or temporary compatibility stubs. If a temporary central skill folder remains, its folder name should stay unchanged until its content is moved; do not rename historical folders just to match a new route ID.

### 4. Skill entry file

For the target shape, the entry file is **not** `_index.md`, `README.md`, or `SKILL.md`.

Use:

| Content type | Canonical entry file |
|---|---|
| Domain/local agent guidance | `AGENTS.md` in the domain folder |
| Role overview | `.agents/roles/<role>/role.md` |
| Reusable workflow | `.agents/workflows/<workflow-id>.md` |
| Root universal entry | `AGENTS.md` plus `.agents/ENTRY.md` generated/manifest-backed routing summary |
| Pi transitional wrapper | `retired Pi skill mirror/<wrapper>/SKILL.md` only until cutover |

`_index.md` is deprecated for new content. Existing `_index.md` files are migration sources and should not be the target of new references after Phase E.

### 5. Co-located agent doc filename

Use **`AGENTS.md`** as the only canonical co-located agent doc filename.

- Do not create local `CLAUDE.md` files by default.
- Root `CLAUDE.md` remains a compatibility pointer only.
- If a tool later requires local `CLAUDE.md`, make it a generated pointer to the local `AGENTS.md`; do not duplicate content.

### 6. Workflow names

Workflow IDs use plain kebab-case with no prefix: `canon-guardian`, `change-summary`, `sync`, `validation`, `coordination`.

Rationale: the manifest schema already provides the `workflows` namespace, so `wf-canon-guardian` would add noise and make links less readable.

### 7. Manifest IDs

Manifest IDs are schema-defined, with these exact-match rules:

- Role IDs must match `.agents/roles/<role>/` folder names.
- Workflow IDs must match `.agents/workflows/<workflow-id>.md` basenames.
- Domain route IDs do **not** need to match filesystem folder names. They should be normalized from path segments using the domain prefix and kebab-case, e.g. `Actions/Rest Focus Loop/` → `actions-rest-focus-loop`.
- Compatibility alias IDs preserve historical wrapper/alias names and point to a canonical role, route, or workflow.

### 8. Cross-references

Use explicit real repo paths in links. Do not use symbolic IDs as the only way to navigate between files.

- Preferred for docs: `[link text](../../relative/path.md)` when the relative path is simple and stable.
- Preferred for generated/root routing summaries and manifest-adjacent docs: `[link text](.agents/workflows/change-summary.md)` repo-rooted path text.
- Backticks are for identifiers or paths mentioned but not intended as a clickable navigation edge.
- Validators should resolve Markdown links as paths and should separately verify manifest IDs.
- Do not use unnormalized shorthand like `agents/...`, `pi/...`, `/mnt/c/...`, or absolute local filesystem paths.

## Link Conventions

### Markdown links vs. backtick mentions

Use Markdown links for any reference the next agent is expected to follow.

Examples:

- Followable file reference: `[Actions/Squad/AGENTS.md](../../Actions/Squad/AGENTS.md)`
- Identifier mention: `actions-squad`
- Path mention without navigation intent: `Actions/Squad/`

A bare backtick path does not count as a graph edge for validators unless the validator explicitly implements path mention scanning.

### Relative vs. repo-rooted paths

Use one of two forms:

1. **Relative Markdown links** for hand-written docs inside a stable neighborhood.
2. **Repo-rooted Markdown links without leading slash** for generated docs, root docs, findings, handoffs, and manifest-oriented summaries.

Do not use leading `/` absolute paths. Do not use Windows drive paths or WSL mount paths in repository docs.

### Links to validators and scripts

Use the same path convention for scripts/validators as any other file.

- Link to the script path: `[Tools/StreamerBot/validate-shared-constants.py](../../Tools/StreamerBot/validate-shared-constants.py)`.
- Put commands in fenced code blocks with repo-relative paths:

```bash
python Tools/StreamerBot/validate-shared-constants.py
```

The command block is execution guidance; the Markdown link is the documentation graph edge.

## Heading and Section Conventions

### `AGENTS.md` domain docs

Required section order:

1. `# <Domain or Folder Name> — Agent Guide`
2. `## Purpose`
3. `## When to Activate`
4. `## Primary Owner`
5. `## Secondary Owners / Chain To`
6. `## Required Reading`
7. `## Local Workflow`
8. `## Validation`
9. `## Boundaries / Out of Scope`
10. `## Handoff Notes`

Optional sections, after required sections unless a template says otherwise:

- `## Runtime Notes`
- `## Paste / Sync Targets`
- `## Setup`
- `## Build / Test Commands`
- `## Public Copy / Brand Notes`
- `## Canon Notes`
- `## Known Gotchas`
- `## Generated Content`

### Role overview files

Required section order for `.agents/roles/<role>/role.md` after collapse:

1. `# Role: <role-id>`
2. `## Purpose`
3. `## Owns`
4. `## When to Activate`
5. `## Do Not Activate For`
6. `## Common Routes`
7. `## Required Workflows`
8. `## Chain To`
9. `## Living Context`

### Workflow files

Required section order for `.agents/workflows/<workflow-id>.md`:

1. `# Workflow: <workflow-id>`
2. `## Purpose`
3. `## When to Run`
4. `## Inputs`
5. `## Procedure`
6. `## Validation / Done Criteria`
7. `## Output / Handoff`
8. `## Related Routes`

Optional workflow sections:

- `## Role-Specific Notes`
- `## Templates`
- `## Failure Modes`
- `## Examples`

## Frontmatter Convention

Use YAML frontmatter in target agent docs, role files, and workflows once manifest v2 tooling supports it. Existing docs without frontmatter can be migrated incrementally.

### Required fields

| Field | Required for | Schema |
|---|---|---|
| `id` | all agent docs/workflows/roles | Must match manifest ID for roles/workflows/routes. |
| `type` | all agent docs/workflows/roles | One of `role`, `domain-route`, `workflow`, `shared`, `compatibility`, `template`. |
| `description` | all non-template entries | One sentence, 80–160 characters preferred, no Markdown links. |
| `owner` | `domain-route`, `shared` | Primary role ID. |
| `secondaryOwners` | `domain-route` | Array of role IDs; empty array allowed. |
| `workflows` | `domain-route`, `role` | Array of workflow IDs; empty array allowed. |
| `status` | all entries | One of `active`, `deprecated`, `generated`, `template`, `archive`. |
| `generated` | generated files/blocks where file-level generation applies | Boolean. |

### Optional fields

- `path`
- `coveredBy`
- `aliases`
- `appliesTo`
- `terminal`
- `lastReviewed`
- `sourceOfTruth`

### Description style

Descriptions should be short routing descriptions, not full instructions.

Good:

> Streamer.bot C# runtime actions, paste targets, and validation for Squad mini-games.

Avoid:

> Load this when maybe editing the squad stuff and remember to check a lot of files.

## Migration Rules

1. Preserve existing domain folder names unless a later prompt explicitly approves renaming them.
2. Normalize IDs, not paths: `Actions/Rest Focus Loop/` becomes route ID `actions-rest-focus-loop` while the folder stays `Actions/Rest Focus Loop/`.
3. Move central role skill content to either a local `AGENTS.md` or a workflow file.
4. Convert Pi wrappers to manifest compatibility aliases during the transitional period; do not hand-maintain new wrappers.
5. Convert `_index.md` entrypoints into target docs, then retire or replace old files with generated compatibility stubs only if needed.
6. Canon-guardian variants collapse into one workflow ID, `canon-guardian`, with role-specific sections.
7. Ops procedure skills collapse into workflows: `change-summary`, `sync`, and `validation`.
8. Broad role wrappers collapse to role IDs; detailed instructions move out of `retired Pi skill mirror/` and out of `.agents/roles/*/skills/core.md`.
9. Deprecated aliases stay listed in manifest v2 only as `compatibilityAliases` until cutover validation proves no consumer uses them.
10. Generated routing summaries must include generated markers around generated blocks.

## Proposed Target IDs

### Core role IDs

| Current role folder | Proposed ID | Target location |
|---|---|---|
| `.agents/roles/app-dev/` | `app-dev` | `.agents/roles/app-dev/role.md` |
| `.agents/roles/art-director/` | `art-director` | `.agents/roles/art-director/role.md` |
| `.agents/roles/brand-steward/` | `brand-steward` | `.agents/roles/brand-steward/role.md` |
| `.agents/roles/content-repurposer/` | `content-repurposer` | `.agents/roles/content-repurposer/role.md` |
| `.agents/roles/lotat-tech/` | `lotat-tech` | `.agents/roles/lotat-tech/role.md` |
| `.agents/roles/lotat-writer/` | `lotat-writer` | `.agents/roles/lotat-writer/role.md` |
| `.agents/roles/ops/` | `ops` | `.agents/roles/ops/role.md` |
| `.agents/roles/product-dev/` | `product-dev` | `.agents/roles/product-dev/role.md` |
| `.agents/roles/streamerbot-dev/` | `streamerbot-dev` | `.agents/roles/streamerbot-dev/role.md` |

### Workflow IDs

| Current concept | Proposed ID | Target location |
|---|---|---|
| Brand/LotAT canon review procedures | `canon-guardian` | `.agents/workflows/canon-guardian.md` |
| Ops change summary terminal procedure | `change-summary` | `.agents/workflows/change-summary.md` |
| Repo-to-Streamer.bot paste/sync procedure | `sync` | `.agents/workflows/sync.md` |
| Validation procedure | `validation` | `.agents/workflows/validation.md` |
| WORKING.md coordination protocol, if extracted | `coordination` | `.agents/workflows/coordination.md` |

## Final Mapping Table — Current `.agents/roles/<role>/skills/<feature>/`

Template folders are included for completeness but should remain template-only, not active manifest routes.

| Current folder | Proposed new identifier | Proposed target location | Migration disposition |
|---|---|---|---|
| `.agents/roles/_template/skills/example-skill/` | `template-example-skill` | `.agents/roles/_template/` or future template path | Keep as template only; exclude from active routes. |
| `.agents/roles/app-dev/skills/stream-interactions/` | `apps-stream-overlay` | `Apps/stream-overlay/AGENTS.md` | Move overlay/broker/protocol guidance to stream-overlay local doc and linked app docs as needed. |
| `.agents/roles/art-director/skills/characters/` | `creative-art` | `Creative/Art/AGENTS.md` | Move character art guidance to local art doc; link brand/canon sources. |
| `.agents/roles/art-director/skills/pipeline/` | `tools-art-pipeline` | `Tools/ArtPipeline/AGENTS.md` | Move tool setup/validation to ArtPipeline local doc. |
| `.agents/roles/art-director/skills/stream-style/` | `creative-art` | `Creative/Art/AGENTS.md` | Merge into art style/local creative guidance. |
| `.agents/roles/brand-steward/skills/canon-guardian/` | `canon-guardian` | `.agents/workflows/canon-guardian.md` | Move procedure to shared workflow with brand-specific section. |
| `.agents/roles/brand-steward/skills/community-growth/` | `creative-marketing` | `Creative/Marketing/AGENTS.md` | Move public/community growth guidance to marketing local doc. |
| `.agents/roles/brand-steward/skills/content-strategy/` | `creative-marketing` | `Creative/Marketing/AGENTS.md` | Move build/story strategy to marketing local doc; promote reusable pieces only if later needed. |
| `.agents/roles/brand-steward/skills/voice/` | `creative-brand` | `Creative/Brand/AGENTS.md` | Move brand voice routing to brand local doc and source docs. |
| `.agents/roles/content-repurposer/skills/clip-strategy/` | `creative-marketing` | `Creative/Marketing/AGENTS.md` | Move strategy handoff to marketing doc; chain to content repurposer for platform packaging. |
| `.agents/roles/content-repurposer/skills/pipeline/` | `tools-content-pipeline` | `Tools/ContentPipeline/AGENTS.md` | Move tooling details to ContentPipeline local doc. |
| `.agents/roles/content-repurposer/skills/platforms/` | `creative-marketing` | `Creative/Marketing/AGENTS.md` | Move platform formatting guidance to marketing/content local doc or linked content pipeline doc. |
| `.agents/roles/lotat-tech/skills/engine/` | `actions-lotat` | `Actions/LotAT/AGENTS.md` | Move Streamer.bot engine runtime details to Actions/LotAT local doc. |
| `.agents/roles/lotat-tech/skills/story-pipeline/` | `tools-lotat` | `Tools/LotAT/AGENTS.md` | Move story schema/tooling details to Tools/LotAT local doc; link Actions/LotAT. |
| `.agents/roles/lotat-writer/skills/adventures/` | `creative-worldbuilding` | `Creative/WorldBuilding/AGENTS.md` | Move adventure writing guidance to worldbuilding local doc. |
| `.agents/roles/lotat-writer/skills/canon-guardian/` | `canon-guardian` | `.agents/workflows/canon-guardian.md` | Move procedure to shared workflow with LotAT-specific section. |
| `.agents/roles/lotat-writer/skills/franchises/` | `creative-worldbuilding` | `Creative/WorldBuilding/AGENTS.md` | Move franchise guidance to worldbuilding local doc. |
| `.agents/roles/lotat-writer/skills/universe/` | `creative-worldbuilding` | `Creative/WorldBuilding/AGENTS.md` | Move universe/cast/rules guidance to worldbuilding local doc. |
| `.agents/roles/ops/skills/change-summary/` | `change-summary` | `.agents/workflows/change-summary.md` | Convert to workflow. |
| `.agents/roles/ops/skills/sync/` | `sync` | `.agents/workflows/sync.md` | Convert to workflow. |
| `.agents/roles/ops/skills/validation/` | `validation` | `.agents/workflows/validation.md` | Convert to workflow. |
| `.agents/roles/streamerbot-dev/skills/commanders/` | `actions-commanders` | `Actions/Commanders/AGENTS.md` | Move feature guidance to Commanders local doc. |
| `.agents/roles/streamerbot-dev/skills/lotat/` | `actions-lotat` | `Actions/LotAT/AGENTS.md` | Move Streamer.bot LotAT integration guidance to Actions/LotAT local doc. |
| `.agents/roles/streamerbot-dev/skills/squad/` | `actions-squad` | `Actions/Squad/AGENTS.md` | Move feature guidance to Squad local doc. |
| `.agents/roles/streamerbot-dev/skills/twitch/` | `actions-twitch-core-integrations` plus related Twitch routes | `Actions/Twitch Core Integrations/AGENTS.md` plus Bits/Channel Points/Hype Train local docs | Split by actual domain folder; keep one streamerbot-dev owner family. |
| `.agents/roles/streamerbot-dev/skills/voice-commands/` | `actions-voice-commands` | `Actions/Voice Commands/AGENTS.md` | Move feature guidance to Voice Commands local doc. |

## Final Mapping Table — Current `retired Pi skill mirror/<wrapper>/`

| Current wrapper | Proposed new identifier | Proposed target location | Migration disposition |
|---|---|---|---|
| `retired Pi skill mirror/app-dev/` | `app-dev` | `.agents/roles/app-dev/role.md` | Role wrapper becomes manifest role route; delete wrapper at Pi cutover. |
| `retired Pi skill mirror/art-director/` | `art-director` | `.agents/roles/art-director/role.md` | Role wrapper becomes manifest role route; delete wrapper at Pi cutover. |
| `retired Pi skill mirror/brand-canon-guardian/` | `canon-guardian` | `.agents/workflows/canon-guardian.md` | Deprecated alias to workflow. |
| `retired Pi skill mirror/brand-steward/` | `brand-steward` | `.agents/roles/brand-steward/role.md` | Role wrapper becomes manifest role route; delete wrapper at Pi cutover. |
| `retired Pi skill mirror/brand-steward-canon-guardian/` | `canon-guardian` | `.agents/workflows/canon-guardian.md` | Canonical sub-skill becomes workflow applicability for brand-steward. |
| `retired Pi skill mirror/brand-steward-content-strategy/` | `creative-marketing` | `Creative/Marketing/AGENTS.md` | Domain route; no new wrapper. |
| `retired Pi skill mirror/buildtools/` | `ops` | `.agents/roles/ops/role.md` | Deprecated alias to ops role. |
| `retired Pi skill mirror/change-summary/` | `change-summary` | `.agents/workflows/change-summary.md` | Deprecated alias to workflow. |
| `retired Pi skill mirror/content-repurposer/` | `content-repurposer` | `.agents/roles/content-repurposer/role.md` | Role wrapper becomes manifest role route; delete wrapper at Pi cutover. |
| `retired Pi skill mirror/content-repurposer-pipeline/` | `tools-content-pipeline` | `Tools/ContentPipeline/AGENTS.md` | Domain route; no new wrapper. |
| `retired Pi skill mirror/content-strategy/` | `creative-marketing` | `Creative/Marketing/AGENTS.md` | Deprecated alias to domain route. |
| `retired Pi skill mirror/creative-art/` | `creative-art` | `Creative/Art/AGENTS.md` | Deprecated alias to domain route. |
| `retired Pi skill mirror/creative-worldbuilding/` | `creative-worldbuilding` | `Creative/WorldBuilding/AGENTS.md` | Deprecated alias to domain route, with `lotat-writer`/`lotat-tech` owner split in manifest. |
| `retired Pi skill mirror/feature-channel-points/` | `actions-twitch-channel-points` | `Actions/Twitch Channel Points/AGENTS.md` | Deprecated alias to domain route. |
| `retired Pi skill mirror/feature-commanders/` | `actions-commanders` | `Actions/Commanders/AGENTS.md` | Deprecated alias to domain route. |
| `retired Pi skill mirror/feature-hype-train/` | `actions-twitch-hype-train` | `Actions/Twitch Hype Train/AGENTS.md` | Deprecated alias to domain route. |
| `retired Pi skill mirror/feature-squad/` | `actions-squad` | `Actions/Squad/AGENTS.md` | Deprecated alias to domain route. |
| `retired Pi skill mirror/feature-twitch-integration/` | `actions-twitch-core-integrations` plus related Twitch routes | Twitch local `AGENTS.md` docs | Deprecated broad alias; manifest should route by actual folder path. |
| `retired Pi skill mirror/feature-voice-commands/` | `actions-voice-commands` | `Actions/Voice Commands/AGENTS.md` | Deprecated alias to domain route. |
| `retired Pi skill mirror/lotat-tech/` | `lotat-tech` | `.agents/roles/lotat-tech/role.md` | Role wrapper becomes manifest role route; delete wrapper at Pi cutover. |
| `retired Pi skill mirror/lotat-writer/` | `lotat-writer` | `.agents/roles/lotat-writer/role.md` | Role wrapper becomes manifest role route; delete wrapper at Pi cutover. |
| `retired Pi skill mirror/lotat-writer-canon-guardian/` | `canon-guardian` | `.agents/workflows/canon-guardian.md` | Canonical sub-skill becomes workflow applicability for lotat-writer. |
| `retired Pi skill mirror/meta/` | `meta` | `.agents/workflows/coordination.md` or future `.agents/meta/` | Transitional helper; decide final Pi meta disposition at cutover. |
| `retired Pi skill mirror/meta-agents-navigate/` | `meta-agents-navigate` | `.agents/workflows/coordination.md` or future `.agents/meta/navigate.md` | Transitional helper with real content; migrate before deleting Pi mirror. |
| `retired Pi skill mirror/meta-agents-update/` | `meta-agents-update` | `.agents/workflows/coordination.md` or future `.agents/meta/update.md` | Transitional helper with real content; migrate before deleting Pi mirror. |
| `retired Pi skill mirror/ops/` | `ops` | `.agents/roles/ops/role.md` | Role wrapper becomes manifest role route; delete wrapper at Pi cutover. |
| `retired Pi skill mirror/ops-change-summary/` | `change-summary` | `.agents/workflows/change-summary.md` | Canonical sub-skill becomes workflow. |
| `retired Pi skill mirror/ops-sync/` | `sync` | `.agents/workflows/sync.md` | Canonical sub-skill becomes workflow. |
| `retired Pi skill mirror/ops-validation/` | `validation` | `.agents/workflows/validation.md` | Canonical sub-skill becomes workflow. |
| `retired Pi skill mirror/product-dev/` | `product-dev` | `.agents/roles/product-dev/role.md` | Role wrapper becomes manifest role route; delete wrapper at Pi cutover. |
| `retired Pi skill mirror/streamerbot-dev/` | `streamerbot-dev` | `.agents/roles/streamerbot-dev/role.md` | Role wrapper becomes manifest role route; delete wrapper at Pi cutover. |
| `retired Pi skill mirror/streamerbot-dev-commanders/` | `actions-commanders` | `Actions/Commanders/AGENTS.md` | Canonical sub-skill becomes domain route. |
| `retired Pi skill mirror/streamerbot-dev-lotat/` | `actions-lotat` | `Actions/LotAT/AGENTS.md` | Canonical sub-skill becomes domain route. |
| `retired Pi skill mirror/streamerbot-dev-squad/` | `actions-squad` | `Actions/Squad/AGENTS.md` | Canonical sub-skill becomes domain route. |
| `retired Pi skill mirror/streamerbot-dev-twitch/` | `actions-twitch-core-integrations` plus related Twitch routes | Twitch local `AGENTS.md` docs | Canonical sub-skill splits by actual Twitch domain folder. |
| `retired Pi skill mirror/streamerbot-dev-voice-commands/` | `actions-voice-commands` | `Actions/Voice Commands/AGENTS.md` | Canonical sub-skill becomes domain route. |
| `retired Pi skill mirror/streamerbot-scripting/` | `streamerbot-dev` | `.agents/roles/streamerbot-dev/role.md` | Deprecated alias to role. |
| `retired Pi skill mirror/sync-workflow/` | `sync` | `.agents/workflows/sync.md` | Deprecated alias to workflow. |

## Validator / Acceptance Notes

Operator ratification should confirm:

1. IDs are kebab-case and namespace-scoped by manifest type.
2. No `wf-` prefix is used for workflows.
3. Domain route IDs normalize paths but do not force folder renames.
4. `AGENTS.md` is the canonical local agent doc.
5. `_index.md` and `SKILL.md` are migration/compatibility only, not target entry files.
6. Links use explicit real repo paths and Markdown links for navigation.

## Handoff

### State changes

- Wrote and ratified `Projects/agent-reflow/findings/06-naming-convention.md`.
- Updated the draft to resolve operator review items and unblock prompt 07.
- No edits made to `.agents/`, `.pi/`, domain folders, root docs, or git state.

### Findings appended

- Naming rules for IDs, skill/domain route folders, entry files, co-located docs, workflow names, manifest IDs, and cross-references.
- Link conventions for Markdown links, backtick mentions, relative/repo-rooted paths, and validator/script references.
- Required/optional heading conventions for domain `AGENTS.md`, role overview files, and workflow files.
- Frontmatter schema and description style.
- Migration rules and mapping tables for current `.agents/roles/*/skills/*/` folders and `retired Pi skill mirror/*/` wrappers.

### Validator status

Suggested acceptance check after operator review:

```bash
test -f Projects/agent-reflow/findings/06-naming-convention.md \
  && grep -q "Final Mapping Table" Projects/agent-reflow/findings/06-naming-convention.md \
  && grep -q "Status: **RATIFIED" Projects/agent-reflow/findings/06-naming-convention.md \
  && grep -q "Prompt 07 unblock" Projects/agent-reflow/findings/06-naming-convention.md \
  && echo ok
```

### Open questions / blockers

- RESOLVED — No `wf-` workflow prefix; use manifest namespace and `.agents/workflows/<workflow-id>.md` path.
- RESOLVED — `AGENTS.md` is the only canonical co-located agent doc filename.
- RESOLVED — `_index.md` and `SKILL.md` are deprecated/compatibility entry files only.
- RESOLVED — Split CamelCase/PascalCase folder names into readable kebab-case route IDs, e.g. `tools-content-pipeline` and `tools-art-pipeline`.
- RESOLVED — Prompt 07 is unblocked.

### Next prompt entry point

Proceed to `Projects/agent-reflow/prompts/07-manifest-schema-v2.md` using this naming convention as a ratified input.
