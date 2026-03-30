# LotAT Story JSON Schema

## Authority Status

- **Authoritative contract:** `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`
- **This file:** tech-facing synced summary of that contract
- **Rule:** if this file conflicts with the authoritative contract, this file must be updated to match the authoritative contract

`lotat-tech` owns schema and command-contract changes. `lotat-writer` owns story content written inside the contract. Canon, cast, or metaphor changes escalate to `brand-steward`.

## Sync Rule

When the story schema or command contract changes:
1. update `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` first
2. update this file in the same pass
3. check writer-facing guidance for stale field assumptions
4. escalate to `brand-steward` if canon, cast, or metaphor meaning changed

Do not let this summary become a competing schema authority.
Do not treat runtime-session documentation in `../engine/` as permission to invent authored JSON fields for join flow, participant rosters, or vote auto-close behavior.

## Top-Level Fields

Use this exact top-level shape:

```json
{
  "story_id": "string",
  "title": "string",
  "tone": "absurd-chaotic-humorous",
  "version": "1.0",
  "summary": "short one paragraph pitch",
  "starting_ship_section": "Command Deck",
  "starting_node_id": "node_001",
  "cast": {
    "commanders_used": ["The Water Wizard"],
    "squad_members_used": ["Pedro the Raccoon", "Duck the Duck"]
  },
  "ship_sections_used": ["Command Deck", "Engineering", "The Bar"],
  "commands_used": ["!scan", "!reroute", "!drink"],
  "nodes": []
}
```

| Field | Type | Required | Notes |
|---|---|---|---|
| `story_id` | string | yes | Unique identifier for the story |
| `title` | string | yes | Story display title |
| `tone` | string | yes | Current contract literal/example is `"absurd-chaotic-humorous"` |
| `version` | string | yes | Story/schema version string |
| `summary` | string | yes | Short one-paragraph pitch |
| `starting_ship_section` | string | yes | Initial ship location for the story |
| `starting_node_id` | string | yes | Must reference a valid `node_id` in `nodes[]` |
| `cast` | object | yes | Must be an object with `commanders_used` and `squad_members_used` arrays |
| `ship_sections_used` | array | yes | Ship sections that appear in the story |
| `commands_used` | array | yes | Commands used anywhere in the story; must all be engine-supported |
| `nodes` | array | yes | All stage and ending nodes |

## `cast`

| Field | Type | Required | Notes |
|---|---|---|---|
| `commanders_used` | array of strings | yes | Known commanders featured in the story |
| `squad_members_used` | array of strings | yes | Known squad members featured in the story |

`cast` is **not** a freeform array and is **not** a loose metadata blob. It is a fixed object with those two arrays.

## Node Shape

Each node must use this shape:

```json
{
  "node_id": "node_001",
  "node_type": "stage",
  "ship_section": "Command Deck",
  "title": "Short internal stage title",
  "read_aloud": "One short stream-friendly narration block.",
  "sfx_hint": null,
  "crew_focus": {
    "commander": null,
    "squad_member": "Pedro the Raccoon"
  },
  "chaos": {
    "delta": 0
  },
  "dice_hook": {
    "enabled": false,
    "purpose": null,
    "roll_window_seconds": null,
    "success_threshold": null,
    "failure_text": null,
    "success_text": null
  },
  "commander_moment": {
    "enabled": false,
    "commander": null,
    "prompt": null,
    "window_seconds": null,
    "success_text": null
  },
  "choices": [
    {
      "choice_id": "node_001_a",
      "label": "Scan the anomaly",
      "command": "!scan",
      "result_flavor": "The bridge lights dim as the sensors lock on.",
      "next_node_id": "node_002"
    },
    {
      "choice_id": "node_001_b",
      "label": "Reroute power to the dish",
      "command": "!reroute",
      "result_flavor": "Pedro starts touching wires immediately.",
      "next_node_id": "node_003"
    }
  ],
  "tags": ["opening", "anomaly"],
  "end_state": null
}
```

| Field | Type | Required | Notes |
|---|---|---|---|
| `node_id` | string | yes | Unique within the story |
| `node_type` | string | yes | `"stage"` or `"ending"` |
| `ship_section` | string | yes | Ship location for the node |
| `title` | string | yes | Required short internal stage/ending title |
| `read_aloud` | string | yes | Required narration block |
| `sfx_hint` | string or null | yes | Required on every node in v1; use `null` when no production hint is needed |
| `crew_focus` | object | yes | Fixed object with `commander` and `squad_member` keys; both keys must always exist |
| `chaos` | object | yes | Per-node chaos object; see below |
| `dice_hook` | object | yes | Structured dice hook object; all documented subkeys must always exist |
| `commander_moment` | object | yes | Structured commander interruption object; all documented subkeys must always exist |
| `choices` | array | yes | Stage nodes must have 1 or 2 choices in v1; ending nodes use `[]` |
| `tags` | array of strings | yes | Lightweight categorization tags |
| `end_state` | string or null | yes | `null` for stage nodes; ending classification for ending nodes |

