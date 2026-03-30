# Story Pipeline — Overview

## Contract Hierarchy

| Contract Level | File | Authority |
|---|---|---|
| **Authoritative story contract** | `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` | Defines valid authored story JSON. When field definitions conflict elsewhere, this file wins. |
| **Implementation reference / synced summary** | `.agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` | Tech-facing summary of the authoritative contract. Must be kept in sync with the authoritative contract. |
| **Franchise summary / canon reference** | `Creative/WorldBuilding/Franchises/StarshipShamples.md` | Explains setting, canon boundaries, and feature framing. Not a schema authority. |

## Governance

- `lotat-writer` owns story content, branching, pacing, and authored story files
- `lotat-tech` owns story schema changes, command-contract changes, and engine-facing structure
- `brand-steward` must review canon, cast, and metaphor-level changes
- When schema changes, update the authoritative contract first, then sync every derived summary/reference doc in the same pass

## Flow

```
lotat-writer produces story JSON
        ↓
story-generation / review tooling performs the main validation pass
        ↓
operator reviews the candidate story in the existing story viewer/tooling
        ↓
operator/story-viewer copies the approved story into loaded/current-story.json
        ↓
lotat-tech + streamerbot-dev implement C# engine to consume that single loaded runtime file
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
| Handed-off stories (ready for runtime loading) | `Creative/WorldBuilding/Storylines/ready/` |
| Canonical runtime story copy for engine consumption | `Creative/WorldBuilding/Storylines/loaded/current-story.json` |
| Finished stories | `Creative/WorldBuilding/Storylines/finished/` |
| In-progress / experiments | `Creative/WorldBuilding/Experiments/` |

## Validation Before Engine Consumption

Primary validation belongs to story generation and operator review tooling, not the live engine.

Before treating a story as ready for runtime use, verify the story JSON during story creation / review:
- [ ] All required top-level fields from the authoritative contract are present (`story_id`, `title`, `tone`, `version`, `summary`, `starting_ship_section`, `starting_node_id`, `cast`, `ship_sections_used`, `commands_used`, `nodes[]`)
- [ ] `starting_node_id` references a valid node in the file
- [ ] All nodes have `node_id`, `node_type`, `ship_section`, `title`, `read_aloud`, `sfx_hint`, `crew_focus`, `chaos`, `dice_hook`, `commander_moment`, `choices`, `tags`, and `end_state`
- [ ] `sfx_hint` exists on every node and uses either a string or `null`
- [ ] `crew_focus`, `dice_hook`, and `commander_moment` include all documented subkeys even when inactive
- [ ] Stage nodes use 1 or 2 choices in v1; ending nodes use `choices: []`
- [ ] No stage node uses 3 or more choices in v1
- [ ] Each choice has `choice_id`, `label`, `command`, `result_flavor`, and `next_node_id`
- [ ] All `next_node_id` values reference valid node IDs in the file
- [ ] Only supported authored decision commands are used in `choices[].command` / `commands_used` (see `engine/commands.md`)
- [ ] Runtime commands such as `!join` and `!roll` do not appear in story JSON fields
- [ ] Ending nodes keep `dice_hook.enabled: false`
- [ ] Enabled stage-node dice hooks include `roll_window_seconds` and a `success_threshold` from 1–90
- [ ] No invented schema fields outside the authoritative contract

V1 engine-side preflight should stay minimal:
- load exactly `Creative/WorldBuilding/Storylines/loaded/current-story.json`
- do not scan `ready/` directly
- do not perform a second full schema-review pass at session start
- do perform the bare-minimum runtime load checks needed to avoid a broken live session (file exists, JSON parses, core runtime fields needed to begin exist)
- if those minimal runtime checks fail, abort before `join_open`, log verbosely, send a chat-safe fallback message, and leave the session inactive

> **V1 runtime source-of-truth note:** `Creative/WorldBuilding/Storylines/loaded/current-story.json` is the only runtime story file the engine should read. `ready/` remains operator/tooling staging, not an engine-scanned input directory.

## Sub-Skills

- `json-schema.md` — full schema field reference
