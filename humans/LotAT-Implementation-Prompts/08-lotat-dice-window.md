You are acting as the `streamerbot-dev` role for the SharkStreamerBot project.

Your job in this run is to implement the **LotAT dice window** mechanic: open pre-vote `!roll` windows, accept rolls from all chat, resolve immediately on first success, fail on timeout, and then continue into the normal decision window.

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
- `Actions/LotAT/lotat-roll.cs`
- `Actions/LotAT/lotat-dice-window-end.cs`
- update `Actions/LotAT/lotat-enter-node.cs` as needed
- update `Actions/LotAT/README.md` if needed

## Required behavior
Implement dice-window behavior that:
1. opens only when the current stage node has `dice_hook.enabled = true`
2. uses authored node values for:
   - roll window seconds
   - success threshold
   - success text
   - failure text
3. accepts `!roll` only during `dice_open`
4. accepts `!roll` from **any viewer in chat**, not just joined participants
5. generates a random roll from 1 to 100 for each valid roll
6. surfaces roll results to chat in v1
7. resolves success on the first roll where `roll >= success_threshold`
8. on success:
   - closes the dice window immediately
   - emits the authored `success_text`
   - continues into the normal decision window
9. on timeout with no successful roll:
   - emits the authored `failure_text`
   - continues into the normal decision window

## Important boundaries
- Do not gate dice participation by joined roster.
- Dice outcomes are narrative-only in v1.
- Do not change chaos, branching, vote eligibility, or vote resolution because of dice success/failure.
- Do not integrate `!offering`.
- Preserve the rule that commander windows and dice windows do not coexist on the same node in v1.

## Guard requirements
Use stage/window guards so stale timer events and late rolls no-op safely.

## After editing
Load and follow `.pi/skills/ops-change-summary/SKILL.md` before your final response.

## Final response requirements
Include:
- files changed
- how `!roll` is gated and resolved
- success/failure flow
- any timer interval assumptions
- operator wiring notes
- validation performed
