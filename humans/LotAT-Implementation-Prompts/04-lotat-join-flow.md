You are acting as the `streamerbot-dev` role for the SharkStreamerBot project.

Your job in this run is to implement the **LotAT join flow**: `!join` handling during `join_open`, roster dedupe/storage, and the join-window timer-end path that either ends on zero joins or enters the starting node.

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
18. `Actions/LotAT/lotat-start.cs`
19. `Humans/LotAT-Implementation-Prompts/README.md`
20. `Humans/LotAT-Implementation-Prompts/01-lotat-action-matrix-and-wiring-plan.md`
21. `Humans/LotAT-Implementation-Prompts/03-lotat-session-bootstrap-and-story-load.md`

## Required target files
At minimum, create/update:
- `Actions/LotAT/lotat-join.cs`
- `Actions/LotAT/lotat-join-window-end.cs`
- update `Actions/LotAT/README.md` if needed

## Required behavior
### `lotat-join.cs`
Implement `!join` handling that:
- only accepts joins during `join_open`
- uses lowercase username/login string as the canonical participant identity
- ignores duplicate joins
- updates:
  - joined roster json
  - joined count
- does not count decision commands
- does not mutate state outside active join phase
- keeps chat feedback lightweight and safe

### `lotat-join-window-end.cs`
Implement join timer-end handling that:
- validates the timer fire is still relevant using current stage/window/session guards
- when joined count is zero:
  - ends the run cleanly as a non-error zero-join outcome
  - returns runtime to idle-safe state or routes through the chosen normal end path
- when one or more users joined:
  - freezes the roster
  - resolves the story’s `starting_node_id`
  - stores current node id
  - transitions toward node entry

## Important boundaries
- Do not implement full node-entry behavior in this run unless only a tiny stub/call-through is needed.
- Do not implement decision voting yet.
- Do not integrate `!offering`.
- Do not allow late-join support after join closes.
- Keep the runtime contract deterministic and unattended-friendly.

## Guard expectations
Use stage/window guards so stale join timer events cannot incorrectly mutate a later run.

## After editing
Load and follow `.pi/skills/ops-change-summary/SKILL.md` before your final response.

## Final response requirements
Include:
- files changed
- join behavior implemented
- zero-join behavior implemented
- timer wiring/manual setup notes
- validation performed
- dependencies for the next prompt
