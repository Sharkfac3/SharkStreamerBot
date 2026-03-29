# LotAT Engine — State and Voting Spec

## Purpose

This document defines the recommended **runtime state model** and **voting contract** for LotAT sessions.

Use it when planning engine behavior, reviewing schema/runtime boundaries, or aligning future Streamer.bot implementation work with the live-session contract.

This is agent scaffolding for the engine layer. It is intentionally implementation-aware and should align to the canonical v1 runtime variable names and JSON storage shapes documented in this file and the surrounding engine docs.

## Runtime Boundary

This document covers runtime behavior only.

Do not convert these rules into new story JSON fields unless an intentional schema change is approved.

In particular:
- `!join` is a runtime session command
- commander-input commands are runtime commander-window commands
- `!roll` is a runtime dice-window command
- join and decision timer defaults are runtime-owned
- joined-participant tracking is runtime state
- vote eligibility is runtime logic
- latest-vote-wins behavior is runtime logic
- early-close when all joined participants have voted is runtime logic
- tie-break behavior is runtime logic
- story JSON still only declares the node graph and authored decision commands

## Core Runtime Assumption

LotAT uses a **session-scoped participant model**, not an open-chat-everyone-counts model.

That means:
- each run begins with a join phase
- viewers opt into that run with `!join`
- the engine builds a session roster from those joins
- later votes are evaluated against that roster
- the roster is the denominator for the "everyone has voted" early-close rule

This keeps live behavior deterministic and recoverable.

Timer ownership in v1:
- join window length is a fixed runtime default of **120 seconds**
- decision window length is a fixed runtime default of **120 seconds**
- commander-window length comes from authored `commander_moment.window_seconds`
- dice-window length comes from authored `dice_hook.roll_window_seconds`
- join/decision timing is not story-authored and does not support per-story, per-node, or live-operator overrides in v1

Important exceptions:
- nodes with enabled commander moments open a pre-vote commander-input window that is **not** roster-gated
- only the snapshotted assigned commander user for that node may satisfy that commander window
- commander-window participation remains separate from the joined-participant voting model
- nodes with enabled dice hooks open a pre-vote `!roll` window that is **not** roster-gated
- any viewer in chat may roll during that window
- dice-window participation remains separate from the joined-participant voting model

## Participant Identity Rules

Canonical participant identity key for v1:
- `lowercased username/login string`

Canonical normalized runtime identity rule:
1. read the chat user/login value
2. normalize it to lowercase
3. use that lowercase username/login string everywhere runtime participation is checked or compared

This same identity rule should be used consistently for:
- join deduplication
- joined-roster membership checks
- per-node vote storage
- vote replacement
- early-close calculations
- commander-target comparison
- operator inspection / recovery output

Reasoning:
- this keeps LotAT identity storage aligned with the existing commander-user comparison model
- one canonical lowercase username format is easier for coding agents to implement consistently than mixed `userId` / username fallback logic
- using one normalized identity rule avoids join/vote/commander mismatches

## Joined-Participant Roster Rules

For a single LotAT run:
- viewers may join only during the join phase with `!join`
- each participant may appear only once in the roster
- duplicate joins do not create duplicate entries
- the roster is scoped to the current session only
- when the join phase closes, the roster is frozen for the rest of the run

Recommended v1 assumptions:
- late joiners are not added after join closes
- no leave/remove command is required in v1
- disconnected or silent participants remain part of the frozen roster for that run

The runtime should always be able to answer:
- who joined this run
- how many joined participants there are
- whether a specific viewer is in the roster
- whether the roster is still open or already frozen

## Vote Eligibility Rules

During a decision window, a vote is valid only when **all** of the following are true:
- runtime stage is `decision_open`
- the viewer is in the frozen joined roster for this session
- the submitted command matches one of the current node's allowed authored decision commands
- the vote arrives before the decision window closes

A vote is not valid when any of the following are true:
- the viewer never joined
- the viewer tries to vote during `join_open`, `node_intro`, `decision_resolving`, `ended`, or `idle`
- the submitted command is not one of the active node's choices
- the vote arrives after the window is already closed
- the session has already been cancelled or torn down

