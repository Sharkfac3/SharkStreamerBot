# Findings 04 — Inventory Cross-References

Generated: 2026-04-26
Source: prompt 04-inventory-cross-refs

## Scope

- In-scope files: 144
- Link sources: markdown links (`[text](path)`) and backtick-wrapped path mentions that look like agent-doc paths.
- Inbound counts only count existing references from other in-scope files to in-scope files.
- Directory references are canonicalized to `SKILL.md`, `role.md`, `_index.md`, or `README.md` where present.
- URLs, anchors-only links, globs, API routes, package names, commands, code/runtime artifact paths, template placeholders, and non-doc assets are filtered from broken-link findings.

## Summary

- Total resolved doc/path references recorded: 702
- Broken doc/path references: 256
- Orphans / zero inbound references: 14
- Mutual-reference pairs: 6
- Candidate chains listed: 3

## Broken Links / Missing Targets

| Source | Kind | Raw reference | Resolved target |
|---|---|---|---|
| `.agents/_shared/conventions.md` | backtick | `SCREAMING-KEBAB-CASE.md` | `.agents/_shared/SCREAMING-KEBAB-CASE.md` |
| `.agents/_shared/conventions.md` | backtick | `_index.md` | `.agents/_shared/_index.md` |
| `.agents/_shared/conventions.md` | backtick | `character-name-art-agent.md` | `.agents/_shared/character-name-art-agent.md` |
| `.agents/_shared/conventions.md` | backtick | `franchise-name-agent.md` | `.agents/_shared/franchise-name-agent.md` |
| `.agents/_shared/info-service-protocol.md` | backtick | `humans/info-service/COORDINATION.md` | `.agents/_shared/humans/info-service/COORDINATION.md` |
| `.agents/roles/_template/role.md` | backtick | `ops/skills/change-summary/_index.md` | `.agents/roles/_template/ops/skills/change-summary/_index.md` |
| `.agents/roles/_template/skills/example-skill/_index.md` | backtick | `/core.md` | `core.md` |
| `.agents/roles/app-dev/context/info-service.md` | backtick | `agents/_shared/info-service-protocol.md` | `.agents/roles/app-dev/context/agents/_shared/info-service-protocol.md` |
| `.agents/roles/app-dev/context/info-service.md` | backtick | `agents/_shared/mixitup-api.md` | `.agents/roles/app-dev/context/agents/_shared/mixitup-api.md` |
| `.agents/roles/app-dev/context/info-service.md` | backtick | `humans/info-service/COORDINATION.md` | `.agents/roles/app-dev/context/humans/info-service/COORDINATION.md` |
| `.agents/roles/app-dev/skills/core.md` | backtick | `agents/_shared/info-service-protocol.md` | `.agents/roles/app-dev/skills/agents/_shared/info-service-protocol.md` |
| `.agents/roles/app-dev/skills/core.md` | backtick | `agents/_shared/mixitup-api.md` | `.agents/roles/app-dev/skills/agents/_shared/mixitup-api.md` |
| `.agents/roles/app-dev/skills/core.md` | backtick | `agents/_shared/project.md` | `.agents/roles/app-dev/skills/agents/_shared/project.md` |
| `.agents/roles/app-dev/skills/core.md` | backtick | `agents/roles/app-dev/context/info-service.md` | `.agents/roles/app-dev/skills/agents/roles/app-dev/context/info-service.md` |
| `.agents/roles/app-dev/skills/stream-interactions/_index.md` | backtick | `agents/_shared/info-service-protocol.md` | `.agents/roles/app-dev/skills/stream-interactions/agents/_shared/info-service-protocol.md` |
| `.agents/roles/app-dev/skills/stream-interactions/broker.md` | backtick | `agents/roles/streamerbot-dev/skills/overlay-integration.md` | `.agents/roles/app-dev/skills/stream-interactions/agents/roles/streamerbot-dev/skills/overlay-integration.md` |
| `.agents/roles/app-dev/skills/stream-interactions/protocol.md` | backtick | `/../lotat-tech/skills/engine/session-lifecycle.md` | `../lotat-tech/skills/engine/session-lifecycle.md` |
| `.agents/roles/app-dev/skills/stream-interactions/protocol.md` | backtick | `/../lotat-tech/skills/story-pipeline/json-schema.md` | `../lotat-tech/skills/story-pipeline/json-schema.md` |
| `.agents/roles/app-dev/skills/stream-interactions/protocol.md` | backtick | `session-lifecycle.md` | `.agents/roles/app-dev/skills/stream-interactions/session-lifecycle.md` |
| `.agents/roles/app-dev/skills/stream-interactions/protocol.md` | markdown | `/../lotat-tech/skills/engine/session-lifecycle.md` | `../lotat-tech/skills/engine/session-lifecycle.md` |
| `.agents/roles/app-dev/skills/stream-interactions/protocol.md` | markdown | `/../lotat-tech/skills/story-pipeline/json-schema.md` | `../lotat-tech/skills/story-pipeline/json-schema.md` |
| `.agents/roles/art-director/skills/characters/captain-stretch.md` | backtick | `captain-stretch-art-agent.md` | `.agents/roles/art-director/skills/characters/captain-stretch-art-agent.md` |
| `.agents/roles/art-director/skills/characters/the-director.md` | backtick | `the-director-art-agent.md` | `.agents/roles/art-director/skills/characters/the-director-art-agent.md` |
| `.agents/roles/art-director/skills/characters/water-wizard.md` | backtick | `water-wizard-art-agent.md` | `.agents/roles/art-director/skills/characters/water-wizard-art-agent.md` |
| `.agents/roles/art-director/skills/core.md` | backtick | `agents/_shared/project.md` | `.agents/roles/art-director/skills/agents/_shared/project.md` |
| `.agents/roles/art-director/skills/pipeline/_index.md` | backtick | `/characters/_index.md` | `characters/_index.md` |
| `.agents/roles/art-director/skills/pipeline/_index.md` | backtick | `/core.md` | `core.md` |
| `.agents/roles/art-director/skills/pipeline/_index.md` | backtick | `/stream-style/_index.md` | `stream-style/_index.md` |
| `.agents/roles/art-director/skills/stream-style/_index.md` | backtick | `art-director/skills/core.md` | `.agents/roles/art-director/skills/stream-style/art-director/skills/core.md` |
| `.agents/roles/brand-steward/skills/core.md` | backtick | `agents/_shared/project.md` | `.agents/roles/brand-steward/skills/agents/_shared/project.md` |
| `.agents/roles/content-repurposer/skills/clip-strategy/_index.md` | backtick | `skills/platforms/_index.md` | `.agents/roles/content-repurposer/skills/clip-strategy/skills/platforms/_index.md` |
| `.agents/roles/content-repurposer/skills/core.md` | backtick | `agents/_shared/project.md` | `.agents/roles/content-repurposer/skills/agents/_shared/project.md` |
| `.agents/roles/content-repurposer/skills/pipeline/_index.md` | backtick | `/clip-strategy/_index.md` | `clip-strategy/_index.md` |
| `.agents/roles/content-repurposer/skills/pipeline/_index.md` | backtick | `/core.md` | `core.md` |
| `.agents/roles/content-repurposer/skills/pipeline/_index.md` | backtick | `/platforms/_index.md` | `platforms/_index.md` |
| `.agents/roles/lotat-tech/role.md` | backtick | `ops/skills/change-summary/_index.md` | `.agents/roles/lotat-tech/ops/skills/change-summary/_index.md` |
| `.agents/roles/lotat-tech/skills/core.md` | backtick | `agents/_shared/project.md` | `.agents/roles/lotat-tech/skills/agents/_shared/project.md` |
| `.agents/roles/lotat-tech/skills/core.md` | backtick | `agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` | `.agents/roles/lotat-tech/skills/agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` |
| `.agents/roles/lotat-tech/skills/core.md` | backtick | `skills/engine/docs-map.md` | `.agents/roles/lotat-tech/skills/skills/engine/docs-map.md` |
| `.agents/roles/lotat-tech/skills/core.md` | backtick | `skills/engine/session-lifecycle.md` | `.agents/roles/lotat-tech/skills/skills/engine/session-lifecycle.md` |
| `.agents/roles/lotat-tech/skills/core.md` | backtick | `skills/engine/state-and-voting.md` | `.agents/roles/lotat-tech/skills/skills/engine/state-and-voting.md` |
| `.agents/roles/lotat-tech/skills/engine/_index.md` | backtick | `agents/roles/app-dev/skills/stream-interactions/lotat-rendering.md` | `.agents/roles/lotat-tech/skills/engine/agents/roles/app-dev/skills/stream-interactions/lotat-rendering.md` |
| `.agents/roles/lotat-tech/skills/engine/docs-map.md` | backtick | `/core.md` | `core.md` |
| `.agents/roles/lotat-tech/skills/engine/docs-map.md` | backtick | `/story-pipeline/json-schema.md` | `story-pipeline/json-schema.md` |
| `.agents/roles/lotat-tech/skills/engine/docs-map.md` | backtick | `agents/roles/streamerbot-dev/skills/lotat/_index.md` | `.agents/roles/lotat-tech/skills/engine/agents/roles/streamerbot-dev/skills/lotat/_index.md` |
| `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md` | backtick | `/story-pipeline/json-schema.md` | `story-pipeline/json-schema.md` |
| `.agents/roles/lotat-tech/skills/story-pipeline/_index.md` | backtick | `agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` | `.agents/roles/lotat-tech/skills/story-pipeline/agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` |
| `.agents/roles/lotat-tech/skills/story-pipeline/_index.md` | backtick | `engine/commands.md` | `.agents/roles/lotat-tech/skills/story-pipeline/engine/commands.md` |
| `.agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` | backtick | `agents/roles/lotat-tech/skills/engine/commands.md` | `.agents/roles/lotat-tech/skills/story-pipeline/agents/roles/lotat-tech/skills/engine/commands.md` |
| `.agents/roles/lotat-writer/skills/canon-guardian/_index.md` | backtick | `pi/skills/brand-steward-canon-guardian/SKILL.md` | `.agents/roles/lotat-writer/skills/canon-guardian/pi/skills/brand-steward-canon-guardian/SKILL.md` |
| `.agents/roles/lotat-writer/skills/core.md` | backtick | `agents/_shared/project.md` | `.agents/roles/lotat-writer/skills/agents/_shared/project.md` |
| `.agents/roles/lotat-writer/skills/core.md` | backtick | `lotat-tech/skills/engine/commands.md` | `.agents/roles/lotat-writer/skills/lotat-tech/skills/engine/commands.md` |
| `.agents/roles/ops/skills/change-summary/_index.md` | backtick | `ops/skills/sync/_index.md` | `.agents/roles/ops/skills/change-summary/ops/skills/sync/_index.md` |
| `.agents/roles/ops/skills/core.md` | backtick | `agents/ENTRY.md` | `.agents/roles/ops/skills/agents/ENTRY.md` |
| `.agents/roles/ops/skills/core.md` | backtick | `agents/_shared/project.md` | `.agents/roles/ops/skills/agents/_shared/project.md` |
| `.agents/roles/ops/skills/core.md` | backtick | `pi/skills/README.md` | `.agents/roles/ops/skills/pi/skills/README.md` |
| `.agents/roles/ops/skills/validation/_index.md` | backtick | `agents/ENTRY.md` | `.agents/roles/ops/skills/validation/agents/ENTRY.md` |
| `.agents/roles/ops/skills/validation/_index.md` | backtick | `pi/skills/README.md` | `.agents/roles/ops/skills/validation/pi/skills/README.md` |
| `.agents/roles/product-dev/role.md` | backtick | `agents/roles/ops/skills/change-summary/_index.md` | `.agents/roles/product-dev/agents/roles/ops/skills/change-summary/_index.md` |
| `.agents/roles/product-dev/skills/core.md` | backtick | `agents/_shared/project.md` | `.agents/roles/product-dev/skills/agents/_shared/project.md` |
| `.agents/roles/streamerbot-dev/role.md` | backtick | `ops/skills/change-summary/_index.md` | `.agents/roles/streamerbot-dev/ops/skills/change-summary/_index.md` |
| `.agents/roles/streamerbot-dev/skills/commanders/_index.md` | backtick | `Actions/Commanders/Captain Stretch/README.md` | `Actions/Commanders/Captain` |
| `.agents/roles/streamerbot-dev/skills/commanders/_index.md` | backtick | `Actions/Commanders/The Director/README.md` | `Actions/Commanders/The` |
| `.agents/roles/streamerbot-dev/skills/commanders/_index.md` | backtick | `Actions/Commanders/Water Wizard/README.md` | `Actions/Commanders/Water` |
| `.agents/roles/streamerbot-dev/skills/commanders/captain-stretch.md` | backtick | `Actions/Commanders/Captain Stretch/README.md` | `Actions/Commanders/Captain` |
| `.agents/roles/streamerbot-dev/skills/commanders/the-director.md` | backtick | `Actions/Commanders/The Director/README.md` | `Actions/Commanders/The` |
| `.agents/roles/streamerbot-dev/skills/commanders/water-wizard.md` | backtick | `Actions/Commanders/Water Wizard/README.md` | `Actions/Commanders/Water` |
| `.agents/roles/streamerbot-dev/skills/core.md` | backtick | `agents/_shared/project.md` | `.agents/roles/streamerbot-dev/skills/agents/_shared/project.md` |
| `.agents/roles/streamerbot-dev/skills/lotat/_index.md` | backtick | `agents/roles/lotat-tech/skills/engine/commands.md` | `.agents/roles/streamerbot-dev/skills/lotat/agents/roles/lotat-tech/skills/engine/commands.md` |
| `.agents/roles/streamerbot-dev/skills/lotat/_index.md` | backtick | `agents/roles/lotat-tech/skills/engine/docs-map.md` | `.agents/roles/streamerbot-dev/skills/lotat/agents/roles/lotat-tech/skills/engine/docs-map.md` |
| `.agents/roles/streamerbot-dev/skills/lotat/_index.md` | backtick | `agents/roles/lotat-tech/skills/engine/session-lifecycle.md` | `.agents/roles/streamerbot-dev/skills/lotat/agents/roles/lotat-tech/skills/engine/session-lifecycle.md` |
| `.agents/roles/streamerbot-dev/skills/lotat/_index.md` | backtick | `agents/roles/lotat-tech/skills/engine/state-and-voting.md` | `.agents/roles/streamerbot-dev/skills/lotat/agents/roles/lotat-tech/skills/engine/state-and-voting.md` |
| `.agents/roles/streamerbot-dev/skills/overlay-integration.md` | backtick | `agents/roles/app-dev/skills/stream-interactions/broker.md` | `.agents/roles/streamerbot-dev/skills/agents/roles/app-dev/skills/stream-interactions/broker.md` |
| `.agents/roles/streamerbot-dev/skills/overlay-integration.md` | backtick | `agents/roles/app-dev/skills/stream-interactions/protocol.md` | `.agents/roles/streamerbot-dev/skills/agents/roles/app-dev/skills/stream-interactions/protocol.md` |
| `.agents/roles/streamerbot-dev/skills/overlay-integration.md` | backtick | `protocol.md` | `.agents/roles/streamerbot-dev/skills/protocol.md` |
| `.agents/roles/streamerbot-dev/skills/squad/_index.md` | backtick | `agents/roles/app-dev/skills/stream-interactions/squad-rendering.md` | `.agents/roles/streamerbot-dev/skills/squad/agents/roles/app-dev/skills/stream-interactions/squad-rendering.md` |
| `.agents/roles/streamerbot-dev/skills/squad/_index.md` | backtick | `streamerbot-dev/skills/core.md` | `.agents/roles/streamerbot-dev/skills/squad/streamerbot-dev/skills/core.md` |
| `.agents/roles/streamerbot-dev/skills/twitch/_index.md` | backtick | `Actions/Twitch Bits Integrations/README.md` | `Actions/Twitch` |
| `.agents/roles/streamerbot-dev/skills/twitch/_index.md` | backtick | `Actions/Twitch Core Integrations/README.md` | `Actions/Twitch` |
| `.agents/roles/streamerbot-dev/skills/twitch/bits.md` | backtick | `Actions/Twitch Bits Integrations/README.md` | `Actions/Twitch` |
| `.agents/roles/streamerbot-dev/skills/twitch/channel-points.md` | backtick | `Actions/Twitch Channel Points/README.md` | `Actions/Twitch` |
| `.agents/roles/streamerbot-dev/skills/twitch/core-events.md` | backtick | `Actions/Twitch Core Integrations/README.md` | `Actions/Twitch` |
| `.agents/roles/streamerbot-dev/skills/twitch/hype-train.md` | backtick | `Actions/Twitch Hype Train/README.md` | `Actions/Twitch` |
| `.agents/roles/streamerbot-dev/skills/voice-commands/_index.md` | backtick | `Actions/Voice Commands/README.md` | `Actions/Voice` |
| `.agents/roles/streamerbot-dev/skills/voice-commands/_index.md` | backtick | `streamerbot-dev/skills/core.md` | `.agents/roles/streamerbot-dev/skills/voice-commands/streamerbot-dev/skills/core.md` |
| `retired Pi skill mirror/README.md` | backtick | `agents/ENTRY.md` | `retired Pi skill mirror/agents/ENTRY.md` |
| `retired Pi skill mirror/README.md` | backtick | `role.md` | `retired Pi skill mirror/role.md` |
| `retired Pi skill mirror/README.md` | backtick | `skills/core.md` | `retired Pi skill mirror/skills/core.md` |
| `retired Pi skill mirror/app-dev/SKILL.md` | backtick | `agents/roles/app-dev/role.md` | `retired Pi skill mirror/app-dev/agents/roles/app-dev/role.md` |
| `retired Pi skill mirror/app-dev/SKILL.md` | backtick | `agents/roles/app-dev/skills/core.md` | `retired Pi skill mirror/app-dev/agents/roles/app-dev/skills/core.md` |
| `retired Pi skill mirror/app-dev/SKILL.md` | backtick | `agents/roles/app-dev/skills/stream-interactions/_index.md` | `retired Pi skill mirror/app-dev/agents/roles/app-dev/skills/stream-interactions/_index.md` |
| `retired Pi skill mirror/art-director/SKILL.md` | backtick | `agents/roles/art-director/role.md` | `retired Pi skill mirror/art-director/agents/roles/art-director/role.md` |
| `retired Pi skill mirror/art-director/SKILL.md` | backtick | `agents/roles/art-director/skills/characters/_index.md` | `retired Pi skill mirror/art-director/agents/roles/art-director/skills/characters/_index.md` |
| `retired Pi skill mirror/art-director/SKILL.md` | backtick | `agents/roles/art-director/skills/characters/captain-stretch.md` | `retired Pi skill mirror/art-director/agents/roles/art-director/skills/characters/captain-stretch.md` |
| `retired Pi skill mirror/art-director/SKILL.md` | backtick | `agents/roles/art-director/skills/characters/the-director.md` | `retired Pi skill mirror/art-director/agents/roles/art-director/skills/characters/the-director.md` |
| `retired Pi skill mirror/art-director/SKILL.md` | backtick | `agents/roles/art-director/skills/characters/water-wizard.md` | `retired Pi skill mirror/art-director/agents/roles/art-director/skills/characters/water-wizard.md` |
| `retired Pi skill mirror/art-director/SKILL.md` | backtick | `agents/roles/art-director/skills/core.md` | `retired Pi skill mirror/art-director/agents/roles/art-director/skills/core.md` |
| `retired Pi skill mirror/art-director/SKILL.md` | backtick | `agents/roles/art-director/skills/pipeline/_index.md` | `retired Pi skill mirror/art-director/agents/roles/art-director/skills/pipeline/_index.md` |
| `retired Pi skill mirror/art-director/SKILL.md` | backtick | `agents/roles/art-director/skills/stream-style/_index.md` | `retired Pi skill mirror/art-director/agents/roles/art-director/skills/stream-style/_index.md` |
| `retired Pi skill mirror/brand-canon-guardian/SKILL.md` | backtick | `agents/roles/brand-steward/skills/canon-guardian/_index.md` | `retired Pi skill mirror/brand-canon-guardian/agents/roles/brand-steward/skills/canon-guardian/_index.md` |
| `retired Pi skill mirror/brand-canon-guardian/SKILL.md` | backtick | `pi/skills/brand-steward-canon-guardian/SKILL.md` | `retired Pi skill mirror/brand-canon-guardian/pi/skills/brand-steward-canon-guardian/SKILL.md` |
| `retired Pi skill mirror/brand-steward-canon-guardian/SKILL.md` | backtick | `agents/roles/brand-steward/skills/canon-guardian/_index.md` | `retired Pi skill mirror/brand-steward-canon-guardian/agents/roles/brand-steward/skills/canon-guardian/_index.md` |
| `retired Pi skill mirror/brand-steward-content-strategy/SKILL.md` | backtick | `agents/roles/brand-steward/skills/content-strategy/_index.md` | `retired Pi skill mirror/brand-steward-content-strategy/agents/roles/brand-steward/skills/content-strategy/_index.md` |
| `retired Pi skill mirror/brand-steward/SKILL.md` | backtick | `agents/roles/brand-steward/role.md` | `retired Pi skill mirror/brand-steward/agents/roles/brand-steward/role.md` |
| `retired Pi skill mirror/brand-steward/SKILL.md` | backtick | `agents/roles/brand-steward/skills/canon-guardian/_index.md` | `retired Pi skill mirror/brand-steward/agents/roles/brand-steward/skills/canon-guardian/_index.md` |
| `retired Pi skill mirror/brand-steward/SKILL.md` | backtick | `agents/roles/brand-steward/skills/community-growth/_index.md` | `retired Pi skill mirror/brand-steward/agents/roles/brand-steward/skills/community-growth/_index.md` |
| `retired Pi skill mirror/brand-steward/SKILL.md` | backtick | `agents/roles/brand-steward/skills/content-strategy/_index.md` | `retired Pi skill mirror/brand-steward/agents/roles/brand-steward/skills/content-strategy/_index.md` |
| `retired Pi skill mirror/brand-steward/SKILL.md` | backtick | `agents/roles/brand-steward/skills/core.md` | `retired Pi skill mirror/brand-steward/agents/roles/brand-steward/skills/core.md` |
| `retired Pi skill mirror/brand-steward/SKILL.md` | backtick | `agents/roles/brand-steward/skills/voice/_index.md` | `retired Pi skill mirror/brand-steward/agents/roles/brand-steward/skills/voice/_index.md` |
| `retired Pi skill mirror/brand-steward/SKILL.md` | backtick | `brand-steward-canon-guardian/SKILL.md` | `retired Pi skill mirror/brand-steward/brand-steward-canon-guardian/SKILL.md` |
| `retired Pi skill mirror/brand-steward/SKILL.md` | backtick | `brand-steward-content-strategy/SKILL.md` | `retired Pi skill mirror/brand-steward/brand-steward-content-strategy/SKILL.md` |
| `retired Pi skill mirror/buildtools/SKILL.md` | backtick | `agents/roles/ops/skills/core.md` | `retired Pi skill mirror/buildtools/agents/roles/ops/skills/core.md` |
| `retired Pi skill mirror/buildtools/SKILL.md` | backtick | `pi/skills/ops/SKILL.md` | `retired Pi skill mirror/buildtools/pi/skills/ops/SKILL.md` |
| `retired Pi skill mirror/change-summary/SKILL.md` | backtick | `agents/roles/ops/skills/change-summary/_index.md` | `retired Pi skill mirror/change-summary/agents/roles/ops/skills/change-summary/_index.md` |
| `retired Pi skill mirror/change-summary/SKILL.md` | backtick | `pi/skills/ops-change-summary/SKILL.md` | `retired Pi skill mirror/change-summary/pi/skills/ops-change-summary/SKILL.md` |
| `retired Pi skill mirror/content-repurposer-pipeline/SKILL.md` | backtick | `agents/roles/content-repurposer/skills/core.md` | `retired Pi skill mirror/content-repurposer-pipeline/agents/roles/content-repurposer/skills/core.md` |
| `retired Pi skill mirror/content-repurposer-pipeline/SKILL.md` | backtick | `agents/roles/content-repurposer/skills/pipeline/_index.md` | `retired Pi skill mirror/content-repurposer-pipeline/agents/roles/content-repurposer/skills/pipeline/_index.md` |
| `retired Pi skill mirror/content-repurposer/SKILL.md` | backtick | `agents/roles/content-repurposer/role.md` | `retired Pi skill mirror/content-repurposer/agents/roles/content-repurposer/role.md` |
| `retired Pi skill mirror/content-repurposer/SKILL.md` | backtick | `agents/roles/content-repurposer/skills/clip-strategy/_index.md` | `retired Pi skill mirror/content-repurposer/agents/roles/content-repurposer/skills/clip-strategy/_index.md` |
| `retired Pi skill mirror/content-repurposer/SKILL.md` | backtick | `agents/roles/content-repurposer/skills/core.md` | `retired Pi skill mirror/content-repurposer/agents/roles/content-repurposer/skills/core.md` |
| `retired Pi skill mirror/content-repurposer/SKILL.md` | backtick | `agents/roles/content-repurposer/skills/platforms/_index.md` | `retired Pi skill mirror/content-repurposer/agents/roles/content-repurposer/skills/platforms/_index.md` |
| `retired Pi skill mirror/content-repurposer/SKILL.md` | backtick | `content-repurposer-pipeline/SKILL.md` | `retired Pi skill mirror/content-repurposer/content-repurposer-pipeline/SKILL.md` |
| `retired Pi skill mirror/content-strategy/SKILL.md` | backtick | `agents/roles/brand-steward/skills/content-strategy/_index.md` | `retired Pi skill mirror/content-strategy/agents/roles/brand-steward/skills/content-strategy/_index.md` |
| `retired Pi skill mirror/content-strategy/SKILL.md` | backtick | `pi/skills/brand-steward-content-strategy/SKILL.md` | `retired Pi skill mirror/content-strategy/pi/skills/brand-steward-content-strategy/SKILL.md` |
| `retired Pi skill mirror/creative-art/SKILL.md` | backtick | `pi/skills/art-director/SKILL.md` | `retired Pi skill mirror/creative-art/pi/skills/art-director/SKILL.md` |
| `retired Pi skill mirror/creative-worldbuilding/SKILL.md` | backtick | `pi/skills/lotat-tech/SKILL.md` | `retired Pi skill mirror/creative-worldbuilding/pi/skills/lotat-tech/SKILL.md` |
| `retired Pi skill mirror/creative-worldbuilding/SKILL.md` | backtick | `pi/skills/lotat-writer/SKILL.md` | `retired Pi skill mirror/creative-worldbuilding/pi/skills/lotat-writer/SKILL.md` |
| `retired Pi skill mirror/feature-channel-points/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/twitch/channel-points.md` | `retired Pi skill mirror/feature-channel-points/agents/roles/streamerbot-dev/skills/twitch/channel-points.md` |
| `retired Pi skill mirror/feature-channel-points/SKILL.md` | backtick | `pi/skills/streamerbot-dev-twitch/SKILL.md` | `retired Pi skill mirror/feature-channel-points/pi/skills/streamerbot-dev-twitch/SKILL.md` |
| `retired Pi skill mirror/feature-commanders/SKILL.md` | backtick | `pi/skills/streamerbot-dev-commanders/SKILL.md` | `retired Pi skill mirror/feature-commanders/pi/skills/streamerbot-dev-commanders/SKILL.md` |
| `retired Pi skill mirror/feature-hype-train/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/twitch/hype-train.md` | `retired Pi skill mirror/feature-hype-train/agents/roles/streamerbot-dev/skills/twitch/hype-train.md` |
| `retired Pi skill mirror/feature-hype-train/SKILL.md` | backtick | `pi/skills/streamerbot-dev-twitch/SKILL.md` | `retired Pi skill mirror/feature-hype-train/pi/skills/streamerbot-dev-twitch/SKILL.md` |
| `retired Pi skill mirror/feature-squad/SKILL.md` | backtick | `pi/skills/streamerbot-dev-squad/SKILL.md` | `retired Pi skill mirror/feature-squad/pi/skills/streamerbot-dev-squad/SKILL.md` |
| `retired Pi skill mirror/feature-twitch-integration/SKILL.md` | backtick | `pi/skills/streamerbot-dev-twitch/SKILL.md` | `retired Pi skill mirror/feature-twitch-integration/pi/skills/streamerbot-dev-twitch/SKILL.md` |
| `retired Pi skill mirror/feature-voice-commands/SKILL.md` | backtick | `pi/skills/streamerbot-dev-voice-commands/SKILL.md` | `retired Pi skill mirror/feature-voice-commands/pi/skills/streamerbot-dev-voice-commands/SKILL.md` |
| `retired Pi skill mirror/lotat-tech/SKILL.md` | backtick | `agents/roles/lotat-tech/role.md` | `retired Pi skill mirror/lotat-tech/agents/roles/lotat-tech/role.md` |
| `retired Pi skill mirror/lotat-tech/SKILL.md` | backtick | `agents/roles/lotat-tech/skills/core.md` | `retired Pi skill mirror/lotat-tech/agents/roles/lotat-tech/skills/core.md` |
| `retired Pi skill mirror/lotat-tech/SKILL.md` | backtick | `agents/roles/lotat-tech/skills/engine/_index.md` | `retired Pi skill mirror/lotat-tech/agents/roles/lotat-tech/skills/engine/_index.md` |
| `retired Pi skill mirror/lotat-tech/SKILL.md` | backtick | `agents/roles/lotat-tech/skills/engine/commands.md` | `retired Pi skill mirror/lotat-tech/agents/roles/lotat-tech/skills/engine/commands.md` |
| `retired Pi skill mirror/lotat-tech/SKILL.md` | backtick | `agents/roles/lotat-tech/skills/engine/docs-map.md` | `retired Pi skill mirror/lotat-tech/agents/roles/lotat-tech/skills/engine/docs-map.md` |
| `retired Pi skill mirror/lotat-tech/SKILL.md` | backtick | `agents/roles/lotat-tech/skills/engine/session-lifecycle.md` | `retired Pi skill mirror/lotat-tech/agents/roles/lotat-tech/skills/engine/session-lifecycle.md` |
| `retired Pi skill mirror/lotat-tech/SKILL.md` | backtick | `agents/roles/lotat-tech/skills/engine/state-and-voting.md` | `retired Pi skill mirror/lotat-tech/agents/roles/lotat-tech/skills/engine/state-and-voting.md` |
| `retired Pi skill mirror/lotat-tech/SKILL.md` | backtick | `agents/roles/lotat-tech/skills/story-pipeline/_index.md` | `retired Pi skill mirror/lotat-tech/agents/roles/lotat-tech/skills/story-pipeline/_index.md` |
| `retired Pi skill mirror/lotat-tech/SKILL.md` | backtick | `agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` | `retired Pi skill mirror/lotat-tech/agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` |
| `retired Pi skill mirror/lotat-tech/SKILL.md` | backtick | `ops-change-summary/SKILL.md` | `retired Pi skill mirror/lotat-tech/ops-change-summary/SKILL.md` |
| `retired Pi skill mirror/lotat-writer-canon-guardian/SKILL.md` | backtick | `agents/roles/lotat-writer/skills/canon-guardian/_index.md` | `retired Pi skill mirror/lotat-writer-canon-guardian/agents/roles/lotat-writer/skills/canon-guardian/_index.md` |
| `retired Pi skill mirror/lotat-writer-canon-guardian/SKILL.md` | backtick | `pi/skills/brand-steward-canon-guardian/SKILL.md` | `retired Pi skill mirror/lotat-writer-canon-guardian/pi/skills/brand-steward-canon-guardian/SKILL.md` |
| `retired Pi skill mirror/lotat-writer/SKILL.md` | backtick | `agents/roles/lotat-writer/role.md` | `retired Pi skill mirror/lotat-writer/agents/roles/lotat-writer/role.md` |
| `retired Pi skill mirror/lotat-writer/SKILL.md` | backtick | `agents/roles/lotat-writer/skills/adventures/_index.md` | `retired Pi skill mirror/lotat-writer/agents/roles/lotat-writer/skills/adventures/_index.md` |
| `retired Pi skill mirror/lotat-writer/SKILL.md` | backtick | `agents/roles/lotat-writer/skills/adventures/mechanics.md` | `retired Pi skill mirror/lotat-writer/agents/roles/lotat-writer/skills/adventures/mechanics.md` |
| `retired Pi skill mirror/lotat-writer/SKILL.md` | backtick | `agents/roles/lotat-writer/skills/adventures/session-format.md` | `retired Pi skill mirror/lotat-writer/agents/roles/lotat-writer/skills/adventures/session-format.md` |
| `retired Pi skill mirror/lotat-writer/SKILL.md` | backtick | `agents/roles/lotat-writer/skills/core.md` | `retired Pi skill mirror/lotat-writer/agents/roles/lotat-writer/skills/core.md` |
| `retired Pi skill mirror/lotat-writer/SKILL.md` | backtick | `agents/roles/lotat-writer/skills/franchises/starship-shamples.md` | `retired Pi skill mirror/lotat-writer/agents/roles/lotat-writer/skills/franchises/starship-shamples.md` |
| `retired Pi skill mirror/lotat-writer/SKILL.md` | backtick | `agents/roles/lotat-writer/skills/universe/_index.md` | `retired Pi skill mirror/lotat-writer/agents/roles/lotat-writer/skills/universe/_index.md` |
| `retired Pi skill mirror/lotat-writer/SKILL.md` | backtick | `agents/roles/lotat-writer/skills/universe/cast.md` | `retired Pi skill mirror/lotat-writer/agents/roles/lotat-writer/skills/universe/cast.md` |
| `retired Pi skill mirror/lotat-writer/SKILL.md` | backtick | `agents/roles/lotat-writer/skills/universe/rules.md` | `retired Pi skill mirror/lotat-writer/agents/roles/lotat-writer/skills/universe/rules.md` |
| `retired Pi skill mirror/lotat-writer/SKILL.md` | backtick | `brand-steward-canon-guardian/SKILL.md` | `retired Pi skill mirror/lotat-writer/brand-steward-canon-guardian/SKILL.md` |
| `retired Pi skill mirror/lotat-writer/SKILL.md` | backtick | `lotat-writer-canon-guardian/SKILL.md` | `retired Pi skill mirror/lotat-writer/lotat-writer-canon-guardian/SKILL.md` |
| `retired Pi skill mirror/meta-agents-navigate/SKILL.md` | backtick | `ENTRY.md` | `retired Pi skill mirror/meta-agents-navigate/ENTRY.md` |
| `retired Pi skill mirror/meta-agents-navigate/SKILL.md` | backtick | `_index.md` | `retired Pi skill mirror/meta-agents-navigate/_index.md` |
| `retired Pi skill mirror/meta-agents-navigate/SKILL.md` | backtick | `agents/ENTRY.md` | `retired Pi skill mirror/meta-agents-navigate/agents/ENTRY.md` |
| `retired Pi skill mirror/meta-agents-update/SKILL.md` | backtick | `ENTRY.md` | `retired Pi skill mirror/meta-agents-update/ENTRY.md` |
| `retired Pi skill mirror/meta-agents-update/SKILL.md` | backtick | `_index.md` | `retired Pi skill mirror/meta-agents-update/_index.md` |
| `retired Pi skill mirror/meta-agents-update/SKILL.md` | backtick | `pi/skills/README.md` | `retired Pi skill mirror/meta-agents-update/pi/skills/README.md` |
| `retired Pi skill mirror/meta-agents-update/SKILL.md` | backtick | `role.md` | `retired Pi skill mirror/meta-agents-update/role.md` |
| `retired Pi skill mirror/meta/SKILL.md` | backtick | `meta-agents-navigate/SKILL.md` | `retired Pi skill mirror/meta/meta-agents-navigate/SKILL.md` |
| `retired Pi skill mirror/meta/SKILL.md` | backtick | `meta-agents-update/SKILL.md` | `retired Pi skill mirror/meta/meta-agents-update/SKILL.md` |
| `retired Pi skill mirror/ops-change-summary/SKILL.md` | backtick | `agents/roles/ops/skills/change-summary/_index.md` | `retired Pi skill mirror/ops-change-summary/agents/roles/ops/skills/change-summary/_index.md` |
| `retired Pi skill mirror/ops-sync/SKILL.md` | backtick | `agents/roles/ops/skills/sync/_index.md` | `retired Pi skill mirror/ops-sync/agents/roles/ops/skills/sync/_index.md` |
| `retired Pi skill mirror/ops-validation/SKILL.md` | backtick | `agents/roles/ops/skills/validation/_index.md` | `retired Pi skill mirror/ops-validation/agents/roles/ops/skills/validation/_index.md` |
| `retired Pi skill mirror/ops/SKILL.md` | backtick | `agents/roles/ops/role.md` | `retired Pi skill mirror/ops/agents/roles/ops/role.md` |
| `retired Pi skill mirror/ops/SKILL.md` | backtick | `agents/roles/ops/skills/core.md` | `retired Pi skill mirror/ops/agents/roles/ops/skills/core.md` |
| `retired Pi skill mirror/ops/SKILL.md` | backtick | `ops-change-summary/SKILL.md` | `retired Pi skill mirror/ops/ops-change-summary/SKILL.md` |
| `retired Pi skill mirror/ops/SKILL.md` | backtick | `ops-sync/SKILL.md` | `retired Pi skill mirror/ops/ops-sync/SKILL.md` |
| `retired Pi skill mirror/ops/SKILL.md` | backtick | `ops-validation/SKILL.md` | `retired Pi skill mirror/ops/ops-validation/SKILL.md` |
| `retired Pi skill mirror/product-dev/SKILL.md` | backtick | `agents/roles/product-dev/role.md` | `retired Pi skill mirror/product-dev/agents/roles/product-dev/role.md` |
| `retired Pi skill mirror/product-dev/SKILL.md` | backtick | `agents/roles/product-dev/skills/core.md` | `retired Pi skill mirror/product-dev/agents/roles/product-dev/skills/core.md` |
| `retired Pi skill mirror/streamerbot-dev-commanders/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/commanders/_index.md` | `retired Pi skill mirror/streamerbot-dev-commanders/agents/roles/streamerbot-dev/skills/commanders/_index.md` |
| `retired Pi skill mirror/streamerbot-dev-commanders/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/commanders/captain-stretch.md` | `retired Pi skill mirror/streamerbot-dev-commanders/agents/roles/streamerbot-dev/skills/commanders/captain-stretch.md` |
| `retired Pi skill mirror/streamerbot-dev-commanders/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/commanders/the-director.md` | `retired Pi skill mirror/streamerbot-dev-commanders/agents/roles/streamerbot-dev/skills/commanders/the-director.md` |
| `retired Pi skill mirror/streamerbot-dev-commanders/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/commanders/water-wizard.md` | `retired Pi skill mirror/streamerbot-dev-commanders/agents/roles/streamerbot-dev/skills/commanders/water-wizard.md` |
| `retired Pi skill mirror/streamerbot-dev-lotat/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/lotat/_index.md` | `retired Pi skill mirror/streamerbot-dev-lotat/agents/roles/streamerbot-dev/skills/lotat/_index.md` |
| `retired Pi skill mirror/streamerbot-dev-lotat/SKILL.md` | backtick | `lotat-tech/SKILL.md` | `retired Pi skill mirror/streamerbot-dev-lotat/lotat-tech/SKILL.md` |
| `retired Pi skill mirror/streamerbot-dev-squad/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/squad/_index.md` | `retired Pi skill mirror/streamerbot-dev-squad/agents/roles/streamerbot-dev/skills/squad/_index.md` |
| `retired Pi skill mirror/streamerbot-dev-squad/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/squad/clone.md` | `retired Pi skill mirror/streamerbot-dev-squad/agents/roles/streamerbot-dev/skills/squad/clone.md` |
| `retired Pi skill mirror/streamerbot-dev-squad/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/squad/duck.md` | `retired Pi skill mirror/streamerbot-dev-squad/agents/roles/streamerbot-dev/skills/squad/duck.md` |
| `retired Pi skill mirror/streamerbot-dev-squad/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/squad/pedro.md` | `retired Pi skill mirror/streamerbot-dev-squad/agents/roles/streamerbot-dev/skills/squad/pedro.md` |
| `retired Pi skill mirror/streamerbot-dev-squad/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/squad/toothless.md` | `retired Pi skill mirror/streamerbot-dev-squad/agents/roles/streamerbot-dev/skills/squad/toothless.md` |
| `retired Pi skill mirror/streamerbot-dev-twitch/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/twitch/_index.md` | `retired Pi skill mirror/streamerbot-dev-twitch/agents/roles/streamerbot-dev/skills/twitch/_index.md` |
| `retired Pi skill mirror/streamerbot-dev-twitch/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/twitch/bits.md` | `retired Pi skill mirror/streamerbot-dev-twitch/agents/roles/streamerbot-dev/skills/twitch/bits.md` |
| `retired Pi skill mirror/streamerbot-dev-twitch/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/twitch/channel-points.md` | `retired Pi skill mirror/streamerbot-dev-twitch/agents/roles/streamerbot-dev/skills/twitch/channel-points.md` |
| `retired Pi skill mirror/streamerbot-dev-twitch/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/twitch/core-events.md` | `retired Pi skill mirror/streamerbot-dev-twitch/agents/roles/streamerbot-dev/skills/twitch/core-events.md` |
| `retired Pi skill mirror/streamerbot-dev-twitch/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/twitch/hype-train.md` | `retired Pi skill mirror/streamerbot-dev-twitch/agents/roles/streamerbot-dev/skills/twitch/hype-train.md` |
| `retired Pi skill mirror/streamerbot-dev-voice-commands/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/voice-commands/_index.md` | `retired Pi skill mirror/streamerbot-dev-voice-commands/agents/roles/streamerbot-dev/skills/voice-commands/_index.md` |
| `retired Pi skill mirror/streamerbot-dev/SKILL.md` | backtick | `agents/roles/streamerbot-dev/role.md` | `retired Pi skill mirror/streamerbot-dev/agents/roles/streamerbot-dev/role.md` |
| `retired Pi skill mirror/streamerbot-dev/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/core.md` | `retired Pi skill mirror/streamerbot-dev/agents/roles/streamerbot-dev/skills/core.md` |
| `retired Pi skill mirror/streamerbot-dev/SKILL.md` | backtick | `ops-change-summary/SKILL.md` | `retired Pi skill mirror/streamerbot-dev/ops-change-summary/SKILL.md` |
| `retired Pi skill mirror/streamerbot-dev/SKILL.md` | backtick | `streamerbot-dev-commanders/SKILL.md` | `retired Pi skill mirror/streamerbot-dev/streamerbot-dev-commanders/SKILL.md` |
| `retired Pi skill mirror/streamerbot-dev/SKILL.md` | backtick | `streamerbot-dev-lotat/SKILL.md` | `retired Pi skill mirror/streamerbot-dev/streamerbot-dev-lotat/SKILL.md` |
| `retired Pi skill mirror/streamerbot-dev/SKILL.md` | backtick | `streamerbot-dev-squad/SKILL.md` | `retired Pi skill mirror/streamerbot-dev/streamerbot-dev-squad/SKILL.md` |
| `retired Pi skill mirror/streamerbot-dev/SKILL.md` | backtick | `streamerbot-dev-twitch/SKILL.md` | `retired Pi skill mirror/streamerbot-dev/streamerbot-dev-twitch/SKILL.md` |
| `retired Pi skill mirror/streamerbot-dev/SKILL.md` | backtick | `streamerbot-dev-voice-commands/SKILL.md` | `retired Pi skill mirror/streamerbot-dev/streamerbot-dev-voice-commands/SKILL.md` |
| `retired Pi skill mirror/streamerbot-scripting/SKILL.md` | backtick | `agents/roles/streamerbot-dev/skills/core.md` | `retired Pi skill mirror/streamerbot-scripting/agents/roles/streamerbot-dev/skills/core.md` |
| `retired Pi skill mirror/streamerbot-scripting/SKILL.md` | backtick | `pi/skills/streamerbot-dev/SKILL.md` | `retired Pi skill mirror/streamerbot-scripting/pi/skills/streamerbot-dev/SKILL.md` |
| `retired Pi skill mirror/sync-workflow/SKILL.md` | backtick | `agents/roles/ops/skills/sync/_index.md` | `retired Pi skill mirror/sync-workflow/agents/roles/ops/skills/sync/_index.md` |
| `retired Pi skill mirror/sync-workflow/SKILL.md` | backtick | `pi/skills/ops-sync/SKILL.md` | `retired Pi skill mirror/sync-workflow/pi/skills/ops-sync/SKILL.md` |
| `AGENTS.md` | backtick | `agents/ENTRY.md` | `agents/ENTRY.md` |
| `AGENTS.md` | backtick | `pi/skills/README.md` | `pi/skills/README.md` |
| `Actions/Overlay/README.md` | backtick | `agents/roles/app-dev/skills/stream-interactions/protocol.md` | `Actions/Overlay/agents/roles/app-dev/skills/stream-interactions/protocol.md` |
| `Apps/info-service/README.md` | backtick | `agents/_shared/info-service-protocol.md` | `Apps/info-service/agents/_shared/info-service-protocol.md` |
| `Apps/stream-overlay/README.md` | backtick | `SHARED-CONSTANTS.md` | `Apps/stream-overlay/SHARED-CONSTANTS.md` |
| `Apps/stream-overlay/README.md` | backtick | `agents/roles/streamerbot-dev/skills/overlay-integration.md` | `Apps/stream-overlay/agents/roles/streamerbot-dev/skills/overlay-integration.md` |
| `CLAUDE.md` | backtick | `agents/ENTRY.md` | `agents/ENTRY.md` |
| `CLAUDE.md` | backtick | `agents/roles/ops/skills/change-summary/_index.md` | `agents/roles/ops/skills/change-summary/_index.md` |
| `Docs/AGENT-WORKFLOW.md` | backtick | `SHARED-CONSTANTS.md` | `Docs/SHARED-CONSTANTS.md` |
| `Docs/INFO-SERVICE-PLAN.md` | backtick | `Actions/Intros/README.md` | `Actions/Intros/README.md` |
| `Docs/INFO-SERVICE-PLAN.md` | backtick | `COORDINATION.md` | `Docs/COORDINATION.md` |
| `Docs/INFO-SERVICE-PLAN.md` | backtick | `SHARED-CONSTANTS.md` | `Docs/SHARED-CONSTANTS.md` |
| `Docs/INFO-SERVICE-PLAN.md` | backtick | `_shared/info-service-protocol.md` | `Docs/_shared/info-service-protocol.md` |
| `Docs/INFO-SERVICE-PLAN.md` | backtick | `agents/_shared/info-service-protocol.md` | `Docs/agents/_shared/info-service-protocol.md` |
| `Docs/INFO-SERVICE-PLAN.md` | backtick | `agents/_shared/mixitup-api.md` | `Docs/agents/_shared/mixitup-api.md` |
| `Docs/INFO-SERVICE-PLAN.md` | backtick | `agents/roles/app-dev/role.md` | `Docs/agents/roles/app-dev/role.md` |
| `Docs/INFO-SERVICE-PLAN.md` | backtick | `agents/roles/app-dev/skills/core.md` | `Docs/agents/roles/app-dev/skills/core.md` |
| `Docs/INFO-SERVICE-PLAN.md` | backtick | `agents/roles/app-dev/skills/stream-interactions/_index.md` | `Docs/agents/roles/app-dev/skills/stream-interactions/_index.md` |
| `Docs/INFO-SERVICE-PLAN.md` | backtick | `humans/info-service/COORDINATION.md` | `Docs/humans/info-service/COORDINATION.md` |
| `Docs/INFO-SERVICE-PLAN.md` | backtick | `info-service-protocol.md` | `Docs/info-service-protocol.md` |
| `Docs/ONBOARDING.md` | backtick | `agents/ENTRY.md` | `Docs/agents/ENTRY.md` |
| `Docs/ONBOARDING.md` | backtick | `agents/roles/art-director/skills/core.md` | `Docs/agents/roles/art-director/skills/core.md` |
| `Docs/ONBOARDING.md` | backtick | `agents/roles/brand-steward/skills/canon-guardian/_index.md` | `Docs/agents/roles/brand-steward/skills/canon-guardian/_index.md` |
| `Docs/ONBOARDING.md` | backtick | `agents/roles/brand-steward/skills/content-strategy/_index.md` | `Docs/agents/roles/brand-steward/skills/content-strategy/_index.md` |
| `Docs/ONBOARDING.md` | backtick | `agents/roles/brand-steward/skills/core.md` | `Docs/agents/roles/brand-steward/skills/core.md` |
| `Docs/ONBOARDING.md` | backtick | `agents/roles/content-repurposer/skills/core.md` | `Docs/agents/roles/content-repurposer/skills/core.md` |
| `Docs/ONBOARDING.md` | backtick | `agents/roles/content-repurposer/skills/pipeline/_index.md` | `Docs/agents/roles/content-repurposer/skills/pipeline/_index.md` |
| `Docs/ONBOARDING.md` | backtick | `agents/roles/lotat-tech/skills/core.md` | `Docs/agents/roles/lotat-tech/skills/core.md` |
| `Docs/ONBOARDING.md` | backtick | `agents/roles/lotat-writer/skills/core.md` | `Docs/agents/roles/lotat-writer/skills/core.md` |
| `Docs/ONBOARDING.md` | backtick | `agents/roles/ops/skills/core.md` | `Docs/agents/roles/ops/skills/core.md` |
| `Docs/ONBOARDING.md` | backtick | `agents/roles/product-dev/skills/core.md` | `Docs/agents/roles/product-dev/skills/core.md` |
| `Docs/ONBOARDING.md` | backtick | `agents/roles/streamerbot-dev/skills/core.md` | `Docs/agents/roles/streamerbot-dev/skills/core.md` |
| `Docs/ONBOARDING.md` | backtick | `role.md` | `Docs/role.md` |
| `Docs/ONBOARDING.md` | backtick | `roles/art-director/role.md` | `Docs/roles/art-director/role.md` |
| `Docs/ONBOARDING.md` | backtick | `roles/brand-steward/role.md` | `Docs/roles/brand-steward/role.md` |
| `Docs/ONBOARDING.md` | backtick | `roles/content-repurposer/role.md` | `Docs/roles/content-repurposer/role.md` |
| `Docs/ONBOARDING.md` | backtick | `roles/lotat-tech/role.md` | `Docs/roles/lotat-tech/role.md` |
| `Docs/ONBOARDING.md` | backtick | `roles/lotat-writer/role.md` | `Docs/roles/lotat-writer/role.md` |
| `Docs/ONBOARDING.md` | backtick | `roles/ops/role.md` | `Docs/roles/ops/role.md` |
| `Docs/ONBOARDING.md` | backtick | `roles/product-dev/role.md` | `Docs/roles/product-dev/role.md` |
| `Docs/ONBOARDING.md` | backtick | `roles/streamerbot-dev/role.md` | `Docs/roles/streamerbot-dev/role.md` |
| `README.md` | backtick | `Actions/Commanders/Captain Stretch/README.md` | `Actions/Commanders/Captain` |
| `README.md` | backtick | `Actions/Commanders/The Director/README.md` | `Actions/Commanders/The` |
| `README.md` | backtick | `Actions/Commanders/Water Wizard/README.md` | `Actions/Commanders/Water` |
| `README.md` | backtick | `Actions/Twitch Bits Integrations/README.md` | `Actions/Twitch` |
| `README.md` | backtick | `Actions/Twitch Channel Points/README.md` | `Actions/Twitch` |
| `README.md` | backtick | `Actions/Twitch Core Integrations/README.md` | `Actions/Twitch` |
| `README.md` | backtick | `Actions/Twitch Hype Train/README.md` | `Actions/Twitch` |
| `README.md` | backtick | `Actions/Voice Commands/README.md` | `Actions/Voice` |
| `README.md` | backtick | `agents/ENTRY.md` | `agents/ENTRY.md` |

