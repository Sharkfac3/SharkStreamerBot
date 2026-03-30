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
- commander-input handling for pre-vote commander windows
- `!roll` handling for pre-vote dice windows
- participant roster creation and freeze rules
- timer ownership, defaults, and reset expectations
- zero-join behavior
- unresolved timeout behavior
- node-entry flow
- decision-window open/close flow
- fault-abort behavior
- end-of-session teardown

Does **not** belong here:
- new story JSON fields
- authored narrative text
- per-story overrides for join/vote lifecycle
- operator babysitting workflows for normal play
- changes to `choices[].command` semantics beyond runtime handling

## Core Runtime Principle

Every LotAT session begins with a **join phase** before the first story decision.

Viewers opt into that session with `!join`. The engine records a per-session participant roster and uses that roster for later runtime rules, especially the "all joined participants have voted" early-close condition.

> Runtime contract: join roster state is runtime-owned session state. It is **not** authored into story JSON.

> Runtime loading contract: in v1, the engine reads exactly one runtime story file — `Creative/WorldBuilding/Storylines/loaded/current-story.json`. It does **not** scan `ready/` or choose among multiple candidate stories at session start.

## V1 Timer Contract

LotAT v1 has exactly four timer concepts:
- **join window** — runtime-owned, fixed global default of **120 seconds**
- **decision window** — runtime-owned, fixed global default of **120 seconds**
- **commander window** — node-authored via `commander_moment.window_seconds`
- **dice window** — node-authored via `dice_hook.roll_window_seconds`

Rules:
- join and decision timing are **not** authored per story or per node in v1
- operator live override of join/decision duration is **not** part of v1
- timer durations do **not** belong in `Actions/SHARED-CONSTANTS.md`; only timer names should be shared there when implementation begins
- recommended implementation model is a **hybrid**: Streamer.bot named timers drive timer-end actions, while runtime stage/window state prevents stale timer events from mutating the session after state has already advanced
- stream-start / reset behavior should always disable all LotAT timers and clear LotAT runtime window state back to `idle`

## Canonical v1 Runtime State Buckets

To remove ambiguity before C# implementation starts, v1 runtime state should be divided into four buckets:

### 1. Session-level state
Must exist for every active run:
- `lotat_active`
- `lotat_session_id`
- `lotat_session_stage`
- `lotat_session_story_id`
- `lotat_session_current_node_id`
- `lotat_session_chaos_total`
- `lotat_session_roster_frozen`
- `lotat_session_joined_roster_json`
- `lotat_session_joined_count`

### 2. Node/window-level state
Must exist while a node is active:
- `lotat_node_active_window`
- `lotat_node_window_resolved`
- `lotat_node_allowed_commands_json`

Conditional window state:
- commander window: `lotat_node_commander_name`, `lotat_node_commander_target_user`, `lotat_node_commander_allowed_commands_json`
- dice window: `lotat_node_dice_success_threshold`

### 3. History-level state
Keep this minimal in v1:
- `lotat_session_last_choice_id`
- optionally `lotat_session_last_end_state`

### 4. Recovery-level state
Deep recovery state is intentionally deferred until after the first working implementation.
For v1, logs plus the active runtime globals above are considered sufficient.

Implementation posture for v1:
- prefer explicit scalar globals over one large runtime blob
- use JSON-packed globals only for structured collections such as roster, allowed commands, and vote maps
- treat the roster / allowed-command / vote-map JSON shapes documented in the engine runtime docs as canonical v1 contract
- use a **lowercase username/login string** as the canonical runtime identity wherever LotAT stores or compares a user
- optimize for normal action-to-action continuity, not crash recovery
- do not require recovery snapshots beyond logs in v1

## Canonical Runtime Stages

Recommended stage set for the session state machine:

| Stage | Meaning | Accepts Chat Input? | Exit Conditions |
|---|---|---|---|
| `idle` | No active LotAT session exists. | No LotAT session input should be processed. | Start trigger begins a session. |
| `join_open` | Session exists and is collecting participants. | `!join` only. Decision commands do not count yet. | Join timer ends and either starts story play or ends the run with zero joins; unrecoverable fault aborts the run. |
| `node_intro` | Engine has entered a node and is presenting runtime/story output. | No vote-counting yet. | Intro/setup work completes, then either open commander window, open dice window, open decision window, end session if node is an ending, or abort on unrecoverable fault. |
| `commander_open` | Current node has an enabled commander moment and the pre-vote commander-input window is live. | Only the mapped commander-input commands from the snapshotted assigned commander user should count. | First valid commander input, timer expiry, silent skip, or unrecoverable fault abort. |
| `dice_open` | Current node has an enabled dice hook and the pre-vote `!roll` window is live. | `!roll` only; available to all chat, not just joined participants. | First successful roll, timer expiry, or unrecoverable fault abort. |
| `decision_open` | Current node choices are live and votes may be collected. | Valid current-node decision commands from joined participants. | Timer expires, all joined participants vote, unresolved end on zero valid votes, or unrecoverable fault abort. |
| `decision_resolving` | Voting is closed and the engine is determining the winning path. | No new votes should count. | Resolution completes and engine enters next node, ends unresolved, or aborts on unrecoverable fault. |
| `ended` | Run has finished, fault-aborted, or ended unresolved and is awaiting final cleanup / safe return. | No further session participation should be accepted. | Cleanup completes and engine returns to `idle`. |