Important boundary:
- `!join` is never a story-choice vote
- commander-input commands are never story-choice votes
- `!roll` is never a story-choice vote
- authored `choices[].command` values are the only commands that may count as decision votes

## Valid-Vote Storage Rules

Recommended v1 contract:

> Each joined participant has at most one active valid vote per node, and the latest valid vote replaces any earlier valid vote from that same participant for that same node.

Implications:
- vote storage is **per node**, not global across the whole story
- only valid votes should be persisted in the active vote map
- invalid vote attempts do not overwrite a previously valid vote
- when the node changes, the prior node's active vote map must not carry forward as live vote state

A practical runtime model should let the engine answer:
- which joined participants have a current valid vote for this node
- which command each of them currently supports
- how many joined participants have submitted a valid vote
- whether all joined participants have submitted a valid vote

## Latest-Vote-Replaces-Prior-Vote Behavior

Recommended viewer-facing behavior:
- a joined participant may change their mind during the open decision window
- if they submit another valid command for the same node before close, the new valid command replaces the prior one
- only the latest valid vote from that participant counts at tally time

Reasons this is preferred:
- more forgiving for live chat mistakes
- easier for viewers to understand
- reduces operator friction when chat reacts quickly or misfires
- still works cleanly with early-close rules

Recommended guardrail:
- replacement only applies while the window is still open
- once the runtime stage leaves `decision_open`, the vote map is locked for resolution
- because early-close may fire as soon as the last missing joined vote arrives, a participant is only guaranteed the ability to change a vote until that close condition is met

## Early-Close Rule

A decision window may auto-close when all of the following are true:
- runtime stage is `decision_open`
- the joined roster is frozen for the current session
- joined participant count is greater than zero
- every joined participant currently has a valid recorded vote for the active node

Once those conditions are met:
- stop the decision timer
- close the decision window immediately
- transition into the normal decision-resolution path

Clarification for future implementers:
- this is the same runtime early-close rule referenced in `commands.md` and `session-lifecycle.md`
- the denominator is the frozen joined roster for the current session only
- the rule must not be converted into a story JSON field, per-story override, or open-chat tally rule without an intentional contract change

Important denominator rule:
- the check is against the **joined roster**, not against all chat viewers who happen to speak
- non-joined chatter never delays or accelerates early-close

## Commander-Window Participation Rules

When a node opens a commander window:
- runtime stage must be `commander_open` for commander-input commands to be accepted
- the engine snapshots the assigned commander user at window open
- only that snapshotted assigned commander user may satisfy the commander moment
- no joined-roster check is applied to commander input in v1
- valid commands are determined only by the authored `commander_moment.commander` value and the engine's commander-command mapping
- the first valid mapped commander-input command ends the commander window immediately
- if the timer expires before any valid assigned-commander input occurs, the result is a silent skip/failure
- commander success/failure is narrative-only in v1 and does **not** alter chaos, branching, vote eligibility, or vote tallying
- non-assigned users typing mapped commander commands during the window are ignored silently

Recommended runtime state for commander windows:
- whether the active node currently has a commander window open
- which commander the active node expects
- which assigned commander identity was snapshotted at window open
- the current node's authored `window_seconds`
- whether the commander result already resolved as success/skip
- optional debug snapshot such as last valid commander-input command seen for the node

## Dice-Window Participation Rules

When a node opens a dice window:
- runtime stage must be `dice_open` for `!roll` to be accepted
- `!roll` is open to **all chat viewers**, not just joined participants
- no participant roster check is applied to dice rolls in v1
- users may roll repeatedly while the window remains open
- each roll generates a public 1–100 result in chat
- success is `roll >= success_threshold`
- the first successful roll ends the dice window immediately
- if the timer expires before any successful roll occurs, the result is failure
- dice success/failure is narrative-only in v1 and does **not** alter chaos, branching, vote eligibility, or vote tallying

