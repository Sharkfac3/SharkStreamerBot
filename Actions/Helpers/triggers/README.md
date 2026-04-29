---
id: triggers-index
type: reference
description: Top-level index of the Streamer.bot trigger catalog — platform list, lookup-order rule, and coverage flags.
owner: streamerbot-dev
status: active
---

# Streamer.bot Trigger Catalog

## Why this exists

Repo-local mirror of every Streamer.bot trigger so agents stop fetching upstream nav before writing scripts. See [Projects/streamerbot-trigger-catalog/README.md](../../../Projects/streamerbot-trigger-catalog/README.md) for full project rationale and phase sequence.

## Lookup order

1. **Catalog** (`Actions/Helpers/triggers/<platform>/<subcategory>.md#<trigger>`) — canonical upstream args.
2. **Feature README + `.cs` header `## Args Consumed`** — what this script actually reads and how.
3. **Upstream docs** — source of truth for catalog content only. Do not bypass the catalog when wiring scripts; if the catalog is wrong, fix it first.

## Platforms

| Platform | README | Coverage |
|---|---|---|
| Core | [core/README.md](core/README.md) | stub |
| Custom | [custom/README.md](custom/README.md) | stub |
| Elgato | [elgato/README.md](elgato/README.md) | stub |
| Integrations | [integrations/README.md](integrations/README.md) | stub |
| Kick | [kick/README.md](kick/README.md) | stub |
| Meld Studio | [meld-studio/README.md](meld-studio/README.md) | stub |
| OBS Studio | [obs-studio/README.md](obs-studio/README.md) | stub |
| Streamlabs Desktop | [streamlabs-desktop/README.md](streamlabs-desktop/README.md) | stub |
| Twitch | [twitch/README.md](twitch/README.md) | partial |
| YouTube | [youtube/README.md](youtube/README.md) | stub |

## How to add a new trigger entry

See [Projects/streamerbot-trigger-catalog/README.md](../../../Projects/streamerbot-trigger-catalog/README.md) for file schemas, slug rules, and frontmatter conventions.
