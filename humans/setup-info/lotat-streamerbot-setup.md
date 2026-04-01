# LotAT Streamer.bot Setup

This guide is for the **human operator** setting up **Legends of the ASCII Temple (LotAT)** inside Streamer.bot.

It is intentionally practical.
It tells you:
- what actions exist
- what commands and timers to create
- how to wire them
- what to test before using LotAT live

If you need implementation details, use the internal docs instead:
- `Actions/LotAT/README.md`
- `Actions/SHARED-CONSTANTS.md`

---

## What LotAT needs in Streamer.bot

LotAT v1 expects these pieces to exist and be wired:

1. the LotAT action files pasted into Streamer.bot actions
2. one **voice trigger** to start a session
3. chat command triggers for join, dice, commander commands, and story decision commands
4. four named timers
5. a valid loaded runtime story file in the repo
6. working commander assignment globals from the existing commander system
7. the normal stream-start reset action pasted and up to date

---

## LotAT action list

Create or update these Streamer.bot actions from the matching repo files under `Actions/LotAT/`:

- `lotat-start-main.cs`
- `lotat-join.cs`
- `lotat-join-timeout.cs`
- `lotat-node-enter.cs`
- `lotat-commander-input.cs`
- `lotat-commander-timeout.cs`
- `lotat-dice-roll.cs`
- `lotat-dice-timeout.cs`
- `lotat-decision-input.cs`
- `lotat-decision-timeout.cs`
- `lotat-decision-resolve.cs`
- `lotat-end-session.cs`

Recommended naming in Streamer.bot:
- put them in a `LotAT` group or folder so they stay easy to manage

---

## Timers to create

Create these timers with these exact names:

- `LotAT - Join Window`
- `LotAT - Decision Window`
- `LotAT - Commander Window`
- `LotAT - Dice Window`

### Timer interval setup

Set these **in the Streamer.bot UI**:

- `LotAT - Join Window` = `120 seconds`
- `LotAT - Decision Window` = `120 seconds`

### Important note for commander and dice timers

The runtime code attempts to set commander and dice timer lengths dynamically from the story JSON.
That means:
- commander windows should use the node's `commander_moment.window_seconds`
- dice windows should use the node's `dice_hook.roll_window_seconds`

But this behavior still needs real Streamer.bot verification.
Until you verify it, treat commander/dice timing as a setup risk.

Recommended test:
- use a node authored for `15` seconds
- confirm the actual timeout really happens around `15` seconds

---

## Triggers and command routing

## 1) Voice trigger for session start

Create one voice trigger that runs this action group in this order:

1. `lotat-start-main.cs`
2. `lotat-end-session.cs`

Why this order:
- a successful start leaves the session active, so `lotat-end-session.cs` does nothing
- a startup fault-abort can be cleaned up immediately

Important:
- LotAT start is designed for a **voice trigger**, not a chat command
- it does **not** expect a chat `message` payload

---

## 2) Chat command: `!join`

Route:
1. `lotat-join.cs`

Behavior:
- only works during the join window
- duplicate joins are ignored
- usernames are normalized to lowercase for roster tracking

---

## 3) Chat command: `!roll`

Route:
1. `lotat-dice-roll.cs`

Behavior:
- only works during a dice window
- any chatter can roll
- joined-roster status does not matter for dice rolls

---

## 4) Commander commands

Route all of these to the same action:
1. `lotat-commander-input.cs`

Commands:
- `!stretch`
- `!shrimp`
- `!hydrate`
- `!orb`
- `!checkchat`
- `!toad`

Behavior:
- only the currently snapshotted assigned commander user counts during a commander window
- everyone else is ignored silently

---

## 5) Story decision commands

Route all of these to the same action group in this order:

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

Behavior:
- only joined users in the frozen roster can cast counted decision votes
- latest valid vote replaces earlier valid vote from the same joined user
- if all joined users have voted, the decision window can close early

---

## Timer end wiring

## `LotAT - Join Window`
Run this action group in this order:

1. `lotat-join-timeout.cs`
2. `lotat-end-session.cs`
3. `lotat-node-enter.cs`

Expected result:
- zero joiners -> clean end
- one or more joiners -> freeze roster and begin story node flow

---

## `LotAT - Commander Window`
Run this action group in this order:

1. `lotat-commander-timeout.cs`
2. `lotat-end-session.cs`

Expected result:
- timeout skips silently into the normal decision path

---

## `LotAT - Dice Window`
Run this action group in this order:

1. `lotat-dice-timeout.cs`
2. `lotat-end-session.cs`

Expected result:
- timeout emits the node's authored failure text and continues into the normal decision path

---

## `LotAT - Decision Window`
Run this action group in this order:

