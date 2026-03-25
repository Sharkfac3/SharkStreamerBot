You are acting as the `lotat-tech` role for the SharkStreamerBot project, with permission to update related agent scaffolding so the LotAT runtime spec is easy to find later.

Before doing anything else, load and follow these files in this order:
1. `WORKING.md`
2. `.agents/ENTRY.md`
3. `.agents/_shared/project.md`
4. `.agents/_shared/conventions.md`
5. `.agents/_shared/coordination.md`
6. `.agents/roles/lotat-tech/role.md`
7. `.agents/roles/lotat-tech/skills/core.md`
8. `.agents/roles/lotat-tech/skills/engine/_index.md`
9. `.agents/roles/streamerbot-dev/skills/lotat/_index.md`
10. `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md`
11. `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`

Task:
Update the agent scaffolding so future agents can easily navigate to the LotAT runtime session spec.

What to do:
- ensure the lotat-tech role points to the new/relevant engine spec docs in its skill load order or index files
- ensure the engine index references the runtime-session spec docs
- ensure any LotAT-adjacent streamerbot-dev guidance points implementers back to the lotat-tech runtime contract docs
- keep the boundary clear between lotat-tech runtime contract docs and any future C# implementation work

Rules:
- Do not write any C# scripts.
- Do not change story JSON or story-authoring rules beyond clarifying existing runtime boundaries.
- Prefer small, navigation-focused documentation edits.
- If you add new files, make sure parent index docs mention them.

When responding:
- list the navigation docs you updated
- explain how a future agent should now find the runtime spec
- explicitly note that this was scaffolding/navigation work only
