You are acting as the `lotat-tech` role for the SharkStreamerBot project.

Your job: perform a documentation-only review of the LotAT runtime session scaffolding to make sure it is internally consistent and does not drift into schema or implementation work.

Before doing anything else, load and follow these files in this order:
1. `WORKING.md`
2. `.agents/ENTRY.md`
3. `.agents/_shared/project.md`
4. `.agents/_shared/conventions.md`
5. `.agents/_shared/coordination.md`
6. `.agents/roles/lotat-tech/role.md`
7. `.agents/roles/lotat-tech/skills/core.md`
8. `.agents/roles/lotat-tech/skills/engine/_index.md`
9. `.agents/roles/lotat-tech/skills/engine/commands.md`
10. `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md`
11. `.agents/roles/lotat-tech/skills/engine/state-and-voting.md`
12. `.agents/roles/lotat-tech/skills/story-pipeline/json-schema.md`
13. `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`
14. `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md`

Task:
Review the runtime-session scaffolding and tighten anything unclear.

Review goals:
- confirm the runtime docs do not accidentally redefine the authored story schema
- confirm `!join` is consistently treated as a runtime/session command
- confirm the join roster and early-close rules are documented consistently across the engine docs
- confirm the docs are useful for future lotat-tech and streamerbot-dev agents
- fix any small wording drift that would confuse future agents

Rules:
- Documentation only.
- No C#.
- No story JSON edits.
- No schema expansion unless explicitly asked.
- If you discover a breaking ambiguity, call it out instead of silently changing the contract.

When responding:
- list any inconsistencies you found and fixed
- call out any unresolved ambiguity that still needs operator input
- explicitly state whether the review changed schema, story content, or implementation code
