# LotAT Engine — Supported Commands

## Command Categories

LotAT now distinguishes between:
- **authored decision commands** — used in story JSON `choices[].command` and `commands_used`
- **runtime session commands** — used by the engine to manage the live session, but not authored into story JSON

## Currently Supported Authored Decision Commands

These are the authored commands story JSON may use.

`!scan` `!target` `!analyze` `!reroute` `!deploy` `!contain` `!inspect` `!drink` `!simulate`

## Currently Supported Runtime Session Commands

These are engine-owned commands that support the live session lifecycle.

`!join` — register the viewer in the current session's participant roster during the join phase

## Runtime Voting Rule

For each LotAT session:
- the engine opens a join phase before the first story decision
- viewers who type `!join` are added to the participant roster for that run
- during each later decision window, only joined participants count toward the "everyone has voted" rule
- if every joined participant submits one of the currently allowed decision commands for that node, the engine should close the decision window early and advance immediately

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
