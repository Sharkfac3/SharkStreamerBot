You are acting as the `streamerbot-dev` role for the SharkStreamerBot project.

Your job in this run is to implement the **LotAT commander window** mechanic using the existing commander assignment globals already present elsewhere in the project.

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
11. `.agents/roles/lotat-tech/skills/engine/docs-map.md`
12. `.agents/roles/lotat-tech/skills/engine/_index.md`
13. `.agents/roles/lotat-tech/skills/engine/commands.md`
14. `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md`
15. `.agents/roles/lotat-tech/skills/engine/state-and-voting.md`
16. `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`
17. `Creative/WorldBuilding/Storylines/loaded/current-story.json`
18. `Actions/LotAT/lotat-enter-node.cs`
19. `Actions/LotAT/lotat-vote-command.cs`
20. `Actions/LotAT/lotat-decision-window-end.cs`
21. `Actions/LotAT/lotat-resolve-decision.cs`
22. `Humans/LotAT-Implementation-Prompts/README.md`
23. `Humans/LotAT-Implementation-Prompts/01-lotat-action-matrix-and-wiring-plan.md`

## Required target files
At minimum, create/update:
- `Actions/LotAT/lotat-commander-input.cs`
- `Actions/LotAT/lotat-commander-window-end.cs`
- update `Actions/LotAT/lotat-enter-node.cs` as needed
- update `Actions/LotAT/README.md` if needed

## Required behavior
Implement commander-window behavior that:
1. opens only when the current stage node has `commander_moment.enabled = true`
2. maps the authored commander name to the existing valid commander command set:
   - Captain Stretch -> `!stretch` / `!shrimp`
   - The Water Wizard -> `!hydrate` / `!orb`
   - The Director -> `!checkchat` / `!toad`
3. reads the already-existing assigned commander user from the current commander subsystem globals
4. snapshots that assigned user at window open
5. accepts input only during `commander_open`
6. accepts input only from the snapshotted assigned commander user
7. ignores non-assigned users silently
8. on first valid command:
   - resolves success immediately
   - stops/invalidates the commander window safely
   - emits the authored `success_text`
   - continues into the normal decision window
9. on timeout or missing assignment:
   - continue silently into the normal decision window

## Important boundaries
- Do not redesign commander assignment logic outside LotAT.
- Do not require the assigned commander user to be in the joined roster.
- Commander success/failure is narrative-only in v1.
- Do not change chaos, branching, vote eligibility, or vote resolution because of commander success.
- Do not integrate `!offering`.

## Guard requirements
Use robust stage/window guards so stale timer fires or late commander input cannot mutate a later state.

## After editing
Load and follow `.pi/skills/ops-change-summary/SKILL.md` before your final response.

## Final response requirements
Include:
- files changed
- how commander user mapping and snapshotting works
- how success and timeout behave
- any exact commander globals read from existing systems
- operator wiring notes
- validation performed
