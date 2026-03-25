# LotAT Engine — State and Voting Spec

## Purpose

This document defines the recommended **runtime state model** and **voting contract** for LotAT sessions.

Use it when planning engine behavior, reviewing schema/runtime boundaries, or aligning future Streamer.bot implementation work with the live-session contract.

This is agent scaffolding for the engine layer. It is intentionally implementation-aware, but not tied to specific C# classes, global-variable names, or one exact storage shape.

## Runtime Boundary

This document covers runtime behavior only.

Do not convert these rules into new story JSON fields unless an intentional schema change is approved.

In particular:
- `!join` is a runtime session command
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

## Participant Identity Rules

Recommended participant identity key:
- primary: `userId`
- fallback: lowercased `user`

Recommended normalized runtime identity rule:
1. if a stable `userId` exists, use it
2. otherwise use the lowercased username/login value
3. use the same resolved identity key everywhere runtime participation is checked

This same identity rule should be used consistently for:
- join deduplication
- joined-roster membership checks
- per-node vote storage
- vote replacement
- early-close calculations
- operator inspection / recovery output

Reasoning:
- `userId` is the most stable viewer identity
- lowercased `user` provides a practical fallback when ID data is unavailable
- using one normalized identity rule avoids join/vote mismatches

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

## Suggested Runtime State Categories

### 1. Session-level state

Tracks whether a run exists and what run-level context is active.

Examples:
- whether a LotAT run is active
- current runtime stage
- current story ID
- current node ID
- current chaos total
- join-phase open/closed status
- joined-participant roster
- joined-participant count
- whether the roster is frozen

### 2. Node-level state

Tracks what matters for the currently active node.

Examples:
- active node type
- allowed commands for the current node
- vote map for the current node
- count of joined participants who currently have a valid vote
- whether the node has already entered resolution
- last winning command / last winning choice snapshot

### 3. History / trace state

Tracks what already happened so the run can be inspected or resumed safely.

Examples:
- branch history
- previously resolved node IDs
- last resolved choice
- last emitted result flavor snapshot
- last ending reached

### 4. Recovery / operator-support state

Tracks enough information to inspect or recover from interruptions.

Examples:
- current roster snapshot
- current vote snapshot
- last stage transition
- last successful resolution snapshot
- cancellation / abort reason if the run was interrupted

## Suggested Runtime Data Shape Expectations

Exact final names belong to implementation work, but the contract likely needs runtime values representing:
- active session stage
- joined-participant roster
- joined-participant count
- roster frozen/open state
- active node identifier
- current node allowed commands
- current node vote map
- current node valid-voter count
- current chaos total
- branch history
- last resolution snapshot

Reminder:
- any new LotAT runtime variable introduced during implementation must also be reflected in `Actions/SHARED-CONSTANTS.md`
- any persisted reset-sensitive runtime variable must be included in the relevant stream-start / reset flow when code work begins

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
- modifying the authored story JSON shape
- adding new top-level or node-level schema fields
- supporting mid-story roster changes in v1
- exposing all runtime state publicly in chat
- random tie-break behavior
- treating `!join` as an authored story-choice command

## Related Docs

- `session-lifecycle.md` — full run flow from idle → join → node → decision → ending
- `commands.md` — authored decision commands vs runtime session commands
- `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` — technical-agent operating guidance
- `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` — authoritative authored story contract
