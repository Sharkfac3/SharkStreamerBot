# LotAT Engine — Overview

## What the Engine Does

The C# engine running in Streamer.bot actions under `Actions/LotAT/`:
- Loads and parses story JSON
- Opens a session-start join phase and registers `!join` participants
- Tracks current node state
- Handles chat command routing to story mechanics
- Advances the story based on chat votes/commands
- Closes a decision window early when all joined participants have voted
- Manages the Chaos Meter
- Triggers Mix It Up overlays for narration/effects

## Engine Design Principles

- Story content is **data** (JSON), not **code** — the engine should not contain hardcoded story text
- Engine is story-agnostic — any valid JSON story file should run without engine modifications
- Chat commands are the primary interaction surface — see `commands.md` for supported set
- Chaos Meter state is tracked in global variables and must survive action re-entry
- Runtime contract docs in this folder describe behavior boundaries; implementation work in `Actions/LotAT/` should follow them rather than move those rules into ad hoc script-specific docs
- V1 timing is split cleanly by ownership: join and decision windows are fixed runtime defaults, while commander and dice windows are authored per node in story JSON
- Recommended implementation model is a hybrid: Streamer.bot named timers trigger window-close actions, and runtime stage/window state guards make stale timer fires harmless

## Current Status

`Actions/LotAT/` is reserved and in active development. As engine scripts are created, document each one here.

## Runtime Session Spec — Start Here

If your question is about how a live LotAT run behaves before or between any future C# implementation details, read these docs in order:
1. `docs-map.md` — navigation entry point
2. `session-lifecycle.md` — canonical runtime session flow spec
3. `state-and-voting.md` — canonical runtime roster/voting spec
4. `commands.md` — runtime vs authored command boundary

These docs define the **runtime contract**. Future Streamer.bot scripts should implement them, not silently redefine them.

## State Variables

All LotAT engine state in `Actions/SHARED-CONSTANTS.md`:
- `lotat_active` — session running flag
- `lotat_announcement_sent` — session start dedup
- `lotat_offering_steal_chance` — offering integration
- `lotat_steal_multiplier` — offering steal scaling
- `boost_*` — boost state

### Canonical v1 runtime variable contract

The current runtime docs now assume a **minimal implementation-first state contract**.

Use **individual globals** for scalar values and **JSON-packed globals** only for structured collections.
This keeps the engine easier for coding agents to implement while still giving timers and chat handlers enough shared state to operate safely between actions.

The JSON shapes documented for roster, allowed commands, and vote storage are **canonical v1 contract**, not loose implementation examples.
LotAT runtime identity should use a **lowercase username/login string** everywhere runtime user comparison occurs, including joined-roster storage, vote-map keys, and commander-target comparison.

#### Session-level required
- `lotat_active`
- `lotat_session_id`
- `lotat_session_stage`
- `lotat_session_story_id`
- `lotat_session_current_node_id`
- `lotat_session_chaos_total`
- `lotat_session_roster_frozen`
- `lotat_session_joined_roster_json`
- `lotat_session_joined_count`

#### Node/window-level required
- `lotat_node_active_window`
- `lotat_node_window_resolved`
- `lotat_node_allowed_commands_json`

#### Vote-level required
- `lotat_vote_map_json`
- `lotat_vote_valid_count`

#### Required only when a commander window is active
- `lotat_node_commander_name`
- `lotat_node_commander_target_user`
- `lotat_node_commander_allowed_commands_json`

#### Required only when a dice window is active
- `lotat_node_dice_success_threshold`

#### Minimal history state recommended for v1
- `lotat_session_last_choice_id`
- `lotat_session_last_end_state` *(recommended, not strictly required for first runnable implementation)*

### Deliberately deferred beyond v1
- full branch-history globals beyond the minimal `lotat_session_last_choice_id`
- rich recovery snapshots
- per-roll history
- per-vote history beyond the active tally
- operator-inspection blobs beyond what logs already provide

### Coordination rule

When implementation begins:
1. add the canonical variable names to `Actions/SHARED-CONSTANTS.md`
2. reset them in `Actions/Twitch Core Integrations/stream-start.cs`
3. disable all four LotAT timers at stream start before returning runtime state to `idle`
4. reset JSON globals to safe empty JSON values and scalar globals to safe empty defaults

## Sub-Skills

- `docs-map.md` — high-level navigation map for the LotAT engine docs in this folder
- `commands.md` — supported chat commands and engine constraints
- `session-lifecycle.md` — canonical runtime session contract: stages, join flow, roster freeze, zero-join handling, node-entry flow, decision flow, teardown, and operator recovery controls
- `state-and-voting.md` — participant roster, vote storage, early-close rules, tie-break behavior, and edge cases
