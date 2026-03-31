You are acting as the `streamerbot-dev` role for the SharkStreamerBot project.

Your job in this run is to implement the **LotAT session bootstrap path**: start trigger handling, runtime story load, minimal-safe preflight, session initialization, and join-window open.

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
21. `Creative/WorldBuilding/Storylines/loaded/current-story.json`
22. `Humans/LotAT-Implementation-Prompts/README.md`
23. `Humans/LotAT-Implementation-Prompts/01-lotat-action-matrix-and-wiring-plan.md`
24. `Humans/LotAT-Implementation-Prompts/02-lotat-shared-constants-and-stream-start-reset.md`

## Scope
Implement only the startup/bootstrap path.

## Required target files
At minimum, create/update:
- `Actions/LotAT/lotat-start.cs`
- `Actions/LotAT/README.md` if needed for script documentation

## Required behavior
Implement a `lotat-start` action that:
1. is designed for a **voice-command trigger**
2. does not rely on chat `message` or `rawInput`
3. refuses to start if a LotAT session is already active
4. loads exactly one runtime story file:
   - `Creative/WorldBuilding/Storylines/loaded/current-story.json`
5. performs only the minimal-safe runtime checks needed to begin:
   - file exists/readable
   - JSON parses
   - core runtime fields needed to start exist
   - starting node can be resolved
6. does **not** perform a second full schema/graph validation pass
7. on success:
   - clears stale LotAT runtime state
   - creates a fresh session id
   - stores story/session bootstrap state in globals
   - sets stage to `join_open`
   - initializes empty roster/vote/window globals
   - announces the join phase in chat
   - starts the LotAT join timer
8. on minimal-load failure:
   - fail closed
   - keep runtime inactive
   - log enough detail for debugging
   - send a simple chat-safe failure message

## Important boundaries
- Do not implement `!join` handling yet.
- Do not implement node-entry flow yet.
- Do not integrate `!offering`.
- Do not redesign the commander subsystem.
- Keep chat output simple and safe.
- If you need helper methods inside the script, keep them local and well-commented.
- Match the story contract, not just the prototype story.

## Special implementation note
If Streamer.bot file I/O / JSON parsing forces any practical compromise, document it clearly in comments and in your final response instead of silently changing the contract.

## After editing
Load and follow `.pi/skills/ops-change-summary/SKILL.md` before your final response.

## Final response requirements
Include:
- files changed
- what startup behavior now exists
- any Streamer.bot assumptions made for file I/O / JSON parsing
- manual operator wiring steps for the voice trigger and join timer
- validation performed
- remaining follow-up dependencies
