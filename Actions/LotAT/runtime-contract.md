# Actions/LotAT Runtime Contract

This file documents the checked-in Streamer.bot runtime contract for LotAT v1. For cross-domain facts shared by runtime, tooling, worldbuilding, and overlay presentation, see [LotAT contract](../../.agents/_shared/lotat-contract.md).

## Runtime file contract

The engine reads exactly one story file:

- [Creative/WorldBuilding/Storylines/loaded/current-story.json](../../Creative/WorldBuilding/Storylines/loaded/current-story.json)

The runtime does not scan [Creative/WorldBuilding/Storylines/ready/](../../Creative/WorldBuilding/Storylines/ready/). Story loading/review belongs upstream in [Tools/LotAT/](../../Tools/LotAT/).

## Current implementation scope

Implemented:

- voice-triggered session bootstrap
- minimal runtime story load checks against the loaded story file
- 120-second join window using `!join`
- session-scoped joined-roster tracking with lowercase usernames
- zero-join clean end or non-zero roster freeze and starting-node handoff
- node entry, chaos accumulation, and stage intro output
- commander and dice pre-vote windows
- 120-second decision windows for authored commands
- joined-user-only voting and latest-valid-vote replacement
- early-close when all joined users have valid votes
- deterministic resolution by earliest authored choice in tie order
- centralized teardown for `success`, `partial`, `failure`, `zero_join`, `unresolved`, and `fault_abort`

Not implemented:

- `!offering` or boost-state integration
- operator force-close, manual advance, or inspection tools
- full schema/graph validation at runtime start
- late join / leave flow
- Mix It Up-heavy production polish beyond chat-safe messages
- robust repo-root discovery beyond candidate-path probing

## Runtime globals

These names match the current scripts and [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md).

### Session-level

- `lotat_active`
- `lotat_session_id`
- `lotat_session_stage`
- `lotat_session_story_id`
- `lotat_session_current_node_id`
- `lotat_session_chaos_total`
- `lotat_session_roster_frozen`
- `lotat_session_joined_roster_json`
- `lotat_session_joined_count`

### Node/window-level

- `lotat_node_active_window`
- `lotat_node_window_resolved`
- `lotat_node_allowed_commands_json`

### Vote-level

- `lotat_vote_map_json`
- `lotat_vote_valid_count`

### Commander-window-only

- `lotat_node_commander_name`
- `lotat_node_commander_target_user`
- `lotat_node_commander_allowed_commands_json`

### Dice-window-only

- `lotat_node_dice_success_threshold`

### Minimal history / terminal breadcrumbs

- `lotat_session_last_choice_id`
- `lotat_session_last_end_state`

### Existing non-LotAT globals read by the engine

These are read only for commander snapshotting:

- `current_captain_stretch`
- `current_the_director`
- `current_water_wizard`

### Safe idle/default values

- booleans: `false`
- counts / thresholds: `0`
- stage: `idle`
- active window: `none`
- string ids / names: `""`
- roster JSON: `[]`
- allowed command JSON: `[]`
- vote map JSON: `{}`

## Runtime stage set

- `idle`
- `join_open`
- `node_intro`
- `commander_open`
- `dice_open`
- `decision_open`
- `decision_resolving`
- `ended`

## Timers

Create/maintain these exact Streamer.bot timers:

- `LotAT - Join Window`
- `LotAT - Decision Window`
- `LotAT - Commander Window`
- `LotAT - Dice Window`

Join and decision windows use fixed 120-second v1 defaults configured in Streamer.bot. Commander and dice windows use authored node durations. Current code comments still require operator verification of `CPH.SetTimerInterval(string, int)` behavior for authored commander/dice durations.

## Commands

Runtime session commands:

- `!join`
- `!roll`

Commander-window commands:

- `!stretch`
- `!shrimp`
- `!hydrate`
- `!orb`
- `!checkchat`
- `!toad`

Authored decision commands:

- `!scan`
- `!target`
- `!analyze`
- `!reroute`
- `!deploy`
- `!contain`
- `!inspect`
- `!drink`
- `!simulate`

`!offering` is out of scope for LotAT v1 runtime and story schema behavior.

## Participation rules

- `!join` only counts during `join_open`.
- Joined roster freezes after join closes.
- Only frozen-roster users count for authored decision voting.
- Latest valid decision vote replaces a previous valid vote from the same joined user.
- Commander windows ignore joined-roster status and only accept the snapshotted assigned commander user.
- Dice windows ignore joined-roster status and accept rolls from any chatter.

## Runtime flow summary

1. `lotat-start-main.cs` starts from a voice trigger.
2. Runtime story is minimally loaded from the loaded story file.
3. Stale LotAT timers/state are cleared.
4. Stage becomes `join_open`; chat is told to use `!join`.
5. `lotat-join.cs` collects unique lowercase usernames.
6. `lotat-join-timeout.cs` closes the join window.
7. Zero joiners end as `zero_join`; non-zero joiners freeze the roster and enter the starting node.
8. `lotat-node-enter.cs` applies chaos, sends intro text, and routes to ending, commander, dice, or decision flow.
9. `lotat-decision-input.cs` records valid votes; early-close may disable the decision timer.
10. `lotat-decision-resolve.cs` tallies, breaks ties by authored choice order, emits `result_flavor`, and sets the next node.
11. Node entry repeats until a terminal state.
12. `lotat-end-session.cs` performs centralized cleanup.

## Known limitations

- No `!offering` integration.
- No operator control surface for manual advance, inspect, force-close, or debug commands.
- No full runtime schema/graph validation pass.
- Story file path discovery is machine-local and brittle.
- Commander/dice timer duration setting needs operator verification.
- Decision timer restart is UI-driven.
- Limited chat polish.
- No Mix It Up-rich production hooks documented here as finished behavior.
- No late join / leave flow.
- No extra recovery snapshots or operator-inspection blobs.