1. `lotat-decision-timeout.cs`
2. `lotat-end-session.cs`
3. `lotat-decision-resolve.cs`
4. `lotat-node-enter.cs`
5. `lotat-end-session.cs`

Expected result:
- zero valid votes -> unresolved end
- one or more valid votes -> resolve winning choice and continue

---

## Required prerequisites outside Streamer.bot wiring

## 1) Runtime story file must exist

LotAT v1 loads exactly this file:

- `Creative/WorldBuilding/Storylines/loaded/current-story.json`

It does **not** scan `ready/` during runtime.

If the loaded story file is missing or malformed:
- LotAT start will fail closed
- chat gets a simple safe failure message

---

## 2) Commander assignment globals must already exist

LotAT reads these existing globals from the commander system:

- `current_captain_stretch`
- `current_the_director`
- `current_water_wizard`

If these are empty:
- commander moments will skip
- normal decision voting should still continue

---

## 3) Stream-start reset must be current

Make sure the repo version of:
- `Actions/Twitch Core Integrations/stream-start.cs`

has already been pasted into Streamer.bot.

Why:
- it disables all four LotAT timers
- it resets LotAT state back to idle defaults
- it clears stale roster/window/vote state from prior runs

---

## Important LotAT v1 boundaries

These are intentional and should stay true in setup and testing:

- `!offering` is **not** part of LotAT v1
- offering globals are **not** LotAT runtime behavior
- boost state is **not** part of LotAT v1
- the loaded runtime story file is the only story source at runtime
- join and decision windows are fixed 120-second runtime windows
- commander and dice windows happen before normal decision voting when authored on a node

---

## Recommended smoke test checklist

After wiring everything, test at least these flows:

### Startup and join
- [ ] voice trigger starts LotAT
- [ ] join window opens
- [ ] `!join` registers a user
- [ ] duplicate `!join` does not double-add the same user
- [ ] zero-join timeout ends cleanly
- [ ] non-zero-join timeout enters the first node

### Decision voting
- [ ] only joined users can cast counted decision votes
- [ ] invalid decision commands do not count
- [ ] latest valid vote replaces earlier valid vote from the same joined user
- [ ] early close triggers when all joined users have voted
- [ ] zero-valid-vote timeout ends unresolved
- [ ] ties resolve by earliest authored choice order

### Commander window
- [ ] commander node opens commander window
- [ ] wrong user is ignored silently
- [ ] correct assigned commander user succeeds immediately
- [ ] missing commander assignment skips into normal voting
- [ ] commander timeout skips into normal voting

### Dice window
- [ ] dice node opens dice window
- [ ] non-joined chatters can use `!roll`
- [ ] every roll result is surfaced to chat
- [ ] first successful roll closes the window immediately
- [ ] timeout emits failure text and continues into normal voting

### End and cleanup
- [ ] ending node returns LotAT to idle
- [ ] zero-join end returns LotAT to idle
- [ ] unresolved timeout returns LotAT to idle
- [ ] fault-abort returns LotAT to idle
- [ ] all four LotAT timers are stopped on cleanup

---

## Known setup risks

These are the main things to watch for during setup:

1. **Commander/dice dynamic timer duration may need verification**
   - runtime code attempts to set per-node timer lengths
   - verify this works in your Streamer.bot install

2. **Story file path assumptions are machine-local in code**
   - if the repo checkout moves, some LotAT path probing may need updating in source

3. **Decision and join timers depend on correct UI timer intervals**
   - make sure they are set to `120 seconds`

4. **LotAT is unattended once started**
   - there is no operator force-close or manual advance tool in the current prototype

---

## Quick setup summary

If you only want the shortest operator checklist:

1. Paste all `Actions/LotAT/*.cs` files into Streamer.bot actions
2. Paste the latest `Actions/Twitch Core Integrations/stream-start.cs`
3. Create these timers exactly:
   - `LotAT - Join Window`
   - `LotAT - Decision Window`
   - `LotAT - Commander Window`
   - `LotAT - Dice Window`
4. Set:
   - join timer = `120s`
   - decision timer = `120s`
5. Wire:
   - voice start -> `lotat-start-main.cs`, `lotat-end-session.cs`
   - `!join` -> `lotat-join.cs`
   - `!roll` -> `lotat-dice-roll.cs`
   - commander commands -> `lotat-commander-input.cs`
   - decision commands -> `lotat-decision-input.cs`, `lotat-decision-resolve.cs`, `lotat-node-enter.cs`, `lotat-end-session.cs`
6. Wire timer-end action groups as documented above
7. Confirm `current-story.json` exists
8. Confirm commander globals are populated
9. Run the smoke tests before using it live
