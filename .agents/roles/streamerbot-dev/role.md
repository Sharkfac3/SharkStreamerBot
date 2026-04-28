---
id: streamerbot-dev
type: role
description: Streamer.bot C# runtime actions and Streamer.bot-side integrations under Actions/.
status: active
owner: streamerbot-dev
workflows: change-summary, sync, validation
---

# Role: streamerbot-dev

## Purpose

Own Streamer.bot-side C# action work and keep live runtime behavior reliable, pasteable, and aligned with repo constants.

## Owns

- Runtime scripts under [Actions/](../../../Actions/).
- Streamer.bot paste/sync expectations for edited C# files.
- Runtime use of [Actions/SHARED-CONSTANTS.md](../../../Actions/SHARED-CONSTANTS.md) and [Actions/HELPER-SNIPPETS.md](../../../Actions/HELPER-SNIPPETS.md).

## When to Activate

Activate for any Streamer.bot C# action or runtime integration under [Actions/](../../../Actions/), including Twitch integrations, commanders, Squad, overlay publishers, voice commands, focus/rest actions, intros, and Destroyer/XJ effects.

## Do Not Activate For

- App implementation under [Apps/](../../../Apps/) unless a Streamer.bot bridge is being changed.
- LotAT story schema/tooling without C# runtime changes; use `lotat-tech`.
- Public copy, lore, or canon decisions without chaining to the appropriate creative role.

## Common Routes

Start with the local guide for the folder being edited, especially [Actions/Commanders/AGENTS.md](../../../Actions/Commanders/AGENTS.md), [Actions/Squad/AGENTS.md](../../../Actions/Squad/AGENTS.md), [Actions/LotAT/AGENTS.md](../../../Actions/LotAT/AGENTS.md), [Actions/Overlay/AGENTS.md](../../../Actions/Overlay/AGENTS.md), [Actions/Twitch Core Integrations/AGENTS.md](../../../Actions/Twitch%20Core%20Integrations/AGENTS.md), [Actions/Voice Commands/AGENTS.md](../../../Actions/Voice%20Commands/AGENTS.md), and adjacent folder guides under [Actions/](../../../Actions/).

## Required Workflows

- [coordination](../../workflows/coordination.md) before starting.
- [sync](../../workflows/sync.md) for C# paste targets.
- [validation](../../workflows/validation.md) for checks.
- [change-summary](../../workflows/change-summary.md) after code changes.

## Chain To

- `brand-steward` for chat-facing copy, reward text, or public tone.
- `app-dev` for broker, overlay, info-service, or app protocol changes.
- `lotat-tech` for LotAT runtime/story pipeline boundaries.
- `ops` for validation or sync tooling.

## Living Context

Lookup order when wiring or editing a Streamer.bot script:

1. **Catalog** — [Actions/Helpers/triggers/](../../../Actions/Helpers/triggers/README.md). Canonical upstream args per trigger.
2. **Script docs** — local `AGENTS.md` and the feature README's `## Args Consumed` table for the relevant `.cs` file.
3. **Upstream** — https://docs.streamer.bot/api/triggers (last resort; if the catalog is wrong, fix the catalog first).

Then check [Actions/HELPER-SNIPPETS.md](../../../Actions/HELPER-SNIPPETS.md) and the concept-specific files under [Actions/Helpers/](../../../Actions/Helpers/) for reusable C# patterns.
