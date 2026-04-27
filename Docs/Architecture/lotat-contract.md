---
id: lotat-contract
type: shared
description: Shared Legends of the ASCII Temple contract across runtime, tooling, story authoring, and stream overlay presentation.
status: active
owner: lotat-tech
---

# LotAT Contract Index

## Purpose and scope

This is the shared contract index for **Legends of the ASCII Temple (LotAT) v1** across Streamer.bot runtime actions, story tooling, worldbuilding/story authoring, and stream-overlay presentation.

Use this file for facts that must be true in more than one domain. Domain-local guides should link here instead of restating these facts.

This index summarizes the active contract; when a detailed source is more authoritative, that source is linked in the relevant section.

## Contract authority map

| Concern | Source of truth |
|---|---|
| Authored story JSON shape, supported authored commands, story validation taxonomy | [Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md](../../Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md) |
| Checked-in Streamer.bot runtime wiring, globals, timers, and current script behavior | [Actions/LotAT/runtime-contract.md](../../Actions/LotAT/runtime-contract.md) and [Actions/LotAT/implementation-map.md](../../Actions/LotAT/implementation-map.md) |
| Operator timer/trigger setup | [Actions/LotAT/operator-setup.md](../../Actions/LotAT/operator-setup.md) |
| Story viewer and file movement behavior | [Tools/LotAT/README.md](../../Tools/LotAT/README.md) |
| Broker topic strings and payload types | [Apps/stream-overlay/packages/shared/src/topics.ts](../../Apps/stream-overlay/packages/shared/src/topics.ts) and [protocol.ts](../../Apps/stream-overlay/packages/shared/src/protocol.ts) |
| Starship Shamples franchise/canon baseline | [Creative/WorldBuilding/Franchises/StarshipShamples.md](../../Creative/WorldBuilding/Franchises/StarshipShamples.md) |

## Story file schema

Runtime consumes exactly one loaded story file:

- [Creative/WorldBuilding/Storylines/loaded/current-story.json](../../Creative/WorldBuilding/Storylines/loaded/current-story.json)

The runtime does **not** scan [Creative/WorldBuilding/Storylines/ready/](../../Creative/WorldBuilding/Storylines/ready/) during a live run. Tooling copies an approved ready story into the loaded runtime file.

### Required top-level fields

Authored stories use this top-level shape in v1:

- `story_id`
- `title`
- `tone`
- `version`
- `summary`
- `starting_ship_section`
- `starting_node_id`
- `cast.commanders_used`
- `cast.squad_members_used`
- `ship_sections_used`
- `commands_used`
- `nodes`

### Required node fields

Every node must include:

- `node_id`
- `node_type` (`stage` or `ending`)
- `ship_section`
- `title`
- `read_aloud`
- `sfx_hint` (may be `null`)
- `crew_focus.commander`
- `crew_focus.squad_member`
- `chaos.delta`
- `dice_hook.enabled`, `purpose`, `roll_window_seconds`, `success_threshold`, `failure_text`, `success_text`
- `commander_moment.enabled`, `commander`, `prompt`, `window_seconds`, `success_text`
- `choices`
- `tags`
- `end_state`

Stage nodes use `end_state: null` and must have 1 or 2 choices in v1. Ending nodes use `choices: []` and `end_state` of `success`, `partial`, or `failure`.

### Choice fields

Each choice contains:

- `choice_id`
- `label`
- `command`
- `result_flavor`
- `next_node_id`

`next_node_id` must reference an existing node. Ties resolve by the order of the authored `choices` array.

## Runtime globals and timers

Canonical global and timer names are also listed in [Actions/SHARED-CONSTANTS.md](../../Actions/SHARED-CONSTANTS.md). Runtime implementation details live in [Actions/LotAT/runtime-contract.md](../../Actions/LotAT/runtime-contract.md).

Required v1 timers:

- `LotAT - Join Window`
- `LotAT - Decision Window`
- `LotAT - Commander Window`
- `LotAT - Dice Window`

Join and decision windows use fixed 120-second v1 defaults configured in Streamer.bot. Commander and dice window durations are authored per node (`commander_moment.window_seconds` and `dice_hook.roll_window_seconds`), but current runtime comments still call out Streamer.bot timer interval API verification as an operator check.

Shared runtime globals include session state (`lotat_active`, `lotat_session_*`), node/window state (`lotat_node_*`), vote state (`lotat_vote_*`), commander/dice fields, and terminal breadcrumbs. See the runtime contract for the exact list and default values.