## Orphans / Zero Inbound References

These files are in scope but no other in-scope file links to them. Root entry files may be intentionally external-entry or operator-entry files.

- `.agents/ENTRY.md`
- `.agents/roles/app-dev/skills/stream-interactions/lotat-rendering.md`
- `.agents/roles/app-dev/skills/stream-interactions/squad-rendering.md`
- `.agents/roles/content-repurposer/context/pipeline-dev-notes.md`
- `.agents/roles/lotat-writer/skills/franchises/_index.md`
- `.agents/roles/streamerbot-dev/context/patterns.md`
- `.agents/roles/streamerbot-dev/skills/commanders/_index.md`
- `.agents/roles/streamerbot-dev/skills/lotat/_index.md`
- `.agents/roles/streamerbot-dev/skills/overlay-integration.md`
- `.agents/roles/streamerbot-dev/skills/squad/_index.md`
- `.agents/roles/streamerbot-dev/skills/twitch/_index.md`
- `.agents/roles/streamerbot-dev/skills/voice-commands/_index.md`
- `retired Pi skill mirror/README.md`
- `CLAUDE.md`

### Lost Progressive Disclosure Candidates

Orphans that appear to contain important navigation, protocol, workflow, or role/skill content and may need an inbound route:

- `.agents/ENTRY.md`
- `.agents/roles/app-dev/skills/stream-interactions/lotat-rendering.md`
- `.agents/roles/app-dev/skills/stream-interactions/squad-rendering.md`
- `.agents/roles/content-repurposer/context/pipeline-dev-notes.md`
- `.agents/roles/lotat-writer/skills/franchises/_index.md`
- `.agents/roles/streamerbot-dev/context/patterns.md`
- `.agents/roles/streamerbot-dev/skills/commanders/_index.md`
- `.agents/roles/streamerbot-dev/skills/lotat/_index.md`
- `.agents/roles/streamerbot-dev/skills/overlay-integration.md`
- `.agents/roles/streamerbot-dev/skills/squad/_index.md`
- `.agents/roles/streamerbot-dev/skills/twitch/_index.md`
- `.agents/roles/streamerbot-dev/skills/voice-commands/_index.md`
- `retired Pi skill mirror/README.md`
- `CLAUDE.md`

