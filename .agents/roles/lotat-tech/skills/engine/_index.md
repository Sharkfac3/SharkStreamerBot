# LotAT Engine — Overview

## What the Engine Does

The C# engine running in Streamer.bot actions under `Actions/LotAT/`:
- Loads and parses the single v1 runtime story file at `Creative/WorldBuilding/Storylines/loaded/current-story.json`
- Refuses to start a session if that runtime file is missing, malformed, or minimally unusable
- Opens a session-start join phase and registers `!join` participants only after runtime story load succeeds
- Tracks current node state
- Handles chat command routing to story mechanics
- Advances the story based on chat votes/commands
- Closes a decision window early when all joined participants have voted
- Manages the Chaos Meter
- Triggers Mix It Up overlays for narration/effects

## Engine Design Principles

- Story content is **data** (JSON), not **code** — the engine should not contain hardcoded story text
- Engine is story-agnostic — any valid JSON story file should run without engine modifications
- V1 runtime uses a **single-source-of-truth file**: `Creative/WorldBuilding/Storylines/loaded/current-story.json`
- The engine must not scan `ready/` or choose among multiple stories at runtime
- Chat commands are the primary interaction surface — see `commands.md` for supported set
- Chaos Meter state is tracked in global variables and must survive action re-entry
- Runtime contract docs in this folder describe behavior boundaries; implementation work in `Actions/LotAT/` should follow them rather than move those rules into ad hoc script-specific docs
- V1 timing is split cleanly by ownership: join and decision windows are fixed runtime defaults, while commander and dice windows are authored per node in story JSON
- Recommended implementation model is a hybrid: Streamer.bot named timers trigger window-close actions, and runtime stage/window state guards make stale timer fires harmless
- V1 recovery posture is unattended and fail-closed: normal mechanic timeouts continue through deterministic runtime rules, while true runtime/code faults abort the session safely
- Full contract/schema validation belongs primarily to the story-writing / review tooling; session-start runtime checks should stay minimal-safe and live-safety-focused
- Engine-breaking malformed nodes, graph defects, invalid commands, duplicate IDs, and mechanic-payload errors should be rejected upstream before runtime handoff instead of relying on engine-side validation in v1
- If a malformed story condition still slips through and causes a live runtime fault, the engine should fail closed: generic chat-safe message, generic Mix It Up failure alert, detailed logs, teardown, return to `idle`

## Current Status

The v1 engine is implemented. All scripts live under `Actions/LotAT/`:

| Script | Responsibility |
|---|---|
| `lotat-start-main.cs` | Session start — load story, open join phase |
| `lotat-join.cs` | Process `!join` during join phase |
| `lotat-join-timeout.cs` | Join timer expiry — freeze roster or end zero-join session |
| `lotat-node-enter.cs` | Enter a node — apply chaos, surface narration, open mechanic windows |
| `lotat-commander-input.cs` | Process commander-input commands during `commander_open` |
| `lotat-commander-timeout.cs` | Commander timer expiry — skip silently, open decision window |
| `lotat-dice-roll.cs` | Process `!roll` during `dice_open` |
| `lotat-dice-timeout.cs` | Dice timer expiry — resolve failure, open decision window |
| `lotat-decision-input.cs` | Process chat votes during `decision_open` |
| `lotat-decision-resolve.cs` | Tally votes, advance to next node or end session |
| `lotat-decision-timeout.cs` | Decision timer expiry — resolve or end unresolved |
| `lotat-end-session.cs` | Shared teardown — clear state, disable timers, return to `idle` |
| `overlay-publish.cs` | Shared overlay publish helpers — copy `PublishLotat*` methods into each script |

## Runtime Session Spec — Start Here

If your question is about how a live LotAT run behaves before or between any future C# implementation details, read these docs in order:
1. `docs-map.md` — navigation entry point
2. `session-lifecycle.md` — canonical runtime session flow spec
3. `state-and-voting.md` — canonical runtime roster/voting spec
4. `commands.md` — runtime vs authored command boundary

