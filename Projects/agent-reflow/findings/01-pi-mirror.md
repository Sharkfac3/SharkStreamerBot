# Findings 01 — Pi Mirror Inventory

Generated: 2026-04-26
Source: prompt 01-inventory-pi-mirror

## Wrapper Inventory

| Wrapper | Lines | Frontmatter `name` | Frontmatter `description` | Manifest entry | Body category | Source / target |
|---|---:|---|---|---|---|---|
| `app-dev` | 27 | `app-dev` | Stream interaction apps, dashboards, overlays, and future standalone tooling. Load when building or planning an app that runs outside Streamer.bot. | role | pure-routing stub | .agents/roles/app-dev/role.md + .agents/roles/app-dev/skills/core.md |
| `art-director` | 23 | `art-director` | Diffusion model prompts, character art, stream visuals. Load when generating any art asset for the stream. | role | pure-routing stub | .agents/roles/art-director/role.md + .agents/roles/art-director/skills/core.md |
| `brand-canon-guardian` | 9 | `brand-canon-guardian` | MIGRATED — content moved to .agents/roles/brand-steward/skills/canon-guardian/. Load brand-steward-canon-guardian/SKILL.md instead. | alias | migrated alias | brand-steward-canon-guardian |
| `brand-steward` | 26 | `brand-steward` | Brand consistency for all public-facing output — voice, tone, values, neurodivergent metaphor. Load before writing any chat text, stream titles, announcements, or community messaging. | role | pure-routing stub | .agents/roles/brand-steward/role.md + .agents/roles/brand-steward/skills/core.md |
| `brand-steward-canon-guardian` | 8 | `brand-steward-canon-guardian` | Brand-steward canon review for new characters, lore, and franchise-level consistency. Load when content could change Starship Shamples canon beyond a single story. | canonical sub-skill | pure-routing stub | .agents/roles/brand-steward/skills/canon-guardian/_index.md |
| `brand-steward-content-strategy` | 19 | `brand-steward-content-strategy` | Connects LotAT story content to real-world build sessions. Load when planning stories tied to a specific build or writing build-specific stream content. | canonical sub-skill | pure-routing stub | .agents/roles/brand-steward/skills/content-strategy/_index.md |
| `buildtools` | 9 | `buildtools` | MIGRATED — content moved to .agents/roles/ops/skills/core.md. Load ops/SKILL.md instead. | alias | migrated alias | ops |
| `change-summary` | 9 | `change-summary` | MIGRATED — content moved to .agents/roles/ops/skills/change-summary/. Load ops-change-summary/SKILL.md instead. | alias | migrated alias | ops-change-summary |
| `content-repurposer` | 24 | `content-repurposer` | Short-form content pipeline ownership — clip selection, captions, content calendars, platform formatting, and routing into content-pipeline tooling for repurposed stream content. | role | pure-routing stub | .agents/roles/content-repurposer/role.md + .agents/roles/content-repurposer/skills/core.md |
| `content-repurposer-pipeline` | 16 | `content-repurposer-pipeline` | Content-pipeline tooling for `Tools/ContentPipeline/` — transcription, highlight detection, clip extraction, formatting, review queue, and feedback tooling. | canonical sub-skill | pure-routing stub | .agents/roles/content-repurposer/skills/pipeline/_index.md |
| `content-strategy` | 9 | `content-strategy` | MIGRATED — content moved to .agents/roles/brand-steward/skills/content-strategy/. Load brand-steward-content-strategy/SKILL.md instead. | alias | migrated alias | brand-steward-content-strategy |
| `creative-art` | 9 | `creative-art` | MIGRATED — content moved to .agents/roles/art-director/. Load art-director/SKILL.md instead. | alias | migrated alias | art-director |
| `creative-worldbuilding` | 11 | `creative-worldbuilding` | MIGRATED — split into lotat-writer and lotat-tech roles. Load lotat-writer/SKILL.md or lotat-tech/SKILL.md instead. | alias | migrated alias | lotat-writer → lotat-tech |
| `feature-channel-points` | 9 | `feature-channel-points` | MIGRATED — content moved to .agents/roles/streamerbot-dev/skills/twitch/channel-points.md. Load streamerbot-dev-twitch/SKILL.md instead. | alias | migrated alias | streamerbot-dev-twitch |
| `feature-commanders` | 9 | `feature-commanders` | MIGRATED — content moved to .agents/roles/streamerbot-dev/skills/commanders/. Load streamerbot-dev-commanders/SKILL.md instead. | alias | migrated alias | streamerbot-dev-commanders |
| `feature-hype-train` | 9 | `feature-hype-train` | MIGRATED — content moved to .agents/roles/streamerbot-dev/skills/twitch/hype-train.md. Load streamerbot-dev-twitch/SKILL.md instead. | alias | migrated alias | streamerbot-dev-twitch |
| `feature-squad` | 9 | `feature-squad` | MIGRATED — content moved to .agents/roles/streamerbot-dev/skills/squad/. Load streamerbot-dev-squad/SKILL.md instead. | alias | migrated alias | streamerbot-dev-squad |
| `feature-twitch-integration` | 9 | `feature-twitch-integration` | MIGRATED — content moved to .agents/roles/streamerbot-dev/skills/twitch/. Load streamerbot-dev-twitch/SKILL.md instead. | alias | migrated alias | streamerbot-dev-twitch |
| `feature-voice-commands` | 9 | `feature-voice-commands` | MIGRATED — content moved to .agents/roles/streamerbot-dev/skills/voice-commands/. Load streamerbot-dev-voice-commands/SKILL.md instead. | alias | migrated alias | streamerbot-dev-voice-commands |
| `lotat-tech` | 28 | `lotat-tech` | LotAT technical pipeline — JSON story schema, engine architecture, C# engine implementation. Load when working on the story pipeline or engine layer. | role | pure-routing stub | .agents/roles/lotat-tech/role.md + .agents/roles/lotat-tech/skills/core.md |
| `lotat-writer` | 30 | `lotat-writer` | LotAT story builder — adventure design, narrative content, lore, worldbuilding. Load when writing story JSON, planning adventures, or building out the universe. | role | pure-routing stub | .agents/roles/lotat-writer/role.md + .agents/roles/lotat-writer/skills/core.md |
| `lotat-writer-canon-guardian` | 10 | `lotat-writer-canon-guardian` | LotAT-specific canon review for adventures, lore, and reusable story elements. Load when reviewing LotAT stories or expanding world elements used by adventures. | canonical sub-skill | pure-routing stub | .agents/roles/lotat-writer/skills/canon-guardian/_index.md |
| `meta` | 15 | `meta` | Pi meta-skills — how to navigate and update the .agents/ project knowledge tree. Load when working on agent infrastructure, adding new roles, or updating existing skill content. | helper | pure-routing stub | Pi-only helper; source content lives in wrapper, not .agents role tree |
| `meta-agents-navigate` | 51 | `meta-agents-navigate` | How to navigate and use the .agents/ project knowledge tree. Load when you need to find context for a task or explore what's available in the agent tree. | helper | real content | Pi-only helper; source content lives in wrapper, not .agents role tree |
| `meta-agents-update` | 77 | `meta-agents-update` | How to add or update content in the .agents/ project knowledge tree. Load when creating a new role, adding a skill file, or leaving context notes for future agents. | helper | real content | Pi-only helper; source content lives in wrapper, not .agents role tree |
| `ops` | 20 | `ops` | Operational layer — change summaries, sync workflow, validation, local tooling. Terminal role loaded after every code change. | role | pure-routing stub | .agents/roles/ops/role.md + .agents/roles/ops/skills/core.md |
| `ops-change-summary` | 8 | `ops-change-summary` | Standard response format for code changes — paste targets, setup steps, validation output. Terminal skill — load after every code change. | canonical sub-skill | pure-routing stub | .agents/roles/ops/skills/change-summary/_index.md |
| `ops-sync` | 8 | `ops-sync` | Repo-to-Streamer.bot paste process and validation checklists. Load when preparing to sync scripts into Streamer.bot. | canonical sub-skill | pure-routing stub | .agents/roles/ops/skills/sync/_index.md |
| `ops-validation` | 8 | `ops-validation` | SHARED-CONSTANTS drift checks, local validation checks, schema compliance. Load when running validation passes. | canonical sub-skill | pure-routing stub | .agents/roles/ops/skills/validation/_index.md |
| `product-dev` | 24 | `product-dev` | Product documentation, technical knowledge articles, specifications, and future customer-facing content for R&D products developed on stream. | role | pure-routing stub | .agents/roles/product-dev/role.md + .agents/roles/product-dev/skills/core.md |
| `streamerbot-dev` | 34 | `streamerbot-dev` | Streamer.bot C# actions development. Base role for all .cs script work. Routes to .agents/roles/streamerbot-dev/ for full context. | role | pure-routing stub | .agents/roles/streamerbot-dev/role.md + .agents/roles/streamerbot-dev/skills/core.md |
| `streamerbot-dev-commanders` | 16 | `streamerbot-dev-commanders` | Commander role system (Captain Stretch, The Director, Water Wizard). Load when working on any script under Actions/Commanders/. | canonical sub-skill | pure-routing stub | .agents/roles/streamerbot-dev/skills/commanders/_index.md, .agents/roles/streamerbot-dev/skills/commanders/captain-stretch.md, .agents/roles/streamerbot-dev/skills/commanders/the-director.md, .agents/roles/streamerbot-dev/skills/commanders/water-wizard.md |
| `streamerbot-dev-lotat` | 10 | `streamerbot-dev-lotat` | LotAT C# engine work under Actions/LotAT/. Load when building or modifying the engine that runs story nodes in Streamer.bot. | canonical sub-skill | pure-routing stub | .agents/roles/streamerbot-dev/skills/lotat/_index.md |
| `streamerbot-dev-squad` | 17 | `streamerbot-dev-squad` | Squad mini-games (Clone, Duck, Pedro, Toothless, offering/LotAT steal). Load when working on any script under Actions/Squad/. | canonical sub-skill | pure-routing stub | .agents/roles/streamerbot-dev/skills/squad/_index.md, .agents/roles/streamerbot-dev/skills/squad/clone.md, .agents/roles/streamerbot-dev/skills/squad/duck.md, .agents/roles/streamerbot-dev/skills/squad/pedro.md, .agents/roles/streamerbot-dev/skills/squad/toothless.md |
| `streamerbot-dev-twitch` | 17 | `streamerbot-dev-twitch` | Twitch integrations — stream lifecycle, bits, channel points, hype train. Load when working on scripts under Actions/Twitch Core Integrations/, Actions/Twitch Bits Integrations/, Actions/Twitch Channel Points/, or Actions/Twitch Hype Train/. | canonical sub-skill | pure-routing stub | .agents/roles/streamerbot-dev/skills/twitch/_index.md, .agents/roles/streamerbot-dev/skills/twitch/bits.md, .agents/roles/streamerbot-dev/skills/twitch/channel-points.md, .agents/roles/streamerbot-dev/skills/twitch/core-events.md, .agents/roles/streamerbot-dev/skills/twitch/hype-train.md |
| `streamerbot-dev-voice-commands` | 8 | `streamerbot-dev-voice-commands` | Voice-command mode and OBS scene switching actions under Actions/Voice Commands/. Load when working on voice command scripts. | canonical sub-skill | pure-routing stub | .agents/roles/streamerbot-dev/skills/voice-commands/_index.md |
| `streamerbot-scripting` | 11 | `streamerbot-scripting` | MIGRATED — content moved to .agents/roles/streamerbot-dev/skills/core.md. Load streamerbot-dev/SKILL.md instead. | alias | migrated alias | streamerbot-dev |
| `sync-workflow` | 9 | `sync-workflow` | MIGRATED — content moved to .agents/roles/ops/skills/sync/. Load ops-sync/SKILL.md instead. | alias | migrated alias | ops-sync |

