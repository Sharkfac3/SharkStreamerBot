# LotAT Engine — State and Voting Spec

## Purpose

This document defines the recommended **runtime state model** and **vote resolution contract** for LotAT sessions.

Use it when planning engine behavior, reviewing schema/runtime boundaries, or aligning Streamer.bot implementation work with the LotAT engine contract.

## Runtime Boundary

This document covers runtime behavior only.

Do not convert these rules into new story JSON fields unless an intentional schema change is approved.

In particular:
- `!join` is a runtime session command
- joined-user tracking is runtime state
- "all joined users have voted" is runtime logic
- story JSON still only declares the node graph and authored decision commands

## Participant Identity Rule

Recommended participant identity key:
- primary: `userId`
- fallback: lowercased `user`

This same identity rule should be used consistently for:
- join deduplication
- vote storage
- vote replacement
- roster membership checks

## Participant Roster Rules

For a single LotAT run:
- viewers may join during the join phase with `!join`
- each participant may appear only once in the roster
- duplicate joins are ignored
- once the join phase closes, the roster is frozen for the remainder of the run

Recommended v1 rule:
- late joiners are not added after the join phase ends
- no leave/removal command is required in v1

## Vote Eligibility Rules

During a decision window:
- only joined participants are eligible to vote
- only commands allowed by the active node are valid votes
- votes from non-joined viewers do not count
- votes for commands not present in the current node do not count
- votes received after the decision window closes do not count

## Vote Storage Rule

Recommended v1 contract:

> Each joined participant has one active vote per node, and the latest valid vote replaces any earlier valid vote from that same participant for that node.

Why this is preferred:
- simpler live UX
- less punishing for chat mistakes
- still compatible with early-close logic

## Early-Close Rule

A decision window may auto-close when all of the following are true:
- runtime stage is `decision_open`
- the roster is frozen for the current run
- joined participant count is greater than zero
- every joined participant currently has a valid recorded vote for the active node

Once those conditions are met:
- stop the decision timer
- close the decision window immediately
- transition to the normal resolution path

## Vote Count Model

For each active node, the engine should be able to answer:
- who joined this run
- which joined participants have voted on this node
- which command each joined participant currently supports
- whether all joined participants have voted

That means vote state should be **per node**, not carried forward implicitly from earlier nodes.

## Tie-Break Rule

Recommended deterministic tie rule:

> If the vote is tied, resolve to the earliest matching choice in the active node's `choices` array.

Why this is preferred:
- deterministic and debuggable
- avoids injecting extra randomness into live flow
- easy for future agents to reason about

## Recommended Runtime State Categories

### Session-level state
Examples:
- whether a LotAT run is active
- current runtime stage
- current story ID
- current node ID
- current chaos total
- joined participant roster
- join count

### Node-level state
Examples:
- allowed commands for the current node
- recorded votes for the current node
- count of joined participants who have voted
- whether the current node has already been resolved

### Recovery/support state
Examples:
- branch history
- last resolved choice
- last ending reached
- operator inspection/debug values

## Recommended Global-Variable Themes

Exact final names belong to implementation work, but the runtime contract likely needs values representing:
- active session stage
- joined participant roster
- joined participant count
- current node allowed commands
- current node vote map
- current node vote count
- current chaos total
- branch history / last resolution snapshot

Reminder:
- any new LotAT global variable must also be added to `Actions/Twitch Core Integrations/stream-start.cs` and `Actions/SHARED-CONSTANTS.md` when implementation begins

## Decision Resolution Contract

When a decision window closes, the engine should:
- lock out new votes for that node
- tally votes using only the current node's stored vote map
- apply the tie-break rule if needed
- select the winning authored choice
- emit that choice's `result_flavor`
- apply the node's success/failure-related runtime effects as designed
- transition to the winning choice's `next_node_id`

## Edge Cases to Preserve in the Contract

### Duplicate `!join`
Ignore duplicates; do not inflate participant count.

### Non-joined viewer votes
Do not count them toward tally or early-close.

### Joined viewer changes vote
Count only the latest valid vote for that node.

### Everyone votes immediately
Close early and resolve without waiting for timer expiry.

### Not everyone votes
Let the timer expire normally, then resolve with the available votes.

### Vote arrives exactly as the timer closes
Once the runtime stage leaves `decision_open`, ignore late-arriving votes.

### Zero joined users
Recommended v1 behavior: do not open normal story voting at all; end the session when the join phase closes empty.

## Recovery Expectations

For live reliability, runtime state should be sufficient for an operator or later action to determine:
- whether the run is active
- which runtime stage it is in
- which node is active
- who joined
- which votes are currently recorded
- whether the current node is still open or already resolving

This makes manual recovery and operator intervention much safer.

## Non-Goals

This spec does **not** require:
- modifying story JSON shape
- supporting mid-story roster changes in v1
- exposing all state publicly in chat
- random tie-break behavior

## Related Docs

- `session-lifecycle.md` — full run flow from idle → join → node → decision → ending
- `commands.md` — authored decision commands vs runtime session commands
- `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` — technical-agent operating guidance
