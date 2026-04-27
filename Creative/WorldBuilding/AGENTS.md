---
id: creative-worldbuilding
type: domain-route
description: LotAT adventures, Starship Shamples lore, franchise references, settings, storylines, and universe rules.
owner: lotat-writer
secondaryOwners:
  - lotat-tech
  - brand-steward
  - art-director
workflows:
  - canon-guardian
  - validation
status: active
---

# Creative/WorldBuilding — Agent Guide

## Purpose

[Creative/WorldBuilding/](./) contains LotAT and Starship Shamples lore, story systems, franchise notes, settings, storylines, and exploratory worldbuilding prompts.

Use this folder for adventure design, authored story content, franchise continuity, world rules, cast usage, and story-facing canon. This guide migrates the former LotAT writer adventure, franchise, and universe routing into the local worldbuilding entrypoint.

## When to Activate

Use this guide when working on:

- LotAT adventure planning or story JSON authoring
- storylines under [Creative/WorldBuilding/Storylines/](Storylines/)
- franchise references under [Creative/WorldBuilding/Franchises/](Franchises/)
- story or game agents under [Creative/WorldBuilding/Agents/](Agents/)
- story contract or experimental prompt docs under [Creative/WorldBuilding/Experiments/](Experiments/)
- settings, ship sections, regions of space, recurring locations, or world rules
- canon review for LotAT story content
- build-specific story planning where the real stream build is mirrored by Starship Shamples

Do not activate this guide for C# engine implementation; use [Actions/LotAT/AGENTS.md](../../Actions/LotAT/AGENTS.md). Use [Tools/LotAT/AGENTS.md](../../Tools/LotAT/AGENTS.md) for story validator/tooling work.

## Primary Owner

Primary owner: `lotat-writer`.

`lotat-writer` owns narrative content, branching adventure design, character usage in story scenes, franchise continuity from a writing perspective, and pre-handoff story safety checks.

## Secondary Owners / Chain To

| Role | Chain when |
|---|---|
| `lotat-tech` | Story schema, command contract, JSON validation taxonomy, engine assumptions, or runtime behavior changes are involved. |
| `brand-steward` | A story changes character identity, metaphor meaning, permanent canon, or public brand interpretation. |
| `art-director` | Story content requires character art, scene art, thumbnails, overlays, or visual references. |
| `content-repurposer` | A story arc is being planned for clip-worthy short-form moments. |
| `ops` | Validation, handoff formatting, or agent-tree maintenance is needed. |

Run the [canon-guardian workflow](../../.agents/workflows/canon-guardian.md) for new or changed recurring lore, named characters, permanent locations, metaphor changes, franchise-level setting changes, or story content that could escape a single adventure and become reusable canon.

## Required Reading

Read these first for worldbuilding work:

1. [Creative/WorldBuilding/README.md](README.md) — folder scope and local structure.
2. [Creative/Brand/BRAND-IDENTITY.md](../Brand/BRAND-IDENTITY.md) — brand metaphor and values.
3. [Creative/Brand/BRAND-VOICE.md](../Brand/BRAND-VOICE.md) — narration and public-copy tone.
4. [Creative/Brand/CHARACTER-CODEX.md](../Brand/CHARACTER-CODEX.md) — authoritative cast identities and metaphor roles.
5. [Creative/WorldBuilding/Franchises/StarshipShamples.md](Franchises/StarshipShamples.md) — franchise summary and canon baseline.
6. [Creative/WorldBuilding/Agents/D&D-Agent.md](Agents/D&D-Agent.md) — game design, mechanics, tone, ship sections, and commands.
7. [Docs/Architecture/lotat-contract.md](../../Docs/Architecture/lotat-contract.md) — shared LotAT contract across story, runtime, tooling, and overlay domains.
8. [Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md](Experiments/StarshipShamples-story-agent.md) — authoritative authored story contract.
9. [Tools/LotAT/AGENTS.md](../../Tools/LotAT/AGENTS.md) when story validation or tooling is needed.
10. [Actions/LotAT/AGENTS.md](../../Actions/LotAT/AGENTS.md) when runtime engine behavior matters.

## Local Workflow

1. Classify the work as adventure, franchise, universe/cast/rules, setting, story arc, or experiment.
2. For adventures, use the authoritative story contract before finalizing structure and [Docs/Architecture/lotat-contract.md](../../Docs/Architecture/lotat-contract.md) for shared runtime/tooling/presentation boundaries. Do not restate or redefine shared contract facts here.
3. Treat adventures as live-stream content:
   - read-aloud narration should be one to four sentences
   - most nodes should be two or three sentences
   - choices should be short, action-oriented, and plausible
   - high chaos should tighten pacing
   - endings should reflect final chaos state
4. Build a complete adventure arc:
   - a clear starting node
   - roughly twelve or more stage nodes for a full mission
   - one or two choices per stage node in v1, usually two for contrast
   - multiple distinct endings
   - Chaos Meter escalation across the arc
   - at least one Pedro makes it worse moment
   - at least one valid spectacular-failure ending
5. Respect the runtime/story boundary documented in the shared LotAT contract. Story-authored choices must use only supported authored decision commands from the authoritative story contract.
6. Preserve cast canon. The cast is fixed; prefer existing characters over new named cast members. Commander moments are rare and must be personality-specific.
7. For franchise work, distinguish local adventure flavor from reusable LotAT lore and franchise-level canon. Single-gimmick regions and mission starting points are usually safe as local flavor; new permanent locations, mechanics, or named cast require escalation.
8. For build-specific story planning, map the real build problem to a Starship Shamples mission beat only when it strengthens the stream. The best stories mirror the actual build chaos without requiring every story to be literal.

## Validation

For worldbuilding-doc or local agent-guide changes, run:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-09-validator.failures.txt
```

For story content, validate before handoff using the authoritative story contract and the shared [LotAT contract](../../Docs/Architecture/lotat-contract.md). At minimum, confirm graph integrity, supported commands, valid stage/ending shapes, supported dice/commander structures, top-level cast usage, and canon classification.

Use [Tools/LotAT/AGENTS.md](../../Tools/LotAT/AGENTS.md) for tool commands and schema validation mechanics.

## Boundaries / Out of Scope

Do not use this folder to:

- implement C# engine changes
- change the story schema, command contract, or validation taxonomy without `lotat-tech`
- infer approved runtime behavior from experimental scripts
- use unsupported story fields or command aliases
- add new named cast members without operator approval
- make one-off flavor look like permanent franchise canon
- generate art assets directly instead of chaining to [Creative/Art/AGENTS.md](../Art/AGENTS.md)
- write campaign/platform copy without [Creative/Marketing/AGENTS.md](../Marketing/AGENTS.md)

## Handoff Notes

Worldbuilding handoffs should include:

- content type: adventure, setting, franchise note, story arc, experiment, or canon review
- required source docs read
- story contract or validation status when authored JSON is involved
- canon classification: local flavor, reusable LotAT lore, or franchise-level canon
- [canon-guardian](../../.agents/workflows/canon-guardian.md) decision or escalation status
- chain-to requirements for `lotat-tech`, `brand-steward`, `art-director`, or `content-repurposer`

Canon-sensitive content requiring operator review includes new named cast members, changes to character personalities or metaphor roles, new permanent ship sections or locations, new story-authored mechanics, command-contract changes, or anything requiring updates to [Creative/Brand/CHARACTER-CODEX.md](../Brand/CHARACTER-CODEX.md), [Creative/WorldBuilding/Franchises/StarshipShamples.md](Franchises/StarshipShamples.md), or [Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md](Experiments/StarshipShamples-story-agent.md).
