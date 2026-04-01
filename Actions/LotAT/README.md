# Actions/LotAT

## Purpose

`Actions/LotAT/` contains the current Streamer.bot runtime engine for **Legends of the ASCII Temple (LotAT) v1**.

This README is an **internal runtime/developer reference**, not the human operator setup guide.
For Streamer.bot setup steps, use:
- `humans/setup-info/lotat-streamerbot-setup.md`

This implementation is the live session layer only:
- start a LotAT run
- open and close the `!join` window
- freeze the participant roster
- enter story nodes from the loaded runtime JSON
- optionally run commander or dice pre-vote windows
- collect and resolve authored decision-command votes
- end and clean up the session safely

This README documents **what the checked-in C# actually does right now**. It is not a promise of future behavior beyond the current scripts.

---

## Current implementation scope

Implemented in the current `Actions/LotAT/*.cs` set:
- voice-triggered session bootstrap
- minimal runtime story load checks against the single loaded story file
- 120-second join window using `!join`
- session-scoped joined-roster tracking with lowercase usernames
- join close handling for:
  - zero-join clean end
  - non-zero-join roster freeze and starting-node handoff
- node-entry orchestration
- chaos accumulation from `chaos.delta`
- stage-node intro chat output
- commander moments before voting
- dice moments before voting
- 120-second decision window for authored commands
- joined-user-only decision voting
- latest-valid-vote replacement per joined user
- early-close when all joined users have valid votes
- deterministic resolution by earliest authored choice in tie order
- centralized session teardown for success / partial / failure / zero_join / unresolved / fault_abort

Not implemented as part of LotAT v1 runtime behavior here:
- `!offering` integration
- boost-state integration
- operator force-close / manual advance / inspection tools
- full schema or graph validation pass at runtime start
- Mix It Up-heavy polish beyond chat-safe runtime messaging
- robust repo-root discovery beyond hardcoded candidate-path probing in the scripts

Important current implementation note:
- the runtime uses **exactly one story file**: `Creative/WorldBuilding/Storylines/loaded/current-story.json`
- the engine does **not** scan `ready/`

---

## Action inventory

| File | Current purpose |
|---|---|
| `lotat-start-main.cs` | Start a session from a voice trigger, minimally load the runtime story, clear stale state, open join window |
| `lotat-join.cs` | Accept `!join` during `join_open` and update joined roster/count |
| `lotat-join-timeout.cs` | Close join window, end on zero joiners, or freeze roster and hand off to starting node |
| `lotat-node-enter.cs` | Enter a node, apply chaos, send intro text, and route to ending / commander / dice / decision flow |
| `lotat-commander-input.cs` | Shared handler for commander commands during `commander_open` |
| `lotat-commander-timeout.cs` | Commander timer-end handler; silently skip to normal voting |
| `lotat-dice-roll.cs` | Shared `!roll` handler during `dice_open` |
| `lotat-dice-timeout.cs` | Dice timer-end handler; emit failure text and continue to normal voting |
| `lotat-decision-input.cs` | Shared authored decision-command vote intake during `decision_open` |
| `lotat-decision-timeout.cs` | Decision timer-end handler; unresolved end on zero valid votes or handoff to resolve |
| `lotat-decision-resolve.cs` | Tally votes, apply tie-break, emit `result_flavor`, store winning choice, set next node |
| `lotat-end-session.cs` | Shared terminal cleanup back to idle |

---

## Expected trigger/input for each action

### `lotat-start-main.cs`
- **Expected trigger:** voice command trigger only
- **Input:** none required
- **Important guard:** refuses to start if `lotat_active` is already true
- **Story load:** performs minimal-safe load checks only

### `lotat-join.cs`
- **Expected trigger:** `!join`
- **Input:** `user` preferred, `userName` fallback
- **Important guard:** only counts during `join_open` while active window is still `join`

### `lotat-join-timeout.cs`
- **Expected trigger:** timer end for `LotAT - Join Window`
- **Input:** none required
- **Important guard:** stale timer fires no-op if stage/window already moved on

### `lotat-node-enter.cs`
- **Expected trigger:** internal follow-up after join close or decision resolve
- **Input:** optional `input0 = <node_id>`; otherwise uses `lotat_session_current_node_id`
- **Important guard:** fail-closes if node resolution or node payload checks fail

