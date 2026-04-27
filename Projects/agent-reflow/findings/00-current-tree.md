# Findings 00 — Current `.agents/` Tree

Generated: 2026-04-26
Source: prompt 00-inventory-agents-tree

## Top-Level Layout

| Path | Type | Lines | Notes |
|---|---|---:|---|
| .agents/ENTRY.md | entry doc | 44 | SharkStreamerBot — Agent Entry Point |
| legacy v1 routing manifest (retired) | manifest | 349 | 9 roles, 12 sub-skills, 14 aliases |
| .agents/_shared/ | shared dir | 407 | `conventions.md`, `coordination.md`, `info-service-protocol.md`, `mixitup-api.md`, `project.md` |
| .agents/roles/ | roles dir | 7524 | 10 folders (9 manifest roles plus 1 undeclared/template folders) |

## Complete File Inventory

| Path | Type | Lines | Frontmatter `name` | Frontmatter `description` | First H1 |
|---|---|---:|---|---|---|
| .agents/ENTRY.md | other | 44 |  |  | SharkStreamerBot — Agent Entry Point |
| .agents/_shared/conventions.md | other | 52 |  |  | Conventions |
| .agents/_shared/coordination.md | other | 48 |  |  | Agent Coordination |
| .agents/_shared/info-service-protocol.md | other | 119 |  |  | Info Service — Protocol Reference |
| .agents/_shared/mixitup-api.md | other | 112 |  |  | Mix It Up API — Run Command Convention |
| .agents/_shared/project.md | other | 76 |  |  | Project: SharkStreamerBot |
| .agents/roles/_template/context/.gitkeep | other | 0 |  |  |  |
| .agents/roles/_template/role.md | role.md | 33 |  |  | Role: <role-name> |
| .agents/roles/_template/skills/core.md | core.md | 15 |  |  | Core Skills — <role-name> |
| .agents/roles/_template/skills/example-skill/_index.md | _index.md | 76 |  |  | Example Skill Area — Overview |
| .agents/roles/app-dev/context/.gitkeep | other | 0 |  |  |  |
| .agents/roles/app-dev/context/info-service.md | other | 64 |  |  | info-service — Agent Orientation |
| .agents/roles/app-dev/role.md | role.md | 42 |  |  | Role: app-dev |
| .agents/roles/app-dev/skills/core.md | core.md | 135 |  |  | Core Skills — app-dev |
| .agents/roles/app-dev/skills/stream-interactions/_index.md | _index.md | 107 |  |  | Stream Interactions — Overview |
| .agents/roles/app-dev/skills/stream-interactions/asset-system.md | leaf skill .md | 548 |  |  | Asset System |
| .agents/roles/app-dev/skills/stream-interactions/broker.md | leaf skill .md | 273 |  |  | Broker |
| .agents/roles/app-dev/skills/stream-interactions/lotat-rendering.md | leaf skill .md | 219 |  |  | LotAT Rendering — Visual Layer |
| .agents/roles/app-dev/skills/stream-interactions/overlay.md | leaf skill .md | 293 |  |  | Overlay |
| .agents/roles/app-dev/skills/stream-interactions/protocol.md | leaf skill .md | 347 |  |  | Message Protocol |
| .agents/roles/app-dev/skills/stream-interactions/squad-rendering.md | leaf skill .md | 175 |  |  | Squad Rendering — Visual Layer |
| .agents/roles/art-director/context/.gitkeep | other | 0 |  |  |  |
| .agents/roles/art-director/role.md | role.md | 38 |  |  | Role: art-director |
| .agents/roles/art-director/skills/characters/_index.md | _index.md | 24 |  |  | Characters — Art Overview |
| .agents/roles/art-director/skills/characters/captain-stretch.md | leaf skill .md | 24 |  |  | Character Art — Captain Stretch |
| .agents/roles/art-director/skills/characters/the-director.md | leaf skill .md | 24 |  |  | Character Art — The Director |
| .agents/roles/art-director/skills/characters/water-wizard.md | leaf skill .md | 24 |  |  | Character Art — Water Wizard |
| .agents/roles/art-director/skills/core.md | core.md | 65 |  |  | Core Skills — art-director |
| .agents/roles/art-director/skills/pipeline/_index.md | _index.md | 274 |  |  | Pipeline Tooling — art-director |
| .agents/roles/art-director/skills/stream-style/_index.md | _index.md | 29 |  |  | Stream Style — Overview |
| .agents/roles/brand-steward/context/.gitkeep | other | 0 |  |  |  |
| .agents/roles/brand-steward/role.md | role.md | 52 |  |  | Role: brand-steward |
| .agents/roles/brand-steward/skills/canon-guardian/_index.md | _index.md | 79 |  |  | Canon Guardian — Overview |
| .agents/roles/brand-steward/skills/community-growth/_index.md | _index.md | 39 |  |  | Community Growth — brand-steward |
| .agents/roles/brand-steward/skills/content-strategy/_index.md | _index.md | 51 |  |  | Content Strategy — Overview |
| .agents/roles/brand-steward/skills/core.md | core.md | 58 |  |  | Core Skills — brand-steward |
| .agents/roles/brand-steward/skills/voice/_index.md | _index.md | 31 |  |  | Brand Voice — Overview |
| .agents/roles/content-repurposer/context/.gitkeep | other | 0 |  |  |  |
| .agents/roles/content-repurposer/context/pipeline-dev-notes.md | other | 7 |  |  | Pipeline Dev Notes |
| .agents/roles/content-repurposer/role.md | role.md | 49 |  |  | Role: content-repurposer |
| .agents/roles/content-repurposer/skills/clip-strategy/_index.md | _index.md | 26 |  |  | Clip Strategy — content-repurposer |
| .agents/roles/content-repurposer/skills/core.md | core.md | 58 |  |  | Core Skills — content-repurposer |
| .agents/roles/content-repurposer/skills/pipeline/_index.md | _index.md | 82 |  |  | Pipeline Tooling — content-repurposer |
| .agents/roles/content-repurposer/skills/pipeline/phase-map.md | leaf skill .md | 89 |  |  | Content Pipeline Phase Map |
| .agents/roles/content-repurposer/skills/platforms/_index.md | _index.md | 37 |  |  | Platforms — content-repurposer |
| .agents/roles/lotat-tech/context/.gitkeep | other | 0 |  |  |  |
| .agents/roles/lotat-tech/role.md | role.md | 47 |  |  | Role: lotat-tech |
| .agents/roles/lotat-tech/skills/core.md | core.md | 99 |  |  | Core Skills — lotat-tech |
| .agents/roles/lotat-tech/skills/engine/_index.md | _index.md | 194 |  |  | LotAT Engine — Overview |
| .agents/roles/lotat-tech/skills/engine/commands.md | leaf skill .md | 101 |  |  | LotAT Engine — Supported Commands |
| .agents/roles/lotat-tech/skills/engine/docs-map.md | leaf skill .md | 144 |  |  | LotAT Engine — Docs Map |
| .agents/roles/lotat-tech/skills/engine/session-lifecycle.md | leaf skill .md | 588 |  |  | LotAT Engine — Session Lifecycle Runtime Contract |
| .agents/roles/lotat-tech/skills/engine/state-and-voting.md | leaf skill .md | 474 |  |  | LotAT Engine — State and Voting Spec |
| .agents/roles/lotat-tech/skills/story-pipeline/_index.md | _index.md | 113 |  |  | Story Pipeline — Overview |
| .agents/roles/lotat-tech/skills/story-pipeline/json-schema.md | leaf skill .md | 384 |  |  | LotAT Story JSON Schema |
| .agents/roles/lotat-writer/context/.gitkeep | other | 0 |  |  |  |
| .agents/roles/lotat-writer/role.md | role.md | 55 |  |  | Role: lotat-writer |
| .agents/roles/lotat-writer/skills/adventures/_index.md | _index.md | 48 |  |  | Adventures — Overview |
| .agents/roles/lotat-writer/skills/adventures/mechanics.md | leaf skill .md | 52 |  |  | Adventure Mechanics |
| .agents/roles/lotat-writer/skills/adventures/session-format.md | leaf skill .md | 40 |  |  | Session Format — Live Stream Pacing |
| .agents/roles/lotat-writer/skills/canon-guardian/_index.md | _index.md | 84 |  |  | Canon Guardian — lotat-writer |
| .agents/roles/lotat-writer/skills/core.md | core.md | 123 |  |  | Core Skills — lotat-writer |
| .agents/roles/lotat-writer/skills/franchises/_index.md | _index.md | 28 |  |  | Franchises — Overview |
| .agents/roles/lotat-writer/skills/franchises/starship-shamples.md | leaf skill .md | 47 |  |  | Franchise: Starship Shamples |
| .agents/roles/lotat-writer/skills/universe/_index.md | _index.md | 22 |  |  | Universe — Overview |
| .agents/roles/lotat-writer/skills/universe/cast.md | leaf skill .md | 36 |  |  | Cast Canon |
| .agents/roles/lotat-writer/skills/universe/rules.md | leaf skill .md | 42 |  |  | Universe Rules |
| .agents/roles/ops/context/.gitkeep | other | 0 |  |  |  |
| .agents/roles/ops/role.md | role.md | 67 |  |  | Role: ops |
| .agents/roles/ops/skills/change-summary/_index.md | _index.md | 44 |  |  | Change Summary — Terminal Skill |
| .agents/roles/ops/skills/core.md | core.md | 43 |  |  | Core Skills — ops |
| .agents/roles/ops/skills/sync/_index.md | _index.md | 47 |  |  | Sync Workflow |
| .agents/roles/ops/skills/validation/_index.md | _index.md | 24 |  |  | Validation |
| .agents/roles/product-dev/context/.gitkeep | other | 0 |  |  |  |
| .agents/roles/product-dev/role.md | role.md | 62 |  |  | Role: product-dev |
| .agents/roles/product-dev/skills/core.md | core.md | 28 |  |  | Core Skills — product-dev |
| .agents/roles/streamerbot-dev/context/patterns.md | other | 34 |  |  | streamerbot-dev — Discovered Patterns & Gotchas |
| .agents/roles/streamerbot-dev/role.md | role.md | 41 |  |  | Role: streamerbot-dev |
| .agents/roles/streamerbot-dev/skills/commanders/_index.md | _index.md | 61 |  |  | Commanders — Feature Overview |
| .agents/roles/streamerbot-dev/skills/commanders/captain-stretch.md | leaf skill .md | 26 |  |  | Commander — Captain Stretch |
| .agents/roles/streamerbot-dev/skills/commanders/the-director.md | leaf skill .md | 27 |  |  | Commander — The Director |
| .agents/roles/streamerbot-dev/skills/commanders/water-wizard.md | leaf skill .md | 26 |  |  | Commander — Water Wizard |
| .agents/roles/streamerbot-dev/skills/core.md | core.md | 88 |  |  | Core Skills — streamerbot-dev |
| .agents/roles/streamerbot-dev/skills/lotat/_index.md | _index.md | 68 |  |  | LotAT — streamerbot-dev Scope |
| .agents/roles/streamerbot-dev/skills/overlay-integration.md | leaf skill .md | 165 |  |  | Overlay Integration — streamerbot-dev |
| .agents/roles/streamerbot-dev/skills/squad/_index.md | _index.md | 96 |  |  | Squad — Feature Overview |
| .agents/roles/streamerbot-dev/skills/squad/clone.md | leaf skill .md | 31 |  |  | Squad — Clone |
| .agents/roles/streamerbot-dev/skills/squad/duck.md | leaf skill .md | 27 |  |  | Squad — Duck |
| .agents/roles/streamerbot-dev/skills/squad/pedro.md | leaf skill .md | 30 |  |  | Squad — Pedro |
| .agents/roles/streamerbot-dev/skills/squad/toothless.md | leaf skill .md | 26 |  |  | Squad — Toothless |
| .agents/roles/streamerbot-dev/skills/twitch/_index.md | _index.md | 60 |  |  | Twitch Integrations — Feature Overview |
| .agents/roles/streamerbot-dev/skills/twitch/bits.md | leaf skill .md | 33 |  |  | Twitch Bits Integrations |
| .agents/roles/streamerbot-dev/skills/twitch/channel-points.md | leaf skill .md | 35 |  |  | Twitch Channel Points |
| .agents/roles/streamerbot-dev/skills/twitch/core-events.md | leaf skill .md | 41 |  |  | Twitch Core Events |
| .agents/roles/streamerbot-dev/skills/twitch/hype-train.md | leaf skill .md | 26 |  |  | Twitch Hype Train |
| .agents/roles/streamerbot-dev/skills/voice-commands/_index.md | _index.md | 60 |  |  | Voice Commands — Feature Overview |
| legacy v1 routing manifest (retired) | legacy-v1-routing-manifest | 349 |  |  |  |