## Commands

### Runtime-only commands

These are engine commands, not authored story-choice commands:

- `!join` — join the active session during `join_open`
- `!roll` — roll during an active dice window
- `!stretch`, `!shrimp` — Captain Stretch commander moment commands
- `!hydrate`, `!orb` — Water Wizard commander moment commands
- `!checkchat`, `!toad` — The Director commander moment commands

Runtime-only commands must not appear in `choices[].command` or `commands_used`.

### Authored decision commands

Supported v1 story-choice commands are:

- `!scan`
- `!target`
- `!analyze`
- `!reroute`
- `!deploy`
- `!contain`
- `!inspect`
- `!drink`
- `!simulate`

## Participation and voting

- Live runs begin with a `!join` phase.
- Joined users form the participant roster for the run.
- The roster freezes when the join window closes.
- Only frozen-roster users count for authored decision votes.
- Latest valid vote replaces a prior valid vote from the same joined user while the decision window is open.
- If every joined participant has a valid vote, the runtime may early-close voting.
- Zero joiners end the run cleanly before story play.
- Zero valid votes at decision close ends the run unresolved.

## Commander moments

Commander moments are optional, blocking pre-vote runtime windows on stage nodes only.

- Only the currently assigned user for the named commander can satisfy the moment.
- Commander participation is independent of the joined roster.
- Any valid command for that commander counts.
- First valid assigned-commander command closes the window as success.
- Timeout, missing assignment, offline commander, or invalid chatter input silently continues to normal voting.
- Success is narrative-only in v1 and does not change branching, chaos, vote eligibility, or vote resolution.
- A node may not enable both a commander moment and a dice hook in v1.

## Dice hooks

Dice hooks are optional, blocking pre-vote runtime windows on stage nodes only.

- Any viewer may use `!roll`, even if not joined.
- Rolls are random values from 1 to 100.
- Success is `roll >= success_threshold`.
- `success_threshold` must be an authored integer from 1 to 90.
- First successful roll closes the window immediately.
- Timeout emits authored failure text and continues to normal voting.
- Success/failure is narrative-only in v1 and does not change branching, chaos, vote eligibility, or vote resolution.

## Offering / steal mechanic boundary

`!offering` is explicitly out of scope for LotAT v1.

Do not infer LotAT support from `Actions/Squad/offering.cs`, offering globals, boost-state globals, or older experiments. LotAT branching, chaos, commander windows, dice windows, join flow, voting, and endings do not depend on offering state in the current implementation.

If future LotAT/offering integration is approved, it must be introduced as a new explicit contract decision and synchronized across story contract, runtime docs, tooling, overlay presentation, and shared constants.

## Presentation events

LotAT presentation uses the local stream-overlay broker. The broker routes messages only; Streamer.bot owns business rules and the overlay renders what messages request.

Core envelope:

- `id`: sender-generated UUID/string unique per message
- `topic`: shared topic constant
- `sender`: stable client name, usually `streamerbot`
- `timestamp`: Unix epoch milliseconds
- `payload`: topic-specific object

LotAT topics currently include:

- `lotat.session.start`
- `lotat.session.end`
- `lotat.join.open`
- `lotat.join.update`
- `lotat.join.close`
- `lotat.node.enter`
- `lotat.vote.open`
- `lotat.vote.cast`
- `lotat.vote.close`
- `lotat.dice.open`
- `lotat.dice.roll`
- `lotat.dice.close`
- `lotat.commander.open`
- `lotat.commander.close`
- `lotat.chaos.update`

`lotat.node.enter` carries renderable node data derived from story JSON: node identity/type, ship section, title, read-aloud text, `sfx_hint`, `crew_focus`, chaos delta, optional dice/commander prompt data, display choices, and ending state. The authoritative TypeScript payload shapes are in [protocol.ts](../../Apps/stream-overlay/packages/shared/src/protocol.ts).

## V1 boundaries and future scope

LotAT v1 intentionally excludes:

- `!offering` integration
- boost-state integration
- operator force-close, manual advance, or inspection tools
- late join / leave flow after roster freeze
- full schema/graph validation at runtime start
- story-authored join/vote/timer state fields
- commander/dice outcomes affecting branch resolution or chaos
- hidden recovery snapshots or operator-inspection blobs
- landing party systems and other future mechanics unless explicitly approved

Future changes that alter story schema, command contract, validation taxonomy, runtime stage behavior, or overlay payloads require coordinated updates across the source-of-truth docs above.