### `lotat-commander-input.cs`
- **Expected trigger:** shared routing for:
  - `!stretch`
  - `!shrimp`
  - `!hydrate`
  - `!orb`
  - `!checkchat`
  - `!toad`
- **Input:** sender from `user` / `userName`, command text from `command` with fallback to `message` / `rawInput`
- **Important guard:** only the snapshotted assigned commander user may satisfy the moment

### `lotat-commander-timeout.cs`
- **Expected trigger:** timer end for `LotAT - Commander Window`
- **Input:** none required
- **Important guard:** stale timer fires no-op safely

### `lotat-dice-roll.cs`
- **Expected trigger:** `!roll`
- **Input:** sender from `user` / `userName`
- **Important guard:** only counts during `dice_open`
- **Participation rule:** any chatter may roll; joined-roster check is intentionally skipped

### `lotat-dice-timeout.cs`
- **Expected trigger:** timer end for `LotAT - Dice Window`
- **Input:** none required
- **Important guard:** stale timer fires no-op safely

### `lotat-decision-input.cs`
- **Expected trigger:** shared routing for authored decision commands:
  - `!scan`
  - `!target`
  - `!analyze`
  - `!reroute`
  - `!deploy`
  - `!contain`
  - `!inspect`
  - `!drink`
  - `!simulate`
- **Input:** sender from `user` / `userName`, command text from `command` with fallback to `message` / `rawInput`
- **Important guard:** only joined users in the frozen roster may cast counted votes

### `lotat-decision-timeout.cs`
- **Expected trigger:** timer end for `LotAT - Decision Window`
- **Input:** none required
- **Important guard:** stale timer fires no-op safely

### `lotat-decision-resolve.cs`
- **Expected trigger:** immediately after decision-input early-close or decision-timeout handoff
- **Input:** none required in current code
- **Important guard:** no-ops unless decision window is already marked resolved

### `lotat-end-session.cs`
- **Expected trigger:** after any action that may leave stage = `ended`
- **Input:** optional `input0 = <endReason>`; current runtime usually relies on `lotat_session_last_end_state`
- **Important guard:** safe to include after non-terminal actions because it no-ops when the session is not ending

---

## Required runtime globals

These names match the current checked-in scripts and `Actions/SHARED-CONSTANTS.md`.

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

### Current safe idle/default values used by the runtime
- booleans: `false`
- counts / thresholds: `0`
- stage: `idle`
- active window: `none`
- string ids / names: `""`
- roster JSON: `[]`
- allowed command JSON: `[]`
- vote map JSON: `{}`

---

## Timer names and what they drive

### `LotAT - Join Window`
Drives:
- `lotat-join-timeout.cs`

Current behavior:
- opens at session start
- expected UI duration: **120 seconds**
- zero joiners => normal `zero_join` end
- one or more joiners => roster freeze and starting-node handoff

### `LotAT - Decision Window`
Drives:
- `lotat-decision-timeout.cs`

Current behavior:
- normal voting window for authored decision commands
- expected UI duration: **120 seconds**
- zero valid votes => `unresolved` end
- one or more valid votes => shared resolution path
- may be disabled early when all joined participants have voted

### `LotAT - Commander Window`
Drives:
- `lotat-commander-timeout.cs`

Current behavior:
- opens only for nodes with `commander_moment.enabled = true`
- intended duration comes from node-authored `commander_moment.window_seconds`
- first valid assigned-commander command closes it immediately
- timeout silently continues into normal decision voting

### `LotAT - Dice Window`
Drives:
- `lotat-dice-timeout.cs`

Current behavior:
- opens only for nodes with `dice_hook.enabled = true`
- intended duration comes from node-authored `dice_hook.roll_window_seconds`
- first successful `!roll` closes it immediately
- timeout emits authored `failure_text` and continues into normal decision voting

### Important timer caveat
The current codebase treats join/decision windows differently from commander/dice windows:
- join and decision timers currently assume the Streamer.bot timer entries are already configured correctly in the UI and mainly use disable/enable behavior
- commander and dice windows attempt to call `CPH.SetTimerInterval(string, int)` to honor authored durations

That method is still called out in comments as **VERIFY/unconfirmed**. Operators should test commander/dice timer interval behavior in Streamer.bot before relying on authored window lengths live.