## Roles

### Role: _template

- Folder: `.agents/roles/_template/`
- Total lines: 124
- `role.md`: yes, 33 lines
- `skills/core.md`: yes, 15 lines
- Sub-skill folders: `example-skill`
- Leaf skill files: none
- `context/`: yes, `.gitkeep`
- Manifest declares this role: no
- Manifest canonical sub-skills for this role: none
- Mismatches: `role folder exists but manifest does not declare this role`

### Role: app-dev

- Folder: `.agents/roles/app-dev/`
- Total lines: 2203
- `role.md`: yes, 42 lines
- `skills/core.md`: yes, 135 lines
- Sub-skill folders: `stream-interactions`
- Leaf skill files: none
- `context/`: yes, `.gitkeep`, `info-service.md`
- Manifest declares this role: yes
- Manifest canonical sub-skills for this role: none
- Mismatches: `skills/stream-interactions/ exists but no canonical manifest sub-skill declares it`

### Role: art-director

- Folder: `.agents/roles/art-director/`
- Total lines: 502
- `role.md`: yes, 38 lines
- `skills/core.md`: yes, 65 lines
- Sub-skill folders: `characters`, `pipeline`, `stream-style`
- Leaf skill files: none
- `context/`: yes, `.gitkeep`
- Manifest declares this role: yes
- Manifest canonical sub-skills for this role: none
- Mismatches: `skills/characters/ exists but no canonical manifest sub-skill declares it`, `skills/pipeline/ exists but no canonical manifest sub-skill declares it`, `skills/stream-style/ exists but no canonical manifest sub-skill declares it`