## Hubs — Top 10 by Inbound Count

| Rank | File | Inbound refs |
|---:|---|---:|
| 1 | `Creative/README.md` | 11 |
| 2 | `Apps/stream-overlay/README.md` | 9 |
| 3 | `WORKING.md` | 8 |
| 4 | `Apps/info-service/README.md` | 7 |
| 5 | `Tools/ContentPipeline/README.md` | 7 |
| 6 | `.agents/roles/lotat-tech/skills/engine/commands.md` | 5 |
| 7 | `AGENTS.md` | 5 |
| 8 | `Creative/Art/README.md` | 5 |
| 9 | `Creative/WorldBuilding/README.md` | 5 |
| 10 | `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md` | 4 |

## Mutual-Reference Pairs

- `.agents/roles/app-dev/skills/stream-interactions/broker.md` ↔ `.agents/roles/app-dev/skills/stream-interactions/protocol.md`
- `.agents/roles/lotat-tech/skills/engine/_index.md` ↔ `.agents/roles/lotat-tech/skills/engine/docs-map.md`
- `.agents/roles/lotat-tech/skills/engine/docs-map.md` ↔ `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md`
- `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md` ↔ `.agents/roles/lotat-tech/skills/engine/state-and-voting.md`
- `AGENTS.md` ↔ `Docs/ONBOARDING.md`
- `Docs/AGENT-WORKFLOW.md` ↔ `WORKING.md`

## Candidate Linear Chains

Linear paths where each step has exactly one continuing outbound doc reference during greedy traversal; use as review hints, not proof of required load order.

- `.agents/ENTRY.md` → `.agents/_shared/info-service-protocol.md` → `Docs/INFO-SERVICE-PLAN.md` → `Apps/info-service/README.md`
- `.agents/roles/app-dev/skills/stream-interactions/_index.md` → `.agents/roles/app-dev/skills/stream-interactions/broker.md` → `.agents/roles/app-dev/skills/stream-interactions/protocol.md` → `.agents/roles/app-dev/skills/stream-interactions/overlay.md`
- `retired Pi skill mirror/meta-agents-navigate/SKILL.md` → `.agents/_shared/info-service-protocol.md` → `Docs/INFO-SERVICE-PLAN.md` → `Apps/info-service/README.md`

## Adjacency List

Every in-scope file appears below with outbound and inbound references.

### `.agents/ENTRY.md`
- Outbound refs (17):
  - backtick: `conventions.md` → `.agents/_shared/conventions.md` (ok)
  - backtick: `coordination.md` → `.agents/_shared/coordination.md` (ok)
  - backtick: `info-service-protocol.md` → `.agents/_shared/info-service-protocol.md` (ok)
  - backtick: `mixitup-api.md` → `.agents/_shared/mixitup-api.md` (ok)
  - backtick: `_shared/project.md` → `.agents/_shared/project.md` (ok)
  - backtick: `project.md` → `.agents/_shared/project.md` (ok)
  - backtick: `roles/_template/` → `.agents/roles/_template/role.md` (ok)
  - backtick: `roles/app-dev/` → `.agents/roles/app-dev/role.md` (ok)
  - backtick: `roles/art-director/` → `.agents/roles/art-director/role.md` (ok)
  - backtick: `roles/brand-steward/` → `.agents/roles/brand-steward/role.md` (ok)
  - backtick: `roles/content-repurposer/` → `.agents/roles/content-repurposer/role.md` (ok)
  - backtick: `roles/lotat-tech/` → `.agents/roles/lotat-tech/role.md` (ok)
  - backtick: `roles/lotat-writer/` → `.agents/roles/lotat-writer/role.md` (ok)
  - backtick: `roles/ops/` → `.agents/roles/ops/role.md` (ok)
  - backtick: `roles/product-dev/` → `.agents/roles/product-dev/role.md` (ok)
  - backtick: `roles/streamerbot-dev/` → `.agents/roles/streamerbot-dev/role.md` (ok)
  - backtick: `WORKING.md` → `WORKING.md` (ok)
- Inbound refs (0):
  - None

### `.agents/_shared/conventions.md`
- Outbound refs (9):
  - backtick: `SCREAMING-KEBAB-CASE.md` → `.agents/_shared/SCREAMING-KEBAB-CASE.md` (BROKEN)
  - backtick: `_index.md` → `.agents/_shared/_index.md` (BROKEN)
  - backtick: `character-name-art-agent.md` → `.agents/_shared/character-name-art-agent.md` (BROKEN)
  - backtick: `franchise-name-agent.md` → `.agents/_shared/franchise-name-agent.md` (BROKEN)
  - backtick: `Creative/` → `Creative/README.md` (ok)
  - backtick: `Docs/` → `Docs` (ok)
  - backtick: `Docs/AGENT-WORKFLOW.md` → `Docs/AGENT-WORKFLOW.md` (ok)
  - backtick: `Tools/` → `Tools/README.md` (ok)
  - backtick: `WORKING.md` → `WORKING.md` (ok)
- Inbound refs (2):
  - `.agents/ENTRY.md`
  - `retired Pi skill mirror/meta-agents-navigate/SKILL.md`

### `.agents/_shared/coordination.md`
- Outbound refs (6):
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Creative/Brand/BRAND-VOICE.md` → `Creative/Brand/BRAND-VOICE.md` (ok)
  - backtick: `Creative/` → `Creative/README.md` (ok)
  - backtick: `Docs/` → `Docs` (ok)
  - backtick: `Tools/` → `Tools/README.md` (ok)
  - backtick: `WORKING.md` → `WORKING.md` (ok)
- Inbound refs (2):
  - `.agents/ENTRY.md`
  - `retired Pi skill mirror/meta-agents-navigate/SKILL.md`

### `.agents/_shared/info-service-protocol.md`
- Outbound refs (3):
  - backtick: `humans/info-service/COORDINATION.md` → `.agents/_shared/humans/info-service/COORDINATION.md` (BROKEN)
  - backtick: `Docs/INFO-SERVICE-PLAN.md §Schemas` → `Docs/INFO-SERVICE-PLAN.md` (ok)
  - backtick: `Docs/INFO-SERVICE-PLAN.md §Schemas §1` → `Docs/INFO-SERVICE-PLAN.md` (ok)
- Inbound refs (2):
  - `.agents/ENTRY.md`
  - `retired Pi skill mirror/meta-agents-navigate/SKILL.md`

### `.agents/_shared/mixitup-api.md`
- Outbound refs (1):
  - backtick: `Actions/HELPER-SNIPPETS.md` → `Actions/HELPER-SNIPPETS.md` (ok)
- Inbound refs (2):
  - `.agents/ENTRY.md`
  - `retired Pi skill mirror/meta-agents-navigate/SKILL.md`

### `.agents/_shared/project.md`
- Outbound refs (10):
  - backtick: `Actions/HELPER-SNIPPETS.md` → `Actions/HELPER-SNIPPETS.md` (ok)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Creative/Brand/BRAND-IDENTITY.md` → `Creative/Brand/BRAND-IDENTITY.md` (ok)
  - backtick: `Creative/Brand/BRAND-VOICE.md` → `Creative/Brand/BRAND-VOICE.md` (ok)
  - backtick: `Creative/Brand/CHARACTER-CODEX.md` → `Creative/Brand/CHARACTER-CODEX.md` (ok)
  - backtick: `Creative/` → `Creative/README.md` (ok)
  - backtick: `Docs/` → `Docs` (ok)
  - backtick: `Docs/ONBOARDING.md` → `Docs/ONBOARDING.md` (ok)
  - backtick: `Tools/` → `Tools/README.md` (ok)
  - backtick: `WORKING.md` → `WORKING.md` (ok)
- Inbound refs (2):
  - `.agents/ENTRY.md`
  - `retired Pi skill mirror/meta-agents-navigate/SKILL.md`

### `.agents/roles/_template/role.md`
- Outbound refs (3):
  - backtick: `ops/skills/change-summary/_index.md` → `.agents/roles/_template/ops/skills/change-summary/_index.md` (BROKEN)
  - backtick: `skills/core.md` → `.agents/roles/_template/skills/core.md` (ok)
  - backtick: `skills/example-skill/_index.md` → `.agents/roles/_template/skills/example-skill/_index.md` (ok)
- Inbound refs (1):
  - `.agents/ENTRY.md`

### `.agents/roles/_template/skills/core.md`
- Outbound refs (0):
  - None
- Inbound refs (1):
  - `.agents/roles/_template/role.md`

