# Story Pipeline — Overview

## Flow

```
lotat-writer produces story JSON
        ↓
lotat-tech validates JSON against schema
        ↓
lotat-tech implements/updates C# engine to consume it
        ↓
streamerbot-dev deploys engine scripts to Actions/LotAT/
```

## Story File Locations

| Type | Path |
|---|---|
| Story agent prompt | `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` |
| Coding agent prompt | `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` |
| Franchise overview | `Creative/WorldBuilding/Franchises/StarshipShamples.md` |
| Draft stories (pre-review) | `Creative/WorldBuilding/Storylines/drafts/` |
| Handed-off stories (ready for engine) | `Creative/WorldBuilding/Storylines/ready/` |
| In-progress / experiments | `Creative/WorldBuilding/Experiments/` |

## Validation Before Engine Consumption

Before implementing engine changes for a new story, verify the story JSON:
- [ ] All required top-level fields present (`story_id`, `title`, `tone`, `version`, `cast`, `nodes[]`)
- [ ] All nodes have `node_id`, `node_type`, `ship_section`, `read_aloud`
- [ ] `choices[]` nodes have exactly 2 choices, each with `label` and `next_node_id`
- [ ] All `next_node_id` values reference valid node IDs in the file
- [ ] Only supported commands used (see `engine/commands.md`)
- [ ] No invented schema fields

> **Note:** Only consume stories from `ready/`. Do not process a story that has not been handed off by the operator via the story viewer.

## Sub-Skills

- `json-schema.md` — full schema field reference
