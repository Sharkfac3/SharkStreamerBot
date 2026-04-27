# Actions/LotAT Operator Setup

This file covers Streamer.bot timer/trigger setup and live-test checks for the checked-in LotAT v1 runtime. For cross-domain contract facts, see [LotAT contract](../../Docs/Architecture/lotat-contract.md).

## Timers

Create these exact Streamer.bot timers:

- `LotAT - Join Window`
- `LotAT - Decision Window`
- `LotAT - Commander Window`
- `LotAT - Dice Window`

Set the join and decision timers to 120 seconds in the Streamer.bot UI.

Commander and dice windows attempt to use authored per-node durations through `CPH.SetTimerInterval(string, int)`. Test that behavior in the local Streamer.bot install before relying on authored commander/dice lengths live.

## Trigger wiring

### Start

Wire the voice trigger to this action group:

1. `lotat-start-main.cs`
2. `lotat-end-session.cs`

Startup fault-aborts are cleaned immediately; successful starts leave the session non-terminal, so end-session no-ops.

### Join

Wire `!join` directly to:

1. `lotat-join.cs`

### Join timer

Wire `LotAT - Join Window` to:

1. `lotat-join-timeout.cs`
2. `lotat-end-session.cs`
3. `lotat-node-enter.cs`

Zero-join and fault-abort paths clean up before node-entry. Joined-crew paths continue into the starting node.

### Commander commands

Wire all six commander commands to:

1. `lotat-commander-input.cs`

Commands:

- `!stretch`
- `!shrimp`
- `!hydrate`
- `!orb`
- `!checkchat`
- `!toad`

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

Commands:

- `!scan`
- `!target`
- `!analyze`
- `!reroute`
- `!deploy`
- `!contain`
- `!inspect`
- `!drink`
- `!simulate`

### Decision timer

Wire `LotAT - Decision Window` to this action group:

1. `lotat-decision-timeout.cs`
2. `lotat-end-session.cs`
3. `lotat-decision-resolve.cs`
4. `lotat-node-enter.cs`
5. `lotat-end-session.cs`

Unresolved/fault-abort terminal states are caught before resolve. The trailing end-session catches terminal states after node-entry.

## Runtime prerequisites

Before testing LotAT, confirm:

- [Creative/WorldBuilding/Storylines/loaded/current-story.json](../../Creative/WorldBuilding/Storylines/loaded/current-story.json) exists and is valid enough to load.
- Commander assignment globals are populated when commander moments should work:
  - `current_captain_stretch`
  - `current_the_director`
  - `current_water_wizard`
- [Actions/Twitch Core Integrations/stream-start.cs](../Twitch%20Core%20Integrations/stream-start.cs) clears LotAT runtime state and disables all four LotAT timers before a fresh stream run.

If commander globals are empty, commander moments silently skip into normal decision voting.

## Path caveat

Several scripts probe a small list of machine-local candidate paths for the runtime story file. If the repo checkout moves, update those candidate-path lists inside the LotAT scripts.

## Quick operator checklist

Before live testing:

- create all four LotAT timers with the exact names above
- set join timer to 120 seconds in the UI
- set decision timer to 120 seconds in the UI
- confirm commander assignment globals are populated
- confirm `Creative/WorldBuilding/Storylines/loaded/current-story.json` exists
- wire the action groups in the orders documented above
- test whether commander/dice authored window lengths are honored by Streamer.bot

Smoke test at least:

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