## `crew_focus`

| Field | Type | Required | Notes |
|---|---|---|---|
| `commander` | string or null | yes | Focused commander for the node, if any |
| `squad_member` | string or null | yes | Focused squad member for the node, if any |

`crew_focus` is a fixed object. It is not an array, list, or arbitrary cast payload.

## `chaos`

| Field | Type | Required | Notes |
|---|---|---|---|
| `delta` | number | yes | Non-negative chaos increase applied when entering the node |

The contract uses a `chaos` object with a single `delta` field in v1. Do **not** replace it with a flat `chaos_change` field.

## `dice_hook`

| Field | Type | Required | Notes |
|---|---|---|---|
| `enabled` | boolean | yes | Whether a dice hook is active |
| `purpose` | string or null | yes | What the roll is for |
| `roll_window_seconds` | number or null | yes | Whole-second pre-vote `!roll` window length when enabled |
| `success_threshold` | number or null | yes | Success target when enabled; valid authored range is 1–90 and success is `roll >= success_threshold` |
| `failure_text` | string or null | yes | Failure outcome text read aloud by the operator |
| `success_text` | string or null | yes | Success outcome text read aloud by the operator |

## `commander_moment`

| Field | Type | Required | Notes |
|---|---|---|---|
| `enabled` | boolean | yes | Whether a commander moment is active |
| `commander` | string or null | yes | Which commander owns the moment |
| `prompt` | string or null | yes | Chat/operator-facing call-to-action text for the commander window |
| `window_seconds` | number or null | yes | Whole-second commander-input window length when enabled |
| `success_text` | string or null | yes | Operator-read flavor text emitted on successful commander input |

## Choice Shape

Each choice must use this shape:

```json
{
  "choice_id": "node_001_a",
  "label": "Scan the anomaly",
  "command": "!scan",
  "result_flavor": "The bridge lights dim as the sensors lock on.",
  "next_node_id": "node_002"
}
```

| Field | Type | Required | Notes |
|---|---|---|---|
| `choice_id` | string | yes | Unique choice identifier |
| `label` | string | yes | Chat-facing choice label |
| `command` | string | yes | Required on every choice; must be engine-supported |
| `result_flavor` | string | yes | Short flavor text for the selected result |
| `next_node_id` | string | yes | Must reference a valid downstream node |

## Ending Node Expectations

Ending nodes must use this shape:

```json
{
  "node_id": "node_999",
  "node_type": "ending",
  "ship_section": "Engineering",
  "title": "Catastrophic Pretzel Collapse",
  "read_aloud": "The ship survives technically, but only as a franchise location.",
  "sfx_hint": "alarm_then_cash_register",
  "crew_focus": {
    "commander": null,
    "squad_member": "Duck the Duck"
  },
  "chaos": {
    "delta": 1
  },
  "dice_hook": {
    "enabled": false,
    "purpose": null,
    "roll_window_seconds": null,
    "success_threshold": null,
    "failure_text": null,
    "success_text": null
  },
  "commander_moment": {
    "enabled": false,
    "commander": null,
    "prompt": null,
    "window_seconds": null,
    "success_text": null
  },
  "choices": [],
  "tags": ["ending", "failure"],
  "end_state": "failure"
}
```

Stage vs ending expectations:
- stage nodes use `node_type: "stage"`
- stage nodes must present 1 or 2 choices in v1
- stage nodes with 3 or more choices are invalid in v1
- stage nodes use `end_state: null`
- ending nodes use `node_type: "ending"`
- ending nodes must use `choices: []`
- ending nodes must use an `end_state` of `"success"`, `"partial"`, or `"failure"`
- ending outcome classification is authored for the final node only and should remain hidden from viewers until that ending is reached in play

## Command Restrictions

Story JSON may only use authored decision commands that exist in `.agents/roles/lotat-tech/skills/engine/commands.md`.

Currently supported authored decision commands:
- `!scan`
- `!target`
- `!analyze`
- `!reroute`
- `!deploy`
- `!contain`
- `!inspect`
- `!drink`
- `!simulate`

Runtime-only session commands:
- `!join` — used by the engine during the session join phase; not valid in `choices[].command` and not listed in `commands_used`
- `!roll` — used by the engine only during an active node dice-roll window; not valid in `choices[].command` and not listed in `commands_used`
- `!stretch` / `!shrimp` — existing Captain Stretch commands used by the engine only during an active Captain Stretch commander moment; not valid in `choices[].command` and not listed in `commands_used`
- `!hydrate` / `!orb` — existing Water Wizard commands used by the engine only during an active Water Wizard commander moment; not valid in `choices[].command` and not listed in `commands_used`
- `!checkchat` / `!toad` — existing The Director commands used by the engine only during an active The Director commander moment; not valid in `choices[].command` and not listed in `commands_used`

