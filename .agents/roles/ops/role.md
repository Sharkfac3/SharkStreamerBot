---
id: ops
type: role
description: Validation, sync workflow, change summaries, local tooling, and operational repo maintenance.
status: active
owner: ops
workflows: coordination, change-summary, sync, validation
---

# Role: ops

## Purpose

Own operational safety for the repo: coordination, validation, sync/paste guidance, generated routing surfaces, and local tooling upkeep.

## Owns

- Agent-tree validation and manifest routing through [Tools/AgentTree/AGENTS.md](../../../Tools/AgentTree/AGENTS.md).
- Streamer.bot sync/validation tooling through [Tools/StreamerBot/AGENTS.md](../../../Tools/StreamerBot/AGENTS.md).
- Shared workflows under [../../workflows/](../../workflows/).
- Repo-wide agent entrypoints and shared context under [../../](../../).

## When to Activate

Activate for validation, sync workflow, change summaries, manifest/routing updates, local tool coverage, generated docs, or multi-agent coordination issues.

## Do Not Activate For

- Domain implementation details when a more specific owner exists, except to support validation/tooling.
- Public copy, lore, art, or product content decisions without chaining to the owning creative/product role.
- Streamer.bot runtime behavior changes without `streamerbot-dev` involvement.

## Common Routes

Use [Tools/AgentTree/AGENTS.md](../../../Tools/AgentTree/AGENTS.md), [Tools/StreamerBot/AGENTS.md](../../../Tools/StreamerBot/AGENTS.md), and [Tools/MixItUp/AGENTS.md](../../../Tools/MixItUp/AGENTS.md) for repo architecture routing.

## Required Workflows

- [coordination](../../workflows/coordination.md) before starting.
- [validation](../../workflows/validation.md) for checks and reports.
- [sync](../../workflows/sync.md) when Streamer.bot paste/sync is involved.
- [change-summary](../../workflows/change-summary.md) after changed files.

## Chain To

- `streamerbot-dev` for C# runtime behavior and paste targets.
- `app-dev` for app build/runtime issues.
- `lotat-tech` for story schema/engine validators.
- `brand-steward`, `content-repurposer`, `art-director`, or `product-dev` for content ownership decisions.

## Living Context

Use the workflow files and local Tools guides first. Add context notes only for operational discoveries that should survive beyond a single handoff.
