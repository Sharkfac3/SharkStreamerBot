# LotAT Engine — Supported Commands

## Command Categories

LotAT distinguishes between:
- **authored decision commands** — used in story JSON `choices[].command` and `commands_used`
- **runtime session commands** — used by the engine to manage the live session, but not authored into story JSON

This distinction is a runtime/story boundary, not a schema expansion. Runtime session commands may be documented in story-agent or tech docs so future agents understand the live-play contract, but they do **not** become valid authored `choices[].command` values unless the schema is intentionally changed.

## Currently Supported Authored Decision Commands

These are the authored commands story JSON may use.

`!scan` `!target` `!analyze` `!reroute` `!deploy` `!contain` `!inspect` `!drink` `!simulate`

## Currently Supported Runtime Session Commands

These are engine-owned commands that support the live session lifecycle.

`!join` — register the viewer in the current session's participant roster during the join phase
`!roll` — submit a public 1–100 dice roll during an active node dice-roll window; available to all chat, not just joined participants

## Existing Commander Commands Used by LotAT Runtime

These are pre-existing commander feature commands that LotAT may temporarily listen for during an active commander moment. They remain runtime-only and do **not** become authored story-choice commands.

`!stretch` / `!shrimp` — valid only for an active Captain Stretch commander moment
`!hydrate` / `!orb` — valid only for an active Water Wizard commander moment
`!checkchat` / `!toad` — valid only for an active The Director commander moment

## Runtime Commander / Dice / Voting Rule

For each LotAT session:
- the engine opens a join phase before the first story decision
- the join window uses a fixed runtime default of **120 seconds** in v1
- viewers who type `!join` are added to the participant roster for that run
- when the join phase closes, that roster is frozen for the rest of the session
- when the engine enters a stage node with `commander_moment.enabled = true`, it opens a timed commander-input window before normal voting
- the commander window duration comes from the node's authored `commander_moment.window_seconds`
- the engine snapshots the currently assigned commander user at commander-window open
- only that assigned commander user may satisfy the moment; joined-session status does not matter for commander input
- any mapped valid command for that commander counts, and the first valid command closes the commander window immediately as a success
- commander-moment success is narrative-only in v1 and does **not** change chaos, branching, vote eligibility, or vote resolution
- if the commander window times out, no assigned commander exists, or the assigned commander never responds, the engine continues silently into normal voting
- when the engine enters a stage node with `dice_hook.enabled = true`, it opens a timed pre-vote `!roll` window
- the dice window duration comes from the node's authored `dice_hook.roll_window_seconds`
- during that dice window, **any viewer in chat** may type `!roll`; joined-session status does not matter for rolling
- the first roll meeting `roll >= success_threshold` closes the dice window immediately as a success
- if the timer expires with no successful roll, the dice window resolves as a failure
- dice-hook success/failure is narrative-only in v1 and does **not** change chaos, branching, vote eligibility, or vote resolution
- after the commander window or dice window resolves, the engine opens the normal decision window for that node if it is a stage node
- the decision window uses a fixed runtime default of **120 seconds** in v1
- during each decision window, only joined participants from that frozen roster count toward the "everyone has voted" rule
- if every joined participant submits one of the currently allowed decision commands for that node, the engine may auto-close the decision window early and advance immediately through the normal resolution path

Implementation note for future Streamer.bot work:
- v1 should use one named timer per window type (`LotAT - Join Window`, `LotAT - Decision Window`, `LotAT - Commander Window`, `LotAT - Dice Window`) plus runtime stage/window guards so stale timer events cannot advance the session incorrectly

V1 guardrails:
- commander moments and dice hooks do **not** coexist on the same node
- commander-input commands are never authored story-choice commands and never appear in `commands_used`

## Adding a New Command

### If it is an authored decision command
1. Add the command handler to the C# engine in `Actions/LotAT/`
2. Add the command name to the authored-decision list in this file
3. Update `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` so the story agent knows it is available
4. Update the `commands_used` field in any story that uses it

### If it is a runtime session command
1. Add the runtime handling to the C# engine in `Actions/LotAT/`
2. Add the command name to the runtime-session list in this file
3. Update `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` and technical docs so agents understand the session contract
4. Do **not** treat it as a story-choice command unless the authored schema is intentionally changed

## Rules

- Do not invent new commands in story JSON without a corresponding engine update
- Each command must have a defined interaction path in the engine
- Authored decision commands should map to thematic ship actions (scanning, targeting, deploying crew, etc.)
- Runtime session commands manage session flow and participation; they are not story-content fields
- `!roll` is never an authored story-choice command and must not appear in `commands_used`
- commander-input commands (`!stretch`, `!shrimp`, `!hydrate`, `!orb`, `!checkchat`, `!toad`) are never authored story-choice commands and must not appear in `commands_used`
