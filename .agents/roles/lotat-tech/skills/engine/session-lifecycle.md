# LotAT Engine — Session Lifecycle Runtime Contract

## Purpose

This document defines the **runtime session contract** for Legends of the ASCII Temple (LotAT) live play.

It explains how a LotAT run progresses at runtime in Streamer.bot-adjacent engine logic **without changing the authored story JSON schema**. Future agents should use this doc as scaffolding when implementing, reviewing, or debugging session flow.

## Scope Boundary

This is an **engine/runtime contract**, not authored story content.

Belongs here:
- runtime stage names and allowed transitions
- session-start join behavior
- `!join` handling
- `!roll` handling for pre-vote dice windows
- participant roster creation and freeze rules
- zero-join behavior
- node-entry flow
- decision-window open/close flow
- end-of-session teardown
- operator recovery controls

Does **not** belong here:
- new story JSON fields
- authored narrative text
- per-story overrides for join/vote lifecycle
- changes to `choices[].command` semantics beyond runtime handling

## Core Runtime Principle

Every LotAT session begins with a **join phase** before the first story decision.

Viewers opt into that session with `!join`. The engine records a per-session participant roster and uses that roster for later runtime rules, especially the "all joined participants have voted" early-close condition.

> Runtime contract: join roster state is runtime-owned session state. It is **not** authored into story JSON.

## Canonical Runtime Stages

Recommended stage set for the session state machine:

| Stage | Meaning | Accepts Chat Input? | Exit Conditions |
|---|---|---|---|
| `idle` | No active LotAT session exists. | No LotAT session input should be processed. | Operator starts a session. |
| `join_open` | Session exists and is collecting participants. | `!join` only. Decision commands do not count yet. | Join timer ends, operator force-closes join, or operator cancels session. |
| `node_intro` | Engine has entered a node and is presenting runtime/story output. | No vote-counting yet. | Intro/setup work completes, then either open dice window, open decision window, or end session if node is an ending. |
| `dice_open` | Current node has an enabled dice hook and the pre-vote `!roll` window is live. | `!roll` only; available to all chat, not just joined participants. | First successful roll, timer expiry, operator force-close, or operator cancel. |
| `decision_open` | Current node choices are live and votes may be collected. | Valid current-node decision commands from joined participants. | Timer expires, all joined participants vote, operator force-closes, or operator cancels session. |
| `decision_resolving` | Voting is closed and the engine is determining the winning path. | No new votes should count. | Resolution completes and engine enters next node or ends session. |
| `ended` | Run has finished or been cancelled and is awaiting final cleanup / safe return. | No further session participation should be accepted. | Cleanup completes and engine returns to `idle`. |

## Stage Transition Contract

Normal happy-path flow:

`idle` → `join_open` → `node_intro` → `dice_open` (optional) → `decision_open` → `decision_resolving` → `node_intro` ... → `ended` → `idle`

Exceptional but expected paths:
- `join_open` → `ended` when zero users joined
- `join_open` → `ended` when operator cancels before story start
- `dice_open` → `ended` when operator cancels mid-session
- `decision_open` → `ended` when operator cancels mid-session
- `node_intro` → `ended` when the entered node is an ending node

Recommended rule:
- never skip directly from `decision_open` to a new node without entering `decision_resolving`
- never accept decision votes outside `decision_open`
- never accept `!join` outside `join_open`
- never accept `!roll` outside `dice_open`

## Start-of-Session Contract

When a LotAT session starts, the engine should:

1. confirm no conflicting active LotAT session is already running
2. load the selected / canonical story definition
3. validate that required runtime inputs exist (`story_id`, `starting_node_id`, nodes, choices where applicable)
4. initialize fresh session state
5. clear any stale roster, vote, timer, or branch state from a prior run
6. set runtime stage to `join_open`
7. announce the join phase to chat
8. start the join timer

Recommended chat/operator expectation:
- chat is told clearly to type `!join` during the join window to participate in **this** LotAT mission
- the announcement should make clear that joining is per-session, not permanent

## Join-Phase Contract

During `join_open`:
- `!join` is the only LotAT session command that should be accepted from chat
- each unique viewer may join at most once for that session
- duplicate `!join` attempts do not create duplicate roster entries
- decision commands received during this phase do not count as votes
- the engine should be able to inspect the current roster while the join window is still open

Recommended participant identity rule:
- primary key: `userId`
- fallback key: lowercased `user`

Recommended UX behavior:
- duplicates may be ignored silently or acknowledged lightly, but must not mutate the roster
- the operator should be able to see the roster count and identities if recovery is needed

## `!join` as a Runtime Session Command

`!join` is an **engine-owned runtime session command**.

Contract implications:
- it belongs to runtime lifecycle handling, not authored story choices
- it should **not** appear in `choices[].command`
- it should **not** appear in `commands_used`
- it exists to build the participant roster for the current run