Recommended runtime state for dice windows:
- whether the active node currently has a dice window open
- the current node's `success_threshold`
- the current node's authored `roll_window_seconds`
- whether the dice result already resolved as success/failure
- optional debug snapshot such as total roll attempts during the node

## Tie-Break Behavior

Recommended deterministic tie-break rule:

> If the vote is tied, resolve to the earliest matching choice in the active node's `choices` array.

Why this is the preferred default:
- deterministic and easy to debug
- no extra randomness injected into live flow
- story authors can reason about predictable fallback behavior
- future agents can inspect a node and know exactly how ties resolve

Recommended interpretation:
- tally votes by command
- find the highest vote count
- if more than one choice shares that count, choose the earliest of those tied commands in authored `choices` order

## Canonical v1 Runtime State Categories

To reduce implementation ambiguity, v1 should use the following runtime categories and canonical variable names.

### 1. Session-level state

These values are required for every active run:
- `lotat_active`
- `lotat_session_id`
- `lotat_session_stage`
- `lotat_session_story_id`
- `lotat_session_current_node_id`
- `lotat_session_chaos_total`
- `lotat_session_roster_frozen`
- `lotat_session_joined_roster_json`
- `lotat_session_joined_count`

Contract notes:
- these values must survive normal action-to-action execution during a live run
- `lotat_session_id` exists both for logging clarity and as a stale-event guard anchor
- `lotat_session_joined_roster_json` should use a minimal identity-only JSON array of lowercase username/login strings in v1
- `lotat_session_joined_count` is intentionally stored separately for simple checks and logging

### 2. Node-level state

These values are required while a node is active:
- `lotat_node_active_window`
- `lotat_node_window_resolved`
- `lotat_node_allowed_commands_json`

Conditional node/window values:
- commander window: `lotat_node_commander_name`, `lotat_node_commander_target_user`, `lotat_node_commander_allowed_commands_json`
- dice window: `lotat_node_dice_success_threshold`

Contract notes:
- `lotat_node_active_window` should be a simple literal such as `none`, `join`, `commander`, `dice`, or `decision`
- `lotat_node_window_resolved` exists so timer callbacks and chat handlers can no-op safely once the node has already advanced
- `lotat_node_allowed_commands_json` should hold only the active node's authored decision commands
- the contract intentionally avoids requiring richer per-window history in v1

### 3. Vote-level state

These values are required during decision handling:
- `lotat_vote_map_json`
- `lotat_vote_valid_count`

Contract notes:
- `lotat_vote_map_json` should be a minimal lowercase-username → command map for the active node only
- the engine should not preserve full vote history in v1
- `lotat_vote_valid_count` should reflect how many joined participants currently have a valid active vote on the node

### 4. Minimal history state

Keep history intentionally small in v1:
- `lotat_session_last_choice_id`
- `lotat_session_last_end_state` *(recommended but optional for first runnable implementation)*

Do not introduce richer branch-history storage in v1 unless the runtime contract is intentionally expanded later.

### 5. Recovery / operator-support state

Deep recovery state is intentionally deferred until after v1.
Recovery snapshots are **not part of the v1 required contract**.
For now, logs plus the active runtime globals above are the preferred debugging surface.

## Canonical v1 Data Shape Expectations

Use **explicit scalar globals** for simple values and **JSON-packed globals** only when a structured collection is required.

### Minimal joined-roster shape
```json
["sharkfac3","viewername"]
```

### Minimal allowed-command-list shape
```json
["!scan","!target"]
```

### Minimal vote-map shape
```json
{
  "sharkfac3": "!scan",
  "viewername": "!target"
}
```

Rationale:
- easier for coding agents to implement than one large runtime blob
- easier to reason about than many over-detailed per-user structures
- enough to support dedupe, membership checks, tallying, early-close behavior, and commander-target comparison
- keeps the canonical JSON shapes fully locked before engine implementation begins