## Routing Stubs

Pure-routing stubs carry little/no domain knowledge; they point Pi at `.agents/roles/` or at another flat wrapper.

| Wrapper | Stub type | `.agents/roles/` targets referenced | Target existence |
|---|---|---|---|
| `app-dev` | role wrapper | `.agents/roles/app-dev/role.md`<br>`.agents/roles/app-dev/skills/core.md`<br>`.agents/roles/app-dev/skills/stream-interactions/_index.md` | `.agents/roles/app-dev/role.md`: yes, `.agents/roles/app-dev/skills/core.md`: yes, `.agents/roles/app-dev/skills/stream-interactions/_index.md`: yes |
| `art-director` | role wrapper | `.agents/roles/art-director/role.md`<br>`.agents/roles/art-director/skills/characters/_index.md`<br>`.agents/roles/art-director/skills/characters/captain-stretch.md`<br>`.agents/roles/art-director/skills/characters/the-director.md`<br>`.agents/roles/art-director/skills/characters/water-wizard.md`<br>`.agents/roles/art-director/skills/core.md`<br>`.agents/roles/art-director/skills/pipeline/_index.md`<br>`.agents/roles/art-director/skills/stream-style/_index.md` | `.agents/roles/art-director/role.md`: yes, `.agents/roles/art-director/skills/characters/_index.md`: yes, `.agents/roles/art-director/skills/characters/captain-stretch.md`: yes, `.agents/roles/art-director/skills/characters/the-director.md`: yes, `.agents/roles/art-director/skills/characters/water-wizard.md`: yes, `.agents/roles/art-director/skills/core.md`: yes, `.agents/roles/art-director/skills/pipeline/_index.md`: yes, `.agents/roles/art-director/skills/stream-style/_index.md`: yes |
| `brand-steward` | role wrapper | `.agents/roles/brand-steward/role.md`<br>`.agents/roles/brand-steward/skills/canon-guardian/_index.md`<br>`.agents/roles/brand-steward/skills/community-growth/_index.md`<br>`.agents/roles/brand-steward/skills/content-strategy/_index.md`<br>`.agents/roles/brand-steward/skills/core.md`<br>`.agents/roles/brand-steward/skills/voice/_index.md` | `.agents/roles/brand-steward/role.md`: yes, `.agents/roles/brand-steward/skills/canon-guardian/_index.md`: yes, `.agents/roles/brand-steward/skills/community-growth/_index.md`: yes, `.agents/roles/brand-steward/skills/content-strategy/_index.md`: yes, `.agents/roles/brand-steward/skills/core.md`: yes, `.agents/roles/brand-steward/skills/voice/_index.md`: yes |
| `brand-steward-canon-guardian` | canonical sub-skill wrapper | `.agents/roles/brand-steward/skills/canon-guardian/_index.md` | `.agents/roles/brand-steward/skills/canon-guardian/_index.md`: yes |
| `brand-steward-content-strategy` | canonical sub-skill wrapper | `.agents/roles/brand-steward/skills/content-strategy/_index.md` | `.agents/roles/brand-steward/skills/content-strategy/_index.md`: yes |
| `content-repurposer` | role wrapper | `.agents/roles/content-repurposer/role.md`<br>`.agents/roles/content-repurposer/skills/clip-strategy/_index.md`<br>`.agents/roles/content-repurposer/skills/core.md`<br>`.agents/roles/content-repurposer/skills/platforms/_index.md` | `.agents/roles/content-repurposer/role.md`: yes, `.agents/roles/content-repurposer/skills/clip-strategy/_index.md`: yes, `.agents/roles/content-repurposer/skills/core.md`: yes, `.agents/roles/content-repurposer/skills/platforms/_index.md`: yes |
| `content-repurposer-pipeline` | canonical sub-skill wrapper | `.agents/roles/content-repurposer/skills/core.md`<br>`.agents/roles/content-repurposer/skills/pipeline/_index.md` | `.agents/roles/content-repurposer/skills/core.md`: yes, `.agents/roles/content-repurposer/skills/pipeline/_index.md`: yes |
| `lotat-tech` | role wrapper | `.agents/roles/lotat-tech/role.md`<br>`.agents/roles/lotat-tech/skills/core.md`<br>`.agents/roles/lotat-tech/skills/engine/_index.md`<br>`.agents/roles/lotat-tech/skills/engine/commands.md`<br>`.agents/roles/lotat-tech/skills/engine/docs-map.md`<br>`.agents/roles/lotat-tech/skills/engine/session-lifecycle.md`<br>`.agents/roles/lotat-tech/skills/engine/state-and-voting.md`<br>`.agents/roles/lotat-tech/skills/story-pipeline/_index.md`<br>`.agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` | `.agents/roles/lotat-tech/role.md`: yes, `.agents/roles/lotat-tech/skills/core.md`: yes, `.agents/roles/lotat-tech/skills/engine/_index.md`: yes, `.agents/roles/lotat-tech/skills/engine/commands.md`: yes, `.agents/roles/lotat-tech/skills/engine/docs-map.md`: yes, `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md`: yes, `.agents/roles/lotat-tech/skills/engine/state-and-voting.md`: yes, `.agents/roles/lotat-tech/skills/story-pipeline/_index.md`: yes, `.agents/roles/lotat-tech/skills/story-pipeline/json-schema.md`: yes |
| `lotat-writer` | role wrapper | `.agents/roles/lotat-writer/role.md`<br>`.agents/roles/lotat-writer/skills/adventures/_index.md`<br>`.agents/roles/lotat-writer/skills/adventures/mechanics.md`<br>`.agents/roles/lotat-writer/skills/adventures/session-format.md`<br>`.agents/roles/lotat-writer/skills/core.md`<br>`.agents/roles/lotat-writer/skills/franchises/starship-shamples.md`<br>`.agents/roles/lotat-writer/skills/universe/_index.md`<br>`.agents/roles/lotat-writer/skills/universe/cast.md`<br>`.agents/roles/lotat-writer/skills/universe/rules.md` | `.agents/roles/lotat-writer/role.md`: yes, `.agents/roles/lotat-writer/skills/adventures/_index.md`: yes, `.agents/roles/lotat-writer/skills/adventures/mechanics.md`: yes, `.agents/roles/lotat-writer/skills/adventures/session-format.md`: yes, `.agents/roles/lotat-writer/skills/core.md`: yes, `.agents/roles/lotat-writer/skills/franchises/starship-shamples.md`: yes, `.agents/roles/lotat-writer/skills/universe/_index.md`: yes, `.agents/roles/lotat-writer/skills/universe/cast.md`: yes, `.agents/roles/lotat-writer/skills/universe/rules.md`: yes |
| `lotat-writer-canon-guardian` | canonical sub-skill wrapper | `.agents/roles/lotat-writer/skills/canon-guardian/_index.md` | `.agents/roles/lotat-writer/skills/canon-guardian/_index.md`: yes |
| `meta` | Pi meta wrapper | none | n/a (wrapper-to-wrapper routing only) |
| `ops` | role wrapper | `.agents/roles/ops/role.md`<br>`.agents/roles/ops/skills/core.md` | `.agents/roles/ops/role.md`: yes, `.agents/roles/ops/skills/core.md`: yes |
| `ops-change-summary` | canonical sub-skill wrapper | `.agents/roles/ops/skills/change-summary/_index.md` | `.agents/roles/ops/skills/change-summary/_index.md`: yes |
| `ops-sync` | canonical sub-skill wrapper | `.agents/roles/ops/skills/sync/_index.md` | `.agents/roles/ops/skills/sync/_index.md`: yes |
| `ops-validation` | canonical sub-skill wrapper | `.agents/roles/ops/skills/validation/_index.md` | `.agents/roles/ops/skills/validation/_index.md`: yes |
| `product-dev` | role wrapper | `.agents/roles/product-dev/role.md`<br>`.agents/roles/product-dev/skills/core.md` | `.agents/roles/product-dev/role.md`: yes, `.agents/roles/product-dev/skills/core.md`: yes |
| `streamerbot-dev` | role wrapper | `.agents/roles/streamerbot-dev/role.md`<br>`.agents/roles/streamerbot-dev/skills/core.md` | `.agents/roles/streamerbot-dev/role.md`: yes, `.agents/roles/streamerbot-dev/skills/core.md`: yes |
| `streamerbot-dev-commanders` | canonical sub-skill wrapper | `.agents/roles/streamerbot-dev/skills/commanders/_index.md`<br>`.agents/roles/streamerbot-dev/skills/commanders/captain-stretch.md`<br>`.agents/roles/streamerbot-dev/skills/commanders/the-director.md`<br>`.agents/roles/streamerbot-dev/skills/commanders/water-wizard.md` | `.agents/roles/streamerbot-dev/skills/commanders/_index.md`: yes, `.agents/roles/streamerbot-dev/skills/commanders/captain-stretch.md`: yes, `.agents/roles/streamerbot-dev/skills/commanders/the-director.md`: yes, `.agents/roles/streamerbot-dev/skills/commanders/water-wizard.md`: yes |
| `streamerbot-dev-lotat` | canonical sub-skill wrapper | `.agents/roles/streamerbot-dev/skills/lotat/_index.md` | `.agents/roles/streamerbot-dev/skills/lotat/_index.md`: yes |
| `streamerbot-dev-squad` | canonical sub-skill wrapper | `.agents/roles/streamerbot-dev/skills/squad/_index.md`<br>`.agents/roles/streamerbot-dev/skills/squad/clone.md`<br>`.agents/roles/streamerbot-dev/skills/squad/duck.md`<br>`.agents/roles/streamerbot-dev/skills/squad/pedro.md`<br>`.agents/roles/streamerbot-dev/skills/squad/toothless.md` | `.agents/roles/streamerbot-dev/skills/squad/_index.md`: yes, `.agents/roles/streamerbot-dev/skills/squad/clone.md`: yes, `.agents/roles/streamerbot-dev/skills/squad/duck.md`: yes, `.agents/roles/streamerbot-dev/skills/squad/pedro.md`: yes, `.agents/roles/streamerbot-dev/skills/squad/toothless.md`: yes |
| `streamerbot-dev-twitch` | canonical sub-skill wrapper | `.agents/roles/streamerbot-dev/skills/twitch/_index.md`<br>`.agents/roles/streamerbot-dev/skills/twitch/bits.md`<br>`.agents/roles/streamerbot-dev/skills/twitch/channel-points.md`<br>`.agents/roles/streamerbot-dev/skills/twitch/core-events.md`<br>`.agents/roles/streamerbot-dev/skills/twitch/hype-train.md` | `.agents/roles/streamerbot-dev/skills/twitch/_index.md`: yes, `.agents/roles/streamerbot-dev/skills/twitch/bits.md`: yes, `.agents/roles/streamerbot-dev/skills/twitch/channel-points.md`: yes, `.agents/roles/streamerbot-dev/skills/twitch/core-events.md`: yes, `.agents/roles/streamerbot-dev/skills/twitch/hype-train.md`: yes |
| `streamerbot-dev-voice-commands` | canonical sub-skill wrapper | `.agents/roles/streamerbot-dev/skills/voice-commands/_index.md` | `.agents/roles/streamerbot-dev/skills/voice-commands/_index.md`: yes |