### `.agents/roles/_template/skills/example-skill/_index.md`
- Outbound refs (2):
  - backtick: `_index.md` → `.agents/roles/_template/skills/example-skill/_index.md` (ok)
  - backtick: `/core.md` → `core.md` (BROKEN)
- Inbound refs (1):
  - `.agents/roles/_template/role.md`

### `.agents/roles/app-dev/context/info-service.md`
- Outbound refs (6):
  - backtick: `agents/_shared/info-service-protocol.md` → `.agents/roles/app-dev/context/agents/_shared/info-service-protocol.md` (BROKEN)
  - backtick: `agents/_shared/mixitup-api.md` → `.agents/roles/app-dev/context/agents/_shared/mixitup-api.md` (BROKEN)
  - backtick: `humans/info-service/COORDINATION.md` → `.agents/roles/app-dev/context/humans/info-service/COORDINATION.md` (BROKEN)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Apps/info-service/` → `Apps/info-service/README.md` (ok)
  - backtick: `Docs/INFO-SERVICE-PLAN.md` → `Docs/INFO-SERVICE-PLAN.md` (ok)
- Inbound refs (1):
  - `.agents/roles/app-dev/role.md`

### `.agents/roles/app-dev/role.md`
- Outbound refs (6):
  - backtick: `context/info-service.md` → `.agents/roles/app-dev/context/info-service.md` (ok)
  - backtick: `skills/core.md` → `.agents/roles/app-dev/skills/core.md` (ok)
  - backtick: `skills/stream-interactions/_index.md` → `.agents/roles/app-dev/skills/stream-interactions/_index.md` (ok)
  - backtick: `Apps/info-service/` → `Apps/info-service/README.md` (ok)
  - backtick: `Apps/production-manager/` → `Apps/production-manager/README.md` (ok)
  - backtick: `Apps/stream-overlay/` → `Apps/stream-overlay/README.md` (ok)
- Inbound refs (1):
  - `.agents/ENTRY.md`

### `.agents/roles/app-dev/skills/core.md`
- Outbound refs (8):
  - backtick: `agents/_shared/info-service-protocol.md` → `.agents/roles/app-dev/skills/agents/_shared/info-service-protocol.md` (BROKEN)
  - backtick: `agents/_shared/mixitup-api.md` → `.agents/roles/app-dev/skills/agents/_shared/mixitup-api.md` (BROKEN)
  - backtick: `agents/_shared/project.md` → `.agents/roles/app-dev/skills/agents/_shared/project.md` (BROKEN)
  - backtick: `agents/roles/app-dev/context/info-service.md` → `.agents/roles/app-dev/skills/agents/roles/app-dev/context/info-service.md` (BROKEN)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Apps/info-service/` → `Apps/info-service/README.md` (ok)
  - backtick: `Apps/production-manager/` → `Apps/production-manager/README.md` (ok)
  - backtick: `Assets/` → `Assets/README.md` (ok)
- Inbound refs (1):
  - `.agents/roles/app-dev/role.md`

### `.agents/roles/app-dev/skills/stream-interactions/_index.md`
- Outbound refs (9):
  - backtick: `agents/_shared/info-service-protocol.md` → `.agents/roles/app-dev/skills/stream-interactions/agents/_shared/info-service-protocol.md` (BROKEN)
  - backtick: `broker.md` → `.agents/roles/app-dev/skills/stream-interactions/broker.md` (ok)
  - markdown: `broker.md` → `.agents/roles/app-dev/skills/stream-interactions/broker.md` (ok)
  - backtick: `overlay.md` → `.agents/roles/app-dev/skills/stream-interactions/overlay.md` (ok)
  - markdown: `overlay.md` → `.agents/roles/app-dev/skills/stream-interactions/overlay.md` (ok)
  - backtick: `Actions/Overlay/` → `Actions/Overlay/README.md` (ok)
  - backtick: `Apps/info-service/` → `Apps/info-service/README.md` (ok)
  - backtick: `Apps/production-manager/` → `Apps/production-manager/README.md` (ok)
  - backtick: `Apps/stream-overlay/` → `Apps/stream-overlay/README.md` (ok)
- Inbound refs (2):
  - `.agents/roles/app-dev/role.md`
  - `.agents/roles/app-dev/skills/stream-interactions/protocol.md`

### `.agents/roles/app-dev/skills/stream-interactions/asset-system.md`
- Outbound refs (0):
  - None
- Inbound refs (2):
  - `.agents/roles/app-dev/skills/stream-interactions/lotat-rendering.md`
  - `.agents/roles/app-dev/skills/stream-interactions/overlay.md`

### `.agents/roles/app-dev/skills/stream-interactions/broker.md`
- Outbound refs (3):
  - backtick: `agents/roles/streamerbot-dev/skills/overlay-integration.md` → `.agents/roles/app-dev/skills/stream-interactions/agents/roles/streamerbot-dev/skills/overlay-integration.md` (BROKEN)
  - backtick: `protocol.md` → `.agents/roles/app-dev/skills/stream-interactions/protocol.md` (ok)
  - markdown: `protocol.md` → `.agents/roles/app-dev/skills/stream-interactions/protocol.md` (ok)
- Inbound refs (2):
  - `.agents/roles/app-dev/skills/stream-interactions/_index.md`
  - `.agents/roles/app-dev/skills/stream-interactions/protocol.md`

### `.agents/roles/app-dev/skills/stream-interactions/lotat-rendering.md`
- Outbound refs (1):
  - backtick: `asset-system.md` → `.agents/roles/app-dev/skills/stream-interactions/asset-system.md` (ok)
- Inbound refs (0):
  - None

### `.agents/roles/app-dev/skills/stream-interactions/overlay.md`
- Outbound refs (3):
  - backtick: `asset-system.md` → `.agents/roles/app-dev/skills/stream-interactions/asset-system.md` (ok)
  - markdown: `asset-system.md` → `.agents/roles/app-dev/skills/stream-interactions/asset-system.md` (ok)
  - backtick: `Apps/stream-overlay/` → `Apps/stream-overlay/README.md` (ok)
- Inbound refs (2):
  - `.agents/roles/app-dev/skills/stream-interactions/_index.md`
  - `.agents/roles/app-dev/skills/stream-interactions/protocol.md`

### `.agents/roles/app-dev/skills/stream-interactions/protocol.md`
- Outbound refs (11):
  - backtick: `/../lotat-tech/skills/engine/session-lifecycle.md` → `../lotat-tech/skills/engine/session-lifecycle.md` (BROKEN)
  - markdown: `/../lotat-tech/skills/engine/session-lifecycle.md` → `../lotat-tech/skills/engine/session-lifecycle.md` (BROKEN)
  - backtick: `/../lotat-tech/skills/story-pipeline/json-schema.md` → `../lotat-tech/skills/story-pipeline/json-schema.md` (BROKEN)
  - markdown: `/../lotat-tech/skills/story-pipeline/json-schema.md` → `../lotat-tech/skills/story-pipeline/json-schema.md` (BROKEN)
  - backtick: `_index.md` → `.agents/roles/app-dev/skills/stream-interactions/_index.md` (ok)
  - markdown: `_index.md` → `.agents/roles/app-dev/skills/stream-interactions/_index.md` (ok)
  - backtick: `broker.md` → `.agents/roles/app-dev/skills/stream-interactions/broker.md` (ok)
  - markdown: `broker.md` → `.agents/roles/app-dev/skills/stream-interactions/broker.md` (ok)
  - backtick: `overlay.md` → `.agents/roles/app-dev/skills/stream-interactions/overlay.md` (ok)
  - markdown: `overlay.md` → `.agents/roles/app-dev/skills/stream-interactions/overlay.md` (ok)
  - backtick: `session-lifecycle.md` → `.agents/roles/app-dev/skills/stream-interactions/session-lifecycle.md` (BROKEN)
- Inbound refs (1):
  - `.agents/roles/app-dev/skills/stream-interactions/broker.md`

### `.agents/roles/app-dev/skills/stream-interactions/squad-rendering.md`
- Outbound refs (0):
  - None
- Inbound refs (0):
  - None

### `.agents/roles/art-director/role.md`
- Outbound refs (5):
  - backtick: `skills/characters/_index.md` → `.agents/roles/art-director/skills/characters/_index.md` (ok)
  - backtick: `skills/core.md` → `.agents/roles/art-director/skills/core.md` (ok)
  - backtick: `skills/pipeline/_index.md` → `.agents/roles/art-director/skills/pipeline/_index.md` (ok)
  - backtick: `skills/stream-style/_index.md` → `.agents/roles/art-director/skills/stream-style/_index.md` (ok)
  - backtick: `Tools/ArtPipeline/` → `Tools/ArtPipeline/README.md` (ok)
- Inbound refs (1):
  - `.agents/ENTRY.md`

### `.agents/roles/art-director/skills/characters/_index.md`
- Outbound refs (7):
  - backtick: `captain-stretch.md` → `.agents/roles/art-director/skills/characters/captain-stretch.md` (ok)
  - backtick: `the-director.md` → `.agents/roles/art-director/skills/characters/the-director.md` (ok)
  - backtick: `water-wizard.md` → `.agents/roles/art-director/skills/characters/water-wizard.md` (ok)
  - backtick: `Creative/Art/Agents/captain-stretch-art-agent.md` → `Creative/Art/Agents/captain-stretch-art-agent.md` (ok)
  - backtick: `Creative/Art/Agents/the-director-art-agent.md` → `Creative/Art/Agents/the-director-art-agent.md` (ok)
  - backtick: `Creative/Art/Agents/water-wizard-art-agent.md` → `Creative/Art/Agents/water-wizard-art-agent.md` (ok)
  - backtick: `Creative/Brand/CHARACTER-CODEX.md` → `Creative/Brand/CHARACTER-CODEX.md` (ok)
- Inbound refs (1):
  - `.agents/roles/art-director/role.md`

### `.agents/roles/art-director/skills/characters/captain-stretch.md`
- Outbound refs (3):
  - backtick: `captain-stretch-art-agent.md` → `.agents/roles/art-director/skills/characters/captain-stretch-art-agent.md` (BROKEN)
  - backtick: `Creative/Art/Agents/captain-stretch-art-agent.md` → `Creative/Art/Agents/captain-stretch-art-agent.md` (ok)
  - backtick: `Creative/Art/Agents/stream-style-art-agent.md` → `Creative/Art/Agents/stream-style-art-agent.md` (ok)
- Inbound refs (1):
  - `.agents/roles/art-director/skills/characters/_index.md`

### `.agents/roles/art-director/skills/characters/the-director.md`
- Outbound refs (3):
  - backtick: `the-director-art-agent.md` → `.agents/roles/art-director/skills/characters/the-director-art-agent.md` (BROKEN)
  - backtick: `Creative/Art/Agents/stream-style-art-agent.md` → `Creative/Art/Agents/stream-style-art-agent.md` (ok)
  - backtick: `Creative/Art/Agents/the-director-art-agent.md` → `Creative/Art/Agents/the-director-art-agent.md` (ok)
- Inbound refs (1):
  - `.agents/roles/art-director/skills/characters/_index.md`

### `.agents/roles/art-director/skills/characters/water-wizard.md`
- Outbound refs (3):
  - backtick: `water-wizard-art-agent.md` → `.agents/roles/art-director/skills/characters/water-wizard-art-agent.md` (BROKEN)
  - backtick: `Creative/Art/Agents/stream-style-art-agent.md` → `Creative/Art/Agents/stream-style-art-agent.md` (ok)
  - backtick: `Creative/Art/Agents/water-wizard-art-agent.md` → `Creative/Art/Agents/water-wizard-art-agent.md` (ok)
- Inbound refs (1):
  - `.agents/roles/art-director/skills/characters/_index.md`

### `.agents/roles/art-director/skills/core.md`
- Outbound refs (2):
  - backtick: `agents/_shared/project.md` → `.agents/roles/art-director/skills/agents/_shared/project.md` (BROKEN)
  - backtick: `Creative/Art/Agents/stream-style-art-agent.md` → `Creative/Art/Agents/stream-style-art-agent.md` (ok)
- Inbound refs (1):
  - `.agents/roles/art-director/role.md`

### `.agents/roles/art-director/skills/pipeline/_index.md`
- Outbound refs (9):
  - backtick: `Creative/Art/Agents/stream-style-art-agent.md` → `Creative/Art/Agents/stream-style-art-agent.md` (ok)
  - backtick: `README.md` → `README.md` (ok)
  - backtick: `Tools/ArtPipeline/FULL-RUN.md` → `Tools/ArtPipeline/FULL-RUN.md` (ok)
  - backtick: `Tools/ArtPipeline/` → `Tools/ArtPipeline/README.md` (ok)
  - backtick: `Tools/ArtPipeline/README.md` → `Tools/ArtPipeline/README.md` (ok)
  - backtick: `Tools/ArtPipeline/SETUP.md` → `Tools/ArtPipeline/SETUP.md` (ok)
  - backtick: `/characters/_index.md` → `characters/_index.md` (BROKEN)
  - backtick: `/core.md` → `core.md` (BROKEN)
  - backtick: `/stream-style/_index.md` → `stream-style/_index.md` (BROKEN)
- Inbound refs (1):
  - `.agents/roles/art-director/role.md`

### `.agents/roles/art-director/skills/stream-style/_index.md`
- Outbound refs (2):
  - backtick: `art-director/skills/core.md` → `.agents/roles/art-director/skills/stream-style/art-director/skills/core.md` (BROKEN)
  - backtick: `Creative/Art/Agents/stream-style-art-agent.md` → `Creative/Art/Agents/stream-style-art-agent.md` (ok)
- Inbound refs (1):
  - `.agents/roles/art-director/role.md`

### `.agents/roles/brand-steward/role.md`
- Outbound refs (5):
  - backtick: `skills/canon-guardian/_index.md` → `.agents/roles/brand-steward/skills/canon-guardian/_index.md` (ok)
  - backtick: `skills/community-growth/_index.md` → `.agents/roles/brand-steward/skills/community-growth/_index.md` (ok)
  - backtick: `skills/content-strategy/_index.md` → `.agents/roles/brand-steward/skills/content-strategy/_index.md` (ok)
  - backtick: `skills/core.md` → `.agents/roles/brand-steward/skills/core.md` (ok)
  - backtick: `skills/voice/_index.md` → `.agents/roles/brand-steward/skills/voice/_index.md` (ok)
- Inbound refs (1):
  - `.agents/ENTRY.md`

### `.agents/roles/brand-steward/skills/canon-guardian/_index.md`
- Outbound refs (3):
  - backtick: `Creative/Brand/BRAND-IDENTITY.md` → `Creative/Brand/BRAND-IDENTITY.md` (ok)
  - backtick: `Creative/Brand/CHARACTER-CODEX.md` → `Creative/Brand/CHARACTER-CODEX.md` (ok)
  - backtick: `Creative/WorldBuilding/Agents/D&D-Agent.md` → `Creative/WorldBuilding/Agents/D&D-Agent.md` (ok)
- Inbound refs (1):
  - `.agents/roles/brand-steward/role.md`

### `.agents/roles/brand-steward/skills/community-growth/_index.md`
- Outbound refs (1):
  - backtick: `Creative/Brand/BRAND-IDENTITY.md` → `Creative/Brand/BRAND-IDENTITY.md` (ok)
- Inbound refs (1):
  - `.agents/roles/brand-steward/role.md`

### `.agents/roles/brand-steward/skills/content-strategy/_index.md`
- Outbound refs (0):
  - None
- Inbound refs (1):
  - `.agents/roles/brand-steward/role.md`

### `.agents/roles/brand-steward/skills/core.md`
- Outbound refs (4):
  - backtick: `agents/_shared/project.md` → `.agents/roles/brand-steward/skills/agents/_shared/project.md` (BROKEN)
  - backtick: `Creative/Brand/BRAND-IDENTITY.md` → `Creative/Brand/BRAND-IDENTITY.md` (ok)
  - backtick: `Creative/Brand/BRAND-VOICE.md` → `Creative/Brand/BRAND-VOICE.md` (ok)
  - backtick: `Creative/Brand/CHARACTER-CODEX.md` → `Creative/Brand/CHARACTER-CODEX.md` (ok)
- Inbound refs (1):
  - `.agents/roles/brand-steward/role.md`

### `.agents/roles/brand-steward/skills/voice/_index.md`
- Outbound refs (2):
  - backtick: `Creative/Brand/BRAND-IDENTITY.md` → `Creative/Brand/BRAND-IDENTITY.md` (ok)
  - backtick: `Creative/Brand/BRAND-VOICE.md` → `Creative/Brand/BRAND-VOICE.md` (ok)
- Inbound refs (1):
  - `.agents/roles/brand-steward/role.md`

### `.agents/roles/content-repurposer/context/pipeline-dev-notes.md`
- Outbound refs (0):
  - None
- Inbound refs (0):
  - None

### `.agents/roles/content-repurposer/role.md`
- Outbound refs (5):
  - backtick: `skills/clip-strategy/_index.md` → `.agents/roles/content-repurposer/skills/clip-strategy/_index.md` (ok)
  - backtick: `skills/core.md` → `.agents/roles/content-repurposer/skills/core.md` (ok)
  - backtick: `skills/pipeline/_index.md` → `.agents/roles/content-repurposer/skills/pipeline/_index.md` (ok)
  - backtick: `skills/platforms/_index.md` → `.agents/roles/content-repurposer/skills/platforms/_index.md` (ok)
  - backtick: `Tools/ContentPipeline/` → `Tools/ContentPipeline/README.md` (ok)
- Inbound refs (1):
  - `.agents/ENTRY.md`

### `.agents/roles/content-repurposer/skills/clip-strategy/_index.md`
- Outbound refs (1):
  - backtick: `skills/platforms/_index.md` → `.agents/roles/content-repurposer/skills/clip-strategy/skills/platforms/_index.md` (BROKEN)
- Inbound refs (1):
  - `.agents/roles/content-repurposer/role.md`

### `.agents/roles/content-repurposer/skills/core.md`
- Outbound refs (3):
  - backtick: `agents/_shared/project.md` → `.agents/roles/content-repurposer/skills/agents/_shared/project.md` (BROKEN)
  - backtick: `Creative/Brand/BRAND-IDENTITY.md` → `Creative/Brand/BRAND-IDENTITY.md` (ok)
  - backtick: `Creative/Brand/BRAND-VOICE.md` → `Creative/Brand/BRAND-VOICE.md` (ok)
- Inbound refs (1):
  - `.agents/roles/content-repurposer/role.md`

### `.agents/roles/content-repurposer/skills/pipeline/_index.md`
- Outbound refs (8):
  - backtick: `_index.md` → `.agents/roles/content-repurposer/skills/pipeline/_index.md` (ok)
  - backtick: `phase-map.md` → `.agents/roles/content-repurposer/skills/pipeline/phase-map.md` (ok)
  - backtick: `README.md` → `README.md` (ok)
  - backtick: `Tools/ContentPipeline/` → `Tools/ContentPipeline/README.md` (ok)
  - backtick: `Tools/ContentPipeline/README.md` → `Tools/ContentPipeline/README.md` (ok)
  - backtick: `/clip-strategy/_index.md` → `clip-strategy/_index.md` (BROKEN)
  - backtick: `/core.md` → `core.md` (BROKEN)
  - backtick: `/platforms/_index.md` → `platforms/_index.md` (BROKEN)
- Inbound refs (1):
  - `.agents/roles/content-repurposer/role.md`

### `.agents/roles/content-repurposer/skills/pipeline/phase-map.md`
- Outbound refs (1):
  - backtick: `Tools/ContentPipeline/` → `Tools/ContentPipeline/README.md` (ok)
- Inbound refs (1):
  - `.agents/roles/content-repurposer/skills/pipeline/_index.md`

### `.agents/roles/content-repurposer/skills/platforms/_index.md`
- Outbound refs (0):
  - None
- Inbound refs (1):
  - `.agents/roles/content-repurposer/role.md`

### `.agents/roles/lotat-tech/role.md`
- Outbound refs (9):
  - backtick: `ops/skills/change-summary/_index.md` → `.agents/roles/lotat-tech/ops/skills/change-summary/_index.md` (BROKEN)
  - backtick: `skills/core.md` → `.agents/roles/lotat-tech/skills/core.md` (ok)
  - backtick: `skills/engine/_index.md` → `.agents/roles/lotat-tech/skills/engine/_index.md` (ok)
  - backtick: `skills/engine/commands.md` → `.agents/roles/lotat-tech/skills/engine/commands.md` (ok)
  - backtick: `skills/engine/docs-map.md` → `.agents/roles/lotat-tech/skills/engine/docs-map.md` (ok)
  - backtick: `skills/engine/session-lifecycle.md` → `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md` (ok)
  - backtick: `skills/engine/state-and-voting.md` → `.agents/roles/lotat-tech/skills/engine/state-and-voting.md` (ok)
  - backtick: `skills/story-pipeline/_index.md` → `.agents/roles/lotat-tech/skills/story-pipeline/_index.md` (ok)
  - backtick: `skills/story-pipeline/json-schema.md` → `.agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` (ok)
