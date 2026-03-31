You are acting as the `streamerbot-dev` role for the SharkStreamerBot project.

Your job in this run is to implement the **LotAT decision window**, including vote intake for authored decision commands, latest-valid-vote replacement, early close when all joined users have voted, timer-end handling, deterministic tie-break resolution, and node advancement.

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
11. `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`
12. `.agents/roles/lotat-tech/skills/story-pipeline/json-schema.md`
13. `.agents/roles/lotat-tech/skills/engine/docs-map.md`
14. `.agents/roles/lotat-tech/skills/engine/_index.md`
15. `.agents/roles/lotat-tech/skills/engine/commands.md`
16. `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md`
17. `.agents/roles/lotat-tech/skills/engine/state-and-voting.md`
18. `Creative/WorldBuilding/Storylines/loaded/current-story.json`
19. `Actions/LotAT/lotat-enter-node.cs`
20. `Humans/LotAT-Implementation-Prompts/README.md`
21. `Humans/LotAT-Implementation-Prompts/01-lotat-action-matrix-and-wiring-plan.md`
22. `Humans/LotAT-Implementation-Prompts/05-lotat-node-entry-core.md`

## Required target files
At minimum, create/update:
- `Actions/LotAT/lotat-vote-command.cs`
- `Actions/LotAT/lotat-decision-window-end.cs`
- `Actions/LotAT/lotat-resolve-decision.cs`
- update `Actions/LotAT/lotat-enter-node.cs` only as needed to open the decision window
- update `Actions/LotAT/README.md` if needed

## Required behavior
### `lotat-vote-command.cs`
Implement authored decision command intake that:
- works for all authored decision commands through one shared action if practical
- only counts votes during `decision_open`
- only accepts votes from users in the frozen joined roster
- only accepts commands present in the active node’s allowed commands json
- stores one active valid vote per joined participant for the current node
- replaces earlier valid votes with later valid votes from the same participant
- updates valid vote count
- triggers early close when every joined participant has a valid recorded vote

### `lotat-decision-window-end.cs`
Implement timer-end behavior that:
- no-ops safely on stale/irrelevant timer fires
- ends unresolved if zero valid votes exist at close
- otherwise routes through the single resolution path

### `lotat-resolve-decision.cs`
Implement resolution that:
- sets stage to `decision_resolving`
- tallies votes from the active node’s vote map
- uses deterministic tie-break by earliest matching choice in authored `choices` order
- emits the winning choice’s `result_flavor`
- stores `lotat_session_last_choice_id`
- updates current node id to the winner’s `next_node_id`
- re-enters node flow through the normal node-entry action

## Important boundaries
- Do not count `!join`, `!roll`, or commander commands as decision votes.
- Do not integrate `!offering`.
- Do not invent fallback winners for zero-vote timeout.
- Keep vote state per-node only.
- Keep implementation consistent with the story contract, not only the prototype story.

## Practical operator note
If Streamer.bot trigger wiring for all authored commands requires a specific UI pattern, document it clearly.

## After editing
Load and follow `.pi/skills/ops-change-summary/SKILL.md` before your final response.

## Final response requirements
Include:
- files changed
- how authored decision commands are routed
- vote storage behavior
- early-close behavior
- tie-break behavior
- unresolved zero-vote behavior
- validation performed