---

## Session flow summary

### Happy-path overview
1. `lotat-start-main.cs` starts from a voice trigger
2. runtime story is minimally loaded from `Creative/WorldBuilding/Storylines/loaded/current-story.json`
3. stale LotAT timers/state are cleared
4. stage becomes `join_open`
5. chat is told to use `!join`
6. `lotat-join.cs` collects unique lowercase usernames into the session roster
7. `lotat-join-timeout.cs` closes the join window
8. if nobody joined, session ends cleanly as `zero_join`
9. if users joined, roster freezes and the starting node is stored
10. `lotat-node-enter.cs` enters the node, applies chaos, sends intro text, and routes onward
11. node routes to one of:
    - ending => mark `ended`
    - commander window => commander commands or timeout => decision window
    - dice window => `!roll` success or timeout => decision window
    - direct decision window
12. `lotat-decision-input.cs` records votes from joined users only
13. if everyone joined has voted, early-close disables the decision timer
14. `lotat-decision-resolve.cs` tallies valid votes, breaks ties by earliest authored choice order, emits `result_flavor`, and sets next node id
15. `lotat-node-enter.cs` runs again for the next node
16. `lotat-end-session.cs` performs centralized cleanup whenever a terminal state is reached

### Terminal outcomes currently supported
- `success`
- `partial`
- `failure`
- `zero_join`
- `unresolved`
- `fault_abort`

### Current runtime stage set
- `idle`
- `join_open`
- `node_intro`
- `commander_open`
- `dice_open`
- `decision_open`
- `decision_resolving`
- `ended`

---

## Command routing summary

### Runtime session commands
- `!join` -> `lotat-join.cs`
- `!roll` -> `lotat-dice-roll.cs`

### Commander command family
All of these should route to `lotat-commander-input.cs`:
- `!stretch`
- `!shrimp`
- `!hydrate`
- `!orb`
- `!checkchat`
- `!toad`

### Authored decision command family
All of these should route to `lotat-decision-input.cs`:
- `!scan`
- `!target`
- `!analyze`
- `!reroute`
- `!deploy`
- `!contain`
- `!inspect`
- `!drink`
- `!simulate`

### Current vote/participation rules in code
- `!join` only counts during `join_open`
- joined roster freezes after join closes
- only frozen-roster users count for authored decision voting
- latest valid decision vote replaces prior valid vote from the same joined user
- commander windows ignore joined-roster status and only accept the snapshotted assigned commander user
- dice windows ignore joined-roster status and accept rolls from any chatter

---

## Internal wiring reference for implementers

## 1) Create these exact timers
- `LotAT - Join Window`
- `LotAT - Decision Window`
- `LotAT - Commander Window`
- `LotAT - Dice Window`

## 2) Trigger wiring

### Start
Wire the voice trigger to this action group:
1. `lotat-start-main.cs`
2. `lotat-end-session.cs`

Why:
- successful starts leave stage non-terminal, so `lotat-end-session.cs` no-ops
- startup fault-aborts are cleaned up immediately

### Join
Wire `!join` directly to:
1. `lotat-join.cs`

### Join timer
Wire `LotAT - Join Window` to:
1. `lotat-join-timeout.cs`
2. `lotat-end-session.cs`
3. `lotat-node-enter.cs`

Why:
- zero-join and fault-abort paths are cleaned up before node-entry runs
- joined-crew path leaves stage at `node_intro`, so end-session no-ops and node-entry continues

### Commander commands
Wire all six commander commands to:
1. `lotat-commander-input.cs`

### Commander timer
Wire `LotAT - Commander Window` to:
1. `lotat-commander-timeout.cs`
2. `lotat-end-session.cs`

### Dice command
Wire `!roll` directly to:
1. `lotat-dice-roll.cs`

### Dice timer
Wire `LotAT - Dice Window` to:
1. `lotat-dice-timeout.cs`
2. `lotat-end-session.cs`

### Authored decision commands
Wire all authored decision commands to this action group:
1. `lotat-decision-input.cs`
2. `lotat-decision-resolve.cs`
3. `lotat-node-enter.cs`
4. `lotat-end-session.cs`