## Migrated Aliases

| Alias wrapper / original pre-migration name | Canonical wrapper(s) redirected to | Traceable `.agents/roles/` source | Target wrappers exist? | Source exists? |
|---|---|---|---|---|
| `brand-canon-guardian` | `brand-steward-canon-guardian` | `.agents/roles/brand-steward/skills/canon-guardian/_index.md` | `brand-steward-canon-guardian`: yes | `.agents/roles/brand-steward/skills/canon-guardian/_index.md`: yes |
| `buildtools` | `ops` | `.agents/roles/ops/skills/core.md` | `ops`: yes | `.agents/roles/ops/skills/core.md`: yes |
| `change-summary` | `ops-change-summary` | `.agents/roles/ops/skills/change-summary/_index.md` | `ops-change-summary`: yes | `.agents/roles/ops/skills/change-summary/_index.md`: yes |
| `content-strategy` | `brand-steward-content-strategy` | `.agents/roles/brand-steward/skills/content-strategy/_index.md` | `brand-steward-content-strategy`: yes | `.agents/roles/brand-steward/skills/content-strategy/_index.md`: yes |
| `creative-art` | `art-director` | `.agents/roles/art-director/` | `art-director`: yes | `.agents/roles/art-director/`: yes |
| `creative-worldbuilding` | `lotat-writer`, `lotat-tech` | split alias; see canonical wrappers | `lotat-writer`: yes, `lotat-tech`: yes | not directly listed in alias body |
| `feature-channel-points` | `streamerbot-dev-twitch` | `.agents/roles/streamerbot-dev/skills/twitch/channel-points.md` | `streamerbot-dev-twitch`: yes | `.agents/roles/streamerbot-dev/skills/twitch/channel-points.md`: yes |
| `feature-commanders` | `streamerbot-dev-commanders` | `.agents/roles/streamerbot-dev/skills/commanders/` | `streamerbot-dev-commanders`: yes | `.agents/roles/streamerbot-dev/skills/commanders/`: yes |
| `feature-hype-train` | `streamerbot-dev-twitch` | `.agents/roles/streamerbot-dev/skills/twitch/hype-train.md` | `streamerbot-dev-twitch`: yes | `.agents/roles/streamerbot-dev/skills/twitch/hype-train.md`: yes |
| `feature-squad` | `streamerbot-dev-squad` | `.agents/roles/streamerbot-dev/skills/squad/` | `streamerbot-dev-squad`: yes | `.agents/roles/streamerbot-dev/skills/squad/`: yes |
| `feature-twitch-integration` | `streamerbot-dev-twitch` | `.agents/roles/streamerbot-dev/skills/twitch/` | `streamerbot-dev-twitch`: yes | `.agents/roles/streamerbot-dev/skills/twitch/`: yes |
| `feature-voice-commands` | `streamerbot-dev-voice-commands` | `.agents/roles/streamerbot-dev/skills/voice-commands/` | `streamerbot-dev-voice-commands`: yes | `.agents/roles/streamerbot-dev/skills/voice-commands/`: yes |
| `streamerbot-scripting` | `streamerbot-dev` | `.agents/roles/streamerbot-dev/skills/core.md` | `streamerbot-dev`: yes | `.agents/roles/streamerbot-dev/skills/core.md`: yes |
| `sync-workflow` | `ops-sync` | `.agents/roles/ops/skills/sync/_index.md` | `ops-sync`: yes | `.agents/roles/ops/skills/sync/_index.md`: yes |

