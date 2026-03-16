---
name: creative-worldbuilding
description: Lore, canon, and CYOA story generation — franchise conventions, cast canon, JSON story schema, and separation of story vs. engine concerns for Starship Shamples and future franchises.
---

# Creative WorldBuilding

## When to Load

Load this skill for any task involving:
- Lore building or canon decisions
- CYOA / branching story generation
- Franchise development or narrative continuity
- Character backstory or personality
- Story engine architecture (C# implementation of a story system)

For **story engine C# code**, also chain `streamerbot-scripting` after this skill.

## Agent Files

| Agent | File | Purpose |
|---|---|---|
| Game Design / Mechanics | `Creative/WorldBuilding/Agents/D&D-Agent.md` | Starship Shamples rules — dice system, Chaos Meter, voting, commands, tone |
| Story Content Agent | `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` | Produces structured JSON story files (narrative layer) |
| Technical / Engine Agent | `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` | Consumes story JSON, produces C# for Streamer.bot (engine layer) |

Always read the relevant agent file(s) before generating content. Do not invent mechanics or schema fields not defined there.

## Directory Structure

| Directory | Contents |
|---|---|
| `Creative/WorldBuilding/Agents/` | Agent prompt files (game design, tone, mechanics) |
| `Creative/WorldBuilding/Franchises/` | Franchise-level overviews (scope, cast, canon summary) |
| `Creative/WorldBuilding/Settings/` | World / environment docs (locations, lore, rules of the universe) |
| `Creative/WorldBuilding/Storylines/` | Completed story files (final JSON + operator notes) |
| `Creative/WorldBuilding/Experiments/` | Draft agents, proof-of-concept work, in-progress story tools |

## Cast Canon

The cast is fixed. Do **not** invent new named cast members without explicit operator approval.

**Commanders** (3 active slots):
- Water Wizard
- Captain Stretch
- The Director

**Squad**:
- Pedro the Raccoon (engineering, chaos agent)
- Toothless the Dragon (security)
- Duck the Duck (The Bar)
- Clone the Clone Trooper (wildcard)

## Starship Shamples — Story Rules

### JSON Schema Contract

All story content must comply with the shared schema defined in `StarshipShamples-story-agent.md`. Do not add top-level fields or rename existing ones. The technical agent reads this schema directly — schema drift breaks the engine.

Key schema elements:
- `story_id`, `title`, `tone`, `version`, `cast`, `ship_sections_used`, `commands_used` (top-level metadata)
- `nodes[]` — each node has: `node_id`, `node_type` (`stage` or `ending`), `ship_section`, `read_aloud`, `crew_focus`, `chaos_change`, and optional `dice_hook`, `commander_moment`, `choices[]`
- `choices[]` — each choice has: `label`, `next_node_id`

### Supported Commands

Only use commands that are currently implemented. Do not invent new commands without a corresponding engine update.

`!scan` `!target` `!analyze` `!reroute` `!deploy` `!contain` `!inspect` `!drink` `!simulate`

### Chaos Meter Rules

- Starts low, escalates over the story arc.
- Do not reset the Chaos Meter mid-story.
- Terminal nodes (endings) should reflect the final chaos level in tone.

### Stage / Story Constraints

- Stage narration: 1–4 sentences. Keep it fast — this is live stream content.
- Minimum story length: 12 stages.
- Each stage offers exactly 2 choices.
- Branching variety: avoid linear chains; multiple paths should lead to unique outcomes.
- Commander moments are rare (1–2 per story max).
- Dice hooks add tension — use sparingly, not as a default mechanic.

## Separation of Concerns

**Story Agent** (content layer) and **Technical Agent** (engine layer) are separate invocations:

| Layer | Produces | Consumes |
|---|---|---|
| Story Agent | Narrative structure + JSON story file | Game design agent file |
| Technical Agent | C# Streamer.bot actions | Story JSON from Story Agent |

Do not mix concerns in a single output. If the task is "write a new story AND implement it," complete the story JSON first, then chain to `streamerbot-scripting` for the C# implementation.

## Skill Chain for Full Story Pipeline

```
creative-worldbuilding → (story JSON complete) → streamerbot-scripting + change-summary
```
