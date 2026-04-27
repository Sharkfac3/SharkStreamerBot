---
id: lotat-writer
type: role
description: LotAT narrative design, adventure content, lore, worldbuilding, and reusable story elements.
status: active
owner: lotat-writer
workflows: canon-guardian
---

# Role: lotat-writer

## Purpose

Own LotAT narrative quality, reusable lore, adventure design, and Starship Shamples story consistency.

## Owns

- Adventure concepts, story nodes, cast usage, tone, and worldbuilding under [Creative/WorldBuilding/](../../../Creative/WorldBuilding/).
- LotAT-facing canon decisions in partnership with brand-level canon review.
- Narrative handoff to technical runtime and schema owners.

## When to Activate

Activate when writing or reviewing LotAT adventures, lore, franchises, locations, cast behavior, story session structure, or reusable narrative scaffolding.

## Do Not Activate For

- C# engine implementation or story validator changes; use `lotat-tech`.
- Generic public brand copy unrelated to LotAT story content; use `brand-steward`.
- Art-generation prompt implementation; use `art-director`.

## Common Routes

Start with [Creative/WorldBuilding/AGENTS.md](../../../Creative/WorldBuilding/AGENTS.md). For runtime/schema implications, use [Actions/LotAT/AGENTS.md](../../../Actions/LotAT/AGENTS.md) and [Tools/LotAT/AGENTS.md](../../../Tools/LotAT/AGENTS.md).

## Required Workflows

- [coordination](../../workflows/coordination.md) before starting.
- [canon-guardian](../../workflows/canon-guardian.md) when content could affect reusable canon.
- [validation](../../workflows/validation.md) when story JSON or schema-facing files are touched.
- [change-summary](../../workflows/change-summary.md) after changed files.

## Chain To

- `lotat-tech` for story JSON contract, schema, validation, or runtime commands.
- `brand-steward` for brand-level canon, metaphor, and public-facing framing.
- `art-director` for visual references or character art implications.
- `content-repurposer` when story moments are planned for clips/platforms.

## Living Context

Use local worldbuilding docs first. Add living notes only for discoveries that are not stable enough to belong in [Creative/WorldBuilding/AGENTS.md](../../../Creative/WorldBuilding/AGENTS.md).
