# Franchise: Starship Shamples

## Overview

Starship Shamples is the flagship franchise of this brand — a live, Twitch chat-controlled spaceship adventure game that plays out during real-world build streams. It is simultaneously an interactive game, an entertainment layer, and a metaphor for the neurodivergent creative experience.

> **Authority note:** This file is a franchise summary and canon reference. It is **not** the authoritative story JSON schema. For valid authored story-file structure, use `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`.

See `Creative/Brand/BRAND-IDENTITY.md` for the full brand context, including the metaphor explained in detail.

## Franchise Scope

| Layer | Description |
|---|---|
| **Interactive game** | A chat-driven CYOA/D&D hybrid where Twitch chat collectively plays the crew and makes mission decisions |
| **Entertainment layer** | Runs on stream alongside or between build sessions; keeps chat engaged during slow phases of a build |
| **Brand metaphor** | The ship, crew, and chaos represent the neurodivergent mind building something real |
| **Community ritual** | The cast, running jokes, and failure-forward design create shared vocabulary and community bonding |

## Canon Summary

**The Ship:** Starship Shamples — a totally legally distinct exploration vessel traveling strange and poorly understood regions of space.

**The Crew:** Twitch chat itself. The audience is the crew. Individual chatters vote; the outcome belongs to "the crew" collectively.

**The Commanders:** Three special roles played by designated chat members. Active slots: Water Wizard, Captain Stretch, The Director. See `Creative/Brand/CHARACTER-CODEX.md` for full profiles.

**The Squad:** Recurring NPCs who cause or complicate problems. Pedro the Raccoon, Toothless the Dragon, Duck the Duck, Clone the Clone Trooper. See `Creative/Brand/CHARACTER-CODEX.md` for full profiles.

**The Cast is fixed.** Do not add new named cast members without operator approval. See `Creative/Brand/CHARACTER-CODEX.md` — canon guardian questions apply.

## Tone

Absurd, chaotic, slightly dramatic, fast-paced, humorous. Failure-forward. Most missions end in failure; this is intentional and celebrated. Replayability is the design goal.

For detailed tone and language guidance: `Creative/Brand/BRAND-VOICE.md`

## Mechanics

Managed by the game design agent: `Creative/WorldBuilding/Agents/D&D-Agent.md`

Core systems:
- 100-sided dice rolls for tension
- Chaos Meter (escalates over the story; never resets mid-story)
- Chat voting (2 choices per stage)
- Commander moments (rare — 1–2 per story max)
- Branching outcomes (success, failure, partial survival, bizarre unresolved, total destruction)

## Story Content

**Authoritative story contract:** `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`
**Implementation reference:** `.agents/roles/lotat-tech/skills/story-pipeline/json-schema.md`
**Implemented by:** `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md`
**Story files live in:** `Creative/WorldBuilding/Storylines/`

Ownership:
- `lotat-writer` owns story content and authored story files
- `lotat-tech` owns story schema and command-contract changes
- `brand-steward` reviews canon, cast, and metaphor changes

Stories are modular data — the game engine is separate from story content. A new story does not require new code unless it introduces a new mechanic. New mechanics require engine changes before story content can use them. If the schema changes, the authoritative story contract must be updated first and all summary/reference docs must be synced in the same pass.

## Skill Routing

| Task | Skills |
|---|---|
| Write a new story | `content-strategy` (if build-tied) → `creative-worldbuilding` → `brand-canon-guardian` (review) |
| Audit a story for canon | `brand-canon-guardian` |
| Implement story engine (C#) | `creative-worldbuilding` → `streamerbot-scripting` → `change-summary` |
| Add new character (requires operator approval first) | `brand-canon-guardian` → `creative-worldbuilding` → `creative-art` |

## Expansion Boundaries

**Currently active:**
- Core ship (all 6 sections)
- Full cast (Commanders + Squad)
- Chaos Meter
- Dice hooks
- Commander moments

**Planned but not yet implemented:**
- Landing party system

**Not yet defined:**
- Additional franchises within this world
- Multi-franchise crossovers
- Non-Twitch distribution of story content (clips, VODs, standalone)

## Key Files

| File | Purpose |
|---|---|
| `Creative/Brand/BRAND-IDENTITY.md` | Brand foundation and metaphor |
| `Creative/Brand/CHARACTER-CODEX.md` | Canonical character reference |
| `Creative/WorldBuilding/Agents/D&D-Agent.md` | Game mechanics |
| `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` | Story generation rules and schema |
| `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` | C# engine agent |
| `Creative/WorldBuilding/Storylines/` | Completed story files |