`!join` should be processed only when:
- runtime stage is `join_open`
- the session has not already been cancelled or ended

`!join` should be ignored when:
- stage is `idle`
- stage is `node_intro`
- stage is `decision_open`
- stage is `decision_resolving`
- stage is `ended`

## Participant Roster Creation Contract

The participant roster is created during `join_open`.

Minimum contract:
- roster starts empty at session initialization
- valid `!join` messages append unique participants to the active-session roster
- roster is session-scoped, not global across runs
- roster is the authoritative source for participant eligibility during all later decision windows

The runtime should be able to answer:
- who joined this session
- how many participants are in the session
- whether a given viewer is a joined participant

## Roster Freeze Contract

Recommended v1 rule:

> When the join phase closes, the roster is frozen and remains fixed for the rest of the session.

This is a runtime-only rule. It must not be reintroduced later as a per-story JSON field or per-node authored override unless the schema is intentionally changed.

Why this is the preferred runtime contract:
- early-close logic stays deterministic
- participant targets do not change mid-vote
- live recovery is simpler
- future agents have a clear boundary between session setup and story play

Implications:
- late joiners are not added after the join phase ends
- no mid-story participant churn is assumed in v1
- decision windows should count only the frozen joined roster

## Zero-Join Contract

Recommended v1 rule:

> If the join window closes with zero participants, the session ends cleanly and does not continue into the story.

Required behavior:
- do **not** enter the starting node
- do **not** open a decision window with an empty roster
- announce that no crew joined / no participants entered the mission
- transition to `ended`
- perform normal end-of-session cleanup
- return to `idle`

Reasoning:
- avoids inventing a second fallback voting model
- keeps participation explicit
- reduces runtime ambiguity for future implementation work

## Join Close → Story Start Contract

When the join phase closes with one or more participants:

1. stop the join timer
2. freeze the roster
3. resolve the story's `starting_node_id`
4. set current node state
5. transition to `node_intro`

This is the only normal path from session-start setup into active story progression.

## Node-Entry Contract

Each time the engine enters a node, it should:

1. resolve the node definition by `node_id`
2. set the active node state
3. set runtime stage to `node_intro`
4. clear any stale per-node transient state that should not leak from the prior node
5. apply `chaos.delta`
6. surface `read_aloud`
7. surface `sfx_hint` as a production hook if present
8. surface `crew_focus` if present
9. surface `commander_moment` if enabled
10. inspect `dice_hook` if present
11. inspect `node_type`

Then:
- if `node_type = "ending"`, transition directly into end-of-session handling
- if `node_type = "stage"` and `dice_hook.enabled = true`, open the dice window before normal voting
- if `node_type = "stage"` and `dice_hook.enabled = false`, open a decision window

## Dice-Window Open Contract

When a stage node has `dice_hook.enabled = true`, the engine should:
- validate that `purpose`, `roll_window_seconds`, `success_threshold`, `success_text`, and `failure_text` are present
- set runtime stage to `dice_open`
- announce the roll purpose, success target, and window length to chat
- announce that chat may use `!roll`
- start the dice timer in whole seconds from `roll_window_seconds`
- accept `!roll` from **any viewer in chat**, not just joined participants

Runtime boundaries:
- `!roll` is a runtime-only command and never a story choice
- dice participation does **not** consult the frozen joined roster
- dice success/failure is narrative-only in v1 and does **not** change chaos, branch routing, or later vote eligibility

## Dice-Window Resolution Contract

During `dice_open`:
- each `!roll` generates a random value from 1 to 100
- every roll result should be surfaced to chat in v1
- users may roll repeatedly while the window remains open
- a roll succeeds when `roll >= success_threshold`
- the **first** successful roll closes the dice window immediately as a success
- if the timer expires before any successful roll occurs, the dice window resolves as a failure
- if nobody rolls at all, that still resolves as a failure

On dice success:
1. stop the dice timer
2. lock out further `!roll` handling for that node
3. surface the authored `success_text` for operator read-aloud
4. proceed into the normal decision-window open contract for that same node

On dice failure:
1. stop the dice timer if still active
2. lock out further `!roll` handling for that node
3. surface the authored `failure_text` for operator read-aloud
4. proceed into the normal decision-window open contract for that same node

Operator note:
- the current v1 contract treats dice success as anonymous crowd success; the engine does not need to preserve a named "winner" for branching purposes

## Decision-Window Open Contract

When a stage node opens for voting, the engine should:
- derive the allowed commands from the current node's `choices`
- clear prior-node vote state
- initialize a fresh vote map for the active node
- set runtime stage to `decision_open`
- announce the available choices clearly to chat
- start the decision timer

Only the active node's allowed commands should count as valid votes.