## Stage Transition Contract

Normal happy-path flow:

`idle` → `join_open` → `node_intro` → `commander_open` (optional) / `dice_open` (optional) → `decision_open` → `decision_resolving` → `node_intro` ... → `ended` → `idle`

Exceptional but expected paths:
- `join_open` → `ended` when zero users joined
- `decision_open` → `ended` when the decision timer closes with zero valid votes and the run ends unresolved
- any active stage → `ended` when an unrecoverable runtime/code fault requires safe abort
- `node_intro` → `ended` when the entered node is an ending node

Recommended rule:
- never skip directly from `decision_open` to a new node without entering `decision_resolving`
- never accept decision votes outside `decision_open`
- never accept `!join` outside `join_open`
- never accept commander-input commands outside `commander_open`
- never accept `!roll` outside `dice_open`

## Start-of-Session Contract

When a LotAT session starts, the engine should:

1. confirm no conflicting active LotAT session is already running
2. load **only** the canonical runtime story file at `Creative/WorldBuilding/Storylines/loaded/current-story.json`
3. perform only the bare-minimum runtime load checks needed to start safely in v1 (`file exists`, `JSON parses`, core runtime fields such as `story_id`, `starting_node_id`, and `nodes` are present enough to begin)
4. if any of those minimal load checks fail, abort the start attempt immediately
5. on abort, log the failure verbosely, send the fixed generic chat-safe fault message, trigger the generic Mix It Up failure alert, and leave the runtime in `idle`
6. initialize fresh session state
7. clear any stale roster, vote, timer, or branch state from a prior run
8. set runtime stage to `join_open`
9. announce the join phase to chat
10. start the join timer using the fixed v1 runtime default of **120 seconds**

Recommended v1 expectation:
- chat is told clearly to type `!join` during the join window to participate in **this** LotAT mission
- the announcement should make clear that joining is per-session, not permanent
- the engine is expected to run unattended after the start trigger in normal use
- the join phase must never open if the runtime story file is missing, malformed, or minimally unusable

## Join-Phase Contract

During `join_open`:
- `!join` is the only LotAT session command that should be accepted from chat
- each unique viewer may join at most once for that session
- duplicate `!join` attempts do not create duplicate roster entries
- decision commands received during this phase do not count as votes
- the engine should keep the current roster available in runtime state while the join window is still open
- the window length is a fixed **120 seconds** in v1 and is not story-authored

Canonical participant identity rule for v1:
- store participant identity as the **lowercased username/login string**
- use that same lowercased username/login form everywhere LotAT compares users at runtime, including roster membership, vote-map keys, and commander-target comparison
- do not use `userId` as the canonical stored participant key in v1

Recommended UX behavior:
- duplicates may be ignored silently or acknowledged lightly, but must not mutate the roster
- no operator inspection workflow is required in v1 normal use

## `!join` as a Runtime Session Command

`!join` is an **engine-owned runtime session command**.

Contract implications:
- it belongs to runtime lifecycle handling, not authored story choices
- it should **not** appear in `choices[].command`
- it should **not** appear in `commands_used`
- it exists to build the participant roster for the current run

`!join` should be processed only when:
- runtime stage is `join_open`
- the session has not already ended

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

When the join phase closes with zero participants:
1. stop the join timer
2. transition to `ended`
3. announce the non-error zero-join outcome
4. clear session state and return to `idle`

This zero-join path is a normal session end, not a runtime fault.

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
9. inspect `commander_moment` if present
10. inspect `dice_hook` if present
11. inspect `node_type`

Then:
- if `node_type = "ending"`, transition directly into end-of-session handling
- if `node_type = "stage"` and `commander_moment.enabled = true`, open the commander window before normal voting
- if `node_type = "stage"` and `dice_hook.enabled = true`, open the dice window before normal voting
- if `node_type = "stage"` and neither pre-vote mechanic is enabled, open a decision window

