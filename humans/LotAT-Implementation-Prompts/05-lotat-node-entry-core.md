You are acting as the `streamerbot-dev` role for the SharkStreamerBot project.

Your job in this run is to implement the **LotAT node-entry core**: the action that reads the current node from the loaded story, applies node-intro behavior, applies chaos, clears stale node-local state, and routes to ending / commander / dice / decision flow according to the contract.

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
13. `.agents/roles/lotat-tech/skills/story-pipeline/json-schema.md`
14. `.agents/roles/lotat-tech/skills/engine/docs-map.md`
15. `.agents/roles/lotat-tech/skills/engine/_index.md`
16. `.agents/roles/lotat-tech/skills/engine/commands.md`
17. `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md`
18. `.agents/roles/lotat-tech/skills/engine/state-and-voting.md`
19. `Creative/WorldBuilding/Storylines/loaded/current-story.json`
20. `Actions/LotAT/lotat-start.cs`
21. `Actions/LotAT/lotat-join.cs`
22. `Actions/LotAT/lotat-join-window-end.cs`
23. `Humans/LotAT-Implementation-Prompts/README.md`
24. `Humans/LotAT-Implementation-Prompts/01-lotat-action-matrix-and-wiring-plan.md`

## Required target files
At minimum, create/update:
- `Actions/LotAT/lotat-enter-node.cs`
- update `Actions/LotAT/README.md` if needed

## Required behavior
Implement node-entry behavior that:
1. resolves the active node using `lotat_session_current_node_id`
2. sets runtime stage to `node_intro`
3. clears stale per-node window/vote state from the prior node
4. applies the node’s `chaos.delta` to session chaos total
5. surfaces node intro information to chat in a practical v1 form
   - read_aloud
   - optionally title / ship section if useful
   - do not overcomplicate output
6. derives and stores current allowed authored decision commands for the node when relevant
7. routes according to contract:
   - ending node -> end path
   - commander moment enabled -> commander path
   - dice hook enabled -> dice path
   - otherwise -> decision path

## Important boundaries
- Do not invent new story fields.
- Preserve the rule that commander moments and dice hooks do not coexist in v1.
- Do not integrate `!offering`.
- If a malformed runtime node condition is encountered that prevents safe progress, fail closed in a simple practical way and document it.
- Match the general story contract, not only the prototype story.

## Practical note
If needed, stub or clearly mark calls into not-yet-built actions for later prompts, but keep this action coherent and copy/paste ready.

## After editing
Load and follow `.pi/skills/ops-change-summary/SKILL.md` before your final response.

## Final response requirements
Include:
- files changed
- node-entry behavior implemented
- how chaos is handled
- how current-node routing is handled
- any temporary stub assumptions for later prompts
- validation performed