### Role: brand-steward

- Folder: `.agents/roles/brand-steward/`
- Total lines: 310
- `role.md`: yes, 52 lines
- `skills/core.md`: yes, 58 lines
- Sub-skill folders: `canon-guardian`, `community-growth`, `content-strategy`, `voice`
- Leaf skill files: none
- `context/`: yes, `.gitkeep`
- Manifest declares this role: yes
- Manifest canonical sub-skills for this role: `brand-steward-canon-guardian`, `brand-steward-content-strategy`
- Mismatches: `skills/community-growth/ exists but no canonical manifest sub-skill declares it`, `skills/voice/ exists but no canonical manifest sub-skill declares it`

### Role: content-repurposer

- Folder: `.agents/roles/content-repurposer/`
- Total lines: 348
- `role.md`: yes, 49 lines
- `skills/core.md`: yes, 58 lines
- Sub-skill folders: `clip-strategy`, `pipeline`, `platforms`
- Leaf skill files: none
- `context/`: yes, `.gitkeep`, `pipeline-dev-notes.md`
- Manifest declares this role: yes
- Manifest canonical sub-skills for this role: `content-repurposer-pipeline`
- Mismatches: `skills/clip-strategy/ exists but no canonical manifest sub-skill declares it`, `skills/platforms/ exists but no canonical manifest sub-skill declares it`