- Inbound refs (1):
  - `.agents/ENTRY.md`

### `.agents/roles/lotat-tech/skills/core.md`
- Outbound refs (9):
  - backtick: `agents/_shared/project.md` → `.agents/roles/lotat-tech/skills/agents/_shared/project.md` (BROKEN)
  - backtick: `agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` → `.agents/roles/lotat-tech/skills/agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` (BROKEN)
  - backtick: `skills/engine/docs-map.md` → `.agents/roles/lotat-tech/skills/skills/engine/docs-map.md` (BROKEN)
  - backtick: `skills/engine/session-lifecycle.md` → `.agents/roles/lotat-tech/skills/skills/engine/session-lifecycle.md` (BROKEN)
  - backtick: `skills/engine/state-and-voting.md` → `.agents/roles/lotat-tech/skills/skills/engine/state-and-voting.md` (BROKEN)
  - backtick: `Actions/HELPER-SNIPPETS.md` → `Actions/HELPER-SNIPPETS.md` (ok)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` → `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` (ok)
  - backtick: `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` → `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` (ok)
- Inbound refs (1):
  - `.agents/roles/lotat-tech/role.md`

### `.agents/roles/lotat-tech/skills/engine/_index.md`
- Outbound refs (8):
  - backtick: `agents/roles/app-dev/skills/stream-interactions/lotat-rendering.md` → `.agents/roles/lotat-tech/skills/engine/agents/roles/app-dev/skills/stream-interactions/lotat-rendering.md` (BROKEN)
  - backtick: `commands.md` → `.agents/roles/lotat-tech/skills/engine/commands.md` (ok)
  - backtick: `docs-map.md` → `.agents/roles/lotat-tech/skills/engine/docs-map.md` (ok)
  - backtick: `session-lifecycle.md` → `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md` (ok)
  - backtick: `state-and-voting.md` → `.agents/roles/lotat-tech/skills/engine/state-and-voting.md` (ok)
  - backtick: `Actions/LotAT/` → `Actions/LotAT/README.md` (ok)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Apps/stream-overlay/` → `Apps/stream-overlay/README.md` (ok)
- Inbound refs (2):
  - `.agents/roles/lotat-tech/role.md`
  - `.agents/roles/lotat-tech/skills/engine/docs-map.md`

### `.agents/roles/lotat-tech/skills/engine/commands.md`
- Outbound refs (2):
  - backtick: `Actions/LotAT/` → `Actions/LotAT/README.md` (ok)
  - backtick: `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` → `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` (ok)
- Inbound refs (5):
  - `.agents/roles/lotat-tech/role.md`
  - `.agents/roles/lotat-tech/skills/engine/_index.md`
  - `.agents/roles/lotat-tech/skills/engine/docs-map.md`
  - `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md`
  - `.agents/roles/lotat-tech/skills/engine/state-and-voting.md`

### `.agents/roles/lotat-tech/skills/engine/docs-map.md`
- Outbound refs (8):
  - backtick: `_index.md` → `.agents/roles/lotat-tech/skills/engine/_index.md` (ok)
  - backtick: `agents/roles/streamerbot-dev/skills/lotat/_index.md` → `.agents/roles/lotat-tech/skills/engine/agents/roles/streamerbot-dev/skills/lotat/_index.md` (BROKEN)
  - backtick: `commands.md` → `.agents/roles/lotat-tech/skills/engine/commands.md` (ok)
  - backtick: `docs-map.md` → `.agents/roles/lotat-tech/skills/engine/docs-map.md` (ok)
  - backtick: `session-lifecycle.md` → `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md` (ok)
  - backtick: `state-and-voting.md` → `.agents/roles/lotat-tech/skills/engine/state-and-voting.md` (ok)
  - backtick: `/core.md` → `core.md` (BROKEN)
  - backtick: `/story-pipeline/json-schema.md` → `story-pipeline/json-schema.md` (BROKEN)
- Inbound refs (3):
  - `.agents/roles/lotat-tech/role.md`
  - `.agents/roles/lotat-tech/skills/engine/_index.md`
  - `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md`

### `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md`
- Outbound refs (6):
  - backtick: `commands.md` → `.agents/roles/lotat-tech/skills/engine/commands.md` (ok)
  - backtick: `docs-map.md` → `.agents/roles/lotat-tech/skills/engine/docs-map.md` (ok)
  - backtick: `state-and-voting.md` → `.agents/roles/lotat-tech/skills/engine/state-and-voting.md` (ok)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` → `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` (ok)
  - backtick: `/story-pipeline/json-schema.md` → `story-pipeline/json-schema.md` (BROKEN)
- Inbound refs (4):
  - `.agents/roles/lotat-tech/role.md`
  - `.agents/roles/lotat-tech/skills/engine/_index.md`
  - `.agents/roles/lotat-tech/skills/engine/docs-map.md`
  - `.agents/roles/lotat-tech/skills/engine/state-and-voting.md`

### `.agents/roles/lotat-tech/skills/engine/state-and-voting.md`
- Outbound refs (5):
  - backtick: `commands.md` → `.agents/roles/lotat-tech/skills/engine/commands.md` (ok)
  - backtick: `session-lifecycle.md` → `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md` (ok)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` → `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` (ok)
  - backtick: `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` → `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` (ok)
- Inbound refs (4):
  - `.agents/roles/lotat-tech/role.md`
  - `.agents/roles/lotat-tech/skills/engine/_index.md`
  - `.agents/roles/lotat-tech/skills/engine/docs-map.md`
  - `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md`

### `.agents/roles/lotat-tech/skills/story-pipeline/_index.md`
- Outbound refs (6):
  - backtick: `agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` → `.agents/roles/lotat-tech/skills/story-pipeline/agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` (BROKEN)
  - backtick: `engine/commands.md` → `.agents/roles/lotat-tech/skills/story-pipeline/engine/commands.md` (BROKEN)
  - backtick: `json-schema.md` → `.agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` (ok)
  - backtick: `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` → `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` (ok)
  - backtick: `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` → `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` (ok)
  - backtick: `Creative/WorldBuilding/Franchises/StarshipShamples.md` → `Creative/WorldBuilding/Franchises/StarshipShamples.md` (ok)
- Inbound refs (1):
  - `.agents/roles/lotat-tech/role.md`

### `.agents/roles/lotat-tech/skills/story-pipeline/json-schema.md`
- Outbound refs (2):
  - backtick: `agents/roles/lotat-tech/skills/engine/commands.md` → `.agents/roles/lotat-tech/skills/story-pipeline/agents/roles/lotat-tech/skills/engine/commands.md` (BROKEN)
  - backtick: `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` → `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` (ok)
- Inbound refs (2):
  - `.agents/roles/lotat-tech/role.md`
  - `.agents/roles/lotat-tech/skills/story-pipeline/_index.md`

### `.agents/roles/lotat-writer/role.md`
- Outbound refs (9):
  - backtick: `skills/adventures/_index.md` → `.agents/roles/lotat-writer/skills/adventures/_index.md` (ok)
  - backtick: `skills/adventures/mechanics.md` → `.agents/roles/lotat-writer/skills/adventures/mechanics.md` (ok)
  - backtick: `skills/adventures/session-format.md` → `.agents/roles/lotat-writer/skills/adventures/session-format.md` (ok)
  - backtick: `skills/canon-guardian/_index.md` → `.agents/roles/lotat-writer/skills/canon-guardian/_index.md` (ok)
  - backtick: `skills/core.md` → `.agents/roles/lotat-writer/skills/core.md` (ok)
  - backtick: `skills/franchises/starship-shamples.md` → `.agents/roles/lotat-writer/skills/franchises/starship-shamples.md` (ok)
  - backtick: `skills/universe/_index.md` → `.agents/roles/lotat-writer/skills/universe/_index.md` (ok)
  - backtick: `skills/universe/cast.md` → `.agents/roles/lotat-writer/skills/universe/cast.md` (ok)
  - backtick: `skills/universe/rules.md` → `.agents/roles/lotat-writer/skills/universe/rules.md` (ok)
- Inbound refs (1):
  - `.agents/ENTRY.md`

### `.agents/roles/lotat-writer/skills/adventures/_index.md`
- Outbound refs (4):
  - backtick: `mechanics.md` → `.agents/roles/lotat-writer/skills/adventures/mechanics.md` (ok)
  - backtick: `session-format.md` → `.agents/roles/lotat-writer/skills/adventures/session-format.md` (ok)
  - backtick: `Creative/WorldBuilding/Agents/D&D-Agent.md` → `Creative/WorldBuilding/Agents/D&D-Agent.md` (ok)
  - backtick: `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` → `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` (ok)
- Inbound refs (1):
  - `.agents/roles/lotat-writer/role.md`

### `.agents/roles/lotat-writer/skills/adventures/mechanics.md`
- Outbound refs (1):
  - backtick: `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` → `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` (ok)
- Inbound refs (2):
  - `.agents/roles/lotat-writer/role.md`
  - `.agents/roles/lotat-writer/skills/adventures/_index.md`

### `.agents/roles/lotat-writer/skills/adventures/session-format.md`
- Outbound refs (0):
  - None
- Inbound refs (2):
  - `.agents/roles/lotat-writer/role.md`
  - `.agents/roles/lotat-writer/skills/adventures/_index.md`

### `.agents/roles/lotat-writer/skills/canon-guardian/_index.md`
- Outbound refs (6):
  - backtick: `pi/skills/brand-steward-canon-guardian/SKILL.md` → `.agents/roles/lotat-writer/skills/canon-guardian/pi/skills/brand-steward-canon-guardian/SKILL.md` (BROKEN)
  - backtick: `Creative/Brand/BRAND-IDENTITY.md` → `Creative/Brand/BRAND-IDENTITY.md` (ok)
  - backtick: `Creative/Brand/CHARACTER-CODEX.md` → `Creative/Brand/CHARACTER-CODEX.md` (ok)
  - backtick: `Creative/WorldBuilding/Agents/D&D-Agent.md` → `Creative/WorldBuilding/Agents/D&D-Agent.md` (ok)
  - backtick: `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` → `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` (ok)
  - backtick: `Creative/WorldBuilding/Franchises/StarshipShamples.md` → `Creative/WorldBuilding/Franchises/StarshipShamples.md` (ok)
- Inbound refs (1):
  - `.agents/roles/lotat-writer/role.md`

### `.agents/roles/lotat-writer/skills/core.md`
- Outbound refs (6):
  - backtick: `agents/_shared/project.md` → `.agents/roles/lotat-writer/skills/agents/_shared/project.md` (BROKEN)
  - backtick: `lotat-tech/skills/engine/commands.md` → `.agents/roles/lotat-writer/skills/lotat-tech/skills/engine/commands.md` (BROKEN)
  - backtick: `Creative/Brand/BRAND-IDENTITY.md` → `Creative/Brand/BRAND-IDENTITY.md` (ok)
  - backtick: `Creative/Brand/CHARACTER-CODEX.md` → `Creative/Brand/CHARACTER-CODEX.md` (ok)
  - backtick: `Creative/WorldBuilding/Agents/D&D-Agent.md` → `Creative/WorldBuilding/Agents/D&D-Agent.md` (ok)
  - backtick: `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` → `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` (ok)
- Inbound refs (1):
  - `.agents/roles/lotat-writer/role.md`

### `.agents/roles/lotat-writer/skills/franchises/_index.md`
- Outbound refs (1):
  - backtick: `starship-shamples.md` → `.agents/roles/lotat-writer/skills/franchises/starship-shamples.md` (ok)
- Inbound refs (0):
  - None

### `.agents/roles/lotat-writer/skills/franchises/starship-shamples.md`
- Outbound refs (6):
  - backtick: `Creative/Brand/BRAND-IDENTITY.md` → `Creative/Brand/BRAND-IDENTITY.md` (ok)
  - backtick: `Creative/Brand/CHARACTER-CODEX.md` → `Creative/Brand/CHARACTER-CODEX.md` (ok)
  - backtick: `Creative/WorldBuilding/Agents/D&D-Agent.md` → `Creative/WorldBuilding/Agents/D&D-Agent.md` (ok)
  - backtick: `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` → `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` (ok)
  - backtick: `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` → `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` (ok)
  - backtick: `Creative/WorldBuilding/Franchises/StarshipShamples.md` → `Creative/WorldBuilding/Franchises/StarshipShamples.md` (ok)
- Inbound refs (2):
  - `.agents/roles/lotat-writer/role.md`
  - `.agents/roles/lotat-writer/skills/franchises/_index.md`

### `.agents/roles/lotat-writer/skills/universe/_index.md`
- Outbound refs (4):
  - backtick: `cast.md` → `.agents/roles/lotat-writer/skills/universe/cast.md` (ok)
  - backtick: `rules.md` → `.agents/roles/lotat-writer/skills/universe/rules.md` (ok)
  - backtick: `Creative/WorldBuilding/Agents/D&D-Agent.md` → `Creative/WorldBuilding/Agents/D&D-Agent.md` (ok)
  - backtick: `Creative/WorldBuilding/Franchises/StarshipShamples.md` → `Creative/WorldBuilding/Franchises/StarshipShamples.md` (ok)
- Inbound refs (1):
  - `.agents/roles/lotat-writer/role.md`

### `.agents/roles/lotat-writer/skills/universe/cast.md`
- Outbound refs (1):
  - backtick: `Creative/Brand/CHARACTER-CODEX.md` → `Creative/Brand/CHARACTER-CODEX.md` (ok)
- Inbound refs (2):
  - `.agents/roles/lotat-writer/role.md`
  - `.agents/roles/lotat-writer/skills/universe/_index.md`

### `.agents/roles/lotat-writer/skills/universe/rules.md`
- Outbound refs (2):
  - backtick: `Creative/WorldBuilding/Agents/D&D-Agent.md` → `Creative/WorldBuilding/Agents/D&D-Agent.md` (ok)
  - backtick: `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` → `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` (ok)
- Inbound refs (2):
  - `.agents/roles/lotat-writer/role.md`
  - `.agents/roles/lotat-writer/skills/universe/_index.md`

### `.agents/roles/ops/role.md`
- Outbound refs (5):
  - backtick: `skills/change-summary/_index.md` → `.agents/roles/ops/skills/change-summary/_index.md` (ok)
  - backtick: `skills/core.md` → `.agents/roles/ops/skills/core.md` (ok)
  - backtick: `skills/sync/_index.md` → `.agents/roles/ops/skills/sync/_index.md` (ok)
  - backtick: `skills/validation/_index.md` → `.agents/roles/ops/skills/validation/_index.md` (ok)
  - backtick: `Tools/` → `Tools/README.md` (ok)
- Inbound refs (1):
  - `.agents/ENTRY.md`

### `.agents/roles/ops/skills/change-summary/_index.md`
- Outbound refs (3):
  - backtick: `ops/skills/sync/_index.md` → `.agents/roles/ops/skills/change-summary/ops/skills/sync/_index.md` (BROKEN)
  - backtick: `Creative/` → `Creative/README.md` (ok)
  - backtick: `Tools/` → `Tools/README.md` (ok)
- Inbound refs (1):
  - `.agents/roles/ops/role.md`

### `.agents/roles/ops/skills/core.md`
- Outbound refs (6):
  - backtick: `agents/ENTRY.md` → `.agents/roles/ops/skills/agents/ENTRY.md` (BROKEN)
  - backtick: `agents/_shared/project.md` → `.agents/roles/ops/skills/agents/_shared/project.md` (BROKEN)
  - backtick: `pi/skills/README.md` → `.agents/roles/ops/skills/pi/skills/README.md` (BROKEN)
  - backtick: `AGENTS.md` → `AGENTS.md` (ok)
  - backtick: `Creative/` → `Creative/README.md` (ok)
  - backtick: `Tools/` → `Tools/README.md` (ok)
- Inbound refs (1):
  - `.agents/roles/ops/role.md`

### `.agents/roles/ops/skills/sync/_index.md`
- Outbound refs (2):
  - backtick: `Creative/` → `Creative/README.md` (ok)
  - backtick: `Tools/` → `Tools/README.md` (ok)
- Inbound refs (1):
  - `.agents/roles/ops/role.md`

### `.agents/roles/ops/skills/validation/_index.md`
- Outbound refs (4):
  - backtick: `agents/ENTRY.md` → `.agents/roles/ops/skills/validation/agents/ENTRY.md` (BROKEN)
  - backtick: `pi/skills/README.md` → `.agents/roles/ops/skills/validation/pi/skills/README.md` (BROKEN)
  - backtick: `AGENTS.md` → `AGENTS.md` (ok)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
- Inbound refs (1):
  - `.agents/roles/ops/role.md`

### `.agents/roles/product-dev/role.md`
- Outbound refs (3):
  - backtick: `agents/roles/ops/skills/change-summary/_index.md` → `.agents/roles/product-dev/agents/roles/ops/skills/change-summary/_index.md` (BROKEN)
  - backtick: `skills/core.md` → `.agents/roles/product-dev/skills/core.md` (ok)
  - backtick: `Docs/` → `Docs` (ok)
- Inbound refs (1):
  - `.agents/ENTRY.md`

### `.agents/roles/product-dev/skills/core.md`
- Outbound refs (3):
  - backtick: `agents/_shared/project.md` → `.agents/roles/product-dev/skills/agents/_shared/project.md` (BROKEN)
  - backtick: `Creative/Brand/BRAND-IDENTITY.md` → `Creative/Brand/BRAND-IDENTITY.md` (ok)
  - backtick: `Creative/Brand/BRAND-VOICE.md` → `Creative/Brand/BRAND-VOICE.md` (ok)
- Inbound refs (1):
  - `.agents/roles/product-dev/role.md`

### `.agents/roles/streamerbot-dev/context/patterns.md`
- Outbound refs (1):
  - backtick: `Actions/HELPER-SNIPPETS.md` → `Actions/HELPER-SNIPPETS.md` (ok)
- Inbound refs (0):
  - None

### `.agents/roles/streamerbot-dev/role.md`
- Outbound refs (2):
  - backtick: `ops/skills/change-summary/_index.md` → `.agents/roles/streamerbot-dev/ops/skills/change-summary/_index.md` (BROKEN)
  - backtick: `skills/core.md` → `.agents/roles/streamerbot-dev/skills/core.md` (ok)
- Inbound refs (1):
  - `.agents/ENTRY.md`

### `.agents/roles/streamerbot-dev/skills/commanders/_index.md`
- Outbound refs (8):
  - backtick: `captain-stretch.md` → `.agents/roles/streamerbot-dev/skills/commanders/captain-stretch.md` (ok)
  - backtick: `the-director.md` → `.agents/roles/streamerbot-dev/skills/commanders/the-director.md` (ok)
  - backtick: `water-wizard.md` → `.agents/roles/streamerbot-dev/skills/commanders/water-wizard.md` (ok)
  - backtick: `Actions/Commanders/Captain Stretch/README.md` → `Actions/Commanders/Captain` (BROKEN)
  - backtick: `Actions/Commanders/README.md` → `Actions/Commanders/README.md` (ok)
  - backtick: `Actions/Commanders/The Director/README.md` → `Actions/Commanders/The` (BROKEN)
  - backtick: `Actions/Commanders/Water Wizard/README.md` → `Actions/Commanders/Water` (BROKEN)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
- Inbound refs (0):
  - None

### `.agents/roles/streamerbot-dev/skills/commanders/captain-stretch.md`
- Outbound refs (1):
  - backtick: `Actions/Commanders/Captain Stretch/README.md` → `Actions/Commanders/Captain` (BROKEN)
- Inbound refs (1):
  - `.agents/roles/streamerbot-dev/skills/commanders/_index.md`

### `.agents/roles/streamerbot-dev/skills/commanders/the-director.md`
- Outbound refs (1):
  - backtick: `Actions/Commanders/The Director/README.md` → `Actions/Commanders/The` (BROKEN)
- Inbound refs (1):
  - `.agents/roles/streamerbot-dev/skills/commanders/_index.md`

### `.agents/roles/streamerbot-dev/skills/commanders/water-wizard.md`
- Outbound refs (1):
  - backtick: `Actions/Commanders/Water Wizard/README.md` → `Actions/Commanders/Water` (BROKEN)
- Inbound refs (1):
  - `.agents/roles/streamerbot-dev/skills/commanders/_index.md`

### `.agents/roles/streamerbot-dev/skills/core.md`
- Outbound refs (3):
  - backtick: `agents/_shared/project.md` → `.agents/roles/streamerbot-dev/skills/agents/_shared/project.md` (BROKEN)
  - backtick: `Actions/HELPER-SNIPPETS.md` → `Actions/HELPER-SNIPPETS.md` (ok)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
- Inbound refs (1):
  - `.agents/roles/streamerbot-dev/role.md`