Rules:
- do not invent new commands in `choices[].command`
- every command listed in `commands_used` must come from the authored decision-command list above
- runtime commands such as `!join`, `!roll`, and commander-input commands (`!stretch`, `!shrimp`, `!hydrate`, `!orb`, `!checkchat`, `!toad`) are engine behavior, not story schema content
- if a new authored decision command is added to the engine, update the engine command doc, the authoritative story contract, and this summary together
- if a new runtime/session command is added, update the engine command doc and authoritative contract guidance without pretending it is a story-choice command

## Schema Rules

- Do not invent fields outside the authoritative contract
- Do not preserve stale aliases from older summaries
- `starting_node_id` must reference a valid `node_id` in the same story file
- `starting_ship_section` is a required top-level field
- `summary` is a required top-level field
- `title` is required both at the top level and at the node level
- `sfx_hint` belongs at node level, not top level
- `sfx_hint` is required on every node and may be `null`
- `tags` is a required node-level array
- `end_state` is required on every node: `null` for stages, `"success"`, `"partial"`, or `"failure"` for endings
- outcome classification belongs to ending nodes only; the engine should not infer per-stage success/failure from prose, tags, or downstream guesses
- all `next_node_id` values must reference valid `node_id` values in the same story file
- `crew_focus` must always include both `commander` and `squad_member`
- `dice_hook` must always include all documented subkeys, using `null` for inactive values when `enabled = false`
- `commander_moment` must always include all documented subkeys, using `null` for inactive values when `enabled = false`
- stage nodes must present 1 or 2 choices in v1
- stage nodes with 3 or more choices are invalid in v1
- join-roster tracking and vote auto-close behavior are runtime rules and must not be encoded as new story JSON fields
- dice hooks are stage-only in v1; ending nodes must keep `dice_hook.enabled = false`
- commander moments are stage-only in v1; ending nodes must keep `commander_moment.enabled = false`
- `dice_hook.enabled = true` and `commander_moment.enabled = true` may not coexist on the same node in v1
- if `dice_hook.enabled = true`, the node must provide non-null `purpose`, `roll_window_seconds`, `success_threshold`, `success_text`, and `failure_text`
- if `dice_hook.enabled = true`, `roll_window_seconds` must be a positive whole number and `success_threshold` must be an integer from 1 to 90
- dice-hook success/failure is narrative-only in v1 and must not be treated as a branch redirect, chaos modifier, or vote override
- if `commander_moment.enabled = true`, the node must provide non-null `commander`, `prompt`, `window_seconds`, and `success_text`
- if `commander_moment.enabled = true`, `commander` must name a known commander, `window_seconds` must be a positive whole number, and the moment remains narrative-only in v1
- commander moments are resolved before the node's normal decision vote and do not change branching, chaos, vote eligibility, or vote resolution
- chaos should escalate across the story arc rather than reset arbitrarily

## Editorial Guidance (Not Validation Rules)

The following guidance is useful for writer review but should not be treated as hard schema validation in v1:
- keeping `read_aloud` concise for live narration
- preferring 2 choices when a stage benefits from contrast
- using commander moments sparingly
- pacing chaos escalation for comedic payoff

## Ready-for-Engine Checklist

Before treating a story as implementation-ready, verify:
- [ ] all required top-level fields are present exactly as defined above
- [ ] `cast` uses the fixed object shape
- [ ] each node includes the full node contract, including `title`, `sfx_hint`, `crew_focus`, `chaos`, `dice_hook`, `commander_moment`, `tags`, and `end_state`
- [ ] `sfx_hint` exists on every node, using a string or `null`
- [ ] `crew_focus`, `dice_hook`, and `commander_moment` include all documented subkeys even when inactive
- [ ] each stage node has 1 or 2 choices; no stage node has 3 or more choices in v1
- [ ] each choice includes `command`
- [ ] all commands are from the supported engine command list
- [ ] endings use `choices: []`, a valid `end_state`, `dice_hook.enabled: false`, and `commander_moment.enabled: false`
- [ ] enabled stage-node dice hooks include `purpose`, `roll_window_seconds`, `success_threshold`, `success_text`, and `failure_text`
- [ ] enabled stage-node commander moments include `commander`, `prompt`, `window_seconds`, and `success_text`
- [ ] no node enables both a dice hook and a commander moment in v1
- [ ] stages use `end_state: null`
- [ ] no stale schema aliases or invented fields were introduced