### Role: lotat-tech

- Folder: `.agents/roles/lotat-tech/`
- Total lines: 2144
- `role.md`: yes, 47 lines
- `skills/core.md`: yes, 99 lines
- Sub-skill folders: `engine`, `story-pipeline`
- Leaf skill files: none
- `context/`: yes, `.gitkeep`
- Manifest declares this role: yes
- Manifest canonical sub-skills for this role: none
- Mismatches: `skills/engine/ exists but no canonical manifest sub-skill declares it`, `skills/story-pipeline/ exists but no canonical manifest sub-skill declares it`

### Role: lotat-writer

- Folder: `.agents/roles/lotat-writer/`
- Total lines: 577
- `role.md`: yes, 55 lines
- `skills/core.md`: yes, 123 lines
- Sub-skill folders: `adventures`, `canon-guardian`, `franchises`, `universe`
- Leaf skill files: none
- `context/`: yes, `.gitkeep`
- Manifest declares this role: yes
- Manifest canonical sub-skills for this role: `lotat-writer-canon-guardian`
- Mismatches: `skills/adventures/ exists but no canonical manifest sub-skill declares it`, `skills/franchises/ exists but no canonical manifest sub-skill declares it`, `skills/universe/ exists but no canonical manifest sub-skill declares it`

### Role: ops

