# Actions/LotAT Implementation Map

This file maps the checked-in LotAT v1 Streamer.bot action scripts and their internal wiring responsibilities. For shared runtime/tooling/story/presentation facts, see [LotAT contract](../../Docs/Architecture/lotat-contract.md). For exact Streamer.bot setup, see [operator setup](operator-setup.md).

## Action inventory

| File | Current purpose |
|---|---|
| [lotat-start-main.cs](lotat-start-main.cs) | Start a session from a voice trigger, minimally load the runtime story, clear stale state, open join window. |
| [lotat-join.cs](lotat-join.cs) | Accept `!join` during `join_open` and update joined roster/count. |
| [lotat-join-timeout.cs](lotat-join-timeout.cs) | Close join window, end on zero joiners, or freeze roster and hand off to starting node. |
| [lotat-node-enter.cs](lotat-node-enter.cs) | Enter a node, apply chaos, send intro text, and route to ending / commander / dice / decision flow. |
| [lotat-commander-input.cs](lotat-commander-input.cs) | Shared handler for commander commands during `commander_open`. |
| [lotat-commander-timeout.cs](lotat-commander-timeout.cs) | Commander timer-end handler; silently skip to normal voting. |
| [lotat-dice-roll.cs](lotat-dice-roll.cs) | Shared `!roll` handler during `dice_open`. |
| [lotat-dice-timeout.cs](lotat-dice-timeout.cs) | Dice timer-end handler; emit failure text and continue to normal voting. |
| [lotat-decision-input.cs](lotat-decision-input.cs) | Shared authored decision-command vote intake during `decision_open`. |
| [lotat-decision-timeout.cs](lotat-decision-timeout.cs) | Decision timer-end handler; unresolved end on zero valid votes or handoff to resolve. |
| [lotat-decision-resolve.cs](lotat-decision-resolve.cs) | Tally votes, apply tie-break, emit `result_flavor`, store winning choice, set next node. |
| [lotat-end-session.cs](lotat-end-session.cs) | Shared terminal cleanup back to idle. |
| [overlay-publish.cs](overlay-publish.cs) | Reference template for broker publishing; copy `PublishLotat*` methods and constants into engine scripts as needed. Not a standalone deployed action. |

## Expected trigger/input notes

### `lotat-start-main.cs`

- Expected trigger: voice command trigger only.
- Input: none required.
- Important guard: refuses to start if `lotat_active` is already true.
- Story load: performs minimal-safe load checks only.

### `lotat-join.cs`

- Expected trigger: `!join`.
- Input: `user` preferred, `userName` fallback.
- Important guard: only counts during `join_open` while active window is still `join`.

### `lotat-join-timeout.cs`

- Expected trigger: timer end for `LotAT - Join Window`.
- Input: none required.
- Important guard: stale timer fires no-op if stage/window already moved on.

### `lotat-node-enter.cs`

- Expected trigger: internal follow-up after join close or decision resolve.
- Input: optional `input0 = <node_id>`; otherwise uses `lotat_session_current_node_id`.
- Important guard: fail-closes if node resolution or node payload checks fail.

### `lotat-commander-input.cs`

- Expected trigger: shared routing for commander commands.
- Input: sender from `user` / `userName`, command text from `command` with fallback to `message` / `rawInput`.
- Important guard: only the snapshotted assigned commander user may satisfy the moment.

### `lotat-commander-timeout.cs`

- Expected trigger: timer end for `LotAT - Commander Window`.
- Input: none required.
- Important guard: stale timer fires no-op safely.

### `lotat-dice-roll.cs`

- Expected trigger: `!roll`.
- Input: sender from `user` / `userName`.
- Important guard: only counts during `dice_open`.
- Participation rule: any chatter may roll; joined-roster check is intentionally skipped.

### `lotat-dice-timeout.cs`

- Expected trigger: timer end for `LotAT - Dice Window`.
- Input: none required.
- Important guard: stale timer fires no-op safely.

### `lotat-decision-input.cs`

- Expected trigger: shared routing for authored decision commands.
- Input: sender from `user` / `userName`, command text from `command` with fallback to `message` / `rawInput`.
- Important guard: only joined users in the frozen roster may cast counted votes.

### `lotat-decision-timeout.cs`

- Expected trigger: timer end for `LotAT - Decision Window`.
- Input: none required.
- Important guard: stale timer fires no-op safely.

### `lotat-decision-resolve.cs`

- Expected trigger: immediately after decision-input early-close or decision-timeout handoff.
- Input: none required in current code.
- Important guard: no-ops unless decision window is already marked resolved.

### `lotat-end-session.cs`

- Expected trigger: after any action that may leave stage = `ended`.
- Input: optional `input0 = <endReason>`; current runtime usually relies on `lotat_session_last_end_state`.
- Important guard: safe to include after non-terminal actions because it no-ops when the session is not ending.

## Paste / sync targets

Any edited C# file under [Actions/LotAT/](./) is a Streamer.bot paste target. Include each changed script in the final sync/change-summary output.

Also flag operator setup changes for:

- timer names or durations
- trigger wiring
- action group ordering
- new global variables
- updates to the loaded runtime story prerequisite

## Runtime implementation gotchas

- Runtime identity uses lowercase username/login strings, not user IDs, for roster, votes, and commander-target comparison.
- The roster freezes after the join window closes; late join/leave flow is not part of v1.
- Zero joiners end the session cleanly before story play.
- Zero valid votes on a node ends the run unresolved; do not invent fallback winners.
- Tie-breaks resolve to the earliest matching choice in authored `choices` order.
- Commander and dice outcomes are narrative-only in v1; they do not change chaos, branching, vote eligibility, or vote resolution.
- `Actions/Squad/offering.cs` and offering globals are legacy/provisional experimentation, not LotAT v1 mechanics.