### `.agents/roles/streamerbot-dev/skills/lotat/_index.md`
- Outbound refs (6):
  - backtick: `agents/roles/lotat-tech/skills/engine/commands.md` → `.agents/roles/streamerbot-dev/skills/lotat/agents/roles/lotat-tech/skills/engine/commands.md` (BROKEN)
  - backtick: `agents/roles/lotat-tech/skills/engine/docs-map.md` → `.agents/roles/streamerbot-dev/skills/lotat/agents/roles/lotat-tech/skills/engine/docs-map.md` (BROKEN)
  - backtick: `agents/roles/lotat-tech/skills/engine/session-lifecycle.md` → `.agents/roles/streamerbot-dev/skills/lotat/agents/roles/lotat-tech/skills/engine/session-lifecycle.md` (BROKEN)
  - backtick: `agents/roles/lotat-tech/skills/engine/state-and-voting.md` → `.agents/roles/streamerbot-dev/skills/lotat/agents/roles/lotat-tech/skills/engine/state-and-voting.md` (BROKEN)
  - backtick: `Actions/LotAT/` → `Actions/LotAT/README.md` (ok)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
- Inbound refs (0):
  - None

### `.agents/roles/streamerbot-dev/skills/overlay-integration.md`
- Outbound refs (8):
  - backtick: `agents/roles/app-dev/skills/stream-interactions/broker.md` → `.agents/roles/streamerbot-dev/skills/agents/roles/app-dev/skills/stream-interactions/broker.md` (BROKEN)
  - backtick: `agents/roles/app-dev/skills/stream-interactions/protocol.md` → `.agents/roles/streamerbot-dev/skills/agents/roles/app-dev/skills/stream-interactions/protocol.md` (BROKEN)
  - backtick: `protocol.md` → `.agents/roles/streamerbot-dev/skills/protocol.md` (BROKEN)
  - backtick: `Actions/HELPER-SNIPPETS.md § 7` → `Actions/HELPER-SNIPPETS.md` (ok)
  - backtick: `Actions/Overlay/README.md` → `Actions/Overlay/README.md` (ok)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Actions/SHARED-CONSTANTS.md → Overlay / Broker` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Apps/stream-overlay/` → `Apps/stream-overlay/README.md` (ok)
- Inbound refs (0):
  - None

### `.agents/roles/streamerbot-dev/skills/squad/_index.md`
- Outbound refs (17):
  - backtick: `agents/roles/app-dev/skills/stream-interactions/squad-rendering.md` → `.agents/roles/streamerbot-dev/skills/squad/agents/roles/app-dev/skills/stream-interactions/squad-rendering.md` (BROKEN)
  - backtick: `clone.md` → `.agents/roles/streamerbot-dev/skills/squad/clone.md` (ok)
  - backtick: `duck.md` → `.agents/roles/streamerbot-dev/skills/squad/duck.md` (ok)
  - backtick: `pedro.md` → `.agents/roles/streamerbot-dev/skills/squad/pedro.md` (ok)
  - backtick: `streamerbot-dev/skills/core.md` → `.agents/roles/streamerbot-dev/skills/squad/streamerbot-dev/skills/core.md` (BROKEN)
  - backtick: `toothless.md` → `.agents/roles/streamerbot-dev/skills/squad/toothless.md` (ok)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Actions/Squad/Clone/` → `Actions/Squad/Clone/README.md` (ok)
  - backtick: `Actions/Squad/Clone/README.md` → `Actions/Squad/Clone/README.md` (ok)
  - backtick: `Actions/Squad/Duck/` → `Actions/Squad/Duck/README.md` (ok)
  - backtick: `Actions/Squad/Duck/README.md` → `Actions/Squad/Duck/README.md` (ok)
  - backtick: `Actions/Squad/Pedro/` → `Actions/Squad/Pedro/README.md` (ok)
  - backtick: `Actions/Squad/Pedro/README.md` → `Actions/Squad/Pedro/README.md` (ok)
  - backtick: `Actions/Squad/` → `Actions/Squad/README.md` (ok)
  - backtick: `Actions/Squad/README.md` → `Actions/Squad/README.md` (ok)
  - backtick: `Actions/Squad/Toothless/` → `Actions/Squad/Toothless/README.md` (ok)
  - backtick: `Actions/Squad/Toothless/README.md` → `Actions/Squad/Toothless/README.md` (ok)
- Inbound refs (0):
  - None

### `.agents/roles/streamerbot-dev/skills/squad/clone.md`
- Outbound refs (5):
  - backtick: `Clone - Game Tick` → `.agents/roles/streamerbot-dev/skills/squad/Clone.md` (ok)
  - backtick: `Clone - Join Window` → `.agents/roles/streamerbot-dev/skills/squad/Clone.md` (ok)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Actions/Squad/Clone/` → `Actions/Squad/Clone/README.md` (ok)
  - backtick: `Actions/Squad/Clone/README.md` → `Actions/Squad/Clone/README.md` (ok)
- Inbound refs (1):
  - `.agents/roles/streamerbot-dev/skills/squad/_index.md`

### `.agents/roles/streamerbot-dev/skills/squad/duck.md`
- Outbound refs (3):
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Actions/Squad/Duck/` → `Actions/Squad/Duck/README.md` (ok)
  - backtick: `Actions/Squad/Duck/README.md` → `Actions/Squad/Duck/README.md` (ok)
- Inbound refs (1):
  - `.agents/roles/streamerbot-dev/skills/squad/_index.md`

### `.agents/roles/streamerbot-dev/skills/squad/pedro.md`
- Outbound refs (3):
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Actions/Squad/Pedro/` → `Actions/Squad/Pedro/README.md` (ok)
  - backtick: `Actions/Squad/Pedro/README.md` → `Actions/Squad/Pedro/README.md` (ok)
- Inbound refs (1):
  - `.agents/roles/streamerbot-dev/skills/squad/_index.md`

### `.agents/roles/streamerbot-dev/skills/squad/toothless.md`
- Outbound refs (3):
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Actions/Squad/Toothless/` → `Actions/Squad/Toothless/README.md` (ok)
  - backtick: `Actions/Squad/Toothless/README.md` → `Actions/Squad/Toothless/README.md` (ok)
- Inbound refs (1):
  - `.agents/roles/streamerbot-dev/skills/squad/_index.md`

### `.agents/roles/streamerbot-dev/skills/twitch/_index.md`
- Outbound refs (7):
  - backtick: `bits.md` → `.agents/roles/streamerbot-dev/skills/twitch/bits.md` (ok)
  - backtick: `channel-points.md` → `.agents/roles/streamerbot-dev/skills/twitch/channel-points.md` (ok)
  - backtick: `core-events.md` → `.agents/roles/streamerbot-dev/skills/twitch/core-events.md` (ok)
  - backtick: `hype-train.md` → `.agents/roles/streamerbot-dev/skills/twitch/hype-train.md` (ok)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Actions/Twitch Bits Integrations/README.md` → `Actions/Twitch` (BROKEN)
  - backtick: `Actions/Twitch Core Integrations/README.md` → `Actions/Twitch` (BROKEN)
- Inbound refs (0):
  - None

### `.agents/roles/streamerbot-dev/skills/twitch/bits.md`
- Outbound refs (2):
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Actions/Twitch Bits Integrations/README.md` → `Actions/Twitch` (BROKEN)
- Inbound refs (1):
  - `.agents/roles/streamerbot-dev/skills/twitch/_index.md`

### `.agents/roles/streamerbot-dev/skills/twitch/channel-points.md`
- Outbound refs (1):
  - backtick: `Actions/Twitch Channel Points/README.md` → `Actions/Twitch` (BROKEN)
- Inbound refs (1):
  - `.agents/roles/streamerbot-dev/skills/twitch/_index.md`

### `.agents/roles/streamerbot-dev/skills/twitch/core-events.md`
- Outbound refs (1):
  - backtick: `Actions/Twitch Core Integrations/README.md` → `Actions/Twitch` (BROKEN)
- Inbound refs (1):
  - `.agents/roles/streamerbot-dev/skills/twitch/_index.md`

### `.agents/roles/streamerbot-dev/skills/twitch/hype-train.md`
- Outbound refs (1):
  - backtick: `Actions/Twitch Hype Train/README.md` → `Actions/Twitch` (BROKEN)
- Inbound refs (1):
  - `.agents/roles/streamerbot-dev/skills/twitch/_index.md`

### `.agents/roles/streamerbot-dev/skills/voice-commands/_index.md`
- Outbound refs (2):
  - backtick: `streamerbot-dev/skills/core.md` → `.agents/roles/streamerbot-dev/skills/voice-commands/streamerbot-dev/skills/core.md` (BROKEN)
  - backtick: `Actions/Voice Commands/README.md` → `Actions/Voice` (BROKEN)
- Inbound refs (0):
  - None

### `retired Pi skill mirror/README.md`
- Outbound refs (60):
  - backtick: `agents/ENTRY.md` → `retired Pi skill mirror/agents/ENTRY.md` (BROKEN)
  - backtick: `app-dev` → `retired Pi skill mirror/app-dev/SKILL.md` (ok)
  - backtick: `app-dev/SKILL.md` → `retired Pi skill mirror/app-dev/SKILL.md` (ok)
  - backtick: `art-director` → `retired Pi skill mirror/art-director/SKILL.md` (ok)
  - backtick: `art-director/SKILL.md` → `retired Pi skill mirror/art-director/SKILL.md` (ok)
  - backtick: `brand-canon-guardian` → `retired Pi skill mirror/brand-canon-guardian/SKILL.md` (ok)
  - backtick: `brand-steward-canon-guardian` → `retired Pi skill mirror/brand-steward-canon-guardian/SKILL.md` (ok)
  - backtick: `brand-steward-canon-guardian/SKILL.md` → `retired Pi skill mirror/brand-steward-canon-guardian/SKILL.md` (ok)
  - backtick: `brand-steward-content-strategy` → `retired Pi skill mirror/brand-steward-content-strategy/SKILL.md` (ok)
  - backtick: `brand-steward-content-strategy/SKILL.md` → `retired Pi skill mirror/brand-steward-content-strategy/SKILL.md` (ok)
  - backtick: `brand-steward` → `retired Pi skill mirror/brand-steward/SKILL.md` (ok)
  - backtick: `brand-steward/SKILL.md` → `retired Pi skill mirror/brand-steward/SKILL.md` (ok)
  - backtick: `buildtools` → `retired Pi skill mirror/buildtools/SKILL.md` (ok)
  - backtick: `change-summary` → `retired Pi skill mirror/change-summary/SKILL.md` (ok)
  - backtick: `content-repurposer-pipeline/SKILL.md` → `retired Pi skill mirror/content-repurposer-pipeline/SKILL.md` (ok)
  - backtick: `content-repurposer` → `retired Pi skill mirror/content-repurposer/SKILL.md` (ok)
  - backtick: `content-repurposer/SKILL.md` → `retired Pi skill mirror/content-repurposer/SKILL.md` (ok)
  - backtick: `content-strategy` → `retired Pi skill mirror/content-strategy/SKILL.md` (ok)
  - backtick: `creative-art` → `retired Pi skill mirror/creative-art/SKILL.md` (ok)
  - backtick: `creative-worldbuilding` → `retired Pi skill mirror/creative-worldbuilding/SKILL.md` (ok)
  - backtick: `feature-channel-points` → `retired Pi skill mirror/feature-channel-points/SKILL.md` (ok)
  - backtick: `feature-commanders` → `retired Pi skill mirror/feature-commanders/SKILL.md` (ok)
  - backtick: `feature-hype-train` → `retired Pi skill mirror/feature-hype-train/SKILL.md` (ok)
  - backtick: `feature-squad` → `retired Pi skill mirror/feature-squad/SKILL.md` (ok)
  - backtick: `feature-twitch-integration` → `retired Pi skill mirror/feature-twitch-integration/SKILL.md` (ok)
  - backtick: `feature-voice-commands` → `retired Pi skill mirror/feature-voice-commands/SKILL.md` (ok)
  - backtick: `lotat-tech` → `retired Pi skill mirror/lotat-tech/SKILL.md` (ok)
  - backtick: `lotat-tech/SKILL.md` → `retired Pi skill mirror/lotat-tech/SKILL.md` (ok)
  - backtick: `lotat-writer-canon-guardian/SKILL.md` → `retired Pi skill mirror/lotat-writer-canon-guardian/SKILL.md` (ok)
  - backtick: `lotat-writer` → `retired Pi skill mirror/lotat-writer/SKILL.md` (ok)
  - backtick: `lotat-writer/SKILL.md` → `retired Pi skill mirror/lotat-writer/SKILL.md` (ok)
  - backtick: `meta-agents-navigate/SKILL.md` → `retired Pi skill mirror/meta-agents-navigate/SKILL.md` (ok)
  - backtick: `meta-agents-update/SKILL.md` → `retired Pi skill mirror/meta-agents-update/SKILL.md` (ok)
  - backtick: `meta/SKILL.md` → `retired Pi skill mirror/meta/SKILL.md` (ok)
  - backtick: `ops-change-summary` → `retired Pi skill mirror/ops-change-summary/SKILL.md` (ok)
  - backtick: `ops-change-summary/SKILL.md` → `retired Pi skill mirror/ops-change-summary/SKILL.md` (ok)
  - backtick: `ops-sync` → `retired Pi skill mirror/ops-sync/SKILL.md` (ok)
  - backtick: `ops-sync/SKILL.md` → `retired Pi skill mirror/ops-sync/SKILL.md` (ok)
  - backtick: `ops-validation/SKILL.md` → `retired Pi skill mirror/ops-validation/SKILL.md` (ok)
  - backtick: `ops` → `retired Pi skill mirror/ops/SKILL.md` (ok)
  - backtick: `ops/SKILL.md` → `retired Pi skill mirror/ops/SKILL.md` (ok)
  - backtick: `product-dev` → `retired Pi skill mirror/product-dev/SKILL.md` (ok)
  - backtick: `product-dev/SKILL.md` → `retired Pi skill mirror/product-dev/SKILL.md` (ok)
  - backtick: `role.md` → `retired Pi skill mirror/role.md` (BROKEN)
  - backtick: `skills/core.md` → `retired Pi skill mirror/skills/core.md` (BROKEN)
  - backtick: `streamerbot-dev-commanders` → `retired Pi skill mirror/streamerbot-dev-commanders/SKILL.md` (ok)
  - backtick: `streamerbot-dev-commanders/SKILL.md` → `retired Pi skill mirror/streamerbot-dev-commanders/SKILL.md` (ok)
  - backtick: `streamerbot-dev-lotat/SKILL.md` → `retired Pi skill mirror/streamerbot-dev-lotat/SKILL.md` (ok)
  - backtick: `streamerbot-dev-squad` → `retired Pi skill mirror/streamerbot-dev-squad/SKILL.md` (ok)
  - backtick: `streamerbot-dev-squad/SKILL.md` → `retired Pi skill mirror/streamerbot-dev-squad/SKILL.md` (ok)
  - backtick: `streamerbot-dev-twitch` → `retired Pi skill mirror/streamerbot-dev-twitch/SKILL.md` (ok)
  - backtick: `streamerbot-dev-twitch/SKILL.md` → `retired Pi skill mirror/streamerbot-dev-twitch/SKILL.md` (ok)
  - backtick: `streamerbot-dev-voice-commands` → `retired Pi skill mirror/streamerbot-dev-voice-commands/SKILL.md` (ok)
  - backtick: `streamerbot-dev-voice-commands/SKILL.md` → `retired Pi skill mirror/streamerbot-dev-voice-commands/SKILL.md` (ok)
  - backtick: `streamerbot-dev` → `retired Pi skill mirror/streamerbot-dev/SKILL.md` (ok)
  - backtick: `streamerbot-dev/SKILL.md` → `retired Pi skill mirror/streamerbot-dev/SKILL.md` (ok)
  - backtick: `streamerbot-scripting` → `retired Pi skill mirror/streamerbot-scripting/SKILL.md` (ok)
  - backtick: `sync-workflow` → `retired Pi skill mirror/sync-workflow/SKILL.md` (ok)
  - backtick: `AGENTS.md` → `AGENTS.md` (ok)
  - backtick: `Tools/ContentPipeline/` → `Tools/ContentPipeline/README.md` (ok)
- Inbound refs (0):
  - None

### `retired Pi skill mirror/app-dev/SKILL.md`
- Outbound refs (6):
  - backtick: `agents/roles/app-dev/role.md` → `retired Pi skill mirror/app-dev/agents/roles/app-dev/role.md` (BROKEN)
  - backtick: `agents/roles/app-dev/skills/core.md` → `retired Pi skill mirror/app-dev/agents/roles/app-dev/skills/core.md` (BROKEN)
  - backtick: `agents/roles/app-dev/skills/stream-interactions/_index.md` → `retired Pi skill mirror/app-dev/agents/roles/app-dev/skills/stream-interactions/_index.md` (BROKEN)
  - backtick: `Apps/info-service/` → `Apps/info-service/README.md` (ok)
  - backtick: `Apps/production-manager/` → `Apps/production-manager/README.md` (ok)
  - backtick: `Apps/stream-overlay/` → `Apps/stream-overlay/README.md` (ok)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/art-director/SKILL.md`
- Outbound refs (9):
  - backtick: `agents/roles/art-director/role.md` → `retired Pi skill mirror/art-director/agents/roles/art-director/role.md` (BROKEN)
  - backtick: `agents/roles/art-director/skills/characters/_index.md` → `retired Pi skill mirror/art-director/agents/roles/art-director/skills/characters/_index.md` (BROKEN)
  - backtick: `agents/roles/art-director/skills/characters/captain-stretch.md` → `retired Pi skill mirror/art-director/agents/roles/art-director/skills/characters/captain-stretch.md` (BROKEN)
  - backtick: `agents/roles/art-director/skills/characters/the-director.md` → `retired Pi skill mirror/art-director/agents/roles/art-director/skills/characters/the-director.md` (BROKEN)
  - backtick: `agents/roles/art-director/skills/characters/water-wizard.md` → `retired Pi skill mirror/art-director/agents/roles/art-director/skills/characters/water-wizard.md` (BROKEN)
  - backtick: `agents/roles/art-director/skills/core.md` → `retired Pi skill mirror/art-director/agents/roles/art-director/skills/core.md` (BROKEN)
  - backtick: `agents/roles/art-director/skills/pipeline/_index.md` → `retired Pi skill mirror/art-director/agents/roles/art-director/skills/pipeline/_index.md` (BROKEN)
  - backtick: `agents/roles/art-director/skills/stream-style/_index.md` → `retired Pi skill mirror/art-director/agents/roles/art-director/skills/stream-style/_index.md` (BROKEN)
  - backtick: `Tools/ArtPipeline/` → `Tools/ArtPipeline/README.md` (ok)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/brand-canon-guardian/SKILL.md`
- Outbound refs (2):
  - backtick: `agents/roles/brand-steward/skills/canon-guardian/_index.md` → `retired Pi skill mirror/brand-canon-guardian/agents/roles/brand-steward/skills/canon-guardian/_index.md` (BROKEN)
  - backtick: `pi/skills/brand-steward-canon-guardian/SKILL.md` → `retired Pi skill mirror/brand-canon-guardian/pi/skills/brand-steward-canon-guardian/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/brand-steward-canon-guardian/SKILL.md`
- Outbound refs (1):
  - backtick: `agents/roles/brand-steward/skills/canon-guardian/_index.md` → `retired Pi skill mirror/brand-steward-canon-guardian/agents/roles/brand-steward/skills/canon-guardian/_index.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/brand-steward-content-strategy/SKILL.md`
- Outbound refs (1):
  - backtick: `agents/roles/brand-steward/skills/content-strategy/_index.md` → `retired Pi skill mirror/brand-steward-content-strategy/agents/roles/brand-steward/skills/content-strategy/_index.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/brand-steward/SKILL.md`
- Outbound refs (8):
  - backtick: `agents/roles/brand-steward/role.md` → `retired Pi skill mirror/brand-steward/agents/roles/brand-steward/role.md` (BROKEN)
  - backtick: `agents/roles/brand-steward/skills/canon-guardian/_index.md` → `retired Pi skill mirror/brand-steward/agents/roles/brand-steward/skills/canon-guardian/_index.md` (BROKEN)
  - backtick: `agents/roles/brand-steward/skills/community-growth/_index.md` → `retired Pi skill mirror/brand-steward/agents/roles/brand-steward/skills/community-growth/_index.md` (BROKEN)
  - backtick: `agents/roles/brand-steward/skills/content-strategy/_index.md` → `retired Pi skill mirror/brand-steward/agents/roles/brand-steward/skills/content-strategy/_index.md` (BROKEN)
  - backtick: `agents/roles/brand-steward/skills/core.md` → `retired Pi skill mirror/brand-steward/agents/roles/brand-steward/skills/core.md` (BROKEN)
  - backtick: `agents/roles/brand-steward/skills/voice/_index.md` → `retired Pi skill mirror/brand-steward/agents/roles/brand-steward/skills/voice/_index.md` (BROKEN)
  - backtick: `brand-steward-canon-guardian/SKILL.md` → `retired Pi skill mirror/brand-steward/brand-steward-canon-guardian/SKILL.md` (BROKEN)
  - backtick: `brand-steward-content-strategy/SKILL.md` → `retired Pi skill mirror/brand-steward/brand-steward-content-strategy/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/buildtools/SKILL.md`
- Outbound refs (2):
  - backtick: `agents/roles/ops/skills/core.md` → `retired Pi skill mirror/buildtools/agents/roles/ops/skills/core.md` (BROKEN)
  - backtick: `pi/skills/ops/SKILL.md` → `retired Pi skill mirror/buildtools/pi/skills/ops/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/change-summary/SKILL.md`
- Outbound refs (2):
  - backtick: `agents/roles/ops/skills/change-summary/_index.md` → `retired Pi skill mirror/change-summary/agents/roles/ops/skills/change-summary/_index.md` (BROKEN)
  - backtick: `pi/skills/ops-change-summary/SKILL.md` → `retired Pi skill mirror/change-summary/pi/skills/ops-change-summary/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/content-repurposer-pipeline/SKILL.md`
- Outbound refs (3):
  - backtick: `agents/roles/content-repurposer/skills/core.md` → `retired Pi skill mirror/content-repurposer-pipeline/agents/roles/content-repurposer/skills/core.md` (BROKEN)
  - backtick: `agents/roles/content-repurposer/skills/pipeline/_index.md` → `retired Pi skill mirror/content-repurposer-pipeline/agents/roles/content-repurposer/skills/pipeline/_index.md` (BROKEN)
  - backtick: `Tools/ContentPipeline/` → `Tools/ContentPipeline/README.md` (ok)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/content-repurposer/SKILL.md`