- Folder: `.agents/roles/ops/`
- Total lines: 225
- `role.md`: yes, 67 lines
- `skills/core.md`: yes, 43 lines
- Sub-skill folders: `change-summary`, `sync`, `validation`
- Leaf skill files: none
- `context/`: yes, `.gitkeep`
- Manifest declares this role: yes
- Manifest canonical sub-skills for this role: `ops-change-summary`, `ops-sync`, `ops-validation`
- Mismatches: none

### Role: product-dev

- Folder: `.agents/roles/product-dev/`
- Total lines: 90
- `role.md`: yes, 62 lines
- `skills/core.md`: yes, 28 lines
- Sub-skill folders: none
- Leaf skill files: none
- `context/`: yes, `.gitkeep`
- Manifest declares this role: yes
- Manifest canonical sub-skills for this role: none
- Mismatches: none

### Role: streamerbot-dev

- Folder: `.agents/roles/streamerbot-dev/`
- Total lines: 1001
- `role.md`: yes, 41 lines
- `skills/core.md`: yes, 88 lines
- Sub-skill folders: `commanders`, `lotat`, `squad`, `twitch`, `voice-commands`
- Leaf skill files: `overlay-integration.md`
- `context/`: yes, `patterns.md`
- Manifest declares this role: yes
- Manifest canonical sub-skills for this role: `streamerbot-dev-commanders`, `streamerbot-dev-lotat`, `streamerbot-dev-squad`, `streamerbot-dev-twitch`, `streamerbot-dev-voice-commands`
- Mismatches: none

## `_shared/` Contents

| File | Lines | First H1 |
|---|---:|---|
| .agents/_shared/conventions.md | 52 | Conventions |
| .agents/_shared/coordination.md | 48 | Agent Coordination |
| .agents/_shared/info-service-protocol.md | 119 | Info Service — Protocol Reference |
| .agents/_shared/mixitup-api.md | 112 | Mix It Up API — Run Command Convention |
| .agents/_shared/project.md | 76 | Project: SharkStreamerBot |

## Manifest Cross-Check

### Manifest says exists, folder missing
- none

### Folder exists, manifest does not declare
- role folder .agents/roles/_template/
- sub-skill folder .agents/roles/app-dev/skills/stream-interactions/
- sub-skill folder .agents/roles/art-director/skills/characters/
- sub-skill folder .agents/roles/art-director/skills/pipeline/
- sub-skill folder .agents/roles/art-director/skills/stream-style/
- sub-skill folder .agents/roles/brand-steward/skills/community-growth/
- sub-skill folder .agents/roles/brand-steward/skills/voice/
- sub-skill folder .agents/roles/content-repurposer/skills/clip-strategy/
- sub-skill folder .agents/roles/content-repurposer/skills/platforms/
- sub-skill folder .agents/roles/lotat-tech/skills/engine/
- sub-skill folder .agents/roles/lotat-tech/skills/story-pipeline/
- sub-skill folder .agents/roles/lotat-writer/skills/adventures/
- sub-skill folder .agents/roles/lotat-writer/skills/franchises/
- sub-skill folder .agents/roles/lotat-writer/skills/universe/

### Required surfaces declared, file missing
- none

## Aliases

| Alias | Targets | Targets exist? |
|---|---|---|
| change-summary | `ops-change-summary` | ops-change-summary: yes |
| sync-workflow | `ops-sync` | ops-sync: yes |
| brand-canon-guardian | `brand-steward-canon-guardian` | brand-steward-canon-guardian: yes |
| content-strategy | `brand-steward-content-strategy` | brand-steward-content-strategy: yes |
| feature-commanders | `streamerbot-dev-commanders` | streamerbot-dev-commanders: yes |
| feature-squad | `streamerbot-dev-squad` | streamerbot-dev-squad: yes |
| feature-twitch-integration | `streamerbot-dev-twitch` | streamerbot-dev-twitch: yes |
| feature-channel-points | `streamerbot-dev-twitch` | streamerbot-dev-twitch: yes |
| feature-hype-train | `streamerbot-dev-twitch` | streamerbot-dev-twitch: yes |
| feature-voice-commands | `streamerbot-dev-voice-commands` | streamerbot-dev-voice-commands: yes |
| streamerbot-scripting | `streamerbot-dev` | streamerbot-dev: yes |
| buildtools | `ops` | ops: yes |
| creative-art | `art-director` | art-director: yes |
| creative-worldbuilding | `lotat-writer`, `lotat-tech` | lotat-writer: yes, lotat-tech: yes |