Why:
- vote intake usually no-ops until a valid vote arrives
- early-close marks the node resolved and disables the timer
- resolve then runs immediately in the same action group
- node-enter advances the story
- end-session catches ending-node and fault-abort outcomes

### Decision timer
Wire `LotAT - Decision Window` to this action group:
1. `lotat-decision-timeout.cs`
2. `lotat-end-session.cs`
3. `lotat-decision-resolve.cs`
4. `lotat-node-enter.cs`
5. `lotat-end-session.cs`

Why:
- unresolved or fault-abort terminal states are caught before resolve
- vote-bearing closes share the same resolve + node-enter path as early-close
- the trailing end-session catches terminal states after node-entry

## 3) Runtime story prerequisite
Before testing LotAT, make sure this file exists and is valid enough to load:
- `Creative/WorldBuilding/Storylines/loaded/current-story.json`

## 4) Commander prerequisite
The commander subsystem must already be populating these globals:
- `current_captain_stretch`
- `current_the_director`
- `current_water_wizard`

If those are empty, commander moments silently skip into normal decision voting.

## 5) Path caveat
Several scripts probe a small list of machine-local candidate paths for the runtime story file.

If the repo checkout moves, update those candidate-path lists inside the LotAT scripts.

## 6) Stream-start/reset prerequisite
`Actions/Twitch Core Integrations/stream-start.cs` should continue to clear LotAT runtime state and disable all four LotAT timers before a fresh stream run.

---

## Known limitations / not-yet-implemented polish

- **No `!offering` integration.** This is intentional for v1.
- **No operator control surface** for manual advance, inspect, force-close, or debug commands.
- **No full runtime schema/graph validation pass.** Startup only performs minimal-safe checks.
- **Story file path discovery is machine-local and brittle.** Current scripts probe hardcoded candidate paths plus a couple environment-based guesses.
- **Commander/dice timer duration setting needs operator verification.** Current code attempts `CPH.SetTimerInterval(string, int)` for authored durations, but comments still mark that method as unconfirmed in project docs.
- **Decision timer restart is UI-driven.** The current decision-open paths disable/enable the named timer and assume the timer is configured correctly in Streamer.bot.
- **Limited chat polish.** The runtime is functional-first and mostly uses straightforward chat lines.
- **No Mix It Up-rich production hooks documented here as finished behavior.** Chat output is the reliable current surface.
- **No late join / leave flow.** The roster freezes after join close and stays fixed for the session.
- **No extra recovery snapshots or operator-inspection blobs.** Debugging is primarily via logs plus runtime globals.

---

## Explicit offering boundary

This boundary is intentional and must stay explicit in LotAT v1 docs and wiring.

### Out of scope for LotAT v1
- `!offering` is **out of scope** for LotAT v1
- offering globals are **not** LotAT engine behavior
- `Actions/Squad/offering.cs` is **not** part of the LotAT runtime documented here
- LotAT branching, chaos, commander windows, dice windows, join flow, voting, and endings do **not** depend on offering state in the current implementation

### Related shared constants that are not active LotAT v1 engine behavior
These exist in `Actions/SHARED-CONSTANTS.md` but should not be treated as LotAT runtime mechanics for this README:
- `lotat_announcement_sent`
- `lotat_offering_steal_chance`
- `lotat_steal_multiplier`
- `boost_*`

If a future operator wants LotAT/offering integration, that should be documented as a new explicit runtime contract rather than inferred from those legacy/provisional variables.

---

## Quick operator checklist

Before live testing:
- create all four LotAT timers with the exact names above
- set join timer to 120 seconds in the UI
- set decision timer to 120 seconds in the UI
- confirm commander assignment globals are populated
- confirm `Creative/WorldBuilding/Storylines/loaded/current-story.json` exists
- wire the action groups in the orders documented above
- test whether commander/dice authored window lengths are actually honored in your Streamer.bot install

After wiring, smoke test at least:
- start -> join window opens
- `!join` registers one user and dedupes duplicates
- zero-join timeout ends cleanly
- non-zero-join timeout enters the starting node
- direct decision node voting resolves normally
- early-close works when all joined users vote
- commander node skips when commander slot is empty
- commander node succeeds when correct assigned user responds
- dice node accepts `!roll` from non-joined chatters
- ending node cleans up back to idle
- fault path returns LotAT to idle cleanly