- Outbound refs (6):
  - backtick: `agents/roles/content-repurposer/role.md` → `retired Pi skill mirror/content-repurposer/agents/roles/content-repurposer/role.md` (BROKEN)
  - backtick: `agents/roles/content-repurposer/skills/clip-strategy/_index.md` → `retired Pi skill mirror/content-repurposer/agents/roles/content-repurposer/skills/clip-strategy/_index.md` (BROKEN)
  - backtick: `agents/roles/content-repurposer/skills/core.md` → `retired Pi skill mirror/content-repurposer/agents/roles/content-repurposer/skills/core.md` (BROKEN)
  - backtick: `agents/roles/content-repurposer/skills/platforms/_index.md` → `retired Pi skill mirror/content-repurposer/agents/roles/content-repurposer/skills/platforms/_index.md` (BROKEN)
  - backtick: `content-repurposer-pipeline/SKILL.md` → `retired Pi skill mirror/content-repurposer/content-repurposer-pipeline/SKILL.md` (BROKEN)
  - backtick: `Tools/ContentPipeline/` → `Tools/ContentPipeline/README.md` (ok)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/content-strategy/SKILL.md`
- Outbound refs (2):
  - backtick: `agents/roles/brand-steward/skills/content-strategy/_index.md` → `retired Pi skill mirror/content-strategy/agents/roles/brand-steward/skills/content-strategy/_index.md` (BROKEN)
  - backtick: `pi/skills/brand-steward-content-strategy/SKILL.md` → `retired Pi skill mirror/content-strategy/pi/skills/brand-steward-content-strategy/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/creative-art/SKILL.md`
- Outbound refs (1):
  - backtick: `pi/skills/art-director/SKILL.md` → `retired Pi skill mirror/creative-art/pi/skills/art-director/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/creative-worldbuilding/SKILL.md`
- Outbound refs (2):
  - backtick: `pi/skills/lotat-tech/SKILL.md` → `retired Pi skill mirror/creative-worldbuilding/pi/skills/lotat-tech/SKILL.md` (BROKEN)
  - backtick: `pi/skills/lotat-writer/SKILL.md` → `retired Pi skill mirror/creative-worldbuilding/pi/skills/lotat-writer/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/feature-channel-points/SKILL.md`
- Outbound refs (2):
  - backtick: `agents/roles/streamerbot-dev/skills/twitch/channel-points.md` → `retired Pi skill mirror/feature-channel-points/agents/roles/streamerbot-dev/skills/twitch/channel-points.md` (BROKEN)
  - backtick: `pi/skills/streamerbot-dev-twitch/SKILL.md` → `retired Pi skill mirror/feature-channel-points/pi/skills/streamerbot-dev-twitch/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/feature-commanders/SKILL.md`
- Outbound refs (1):
  - backtick: `pi/skills/streamerbot-dev-commanders/SKILL.md` → `retired Pi skill mirror/feature-commanders/pi/skills/streamerbot-dev-commanders/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/feature-hype-train/SKILL.md`
- Outbound refs (2):
  - backtick: `agents/roles/streamerbot-dev/skills/twitch/hype-train.md` → `retired Pi skill mirror/feature-hype-train/agents/roles/streamerbot-dev/skills/twitch/hype-train.md` (BROKEN)
  - backtick: `pi/skills/streamerbot-dev-twitch/SKILL.md` → `retired Pi skill mirror/feature-hype-train/pi/skills/streamerbot-dev-twitch/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/feature-squad/SKILL.md`
- Outbound refs (1):
  - backtick: `pi/skills/streamerbot-dev-squad/SKILL.md` → `retired Pi skill mirror/feature-squad/pi/skills/streamerbot-dev-squad/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/feature-twitch-integration/SKILL.md`
- Outbound refs (1):
  - backtick: `pi/skills/streamerbot-dev-twitch/SKILL.md` → `retired Pi skill mirror/feature-twitch-integration/pi/skills/streamerbot-dev-twitch/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/feature-voice-commands/SKILL.md`
- Outbound refs (1):
  - backtick: `pi/skills/streamerbot-dev-voice-commands/SKILL.md` → `retired Pi skill mirror/feature-voice-commands/pi/skills/streamerbot-dev-voice-commands/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/lotat-tech/SKILL.md`
- Outbound refs (10):
  - backtick: `agents/roles/lotat-tech/role.md` → `retired Pi skill mirror/lotat-tech/agents/roles/lotat-tech/role.md` (BROKEN)
  - backtick: `agents/roles/lotat-tech/skills/core.md` → `retired Pi skill mirror/lotat-tech/agents/roles/lotat-tech/skills/core.md` (BROKEN)
  - backtick: `agents/roles/lotat-tech/skills/engine/_index.md` → `retired Pi skill mirror/lotat-tech/agents/roles/lotat-tech/skills/engine/_index.md` (BROKEN)
  - backtick: `agents/roles/lotat-tech/skills/engine/commands.md` → `retired Pi skill mirror/lotat-tech/agents/roles/lotat-tech/skills/engine/commands.md` (BROKEN)
  - backtick: `agents/roles/lotat-tech/skills/engine/docs-map.md` → `retired Pi skill mirror/lotat-tech/agents/roles/lotat-tech/skills/engine/docs-map.md` (BROKEN)
  - backtick: `agents/roles/lotat-tech/skills/engine/session-lifecycle.md` → `retired Pi skill mirror/lotat-tech/agents/roles/lotat-tech/skills/engine/session-lifecycle.md` (BROKEN)
  - backtick: `agents/roles/lotat-tech/skills/engine/state-and-voting.md` → `retired Pi skill mirror/lotat-tech/agents/roles/lotat-tech/skills/engine/state-and-voting.md` (BROKEN)
  - backtick: `agents/roles/lotat-tech/skills/story-pipeline/_index.md` → `retired Pi skill mirror/lotat-tech/agents/roles/lotat-tech/skills/story-pipeline/_index.md` (BROKEN)
  - backtick: `agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` → `retired Pi skill mirror/lotat-tech/agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` (BROKEN)
  - backtick: `ops-change-summary/SKILL.md` → `retired Pi skill mirror/lotat-tech/ops-change-summary/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/lotat-writer-canon-guardian/SKILL.md`
- Outbound refs (2):
  - backtick: `agents/roles/lotat-writer/skills/canon-guardian/_index.md` → `retired Pi skill mirror/lotat-writer-canon-guardian/agents/roles/lotat-writer/skills/canon-guardian/_index.md` (BROKEN)
  - backtick: `pi/skills/brand-steward-canon-guardian/SKILL.md` → `retired Pi skill mirror/lotat-writer-canon-guardian/pi/skills/brand-steward-canon-guardian/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/lotat-writer/SKILL.md`
- Outbound refs (11):
  - backtick: `agents/roles/lotat-writer/role.md` → `retired Pi skill mirror/lotat-writer/agents/roles/lotat-writer/role.md` (BROKEN)
  - backtick: `agents/roles/lotat-writer/skills/adventures/_index.md` → `retired Pi skill mirror/lotat-writer/agents/roles/lotat-writer/skills/adventures/_index.md` (BROKEN)
  - backtick: `agents/roles/lotat-writer/skills/adventures/mechanics.md` → `retired Pi skill mirror/lotat-writer/agents/roles/lotat-writer/skills/adventures/mechanics.md` (BROKEN)
  - backtick: `agents/roles/lotat-writer/skills/adventures/session-format.md` → `retired Pi skill mirror/lotat-writer/agents/roles/lotat-writer/skills/adventures/session-format.md` (BROKEN)
  - backtick: `agents/roles/lotat-writer/skills/core.md` → `retired Pi skill mirror/lotat-writer/agents/roles/lotat-writer/skills/core.md` (BROKEN)
  - backtick: `agents/roles/lotat-writer/skills/franchises/starship-shamples.md` → `retired Pi skill mirror/lotat-writer/agents/roles/lotat-writer/skills/franchises/starship-shamples.md` (BROKEN)
  - backtick: `agents/roles/lotat-writer/skills/universe/_index.md` → `retired Pi skill mirror/lotat-writer/agents/roles/lotat-writer/skills/universe/_index.md` (BROKEN)
  - backtick: `agents/roles/lotat-writer/skills/universe/cast.md` → `retired Pi skill mirror/lotat-writer/agents/roles/lotat-writer/skills/universe/cast.md` (BROKEN)
  - backtick: `agents/roles/lotat-writer/skills/universe/rules.md` → `retired Pi skill mirror/lotat-writer/agents/roles/lotat-writer/skills/universe/rules.md` (BROKEN)
  - backtick: `brand-steward-canon-guardian/SKILL.md` → `retired Pi skill mirror/lotat-writer/brand-steward-canon-guardian/SKILL.md` (BROKEN)
  - backtick: `lotat-writer-canon-guardian/SKILL.md` → `retired Pi skill mirror/lotat-writer/lotat-writer-canon-guardian/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/meta-agents-navigate/SKILL.md`
- Outbound refs (8):
  - backtick: `conventions.md` → `.agents/_shared/conventions.md` (ok)
  - backtick: `coordination.md` → `.agents/_shared/coordination.md` (ok)
  - backtick: `info-service-protocol.md` → `.agents/_shared/info-service-protocol.md` (ok)
  - backtick: `mixitup-api.md` → `.agents/_shared/mixitup-api.md` (ok)
  - backtick: `project.md` → `.agents/_shared/project.md` (ok)
  - backtick: `ENTRY.md` → `retired Pi skill mirror/meta-agents-navigate/ENTRY.md` (BROKEN)
  - backtick: `_index.md` → `retired Pi skill mirror/meta-agents-navigate/_index.md` (BROKEN)
  - backtick: `agents/ENTRY.md` → `retired Pi skill mirror/meta-agents-navigate/agents/ENTRY.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/meta-agents-update/SKILL.md`
- Outbound refs (9):
  - backtick: `ENTRY.md` → `retired Pi skill mirror/meta-agents-update/ENTRY.md` (BROKEN)
  - backtick: `_index.md` → `retired Pi skill mirror/meta-agents-update/_index.md` (BROKEN)
  - backtick: `pi/skills/README.md` → `retired Pi skill mirror/meta-agents-update/pi/skills/README.md` (BROKEN)
  - backtick: `role.md` → `retired Pi skill mirror/meta-agents-update/role.md` (BROKEN)
  - backtick: `AGENTS.md` → `AGENTS.md` (ok)
  - backtick: `Actions/HELPER-SNIPPETS.md` → `Actions/HELPER-SNIPPETS.md` (ok)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Creative/WorldBuilding/` → `Creative/WorldBuilding/README.md` (ok)
  - backtick: `Tools/` → `Tools/README.md` (ok)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/meta/SKILL.md`
- Outbound refs (2):
  - backtick: `meta-agents-navigate/SKILL.md` → `retired Pi skill mirror/meta/meta-agents-navigate/SKILL.md` (BROKEN)
  - backtick: `meta-agents-update/SKILL.md` → `retired Pi skill mirror/meta/meta-agents-update/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/ops-change-summary/SKILL.md`
- Outbound refs (1):
  - backtick: `agents/roles/ops/skills/change-summary/_index.md` → `retired Pi skill mirror/ops-change-summary/agents/roles/ops/skills/change-summary/_index.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/ops-sync/SKILL.md`
- Outbound refs (1):
  - backtick: `agents/roles/ops/skills/sync/_index.md` → `retired Pi skill mirror/ops-sync/agents/roles/ops/skills/sync/_index.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/ops-validation/SKILL.md`
- Outbound refs (1):
  - backtick: `agents/roles/ops/skills/validation/_index.md` → `retired Pi skill mirror/ops-validation/agents/roles/ops/skills/validation/_index.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/ops/SKILL.md`
- Outbound refs (5):
  - backtick: `agents/roles/ops/role.md` → `retired Pi skill mirror/ops/agents/roles/ops/role.md` (BROKEN)
  - backtick: `agents/roles/ops/skills/core.md` → `retired Pi skill mirror/ops/agents/roles/ops/skills/core.md` (BROKEN)
  - backtick: `ops-change-summary/SKILL.md` → `retired Pi skill mirror/ops/ops-change-summary/SKILL.md` (BROKEN)
  - backtick: `ops-sync/SKILL.md` → `retired Pi skill mirror/ops/ops-sync/SKILL.md` (BROKEN)
  - backtick: `ops-validation/SKILL.md` → `retired Pi skill mirror/ops/ops-validation/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/product-dev/SKILL.md`
- Outbound refs (2):
  - backtick: `agents/roles/product-dev/role.md` → `retired Pi skill mirror/product-dev/agents/roles/product-dev/role.md` (BROKEN)
  - backtick: `agents/roles/product-dev/skills/core.md` → `retired Pi skill mirror/product-dev/agents/roles/product-dev/skills/core.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/streamerbot-dev-commanders/SKILL.md`
- Outbound refs (4):
  - backtick: `agents/roles/streamerbot-dev/skills/commanders/_index.md` → `retired Pi skill mirror/streamerbot-dev-commanders/agents/roles/streamerbot-dev/skills/commanders/_index.md` (BROKEN)
  - backtick: `agents/roles/streamerbot-dev/skills/commanders/captain-stretch.md` → `retired Pi skill mirror/streamerbot-dev-commanders/agents/roles/streamerbot-dev/skills/commanders/captain-stretch.md` (BROKEN)
  - backtick: `agents/roles/streamerbot-dev/skills/commanders/the-director.md` → `retired Pi skill mirror/streamerbot-dev-commanders/agents/roles/streamerbot-dev/skills/commanders/the-director.md` (BROKEN)
  - backtick: `agents/roles/streamerbot-dev/skills/commanders/water-wizard.md` → `retired Pi skill mirror/streamerbot-dev-commanders/agents/roles/streamerbot-dev/skills/commanders/water-wizard.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/streamerbot-dev-lotat/SKILL.md`
- Outbound refs (2):
  - backtick: `agents/roles/streamerbot-dev/skills/lotat/_index.md` → `retired Pi skill mirror/streamerbot-dev-lotat/agents/roles/streamerbot-dev/skills/lotat/_index.md` (BROKEN)
  - backtick: `lotat-tech/SKILL.md` → `retired Pi skill mirror/streamerbot-dev-lotat/lotat-tech/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/streamerbot-dev-squad/SKILL.md`
- Outbound refs (5):
  - backtick: `agents/roles/streamerbot-dev/skills/squad/_index.md` → `retired Pi skill mirror/streamerbot-dev-squad/agents/roles/streamerbot-dev/skills/squad/_index.md` (BROKEN)
  - backtick: `agents/roles/streamerbot-dev/skills/squad/clone.md` → `retired Pi skill mirror/streamerbot-dev-squad/agents/roles/streamerbot-dev/skills/squad/clone.md` (BROKEN)
  - backtick: `agents/roles/streamerbot-dev/skills/squad/duck.md` → `retired Pi skill mirror/streamerbot-dev-squad/agents/roles/streamerbot-dev/skills/squad/duck.md` (BROKEN)
  - backtick: `agents/roles/streamerbot-dev/skills/squad/pedro.md` → `retired Pi skill mirror/streamerbot-dev-squad/agents/roles/streamerbot-dev/skills/squad/pedro.md` (BROKEN)
  - backtick: `agents/roles/streamerbot-dev/skills/squad/toothless.md` → `retired Pi skill mirror/streamerbot-dev-squad/agents/roles/streamerbot-dev/skills/squad/toothless.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/streamerbot-dev-twitch/SKILL.md`
- Outbound refs (5):
  - backtick: `agents/roles/streamerbot-dev/skills/twitch/_index.md` → `retired Pi skill mirror/streamerbot-dev-twitch/agents/roles/streamerbot-dev/skills/twitch/_index.md` (BROKEN)
  - backtick: `agents/roles/streamerbot-dev/skills/twitch/bits.md` → `retired Pi skill mirror/streamerbot-dev-twitch/agents/roles/streamerbot-dev/skills/twitch/bits.md` (BROKEN)
  - backtick: `agents/roles/streamerbot-dev/skills/twitch/channel-points.md` → `retired Pi skill mirror/streamerbot-dev-twitch/agents/roles/streamerbot-dev/skills/twitch/channel-points.md` (BROKEN)
  - backtick: `agents/roles/streamerbot-dev/skills/twitch/core-events.md` → `retired Pi skill mirror/streamerbot-dev-twitch/agents/roles/streamerbot-dev/skills/twitch/core-events.md` (BROKEN)
  - backtick: `agents/roles/streamerbot-dev/skills/twitch/hype-train.md` → `retired Pi skill mirror/streamerbot-dev-twitch/agents/roles/streamerbot-dev/skills/twitch/hype-train.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/streamerbot-dev-voice-commands/SKILL.md`
- Outbound refs (1):
  - backtick: `agents/roles/streamerbot-dev/skills/voice-commands/_index.md` → `retired Pi skill mirror/streamerbot-dev-voice-commands/agents/roles/streamerbot-dev/skills/voice-commands/_index.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/streamerbot-dev/SKILL.md`
- Outbound refs (8):
  - backtick: `agents/roles/streamerbot-dev/role.md` → `retired Pi skill mirror/streamerbot-dev/agents/roles/streamerbot-dev/role.md` (BROKEN)
  - backtick: `agents/roles/streamerbot-dev/skills/core.md` → `retired Pi skill mirror/streamerbot-dev/agents/roles/streamerbot-dev/skills/core.md` (BROKEN)
  - backtick: `ops-change-summary/SKILL.md` → `retired Pi skill mirror/streamerbot-dev/ops-change-summary/SKILL.md` (BROKEN)
  - backtick: `streamerbot-dev-commanders/SKILL.md` → `retired Pi skill mirror/streamerbot-dev/streamerbot-dev-commanders/SKILL.md` (BROKEN)
  - backtick: `streamerbot-dev-lotat/SKILL.md` → `retired Pi skill mirror/streamerbot-dev/streamerbot-dev-lotat/SKILL.md` (BROKEN)
  - backtick: `streamerbot-dev-squad/SKILL.md` → `retired Pi skill mirror/streamerbot-dev/streamerbot-dev-squad/SKILL.md` (BROKEN)
  - backtick: `streamerbot-dev-twitch/SKILL.md` → `retired Pi skill mirror/streamerbot-dev/streamerbot-dev-twitch/SKILL.md` (BROKEN)
  - backtick: `streamerbot-dev-voice-commands/SKILL.md` → `retired Pi skill mirror/streamerbot-dev/streamerbot-dev-voice-commands/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/streamerbot-scripting/SKILL.md`
- Outbound refs (2):
  - backtick: `agents/roles/streamerbot-dev/skills/core.md` → `retired Pi skill mirror/streamerbot-scripting/agents/roles/streamerbot-dev/skills/core.md` (BROKEN)
  - backtick: `pi/skills/streamerbot-dev/SKILL.md` → `retired Pi skill mirror/streamerbot-scripting/pi/skills/streamerbot-dev/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `retired Pi skill mirror/sync-workflow/SKILL.md`