Reminder:
- any new LotAT runtime variable introduced during implementation must also be reflected in `Actions/SHARED-CONSTANTS.md`
- any reset-sensitive runtime variable must be included in `Actions/Twitch Core Integrations/stream-start.cs` when implementation begins

## Decision Resolution Contract

When a decision window closes, the engine should:
1. lock out new votes for that node
2. tally votes using only the current node's stored valid-vote map
3. apply the tie-break rule if needed
4. select the winning authored choice
5. emit that choice's `result_flavor`
6. apply the node's intended runtime result effects
7. transition to the winning choice's `next_node_id`

Recommended invariant:
- there should be only one resolution pass per node

## Edge Cases to Preserve in the Contract

### Duplicate joins
- ignore duplicates
- do not inflate participant count
- do not create parallel roster entries for the same identity

### Non-joined votes
- do not count them toward the tally
- do not count them toward early-close
- do not let them overwrite any stored vote

### Invalid-command votes from joined viewers
- do not count them as votes for the node
- do not overwrite an already valid vote for that viewer
- do not increment the valid-voter count

### Joined viewer changes vote
- accept the latest valid vote while `decision_open`
- replace the prior valid vote for that same node and participant

### Everyone votes immediately
- close early and resolve without waiting for timer expiry

### Not everyone votes
- let the timer expire normally
- resolve using whatever valid votes exist at close time

### Commander window with no assigned commander
- skip the commander moment silently to chat
- log the skip for operator/debug visibility
- continue into the normal decision window afterward

### Wrong user types a valid commander command
- ignore it silently
- do not treat it as a vote
- do not mutate commander-window state

### Assigned commander responds correctly
- accept the first valid mapped command while `commander_open`
- close the commander window immediately
- emit the authored `success_text`
- continue into the normal decision window afterward

### Commander window times out
- let the commander timer expire normally
- emit no failure text in v1
- continue into the normal decision window afterward

### Dice window with lots of chat activity
- allow repeated `!roll` attempts while runtime stage is `dice_open`
- close immediately on the first roll meeting the threshold
- ignore later `!roll` messages after the dice window resolves

### Nobody rolls
- let the dice timer expire normally
- treat the node's dice outcome as failure
- continue into the normal decision window afterward

### Vote arrives exactly as the timer closes
- once runtime stage leaves `decision_open`, treat subsequent arrivals as late votes and ignore them
- do not attempt ambiguous half-open acceptance rules after closure

### Late votes after early-close or timer expiry
- ignore them
- do not reopen the node
- do not mutate the locked vote snapshot

### Duplicate vote message with same valid command
- harmlessly overwrite same participant/same-node vote with the same command or no-op it
- result should remain identical either way

### Zero joiners
Recommended v1 behavior:
- do not open normal story voting at all
- end the session cleanly when the join phase closes empty

### Zero valid votes on a node
Recommended behavior:
- still resolve through the normal close path
- because there is no winning vote total, fall back to the earliest authored choice in `choices` order unless a later implementation contract explicitly defines another deterministic fallback

This preserves progress without inventing a new schema field.

## Recovery Expectations

For live reliability, runtime state should be sufficient for an operator or later action to determine:
- whether a run is active
- which runtime stage it is in
- which node is active
- whether the roster is still open or already frozen
- who joined this session
- which valid votes are currently recorded for the node
- whether the current node is still open, already resolving, or already resolved
- what the last successful resolution decision was

Recommended recovery posture:
- runtime state should support safe inspection without requiring story edits
- runtime state should make it possible to force-close, cancel, or resume with minimal ambiguity
- recovery should prefer deterministic continuation over clever reconstruction

## Non-Goals

This spec does **not** require:
- supporting mid-story roster changes in v1
- exposing all runtime state publicly in chat
- random tie-break behavior
- treating `!join` as an authored story-choice command
- treating commander-input commands as authored story-choice commands

## Related Docs

- `session-lifecycle.md` — full run flow from idle → join → node → decision → ending
- `commands.md` — authored decision commands vs runtime session commands
- `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` — technical-agent operating guidance
- `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` — authoritative authored story contract
