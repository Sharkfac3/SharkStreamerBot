---
id: creative-art
type: domain-route
description: Creative art assets, character references, art prompts, approved visuals, and stream visual style guidance.
owner: art-director
secondaryOwners:
  - brand-steward
  - lotat-writer
workflows:
  - canon-guardian
status: active
---

# Creative/Art — Agent Guide

## Purpose

[Creative/Art/](./) contains visual canon, art-agent prompts, art projects, approved assets, and references for Starship Shamples stream visuals.

Use this folder for character art direction, stream visual style, prompt scaffolding, approved asset storage, and visual references. The local art guide preserves the former art-director character and stream-style routing while keeping the source visual canon beside the creative assets.

## When to Activate

Use this guide when working on:

- character art agents under [Creative/Art/Agents/](Agents/)
- stream style prompts, visual references, and visual identity notes
- scoped art work under [Creative/Art/Projects/](Projects/)
- approved or final assets under [Creative/Art/Assets/](Assets/)
- art references under [Creative/Art/References/](References/)
- visual changes that affect Captain Stretch, The Director, Water Wizard, Squad characters, or LotAT visuals

Do not activate this guide for art-pipeline implementation work; use [Tools/ArtPipeline/AGENTS.md](../../Tools/ArtPipeline/AGENTS.md) for tooling. Do not author lore or story beats here unless they exist to support an art asset.

## Primary Owner

Primary owner: `art-director`.

`art-director` owns visual canon interpretation, prompt quality, asset readability, final asset approval, and consistency with the shared stream style.

## Secondary Owners / Chain To

| Role | Chain when |
|---|---|
| `brand-steward` | A visual change affects character identity, public brand identity, metaphor meaning, or franchise-level canon. |
| `lotat-writer` | Art depends on adventure lore, ship locations, story scenes, or reusable LotAT world details. |
| `ops` | Local validation, handoff formatting, or agent-tree maintenance is needed. |
| `streamerbot-dev` | Approved art must be wired into a Streamer.bot action or OBS/source behavior. |

Run the [canon-guardian workflow](../../.agents/workflows/canon-guardian.md) before approving new or changed visual canon for named characters, permanent ship locations, recurring factions, or any asset that could change how the franchise is understood.

## Required Reading

Read these first for art work:

1. [Creative/Art/README.md](README.md) — folder scope and local structure.
2. [Creative/Art/Agents/stream-style-art-agent.md](Agents/stream-style-art-agent.md) — foundational visual style for all stream assets.
3. [Creative/Brand/CHARACTER-CODEX.md](../Brand/CHARACTER-CODEX.md) — canonical character identities, personality, appearance, and metaphor roles.
4. [Creative/Brand/BRAND-IDENTITY.md](../Brand/BRAND-IDENTITY.md) — brand metaphor and values behind all visual decisions.
5. Character-specific agent file when generating a named commander:
   - [Captain Stretch art agent](Agents/captain-stretch-art-agent.md)
   - [The Director art agent](Agents/the-director-art-agent.md)
   - [Water Wizard art agent](Agents/water-wizard-art-agent.md)
6. [Creative/WorldBuilding/Franchises/StarshipShamples.md](../WorldBuilding/Franchises/StarshipShamples.md) when art depends on LotAT setting or franchise scope.

## Local Workflow

1. Classify the requested asset: overlay, emote, thumbnail, banner, character sheet, panel, environmental asset, UI element, or reference.
2. Load the shared stream style before any character-specific prompt. All stream visuals use anime illustration, clean expressive linework, anime cel shading, high contrast, and strong silhouettes.
3. Load the relevant character source:
   - Captain Stretch: humanoid shrimp captain, upright posture, naval uniform, crustacean features, command authority.
   - The Director: cartoon toad executive, four eyes with different pupil shapes, calm and supportive, never cinema-themed.
   - Water Wizard: middle-aged or elderly hydration wizard, silver hair and beard, deep teal cloak, glowing blue gemstone pendant, water magic.
   - Squad members: use the [Character Codex](../Brand/CHARACTER-CODEX.md) until dedicated art agents exist.
4. Keep assets readable for stream use. Emotes must read at very small sizes; overlays must work over varied backgrounds; thumbnails and banners need high contrast and strong composition.
5. Use [Tools/ArtPipeline/AGENTS.md](../../Tools/ArtPipeline/AGENTS.md) when the task needs manifests, generation, review, or publishing mechanics.
6. Store only approved/final outputs in [Creative/Art/Assets/](Assets/). Use [Creative/Art/Projects/](Projects/) for scoped work-in-progress notes and prompt records.

## Validation

For art-agent or prompt-guide changes:

```bash
python3 Tools/AgentTree/validate.py
```

For pipeline-backed production runs, follow [Tools/ArtPipeline/AGENTS.md](../../Tools/ArtPipeline/AGENTS.md) and use its dry-run smoke tests before any real generation or publish step.

For canon-sensitive visual changes, include a canon review note from the [canon-guardian workflow](../../.agents/workflows/canon-guardian.md) in the handoff.

## Boundaries / Out of Scope

Do not use this folder to:

- invent new named cast members without operator approval
- change character personality, metaphor role, or franchise canon through art alone
- store local generation caches or unreviewed pipeline output as approved assets
- implement art tooling code
- implement Streamer.bot runtime behavior
- write LotAT adventure content unless the work is a visual reference for a story

## Handoff Notes

Art handoffs should include:

- asset type and intended use
- source character/style files read
- output location or proposed output location
- canon classification: local visual variant, reusable visual canon, or franchise-level change
- whether [canon-guardian](../../.agents/workflows/canon-guardian.md) review is required or completed
- any runtime wiring follow-up for `streamerbot-dev` or tool follow-up for `ops`

Canon-sensitive items for operator review include new named characters, permanent ship/location visuals, changes to commander silhouettes or required accessories, and any visual design that conflicts with the [Character Codex](../Brand/CHARACTER-CODEX.md).
