---
id: lotat-tech
type: role
description: LotAT technical pipeline, C# engine, JSON schema, and story runtime implementation.
status: active
owner: lotat-tech
workflows: change-summary, validation
---

# Role: lotat-tech

## Purpose

Own the technical contract that lets Legends of the ASCII Temple stories run safely in Streamer.bot and related tooling.

## Owns

- LotAT runtime engine behavior under [Actions/LotAT/](../../../Actions/LotAT/).
- LotAT story schema, validation, and viewer tooling under [Tools/LotAT/](../../../Tools/LotAT/).
- Technical boundaries between authored story JSON, Streamer.bot state, and overlay rendering.

## When to Activate

Activate for LotAT C# engine changes, story schema or validation changes, story pipeline/tooling changes, or technical debugging across the LotAT runtime path.

## Do Not Activate For

- Writing adventure prose, lore, cast, or reusable story canon; use `lotat-writer`.
- Generic Streamer.bot actions outside LotAT; use `streamerbot-dev`.
- Overlay implementation outside the LotAT contract; use `app-dev`.

## Common Routes

Use [Actions/LotAT/AGENTS.md](../../../Actions/LotAT/AGENTS.md) for runtime scripts and [Tools/LotAT/AGENTS.md](../../../Tools/LotAT/AGENTS.md) for story pipeline, schema, validation, and viewer work. Chain into [Creative/WorldBuilding/AGENTS.md](../../../Creative/WorldBuilding/AGENTS.md) when technical changes affect authored story content.

## Required Workflows

- [coordination](../../workflows/coordination.md) before starting.
- [validation](../../workflows/validation.md) for schema/tool/runtime checks.
- [sync](../../workflows/sync.md) when C# runtime scripts are edited.
- [change-summary](../../workflows/change-summary.md) after code or tooling changes.

## Chain To

- `streamerbot-dev` for Streamer.bot paste/sync implementation details.
- `lotat-writer` for authored content, lore, or adventure structure.
- `app-dev` for overlay protocol or rendering behavior.
- `brand-steward` for canon/public metaphor changes.
- `ops` for validator/tooling maintenance.

## Living Context

No dedicated living context is required for normal starts. Use local LotAT route guides first; old central LotAT skill files are migration sources only until cleanup.