V1 guardrail:
- a node may not enable both `commander_moment` and `dice_hook`

## Commander-Window Open Contract

When a stage node has `commander_moment.enabled = true`, the engine should:
- validate that `commander`, `prompt`, `window_seconds`, and `success_text` are present
- validate that the commander name maps to a known commander-input command set
- validate that `dice_hook.enabled` is not also true on the same node in v1
- resolve the currently assigned user for that commander slot from runtime/shared commander state
- if no assigned commander user exists, log the skip for debugging visibility and continue silently into the normal decision window
- snapshot the assigned commander identity at window open so the target user cannot change mid-window
- set runtime stage to `commander_open`
- surface the authored `prompt` as the commander call-to-action
- announce the commander window length to chat
- start the commander timer in whole seconds from authored `window_seconds`
- accept only the mapped commander-input commands from the snapshotted assigned commander user

Runtime boundaries:
- commander-input commands are existing commander feature commands reused by LotAT; they are runtime-only and never story choices
- commander participation does **not** consult the frozen joined roster
- commander-moment success/failure is narrative-only in v1 and does **not** change chaos, branch routing, vote eligibility, or later vote resolution
- non-assigned users typing commander commands during the window are ignored silently

## Commander-Window Resolution Contract

During `commander_open`:
- the first valid mapped commander-input command from the snapshotted assigned commander user closes the commander window immediately as a success
- the exact valid command used does **not** change branching in v1
- if the timer expires before any valid assigned-commander input occurs, the commander window resolves silently as a skip/failure
- if the assigned commander is offline, absent, or never responds, the commander window still resolves silently as a skip/failure

On commander success:
1. stop the commander timer
2. lock out further commander-input handling for that node
3. surface the authored `success_text` for normal runtime output
4. proceed into the normal decision-window open contract for that same node

On commander skip/failure:
1. stop the commander timer if still active
2. lock out further commander-input handling for that node
3. emit no chat-facing failure text in v1
4. proceed into the normal decision-window open contract for that same node

## Dice-Window Open Contract

When a stage node has `dice_hook.enabled = true`, the engine should:
- validate that `purpose`, `roll_window_seconds`, `success_threshold`, `success_text`, and `failure_text` are present
- set runtime stage to `dice_open`
- announce the roll purpose, success target, and window length to chat
- announce that chat may use `!roll`
- start the dice timer in whole seconds from authored `roll_window_seconds`
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
3. surface the authored `success_text` for normal runtime output
4. proceed into the normal decision-window open contract for that same node

On dice failure:
1. stop the dice timer if still active
2. lock out further `!roll` handling for that node
3. surface the authored `failure_text` for normal runtime output
4. proceed into the normal decision-window open contract for that same node

Implementation note:
- the current v1 contract treats dice success as anonymous crowd success; the engine does not need to preserve a named "winner" for branching purposes

## Decision-Window Open Contract

When a stage node opens for voting, the engine should:
- derive the allowed commands from the current node's `choices`
- clear prior-node vote state
- initialize a fresh vote map for the active node
- set runtime stage to `decision_open`
- announce the available choices clearly to chat
- start the decision timer using the fixed v1 runtime default of **120 seconds**

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

These normal close paths then split into one of two outcomes:
- if at least one valid vote exists, continue into the normal resolution flow
- if zero valid votes exist when the timer closes, end the run as unresolved in v1

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

When the window closes with one or more valid votes, the engine should:

1. set runtime stage to `decision_resolving`
2. lock out new votes for the active node
3. tally only the current node's recorded valid votes
4. resolve ties according to the voting contract (`state-and-voting.md` currently recommends earliest matching choice in `choices` order)
5. select the winning authored choice
6. emit that choice's `result_flavor`
7. transition to the winning `next_node_id`
8. enter the next node through the normal node-entry contract

If the decision timer closes with zero valid votes in v1:
1. stop the decision timer if still active
2. transition to `ended`
3. announce the unresolved session outcome
4. clear session state and return to `idle`

In v1, do **not** classify stage resolution as success/failure for chaos purposes. The engine should use the authored ending node's `end_state` (`"success"`, `"partial"`, or `"failure"`) only when that ending is actually reached.

Recommended safety rule:
- there should be exactly one resolution pass per node
- unresolved zero-vote shutdown should not attempt to invent a fallback winner

## End-of-Session Contract

A session ends when:
- an ending node is reached
- the join phase closes with zero participants
- the decision timer closes with zero valid votes and the run ends unresolved
- unrecoverable minimal runtime story-load failure requires safe abort before play begins
- any later unrecoverable runtime/code fault requires a safe abort during play

