# LotAT Engine — Session Lifecycle Spec

## Purpose

This document defines the **runtime session contract** for LotAT live play.

It describes how a LotAT run should behave on stream **without changing the authored story JSON schema**. Use this when planning or reviewing the engine lifecycle in Streamer.bot.

## Scope Boundary

This is an **engine/runtime spec**, not a story-authoring spec.

Belongs here:
- join phase behavior
- participant roster rules
- runtime stages
- timer behavior
- node-entry and decision-window flow
- early-close voting rules
- session-end behavior

Does **not** belong here:
- new story JSON fields
- authored narrative content
- changes to `choices[].command` semantics beyond runtime handling

## Core Principle

Every LotAT session begins with a **join phase** before the first story decision.

Viewers opt into that run with `!join`. The engine tracks that roster and later uses it to decide whether a node's decision window can end early once **all joined participants have voted**.

## Runtime Stage Model

The recommended runtime stage model is:

1. `idle`
   - no active LotAT run

2. `join_open`
   - session exists
   - chat may use `!join`
   - no story decision is active yet

3. `node_intro`
   - engine has entered the current node
   - narration, overlays, and mechanic setup happen here

4. `decision_open`
   - current node choices are active
   - joined participants may submit one of the current node's allowed commands

5. `decision_resolving`
   - voting is closed
   - winner is computed
   - chaos/result logic is applied
   - next node is selected

6. `ended`
   - session reached an ending node or was cancelled/reset

## Start-of-Session Flow

When a LotAT session starts, the engine should:

1. confirm no conflicting active LotAT run is already in progress
2. load the canonical runtime story file
3. initialize session state
4. set runtime stage to `join_open`
5. clear any stale join roster or vote state from the last run
6. announce the join window to chat
7. start the join timer

Recommended chat expectation:
- tell viewers clearly that they must type `!join` during the join window to participate in that mission

## Join Phase Contract

During `join_open`:
- `!join` is accepted
- each unique viewer may join once
- duplicate joins are ignored
- the engine records a participant roster for the current run

Recommended identity rule:
- use `userId` when available
- otherwise fall back to lowercased `user`

## Roster Freeze Rule

Recommended v1 contract:

> The participant roster is finalized when the join phase closes and remains fixed for the rest of the LotAT run.

Why this is preferred:
- prevents the target participant count from changing mid-vote
- keeps early-close logic deterministic
- reduces live confusion and recovery complexity

Implication:
- `!join` is a session-start participation command, not a mid-story opt-in command

## Zero-Join Rule

Recommended v1 contract:

> If the join window closes with zero participants, the LotAT session ends cleanly instead of continuing with an empty roster.

Recommended operator/chat framing:
- announce that no crew joined the mission
- clear session state cleanly
- return to `idle`

This keeps participation explicit and avoids creating a second fallback voting model.

## Node Entry Flow

After a non-empty roster is locked and the join phase closes, the engine should enter the story's `starting_node_id`.

For each node entry, the engine should:
- load the node from the current story definition
- set the current node state
- apply `chaos.on_enter`
- present `read_aloud`
- surface `sfx_hint` as a production hook if needed
- surface optional `crew_focus`
- surface optional `commander_moment`
- surface optional `dice_hook`
- if the node is an ending, transition to session-end handling
- if the node is a stage, open a decision window

## Decision Window Flow

When a stage node opens:
- clear prior node vote state
- derive the allowed commands from the current node's two choices
- set runtime stage to `decision_open`
- announce the current choices
- start the decision timer

Only the current node's allowed commands should count.

## Decision Closure Paths

A decision window may close in exactly two normal ways:

### 1. Timer expiry
The decision timer reaches the end of its configured duration.

### 2. Early close
Every joined participant has submitted a valid vote for the current node.

Both paths should converge into the same resolution step so the result logic stays consistent.

## End-of-Session Flow

A session ends when:
- an ending node is reached
- the session is cancelled manually
- the join phase closes with zero participants
- a reset action explicitly tears the run down

On session end, the engine should:
- stop active LotAT timers
- announce the ending or cancellation state
- clear or reset session-specific globals
- return to a safe inactive stage

## Operator-Friendly Controls to Preserve

For live reliability, the runtime design should keep room for manual controls such as:
- force close join window
- force close decision window
- advance to next node
- cancel session
- inspect current roster
- inspect current votes
- reset LotAT state

These are not story mechanics. They are operator recovery tools.

## Non-Goals

This spec does **not** require:
- new story JSON fields
- changes to authored story structure
- late-join support in v1
- a leave-session command in v1
- C# implementation details in this doc

## Related Docs

- `commands.md` — command categories and runtime `!join` rule
- `state-and-voting.md` — roster, vote, tie-break, and edge-case rules
- `../story-pipeline/json-schema.md` — authored story schema boundary
- `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` — authoritative authored story contract