## Helpers (`meta/`, etc.)

| Name | Folder under `retired Pi skill mirror/`? | Manifest required_surfaces present? |
|---|---|---|
| meta | yes | retired Pi skill mirror/README.md: yes |
| meta-agents-navigate | yes | retired Pi skill mirror/README.md: yes, retired Pi skill mirror/meta/SKILL.md: yes |
| meta-agents-update | yes | retired Pi skill mirror/README.md: yes, retired Pi skill mirror/meta/SKILL.md: yes |

## Manifest Details

### Declared roles
- streamerbot-dev
- lotat-tech
- lotat-writer
- art-director
- brand-steward
- content-repurposer
- app-dev
- product-dev
- ops

### Canonical sub-skills and required surfaces
- `brand-steward-canon-guardian` (owner: `brand-steward`): `retired Pi skill mirror/README.md`, `retired Pi skill mirror/brand-steward/SKILL.md`
- `brand-steward-content-strategy` (owner: `brand-steward`): `retired Pi skill mirror/README.md`, `retired Pi skill mirror/brand-steward/SKILL.md`
- `content-repurposer-pipeline` (owner: `content-repurposer`): `retired Pi skill mirror/README.md`, `retired Pi skill mirror/content-repurposer/SKILL.md`
- `lotat-writer-canon-guardian` (owner: `lotat-writer`): `retired Pi skill mirror/README.md`, `retired Pi skill mirror/lotat-writer/SKILL.md`
- `ops-change-summary` (owner: `ops`): `retired Pi skill mirror/README.md`, `retired Pi skill mirror/ops/SKILL.md`, `retired Pi skill mirror/streamerbot-dev/SKILL.md`, `retired Pi skill mirror/lotat-tech/SKILL.md`
- `ops-sync` (owner: `ops`): `retired Pi skill mirror/README.md`, `retired Pi skill mirror/ops/SKILL.md`
- `ops-validation` (owner: `ops`): `retired Pi skill mirror/README.md`, `retired Pi skill mirror/ops/SKILL.md`
- `streamerbot-dev-commanders` (owner: `streamerbot-dev`): `retired Pi skill mirror/README.md`, `retired Pi skill mirror/streamerbot-dev/SKILL.md`
- `streamerbot-dev-lotat` (owner: `streamerbot-dev`): `retired Pi skill mirror/README.md`, `retired Pi skill mirror/streamerbot-dev/SKILL.md`
- `streamerbot-dev-squad` (owner: `streamerbot-dev`): `retired Pi skill mirror/README.md`, `retired Pi skill mirror/streamerbot-dev/SKILL.md`
- `streamerbot-dev-twitch` (owner: `streamerbot-dev`): `retired Pi skill mirror/README.md`, `retired Pi skill mirror/streamerbot-dev/SKILL.md`
- `streamerbot-dev-voice-commands` (owner: `streamerbot-dev`): `retired Pi skill mirror/README.md`, `retired Pi skill mirror/streamerbot-dev/SKILL.md`

### Helpers
- `meta`: Meta entry point for agent-infrastructure work canonical_children=`meta-agents-navigate`, `meta-agents-update`
- `meta-agents-navigate`: Navigate and read the `.agents/` tree canonical_children=none
- `meta-agents-update`: Add or update `.agents/` content canonical_children=none

## Summary Statistics

- Total `.md` files in `.agents/`: 87
- Total lines across all `.md` files: 7975
- Roles count: 10 role folders (9 manifest-declared roles)
- Total sub-skill folders: 26
- Total leaf skill files: 1
- Manifest-vs-folder mismatches: 14
- Largest single file: .agents/roles/lotat-tech/skills/engine/session-lifecycle.md, 588 lines
- Smallest non-empty file: .agents/roles/content-repurposer/context/pipeline-dev-notes.md, 7 lines
