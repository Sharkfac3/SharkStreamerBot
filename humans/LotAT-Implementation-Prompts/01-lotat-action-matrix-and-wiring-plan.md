You are acting as the `streamerbot-dev` role for the SharkStreamerBot project in **LotAT implementation-planning mode**.

Your job in this run is **not to write C# yet**. Your job is to produce the detailed implementation matrix that later coding prompts will follow.

## First: load and follow these files in this order
1. `WORKING.md`
2. `.agents/ENTRY.md`
3. `.agents/_shared/project.md`
4. `.agents/_shared/conventions.md`
5. `.agents/_shared/coordination.md`
6. `.agents/roles/streamerbot-dev/role.md`
7. `.agents/roles/streamerbot-dev/skills/core.md`
8. `.agents/roles/streamerbot-dev/skills/lotat/_index.md`
9. `Actions/SHARED-CONSTANTS.md`
10. `Actions/HELPER-SNIPPETS.md`
11. `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md`
12. `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`
13. `.agents/roles/lotat-tech/skills/story-pipeline/_index.md`
14. `.agents/roles/lotat-tech/skills/story-pipeline/json-schema.md`
15. `.agents/roles/lotat-tech/skills/engine/docs-map.md`
16. `.agents/roles/lotat-tech/skills/engine/_index.md`
17. `.agents/roles/lotat-tech/skills/engine/commands.md`
18. `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md`
19. `.agents/roles/lotat-tech/skills/engine/state-and-voting.md`
20. `Creative/WorldBuilding/Storylines/finished/test-prototype-soda-comet.json`
21. `Humans/LotAT-Implementation-Prompts/README.md`

## Scope rules
- Do not write runtime C# yet.
- Do not invent schema changes.
- Do not integrate `!offering`.
- Do not make LotAT depend on boost state or offering globals.
- Treat `Creative/WorldBuilding/Storylines/loaded/current-story.json` as the only runtime story source.
- The start trigger is a **voice command trigger** in Streamer.bot and carries no chat message payload.
- Commander assignment globals already exist and should be read, not redesigned.
- Chat output is the priority; Mix It Up hooks can remain minimal/scaffold-only.

## Deliverable
Produce a detailed **LotAT v1 action matrix and wiring plan** that later coding prompts can implement without drifting.

Include all of the following:

1. **Implementation goal recap**
   - summarize the LotAT v1 target behavior
   - summarize what is in scope and out of scope

2. **Proposed `Actions/LotAT/` action inventory**
   - list every proposed action file
   - one-line purpose for each

3. **Per-action implementation matrix**
   For each proposed action, provide:
   - suggested file name
   - purpose
   - trigger source
   - expected available args
   - globals read
   - globals written
   - timers started/stopped/disabled
   - downstream actions called
   - notable guard conditions

4. **Timer-to-action mapping table**
   Cover all four LotAT timers and the action each one should trigger.

5. **Command-routing table**
   Cover:
   - `!join`
   - `!roll`
   - commander commands
   - authored decision commands
   Include your recommendation for whether authored decision commands should share one action or be split.

6. **Stage transition matrix**
   Cover all documented stages:
   - `idle`
   - `join_open`
   - `node_intro`
   - `commander_open`
   - `dice_open`
   - `decision_open`
   - `decision_resolving`
   - `ended`
   Show which actions are responsible for entering and leaving each stage.

7. **Manual Streamer.bot UI wiring tasks**
   - timer setup
   - trigger wiring
   - voice trigger notes
   - chat command routing notes
   - any operator setup that must exist before coding prompts are run

8. **Known implementation risks / assumptions to validate early**
   Include at minimum:
   - file I/O and JSON parsing inside Streamer.bot C# actions
   - timer interval handling for commander/dice windows
   - chat trigger arg availability
   - stale timer guard strategy

9. **Recommended coding prompt order**
   - provide the implementation sequence the remaining prompt pack should follow
   - note any dependencies

## Output format
Use clear headings and tables where useful.

## Important success condition
This run succeeds if it creates a practical, contract-grounded action matrix that later coding prompts can follow safely.