On session end, the engine should:
1. set runtime stage to `ended`
2. stop any active LotAT timers
3. announce the normal ending, zero-join outcome, unresolved outcome, or fault-abort outcome as appropriate
4. log any fault details needed for debugging
5. trigger the generic Mix It Up failure alert only for fault-abort outcomes
6. clear or reset session-specific state
7. return to a safe inactive condition
8. transition back to `idle`

Minimum state to clear/reset before returning to `idle`:
- `lotat_active`
- `lotat_session_id`
- `lotat_session_stage`
- `lotat_session_story_id`
- `lotat_session_current_node_id`
- `lotat_session_chaos_total`
- `lotat_session_roster_frozen`
- `lotat_session_joined_roster_json`
- `lotat_session_joined_count`
- `lotat_node_active_window`
- `lotat_node_window_resolved`
- `lotat_node_allowed_commands_json`
- `lotat_vote_map_json`
- `lotat_vote_valid_count`
- any conditional commander/dice globals for the active node
- minimal history globals if implementation created them

## V1 Unattended Fail-Closed Contract

LotAT v1 is designed to run unattended after a single start trigger.

That means:
- normal timer behavior is part of the primary runtime contract, not an operator-recovery workflow
- there are no planned operator force-close, inspect, manual-advance, or emergency-exit controls in v1
- commander timeout, missing commander assignment, dice timeout, and decision timeout with partial votes are all normal runtime outcomes
- decision timeout with zero valid votes ends the run as unresolved
- true runtime/code faults end the run immediately through a safe fault-abort path

Fault-abort expectations in v1:
- send one fixed generic chat-safe fault message
- trigger one generic Mix It Up failure alert
- write detailed cause information to logs only
- disable all LotAT timers
- clear LotAT runtime state
- return to `idle`

Implementation posture:
- prefer deterministic continuation when the contract already defines a normal timeout outcome
- prefer safe session termination over speculative recovery when runtime state is invalid or code execution fails
- do not add new story-schema fields to support unattended failure handling

## Runtime Assumptions Locked by This Contract

This document assumes:
- every LotAT session begins in `idle` and must pass through `join_open`
- the engine reads exactly one runtime story file in v1: `Creative/WorldBuilding/Storylines/loaded/current-story.json`
- the engine does not scan `ready/` directly
- a single start trigger is the only planned operator control for v1
- `!join` is the session-start participation command
- the join window duration is a fixed runtime-owned 120 seconds in v1
- roster creation happens only during `join_open`
- roster freezes when join closes if one or more participants joined
- zero joins ends the session as a normal non-error outcome instead of starting story playback
- the join phase does not begin unless the runtime story file loads and passes bare-minimum runtime start checks
- full schema/contract validation belongs primarily to story generation and operator review tooling, not a second full engine pass at session start
- every non-ending node is introduced through `node_intro`
- nodes with enabled commander moments open `commander_open` before voting
- commander-input commands are accepted only during `commander_open`
- commander-window duration comes from authored `commander_moment.window_seconds`
- missing commander assignment or commander timeout continues through the normal runtime path in v1
- nodes with enabled dice hooks open `dice_open` before voting
- `!roll` is accepted only during `dice_open`
- dice-window duration comes from authored `dice_hook.roll_window_seconds`
- dice timeout continues through the normal runtime path in v1
- commander moments and dice hooks do not coexist on the same node in v1
- every decision window opens through `decision_open`
- the decision window duration is a fixed runtime-owned 120 seconds in v1
- a decision timer with one or more valid votes closes into `decision_resolving`
- a decision timer with zero valid votes ends the run unresolved in v1
- only joined participants count toward early-close
- commander-moment resolution is narrative-only in v1
- dice-hook resolution is narrative-only in v1
- true runtime/code faults fail closed: generic chat message, generic Mix It Up failure alert, logs-only details, teardown, return to `idle`
- session lifecycle rules are runtime-owned, not story-authored
- stream-start / reset should disable all LotAT timers and clear runtime window state before returning to `idle`

## Non-Goals

This contract does **not** require:
- new story JSON fields
- story-level configuration of join behavior in v1
- late-join support in v1
- a leave-session command in v1
- operator force-close / inspect / advance controls in v1
- C# implementation details in this doc

## Related Docs

- `commands.md` — command categories and the `!join` runtime boundary
- `state-and-voting.md` — roster, vote, tie-break, and edge-case rules
- `docs-map.md` — navigation guide for engine docs
- `../story-pipeline/json-schema.md` — authored story schema boundary
- `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` — authoritative authored story contract