## Orphans

### Wrappers without a manifest entry
- none

### Manifest entries without a wrapper
- none

### Wrappers pointing to nonexistent `.agents` paths
- none

## Manifest Cross-Check

- Manifest-declared roles with wrappers: `app-dev`, `art-director`, `brand-steward`, `content-repurposer`, `lotat-tech`, `lotat-writer`, `ops`, `product-dev`, `streamerbot-dev`
- Manifest-declared canonical sub-skills with wrappers: `brand-steward-canon-guardian`, `brand-steward-content-strategy`, `content-repurposer-pipeline`, `lotat-writer-canon-guardian`, `ops-change-summary`, `ops-sync`, `ops-validation`, `streamerbot-dev-commanders`, `streamerbot-dev-lotat`, `streamerbot-dev-squad`, `streamerbot-dev-twitch`, `streamerbot-dev-voice-commands`
- Manifest-declared helpers with wrappers: `meta`, `meta-agents-navigate`, `meta-agents-update`
- Manifest-declared aliases with wrappers: `brand-canon-guardian`, `buildtools`, `change-summary`, `content-strategy`, `creative-art`, `creative-worldbuilding`, `feature-channel-points`, `feature-commanders`, `feature-hype-train`, `feature-squad`, `feature-twitch-integration`, `feature-voice-commands`, `streamerbot-scripting`, `sync-workflow`

