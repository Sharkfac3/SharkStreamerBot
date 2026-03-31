You are acting as the `streamerbot-dev` role for the SharkStreamerBot project.

Your job in this run is to document the current LotAT implementation clearly for the operator and for future agents.

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
10. `.agents/roles/lotat-tech/skills/engine/docs-map.md`
11. `.agents/roles/lotat-tech/skills/engine/_index.md`
12. `.agents/roles/lotat-tech/skills/engine/commands.md`
13. `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md`
14. `.agents/roles/lotat-tech/skills/engine/state-and-voting.md`
15. all current `Actions/LotAT/*.cs` files
16. `Humans/LotAT-Implementation-Prompts/README.md`
17. `Humans/LotAT-Implementation-Prompts/01-lotat-action-matrix-and-wiring-plan.md`

## Required target files
At minimum, create/update:
- `Actions/LotAT/README.md`
- optionally a short operator note file under `Humans/LotAT-Implementation-Prompts/` if that would help, but prioritize the runtime README

## Required documentation content
Update the LotAT README so it clearly documents:
1. **Purpose**
2. **Current implementation scope**
3. **Action inventory**
4. **Expected trigger/input for each action**
5. **Required runtime globals**
6. **Timer names and what they drive**
7. **Session flow summary**
8. **Command routing summary**
9. **Operator setup / manual Streamer.bot wiring notes**
10. **Known limitations / not-yet-implemented polish**
11. **Explicit offering boundary**
   - `!offering` is out of scope for LotAT v1
   - offering globals are not LotAT engine behavior

## Important boundaries
- Keep the README aligned to the actual code currently present.
- Do not invent finished behavior that is not implemented.
- Do not blur story contract and runtime contract.
- Preserve the documented LotAT/offering boundary.

## After editing
Load and follow `.pi/skills/ops-change-summary/SKILL.md` before your final response.

## Final response requirements
Include:
- files changed
- what documentation was added/updated
- any operator-critical wiring notes highlighted
- validation performed
