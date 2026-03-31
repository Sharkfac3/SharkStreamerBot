You are acting as the `streamerbot-dev` role for the SharkStreamerBot project.

Your job in this run is to perform a **LotAT implementation smoke-test and drift audit** against the repo contracts after the earlier prompts have been completed.

This run may make small corrective edits if clear contract drift or reset drift is found, but it should not become a broad refactor.

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
18. `Actions/Twitch Core Integrations/stream-start.cs`
19. all current `Actions/LotAT/*.cs` files
20. `Actions/LotAT/README.md`
21. `Humans/LotAT-Implementation-Prompts/README.md`
22. all prior prompt files in `Humans/LotAT-Implementation-Prompts/`
23. `Creative/WorldBuilding/Storylines/loaded/current-story.json`
24. `Creative/WorldBuilding/Storylines/finished/test-prototype-soda-comet.json`

## Audit goals
Check for drift across at least these categories:
1. **Story/runtime boundary drift**
   - no invented story JSON fields
   - no runtime commands treated as authored decision commands
2. **Offering boundary drift**
   - no LotAT v1 dependence on `!offering`, offering globals, or boost state
3. **State drift**
   - all implemented LotAT globals are documented in `Actions/SHARED-CONSTANTS.md`
   - all reset-sensitive LotAT globals are reset in `stream-start.cs`
4. **Session flow drift**
   - start -> join -> node -> optional commander/dice -> decision -> resolve/end remains aligned to the contract
5. **Voting drift**
   - joined roster only
   - latest valid vote wins for a participant
   - early close uses frozen joined roster
   - zero valid votes end unresolved
6. **Timer safety drift**
   - timer-end actions use safe guards against stale events
7. **Documentation drift**
   - `Actions/LotAT/README.md` reflects the current implementation honestly

## Required output
Produce:
1. **Audit summary**
2. **Pass/fail checklist by category**
3. **Small corrective edits made**, if any
4. **Remaining known gaps** that are acceptable for the current prototype
5. **Recommended next follow-up work** after the initial prototype line is complete

## Editing rule
Only make focused corrective edits if the drift is clear and the fix is small/safe. If a larger redesign would be required, report it instead of performing it.

## After editing
Load and follow `.pi/skills/ops-change-summary/SKILL.md` before your final response.

## Final response requirements
Include:
- files changed, if any
- audit findings
- drift found vs corrected
- validation performed
- recommended next tasks