- Outbound refs (2):
  - backtick: `agents/roles/ops/skills/sync/_index.md` → `retired Pi skill mirror/sync-workflow/agents/roles/ops/skills/sync/_index.md` (BROKEN)
  - backtick: `pi/skills/ops-sync/SKILL.md` → `retired Pi skill mirror/sync-workflow/pi/skills/ops-sync/SKILL.md` (BROKEN)
- Inbound refs (1):
  - `retired Pi skill mirror/README.md`

### `AGENTS.md`
- Outbound refs (12):
  - backtick: `Actions/HELPER-SNIPPETS.md` → `Actions/HELPER-SNIPPETS.md` (ok)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Creative/Brand/BRAND-IDENTITY.md` → `Creative/Brand/BRAND-IDENTITY.md` (ok)
  - backtick: `Creative/Brand/CHARACTER-CODEX.md` → `Creative/Brand/CHARACTER-CODEX.md` (ok)
  - backtick: `Creative/` → `Creative/README.md` (ok)
  - backtick: `Docs/` → `Docs` (ok)
  - backtick: `Docs/AGENT-WORKFLOW.md` → `Docs/AGENT-WORKFLOW.md` (ok)
  - backtick: `Docs/ONBOARDING.md` → `Docs/ONBOARDING.md` (ok)
  - backtick: `Tools/` → `Tools/README.md` (ok)
  - backtick: `WORKING.md` → `WORKING.md` (ok)
  - backtick: `agents/ENTRY.md` → `agents/ENTRY.md` (BROKEN)
  - backtick: `pi/skills/README.md` → `pi/skills/README.md` (BROKEN)
- Inbound refs (5):
  - `.agents/roles/ops/skills/core.md`
  - `.agents/roles/ops/skills/validation/_index.md`
  - `retired Pi skill mirror/README.md`
  - `retired Pi skill mirror/meta-agents-update/SKILL.md`
  - `Docs/ONBOARDING.md`

### `Actions/Overlay/README.md`
- Outbound refs (4):
  - backtick: `Actions/HELPER-SNIPPETS.md § 7` → `Actions/HELPER-SNIPPETS.md` (ok)
  - backtick: `agents/roles/app-dev/skills/stream-interactions/protocol.md` → `Actions/Overlay/agents/roles/app-dev/skills/stream-interactions/protocol.md` (BROKEN)
  - backtick: `Actions/SHARED-CONSTANTS.md → Overlay / Broker` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Apps/stream-overlay/` → `Apps/stream-overlay/README.md` (ok)
- Inbound refs (2):
  - `.agents/roles/app-dev/skills/stream-interactions/_index.md`
  - `.agents/roles/streamerbot-dev/skills/overlay-integration.md`

### `Apps/info-service/README.md`
- Outbound refs (1):
  - backtick: `agents/_shared/info-service-protocol.md` → `Apps/info-service/agents/_shared/info-service-protocol.md` (BROKEN)
- Inbound refs (7):
  - `.agents/roles/app-dev/context/info-service.md`
  - `.agents/roles/app-dev/role.md`
  - `.agents/roles/app-dev/skills/core.md`
  - `.agents/roles/app-dev/skills/stream-interactions/_index.md`
  - `retired Pi skill mirror/app-dev/SKILL.md`
  - `Docs/Architecture/repo-structure.md`
  - `Docs/INFO-SERVICE-PLAN.md`

### `Apps/stream-overlay/README.md`
- Outbound refs (2):
  - backtick: `SHARED-CONSTANTS.md` → `Apps/stream-overlay/SHARED-CONSTANTS.md` (BROKEN)
  - backtick: `agents/roles/streamerbot-dev/skills/overlay-integration.md` → `Apps/stream-overlay/agents/roles/streamerbot-dev/skills/overlay-integration.md` (BROKEN)
- Inbound refs (9):
  - `.agents/roles/app-dev/role.md`
  - `.agents/roles/app-dev/skills/stream-interactions/_index.md`
  - `.agents/roles/app-dev/skills/stream-interactions/overlay.md`
  - `.agents/roles/lotat-tech/skills/engine/_index.md`
  - `.agents/roles/streamerbot-dev/skills/overlay-integration.md`
  - `retired Pi skill mirror/app-dev/SKILL.md`
  - `Actions/Overlay/README.md`
  - `Docs/Architecture/repo-structure.md`
  - `README.md`

### `CLAUDE.md`
- Outbound refs (4):
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `WORKING.md` → `WORKING.md` (ok)
  - backtick: `agents/ENTRY.md` → `agents/ENTRY.md` (BROKEN)
  - backtick: `agents/roles/ops/skills/change-summary/_index.md` → `agents/roles/ops/skills/change-summary/_index.md` (BROKEN)
- Inbound refs (0):
  - None

### `Creative/Art/README.md`
- Outbound refs (2):
  - backtick: `Assets/` → `Assets/README.md` (ok)
  - backtick: `Creative/Art/` → `Creative/Art/README.md` (ok)
- Inbound refs (5):
  - `Creative/Marketing/README.md`
  - `Creative/README.md`
  - `Docs/Architecture/repo-structure.md`
  - `README.md`
  - `WORKING.md`

### `Creative/Marketing/README.md`
- Outbound refs (6):
  - backtick: `Creative/Art/` → `Creative/Art/README.md` (ok)
  - backtick: `Creative/Brand/BRAND-IDENTITY.md` → `Creative/Brand/BRAND-IDENTITY.md` (ok)
  - backtick: `Creative/Brand/BRAND-VOICE.md` → `Creative/Brand/BRAND-VOICE.md` (ok)
  - backtick: `Creative/Marketing/` → `Creative/Marketing/README.md` (ok)
  - backtick: `Creative/WorldBuilding/` → `Creative/WorldBuilding/README.md` (ok)
  - backtick: `Tools/` → `Tools/README.md` (ok)
- Inbound refs (2):
  - `Creative/README.md`
  - `README.md`

### `Creative/README.md`
- Outbound refs (7):
  - backtick: `Art/` → `Creative/Art/README.md` (ok)
  - backtick: `Creative/Art/` → `Creative/Art/README.md` (ok)
  - backtick: `Creative/Marketing/` → `Creative/Marketing/README.md` (ok)
  - backtick: `Marketing/` → `Creative/Marketing/README.md` (ok)
  - backtick: `Creative/` → `Creative/README.md` (ok)
  - backtick: `Creative/WorldBuilding/` → `Creative/WorldBuilding/README.md` (ok)
  - backtick: `WorldBuilding/` → `Creative/WorldBuilding/README.md` (ok)
- Inbound refs (11):
  - `.agents/_shared/conventions.md`
  - `.agents/_shared/coordination.md`
  - `.agents/_shared/project.md`
  - `.agents/roles/ops/skills/change-summary/_index.md`
  - `.agents/roles/ops/skills/core.md`
  - `.agents/roles/ops/skills/sync/_index.md`
  - `AGENTS.md`
  - `Docs/AGENT-WORKFLOW.md`
  - `Docs/Architecture/repo-structure.md`
  - `README.md`
  - `WORKING.md`

### `Creative/WorldBuilding/README.md`
- Outbound refs (1):
  - backtick: `Creative/WorldBuilding/` → `Creative/WorldBuilding/README.md` (ok)
- Inbound refs (5):
  - `retired Pi skill mirror/meta-agents-update/SKILL.md`
  - `Creative/Marketing/README.md`
  - `Creative/README.md`
  - `Docs/Architecture/repo-structure.md`
  - `README.md`

### `Docs/AGENT-WORKFLOW.md`
- Outbound refs (3):
  - backtick: `Creative/` → `Creative/README.md` (ok)
  - backtick: `SHARED-CONSTANTS.md` → `Docs/SHARED-CONSTANTS.md` (BROKEN)
  - backtick: `WORKING.md` → `WORKING.md` (ok)
- Inbound refs (4):
  - `.agents/_shared/conventions.md`
  - `AGENTS.md`
  - `Docs/ONBOARDING.md`
  - `WORKING.md`

### `Docs/Architecture/repo-structure.md`
- Outbound refs (10):
  - backtick: `Apps/info-service/` → `Apps/info-service/README.md` (ok)
  - backtick: `Apps/production-manager/` → `Apps/production-manager/README.md` (ok)
  - backtick: `Apps/stream-overlay/` → `Apps/stream-overlay/README.md` (ok)
  - backtick: `Creative/Art/` → `Creative/Art/README.md` (ok)
  - backtick: `Creative/` → `Creative/README.md` (ok)
  - backtick: `Creative/WorldBuilding/` → `Creative/WorldBuilding/README.md` (ok)
  - backtick: `Docs/` → `Docs` (ok)
  - backtick: `Tools/MixItUp/` → `Tools/MixItUp/README.md` (ok)
  - backtick: `Tools/` → `Tools/README.md` (ok)
  - backtick: `Tools/StreamerBot/` → `Tools/StreamerBot/README.md` (ok)
- Inbound refs (1):
  - `README.md`

### `Docs/INFO-SERVICE-PLAN.md`
- Outbound refs (16):
  - backtick: `Actions/Intros/README.md` → `Actions/Intros/README.md` (BROKEN)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Apps/info-service/` → `Apps/info-service/README.md` (ok)
  - backtick: `Apps/production-manager/` → `Apps/production-manager/README.md` (ok)
  - backtick: `Assets/` → `Assets/README.md` (ok)
  - backtick: `COORDINATION.md` → `Docs/COORDINATION.md` (BROKEN)
  - backtick: `INFO-SERVICE-PLAN.md` → `Docs/INFO-SERVICE-PLAN.md` (ok)
  - backtick: `SHARED-CONSTANTS.md` → `Docs/SHARED-CONSTANTS.md` (BROKEN)
  - backtick: `_shared/info-service-protocol.md` → `Docs/_shared/info-service-protocol.md` (BROKEN)
  - backtick: `agents/_shared/info-service-protocol.md` → `Docs/agents/_shared/info-service-protocol.md` (BROKEN)
  - backtick: `agents/_shared/mixitup-api.md` → `Docs/agents/_shared/mixitup-api.md` (BROKEN)
  - backtick: `agents/roles/app-dev/role.md` → `Docs/agents/roles/app-dev/role.md` (BROKEN)
  - backtick: `agents/roles/app-dev/skills/core.md` → `Docs/agents/roles/app-dev/skills/core.md` (BROKEN)
  - backtick: `agents/roles/app-dev/skills/stream-interactions/_index.md` → `Docs/agents/roles/app-dev/skills/stream-interactions/_index.md` (BROKEN)
  - backtick: `humans/info-service/COORDINATION.md` → `Docs/humans/info-service/COORDINATION.md` (BROKEN)
  - backtick: `info-service-protocol.md` → `Docs/info-service-protocol.md` (BROKEN)
- Inbound refs (2):
  - `.agents/_shared/info-service-protocol.md`
  - `.agents/roles/app-dev/context/info-service.md`

### `Docs/ONBOARDING.md`
- Outbound refs (35):
  - backtick: `AGENTS.md` → `AGENTS.md` (ok)
  - backtick: `Actions/HELPER-SNIPPETS.md` → `Actions/HELPER-SNIPPETS.md` (ok)
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Creative/Art/Agents/stream-style-art-agent.md` → `Creative/Art/Agents/stream-style-art-agent.md` (ok)
  - backtick: `Creative/Brand/BRAND-IDENTITY.md` → `Creative/Brand/BRAND-IDENTITY.md` (ok)
  - backtick: `Creative/Brand/BRAND-VOICE.md` → `Creative/Brand/BRAND-VOICE.md` (ok)
  - backtick: `Creative/Brand/CHARACTER-CODEX.md` → `Creative/Brand/CHARACTER-CODEX.md` (ok)
  - backtick: `Creative/WorldBuilding/Agents/D&D-Agent.md` → `Creative/WorldBuilding/Agents/D&D-Agent.md` (ok)
  - backtick: `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` → `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` (ok)
  - backtick: `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` → `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` (ok)
  - backtick: `Docs/AGENT-WORKFLOW.md` → `Docs/AGENT-WORKFLOW.md` (ok)
  - backtick: `agents/ENTRY.md` → `Docs/agents/ENTRY.md` (BROKEN)
  - backtick: `agents/roles/art-director/skills/core.md` → `Docs/agents/roles/art-director/skills/core.md` (BROKEN)
  - backtick: `agents/roles/brand-steward/skills/canon-guardian/_index.md` → `Docs/agents/roles/brand-steward/skills/canon-guardian/_index.md` (BROKEN)
  - backtick: `agents/roles/brand-steward/skills/content-strategy/_index.md` → `Docs/agents/roles/brand-steward/skills/content-strategy/_index.md` (BROKEN)
  - backtick: `agents/roles/brand-steward/skills/core.md` → `Docs/agents/roles/brand-steward/skills/core.md` (BROKEN)
  - backtick: `agents/roles/content-repurposer/skills/core.md` → `Docs/agents/roles/content-repurposer/skills/core.md` (BROKEN)
  - backtick: `agents/roles/content-repurposer/skills/pipeline/_index.md` → `Docs/agents/roles/content-repurposer/skills/pipeline/_index.md` (BROKEN)
  - backtick: `agents/roles/lotat-tech/skills/core.md` → `Docs/agents/roles/lotat-tech/skills/core.md` (BROKEN)
  - backtick: `agents/roles/lotat-writer/skills/core.md` → `Docs/agents/roles/lotat-writer/skills/core.md` (BROKEN)
  - backtick: `agents/roles/ops/skills/core.md` → `Docs/agents/roles/ops/skills/core.md` (BROKEN)
  - backtick: `agents/roles/product-dev/skills/core.md` → `Docs/agents/roles/product-dev/skills/core.md` (BROKEN)
  - backtick: `agents/roles/streamerbot-dev/skills/core.md` → `Docs/agents/roles/streamerbot-dev/skills/core.md` (BROKEN)
  - backtick: `role.md` → `Docs/role.md` (BROKEN)
  - backtick: `roles/art-director/role.md` → `Docs/roles/art-director/role.md` (BROKEN)
  - backtick: `roles/brand-steward/role.md` → `Docs/roles/brand-steward/role.md` (BROKEN)
  - backtick: `roles/content-repurposer/role.md` → `Docs/roles/content-repurposer/role.md` (BROKEN)
  - backtick: `roles/lotat-tech/role.md` → `Docs/roles/lotat-tech/role.md` (BROKEN)
  - backtick: `roles/lotat-writer/role.md` → `Docs/roles/lotat-writer/role.md` (BROKEN)
  - backtick: `roles/ops/role.md` → `Docs/roles/ops/role.md` (BROKEN)
  - backtick: `roles/product-dev/role.md` → `Docs/roles/product-dev/role.md` (BROKEN)
  - backtick: `roles/streamerbot-dev/role.md` → `Docs/roles/streamerbot-dev/role.md` (BROKEN)
  - backtick: `README.md` → `README.md` (ok)
  - backtick: `Tools/ContentPipeline/` → `Tools/ContentPipeline/README.md` (ok)
  - backtick: `WORKING.md` → `WORKING.md` (ok)
- Inbound refs (2):
  - `.agents/_shared/project.md`
  - `AGENTS.md`

### `README.md`
- Outbound refs (30):
  - backtick: `Actions/Commanders/Captain Stretch/README.md` → `Actions/Commanders/Captain` (BROKEN)
  - backtick: `Actions/Commanders/README.md` → `Actions/Commanders/README.md` (ok)
  - backtick: `Actions/Commanders/The Director/README.md` → `Actions/Commanders/The` (BROKEN)
  - backtick: `Actions/Commanders/Water Wizard/README.md` → `Actions/Commanders/Water` (BROKEN)
  - backtick: `Actions/HELPER-SNIPPETS.md` → `Actions/HELPER-SNIPPETS.md` (ok)
  - backtick: `Actions/Squad/Clone/README.md` → `Actions/Squad/Clone/README.md` (ok)
  - backtick: `Actions/Squad/Duck/README.md` → `Actions/Squad/Duck/README.md` (ok)
  - backtick: `Actions/Squad/Pedro/README.md` → `Actions/Squad/Pedro/README.md` (ok)
  - backtick: `Actions/Squad/` → `Actions/Squad/README.md` (ok)
  - backtick: `Actions/Squad/README.md` → `Actions/Squad/README.md` (ok)
  - backtick: `Actions/Squad/Toothless/README.md` → `Actions/Squad/Toothless/README.md` (ok)
  - backtick: `Actions/Twitch Bits Integrations/README.md` → `Actions/Twitch` (BROKEN)
  - backtick: `Actions/Twitch Channel Points/README.md` → `Actions/Twitch` (BROKEN)
  - backtick: `Actions/Twitch Core Integrations/README.md` → `Actions/Twitch` (BROKEN)
  - backtick: `Actions/Twitch Hype Train/README.md` → `Actions/Twitch` (BROKEN)
  - backtick: `Actions/Voice Commands/README.md` → `Actions/Voice` (BROKEN)
  - backtick: `Apps/stream-overlay/README.md` → `Apps/stream-overlay/README.md` (ok)
  - backtick: `Creative/Art/README.md` → `Creative/Art/README.md` (ok)
  - backtick: `Creative/Marketing/README.md` → `Creative/Marketing/README.md` (ok)
  - backtick: `Creative/` → `Creative/README.md` (ok)
  - backtick: `Creative/README.md` → `Creative/README.md` (ok)
  - backtick: `Creative/WorldBuilding/README.md` → `Creative/WorldBuilding/README.md` (ok)
  - backtick: `Docs/` → `Docs` (ok)
  - backtick: `Docs/Architecture/repo-structure.md` → `Docs/Architecture/repo-structure.md` (ok)
  - backtick: `Tools/MixItUp/Api/README.md` → `Tools/MixItUp/Api/README.md` (ok)
  - backtick: `Tools/MixItUp/README.md` → `Tools/MixItUp/README.md` (ok)
  - backtick: `Tools/` → `Tools/README.md` (ok)
  - backtick: `Tools/README.md` → `Tools/README.md` (ok)
  - backtick: `Tools/StreamerBot/README.md` → `Tools/StreamerBot/README.md` (ok)
  - backtick: `agents/ENTRY.md` → `agents/ENTRY.md` (BROKEN)
- Inbound refs (3):
  - `.agents/roles/art-director/skills/pipeline/_index.md`
  - `.agents/roles/content-repurposer/skills/pipeline/_index.md`
  - `Docs/ONBOARDING.md`

### `Tools/ArtPipeline/README.md`
- Outbound refs (4):
  - backtick: `Creative/Art/Agents/stream-style-art-agent.md` → `Creative/Art/Agents/stream-style-art-agent.md` (ok)
  - backtick: `Tools/ArtPipeline/FULL-RUN.md` → `Tools/ArtPipeline/FULL-RUN.md` (ok)
  - backtick: `Tools/ArtPipeline/` → `Tools/ArtPipeline/README.md` (ok)
  - backtick: `Tools/ArtPipeline/SETUP.md` → `Tools/ArtPipeline/SETUP.md` (ok)
- Inbound refs (3):
  - `.agents/roles/art-director/role.md`
  - `.agents/roles/art-director/skills/pipeline/_index.md`
  - `retired Pi skill mirror/art-director/SKILL.md`

### `Tools/ContentPipeline/README.md`
- Outbound refs (1):
  - backtick: `Tools/ContentPipeline/` → `Tools/ContentPipeline/README.md` (ok)
- Inbound refs (7):
  - `.agents/roles/content-repurposer/role.md`
  - `.agents/roles/content-repurposer/skills/pipeline/_index.md`
  - `.agents/roles/content-repurposer/skills/pipeline/phase-map.md`
  - `retired Pi skill mirror/README.md`
  - `retired Pi skill mirror/content-repurposer-pipeline/SKILL.md`
  - `retired Pi skill mirror/content-repurposer/SKILL.md`
  - `Docs/ONBOARDING.md`

### `Tools/MixItUp/Api/README.md`
- Outbound refs (0):
  - None
- Inbound refs (1):
  - `README.md`

### `WORKING.md`
- Outbound refs (8):
  - backtick: `Actions/SHARED-CONSTANTS.md` → `Actions/SHARED-CONSTANTS.md` (ok)
  - backtick: `Creative/Art/` → `Creative/Art/README.md` (ok)
  - backtick: `Creative/Brand/BRAND-VOICE.md` → `Creative/Brand/BRAND-VOICE.md` (ok)
  - backtick: `Creative/` → `Creative/README.md` (ok)
  - backtick: `Docs/` → `Docs` (ok)
  - backtick: `Docs/AGENT-WORKFLOW.md` → `Docs/AGENT-WORKFLOW.md` (ok)
  - backtick: `Tools/` → `Tools/README.md` (ok)
  - backtick: `claude` → `claude.md` (ok)
- Inbound refs (8):
  - `.agents/ENTRY.md`
  - `.agents/_shared/conventions.md`
  - `.agents/_shared/coordination.md`
  - `.agents/_shared/project.md`
  - `AGENTS.md`
  - `CLAUDE.md`
  - `Docs/AGENT-WORKFLOW.md`
  - `Docs/ONBOARDING.md`