### Cross-reference to findings/00-current-tree.md

- findings/00 reported 9 manifest roles, 12 canonical sub-skills, 3 helpers, and 14 aliases; this inventory finds wrappers for all 38 expected names.
- findings/00 reported 14 `.agents/` folder-vs-manifest mismatches. Several Pi role wrappers route into those existing-but-undeclared sub-skill folders:
  - `app-dev` wrapper references `stream-interactions` paths that exist in `.agents/` but are not canonical manifest sub-skills.
  - `art-director` wrapper references `characters, pipeline, stream-style` paths that exist in `.agents/` but are not canonical manifest sub-skills.
  - `brand-steward` wrapper references `community-growth, voice` paths that exist in `.agents/` but are not canonical manifest sub-skills.
  - `content-repurposer` wrapper references `clip-strategy, platforms` paths that exist in `.agents/` but are not canonical manifest sub-skills.
  - `lotat-tech` wrapper references `engine, story-pipeline` paths that exist in `.agents/` but are not canonical manifest sub-skills.
  - `lotat-writer` wrapper references `adventures, franchises, universe` paths that exist in `.agents/` but are not canonical manifest sub-skills.
- No Pi wrapper points to a missing `.agents/roles/` path.
- No manifest-declared wrapper is missing from `retired Pi skill mirror/`.

## Summary Statistics

- Total wrapper folders under `retired Pi skill mirror/`: 38
- Total `SKILL.md` files found: 38
- Pure-routing stubs: 22
- Migrated aliases: 14
- Real-content wrappers: 2 (`meta-agents-navigate`, `meta-agents-update`)
- Manifest-backed role wrappers: 9
- Manifest-backed canonical sub-skill wrappers: 12
- Manifest-backed helper wrappers: 3
- Manifest-backed alias wrappers: 14
- Wrappers without manifest entry: 0
- Manifest entries without wrapper: 0
- Missing `.agents` target references: 0

## Pi Cutover Notes

- Pi currently relies on `retired Pi skill mirror/README.md` for project skill discovery and on wrapper `SKILL.md` files for routing. Later cutover that removes `retired Pi skill mirror/` must replace this with an equivalent entry point in the new manifest/docs flow.
- The three meta wrappers (`meta`, `meta-agents-navigate`, `meta-agents-update`) are Pi-specific helpers and do not map to `.agents/roles/*`; they need explicit disposition during cutover.
- Compatibility aliases are dead weight from a single-source routing perspective, but they are still exposed in Pi skill discovery until removed.