Recommended rule:
- the active node vote map is per-node state and must not carry forward implicitly from earlier nodes

## Decision Eligibility Contract

A vote counts only when all of the following are true:
- runtime stage is `decision_open`
- the viewer is in the frozen roster for this session
- the submitted command matches one of the current node's allowed commands
- the vote arrives before the decision window closes

A vote does **not** count when:
- the viewer never joined
- the command is not valid for the active node
- the window is already closed
- the session is not in `decision_open`

## Decision-Window Close Contract

A decision window may close through exactly these normal paths:

### 1. Timer expiry
The configured decision timer ends before all joined participants vote.

### 2. Early close
All joined participants have submitted a valid vote for the current node.

### 3. Operator force-close
The operator manually closes the decision window for recovery or pacing reasons.

All three paths should converge into the same resolution flow.

## Early-Close Contract

Recommended v1 rule:

> If every joined participant currently in the frozen roster has submitted a valid vote for the active node, the engine may close the decision window immediately.

This is the same runtime early-close condition referenced in `commands.md` and `state-and-voting.md`: the denominator is the frozen joined roster for the current session, not all chatters and not a story-authored value.

Required consequences:
- stop the decision timer
- prevent new votes from being counted
- transition to `decision_resolving`
- use the normal vote-resolution path

This rule depends on the frozen roster and should not be implemented against all chatters generally.

## Decision-Resolving Contract

When the window closes, the engine should:

1. set runtime stage to `decision_resolving`
2. lock out new votes for the active node
3. tally only the current node's recorded valid votes
4. resolve ties according to the voting contract (`state-and-voting.md` currently recommends earliest matching choice in `choices` order)
5. select the winning authored choice
6. emit that choice's `result_flavor`
7. transition to the winning `next_node_id`
8. enter the next node through the normal node-entry contract

In v1, do **not** classify stage resolution as success/failure for chaos purposes. The engine should use the authored ending node's `end_state` (`"success"`, `"partial"`, or `"failure"`) only when that ending is actually reached.

Recommended safety rule:
- there should be exactly one resolution pass per node

## End-of-Session Contract

A session ends when:
- an ending node is reached
- the join phase closes with zero participants
- the operator cancels the session
- a reset action explicitly tears the session down
- unrecoverable runtime validation failure requires safe abort

On session end, the engine should:
1. set runtime stage to `ended`
2. stop any active LotAT timers
3. announce the ending, cancellation, or zero-join outcome as appropriate
4. persist any final inspection snapshot needed for debugging/operator visibility
5. clear or reset session-specific state
6. return to a safe inactive condition
7. transition back to `idle`

Minimum state to clear/reset before returning to `idle`:
- active-session flag
- join-phase state
- participant roster / count
- current node vote map
- current allowed commands
- active node identifier if session-scoped
- any timer state specific to the run

## Operator Recovery Controls

These controls are runtime/operator tools, not story mechanics. The contract should leave room for them even before implementation details are finalized.

Recommended recovery controls:
- **force close join window** — stop join collection and move to zero-join handling or story start based on roster count
- **force close dice window** — resolve the active node's dice window as failure and continue into the normal decision window
- **force close decision window** — stop voting and enter normal resolution immediately
- **advance to next node** — move forward manually when recovery requires bypassing normal pacing
- **cancel session** — abort the run safely and move to teardown
- **inspect current roster** — show who joined and how many participants are locked for the session
- **inspect current votes** — show current valid vote map for the active node
- **reset LotAT state** — clear LotAT runtime state after interruption or partial failure

Recommended operator assumptions:
- these controls must be safe to use live on stream
- they should not require story-schema changes
- they should operate against runtime state only

## Runtime Assumptions Locked by This Contract

This document assumes:
- every LotAT session begins in `idle` and must pass through `join_open`
- `!join` is the session-start participation command
- roster creation happens only during `join_open`
- roster freezes when join closes
- zero joins ends the session instead of starting story playback
- every non-ending node is introduced through `node_intro`
- nodes with enabled dice hooks open `dice_open` before voting
- `!roll` is accepted only during `dice_open`
- every decision window opens through `decision_open`
- every decision window closes into `decision_resolving`
- only joined participants count toward early-close
- dice-hook resolution is narrative-only in v1
- session lifecycle rules are runtime-owned, not story-authored

## Non-Goals

This contract does **not** require:
- new story JSON fields
- story-level configuration of join behavior in v1
- late-join support in v1
- a leave-session command in v1
- C# implementation details in this doc

## Related Docs

- `commands.md` — command categories and the `!join` runtime boundary
- `state-and-voting.md` — roster, vote, tie-break, and edge-case rules
- `docs-map.md` — navigation guide for engine docs
- `../story-pipeline/json-schema.md` — authored story schema boundary
- `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` — authoritative authored story contract
