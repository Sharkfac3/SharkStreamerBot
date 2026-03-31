# Story Pipeline — Overview

## Contract Hierarchy

| Contract Level | File | Authority |
|---|---|---|
| **Authoritative story contract** | `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` | Defines valid authored story JSON. When field definitions conflict elsewhere, this file wins. |
| **Implementation reference / synced summary** | `.agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` | Tech-facing summary of the authoritative contract. Must be kept in sync with the authoritative contract. |
| **Franchise summary / canon reference** | `Creative/WorldBuilding/Franchises/StarshipShamples.md` | Explains setting, canon boundaries, and feature framing. Not a schema authority. |

## Governance

- `lotat-writer` owns story content, branching, pacing, authored story files, and pre-review validation that stories are safe to hand to the engine path
- `lotat-tech` owns story schema changes, command-contract changes, validation taxonomy, and engine-facing structure
- `brand-steward` must review canon, cast, and metaphor-level changes
- When schema or validation-taxonomy rules change, update the authoritative contract first, then sync every derived summary/reference doc in the same pass
- For v1, `!offering` remains out of scope for LotAT story/runtime design; do not infer story-contract support from the existing experimental offering script

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

Primary validation belongs to the story-writing / review pipeline, not the live engine.

V1 contract direction:
- engine-breaking story defects should be caught **before** a story is handed to the reviewer/runtime load path
- the live engine should assume it was given a vetted story and should **not** perform a second full schema/graph review at session start
- runtime preflight stays **minimal-safe**, not strict-comprehensive
- warnings are only worth carrying when the story can still run without risking engine stability; low-ROI editorial warnings can stay out of v1

### Hard-fatal before runtime handoff

Treat these as **hard-fatal** in story-generation / writer-side validation because they can make the engine unsafe, ambiguous, or unable to execute deterministically:
- malformed JSON / parse failure
- missing required top-level fields
- missing required node fields
- missing, empty, or unresolved `starting_node_id`
- duplicate `node_id`
- duplicate `choice_id`
- invalid `node_type`
- invalid authored decision commands in `choices[].command` or `commands_used`
- runtime-only commands such as `!join`, `!roll`, or commander commands appearing in authored story-choice fields
- `next_node_id` values that reference missing nodes
- graph defects that can strand progression or break node resolution
- stage nodes with zero choices in v1
- stage nodes with more than 2 choices in v1
- malformed ending nodes
- malformed commander-moment payloads
- malformed dice-hook payloads
- any related structure/graph issue that could break live execution

### Warning-only in v1

Warnings are optional and low priority unless they are useful to the writer and still allow a story to run safely.

Use warnings only for issues that do **not** threaten engine safety or session continuity.
If an issue could realistically break runtime execution, upgrade it to hard-fatal instead of downgrading it to a warning.

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

V1 engine-side preflight should stay minimal-safe:
- load exactly `Creative/WorldBuilding/Storylines/loaded/current-story.json`
- do not scan `ready/` directly
- do not perform a second full schema-review pass at session start
- do perform only the bare-minimum runtime load checks needed to avoid a broken live session (file exists, JSON parses, core runtime fields needed to begin exist)
- if those minimal runtime checks fail, abort before `join_open`, log verbosely, send a chat-safe fallback message, optionally trigger the generic Mix It Up failure alert, and leave the session inactive

> **V1 runtime source-of-truth note:** `Creative/WorldBuilding/Storylines/loaded/current-story.json` is the only runtime story file the engine should read. `ready/` remains operator/tooling staging, not an engine-scanned input directory.

## Sub-Skills

- `json-schema.md` — full schema field reference
