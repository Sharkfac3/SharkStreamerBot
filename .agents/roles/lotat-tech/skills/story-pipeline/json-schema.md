# LotAT Story JSON Schema

## Top-Level Fields

| Field | Type | Required | Notes |
|---|---|---|---|
| `story_id` | string | yes | Unique identifier for this story |
| `title` | string | yes | Display title |
| `tone` | string | yes | Overall tone descriptor |
| `version` | string | yes | Schema version |
| `cast` | array | yes | Character names featured |
| `ship_sections_used` | array | yes | Ship sections that appear in this story |
| `commands_used` | array | yes | Chat commands used (must all be supported) |
| `nodes` | array | yes | All story nodes |

## Node Fields

| Field | Type | Required | Notes |
|---|---|---|---|
| `node_id` | string | yes | Unique within the story |
| `node_type` | string | yes | `"stage"` or `"ending"` |
| `ship_section` | string | yes | Location in the ship |
| `read_aloud` | string | yes | 1–4 sentences of narration |
| `crew_focus` | string | no | Which crew member is featured |
| `chaos_change` | number | no | How much the Chaos Meter moves |
| `dice_hook` | object | no | Optional dice mechanic |
| `commander_moment` | object | no | Optional commander interaction (1–2 per story max) |
| `choices` | array | stage nodes only | Exactly 2 choices |

## Choice Fields

| Field | Type | Required |
|---|---|---|
| `label` | string | yes |
| `next_node_id` | string | yes |

## Schema Rules

- Do not add top-level fields not listed above without updating both this doc and the engine
- Do not rename existing fields without a migration plan
- `choices` array must have **exactly 2 elements** on stage nodes
- All `next_node_id` values must reference valid `node_id` values within the same story file
- `read_aloud` must be 1–4 sentences (live stream pace constraint)
- Chaos Meter must escalate over the story arc — do not reset mid-story

## Authoritative Source

Schema is defined in `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`. This file is the reference summary — the story agent file is authoritative.
