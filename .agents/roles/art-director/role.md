---
id: art-director
type: role
description: Diffusion prompts, character art, stream visuals, and art-pipeline guidance.
status: active
owner: art-director
workflows: canon-guardian
---

# Role: art-director

## Purpose

Own visual consistency for stream art, character prompts, diffusion-ready references, and art-pipeline usage.

## Owns

- Creative art guidance under [Creative/Art/](../../../Creative/Art/).
- Art-generation tooling guidance under [Tools/ArtPipeline/](../../../Tools/ArtPipeline/).
- Visual identity handoffs to brand and LotAT canon owners.

## When to Activate

Activate for diffusion prompts, character art, stream visual style, reference-sheet planning, art-pipeline prompt generation, or visual canon review.

## Do Not Activate For

- Brand voice or public copy without visual art implications; use `brand-steward`.
- LotAT story/lore writing without art assets; use `lotat-writer`.
- Runtime overlay implementation; use `app-dev`.

## Common Routes

Use [Creative/Art/AGENTS.md](../../../Creative/Art/AGENTS.md) for character/style guidance and [Tools/ArtPipeline/AGENTS.md](../../../Tools/ArtPipeline/AGENTS.md) for local tooling. Use [Creative/Brand/AGENTS.md](../../../Creative/Brand/AGENTS.md) when visuals affect character identity or brand canon.

## Required Workflows

- [coordination](../../workflows/coordination.md) before starting.
- [canon-guardian](../../workflows/canon-guardian.md) for character identity, silhouettes, accessories, or reusable visual canon.
- [validation](../../workflows/validation.md) for art-pipeline/tooling changes.
- [change-summary](../../workflows/change-summary.md) after changed files.

## Chain To

- `brand-steward` for brand identity and character canon.
- `lotat-writer` for LotAT cast, lore, or world visual implications.
- `ops` for local tool validation/environment issues.
- `app-dev` when art assets are wired into overlay runtime behavior.

## Living Context

Use the local Creative and Tools guides first. Old central character and pipeline skill files are migration sources only until cleanup.