These docs define the **runtime contract**. Future Streamer.bot scripts should implement them, not silently redefine them.

## State Variables

All LotAT engine state in `Actions/SHARED-CONSTANTS.md`:
- `lotat_active` — session running flag for LotAT itself; do not define this as an offering toggle
- `lotat_announcement_sent` — existing experimental offering-system latch, not part of the active LotAT v1 runtime contract
- `lotat_offering_steal_chance` — legacy / provisional offering variable, not part of the active LotAT v1 runtime contract
- `lotat_steal_multiplier` — legacy / provisional offering variable, not part of the active LotAT v1 runtime contract
- `boost_*` — external boost-system state, not LotAT v1 engine state

## Offering Boundary for v1

The existing `!offering` command and `Actions/Squad/offering.cs` are currently **out of scope** for LotAT v1.

Until a future contract decision says otherwise:
- LotAT runtime docs should not assume offering effects
- LotAT engine implementation should not depend on offering globals or boost state
- authored story JSON should not contain offering hooks, flags, or node logic
- future agents should treat the current offering script as separate experimentation rather than an approved LotAT mechanic

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

## Overlay Integration

An overlay visual rendering layer now exists in `Apps/stream-overlay/`.
The engine does not need to know about the overlay internals — it simply
publishes `lotat.*` broker messages at the right lifecycle moments and the
overlay renders them.

### Engine → Overlay integration boundary

The engine **publishes** broker messages; the overlay **subscribes** and renders.
No reverse communication exists — the overlay never sends back to the engine.

### Messages the engine must publish

All methods are provided in `Actions/LotAT/overlay-publish.cs`.
Copy the relevant `PublishLotat*` method into each engine script.

| When | Method to call |
|---|---|
| Session initialised, join timer started | `PublishLotatSessionStart` + `PublishLotatJoinOpen` |
| Valid !join processed | `PublishLotatJoinUpdate` |
| Join timer fires | `PublishLotatJoinClose` |
| Node entered | `PublishLotatNodeEnter` + `PublishLotatChaosUpdate` (if delta > 0) |
| Node has commander moment | `PublishLotatCommanderOpen` (after node.enter) |
| Commander responds | `PublishLotatCommanderClose` (outcome: success) |
| Commander timer expires | `PublishLotatCommanderClose` (outcome: skipped) |
| Node has dice hook | `PublishLotatDiceOpen` (after node.enter) |
| Viewer !rolls | `PublishLotatDiceRoll` |
| Dice succeeds or timer expires | `PublishLotatDiceClose` |
| Decision window opens | `PublishLotatVoteOpen` |
| Valid vote cast | `PublishLotatVoteCast` |
| Decision resolves | `PublishLotatVoteClose` |
| Session ends (any reason) | `PublishLotatSessionEnd` |

### Payload contract

All payload shapes are defined in `Apps/stream-overlay/packages/shared/src/protocol.ts`.
The engine should not invent new fields — only the fields defined there are consumed by the overlay.

### Timing note

`PublishLotatNodeEnter` should be called **before** `PublishLotatCommanderOpen`
or `PublishLotatDiceOpen` on the same node, so the overlay can display the
narration text first and then transition into the mechanic panel.

### Full rendering doc

`App-dev` skill: `.agents/roles/app-dev/skills/stream-interactions/lotat-rendering.md`

---

## Sub-Skills

- `docs-map.md` — high-level navigation map for the LotAT engine docs in this folder
- `commands.md` — supported chat commands and engine constraints
- `session-lifecycle.md` — canonical runtime session contract: stages, join flow, roster freeze, zero-join handling, unresolved/fault-abort handling, node-entry flow, decision flow, and teardown
- `state-and-voting.md` — participant roster, vote storage, early-close rules, tie-break behavior, timeout behavior, and edge cases
