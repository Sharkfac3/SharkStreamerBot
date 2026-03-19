# LotAT Engine — Supported Commands

## Currently Implemented Commands

These are the only commands that exist in the engine. Story JSON must not use commands outside this list without a corresponding engine implementation.

`!scan` `!target` `!analyze` `!reroute` `!deploy` `!contain` `!inspect` `!drink` `!simulate`

## Adding a New Command

1. Add the command handler to the C# engine in `Actions/LotAT/`
2. Add the command name to this list
3. Update `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` so the story agent knows it is available
4. Update the `commands_used` field in any story that uses it

## Rules

- Do not invent new commands in a story JSON without a corresponding engine update
- Each command must have a defined interaction path in the engine
- Commands should map to thematic ship actions (scanning, targeting, deploying crew, etc.)
