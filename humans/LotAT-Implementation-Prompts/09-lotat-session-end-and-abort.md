You are acting as the `streamerbot-dev` role for the SharkStreamerBot project.

Your job in this run is to implement the **LotAT shared session-end and abort cleanup paths** so all terminal outcomes can return the engine safely to idle.

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
16. `Actions/LotAT/lotat-start.cs`
17. `Actions/LotAT/lotat-join-window-end.cs`
18. `Actions/LotAT/lotat-enter-node.cs`
19. `Actions/LotAT/lotat-decision-window-end.cs`
20. `Actions/LotAT/lotat-resolve-decision.cs`
21. `Actions/LotAT/lotat-commander-window-end.cs`
22. `Actions/LotAT/lotat-dice-window-end.cs`
23. `Humans/LotAT-Implementation-Prompts/README.md`
24. `Humans/LotAT-Implementation-Prompts/01-lotat-action-matrix-and-wiring-plan.md`

## Required target files
At minimum, create/update:
- `Actions/LotAT/lotat-end-session.cs`
- `Actions/LotAT/lotat-abort.cs`
- update existing LotAT actions to route terminal paths through them where practical
- update `Actions/LotAT/README.md` if needed

## Required behavior
### `lotat-end-session.cs`
Create a shared normal teardown path for:
- normal ending reached
- zero-join session end
- unresolved zero-vote session end

It should:
- set stage to `ended` appropriately
- disable all four LotAT timers
- emit simple chat-safe terminal output appropriate to the reason
- clear/reset LotAT session/node/vote globals
- return runtime to `idle`

### `lotat-abort.cs`
Create a shared fail-closed abort path for unrecoverable runtime/code faults.
For this pass, keep it simple and practical:
- generic chat-safe failure message
- verbose logs for debugging
- disable all LotAT timers
- clear/reset LotAT runtime state
- return to `idle`

## Important boundaries
- Keep fault UX simple; polished abort UX can come later.
- Do not integrate `!offering`.
- Do not disturb unrelated systems.
- Preserve unattended fail-closed behavior from the contract.

## Practical note
If centralizing every terminal path at once would be risky, do the safest practical partial centralization and clearly document any remaining direct terminal paths.

## After editing
Load and follow `.pi/skills/ops-change-summary/SKILL.md` before your final response.

## Final response requirements
Include:
- files changed
- terminal paths now covered by shared cleanup
- what state/timers are cleared
- any remaining non-centralized terminal paths
- validation performed
